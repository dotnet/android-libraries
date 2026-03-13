# export-clearlydefined

[Component Governance][cgdocs] is a Microsoft internal DevOps tool which scans
code to find all dependencies, and issues reports if dependencies have legal or
security issues. 

  * [xamarin/AndroidX Component Governance issues][androidx-cg-issues]

There are many reported issues concerning the license of bound
Maven packages, which hilariously wrong, e.g.
`com.google.mlkit:barcode-scanning` 17.3.0 is detected as having a license of
APSL-1.0, and ` com.android.billingclient:billing` 7.1.1 is detected as having
a license of GPL-2.0.  (Just, *hilariously* wrong.)

Unfortunately, the way to fix the Component Governance alerts is to fix the
underlying data source to mention appropriate license info:
[ClearlyDefined.io][clearlydefined-io].  This in turn requires submitting a PR
to [clearlydefined/curated-data][clearlydefined-data].

`util/export-clearlydefined` will add or update the YAML files to contain
`licensed: declared: OTHER` for specific Maven packages declared within
[`cgmanifest.json`](../../cgmanifest.json): Maven packages with group ids
starting with:

  * `com.android`
  * `com.google`

# Usage

To use `export-clearlydefined`:

 1. Checkout the [clearlydefined/curated-data][clearlydefined-data] repo.

    ***Note***: This repo ***must*** be checked out on a *case-sensitive* filesystem.
    (Windows need not apply!  macOS will require creating a new Disk Image with the
    "APFS (Case-sensitive)" file system.)

 2. Use `dotnet run --project util/export-clearlydefined` to update (1).
    `-m` should be the path to the `cgmanifest.json` to process, and
    `-o` should be the path to (1):

        dotnet run --project util/export-clearlydefined -- -m cgmanifest.json -o path/to/(1)

[cgdocs]: https://aka.ms/cgdocs
[androidx-cg-issues]: https://devdiv.visualstudio.com/DevDiv/_componentGovernance/141724?_a=alerts&typeId=22907042&alerts-view-option=active
[clearlydefined-io]: https://clearlydefined.io/
[clearlydefined-data]: https://github.com/clearlydefined/curated-data
