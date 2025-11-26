using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace pdfeditor.Models.DynamicStamps
{
	// Token: 0x0200018A RID: 394
	public class DynamicStampProperties
	{
		// Token: 0x06001677 RID: 5751 RVA: 0x00055A8A File Offset: 0x00053C8A
		public DynamicStampProperties()
			: this(new Dictionary<string, string>())
		{
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00055A97 File Offset: 0x00053C97
		public DynamicStampProperties(Dictionary<string, string> dict)
		{
			this.dict = dict;
			this.stampContents = new DynamicStampProperties.StampContentList(dict);
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06001679 RID: 5753 RVA: 0x00055AB2 File Offset: 0x00053CB2
		public Dictionary<string, string> Data
		{
			get
			{
				return this.dict;
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x0600167A RID: 5754 RVA: 0x00055ABC File Offset: 0x00053CBC
		// (set) Token: 0x0600167B RID: 5755 RVA: 0x00055AE0 File Offset: 0x00053CE0
		public string FontFamily
		{
			get
			{
				string text;
				if (this.Data.TryGetValue("FontFamily", out text))
				{
					return text;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.Data["FontFamily"] = value;
					return;
				}
				this.Data.Remove("FontFamily");
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x0600167C RID: 5756 RVA: 0x00055B10 File Offset: 0x00053D10
		// (set) Token: 0x0600167D RID: 5757 RVA: 0x00055BB0 File Offset: 0x00053DB0
		public DynamicStampProperties.FontWeights FontWeight
		{
			get
			{
				string text;
				if (this.Data.TryGetValue("FontWeight", out text))
				{
					int num;
					if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
					{
						if (num != 300)
						{
							if (num != 400)
							{
								if (num == 700)
								{
									return DynamicStampProperties.FontWeights.Bold;
								}
							}
							return DynamicStampProperties.FontWeights.Normal;
						}
						return DynamicStampProperties.FontWeights.Light;
					}
					else
					{
						if (string.Equals(text, "Light", StringComparison.OrdinalIgnoreCase))
						{
							return DynamicStampProperties.FontWeights.Light;
						}
						if (string.Equals(text, "Normal", StringComparison.OrdinalIgnoreCase))
						{
							return DynamicStampProperties.FontWeights.Normal;
						}
						if (string.Equals(text, "Bold", StringComparison.OrdinalIgnoreCase))
						{
							return DynamicStampProperties.FontWeights.Bold;
						}
					}
				}
				return DynamicStampProperties.FontWeights.Normal;
			}
			set
			{
				if (value == DynamicStampProperties.FontWeights.Bold)
				{
					this.Data["FontWeight"] = "bold";
					return;
				}
				if (value == DynamicStampProperties.FontWeights.Light)
				{
					this.Data["FontWeight"] = "light";
					return;
				}
				this.Data.Remove("FontWeight");
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x0600167E RID: 5758 RVA: 0x00055C0C File Offset: 0x00053E0C
		// (set) Token: 0x0600167F RID: 5759 RVA: 0x00055C53 File Offset: 0x00053E53
		public bool FontItalic
		{
			get
			{
				string text;
				if (this.Data.TryGetValue("FontItalic", out text))
				{
					int num;
					if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
					{
						return num != 0;
					}
					if (string.Equals(text, "true", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			set
			{
				if (value)
				{
					this.Data["FontItalic"] = "1";
					return;
				}
				this.Data.Remove("FontItalic");
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06001680 RID: 5760 RVA: 0x00055C80 File Offset: 0x00053E80
		// (set) Token: 0x06001681 RID: 5761 RVA: 0x00055CD4 File Offset: 0x00053ED4
		public double? FontSize
		{
			get
			{
				string text;
				double num;
				if (this.Data.TryGetValue("FontSize", out text) && double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num) && num > 0.0)
				{
					return new double?(num);
				}
				return null;
			}
			set
			{
				if (value != null && value.Value > 0.0)
				{
					this.Data["FontSize"] = value.Value.ToString(CultureInfo.InvariantCulture);
					return;
				}
				this.Data.Remove("FontSize");
			}
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06001682 RID: 5762 RVA: 0x00055D34 File Offset: 0x00053F34
		// (set) Token: 0x06001683 RID: 5763 RVA: 0x00055D68 File Offset: 0x00053F68
		public int Style
		{
			get
			{
				string text;
				int num;
				if (this.Data.TryGetValue("Style", out text) && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					return num;
				}
				return 1;
			}
			set
			{
				if (value != 1)
				{
					this.Data["Style"] = value.ToString(CultureInfo.InvariantCulture);
					return;
				}
				this.Data.Remove("Style");
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06001684 RID: 5764 RVA: 0x00055D9C File Offset: 0x00053F9C
		// (set) Token: 0x06001685 RID: 5765 RVA: 0x00055DDD File Offset: 0x00053FDD
		public DynamicStampProperties.ArrowDirections ArrowDirection
		{
			get
			{
				string text;
				if (!this.Data.TryGetValue("ArrowDirection", out text))
				{
					return DynamicStampProperties.ArrowDirections.Right;
				}
				if (string.Equals(text, "left", StringComparison.OrdinalIgnoreCase))
				{
					return DynamicStampProperties.ArrowDirections.Left;
				}
				string.Equals(text, "right", StringComparison.OrdinalIgnoreCase);
				return DynamicStampProperties.ArrowDirections.Right;
			}
			set
			{
				if (value == DynamicStampProperties.ArrowDirections.Left)
				{
					this.Data["ArrowDirection"] = "left";
					return;
				}
				this.Data.Remove("ArrowDirection");
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06001686 RID: 5766 RVA: 0x00055E0C File Offset: 0x0005400C
		// (set) Token: 0x06001687 RID: 5767 RVA: 0x00055E30 File Offset: 0x00054030
		public string Locale
		{
			get
			{
				string text;
				if (this.Data.TryGetValue("Locale", out text))
				{
					return text;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.Data["Locale"] = value;
					return;
				}
				this.Data.Remove("Locale");
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06001688 RID: 5768 RVA: 0x00055E5D File Offset: 0x0005405D
		public IReadOnlyList<DynamicStampProperties.StampContent> Contents
		{
			get
			{
				return this.stampContents;
			}
		}

		// Token: 0x0400077E RID: 1918
		private Dictionary<string, string> dict;

		// Token: 0x0400077F RID: 1919
		private readonly DynamicStampProperties.StampContentList stampContents;

		// Token: 0x02000594 RID: 1428
		public enum FontWeights
		{
			// Token: 0x04001E73 RID: 7795
			Light = 300,
			// Token: 0x04001E74 RID: 7796
			Normal = 400,
			// Token: 0x04001E75 RID: 7797
			Bold = 700
		}

		// Token: 0x02000595 RID: 1429
		public enum ArrowDirections
		{
			// Token: 0x04001E77 RID: 7799
			Right,
			// Token: 0x04001E78 RID: 7800
			Left
		}

		// Token: 0x02000596 RID: 1430
		public enum ContentType
		{
			// Token: 0x04001E7A RID: 7802
			None,
			// Token: 0x04001E7B RID: 7803
			Text,
			// Token: 0x04001E7C RID: 7804
			Time
		}

		// Token: 0x02000597 RID: 1431
		public class StampContent
		{
			// Token: 0x06003185 RID: 12677 RVA: 0x000F2E5A File Offset: 0x000F105A
			internal StampContent(int index, Dictionary<string, string> dict)
			{
				this.index = index;
				this.dict = dict;
			}

			// Token: 0x17000D1E RID: 3358
			// (get) Token: 0x06003186 RID: 12678 RVA: 0x000F2E70 File Offset: 0x000F1070
			// (set) Token: 0x06003187 RID: 12679 RVA: 0x000F2EC4 File Offset: 0x000F10C4
			public DynamicStampProperties.ContentType ContentType
			{
				get
				{
					string text;
					if (this.dict.TryGetValue(string.Format("Content{0}Type", this.index), out text))
					{
						if (string.Equals(text, "Text", StringComparison.OrdinalIgnoreCase))
						{
							return DynamicStampProperties.ContentType.Text;
						}
						if (string.Equals(text, "Time", StringComparison.OrdinalIgnoreCase))
						{
							return DynamicStampProperties.ContentType.Time;
						}
					}
					return DynamicStampProperties.ContentType.None;
				}
				set
				{
					string text = string.Format("Content{0}Type", this.index);
					if (value == DynamicStampProperties.ContentType.Text)
					{
						this.dict[text] = "text";
						return;
					}
					if (value == DynamicStampProperties.ContentType.Time)
					{
						this.dict[text] = "time";
						return;
					}
					this.dict.Remove(text);
				}
			}

			// Token: 0x17000D1F RID: 3359
			// (get) Token: 0x06003188 RID: 12680 RVA: 0x000F2F20 File Offset: 0x000F1120
			// (set) Token: 0x06003189 RID: 12681 RVA: 0x000F2F54 File Offset: 0x000F1154
			public string Content
			{
				get
				{
					string text;
					if (this.dict.TryGetValue(string.Format("Content{0}Text", this.index), out text))
					{
						return text;
					}
					return null;
				}
				set
				{
					if (value != null)
					{
						this.dict[string.Format("Content{0}Text", this.index)] = value;
						return;
					}
					this.dict.Remove(string.Format("Content{0}Text", this.index));
				}
			}

			// Token: 0x17000D20 RID: 3360
			// (get) Token: 0x0600318A RID: 12682 RVA: 0x000F2FA8 File Offset: 0x000F11A8
			// (set) Token: 0x0600318B RID: 12683 RVA: 0x000F2FF8 File Offset: 0x000F11F8
			public double TextMargin
			{
				get
				{
					string text;
					double num;
					if (this.dict.TryGetValue(string.Format("Content{0}TextMargin", this.index), out text) && double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
					{
						return num;
					}
					return 2.0;
				}
				set
				{
					Math.Max(0.0, value);
					this.dict[string.Format("Content{0}TextMargin", this.index)] = value.ToString(CultureInfo.InvariantCulture);
				}
			}

			// Token: 0x04001E7D RID: 7805
			private readonly int index;

			// Token: 0x04001E7E RID: 7806
			private Dictionary<string, string> dict;
		}

		// Token: 0x02000598 RID: 1432
		private class StampContentList : IReadOnlyList<DynamicStampProperties.StampContent>, IReadOnlyCollection<DynamicStampProperties.StampContent>, IEnumerable<DynamicStampProperties.StampContent>, IEnumerable
		{
			// Token: 0x0600318C RID: 12684 RVA: 0x000F3036 File Offset: 0x000F1236
			public StampContentList(Dictionary<string, string> dict)
			{
				this.dict = dict;
				this.stampContent0 = new DynamicStampProperties.StampContent(0, dict);
				this.stampContent1 = new DynamicStampProperties.StampContent(1, dict);
				this.stampContent2 = new DynamicStampProperties.StampContent(2, dict);
			}

			// Token: 0x17000D21 RID: 3361
			public DynamicStampProperties.StampContent this[int index]
			{
				get
				{
					if (index == 0)
					{
						return this.stampContent0;
					}
					if (index == 1)
					{
						return this.stampContent1;
					}
					if (index == 2)
					{
						return this.stampContent2;
					}
					DynamicStampProperties.StampContentList.<get_Item>g__ThrowOutOfRangeException|6_0();
					return null;
				}
			}

			// Token: 0x17000D22 RID: 3362
			// (get) Token: 0x0600318E RID: 12686 RVA: 0x000F3094 File Offset: 0x000F1294
			public int Count
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x0600318F RID: 12687 RVA: 0x000F3097 File Offset: 0x000F1297
			public IEnumerator<DynamicStampProperties.StampContent> GetEnumerator()
			{
				DynamicStampProperties.StampContentList.<GetEnumerator>d__9 <GetEnumerator>d__ = new DynamicStampProperties.StampContentList.<GetEnumerator>d__9(0);
				<GetEnumerator>d__.<>4__this = this;
				return <GetEnumerator>d__;
			}

			// Token: 0x06003190 RID: 12688 RVA: 0x000F30A6 File Offset: 0x000F12A6
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			// Token: 0x06003191 RID: 12689 RVA: 0x000F30AE File Offset: 0x000F12AE
			[CompilerGenerated]
			internal static void <get_Item>g__ThrowOutOfRangeException|6_0()
			{
				throw new IndexOutOfRangeException();
			}

			// Token: 0x04001E7F RID: 7807
			private readonly Dictionary<string, string> dict;

			// Token: 0x04001E80 RID: 7808
			private readonly DynamicStampProperties.StampContent stampContent0;

			// Token: 0x04001E81 RID: 7809
			private readonly DynamicStampProperties.StampContent stampContent1;

			// Token: 0x04001E82 RID: 7810
			private readonly DynamicStampProperties.StampContent stampContent2;
		}
	}
}
