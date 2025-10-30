namespace Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

public partial class DescriptorProtos
{
    public partial class FeatureSet : global::Xamarin.Google.Crypto.Tink.Shaded.Protobuf.DescriptorProtos.IFeatureSetOrBuilder
    {
        // Fix for CS0535: 'DescriptorProtos.FeatureSet' does not implement interface member 'DescriptorProtos.IFeatureSetOrBuilder.EnforceNamingStyle'
        public bool EnforceNamingStyle
        {
            get
            {
                // Return a default value or delegate to an existing property if available
                return false;
            }
        }
    }
}
