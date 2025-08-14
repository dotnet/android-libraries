// Extensions to improve usability of DashMediaSource.Factory in .NET bindings.
// Provides fluent-style chaining methods that return the concrete Factory type instead of the
// generic IMediaSourceMediaSourceIFactory interface, which is the Java-exposed return type.
// This lets callers write: new DashMediaSource.Factory(ds).SetDrmSessionManagerProviderChained(...).SetLoadErrorHandlingPolicyChained(...);

using AndroidX.Media3.ExoPlayer.Dash;
using AndroidX.Media3.ExoPlayer.Drm;
using AndroidX.Media3.ExoPlayer.Upstream;

namespace AndroidX.Media3.ExoPlayer.Dash
{
    /// <summary>
    /// Extension helpers for <see cref="DashMediaSource.Factory"/> to enable fluent chaining
    /// with strongly-typed return values. The original Java API returns a MediaSource.Factory
    /// interface causing the bound method to return <c>IMediaSourceMediaSourceIFactory</c>.
    /// These helpers wrap those calls and return the concrete <see cref="DashMediaSource.Factory"/>.
    /// </summary>
    public static class DashMediaSourceFactoryExtensions
    {
        /// <summary>
        /// Sets the DRM session manager provider and returns the same <see cref="DashMediaSource.Factory"/> instance for chaining.
        /// </summary>
        public static DashMediaSource.Factory SetDrmSessionManagerProviderChained(this DashMediaSource.Factory factory, IDrmSessionManagerProvider? provider)
        {
            // Call the original bound method (returns the interface) and ignore its return to preserve fluent API.
            factory.SetDrmSessionManagerProvider(provider);
            return factory;
        }

        /// <summary>
        /// Sets the load error handling policy and returns the same <see cref="DashMediaSource.Factory"/> instance for chaining.
        /// </summary>
        public static DashMediaSource.Factory SetLoadErrorHandlingPolicyChained(this DashMediaSource.Factory factory, ILoadErrorHandlingPolicy? policy)
        {
            factory.SetLoadErrorHandlingPolicy(policy);
            return factory;
        }
    }
}
