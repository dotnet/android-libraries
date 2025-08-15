using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;
using Java.Lang;

namespace Com.Google.Common.Util.Concurrent {

	// Metadata.xml XPath interface reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']"
	[Register ("com/google/common/util/concurrent/ListeningExecutorService", "", "Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker")]
	public partial interface IListeningExecutorService : global::Java.Util.Concurrent.IExecutorService {
		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']/method[@name='invokeAll' and count(parameter)=1 and parameter[1][@type='java.util.Collection&lt;? extends java.util.concurrent.Callable&lt;T&gt;&gt;']]"
		[Register ("invokeAll", "(Ljava/util/Collection;)Ljava/util/List;", "GetInvokeAll_Ljava_util_Collection_Handler:Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker, Google.Guava")]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
		global::System.Collections.Generic.IList<global::Java.Util.Concurrent.IFuture>? InvokeAll (global::System.Collections.Generic.ICollection<global::Java.Util.Concurrent.ICallable>? tasks);

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']/method[@name='invokeAll' and count(parameter)=3 and parameter[1][@type='java.util.Collection&lt;? extends java.util.concurrent.Callable&lt;T&gt;&gt;'] and parameter[2][@type='long'] and parameter[3][@type='java.util.concurrent.TimeUnit']]"
		[Register ("invokeAll", "(Ljava/util/Collection;JLjava/util/concurrent/TimeUnit;)Ljava/util/List;", "GetInvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit_Handler:Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker, Google.Guava")]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
		global::System.Collections.Generic.IList<global::Java.Util.Concurrent.IFuture>? InvokeAll (global::System.Collections.Generic.ICollection<global::Java.Util.Concurrent.ICallable>? tasks, long timeout, global::Java.Util.Concurrent.TimeUnit? unit);

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']/method[@name='submit' and count(parameter)=1 and parameter[1][@type='java.lang.Runnable']]"
		[Register ("submit", "(Ljava/lang/Runnable;)Lcom/google/common/util/concurrent/ListenableFuture;", "GetSubmit_Ljava_lang_Runnable_Handler:Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker, Google.Guava")]
		global::Java.Util.Concurrent.IFuture? Submit (global::Java.Lang.IRunnable? task);

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']/method[@name='submit' and count(parameter)=2 and parameter[1][@type='java.lang.Runnable'] and parameter[2][@type='T']]"
		[Register ("submit", "(Ljava/lang/Runnable;Ljava/lang/Object;)Lcom/google/common/util/concurrent/ListenableFuture;", "GetSubmit_Ljava_lang_Runnable_Ljava_lang_Object_Handler:Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker, Google.Guava")]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
		global::Java.Util.Concurrent.IFuture? Submit (global::Java.Lang.IRunnable? task, global::Java.Lang.Object? result);

		// Metadata.xml XPath method reference: path="/api/package[@name='com.google.common.util.concurrent']/interface[@name='ListeningExecutorService']/method[@name='submit' and count(parameter)=1 and parameter[1][@type='java.util.concurrent.Callable&lt;T&gt;']]"
		[Register ("submit", "(Ljava/util/concurrent/Callable;)Lcom/google/common/util/concurrent/ListenableFuture;", "GetSubmit_Ljava_util_concurrent_Callable_Handler:Com.Google.Common.Util.Concurrent.IListeningExecutorServiceInvoker, Google.Guava")]
		[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
		global::Java.Util.Concurrent.IFuture? Submit (global::Java.Util.Concurrent.ICallable? task);

	}

	[global::Android.Runtime.Register ("com/google/common/util/concurrent/ListeningExecutorService", DoNotGenerateAcw=true)]
	internal partial class IListeningExecutorServiceInvoker : global::Java.Lang.Object, IListeningExecutorService {
		static IntPtr java_class_ref {
			get { return _members_com_google_common_util_concurrent_ListeningExecutorService.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members_com_google_common_util_concurrent_ListeningExecutorService; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return _members_com_google_common_util_concurrent_ListeningExecutorService.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return _members_com_google_common_util_concurrent_ListeningExecutorService.ManagedPeerType; }
		}

