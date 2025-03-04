#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.ExoPlayer.Audio
{
	public partial class MediaCodecAudioRenderer
    {

		public override unsafe string? NameRendererCapabilities 
        {
            get
            {
                return this.Name;
            }

		}
    }
}
