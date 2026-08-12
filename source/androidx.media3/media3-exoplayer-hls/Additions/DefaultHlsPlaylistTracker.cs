using System.Collections.Generic;
using Android.Runtime;
using AndroidX.Media3.ExoPlayer.Upstream.Contentsteering;

namespace AndroidX.Media3.ExoPlayer.Hls.Playlist;

public sealed partial class DefaultHlsPlaylistTracker
{
    IList<HlsRedundantGroup>? IHlsPlaylistTracker.GetRedundantGroups(int type)
    {
        var raw = GetRedundantGroupsImpl(type);
        if (raw == null) return null;
        return new JavaList<HlsRedundantGroup>(raw.Handle, JniHandleOwnership.DoNotTransfer);
    }

    // New in 1.11.0: getContentSteeringTracker() covariantly returns the concrete HlsContentSteeringTracker
    // type instead of the interface's declared IContentSteeringTracker, which C# cannot satisfy implicitly.
    IContentSteeringTracker? IHlsPlaylistTracker.ContentSteeringTracker
        => this.ContentSteeringTracker;
}
