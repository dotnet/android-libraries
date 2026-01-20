using System;

namespace Android.Gms.Common.Api.Internal
{
    public partial class Zacr
    {
        void Android.Gms.Common.Apis.IResultCallback.OnResult(Java.Lang.Object? result)
            => this.OnResult((Android.Gms.Common.Apis.IResult?)result);
    }
}
