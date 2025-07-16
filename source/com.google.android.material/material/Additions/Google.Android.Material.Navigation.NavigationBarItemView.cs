using Android.Runtime;
using Java.Interop;
using Java.Lang;

namespace Google.Android.Material.Navigation
{
	// Metadata.xml XPath class reference: path="/api/package[@name='com.google.android.material.navigation']/class[@name='NavigationBarItemView']"
	// [global::System.Obsolete ("While this type is 'public', Google considers it internal API and reserves the right to modify or delete it in the future. Use at your own risk.", DiagnosticId = "XAOBS001")]
	// [global::Android.Runtime.Register ("com/google/android/material/navigation/NavigationBarItemView", DoNotGenerateAcw=true)]
	public abstract partial class NavigationBarItemView // : global::Android.Widget.FrameLayout, global::AndroidX.AppCompat.View.Menu.IMenuViewItemView 
    {
         public override bool Enabled
        {
            get => base.Enabled;
            set => base.Enabled = value;
        }

        public void SetEnabled(bool enabled) =>
            Enabled = enabled;
   }
}