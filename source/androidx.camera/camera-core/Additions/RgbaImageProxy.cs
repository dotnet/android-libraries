namespace AndroidX.Camera.Core.ImageCaptures
{
	public partial class RgbaImageProxy
	{
		long AndroidX.Camera.Common.IImageWrapper.Timestamp => ((IImageProxy) this).Timestamp;
	}
}
