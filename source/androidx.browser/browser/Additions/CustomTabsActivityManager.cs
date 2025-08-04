using System;
using Android.Content;
using Android.OS;
using System.Collections.Generic;
using Android.App;
using AndroidX.Browser.CustomTabs;

namespace AndroidX.Browser.CustomTabs
{
    public class CustomTabsActivityManager
    {
        static CustomTabsActivityManager? instance;

        [Obsolete ("This method only supports a single parentActivity and caches a reference to it forever. It is recommended to use the CustomTabsActivityManager constructor instead and implement any desired caching in the consuming application.")]
        public static CustomTabsActivityManager From (Android.App.Activity parentActivity, string? servicePackageName = null)
        {
            if (instance == null) {
                instance = new CustomTabsActivityManager (parentActivity);
            }

            return instance;
        }

        public Android.App.Activity ParentActivity { get; private set; }
        public CustomTabsClient Client { get; private set; }

        CustomTabsSession? session = null;
        public CustomTabsSession? Session { 
            get {
                if (Client == null) {
                    return null;
                }

                if (session == null) {
                    session = Client.NewSession ((navEvent, extras) => {
                        var evt = NavigationEvent;
                        if (evt != null)
                            evt (navEvent, extras);
                    }, (callbackName, args) => {
                        var evt = ExtraCallback;
                        if (evt != null)
                            evt (this, new ExtraCallbackEventArgs { CallbackName = callbackName, Args = args });
                    });
                }

                return session;
            }
        }

        CustomTabsServiceConnectionImpl connection;

        public delegate void NavigationEventDelegate (int navigationEvent, Bundle extras);
        public delegate void ExtraCallbackDelegate (object sender, ExtraCallbackEventArgs e);
        public delegate void CustomTabsServiceConnectedDelegate (ComponentName name, CustomTabsClient client);
        public delegate void CustomTabsServiceDisconnectedDelegate (ComponentName name);

        public event NavigationEventDelegate NavigationEvent;
        public event ExtraCallbackDelegate ExtraCallback;
        public event CustomTabsServiceConnectedDelegate CustomTabsServiceConnected;
        public event CustomTabsServiceDisconnectedDelegate CustomTabsServiceDisconnected;

        public class ExtraCallbackEventArgs
        {
            public string CallbackName { get; set; }
            public Bundle Args { get; set; }
        }

        public CustomTabsActivityManager (Android.App.Activity parentActivity)
        {
            ParentActivity = parentActivity;
        }

        public bool BindService (string servicePackageName = null)
        {
            if (string.IsNullOrEmpty (servicePackageName)) {
                servicePackageName = CustomTabsHelper.GetPackageNameToUse (ParentActivity);

                if (servicePackageName == null) 
                    return false;
            }

            connection = new CustomTabsServiceConnectionImpl {
                CustomTabsServiceConnectedHandler = (name, client) => {
                    Client = client;
                    var evt = CustomTabsServiceConnected;
                    if (evt != null)
                        evt (name, client);
                },
                OnServiceDisconnectedHandler = (name) => {
                    var evt = CustomTabsServiceDisconnected;
                    if (evt != null)
                        evt (name);
                }
            };

            return CustomTabsClient.BindCustomTabsService (ParentActivity, servicePackageName, connection);
        }

        public bool Warmup (long flags = 0)
        {
            if (Client == null)
                return false;
            
            return Client.Warmup (flags);
        }

        public bool MayLaunchUrl (string url, Bundle extras, List<string> otherLikelyUrls) 
        {
            if (Session == null)
                return false;
            
            var otherLikelyBundles = new List<Bundle> ();

            if (otherLikelyUrls != null) {
                foreach (var otherUrl in otherLikelyUrls) {
                    var bundle = new Bundle ();
                    bundle.PutString ("url", otherUrl);
                    otherLikelyBundles.Add (bundle);
                }
            }

            return Session.MayLaunchUrl (Android.Net.Uri.Parse (url), extras, otherLikelyBundles);
        }

        public void LaunchUrl (string url, CustomTabsIntent customTabsIntent = null) 
        {            
            if (customTabsIntent == null) {
                customTabsIntent = new CustomTabsIntent.Builder ()
                    .Build ();                    
            }

            CustomTabsHelper.AddKeepAliveExtra (ParentActivity, customTabsIntent.Intent);

            customTabsIntent.LaunchUrl (ParentActivity, Android.Net.Uri.Parse (url));
        }

    }
}

