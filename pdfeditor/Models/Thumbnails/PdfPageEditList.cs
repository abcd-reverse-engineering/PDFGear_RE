using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace pdfeditor.Models.Thumbnails
{
	// Token: 0x02000135 RID: 309
	public class PdfPageEditList : ObservableCollection<PdfPageEditListModel>
	{
		// Token: 0x060012BC RID: 4796 RVA: 0x0004C278 File Offset: 0x0004A478
		public PdfPageEditList()
		{
			this.selectedItems = new List<PdfPageEditListModel>();
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0004C2DC File Offset: 0x0004A4DC
		public PdfPageEditList(IEnumerable<PdfPageEditListModel> source)
			: base(source)
		{
			this.selectedItems = new List<PdfPageEditListModel>();
			foreach (PdfPageEditListModel pdfPageEditListModel in base.Items.OfType<PdfPageEditListModel>())
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(pdfPageEditListModel, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
				if (pdfPageEditListModel.Selected)
				{
					this.selectedItems.Add(pdfPageEditListModel);
				}
			}
			this.UpdateMinAspectRatio();
			this.UpdateAllItemSelected();
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x0004C3B4 File Offset: 0x0004A5B4
		public IReadOnlyCollection<PdfPageEditListModel> SelectedItems
		{
			get
			{
				return this.selectedItems;
			}
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x060012BF RID: 4799 RVA: 0x0004C3BC File Offset: 0x0004A5BC
		// (set) Token: 0x060012C0 RID: 4800 RVA: 0x0004C3C4 File Offset: 0x0004A5C4
		public double Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				if (this.scale != value)
				{
					this.scale = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Scale"));
					this.UpdateChildrenThumbnailSize();
					this.PlaceholderWidth = PdfPageEditListModel.DefaultThumbnailWidth * value;
					if (this.minAspectRatio == 0.0)
					{
						this.PlaceholderHeight = this.PlaceholderWidth * 1.414;
						return;
					}
					this.PlaceholderHeight = this.PlaceholderWidth / this.minAspectRatio;
				}
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x060012C1 RID: 4801 RVA: 0x0004C43F File Offset: 0x0004A63F
		// (set) Token: 0x060012C2 RID: 4802 RVA: 0x0004C447 File Offset: 0x0004A647
		public double PlaceholderWidth
		{
			get
			{
				return this.placeholderWidth;
			}
			set
			{
				if (this.placeholderWidth != value)
				{
					this.placeholderWidth = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("PlaceholderWidth"));
				}
			}
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x060012C3 RID: 4803 RVA: 0x0004C469 File Offset: 0x0004A669
		// (set) Token: 0x060012C4 RID: 4804 RVA: 0x0004C471 File Offset: 0x0004A671
		public double PlaceholderHeight
		{
			get
			{
				return this.placeholderHeight;
			}
			set
			{
				if (this.placeholderHeight != value)
				{
					this.placeholderHeight = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("PlaceholderHeight"));
				}
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x060012C5 RID: 4805 RVA: 0x0004C493 File Offset: 0x0004A693
		// (set) Token: 0x060012C6 RID: 4806 RVA: 0x0004C49C File Offset: 0x0004A69C
		public double MinAspectRatio
		{
			get
			{
				return this.minAspectRatio;
			}
			set
			{
				if (this.minAspectRatio != value)
				{
					this.minAspectRatio = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("MinAspectRatio"));
					if (this.minAspectRatio == 0.0)
					{
						this.PlaceholderHeight = this.PlaceholderWidth * 1.414;
						return;
					}
					this.PlaceholderHeight = this.PlaceholderWidth / this.minAspectRatio;
				}
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x0004C504 File Offset: 0x0004A704
		// (set) Token: 0x060012C8 RID: 4808 RVA: 0x0004C50C File Offset: 0x0004A70C
		public bool? AllItemSelected
		{
			get
			{
				return this.allItemSelected;
			}
			set
			{
				bool? flag = this.allItemSelected;
				bool? flag2 = value;
				if (!((flag.GetValueOrDefault() == flag2.GetValueOrDefault()) & (flag != null == (flag2 != null))))
				{
					if (value == null)
					{
						throw new ArgumentException("AllItemSelected");
					}
					lock (this)
					{
						this.allItemSelectedPropChanging = true;
						try
						{
							if (value.GetValueOrDefault())
							{
								using (IEnumerator<PdfPageEditListModel> enumerator = base.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										PdfPageEditListModel pdfPageEditListModel = enumerator.Current;
										pdfPageEditListModel.Selected = true;
									}
									goto IL_00CD;
								}
							}
							foreach (PdfPageEditListModel pdfPageEditListModel2 in this)
							{
								pdfPageEditListModel2.Selected = false;
							}
						}
						finally
						{
							this.allItemSelectedPropChanging = false;
						}
					}
				}
				IL_00CD:
				this.SetAllItemSelectedCore(value);
				this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
			}
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x0004C634 File Offset: 0x0004A834
		private void SetAllItemSelectedCore(bool? value)
		{
			bool? flag = this.allItemSelected;
			bool? flag2 = value;
			if (!((flag.GetValueOrDefault() == flag2.GetValueOrDefault()) & (flag != null == (flag2 != null))))
			{
				this.allItemSelected = value;
				this.OnPropertyChanged(new PropertyChangedEventArgs("AllItemSelected"));
			}
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x0004C684 File Offset: 0x0004A884
		private void UpdateMinAspectRatio()
		{
			bool flag;
			this.UpdateMaxPageSize(out flag);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0004C69C File Offset: 0x0004A89C
		private void UpdateMaxPageSize(out bool childrenThumbnailUpdated)
		{
			childrenThumbnailUpdated = false;
			if (this.selectedItems == null)
			{
				return;
			}
			PdfPageEditListModel[] array = null;
			lock (this)
			{
				array = this.ToArray<PdfPageEditListModel>();
			}
			double num = this.MinAspectRatio;
			this.MinAspectRatio = (from c in array
				where c.PageHeight != 0.0
				select c.PageWidth / c.PageHeight).DefaultIfEmpty<double>().Min();
			if (num != this.MinAspectRatio)
			{
				this.UpdateChildrenThumbnailSize();
				childrenThumbnailUpdated = true;
			}
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x0004C758 File Offset: 0x0004A958
		private void UpdateChildrenThumbnailSize()
		{
			lock (this)
			{
				foreach (PdfPageEditListModel pdfPageEditListModel in this)
				{
					pdfPageEditListModel.UpdateThumbnailSize(this.Scale, this.MinAspectRatio);
				}
			}
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x0004C7CC File Offset: 0x0004A9CC
		private void UpdateAllItemSelected()
		{
			List<PdfPageEditListModel> list = this.selectedItems;
			lock (list)
			{
				bool? flag2 = new bool?(false);
				if (base.Count == 0 || this.selectedItems.Count == 0)
				{
					flag2 = new bool?(false);
				}
				else if (base.Count == this.selectedItems.Count)
				{
					flag2 = new bool?(true);
				}
				else
				{
					flag2 = null;
				}
				this.SetAllItemSelectedCore(flag2);
			}
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0004C85C File Offset: 0x0004AA5C
		protected override void InsertItem(int index, PdfPageEditListModel item)
		{
			lock (this)
			{
				base.InsertItem(index, item);
			}
			bool flag2;
			this.UpdateMaxPageSize(out flag2);
			if (!flag2)
			{
				item.UpdateThumbnailSize(this.Scale, this.MinAspectRatio);
			}
			if (item != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
			}
			if (item.Selected)
			{
				List<PdfPageEditListModel> list = this.selectedItems;
				lock (list)
				{
					this.selectedItems.Add(item);
					this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
				}
			}
			this.UpdateAllItemSelected();
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x0004C928 File Offset: 0x0004AB28
		protected override void SetItem(int index, PdfPageEditListModel item)
		{
			PdfPageEditListModel pdfPageEditListModel = base.Items[index];
			if (pdfPageEditListModel != null)
			{
				if (pdfPageEditListModel.Selected)
				{
					List<PdfPageEditListModel> list = this.selectedItems;
					lock (list)
					{
						this.selectedItems.Remove(pdfPageEditListModel);
						this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
					}
				}
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(pdfPageEditListModel, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
			}
			lock (this)
			{
				base.SetItem(index, item);
			}
			bool flag2;
			this.UpdateMaxPageSize(out flag2);
			if (!flag2)
			{
				item.UpdateThumbnailSize(this.Scale, this.MinAspectRatio);
			}
			if (item != null)
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
			}
			if (item.Selected)
			{
				List<PdfPageEditListModel> list = this.selectedItems;
				lock (list)
				{
					this.selectedItems.Add(item);
					this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
				}
			}
			this.UpdateAllItemSelected();
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0004CA70 File Offset: 0x0004AC70
		protected override void RemoveItem(int index)
		{
			PdfPageEditListModel pdfPageEditListModel = base.Items[index];
			if (pdfPageEditListModel != null)
			{
				if (pdfPageEditListModel.Selected)
				{
					List<PdfPageEditListModel> list = this.selectedItems;
					lock (list)
					{
						this.selectedItems.Remove(pdfPageEditListModel);
						this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
					}
				}
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(pdfPageEditListModel, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
			}
			lock (this)
			{
				base.RemoveItem(index);
			}
			this.UpdateMinAspectRatio();
			this.UpdateAllItemSelected();
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x0004CB30 File Offset: 0x0004AD30
		protected override void ClearItems()
		{
			foreach (INotifyPropertyChanged notifyPropertyChanged in base.Items.OfType<INotifyPropertyChanged>())
			{
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnItemPropertyChanged));
			}
			List<PdfPageEditListModel> list = this.selectedItems;
			lock (list)
			{
				this.selectedItems.Clear();
				this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
			}
			lock (this)
			{
				base.ClearItems();
			}
			this.UpdateMinAspectRatio();
			this.UpdateAllItemSelected();
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x0004CC0C File Offset: 0x0004AE0C
		private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PdfPageEditListModel pdfPageEditListModel = sender as PdfPageEditListModel;
			if (pdfPageEditListModel != null)
			{
				if (e.PropertyName == "Selected")
				{
					List<PdfPageEditListModel> list = this.selectedItems;
					lock (list)
					{
						if (pdfPageEditListModel.Selected)
						{
							this.selectedItems.Add(pdfPageEditListModel);
						}
						else
						{
							this.selectedItems.Remove(pdfPageEditListModel);
						}
						if (!this.allItemSelectedPropChanging)
						{
							this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
						}
					}
					if (!this.allItemSelectedPropChanging)
					{
						this.UpdateAllItemSelected();
					}
				}
				if (e.PropertyName == "PageWidth" || e.PropertyName == "PageHeight")
				{
					this.UpdateMinAspectRatio();
				}
			}
		}

		// Token: 0x040005EF RID: 1519
		private List<PdfPageEditListModel> selectedItems;

		// Token: 0x040005F0 RID: 1520
		private double scale = 0.5;

		// Token: 0x040005F1 RID: 1521
		private double minAspectRatio;

		// Token: 0x040005F2 RID: 1522
		private double placeholderWidth = PdfPageEditListModel.DefaultThumbnailWidth * 0.5;

		// Token: 0x040005F3 RID: 1523
		private double placeholderHeight = PdfPageEditListModel.DefaultThumbnailWidth * 0.5 * 1.414;

		// Token: 0x040005F4 RID: 1524
		private bool? allItemSelected;

		// Token: 0x040005F5 RID: 1525
		private bool allItemSelectedPropChanging;
	}
}
