using System;
using System.Runtime.InteropServices;
using Android.Runtime;
using Java.Interop;

// These are needed until this hand generated code is updated to use the new generator pattern
#if NET10_0_OR_GREATER
[UnmanagedFunctionPointer (CallingConvention.Winapi)]
delegate bool _JniMarshal_PPL_Z (nint jnienv, nint klass, nint p0);
[UnmanagedFunctionPointer (CallingConvention.Winapi)]
delegate bool _JniMarshal_PPIL_Z (nint jnienv, nint klass, int p0, nint p1);
#endif

namespace Xamarin.Protobuf.Lite
{
    
    public partial class LazyStringArrayList
    {
        static Delegate cb_get_I;
#pragma warning disable 0169
        static Delegate GetGet_IHandler()
        {
            if (cb_get_I == null)
                cb_get_I = JNINativeWrapper.CreateDelegate((Func<IntPtr, IntPtr, int, IntPtr>)n_Get_I);
            return cb_get_I;
        }

        static IntPtr n_Get_I(IntPtr jnienv, IntPtr native__this, int index)
        {
            global::Xamarin.Protobuf.Lite.LazyStringArrayList __this = global::Java.Lang.Object.GetObject<global::Xamarin.Protobuf.Lite.LazyStringArrayList>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            return JNIEnv.NewString((string)__this.Get(index));
        }
#pragma warning restore 0169

        static IntPtr id_get_I;
        // Metadata.xml XPath method reference: path="/api/package[@name='com.google.protobuf']/class[@name='LazyStringArrayList']/method[@name='get' and count(parameter)=1 and parameter[1][@type='int']]"
        [Register("get", "(I)Ljava/lang/String;", "GetGet_IHandler")]
        public override unsafe global::Java.Lang.Object Get(int index)
        {
            if (id_get_I == IntPtr.Zero)
                id_get_I = JNIEnv.GetMethodID(class_ref, "get", "(I)Ljava/lang/String;");
            try
            {
                JValue* __args = stackalloc JValue[1];
                __args[0] = new JValue(index);

                if (((object)this).GetType() == ThresholdType)
                    return JNIEnv.GetString(JNIEnv.CallObjectMethod(((global::Java.Lang.Object)this).Handle, id_get_I, __args), JniHandleOwnership.TransferLocalRef);
                else
                    return JNIEnv.GetString(JNIEnv.CallNonvirtualObjectMethod(((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID(ThresholdClass, "get", "(I)Ljava/lang/String;"), __args), JniHandleOwnership.TransferLocalRef);
            }
            finally
            {
            }
        }


		static Delegate cb_addAll_Ljava_util_Collection_;
#pragma warning disable 0169
		static Delegate GetAddAll_Ljava_util_Collection_Handler ()
		{
			if (cb_addAll_Ljava_util_Collection_ == null)
				cb_addAll_Ljava_util_Collection_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_AddAll_Ljava_util_Collection_);
			return cb_addAll_Ljava_util_Collection_;
		}

		static bool n_AddAll_Ljava_util_Collection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_c)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Protobuf.Lite.LazyStringArrayList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var c = global::Android.Runtime.JavaCollection<string>.FromJniHandle (native_c, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.AddAll (c);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.protobuf']/class[@name='LazyStringArrayList']/method[@name='addAll' and count(parameter)=1 and parameter[1][@type='java.util.Collection&lt;? extends java.lang.String&gt;']]"
		[Register ("addAll", "(Ljava/util/Collection;)Z", "GetAddAll_Ljava_util_Collection_Handler")]
		public unsafe bool AddAll (global::System.Collections.Generic.ICollection<string> c)
		{
			const string __id = "addAll.(Ljava/util/Collection;)Z";
			IntPtr native_c = global::Android.Runtime.JavaCollection<string>.ToLocalJniHandle (c);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_c);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				JNIEnv.DeleteLocalRef (native_c);
				global::System.GC.KeepAlive (c);
			}
		}


		static Delegate cb_addAll_ILjava_util_Collection_;
#pragma warning disable 0169
		static Delegate GetAddAll_ILjava_util_Collection_Handler ()
		{
			if (cb_addAll_ILjava_util_Collection_ == null)
				cb_addAll_ILjava_util_Collection_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPIL_Z) n_AddAll_ILjava_util_Collection_);
			return cb_addAll_ILjava_util_Collection_;
		}

		static bool n_AddAll_ILjava_util_Collection_ (IntPtr jnienv, IntPtr native__this, int index, IntPtr native_c)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Xamarin.Protobuf.Lite.LazyStringArrayList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var c = global::Android.Runtime.JavaCollection<string>.FromJniHandle (native_c, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.AddAll (index, c);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.protobuf']/class[@name='LazyStringArrayList']/method[@name='addAll' and count(parameter)=2 and parameter[1][@type='int'] and parameter[2][@type='java.util.Collection&lt;? extends java.lang.String&gt;']]"
		[Register ("addAll", "(ILjava/util/Collection;)Z", "GetAddAll_ILjava_util_Collection_Handler")]
		public unsafe bool AddAll (int index, global::System.Collections.Generic.ICollection<string> c)
		{
			const string __id = "addAll.(ILjava/util/Collection;)Z";
			IntPtr native_c = global::Android.Runtime.JavaCollection<string>.ToLocalJniHandle (c);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (index);
				__args [1] = new JniArgumentValue (native_c);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				JNIEnv.DeleteLocalRef (native_c);
				global::System.GC.KeepAlive (c);
			}
		}
    }
}
