using System.Collections.Generic;

namespace AndroidX.Media3.Common.Video
{
	public sealed partial class DefaultHardwareBufferFrame
	{
		public sealed partial class Builder
		{
			// Java's HardwareBufferFrame.Builder methods use covariant return types
			// (DefaultHardwareBufferFrame / DefaultHardwareBufferFrame.Builder instead of
			// the IHardwareBufferFrame / IHardwareBufferFrameBuilder interface types), which
			// C# does not allow for implicit interface implementations. Provide explicit
			// interface implementations that delegate to the concrete-typed members.
			IHardwareBufferFrame? IHardwareBufferFrameBuilder.Build()
				=> this.Build();

			IHardwareBufferFrameBuilder? IHardwareBufferFrameBuilder.SetContentTimeUs(long contentTimeUs)
				=> this.SetContentTimeUs(contentTimeUs);

			IHardwareBufferFrameBuilder? IHardwareBufferFrameBuilder.SetMetadata(IDictionary<string, Java.Lang.Object>? metadata)
				=> this.SetMetadata(metadata);
		}
	}
}
