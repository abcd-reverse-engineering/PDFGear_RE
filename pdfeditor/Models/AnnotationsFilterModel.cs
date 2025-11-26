using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models
{
	// Token: 0x02000126 RID: 294
	public class AnnotationsFilterModel : ObservableObject
	{
		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x00049A7E File Offset: 0x00047C7E
		// (set) Token: 0x06001204 RID: 4612 RVA: 0x00049A86 File Offset: 0x00047C86
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x00049A8F File Offset: 0x00047C8F
		// (set) Token: 0x06001206 RID: 4614 RVA: 0x00049A97 File Offset: 0x00047C97
		public bool IsSelect
		{
			get
			{
				return this.isSelect;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelect, value, "IsSelect");
			}
		}

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x00049AAC File Offset: 0x00047CAC
		// (set) Token: 0x06001208 RID: 4616 RVA: 0x00049AB4 File Offset: 0x00047CB4
		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				base.SetProperty<int>(ref this.count, value, "Count");
			}
		}

		// Token: 0x040005B2 RID: 1458
		private string text;

		// Token: 0x040005B3 RID: 1459
		private bool isSelect = true;

		// Token: 0x040005B4 RID: 1460
		private int count = 1;
	}
}
