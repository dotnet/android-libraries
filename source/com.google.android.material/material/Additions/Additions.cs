using System;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Google.Android.Material.Animation
{
    public partial class ChildrenAlphaProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((ViewGroup)@object);
        public override void Set(Java.Lang.Object @object, Java.Lang.Object value) =>
            Set((ViewGroup)@object, (Java.Lang.Float)value);
    }

    public partial class DrawableAlphaProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((global::Android.Graphics.Drawables.Drawable)@object);
        public override void Set(Java.Lang.Object @object, Java.Lang.Object value) =>
            Set((global::Android.Graphics.Drawables.Drawable)@object, (Java.Lang.Integer)value);
    }

    public partial class ImageMatrixProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((global::Android.Widget.ImageView)@object);
        public override void Set(Java.Lang.Object @object, Java.Lang.Object value) =>
            Set((global::Android.Widget.ImageView)@object, (Matrix)value);
    }

    public partial class ArgbEvaluatorCompat
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (Java.Lang.Integer)startValue, (Java.Lang.Integer)endValue);
    }

    public partial class MatrixEvaluator
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (Matrix)startValue, (Matrix)endValue);
    }
}

namespace Google.Android.Material.AppBar
{
    public partial class CollapsingToolbarLayout
    {
        public override ViewStates Visibility
        {
            get => base.Visibility;
            set => base.Visibility = value;
        }

        public void SetTitle(string title) =>
            Title = title;

        public void SetVisibility(ViewStates visibility) =>
            Visibility = visibility;
    }
}

namespace Google.Android.Material.BottomNavigation
{
    public partial class BottomNavigationItemView
    {
        public void SetEnabled(bool enabled) =>
            Enabled = enabled;
    }
}

namespace Google.Android.Material.CircularReveal
{
    public partial class CircularRevealWidgetCircularRevealEvaluator
    {
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue) =>
            Evaluate(fraction, (CircularRevealWidgetRevealInfo)startValue, (CircularRevealWidgetRevealInfo)endValue);
    }

    public partial class CircularRevealWidgetCircularRevealProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((ICircularRevealWidget)@object);
        public override void Set(Java.Lang.Object @object, Java.Lang.Object value) =>
            Set((ICircularRevealWidget)@object, (CircularRevealWidgetRevealInfo)value);
    }

    public partial class CircularRevealWidgetCircularRevealScrimColorProperty
    {
        public override Java.Lang.Object Get(Java.Lang.Object @object) =>
            Get((ICircularRevealWidget)@object);
        public override void Set(Java.Lang.Object @object, Java.Lang.Object value) =>
            Set((ICircularRevealWidget)@object, (Java.Lang.Integer)value);
    }
}

namespace Google.Android.Material.Internal
{
    public partial class NavigationMenuItemView
    {
        public void SetEnabled(bool enabled) =>
            Enabled = enabled;
    }

    public partial class VisibilityAwareImageButton
    {
        public override ViewStates Visibility
        {
            get => base.Visibility;
            set => base.Visibility = value;
        }

        public void SetVisibility(ViewStates visibility) =>
            Visibility = visibility;
    }
}

namespace Google.Android.Material.Snackbar
{
    public partial class Snackbar
    {
        public Snackbar SetAction(string text, Action<View> clickHandler) =>
            SetAction(text, new SnackbarActionClickImplementor(clickHandler));

        public Snackbar SetAction(Java.Lang.ICharSequence text, Action<View> clickHandler) =>
            SetAction(text, new SnackbarActionClickImplementor(clickHandler));

        public Snackbar SetAction(int resId, Action<View> clickHandler) =>
            SetAction(resId, new SnackbarActionClickImplementor(clickHandler));

        internal class SnackbarActionClickImplementor : Java.Lang.Object, View.IOnClickListener
        {
            private Action<View> handler;

            public SnackbarActionClickImplementor(Action<View> handler) =>
                this.handler = handler;

            public void OnClick(View v) =>
                handler?.Invoke(v);
        }
    }
}

namespace Google.Android.Material.Tabs
{
    public partial class TabLayout
    {
        public partial class TabView
        {
            public override bool Selected
            {
                get => base.Selected;
                set => base.Selected = value;
            }

            public void SetSelected(bool selected) =>
                Selected = selected;
        }
    }
}

namespace Google.Android.Material.TextField
{
    public partial class TextInputLayout
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

// Interface method parameter names were p0, fixing them would break these
// property names, so add these so we have both property names.
namespace Google.Android.Material.Navigation
{
    public partial class NavigationBarView
    {
        public partial class ItemSelectedEventArgs : EventArgs
        {
            public IMenuItem Item => P0;
        }

        public partial class ItemReselectedEventArgs : EventArgs
        {
            public IMenuItem Item => P0;
        }
    }
}
