# Publishing signed packages through BAR

The `publish_signed_packages` stage consumes only the `nuget-signed` pipeline artifact produced by the existing Xamarin signing job. It reads each signed `.nupkg` identity and normalized version, queries that exact package in the public `dotnet10` NuGet V3 feed, and creates an Arcade V3 manifest containing only missing packages. Every inspected package and its SHA-256 is recorded in the `SignedPackagePublishingInventory` pipeline artifact.

The manifest uses Arcade's `PackageArtifactModel` with `Category=Package` and `NonShipping=false`. Arcade's .NET 10 public channel maps that combination to the `dotnet10` shipping feed. The stage does not consume or register `output-windows`, and it does not publish to NuGet.org.

## One-time Azure DevOps setup

Publishing requires the following external setup:

1. Authorize the AndroidX pipeline to use the `Darc: Maestro Production` service connection.
2. Add an **Exclusive lock** check to that service connection. The stage sets `lockBehavior: sequential`, so the feed check, BAR registration, and promotion remain in one serialized critical section.
3. For each branch intended to publish (`main` and applicable `release/*` branches), configure its Darc default channel to the intended public .NET 10 channel, and confirm that channel maps shipping `Package` assets to `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet10/nuget/v3/index.json`. Publishing uses Arcade's vendored `publish-using-darc.ps1` to publish through configured default channels. Until a branch's default channel is configured, its builds remain registered in BAR without publishing packages.

The public `dotnet10` feed currently permits anonymous reads. If that policy changes, supply a read token through a secret environment variable and pass its name with `--feed-token-env`; the tool never accepts a token value on its command line.

The stage uses the same real-sign condition as `build/ci/stage-sign-artifacts.yml`: non-PR `release/*` builds and non-scheduled `main` builds. PRs, scheduled builds, public validation, and all test-signed artifacts are excluded.

Publishing tools restore through `build/publishing/NuGet.config`, which adds `dotnet-eng` only for this isolated publishing surface. The repository-wide `NuGet.config` and normal Cake restore sources remain unchanged.

## Validate BAR registration without promotion

For a manual feature-branch run, set `RunBarValidation=true` and `BarValidationSignedBuildId` to a successful non-scheduled `main` or `release/*` AndroidX build containing `nuget-signed`. Validation mode rejects test-signed source builds, registers the filtered real-signed assets in BAR, and omits the channel-promotion task from the compiled job. The validation build therefore remains unassociated with `.NET 10` and cannot publish packages to the `dotnet10` feed.
