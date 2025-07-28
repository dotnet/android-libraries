using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

[assembly: AssemblyMetadata ("BUILD_COMMIT",      "{BUILD_COMMIT}")]
[assembly: AssemblyMetadata ("BUILD_NUMBER",    "{BUILD_NUMBER}")]
[assembly: AssemblyMetadata ("BUILD_TIMESTAMP", "{BUILD_TIMESTAMP}")]

#if !NETCOREAPP
[assembly: Android.LinkerSafe]
#endif

[assembly: AssemblyMetadata ("IsTrimmable", "True")]
