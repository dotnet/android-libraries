#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.ExoPlayer.Metadata 
{
	public partial class MetadataRenderer
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
