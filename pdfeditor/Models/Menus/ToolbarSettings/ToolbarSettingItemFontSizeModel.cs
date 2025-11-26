using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000185 RID: 389
	public class ToolbarSettingItemFontSizeModel : ToolbarSettingItemModel
	{
		// Token: 0x06001641 RID: 5697 RVA: 0x00055361 File Offset: 0x00053561
		public ToolbarSettingItemFontSizeModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0005536A File Offset: 0x0005356A
		public ToolbarSettingItemFontSizeModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x00055374 File Offset: 0x00053574
		protected override void OnSelectedValueChanged()
		{
			base.OnSelectedValueChanged();
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06001644 RID: 5700 RVA: 0x0005537C File Offset: 0x0005357C
		// (set) Token: 0x06001645 RID: 5701 RVA: 0x00055384 File Offset: 0x00053584
		public ObservableCollection<float> StandardItems
		{
			get
			{
				return this.standardItems;
			}
			set
			{
				ObservableCollection<float> observableCollection = this.standardItems;
				if (base.SetProperty<ObservableCollection<float>>(ref this.standardItems, value, "StandardItems"))
				{
					if (observableCollection != null)
					{
						WeakEventManager<ObservableCollection<float>, CollectionChangeEventArgs>.RemoveHandler(observableCollection, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					if (this.standardItems != null)
					{
						WeakEventManager<ObservableCollection<float>, CollectionChangeEventArgs>.AddHandler(this.standardItems, "CollectionChanged", new EventHandler<CollectionChangeEventArgs>(this.OnStandardColorsItemChanged));
					}
					EventHandler standardItemsChanged = this.StandardItemsChanged;
					if (standardItemsChanged == null)
					{
						return;
					}
					standardItemsChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x00055400 File Offset: 0x00053600
		private void OnStandardColorsItemChanged(object sender, CollectionChangeEventArgs e)
		{
			EventHandler standardItemsChanged = this.StandardItemsChanged;
			if (standardItemsChanged == null)
			{
				return;
			}
			standardItemsChanged(this, EventArgs.Empty);
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06001647 RID: 5703 RVA: 0x00055418 File Offset: 0x00053618
		// (remove) Token: 0x06001648 RID: 5704 RVA: 0x00055450 File Offset: 0x00053650
		public event EventHandler StandardItemsChanged;

		// Token: 0x06001649 RID: 5705 RVA: 0x00055485 File Offset: 0x00053685
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			dict["SelectedValue"] = base.NontransientSelectedValue.ToString();
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x000554A4 File Offset: 0x000536A4
		protected override void ApplyConfigCore(Dictionary<string, string> dict)
		{
			base.ApplyConfigCore(dict);
			string text;
			float num;
			if (dict.TryGetValue("SelectedValue", out text) && float.TryParse(text, out num))
			{
				base.NontransientSelectedValue = num;
			}
		}

		// Token: 0x04000771 RID: 1905
		private ObservableCollection<float> standardItems;
	}
}
