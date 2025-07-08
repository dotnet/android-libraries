namespace KotlinX.Serialization.Builtins;

public partial class InstantComponentSerializer
{
    public unsafe void Serialize (global::KotlinX.Serialization.Encoding.IEncoder encoder, global::Java.Lang.Object? value)
    {
        this.Serialize (encoder, (global::Kotlin.Time.Instant?) value);
    }
}