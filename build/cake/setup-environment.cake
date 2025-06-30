// Contains tasks for preparing the build environment

// Adds git commit info to build files
Task ("inject-variables")
    .WithCriteria (!BuildSystem.IsLocalBuild)
    .Does (() =>
{
    var glob = "./source/AssemblyInfo.cs";

    ReplaceTextInFile (glob, "{BUILD_COMMIT}", BUILD_COMMIT);
    ReplaceTextInFile (glob, "{BUILD_NUMBER}", BUILD_NUMBER);
    ReplaceTextInFile (glob, "{BUILD_TIMESTAMP}", BUILD_TIMESTAMP);
});

void ReplaceTextInFile (string filePath, string oldValue, string newValue)
{
    var text = System.IO.File.ReadAllText (filePath);
    System.IO.File.WriteAllText (filePath, text.Replace (oldValue, newValue));
}
