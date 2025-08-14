using AndroidX.Media3.Common;

namespace AndroidX.Media3.ExoPlayer.Source;

// Convenience additions to match common ExoPlayer Java samples (trackGroups.get(i))
// and the intuitive GetGroup(...) naming some developers attempt to use.
public partial class TrackGroupArray
{

    // Indexer for more idiomatic C# access: trackGroups[i]
    public TrackGroup? this[int index] => Get(index);
}
