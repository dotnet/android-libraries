using Android.Content;
using AndroidX.Activity.Result.Contract;

namespace AndroidX.Health.Connect.Client.Contracts
{
    public partial class ExerciseRouteRequestContract
    {
        public override Intent CreateIntent(Context context, Java.Lang.Object? input)
        {
            return CreateIntent(context, input?.ToString());
        }

        public override Java.Lang.Object? ParseResult(int resultCode, Intent? intent)
        {
            return ParseResult(resultCode, intent);
        }
    }
}