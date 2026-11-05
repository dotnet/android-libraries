---
name: maven-mirroring
description: Seed and audit dotnet-public-maven for Android library update PRs, held or frozen Maven candidates, and CI feed failures. Use when mirroring dependencies or distinguishing feed availability from missing binding/configuration mappings.
---

# Maven mirroring

Run from the repository root using the scripts in `build/gradle`, not ad hoc
audits or Binderator changes. Requires PowerShell 7, authenticated `gh` and Azure
CLI (`azure-devops` extension), `dotnet`, and the `BUILDING.md` prerequisites on
`PATH`. Use `az login` if needed; never print or save tokens.

If the Maven assembly under `tools/` is missing, restore without updating config:

```bash
dotnet tool restore
dotnet cake --target=update-config --dryrun
```

## Audit an update PR

```bash
pwsh build/gradle/mirror-dependencies.ps1 -PullRequest 1572 -ReportPath /tmp/pr-1572-mirror.json
```

Use the requested PR number. Reports default to the system temporary directory;
keep explicit report paths outside the repository. No PR checkout is needed.

The script audits pinned bindings, both sides of the version diff, latest stable
updater candidates (including frozen rows), CI failures, and parent/BOM plus
compile/runtime dependencies absent from config. Binderator runs against a
temporary config; do not change real versions or frozen flags for discovery.
Use upstream versions such as `2.71828`, not NuGet-normalized spellings.

Sources/Javadoc honor `downloadExternals`, `downloadJavaSourceJars`, and
`downloadJavaDocJars` (default true), including baseline settings. Optional
`.module` files are included when external downloads are enabled; Binderator has
no module-specific switch. Genuine POM-only requests need no payload; bindings
with unrecognized packaging use Binderator's JAR/AAR fallback.

Append `-AzureBuildId 1661997` for older failures in `dnceng-public/public`,
`-CiLogPath /path/to/ci.log` for downloaded logs, or `-Repository owner/repo`.
Explicit `-MavenArtifact group:artifact:version[:filename]` and Gradle
`-ProjectDir ... -Task ...` modes remain available for targeted work, not as a
substitute for PR-wide discovery.

## Interpret the report

Read `Complete`, `Errors`, `Scope`, and `MissingMappings`, not just file counts.
Each file is **newly mirrored**, **already accessible**, **unavailable upstream**,
**unresolved**, or (for optional files absent from configured/Google/Central
upstreams) **not published upstream**. Only optional absence is nonfatal.

Published files require anonymous GET verification after seeding. Retries are
bounded; Azure credentials stay on the feed and authenticated redirects are
blocked. Discovery, resolution, authentication, or download failures produce an
incomplete report and nonzero exit.

This is an availability audit, not a build graph: optional dependencies are
included without exclusions/conflict mediation; profiles and Gradle variants are
not evaluated. Profile-dependent properties/dependencies, unresolved versions,
and unsupported packaging remain explicit limits, even if other files succeed.

**Feed availability does not mean CI is fixed.** Missing binding/configuration
mappings are separate, nonfatal findings. `No matching artifact config found` is
not a feed 401. Do not change versions or bindings to make the report green.
