using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Controls.Menus;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200015D RID: 349
	public class ContextMenuRecentFileList : ObservableCollection<IContextMenuModel>, IContextMenuModel, INotifyPropertyChanging, INotifyPropertyChanged
	{
		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x00052074 File Offset: 0x00050274
		public AsyncRelayCommand<RecentFileItem> DeleteSelectedRecentFileCmd
		{
			get
			{
				AsyncRelayCommand<RecentFileItem> asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteSelectedRecentFileCmd) == null)
				{
					asyncRelayCommand = (this.deleteSelectedRecentFileCmd = new AsyncRelayCommand<RecentFileItem>(async delegate([Nullable(2)] RecentFileItem item)
					{
						if (this.RecentFiles != null && item != null && this.RecentFiles.Contains(item))
						{
							this.RecentFiles.Remove(item);
						}
						bool flag;
						HistoryManager.RemoveHistoryItem(item.FilePath, out flag);
					}, (RecentFileItem item) => !this.DeleteSelectedRecentFileCmd.IsRunning && item != null));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x06001500 RID: 5376 RVA: 0x000520B1 File Offset: 0x000502B1
		// (set) Token: 0x06001501 RID: 5377 RVA: 0x000520B9 File Offset: 0x000502B9
		public virtual IContextMenuModel Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (this.parent != value)
				{
					this.OnPropertyChanging("Level");
				}
				if (this.SetProperty<IContextMenuModel>(ref this.parent, value, "Parent"))
				{
					this.OnPropertyChanged("Level");
				}
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x06001502 RID: 5378 RVA: 0x000520EE File Offset: 0x000502EE
		public virtual int Level
		{
			get
			{
				IContextMenuModel contextMenuModel = this.Parent;
				if (contextMenuModel == null)
				{
					return -1;
				}
				return contextMenuModel.Level + 1;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x00052103 File Offset: 0x00050303
		// (set) Token: 0x06001504 RID: 5380 RVA: 0x0005210B File Offset: 0x0005030B
		public bool IsEndPoint
		{
			get
			{
				return this.isEndPoint;
			}
			private set
			{
				if (this.SetProperty<bool>(ref this.isEndPoint, value, "IsEndPoint"))
				{
					this.OnPropertyChanged("IsCheckable");
				}
			}
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x0005212C File Offset: 0x0005032C
		// (set) Token: 0x06001506 RID: 5382 RVA: 0x00052134 File Offset: 0x00050334
		public ObservableCollection<RecentFileItem> RecentFiles
		{
			get
			{
				return this.recentFiles;
			}
			set
			{
				this.SetProperty<ObservableCollection<RecentFileItem>>(ref this.recentFiles, value, "RecentFiles");
			}
		}

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x00052149 File Offset: 0x00050349
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x00052151 File Offset: 0x00050351
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.SetProperty<string>(ref this.name, value, "Name");
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x00052166 File Offset: 0x00050366
		// (set) Token: 0x0600150A RID: 5386 RVA: 0x0005216E File Offset: 0x0005036E
		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.SetProperty<bool>(ref this.isEnabled, value, "IsEnabled");
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x0600150B RID: 5387 RVA: 0x00052183 File Offset: 0x00050383
		// (set) Token: 0x0600150C RID: 5388 RVA: 0x0005218B File Offset: 0x0005038B
		public virtual bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				this.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x0600150D RID: 5389 RVA: 0x000521A0 File Offset: 0x000503A0
		// (set) Token: 0x0600150E RID: 5390 RVA: 0x000521CB File Offset: 0x000503CB
		public virtual bool IsCheckable
		{
			get
			{
				bool? flag = this.isCheckable;
				if (flag == null)
				{
					return this.IsEndPoint;
				}
				return flag.GetValueOrDefault();
			}
			set
			{
				this.SetProperty<bool?>(ref this.isCheckable, new bool?(value), "IsCheckable");
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x0600150F RID: 5391 RVA: 0x000521E5 File Offset: 0x000503E5
		// (set) Token: 0x06001510 RID: 5392 RVA: 0x000521ED File Offset: 0x000503ED
		public TagDataModel TagData
		{
			get
			{
				return this.tagData;
			}
			set
			{
				this.SetProperty<TagDataModel>(ref this.tagData, value, "TagData");
			}
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00052202 File Offset: 0x00050402
		protected override void InsertItem(int index, IContextMenuModel item)
		{
			if (item is ContextMenuModel)
			{
				throw new ArgumentException("ContextMenuModel");
			}
			item.Parent = this;
			base.InsertItem(index, item);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00052226 File Offset: 0x00050426
		protected override void SetItem(int index, IContextMenuModel item)
		{
			if (item is ContextMenuModel)
			{
				throw new ArgumentException("ContextMenuModel");
			}
			IContextMenuModel contextMenuModel = base[index];
			base.SetItem(index, item);
			contextMenuModel.Parent = null;
			item.Parent = this;
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00052257 File Offset: 0x00050457
		protected override void RemoveItem(int index)
		{
			IContextMenuModel contextMenuModel = base[index];
			base.RemoveItem(index);
			contextMenuModel.Parent = null;
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00052270 File Offset: 0x00050470
		protected override void ClearItems()
		{
			foreach (IContextMenuModel contextMenuModel in this)
			{
				contextMenuModel.Parent = null;
			}
			base.ClearItems();
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x000522BC File Offset: 0x000504BC
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			this.IsEndPoint = base.Count == 0;
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001516 RID: 5398 RVA: 0x000522D4 File Offset: 0x000504D4
		// (remove) Token: 0x06001517 RID: 5399 RVA: 0x0005230C File Offset: 0x0005050C
		public event PropertyChangingEventHandler PropertyChanging;

		// Token: 0x06001518 RID: 5400 RVA: 0x00052341 File Offset: 0x00050541
		protected void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, e);
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00052355 File Offset: 0x00050555
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00052363 File Offset: 0x00050563
		protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00052371 File Offset: 0x00050571
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

		// Token: 0x0600151C RID: 5404 RVA: 0x0005239E File Offset: 0x0005059E
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

		// Token: 0x040006F2 RID: 1778
		private IContextMenuModel parent;

		// Token: 0x040006F3 RID: 1779
		private string name;

		// Token: 0x040006F4 RID: 1780
		private ObservableCollection<RecentFileItem> recentFiles;

		// Token: 0x040006F5 RID: 1781
		private bool isEnabled = true;

		// Token: 0x040006F6 RID: 1782
		private bool isChecked;

		// Token: 0x040006F7 RID: 1783
		private bool? isCheckable;

		// Token: 0x040006F8 RID: 1784
		private TagDataModel tagData;

		// Token: 0x040006F9 RID: 1785
		private bool isEndPoint = true;

		// Token: 0x040006FA RID: 1786
		private AsyncRelayCommand<RecentFileItem> deleteSelectedRecentFileCmd;
	}
}
