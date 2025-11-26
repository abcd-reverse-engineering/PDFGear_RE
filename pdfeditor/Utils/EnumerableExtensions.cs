using System;
using System.Collections.Generic;

namespace pdfeditor.Utils
{
	// Token: 0x0200009A RID: 154
	public static class EnumerableExtensions
	{
		// Token: 0x060009F3 RID: 2547 RVA: 0x00032787 File Offset: 0x00030987
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
		{
			key = pair.Key;
			value = pair.Value;
		}
	}
}
