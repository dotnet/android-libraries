namespace AndroidX.Media3.ExoPlayer.Source.Preload;

public partial class DefaultPreloadManager
{
    protected override void PreloadSourceInternal(AndroidX.Media3.ExoPlayer.Source.IMediaSource? mediaSource, Java.Lang.Object? targetPreloadStatus)
    {
        this.PreloadSourceInternal(mediaSource, (DefaultPreloadManager.PreloadStatus?) targetPreloadStatus);
    }
}