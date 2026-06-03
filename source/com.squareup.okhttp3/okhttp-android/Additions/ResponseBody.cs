using System;
using System.Threading.Tasks;
using Android.Runtime;

namespace Square.OkHttp3
{
	partial class ResponseBody
	{
		public Task<byte[]> BytesAsync()
		{
			return Task.Run(() => Bytes());
		}

		public Task<string> StringAsync()
		{
			return Task.Run(() => String());
		}
	}
}
