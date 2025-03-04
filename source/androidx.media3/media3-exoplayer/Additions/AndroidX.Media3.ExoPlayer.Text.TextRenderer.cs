#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.Media3.ExoPlayer.Text
{
	public partial class TextRenderer
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
