using System;
using Android.Runtime;
using Java.IO;
using Java.Interop;

namespace Android.Gms.Games.Snapshot
{
    [Register("com/google/android/gms/games/snapshot/SnapshotContents", DoNotGenerateAcw = true)]
    public sealed partial class SnapshotContents : Java.Lang.Object
    {
        static readonly JniPeerMembers _members =
            new XAPeerMembers("com/google/android/gms/games/snapshot/SnapshotContents", typeof(SnapshotContents));

        internal SnapshotContents(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        [Register("readFully", "()[B", "")]
        public unsafe byte[] ReadFully()
        {
            const string __id = "readFully.()[B";
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod(__id, this, null);
            return (byte[])JNIEnv.GetArray(__rm.Handle, JniHandleOwnership.TransferLocalRef, typeof(byte));
        }

        [Register("writeBytes", "([B)Z", "")]
        public unsafe bool WriteBytes(byte[] data)
        {
            const string __id = "writeBytes.([B)Z";
            IntPtr native_data = JNIEnv.NewArray(data);
            try
            {
                JniArgumentValue* __args = stackalloc JniArgumentValue[1];
                __args[0] = new JniArgumentValue(native_data);
                var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, __args);
                return __rm;
            }
            finally
            {
                if (data != null)
                {
                    JNIEnv.CopyArray(native_data, data);
                    JNIEnv.DeleteLocalRef(native_data);
                }
            }
        }

        [Register("close", "()V", "")]
        public unsafe void Close()
        {
            const string __id = "close.()V";
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, null);
        }

        [Register("isClosed", "()Z", "")]
        public unsafe bool IsClosed
        {
            get
            {
                const string __id = "isClosed.()Z";
                var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod(__id, this, null);
                return __rm;
            }
        }
    }
}
