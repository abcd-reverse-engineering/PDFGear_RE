using System;

namespace XmpCore.Impl
{
	// Token: 0x0200002F RID: 47
	internal sealed class ParseState
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00004E25 File Offset: 0x00003025
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00004E2D File Offset: 0x0000302D
		public int Pos { get; private set; }

		// Token: 0x060001B4 RID: 436 RVA: 0x00004E36 File Offset: 0x00003036
		public ParseState(string str)
		{
			this._str = str;
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00004E45 File Offset: 0x00003045
		public bool HasNext
		{
			get
			{
				return this.Pos < this._str.Length;
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00004E5A File Offset: 0x0000305A
		public char Ch(int index)
		{
			if (index >= this._str.Length)
			{
				return '\0';
			}
			return this._str[index];
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00004E78 File Offset: 0x00003078
		public char Ch()
		{
			if (this.Pos >= this._str.Length)
			{
				return '\0';
			}
			return this._str[this.Pos];
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00004EA0 File Offset: 0x000030A0
		public void Skip()
		{
			int pos = this.Pos;
			this.Pos = pos + 1;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00004EC0 File Offset: 0x000030C0
		public int GatherInt(string errorMsg, int maxValue)
		{
			int num = 0;
			bool flag = false;
			char c = this.Ch(this.Pos);
			while ('0' <= c && c <= '9')
			{
				num = num * 10 + (int)(c - '0');
				flag = true;
				int pos = this.Pos;
				this.Pos = pos + 1;
				c = this.Ch(this.Pos);
			}
			if (!flag)
			{
				throw new XmpException(errorMsg, XmpErrorCode.BadValue);
			}
			if (num > maxValue)
			{
				return maxValue;
			}
			if (num >= 0)
			{
				return num;
			}
			return 0;
		}

		// Token: 0x040000C8 RID: 200
		private readonly string _str;
	}
}
