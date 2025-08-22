using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

[assembly: AssemblyMetadata ("BUILD_COMMIT",      "DEV")]
[assembly: AssemblyMetadata ("BUILD_NUMBER",    "DEBUG")]
[assembly: AssemblyMetadata ("BUILD_TIMESTAMP", "08/22/2025 15:41:50")]

#if !NETCOREAPP
[assembly: Android.LinkerSafe]
#endif

[assembly: AssemblyMetadata ("IsTrimmable", "True")]
