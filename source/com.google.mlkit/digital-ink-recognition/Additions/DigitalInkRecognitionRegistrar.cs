using System;
using System.Collections.Generic;

namespace Google.MLKit.Vision.Digitalink.Recognition.Internal
{
    public partial class DigitalInkRecognitionRegistrar
    {
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