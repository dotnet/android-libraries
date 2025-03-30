This package provides .NET for Android bindings for the Java library `com.google.guava:listenablefuture`.

## Java Library

| | |
|-|-|
| **Group ID** | com.google.guava |
| **Artifact ID** | listenablefuture |
| **Version** | 1.0 |
| **License(s)** | [The Apache Software License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt) |
| **Description** | &#xA;    Contains Guava&#x27;s com.google.common.util.concurrent.ListenableFuture class,&#xA;    without any of its other classes -- but is also available in a second&#xA;    &quot;version&quot; that omits the class to avoid conflicts with the copy in Guava&#xA;    itself. The idea is:&#xA;&#xA;    - If users want only ListenableFuture, they depend on listenablefuture-1.0.&#xA;&#xA;    - If users want all of Guava, they depend on guava, which, as of Guava&#xA;    27.0, depends on&#xA;    listenablefuture-9999.0-empty-to-avoid-conflict-with-guava. The 9999.0-...&#xA;    version number is enough for some build systems (notably, Gradle) to select&#xA;    that empty artifact over the &quot;real&quot; listenablefuture-1.0 -- avoiding a&#xA;    conflict with the copy of ListenableFuture in guava itself. If users are&#xA;    using an older version of Guava or a build system other than Gradle, they&#xA;    may see class conflicts. If so, they can solve them by manually excluding&#xA;    the listenablefuture artifact or manually forcing their build systems to&#xA;    use 9999.0-....&#xA;   |

## Bindings

These bindings are provided by Microsoft under the [MIT License](https://opensource.org/licenses/MIT). The Java
library may be licensed under a different license as specified above.

To report issues with this package or the bindings, please visit our [GitHub repository](https://aka.ms/android-libraries).

Note that Microsoft only provides support for the bindings. For support on
usage or issues with the Java library itself, please visit the library's project page.
