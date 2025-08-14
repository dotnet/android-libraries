using AndroidX.Media3.ExoPlayer.Drm;
using AndroidX.Media3.ExoPlayer.Source;
using AndroidX.Media3.ExoPlayer.Upstream;

namespace AndroidX.Media3.ExoPlayer.Dash
{
    public sealed partial class DashMediaSource
    {
        public sealed partial class Factory
        {
            // Extension methods for fluent API chaining with concrete return type
            public Factory SetDrmSessionManagerProviderFluent(IDrmSessionManagerProvider? drmSessionManagerProvider)
            {
                SetDrmSessionManagerProvider(drmSessionManagerProvider);
                return this;
            }

            public Factory SetLoadErrorHandlingPolicyFluent(ILoadErrorHandlingPolicy? loadErrorHandlingPolicy)
            {
                SetLoadErrorHandlingPolicy(loadErrorHandlingPolicy);
                return this;
            }
        }
    }

    public static class DashMediaSourceFactoryExtensions
    {
        /// <summary>
        /// Sets the DRM session manager provider and returns the Factory for fluent chaining.
        /// </summary>
        public static DashMediaSource.Factory SetDrmSessionManagerProviderChained(this DashMediaSource.Factory factory, IDrmSessionManagerProvider? provider)
        {
            factory.SetDrmSessionManagerProvider(provider);
            return factory;
        }

        /// <summary>
        /// Sets the load error handling policy and returns the Factory for fluent chaining.
        /// </summary>
        public static DashMediaSource.Factory SetLoadErrorHandlingPolicyChained(this DashMediaSource.Factory factory, ILoadErrorHandlingPolicy? policy)
        {
            factory.SetLoadErrorHandlingPolicy(policy);
            return factory;
        }
    }
}
