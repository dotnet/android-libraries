#tool nuget:?package=Cake.CoreCLR
/*
     dotnet cake spell-check.cake
    dotnet cake spell-check.cake -t=spell-check
 */
#addin nuget:?package=WeCantSpell.Hunspell&version=3.0.1
#addin nuget:?package=Newtonsoft.Json&version=12.0.3
#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin nuget:?package=Mono.Cecil&version=0.11.4

#addin nuget:?package=HolisticWare.Xamarin.Tools.ComponentGovernance&version=0.0.1.1
#addin nuget:?package=HolisticWare.Core.Net.HTTP&version=0.0.1
#addin nuget:?package=HolisticWare.Core.IO&version=0.0.1

using System.Collections.Generic;

using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HolisticWare.Xamarin.Tools.ComponentGovernance;

var TARGET = Argument ("t", Argument ("target", "Default"));

string file_spell_errors = "./output/spell-errors.txt";
List<string> spell_errors = null;
JArray binderator_json_array = null;

List<(string, string, string, string)> mappings_artifact_nuget = new List<(string, string, string, string)>();
Dictionary<string, string> Licenses = new Dictionary<string, string>();

// modifying default method for licenses
Manifest.Defaults.VersionBasedOnFullyQualifiedArtifactIdDelegate = delegate(string fully_qualified_artifact_id)
{
    if
        (
            fully_qualified_artifact_id.StartsWith("androidx")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android.material")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.firebase")
            ||
            fully_qualified_artifact_id.StartsWith("org.jetbrains.kotlin")
            ||
            fully_qualified_artifact_id.StartsWith("org.jetbrains.kotlinx")
            ||
            fully_qualified_artifact_id.StartsWith("org.jetbrains")
            ||
            fully_qualified_artifact_id.StartsWith("com.squareup")
            ||
            fully_qualified_artifact_id.StartsWith("io.grpc")
            ||
            fully_qualified_artifact_id.StartsWith("io.reactivex.rxjava3")
            ||
            fully_qualified_artifact_id.StartsWith("io.reactivex.rxjava2")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.j2objc")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.guava")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.auto.value")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.code.gson")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.crypto.tink")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android:annotations")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android.datatransport")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.code.findbugs")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.dagger")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.errorprone")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.zxing")
            ||
            fully_qualified_artifact_id.StartsWith("io.opencensus")
            ||
            fully_qualified_artifact_id.StartsWith("io.perfmark")
            ||
            fully_qualified_artifact_id.StartsWith("javax.inject")
            ||
            fully_qualified_artifact_id.StartsWith("org.tensorflow")
            ||
            fully_qualified_artifact_id.StartsWith("com.android.volley")
        )
    {
        const string l = "The Apache Software License, Version 2.0";
        const string u = "https://www.apache.org/licenses/LICENSE-2.0.txt";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("org.checkerframework")
            ||
            fully_qualified_artifact_id.StartsWith("org.codehaus.mojo")            
        )
    {
        const string l = "MIT";
        const string u = "http://opensource.org/licenses/MIT";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("com.github.bumptech.glide")
        )
    {
        const string l = "The Apache Software License, Version 2.0 AND Simplified BSD License";
        const string u = "https://www.apache.org/licenses/LICENSE-2.0.txt;http://www.opensource.org/licenses/bsd-license";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("org.reactivestreams")
        )
    {
        const string l = "MIT-0";
        const string u = "https://spdx.org/licenses/MIT-0.html";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("com.google.android.gms")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android.odml")
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android.ump")
        )
    {
        const string l = "Android Software Development Kit License";
        const string u = "https://developer.android.com/studio/terms";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("org.chromium.net")
        )
    {
        const string l = "Chromium and built-in dependencies";
        const string u = "https://storage.cloud.google.com/chromium-cronet/android/72.0.3626.96/Release/cronet/LICENSE";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("com.google.mlkit")
        )
    {
        const string l = "ML Kit Terms of Service";
        const string u = "https://developers.google.com/ml-kit/terms";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("com.google.android.play")
        )
    {
        const string l = "Play Core Software Development Kit Terms of Service";
        const string u = "https://developer.android.com/guide/playcore#license";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }

    if
        (
            fully_qualified_artifact_id.StartsWith("com.google.android.libraries.places")
        )
    {
        const string l = "Google Maps Platform Terms of Service";
        const string u = "https://cloud.google.com/maps-platform/terms/";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }
    
    if
        (
            fully_qualified_artifact_id.StartsWith("com.google.protobuf")
        )
    {
        const string l = "BSD 2 Clause AND BSD 3 Clause";
        const string u = "https://opensource.org/licenses/BSD-2-Clause;https://opensource.org/licenses/BSD-3-Clause;";

        if (!Licenses.ContainsKey(l))
        {
            Licenses.Add(l, u);
        }

        return l;
    }


    return null;
};

