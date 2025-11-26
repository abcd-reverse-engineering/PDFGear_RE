using System;
using System.Collections.Generic;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200026D RID: 621
	[Serializable]
	public class ToggleButtonMenu : IMenuItem
	{
		// Token: 0x0600241D RID: 9245 RVA: 0x000A8454 File Offset: 0x000A6654
		public ToggleButtonMenu()
		{
			this.m_SubMenus = new List<IMenuItem>();
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x0600241E RID: 9246 RVA: 0x000A8467 File Offset: 0x000A6667
		// (set) Token: 0x0600241F RID: 9247 RVA: 0x000A846F File Offset: 0x000A666F
		public ImageSource ImageUrl { get; set; }

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x06002420 RID: 9248 RVA: 0x000A8478 File Offset: 0x000A6678
		// (set) Token: 0x06002421 RID: 9249 RVA: 0x000A8480 File Offset: 0x000A6680
		public string Caption { get; set; }

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06002422 RID: 9250 RVA: 0x000A8489 File Offset: 0x000A6689
		// (set) Token: 0x06002423 RID: 9251 RVA: 0x000A8491 File Offset: 0x000A6691
		public bool IsBeginGroup { get; set; }

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06002424 RID: 9252 RVA: 0x000A849A File Offset: 0x000A669A
		// (set) Token: 0x06002425 RID: 9253 RVA: 0x000A84A2 File Offset: 0x000A66A2
		public bool IsEnable { get; set; }

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06002426 RID: 9254 RVA: 0x000A84AB File Offset: 0x000A66AB
		// (set) Token: 0x06002427 RID: 9255 RVA: 0x000A84B3 File Offset: 0x000A66B3
		public bool IsVisible { get; set; }

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06002428 RID: 9256 RVA: 0x000A84BC File Offset: 0x000A66BC
		// (set) Token: 0x06002429 RID: 9257 RVA: 0x000A84C4 File Offset: 0x000A66C4
		public int level { get; set; }

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x0600242A RID: 9258 RVA: 0x000A84CD File Offset: 0x000A66CD
		// (set) Token: 0x0600242B RID: 9259 RVA: 0x000A84D5 File Offset: 0x000A66D5
		public List<IMenuItem> SubMenus
		{
			get
			{
				return this.m_SubMenus;
			}
			set
			{
				this.m_SubMenus = value;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x0600242C RID: 9260 RVA: 0x000A84DE File Offset: 0x000A66DE
		// (set) Token: 0x0600242D RID: 9261 RVA: 0x000A84E6 File Offset: 0x000A66E6
		public TagDataModel TagData
		{
			get
			{
				return this.tagData;
			}
			set
			{
				this.tagData = value;
			}
		}

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x0600242E RID: 9262 RVA: 0x000A84EF File Offset: 0x000A66EF
		// (set) Token: 0x0600242F RID: 9263 RVA: 0x000A84F7 File Offset: 0x000A66F7
		public bool IsChecked { get; set; }

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x06002430 RID: 9264 RVA: 0x000A8500 File Offset: 0x000A6700
		// (set) Token: 0x06002431 RID: 9265 RVA: 0x000A850F File Offset: 0x000A670F
		public RelayCommand<TagDataModel> MenuItemCmd
		{
			get
			{
				RelayCommand<TagDataModel> relayCommand = this.menuItemCmd;
				return this.menuItemCmd;
			}
			set
			{
				this.menuItemCmd = value;
			}
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x000A8518 File Offset: 0x000A6718
		private bool CanMenuItemCmd(TagDataModel param)
		{
			return true;
		}

		// Token: 0x04000F45 RID: 3909
		private List<IMenuItem> m_SubMenus;

		// Token: 0x04000F46 RID: 3910
		private TagDataModel tagData;

		// Token: 0x04000F48 RID: 3912
		private RelayCommand<TagDataModel> menuItemCmd;
	}
}
