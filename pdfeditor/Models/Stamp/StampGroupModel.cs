using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace pdfeditor.Models.Stamp
{
	// Token: 0x02000138 RID: 312
	public class StampGroupModel : INotifyPropertyChanged
	{
		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x0600130A RID: 4874 RVA: 0x0004E0E7 File Offset: 0x0004C2E7
		// (set) Token: 0x0600130B RID: 4875 RVA: 0x0004E0EF File Offset: 0x0004C2EF
		public TrackableGroupItemCollection Items { get; set; } = new TrackableGroupItemCollection();

		// Token: 0x0600130C RID: 4876 RVA: 0x0004E0F8 File Offset: 0x0004C2F8
		public StampGroupModel()
		{
			this.Items.CollectionChanged += this.OnItemsCollectionChanged;
			this.Items.GroupItemPropertyChanged += this.OnGroupItemPropertyChanged;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0004E144 File Offset: 0x0004C344
		private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("ItemCount");
			this.OnPropertyChanged("Items");
			this.UpdateAllItemSelected();
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0004E162 File Offset: 0x0004C362
		private void OnGroupItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsChecked")
			{
				this.UpdateAllItemSelected();
			}
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0004E17C File Offset: 0x0004C37C
		private void UpdateAllItemSelected()
		{
			if (this.Items.Count == 0)
			{
				this.AllItemSelected = new bool?(false);
				return;
			}
			int num = this.Items.Count((GroupItem item) => item.IsChecked);
			this._isUpdatingFromChild = true;
			try
			{
				if (num == 0)
				{
					this.AllItemSelected = new bool?(false);
				}
				else if (num == this.Items.Count)
				{
					this.AllItemSelected = new bool?(true);
				}
				else
				{
					this.AllItemSelected = null;
				}
			}
			finally
			{
				this._isUpdatingFromChild = false;
			}
		}

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06001310 RID: 4880 RVA: 0x0004E22C File Offset: 0x0004C42C
		public int ItemCount
		{
			get
			{
				return this.Items.Count;
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x0004E239 File Offset: 0x0004C439
		// (set) Token: 0x06001312 RID: 4882 RVA: 0x0004E241 File Offset: 0x0004C441
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name != value)
				{
					this._name = value;
				}
				this.OnPropertyChanged("Name");
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06001313 RID: 4883 RVA: 0x0004E263 File Offset: 0x0004C463
		// (set) Token: 0x06001314 RID: 4884 RVA: 0x0004E26B File Offset: 0x0004C46B
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
				}
				this.OnPropertyChanged("IsSelected");
			}
		}

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06001315 RID: 4885 RVA: 0x0004E288 File Offset: 0x0004C488
		// (set) Token: 0x06001316 RID: 4886 RVA: 0x0004E290 File Offset: 0x0004C490
		public bool? AllItemSelected
		{
			get
			{
				return this._allItemSelected;
			}
			set
			{
				bool? allItemSelected = this._allItemSelected;
				bool? flag = value;
				if ((allItemSelected.GetValueOrDefault() == flag.GetValueOrDefault()) & (allItemSelected != null == (flag != null)))
				{
					return;
				}
				this._allItemSelected = value;
				if (!this._isUpdatingFromChild && value != null)
				{
					foreach (GroupItem groupItem in this.Items)
					{
						groupItem.IsChecked = value.Value;
					}
				}
				this.OnPropertyChanged("AllItemSelected");
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06001317 RID: 4887 RVA: 0x0004E334 File Offset: 0x0004C534
		// (remove) Token: 0x06001318 RID: 4888 RVA: 0x0004E36C File Offset: 0x0004C56C
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001319 RID: 4889 RVA: 0x0004E3A1 File Offset: 0x0004C5A1
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x04000601 RID: 1537
		private bool _isUpdatingFromChild;

		// Token: 0x04000602 RID: 1538
		private string _name;

		// Token: 0x04000603 RID: 1539
		private bool _isSelected;

		// Token: 0x04000604 RID: 1540
		private bool? _allItemSelected;
	}
}
