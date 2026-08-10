using Java.Lang;

namespace AndroidX.Media3.ExoPlayer.Source.Preload;

public partial class DefaultPreloadManager
{
	public partial class SimpleRankingDataComparator
	{
		// Java's Comparator<Integer>.compare(Integer, Integer) satisfies java.util.Comparator<T>.compare(Object, Object)
		// via generic erasure, but C# cannot implicitly implement IComparator.Compare(Object?, Object?) with a method
		// that only accepts the more specific Java.Lang.Integer? type. Provide the explicit interface implementation
		// so the covariant/contravariant mismatch is satisfied without changing the public Compare(Integer?, Integer?) API.
		int Java.Util.IComparator.Compare(Object? o1, Object? o2)
			=> this.Compare((Integer?) o1, (Integer?) o2);
	}
}
