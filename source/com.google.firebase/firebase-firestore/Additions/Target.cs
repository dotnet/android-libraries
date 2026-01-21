using System;

namespace Google.Firestore.V1
{
    public partial class Target
    {
        public sealed partial class PipelineQueryTarget
        {
            global::Google.Firestore.V1.Target.PipelineQueryTarget.PipelineTypeCase? IPipelineQueryTargetOrBuilder.PipelineTypeCase
            {
                get
                {
                    unsafe
                    {
                        const string __id = "getPipelineTypeCase.()Lcom/google/firestore/v1/Target$PipelineQueryTarget$PipelineTypeCase;";
                        var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod(__id, this, null);
                        return global::Java.Lang.Object.GetObject<global::Google.Firestore.V1.Target.PipelineQueryTarget.PipelineTypeCase>(__rm.Handle, global::Android.Runtime.JniHandleOwnership.TransferLocalRef);
                    }
                }
            }
        }
    }
}
