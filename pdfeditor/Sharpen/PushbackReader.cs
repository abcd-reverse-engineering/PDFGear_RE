using System;
using System.IO;

namespace Sharpen
{
	// Token: 0x02000015 RID: 21
	public class PushbackReader : StreamReader
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00002EE6 File Offset: 0x000010E6
		public PushbackReader(StreamReader stream, int size)
			: base(stream.BaseStream)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException("size", "size <= 0");
			}
			this._buf = new char[size];
			this._pos = size;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002F28 File Offset: 0x00001128
		public override int Read()
		{
			object @lock = this._lock;
			int num2;
			lock (@lock)
			{
				int num;
				if (this._pos >= this._buf.Length)
				{
					num = base.Read();
				}
				else
				{
					char[] buf = this._buf;
					num2 = this._pos;
					this._pos = num2 + 1;
					num = buf[num2];
				}
				num2 = num;
			}
			return num2;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002F94 File Offset: 0x00001194
		public override int Read(char[] buffer, int off, int len)
		{
			object @lock = this._lock;
			int num;
			lock (@lock)
			{
				try
				{
					if (len <= 0)
					{
						if (len < 0)
						{
							throw new ArgumentException();
						}
						if (off < 0 || off > buffer.Length)
						{
							throw new ArgumentException();
						}
						num = 0;
					}
					else
					{
						int num2 = this._buf.Length - this._pos;
						if (num2 > 0)
						{
							if (len < num2)
							{
								num2 = len;
							}
							Array.Copy(this._buf, this._pos, buffer, off, num2);
							this._pos += num2;
							off += num2;
							len -= num2;
						}
						if (len > 0)
						{
							len = base.Read(buffer, off, len);
							num = ((len != -1) ? (num2 + len) : ((num2 == 0) ? (-1) : num2));
						}
						else
						{
							num = num2;
						}
					}
				}
				catch (IndexOutOfRangeException)
				{
					throw new ArgumentException();
				}
			}
			return num;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003070 File Offset: 0x00001270
		public void Unread(char[] buffer, int off, int len)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (len > this._pos)
				{
					throw new IOException("Pushback buffer overflow");
				}
				this._pos -= len;
				Array.Copy(buffer, off, this._buf, this._pos, len);
			}
		}

		// Token: 0x04000028 RID: 40
		private readonly object _lock = new object();

		// Token: 0x04000029 RID: 41
		private readonly char[] _buf;

		// Token: 0x0400002A RID: 42
		private int _pos;
	}
}
