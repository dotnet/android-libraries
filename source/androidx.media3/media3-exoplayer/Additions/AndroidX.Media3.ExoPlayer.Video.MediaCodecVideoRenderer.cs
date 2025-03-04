#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.ExoPlayer.Video 
{
	public partial class MediaCodecVideoRenderer
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
