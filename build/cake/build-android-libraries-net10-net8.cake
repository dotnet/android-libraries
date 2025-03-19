// Install ZString as a Cake Addin
#addin nuget:?package=ZString&version=2.6.0

// Install ZString as a Cake Tool
// #tool nuget:?package=ZString&version=2.6.0

/*
Quick test

```shell
    git clean -xdf ; dotnet cake -t=build-android-libraries-net10-net8
```

Step by step tests:

```shell
    git clean -xdf
    dotnet cake -t=net8-prepare-binderate-build
    dotnet cake -t=revert-changes-net8
    dotnet cake -t=net10-prepare-binderate-build
    dotnet cake -t=revert-changes-net10
    dotnet cake -t=net10-net8-prepare-binderate-build
    dotnet cake -t=copy-net8-with-net8-to-multi-target
    dotnet cake -t=nuget-pack-without-build-net10-net8
    dotnet cake -t=revert-changes-net10-net8
```

Tests from scratch:

```bash
git clone \
    --branch mu-20241209-net10-removal-of-generator-workarounds \
    https://github.com/dotnet/android-libraries.git \
    al-bash/
cd al-bash/
dotnet cake -t:build-android-libraries-net10-net8
```

```pwsh
git clone `
    --branch mu-20241209-net10-removal-of-generator-workarounds `
    https://github.com/dotnet/android-libraries.git `
    al-pwsh/
cd al-pwsh/
dotnet cake -t:build-android-libraries-net10-net8
```
*/

bool build_dotnet_android = false;

using System.Threading.Tasks;

Task ("build-android-libraries-net10-net8")
    .Does
    (
        () =>
        {
            // Parallel.Invoke
            //             (
            //                 () => RunTarget("build-prepare-dotnet-android"),
            //                 () => RunTarget("net8-prepare-binderate-build")
            //             );

            if (build_dotnet_android)
            {
                RunTarget("build-prepare-dotnet-android");
            }
            RunTarget("net8-prepare-binderate-build");
            RunTarget("revert-changes-net8");
            // RunTarget("net10-prepare-binderate-build");       // not needed -  for testing purposes only
            // RunTarget("revert-changes-net10");
            RunTarget("net10-net8-prepare-binderate-build");
            RunTarget("copy-net8-with-net8-to-multi-target");
            RunTarget("nuget-pack-without-build-net10-net8");
            RunTarget("revert-changes-net10-net8");
        }
    );

string dotnet;
string path_global_json = "./global.json";
string content_global_json;
DeleteDirectorySettings delete_directory_setting = new ()
                                                    {
                                                        Recursive = true,
                                                        Force = true
                                                    };

Task ("nuget-pack-without-build-net10-net8")
    .Does
    (
        () =>
        {
            if (build_dotnet_android)
            {
                dotnet = "../dotnet-android/dotnet-local.sh";
            }
            else 
            {
                dotnet = "dotnet";
            }

            var projects = GetFiles($"./generated/**/*.csproj");

            Information($"{new string('=', 120)}");
            string dotnet_pack =    "pack"
                                    + " " +
                                    "__PLACEHOLDER_PROJECT__"
                                    + " " +
                                    "--no-build"
                                    + " " +
                                    "--output ./output/nupkgs/"
                                    ;
            /*
            System has ran out of application memory.

            dotnet(94604) MallocStackLogging: can't turn off malloc stack logging because it was not enabled.
            */
            Information($"{new string('=', 120)}");
            Information($"  Environment.ProcessorCount = {Environment.ProcessorCount}");
            
            Parallel.ForEach
                         (
                            projects,
                            new ParallelOptions 
                                        {
                                            MaxDegreeOfParallelism = (int) Math.Round(0.75 * Environment.ProcessorCount)
                                        },
                            (FilePath file) =>
                            {
                                string dotnet_pack_project = dotnet_pack.Replace
                                                                        (
                                                                            "__PLACEHOLDER_PROJECT__",
                                                                            file.ToString()
                                                                        );
                                StartProcess(dotnet, dotnet_pack_project);
                                Information($"{file.ToString()}");
                            }
                         );

        }
    );

