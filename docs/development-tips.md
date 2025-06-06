# Development Tips

This is a collection of tasks you might want to do in this repository, and how to do them.

## Bumping Versions

In the case of [#1118][1118], we introduced `net10.0-android` target
framework to better support trimming and NativeAOT scenarios.

This means that many of the packages *changed*, but the Maven artifact
did not change. The NuGet packages here use the fourth component of
the version number to indicate this, so you can bump the version of
the NuGet package without changing the Maven artifact.

To "bump" the fourth component of the version number, you can run:

```bash
dotnet cake --target=bump-config
```

You should see the changes to `config.json`, noticing that none of the
`dependencyOnly=true` entries have been changed.

[1118]: https://github.com/dotnet/android-libraries/pull/1118