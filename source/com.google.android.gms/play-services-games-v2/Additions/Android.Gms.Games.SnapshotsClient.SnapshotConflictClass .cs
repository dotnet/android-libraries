using System;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games
{
    [Register("com/google/android/gms/games/SnapshotsClient$SnapshotConflict", DoNotGenerateAcw = true)]
    public sealed partial class SnapshotConflict : Java.Lang.Object
    {
        static readonly JniPeerMembers _members =
            new XAPeerMembers("com/google/android/gms/games/SnapshotsClient$SnapshotConflict", typeof(SnapshotConflict));

        internal SnapshotConflict(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        // getSnapshot()
        [Register("getSnapshot", "()Lcom/google/android/gms/games/snapshot/Snapshot;", "")]
        public unsafe Android.Gms.Games.Snapshot.Snapshot GetSnapshot()
        {
            const string __id = "getSnapshot.()Lcom/google/android/gms/games/snapshot/Snapshot;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return Java.Lang.Object.GetObject<Android.Gms.Games.Snapshot.Snapshot>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        // getConflictingSnapshot()
        [Register("getConflictingSnapshot", "()Lcom/google/android/gms/games/snapshot/Snapshot;", "")]
        public unsafe Android.Gms.Games.Snapshot.Snapshot GetConflictingSnapshot()
        {
            const string __id = "getConflictingSnapshot.()Lcom/google/android/gms/games/snapshot/Snapshot;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return Java.Lang.Object.GetObject<Android.Gms.Games.Snapshot.Snapshot>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        // getConflictId()
        [Register("getConflictId", "()Ljava/lang/String;", "")]
        public unsafe string GetConflictId()
        {
            const string __id = "getConflictId.()Ljava/lang/String;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        // getResolutionSnapshotContents()
        [Register("getResolutionSnapshotContents", "()Lcom/google/android/gms/games/snapshot/SnapshotContents;", "")]
        public unsafe Android.Gms.Games.Snapshot.SnapshotContents GetResolutionSnapshotContents()
        {
            const string __id = "getResolutionSnapshotContents.()Lcom/google/android/gms/games/snapshot/SnapshotContents;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return Java.Lang.Object.GetObject<Android.Gms.Games.Snapshot.SnapshotContents>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }
    }
}
