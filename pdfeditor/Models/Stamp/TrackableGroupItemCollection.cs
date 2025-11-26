using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace pdfeditor.Models.Stamp
{
	// Token: 0x0200013A RID: 314
	public class TrackableGroupItemCollection : ObservableCollection<GroupItem>
	{
		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06001324 RID: 4900 RVA: 0x0004E494 File Offset: 0x0004C694
		// (remove) Token: 0x06001325 RID: 4901 RVA: 0x0004E4CC File Offset: 0x0004C6CC
		public event EventHandler<PropertyChangedEventArgs> GroupItemPropertyChanged;

		// Token: 0x06001326 RID: 4902 RVA: 0x0004E501 File Offset: 0x0004C701
		protected override void InsertItem(int index, GroupItem item)
		{
			base.InsertItem(index, item);
			if (item != null)
			{
				item.PropertyChanged += this.OnGroupItemPropertyChanged;
			}
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0004E520 File Offset: 0x0004C720
		protected override void RemoveItem(int index)
		{
			if (base[index] != null)
			{
				base[index].PropertyChanged -= this.OnGroupItemPropertyChanged;
			}
			base.RemoveItem(index);
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0004E54C File Offset: 0x0004C74C
		protected override void ClearItems()
		{
			foreach (GroupItem groupItem in this)
			{
				if (groupItem != null)
				{
					groupItem.PropertyChanged -= this.OnGroupItemPropertyChanged;
				}
			}
			base.ClearItems();
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0004E5A8 File Offset: 0x0004C7A8
		protected override void SetItem(int index, GroupItem item)
		{
			if (base[index] != null)
			{
				base[index].PropertyChanged -= this.OnGroupItemPropertyChanged;
			}
			base.SetItem(index, item);
			if (item != null)
			{
				item.PropertyChanged += this.OnGroupItemPropertyChanged;
			}
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0004E5E8 File Offset: 0x0004C7E8
		private void OnGroupItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			EventHandler<PropertyChangedEventArgs> groupItemPropertyChanged = this.GroupItemPropertyChanged;
			if (groupItemPropertyChanged == null)
			{
				return;
			}
			groupItemPropertyChanged(sender, e);
		}
	}
}
