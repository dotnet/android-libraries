# Development Tips

This is a collection of tasks you might want to do in this repository, and how to do them.

## Updating Packages

Initially, run:

```bash
dotnet cake -t:update-config
```

Which will lookup Java Maven dependencies and update the `config.json` file
with the latest versions.

To update other files in the repo, run:

```bash
dotnet cake utilities.cake -t=generate-component-governance
dotnet cake utilities.cake -t=generate-namespace-file
dotnet cake utilities.cake -t=list-artifacts
```

### Troubleshooting

This is an example list of problems and how to fix them.

#### Example 1

```
generated\androidx.webkit.webkit\obj\Debug\net8.0-android\generated\src\AndroidX.WebKit.ChromiumLibBoundary.IWebViewBuilderBoundaryInterface.cs(202,89):
error CS0535: 'WebViewBuilderBoundaryInterfaceConfig' does not implement interface member 'IConsumer.Accept(Object?)'
[generated\androidx.webkit.webkit\androidx.webkit.webkit.csproj]
```

In this case, the `WebViewBuilderBoundaryInterfaceConfig` class has:

```csharp
// Metadata.xml XPath method reference: path="/api/package[@name='org.chromium.support_lib_boundary']/class[@name='WebViewBuilderBoundaryInterface.Config']/method[@name='accept' and count(parameter)=1 and parameter[1][@type='java.util.function.BiConsumer&lt;java.lang.Integer, java.lang.Object&gt;']]"
[Register ("accept", "(Ljava/util/function/BiConsumer;)V", "GetAccept_Ljava_util_function_BiConsumer_Handler")]
public virtual unsafe void Accept (global::Java.Util.Functions.IBiConsumer? chromiumConfig)
```

Java can change the type of parameters on methods in type inheritance, while C# does *not* allow this.

In C#, the `IConsumer` interface requires:

```csharp
public virtual unsafe void Accept (global::Java.Lang.Object? chromiumConfig)
```

To fix this, I added `source\androidx.webkit\webkit\Additions\AndroidX.WebKit.ChromiumLibBoundary.IWebViewBuilderBoundaryInterface.cs`:

```csharp
namespace AndroidX.WebKit.ChromiumLibBoundary;

public partial class WebViewBuilderBoundaryInterfaceConfig
{
    public virtual unsafe void Accept (Java.Lang.Object? chromiumConfig)
    {
        this.Accept ((Java.Util.Functions.IBiConsumer?) chromiumConfig);
    }
}
```

## Tagging and Releasing

When ready to release, tag a commit such as:

```bash
git tag 20250602-weekly-stable-updates-20250602
git push upstream 20250602-weekly-stable-updates-20250602
```

The second date can just be used if you need to push again with the
same set of bindings.

Use the [AndroidX Push NuGet.org pipeline][androidx-pipeline] and
select a specific build via `Resources > AndroidX`.

## Bumping Versions

In the case of [#1118][1118], we introduced `net10.0-android` target
framework to better support trimming and NativeAOT scenarios.

This means that many of the packages *changed*, but the Maven artifact
did not change. The NuGet packages here use the fourth component of
the version number to indicate this, so you can bump the version of
the NuGet package without changing the Maven artifact.

To "bump" the fourth component of the version number, you can run:

```bash
dotnet cake --target=bump-config
```

You should see the changes to `config.json`, noticing that none of the
`dependencyOnly=true` entries have been changed.

## Releasing Preview Packages

.NET MAUI was using version `1.1.0.3-alpha07` of
Xamarin.AndroidX.Security.SecurityCrypto, but main is building
`1.0.0.26`.

The versions of the Java packages are:

* https://mvnrepository.com/artifact/androidx.security/security-crypto/1.0.0
* https://mvnrepository.com/artifact/androidx.security/security-crypto/1.1.0-alpha07

To release a preview package, I created a [androidx.security][androidx.security]
branch with the new version and tagged it (such as `git tag
20250606-androidx.security-20250606`) as mentioned above.

Then I released the package using the [AndroidX Push NuGet.org
pipeline][androidx-pipeline] and selected the appropriate branch by
selecting a specific build via `Resources > AndroidX`.

[1118]: https://github.com/dotnet/android-libraries/pull/1118
[androidx.security]: https://github.com/dotnet/android-libraries/tree/androidx.security
[androidx-pipeline]: https://devdiv.visualstudio.com/DevDiv/_build?definitionId=25324