#! "net7.0"

#r "nuget: Newtonsoft.Json, 13.0.3"

#r "nuget: HolisticWare.Xamarin.Tools.ComponentGovernance, 0.0.1.4"
#r "nuget: HolisticWare.Core.Net.HTTP, 0.0.4"
#r "nuget: HolisticWare.Core.IO, 0.0.4"

// Usage:
//   dotnet tool install -g dotnet-script
/*
	dotnet script ./build/scripts/utilities.csx	
*/

using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HolisticWare.Xamarin.Tools.ComponentGovernance;

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
            ||
            fully_qualified_artifact_id.StartsWith("com.google.android.libraries.ads.mobile.sdk")
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

