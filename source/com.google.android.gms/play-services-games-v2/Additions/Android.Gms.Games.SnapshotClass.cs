using System;
using System.IO;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games.Snapshot
{
    // Manual binding for com.google.android.gms.games.snapshot.Snapshot
    [Register ("com/google/android/gms/games/snapshot/Snapshot", DoNotGenerateAcw = true)]
    public sealed partial class Snapshot : Java.Lang.Object
    {
        static readonly JniPeerMembers _members =
            new XAPeerMembers ("com/google/android/gms/games/snapshot/Snapshot", typeof (Snapshot));

        internal Snapshot (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) { }

        public override JniPeerMembers JniPeerMembers => _members;
        protected override IntPtr ThresholdClass => _members.JniPeerType.PeerReference.Handle;
        protected override Type ThresholdType => _members.ManagedPeerType;

        // -------------------------
        // Core Java API bindings
        // -------------------------

        // SnapshotMetadata getMetadata()
        [Register ("getMetadata", "()Lcom/google/android/gms/games/snapshot/SnapshotMetadata;", "")]
        public unsafe SnapshotMetadata GetMetadata ()
        {
            const string __id = "getMetadata.()Lcom/google/android/gms/games/snapshot/SnapshotMetadata;";
            JniArgumentValue* __args = stackalloc JniArgumentValue [0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<SnapshotMetadata> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        // SnapshotContents getSnapshotContents()
        [Register ("getSnapshotContents", "()Lcom/google/android/gms/games/snapshot/SnapshotContents;", "")]
        public unsafe SnapshotContents GetSnapshotContents ()
        {
            const string __id = "getSnapshotContents.()Lcom/google/android/gms/games/snapshot/SnapshotContents;";
            JniArgumentValue* __args = stackalloc JniArgumentValue [0];
            var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, __args);
            return Java.Lang.Object.GetObject<SnapshotContents> (__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
        }

        /// <summary>
        /// Read the full byte[] contents of this snapshot.
        /// </summary>
        public byte [] ReadFully ()
        {
            var contents = GetSnapshotContents ();
            return contents?.ReadFully () ?? Array.Empty<byte> ();
        }

        public void WriteBytes (byte [] data)
        {
            var contents = GetSnapshotContents ();
            if (contents == null)
                throw new InvalidOperationException ("SnapshotContents is null.");

            if (!contents.WriteBytes (data))
                throw new IOException ("Failed to write snapshot data.");
        }

        /// <summary>
        /// Close the underlying SnapshotContents (if you want to close manually).
        /// Usually CommitAndClose on SnapshotsClient is preferred.
        /// </summary>
        public void CloseContents ()
        {
            var contents = GetSnapshotContents ();
            contents?.Close ();
        }

        // Optional: convenience property that returns SnapshotMetadata
        public SnapshotMetadata Metadata => GetMetadata ();

        // A small helper to get a friendly SnapshotId through metadata (returns null if metadata missing)
        public string? SnapshotId => GetMetadata ()?.SnapshotId;
    }
}
