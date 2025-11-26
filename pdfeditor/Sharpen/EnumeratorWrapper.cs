using System;
using System.Collections.Generic;

namespace Sharpen
{
	// Token: 0x0200000F RID: 15
	public sealed class EnumeratorWrapper<T> : Iterator<T>
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00002CEE File Offset: 0x00000EEE
		public EnumeratorWrapper(object collection, IEnumerator<T> e)
		{
			this._e = e;
			this._collection = collection;
			this._more = e.MoveNext();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002D10 File Offset: 0x00000F10
		public override bool HasNext()
		{
			return this._more;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002D18 File Offset: 0x00000F18
		public override T Next()
		{
			if (!this._more)
			{
				throw new InvalidOperationException();
			}
			this._lastVal = this._e.Current;
			this._more = this._e.MoveNext();
			return this._lastVal;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002D50 File Offset: 0x00000F50
		public override void Remove()
		{
			ICollection<T> collection = this._collection as ICollection<T>;
			if (collection == null)
			{
				throw new NotSupportedException();
			}
			if (this._more && !this._copied)
			{
				List<T> list = new List<T>();
				do
				{
					list.Add(this._e.Current);
				}
				while (this._e.MoveNext());
				this._e = list.GetEnumerator();
				this._e.MoveNext();
				this._copied = true;
			}
			collection.Remove(this._lastVal);
		}

		// Token: 0x0400001F RID: 31
		private readonly object _collection;

		// Token: 0x04000020 RID: 32
		private IEnumerator<T> _e;

		// Token: 0x04000021 RID: 33
		private T _lastVal;

		// Token: 0x04000022 RID: 34
		private bool _more;

		// Token: 0x04000023 RID: 35
		private bool _copied;
	}
}
