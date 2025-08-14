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

        /// <summary>
        /// Returns all non-null track selections of type <typeparamref name="T"/>.
        /// </summary>
        public IEnumerable<T> OfType<T>() where T : class, ITrackSelection
        {
            for (int i = 0; i < Length; i++)
            {
                if (Get(i) is T t)
                    yield return t;
            }
        }
    }
}
