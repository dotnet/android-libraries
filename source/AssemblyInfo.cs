using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

[assembly: AssemblyMetadata ("BUILD_COMMIT",      "DEV")]
[assembly: AssemblyMetadata ("BUILD_NUMBER",    "DEBUG")]
[assembly: AssemblyMetadata ("BUILD_TIMESTAMP", "10/29/2025 21:56:22")]

#if !NETCOREAPP
[assembly: Android.LinkerSafe]
#endif

[assembly: AssemblyMetadata ("IsTrimmable", "True")]
