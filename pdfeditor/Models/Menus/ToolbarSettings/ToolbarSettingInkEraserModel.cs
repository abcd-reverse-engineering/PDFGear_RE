using System;
using System.Windows.Media;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000178 RID: 376
	public class ToolbarSettingInkEraserModel : ToolbarSettingItemModel
	{
		// Token: 0x060015DF RID: 5599 RVA: 0x0005446C File Offset: 0x0005266C
		public ToolbarSettingInkEraserModel()
			: base(ContextMenuItemType.None)
		{
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060015E0 RID: 5600 RVA: 0x00054487 File Offset: 0x00052687
		// (set) Token: 0x060015E1 RID: 5601 RVA: 0x0005448F File Offset: 0x0005268F
		public bool IsCheckable
		{
			get
			{
				return this.isCheckable;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.isCheckable, value, "IsCheckable") && !value && this.IsChecked)
				{
					this.IsChecked = false;
				}
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x000544B7 File Offset: 0x000526B7
		// (set) Token: 0x060015E3 RID: 5603 RVA: 0x000544BF File Offset: 0x000526BF
		public int SelectSize
		{
			get
			{
				return this.selectSize;
			}
			set
			{
				ConfigManager.SetEraserSize(value);
				base.SetProperty<int>(ref this.selectSize, value, "SelectSize");
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x000544DA File Offset: 0x000526DA
		// (set) Token: 0x060015E5 RID: 5605 RVA: 0x000544E2 File Offset: 0x000526E2
		public ToolbarSettingInkEraserModel.EraserType IsPartial
		{
			get
			{
				return this.isPartial;
			}
			set
			{
				base.SetProperty<ToolbarSettingInkEraserModel.EraserType>(ref this.isPartial, value, "IsPartial");
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x000544F7 File Offset: 0x000526F7
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x000544FF File Offset: 0x000526FF
		public string InkName
		{
			get
			{
				return this.inkName;
			}
			set
			{
				base.SetProperty<string>(ref this.inkName, value, "InkName");
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060015E8 RID: 5608 RVA: 0x00054514 File Offset: 0x00052714
		public ImageSource EraserImage
		{
			get
			{
				return ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Eraser.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Eraser.png"));
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x0005452F File Offset: 0x0005272F
		// (set) Token: 0x060015EA RID: 5610 RVA: 0x00054538 File Offset: 0x00052738
		public bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				if (requiredService.AnnotationToolbar != null)
				{
					if (value)
					{
						requiredService.AnnotationToolbar.InkButtonModel.IsChecked = value;
						requiredService.SelectedAnnotation = null;
						if (this.IsPartial == ToolbarSettingInkEraserModel.EraserType.None)
						{
							if (ConfigManager.GetEraserMode() == ToolbarSettingInkEraserModel.EraserType.Partial.ToString())
							{
								this.IsPartial = ToolbarSettingInkEraserModel.EraserType.Partial;
							}
							else
							{
								this.IsPartial = ToolbarSettingInkEraserModel.EraserType.Whole;
							}
						}
						ConfigManager.SetEraserMode(this.IsPartial.ToString());
					}
					if (!value && this.isPartial != ToolbarSettingInkEraserModel.EraserType.None)
					{
						this.IsPartial = ToolbarSettingInkEraserModel.EraserType.None;
					}
				}
				base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
			}
		}

		// Token: 0x04000752 RID: 1874
		private bool isCheckable = true;

		// Token: 0x04000753 RID: 1875
		private bool isChecked;

		// Token: 0x04000754 RID: 1876
		private string inkName = "Ink";

		// Token: 0x04000755 RID: 1877
		private ToolbarSettingInkEraserModel.EraserType isPartial;

		// Token: 0x04000756 RID: 1878
		private int selectSize;

		// Token: 0x02000589 RID: 1417
		public enum EraserType
		{
			// Token: 0x04001E4D RID: 7757
			None,
			// Token: 0x04001E4E RID: 7758
			Partial,
			// Token: 0x04001E4F RID: 7759
			Whole
		}
	}
}