Task ("build-prepare-dotnet-android")
    .Does
    (
        () =>
        {
            if ( BuildSystem.IsRunningOnAzurePipelines || BuildSystem.IsRunningOnGitHubActions )
            {
                Information("IsRunningOnAzurePipelines || IsRunningOnGitHubActions");
                RunTarget("prepare-dotnet-android");
            }

            Information("Local Build");
            content_global_json =
            """
            {
                "sdk":
                {
                    "version": "8.0.407",
                    "rollForward": "patch"
                },
                "msbuild-sdks":
                {
                    "MSBuild.Sdk.Extras": "3.0.44",
                    "Microsoft.Build.Traversal": "4.1.82",
                    "Microsoft.Build.NoTargets": "3.7.56",
                    "Xamarin.Legacy.Sdk": "0.2.0-alpha4"
                }
            }
            """;
            System.IO.File.WriteAllText(path_global_json, content_global_json);

            string dir = "../dotnet-android/";
            DeleteDirectories(GetDirectories(dir), delete_directory_setting);

            StartProcess("git", $"clone --recursive https://github.com/dotnet/android.git {dir}");

            ProcessSettings ps = new ProcessSettings
                                            {
                                                WorkingDirectory = dir,
                                                RedirectStandardOutput = true,
                                                RedirectStandardError = true
                                            };

            Cysharp.Text.Utf16ValueStringBuilder sb = Cysharp.Text.ZString.CreateStringBuilder();

    		ps.RedirectedStandardOutputHandler = (output) =>
                                                            {
                                                                sb.AppendLine(output);
                                                                return output;
                                                            };

            Information("make prepare");
            ps.Arguments = new ProcessArgumentBuilder().Append("prepare");
            StartProcess("make",ps);
            Information(sb.ToString());
            sb.Clear();

            Information("make");
            ps.Arguments = new ProcessArgumentBuilder().Append("");
            StartProcess("make",ps);

            ps.Arguments = new ProcessArgumentBuilder().Append("--version");
            StartProcess($"{dir}/dotnet-local.sh", ps);
        }
    );

Task ("prepare-dotnet-android")
    .Does
    (
        () =>
        {
            if (IsRunningOnMacOs())
            {
                Information("IsRunningOnMacOs");
                // https://github.com/dotnet/android/blob/main/Documentation/building/unix/dependencies.md

                StartProcess("curl", $"-c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\"");

                StartProcess("brew", $"install automake");
                StartProcess("brew", $"install autoconf");
                StartProcess("brew", $"install cmake");
                StartProcess("brew", $"install libtool");
                StartProcess("brew", $"install p7zip");
                StartProcess("brew", $"install gdk-pixbuf");
                StartProcess("brew", $"install gettext");
                StartProcess("brew", $"install coreutils");
                StartProcess("brew", $"install findutils");
                StartProcess("brew", $"install gnu-tar");
                StartProcess("brew", $"install gnu-sed");
                StartProcess("brew", $"install gawk");
                StartProcess("brew", $"install gnutls");
                StartProcess("brew", $"install gnu-indent");
                StartProcess("brew", $"install gnu-getopt");
                StartProcess("brew", $"install intltool");
                StartProcess("brew", $"install ninja");
                StartProcess("brew", $"install scons");
                StartProcess("brew", $"install wget");
                StartProcess("brew", $"install xz");

            }
            if (IsRunningOnWindows())
            {
                Information("IsRunningOnWindows");
                // https://github.com/dotnet/android/blob/main/Documentation/building/windows/dependencies.md
                // https://winget.run/search

                /*
                StartProcess("winget", $"install -e --id JernejSimoncic.Wget");
                StartProcess("winget", $"install -e --id Ninja-build.Ninja");
                */
            }

             
        }
    );

