using System;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000176 RID: 374
	public class ToolbarSettingId : IEquatable<ToolbarSettingId>
	{
		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x00054384 File Offset: 0x00052584
		public static ToolbarSettingId None
		{
			get
			{
				ToolbarSettingId toolbarSettingId;
				if ((toolbarSettingId = ToolbarSettingId.none) == null)
				{
					ToolbarSettingId toolbarSettingId2 = new ToolbarSettingId();
					toolbarSettingId2.type = ToolbarSettingType.None;
					toolbarSettingId2.annotationMode = AnnotationMode.None;
					toolbarSettingId = toolbarSettingId2;
					ToolbarSettingId.none = toolbarSettingId2;
				}
				return toolbarSettingId;
			}
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000543A8 File Offset: 0x000525A8
		private ToolbarSettingId()
		{
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060015D5 RID: 5589 RVA: 0x000543B0 File Offset: 0x000525B0
		public ToolbarSettingType Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060015D6 RID: 5590 RVA: 0x000543B8 File Offset: 0x000525B8
		public AnnotationMode AnnotationMode
		{
			get
			{
				return this.annotationMode;
			}
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x000543C0 File Offset: 0x000525C0
		public static ToolbarSettingId CreateAnnotation(AnnotationMode annotationMode)
		{
			return new ToolbarSettingId
			{
				type = ToolbarSettingType.Annotation,
				annotationMode = annotationMode
			};
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x000543D5 File Offset: 0x000525D5
		public static ToolbarSettingId CreateEditDocument()
		{
			return new ToolbarSettingId
			{
				type = ToolbarSettingType.EditDocument
			};
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x000543E3 File Offset: 0x000525E3
		public static ToolbarSettingId CreateRedact()
		{
			return new ToolbarSettingId
			{
				type = ToolbarSettingType.Redact
			};
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x000543F1 File Offset: 0x000525F1
		public static bool operator ==(ToolbarSettingId left, ToolbarSettingId right)
		{
			return (left == null && right == null) || (left != null && right != null && left.Equals(right));
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x0005440A File Offset: 0x0005260A
		public static bool operator !=(ToolbarSettingId left, ToolbarSettingId right)
		{
			return !(left == right);
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00054416 File Offset: 0x00052616
		public override int GetHashCode()
		{
			return HashCode.Combine<ToolbarSettingType, AnnotationMode>(this.type, this.annotationMode);
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x0005442C File Offset: 0x0005262C
		public override bool Equals(object obj)
		{
			ToolbarSettingId toolbarSettingId = obj as ToolbarSettingId;
			return toolbarSettingId != null && toolbarSettingId.Equals(this);
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x0005444C File Offset: 0x0005264C
		public bool Equals(ToolbarSettingId other)
		{
			return this.type == other.type && this.annotationMode == other.annotationMode;
		}

		// Token: 0x0400074A RID: 1866
		private static ToolbarSettingId none;

		// Token: 0x0400074B RID: 1867
		private ToolbarSettingType type;

		// Token: 0x0400074C RID: 1868
		private AnnotationMode annotationMode;
	}
}
