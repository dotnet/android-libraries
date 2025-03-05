using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.ExoPlayer;

delegate void _JniMarshal_PPIIL_V (IntPtr jnienv, IntPtr klass, int p0, int p1, IntPtr p2);

internal partial class IExoPlayerInvoker
{
	// These invokers are generated with IList<global::AndroidX.Media3.Common.MediaItem> instead of IList<MediaItem>
	static Delegate? cb_addMediaItems_AddMediaItems_ILSystem_Collections_Generic_IList_1__V;
#pragma warning disable 0169
	static Delegate GetAddMediaItems_ILSystem_Collections_Generic_IList_1_Handler ()
	{
		if (cb_addMediaItems_AddMediaItems_ILSystem_Collections_Generic_IList_1__V == null)
			cb_addMediaItems_AddMediaItems_ILSystem_Collections_Generic_IList_1__V = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPIL_V (n_AddMediaItems_ILSystem_Collections_Generic_IList_1_));
		return cb_addMediaItems_AddMediaItems_ILSystem_Collections_Generic_IList_1__V;
	}

	static void n_AddMediaItems_ILSystem_Collections_Generic_IList_1_ (IntPtr jnienv, IntPtr native__this, int p0, IntPtr native_p1)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		var p1 = (global::System.Collections.Generic.IList`1?)global::Java.Lang.Object.GetObject<global::System.Collections.Generic.IList`1> (native_p1, JniHandleOwnership.DoNotTransfer);
		__this.AddMediaItems (p0, p1);
	}
#pragma warning restore 0169

	public unsafe void AddMediaItems (int p0, global::System.Collections.Generic.IList`1? p1)
	{
		const string __id = "addMediaItems.(ILSystem/Collections/Generic/IList`1;)V";
		try {
			JniArgumentValue* __args = stackalloc JniArgumentValue [2];
			__args [0] = new JniArgumentValue (p0);
			__args [1] = new JniArgumentValue ((p1 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p1).Handle);
			_members_androidx_media3_common_Player.InstanceMethods.InvokeAbstractVoidMethod (__id, this, __args);
		} finally {
			global::System.GC.KeepAlive (p1);
		}
	}

	static Delegate? cb_addMediaItems_LSystem_Collections_Generic_IList_1_;
#pragma warning disable 0169
	static Delegate GetAddMediaItems_LSystem_Collections_Generic_IList_1_Handler ()
	{
		if (cb_addMediaItems_LSystem_Collections_Generic_IList_1_ == null)
			cb_addMediaItems_LSystem_Collections_Generic_IList_1_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_AddMediaItems_LSystem_Collections_Generic_IList_1_);
		return cb_addMediaItems_LSystem_Collections_Generic_IList_1_;
	}

	static void n_AddMediaItems_LSystem_Collections_Generic_IList_1_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		var p0 = (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>?) global::Java.Lang.Object.GetObject</* global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem> */ Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.AddMediaItems (p0);
	}
#pragma warning restore 0169

	IntPtr id_addMediaItems_LSystem_Collections_Generic_IList_1_;
	public unsafe void AddMediaItems (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>? p0)
	{
		if (id_addMediaItems_LSystem_Collections_Generic_IList_1_ == IntPtr.Zero)
			id_addMediaItems_LSystem_Collections_Generic_IList_1_ = JNIEnv.GetMethodID (class_ref, "addMediaItems", "(LSystem/Collections/Generic/IList<global::AndroidX.Media3.Common.MediaItem>;)V");
		JValue* __args = stackalloc JValue [1];
		__args [0] = new JValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
		JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_addMediaItems_LSystem_Collections_Generic_IList_1_, __args);
	}

	static Delegate? cb_setMediaItems_LSystem_Collections_Generic_IList_1_;
