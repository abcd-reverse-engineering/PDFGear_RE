using System;
using System.Linq;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Menus;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001AE RID: 430
	public class PdfAnnotationPropertyAccessor
	{
		// Token: 0x0600184D RID: 6221 RVA: 0x0005CA57 File Offset: 0x0005AC57
		public PdfAnnotationPropertyAccessor(PdfAnnotation pdfAnnotation, AnnotationMode mode)
		{
			if (pdfAnnotation == null)
			{
				throw new ArgumentNullException("pdfAnnotation");
			}
			this.Mode = mode;
			if (this.Mode != AnnotationMode.None)
			{
				this.annotation = AnnotationFactory.Create(pdfAnnotation);
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x0600184E RID: 6222 RVA: 0x0005CA89 File Offset: 0x0005AC89
		public AnnotationMode Mode { get; }

		// Token: 0x0600184F RID: 6223 RVA: 0x0005CA91 File Offset: 0x0005AC91
		public object GetValue(ContextMenuItemType type)
		{
			if (this.annotation == null)
			{
				return null;
			}
			return this.annotation.GetValue(this.Mode, type);
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x0005CAB0 File Offset: 0x0005ACB0
		public ContextMenuItemModel GetOrCreateContextMenuItem(TypedContextMenuItemModel menu, Action<ContextMenuItemModel> action)
		{
			if (menu == null)
			{
				throw new ArgumentNullException("menu");
			}
			TypedContextMenuModel typedContextMenuModel = menu.Parent as TypedContextMenuModel;
			if (typedContextMenuModel == null || typedContextMenuModel.Mode != this.Mode)
			{
				return null;
			}
			ContextMenuItemType type = menu.Type;
			if (type == ContextMenuItemType.None)
			{
				return null;
			}
			object value = this.GetValue(type);
			if (value == null)
			{
				return null;
			}
			ContextMenuItemModel contextMenuItemModel = menu.OfType<ContextMenuItemModel>().FirstOrDefault((ContextMenuItemModel c) => !(c is ColorMoreItemContextMenuItemModel) && ToolbarContextMenuValueEqualityComparer.MenuValueEquals(this.Mode, type, c.TagData.MenuItemValue, value));
			if (contextMenuItemModel != null)
			{
				return contextMenuItemModel;
			}
			ContextMenuItemModel contextMenuItemModel2 = ToolbarContextMenuHelper.CreateContextMenuItem(this.Mode, type, value, true, action);
			for (int i = menu.Count - 1; i >= 0; i--)
			{
				if (!(menu[i] is ColorMoreItemContextMenuItemModel))
				{
					menu.Insert(i + 1, contextMenuItemModel2);
					break;
				}
			}
			return contextMenuItemModel2;
		}

		// Token: 0x0400081B RID: 2075
		private readonly BaseAnnotation annotation;
	}
}
