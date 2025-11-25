namespace AndroidX.DataStore.Preferences.Core;

public partial class PreferencesFileSerializer
{
    global::Java.Lang.Object? global::AndroidX.DataStore.Core.ISerializer.DefaultValue => DefaultValue;

    global::Java.Lang.Object? global::AndroidX.DataStore.Core.ISerializer.WriteTo(global::Java.Lang.Object? t, global::System.IO.Stream output, global::Kotlin.Coroutines.IContinuation p2)
    {
        return WriteTo(t, output, p2);
    }
}
