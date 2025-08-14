// Additions to adjust fluent interface return types to satisfy IMediaSourceMediaSourceIFactory
using AndroidX.Media3.ExoPlayer.Drm;
using AndroidX.Media3.ExoPlayer.Upstream;
using AndroidX.Media3.ExoPlayer.Source;

namespace AndroidX.Media3.ExoPlayer.Dash;

public sealed partial class DashMediaSource
{
    public sealed partial class Factory : IMediaSourceMediaSourceIFactory
    {
        // Explicit interface implementations returning the interface type for fluent chaining
        IMediaSourceMediaSourceIFactory? IMediaSourceMediaSourceIFactory.SetDrmSessionManagerProvider(IDrmSessionManagerProvider? drmSessionManagerProvider)
            => SetDrmSessionManagerProvider(drmSessionManagerProvider);

        IMediaSourceMediaSourceIFactory? IMediaSourceMediaSourceIFactory.SetLoadErrorHandlingPolicy(ILoadErrorHandlingPolicy? loadErrorHandlingPolicy)
            => SetLoadErrorHandlingPolicy(loadErrorHandlingPolicy);
    }
}
