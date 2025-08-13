using System;

namespace Google.Common.Collect;

partial class ImmutableCollection : global::System.Collections.ICollection
{
    // ICollection

    int global::System.Collections.ICollection.Count =>
        Size();

    bool global::System.Collections.ICollection.IsSynchronized =>
        throw new NotSupportedException();

    object global::System.Collections.ICollection.SyncRoot =>
        throw new NotSupportedException();

    void global::System.Collections.ICollection.CopyTo(Array array, int index) =>
        ToArray().CopyTo(array, index);

    // IEnumerable

    global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() =>
        ToArray().GetEnumerator();

    partial class Builder
    {
        public global::Google.Common.Collect.ImmutableCollection.Builder? Add(global::Java.Lang.Object? element) =>
            OnAdd(element);

        public global::Google.Common.Collect.ImmutableCollection? Build() =>
            OnBuild();
    }
}

partial class ImmutableSet
{
    partial class Builder
    {
        public global::Google.Common.Collect.ImmutableSet.Builder? Add(global::Java.Lang.Object? element) =>
            OnAdd(element) as global::Google.Common.Collect.ImmutableSet.Builder;

        public global::Google.Common.Collect.ImmutableSet? Build() =>
            OnBuild() as global::Google.Common.Collect.ImmutableSet;
    }
}

partial class ImmutableList : global::System.Collections.IList
{
    // Java IList

    global::System.Collections.IList? global::Java.Util.IList.SubList(int fromIndex, int toIndex) =>
        SubList(fromIndex, toIndex);

    // IList

    int global::System.Collections.IList.Add(object? value) =>
        throw new NotSupportedException();

    bool global::System.Collections.IList.Contains(object? value) =>
        Contains(value as Java.Lang.Object);

    int global::System.Collections.IList.IndexOf(object? value) =>
        IndexOf(value as Java.Lang.Object);

    void global::System.Collections.IList.Insert(int index, object? value) =>
        throw new NotSupportedException();

    void global::System.Collections.IList.Remove(object? value) =>
        throw new NotSupportedException();

    void global::System.Collections.IList.RemoveAt(int index) =>
        throw new NotSupportedException();

    bool global::System.Collections.IList.IsFixedSize => true;

    bool global::System.Collections.IList.IsReadOnly => true;

    object? global::System.Collections.IList.this[int index]
    {
        get => Get(index);
        set => throw new NotSupportedException();
    }

    partial class Builder
    {
        public global::Google.Common.Collect.ImmutableList.Builder? Add(global::Java.Lang.Object? element) =>
            OnAdd(element) as global::Google.Common.Collect.ImmutableList.Builder;

        public global::Google.Common.Collect.ImmutableList? Build() =>
            OnBuild() as global::Google.Common.Collect.ImmutableList;
    }
}

partial class ImmutableMap : global::System.Collections.IDictionary
{
    // Java IMap

    System.Collections.ICollection? global::Java.Util.IMap.Values () =>
        Values();

    System.Collections.ICollection? global::Java.Util.IMap.KeySet () =>
        KeySet();

    System.Collections.ICollection? global::Java.Util.IMap.EntrySet () =>
        EntrySet();

    // IDictionary

    void global::System.Collections.IDictionary.Add(object key, object? value) =>
        throw new NotSupportedException();

    bool global::System.Collections.IDictionary.Contains(object key) =>
        ContainsKey(key as Java.Lang.Object);

    global::System.Collections.IDictionaryEnumerator global::System.Collections.IDictionary.GetEnumerator() =>
        throw new NotSupportedException();

    void global::System.Collections.IDictionary.Remove(object key) =>
        throw new NotSupportedException();

    bool global::System.Collections.IDictionary.IsFixedSize => true;

    bool global::System.Collections.IDictionary.IsReadOnly => true;

    object? global::System.Collections.IDictionary.this[object key]
    {
        get => Get(key as Java.Lang.Object);
        set => throw new NotSupportedException();
    }

    System.Collections.ICollection global::System.Collections.IDictionary.Keys =>
        KeySet().ToArray();

    System.Collections.ICollection global::System.Collections.IDictionary.Values =>
        Values().ToArray();

    // ICollection
    
    void global::System.Collections.ICollection.CopyTo(Array array, int index) =>
        throw new NotSupportedException();

    int global::System.Collections.ICollection.Count =>
        Size();

    bool global::System.Collections.ICollection.IsSynchronized => false;

    object global::System.Collections.ICollection.SyncRoot => this;

    // IEnumerable
    
    System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() =>
        EntrySet().ToArray().GetEnumerator();
}
