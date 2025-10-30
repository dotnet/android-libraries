namespace AndroidX.Heifwriter;

public partial class AvifEncoder
{
    public partial class Builder
    {
        // Fix for CS0426: The type name 'EncoderCallback' does not exist in the type 'EncoderBase'
        // The callback should use the full type name
        public unsafe Builder SetCallback(EncoderBase.EncoderCallback? callback)
        {
            return SetCallback((global::AndroidX.Heifwriter.EncoderBase.IEncoderCallback?)callback);
        }
    }
}
