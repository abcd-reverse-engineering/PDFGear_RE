using System;
using System.Windows.Media;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000174 RID: 372
	public class ColorMoreItemContextMenuItemModel : MoreItemContextMenuItemModel
	{
		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060015CE RID: 5582 RVA: 0x00054342 File Offset: 0x00052542
		// (set) Token: 0x060015CF RID: 5583 RVA: 0x0005434A File Offset: 0x0005254A
		public Color? DefaultColor
		{
			get
			{
				return this.defaultColor;
			}
			set
			{
				base.SetProperty<Color?>(ref this.defaultColor, value, "DefaultColor");
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060015D0 RID: 5584 RVA: 0x0005435F File Offset: 0x0005255F
		// (set) Token: 0x060015D1 RID: 5585 RVA: 0x00054367 File Offset: 0x00052567
		public string RecentColorsKey
		{
			get
			{
				return this.recentColorsKey;
			}
			set
			{
				base.SetProperty<string>(ref this.recentColorsKey, value, "RecentColorsKey");
			}
		}

		// Token: 0x0400073E RID: 1854
		private Color? defaultColor;

		// Token: 0x0400073F RID: 1855
		private string recentColorsKey;
	}
}
