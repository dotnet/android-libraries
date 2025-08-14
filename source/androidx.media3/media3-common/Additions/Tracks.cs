using System.Collections.Generic;
using System.Linq;
using Java.Interop;
using Android.Runtime;

namespace AndroidX.Media3.Common;

// Convenience additions to make the modern Media3 Tracks API usable following API guidelines.
// Enables: player.getCurrentTracks().getGroups() and iteration over track groups.
public partial class Tracks : System.Collections.Generic.IEnumerable<Tracks.Group>
{
    /// <summary>
    /// Gets an immutable list of track groups.
    /// This implements the missing getGroups() method from the Java API.
    /// </summary>
    /// <returns>An immutable list of track groups.</returns>
    /// <remarks>
    /// This method provides access to the track groups, where each group contains tracks that 
    /// present the same content but in different formats (different qualities, codecs, languages, etc.).
    /// 
    /// This matches the Java API: public ImmutableList&lt;Tracks.Group&gt; getGroups()
    /// </remarks>
    public unsafe IList<Tracks.Group>? GetGroups()
    {
        // Call the native Java getGroups() method
        // Based on the AndroidX Media3 documentation, this should return ImmutableList<Group>
        const string __id = "getGroups.()Lcom/google/common/collect/ImmutableList;";
        try
        {
            var __rm = _members.InstanceMethods.InvokeNonvirtualObjectMethod(__id, this, null);
            return global::Android.Runtime.JavaList<Tracks.Group>.FromJniHandle(__rm.Handle, JniHandleOwnership.TransferLocalRef);
        }
        finally
        {
            global::System.GC.KeepAlive(this);
        }
    }

    /// <summary>
    /// Gets the track group at the specified index.
    /// This method provides access to individual track groups within the Tracks collection.
    /// </summary>
    /// <param name="index">The index of the group to retrieve.</param>
    /// <returns>The track group at the specified index.</returns>
    /// <remarks>
    /// Each group contains tracks that present the same content but in different formats
    /// (different qualities, codecs, languages, etc.).
    /// </remarks>
    public Tracks.Group? GetGroup(int index)
    {
        var groups = GetGroups();
        if (groups == null || index < 0 || index >= groups.Count)
            return null;
        
        return groups[index];
    }

    /// <summary>
    /// Gets the track group at the specified index using indexer syntax.
    /// </summary>
    /// <param name="index">The index of the group to retrieve.</param>
    /// <returns>The track group at the specified index.</returns>
    public Tracks.Group? this[int index] => GetGroup(index);

    /// <summary>
    /// Gets the number of track groups.
    /// </summary>
    public int Count
    {
        get
        {
            var groups = GetGroups();
            return groups?.Count ?? 0;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the track groups.
    /// </summary>
    /// <returns>An enumerator for the track groups.</returns>
    public IEnumerator<Tracks.Group> GetEnumerator()
    {
        var groups = GetGroups();
        if (groups == null)
            return Enumerable.Empty<Tracks.Group>().GetEnumerator();
        
        return groups.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
