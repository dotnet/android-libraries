using Android.Content;
using AndroidX.Activity.Result.Contract;

namespace AndroidX.Health.Connect.Client.Contracts;

public partial class HealthPermissionsRequestContract
{
    public override Intent CreateIntent(Context? context, global::Java.Lang.Object? input)
    {
        var collection = input as System.Collections.Generic.ICollection<string>;
        if (context == null || collection == null)
            throw new global::System.ArgumentNullException();
        return CreateIntentImpl(context, collection);
    }

    public override global::Java.Lang.Object? ParseResult(int resultCode, Intent? intent)
    {
        var result = ParseResultImpl(resultCode, intent);
        return result as global::Java.Lang.Object;
    }
}
