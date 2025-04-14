using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Java.Util.Concurrent;
using JniArgumentValue = Java.Interop.JniArgumentValue;

namespace AndroidX.Work
{
    public partial class OneTimeWorkRequest
    {

        public static OneTimeWorkRequest From<TWorker>() where TWorker : AndroidX.Work.Worker
            => From(typeof(TWorker));

        public static OneTimeWorkRequest From(Type type)
            => From(Java.Lang.Class.FromType(type));

        public partial class Builder
        {
            public Builder(Type type)
                : this(Java.Lang.Class.FromType(type))
            {
            }

            public static Builder From<TWorker>() where TWorker : AndroidX.Work.Worker
                => new Builder(Java.Lang.Class.FromType(typeof(TWorker)));

            // The base type returns a WorkRequest.Builder and we want it to return OneTimeWorkRequest.Builder instead
            #region Base Builder New Implementations
            public new OneTimeWorkRequest Build()
                => base.Build().JavaCast<OneTimeWorkRequest>();

            public new Builder AddTag(string tag)
                => base.AddTag(tag).JavaCast<Builder>();

            public new Builder KeepResultsForAtLeast(long duration, TimeUnit timeUnit)
                => base.KeepResultsForAtLeast(duration, timeUnit).JavaCast<Builder>();

