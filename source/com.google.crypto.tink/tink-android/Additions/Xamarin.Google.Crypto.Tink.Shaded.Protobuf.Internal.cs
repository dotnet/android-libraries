using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

#if ! NET9_0_OR_GREATER
namespace Xamarin.Google.Crypto.Tink.Shaded.Protobuf 
{
	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.crypto.tink.shaded.protobuf']/class[@name='Internal']"
	// [global::Android.Runtime.Register ("com/google/crypto/tink/shaded/protobuf/Internal", DoNotGenerateAcw=true)]
	public sealed partial class Internal // : global::Java.Lang.Object 
    {
		// Metadata.xml XPath class reference: path="/api/package[@name='com.google.crypto.tink.shaded.protobuf']/class[@name='Internal.MapAdapter']"
		// [global::Android.Runtime.Register ("com/google/crypto/tink/shaded/protobuf/Internal$MapAdapter", DoNotGenerateAcw=true)]
		// [global::Java.Interop.JavaTypeParameters (new string [] {"K", "V", "RealValue"})]
		public partial class MapAdapter // : global::Java.Util.AbstractMap 
        {

			static Delegate? cb_entrySet;
#pragma warning disable 0169
			static Delegate GetEntrySetHandler ()
			{
				if (cb_entrySet == null)
					cb_entrySet = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_L (n_EntrySet));
				return cb_entrySet;
			}

			static IntPtr n_EntrySet (IntPtr jnienv, IntPtr native__this)
			{
				var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Google.Crypto.Tink.Shaded.Protobuf.Internal.MapAdapter> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
				return global::Android.Runtime.JavaSet<global::Java.Util.IMapEntry>.ToLocalJniHandle (__this.EntrySet ());
			}
#pragma warning restore 0169

			// Metadata.xml XPath method reference: path="/api/package[@name='com.google.crypto.tink.shaded.protobuf']/class[@name='Internal.MapAdapter']/method[@name='entrySet' and count(parameter)=0]"
			[Register ("entrySet", "()Ljava/util/Set;", "GetEntrySetHandler")]
			public override unsafe global::System.Collections.ICollection? EntrySet ()
			{
				const string __id = "entrySet.()Ljava/util/Set;";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
					return 
						(System.Collections.ICollection?) // added manually
						global::Android.Runtime.JavaSet<global::Java.Util.IMapEntry>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
        }
    }

}
#endif