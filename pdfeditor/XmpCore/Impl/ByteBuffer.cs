using System;
using System.IO;
using System.Text;

namespace XmpCore.Impl
{
	// Token: 0x0200002C RID: 44
	public sealed class ByteBuffer
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600019C RID: 412 RVA: 0x000042A7 File Offset: 0x000024A7
		// (set) Token: 0x0600019D RID: 413 RVA: 0x000042AF File Offset: 0x000024AF
		public int Length { get; private set; }

		// Token: 0x0600019E RID: 414 RVA: 0x000042B8 File Offset: 0x000024B8
		public ByteBuffer(int initialCapacity)
		{
			this._buffer = new byte[initialCapacity];
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000042CC File Offset: 0x000024CC
		public ByteBuffer(byte[] buffer)
		{
			this._buffer = buffer;
			this.Length = buffer.Length;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x000042E4 File Offset: 0x000024E4
		public ByteBuffer(byte[] buffer, int length)
		{
			if (length > buffer.Length)
			{
				throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
			}
			this._buffer = buffer;
			this.Length = length;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000430C File Offset: 0x0000250C
		public ByteBuffer(Stream stream)
		{
			this._buffer = new byte[16384];
			int num;
			while ((num = stream.Read(this._buffer, this.Length, 16384)) > 0)
			{
				this.Length += num;
				if (num != 16384)
				{
					break;
				}
				this.EnsureCapacity(this.Length + 16384);
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00004375 File Offset: 0x00002575
		public ByteBuffer(byte[] buffer, int offset, int length)
		{
			if (length > buffer.Length - offset)
			{
				throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
			}
			this._buffer = new byte[length];
			Array.Copy(buffer, offset, this._buffer, 0, length);
			this.Length = length;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000043B2 File Offset: 0x000025B2
		public Stream GetByteStream()
		{
			return new MemoryStream(this._buffer, 0, this.Length);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x000043C6 File Offset: 0x000025C6
		public byte ByteAt(int index)
		{
			if (index >= this.Length)
			{
				throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
			}
			return this._buffer[index];
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000043E4 File Offset: 0x000025E4
		public int CharAt(int index)
		{
			if (index >= this.Length)
			{
				throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
			}
			return (int)(this._buffer[index] & byte.MaxValue);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00004408 File Offset: 0x00002608
		public void Append(byte b)
		{
			this.EnsureCapacity(this.Length + 1);
			byte[] buffer = this._buffer;
			int length = this.Length;
			this.Length = length + 1;
			buffer[length] = b;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000443C File Offset: 0x0000263C
		public void Append(byte[] bytes, int offset, int len)
		{
			this.EnsureCapacity(this.Length + len);
			Array.Copy(bytes, offset, this._buffer, this.Length, len);
			this.Length += len;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000446E File Offset: 0x0000266E
		public void Append(byte[] bytes)
		{
			this.Append(bytes, 0, bytes.Length);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000447B File Offset: 0x0000267B
		public void Append(ByteBuffer anotherBuffer)
		{
			this.Append(anotherBuffer._buffer, 0, anotherBuffer.Length);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00004490 File Offset: 0x00002690
		public Encoding GetEncoding()
		{
			if (this._encoding == null)
			{
				if (this.Length < 2)
				{
					this._encoding = Encoding.UTF8;
				}
				else if (this._buffer[0] == 0)
				{
					if (this.Length < 4 || this._buffer[1] != 0)
					{
						this._encoding = Encoding.BigEndianUnicode;
					}
					else
					{
						if ((this._buffer[2] & 255) == 254 && (this._buffer[3] & 255) == 255)
						{
							throw new NotSupportedException("UTF-32BE is not a supported encoding.");
						}
						throw new NotSupportedException("UTF-32 is not a supported encoding.");
					}
				}
				else if ((this._buffer[0] & 255) < 128)
				{
					if (this._buffer[1] != 0)
					{
						this._encoding = Encoding.UTF8;
					}
					else
					{
						if (this.Length >= 4 && this._buffer[2] == 0)
						{
							throw new NotSupportedException("UTF-32LE is not a supported encoding.");
						}
						this._encoding = Encoding.Unicode;
					}
				}
				else
				{
					int num = (int)(this._buffer[0] & byte.MaxValue);
					if (num != 239)
					{
						if (num != 254)
						{
							if (this.Length < 4 || this._buffer[2] != 0)
							{
								throw new NotSupportedException("UTF-16 is not a supported encoding.");
							}
							throw new NotSupportedException("UTF-32 is not a supported encoding.");
						}
						else
						{
							this._encoding = Encoding.BigEndianUnicode;
						}
					}
					else
					{
						this._encoding = Encoding.UTF8;
					}
				}
			}
			return this._encoding;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x000045F4 File Offset: 0x000027F4
		private void EnsureCapacity(int requestedLength)
		{
			if (requestedLength > this._buffer.Length)
			{
				byte[] buffer = this._buffer;
				this._buffer = new byte[buffer.Length * 2];
				Array.Copy(buffer, 0, this._buffer, 0, buffer.Length);
			}
		}

		// Token: 0x040000BB RID: 187
		private byte[] _buffer;

		// Token: 0x040000BC RID: 188
		private Encoding _encoding;
	}
}
