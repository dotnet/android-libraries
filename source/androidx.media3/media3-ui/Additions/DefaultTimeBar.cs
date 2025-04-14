using System;
using System.Runtime.InteropServices;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.UI;

// These are needed until this hand generated code is updated to use the new generator pattern
#if NET10_0_OR_GREATER
[UnmanagedFunctionPointer (CallingConvention.Winapi)]
delegate void _JniMarshal_PPZ_V (nint jnienv, nint klass, bool p0);
#endif

public partial class DefaultTimeBar
{
	// This method cannot be bound because it has a base type method of the same
	// name that has been changed to a property.  We need "SetEnabled" to match
	// an implemented interface name.
	static Delegate? cb_setEnabled_Z;

#pragma warning disable 0169
	static Delegate GetSetEnabled_ZHandler ()
	{
		if (cb_setEnabled_Z == null)
			cb_setEnabled_Z = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPZ_V (n_SetEnabled_Z));
		return cb_setEnabled_Z;
	}

	static void n_SetEnabled_Z (IntPtr jnienv, IntPtr native__this, bool enabled)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.UI.DefaultTimeBar> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		__this.SetEnabled (enabled);
	}
#pragma warning restore 0169

	// Metadata.xml XPath method reference: path="/api/package[@name='androidx.media3.ui']/class[@name='DefaultTimeBar']/method[@name='setDuration' and count(parameter)=1 and parameter[1][@type='long']]"
	[Register ("setEnabled", "(Z)V", "GetSetEnabled_ZHandler")]
	public virtual unsafe void SetEnabled (bool enabled)
	{
		const string __id = "setEnabled.(Z)V";
		try {
			JniArgumentValue* __args = stackalloc JniArgumentValue [1];
			__args [0] = new JniArgumentValue (enabled);
			_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
		} finally {
		}
	}

}