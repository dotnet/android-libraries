// Additions to improve .NET ergonomics for TrackSelectionArray
// Provides indexer and IEnumerable implementation for iterating track selections.

using System.Collections;
using System.Collections.Generic;

namespace AndroidX.Media3.ExoPlayer.TrackSelection
{
    public partial class TrackSelectionArray : IEnumerable<ITrackSelection?>
    {
        public ITrackSelection? this[int index] => Get(index);

        public IEnumerator<ITrackSelection?> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return Get(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
