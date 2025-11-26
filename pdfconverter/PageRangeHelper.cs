using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace pdfconverter
{
	// Token: 0x0200001F RID: 31
	public static class PageRangeHelper
	{
		// Token: 0x060000DA RID: 218 RVA: 0x00003CB8 File Offset: 0x00001EB8
		public static bool TryParsePageRange(string range, out int[] pageIndexes, out int errorCharIndex)
		{
			pageIndexes = null;
			int[][] array;
			if (PageRangeHelper.TryParsePageRangeCore(range, out array, out errorCharIndex))
			{
				pageIndexes = (from c in array.SelectMany((int[] c) => c).Distinct<int>()
					orderby c
					select c).ToArray<int>();
				return true;
			}
			return false;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003D2B File Offset: 0x00001F2B
		public static bool TryParsePageRange2(string range, out int[][] pageIndexes, out int errorCharIndex)
		{
			return PageRangeHelper.TryParsePageRangeCore(range, out pageIndexes, out errorCharIndex);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00003D38 File Offset: 0x00001F38
		private static bool TryParsePageRangeCore(string range, out int[][] pageIndexes, out int errorCharIndex)
		{
			pageIndexes = null;
			errorCharIndex = -1;
			if (string.IsNullOrEmpty(range))
			{
				return false;
			}
			PageRangeHelper.<>c__DisplayClass2_0 CS$<>8__locals1;
			CS$<>8__locals1.list = new List<List<int>>();
			PageRangeHelper.PageRangeReader pageRangeReader = new PageRangeHelper.PageRangeReader(range);
			CS$<>8__locals1.from = -1;
			CS$<>8__locals1.to = -1;
			CS$<>8__locals1.isTo = false;
			int num = -1;
			while (pageRangeReader.HasMore)
			{
				global::System.ValueTuple<PageRangeHelper.PageRangeTokenType, string, int> nextToken = pageRangeReader.GetNextToken();
				PageRangeHelper.PageRangeTokenType item = nextToken.Item1;
				string item2 = nextToken.Item2;
				int item3 = nextToken.Item3;
				num = item3;
				switch (item)
				{
				case PageRangeHelper.PageRangeTokenType.Number:
					if (!CS$<>8__locals1.isTo)
					{
						if (CS$<>8__locals1.from != -1)
						{
							errorCharIndex = item3;
							return false;
						}
						if (!int.TryParse(item2, out CS$<>8__locals1.from))
						{
							errorCharIndex = item3;
							return false;
						}
						continue;
					}
					else
					{
						if (CS$<>8__locals1.to != -1)
						{
							errorCharIndex = item3;
							return false;
						}
						if (!int.TryParse(item2, out CS$<>8__locals1.to))
						{
							errorCharIndex = item3;
							return false;
						}
						if (CS$<>8__locals1.to < CS$<>8__locals1.from)
						{
							errorCharIndex = item3;
							return false;
						}
						continue;
					}
					break;
				case PageRangeHelper.PageRangeTokenType.Dash:
					if (((CS$<>8__locals1.from == -1) | CS$<>8__locals1.isTo) || CS$<>8__locals1.to != -1)
					{
						errorCharIndex = item3;
						return false;
					}
					CS$<>8__locals1.isTo = true;
					continue;
				case PageRangeHelper.PageRangeTokenType.Comma:
					if (!PageRangeHelper.<TryParsePageRangeCore>g__Complete|2_1(ref CS$<>8__locals1))
					{
						errorCharIndex = item3;
						return false;
					}
					continue;
				}
				if (pageRangeReader.HasMore)
				{
					errorCharIndex = item3;
					return false;
				}
			}
			if (!PageRangeHelper.<TryParsePageRangeCore>g__Complete|2_1(ref CS$<>8__locals1))
			{
				errorCharIndex = num;
				return false;
			}
			pageIndexes = CS$<>8__locals1.list.Select((List<int> c) => c.OrderBy((int x) => x).ToArray<int>()).ToArray<int[]>();
			return true;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00003EC0 File Offset: 0x000020C0
		public static string ConvertToRange(this IEnumerable<int> pageIndexes)
		{
			int[] array;
			return pageIndexes.ConvertToRange(out array);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00003ED8 File Offset: 0x000020D8
		public static string ConvertToRange(this IEnumerable<int> pageIndexes, out int[] sortedPageIndexes)
		{
			sortedPageIndexes = null;
			if (pageIndexes == null)
			{
				return string.Empty;
			}
			int[] array = pageIndexes.ToArray<int>();
			if (array.Length == 0)
			{
				return string.Empty;
			}
			Array.Sort<int>(array);
			sortedPageIndexes = array;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(array[0] + 1);
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] != array[i - 1])
				{
					if (array[i] - 1 == array[i - 1])
					{
						flag = true;
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append('-').Append(array[i - 1] + 1);
							flag = false;
						}
						stringBuilder.Append(',').Append(array[i] + 1);
					}
				}
			}
			if (flag)
			{
				stringBuilder.Append('-').Append(array[array.Length - 1] + 1);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00003F94 File Offset: 0x00002194
		[CompilerGenerated]
		internal static bool <TryParsePageRangeCore>g__Complete|2_1(ref PageRangeHelper.<>c__DisplayClass2_0 A_0)
		{
			if (A_0.from == -1)
			{
				return false;
			}
			if (A_0.to == -1)
			{
				if (A_0.isTo)
				{
					return false;
				}
				A_0.list.Add(new List<int> { A_0.from - 1 });
			}
			else if (A_0.from < A_0.to)
			{
				A_0.list.Add(new List<int>(Enumerable.Range(A_0.from - 1, A_0.to - A_0.from + 1)));
			}
			else
			{
				if (A_0.from != A_0.to)
				{
					return false;
				}
				A_0.list.Add(new List<int> { A_0.from - 1 });
			}
			A_0.from = -1;
			A_0.to = -1;
			A_0.isTo = false;
			return true;
		}

		// Token: 0x020000BD RID: 189
		private class PageRangeReader
		{
			// Token: 0x060007A1 RID: 1953 RVA: 0x0001D34E File Offset: 0x0001B54E
			public PageRangeReader(string pageRange)
			{
				if (string.IsNullOrEmpty(pageRange))
				{
					throw new ArgumentException("pageRange");
				}
				this.pageRange = pageRange;
				this.sb = new StringBuilder();
				this.curType = PageRangeHelper.PageRangeTokenType.None;
			}

			// Token: 0x17000260 RID: 608
			// (get) Token: 0x060007A2 RID: 1954 RVA: 0x0001D382 File Offset: 0x0001B582
			public bool HasMore
			{
				get
				{
					return this.curIdx < this.pageRange.Length;
				}
			}

			// Token: 0x060007A3 RID: 1955 RVA: 0x0001D398 File Offset: 0x0001B598
			[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "type", "value", "startIdx" })]
			public global::System.ValueTuple<PageRangeHelper.PageRangeTokenType, string, int> GetNextToken()
			{
				int num = this.curIdx;
				while (this.curIdx < this.pageRange.Length)
				{
					char c = this.pageRange[this.curIdx];
					if (c >= '0' && c <= '9')
					{
						if (this.curType == PageRangeHelper.PageRangeTokenType.None)
						{
							this.curType = PageRangeHelper.PageRangeTokenType.Number;
						}
						this.sb.Append(c);
					}
					else if (c == ' ')
					{
						if (this.curType == PageRangeHelper.PageRangeTokenType.Number)
						{
							break;
						}
						num++;
					}
					else if (c == ',')
					{
						if (this.curType == PageRangeHelper.PageRangeTokenType.None)
						{
							this.curType = PageRangeHelper.PageRangeTokenType.Comma;
							this.sb.Append(c);
							this.curIdx++;
							break;
						}
						if (this.curType == PageRangeHelper.PageRangeTokenType.Number)
						{
							break;
						}
						if (this.curType == PageRangeHelper.PageRangeTokenType.Dash)
						{
							break;
						}
					}
					else
					{
						if (c != '-')
						{
							this.curType = PageRangeHelper.PageRangeTokenType.None;
							this.sb.Length = 0;
							break;
						}
						if (this.curType == PageRangeHelper.PageRangeTokenType.None)
						{
							this.curType = PageRangeHelper.PageRangeTokenType.Dash;
							this.sb.Append(c);
							this.curIdx++;
							break;
						}
						if (this.curType == PageRangeHelper.PageRangeTokenType.Number)
						{
							break;
						}
						if (this.curType == PageRangeHelper.PageRangeTokenType.Comma)
						{
							break;
						}
					}
					this.curIdx++;
				}
				PageRangeHelper.PageRangeTokenType pageRangeTokenType = this.curType;
				string text = this.sb.ToString();
				this.curType = PageRangeHelper.PageRangeTokenType.None;
				this.sb.Length = 0;
				return new global::System.ValueTuple<PageRangeHelper.PageRangeTokenType, string, int>(pageRangeTokenType, text, num);
			}

			// Token: 0x04000407 RID: 1031
			private readonly string pageRange;

			// Token: 0x04000408 RID: 1032
			private int curIdx;

			// Token: 0x04000409 RID: 1033
			private StringBuilder sb;

			// Token: 0x0400040A RID: 1034
			private PageRangeHelper.PageRangeTokenType curType;
		}

		// Token: 0x020000BE RID: 190
		private enum PageRangeTokenType
		{
			// Token: 0x0400040C RID: 1036
			None,
			// Token: 0x0400040D RID: 1037
			Number,
			// Token: 0x0400040E RID: 1038
			Dash,
			// Token: 0x0400040F RID: 1039
			Comma
		}
	}
}
