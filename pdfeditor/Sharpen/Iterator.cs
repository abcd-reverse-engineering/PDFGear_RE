using System;
using System.Collections;
using System.Collections.Generic;

namespace Sharpen
{
	// Token: 0x02000014 RID: 20
	public abstract class Iterator<T> : IEnumerator<T>, IDisposable, IEnumerator, IIterator
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00002E9A File Offset: 0x0000109A
		object IIterator.Next()
		{
			return this.Next();
		}

		// Token: 0x06000053 RID: 83
		public abstract bool HasNext();

		// Token: 0x06000054 RID: 84
		public abstract T Next();

		// Token: 0x06000055 RID: 85
		public abstract void Remove();

		// Token: 0x06000056 RID: 86 RVA: 0x00002EA7 File Offset: 0x000010A7
		bool IEnumerator.MoveNext()
		{
			if (!this.HasNext())
			{
				return false;
			}
			this._lastValue = this.Next();
			return true;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002EC0 File Offset: 0x000010C0
		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002EC7 File Offset: 0x000010C7
		void IDisposable.Dispose()
		{
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002EC9 File Offset: 0x000010C9
		T IEnumerator<T>.Current
		{
			get
			{
				return this._lastValue;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002ED1 File Offset: 0x000010D1
		object IEnumerator.Current
		{
			get
			{
				return this._lastValue;
			}
		}

		// Token: 0x04000027 RID: 39
		private T _lastValue;
	}
}
