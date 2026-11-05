# Dot-sourced by mirror-dependencies.ps1; discovery never writes the repository config.

function New-MavenMirrorPlan([object] $Config, [object] $Baseline, [string] $UpdaterLog, [string] $CiLog) {
    $google = 'https://dl.google.com/dl/android/maven2'
    $central = 'https://repo.maven.apache.org/maven2'
    $plan = @{
        Scope = 'Pinned bindings, PR version diff, newer stable candidates (including frozen), CI failures, and parent/BOM + compile/runtime closure; includes configured sources/Javadoc and optional Gradle module metadata for bindings, and optional dependencies without exclusions or conflict mediation.'
        Artifacts = [Collections.Generic.List[object]]::new()
        Errors = [Collections.Generic.HashSet[string]]::new()
        MissingMappings = [Collections.Generic.HashSet[string]]::new()
    }
    $artifacts = @{}
    $responses = @{}
    $poms = @{}
    $resolving = [Collections.Generic.HashSet[string]]::new()
    $pending = [Collections.Generic.Queue[object]]::new()
    $visited = [Collections.Generic.HashSet[string]]::new()
    $client = [Net.Http.HttpClient]::new()
    $client.Timeout = [TimeSpan]::FromSeconds(60)

    function Get-UpstreamText([string] $Url, [switch] $Probe) {
        if ($responses.ContainsKey($Url)) { return $responses[$Url] }
        for ($attempt = 0; $attempt -lt 4; $attempt++) {
            $response = $null
            try {
                $response = $client.GetAsync($Url, [Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()
                if ([int] $response.StatusCode -eq 404) {
                    $responses[$Url] = $null
                    return $null
                }
                if ($attempt -lt 3 -and ([int] $response.StatusCode -in 408, 429 -or [int] $response.StatusCode -ge 500)) {
                    Start-Sleep -Seconds ($attempt + 1)
                    continue
                }
                $response.EnsureSuccessStatusCode() | Out-Null
                $responses[$Url] = if ($Probe) { 'available' } else { $response.Content.ReadAsStringAsync().GetAwaiter().GetResult() }
                return $responses[$Url]
            }
            catch {
                if ($response -or $attempt -eq 3) { throw }
                Start-Sleep -Seconds ($attempt + 1)
            }
            finally { if ($response) { $response.Dispose() } }
        }
    }

    function Get-Repositories([string] $Group, [string] $Id) {
        $artifact = $Config.artifacts | Where-Object { $_.groupId -ceq $Group -and $_.artifactId -ceq $Id } | Select-Object -First 1
        $template = $Config.templateSets | Where-Object { $_.name -ceq $(if ($artifact.templateSet) { $artifact.templateSet } else { 'default' }) } | Select-Object -First 1
        $setting = @($artifact, $template, $Config) | Where-Object { $null -ne $_.mavenRepositoryType } | Select-Object -First 1
        $type = if ($setting) { $setting.mavenRepositoryType } else { 'Google' }
        $preferred = switch ($type) {
            'Google' { $google }
            'MavenCentral' { $central }
            'Url' {
                $url = [uri] $setting.mavenRepositoryLocation
                if (!$url.IsAbsoluteUri -or $url.Scheme -ne 'https') { throw "Unsupported upstream repository: $url" }
                $url.AbsoluteUri.TrimEnd('/')
            }
            default { throw "Unsupported upstream repository '$type' for ${Group}:$Id" }
        }
        @($preferred, $google, $central) | Select-Object -Unique
    }

    function Get-Entry([string] $Coordinate) {
        if (!$artifacts.ContainsKey($Coordinate)) {
            $entry = [pscustomobject]@{
                Coordinate = $Coordinate
                Reasons = [Collections.Generic.HashSet[string]]::new()
                Files = [Collections.Generic.List[object]]::new()
                Error = $null
            }
            $artifacts[$Coordinate] = $entry
            $plan.Artifacts.Add($entry)
        }
        $artifacts[$Coordinate]
    }

    function Add-Candidate([string] $Coordinate, [string] $Reason, [bool] $Payload = $true, [string] $Type = '', [string] $Classifier = '', [bool] $Binding = $false, [object] $DownloadConfig = $Config) {
        $parts = $Coordinate.Split(':')
        if ($parts.Count -ne 3 -or ($parts | Where-Object { $_ -cnotmatch '^[\w.+-]+$' -or $_.Contains('..') })) {
            throw "Invalid or unresolved Maven coordinate '$Coordinate'"
        }
        if ($Classifier -cnotmatch '^[\w.-]*$' -or $Classifier.Contains('..')) { throw "Unsupported classifier '$Classifier'" }
        (Get-Entry $Coordinate).Reasons.Add($Reason) | Out-Null
        $Binding = $Binding -or [bool] ($Config.artifacts | Where-Object { !$_.dependencyOnly -and $_.groupId -ceq $parts[0] -and $_.artifactId -ceq $parts[1] })
        if ($Payload) { $pending.Enqueue(@{ Coordinate = $Coordinate; Type = $Type; Classifier = $Classifier; Binding = $Binding -and !$Type; DownloadConfig = $DownloadConfig }) }
    }

    function Add-File([string] $Coordinate, [string] $Filename, [string] $Repository, [bool] $Optional = $false) {
        $group, $id, $version = $Coordinate.Split(':')
        $path = "$($group.Replace('.', '/'))/$id/$version/$Filename"
        $entry = Get-Entry $Coordinate
        if (!($entry.Files | Where-Object Path -CEQ $path)) {
            $upstreams = if ($Optional) { @($Repository; Get-Repositories $group $id) | Select-Object -Unique | ForEach-Object { "$_/$path" } } else { @("$Repository/$path") }
            $entry.Files.Add([pscustomobject]@{ Path = $path; Upstream = "$Repository/$path"; Upstreams = @($upstreams); Optional = $Optional; Status = 'pending' })
        }
    }

    function Add-AuxiliaryFiles([string] $Coordinate, [string] $Repository, [object] $Settings) {
        if ($Settings.downloadExternals -eq $false) { return }
        $group, $id, $version = $Coordinate.Split(':')
        if ($Settings.downloadJavaSourceJars -ne $false) { Add-File $Coordinate "$id-$version-sources.jar" $Repository $true }
        if ($Settings.downloadJavaDocJars -ne $false) { Add-File $Coordinate "$id-$version-javadoc.jar" $Repository $true }
        Add-File $Coordinate "$id-$version.module" $Repository $true
    }

    function Add-Payload([string] $Coordinate, [string] $Type, [string] $Classifier, [string] $Repository, [bool] $Binding) {
        if ($Type -ceq 'pom' -and !$Binding) { return }
        $group, $id, $version = $Coordinate.Split(':')
        # Match Binderator's JAR/AAR fallback for binding packaging such as pom or bundle.
        if ($Binding -and $Type -cnotin 'jar', 'aar') {
            foreach ($extension in 'jar', 'aar') {
                $filename = "$id-$version.$extension"
                if ($null -ne (Get-UpstreamText "$Repository/$($group.Replace('.', '/'))/$id/$version/$filename" -Probe)) {
                    Add-File $Coordinate $filename $Repository
                    return
                }
            }
            throw "Could not find binding payload for $Coordinate (packaging '$Type')"
        }
        $extension = if ($Type -cin 'bundle', 'test-jar') { 'jar' } else { $Type }
        if ($extension -cnotin 'jar', 'aar') { throw "Unsupported payload packaging '$Type'" }
        $suffix = if ($Classifier) { "-$Classifier" } elseif ($Type -ceq 'test-jar') { '-tests' } else { '' }
        Add-File $Coordinate "$id-$version$suffix.$extension" $Repository
    }

    function Merge-Dependency([hashtable] $Child, [hashtable] $Inherited) {
        $merged = $Child.Clone()
        foreach ($field in 'Version', 'Scope') {
            if (!$merged[$field] -and $Inherited) { $merged[$field] = $Inherited[$field] }
        }
        $merged
    }

    function Read-Pom([string] $Coordinate) {
        $group, $id, $version = $Coordinate.Split(':')
        foreach ($repository in Get-Repositories $group $id) {
            $text = Get-UpstreamText "$repository/$($group.Replace('.', '/'))/$id/$version/$id-$version.pom"
            if ($null -eq $text) { continue }
            $xml = [xml]::new()
            $xml.XmlResolver = $null
            $xml.LoadXml($text)
            if ($xml.DocumentElement.LocalName -cne 'project') { throw "Response is not a Maven POM for $Coordinate" }
            Add-File $Coordinate "$id-$version.pom" $repository
            return @{ Root = $xml.DocumentElement; Repository = $repository }
        }
        throw "POM unavailable upstream for $Coordinate"
    }

    function Get-Value([Xml.XmlElement] $Element, [string] $Name, [string] $Default = '') {
        $node = $Element.SelectSingleNode("*[local-name()='$Name']")
        if ($node -and $node.InnerText.Trim()) { $node.InnerText.Trim() } else { $Default }
    }

    function Expand-Properties([string] $Value, [Collections.Generic.Dictionary[string,string]] $Properties) {
        for ($i = 0; $i -lt 30 -and $Value.Contains('${'); $i++) {
            $expanded = [regex]::Replace($Value, '\$\{([^}]+)\}', {
                param($match)
                $key = $match.Groups[1].Value
                if ($Properties.ContainsKey($key)) { $Properties[$key] } else { $match.Value }
            })
            if ($expanded -ceq $Value) { break }
            $Value = $expanded
        }
        $Value
    }

    function Resolve-Version([string] $Group, [string] $Id, [string] $Version) {
        if (!$Version -or $Version.Contains('${') -or $Version.StartsWith('unspecified')) { throw "Unresolved dependency version ${Group}:${Id}:$Version" }
        if (!$Version.Contains('[') -and !$Version.Contains('(')) { return $Version }
        $ranges = @([Java.Interop.Tools.Maven.Models.MavenVersionRange]::Parse($Version))
        foreach ($repository in Get-Repositories $Group $Id) {
            $text = Get-UpstreamText "$repository/$($Group.Replace('.', '/'))/$Id/maven-metadata.xml"
            if ($null -eq $text) { continue }
            $matches = @(([xml] $text).metadata.versioning.versions.version | Where-Object {
                $candidate = [Java.Interop.Tools.Maven.Models.MavenVersion]::Parse($_)
                @($ranges | Where-Object { $_.ContainsVersion($candidate) }).Count -gt 0
            } | Sort-Object { [Java.Interop.Tools.Maven.Models.MavenVersion]::Parse($_) } -Descending)
            if ($matches.Count) { return [string] $matches[0] }
        }
        throw "No upstream version satisfies ${Group}:${Id}:$Version"
    }

    function Resolve-Pom([string] $Coordinate) {
        if ($poms.ContainsKey($Coordinate)) { return $poms[$Coordinate] }
        if (!$resolving.Add($Coordinate)) { throw "Cyclic parent/BOM reference at $Coordinate" }
        try {
            $raw = Read-Pom $Coordinate
            $root = $raw.Root
            $properties = [Collections.Generic.Dictionary[string,string]]::new([StringComparer]::Ordinal)
            $managed = @{}
            $dependencies = @{}
            $parent = $root.SelectSingleNode("*[local-name()='parent']")
            if ($parent) {
                $parentCoordinate = "$(Get-Value $parent groupId):$(Get-Value $parent artifactId):$(Get-Value $parent version)"
                Add-Candidate $parentCoordinate "parent of $Coordinate" $false
                $inherited = Resolve-Pom $parentCoordinate
                foreach ($item in $inherited.Properties.GetEnumerator()) { $properties[$item.Key] = $item.Value }
                $managed = $inherited.Managed.Clone()
                $dependencies = $inherited.Dependencies.Clone()
            }
            foreach ($property in $root.SelectNodes("*[local-name()='properties']/*")) { $properties[$property.LocalName] = $property.InnerText.Trim() }
            $group, $id, $version = $Coordinate.Split(':')
            foreach ($prefix in 'project', 'pom') {
                $properties["$prefix.groupId"] = $group
                $properties["$prefix.artifactId"] = $id
                $properties["$prefix.version"] = $version
                if ($parent) {
                    $properties["$prefix.parent.groupId"] = Get-Value $parent groupId
                    $properties["$prefix.parent.version"] = Get-Value $parent version
                }
            }
            function Read-Dependency([Xml.XmlElement] $Element) {
                $dep = @{}
                foreach ($field in 'groupId', 'artifactId', 'type', 'classifier') { $dep[$field] = Expand-Properties (Get-Value $Element $field) $properties }
                $dep.Version = Get-Value $Element version
                $dep.Scope = Get-Value $Element scope
                $type = if ($dep.type) { $dep.type } else { 'jar' }
                $dep.Key = "$($dep.groupId):$($dep.artifactId):${type}:$($dep.classifier)"
                $dep
            }
            $management = @($root.SelectNodes("*[local-name()='dependencyManagement']/*[local-name()='dependencies']/*[local-name()='dependency']") | ForEach-Object { Read-Dependency $_ })
            $imports = @{}
            foreach ($dep in $management) {
                if ((Expand-Properties $dep.Scope $properties) -cne 'import') { continue }
                if ($dep.type -cne 'pom') { throw "Unsupported BOM import type '$($dep.type)'" }
                $bomVersion = Resolve-Version $dep.groupId $dep.artifactId (Expand-Properties $dep.Version $properties)
                $bom = "$($dep.groupId):$($dep.artifactId):$bomVersion"
                Add-Candidate $bom "BOM imported by $Coordinate" $false
                $imported = Resolve-Pom $bom
                foreach ($item in $imported.Managed.GetEnumerator()) {
                    if ($imports.ContainsKey($item.Key)) { continue }
                    $copy = $item.Value.Clone()
                    $copy.Version = Expand-Properties $copy.Version $imported.Properties
                    $copy.Scope = Expand-Properties $copy.Scope $imported.Properties
                    $imports[$item.Key] = $copy
                }
            }
            foreach ($dep in $management) {
                if ((Expand-Properties $dep.Scope $properties) -cne 'import') {
                    $managed[$dep.Key] = Merge-Dependency $dep $managed[$dep.Key]
                }
            }
            foreach ($item in $imports.GetEnumerator()) {
                if (!$managed.ContainsKey($item.Key)) { $managed[$item.Key] = $item.Value }
            }
            foreach ($element in $root.SelectNodes("*[local-name()='dependencies']/*[local-name()='dependency']")) {
                $dep = Read-Dependency $element
                $dependencies[$dep.Key] = Merge-Dependency $dep $dependencies[$dep.Key]
            }
            if ($root.SelectNodes("*[local-name()='profiles']/*[local-name()='profile']/*[local-name()='properties']/*").Count) {
                throw 'POM contains profile-dependent properties; profile evaluation is not supported'
            }
            foreach ($dep in $root.SelectNodes("*[local-name()='profiles']/*[local-name()='profile']//*[local-name()='dependencies']/*[local-name()='dependency']")) {
                if ((Get-Value $dep scope) -cnotin 'test', 'provided', 'system') { throw 'POM contains profile-dependent dependencies; profile evaluation is not supported' }
            }
            $relocation = $root.SelectSingleNode("*[local-name()='distributionManagement']/*[local-name()='relocation']")
            if ($relocation) {
                $target = @(
                    (Expand-Properties (Get-Value $relocation groupId $group) $properties),
                    (Expand-Properties (Get-Value $relocation artifactId $id) $properties),
                    (Expand-Properties (Get-Value $relocation version $version) $properties)
                ) -join ':'
                Add-Candidate $target "relocation of $Coordinate"
            }
            $poms[$Coordinate] = @{
                Properties = $properties; Managed = $managed; Dependencies = $dependencies
                Packaging = Expand-Properties (Get-Value $root packaging 'jar') $properties
                Repository = $raw.Repository
                Relocated = $null -ne $relocation
            }
            $poms[$Coordinate]
        }
        finally { $resolving.Remove($Coordinate) | Out-Null }
    }

    function ConvertTo-UpdaterVersion([string] $Version) {
        if ($Version -cnotmatch '^(\d+)(?:\.(\d+))?(?:\.(\d+))?(-[^+]+)?(?:\+.*)?$') { return $null }
        $tag = [string] $Matches[4]
        if ($tag.Contains('_')) { $tag = '' }
        "$([long] $Matches[1]).$([long] $Matches[2]).$([long] $Matches[3])$tag"
    }

    try {
        $updaterRows = @{}
        $packageTable = [regex]::Split($UpdaterLog, '(?m)^\|\s*\d+ Dependencies')[0]
        foreach ($match in [regex]::Matches($packageTable, '(?m)^\|\s*[#* ]*([A-Za-z_][\w.-]+)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|')) {
            $key = $match.Groups[1].Value
            if ($updaterRows.ContainsKey($key)) { throw "Ambiguous updater row '$key'" }
            $updaterRows[$key] = $match.Groups[3].Value.Trim()
        }
        foreach ($artifact in $Config.artifacts) {
            $coordinate = "$($artifact.groupId):$($artifact.artifactId):$($artifact.version)"
            if (!($Baseline.artifacts | Where-Object { $_.groupId -ceq $artifact.groupId -and $_.artifactId -ceq $artifact.artifactId -and $_.version -ceq $artifact.version })) {
                Add-Candidate $coordinate 'PR diff head version'
            }
            if ($artifact.dependencyOnly) { continue }
            Add-Candidate $coordinate 'configured binding'
            try {
                $key = "$($artifact.groupId).$($artifact.artifactId)"
                if (!$updaterRows.ContainsKey($key)) { throw "Missing updater row for $key" }
                $latest = $updaterRows[$key]
                $versions = @()
                foreach ($repository in Get-Repositories $artifact.groupId $artifact.artifactId) {
                    $text = Get-UpstreamText "$repository/$($artifact.groupId.Replace('.', '/'))/$($artifact.artifactId)/maven-metadata.xml"
                    if ($null -eq $text) { continue }
                    $versions = @(([xml] $text).metadata.versioning.versions.version)
                    break
                }
                if (!$versions.Count) { throw "No upstream metadata versions for $key" }
                if ($latest -ceq $artifact.version) { continue }
                $raw = @($versions | Where-Object { (ConvertTo-UpdaterVersion $_) -ceq $latest })
                if (!$raw.Count) { throw "Cannot map updater version '$latest' to upstream metadata for $key" }
                $reason = if ($artifact.frozen) { 'latest stable (held/frozen)' } else { 'latest stable' }
                Add-Candidate "$($artifact.groupId):$($artifact.artifactId):$($raw[0])" $reason
            }
            catch { $plan.Errors.Add("Latest stable discovery incomplete: $($_.Exception.Message)") | Out-Null }
        }
        foreach ($artifact in $Baseline.artifacts) {
            if (!($Config.artifacts | Where-Object { $_.groupId -ceq $artifact.groupId -and $_.artifactId -ceq $artifact.artifactId -and $_.version -ceq $artifact.version })) {
                Add-Candidate "$($artifact.groupId):$($artifact.artifactId):$($artifact.version)" 'PR diff base version' $true '' '' (!$artifact.dependencyOnly) $Baseline
            }
        }
        $diagnostics = [regex]::Matches($CiLog, '(?im)^.*(?:No matching artifact config found|to satisfy dependency of|No POM found|Could not|Unable to|\berror\b|\bfailed\b|\b40[134]\b|\b50[0234]\b).*(?:\r?\n[^\r\n]*){0,2}')
        foreach ($diagnostic in $diagnostics) {
            $coordinates = [regex]::Matches($diagnostic.Value, '(?<![\w:/])([A-Za-z_][\w.-]*):([\w.-]+):([\[(][0-9A-Za-z.+,\[\]()_-]*[\])]|[0-9](?:[\w.+-]*[\w+-])?)')
            foreach ($match in $coordinates) {
                $group, $id, $requirement = $match.Groups[1..3].Value
                try {
                    $version = Resolve-Version $group $id $requirement
                    Add-Candidate "${group}:${id}:$version" 'CI failure'
                }
                catch { $plan.Errors.Add("CI dependency resolution incomplete: $($_.Exception.Message)") | Out-Null }
            }
            foreach ($match in [regex]::Matches($diagnostic.Value, 'https://[^\s''"<>]+/(?:maven/v1|maven2)/([^\s''"<>?]+)')) {
                $parts = $match.Groups[1].Value.TrimEnd(')', '.', ',').Split('/')
                if ($parts.Count -ge 4) {
                    Add-Candidate "$(($parts[0..($parts.Count - 4)]) -join '.'):$($parts[-3]):$($parts[-2])" 'CI failure'
                }
            }
            if ($diagnostic.Value.Contains('No matching artifact config found') -and !$coordinates.Count) {
                $plan.Errors.Add("Unparsed CI dependency failure: $($diagnostic.Value.Trim())") | Out-Null
            }
        }
        while ($pending.Count) {
            $next = $pending.Dequeue()
            $coordinate = $next.Coordinate
            $settings = $next.DownloadConfig
            if (!$visited.Add("${coordinate}:$($next.Type):$($next.Classifier):$($next.Binding):$($settings.downloadExternals):$($settings.downloadJavaSourceJars):$($settings.downloadJavaDocJars)")) { continue }
            $entry = Get-Entry $coordinate
            $group, $id, $version = $coordinate.Split(':')
            if ($entry.Reasons.Contains('CI failure') -and !($Config.artifacts | Where-Object { $_.groupId -ceq $group -and $_.artifactId -ceq $id -and $_.version -ceq $version })) {
                $plan.MissingMappings.Add("$coordinate (named in CI; no exact config mapping)") | Out-Null
            }
            try {
                $raw = Read-Pom $coordinate
                if ($next.Binding) { Add-AuxiliaryFiles $coordinate $raw.Repository $settings }
                $packaging = if ($next.Type) { $next.Type } else { Get-Value $raw.Root packaging 'jar' }
                $relocated = $null -ne $raw.Root.SelectSingleNode("*[local-name()='distributionManagement']/*[local-name()='relocation']")
                if (!$relocated -and !$packaging.Contains('${')) { Add-Payload $coordinate $packaging $next.Classifier $raw.Repository $next.Binding }
                $pom = Resolve-Pom $coordinate
                $packaging = if ($next.Type) { $next.Type } else { $pom.Packaging }
                if (!$pom.Relocated) { Add-Payload $coordinate $packaging $next.Classifier $pom.Repository $next.Binding }
                foreach ($dep in $pom.Dependencies.Values) {
                    try {
                        $defaults = $pom.Managed[$dep.Key]
                        $scope = Expand-Properties $(if ($dep.Scope) { $dep.Scope } else { $defaults.Scope }) $pom.Properties
                        if ($scope -cnotin '', 'compile', 'runtime', 'test', 'provided', 'system') { throw "Unsupported dependency scope '$scope'" }
                        if ($scope -cnotin '', 'compile', 'runtime') { continue }
                        $requirement = Expand-Properties $(if ($dep.Version) { $dep.Version } else { $defaults.Version }) $pom.Properties
                        $resolved = Resolve-Version $dep.groupId $dep.artifactId $requirement
                        Add-Candidate "$($dep.groupId):$($dep.artifactId):$resolved" "dependency of $coordinate" $true $dep.type $dep.classifier
                        $ranges = @([Java.Interop.Tools.Maven.Models.MavenVersionRange]::Parse($requirement))
                        $mapping = $Config.artifacts | Where-Object {
                            if ($_.groupId -cne $dep.groupId -or $_.artifactId -cne $dep.artifactId) { return $false }
                            $mappedVersion = [Java.Interop.Tools.Maven.Models.MavenVersion]::Parse($_.version)
                            @($ranges | Where-Object { $_.ContainsVersion($mappedVersion) }).Count -gt 0
                        } | Select-Object -First 1
                        if (!$mapping) { $plan.MissingMappings.Add("$($dep.groupId):$($dep.artifactId):$requirement (required by $coordinate)") | Out-Null }
                    }
                    catch {
                        $entry.Error = $_.Exception.Message
                        $plan.Errors.Add("${coordinate}: $($_.Exception.Message)") | Out-Null
                    }
                }
            }
            catch {
                $entry.Error = $_.Exception.Message
                $plan.Errors.Add("${coordinate}: $($_.Exception.Message)") | Out-Null
            }
        }
        $plan
    }
    finally { $client.Dispose() }
}
