using AndroidX.Camera.Core;

namespace AndroidX.Camera.Camera2.Interop
{
	public partial class CameraControlCamera2Configurator
	{
		void IInteropConfigurator.Configure (Java.Lang.Object? value) => Configure ((ICameraControl) value!);
	}

	public partial class ImageCaptureCamera2Configurator
	{
		void IInteropConfigurator.Configure (Java.Lang.Object? value) => Configure ((ImageCapture.Builder) value!);
	}

	public partial class SessionConfigCamera2Configurator
	{
		void IInteropConfigurator.Configure (Java.Lang.Object? value) => Configure ((SessionConfig.Builder) value!);
	}

	public partial class UseCaseCamera2Configurator
	{
		void IInteropConfigurator.Configure (Java.Lang.Object? value) => Configure ((UseCase.IInteropConfigurable) value!);
	}
}
