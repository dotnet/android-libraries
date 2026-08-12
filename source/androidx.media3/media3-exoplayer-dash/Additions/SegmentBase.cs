namespace AndroidX.Media3.ExoPlayer.Dash.Manifest;

public abstract partial class SegmentBase
{
	// New in 1.11.0: Java's SegmentList/SegmentTemplate/SingleSegmentBase override copyWithPresentationTimeOffset
	// with a covariant (subclass) return type. C# cannot implicitly satisfy the abstract
	// SegmentBase.CopyWithPresentationTimeOffsetCore(long) -> SegmentBase declaration (renamed via Metadata.xml to
	// avoid a name clash) with a method that only returns the more specific subtype, so provide the real
	// overrides here, delegating to each subclass's public covariant-returning method.
	public sealed partial class SegmentList
	{
		public override SegmentBase? CopyWithPresentationTimeOffsetCore(long p0)
			=> this.CopyWithPresentationTimeOffset(p0);
	}

	public sealed partial class SegmentTemplate
	{
		public override SegmentBase? CopyWithPresentationTimeOffsetCore(long p0)
			=> this.CopyWithPresentationTimeOffset(p0);
	}

	public partial class SingleSegmentBase
	{
		public override SegmentBase? CopyWithPresentationTimeOffsetCore(long p0)
			=> this.CopyWithPresentationTimeOffset(p0);
	}
}
