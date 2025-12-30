using System;

namespace Google.Android.Material.Slider
{
    public partial class Slider
    {
        // Public API overloads that use the non-obsolete Slider.IOnChangeListener instead of IBaseOnChangeListener
        public void AddOnChangeListener(IOnChangeListener listener)
        {
            AddOnChangeListener((IBaseOnChangeListener)listener);
        }

        public void RemoveOnChangeListener(IOnChangeListener listener)
        {
            RemoveOnChangeListener((IBaseOnChangeListener)listener);
        }

        // Public API overloads that use the non-obsolete Slider.IOnSliderTouchListener instead of IBaseOnSliderTouchListener
        public void AddOnSliderTouchListener(IOnSliderTouchListener listener)
        {
            AddOnSliderTouchListener((IBaseOnSliderTouchListener)listener);
        }

        public void RemoveOnSliderTouchListener(IOnSliderTouchListener listener)
        {
            RemoveOnSliderTouchListener((IBaseOnSliderTouchListener)listener);
        }
    }
}
