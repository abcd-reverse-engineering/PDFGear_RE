using System;
using CommonLib.AppTheme;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000173 RID: 371
	public class SpeedContextMenuItemModel : ContextMenuItemModel
	{
		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x000542FF File Offset: 0x000524FF
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x00054307 File Offset: 0x00052507
		public override bool IsChecked
		{
			get
			{
				return base.IsChecked;
			}
			set
			{
				base.IsChecked = value;
				if (!value)
				{
					base.Icon = null;
					return;
				}
				base.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Speech/Checked.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Speech/Checked.png"));
			}
		}
	}
}