            public Builder KeepResultsForAtLeast(TimeSpan duration)
                => base.KeepResultsForAtLeast((long)duration.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();


            public new Builder SetBackoffCriteria(BackoffPolicy policy, long backoffDelay, TimeUnit timeUnit)
                => base.SetBackoffCriteria(policy, backoffDelay, timeUnit).JavaCast<Builder>();

            public Builder SetBackoffCriteria(BackoffPolicy policy, TimeSpan backoffDelay)
                => base.SetBackoffCriteria(policy, (long)backoffDelay.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            public new Builder SetConstraints(Constraints constraints)
                => base.SetConstraints(constraints).JavaCast<Builder>();

            public new Builder SetInitialRunAttemptCount(int runAttemptCount)
                => base.SetInitialRunAttemptCount(runAttemptCount).JavaCast<Builder>();

            public new Builder SetInitialState(WorkInfo.State state)
                => base.SetInitialState(state).JavaCast<Builder>();

            public new Builder SetInputData(Data data)
                => base.SetInputData(data).JavaCast<Builder>();

            // public new Builder SetPeriodStartTime(long periodStartTime, TimeUnit timeUnit)
            //     => base.SetPeriodStartTime(periodStartTime, timeUnit).JavaCast<Builder>();

            // public Builder SetPeriodStartTime(TimeSpan periodStartTime)
            //     => base.SetPeriodStartTime((long)periodStartTime.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            // public new Builder SetScheduleRequestedAt(long scheduleRequestedAt, TimeUnit timeUnit)
            //     => base.SetPeriodStartTime(scheduleRequestedAt, timeUnit).JavaCast<Builder>();

            // public Builder SetScheduleRequestedAt(TimeSpan scheduleRequestedAt)
            //     => base.SetPeriodStartTime((long)scheduleRequestedAt.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            public new Builder SetInitialDelay(TimeSpan initialDelay)
                => base.SetInitialDelay((long)initialDelay.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            #endregion
        }
    }

    public partial class PeriodicWorkRequest
    {
        public partial class Builder
        {
            public Builder(Type type, long repeatInterval, TimeUnit repeatIntervalTimeUnit)
                : this(Java.Lang.Class.FromType(type), repeatInterval, repeatIntervalTimeUnit)
            { }

            public Builder(Type type, long repeatInterval, TimeUnit repeatIntervalTimeUnit, long flexInterval, TimeUnit flexIntervalTimeUnit)
                : this(Java.Lang.Class.FromType(type), repeatInterval, repeatIntervalTimeUnit, flexInterval, flexIntervalTimeUnit)
            { }

            public Builder(Type type, TimeSpan repeatInterval)
                : this(Java.Lang.Class.FromType(type), (long)repeatInterval.TotalMilliseconds, TimeUnit.Milliseconds)
            { }

            public Builder(Type type, TimeSpan repeatInterval, TimeSpan flexInterval)
                : this(Java.Lang.Class.FromType(type), (long)repeatInterval.TotalMilliseconds, TimeUnit.Milliseconds, (long)flexInterval.TotalMilliseconds, TimeUnit.Milliseconds)
            { }

            public static Builder From<TWorker>(long repeatInterval, TimeUnit repeatIntervalTimeUnit) where TWorker : AndroidX.Work.Worker
                => new Builder(typeof(TWorker), repeatInterval, repeatIntervalTimeUnit);

            public static Builder From<TWorker>(TimeSpan repeatInterval) where TWorker : AndroidX.Work.Worker
                => new Builder(typeof(TWorker), (long)repeatInterval.TotalMilliseconds, TimeUnit.Milliseconds);

            public static Builder From<TWorker>(long repeatInterval, TimeUnit repeatIntervalTimeUnit, long flexInterval, TimeUnit flexIntervalTimeUnit) where TWorker : AndroidX.Work.Worker
                => new Builder(typeof(TWorker), repeatInterval, repeatIntervalTimeUnit, flexInterval, flexIntervalTimeUnit);

            public static Builder From<TWorker>(TimeSpan repeatInterval, TimeSpan flexInterval) where TWorker : AndroidX.Work.Worker
                => new Builder(typeof(TWorker), (long)repeatInterval.TotalMilliseconds, TimeUnit.Milliseconds, (long)flexInterval.TotalMilliseconds, TimeUnit.Milliseconds);

            // The base type returns a WorkRequest.Builder and we want it to return PeriodicWorkRequest.Builder instead
            #region Base Builder New Implementations
            public new PeriodicWorkRequest Build()
                => base.Build().JavaCast<PeriodicWorkRequest>();

            public new Builder AddTag(string tag)
                => base.AddTag(tag).JavaCast<Builder>();

            public new Builder KeepResultsForAtLeast(long duration, TimeUnit timeUnit)
                => base.KeepResultsForAtLeast(duration, timeUnit).JavaCast<Builder>();

            public Builder KeepResultsForAtLeast(TimeSpan duration)
                => base.KeepResultsForAtLeast((long)duration.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();


            public new Builder SetBackoffCriteria(BackoffPolicy policy, long backoffDelay, TimeUnit timeUnit)
                => base.SetBackoffCriteria(policy, backoffDelay, timeUnit).JavaCast<Builder>();

            public Builder SetBackoffCriteria(BackoffPolicy policy, TimeSpan backoffDelay)
                => base.SetBackoffCriteria(policy, (long)backoffDelay.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            public new Builder SetConstraints(Constraints constraints)
                => base.SetConstraints(constraints).JavaCast<Builder>();

            public new Builder SetInitialRunAttemptCount(int runAttemptCount)
                => base.SetInitialRunAttemptCount(runAttemptCount).JavaCast<Builder>();

            public new Builder SetInitialState(WorkInfo.State state)
                => base.SetInitialState(state).JavaCast<Builder>();

            public new Builder SetInputData(Data data)
                => base.SetInputData(data).JavaCast<Builder>();

            // public new Builder SetPeriodStartTime(long periodStartTime, TimeUnit timeUnit)
            //     => base.SetPeriodStartTime(periodStartTime, timeUnit).JavaCast<Builder>();

            // public Builder SetPeriodStartTime(TimeSpan periodStartTime)
            //     => base.SetPeriodStartTime((long)periodStartTime.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            // public new Builder SetScheduleRequestedAt(long scheduleRequestedAt, TimeUnit timeUnit)
            //     => base.SetPeriodStartTime(scheduleRequestedAt, timeUnit).JavaCast<Builder>();

            // public Builder SetScheduleRequestedAt(TimeSpan scheduleRequestedAt)
            //     => base.SetPeriodStartTime((long)scheduleRequestedAt.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();
            #endregion

        }
    }

    public partial class Constraints
    {
        public partial class Builder
        {
            public Builder AddContentUriTrigger(System.Uri uri, bool triggerForDescendants)
                => this.AddContentUriTrigger(Android.Net.Uri.Parse(uri.OriginalString), triggerForDescendants).JavaCast<Builder>();


            public Builder SetTriggerContentMaxDelay(TimeSpan duration)
                => this.SetTriggerContentMaxDelay((long)duration.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

            public Builder SetTriggerContentUpdateDelay(TimeSpan duration)
                => this.SetTriggerContentUpdateDelay((long)duration.TotalMilliseconds, TimeUnit.Milliseconds).JavaCast<Builder>();

        }
    }
}

namespace AndroidX.Work.Impl.Constraints.Trackers
{
    // public partial class BatteryChargingTracker
    // {
    //     protected override global::Java.Lang.Object RawInitialState
    //     {
    //         get
    //         {
    //             return this.InitialState;
    //         }
    //     }
    // }

    // public partial class NetworkStateTracker
    // {
    //     protected override global::Java.Lang.Object RawInitialState
    //     {
    //         get
    //         {
    //             return this.InitialState;
    //         }
    //     }
    // }

    // public partial class BatteryNotLowTracker
    // {
    //     protected override global::Java.Lang.Object RawInitialState
    //     {
    //         get
    //         {
    //             return this.InitialState;
    //         }
    //     }
    // }

    // public partial class StorageNotLowTracker
    // {
    //     protected override global::Java.Lang.Object RawInitialState
    //     {
    //         get
    //         {
    //             return this.InitialState;
    //         }
    //     }
    // }
}

namespace AndroidX.Work.Impl.Constraints.Controllers
{
	public partial class BatteryChargingController
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='androidx.work.impl.constraints.controllers']/class[@name='BatteryChargingController']/method[@name='isConstrained' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("isConstrained", "(Z)Z", "")]
		protected override unsafe bool IsConstrained (global::Java.Lang.Object? value)
		{
			const string __id = "isConstrained.(Z)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				//__args [0] = new JniArgumentValue (value ? (sbyte) 1 : (sbyte) 0);  // Generated
				__args [0] = new JniArgumentValue ((bool)value ? (sbyte) 1 : (sbyte) 0);
				var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (value);
			}
		}
	}

	public partial class BatteryNotLowController
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='androidx.work.impl.constraints.controllers']/class[@name='BatteryNotLowController']/method[@name='isConstrained' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("isConstrained", "(Z)Z", "")]
		protected override unsafe bool IsConstrained (global::Java.Lang.Object? value)
		{
			const string __id = "isConstrained.(Z)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				//__args [0] = new JniArgumentValue (value ? (sbyte) 1 : (sbyte) 0);  // Generated
				__args [0] = new JniArgumentValue ((bool)value ? (sbyte) 1 : (sbyte) 0);
				var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (value);
			}
		}
	}

	public partial class StorageNotLowController
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='androidx.work.impl.constraints.controllers']/class[@name='StorageNotLowController']/method[@name='isConstrained' and count(parameter)=1 and parameter[1][@type='boolean']]"
		[Register ("isConstrained", "(Z)Z", "")]
		protected override unsafe bool IsConstrained (global::Java.Lang.Object? value)
		{
			const string __id = "isConstrained.(Z)Z";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				//__args [0] = new JniArgumentValue (value ? (sbyte) 1 : (sbyte) 0);  // Generated
				__args [0] = new JniArgumentValue ((bool)value ? (sbyte) 1 : (sbyte) 0);
				var __rm = _members.InstanceMethods.InvokeAbstractBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				global::System.GC.KeepAlive (value);
			}
		}
	}
}
