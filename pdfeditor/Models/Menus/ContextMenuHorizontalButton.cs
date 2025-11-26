using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using pdfeditor.Controls.Menus;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200015C RID: 348
	public class ContextMenuHorizontalButton : ObservableCollection<IContextMenuModel>, IContextMenuModel, INotifyPropertyChanging, INotifyPropertyChanged
	{
		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x00051C04 File Offset: 0x0004FE04
		// (set) Token: 0x060014CC RID: 5324 RVA: 0x00051C0C File Offset: 0x0004FE0C
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

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x060014CD RID: 5325 RVA: 0x00051C41 File Offset: 0x0004FE41
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

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x060014CE RID: 5326 RVA: 0x00051C56 File Offset: 0x0004FE56
		// (set) Token: 0x060014CF RID: 5327 RVA: 0x00051C5E File Offset: 0x0004FE5E
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

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00051C7F File Offset: 0x0004FE7F
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x00051C87 File Offset: 0x0004FE87
		public ImageSource Icon0
		{
			get
			{
				return this.icon0;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this.icon0, value, "Icon0");
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x00051C9C File Offset: 0x0004FE9C
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x00051CA4 File Offset: 0x0004FEA4
		public ImageSource Icon1
		{
			get
			{
				return this.icon1;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this.icon1, value, "Icon1");
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x00051CB9 File Offset: 0x0004FEB9
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x00051CC1 File Offset: 0x0004FEC1
		public ImageSource Icon2
		{
			get
			{
				return this.icon2;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this.icon2, value, "Icon2");
			}
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x060014D6 RID: 5334 RVA: 0x00051CD6 File Offset: 0x0004FED6
		// (set) Token: 0x060014D7 RID: 5335 RVA: 0x00051CDE File Offset: 0x0004FEDE
		public ImageSource Icon3
		{
			get
			{
				return this.icon3;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this.icon3, value, "Icon3");
			}
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x060014D8 RID: 5336 RVA: 0x00051CF3 File Offset: 0x0004FEF3
		// (set) Token: 0x060014D9 RID: 5337 RVA: 0x00051CFB File Offset: 0x0004FEFB
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

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x060014DA RID: 5338 RVA: 0x00051D10 File Offset: 0x0004FF10
		// (set) Token: 0x060014DB RID: 5339 RVA: 0x00051D18 File Offset: 0x0004FF18
		public string Caption0
		{
			get
			{
				return this.caption0;
			}
			set
			{
				this.SetProperty<string>(ref this.caption0, value, "Caption0");
			}
		}

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x060014DC RID: 5340 RVA: 0x00051D2D File Offset: 0x0004FF2D
		// (set) Token: 0x060014DD RID: 5341 RVA: 0x00051D35 File Offset: 0x0004FF35
		public string Caption1
		{
			get
			{
				return this.caption1;
			}
			set
			{
				this.SetProperty<string>(ref this.caption1, value, "Caption1");
			}
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x00051D4A File Offset: 0x0004FF4A
		// (set) Token: 0x060014DF RID: 5343 RVA: 0x00051D52 File Offset: 0x0004FF52
		public string Caption2
		{
			get
			{
				return this.caption2;
			}
			set
			{
				this.SetProperty<string>(ref this.caption2, value, "Caption2");
			}
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x00051D67 File Offset: 0x0004FF67
		// (set) Token: 0x060014E1 RID: 5345 RVA: 0x00051D6F File Offset: 0x0004FF6F
		public string Caption3
		{
			get
			{
				return this.caption3;
			}
			set
			{
				this.SetProperty<string>(ref this.caption3, value, "Caption3");
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x00051D84 File Offset: 0x0004FF84
		// (set) Token: 0x060014E3 RID: 5347 RVA: 0x00051D8C File Offset: 0x0004FF8C
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

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x060014E4 RID: 5348 RVA: 0x00051DA1 File Offset: 0x0004FFA1
		// (set) Token: 0x060014E5 RID: 5349 RVA: 0x00051DA9 File Offset: 0x0004FFA9
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

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x060014E6 RID: 5350 RVA: 0x00051DC0 File Offset: 0x0004FFC0
		// (set) Token: 0x060014E7 RID: 5351 RVA: 0x00051DEB File Offset: 0x0004FFEB
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

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x00051E05 File Offset: 0x00050005
		// (set) Token: 0x060014E9 RID: 5353 RVA: 0x00051E0D File Offset: 0x0005000D
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

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x00051E22 File Offset: 0x00050022
		// (set) Token: 0x060014EB RID: 5355 RVA: 0x00051E2A File Offset: 0x0005002A
		public ICommand Command0
		{
			get
			{
				return this.command0;
			}
			set
			{
				this.SetProperty<ICommand>(ref this.command0, value, "Command0");
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x060014EC RID: 5356 RVA: 0x00051E3F File Offset: 0x0005003F
		// (set) Token: 0x060014ED RID: 5357 RVA: 0x00051E47 File Offset: 0x00050047
		public ICommand Command1
		{
			get
			{
				return this.command1;
			}
			set
			{
				this.SetProperty<ICommand>(ref this.command1, value, "Command1");
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x060014EE RID: 5358 RVA: 0x00051E5C File Offset: 0x0005005C
		// (set) Token: 0x060014EF RID: 5359 RVA: 0x00051E64 File Offset: 0x00050064
		public ICommand Command2
		{
			get
			{
				return this.command2;
			}
			set
			{
				this.SetProperty<ICommand>(ref this.command2, value, "Command2");
			}
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x060014F0 RID: 5360 RVA: 0x00051E79 File Offset: 0x00050079
		// (set) Token: 0x060014F1 RID: 5361 RVA: 0x00051E81 File Offset: 0x00050081
		public ICommand Command3
		{
			get
			{
				return this.command3;
			}
			set
			{
				this.SetProperty<ICommand>(ref this.command3, value, "Command3");
			}
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00051E96 File Offset: 0x00050096
		protected override void InsertItem(int index, IContextMenuModel item)
		{
			if (item is ContextMenuModel)
			{
				throw new ArgumentException("ContextMenuModel");
			}
			item.Parent = this;
			base.InsertItem(index, item);
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00051EBA File Offset: 0x000500BA
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

		// Token: 0x060014F4 RID: 5364 RVA: 0x00051EEB File Offset: 0x000500EB
		protected override void RemoveItem(int index)
		{
			IContextMenuModel contextMenuModel = base[index];
			base.RemoveItem(index);
			contextMenuModel.Parent = null;
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x00051F04 File Offset: 0x00050104
		protected override void ClearItems()
		{
			foreach (IContextMenuModel contextMenuModel in this)
			{
				contextMenuModel.Parent = null;
			}
			base.ClearItems();
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x00051F50 File Offset: 0x00050150
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			this.IsEndPoint = base.Count == 0;
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060014F7 RID: 5367 RVA: 0x00051F68 File Offset: 0x00050168
		// (remove) Token: 0x060014F8 RID: 5368 RVA: 0x00051FA0 File Offset: 0x000501A0
		public event PropertyChangingEventHandler PropertyChanging;

		// Token: 0x060014F9 RID: 5369 RVA: 0x00051FD5 File Offset: 0x000501D5
		protected void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, e);
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x00051FE9 File Offset: 0x000501E9
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x00051FF7 File Offset: 0x000501F7
		protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
		{
			this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x00052005 File Offset: 0x00050205
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

		// Token: 0x060014FD RID: 5373 RVA: 0x00052032 File Offset: 0x00050232
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

		// Token: 0x040006DE RID: 1758
		private IContextMenuModel parent;

		// Token: 0x040006DF RID: 1759
		private ImageSource icon0;

		// Token: 0x040006E0 RID: 1760
		private ImageSource icon1;

		// Token: 0x040006E1 RID: 1761
		private ImageSource icon2;

		// Token: 0x040006E2 RID: 1762
		private ImageSource icon3;

		// Token: 0x040006E3 RID: 1763
		private string name;

		// Token: 0x040006E4 RID: 1764
		private string caption0;

		// Token: 0x040006E5 RID: 1765
		private string caption1;

		// Token: 0x040006E6 RID: 1766
		private string caption2;

		// Token: 0x040006E7 RID: 1767
		private string caption3;

		// Token: 0x040006E8 RID: 1768
		private bool isEnabled = true;

		// Token: 0x040006E9 RID: 1769
		private bool isChecked;

		// Token: 0x040006EA RID: 1770
		private bool? isCheckable;

		// Token: 0x040006EB RID: 1771
		private TagDataModel tagData;

		// Token: 0x040006EC RID: 1772
		private bool isEndPoint = true;

		// Token: 0x040006ED RID: 1773
		private ICommand command0;

		// Token: 0x040006EE RID: 1774
		private ICommand command1;

		// Token: 0x040006EF RID: 1775
		private ICommand command2;

		// Token: 0x040006F0 RID: 1776
		private ICommand command3;
	}
}
