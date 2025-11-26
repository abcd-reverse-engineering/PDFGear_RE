using System;
using System.Collections.Generic;
using System.Text;

namespace XmpCore.Impl.XPath
{
	// Token: 0x02000042 RID: 66
	public sealed class XmpPath
	{
		// Token: 0x0600030C RID: 780 RVA: 0x0000E899 File Offset: 0x0000CA99
		public void Add(XmpPathSegment segment)
		{
			this._segments.Add(segment);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000E8A7 File Offset: 0x0000CAA7
		public XmpPathSegment GetSegment(int index)
		{
			return this._segments[index];
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000E8B5 File Offset: 0x0000CAB5
		public int Size()
		{
			return this._segments.Count;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000E8C4 File Offset: 0x0000CAC4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i < this.Size(); i++)
			{
				stringBuilder.Append(this.GetSegment(i));
				if (i < this.Size() - 1)
				{
					XmpPathStepType kind = this.GetSegment(i + 1).Kind;
					if (kind == XmpPathStepType.StructFieldStep || kind == XmpPathStepType.QualifierStep)
					{
						stringBuilder.Append('/');
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000137 RID: 311
		public const int StepSchema = 0;

		// Token: 0x04000138 RID: 312
		public const int StepRootProp = 1;

		// Token: 0x04000139 RID: 313
		private readonly List<XmpPathSegment> _segments = new List<XmpPathSegment>(5);
	}
}
