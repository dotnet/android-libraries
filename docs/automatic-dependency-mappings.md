# Automatic Dependency Mappings

The binderator includes automatic dependency mapping functionality to handle artifact migrations and ensure compatible versions are used.

## Overview

When processing Maven dependencies, the binderator automatically applies predefined mappings to handle cases where:

- Artifacts have been migrated to new group/artifact IDs
- Minimum version requirements need to be enforced
- Legacy dependencies need to be upgraded to modern equivalents

## Current Mappings

The automatic mappings are defined in a dictionary at the top of `BindingProjectDependencyVerifier.cs`:

```csharp
static readonly Dictionary<string, (string GroupId, string ArtifactId, string MinimumVersion)> Remappings = new()
{
    ["com.squareup.okhttp:okhttp"] = ("com.squareup.okhttp3", "okhttp", "3.4.0"),
};
```

### OkHttp Migration

| Original | Mapped To | Minimum Version | Reason |
|---|---|---|---|
| `com.squareup.okhttp:okhttp` | `com.squareup.okhttp3:okhttp` | `3.4.0` | OkHttp 2.x is deprecated, OkHttp3 3.4.0+ is required for compatibility |

## Real-World Example: Google Places Library

The Google Places library (com.google.android.libraries.places:places:4.4.1) provides a real-world example of automatic mapping in action. This library depends on the legacy OkHttp 2.7.2, which gets automatically remapped to OkHttp3.

### Original POM File

Here's the complete POM file for Google Places 4.4.1, showing the legacy OkHttp dependency:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<project xmlns="http://maven.apache.org/POM/4.0.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 http://maven.apache.org/xsd/maven-4.0.0.xsd">
  <modelVersion>4.0.0</modelVersion>
  <groupId>com.google.android.libraries.places</groupId>
  <artifactId>places</artifactId>
  <version>4.4.1</version>
  <packaging>aar</packaging>
  <dependencies>
    <dependency>
      <groupId>androidx.appcompat</groupId>
      <artifactId>appcompat</artifactId>
      <version>1.0.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.cardview</groupId>
      <artifactId>cardview</artifactId>
      <version>1.0.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.constraintlayout</groupId>
      <artifactId>constraintlayout</artifactId>
      <version>2.1.4</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.exifinterface</groupId>
      <artifactId>exifinterface</artifactId>
      <version>1.0.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.fragment</groupId>
      <artifactId>fragment</artifactId>
      <version>1.1.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.lifecycle</groupId>
      <artifactId>lifecycle-runtime-ktx</artifactId>
      <version>2.8.7</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.lifecycle</groupId>
      <artifactId>lifecycle-viewmodel</artifactId>
      <version>2.0.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.lifecycle</groupId>
      <artifactId>lifecycle-viewmodel-ktx</artifactId>
      <version>2.8.7</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.recyclerview</groupId>
      <artifactId>recyclerview</artifactId>
      <version>1.3.2</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>androidx.viewpager2</groupId>
      <artifactId>viewpager2</artifactId>
      <version>1.1.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.android.volley</groupId>
      <artifactId>volley</artifactId>
      <version>1.2.1</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.github.bumptech.glide</groupId>
      <artifactId>glide</artifactId>
      <version>4.11.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.datatransport</groupId>
      <artifactId>transport-api</artifactId>
      <version>3.1.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.datatransport</groupId>
      <artifactId>transport-backend-cct</artifactId>
      <version>3.2.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.datatransport</groupId>
      <artifactId>transport-runtime</artifactId>
      <version>3.2.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.gms</groupId>
      <artifactId>play-services-base</artifactId>
      <version>18.5.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.gms</groupId>
      <artifactId>play-services-basement</artifactId>
      <version>18.4.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.gms</groupId>
      <artifactId>play-services-location</artifactId>
      <version>21.0.1</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.gms</groupId>
      <artifactId>play-services-maps</artifactId>
      <version>17.0.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.gms</groupId>
      <artifactId>play-services-tasks</artifactId>
      <version>18.2.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.android.material</groupId>
      <artifactId>material</artifactId>
      <version>1.12.0</version>
      <scope>compile</scope>
      <type>aar</type>
    </dependency>
    <dependency>
      <groupId>com.google.auto.value</groupId>
      <artifactId>auto-value-annotations</artifactId>
      <version>1.6.2</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>com.google.code.gson</groupId>
      <artifactId>gson</artifactId>
      <version>2.10</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>com.google.guava</groupId>
      <artifactId>guava</artifactId>
      <version>33.3.0-android</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>com.squareup.okhttp</groupId>
      <artifactId>okhttp</artifactId>
      <version>2.7.2</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>org.jetbrains.kotlin</groupId>
      <artifactId>kotlin-stdlib</artifactId>
      <version>2.1.0</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>org.jetbrains.kotlinx</groupId>
      <artifactId>kotlinx-coroutines-android</artifactId>
      <version>1.8.0</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>org.jetbrains.kotlinx</groupId>
      <artifactId>kotlinx-coroutines-core</artifactId>
      <version>1.8.0</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
    <dependency>
      <groupId>org.jetbrains.kotlinx</groupId>
      <artifactId>kotlinx-coroutines-play-services</artifactId>
      <version>1.8.0</version>
      <scope>compile</scope>
      <type>jar</type>
    </dependency>
  </dependencies>
  <name>places</name>
  <licenses>
    <license>
      <name>Google Maps Platform Terms of Service</name>
      <url>https://cloud.google.com/maps-platform/terms/</url>
      <distribution>repo</distribution>
    </license>
  </licenses>