#pragma warning disable 0169
	static Delegate GetSetMediaItems_LSystem_Collections_Generic_IList_1_Handler ()
	{
		if (cb_setMediaItems_LSystem_Collections_Generic_IList_1_ == null)
			cb_setMediaItems_LSystem_Collections_Generic_IList_1_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_SetMediaItems_LSystem_Collections_Generic_IList_1_);
		return cb_setMediaItems_LSystem_Collections_Generic_IList_1_;
	}

	static void n_SetMediaItems_LSystem_Collections_Generic_IList_1_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		var p0 = (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>?) global::Java.Lang.Object.GetObject<Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.SetMediaItems (p0);
	}
#pragma warning restore 0169

	IntPtr id_setMediaItems_LSystem_Collections_Generic_IList_1_;
	public unsafe void SetMediaItems (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>? p0)
	{
		if (id_setMediaItems_LSystem_Collections_Generic_IList_1_ == IntPtr.Zero)
			id_setMediaItems_LSystem_Collections_Generic_IList_1_ = JNIEnv.GetMethodID (class_ref, "setMediaItems", "(LSystem/Collections/Generic/IList<global::AndroidX.Media3.Common.MediaItem>;)V");
		JValue* __args = stackalloc JValue [1];
		__args [0] = new JValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
		JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setMediaItems_LSystem_Collections_Generic_IList_1_, __args);
	}

	static Delegate? cb_setMediaItems_LSystem_Collections_Generic_IList_1_Z;
#pragma warning disable 0169
	static Delegate GetSetMediaItems_LSystem_Collections_Generic_IList_1_ZHandler ()
	{
		if (cb_setMediaItems_LSystem_Collections_Generic_IList_1_Z == null)
			cb_setMediaItems_LSystem_Collections_Generic_IList_1_Z = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLZ_V (n_SetMediaItems_LSystem_Collections_Generic_IList_1_Z));
		return cb_setMediaItems_LSystem_Collections_Generic_IList_1_Z;
	}

	static void n_SetMediaItems_LSystem_Collections_Generic_IList_1_Z (IntPtr jnienv, IntPtr native__this, IntPtr native_p0, bool p1)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		var p0 = (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem> ?)global::Java.Lang.Object.GetObject<Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.SetMediaItems (p0, p1);
	}
#pragma warning restore 0169

	IntPtr id_setMediaItems_LSystem_Collections_Generic_IList_1_Z;
	public unsafe void SetMediaItems (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>? p0, bool p1)
	{
		if (id_setMediaItems_LSystem_Collections_Generic_IList_1_Z == IntPtr.Zero)
			id_setMediaItems_LSystem_Collections_Generic_IList_1_Z = JNIEnv.GetMethodID (class_ref, "setMediaItems", "(LSystem/Collections/Generic/IList<global::AndroidX.Media3.Common.MediaItem>;Z)V");
		JValue* __args = stackalloc JValue [2];
		__args [0] = new JValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
		__args [1] = new JValue (p1);
		JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setMediaItems_LSystem_Collections_Generic_IList_1_Z, __args);
	}

	static Delegate? cb_setMediaItems_LSystem_Collections_Generic_IList_1_IJ;
#pragma warning disable 0169
	static Delegate GetSetMediaItems_LSystem_Collections_Generic_IList_1_IJHandler ()
	{
		if (cb_setMediaItems_LSystem_Collections_Generic_IList_1_IJ == null)
			cb_setMediaItems_LSystem_Collections_Generic_IList_1_IJ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLIJ_V (n_SetMediaItems_LSystem_Collections_Generic_IList_1_IJ));
		return cb_setMediaItems_LSystem_Collections_Generic_IList_1_IJ;
	}

	static void n_SetMediaItems_LSystem_Collections_Generic_IList_1_IJ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0, int p1, long p2)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		var p0 = (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem> ?)global::Java.Lang.Object.GetObject <Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.SetMediaItems (p0, p1, p2);
	}
