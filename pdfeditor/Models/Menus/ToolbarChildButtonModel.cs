using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000167 RID: 359
	public class ToolbarChildButtonModel : ObservableObject
	{
		// Token: 0x06001596 RID: 5526 RVA: 0x00053BC8 File Offset: 0x00051DC8
		public ToolbarChildButtonModel()
		{
			this.isDropDownIconVisible = true;
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x00053BDE File Offset: 0x00051DDE
		// (set) Token: 0x06001598 RID: 5528 RVA: 0x00053BE6 File Offset: 0x00051DE6
		public bool IsDropDownIconVisible
		{
			get
			{
				return this.isDropDownIconVisible;
			}
			set
			{
				base.SetProperty<bool>(ref this.isDropDownIconVisible, value, "IsDropDownIconVisible");
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x00053BFB File Offset: 0x00051DFB
		// (set) Token: 0x0600159A RID: 5530 RVA: 0x00053C03 File Offset: 0x00051E03
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

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x0600159B RID: 5531 RVA: 0x00053C18 File Offset: 0x00051E18
		// (set) Token: 0x0600159C RID: 5532 RVA: 0x00053C20 File Offset: 0x00051E20
		public object CommandParameter
		{
			get
			{
				return this.commandParameter;
			}
			set
			{
				base.SetProperty<object>(ref this.commandParameter, value, "CommandParameter");
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x00053C35 File Offset: 0x00051E35
		public virtual void Tap()
		{
			ICommand command = this.Command;
			if (command == null)
			{
				return;
			}
			command.Execute(this.CommandParameter ?? this);
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x0600159E RID: 5534 RVA: 0x00053C52 File Offset: 0x00051E52
		// (set) Token: 0x0600159F RID: 5535 RVA: 0x00053C5A File Offset: 0x00051E5A
		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.isEnabled, value, "IsEnabled");
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x060015A0 RID: 5536 RVA: 0x00053C6F File Offset: 0x00051E6F
		// (set) Token: 0x060015A1 RID: 5537 RVA: 0x00053C77 File Offset: 0x00051E77
		public ToolbarButtonModel Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.parent, value, "Parent");
			}
		}

		// Token: 0x0400072E RID: 1838
		private bool isDropDownIconVisible;

		// Token: 0x0400072F RID: 1839
		private ICommand command;

		// Token: 0x04000730 RID: 1840
		private object commandParameter;

		// Token: 0x04000731 RID: 1841
		private bool isEnabled = true;

		// Token: 0x04000732 RID: 1842
		private ToolbarButtonModel parent;
	}
}
