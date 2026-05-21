// Bridge the typed Kotlin overrides on MutableStateSerializer (which return /
// take IMutableState) to the Object-typed IKSerializer interface contract.
// The generator only emits the typed overrides, so without these explicit
// interface implementations the class fails CS0738/CS0535.

namespace AndroidX.SavedState.Compose.Serialization.Serializers;

public partial class MutableStateSerializer
{
	global::Java.Lang.Object? global::KotlinX.Serialization.IDeserializationStrategy.Deserialize (global::KotlinX.Serialization.Encoding.IDecoder decoder)
		=> (global::Java.Lang.Object?) Deserialize (decoder);

	void global::KotlinX.Serialization.ISerializationStrategy.Serialize (global::KotlinX.Serialization.Encoding.IEncoder encoder, global::Java.Lang.Object? value)
		=> Serialize (encoder, (global::AndroidX.Compose.Runtime.IMutableState) value!);
}
