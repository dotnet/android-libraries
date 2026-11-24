namespace AndroidX.Camera.Camera2.Adapter
{
	public partial class CameraInfoAdapter
	{
		Java.Lang.Object AndroidX.Camera.Core.Impl.ICameraInfoInternal.CameraCharacteristics => CameraCharacteristics;

		AndroidX.Camera.Core.CameraSelector? AndroidX.Camera.Core.ICameraInfo.CameraSelector =>
			((AndroidX.Camera.Core.Impl.ICameraInfoInternal) this).CameraSelector;
	}
}