		static readonly JniPeerMembers _members_Android_Runtime_IJavaObject = new XAPeerMembers ("Android/Runtime/IJavaObject", typeof (IListeningExecutorServiceInvoker));

		static readonly JniPeerMembers _members_com_google_common_util_concurrent_ListeningExecutorService = new XAPeerMembers ("com/google/common/util/concurrent/ListeningExecutorService", typeof (IListeningExecutorServiceInvoker));

		static readonly JniPeerMembers _members_Java_Interop_IJavaPeerable = new XAPeerMembers ("Java/Interop/IJavaPeerable", typeof (IListeningExecutorServiceInvoker));

		static readonly JniPeerMembers _members_java_util_concurrent_Executor = new XAPeerMembers ("java/util/concurrent/Executor", typeof (IListeningExecutorServiceInvoker));

		static readonly JniPeerMembers _members_java_util_concurrent_ExecutorService = new XAPeerMembers ("java/util/concurrent/ExecutorService", typeof (IListeningExecutorServiceInvoker));

		static readonly JniPeerMembers _members_System_IDisposable = new XAPeerMembers ("System/IDisposable", typeof (IListeningExecutorServiceInvoker));

		public IListeningExecutorServiceInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer)
		{
		}

		static Delegate? cb_invokeAll_InvokeAll_Ljava_util_Collection__Ljava_util_List_;
#pragma warning disable 0169
		static Delegate GetInvokeAll_Ljava_util_Collection_Handler ()
		{
			if (cb_invokeAll_InvokeAll_Ljava_util_Collection__Ljava_util_List_ == null)
				cb_invokeAll_InvokeAll_Ljava_util_Collection__Ljava_util_List_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_L (n_InvokeAll_Ljava_util_Collection_));
			return cb_invokeAll_InvokeAll_Ljava_util_Collection__Ljava_util_List_;
		}

		static IntPtr n_InvokeAll_Ljava_util_Collection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection<global::Java.Util.Concurrent.ICallable>.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = global::Android.Runtime.JavaList<global::Java.Util.Concurrent.IFuture>.ToLocalJniHandle (__this.InvokeAll (tasks));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::System.Collections.Generic.IList<global::Java.Util.Concurrent.IFuture>? InvokeAll (global::System.Collections.Generic.ICollection<global::Java.Util.Concurrent.ICallable>? tasks)
		{
			const string __id = "invokeAll.(Ljava/util/Collection;)Ljava/util/List;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection<global::Java.Util.Concurrent.ICallable>.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_tasks);
				var __rm = _members_com_google_common_util_concurrent_ListeningExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Android.Runtime.JavaList<global::Java.Util.Concurrent.IFuture>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
			}
		}

		static Delegate? cb_invokeAll_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_;
#pragma warning disable 0169
		static Delegate GetInvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit_Handler ()
		{
			if (cb_invokeAll_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_ == null)
				cb_invokeAll_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLJL_L (n_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit_));
			return cb_invokeAll_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_;
		}

		static IntPtr n_InvokeAll_Ljava_util_Collection_JLjava_util_concurrent_TimeUnit_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks, long timeout, IntPtr native_unit)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection<global::Java.Util.Concurrent.ICallable>.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			var unit = global::Java.Lang.Object.GetObject<global::Java.Util.Concurrent.TimeUnit> (native_unit, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = global::Android.Runtime.JavaList<global::Java.Util.Concurrent.IFuture>.ToLocalJniHandle (__this.InvokeAll (tasks, timeout, unit));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::System.Collections.Generic.IList<global::Java.Util.Concurrent.IFuture>? InvokeAll (global::System.Collections.Generic.ICollection<global::Java.Util.Concurrent.ICallable>? tasks, long timeout, global::Java.Util.Concurrent.TimeUnit? unit)
		{
			const string __id = "invokeAll.(Ljava/util/Collection;JLjava/util/concurrent/TimeUnit;)Ljava/util/List;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection<global::Java.Util.Concurrent.ICallable>.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (native_tasks);
				__args [1] = new JniArgumentValue (timeout);
				__args [2] = new JniArgumentValue ((unit == null) ? IntPtr.Zero : ((global::Java.Lang.Object) unit).Handle);
				var __rm = _members_com_google_common_util_concurrent_ListeningExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Android.Runtime.JavaList<global::Java.Util.Concurrent.IFuture>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
				global::System.GC.KeepAlive (unit);
			}
		}

		static Delegate? cb_submit_Submit_Ljava_lang_Runnable__Lcom_google_common_util_concurrent_ListenableFuture_;
