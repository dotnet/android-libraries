// Install ZString as a Cake Addin
#addin nuget:?package=ZString&version=2.6.0

// Install ZString as a Cake Tool
// #tool nuget:?package=ZString&version=2.6.0

using System.Threading.Tasks;

Task ("build-android-libraries-net10-net8")
    .Does
    (
        () =>
        {
            Parallel.Invoke
                        (
                            () => RunTarget("net8-prepare-binderate-build"),
                            () => RunTarget("build-prepare-dotnet-android") 
                        );

            RunTarget("net10-prepare-binderate-build");
            RunTarget("net10-net8-prepare-binderate-build");                    
        }        
    )
    ;

string dotnet;
DeleteDirectorySettings delete_directory_setting = new () 
                                                    {
                                                        Recursive = true,
                                                        Force = true
                                                    };


Task ("build-prepare-dotnet-android")
    .Does
    (
        () =>
        {
            string dir = "../dotnet-android";
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

            ps.Arguments = new ProcessArgumentBuilder().Append("prepare");
            StartProcess
                (
                    "make",
                    ps
                );
            Information(sb.ToString());                
            sb.Clear();
            

            ps.Arguments = new ProcessArgumentBuilder().Append("");
            StartProcess
                (
                    "make",
                    ps
                );

            ps.Arguments = new ProcessArgumentBuilder().Append("--version");
            StartProcess
                (
                    $"{dir}/dotnet-local.sh",
                    ps
                );
        }
    );


Task ("net10-net8-prepare-binderate-build")
    .Does
    (
        () =>
        {
            dotnet = "../dotnet-android/dotnet-local.sh";

            Information($"{new string('=', 120)}");
            StartProcess(dotnet, "--version");
            StartProcess
                    (
                        dotnet,
                        "workload restore --project ./generated/androidx.activity.activity/androidx.activity.activity.csproj"
                    );
            StartProcess(dotnet, "cake -t=ci");

            //git restore pathTo/MyFile
            MoveDirectory("generated", "generated--net10.0-net8.0");

        }
    );


Task ("net10-prepare-binderate-build")
    .Does
    (
        () =>
        {
            dotnet = "../dotnet-android/dotnet-local.sh";

            Information($"{new string('=', 120)}");
            StartProcess(dotnet, "--version");
            StartProcess
                    (
                        dotnet,
                        "workload restore --project ./generated/androidx.activity.activity/androidx.activity.activity.csproj"
                    );
            StartProcess(dotnet, "cake -t=binderate");
            Parallel.ForEach
                         (
                            files_net10.Keys,
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

            StartProcess(dotnet, "cake -t=ci");

            MoveDirectory("generated", "generated--net10.0");

            foreach(string file in files_net10.Keys)
            {
                StartProcess("git", $"restore {file}");
                
            }
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

            dotnet = "dotnet";

            Information($"{new string('=', 120)}");
            StartProcess(dotnet, "--version");
            StartProcess(dotnet, "cake -t=ci");

            MoveDirectory("generated", "generated-net8.0");
        }
    );


Dictionary<string, List<(string text_old, string text_new)>> files_net8_net10;
Dictionary<string, List<(string text_old, string text_new)>> files_net10;


files_net10 = new Dictionary<string, List<(string text_old, string text_new)>>
{
    {
        "./global.json",
        [
            (
                """        "version": "8.0.404",""",
                """        "version": "10.0.100-preview.2.25102.3","""
            ),
        ]
    },
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

files_net8_net10 = new Dictionary<string, List<(string text_old, string text_new)>>
{
    {
        "./global.json",
        [
            (
                """        "version": "8.0.404",""",
                """        "version": "10.0.100-preview.2.25102.3","""
            ),
        ]
    },
    {
        "./Directory.Build.props",
        [
            (
                """<_DefaultTargetFrameworks>net8.0</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net8.0;net10.0</_DefaultTargetFrameworks>"""
            ),
            (
                """<_DefaultTargetFrameworks>net8.0-android</_DefaultTargetFrameworks>""",
                """<_DefaultTargetFrameworks>net8.0-android;net10.0-android</_DefaultTargetFrameworks>"""
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