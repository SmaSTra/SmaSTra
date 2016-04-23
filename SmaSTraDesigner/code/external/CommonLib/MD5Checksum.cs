namespace Common
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Security.Cryptography;

	using Common.Collections;

	// TODO: (PS) Comment this.
	[Serializable]
	public class MD5Checksum : ICloneable, ISerializable
	{
		#region fields

		// TODO: (PS) Comment this.
		private byte[] bytes = null;
		private int? hashCode = null;

		#endregion fields

		#region constructors

		public MD5Checksum()
		{
		}

		public MD5Checksum(byte[] data)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			this.Bytes = md5.ComputeHash(data);
		}

		public MD5Checksum(byte[] data, int offset, int length)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			this.Bytes = md5.ComputeHash(data, offset, length);
		}

		public MD5Checksum(Stream inputStream)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			this.Bytes = md5.ComputeHash(inputStream);
		}

		public MD5Checksum(Stream inputStream, long chunkSize, long stepSize)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (!(inputStream.CanSeek || inputStream.CanRead))
			{
				throw new ArgumentException("'inputStream' is supposed to be able to read and seek.", "inputStream");
			}
			if (stepSize < 1)
			{
				throw new ArgumentException("'chunkSize' must be >= 1.", "chunkSize");
			}
			if (stepSize < 1)
			{
				throw new ArgumentException("'stepSize' must be >= 1.", "stepSize");
			}

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			this.Bytes = md5.ComputeHash(new SteppingStream(inputStream, chunkSize, stepSize));
		}

		#endregion constructors

		#region properties

		// TODO: (PS) Comment this.
		public byte[] Bytes
		{
			get
			{
				return this.bytes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length != 16)
				{
					throw new ArgumentException("Bytes array length should be 16.", "value");
				}

				this.bytes = value;
			}
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			MD5Checksum other = obj as MD5Checksum;

			return other != null && ListEqualityComparer<byte>.Equals(this.Bytes, other.Bytes);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = HashCodeOperations.Combine(BitConverter.ToInt32(this.Bytes, 0),
					BitConverter.ToInt32(this.Bytes, 4),
					BitConverter.ToInt32(this.Bytes, 8),
					BitConverter.ToInt32(this.Bytes, 12));
			}

			return this.hashCode.Value;
		}

		public override string ToString()
		{
			return BitConverter.ToString(this.Bytes);
		}

		#endregion overrideable methods

		#region methods

		public MD5Checksum Clone()
		{
			return new MD5Checksum() { Bytes = this.Bytes };
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Bytes", this.Bytes);
		}

		#endregion methods

		#region nested types

		private class SteppingStream : Stream
		{
			#region fields

			private long chunkSize;
			private Stream innerStream;
			private long segmentSize;
			private long stepSize;

			#endregion fields

			#region constructors

			public SteppingStream(Stream innerStream, long chunkSize, long stepSize)
			{
				this.innerStream = innerStream;
				this.chunkSize = chunkSize;
				this.stepSize = stepSize;
				this.segmentSize = chunkSize + stepSize;
			}

			#endregion constructors

			#region overrideable properties

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { return true; }
			}

			public override bool CanWrite
			{
				get { return false; }
			}

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}

			public override long Position
			{
				get { throw new NotSupportedException(); }
				set { throw new NotSupportedException(); }
			}

			#endregion overrideable properties

			#region overrideable methods

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				int result = 0;
				while (result < count && this.innerStream.Position < this.innerStream.Length)
				{
					long readSize = this.chunkSize - this.innerStream.Position % this.segmentSize;
					if (readSize <= 0)
					{
						this.innerStream.Seek(this.stepSize + readSize, SeekOrigin.Current);
						readSize = this.chunkSize;
					}

					if (result + readSize > count)
					{
						result += this.innerStream.Read(buffer, offset + result, count - result);
					}
					else
					{
						if (readSize <= Int32.MaxValue)
						{
							result += this.innerStream.Read(buffer, offset + result, (int)readSize);
						}
						else
						{
							int length = (int)(readSize / Int32.MaxValue);
							for (int i = 0; i < length; i++)
							{
								result += this.innerStream.Read(buffer, offset + result, Int32.MaxValue);
							}

							result += this.innerStream.Read(buffer, offset + result, (int)(readSize % Int32.MaxValue));
						}

						this.innerStream.Seek(this.stepSize, SeekOrigin.Current);
					}
				}

				return result;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			#endregion overrideable methods
		}

		#endregion nested types
	}
}