#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.Gms.Games
{

	public sealed partial class GamesClass
    {
        public sealed partial class GamesOptions
        {
			public System.Collections.Generic.IList<global::Android.Gms.Common.Apis.Scope> ImpliedScopes 
            {
				get 
                {
                    return (System.Collections.Generic.IList <global::Android.Gms.Common.Apis.Scope>) this.ImpliedScopesBound;
				}
			}
        }
    }
}
