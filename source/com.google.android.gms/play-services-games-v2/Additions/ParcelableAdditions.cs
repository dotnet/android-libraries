// Keep the original int overloads after restoring IParcelable.
namespace Android.Gms.Games.Achievement
{
	public partial class AchievementRef
	{
		public void WriteToParcel (Android.OS.Parcel p0, int p1)
			=> WriteToParcel (p0, (Android.OS.ParcelableWriteFlags) p1);
	}
}

namespace Android.Gms.Games.Event
{
	public partial class EventRef
	{
		public void WriteToParcel (Android.OS.Parcel p0, int p1)
			=> WriteToParcel (p0, (Android.OS.ParcelableWriteFlags) p1);
	}
}

namespace Android.Gms.Games.Snapshot
{
	public partial class SnapshotMetadataRef
	{
		public void WriteToParcel (Android.OS.Parcel p0, int p1)
			=> WriteToParcel (p0, (Android.OS.ParcelableWriteFlags) p1);
	}
}
