using System;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Compose.Runtime {

	// MutableFloatState extends both FloatState (read-only Value) and MutableState (read/write Value).
	// The generated Invoker only implements the read-only Value getter, so IMutableState.Value.set
	// is left unimplemented (CS0535). Supply it here by forwarding to MutableState.setValue(Object).
	partial class IMutableFloatStateInvoker {

		global::Java.Lang.Object? global::AndroidX.Compose.Runtime.IMutableState.Value {
			get => Value;
			set => SetMutableStateValue (value);
		}

		unsafe void SetMutableStateValue (global::Java.Lang.Object? value)
		{
			const string __id = "setValue.(Ljava/lang/Object;)V";
			IntPtr native_value = JNIEnv.ToLocalJniHandle (value);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_value);
				_members_androidx_compose_runtime_MutableState.InstanceMethods.InvokeAbstractVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_value);
				global::System.GC.KeepAlive (value);
			}
		}
	}
}
