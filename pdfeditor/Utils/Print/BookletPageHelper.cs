using System;
using System.Collections.Generic;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000BD RID: 189
	public static class BookletPageHelper
	{
		// Token: 0x06000B3C RID: 2876 RVA: 0x00039C38 File Offset: 0x00037E38
		public static List<int> GetBookletPageOrder(int totalPages, BookletBindingDirection BindingDirection, bool PrintReverse, BookletSubset bookletSubset)
		{
			List<int> list = new List<int>();
			int num = totalPages;
			if (totalPages % 4 != 0)
			{
				num = (totalPages / 4 + 1) * 4;
			}
			if (BindingDirection == BookletBindingDirection.Left)
			{
				for (int i = 0; i < num / 4; i++)
				{
					if (bookletSubset != BookletSubset.Back)
					{
						list.Add(num - 2 * i);
						list.Add(2 * i + 1);
					}
					if (bookletSubset != BookletSubset.Frontal)
					{
						list.Add(2 * i + 2);
						list.Add(num - 2 * i - 1);
					}
				}
			}
			else
			{
				for (int j = 0; j < num / 4; j++)
				{
					if (bookletSubset != BookletSubset.Back)
					{
						list.Add(2 * j + 1);
						list.Add(num - 2 * j);
					}
					if (bookletSubset != BookletSubset.Frontal)
					{
						list.Add(num - 2 * j - 1);
						list.Add(2 * j + 2);
					}
				}
			}
			try
			{
				if (PrintReverse)
				{
					list.Reverse();
					for (int k = 0; k < num; k++)
					{
						if (k % 2 == 0)
						{
							int num2 = list[k];
							list[k] = list[k + 1];
							list[k + 1] = num2;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return list;
		}
	}
}
