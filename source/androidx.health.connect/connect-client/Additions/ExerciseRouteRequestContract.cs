using Android.Content;
using AndroidX.Activity.Result.Contract;

namespace AndroidX.Health.Connect.Client.Contracts;

public partial class ExerciseRouteRequestContract
{
    public override Intent CreateIntent(Context? context, global::Java.Lang.Object? input)
    {
        var str = input?.ToString();
        if (context == null || str == null)
            throw new global::System.ArgumentNullException();
        return CreateIntentImpl(context, str);
    }

    public override global::Java.Lang.Object? ParseResult(int resultCode, Intent? intent)
    {
        var result = ParseResultImpl(resultCode, intent);
        return result as global::Java.Lang.Object;
    }
}
