using System;
using Android.Runtime;

// Devices that use the legacy map renderer with Google Play services older than 25.50.34
// need this library, otherwise maps crash with NoClassDefFoundError: org/apache/http/ProtocolVersion.
// See: https://developers.google.com/maps/documentation/android-sdk/config
// and: https://github.com/flutter/packages/pull/11185
[assembly: Android.App.UsesLibrary("org.apache.http.legacy", Required=false)]

namespace Android.Gms.Maps.Model
{

    public partial class Polygon
    {
        static IntPtr id_setHoles_Ljava_util_List_;
        // Metadata.xml XPath method reference: path="/api/package[@name='com.google.android.gms.maps.model']/class[@name='Polygon']/method[@name='setHoles' and count(parameter)=1 and parameter[1][@type='java.util.List&lt;&gt;']]"
        [Register ("setHoles", "(Ljava/util/List;)V", "")]
        public unsafe void SetHoles (global::System.Collections.Generic.IList<global::System.Collections.Generic.IList<LatLng>> holes)
        {
            if (id_setHoles_Ljava_util_List_ == IntPtr.Zero)
                id_setHoles_Ljava_util_List_ = JNIEnv.GetMethodID (class_ref, "setHoles", "(Ljava/util/List;)V");
            IntPtr native_holes = global::Android.Runtime.JavaList<global::System.Collections.Generic.IList<LatLng>>.ToLocalJniHandle (holes);
            try {
                JValue* __args = stackalloc JValue [1];
                __args [0] = new JValue (native_holes);
                JNIEnv.CallVoidMethod (Handle, id_setHoles_Ljava_util_List_, __args);
            } finally {
                JNIEnv.DeleteLocalRef (native_holes);
            }
        }
    }
}

