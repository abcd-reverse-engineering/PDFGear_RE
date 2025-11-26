using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000187 RID: 391
	public class ToolbarSettingModel : ObservableCollection<ToolbarSettingItemModel>
	{
		// Token: 0x06001659 RID: 5721 RVA: 0x00055761 File Offset: 0x00053961
		public ToolbarSettingModel(AnnotationMode mode)
		{
			this.id = ToolbarSettingId.CreateAnnotation(mode);
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00055775 File Offset: 0x00053975
		public ToolbarSettingModel(ToolbarSettingId id)
		{
			this.id = id;
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x0600165B RID: 5723 RVA: 0x00055784 File Offset: 0x00053984
		public ToolbarSettingId Id
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x0005578C File Offset: 0x0005398C
		protected override void InsertItem(int index, ToolbarSettingItemModel item)
		{
			item.Parent = this;
			base.InsertItem(index, item);
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0005579D File Offset: 0x0005399D
		protected override void SetItem(int index, ToolbarSettingItemModel item)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = base[index];
			base.SetItem(index, item);
			toolbarSettingItemModel.Parent = null;
			item.Parent = this;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x000557BB File Offset: 0x000539BB
		protected override void RemoveItem(int index)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = base[index];
			base.RemoveItem(index);
			toolbarSettingItemModel.Parent = null;
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x000557D4 File Offset: 0x000539D4
		protected override void ClearItems()
		{
			foreach (ToolbarSettingItemModel toolbarSettingItemModel in this)
			{
				toolbarSettingItemModel.Parent = null;
			}
			base.ClearItems();
		}

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06001660 RID: 5728 RVA: 0x00055820 File Offset: 0x00053A20
		// (remove) Token: 0x06001661 RID: 5729 RVA: 0x00055858 File Offset: 0x00053A58
		public event PropertyChangingEventHandler PropertyChanging;

		// Token: 0x06001662 RID: 5730 RVA: 0x0005588D File Offset: 0x00053A8D
		protected void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, e);
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x000558A1 File Offset: 0x00053AA1
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x000558AF File Offset: 0x00053AAF
		protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x000558BD File Offset: 0x00053ABD
		protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
			{
				return false;
			}
			this.OnPropertyChanging(propertyName);
			field = newValue;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x000558EA File Offset: 0x00053AEA
		protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = null)
		{
			if (comparer.Equals(field, newValue))
			{
				return false;
			}
			this.OnPropertyChanging(propertyName);
			field = newValue;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x00055918 File Offset: 0x00053B18
		public async Task SaveConfigAsync()
		{
			await Task.WhenAll(this.Select(delegate(ToolbarSettingItemModel c)
			{
				ToolbarSettingModel.<>c__DisplayClass17_0 CS$<>8__locals1 = new ToolbarSettingModel.<>c__DisplayClass17_0();
				CS$<>8__locals1.c = c;
				return delegate
				{
					ToolbarSettingModel.<>c__DisplayClass17_0.<<SaveConfigAsync>b__1>d <<SaveConfigAsync>b__1>d;
					<<SaveConfigAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<SaveConfigAsync>b__1>d.<>4__this = CS$<>8__locals1;
					<<SaveConfigAsync>b__1>d.<>1__state = -1;
					<<SaveConfigAsync>b__1>d.<>t__builder.Start<ToolbarSettingModel.<>c__DisplayClass17_0.<<SaveConfigAsync>b__1>d>(ref <<SaveConfigAsync>b__1>d);
					return <<SaveConfigAsync>b__1>d.<>t__builder.Task;
				}();
			}));
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0005595C File Offset: 0x00053B5C
		public async Task LoadConfigAsync()
		{
			await Task.WhenAll(this.Select(delegate(ToolbarSettingItemModel c)
			{
				ToolbarSettingModel.<>c__DisplayClass18_0 CS$<>8__locals1 = new ToolbarSettingModel.<>c__DisplayClass18_0();
				CS$<>8__locals1.c = c;
				return delegate
				{
					ToolbarSettingModel.<>c__DisplayClass18_0.<<LoadConfigAsync>b__1>d <<LoadConfigAsync>b__1>d;
					<<LoadConfigAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadConfigAsync>b__1>d.<>4__this = CS$<>8__locals1;
					<<LoadConfigAsync>b__1>d.<>1__state = -1;
					<<LoadConfigAsync>b__1>d.<>t__builder.Start<ToolbarSettingModel.<>c__DisplayClass18_0.<<LoadConfigAsync>b__1>d>(ref <<LoadConfigAsync>b__1>d);
					return <<LoadConfigAsync>b__1>d.<>t__builder.Task;
				}();
			}));
		}

		// Token: 0x04000777 RID: 1911
		private ToolbarSettingId id;
	}
}