Task ("generate-component-governance")
    .IsDependentOn("mappings-artifact-nuget")
    .Does
    (
        () =>
        {
            Manifest manifest = new Manifest();

            manifest.MappingMavenArtifact2NuGetPackage = mappings_artifact_nuget;

            Console.WriteLine($"Saving ComponetGovernanceManifest cgmanifest.json...");
            manifest.Save("./cgmanifest.json");

            System.IO.File.WriteAllText
                                (
                                    "Licenses.json",
                                    Newtonsoft.Json.JsonConvert.SerializeObject
                                                                            ( 
                                                                                Licenses,
                                                                                Formatting.Indented
                                                                            )
                                );
            return;
        }
    );

Task ("mappings-artifact-nuget")
    .Does
    (
        () =>
        {
            using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
            {
                JsonTextReader jtr = new JsonTextReader(reader);
                binderator_json_array = (JArray)JToken.ReadFrom(jtr);
            }

            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                bool? dependency_only = (bool?) jo["dependencyOnly"];
                if ( dependency_only == true)
                {
                    continue;
                }

                string group_id  	= (string) jo["groupId"];
                string artifact_id  = (string) jo["artifactId"];
                string artifact_v   = (string) jo["version"];
                string nuget_id  	= (string) jo["nugetId"];
                string nuget_v  	= (string) jo["nugetVersion"];

                mappings_artifact_nuget.Add
                (
                    (
                        $"{group_id}:{artifact_id}",
                        $"{artifact_v}",
                        $"{nuget_id}",
                        $"{nuget_v}"
                    )
                );
            }

            // dump for C# tuple initialization
            string dump = string.Join($",{Environment.NewLine}", mappings_artifact_nuget);
            dump = dump.Replace("(","(\"");
            dump = dump.Replace(")","\")");
            dump = dump.Replace(", ","\", \"");
            EnsureDirectoryExists("./output/");
			System.IO.File.WriteAllText($"./output/mappings-artifact-nuget.md", dump);

            return;
        }
    );
    
Task ("list-artifacts")
    .Does
    (
        () =>
        {
            using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
            {
                JsonTextReader jtr = new JsonTextReader(reader);
                binderator_json_array = (JArray)JToken.ReadFrom(jtr);
            }

            Information("config.json list supported artifacts...");

            List<string> lines1 = new List<string>();
            List<string> lines2 = new List<string>();
            string space = " ";
            string dash = "-";
            int width1 = 70;
            int width2 = 20;

            lines1.Add($"# Artifacts supported");
            lines2.Add($"# Artifacts with versions supported");
            lines1.Add(Environment.NewLine);
            lines1.Add(Environment.NewLine);
            lines2.Add(Environment.NewLine);
            lines2.Add(Environment.NewLine);
            // | Maven Fully Qualified Artifact                                       | NuGet Package                                                        |
            lines1.Add($@"|{space.PadRight(width1)}|{space.PadRight(width1)}|");
            lines1.Add($@"|{dash.PadRight(width1, '-')}|{dash.PadRight(width1, '-')}|");
            lines2.Add($@"|{space.PadRight(width1)}|{space.PadRight(width2)}|{space.PadRight(width1)}|{space.PadRight(width2)}|");
            lines2.Add($@"|{dash.PadRight(width1, '-')}|{dash.PadRight(width2, '-')}|{dash.PadRight(width1, '-')}|{dash.PadRight(width2, '-')}|");

            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                bool? dependency_only = (bool?) jo["dependencyOnly"];
                if ( dependency_only == true)
                {
                    continue;
                }

                string group_id  	= (string) jo["groupId"];
                string artifact_id  = (string) jo["artifactId"];
                string artifact_v   = (string) jo["version"];
                string nuget_id  	= (string) jo["nugetId"];
                string nuget_v  	= (string) jo["nugetVersion"];

                string maven = $"{group_id}:{artifact_id}";
                string nuget = $"{nuget_id}";
                string line1 = $@"|{maven.PadRight(width1)}|{nuget.PadRight(width1)}|";
                string line2 = $@"|{maven.PadRight(width1)}|{artifact_v.PadRight(width2)}|{nuget.PadRight(width1)}|{nuget_v.PadRight(width2)}|";

                lines1.Add(line1);
                lines2.Add(line2);
            }

            EnsureDirectoryExists("./output/");
			System.IO.File.WriteAllLines($"./output/artifact-list.md", lines1.ToArray());
			System.IO.File.WriteAllLines($"./output/artifact-list-with-versions.md", lines2.ToArray());
			System.IO.File.WriteAllLines($"./output/artifact-list-{DateTime.Now.ToString("yyyyMMdd")}.md", lines1.ToArray());
			System.IO.File.WriteAllLines($"./output/artifact-list-with-versions-{DateTime.Now.ToString("yyyyMMdd")}.md", lines2.ToArray());

        }
    );

