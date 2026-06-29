using System;
using Android.Runtime;

namespace Google.Android.Material.Slider;

// Hand-written add/remove listener methods for Slider and RangeSlider.
// The auto-generated versions are removed from Metadata.xml because the base interfaces
// (BaseOnChangeListener, BaseOnSliderTouchListener) use erased generics that cause
// ACW type erasure clashes and implementor failures. These manual implementations use
// the correct JNI signature while accepting the typed sub-interfaces directly.
// This pattern (remove in metadata + re-add in Additions) is standard in .NET Android bindings.

public partial class Slider
{
    const string _addOnChangeListenerJni = "(Lcom/google/android/material/slider/BaseOnChangeListener;)V";
    const string _removeOnChangeListenerJni = "(Lcom/google/android/material/slider/BaseOnChangeListener;)V";
    const string _addOnSliderTouchListenerJni = "(Lcom/google/android/material/slider/BaseOnSliderTouchListener;)V";
    const string _removeOnSliderTouchListenerJni = "(Lcom/google/android/material/slider/BaseOnSliderTouchListener;)V";

    public unsafe void AddOnChangeListener(Slider.IOnChangeListener listener)
    {
        const string __id = "addOnChangeListener." + _addOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void RemoveOnChangeListener(Slider.IOnChangeListener listener)
    {
        const string __id = "removeOnChangeListener." + _removeOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void AddOnSliderTouchListener(Slider.IOnSliderTouchListener listener)
    {
        const string __id = "addOnSliderTouchListener." + _addOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void RemoveOnSliderTouchListener(Slider.IOnSliderTouchListener listener)
    {
        const string __id = "removeOnSliderTouchListener." + _removeOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }
}

public partial class RangeSlider
{
    const string _addOnChangeListenerJni = "(Lcom/google/android/material/slider/BaseOnChangeListener;)V";
    const string _removeOnChangeListenerJni = "(Lcom/google/android/material/slider/BaseOnChangeListener;)V";
    const string _addOnSliderTouchListenerJni = "(Lcom/google/android/material/slider/BaseOnSliderTouchListener;)V";
    const string _removeOnSliderTouchListenerJni = "(Lcom/google/android/material/slider/BaseOnSliderTouchListener;)V";

    public unsafe void AddOnChangeListener(RangeSlider.IOnChangeListener listener)
    {
        const string __id = "addOnChangeListener." + _addOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void RemoveOnChangeListener(RangeSlider.IOnChangeListener listener)
    {
        const string __id = "removeOnChangeListener." + _removeOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void AddOnSliderTouchListener(RangeSlider.IOnSliderTouchListener listener)
    {
        const string __id = "addOnSliderTouchListener." + _addOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }

    public unsafe void RemoveOnSliderTouchListener(RangeSlider.IOnSliderTouchListener listener)
    {
        const string __id = "removeOnSliderTouchListener." + _removeOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        GC.KeepAlive(listener);
    }
}

