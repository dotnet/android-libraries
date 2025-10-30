using System;
using System.Collections.Generic;


namespace Google.MLKit.Vision.Digitalink.Recognition.Internal
{
    public partial class DigitalInkRecognitionRegistrar
    {
        /*
            * This is a workaround for

            <attr
                path="/api/package[@name='com.google.mlkit.vision.digitalink.recognition.internal']/class[@name='DigitalInkRecognitionRegistrar']/method[@name='getComponents' and count(parameter)=0]"
                name="manageReturn"
                >
                System.Collections.Generic.IList &lt; Firebase.Components.Component &gt;
            </attr>
        */
        global::System.Collections.Generic.IList<global::Firebase.Components.Component>? global::Firebase.Components.IComponentRegistrar.Components
        {
            get
            {
                var components = this.Components;
                if (components == null)
                    return null;
                
                var result = new List<global::Firebase.Components.Component>();
                foreach (global::Firebase.Components.Component component in components)
                {
                    result.Add(component);
                }
                return result;
            }
        }
    }
}