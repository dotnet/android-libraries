using AndroidX.AppCompat.View.Menu;

namespace Google.Android.Material.Navigation
{
    public partial class NavigationBarDividerView
    {
        // Implement the missing IMenuViewItemView.SetEnabled method
        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }
    }
}