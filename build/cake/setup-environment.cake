// Contains tasks for preparing the build environment

// Adds git commit info to build files
Task ("inject-variables")
    .WithCriteria (!BuildSystem.IsLocalBuild)
    .Does (() =>
{
    var glob = "./source/AssemblyInfo.cs";

    ReplaceTextInFiles (glob, "{BUILD_COMMIT}", BUILD_COMMIT);
    ReplaceTextInFiles (glob, "{BUILD_NUMBER}", BUILD_NUMBER);
    ReplaceTextInFiles (glob, "{BUILD_TIMESTAMP}", BUILD_TIMESTAMP);
});
