using System;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games
{
    [Register("com/google/android/gms/games/SnapshotsClient$DataOrConflict", DoNotGenerateAcw = true)]
    public sealed partial class DataOrConflict : Java.Lang.Object
    {
        static readonly JniPeerMembers _members =
            new XAPeerMembers("com/google/android/gms/games/SnapshotsClient$DataOrConflict", typeof(DataOrConflict));

        internal DataOrConflict(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        // Raw getData() — Java signature is ()Ljava/lang/Object;
        [Register("getData", "()Ljava/lang/Object;", "")]
        public unsafe Java.Lang.Object GetDataRaw()
        {
            const string __id = "getData.()Ljava/lang/Object;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return Java.Lang.Object.GetObject<Java.Lang.Object>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        // Typed convenience property that returns Snapshot or null
        public Android.Gms.Games.Snapshot.Snapshot Data
        {
            get
            {
                var raw = GetDataRaw();
                if (raw == null) return null;
                 return Java.Lang.Object.GetObject<Android.Gms.Games.Snapshot.Snapshot>(
                    raw.Handle,
                    JniHandleOwnership.DoNotTransfer);
            }
        }

        // getConflict() returns SnapshotConflict
        [Register("getConflict", "()Lcom/google/android/gms/games/SnapshotsClient$SnapshotConflict;", "")]
        public unsafe SnapshotConflict GetConflict()
        {
            const string __id = "getConflict.()Lcom/google/android/gms/games/SnapshotsClient$SnapshotConflict;";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
            return Java.Lang.Object.GetObject<SnapshotConflict>(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }

        // isConflict()
        [Register("isConflict", "()Z", "")]
        public unsafe bool IsConflict()
        {
            const string __id = "isConflict.()Z";
            JniArgumentValue* __args = stackalloc JniArgumentValue[0];
            var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
            return __rm;
        }
    }
}
