using System;
using System.Collections.Generic;

namespace Sharpen
{
	// Token: 0x02000010 RID: 16
	public static class Extensions
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00002DD7 File Offset: 0x00000FD7
		public static Iterator<T> Iterator<T>(this IEnumerable<T> col)
		{
			return new EnumeratorWrapper<T>(col, col.GetEnumerator());
		}
	}
}
