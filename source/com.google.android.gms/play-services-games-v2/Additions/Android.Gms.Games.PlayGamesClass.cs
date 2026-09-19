using System;
using Android.App;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games
{
    public sealed partial class PlayGames
    {
        //Matches: getSnapshotsClient(Activity) -> SnapshotsClient
        [Register("getSnapshotsClient", "(Landroid/app/Activity;)Lcom/google/android/gms/games/SnapshotsClient;", "")]
        public static unsafe global::Android.Gms.Games.SnapshotsClient GetSnapshotsClient(global::Android.App.Activity activity)
        {
            const string __id = "getSnapshotsClient.(Landroid/app/Activity;)Lcom/google/android/gms/games/SnapshotsClient;";
            try
            {
                JniArgumentValue* __args = stackalloc JniArgumentValue[1];
                __args[0] = new JniArgumentValue((activity == null) ? IntPtr.Zero : ((global::Java.Lang.Object)activity).Handle);
                var __rm = _members.StaticMethods.InvokeObjectMethod(__id, __args);
                return global::Java.Lang.Object.GetObject<global::Android.Gms.Games.SnapshotsClient>(__rm.Handle, JniHandleOwnership.TransferLocalRef)!;
            }
            finally
            {
                global::System.GC.KeepAlive(activity);
            }
        }

    }
}
