namespace KotlinX.Serialization.Protobuf.Internal;

public partial class ProtoUnknownFieldHolderSerializer
{
	global::Java.Lang.Object? global::KotlinX.Serialization.IDeserializationStrategy.Deserialize (global::KotlinX.Serialization.Encoding.IDecoder decoder)
		=> Deserialize (decoder);

	void global::KotlinX.Serialization.ISerializationStrategy.Serialize (global::KotlinX.Serialization.Encoding.IEncoder encoder, global::Java.Lang.Object? value)
		=> Serialize (encoder, (global::KotlinX.Serialization.Protobuf.ProtoUnknownFieldHolder) value!);
}
