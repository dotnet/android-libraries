using System;
using Android.Runtime;
using Java.Interop;

#if ! NET9_0_OR_GREATER

namespace AndroidX.Leanback.Widget
{
    public partial class BaseCardView
    {
        public override bool Selected {
            get { return base.Selected; }
            set { SetSelected (value); }
        }

        static Delegate cb_setSelected_Z;
#pragma warning disable 0169
        static Delegate GetSetSelected_ZHandler ()
        {
            if (cb_setSelected_Z == null)
                cb_setSelected_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>)n_SetSelected_Z);
            return cb_setSelected_Z;
        }

        static void n_SetSelected_Z (IntPtr jnienv, IntPtr native__this, bool selected)
        {
            global::AndroidX.Leanback.Widget.BaseCardView __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Widget.BaseCardView> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            __this.SetSelected (selected);
        }
#pragma warning restore 0169

        static IntPtr id_setSelected_Z;
        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget']/class[@name='BaseCardView']/method[@name='setSelected' and count(parameter)=1 and parameter[1][@type='boolean']]"
        [Register ("setSelected", "(Z)V", "GetSetSelected_ZHandler")]
        public unsafe void SetSelected (bool selected)
        {
            if (id_setSelected_Z == IntPtr.Zero)
                id_setSelected_Z = JNIEnv.GetMethodID (class_ref, "setSelected", "(Z)V");
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue (selected);

                if (GetType () == ThresholdType)
                    JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_setSelected_Z, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setSelected", "(Z)V"), __args);
            } finally {
            }
        }


        public bool Activate {
            get { return base.Activated; }
            set { SetActivated (value); }
        }

        static Delegate cb_setActivated_Z;
#pragma warning disable 0169
        static Delegate GetSetActivated_ZHandler ()
        {
            if (cb_setActivated_Z == null)
                cb_setActivated_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>)n_SetActivated_Z);
            return cb_setActivated_Z;
        }

        static void n_SetActivated_Z (IntPtr jnienv, IntPtr native__this, bool activated)
        {
            global::AndroidX.Leanback.Widget.BaseCardView __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Widget.BaseCardView> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            __this.SetActivated (activated);
        }
#pragma warning restore 0169

        static IntPtr id_setActivated_Z;
        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget']/class[@name='BaseCardView']/method[@name='setActivated' and count(parameter)=1 and parameter[1][@type='boolean']]"
        [Register ("setActivated", "(Z)V", "GetSetActivated_ZHandler")]
        public unsafe void SetActivated (bool activated)
        {
            if (id_setActivated_Z == IntPtr.Zero)
                id_setActivated_Z = JNIEnv.GetMethodID (class_ref, "setActivated", "(Z)V");
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue (activated);

                if (GetType () == ThresholdType)
                    JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_setActivated_Z, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setActivated", "(Z)V"), __args);
            } finally {
            }
        }
    }


    public partial class MediaNowPlayingView
    {
        public override Android.Views.ViewStates Visibility {
            get { return base.Visibility; }
            set { SetVisibility (value); }
        }

        static Delegate cb_setVisibility_I;
#pragma warning disable 0169
        static Delegate GetSetVisibility_IHandler ()
        {
            if (cb_setVisibility_I == null)
                cb_setVisibility_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>)n_SetVisibility_I);
            return cb_setVisibility_I;
        }

        static void n_SetVisibility_I (IntPtr jnienv, IntPtr native__this, int native_visibility)
        {
            global::AndroidX.Leanback.Widget.MediaNowPlayingView __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Widget.MediaNowPlayingView> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            global::Android.Views.ViewStates visibility = (global::Android.Views.ViewStates)native_visibility;
            __this.SetVisibility (visibility);
        }
