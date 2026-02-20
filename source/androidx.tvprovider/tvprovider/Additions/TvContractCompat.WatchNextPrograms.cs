using System;
using Android.Runtime;
using Java.Interop;

namespace AndroidX.TvProvider.Media.Tv
{
	public sealed partial class TvContractCompat
	{
		[Register ("androidx/tvprovider/media/tv/TvContractCompat$WatchNextPrograms", DoNotGenerateAcw = true)]
		public sealed class WatchNextPrograms : Java.Lang.Object
		{
			static readonly JniPeerMembers _members = new XAPeerMembers ("androidx/tvprovider/media/tv/TvContractCompat$WatchNextPrograms", typeof (WatchNextPrograms));

			internal static new IntPtr class_ref {
				get { return _members.JniPeerType.PeerReference.Handle; }
			}

			public override JniPeerMembers JniPeerMembers {
				get { return _members; }
			}

			WatchNextPrograms (nint javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
			{
			}

			[Register ("CONTENT_URI")]
			public static Android.Net.Uri? ContentUri {
				get {
					const string __id = "CONTENT_URI.Landroid/net/Uri;";
					var __v = _members.StaticFields.GetObjectValue (__id);
					return Java.Lang.Object.GetObject<Android.Net.Uri> (__v.Handle, JniHandleOwnership.TransferLocalRef);
				}
			}

			[Register ("CONTENT_TYPE")]
			public const string ContentType = "vnd.android.cursor.dir/watch_next_program";

			[Register ("CONTENT_ITEM_TYPE")]
			public const string ContentItemType = "vnd.android.cursor.item/watch_next_program";

			[Register ("WATCH_NEXT_TYPE_CONTINUE")]
			public const int WatchNextTypeContinue = 0;

			[Register ("WATCH_NEXT_TYPE_NEXT")]
			public const int WatchNextTypeNext = 1;

			[Register ("WATCH_NEXT_TYPE_NEW")]
			public const int WatchNextTypeNew = 2;

			[Register ("WATCH_NEXT_TYPE_WATCHLIST")]
			public const int WatchNextTypeWatchlist = 3;

			[Register ("COLUMN_WATCH_NEXT_TYPE")]
			public const string ColumnWatchNextType = "watch_next_type";

			[Register ("COLUMN_LAST_ENGAGEMENT_TIME_UTC_MILLIS")]
			public const string ColumnLastEngagementTimeUtcMillis = "last_engagement_time_utc_millis";
		}
	}
}
