using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PDFKit.Utils;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200016A RID: 362
	public class ContextMenuModelTypedSelectedAccessor : ObservableObject
	{
		// Token: 0x060015B4 RID: 5556 RVA: 0x00053FB2 File Offset: 0x000521B2
		public ContextMenuModelTypedSelectedAccessor(ContextMenuModel parent)
		{
			WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(parent, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnParentPropertyChanged));
			this.parent = parent;
			this.lastSelectedItems = this.GetSelectedItems();
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x00053FE4 File Offset: 0x000521E4
		public ContextMenuItemModel StrokeColor
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.StrokeColor);
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x060015B6 RID: 5558 RVA: 0x00053FED File Offset: 0x000521ED
		public ContextMenuItemModel FillColor
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.FillColor);
			}
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x060015B7 RID: 5559 RVA: 0x00053FF6 File Offset: 0x000521F6
		public ContextMenuItemModel StrokeThickness
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.StrokeThickness);
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060015B8 RID: 5560 RVA: 0x00053FFF File Offset: 0x000521FF
		public ContextMenuItemModel FontSize
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.FontSize);
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060015B9 RID: 5561 RVA: 0x00054008 File Offset: 0x00052208
		public ContextMenuItemModel FontName
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.FontName);
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060015BA RID: 5562 RVA: 0x00054011 File Offset: 0x00052211
		public ContextMenuItemModel FontColor
		{
			get
			{
				return this.GetCachedSelectedItem(ContextMenuItemType.FontColor);
			}
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0005401C File Offset: 0x0005221C
		private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			List<SelectedAccessorSelectionChangedEventArgs> list = null;
			try
			{
				Dictionary<ContextMenuItemType, ContextMenuItemModel> dictionary = this.lastSelectedItems;
				if (e.PropertyName == "SelectedItems" && dictionary != null)
				{
					Dictionary<ContextMenuItemType, ContextMenuItemModel> selectedItems = this.GetSelectedItems();
					IReadOnlyList<ContextMenuItemType> allValues = EnumHelper<ContextMenuItemType>.AllValues;
					for (int i = 0; i < allValues.Count; i++)
					{
						if (allValues[i] != ContextMenuItemType.None)
						{
							ContextMenuItemType contextMenuItemType = allValues[i];
							ContextMenuItemModel contextMenuItemModel = null;
							ContextMenuItemModel contextMenuItemModel2 = null;
							dictionary.TryGetValue(contextMenuItemType, out contextMenuItemModel);
							selectedItems.TryGetValue(contextMenuItemType, out contextMenuItemModel2);
							if (contextMenuItemModel != contextMenuItemModel2 && this.SelectionChanged != null)
							{
								if (list == null)
								{
									list = new List<SelectedAccessorSelectionChangedEventArgs>();
								}
								SelectedAccessorSelectionChangedEventArgs selectedAccessorSelectionChangedEventArgs = new SelectedAccessorSelectionChangedEventArgs(contextMenuItemModel, contextMenuItemModel2, contextMenuItemType);
								list.Add(selectedAccessorSelectionChangedEventArgs);
							}
						}
					}
				}
			}
			finally
			{
				this.lastSelectedItems = this.GetSelectedItems();
			}
			if (list != null)
			{
				foreach (SelectedAccessorSelectionChangedEventArgs selectedAccessorSelectionChangedEventArgs2 in list)
				{
					EventHandler<SelectedAccessorSelectionChangedEventArgs> selectionChanged = this.SelectionChanged;
					if (selectionChanged != null)
					{
						selectionChanged(this, selectedAccessorSelectionChangedEventArgs2);
					}
				}
			}
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x00054138 File Offset: 0x00052338
		private ContextMenuItemModel GetCachedSelectedItem(ContextMenuItemType type)
		{
			if (this.lastSelectedItems == null)
			{
				return null;
			}
			ContextMenuItemModel contextMenuItemModel;
			if (this.lastSelectedItems.TryGetValue(type, out contextMenuItemModel))
			{
				return contextMenuItemModel;
			}
			return null;
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x00054164 File Offset: 0x00052364
		private ContextMenuItemModel GetSelectedItem(ContextMenuItemType type)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = this.parent.OfType<TypedContextMenuItemModel>().FirstOrDefault((TypedContextMenuItemModel c) => c.Type == type);
			if (typedContextMenuItemModel == null)
			{
				return null;
			}
			return typedContextMenuItemModel.SelectedItem;
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x000541A8 File Offset: 0x000523A8
		private Dictionary<ContextMenuItemType, ContextMenuItemModel> GetSelectedItems()
		{
			return (from c in EnumHelper<ContextMenuItemType>.AllValues
				select new global::System.ValueTuple<ContextMenuItemType, ContextMenuItemModel>(c, this.GetSelectedItem(c)) into c
				where c.Item2 != null
				select c).ToDictionary(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "c", null })] global::System.ValueTuple<ContextMenuItemType, ContextMenuItemModel> c) => c.Item1, ([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "c", null })] global::System.ValueTuple<ContextMenuItemType, ContextMenuItemModel> c) => c.Item2);
		}

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060015BF RID: 5567 RVA: 0x00054234 File Offset: 0x00052434
		// (remove) Token: 0x060015C0 RID: 5568 RVA: 0x0005426C File Offset: 0x0005246C
		public event EventHandler<SelectedAccessorSelectionChangedEventArgs> SelectionChanged;

		// Token: 0x0400073A RID: 1850
		private readonly ContextMenuModel parent;

		// Token: 0x0400073B RID: 1851
		private Dictionary<ContextMenuItemType, ContextMenuItemModel> lastSelectedItems;
	}
}
