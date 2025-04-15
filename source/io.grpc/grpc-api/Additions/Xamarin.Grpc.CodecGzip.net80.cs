using System;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Runtime;

#if ! NET9_0_OR_GREATER
namespace Xamarin.Grpc 
{

	// Metadata.xml XPath class reference: path="/api/package[@name='io.grpc']/class[@name='Codec.Gzip']"
	//[global::Android.Runtime.Register ("io/grpc/Codec$Gzip", DoNotGenerateAcw=true)]
	public sealed partial class CodecGzip 
    {
 		public unsafe string MessageEncodingCompressor
        {
			// Metadata.xml XPath method reference: path="/api/package[@name='io.grpc']/class[@name='Codec.Gzip']/method[@name='getMessageEncoding' and count(parameter)=0]"
			[Register ("getMessageEncoding", "()Ljava/lang/String;", "")]
			get {
				const string __id = "getMessageEncoding.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod (__id, this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}

 		public unsafe string MessageEncodingDecompressor
        {
			// Metadata.xml XPath method reference: path="/api/package[@name='io.grpc']/class[@name='Codec.Gzip']/method[@name='getMessageEncoding' and count(parameter)=0]"
			[Register ("getMessageEncoding", "()Ljava/lang/String;", "")]
			get {
				const string __id = "getMessageEncoding.()Ljava/lang/String;";
				try {
					var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod (__id, this, null);
					return JNIEnv.GetString (__rm.Handle, JniHandleOwnership.TransferLocalRef);
				} finally {
				}
			}
		}
   }
}
#endif