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

For PR descriptions, run:

```bash
dotnet cake utilities.cake -t=api-diff-markdown-info-pr
```

This outputs a bunch of lines like this, you can copy/paste into your PR description:

```md
========================================
api-diff-markdown-info-pr
========================================
   1. `androidx.appcompat:appcompat` - 1.7.0 -> 1.7.1
   2. `androidx.appcompat:appcompatresources` - 1.7.0 -> 1.7.1
   3. `androidx.autofill:autofill` - 1.1.0 -> 1.3.0
   4. `androidx.compose.animation:animation` - 1.7.8 -> 1.8.3
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

## Troubleshooting

This is an example list of problems and how to fix them.

### Example 1

```log
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

### Example 2

```log
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(136,44): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(151,42): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(166,44): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(181,25): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(196,45): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
generated\androidx.wear.protolayout.protolayout-expression\obj\Release\net8.0-android\generated\src\AndroidX.Wear.ProtoLayout.Expression.DynamicDataMap.cs(211,44): error CS0111: Type 'DynamicDataMap' already defines a member called 'Get' with the same parameter types
```

Java you can have multiple methods with the same name, parameters, and *different* return type. C# does not allow this.

So, in this case, I renamed the five methods in C#, so for example:

```csharp
// Metadata.xml XPath method reference: path="/api/package[@name='androidx.wear.protolayout.expression']/class[@name='DynamicDataMap']/method[@name='get' and count(parameter)=1 and parameter[1][@type='androidx.wear.protolayout.expression.DynamicDataKey&lt;androidx.wear.protolayout.expression.DynamicBuilders.DynamicBool&gt;']]"
[Register ("get", "(Landroidx/wear/protolayout/expression/DynamicDataKey;)Ljava/lang/Boolean;", "")]
public unsafe global::Java.Lang.Boolean? Get (global::AndroidX.Wear.ProtoLayout.Expression.DynamicDataKey key)
{
    const string __id = "get.(Landroidx/wear/protolayout/expression/DynamicDataKey;)Ljava/lang/Boolean;";
    try {
        JniArgumentValue* __args = stackalloc JniArgumentValue [1];
        __args [0] = new JniArgumentValue ((key == null) ? IntPtr.Zero : ((global::Java.Lang.Object) key).Handle);
        var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, __args);
        return global::Java.Lang.Object.GetObject<global::Java.Lang.Boolean> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
    } finally {
        global::System.GC.KeepAlive (key);
    }
}
```

You can rename this to `GetBoolean` in `source\androidx.wear.protolayout\protolayout-expression\Transforms\Metadata.xml`, such as:

```xml
<attr
    path="/api/package[@name='androidx.wear.protolayout.expression']/class[@name='DynamicDataMap']/method[@name='get' and count(parameter)=1 and parameter[1][@type='androidx.wear.protolayout.expression.DynamicDataKey&lt;androidx.wear.protolayout.expression.DynamicBuilders.DynamicBool&gt;']]"
    name="managedName"
    >
    GetBoolean
</attr>
```

I did this for all five methods.

### Example 3

```log
generated\com.google.android.libraries.places.places\obj\Release\net8.0-android\generated\src\Xamarin.GoogleAndroid.Libraries.Places.Widget.Listener.IPredictionSelectionListener.cs(136,10): error CS0111: Type 'ErrorEventArgs' already defines a member called 'ErrorEventArgs' with the same parameter types
```

This means that more than C# `EventArgs` type is being generated for
two Java callback methods with the same name, in the same namespace.

To solve this, I added to `source\com.google.android.libraries.places\places\Transforms\Metadata.xml`:

```xml
<attr
    path="/api/package[@name='com.google.android.libraries.places.widget.listener']/interface[@name='PredictionSelectionListener']/method[@name='onError' and count(parameter)=1 and parameter[1][@type='com.google.android.gms.common.api.Status']]"
    name="argsType"
    >
    PredictionSelectionEventArgs
</attr>
```

### Example 4

```log
generated\io.grpc.grpc-api\obj\Debug\net8.0-android\generated\src\Xamarin.Grpc.LongGaugeMetricInstrument.cs(20,84):
error CS0738: 'LongGaugeMetricInstrument' does not implement interface member 'IMetricInstrument.OptionalLabelKeys'. 'LongGaugeMetricInstrument.OptionalLabelKeys' cannot implement 'IMetricInstrument.OptionalLabelKeys' because it does not have the matching return type of 'IList<string>'.
generated\io.grpc.grpc-api\obj\Debug\net8.0-android\generated\src\Xamarin.Grpc.LongGaugeMetricInstrument.cs(20,84):
error CS0738: 'LongGaugeMetricInstrument' does not implement interface member 'IMetricInstrument.RequiredLabelKeys'. 'LongGaugeMetricInstrument.RequiredLabelKeys' cannot implement 'IMetricInstrument.RequiredLabelKeys' because it does not have the matching return type of 'IList<string>'.
```

At first, I thought I could do something like:

```xml
<attr
    path="/api/package[@name='io.grpc']/class[@name='LongGaugeMetricInstrument']/method[@name='getOptionalLabelKeys' and count(parameter)=0]"
    name="managedReturn"
    >
    System.Collections.IList&gt;System.String&lt;
</attr>
```

But this results in the error:

```log
BINDINGSGENERATOR : error BG0000: System.ArgumentOutOfRangeException: length ('-15') must be a non-negative value. (Parameter 'length')
```

So, I'm not sure we support putting generic types in the `managedReturn` value.

So instead, I used a C# additions and an explicit interface implementation:

```csharp
using System.Collections.Generic;

namespace Xamarin.Grpc;

public partial class LongGaugeMetricInstrument
{
    IList<string>? IMetricInstrument.OptionalLabelKeys => (IList<string>?) OptionalLabelKeys;

    IList<string>? IMetricInstrument.RequiredLabelKeys => (IList<string>?) RequiredLabelKeys;
}
```

[1118]: https://github.com/dotnet/android-libraries/pull/1118
[androidx.security]: https://github.com/dotnet/android-libraries/tree/androidx.security
[androidx-pipeline]: https://devdiv.visualstudio.com/DevDiv/_build?definitionId=25324