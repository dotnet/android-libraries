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
    // { "cake.coreclr", "1.3.0" }
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
            DownloadFile
                    (
                        "https://api.nuget.org/v3-flatcontainer/cliwrap/3.8.2/cliwrap.3.8.2.nupkg", 
                        $"./output/cliwrap.3.8.2.nupkg"
                    );
        }
    );