Task ("spell-check")
    .Does
    (
        () =>
        {
            if (FileExists(file_spell_errors))
            {
                DeleteFile(file_spell_errors);
            }
            EnsureDirectoryExists("./externals/");
            string url = "https://raw.githubusercontent.com/titoBouzout/Dictionaries/master/";

            string[] files_dictionaries = new[]
            {
                "English (American).dic",
                "English (American).txt",
                "English (American).aff",
            };
            foreach(string file_dictionary in files_dictionaries)
            {
                string url_full = url + System.Uri.EscapeDataString(file_dictionary);
                Information($"Downloading ");
                Information($"      {url_full}");
                if (!FileExists($"./externals/{file_dictionary}"))
                {
                    DownloadFile(url_full, $"./externals/{file_dictionary}");
                }
            }

            var dictionary = WeCantSpell.Hunspell.WordList.CreateFromFiles(@"externals/English (American).dic");
            var words = new[]
            {
                "Xamarin",
                "AndroidX",
                "IdentifierCommon",
                "IdentifierProvider",
                "AppCompat",
                "AppCompatResources",
                "Runtime",
                "AsyncLayoutInflater",
                "AutoFill",
                "Biometric",
                "Camera2",
                "Lifecycle",
                "CardView",
                "ConstraintLayout",
                "CoordinatorLayout",
                "ContentPager",
                "CursorAdapter",
                "CustomView",
                "DataBinding",
                "DataBindingAdapters",
                "DataBindingCommon",
                "DataBindingRuntime",
                "ViewBinding",
                "DocumentFile",
                "DrawerLayout",
                "DynamicAnimation",
                "Emoji",
                "ExifInterface",
                "GridLayout",
                "HeifWriter",
                "Interpolator",
                "Leanback",
                "V14",
                "UI",
                "Utils",
                "V13",
                "V4",
                "LiveData",
                "ViewModel",
                "ViewModelSavedState",
                "LocalBroadcastManager",
                "Media2",
                "MediaRouter",
                "MultiDex",
                "Runtime",
                "PercentLayout",
                "RecyclerView",
                "SavedState",
                "SlidingPaneLayout",
                "Sqlite",
                "SwipeRefreshLayout",
                "TvProvider",
                "VectorDrawable",
                "VersionedParcelable",
                "ViewPager",
                "ViewPager2",
                "WebKit",
                "WindowExtensions",
                "SecurityCrypto",
                "Java8",
                "ReactiveStreams",
                "Ktx",
                "RxJava2",
                "RxJava3",
                "GoogleShortcuts",
                "WindowJava",
                "Startup",
                "StartupRuntime",
                "MaterialIcons",
                "Saveable",
                "Util",
                "ProfileInstaller",
                "Kotlin",
                "StdLib",
                "Jdk7",
                "Jdk8",
                "Jetbrains",
                "KotlinX",
                "Coroutines",
                "Jvm",
                "GoogleGson",
                "Rx2",
                "AutoValue",
                "ReactiveX",
                "RxJava",
                "Crypto",
                "Tink",
                "PhoneInteractions",
                "RemoteInteractions",
                "Emoji2",
                "ViewsHelper",
                "ResourceInspection",
                "SplashScreen",
                "FailureAccess",
                "ListenableFuture",
		        "J2Objc",
		        "CheckerFramework",
		        "CheckerQual",
           };

            var dictionary_custom = WeCantSpell.Hunspell.WordList.CreateFromWords(words);

            using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
            {
                JsonTextReader jtr = new JsonTextReader(reader);
                binderator_json_array = (JArray)JToken.ReadFrom(jtr);
            }

            spell_errors = new List<string>();
            Information("config.json spell checking...");

            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                bool? dependency_only = (bool?) jo["dependencyOnly"];
                if ( dependency_only == true)
                {
                    continue;
                }
                string nuget_id  	= (string) jo["nugetId"];
                Information($"       spell-check {nuget_id}");

                string[] nuget_id_parts = nuget_id.Split('.');
                foreach(string nuget_id_part in nuget_id_parts)
                {
                    bool check_dictionary_custom = dictionary_custom.Check(nuget_id_part);
                    Information($"      check_dictionary_custom {nuget_id_part} = {check_dictionary_custom}");
                    if (check_dictionary_custom)
                    {
                        Information($"          Found in custom dictionary!");
                        continue;
                    }
                    bool check_dictionary = dictionary.Check(nuget_id_part);
                    Information($"      check_dictionary {nuget_id_part} = {check_dictionary}");
                    if (check_dictionary)
                    {
                        Information($"          Found in EN-US dictionary!");
                        continue;
                    }

                    Information($"Added {nuget_id_part} from {nuget_id}");
                    spell_errors.Add(nuget_id_part);
                }
            }

            if (spell_errors.Count > 0)
            {
                System.IO.File.WriteAllLines(file_spell_errors, spell_errors);
            }
        }
    );

