using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfeditor.Utils;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000184 RID: 388
	public class ToolbarSettingItemModel : ObservableObject
	{
		// Token: 0x06001622 RID: 5666 RVA: 0x00054FAD File Offset: 0x000531AD
		public ToolbarSettingItemModel(ContextMenuItemType type, string configCacheKey)
		{
			this.Type = type;
			this.configCacheKey = configCacheKey;
			this.customCacheKey = !string.IsNullOrEmpty(configCacheKey);
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00054FD2 File Offset: 0x000531D2
		public ToolbarSettingItemModel(ContextMenuItemType type)
			: this(type, "")
		{
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06001624 RID: 5668 RVA: 0x00054FE0 File Offset: 0x000531E0
		// (set) Token: 0x06001625 RID: 5669 RVA: 0x00054FE8 File Offset: 0x000531E8
		public ToolbarSettingModel Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (base.SetProperty<ToolbarSettingModel>(ref this.parent, value, "Parent") && !this.customCacheKey)
				{
					this.configCacheKey = ToolbarSettingConfigHelper.BuildConfigKey(this.Id, this.Type);
				}
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x06001626 RID: 5670 RVA: 0x0005501D File Offset: 0x0005321D
		// (set) Token: 0x06001627 RID: 5671 RVA: 0x00055025 File Offset: 0x00053225
		protected object NontransientSelectedValue
		{
			get
			{
				return this.selectedValue;
			}
			set
			{
				if (!object.Equals(this.selectedValue, value))
				{
					this.selectedValue = value;
					if (!this.inTransientScope)
					{
						this.RaiseSelectedValueChanged();
					}
				}
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06001628 RID: 5672 RVA: 0x0005504D File Offset: 0x0005324D
		// (set) Token: 0x06001629 RID: 5673 RVA: 0x00055064 File Offset: 0x00053264
		public object SelectedValue
		{
			get
			{
				if (!this.inTransientScope)
				{
					return this.selectedValue;
				}
				return this.transientValue;
			}
			set
			{
				bool flag;
				if (this.inTransientScope)
				{
					flag = !object.Equals(this.transientValue, value);
					if (flag)
					{
						this.transientValue = value;
					}
					this.selectedValue = value;
				}
				else
				{
					flag = !object.Equals(this.selectedValue, value);
					if (flag)
					{
						this.selectedValue = value;
					}
				}
				if (flag)
				{
					this.RaiseSelectedValueChanged();
				}
			}
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x0600162A RID: 5674 RVA: 0x000550C1 File Offset: 0x000532C1
		// (set) Token: 0x0600162B RID: 5675 RVA: 0x000550C9 File Offset: 0x000532C9
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

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x0600162C RID: 5676 RVA: 0x000550DE File Offset: 0x000532DE
		// (set) Token: 0x0600162D RID: 5677 RVA: 0x000550E6 File Offset: 0x000532E6
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

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x0600162E RID: 5678 RVA: 0x000550FB File Offset: 0x000532FB
		// (set) Token: 0x0600162F RID: 5679 RVA: 0x00055103 File Offset: 0x00053303
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

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06001630 RID: 5680 RVA: 0x00055118 File Offset: 0x00053318
		// (set) Token: 0x06001631 RID: 5681 RVA: 0x00055120 File Offset: 0x00053320
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

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06001632 RID: 5682 RVA: 0x00055135 File Offset: 0x00053335
		public bool InTransientScope
		{
			get
			{
				return this.inTransientScope;
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06001633 RID: 5683 RVA: 0x0005513D File Offset: 0x0005333D
		public ContextMenuItemType Type { get; }

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06001634 RID: 5684 RVA: 0x00055145 File Offset: 0x00053345
		public ToolbarSettingId Id
		{
			get
			{
				ToolbarSettingModel toolbarSettingModel = this.Parent;
				return ((toolbarSettingModel != null) ? toolbarSettingModel.Id : null) ?? ToolbarSettingId.None;
			}
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x00055162 File Offset: 0x00053362
		public void ExecuteCommand()
		{
			ICommand command = this.Command;
			if (command == null)
			{
				return;
			}
			command.Execute(this);
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00055175 File Offset: 0x00053375
		protected virtual void OnSelectedValueChanged()
		{
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x00055177 File Offset: 0x00053377
		private void RaiseSelectedValueChanged()
		{
			this.OnSelectedValueChanged();
			EventHandler selectedValueChanged = this.SelectedValueChanged;
			if (selectedValueChanged != null)
			{
				selectedValueChanged(this, EventArgs.Empty);
			}
			base.OnPropertyChanged("SelectedValue");
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06001638 RID: 5688 RVA: 0x000551A4 File Offset: 0x000533A4
		// (remove) Token: 0x06001639 RID: 5689 RVA: 0x000551DC File Offset: 0x000533DC
		public event EventHandler SelectedValueChanged;

		// Token: 0x0600163A RID: 5690 RVA: 0x00055214 File Offset: 0x00053414
		public async Task SaveConfigAsync()
		{
			string text = this.configCacheKey;
			if (!string.IsNullOrEmpty(text))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				this.SaveConfigCore(dictionary);
				await ToolbarSettingConfigHelper.SaveConfigAsync(text, dictionary).ConfigureAwait(false);
			}
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x00055258 File Offset: 0x00053458
		public async Task LoadConfigAsync()
		{
			string text = this.configCacheKey;
			if (!string.IsNullOrEmpty(text))
			{
				Dictionary<string, string> dictionary = await ToolbarSettingConfigHelper.LoadConfigAsync(text).ConfigureAwait(false);
				Dictionary<string, string> dict = dictionary;
				if (dict != null)
				{
					await DispatcherHelper.RunAsync(delegate
					{
						this.ApplyConfigCore(dict);
					});
				}
			}
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x0005529B File Offset: 0x0005349B
		protected virtual void SaveConfigCore(Dictionary<string, string> dict)
		{
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0005529D File Offset: 0x0005349D
		protected virtual void ApplyConfigCore(Dictionary<string, string> dict)
		{
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x0005529F File Offset: 0x0005349F
		public void BeginTransient(object value)
		{
			if (!this.inTransientScope)
			{
				this.inTransientScope = true;
				this.transientValue = value;
				base.OnPropertyChanged("InTransientScope");
				base.OnPropertyChanged("SelectedValue");
				this.RaiseSelectedValueChanged();
				return;
			}
			this.SelectedValue = value;
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x000552DB File Offset: 0x000534DB
		public void EndTransient()
		{
			if (this.inTransientScope)
			{
				this.inTransientScope = false;
				this.transientValue = null;
				base.OnPropertyChanged("InTransientScope");
				base.OnPropertyChanged("SelectedValue");
				this.RaiseSelectedValueChanged();
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x00055310 File Offset: 0x00053510
		public bool ApplyTransient(bool endTransient)
		{
			if (this.inTransientScope)
			{
				this.selectedValue = this.transientValue;
				if (endTransient)
				{
					this.inTransientScope = false;
					this.transientValue = null;
					base.OnPropertyChanged("InTransientScope");
					base.OnPropertyChanged("SelectedValue");
					this.RaiseSelectedValueChanged();
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000764 RID: 1892
		private const bool ApplyTransientValueToSelectedValue = true;

		// Token: 0x04000765 RID: 1893
		private ToolbarSettingModel parent;

		// Token: 0x04000766 RID: 1894
		private object selectedValue;

		// Token: 0x04000767 RID: 1895
		private object transientValue;

		// Token: 0x04000768 RID: 1896
		private bool inTransientScope;

		// Token: 0x04000769 RID: 1897
		private ImageSource icon;

		// Token: 0x0400076A RID: 1898
		private string caption;

		// Token: 0x0400076B RID: 1899
		private string name;

		// Token: 0x0400076C RID: 1900
		private ICommand command;

		// Token: 0x0400076D RID: 1901
		private bool customCacheKey;

		// Token: 0x0400076E RID: 1902
		private string configCacheKey;
	}
}
