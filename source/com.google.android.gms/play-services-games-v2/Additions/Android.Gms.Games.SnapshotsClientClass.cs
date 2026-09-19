using System;
using Android.App;
using Android.Runtime;
using Android.Gms.Tasks;
using Android.Gms.Games.Snapshot;
using Java.Interop;

namespace Android.Gms.Games
{
    // This binds to com.google.android.gms.games.SnapshotsClient
    [Register ("com/google/android/gms/games/SnapshotsClient", DoNotGenerateAcw = true)]
    public partial class SnapshotsClient : Java.Lang.Object
    {
        internal static readonly JniPeerMembers _members =
           new XAPeerMembers ("com/google/android/gms/games/SnapshotsClient", typeof (SnapshotsClient));
        internal SnapshotsClient (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        // Bind the methods manually
        // --------- Open ---------
        [Register ("open", "(Ljava/lang/String;ZI)Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Android.Gms.Tasks.Task Open (string name, bool createIfMissing, int conflictPolicy)
        {
            const string __id = "open.(Ljava/lang/String;ZI)Lcom/google/android/gms/tasks/Task;";

            IntPtr native_name = JNIEnv.NewString (name);
            try {
                JniArgumentValue* __args = stackalloc JniArgumentValue [3];
                __args [0] = new JniArgumentValue (native_name);
                __args [1] = new JniArgumentValue (createIfMissing);
                __args [2] = new JniArgumentValue (conflictPolicy);

                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
                return Java.Lang.Object.GetObject<Android.Gms.Tasks.Task> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
            } finally {
                JNIEnv.DeleteLocalRef (native_name);
            }
        }

        [Register ("load", "(Z)Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Android.Gms.Tasks.Task Load (bool forceReload)
        {
            const string __id = "load.(Z)Lcom/google/android/gms/tasks/Task;";

            JniArgumentValue* __args = stackalloc JniArgumentValue [1];
            __args [0] = new JniArgumentValue (forceReload);

            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<Android.Gms.Tasks.Task> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        [Register ("commitAndClose", "(Lcom/google/android/gms/games/snapshot/Snapshot;Lcom/google/android/gms/games/snapshot/SnapshotMetadataChange;)Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Task CommitAndClose (Android.Gms.Games.Snapshot.Snapshot snapshot,
                                           Android.Gms.Games.Snapshot.ISnapshotMetadataChange change)
        {
            const string __id = "commitAndClose.(Lcom/google/android/gms/games/snapshot/Snapshot;Lcom/google/android/gms/games/snapshot/SnapshotMetadataChange;)Lcom/google/android/gms/tasks/Task;";
            JniArgumentValue* __args = stackalloc JniArgumentValue [2];
            __args [0] = new JniArgumentValue (snapshot == null ? IntPtr.Zero : snapshot.Handle);
            __args [1] = new JniArgumentValue (change == null ? IntPtr.Zero : change.Handle);
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<Task> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        // --------- Delete ---------
        [Register ("delete", "(Lcom/google/android/gms/games/snapshot/SnapshotMetadata;)Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Task Delete (Android.Gms.Games.Snapshot.SnapshotMetadata metadata)
        {
            const string __id = "delete.(Lcom/google/android/gms/games/snapshot/SnapshotMetadata;)Lcom/google/android/gms/tasks/Task;";
            JniArgumentValue* __args = stackalloc JniArgumentValue [1];
            __args [0] = new JniArgumentValue (metadata == null ? IntPtr.Zero : metadata.Handle);
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<Task> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        // --------- GetSelectSnapshotIntent ---------
        [Register ("getSelectSnapshotIntent", "(ILjava/lang/Boolean;Z)Landroid/content/Intent;", "")]
        public unsafe Android.Content.Intent GetSelectSnapshotIntent (int maxSnapshots,
                                                                    Java.Lang.Boolean allowAddButton,
                                                                    bool allowDelete)
        {
            const string __id = "getSelectSnapshotIntent.(ILjava/lang/Boolean;Z)Landroid/content/Intent;";
            JniArgumentValue* __args = stackalloc JniArgumentValue [3];
            __args [0] = new JniArgumentValue (maxSnapshots);
            __args [1] = new JniArgumentValue (allowAddButton == null ? IntPtr.Zero : allowAddButton.Handle);
            __args [2] = new JniArgumentValue (allowDelete);
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<Android.Content.Intent> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        [Register ("getMaxDataSize", "()Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Android.Gms.Tasks.Task GetMaxDataSize ()
        {
            const string __id = "getMaxDataSize.()Lcom/google/android/gms/tasks/Task;";

            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
            return Java.Lang.Object.GetObject<Android.Gms.Tasks.Task> (__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        [Register("getMaxCoverImageSize", "()Lcom/google/android/gms/tasks/Task;", "")]
        public unsafe Android.Gms.Tasks.Task GetMaxCoverImageSize()
        {
            const string __id = "getMaxCoverImageSize.()Lcom/google/android/gms/tasks/Task;";

            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, null);
            return Java.Lang.Object.GetObject<Android.Gms.Tasks.Task>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }



    }
}