Task ("namespace-check")
    .Does
    (
        () =>
        {
            // Namespace Checks
            FilePath[] files = new FilePath[]{};
            FilePath[] files_androidx = GetFiles("./generated/**/Androidx.*.cs").ToArray();
            FilePath[] files_com = GetFiles("./generated/**/Com.*.cs").ToArray();
            FilePath[] files_org = GetFiles("./generated/**/Org.*.cs").ToArray();
            FilePath[] files_io_1 = GetFiles("./generated/**/Io.*.cs").ToArray();
            FilePath[] files_io_2 = GetFiles("./generated/**/IO.*.cs").ToArray();
            FilePath[] files_net = GetFiles("./generated/**/Net.*.cs").ToArray();
            FilePath[] files_kotlin = GetFiles("./generated/**/Org.Jetbrains.Kotlin*.cs").ToArray();
            FilePath[] files_kotlinx = GetFiles("./generated/**/Org.Jetbrains.Kotlinx*.cs").ToArray();

            files = files.Concat(files_androidx.ToArray()).ToArray();
            files = files.Concat(files_com.ToArray()).ToArray();
            files = files.Concat(files_org.ToArray()).ToArray();
            files = files.Concat(files_net.ToArray()).ToArray();
            files = files.Concat(files_io_1.ToArray()).ToArray();
            files = files.Concat(files_io_2.ToArray()).ToArray();
            files = files.Concat(files_kotlin.ToArray()).ToArray();
            files = files.Concat(files_kotlinx.ToArray()).ToArray();

            if (files.Any())
            {
                string[] namespaces = Array.ConvertAll(files, x => x.ToString());
                System.IO.File.WriteAllLines("./output/namespaces.md", namespaces);

                StringBuilder msg = new StringBuilder("Namespaces!!!");
                msg.AppendLine();
                msg.AppendLine(string.Join(System.Environment.NewLine, namespaces));

                throw new Exception(msg.ToString());
            }

            return;
        }
    );

Task("binderate-diff")
    .Does
    (
        () =>
        {
			EnsureDirectoryExists("./output/");

			// "git diff -U999999 main:config.json config.json" > ./output/config.json.diff-from-main.txt"
			string process = "git";
			string process_args = "diff -U999999 main:config.json config.json";
			IEnumerable<string> redirectedStandardOutput;
			ProcessSettings process_settings = new ProcessSettings ()
			{
             Arguments = process_args,
             RedirectStandardOutput = true
         	};
			int exitCodeWithoutArguments = StartProcess(process, process_settings, out redirectedStandardOutput);
			System.IO.File.WriteAllLines("./output/config.json.diff-from-main.txt", redirectedStandardOutput.ToArray());
			Information("Exit code: {0}", exitCodeWithoutArguments);
		}
	);

