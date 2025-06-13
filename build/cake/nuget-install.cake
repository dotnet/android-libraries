/*
Nuget download
https://www.nuget.org/api/v2/package/{packageID}/
https://api.nuget.org/v3/index.json?package=CliWrap&version=3.8.2
https://api.nuget.org/v3-flatcontainer/cliwrap/3.8.2/cliwrap.3.8.2.nupkg
    },
https://www.nuget.org/api/v2/package/cake.coreclr/1.3.0/
*/
/*
#addin nuget:https://api.nuget.org/v3/index.json?package=Mono.Cecil&version=0.11.6
#addin nuget:https://api.nuget.org/v3/index.json??package=HolisticWare.Xamarin.Tools.ComponentGovernance&version=0.0.1.4
#addin nuget:https://api.nuget.org/v3/index.json??package=HolisticWare.Core.Net.HTTP&version=0.0.4
#addin nuget:https://api.nuget.org/v3/index.json??package=HolisticWare.Core.IO&version=0.0.4
*/

Dictionary<string, string> nuget_packages = new ()
{
    // { "cake.coreclr", "1.3.0" },
    { "Cake.FileHelpers", "7.0.0"},                                     // migrated, but needed for windows only???
    { "HolisticWare.Xamarin.Tools.ComponentGovernance", "0.0.1.4" },
    { "HolisticWare.Core.Net.HTTP", "0.0.4" },
    { "HolisticWare.Core.IO", "0.0.4" },
};

Task("nuget-install")
    .Does
    (
        () =>
        {
            EnsureDirectoryExists("./output");

            /*
            nuget.exe must be in the PATH
            NuGetInstall
                    (
                        "HolisticWare.Xamarin.Tools.ComponentGovernance", 
                        new NuGetInstallSettings 
                            {
                                Version = "0.0.1.4",
                                OutputDirectory = "./tools/Addins"
                                }
                    );
            NuGetInstall
                    (
                        "HolisticWare.Core.Net.HTTP", 
                        new NuGetInstallSettings 
                            {
                                Version = "0.0.4",
                                OutputDirectory = "./tools/Addins"
                            }
                    );
            NuGetInstall
                    (
                        "HolisticWare.Core.IO", 
                        new NuGetInstallSettings 
                            {
                                Version = "0.0.4",
                                OutputDirectory = "./tools/Addins"
                            }
                    );
            NuGetInstall
                    (
                        "CliWrap", 
                        new NuGetInstallSettings 
                            {
                                Version = "3.8.2",
                                OutputDirectory = "./tools/Addins"
                            }
                    );
            */
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/cake.filehelpers/7.0.0/cake.filehelpers.7.0.0.nupkg", 
                        $"./output/cake.filehelpers.7.0.0.nupkg"
                    );
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/cliwrap/3.8.2/cliwrap.3.8.2.nupkg", 
                        $"./output/cliwrap.3.8.2.nupkg"
                    );
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/holisticware.core.net.http/0.0.4/holisticware.core.net.http.0.0.4.nupkg", 
                        $"./output/holisticware.core.net.http.0.0.4.nupkg"
                    );
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/holisticware.core.io/0.0.4/holisticware.core.io.0.0.4.nupkg", 
                        $"./output/holisticware.core.io.0.0.4.nupkg"
                    );
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/holisticware.xamarin.tools.componentgovernance/0.0.1.4/holisticware.xamarin.tools.componentgovernance.0.0.1.4.nupkg", 
                        $"./output/holisticware.xamarin.tools.componentgovernance.0.0.1.4.nupkg"
                    );
            /*
            */
        }
    );

Task("nuget-uninstall")
    .Does
    (
        () =>
        {
            string file;
            // validation fails on CI if the package is in the output directory

            file = $"./output/cake.filehelpers.7.0.0.nupkg";
            if (FileExists (file))
            {
                DeleteFile (file);
            }
            file = $"./output/cliwrap.3.8.2.nupkg";
            if (FileExists (file))
            {
                DeleteFile (file);
            }
            file = $"./output/holisticware.core.net.http.0.0.4.nupkg";
            if (FileExists (file))
            {
                DeleteFile (file);
            }

            file = $"./output/holisticware.core.io.0.0.4.nupkg";
            if (FileExists (file))
            {
                DeleteFile (file);
            }

            file = $"./output/holisticware.xamarin.tools.componentgovernance.0.0.1.4.nupkg";
            if (FileExists (file))
            {
                DeleteFile (file);
            }
        }
    );