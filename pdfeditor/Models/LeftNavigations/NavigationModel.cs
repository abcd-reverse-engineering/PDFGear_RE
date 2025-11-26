using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models.LeftNavigations
{
	// Token: 0x02000189 RID: 393
	public class NavigationModel : ObservableObject
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x000559E9 File Offset: 0x00053BE9
		public NavigationModel()
		{
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x000559F1 File Offset: 0x00053BF1
		public NavigationModel(string name, string displayName, ImageSource icon, ImageSource selectedIcon)
			: this()
		{
			this.name = name;
			this.displayName = displayName;
			this.icon = icon;
			this.selectedIcon = selectedIcon;
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x0600166F RID: 5743 RVA: 0x00055A16 File Offset: 0x00053C16
		// (set) Token: 0x06001670 RID: 5744 RVA: 0x00055A1E File Offset: 0x00053C1E
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				base.SetProperty<string>(ref this.name, value, "Name");
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06001671 RID: 5745 RVA: 0x00055A33 File Offset: 0x00053C33
		// (set) Token: 0x06001672 RID: 5746 RVA: 0x00055A3B File Offset: 0x00053C3B
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				base.SetProperty<string>(ref this.displayName, value, "DisplayName");
			}
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06001673 RID: 5747 RVA: 0x00055A50 File Offset: 0x00053C50
		// (set) Token: 0x06001674 RID: 5748 RVA: 0x00055A58 File Offset: 0x00053C58
		public ImageSource Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				base.SetProperty<ImageSource>(ref this.icon, value, "Icon");
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06001675 RID: 5749 RVA: 0x00055A6D File Offset: 0x00053C6D
		// (set) Token: 0x06001676 RID: 5750 RVA: 0x00055A75 File Offset: 0x00053C75
		public ImageSource SelectedIcon
		{
			get
			{
				return this.selectedIcon;
			}
			set
			{
				base.SetProperty<ImageSource>(ref this.selectedIcon, value, "SelectedIcon");
			}
		}

		// Token: 0x0400077A RID: 1914
		private string name;

		// Token: 0x0400077B RID: 1915
		private string displayName;

		// Token: 0x0400077C RID: 1916
		private ImageSource icon;

		// Token: 0x0400077D RID: 1917
		private ImageSource selectedIcon;
	}
}
