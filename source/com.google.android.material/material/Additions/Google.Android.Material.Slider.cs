using System;

namespace Google.Android.Material.Slider
{
    // The Java BaseSlider.add/removeOnChangeListener and add/removeOnSliderTouchListener methods are
    // generic (parameter types L / T bound by the internal BaseOnChangeListener / BaseOnSliderTouchListener
    // interfaces). Those internal interfaces are removed in Metadata.xml, so the generated methods take
    // Java.Lang.Object. These overloads restore a strongly typed surface using the public typed listener
    // interfaces, delegating to the Java.Lang.Object overloads.
    public partial class Slider
    {
        public void AddOnChangeListener(IOnChangeListener listener) =>
            AddOnChangeListener((Java.Lang.Object)listener);

        public void RemoveOnChangeListener(IOnChangeListener listener) =>
            RemoveOnChangeListener((Java.Lang.Object)listener);

        public void AddOnSliderTouchListener(IOnSliderTouchListener listener) =>
            AddOnSliderTouchListener((Java.Lang.Object)listener);

        public void RemoveOnSliderTouchListener(IOnSliderTouchListener listener) =>
            RemoveOnSliderTouchListener((Java.Lang.Object)listener);
    }

    public partial class RangeSlider
    {
        public void AddOnChangeListener(IOnChangeListener listener) =>
            AddOnChangeListener((Java.Lang.Object)listener);

        public void RemoveOnChangeListener(IOnChangeListener listener) =>
            RemoveOnChangeListener((Java.Lang.Object)listener);

        public void AddOnSliderTouchListener(IOnSliderTouchListener listener) =>
            AddOnSliderTouchListener((Java.Lang.Object)listener);

        public void RemoveOnSliderTouchListener(IOnSliderTouchListener listener) =>
            RemoveOnSliderTouchListener((Java.Lang.Object)listener);
    }
}
