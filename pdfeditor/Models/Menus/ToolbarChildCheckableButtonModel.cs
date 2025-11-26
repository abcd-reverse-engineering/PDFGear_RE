using System;
using System.Windows;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000166 RID: 358
	public class ToolbarChildCheckableButtonModel : ToolbarChildButtonModel
	{
		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x0600158B RID: 5515 RVA: 0x00053A61 File Offset: 0x00051C61
		// (set) Token: 0x0600158C RID: 5516 RVA: 0x00053A69 File Offset: 0x00051C69
		public bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
			}
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x0600158D RID: 5517 RVA: 0x00053A7E File Offset: 0x00051C7E
		// (set) Token: 0x0600158E RID: 5518 RVA: 0x00053A86 File Offset: 0x00051C86
		public bool OpenContextMenuOnChecked
		{
			get
			{
				return this.openContextMenuOnChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.openContextMenuOnChecked, value, "OpenContextMenuOnChecked");
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x0600158F RID: 5519 RVA: 0x00053A9B File Offset: 0x00051C9B
		// (set) Token: 0x06001590 RID: 5520 RVA: 0x00053AA4 File Offset: 0x00051CA4
		public IContextMenuModel ContextMenu
		{
			get
			{
				return this.contextMenu;
			}
			set
			{
				IContextMenuModel contextMenuModel = this.contextMenu;
				if (base.SetProperty<IContextMenuModel>(ref this.contextMenu, value, "ContextMenu"))
				{
					TypedContextMenuModel typedContextMenuModel = contextMenuModel as TypedContextMenuModel;
					if (typedContextMenuModel != null)
					{
						typedContextMenuModel.Owner = null;
						WeakEventManager<TypedContextMenuModel, SelectedAccessorSelectionChangedEventArgs>.RemoveHandler(typedContextMenuModel, "SelectionChanged", new EventHandler<SelectedAccessorSelectionChangedEventArgs>(this.ContextMenu_SelectionChanged));
					}
					TypedContextMenuModel typedContextMenuModel2 = this.contextMenu as TypedContextMenuModel;
					if (typedContextMenuModel2 != null)
					{
						typedContextMenuModel2.Owner = this;
						WeakEventManager<TypedContextMenuModel, SelectedAccessorSelectionChangedEventArgs>.AddHandler(typedContextMenuModel2, "SelectionChanged", new EventHandler<SelectedAccessorSelectionChangedEventArgs>(this.ContextMenu_SelectionChanged));
					}
				}
			}
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x00053B21 File Offset: 0x00051D21
		private void ContextMenu_SelectionChanged(object sender, SelectedAccessorSelectionChangedEventArgs e)
		{
			EventHandler<SelectedAccessorSelectionChangedEventArgs> contextMenuSelectionChanged = this.ContextMenuSelectionChanged;
			if (contextMenuSelectionChanged == null)
			{
				return;
			}
			contextMenuSelectionChanged(this, e);
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x00053B35 File Offset: 0x00051D35
		public override void Tap()
		{
			this.IsChecked = !this.IsChecked;
			base.Tap();
		}

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001593 RID: 5523 RVA: 0x00053B4C File Offset: 0x00051D4C
		// (remove) Token: 0x06001594 RID: 5524 RVA: 0x00053B84 File Offset: 0x00051D84
		public event EventHandler<SelectedAccessorSelectionChangedEventArgs> ContextMenuSelectionChanged;

		// Token: 0x0400072A RID: 1834
		private IContextMenuModel contextMenu;

		// Token: 0x0400072B RID: 1835
		private bool isChecked;

		// Token: 0x0400072C RID: 1836
		private bool openContextMenuOnChecked = true;
	}
}