#pragma warning restore 0169

        static IntPtr id_setVisibility_I;
        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget']/class[@name='MediaNowPlayingView']/method[@name='setVisibility' and count(parameter)=1 and parameter[1][@type='int']]"
        [Register ("setVisibility", "(I)V", "GetSetVisibility_IHandler")]
        public unsafe void SetVisibility ([global::Android.Runtime.GeneratedEnum] global::Android.Views.ViewStates visibility)
        {
            if (id_setVisibility_I == IntPtr.Zero)
                id_setVisibility_I = JNIEnv.GetMethodID (class_ref, "setVisibility", "(I)V");
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue ((int)visibility);

                if (GetType () == ThresholdType)
                    JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_setVisibility_I, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setVisibility", "(I)V"), __args);
            } finally {
            }
        }
    }

    public partial class SearchBar
    {
        public override int NextFocusDownId {
            get { return base.NextFocusDownId; }
            set { SetNextFocusDownId (value); }
        }

        static Delegate cb_setNextFocusDownId_I;
#pragma warning disable 0169
        static Delegate GetSetNextFocusDownId_IHandler ()
        {
            if (cb_setNextFocusDownId_I == null)
                cb_setNextFocusDownId_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, int>)n_SetNextFocusDownId_I);
            return cb_setNextFocusDownId_I;
        }

        static void n_SetNextFocusDownId_I (IntPtr jnienv, IntPtr native__this, int viewId)
        {
            global::AndroidX.Leanback.Widget.SearchBar __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Widget.SearchBar> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            __this.SetNextFocusDownId (viewId);
        }
#pragma warning restore 0169

        static IntPtr id_setNextFocusDownId_I;
        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget']/class[@name='SearchBar']/method[@name='setNextFocusDownId' and count(parameter)=1 and parameter[1][@type='int']]"
        [Register ("setNextFocusDownId", "(I)V", "GetSetNextFocusDownId_IHandler")]
        public unsafe void SetNextFocusDownId (int viewId)
        {
            if (id_setNextFocusDownId_I == IntPtr.Zero)
                id_setNextFocusDownId_I = JNIEnv.GetMethodID (class_ref, "setNextFocusDownId", "(I)V");
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue (viewId);

                if (GetType () == ThresholdType)
                    JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_setNextFocusDownId_I, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setNextFocusDownId", "(I)V"), __args);
            } finally {
            }
        }
    }

//    public partial class ParallaxRecyclerViewSource
//    {

//        static Delegate cb_createProperty_Ljava_lang_String_I;
//#pragma warning disable 0169
//        static Delegate GetCreateProperty_Ljava_lang_String_IHandler()
//        {
//            if (cb_createProperty_Ljava_lang_String_I == null)
//                cb_createProperty_Ljava_lang_String_I = JNINativeWrapper.CreateDelegate((Func<IntPtr, IntPtr, IntPtr, int, IntPtr>)n_CreateProperty_Ljava_lang_String_I);
//            return cb_createProperty_Ljava_lang_String_I;
//        }

//        static IntPtr n_CreateProperty_Ljava_lang_String_I(IntPtr jnienv, IntPtr native__this, IntPtr native_name, int index)
//        {
//            global::AndroidX.Leanback.Leanback.Widget.ParallaxRecyclerViewSource __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Leanback.Widget.ParallaxRecyclerViewSource>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
//            string name = JNIEnv.GetString(native_name, JniHandleOwnership.DoNotTransfer);
//            IntPtr __ret = JNIEnv.ToLocalJniHandle(__this.CreateProperty(name, index));
//            return __ret;
//        }
//#pragma warning restore 0169

//        static IntPtr id_createProperty_Ljava_lang_String_I;
//        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget']/class[@name='ParallaxRecyclerViewSource']/method[@name='createProperty' and count(parameter)=2 and parameter[1][@type='java.lang.String'] and parameter[2][@type='int']]"
//        [Register("createProperty", "(Ljava/lang/String;I)Landroid/support/v17/leanback/widget/ParallaxRecyclerViewSource$ChildPositionProperty;", "GetCreateProperty_Ljava_lang_String_IHandler")]
//        public override unsafe Java.Lang.Object CreateProperty(string name, int index)
//        {
//            if (id_createProperty_Ljava_lang_String_I == IntPtr.Zero)
//                id_createProperty_Ljava_lang_String_I = JNIEnv.GetMethodID(class_ref, "createProperty", "(Ljava/lang/String;I)Landroid/support/v17/leanback/widget/ParallaxRecyclerViewSource$ChildPositionProperty;");
//            IntPtr native_name = JNIEnv.NewString(name);
//            try
//            {
//                JValue* __args = stackalloc JValue[2];
//                __args[0] = new JValue(native_name);
//                __args[1] = new JValue(index);

