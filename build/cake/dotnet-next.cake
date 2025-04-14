// Updates repository to be tested on preview versions of .NET

Task ("dotnet-next")
    .Does (() =>
{
  var next_net_framework_version = Argument<string> ("framework-version");
  var next_api_level_version = Argument<string> ("api-level-version");
  var next_net_sdk_version = Argument<string> ("dotnet-version");
  
  Information ("");
  Information ("Script Arguments:");
  Information ("  dotnet Version: {0}", next_net_framework_version);
  Information ("  dotnet TF Version: {0}", next_net_sdk_version);
  Information ("  Android SDK API Level: {0}", next_api_level_version);
  Information ("");

  // Update global.json
  var global_json = File ("global.json");
  var json = JToken.Parse (FileReadText (global_json));
  json["sdk"]["version"] = next_net_sdk_version;
  
  FileWriteText (global_json, json.ToString ());
  
  // Update Directory.Build.props
  var directory_build = File ("Directory.Build.props");
  
  XmlPoke (directory_build, "/Project/PropertyGroup/_DefaultNetTargetFrameworks", $"net{next_net_framework_version}.0");
  XmlPoke (directory_build, "/Project/PropertyGroup/_DefaultTargetFrameworks", $"net{next_net_framework_version}.0-android");

  XmlPoke (directory_build, "/Project/ItemGroup/AndroidXNuGetTargetFolders[starts-with (@Include, 'build\\')]/@Include", $@"build\net{next_net_framework_version}.0-android{next_api_level_version}.0");
  XmlPoke (directory_build, "/Project/ItemGroup/AndroidXNuGetTargetFolders[starts-with (@Include, 'buildTransitive\\')]/@Include", $@"buildTransitive\net{next_net_framework_version}.0-android{next_api_level_version}.0");

  XmlPoke (directory_build, "/Project/ItemGroup/AndroidXNuGetLibFolders/@Include", $@"lib\net{next_net_framework_version}.0-android{next_api_level_version}.0");
  
  // Update test target versions
  var test_all_packages_file = File ("tests/allpackages/TestAllIndividualPackages.cs");
  var test_all_packages_content = FileReadText (test_all_packages_file);
  test_all_packages_content = test_all_packages_content.Replace ("static string net_version = \"net8.0\"", $"static string net_version = \"net{next_net_framework_version}.0\"");
  FileWriteText (test_all_packages_file, test_all_packages_content);
});    
