using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200015A RID: 346
	public class ContextMenuSeparator : ObservableObject, IContextMenuModel, INotifyPropertyChanging, INotifyPropertyChanged
	{
		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x000517C4 File Offset: 0x0004F9C4
		// (set) Token: 0x0600149F RID: 5279 RVA: 0x000517CC File Offset: 0x0004F9CC
		public IContextMenuModel Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (this.parent != value)
				{
					base.OnPropertyChanging("Level");
				}
				if (base.SetProperty<IContextMenuModel>(ref this.parent, value, "Parent"))
				{
					base.OnPropertyChanged("Level");
				}
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x00051801 File Offset: 0x0004FA01
		public int Level
		{
			get
			{
				IContextMenuModel contextMenuModel = this.Parent;
				if (contextMenuModel == null)
				{
					return -1;
				}
				return contextMenuModel.Level + 1;
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x00051816 File Offset: 0x0004FA16
		public string Name
		{
			get
			{
				return "Separator";
			}
		}

		// Token: 0x040006CF RID: 1743
		private IContextMenuModel parent;
	}
}
