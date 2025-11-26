using System;
using Sharpen;

namespace XmpCore
{
	// Token: 0x02000018 RID: 24
	public interface IXmpIterator : IIterator
	{
		// Token: 0x0600007B RID: 123
		void SkipSubtree();

		// Token: 0x0600007C RID: 124
		void SkipSiblings();
	}
}