//                global::AndroidX.Leanback.Leanback.Widget.ParallaxRecyclerViewSource.ChildPositionProperty __ret;
//                if (GetType() == ThresholdType)
//                    __ret = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Leanback.Widget.ParallaxRecyclerViewSource.ChildPositionProperty>(JNIEnv.CallObjectMethod(((global::Java.Lang.Object)this).Handle, id_createProperty_Ljava_lang_String_I, __args), JniHandleOwnership.TransferLocalRef);
//                else
//                    __ret = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Leanback.Widget.ParallaxRecyclerViewSource.ChildPositionProperty>(JNIEnv.CallNonvirtualObjectMethod(((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID(ThresholdClass, "createProperty", "(Ljava/lang/String;I)Landroid/support/v17/leanback/widget/ParallaxRecyclerViewSource$ChildPositionProperty;"), __args), JniHandleOwnership.TransferLocalRef);
//                return __ret;
//            }
//            finally
//            {
//                JNIEnv.DeleteLocalRef(native_name);
//            }
//        }
//    }


	public partial class RecyclerViewParallax
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='androidX.leanback.leanback.widget']/class[@name='RecyclerViewParallax']/method[@name='createProperty' and count(parameter)=2 and parameter[1][@type='java.lang.String'] and parameter[2][@type='int']]"
		[Register("createProperty", "(Ljava/lang/String;I)Landroid/util/Property;", "GetCreateProperty_Ljava_lang_String_IHandler")]
		public override unsafe Java.Lang.Object CreateProperty(string name, int index)
		{
			// TODO: Implement JNI
			return null;
		}




		//[Register("addProperty", "(Ljava/lang/String;)Landroid/util/Property;", "GetAddProperty_Ljava_lang_String_Handler")]
		//public override unsafe Java.Lang.Object AddProperty(string name)
		//{
		//	// TODO: Implement
		//	return null;
		//}
	}
}

namespace AndroidX.Leanback.App
{
    
}

namespace AndroidX.Leanback.Widget.Picker
{
    public partial class Picker
    {
        public override bool Activated {
            get { return base.Activated; }
            set { SetActivated (value); }
        }
        static Delegate cb_setActivated_Z;
#pragma warning disable 0169
        static Delegate GetSetActivated_ZHandler ()
        {
            if (cb_setActivated_Z == null)
                cb_setActivated_Z = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, bool>)n_SetActivated_Z);
            return cb_setActivated_Z;
        }

        static void n_SetActivated_Z (IntPtr jnienv, IntPtr native__this, bool activated)
        {
            global::AndroidX.Leanback.Widget.Picker.Picker __this = global::Java.Lang.Object.GetObject<global::AndroidX.Leanback.Widget.Picker.Picker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            __this.SetActivated (activated);
        }
#pragma warning restore 0169

        static IntPtr id_setActivated_Z;
        // Metadata.xml XPath method reference: path="/api/package[@name='AndroidX.Leanback.leanback.widget.picker']/class[@name='Picker']/method[@name='setActivated' and count(parameter)=1 and parameter[1][@type='boolean']]"
        [Register ("setActivated", "(Z)V", "GetSetActivated_ZHandler")]
        public unsafe void SetActivated (bool activated)
        {
            if (id_setActivated_Z == IntPtr.Zero)
                id_setActivated_Z = JNIEnv.GetMethodID (class_ref, "setActivated", "(Z)V");
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue (activated);

                if (GetType () == ThresholdType)
                    JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_setActivated_Z, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod (((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "setActivated", "(Z)V"), __args);
            } finally {
            }
        }
    }
}
#endif