Task ("net10-net8-prepare-binderate-build")
    .Does
    (
        () =>
        {
            /*
            ../dotnet-android/dotnet-local.sh cake -t=net10-prepare-binderate-build
            */

            if (build_dotnet_android)
            {
                dotnet = "../dotnet-android/dotnet-local.sh";
            }
            else 
            {
                dotnet = "dotnet";
            }

            content_global_json =
            """
            {
                "sdk":
                {
                    "version": "10.0.100-preview.1.25120.13",
                    "rollForward": "patch"
                },
                "msbuild-sdks":
                {
                    "MSBuild.Sdk.Extras": "3.0.44",
                    "Microsoft.Build.Traversal": "4.1.82",
                    "Microsoft.Build.NoTargets": "3.7.56",
                    "Xamarin.Legacy.Sdk": "0.2.0-alpha4"
                }
            }
            """;
            System.IO.File.WriteAllText(path_global_json, content_global_json);

            Information($"{new string('=', 120)}");
            RunTarget("binderate");
            Parallel.ForEach
                         (
                            files_net10_net8.Keys,
                            new ParallelOptions 
                                        {
                                            MaxDegreeOfParallelism = (int) Math.Round(0.75 * Environment.ProcessorCount)
                                        },
                            (string file) =>
                            {
                                List<(string text_old, string text_new)> replacements = files_net10_net8[file];

                                string content = System.IO.File.ReadAllText(file);

                                foreach((string text_old, string text_new) pair in replacements)
                                {
                                    content = content.Replace(pair.text_old, pair.text_new);
                                }

                                System.IO.File.WriteAllText(file, content);
                            }
                         );


            StartProcess
                    (
                        dotnet,
                        "workload restore --project ./generated/androidx.activity.activity/androidx.activity.activity.csproj"
                    );
            RunTarget("nuget");

            //git restore pathTo/MyFile

            DeleteDirectories(GetDirectories("generated-net10.0-net8.0"), delete_directory_setting);
            DeleteDirectories(GetDirectories("output-net10.0-net8.0"), delete_directory_setting);
            MoveDirectory("generated", "generated-net10.0-net8.0");
            MoveDirectory("output", "output-net10.0-net8.0");
        }
    );

Task ("copy-net8-with-net8-to-multi-target")
    .Does
    (
        () =>
        {
            string assembly_name_source;
            string assembly_name_target;

            var assemblies = GetFiles($"generated-net8.0/**/bin/Release/net8.0-android/*.dll");

            foreach(var assembly in assemblies)
            {
                assembly_name_source = System.IO.Path.GetFullPath(assembly.ToString());
                assembly_name_target = System.IO.Path
                                                .GetDirectoryName(assembly_name_source)
                                                .Replace
                                                    (
                                                        "generated-net8.0",
                                                        "generated-net10.0-net8.0"
                                                    );
                DateTime dt_c_source = System.IO.File.GetCreationTime(assembly_name_source);
                DateTime dt_a_source = System.IO.File.GetLastAccessTime(assembly_name_source);
                DateTime dt_w_source = System.IO.File.GetLastWriteTime(assembly_name_source);
                DateTime dt_c_target = System.IO.File.GetCreationTime(assembly_name_target);
                DateTime dt_a_target = System.IO.File.GetLastAccessTime(assembly_name_target);
                DateTime dt_w_target = System.IO.File.GetLastWriteTime(assembly_name_target);

                Information($"{new string('-', 120)}");
                Information($"source {assembly_name_source}");
                Information($"          c:    {dt_c_source.ToString("yyyyMMdd-HHmmss")}");
                Information($"          w:    {dt_w_source.ToString("yyyyMMdd-HHmmss")}");
                Information($"          a:    {dt_a_source.ToString("yyyyMMdd-HHmmss")}");
                Information($"target {assembly_name_target}");
                Information($"          c:    {dt_c_target.ToString("yyyyMMdd-HHmmss")}");
                Information($"          w:    {dt_w_target.ToString("yyyyMMdd-HHmmss")}");
                Information($"          a:    {dt_a_target.ToString("yyyyMMdd-HHmmss")}");

                CopyFiles(assembly_name_source, assembly_name_target);
            }

            string s = "generated-net10.0-net8.0";
            string t = "generated";

            Information($"{new string('-', 120)}");
            Information($"copying");
            Information($"  source      {s}");
            Information($"  source      {t}");
            CopyDirectory(s, t);
        }
    );


Task ("revert-changes-net8")
    .Does
    (
        () =>
        {
            DeleteFile(path_global_json);
        }
    );

Task ("revert-changes-net10")
    .Does
    (
        () =>
        {
            foreach(string file in files_net10.Keys)
            {
                StartProcess("git", $"restore {file}");
            }
        }
    );

