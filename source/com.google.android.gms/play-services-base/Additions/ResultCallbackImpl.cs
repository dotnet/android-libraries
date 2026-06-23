using System;
using Android.Runtime;
using Java.Util.Concurrent;
using Android.OS;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Android.Gms.Common.Apis
{
    public static class IPendingResultExtensions
    {
        public static void SetResultCallback<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TResult> (this PendingResult pr, Action<TResult> callback) where TResult : class, IResult
        {
            pr.SetResultCallback (new ResultCallback<TResult> (callback));
        }

        public static Task<TResult> AsAsync<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TResult> (this PendingResult pr) where TResult : class, IResult
        {
            var rc = new AwaitableResultCallback<TResult> ();

            pr.SetResultCallback (rc);

            return rc.AwaitAsync ();
        }

        //public static async Task AsAsync (this PendingResult pr)
        //{
        //   var rc = new AwaitableResultCallback<Statuses> ();

        //   pr.SetResultCallback (rc);

        //   await rc.AwaitAsync ();
        //}

        public static TaskAwaiter<TResult> GetAwaiter<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TResult> (this PendingResult pr) where TResult : class, IResult
        {
            var rc = new AwaitableResultCallback<TResult> ();

            pr.SetResultCallback (rc);

            return rc.GetAwaiter ();
        }

        public static TaskAwaiter<IResult> GetAwaiter (this PendingResult pr)
        {
            var rc = new AwaitableResultCallback<IResult> ();

            pr.SetResultCallback (rc);

            return rc.GetAwaiter ();
        }
    }

    public partial class GoogleApiClient
    {
        public partial class Builder
        {
            public Builder AddConnectionCallbacks (Action<Bundle> connectedCallback, Action<int> connectionSuspendedCallback)
            {
                return AddConnectionCallbacks (new GoogleApiClientConnectionCallbacksImpl (connectedCallback, connectionSuspendedCallback));
            }

            public Builder AddConnectionCallbacks (Action<Bundle> connectedCallback)
            {
                return AddConnectionCallbacks (new GoogleApiClientConnectionCallbacksImpl (connectedCallback, null));
            }

            public Builder AddConnectionCallbacks (Action connectedCallback)
            {
                return AddConnectionCallbacks (new GoogleApiClientConnectionCallbacksImpl (h => connectedCallback (), null));
            }

            public Builder AddOnConnectionFailedListener (Action<Android.Gms.Common.ConnectionResult> callback)
            {
                return AddOnConnectionFailedListener (new GoogleApiClientOnConnectionFailedListenerImpl (callback));
            }
        }
    }

    internal class GoogleApiClientConnectionCallbacksImpl : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks
    {
        public GoogleApiClientConnectionCallbacksImpl (Action<Bundle> onConnectedHandler, Action<int> onConnectionSuspendedHandler)
        {
            OnConnectedHandler = onConnectedHandler;
            OnConnectionSuspendedHandler = onConnectionSuspendedHandler;
        }

        public Action<Bundle> OnConnectedHandler { get; private set; }
        public Action<int> OnConnectionSuspendedHandler { get; private set; }

        public void OnConnected (Bundle bundle)
        {
            var h = OnConnectedHandler;
            if (h != null)
                h (bundle);
        }

        public void OnConnectionSuspended (int cause)
        {
            var h = OnConnectionSuspendedHandler;
            if (h != null)
                h (cause);
        }
    }

    internal class GoogleApiClientOnConnectionFailedListenerImpl : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
    {
        public GoogleApiClientOnConnectionFailedListenerImpl (Action<Android.Gms.Common.ConnectionResult> handler)
        {
            OnConnectionFailedHandler = handler;
        }

        public Action<Android.Gms.Common.ConnectionResult> OnConnectionFailedHandler { get; private set; }

        public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
        {
            var h = OnConnectionFailedHandler;
            if (h != null)
                h (result);
        }
    }
}
