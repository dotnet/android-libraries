
#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Kotlin.Ranges;

#if ! NET9_0_OR_GREATER

// Metadata.xml XPath class reference: path="/api/package[@name='kotlin.ranges']/class[@name='CharRange']"
// [global::Android.Runtime.Register ("kotlin/ranges/CharRange", DoNotGenerateAcw=true)]
public sealed partial class CharRange //: global::Kotlin.Ranges.CharProgression, global::Kotlin.Ranges.IClosedRange, global::Kotlin.Ranges.IOpenEndRange 
{
    // Metadata.xml XPath method reference: path="/api/package[@name='kotlin.ranges']/class[@name='CharRange']/method[@name='contains' and count(parameter)=1 and parameter[1][@type='char']]"
    [Register ("contains", "(C)Z", "")]
    public unsafe bool Contains (global::Java.Lang.Character? value)
    {
        const string __id = "contains.(C)Z";
        try {
            JniArgumentValue* __args = stackalloc JniArgumentValue [1];
            __args [0] = new JniArgumentValue (value);
            var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
            return __rm;
        } finally {
            global::System.GC.KeepAlive (value);
        }
    }

}

# else

// Metadata.xml XPath class reference: path="/api/package[@name='kotlin.ranges']/class[@name='CharRange']"
// [global::Android.Runtime.Register ("kotlin/ranges/CharRange", DoNotGenerateAcw=true)]
public sealed partial class CharRange //: global::Kotlin.Ranges.CharProgression, global::Kotlin.Ranges.IClosedRange, global::Kotlin.Ranges.IOpenEndRange 
{
    // Metadata.xml XPath method reference: path="/api/package[@name='kotlin.ranges']/class[@name='CharRange']/method[@name='contains' and count(parameter)=1 and parameter[1][@type='char']]"
    [Register ("contains", "(C)Z", "")]
    public unsafe bool Contains (global::Java.Lang.Character? value)
    {
        const string __id = "contains.(C)Z";
        try {
            JniArgumentValue* __args = stackalloc JniArgumentValue [1];
            /*
            ./generated/org.jetbrains.kotlin.kotlin-stdlib/obj/Release/net10.0-android/generated/src/Kotlin.Ranges.CharRange.cs(174,40): error CS0457: Ambiguous user defined conversions 'Character.explicit operator char(Character)' and 'Object.explicit operator byte(Object)' when converting from 'Character' to 'ushort' [./generated/org.jetbrains.kotlin.kotlin-stdlib/org.jetbrains.kotlin.kotlin-stdlib.csproj::TargetFramework=net10.0-android]
                30 Warning(s)
                1 Error(s)

            __args [0] = new JniArgumentValue ((ushort)value);
            */
            __args [0] = new JniArgumentValue ((ushort) ((char)value));
            var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
            return __rm;
        } finally {
            global::System.GC.KeepAlive (value);
        }
    }
}
#endif