void RunProcess(FilePath file, string args)
{
    int exit_code = StartProcess(file, args);
    if (exit_code != 0)
    {
        throw new Exception ($"Process {file} exited with code {exit_code}.");
    }

    return;
}

System.Xml.XmlDocument xmldoc = null;
System.Xml.XmlNamespaceManager nsmgr = null;

Task ("target-sdk-version-check")
    .Does
    (
        () =>
        {
            FilePath        config_file = MakeAbsolute(new FilePath("./config.json")).FullPath;
            DirectoryPath   base_path = MakeAbsolute(new DirectoryPath ("./")).FullPath;

            // Run the dotnet tool for binderator
            RunProcess
            (
                "xamarin-android-binderator",
                $"--config=\"{config_file}\" --basepath=\"{base_path}\""
            );

            Dictionary<(string group, string artifact), int> artifacts_target_sdk = null;
            Dictionary<(string group, string artifact), string> artifacts_versions = null;
            Parallel.Invoke
            (
                () =>
                {
                    FilePath[] files_android_manifests = GetFiles("./externals/**/AndroidManifest.xml").ToArray();
                    foreach(FilePath fp in files_android_manifests)
                    {
                        Information($"files_android_manifest = {fp}");
                    }

                    artifacts_target_sdk = new Dictionary<(string group, string artifact), int>();

                    foreach(FilePath fp in files_android_manifests)
                    {
                        Information($"      AndroidManifest = {fp}");
                        xmldoc = new System.Xml.XmlDocument();
                        xmldoc.Load(fp.ToString());

                        nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
                        nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");

                        string t = xmldoc.SelectSingleNode($@"/manifest/uses-sdk/@android:targetSdkVersion", nsmgr)?.Value;
                        string mc = xmldoc.SelectSingleNode($@"/manifest/uses-sdk/@android:minCompileSdk", nsmgr)?.Value;

                        string[] path_parts = fp
                                                .ToString()
                                                .Split
                                                    (
                                                        new char[]{ '/' },
                                                        System.StringSplitOptions.None
                                                    );

                        string a = path_parts[path_parts.Length - 2];
                        string g = path_parts[path_parts.Length - 3];
                                                
                        Information($"              artifact  = {g}:{a} - target SDK version = {t} min compile SDK = {mc}");
                        int t_sdk; 
                        bool ok = int.TryParse(t, out t_sdk);
                        if (ok)
                        {
                            artifacts_target_sdk.Add((g,a), t_sdk);
                        }
                    }

                    return;
                },
                () =>
                {
                    FilePath[] files_poms = GetFiles("./externals/**/*.pom").ToArray();
                    foreach(FilePath fp in files_poms)
                    {
                        Information($"files_android_pom     = {fp}");
                    }

                    artifacts_versions = new Dictionary<(string group, string artifact), string>();


                    foreach(FilePath fp in files_poms)
                    {
                        Information($"      pom.xml = {fp}");
                        xmldoc = new System.Xml.XmlDocument();
                        xmldoc.Load(fp.ToString());

                        nsmgr = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
                        nsmgr.AddNamespace("pom", "http://maven.apache.org/POM/4.0.0");

                        string v = xmldoc.SelectSingleNode($@"/pom:project/pom:version", nsmgr)?.InnerText;
                        Information($"              version     = {v}");
                        if ( v == null )
                        {
                            v = xmldoc.SelectSingleNode($@"/pom:project/pom:parent/pom:version", nsmgr)?.InnerText;
                        }
                        
                        string g = xmldoc.SelectSingleNode($@"/pom:project/pom:groupId", nsmgr)?.InnerText;
                        if ( g == null )
                        {
                            g = xmldoc.SelectSingleNode($@"/pom:project/pom:parent/pom:groupId", nsmgr)?.InnerText;
                        }
                        Information($"              groupId     = {g}");

                        string a = xmldoc.SelectSingleNode($@"/pom:project/pom:artifactId", nsmgr)?.InnerText;
                        Information($"              artifactId  = {a}");

                        artifacts_versions.Add((g, a), v);
                    }
                    return;
                }
            );

            Dictionary<(string group, string artifact), (string version, int sdk)> artifact_fq_sdk;
            artifact_fq_sdk = new Dictionary<(string group, string artifact), (string version, int sdk)>();

            foreach (KeyValuePair<(string group, string artifact), int> ga in artifacts_target_sdk)
            {
                int t_sdk = ga.Value;
                string v = artifacts_versions[ga.Key];
                
                artifact_fq_sdk.Add((ga.Key.group, ga.Key.artifact), (v, t_sdk));
            }

            List<string> log_artifacts_sdk_targets = new List<string>();

            foreach (KeyValuePair<(string g, string a), (string v, int sdk)> ga in artifact_fq_sdk)
            {
                string log = $"{ga.Key.g}.{ga.Key.a}:{ga.Value.v} - SDK {ga.Value.sdk}";
                Information(log);
                log_artifacts_sdk_targets.Add(log);
            }

            System.IO.File.WriteAllLines("./output/artifacts_sdk_targets.md", log_artifacts_sdk_targets.ToArray());

            return;
        }
    );

