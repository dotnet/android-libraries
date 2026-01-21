using System;

namespace Google.Firestore.V1
{
    public sealed partial class ExecutePipelineRequest
    {
        global::Google.Firestore.V1.ExecutePipelineRequest.ConsistencySelectorCase? IExecutePipelineRequestOrBuilder.ConsistencySelectorCase
        {
            get
            {
                unsafe
                {
                    const string __id = "getConsistencySelectorCase.()Lcom/google/firestore/v1/ExecutePipelineRequest$ConsistencySelectorCase;";
                    var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod(__id, this, null);
                    return global::Java.Lang.Object.GetObject<global::Google.Firestore.V1.ExecutePipelineRequest.ConsistencySelectorCase>(__rm.Handle, global::Android.Runtime.JniHandleOwnership.TransferLocalRef);
                }
            }
        }

        global::Google.Firestore.V1.ExecutePipelineRequest.PipelineTypeCase? IExecutePipelineRequestOrBuilder.PipelineTypeCase
        {
            get
            {
                unsafe
                {
                    const string __id = "getPipelineTypeCase.()Lcom/google/firestore/v1/ExecutePipelineRequest$PipelineTypeCase;";
                    var __rm = _members.InstanceMethods.InvokeAbstractObjectMethod(__id, this, null);
                    return global::Java.Lang.Object.GetObject<global::Google.Firestore.V1.ExecutePipelineRequest.PipelineTypeCase>(__rm.Handle, global::Android.Runtime.JniHandleOwnership.TransferLocalRef);
                }
            }
        }
    }
}
