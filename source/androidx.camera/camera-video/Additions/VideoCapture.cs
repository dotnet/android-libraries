using AndroidX.Camera.Core.Impl;
using Java.Lang;

namespace AndroidX.Camera.Video
{
    public partial class VideoCapture
    {
        public partial class Builder
        {
            // Fix interface implementation return type mismatch
            Object IUseCaseConfigBuilder.SetStreamUseCase(StreamUseCase streamUseCase)
            {
                return (Object)this.SetStreamUseCase(streamUseCase);
            }
        }
    }
}