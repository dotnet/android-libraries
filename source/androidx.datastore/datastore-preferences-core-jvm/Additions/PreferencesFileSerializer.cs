namespace AndroidX.DataStore.Preferences.Core.Jvm;

public partial class PreferencesFileSerializer
{
    global::Java.Lang.Object? global::AndroidX.DataStore.Core.ISerializer.DefaultValue => DefaultValue;

    global::Java.Lang.Object? global::AndroidX.DataStore.Core.ISerializer.WriteTo(global::Java.Lang.Object? t, global::System.IO.Stream output, global::Kotlin.Coroutines.IContinuation p2)
    {
        return WriteTo((global::AndroidX.DataStore.Preferences.Core.Jvm.Preferences?)t, output, p2);
    }
}
