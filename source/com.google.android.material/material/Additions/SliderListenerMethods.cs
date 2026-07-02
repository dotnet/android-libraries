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
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void RemoveOnChangeListener(Slider.IOnChangeListener listener)
    {
        const string __id = "removeOnChangeListener." + _removeOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void AddOnSliderTouchListener(Slider.IOnSliderTouchListener listener)
    {
        const string __id = "addOnSliderTouchListener." + _addOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void RemoveOnSliderTouchListener(Slider.IOnSliderTouchListener listener)
    {
        const string __id = "removeOnSliderTouchListener." + _removeOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    // C# events wrapping the listener pattern — equivalent to the auto-generated events
    // that the binding generator no longer produces because addOnChangeListener /
    // addOnSliderTouchListener are removed from the generated API (remove-node in Metadata.xml)
    // and re-added via this Additions file instead.

    WeakReference? _weak_implementor_AddOnChangeListener;
    Slider.IOnChangeListenerImplementor __CreateIOnChangeListenerImplementor()
        => new Slider.IOnChangeListenerImplementor(this);

    public event EventHandler<Slider.ChangeEventArgs> Change
    {
        add => Java.Interop.EventHelper.AddEventHandler<Slider.IOnChangeListener, Slider.IOnChangeListenerImplementor>(
            ref _weak_implementor_AddOnChangeListener,
            __CreateIOnChangeListenerImplementor,
            AddOnChangeListener,
            __h => __h.Handler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<Slider.IOnChangeListener, Slider.IOnChangeListenerImplementor>(
            ref _weak_implementor_AddOnChangeListener,
            Slider.IOnChangeListenerImplementor.__IsEmpty,
            __v => RemoveOnChangeListener(__v),
            __h => __h.Handler -= value);
    }

    WeakReference? _weak_implementor_AddOnSliderTouchListener;
    Slider.IOnSliderTouchListenerImplementor __CreateIOnSliderTouchListenerImplementor()
        => new Slider.IOnSliderTouchListenerImplementor(this);

    public event EventHandler<Slider.StartTrackingTouchEventArgs> StartTrackingTouch
    {
        add => Java.Interop.EventHelper.AddEventHandler<Slider.IOnSliderTouchListener, Slider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            __CreateIOnSliderTouchListenerImplementor,
            AddOnSliderTouchListener,
            __h => __h.OnStartTrackingTouchHandler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<Slider.IOnSliderTouchListener, Slider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            Slider.IOnSliderTouchListenerImplementor.__IsEmpty,
            __v => RemoveOnSliderTouchListener(__v),
            __h => __h.OnStartTrackingTouchHandler -= value);
    }

    public event EventHandler<Slider.StopTrackingTouchEventArgs> StopTrackingTouch
    {
        add => Java.Interop.EventHelper.AddEventHandler<Slider.IOnSliderTouchListener, Slider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            __CreateIOnSliderTouchListenerImplementor,
            AddOnSliderTouchListener,
            __h => __h.OnStopTrackingTouchHandler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<Slider.IOnSliderTouchListener, Slider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            Slider.IOnSliderTouchListenerImplementor.__IsEmpty,
            __v => RemoveOnSliderTouchListener(__v),
            __h => __h.OnStopTrackingTouchHandler -= value);
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
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void RemoveOnChangeListener(RangeSlider.IOnChangeListener listener)
    {
        const string __id = "removeOnChangeListener." + _removeOnChangeListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void AddOnSliderTouchListener(RangeSlider.IOnSliderTouchListener listener)
    {
        const string __id = "addOnSliderTouchListener." + _addOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    public unsafe void RemoveOnSliderTouchListener(RangeSlider.IOnSliderTouchListener listener)
    {
        const string __id = "removeOnSliderTouchListener." + _removeOnSliderTouchListenerJni;
        Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue[1];
        __args[0] = new Java.Interop.JniArgumentValue(listener == null ? IntPtr.Zero : ((Java.Lang.Object)listener).Handle);
        try
        {
            _members.InstanceMethods.InvokeVirtualVoidMethod(__id, this, __args);
        }
        finally
        {
            GC.KeepAlive(listener);
        }
    }

    WeakReference? _weak_implementor_AddOnChangeListener;
    RangeSlider.IOnChangeListenerImplementor __CreateIOnChangeListenerImplementor()
        => new RangeSlider.IOnChangeListenerImplementor(this);

    public event EventHandler<RangeSlider.ChangeEventArgs> Change
    {
        add => Java.Interop.EventHelper.AddEventHandler<RangeSlider.IOnChangeListener, RangeSlider.IOnChangeListenerImplementor>(
            ref _weak_implementor_AddOnChangeListener,
            __CreateIOnChangeListenerImplementor,
            AddOnChangeListener,
            __h => __h.Handler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<RangeSlider.IOnChangeListener, RangeSlider.IOnChangeListenerImplementor>(
            ref _weak_implementor_AddOnChangeListener,
            RangeSlider.IOnChangeListenerImplementor.__IsEmpty,
            __v => RemoveOnChangeListener(__v),
            __h => __h.Handler -= value);
    }

    WeakReference? _weak_implementor_AddOnSliderTouchListener;
    RangeSlider.IOnSliderTouchListenerImplementor __CreateIOnSliderTouchListenerImplementor()
        => new RangeSlider.IOnSliderTouchListenerImplementor(this);

    public event EventHandler<RangeSlider.StartTrackingTouchEventArgs> StartTrackingTouch
    {
        add => Java.Interop.EventHelper.AddEventHandler<RangeSlider.IOnSliderTouchListener, RangeSlider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            __CreateIOnSliderTouchListenerImplementor,
            AddOnSliderTouchListener,
            __h => __h.OnStartTrackingTouchHandler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<RangeSlider.IOnSliderTouchListener, RangeSlider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            RangeSlider.IOnSliderTouchListenerImplementor.__IsEmpty,
            __v => RemoveOnSliderTouchListener(__v),
            __h => __h.OnStartTrackingTouchHandler -= value);
    }

    public event EventHandler<RangeSlider.StopTrackingTouchEventArgs> StopTrackingTouch
    {
        add => Java.Interop.EventHelper.AddEventHandler<RangeSlider.IOnSliderTouchListener, RangeSlider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            __CreateIOnSliderTouchListenerImplementor,
            AddOnSliderTouchListener,
            __h => __h.OnStopTrackingTouchHandler += value);
        remove => Java.Interop.EventHelper.RemoveEventHandler<RangeSlider.IOnSliderTouchListener, RangeSlider.IOnSliderTouchListenerImplementor>(
            ref _weak_implementor_AddOnSliderTouchListener,
            RangeSlider.IOnSliderTouchListenerImplementor.__IsEmpty,
            __v => RemoveOnSliderTouchListener(__v),
            __h => __h.OnStopTrackingTouchHandler -= value);
    }
}