Task ("api-diff-markdown-info-pr")
    .IsDependentOn("binderate-diff")
    .Does
    (
        () =>
        {
            // TODO: api-diff markdown info based on diff from main
            string[] lines_from_file = System.IO.File.ReadAllLines("./output/config.json.diff-from-main.txt");

            List<string> lines = new List<string>(lines_from_file);
            List<string> changelog = new List<string>();

            List<List<string>> changelog_blocks = new List<List<string>>();
            List<string> changelog_block = null;
            int idx_start = -1;
            int idx_stop = -1;
            for(int i = 0; i < lines.Count(); i++)
            {
                string line = lines[i];
                if (line.Contains("groupId"))
                {
                    idx_start = i;
                }

                if(line.Contains("dependencyOnly"))
                {
                    if (line.StartsWith("-"))
                    {
                        continue;
                    }
                    idx_stop = i;
                }

                if (idx_start != -1 && idx_stop != -1)
                {
                    changelog_block = lines.GetRange(idx_start, idx_stop - idx_start);
                    changelog_blocks.Add(changelog_block);
                    idx_start = -1;
                    idx_stop = -1;
                }
            }


            foreach (List<string> changelog_block_lines in changelog_blocks)
            {
                string g = null;
                string a = null;
                string v_artifact_new = null;
                string v_artifact_old = null;
                string v_nuget_new = null;
                string v_nuget_old = null;
                string nuget_id = null;

                foreach(string line in changelog_block_lines)
                {
                    if (line.Contains("groupId"))
                    {
                        g = ParseDiffLine(line, "groupId");
                        continue;
                    }
                    if (line.Contains("artifactId"))
                    {
                        a = ParseDiffLine(line, "artifactId");
                        continue;
                    }
                    if (line.Contains("version"))
                    {
                        if (line.StartsWith("+"))
                        {
                            v_artifact_new = ParseDiffLine(line, "version");
                            continue;
                        }

                        v_artifact_old = ParseDiffLine(line, "version");
                        continue;
                    }

                    if (line.Contains("nugetVersion"))
                    {
                        v_nuget_old = ParseDiffLine(line, "nugetVersion");
                        continue;
                    }
                    if (line.Contains("nugetVersion"))
                    {
                        v_nuget_new = ParseDiffLine(line, "nugetVersion");
                        continue;
                    }
                    if (line.Contains("nugetId"))
                    {
                        nuget_id = ParseDiffLine(line, "nugetId");
                        continue;
                    }
                }

                if(v_artifact_new == null)
                {
                    continue;
                }

                string changelog_line = $"- {g}:{a} - {v_artifact_old} -> {v_artifact_new}";

                changelog.Add(changelog_line);
            }

            if (changelog.Count > 0)
            {
                System.IO.File.WriteAllLines("./output/changelog.md", changelog);
            }

            return;
        }
    );

string ParseDiffLine(string line, string name)
{
    if (line[0] == '-')
    {
        StringBuilder sb = new StringBuilder(line);
        sb[0] = ' ';
        line = sb.ToString();
    }

    return new string
                    (
                        line
                            .ToCharArray()
                            .Where(c => !Char.IsWhiteSpace(c))
                            .ToArray()
                    )
                    .Replace("\"", "")
                    .Replace("+", "")
                    .Replace("-", "")
                    .Replace(":", "")
                    .Replace(name, "")
                    .Replace(",", "")
                    ;
}

