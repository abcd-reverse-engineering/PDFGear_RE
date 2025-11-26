using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using CommonLib.Common.HotKeys;
using pdfeditor.Controls.Menus;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200015B RID: 347
	public class ContextMenuItemModel : ObservableCollection<IContextMenuModel>, IContextMenuModel, INotifyPropertyChanging, INotifyPropertyChanged
	{
		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x060014A3 RID: 5283 RVA: 0x00051825 File Offset: 0x0004FA25
		// (set) Token: 0x060014A4 RID: 5284 RVA: 0x0005182D File Offset: 0x0004FA2D
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

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060014A5 RID: 5285 RVA: 0x00051862 File Offset: 0x0004FA62
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

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x00051877 File Offset: 0x0004FA77
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x0005187F File Offset: 0x0004FA7F
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

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x000518A0 File Offset: 0x0004FAA0
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x000518A8 File Offset: 0x0004FAA8
		public ImageSource Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this.icon, value, "Icon");
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x000518BD File Offset: 0x0004FABD
		// (set) Token: 0x060014AB RID: 5291 RVA: 0x000518C5 File Offset: 0x0004FAC5
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

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x000518DA File Offset: 0x0004FADA
		// (set) Token: 0x060014AD RID: 5293 RVA: 0x000518E2 File Offset: 0x0004FAE2
		public string Caption
		{
			get
			{
				return this.caption;
			}
			set
			{
				this.SetProperty<string>(ref this.caption, value, "Caption");
			}
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x000518F7 File Offset: 0x0004FAF7
		// (set) Token: 0x060014AF RID: 5295 RVA: 0x000518FF File Offset: 0x0004FAFF
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

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x060014B0 RID: 5296 RVA: 0x00051914 File Offset: 0x0004FB14
		// (set) Token: 0x060014B1 RID: 5297 RVA: 0x0005191C File Offset: 0x0004FB1C
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

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x00051934 File Offset: 0x0004FB34
		// (set) Token: 0x060014B3 RID: 5299 RVA: 0x0005195F File Offset: 0x0004FB5F
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

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x00051979 File Offset: 0x0004FB79
		// (set) Token: 0x060014B5 RID: 5301 RVA: 0x00051981 File Offset: 0x0004FB81
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

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x00051996 File Offset: 0x0004FB96
		// (set) Token: 0x060014B7 RID: 5303 RVA: 0x0005199E File Offset: 0x0004FB9E
		public ICommand Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.SetProperty<ICommand>(ref this.command, value, "Command");
			}
		}

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x000519B3 File Offset: 0x0004FBB3
		// (set) Token: 0x060014B9 RID: 5305 RVA: 0x000519BB File Offset: 0x0004FBBB
		public string HotKeyInvokeWhen
		{
			get
			{
				return this.hotKeyInvokeWhen;
			}
			set
			{
				this.SetProperty<string>(ref this.hotKeyInvokeWhen, value, "HotKeyInvokeWhen");
			}
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x000519D0 File Offset: 0x0004FBD0
		// (set) Token: 0x060014BB RID: 5307 RVA: 0x000519D8 File Offset: 0x0004FBD8
		public string HotKeyString
		{
			get
			{
				return this.hotKeyString;
			}
			set
			{
				this.SetProperty<string>(ref this.hotKeyString, value, "HotKeyString");
			}
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x000519ED File Offset: 0x0004FBED
		// (set) Token: 0x060014BD RID: 5309 RVA: 0x000519F5 File Offset: 0x0004FBF5
		public HotKeyInvokeAction HotKeyInvokeAction
		{
			get
			{
				return this.hotKeyInvokeAction;
			}
			set
			{
				this.SetProperty<HotKeyInvokeAction>(ref this.hotKeyInvokeAction, value, "HotKeyInvokeAction");
			}
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00051A0A File Offset: 0x0004FC0A
		protected override void InsertItem(int index, IContextMenuModel item)
		{
			if (item is ContextMenuModel)
			{
				throw new ArgumentException("ContextMenuModel");
			}
			item.Parent = this;
			base.InsertItem(index, item);
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x00051A2E File Offset: 0x0004FC2E
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

		// Token: 0x060014C0 RID: 5312 RVA: 0x00051A5F File Offset: 0x0004FC5F
		protected override void RemoveItem(int index)
		{
			IContextMenuModel contextMenuModel = base[index];
			base.RemoveItem(index);
			contextMenuModel.Parent = null;
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00051A78 File Offset: 0x0004FC78
		protected override void ClearItems()
		{
			foreach (IContextMenuModel contextMenuModel in this)
			{
				contextMenuModel.Parent = null;
			}
			base.ClearItems();
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x00051AC4 File Offset: 0x0004FCC4
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			this.IsEndPoint = base.Count == 0;
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060014C3 RID: 5315 RVA: 0x00051ADC File Offset: 0x0004FCDC
		// (remove) Token: 0x060014C4 RID: 5316 RVA: 0x00051B14 File Offset: 0x0004FD14
		public event PropertyChangingEventHandler PropertyChanging;

		// Token: 0x060014C5 RID: 5317 RVA: 0x00051B49 File Offset: 0x0004FD49
		protected void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, e);
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x00051B5D File Offset: 0x0004FD5D
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00051B6B File Offset: 0x0004FD6B
		protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x00051B79 File Offset: 0x0004FD79
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

		// Token: 0x060014C9 RID: 5321 RVA: 0x00051BA6 File Offset: 0x0004FDA6
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

		// Token: 0x040006D0 RID: 1744
		private IContextMenuModel parent;

		// Token: 0x040006D1 RID: 1745
		private ImageSource icon;

		// Token: 0x040006D2 RID: 1746
		private string name;

		// Token: 0x040006D3 RID: 1747
		private string caption;

		// Token: 0x040006D4 RID: 1748
		private bool isEnabled = true;

		// Token: 0x040006D5 RID: 1749
		private bool isChecked;

		// Token: 0x040006D6 RID: 1750
		private bool? isCheckable;

		// Token: 0x040006D7 RID: 1751
		private TagDataModel tagData;

		// Token: 0x040006D8 RID: 1752
		private bool isEndPoint = true;

		// Token: 0x040006D9 RID: 1753
		private ICommand command;

		// Token: 0x040006DA RID: 1754
		private string hotKeyInvokeWhen = string.Empty;

		// Token: 0x040006DB RID: 1755
		private string hotKeyString = string.Empty;

		// Token: 0x040006DC RID: 1756
		private HotKeyInvokeAction hotKeyInvokeAction = HotKeyInvokeAction.Invoke;
	}
}
