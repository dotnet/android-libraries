---
name: maven-mirroring
description: Seed and audit dotnet-public-maven for Android library update PRs, held or frozen Maven candidates, and CI feed failures. Use when mirroring dependencies or distinguishing feed availability from missing binding/configuration mappings.
---

# Maven mirroring

Run commands from the repository root. Use the existing scripts in `build/gradle`;
do not replace them with ad hoc audit scripts or modify Binderator for mirroring.

## Prerequisites

Requires PowerShell 7, `gh`, Azure CLI with the `azure-devops` extension, `dotnet`,
and the prerequisites in `BUILDING.md`. Executables must be on `PATH`.
Authenticate GitHub CLI and Azure CLI with the user's accounts; keep tokens out
of output and files. If Azure authentication is missing, use `az login`.

If the Maven assembly under `tools/` is missing, restore the repository tools
and Cake dependencies without updating config:

```bash
dotnet tool restore
dotnet cake --target=update-config --dryrun
```

## Audit an update PR

```bash
pwsh build/gradle/mirror-dependencies.ps1 -PullRequest 1572 -ReportPath /tmp/pr-1572-mirror.json
```

Use the requested PR number and a report path outside the repository. Without
`-ReportPath`, the report goes to the system temporary directory.

The script reads temporary head/merge-base configs without checking out the PR.
It runs the existing Binderator `update` command on a scratch copy and reads its
latest-stable table, including frozen rows. The audit includes:

- Both sides of the PR's Maven version diff.
- Newer stable updater candidates, including held/frozen packages.
- All pinned binding versions, plus published sources/Javadoc JARs and optional
  Gradle `.module` metadata for pinned/latest bindings.
- Coordinates in GitHub Actions/Azure Pipelines failure diagnostics.
- Parent POMs, imported BOMs, and compile/runtime dependency closure, including
  versions absent from config.

Do not change package versions or frozen flags to discover candidates. Preserve
upstream Maven version strings, such as `2.71828`, rather than NuGet-normalized
spellings. Genuine POM-only requests do not require a payload; configured binding
artifacts with POM or other unrecognized packaging use Binderator's JAR/AAR
fallback. Optional dependencies are included without exclusions or conflict
mediation: this is a conservative availability audit, not a build graph.

Auxiliary downloads honor `downloadExternals`, `downloadJavaSourceJars`, and
`downloadJavaDocJars` (all default to true), including baseline settings for
removed/changed base versions. There is no Binderator `.module` download switch;
module metadata is probed whenever auxiliary downloads are enabled.

To include older failures, append `-AzureBuildId 1661997` for a build in
`dnceng-public/public`, or `-CiLogPath /path/to/ci.log` for downloaded logs.
`-Repository owner/repo` selects another repository.

For targeted work, explicit `-MavenArtifact group:artifact:version[:filename]`
and Gradle `-ProjectDir ... -Task ...` modes remain available. They do not replace
PR-wide discovery for update PRs.

## Interpret the report

Read `Complete`, `Errors`, `Scope`, and `MissingMappings`, not just the download
counts. The report identifies the PR SHA, CI sources, coordinate discovery
reasons, and file statuses: **newly mirrored**, **already accessible**,
**unavailable upstream**, or **unresolved**.
Optional files returning 404 from the configured/Google/Central upstreams are
**not published upstream** and do not fail the audit. Request/authentication
failures and published files that cannot be mirrored or verified still fail.
Azure credentials are restricted to the feed;
authenticated redirects are not followed.

Authenticated seeding must be followed by successful anonymous POM/payload GETs.
Transient HTTP errors and cache propagation have bounded retries. Pending or
unsupported checks, missing logs/updater rows, unmappable versions, unresolved
properties, unsupported profile properties/dependencies or packaging, and failed
downloads produce an incomplete report and nonzero exit. Profile activation is
not evaluated. Report those limits; do not describe a partial audit as successful.

**Feed availability does not mean CI is fixed.** Missing binding/configuration
mappings are reported separately and do not make an otherwise complete
availability audit fail. `No matching artifact config found` is a configuration
error, not a feed 401. Do not hold back packages or add bindings merely to make
the mirror report green; address those changes only within the requested scope.
