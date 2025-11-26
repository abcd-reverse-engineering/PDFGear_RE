using System;

namespace Sharpen
{
	// Token: 0x02000013 RID: 19
	public interface IIterator
	{
		// Token: 0x0600004F RID: 79
		bool HasNext();

		// Token: 0x06000050 RID: 80
		object Next();

		// Token: 0x06000051 RID: 81
		void Remove();
	}
}
