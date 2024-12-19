using System;
using Android.Runtime;
using Java.Interop;
using Java.Lang;

namespace AndroidX.Activity.Result.Contract 
{
	// Metadata.xml XPath class reference: path="/api/package[@name='androidx.activity.result.contract']/class[@name='ActivityResultContracts']"
	//[global::Android.Runtime.Register ("androidx/activity/result/contract/ActivityResultContracts", DoNotGenerateAcw=true)]
	public sealed partial class ActivityResultContracts // : global::Java.Lang.Object 
    {
		public partial class OpenDocument // : global::AndroidX.Activity.Result.Contract.ActivityResultContract 		
        {
			public override unsafe global::Android.Content.Intent CreateIntent (global::Android.Content.Context context, global::Java.Lang.Object input)
			{
				throw new NotImplementedException();
			}

			/*
			// Metadata.xml XPath method reference: path="/api/package[@name='androidx.activity.result.contract']/class[@name='ActivityResultContracts.OpenDocument']/method[@name='createIntent' and count(parameter)=2 and parameter[1][@type='android.content.Context'] and parameter[2][@type='java.lang.String[]']]"
			[Register ("createIntent", "(Landroid/content/Context;[Ljava/lang/String;)Landroid/content/Intent;", "")]
			public override unsafe global::Android.Content.Intent CreateIntent (global::Android.Content.Context context, global::Java.Lang.Object input)
			{
				const string __id = "createIntent.(Landroid/content/Context;[Ljava/lang/String;)Landroid/content/Intent;";
				IntPtr native_input = JNIEnv.NewArray (input);
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [2];
					__args [0] = new JniArgumentValue ((context == null) ? IntPtr.Zero : ((global::Java.Lang.Object) context).Handle);
					__args [1] = new JniArgumentValue (native_input);
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, __args);
					return global::Java.Lang.Object.GetObject<global::Android.Content.Intent> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
				} finally {
					if (input != null) {
						JNIEnv.CopyArray (native_input, input);
						JNIEnv.DeleteLocalRef (native_input);
					}
					global::System.GC.KeepAlive (context);
					global::System.GC.KeepAlive (input);
				}
			}
			*/
        }

		public partial class RequestMultiplePermissions // : global::AndroidX.Activity.Result.Contract.ActivityResultContract 		
        {
			public override unsafe global::Android.Content.Intent CreateIntent (global::Android.Content.Context context, global::Java.Lang.Object input)
			{
				throw new NotImplementedException();
			}

			// Metadata.xml XPath method reference: path="/api/package[@name='androidx.activity.result.contract']/class[@name='ActivityResultContracts.RequestMultiplePermissions']/method[@name='parseResult' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='android.content.Intent']]"
			[Register ("parseResult", "(ILandroid/content/Intent;)Ljava/util/Map;", "")]
			public override unsafe global::Java.Lang.Object ParseResult (int resultCode, global::Android.Content.Intent? intent)
			{
				const string __id = "parseResult.(ILandroid/content/Intent;)Ljava/util/Map;";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [2];
					__args [0] = new JniArgumentValue (resultCode);
					__args [1] = new JniArgumentValue ((intent == null) ? IntPtr.Zero : ((global::Java.Lang.Object) intent).Handle);
					var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
					return 
						(Java.Lang.Object) // manually added
						global::Android.Runtime.JavaDictionary<string, global::Java.Lang.Boolean>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
				} finally {
					global::System.GC.KeepAlive (intent);
				}
			}
		}


		public partial class OpenMultipleDocuments // : global::AndroidX.Activity.Result.Contract.ActivityResultContract 
        {
			public override unsafe global::Android.Content.Intent CreateIntent (global::Android.Content.Context context, global::Java.Lang.Object input)
			{
				throw new NotImplementedException();
			}

			// Metadata.xml XPath method reference: path="/api/package[@name='androidx.activity.result.contract']/class[@name='ActivityResultContracts.OpenMultipleDocuments']/method[@name='parseResult' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='android.content.Intent']]"
			[Register ("parseResult", "(ILandroid/content/Intent;)Ljava/util/List;", "")]
			public override sealed unsafe global::Java.Lang.Object ParseResult (int resultCode, global::Android.Content.Intent? intent)
			{
				const string __id = "parseResult.(ILandroid/content/Intent;)Ljava/util/List;";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [2];
					__args [0] = new JniArgumentValue (resultCode);
					__args [1] = new JniArgumentValue ((intent == null) ? IntPtr.Zero : ((global::Java.Lang.Object) intent).Handle);
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, __args);
					return 
						(Java.Lang.Object) // manually added
						global::Android.Runtime.JavaList<global::Android.Net.Uri>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
				} finally {
					global::System.GC.KeepAlive (intent);
				}
			}
		}

		public partial class PickMultipleVisualMedia //: global::AndroidX.Activity.Result.Contract.ActivityResultContract 
		{
			// Metadata.xml XPath method reference: path="/api/package[@name='androidx.activity.result.contract']/class[@name='ActivityResultContracts.PickMultipleVisualMedia']/method[@name='parseResult' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='android.content.Intent']]"
			[Register ("parseResult", "(ILandroid/content/Intent;)Ljava/util/List;", "")]
			public override sealed unsafe global::Java.Lang.Object ParseResult (int resultCode, global::Android.Content.Intent? intent)
			{
				const string __id = "parseResult.(ILandroid/content/Intent;)Ljava/util/List;";
				try {
					JniArgumentValue* __args = stackalloc JniArgumentValue [2];
					__args [0] = new JniArgumentValue (resultCode);
					__args [1] = new JniArgumentValue ((intent == null) ? IntPtr.Zero : ((global::Java.Lang.Object) intent).Handle);
					var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod (__id, this, __args);
					return 
						(Java.Lang.Object) // manually added						
						global::Android.Runtime.JavaList<global::Android.Net.Uri>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
				} finally {
					global::System.GC.KeepAlive (intent);
				}
			}
		}
	}
}