using System;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000165 RID: 357
	public class ToolbarButtonModel : ObservableObject
	{
		// Token: 0x06001572 RID: 5490 RVA: 0x00053885 File Offset: 0x00051A85
		public ToolbarButtonModel()
		{
			this.caption = "";
			this.tooltip = "";
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06001573 RID: 5491 RVA: 0x000538AA File Offset: 0x00051AAA
		// (set) Token: 0x06001574 RID: 5492 RVA: 0x000538B2 File Offset: 0x00051AB2
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

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x06001575 RID: 5493 RVA: 0x000538C7 File Offset: 0x00051AC7
		// (set) Token: 0x06001576 RID: 5494 RVA: 0x000538CF File Offset: 0x00051ACF
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

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x06001577 RID: 5495 RVA: 0x000538F7 File Offset: 0x00051AF7
		// (set) Token: 0x06001578 RID: 5496 RVA: 0x000538FF File Offset: 0x00051AFF
		public bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
			}
		}

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x06001579 RID: 5497 RVA: 0x00053914 File Offset: 0x00051B14
		// (set) Token: 0x0600157A RID: 5498 RVA: 0x0005391C File Offset: 0x00051B1C
		public ToolbarChildButtonModel ChildButtonModel
		{
			get
			{
				return this.childButtonModel;
			}
			set
			{
				ToolbarChildButtonModel toolbarChildButtonModel = this.childButtonModel;
				if (base.SetProperty<ToolbarChildButtonModel>(ref this.childButtonModel, value, "ChildButtonModel"))
				{
					this.OnChildButtonModelChanged(value, toolbarChildButtonModel);
				}
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0005394C File Offset: 0x00051B4C
		protected virtual void OnChildButtonModelChanged(ToolbarChildButtonModel newValue, ToolbarChildButtonModel oldValue)
		{
			if (oldValue != null)
			{
				oldValue.Parent = null;
			}
			if (newValue != null)
			{
				newValue.Parent = this;
			}
		}

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x0600157C RID: 5500 RVA: 0x00053962 File Offset: 0x00051B62
		// (set) Token: 0x0600157D RID: 5501 RVA: 0x0005396A File Offset: 0x00051B6A
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

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x0600157E RID: 5502 RVA: 0x0005397F File Offset: 0x00051B7F
		// (set) Token: 0x0600157F RID: 5503 RVA: 0x00053987 File Offset: 0x00051B87
		public string Caption
		{
			get
			{
				return this.caption;
			}
			set
			{
				base.SetProperty<string>(ref this.caption, value, "Caption");
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x06001580 RID: 5504 RVA: 0x0005399C File Offset: 0x00051B9C
		// (set) Token: 0x06001581 RID: 5505 RVA: 0x000539A4 File Offset: 0x00051BA4
		public string Tooltip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				base.SetProperty<string>(ref this.tooltip, value, "Tooltip");
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x06001582 RID: 5506 RVA: 0x000539B9 File Offset: 0x00051BB9
		// (set) Token: 0x06001583 RID: 5507 RVA: 0x000539C1 File Offset: 0x00051BC1
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

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x000539D6 File Offset: 0x00051BD6
		// (set) Token: 0x06001585 RID: 5509 RVA: 0x000539DE File Offset: 0x00051BDE
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

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06001586 RID: 5510 RVA: 0x000539F3 File Offset: 0x00051BF3
		// (set) Token: 0x06001587 RID: 5511 RVA: 0x000539FB File Offset: 0x00051BFB
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

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x00053A10 File Offset: 0x00051C10
		// (set) Token: 0x06001589 RID: 5513 RVA: 0x00053A18 File Offset: 0x00051C18
		public SolidColorBrush IndicatorBrush
		{
			get
			{
				return this.indicatorBrush;
			}
			set
			{
				base.SetProperty<SolidColorBrush>(ref this.indicatorBrush, value, "IndicatorBrush");
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00053A2D File Offset: 0x00051C2D
		public void Tap()
		{
			if (this.IsCheckable && !this.IsChecked)
			{
				this.IsChecked = true;
			}
			ICommand command = this.Command;
			if (command == null)
			{
				return;
			}
			command.Execute(this.CommandParameter ?? this);
		}

		// Token: 0x0400071F RID: 1823
		private string name;

		// Token: 0x04000720 RID: 1824
		private bool isCheckable = true;

		// Token: 0x04000721 RID: 1825
		private bool isChecked;

		// Token: 0x04000722 RID: 1826
		private ToolbarChildButtonModel childButtonModel;

		// Token: 0x04000723 RID: 1827
		private ImageSource icon;

		// Token: 0x04000724 RID: 1828
		private string caption;

		// Token: 0x04000725 RID: 1829
		private string tooltip;

		// Token: 0x04000726 RID: 1830
		private bool isEnabled;

		// Token: 0x04000727 RID: 1831
		private ICommand command;

		// Token: 0x04000728 RID: 1832
		private object commandParameter;

		// Token: 0x04000729 RID: 1833
		private SolidColorBrush indicatorBrush;
	}
}
