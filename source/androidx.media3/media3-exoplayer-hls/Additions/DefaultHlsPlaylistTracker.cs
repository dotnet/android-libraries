using System.Collections.Generic;
using Android.Runtime;

namespace AndroidX.Media3.ExoPlayer.Hls.Playlist;

public sealed partial class DefaultHlsPlaylistTracker
{
    IList<HlsRedundantGroup>? IHlsPlaylistTracker.GetRedundantGroups(int type)
    {
        var raw = GetRedundantGroupsImpl(type);
        if (raw == null) return null;
        return new JavaList<HlsRedundantGroup>(raw.Handle, JniHandleOwnership.DoNotTransfer);
    }
}
