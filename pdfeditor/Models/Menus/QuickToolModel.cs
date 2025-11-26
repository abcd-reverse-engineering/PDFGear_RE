using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200015F RID: 351
	public class QuickToolModel : ObservableObject
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x0005246A File Offset: 0x0005066A
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x00052472 File Offset: 0x00050672
		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			set
			{
				base.SetProperty<bool>(ref this.isVisible, value, "IsVisible");
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x00052487 File Offset: 0x00050687
		// (set) Token: 0x06001528 RID: 5416 RVA: 0x0005248F File Offset: 0x0005068F
		public ICommand Command
		{
			get
			{
				return this.command;
			}
			set
			{
				base.SetProperty<ICommand>(ref this.command, value, "Command");
			}
		}

		// Token: 0x040006FE RID: 1790
		private bool isVisible;

		// Token: 0x040006FF RID: 1791
		private ICommand command;
	}
}