Task ("revert-changes-net10-net8")
    .Does
    (
        () =>
        {
            foreach(string file in files_net10_net8.Keys)
            {
                StartProcess("git", $"restore {file}");
            }

            StartProcess("git", $"restore global.json");
        }
    );

Task ("net10-prepare-binderate-build")
    .Does
    (
        () =>
        {
            content_global_json =
            """
            {
                "sdk":
                {
                    "version": "10.0.100-preview.1.25120.13",
                    "rollForward": "patch"
                },
                "msbuild-sdks":
                {
                    "MSBuild.Sdk.Extras": "3.0.44",
                    "Microsoft.Build.Traversal": "4.1.82",
                    "Microsoft.Build.NoTargets": "3.7.56",
                    "Xamarin.Legacy.Sdk": "0.2.0-alpha4"
                }
            }
            """;
            System.IO.File.WriteAllText(path_global_json, content_global_json);

            Information("Change files");
            Parallel.ForEach
                         (
                            files_net10.Keys,
                            new ParallelOptions 
                                        {
                                            MaxDegreeOfParallelism = (int) Math.Round(0.75 * Environment.ProcessorCount)
                                        },
                            (string file) =>
                            {
                                List<(string text_old, string text_new)> replacements = files_net10[file];

                                string content = System.IO.File.ReadAllText(file);

                                foreach((string text_old, string text_new) pair in replacements)
                                {
                                    content = content.Replace(pair.text_old, pair.text_new);
                                }

                                System.IO.File.WriteAllText(file, content);
                            }
                         );

            Information("binderate");
            RunTarget("binderate");
            StartProcess
                    (
                        dotnet,
                        "workload restore --project ./generated/androidx.activity.activity/androidx.activity.activity.csproj"
                    );
            RunTarget("nuget");

            DeleteDirectories(GetDirectories("generated-net10.0"), delete_directory_setting);
            DeleteDirectories(GetDirectories("output-net10.0"), delete_directory_setting);
            MoveDirectory("generated", "generated-net10.0");
            MoveDirectory("output", "output-net10.0");
        }
    );

Task ("net8-prepare-binderate-build")
    .Does
    (
        () =>
        {
            DeleteDirectories(GetDirectories("./output/"), delete_directory_setting);
            DeleteDirectories(GetDirectories("./externals/"), delete_directory_setting);
            DeleteDirectories(GetDirectories("./generated*/"), delete_directory_setting);

            content_global_json =
            """
            {
                "sdk":
                {
                    "version": "8.0.407",
                    "rollForward": "patch"
                },
                "msbuild-sdks":
                {
                    "MSBuild.Sdk.Extras": "3.0.44",
                    "Microsoft.Build.Traversal": "4.1.82",
                    "Microsoft.Build.NoTargets": "3.7.56",
                    "Xamarin.Legacy.Sdk": "0.2.0-alpha4"
                }
            }
            """;
            System.IO.File.WriteAllText(path_global_json, content_global_json);

            EnsureDirectoryExists("./output");

            dotnet = "dotnet";

            Information($"{new string('=', 120)}");
            RunTarget("binderate");
            StartProcess
                    (
                        dotnet,
                        "workload restore --project ./generated/androidx.activity.activity/androidx.activity.activity.csproj"
                    );
            RunTarget("nuget");

            DeleteDirectories(GetDirectories("generated-net8.0"), delete_directory_setting);
            DeleteDirectories(GetDirectories("output-net8.0"), delete_directory_setting);
            MoveDirectory("generated", "generated-net8.0");
            MoveDirectory("output", "output-net8.0");
        }
    );

Dictionary<string, List<(string text_old, string text_new)>> files_net10;
Dictionary<string, List<(string text_old, string text_new)>> files_net10_net8;


