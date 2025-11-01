using AndroidX.AppCompat.View.Menu;

namespace Google.Android.Material.Navigation
{
    public partial class NavigationBarSubheaderView
    {
        // Implement the missing IMenuViewItemView.SetEnabled method
        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }
    }
}