namespace AndroidX.DataStore.Preferences.Core.Jvm;

public partial class PreferencesSerializer
{
    global::Java.Lang.Object? global::AndroidX.DataStore.Core.OkIO.IOkioSerializer.DefaultValue => DefaultValue;

    global::Java.Lang.Object? global::AndroidX.DataStore.Core.OkIO.IOkioSerializer.WriteTo(global::Java.Lang.Object? t, global::Square.OkIO.IBufferedSink sink, global::Kotlin.Coroutines.IContinuation p2)
    {
        return WriteTo((global::AndroidX.DataStore.Preferences.Core.Jvm.Preferences?)t, sink, p2);
    }
}