</project>
```

### Automatic Transformation

When the binderator processes this POM file, the automatic mapping system:

1. **Detects the legacy dependency**: `com.squareup.okhttp:okhttp:2.7.2`
2. **Applies the mapping**: Transforms it to `com.squareup.okhttp3:okhttp:3.4.0` (since 2.7.2 < 3.4.0)
3. **Updates references**: All generated binding projects reference the modern OkHttp3 NuGet package instead
4. **Ensures compatibility**: The minimum version 3.4.0 provides the necessary API compatibility

This happens transparently without requiring any manual configuration changes to support Google Places library bindings.

## How It Works

1. **Dependency Processing**: During dependency verification, each Maven dependency is checked against the mapping dictionary
2. **Key Matching**: Dependencies are matched using the format `"groupId:artifactId"`
3. **Automatic Transformation**: When a match is found:
   - `groupId` and `artifactId` are updated to the new values
   - Version handling is applied based on the minimum version requirement
4. **Version Logic**:
   - No version specified → Use minimum version
   - Version below minimum → Upgrade to minimum version  
   - Version at/above minimum → Keep specified version
   - Version ranges → Adjust lower bound to meet minimum (e.g., `[2.0.0,5.0.0)` → `[3.4.0,5.0.0)` if minimum is `3.4.0`)

## Adding New Mappings

To add a new automatic mapping:

1. **Edit the Dictionary**: Open `BindingProjectDependencyVerifier.cs` and add a new entry to the `Remappings` dictionary:

```csharp
static readonly Dictionary<string, (string GroupId, string ArtifactId, string MinimumVersion)> Remappings = new()
{
    ["com.squareup.okhttp:okhttp"] = ("com.squareup.okhttp3", "okhttp", "3.4.0"),
    ["old.group:old-artifact"] = ("new.group", "new-artifact", "1.0.0"),
};
```

1. **Key Format**: Use `"groupId:artifactId"` as the dictionary key
1. **Value Format**: Provide a tuple with `(newGroupId, newArtifactId, minimumVersion)`
1. **Test**: Run the binderator tests to ensure the mapping works correctly

### Example Transformation

**Input dependency:**

```xml
<dependency>
    <groupId>com.squareup.okhttp</groupId>
    <artifactId>okhttp</artifactId>
    <version>2.7.5</version>
</dependency>
```

**After automatic mapping:**

```xml
<dependency>
    <groupId>com.squareup.okhttp3</groupId>
    <artifactId>okhttp</artifactId>
    <version>3.4.0</version>
</dependency>
```

## Version Handling Examples

| Input Version | Minimum Required | Output Version | Reason |
|---|---|---|---|
| `null` | `3.4.0` | `3.4.0` | No version → use minimum |
| `"2.7.5"` | `3.4.0` | `3.4.0` | Below minimum → upgrade |
| `"4.9.0"` | `3.4.0` | `4.9.0` | Above minimum → preserve |
| `"[2.0.0,5.0.0)"` | `3.4.0` | `"[3.4.0,5.0.0)"` | Range lower bound below minimum → adjust |
| `"[3.5.0,5.0.0)"` | `3.4.0` | `"[3.5.0,5.0.0)"` | Range already above minimum → preserve |

## Smart Version Range Handling

The system intelligently handles Maven version ranges by parsing them and adjusting the lower bound when it falls below the minimum required version:

### Supported Range Formats

- `[1.0.0,2.0.0)` - Inclusive lower, exclusive upper
- `(1.0.0,2.0.0]` - Exclusive lower, inclusive upper  
- `[1.0.0,2.0.0]` - Inclusive both bounds
- `(1.0.0,2.0.0)` - Exclusive both bounds

### Range Adjustment Logic

1. **Parse the range** to extract lower bound, upper bound, and bracket types
2. **Compare lower bound** with minimum required version
3. **If lower bound is below minimum**:
   - Replace lower bound with minimum version
   - Change exclusive lower bound `(` to inclusive `[` when setting to minimum
4. **Preserve upper bound** and bracket type unchanged
5. **If parsing fails**, preserve original range as-is for safety

## Configuration Requirements

For automatic mappings to work correctly, the target artifacts must be present in `config.json`. For the OkHttp mapping, ensure that `com.squareup.okhttp3` entries are available:

```json
{
  "groupId": "com.squareup.okhttp3",
  "artifactId": "okhttp",
  "version": "5.1.0",
  "nugetVersion": "5.1.0.1",
  "nugetId": "Square.OkHttp3",
  "dependencyOnly": false
}
```

## Implementation Details

The automatic mapping uses a dictionary-based approach for maintainability and performance:

```json
{
  "groupId": "com.squareup.okhttp3",
  "artifactId": "okhttp",
  "version": "5.1.0",
  "nugetVersion": "5.1.0.1",
  "nugetId": "Square.OkHttp3",
  "dependencyOnly": false
}
```

## Benefits

- **Maintainable**: All mappings defined in one central dictionary
- **Performant**: Dictionary lookup is O(1) for dependency matching
- **Extensible**: Easy to add new mappings without code duplication
- **Type-Safe**: Tuple values provide compile-time checking
- **Automatic Migration**: Reduces manual configuration overhead
- **Version Safety**: Ensures minimum compatible versions
- **Backward Compatibility**: Allows existing projects to use legacy dependency references

## Testing

Run tests with:

```bash
dotnet test util/Xamarin.AndroidBinderator/Xamarin.AndroidBinderator.Tests/
```

The automatic mappings are applied during dependency verification, so existing integration tests will exercise this functionality.

### Adding Tests for New Mappings

When adding new mappings, consider adding unit tests to verify the transformation logic works correctly for various input scenarios (no version, version below minimum, version above minimum, version ranges).
