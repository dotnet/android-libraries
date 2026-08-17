# Publishing signed packages through BAR

The `publish_signed_packages` stage consumes only the `nuget-signed` pipeline artifact produced by the existing Xamarin signing job. It reads each signed `.nupkg` identity and normalized version, queries that exact package in the public `dotnet10` NuGet V3 feed, and creates an Arcade V3 manifest containing only missing packages. Every inspected package and its SHA-256 is recorded in the `SignedPackagePublishingInventory` pipeline artifact.

The manifest uses Arcade's `PackageArtifactModel` with `Category=Package` and `NonShipping=false`. Arcade's .NET 10 public channel maps that combination to the `dotnet10` shipping feed. The stage does not consume or register `output-windows`, and it does not publish to NuGet.org.

## One-time Azure DevOps setup

Publishing is intentionally disabled by `barPublishingEnabled: false` until all setup below is complete:

1. In the DevDiv project, create the `android-libraries-dotnet10-publishing` environment.
2. Add an **Exclusive lock** check to that environment with one concurrent deployment, and authorize the AndroidX pipeline to use it. The stage sets `lockBehavior: sequential`, so the feed check, BAR registration, and promotion remain in one serialized critical section.
3. Authorize the pipeline to use the `Darc: Maestro Production` service connection.
4. Confirm BAR channel `.NET 10` still has ID `5172` and maps shipping `Package` assets to `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet10/nuget/v3/index.json`. The promotion script fails before promotion if the name and ID no longer match.
5. Set `barPublishingEnabled` to `true` in `build/ci/variables.yml`.

The public `dotnet10` feed currently permits anonymous reads. If that policy changes, supply a read token through a secret environment variable and pass its name with `--feed-token-env`; the tool never accepts a token value on its command line.

The stage uses the same real-sign condition as `build/ci/stage-sign-artifacts.yml`: non-PR `release/*` builds and non-scheduled `main` builds. PRs, scheduled builds, public validation, and all test-signed artifacts are excluded.

Publishing tools restore through `build/publishing/NuGet.config`, which adds `dotnet-eng` only for this isolated publishing surface. The repository-wide `NuGet.config` and normal Cake restore sources remain unchanged.
