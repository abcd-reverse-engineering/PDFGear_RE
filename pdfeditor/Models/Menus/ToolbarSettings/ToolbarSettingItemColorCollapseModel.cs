using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x0200017C RID: 380
	public class ToolbarSettingItemColorCollapseModel : ToolbarSettingItemModel
	{
		// Token: 0x060015F7 RID: 5623 RVA: 0x00054759 File Offset: 0x00052959
		public ToolbarSettingItemColorCollapseModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x00054762 File Offset: 0x00052962
		public ToolbarSettingItemColorCollapseModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x060015F9 RID: 5625 RVA: 0x0005476C File Offset: 0x0005296C
		// (set) Token: 0x060015FA RID: 5626 RVA: 0x00054774 File Offset: 0x00052974
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

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x060015FB RID: 5627 RVA: 0x00054789 File Offset: 0x00052989
		// (set) Token: 0x060015FC RID: 5628 RVA: 0x00054794 File Offset: 0x00052994
		public ObservableCollection<string> StandardColors
		{
			get
			{
				return this.standardColors;
			}
			set
			{
				ObservableCollection<string> observableCollection = this.standardColors;
				if (base.SetProperty<ObservableCollection<string>>(ref this.standardColors, value, "StandardColors"))
				{
					if (observableCollection != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.RemoveHandler(observableCollection, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					if (this.standardColors != null)
					{
						WeakEventManager<ObservableCollection<string>, CollectionChangeEventArgs>.AddHandler(this.standardColors, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					EventHandler colorsChanged = this.ColorsChanged;
					if (colorsChanged == null)
					{
						return;
					}
					colorsChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00054810 File Offset: 0x00052A10
		private void OnStandardColorsItemChanged(object sender, CollectionChangeEventArgs e)
		{
			EventHandler colorsChanged = this.ColorsChanged;
			if (colorsChanged == null)
			{
				return;
			}
			colorsChanged(this, EventArgs.Empty);
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060015FE RID: 5630 RVA: 0x00054828 File Offset: 0x00052A28
		// (remove) Token: 0x060015FF RID: 5631 RVA: 0x00054860 File Offset: 0x00052A60
		public event EventHandler ColorsChanged;

		// Token: 0x06001600 RID: 5632 RVA: 0x00054895 File Offset: 0x00052A95
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			dict["SelectedValue"] = (string)base.NontransientSelectedValue;
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000548B4 File Offset: 0x00052AB4
		protected override void ApplyConfigCore(Dictionary<string, string> dict)
		{
			base.ApplyConfigCore(dict);
			string text;
			if (dict.TryGetValue("SelectedValue", out text))
			{
				base.NontransientSelectedValue = text;
			}
		}

		// Token: 0x04000759 RID: 1881
		private string recentColorsKey;

		// Token: 0x0400075A RID: 1882
		private ObservableCollection<string> standardColors;
	}
}
