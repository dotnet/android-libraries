using Java.Lang;
using Java.Util;

namespace Kotlin.Collections
{
	// TODO: Remove these fixes when this bug is fixed:
	//       https://github.com/xamarin/java.interop/issues/470

	partial class AbstractSetInvoker : Java.Util.ISet
	{
	}

	/*
	AbstractList was removed.

	partial class AbstractListInvoker : Java.Util.IList
	{
	}
	*/
}

#if NET8_0_OR_GREATER
namespace Kotlin.Collections.Builders
{
	public partial class MapBuilder
	{
		int IMap.Size () => Size;

		global::System.Collections.ICollection IMap.Values () => Values;
	}

	public partial class MapBuilderEntries
	{
		public override bool Add (Object? element) => Add ((IMapEntry) element!);

		public override int GetSize () => Size;
	}
}
#endif
