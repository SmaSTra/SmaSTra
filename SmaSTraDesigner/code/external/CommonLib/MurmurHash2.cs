namespace Common
{
	using System.Text;

	// TODO: (PS) Comment this.
	// Source: http://sites.google.com/site/murmurhash/
	public class MurmurHash2
	{
		#region constants

		private const uint DEFAULT_SEED = 0x9fae34d1;

		// 'm' and 'r' are mixing constants generated offline.
		// They're not really 'magic', they just happen to work well.
		private const uint m = 0x5bd1e995;
		private const int r = 24;

		#endregion constants

		#region fields

		private uint seed;

		#endregion fields

		#region constructors

		public MurmurHash2(uint seed)
		{
			this.seed = seed;
		}

		public MurmurHash2()
			: this(DEFAULT_SEED)
		{
		}

		#endregion constructors

		#region methods

		public uint CombuteHash(string text)
		{
			return CombuteHash(Encoding.UTF8.GetBytes(text));
		}

		public uint CombuteHash(byte[] data)
		{
			uint len = (uint)data.LongLength;

			// Initialize the hash to a 'random' value.
			uint result = this.seed ^ len;
			unsafe
			{
				fixed (byte* fixedDataPointer = data)
				{
					byte* dataPointer = fixedDataPointer;

					// Mix 4 bytes at a time into the hash
					while (len >= 4)
					{
						uint k = *(uint*)dataPointer;

						k *= m;
						k ^= k >> r;
						k *= m;

						result *= m;
						result ^= k;

						dataPointer += 4;
						len -= 4;
					}

					// Handle the last few bytes of the input array
					switch (len)
					{
						case 3:
							result ^= (uint)dataPointer[2] << 16;
							goto case 2;

						case 2:
							result ^= (uint)dataPointer[1] << 8;
							goto case 1;

						case 1:
							result ^= (uint)dataPointer[0];
							result *= m;
							break;
					};
				}
			}

			// Do a few final mixes of the hash to ensure the last few
			// bytes are well-incorporated.
			result ^= result >> 13;
			result *= m;
			result ^= result >> 15;

			return result;
		}

		#endregion methods
	}
}