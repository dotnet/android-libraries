using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

[assembly: AssemblyMetadata ("BUILD_COMMIT",      "DEV")]
[assembly: AssemblyMetadata ("BUILD_NUMBER",    "DEBUG")]
[assembly: AssemblyMetadata ("BUILD_TIMESTAMP", "07/28/2025 19:38:34")]

#if !NETCOREAPP
[assembly: Android.LinkerSafe]
#endif

[assembly: AssemblyMetadata ("IsTrimmable", "True")]