#pragma warning restore 0169

	IntPtr id_setMediaItems_LSystem_Collections_Generic_IList_1_IJ;
	public unsafe void SetMediaItems (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>? p0, int p1, long p2)
	{
		if (id_setMediaItems_LSystem_Collections_Generic_IList_1_IJ == IntPtr.Zero)
			id_setMediaItems_LSystem_Collections_Generic_IList_1_IJ = JNIEnv.GetMethodID (class_ref, "setMediaItems", "(LSystem/Collections/Generic/IList<global::AndroidX.Media3.Common.MediaItem>;IJ)V");
		JValue* __args = stackalloc JValue [3];
		__args [0] = new JValue ((p0 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p0).Handle);
		__args [1] = new JValue (p1);
		__args [2] = new JValue (p2);
		JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_setMediaItems_LSystem_Collections_Generic_IList_1_IJ, __args);
	}

	static Delegate? cb_getPlayerError2;
#pragma warning disable 0169
	static Delegate GetGetPlayerErrorHandler2 ()
	{
		if (cb_getPlayerError2 == null)
			cb_getPlayerError2 = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_L (n_GetPlayerError2));
		return cb_getPlayerError2;
	}

	static IntPtr n_GetPlayerError2 (IntPtr jnienv, IntPtr native__this)
	{
		var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
		return JNIEnv.ToLocalJniHandle ((__this as global::AndroidX.Media3.Common.IPlayer).PlayerError);
	}
#pragma warning restore 0169

	IntPtr id_getPlayerError2;
	unsafe global::AndroidX.Media3.Common.PlaybackException? global::AndroidX.Media3.Common.IPlayer.PlayerError {
		get {
			if (id_getPlayerError2 == IntPtr.Zero)
				id_getPlayerError2 = JNIEnv.GetMethodID (class_ref, "getPlayerError", "()Landroidx/media3/exoplayer/ExoPlaybackException;");
			return global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.ExoPlaybackException> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_getPlayerError), JniHandleOwnership.TransferLocalRef);
		}
	}



		static Delegate? cb_replaceMediaItems_IILSystem_Collections_Generic_IList_1_;
#pragma warning disable 0169
		static Delegate GetReplaceMediaItems_IILSystem_Collections_Generic_IList_1_Handler ()
		{
			if (cb_replaceMediaItems_IILSystem_Collections_Generic_IList_1_ == null)
				cb_replaceMediaItems_IILSystem_Collections_Generic_IList_1_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPIIL_V (n_ReplaceMediaItems_IILSystem_Collections_Generic_IList_1_));
			return cb_replaceMediaItems_IILSystem_Collections_Generic_IList_1_;
		}

		static void n_ReplaceMediaItems_IILSystem_Collections_Generic_IList_1_ (IntPtr jnienv, IntPtr native__this, int p0, int p1, IntPtr native_p2)
		{
			var __this = global::Java.Lang.Object.GetObject<global::AndroidX.Media3.ExoPlayer.IExoPlayer> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var p2 = (global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>?)global::Java.Lang.Object.GetObject<Java.Lang.Object> (native_p2, JniHandleOwnership.DoNotTransfer);
			__this.ReplaceMediaItems (p0, p1, p2);
		}
#pragma warning restore 0169

		IntPtr id_replaceMediaItems_IILSystem_Collections_Generic_IList_1_;
		public unsafe void ReplaceMediaItems (int p0, int p1, global::System.Collections.Generic.IList<global::AndroidX.Media3.Common.MediaItem>? p2)
		{
			if (id_replaceMediaItems_IILSystem_Collections_Generic_IList_1_ == IntPtr.Zero)
				id_replaceMediaItems_IILSystem_Collections_Generic_IList_1_ = JNIEnv.GetMethodID (class_ref, "replaceMediaItems", "(IILSystem/Collections/Generic/IList`1;)V");
			JValue* __args = stackalloc JValue [3];
			__args [0] = new JValue (p0);
			__args [1] = new JValue (p1);
			__args [2] = new JValue ((p2 == null) ? IntPtr.Zero : ((global::Java.Lang.Object) p2).Handle);
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_replaceMediaItems_IILSystem_Collections_Generic_IList_1_, __args);
		}


}