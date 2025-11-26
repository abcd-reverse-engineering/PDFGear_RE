using System;
using System.Linq;
using System.Windows;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000168 RID: 360
	public class TypedContextMenuModel : ContextMenuModel
	{
		// Token: 0x060015A2 RID: 5538 RVA: 0x00053C8C File Offset: 0x00051E8C
		public TypedContextMenuModel(AnnotationMode mode)
		{
			this.SelectedItems = new ContextMenuModelTypedSelectedAccessor(this);
			this.SelectedItems.SelectionChanged += delegate(object s, SelectedAccessorSelectionChangedEventArgs a)
			{
				EventHandler<SelectedAccessorSelectionChangedEventArgs> selectionChanged = this.SelectionChanged;
				if (selectionChanged == null)
				{
					return;
				}
				selectionChanged(this, a);
			};
			this.Mode = mode;
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x060015A3 RID: 5539 RVA: 0x00053CBE File Offset: 0x00051EBE
		public AnnotationMode Mode { get; }

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x060015A4 RID: 5540 RVA: 0x00053CC6 File Offset: 0x00051EC6
		public ContextMenuModelTypedSelectedAccessor SelectedItems { get; }

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x00053CCE File Offset: 0x00051ECE
		// (set) Token: 0x060015A6 RID: 5542 RVA: 0x00053CD6 File Offset: 0x00051ED6
		public ToolbarChildCheckableButtonModel Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				base.SetProperty<ToolbarChildCheckableButtonModel>(ref this.owner, value, "Owner");
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x00053CEC File Offset: 0x00051EEC
		protected override void InsertItem(int index, IContextMenuModel item)
		{
			if (!(item is ContextMenuSeparator))
			{
				TypedContextMenuItemModel model = item as TypedContextMenuItemModel;
				if (model == null || !this.OfType<TypedContextMenuItemModel>().All((TypedContextMenuItemModel c) => c.Type != model.Type))
				{
					base.InsertItem(index, item);
					return;
				}
			}
			base.InsertItem(index, item);
			SelectableContextMenuItemModel selectableContextMenuItemModel = item as SelectableContextMenuItemModel;
			if (selectableContextMenuItemModel != null)
			{
				WeakEventManager<SelectableContextMenuItemModel, SelectableModelSelectionChangedEventArgs>.AddHandler(selectableContextMenuItemModel, "SelectionChanged", new EventHandler<SelectableModelSelectionChangedEventArgs>(this.Item_SelectionChanged));
			}
			this.UpdateSelectedItem();
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00053D6C File Offset: 0x00051F6C
		protected override void SetItem(int index, IContextMenuModel item)
		{
			IContextMenuModel oldItem = base[index];
			SelectableContextMenuItemModel selectableContextMenuItemModel = oldItem as SelectableContextMenuItemModel;
			if (selectableContextMenuItemModel != null)
			{
				WeakEventManager<SelectableContextMenuItemModel, SelectableModelSelectionChangedEventArgs>.AddHandler(selectableContextMenuItemModel, "SelectionChanged", new EventHandler<SelectableModelSelectionChangedEventArgs>(this.Item_SelectionChanged));
			}
			if (!(item is ContextMenuSeparator))
			{
				TypedContextMenuItemModel model = item as TypedContextMenuItemModel;
				if (model == null || !this.Where((IContextMenuModel c) => c != oldItem).OfType<TypedContextMenuItemModel>().All((TypedContextMenuItemModel c) => c.Type != model.Type))
				{
					base.InsertItem(index, item);
					goto IL_00B2;
				}
			}
			base.SetItem(index, item);
			SelectableContextMenuItemModel selectableContextMenuItemModel2 = item as SelectableContextMenuItemModel;
			if (selectableContextMenuItemModel2 != null)
			{
				WeakEventManager<SelectableContextMenuItemModel, SelectableModelSelectionChangedEventArgs>.AddHandler(selectableContextMenuItemModel2, "SelectionChanged", new EventHandler<SelectableModelSelectionChangedEventArgs>(this.Item_SelectionChanged));
			}
			IL_00B2:
			this.UpdateSelectedItem();
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x00053E34 File Offset: 0x00052034
		protected override void RemoveItem(int index)
		{
			SelectableContextMenuItemModel selectableContextMenuItemModel = base[index] as SelectableContextMenuItemModel;
			if (selectableContextMenuItemModel != null)
			{
				WeakEventManager<SelectableContextMenuItemModel, SelectableModelSelectionChangedEventArgs>.RemoveHandler(selectableContextMenuItemModel, "SelectionChanged", new EventHandler<SelectableModelSelectionChangedEventArgs>(this.Item_SelectionChanged));
			}
			base.RemoveItem(index);
			this.UpdateSelectedItem();
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x00053E78 File Offset: 0x00052078
		protected override void ClearItems()
		{
			foreach (IContextMenuModel contextMenuModel in this)
			{
				SelectableContextMenuItemModel selectableContextMenuItemModel = contextMenuModel as SelectableContextMenuItemModel;
				if (selectableContextMenuItemModel != null)
				{
					WeakEventManager<SelectableContextMenuItemModel, SelectableModelSelectionChangedEventArgs>.RemoveHandler(selectableContextMenuItemModel, "SelectionChanged", new EventHandler<SelectableModelSelectionChangedEventArgs>(this.Item_SelectionChanged));
				}
			}
			base.ClearItems();
			this.UpdateSelectedItem();
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x00053EE4 File Offset: 0x000520E4
		private void Item_SelectionChanged(object sender, SelectableModelSelectionChangedEventArgs args)
		{
			this.UpdateSelectedItem();
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x00053EEC File Offset: 0x000520EC
		private void UpdateSelectedItem()
		{
			base.OnPropertyChanged("SelectedItems");
		}

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060015AD RID: 5549 RVA: 0x00053EFC File Offset: 0x000520FC
		// (remove) Token: 0x060015AE RID: 5550 RVA: 0x00053F34 File Offset: 0x00052134
		public event EventHandler<SelectedAccessorSelectionChangedEventArgs> SelectionChanged;

		// Token: 0x04000733 RID: 1843
		private ToolbarChildCheckableButtonModel owner;
	}
}
