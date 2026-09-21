using System;

namespace AndroidBinderator;

internal enum MavenRepositoryOperation
{
	Binderation,
	VersionDiscovery,
}

internal static class MavenRepositoryResolver
{
	public const string DotNetPublicMaven = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public-maven/maven/v1";

	public static (MavenRepoType type, string location) Resolve (MavenRepoType type, string location, MavenRepositoryOperation operation)
		=> Resolve (type, location, operation, string.Equals (Environment.GetEnvironmentVariable ("RUNNINGONCI"), "true", StringComparison.OrdinalIgnoreCase));

	internal static (MavenRepoType type, string location) Resolve (MavenRepoType type, string location, MavenRepositoryOperation operation, bool runningOnCI)
		=> runningOnCI && operation == MavenRepositoryOperation.Binderation && type is MavenRepoType.Google or MavenRepoType.MavenCentral
			? (MavenRepoType.Url, DotNetPublicMaven)
			: (type, location);
}
