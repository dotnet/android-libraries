// Used by binderator, "Windows" is fine because we only use managed code from it
#tool nuget:?package=Microsoft.Android.Sdk.Windows&version=35.0.0-rc.1.80

// Cake Addins
#addin nuget:?package=Cake.FileHelpers&version=7.0.0
#addin nuget:?package=Newtonsoft.Json&version=13.0.3
#addin nuget:?package=SharpZipLib&version=1.4.2

// Imported scripts
#load "build/cake/setup-environment.cake"
#load "build/cake/update-config.cake"
#load "build/cake/tests.cake"
#load "build/cake/gps-parameters.cake"
#load "build/cake/dotnet-next.cake"
#load "build/cake/binderate.cake"
#load "build/cake/build-and-package.cake"
#load "build/cake/validations.cake"
#load "build/cake/executive-order.cake"
#load "build/cake/clean.cake"
#load "build/cake/performance-timings.cake"

using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// The main configuration points
var TARGET = Argument ("t", Argument ("target", "Default"));
var CONFIGURATION = Argument ("c", Argument ("configuration", "Release"));
var VERBOSITY = Argument ("v", Argument ("verbosity", Verbosity.Normal));

// Load all the git variables
var BUILD_COMMIT = EnvironmentVariable("BUILD_COMMIT") ?? "DEV";
var BUILD_NUMBER = EnvironmentVariable("BUILD_NUMBER") ?? "DEBUG";
var BUILD_TIMESTAMP = DateTime.UtcNow.ToString();

var JAVA_HOME = EnvironmentVariable ("JAVA_HOME") ?? Argument ("java_home", "");

// Log some variables
Information ("");
Information ($"JAVA_HOME            : {JAVA_HOME}");
Information ($"BUILD_COMMIT         : {BUILD_COMMIT}");
Information ($"BUILD_NUMBER         : {BUILD_NUMBER}");
Information ($"BUILD_TIMESTAMP      : {BUILD_TIMESTAMP}");

Task ("packages")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget");

Task ("full-run")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget")
    .IsDependentOn ("samples-dotnet")
    .IsDependentOn ("tools-executive-order");

Task ("ci")
    .IsDependentOn ("ci-build")
    .IsDependentOn ("ci-samples");

// Builds packages but does not run samples
Task ("ci-build")
    .IsDependentOn ("inject-variables")
    .IsDependentOn ("binderate")
    .IsDependentOn ("nuget")
    .IsDependentOn ("tools-executive-order");

// for local builds, conditionally do the first binderate
if (FileExists ("./generated/AndroidX.sln")) {
    Task ("Default")
        .IsDependentOn ("nuget");
} else {
    Task ("Default")
        .IsDependentOn ("binderate")
        .IsDependentOn ("nuget");
}

RunTarget (TARGET);
