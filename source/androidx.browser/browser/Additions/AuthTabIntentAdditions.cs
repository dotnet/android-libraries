using System;
using Android.Content;

namespace AndroidX.Browser.Auth;

public partial class AuthTabIntent
{
    public partial class AuthenticateUserResultContract
    {
        public override Intent CreateIntent(Context? context, Java.Lang.Object? input)
            => CreateIntentImpl(context ?? throw new ArgumentNullException(nameof(context)), input as Intent);

        public override Java.Lang.Object? ParseResult(int resultCode, Intent? intent)
            => ParseResultImpl(resultCode, intent);
    }
}
