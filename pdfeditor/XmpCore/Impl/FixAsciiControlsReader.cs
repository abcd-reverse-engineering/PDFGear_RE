using System;
using System.IO;
using Sharpen;

namespace XmpCore.Impl
{
	// Token: 0x0200002D RID: 45
	public class FixAsciiControlsReader : PushbackReader
	{
		// Token: 0x060001AC RID: 428 RVA: 0x00004634 File Offset: 0x00002834
		public FixAsciiControlsReader(StreamReader reader)
			: base(reader, 8)
		{
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00004640 File Offset: 0x00002840
		public override int Read(char[] buffer, int off, int len)
		{
			int num = 0;
			int num2 = 0;
			int num3 = off;
			char[] array = new char[8];
			bool flag = true;
			while (flag && num2 < len)
			{
				flag = base.Read(array, num, 1) == 1;
				if (flag)
				{
					char c = this.ProcessChar(array[num]);
					int state = this._state;
					if (state != 0)
					{
						if (state != 5)
						{
							num++;
						}
						else
						{
							base.Unread(array, 0, num + 1);
							num = 0;
						}
					}
					else
					{
						if (Utils.IsControlChar(c))
						{
							c = ' ';
						}
						buffer[num3++] = c;
						num = 0;
						num2++;
					}
				}
				else if (num > 0)
				{
					base.Unread(array, 0, num);
					this._state = 5;
					num = 0;
					flag = true;
				}
			}
			if (num2 <= 0 && !flag)
			{
				return -1;
			}
			return num2;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000046F4 File Offset: 0x000028F4
		private char ProcessChar(char ch)
		{
			switch (this._state)
			{
			case 0:
				if (ch == '&')
				{
					this._state = 1;
				}
				return ch;
			case 1:
				this._state = ((ch == '#') ? 2 : 5);
				return ch;
			case 2:
				if (ch == 'x')
				{
					this._control = 0;
					this._digits = 0;
					this._state = 3;
				}
				else if ('0' <= ch && ch <= '9')
				{
					this._control = (int)(ch - '0');
					this._digits = 1;
					this._state = 4;
				}
				else
				{
					this._state = 5;
				}
				return ch;
			case 3:
				if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'f') || ('A' <= ch && ch <= 'F'))
				{
					this._control = this._control * 16 + Convert.ToInt32(ch.ToString(), 16);
					this._digits++;
					this._state = ((this._digits <= 4) ? 3 : 5);
				}
				else
				{
					if (ch == ';' && Utils.IsControlChar((char)this._control))
					{
						this._state = 0;
						return (char)this._control;
					}
					this._state = 5;
				}
				return ch;
			case 4:
				if ('0' <= ch && ch <= '9')
				{
					this._control = this._control * 10 + (int)(ch - '0');
					this._digits++;
					this._state = ((this._digits <= 5) ? 4 : 5);
				}
				else
				{
					if (ch == ';' && Utils.IsControlChar((char)this._control))
					{
						this._state = 0;
						return (char)this._control;
					}
					this._state = 5;
				}
				return ch;
			case 5:
				this._state = 0;
				return ch;
			default:
				return ch;
			}
		}

		// Token: 0x040000BE RID: 190
		private const int StateStart = 0;

		// Token: 0x040000BF RID: 191
		private const int StateAmp = 1;

		// Token: 0x040000C0 RID: 192
		private const int StateHash = 2;

		// Token: 0x040000C1 RID: 193
		private const int StateHex = 3;

		// Token: 0x040000C2 RID: 194
		private const int StateDig1 = 4;

		// Token: 0x040000C3 RID: 195
		private const int StateError = 5;

		// Token: 0x040000C4 RID: 196
		private const int BufferSize = 8;

		// Token: 0x040000C5 RID: 197
		private int _state;

		// Token: 0x040000C6 RID: 198
		private int _control;

		// Token: 0x040000C7 RID: 199
		private int _digits;
	}
}
