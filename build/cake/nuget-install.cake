/*
Nuget download

https://www.nuget.org/api/v2/package/{packageID}/

https://www.nuget.org/api/v2/package/cake.coreclr/1.3.0/
*/

Task("nuget-install")
    .Does
    (
        () =>
        {
            DownloadNugetPackage("cake.coreclr", "1.3.0");
        }
    );
