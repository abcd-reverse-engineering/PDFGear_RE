using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000161 RID: 353
	public class SelectableContextMenuItemModel : ContextMenuItemModel
	{
		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x0005331E File Offset: 0x0005151E
		public ContextMenuItemModel SelectedItem
		{
			get
			{
				return this.OfType<ContextMenuItemModel>().FirstOrDefault((ContextMenuItemModel c) => c.IsChecked && c.IsEndPoint);
			}
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x0005334C File Offset: 0x0005154C
		protected override void InsertItem(int index, IContextMenuModel item)
		{
			base.InsertItem(index, item);
			if (item != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.Notify_PropertyChanged));
			}
			this.OnItemPropertyChanged(item as ContextMenuItemModel);
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x0005338C File Offset: 0x0005158C
		protected override void SetItem(int index, IContextMenuModel item)
		{
			INotifyPropertyChanged notifyPropertyChanged = base[index];
			if (notifyPropertyChanged != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.Notify_PropertyChanged));
			}
			base.SetItem(index, item);
			if (item != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.Notify_PropertyChanged));
			}
			this.OnItemPropertyChanged(item as ContextMenuItemModel);
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000533EC File Offset: 0x000515EC
		protected override void RemoveItem(int index)
		{
			INotifyPropertyChanged notifyPropertyChanged = base[index];
			if (notifyPropertyChanged != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.Notify_PropertyChanged));
			}
			base.RemoveItem(index);
			this.OnItemPropertyChanged(null);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x0005342C File Offset: 0x0005162C
		protected override void ClearItems()
		{
			foreach (INotifyPropertyChanged notifyPropertyChanged in this)
			{
				if (notifyPropertyChanged != null)
				{
					WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.Notify_PropertyChanged));
				}
			}
			base.ClearItems();
			this.OnItemPropertyChanged(null);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00053494 File Offset: 0x00051694
		private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsChecked")
			{
				this.OnItemPropertyChanged((ContextMenuItemModel)sender);
			}
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000534B4 File Offset: 0x000516B4
		private void OnItemPropertyChanged(ContextMenuItemModel item)
		{
			if (this.innerSet)
			{
				return;
			}
			try
			{
				this.innerSet = true;
				if (item != null && item.IsChecked)
				{
					foreach (ContextMenuItemModel contextMenuItemModel in this.OfType<ContextMenuItemModel>())
					{
						if (contextMenuItemModel != item)
						{
							contextMenuItemModel.IsChecked = false;
						}
					}
				}
				base.OnPropertyChanged("SelectedItem");
				SelectableModelSelectionChangedEventHandler selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(this, new SelectableModelSelectionChangedEventArgs(this.SelectedItem));
				}
			}
			finally
			{
				this.innerSet = false;
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001560 RID: 5472 RVA: 0x00053560 File Offset: 0x00051760
		// (remove) Token: 0x06001561 RID: 5473 RVA: 0x00053598 File Offset: 0x00051798
		public event SelectableModelSelectionChangedEventHandler SelectionChanged;

		// Token: 0x04000718 RID: 1816
		private bool innerSet;
	}
}
