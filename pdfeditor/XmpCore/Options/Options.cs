using System;
using System.Collections.Generic;
using System.Text;

namespace XmpCore.Options
{
	// Token: 0x02000027 RID: 39
	public abstract class Options
	{
		// Token: 0x0600011E RID: 286 RVA: 0x000038A1 File Offset: 0x00001AA1
		protected Options()
		{
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000038A9 File Offset: 0x00001AA9
		protected Options(int options)
		{
			this.AssertOptionsValid(options);
			this.SetOptions(options);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000038BF File Offset: 0x00001ABF
		public void Clear()
		{
			this._options = 0;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000038C8 File Offset: 0x00001AC8
		public bool IsExactly(int optionBits)
		{
			return this.GetOptions() == optionBits;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000038D3 File Offset: 0x00001AD3
		public bool ContainsAllOptions(int optionBits)
		{
			return (this.GetOptions() & optionBits) == optionBits;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000038E0 File Offset: 0x00001AE0
		public bool ContainsOneOf(int optionBits)
		{
			return (this.GetOptions() & optionBits) != 0;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000038ED File Offset: 0x00001AED
		protected bool GetOption(int optionBit)
		{
			return (this._options & optionBit) != 0;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000038FA File Offset: 0x00001AFA
		public void SetOption(int optionBits, bool value)
		{
			this._options = (value ? (this._options | optionBits) : (this._options & ~optionBits));
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00003918 File Offset: 0x00001B18
		public int GetOptions()
		{
			return this._options;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00003920 File Offset: 0x00001B20
		public void SetOptions(int options)
		{
			this.AssertOptionsValid(options);
			this._options = options;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00003930 File Offset: 0x00001B30
		public override bool Equals(object obj)
		{
			Options options = obj as Options;
			return options != null && this.GetOptions() == options.GetOptions();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00003957 File Offset: 0x00001B57
		public override int GetHashCode()
		{
			return this.GetOptions();
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00003960 File Offset: 0x00001B60
		public string GetOptionsString()
		{
			if (this._options != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num2;
				for (int num = this._options; num != 0; num = num2)
				{
					num2 = num & (num - 1);
					int num3 = num ^ num2;
					string optionName = this.GetOptionName(num3);
					stringBuilder.Append(optionName);
					if (num2 != 0)
					{
						stringBuilder.Append(" | ");
					}
				}
				return stringBuilder.ToString();
			}
			return "<none>";
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000039C0 File Offset: 0x00001BC0
		public override string ToString()
		{
			return string.Format("0x{0:X}", this._options);
		}

		// Token: 0x0600012C RID: 300
		protected abstract int GetValidOptions();

		// Token: 0x0600012D RID: 301
		protected abstract string DefineOptionName(int option);

		// Token: 0x0600012E RID: 302 RVA: 0x000039D7 File Offset: 0x00001BD7
		internal virtual void AssertConsistency(int options)
		{
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000039DC File Offset: 0x00001BDC
		private void AssertOptionsValid(int options)
		{
			int num = options & ~this.GetValidOptions();
			if (num != 0)
			{
				throw new XmpException(string.Format("The option bit(s) 0x{0:X} are invalid!", num), XmpErrorCode.BadOptions);
			}
			this.AssertConsistency(options);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00003A18 File Offset: 0x00001C18
		private string GetOptionName(int option)
		{
			IDictionary<int, string> dictionary = this.ProcureOptionNames();
			string text;
			dictionary.TryGetValue(option, out text);
			if (text == null)
			{
				text = this.DefineOptionName(option);
				if (text != null)
				{
					dictionary[option] = text;
				}
				else
				{
					text = "<option name not defined>";
				}
			}
			return text;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00003A58 File Offset: 0x00001C58
		private IDictionary<int, string> ProcureOptionNames()
		{
			IDictionary<int, string> dictionary;
			if ((dictionary = this._optionNames) == null)
			{
				dictionary = (this._optionNames = new Dictionary<int, string>());
			}
			return dictionary;
		}

		// Token: 0x04000089 RID: 137
		private int _options;

		// Token: 0x0400008A RID: 138
		private IDictionary<int, string> _optionNames;
	}
}