#pragma warning disable 0169
		static Delegate GetSubmit_Ljava_lang_Runnable_Handler ()
		{
			if (cb_submit_Submit_Ljava_lang_Runnable__Lcom_google_common_util_concurrent_ListenableFuture_ == null)
				cb_submit_Submit_Ljava_lang_Runnable__Lcom_google_common_util_concurrent_ListenableFuture_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_L (n_Submit_Ljava_lang_Runnable_));
			return cb_submit_Submit_Ljava_lang_Runnable__Lcom_google_common_util_concurrent_ListenableFuture_;
		}

		static IntPtr n_Submit_Ljava_lang_Runnable_ (IntPtr jnienv, IntPtr native__this, IntPtr native_task)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var task = (global::Java.Lang.IRunnable?)global::Java.Lang.Object.GetObject<global::Java.Lang.IRunnable> (native_task, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.Submit (task));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::Java.Util.Concurrent.IFuture? Submit (global::Java.Lang.IRunnable? task)
		{
			const string __id = "submit.(Ljava/lang/Runnable;)Lcom/google/common/util/concurrent/ListenableFuture;";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((task == null) ? IntPtr.Zero : ((global::Java.Lang.Object) task).Handle);
				var __rm = _members_com_google_common_util_concurrent_ListeningExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListenableFuture> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				global::System.GC.KeepAlive (task);
			}
		}

		static Delegate? cb_submit_Submit_Ljava_lang_Runnable_Ljava_lang_Object__Lcom_google_common_util_concurrent_ListenableFuture_;
#pragma warning disable 0169
		static Delegate GetSubmit_Ljava_lang_Runnable_Ljava_lang_Object_Handler ()
		{
			if (cb_submit_Submit_Ljava_lang_Runnable_Ljava_lang_Object__Lcom_google_common_util_concurrent_ListenableFuture_ == null)
				cb_submit_Submit_Ljava_lang_Runnable_Ljava_lang_Object__Lcom_google_common_util_concurrent_ListenableFuture_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLL_L (n_Submit_Ljava_lang_Runnable_Ljava_lang_Object_));
			return cb_submit_Submit_Ljava_lang_Runnable_Ljava_lang_Object__Lcom_google_common_util_concurrent_ListenableFuture_;
		}

		static IntPtr n_Submit_Ljava_lang_Runnable_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_task, IntPtr native_result)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var task = (global::Java.Lang.IRunnable?)global::Java.Lang.Object.GetObject<global::Java.Lang.IRunnable> (native_task, JniHandleOwnership.DoNotTransfer);
			var result = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_result, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.Submit (task, result));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::Java.Util.Concurrent.IFuture? Submit (global::Java.Lang.IRunnable? task, global::Java.Lang.Object? result)
		{
			const string __id = "submit.(Ljava/lang/Runnable;Ljava/lang/Object;)Lcom/google/common/util/concurrent/ListenableFuture;";
			IntPtr native_result = JNIEnv.ToLocalJniHandle (result);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue ((task == null) ? IntPtr.Zero : ((global::Java.Lang.Object) task).Handle);
				__args [1] = new JniArgumentValue (native_result);
				var __rm = _members_com_google_common_util_concurrent_ListeningExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListenableFuture> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_result);
				global::System.GC.KeepAlive (task);
				global::System.GC.KeepAlive (result);
			}
		}

		static Delegate? cb_submit_Submit_Ljava_util_concurrent_Callable__Lcom_google_common_util_concurrent_ListenableFuture_;