Task ("api-diff-analysis")
    .Does
    (
        () =>
        {
            using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
            {
                JsonTextReader jtr = new JsonTextReader(reader);
                binderator_json_array = (JArray)JToken.ReadFrom(jtr);
            }

            DirectoryPathCollection directories = GetSubDirectories("./output/api-diff");
            Dictionary<string, string>  nugets_modified = new Dictionary<string, string>();
            Dictionary<string, int[]>   api_changes_breaking_removed = new Dictionary<string, int[]>();

            foreach(DirectoryPath d in directories)
            {
                string d_name = d.GetDirectoryName();

                Information( $"Directory    = {d}");
                Information( $"     nugetId    = {d.GetDirectoryName()}");

                string groupId      = null;
                string artifactId   = null;
                string nugetId      = null;
                string nugetVersion = null;
                // no guarantees thta config.json is sorted, so linear "search"
                // TODO: sort + (LINQ or binary serch)
                foreach(JObject jo in binderator_json_array[0]["artifacts"])
                {
                    groupId      = (string) jo["groupId"];
                    artifactId   = (string) jo["artifactId"];
                    nugetId      = (string) jo["nugetId"];
                    nugetVersion = (string) jo["nugetVersion"];

                    if (nugetId == d_name)
                    {
                        Information( $"     groupId                     = {groupId}");
                        Information( $"     artifactId                  = {artifactId}");
                        Information( $"     artifactId  fully qualified = {groupId}.{artifactId}");

                        nugets_modified.Add(d_name, $"{groupId}.{artifactId}");
                    }
                }
            }

            FilePathCollection files = GetFiles(@"./output/api-diff/**/*.dll.breaking.md");
            List<string> commands = new List<string>();

            foreach(FilePath f in files)
            {
                Information($"file = {f}");

                string[] lines_in_file = System.IO.File.ReadAllLines(f.ToString());
                List<int> line_numbers = new List<int>();

                for(int i=0; i<lines_in_file.Length; i++)
                {
                    if (lines_in_file[i].ToLower().Contains("remove"))
                    {
                        line_numbers.Add(i + 1);
                        string command = $"code -n -g {f}:{i + 1}";
                        Information($"      command = {command}");
                        commands.Add(command);
                    }
                }

                api_changes_breaking_removed.Add(f.ToString(), line_numbers.ToArray());
            }

            System.IO.File.WriteAllLines("./output/commands-open-file.sh", commands.ToArray());

            var nugets_modified_ordered = nugets_modified.ToList();

            nugets_modified_ordered.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

            string[] lines = nugets_modified_ordered
                                            .Select(kv => $"- {kv.Value}" + Environment.NewLine + "\t - " + kv.Key)
                                            .ToArray();
            System.IO.File.WriteAllLines("./output/nugets-with-changed-APIs.md", lines);

            return;
        }
    );

Task("nuget-structure-analysis")
.Does
    (
        () =>
        {
            string path = "./output/*.nupkg";
            FilePathCollection files_configs = GetFiles(path);
            foreach(FilePath f in files_configs)
            {
                Information($"File  = {f}");
                string d_zip = $"{f.ToString().Replace(".nupkg", "")}";
                if (DirectoryExists(d_zip))
                {
                    DeleteDirectory
                            (
                                d_zip, 
                                new DeleteDirectorySettings 
                                {
                                    Recursive = true,
                                    Force = true
                                }
                            );
                }
                Unzip($"{f}", d_zip);

                IEnumerable<string> redirected_std_out;
                IEnumerable<string> redirected_std_err;
                int exit_code =
                                StartProcess
                                    (
                                        "tree",
                                        new ProcessSettings 
                                        {
                                            Arguments = $"-H {f.ToString().Replace(".nupkg", "")}",
                                            // WorkingDirectory = "./"
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true
                                        },
                                        out redirected_std_out,
                                        out redirected_std_err
                                    );
                
                System.IO.File.WriteAllLines
                                    (
                                        $"{f.ToString().Replace(".nupkg", ".md")}", 
                                        redirected_std_out.ToArray()
                                    );
            }
        }
    );


