using System;
using Android.Runtime;
using Java.Interop;
using Java.Lang;

namespace AndroidX.AppCompat.View.Menu
{
    // Metadata.xml XPath class reference: path="/api/package[@name='androidx.appcompat.view.menu']/class[@name='BaseMenuPresenter']"
    public abstract partial class BaseMenuPresenter
    {
        public void SetCallback (global::AndroidX.AppCompat.View.Menu.IMenuPresenterCallback callback)
        {
            Callback = callback;
        }
    }

    public partial class ActionMenuItemView
    {
        public void SetEnabled (bool enabled)
        {
            Enabled = enabled;
        }
    }

    public partial class ListMenuItemView
    {
        public void SetEnabled (bool enabled)
        {
            Enabled = enabled;
        }
    }

	public partial class MenuBuilder
	{
		public void SetGroupDividerEnabled(bool enabled)
		{
			GroupDividerEnabled = enabled;
		}
	}

	public partial class SupportActionModeWrapper // : global::Android.Views.ActionMode 
    {
        public string? Title 
        {
            get;
            set; 
        }

        public string? Subtitle 
        {
            get;
            set; 
        }

    }
}