#pragma warning disable 0169
		static Delegate GetSubmit_Ljava_util_concurrent_Callable_Handler ()
		{
			if (cb_submit_Submit_Ljava_util_concurrent_Callable__Lcom_google_common_util_concurrent_ListenableFuture_ == null)
				cb_submit_Submit_Ljava_util_concurrent_Callable__Lcom_google_common_util_concurrent_ListenableFuture_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_L (n_Submit_Ljava_util_concurrent_Callable_));
			return cb_submit_Submit_Ljava_util_concurrent_Callable__Lcom_google_common_util_concurrent_ListenableFuture_;
		}

		static IntPtr n_Submit_Ljava_util_concurrent_Callable_ (IntPtr jnienv, IntPtr native__this, IntPtr native_task)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var task = (global::Java.Util.Concurrent.ICallable?)global::Java.Lang.Object.GetObject<global::Java.Util.Concurrent.ICallable> (native_task, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.Submit (task));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::Java.Util.Concurrent.IFuture? Submit (global::Java.Util.Concurrent.ICallable? task)
		{
			const string __id = "submit.(Ljava/util/concurrent/Callable;)Lcom/google/common/util/concurrent/ListenableFuture;";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((task == null) ? IntPtr.Zero : ((global::Java.Lang.Object) task).Handle);
				var __rm = _members_com_google_common_util_concurrent_ListeningExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListenableFuture> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				global::System.GC.KeepAlive (task);
			}
		}

		static Delegate? cb_isShutdown_IsShutdown_Z;
#pragma warning disable 0169
		static Delegate GetIsShutdownHandler ()
		{
			if (cb_isShutdown_IsShutdown_Z == null)
				cb_isShutdown_IsShutdown_Z = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_Z (n_IsShutdown));
			return cb_isShutdown_IsShutdown_Z;
		}

		static bool n_IsShutdown (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			return __this.IsShutdown;
		}
