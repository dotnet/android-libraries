# Xamarin.AndroidX.Compose.Material3Android

This binding ships ~265 real Material3 types (`MaterialTheme`, `ColorScheme`,
`Typography`, `Shapes`, `ButtonDefaults`, `TextFieldDefaults`, ...) plus every
`Get*Color` / `Get*Elevation` / `*Defaults` getter that takes
`(IComposer?, int)`.

## Known gap: `@Composable` functions are not callable from C#

The actual `@Composable` entry points on the `*Kt` classes — `ButtonKt.Button`,
`TextKt.Text`, `CardKt.Card`, `ScaffoldKt.Scaffold`, ... — are **not**
surfaced as C# methods.

### Why

Every `@Composable` overload has at least one inline-class parameter
(`Color` → `long`, `Dp` → `float`, `TextStyle`, `ButtonColors`,
`PaddingValues` with inline `Dp` insets, ...). The Kotlin compiler
hash-mangles the JVM method name to encode the inline-class signature, e.g.:

- `Text--4IGK_g(String, Modifier, long, long, FontStyle, ...)`
- `Text-fLXpl1I(...)`
- `Button-LPr_se0(...)`

`class-parse` emits these mangled names verbatim into `api.xml.class-parse`,
but the binding generator drops them in the `api.xml.fixed` stage because `-`
is not a legal C# identifier character. This is not something this repo's
`Metadata.xml` can fix — the strip happens inside the generator, before
metadata transforms run.

### Tracking

Generator-side demangling of `name-<hash>` siblings (and projecting
inline-class parameters as their underlying primitive types) is tracked in
[dotnet/java-interop#1431][java-interop-1431]. Once that lands, this binding
will surface the `@Composable` entry points natively and no changes will be
needed in this repo.

[java-interop-1431]: https://github.com/dotnet/java-interop/issues/1431

### Workaround today

Until the generator change ships, the only way to invoke a Material3
`@Composable` from C# is a hand-written raw-JNI call. Compose's `$default`
bitmask convention (set bit *n* → "use the default for parameter *n*") lets
callers pass `null` / `0` for the inline-class parameters and have the
function pick defaults. A working reference for `Text` and `Button` lives in
[`jonathanpeppers/compose-net`][compose-net]'s
[`ComposeBridges.cs`][compose-bridges].

[compose-net]: https://github.com/jonathanpeppers/compose-net
[compose-bridges]: https://github.com/jonathanpeppers/compose-net/blob/d1c6a5ecd3b02f624c2e5e4655a93b36126277b4/src/ComposeNet.Compose/ComposeBridges.cs

### History

- The legacy package-wide `<remove-node path="/api/package" />` strip was
  removed in [#1418][pr-1418] (real `Compose.Runtime` bindings), at which
  point Material3's ~265 types and `(IComposer?, int)` getters started
  surfacing.
- This README and the surrounding investigation are tracked in
  [#1417][issue-1417].

[pr-1418]: https://github.com/dotnet/android-libraries/pull/1418
[issue-1417]: https://github.com/dotnet/android-libraries/issues/1417