Task ("read-analysis-files")
    .IsDependentOn ("mappings-artifact-nuget")
    .IsDependentOn ("binderate-diff")
    .IsDependentOn ("api-diff-markdown-info-pr")
    .IsDependentOn ("namespace-check")
    .IsDependentOn ("spell-check")
    .IsDependentOn ("api-diff-analysis")
    .IsDependentOn ("list-artifacts")
    //.IsDependentOn ("nuget-structure-analysis")
    .IsDependentOn ("generate-namespace-file")
    .Does
    (
        () =>
        {
            List<string> files = new List<string>
            {
                "./output/spell-errors.txt",
                "./output/changelog.md",
                "./output/config.json.diff-from-main.txt",
                "./output/missing_dotnet_override_type.csv",
                "./output/missing_dotnet_type.csv",
                "./output/missing_java_type.csv",
                "./output/nugets-with-changed-APIs.md",
                "./output/commands-open-file.sh",
                "./output/artifacts_sdk_targets.md",
            };

            if ( ! FileExists("./output/spell-errors.txt") )
            {
                files.Remove("./output/spell-errors.txt");
            }

			string process = "code";
			string process_args = $"-n {string.Join(" ", files)}";
			IEnumerable<string> redirectedStandardOutput;
			ProcessSettings process_settings = new ProcessSettings ()
			{
             Arguments = process_args,
             RedirectStandardOutput = true
         	};
			int exitCodeWithoutArguments = StartProcess(process, process_settings, out redirectedStandardOutput);
			Information("Exit code: {0}", exitCodeWithoutArguments);
        }
    );

Task("dependencies")
    .Does
    (
        () =>
        {
            if (!(binderator_json_array?.Count > 0))
            {
                using (StreamReader reader = System.IO.File.OpenText(@"./config.json"))
                {
                    JsonTextReader jtr = new JsonTextReader(reader);
                    binderator_json_array = (JArray)JToken.ReadFrom(jtr);
                }
            }

            Warning("config.json dependencies ...");
            foreach(JObject jo in binderator_json_array[0]["artifacts"])
            {
                string groupId      = (string) jo["groupId"];
                string artifactId   = (string) jo["artifactId"];
                string nugetId      = (string) jo["nugetId"];
                string nugetVersion = (string) jo["nugetVersion"];

            }
        }
    );

Task("generate-namespace-file")
    .Does
    (
        () =>
        {
            var list = FindNamespacesInDirectory ("./generated");
            System.IO.File.WriteAllLines ("published-namespaces.txt", list);        
        }
    );

Task("verify-namespace-file")
    .Does
    (
        () =>
        {
            var new_list = FindNamespacesInDirectory ("./generated");
            var old_list = System.IO.File.ReadAllLines ("published-namespaces.txt");

            var unhandled_changes = false;

            var new_ns = new_list.Except (old_list);

            if (new_ns.Any ()) {
                unhandled_changes = true;
                Console.WriteLine ("New Namespaces");
                Console.WriteLine ("--------------");

                foreach (var ns in new_ns)
                  Console.WriteLine (ns);
                  
                Console.WriteLine ();
            }

            var removed_ns = old_list.Except (new_list);

            if (removed_ns.Any ()) {
                unhandled_changes = true;
                Console.WriteLine ("Removed Namespaces");
                Console.WriteLine ("------------------");

                foreach (var ns in removed_ns)
                    Console.WriteLine (ns);
            }

            if (unhandled_changes)
                throw new Exception ("Namespaces were added or removed.");
        }
    );

static List<string> FindNamespacesInDirectory (string directory)
{
    var list = new SortedSet<string> ();

    foreach (var file in System.IO.Directory.EnumerateFiles (directory, "*.dll", SearchOption.AllDirectories))
        foreach (var ns in FindNamespaces (file))
            list.Add (ns);

	return list.ToList ();
}

static List<string> FindNamespaces (string assembly)
{
    var asm = AssemblyDefinition.ReadAssembly (assembly);
    var list = new SortedSet<string> ();

    foreach (var module in asm.Modules)
        foreach (var type in module.Types)
            if (!string.IsNullOrWhiteSpace (type.Namespace))
                list.Add (type.Namespace);

    return list.ToList ();
}


Task ("Default")
    .IsDependentOn ("read-analysis-files")
    ;

if (System.IO.File.Exists(file_spell_errors))
{
    string separator = System.Environment.NewLine + "\t" + "\t";
    string msg = "Spell Errors:" + System.Environment.NewLine + "\t" + "\t"
                    + string.Join(separator, System.IO.File.ReadAllLines(file_spell_errors));
    throw new Exception(msg);
}

RunTarget (TARGET);