files_net10 = new Dictionary<string, List<(string text_old, string text_new)>>
{
    /*
    {
        "./global.json",
        [
            (
                """        "version": "8.0.404",""",
                """        "version": "10.0.100-preview.2.25102.3","""
            ),
        ]
    },
    */
    {
        "./Directory.Build.props",
        [
            (
                """<_DefaultTargetFrameworks>net8.0</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net10.0</_DefaultTargetFrameworks>"""
            ),
            (
                """<_DefaultTargetFrameworks>net8.0-android</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net10.0-android</_DefaultTargetFrameworks>"""
            ),
            (
                """<AndroidXNuGetTargetFolders Include="build\net8.0-android34.0" />""",
                """<AndroidXNuGetTargetFolders Include="build\net10.0-android35.0" />"""
            ),
            (
                """<AndroidXNuGetTargetFolders Include="buildTransitive\net8.0-android34.0" />""",
                """<AndroidXNuGetTargetFolders Include="buildTransitive\net10.0-android35.0" />"""
            ),
            (
                """<AndroidXNuGetLibFolders Include="lib\net8.0-android34.0" />""",
                """<AndroidXNuGetLibFolders Include="lib\\net10.0-android35.0" />"""
            ),
        ]
    },
    {
        "./tests/Directory.Build.props",
        [
            (
                """<_DefaultTargetFrameworks>net8.0-android</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net10.0-android</_DefaultTargetFrameworks>"""
            ),
            (
                """<_DefaultNetTargetFrameworks>net8.0</_DefaultNetTargetFrameworks>""",
                """<_DefaultNetTargetFrameworks>net10.0</_DefaultNetTargetFrameworks>"""
            ),
        ]
    },
    {
        "./source/androidx.appcompat/typeforwarders/androidx.appcompat.appcompat-resources-typeforwarders.csproj",
        [
            (
                """<PackageVersion>1.1.0.3</PackageVersion>""",
                """<PackageVersion>1.1.0.3$(PackageVersionSuffix)</PackageVersion>"""
            ),
        ]
    },
};

files_net10_net8 = new Dictionary<string, List<(string text_old, string text_new)>>
{
    /*
    {
        "./global.json",
        [
            (
                """        "version": "8.0.404",""",
                """        "version": "10.0.100-preview.2.25102.3","""
            ),
        ]
    },
    */
    {
        "./Directory.Build.props",
        [
            (
                """<_DefaultTargetFrameworks>net8.0-android</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net8.0-android;net10.0-android</_DefaultTargetFrameworks>"""
            ),
            (
                """<_DefaultNetTargetFrameworks>net8.0</_DefaultNetTargetFrameworks>""",
                """<_DefaultNetTargetFrameworks>net8.0;net10.0</_DefaultNetTargetFrameworks>"""
            ),
            (
                """<AndroidXNuGetTargetFolders Include="build\net8.0-android34.0" />""",
                """
                <AndroidXNuGetTargetFolders Include="build\net8.0-android34.0" />
                <AndroidXNuGetTargetFolders Include="build\net10.0-android35.0" />
                """
            ),
            (
                """<AndroidXNuGetTargetFolders Include="buildTransitive\net8.0-android34.0" />""",
                """
                <AndroidXNuGetTargetFolders Include="buildTransitive\net8.0-android34.0" />
                <AndroidXNuGetTargetFolders Include="buildTransitive\net10.0-android35.0" />
                """
            ),
            (
                """<AndroidXNuGetLibFolders Include="lib\net8.0-android34.0" />""",
                """
                <AndroidXNuGetLibFolders Include="lib\\net8.0-android34.0" />
                <AndroidXNuGetLibFolders Include="lib\\net10.0-android35.0" />
                """
            ),
        ]
    },
    {
        "./tests/Directory.Build.props",
        [
            (
                """<_DefaultTargetFrameworks>net8.0-android</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net8.0-android;net10.0-android</_DefaultTargetFrameworks>"""
            ),
            (
                """<_DefaultNetTargetFrameworks>net8.0</_DefaultNetTargetFrameworks>""",
                """<_DefaultNetTargetFrameworks>net8.0;net10.0</_DefaultNetTargetFrameworks>"""
            ),
        ]
    },
    {
        "./source/androidx.appcompat/typeforwarders/androidx.appcompat.appcompat-resources-typeforwarders.csproj",
        [
            (
                """<PackageVersion>1.1.0.3</PackageVersion>""",
                """<PackageVersion>1.1.0.3$(PackageVersionSuffix)</PackageVersion>"""
            ),
        ]
    },
};
