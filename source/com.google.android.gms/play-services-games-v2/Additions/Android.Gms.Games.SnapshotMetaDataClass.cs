using System;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games.Snapshot
{
    // Binding for com.google.android.gms.games.snapshot.SnapshotMetadata
    [Register("com/google/android/gms/games/snapshot/SnapshotMetadata", DoNotGenerateAcw = true)]
    public sealed partial class SnapshotMetadata : Java.Lang.Object
    {
        static readonly JniPeerMembers _members =
            new XAPeerMembers("com/google/android/gms/games/snapshot/SnapshotMetadata", typeof(SnapshotMetadata));

        internal SnapshotMetadata(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        // -------- SnapshotId --------
        [Register("getSnapshotId", "()Ljava/lang/String;", "")]
        public unsafe string SnapshotId
        {
            get
            {
                const string __id = "getSnapshotId.()Ljava/lang/String;";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
        }

       // -------- Unique Name --------
        [Register("getUniqueName", "()Ljava/lang/String;", "")]
        public unsafe string UniqueName
        {
            get
            {
                const string __id = "getUniqueName.()Ljava/lang/String;";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
        }

        // -------- Description --------
        [Register ("getDescription", "()Ljava/lang/String;", "")]
        public unsafe string Description
        {
            get
            {
                const string __id = "getDescription.()Ljava/lang/String;";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
        }

        // -------- Last Modified Timestamp --------
        [Register("getLastModifiedTimestamp", "()J", "")]
        public unsafe long LastModifiedTimestamp
        {
            get
            {
                const string __id = "getLastModifiedTimestamp.()J";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualInt64Method(__id, this, __args);
                return __rm;
            }
        }

        // -------- Played Time --------
        [Register("getPlayedTime", "()J", "")]
        public unsafe long PlayedTime
        {
            get
            {
                const string __id = "getPlayedTime.()J";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualInt64Method(__id, this, __args);
                return __rm;
            }
        }

        // -------- Progress Value --------
        [Register("getProgressValue", "()J", "")]
        public unsafe long ProgressValue
        {
            get
            {
                const string __id = "getProgressValue.()J";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualInt64Method(__id, this, __args);
                return __rm;
            }
        }

        // -------- Device Name --------
        [Register ("getDeviceName", "()Ljava/lang/String;", "")]
        public unsafe string DeviceName
        {
            get
            {
                const string __id = "getDeviceName.()Ljava/lang/String;";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
        }

        // -------- Cover Image URL --------
        [Register("getCoverImageUrl", "()Ljava/lang/String;", "")]
        public unsafe string CoverImageUrl
        {
            get
            {
                const string __id = "getCoverImageUrl.()Ljava/lang/String;";
                JniArgumentValue* __args = stackalloc JniArgumentValue[0];
                var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, __args);
                return JNIEnv.GetString(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
        }

        // You can add more properties here, e.g., owner, playedTime, etc.
    }
}