#pragma warning restore 0169

		public unsafe bool IsShutdown {
			get {
				const string __id = "isShutdown.()Z";
				try {
					var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate? cb_isTerminated_IsTerminated_Z;
#pragma warning disable 0169
		static Delegate GetIsTerminatedHandler ()
		{
			if (cb_isTerminated_IsTerminated_Z == null)
				cb_isTerminated_IsTerminated_Z = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_Z (n_IsTerminated));
			return cb_isTerminated_IsTerminated_Z;
		}

		static bool n_IsTerminated (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			return __this.IsTerminated;
		}
#pragma warning restore 0169

		public unsafe bool IsTerminated {
			get {
				const string __id = "isTerminated.()Z";
				try {
					var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

		static Delegate? cb_awaitTermination_AwaitTermination_JLjava_util_concurrent_TimeUnit__Z;
#pragma warning disable 0169
		static Delegate GetAwaitTermination_JLjava_util_concurrent_TimeUnit_Handler ()
		{
			if (cb_awaitTermination_AwaitTermination_JLjava_util_concurrent_TimeUnit__Z == null)
				cb_awaitTermination_AwaitTermination_JLjava_util_concurrent_TimeUnit__Z = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPJL_Z (n_AwaitTermination_JLjava_util_concurrent_TimeUnit_));
			return cb_awaitTermination_AwaitTermination_JLjava_util_concurrent_TimeUnit__Z;
		}

		static bool n_AwaitTermination_JLjava_util_concurrent_TimeUnit_ (IntPtr jnienv, IntPtr native__this, long timeout, IntPtr native_unit)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var unit = global::Java.Lang.Object.GetObject<global::Java.Util.Concurrent.TimeUnit> (native_unit, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.AwaitTermination (timeout, unit);
			return __ret;
		}
#pragma warning restore 0169

		public unsafe bool AwaitTermination (long timeout, global::Java.Util.Concurrent.TimeUnit? unit)
		{
			const string __id = "awaitTermination.(JLjava/util/concurrent/TimeUnit;)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (timeout);
				__args [1] = new JniArgumentValue ((unit == null) ? IntPtr.Zero : ((global::Java.Lang.Object) unit).Handle);
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (unit);
			}
		}

		static Delegate? cb_invokeAll_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_;
#pragma warning disable 0169
		static Delegate GetInvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_Handler ()
		{
			if (cb_invokeAll_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_ == null)
				cb_invokeAll_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLJL_L (n_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_));
			return cb_invokeAll_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_util_List_;
		}

		static IntPtr n_InvokeAll_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks, long timeout, IntPtr native_unit)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			var unit = global::Java.Lang.Object.GetObject<global::Java.Util.Concurrent.TimeUnit> (native_unit, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = global::Android.Runtime.JavaList.ToLocalJniHandle (__this.InvokeAll (tasks, timeout, unit));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::System.Collections.IList? InvokeAll (global::System.Collections.ICollection? tasks, long timeout, global::Java.Util.Concurrent.TimeUnit? unit)
		{
			const string __id = "invokeAll.(LSystem/Collections/ICollection;JLjava/util/concurrent/TimeUnit;)Ljava/util/List;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (native_tasks);
				__args [1] = new JniArgumentValue (timeout);
				__args [2] = new JniArgumentValue ((unit == null) ? IntPtr.Zero : ((global::Java.Lang.Object) unit).Handle);
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Android.Runtime.JavaList.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
				global::System.GC.KeepAlive (unit);
			}
		}

		static Delegate? cb_invokeAll_InvokeAll_LSystem_Collections_ICollection__Ljava_util_List_;
#pragma warning disable 0169
		static Delegate GetInvokeAll_LSystem_Collections_ICollection_Handler ()
		{
			if (cb_invokeAll_InvokeAll_LSystem_Collections_ICollection__Ljava_util_List_ == null)
				cb_invokeAll_InvokeAll_LSystem_Collections_ICollection__Ljava_util_List_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_L (n_InvokeAll_LSystem_Collections_ICollection_));
			return cb_invokeAll_InvokeAll_LSystem_Collections_ICollection__Ljava_util_List_;
		}

		static IntPtr n_InvokeAll_LSystem_Collections_ICollection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = global::Android.Runtime.JavaList.ToLocalJniHandle (__this.InvokeAll (tasks));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::System.Collections.IList? InvokeAll (global::System.Collections.ICollection? tasks)
		{
			const string __id = "invokeAll.(LSystem/Collections/ICollection;)Ljava/util/List;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_tasks);
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Android.Runtime.JavaList.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
			}
		}

		static Delegate? cb_invokeAny_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_lang_Object_;
#pragma warning disable 0169
		static Delegate GetInvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_Handler ()
		{
			if (cb_invokeAny_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_lang_Object_ == null)
				cb_invokeAny_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLJL_L (n_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_));
			return cb_invokeAny_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit__Ljava_lang_Object_;
		}

		static IntPtr n_InvokeAny_LSystem_Collections_ICollection_JLjava_util_concurrent_TimeUnit_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks, long timeout, IntPtr native_unit)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			var unit = global::Java.Lang.Object.GetObject<global::Java.Util.Concurrent.TimeUnit> (native_unit, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.InvokeAny (tasks, timeout, unit));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::Java.Lang.Object? InvokeAny (global::System.Collections.ICollection? tasks, long timeout, global::Java.Util.Concurrent.TimeUnit? unit)
		{
			const string __id = "invokeAny.(LSystem/Collections/ICollection;JLjava/util/concurrent/TimeUnit;)Ljava/lang/Object;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (native_tasks);
				__args [1] = new JniArgumentValue (timeout);
				__args [2] = new JniArgumentValue ((unit == null) ? IntPtr.Zero : ((global::Java.Lang.Object) unit).Handle);
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
				global::System.GC.KeepAlive (unit);
			}
		}

		static Delegate? cb_invokeAny_InvokeAny_LSystem_Collections_ICollection__Ljava_lang_Object_;
