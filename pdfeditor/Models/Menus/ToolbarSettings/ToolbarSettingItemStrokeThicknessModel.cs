using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000186 RID: 390
	public class ToolbarSettingItemStrokeThicknessModel : ToolbarSettingItemModel
	{
		// Token: 0x0600164B RID: 5707 RVA: 0x000554DD File Offset: 0x000536DD
		public ToolbarSettingItemStrokeThicknessModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x000554E6 File Offset: 0x000536E6
		public ToolbarSettingItemStrokeThicknessModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x000554F0 File Offset: 0x000536F0
		protected override void OnSelectedValueChanged()
		{
			base.OnSelectedValueChanged();
			object selectedValue = base.SelectedValue;
			if (!(selectedValue is float))
			{
				this.TransientItem = null;
				return;
			}
			float num = (float)selectedValue;
			if (this.StandardItems == null || !this.StandardItems.Contains(num))
			{
				this.TransientItem = new float?(num);
				return;
			}
			this.TransientItem = null;
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x0600164E RID: 5710 RVA: 0x0005555B File Offset: 0x0005375B
		// (set) Token: 0x0600164F RID: 5711 RVA: 0x00055564 File Offset: 0x00053764
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

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06001650 RID: 5712 RVA: 0x000555E0 File Offset: 0x000537E0
		// (set) Token: 0x06001651 RID: 5713 RVA: 0x000555E8 File Offset: 0x000537E8
		public float? TransientItem
		{
			get
			{
				return this.transientItem;
			}
			set
			{
				if (base.SetProperty<float?>(ref this.transientItem, value, "TransientItem"))
				{
					EventHandler transientItemChanged = this.TransientItemChanged;
					if (transientItemChanged == null)
					{
						return;
					}
					transientItemChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x00055614 File Offset: 0x00053814
		private void OnStandardColorsItemChanged(object sender, CollectionChangeEventArgs e)
		{
			EventHandler standardItemsChanged = this.StandardItemsChanged;
			if (standardItemsChanged == null)
			{
				return;
			}
			standardItemsChanged(this, EventArgs.Empty);
		}

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06001653 RID: 5715 RVA: 0x0005562C File Offset: 0x0005382C
		// (remove) Token: 0x06001654 RID: 5716 RVA: 0x00055664 File Offset: 0x00053864
		public event EventHandler StandardItemsChanged;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06001655 RID: 5717 RVA: 0x0005569C File Offset: 0x0005389C
		// (remove) Token: 0x06001656 RID: 5718 RVA: 0x000556D4 File Offset: 0x000538D4
		public event EventHandler TransientItemChanged;

		// Token: 0x06001657 RID: 5719 RVA: 0x00055709 File Offset: 0x00053909
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			dict["SelectedValue"] = base.NontransientSelectedValue.ToString();
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00055728 File Offset: 0x00053928
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

		// Token: 0x04000773 RID: 1907
		private ObservableCollection<float> standardItems;

		// Token: 0x04000774 RID: 1908
		private float? transientItem;
	}
}
