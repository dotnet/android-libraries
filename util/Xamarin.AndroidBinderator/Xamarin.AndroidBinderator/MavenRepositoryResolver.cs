using System;

namespace AndroidBinderator;

internal static class MavenRepositoryResolver
{
	public const string DotNetPublicMaven = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public-maven/maven/v1";

	public static (MavenRepoType type, string location) Resolve (MavenRepoType type, string location)
		=> Resolve (type, location, string.Equals (Environment.GetEnvironmentVariable ("RUNNINGONCI"), "true", StringComparison.OrdinalIgnoreCase));

	internal static (MavenRepoType type, string location) Resolve (MavenRepoType type, string location, bool runningOnCI)
		=> runningOnCI && type == MavenRepoType.MavenCentral
			? (MavenRepoType.Url, DotNetPublicMaven)
			: (type, location);
}
