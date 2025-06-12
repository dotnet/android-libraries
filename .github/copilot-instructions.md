# Copilot Instructions for .NET Android Libraries Repository

**Note**: Always update `copilot-instructions.md` with new/relevant information to keep GitHub Copilot suggestions current and accurate.

## Repository Overview

This repository creates and maintains .NET for Android bindings for Google's Java/Kotlin Android libraries, including:
- AndroidX libraries (600+ packages)
- Google Play Services 
- Firebase
- ML Kit
- Various third-party dependencies

The repository uses a **config-driven approach** where all bindings are defined in `config.json` and automatically generated using the "binderator" tool.

## Key Architecture Components

### Core Files
- **`config.json`**: Master configuration containing all 600+ Maven artifacts to bind, their versions, and NuGet package information
- **`build.cake`**: Main Cake build script orchestrating the entire build process
- **`BUILDING.md`**: Comprehensive build instructions and prerequisites


### Directory Structure
- **`source/`**: Contains binding customizations organized by Maven groupId (e.g., `androidx.core`, `com.google.android.gms`) and Razor templates (`.cshtml` files)
- **`util/Xamarin.AndroidBinderator/`**: The "binderator" tool that generates binding projects from config
- **`generated/`**: Auto-generated binding projects (created during build, not in source control)
- **`docs/`**: Additional documentation including artifact lists and development tips

## Build System

### Prerequisites
- a recent .NET SDK
- Cake .NET Tool: `dotnet tool install -g cake.tool`
- Microsoft OpenJDK 11 (JDK-17 not supported)
- Android SDK and `$ANDROID_SDK_ROOT` environment variable
- Optional: `api-tools` for API diffs: `dotnet tool install -g api-tools`

### Common Build Commands
```bash
# Full build of all packages
dotnet cake

# Clean build for CI
dotnet cake -t=ci

# Build specific target
dotnet cake --target=libs
dotnet cake --target=nuget
dotnet cake --target=packages

# Update config to latest Maven versions
dotnet cake --target=update-config

# Bump NuGet package versions (4th component)
dotnet cake --target=bump-config

# Generate API differences
dotnet cake nuget-diff.cake

# Run utilities (governance, mappings, etc.)
dotnet cake utilities.cake
```

## Configuration System

### config.json Structure
**Note**: This file is normally updated using Cake build targets (`update-config`, `bump-config`) rather than manual editing.

Each artifact entry contains:
```json
{
  "groupId": "androidx.activity",
  "artifactId": "activity", 
  "version": "1.10.1",
  "nugetVersion": "1.10.1.2",
  "nugetId": "Xamarin.AndroidX.Activity",
  "dependencyOnly": false  // true for transitive deps only
}
```

### Version Conventions
- **Major.Minor.Patch**: Mirrors the Maven artifact version
- **Revision (4th component)**: Used for NuGet-only updates without Maven changes
- **Pre-release suffixes**: Supported (e.g., "1.0.0.1-alpha05")

## Development Workflow

### Adding New Bindings
1. Add artifact entry to `config.json`
2. Run `dotnet cake --target=binderate` to generate projects
3. Add any necessary customizations in `source/{groupId}/{artifactId}/`
4. Build and test: `dotnet cake --target=libs`

### Updating Existing Bindings
1. Use `dotnet cake --target=update-config` for automatic updates
2. Or manually edit versions in `config.json`
3. Run `dotnet cake --target=binderate-diff` to see changes
4. Build and validate

### Version Bumping
- Use `dotnet cake --target=bump-config` to increment revision numbers
- This updates only non-dependency packages (where `dependencyOnly != true`)

### Release Process
1. Tag commit: `git tag YYYYMMDD-weekly-stable-updates-YYYYMMDD`
2. Push tag: `git push upstream <tag>`
3. Use AndroidX Push NuGet.org pipeline in Azure DevOps

## Binding Customizations

### Per-Artifact Customizations
Located in `source/{groupId}/{artifactId}/`:
- **`Additions/`**: Additional C# code to add to generated bindings
- **`Transforms/`**: XML transforms to modify generated API
- **`Metadata.xml`**: Binding metadata and parameter names
- **`merge.targets`**: Custom MSBuild targets to include

### Common Metadata Patterns
For comprehensive guidance on troubleshooting binding issues, see: https://github.com/dotnet/java-interop/wiki/Troubleshooting-Android-Bindings-Issues

```xml
<!-- Remove problematic APIs -->
<remove-node path="/api/package[@name='com.example']/class[@name='ProblematicClass']" />

<!-- Fix parameter names -->
<attr path="/api/package[@name='com.example']/class[@name='Example']/method[@name='example']/parameter[@name='p0']" name="name">properName</attr>

<!-- Add managed wrapper -->
<add-node path="/api/package[@name='com.example']">
  <class abstract="false" deprecated="not deprecated" final="false" name="ManagedWrapper" static="false" visibility="public">
  </class>
</add-node>
```

## Target Frameworks

### Current Support
- **Primary**: `net8.0-android` (API 21+)
- **Migration**: `net10.0-android` (API 35+) - migration capability exists but not currently enabled
- **Legacy**: Xamarin.Android support ended May 1, 2024

## Code Organization Patterns

### Generated Projects
- Follow pattern: `{groupId}.{artifactId}.csproj`
- Located in `generated/` directory
- Include auto-generated targets files: `{nugetId}.targets`

### NuGet Package Structure
```
lib/
  net8.0-android34.0/
    {assembly}.dll
  net10.0-android36.0/
    {assembly}.dll
build/
  net8.0-android34.0/
    {package}.targets
  net10.0-android36.0/
    {package}.targets
```

## Testing and Validation

### Available Test Targets
- **`all-packages-tests`**: Run test suite on built packages
- **`build-run-all-packages-tests`**: Build then test all packages
- **`api-diff`**: Generate API difference reports
- **`binderate-config-verify`**: Validate config.json format
- **`metadata-verify`**: Check binding metadata

### Quality Checks
- **Namespace verification**: `dotnet cake utilities.cake -t=namespace-check`
- **Spell checking**: `dotnet cake utilities.cake -t=spell-check`
- **Target SDK verification**: `dotnet cake utilities.cake -t=target-sdk-version-check`

## Documentation Resources

### Internal Documentation
- **`BUILDING.md`**: Comprehensive build instructions
- **`docs/development-tips.md`**: Workflow tips and release procedures
- **`docs/artifact-list.md`**: Complete Maven-to-NuGet mappings
- **`.github/CONTRIBUTING.md`**: Contribution guidelines

### External Resources
- [.NET for Android Documentation](https://learn.microsoft.com/en-us/dotnet/android/)
- [AndroidX Release Notes](https://developer.android.com/jetpack/androidx/versions/stable-channel)
- [Google Play Services Release Notes](https://developers.google.com/android/guides/releases)

## Best Practices for Contributors

### Before Making Changes
1. Read `BUILDING.md` for full setup instructions
2. Ensure all prerequisites are installed
3. Run `dotnet cake -t=ci` to verify clean build
4. Check existing issues and PRs for related work

This repository represents a critical piece of the .NET ecosystem for Android development, enabling C# developers to use the full range of modern Android libraries through automatically maintained bindings.