#pragma warning disable 0169
		static Delegate GetInvokeAny_LSystem_Collections_ICollection_Handler ()
		{
			if (cb_invokeAny_InvokeAny_LSystem_Collections_ICollection__Ljava_lang_Object_ == null)
				cb_invokeAny_InvokeAny_LSystem_Collections_ICollection__Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_L (n_InvokeAny_LSystem_Collections_ICollection_));
			return cb_invokeAny_InvokeAny_LSystem_Collections_ICollection__Ljava_lang_Object_;
		}

		static IntPtr n_InvokeAny_LSystem_Collections_ICollection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tasks)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var tasks = global::Android.Runtime.JavaCollection.FromJniHandle (native_tasks, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.InvokeAny (tasks));
			return __ret;
		}
#pragma warning restore 0169

		public unsafe global::Java.Lang.Object? InvokeAny (global::System.Collections.ICollection? tasks)
		{
			const string __id = "invokeAny.(LSystem/Collections/ICollection;)Ljava/lang/Object;";
			IntPtr native_tasks = global::Android.Runtime.JavaCollection.ToLocalJniHandle (tasks);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_tasks);
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, __args);
				return global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
				JNIEnv.DeleteLocalRef (native_tasks);
				global::System.GC.KeepAlive (tasks);
			}
		}

		static Delegate? cb_shutdown_Shutdown_V;
#pragma warning disable 0169
		static Delegate GetShutdownHandler ()
		{
			if (cb_shutdown_Shutdown_V == null)
				cb_shutdown_Shutdown_V = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_Shutdown));
			return cb_shutdown_Shutdown_V;
		}

		static void n_Shutdown (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			__this.Shutdown ();
		}
#pragma warning restore 0169

		public unsafe void Shutdown ()
		{
			const string __id = "shutdown.()V";
			try {
				_members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate? cb_shutdownNow_ShutdownNow_Ljava_util_List_;
#pragma warning disable 0169
		static Delegate GetShutdownNowHandler ()
		{
			if (cb_shutdownNow_ShutdownNow_Ljava_util_List_ == null)
				cb_shutdownNow_ShutdownNow_Ljava_util_List_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_L (n_ShutdownNow));
			return cb_shutdownNow_ShutdownNow_Ljava_util_List_;
		}

		static IntPtr n_ShutdownNow (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			return global::Android.Runtime.JavaList<IRunnable>.ToLocalJniHandle (__this.ShutdownNow ());
		}
#pragma warning restore 0169

		public unsafe global::System.Collections.Generic.IList<global::Java.Lang.IRunnable>? ShutdownNow ()
		{
			const string __id = "shutdownNow.()Ljava/util/List;";
			try {
				var __rm = _members_java_util_concurrent_ExecutorService.InstanceMethods.InvokeAbstractObjectMethod (__id, this, null);
				return global::Android.Runtime.JavaList<IRunnable>.FromJniHandle (__rm.Handle, JniHandleOwnership.TransferLocalRef);
			} finally {
			}
		}

		static Delegate? cb_execute_Execute_Ljava_lang_Runnable__V;
#pragma warning disable 0169
		static Delegate GetExecute_Ljava_lang_Runnable_Handler ()
		{
			if (cb_execute_Execute_Ljava_lang_Runnable__V == null)
				cb_execute_Execute_Ljava_lang_Runnable__V = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_V (n_Execute_Ljava_lang_Runnable_));
			return cb_execute_Execute_Ljava_lang_Runnable__V;
		}

		static void n_Execute_Ljava_lang_Runnable_ (IntPtr jnienv, IntPtr native__this, IntPtr native_command)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Google.Common.Util.Concurrent.IListeningExecutorService> (jnienv, native__this, JniHandleOwnership.DoNotTransfer)!;
			var command = (global::Java.Lang.IRunnable?)global::Java.Lang.Object.GetObject<global::Java.Lang.IRunnable> (native_command, JniHandleOwnership.DoNotTransfer);
			__this.Execute (command);
		}
#pragma warning restore 0169

		public unsafe void Execute (global::Java.Lang.IRunnable? command)
		{
			const string __id = "execute.(Ljava/lang/Runnable;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((command == null) ? IntPtr.Zero : ((global::Java.Lang.Object) command).Handle);
				_members_java_util_concurrent_Executor.InstanceMethods.InvokeAbstractVoidMethod (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (command);
			}
		}

	}
}
