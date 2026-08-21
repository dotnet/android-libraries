# Publishing signed packages through BAR

The `publish_signed_packages` stage consumes only the `nuget-signed` pipeline artifact produced by the existing Xamarin signing job. It reads each signed `.nupkg` identity and normalized version, stages every package, and creates an Arcade V3 manifest. Every inspected package and its SHA-256 is recorded in the deterministic `SignedPackagePublishingInventory` pipeline artifact.

The manifest sets `BuildIdentity.IsStable=true` and uses Arcade's `PackageArtifactModel` with `Category=Package` and `NonShipping=false`. Arcade V3 therefore creates a repository/commit-specific Azure DevOps feed for stable shipping packages, marks that feed as isolated, and skips the channel's shared shipping feed. Downstream release tooling can gather the resulting BAR drop and select packages for NuGet.org; this stage does not publish directly to NuGet.org. The stage does not consume or register `output-windows`.

## One-time Azure DevOps setup

Publishing requires the following external setup:

1. Authorize the AndroidX pipeline to use the `Darc: Maestro Production` service connection.
2. Add an **Exclusive lock** check to that service connection. The stage sets `lockBehavior: sequential` so BAR registration and promotion remain serialized.
3. Configure the repository's `main` default channel in Darc to the intended public .NET 10 channel. Publishing uses Arcade's vendored `publish-using-darc.ps1` to publish through configured default channels. Until a default channel is configured, builds remain registered in BAR without publishing packages.

The stage uses the same real-sign condition as `build/ci/stage-sign-artifacts.yml`: non-PR `release/*` builds and non-scheduled `main` builds. PRs, scheduled builds, public validation, and all test-signed artifacts are excluded.

Publishing tools restore through `build/publishing/NuGet.config`, which adds `dotnet-eng` only for this isolated publishing surface. The repository-wide `NuGet.config` and normal Cake restore sources remain unchanged.

## Validate BAR registration without promotion

For a manual feature-branch run, set `RunBarValidation=true` and `BarValidationSignedBuildId` to a successful non-scheduled `main` or `release/*` AndroidX build containing `nuget-signed`. Validation mode rejects test-signed source builds, registers all real-signed assets in BAR, and omits the channel-promotion task from the compiled job. The validation build therefore remains unassociated with `.NET 10` and does not publish packages.
