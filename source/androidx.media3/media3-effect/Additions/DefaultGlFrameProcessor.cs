using AndroidX.Media3.Common.Video;
using Java.Util.Concurrent;

namespace AndroidX.Media3.Effect;

public partial class DefaultGlFrameProcessor
{
	public partial class Factory
	{
		// Same pattern as AndroidX.Media3.Common.Video.DefaultHardwareBufferFrame.Builder: Java allows
		// DefaultGlFrameProcessor.Factory.create(...) to covariantly return the concrete DefaultGlFrameProcessor
		// type instead of the interface's declared IFrameProcessor, which C# cannot satisfy implicitly.
		IFrameProcessor? IFrameProcessorFactory.Create(IFrameWriter? output, IExecutor? listenerExecutor, IFrameProcessorListener? listener)
			=> this.Create(output, listenerExecutor, listener);
	}
}
