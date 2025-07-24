using Android.Content;
using AndroidX.Activity.Result.Contract;

namespace AndroidX.Health.Connect.Client.Contracts
{
    public partial class HealthPermissionsRequestContract  
    {
        public override Intent CreateIntent(Context context, Java.Lang.Object? input)
        {
            var typedInput = input as Java.Util.ISet;
            return CreateIntent(context, typedInput);
        }

        public override Java.Lang.Object? ParseResult(int resultCode, Intent? intent)
        {
            return ParseResult(resultCode, intent);
        }
    }
}