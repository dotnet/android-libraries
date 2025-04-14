using Android.Runtime;
using Java.Interop;
using Java.Lang;

namespace Kotlin.Ranges
{
	unsafe partial class ULongRange : IClosedRange
	{
		Object IClosedRange.EndInclusive => (long)EndInclusive;

		Object IClosedRange.Start => (long)Start;

		bool IClosedRange.Contains(Object value) => Contains((ulong)(long)value);
	}

	unsafe partial class UIntRange : IClosedRange
	{
		Object IClosedRange.EndInclusive => (int)EndInclusive;

		Object IClosedRange.Start => (int)Start;

		bool IClosedRange.Contains(Object value) => Contains((uint)(int)value);
	}

	unsafe partial class UIntRange : IOpenEndRange
	{
		Object IOpenEndRange.EndExclusive => (int)EndInclusive;
		Object IOpenEndRange.Start => (int)Start;

		bool IOpenEndRange.Contains(Object value) => Contains((uint)(int)value);
	}

	unsafe partial class ULongRange : IOpenEndRange
	{
		Object IOpenEndRange.EndExclusive => (int)EndInclusive;
		Object IOpenEndRange.Start => (int)Start;

		bool IOpenEndRange.Contains(Object value) => Contains((uint)(int)value);
	}

	public partial class CharRange
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='kotlin.ranges']/class[@name='CharRange']/method[@name='contains' and count(parameter)=1 and parameter[1][@type='char']]"
		[Register ("contains", "(C)Z", "")]
		public unsafe bool Contains (global::Java.Lang.Character? value)
		{
			const string __id = "contains.(C)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				// error CS0457: Ambiguous user defined conversions 'Character.explicit operator char(Character)' and 'Object.explicit operator byte(Object)' when converting from 'Character' to 'ushort'
				//__args [0] = new JniArgumentValue ((ushort) value);  // Generated as this in .NET 10+
				__args [0] = new JniArgumentValue (value);
				var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (value);
			}
		}
	}
}
