# Copilot Instructions for .NET Android Libraries Repository

**Note**: Update this file with new learnings to keep suggestions accurate.

## Repository Overview

Creates .NET for Android bindings for Google's Java/Kotlin libraries (AndroidX, Play Services, Firebase, ML Kit, etc.) using a **config-driven approach** with `config.json` and the "binderator" tool.

## Key Files & Directories
- **`config.json`**: Master config for 600+ Maven artifacts → NuGet packages. Update via `dotnet cake --target=update-config` or `bump-config`, not manually.
- **`build.cake`**: Main Cake build script
- **`source/{groupId}/{artifactId}/`**: Binding customizations (Additions/, Transforms/, Metadata.xml)
- **`generated/`**: Auto-generated projects (not in source control)
- **`source/AssemblyInfo.cs`**: **DO NOT EDIT** - contains placeholder tokens

## Target Frameworks
- `net9.0-android` (API 35) and `net10.0-android` (API 36)
- Legacy Xamarin.Android support ended May 1, 2024

## Common Build Commands
```bash
dotnet cake                        # Full build
dotnet cake -t=ci                  # CI build
dotnet cake --target=libs          # Build libraries only
dotnet cake --target=binderate     # Regenerate projects from config
dotnet cake --target=update-config # Update to latest Maven versions
dotnet cake --target=bump-config   # Increment NuGet revision (4th component)
```

## config.json Entry Structure
```json
{
  "groupId": "androidx.activity",
  "artifactId": "activity", 
  "version": "1.10.1",           // Maven version
  "nugetVersion": "1.10.1.2",    // 4th component for NuGet-only changes
  "nugetId": "Xamarin.AndroidX.Activity",
  "dependencyOnly": false        // true = transitive dep, no standalone package
}
```

## Binding Customizations (in `source/{groupId}/{artifactId}/`)
- **`Additions/*.cs`**: Extra C# code added to bindings
- **`Metadata.xml`**: Fix parameter names, remove problematic APIs
- **`Transforms/`**: XML transforms for generated API
- **`merge.targets`**: Custom MSBuild targets

### Common Metadata Patterns
```xml
<!-- Remove problematic API -->
<remove-node path="/api/package[@name='com.example']/class[@name='Problematic']" />

<!-- Fix parameter name -->
<attr path="//method[@name='example']/parameter[@name='p0']" name="name">properName</attr>

<!-- Change managed namespace -->
<attr path="/api/package[@name='androidx.example']" name="managedName">AndroidX.Example</attr>
```

Reference: https://github.com/dotnet/java-interop/wiki/Troubleshooting-Android-Bindings-Issues

## Common Issues and Fixes

### CS0535: Interface Implementation Mismatch
When Java method overloading doesn't map to C# interface requirements:
```csharp
// Add in source/{groupId}/{artifactId}/Additions/ClassName.cs
namespace PackageName;
public partial class ClassName
{
    public unsafe void Method(IEncoder encoder, Java.Lang.Object? value)
        => this.Method(encoder, (SpecificType?) value);
}
```

### Build File Locking Errors (XARLP7024)
Transient parallel build issue. Usually resolves on retry. If persistent, may need to reduce build parallelism in `build/cake/build-and-package.cake`.

### Identifying Failing Package from Build Logs
Look for the `.csproj` path in error messages:
```
[C:\...\generated\{groupId}.{artifactId}\{groupId}.{artifactId}.csproj::TargetFramework=net10.0-android36.0]
```
The `{groupId}.{artifactId}` maps to the Maven coordinates and customization folder.

## Validation Commands
```bash
dotnet cake utilities.cake -t=namespace-check
dotnet cake utilities.cake -t=spell-check  
dotnet cake --target=metadata-verify
dotnet cake --target=binderate-config-verify
```

## Internal Documentation
- **`BUILDING.md`**: Full build prerequisites and setup
- **`docs/development-tips.md`**: Workflow tips, release procedures
- **`docs/artifact-list.md`**: Complete Maven → NuGet mappings

## External Resources
- [.NET for Android Docs](https://learn.microsoft.com/en-us/dotnet/android/)
- [AndroidX Release Notes](https://developer.android.com/jetpack/androidx/versions/stable-channel)
- [Google Play Services Release Notes](https://developers.google.com/android/guides/releases)
- [Java Interop Troubleshooting](https://github.com/dotnet/java-interop/wiki/Troubleshooting-Android-Bindings-Issues)
