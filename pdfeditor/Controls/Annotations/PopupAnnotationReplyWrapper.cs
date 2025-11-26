using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A7 RID: 679
	public class PopupAnnotationReplyWrapper : ObservableObject
	{
		// Token: 0x06002714 RID: 10004 RVA: 0x000B8EC9 File Offset: 0x000B70C9
		public PopupAnnotationReplyWrapper(PdfMarkupAnnotation annot)
		{
			this.rawObject = annot;
			this.parent = new Lazy<PdfAnnotation>(delegate
			{
				PdfAnnotation pdfAnnotation = this.rawObject.RelationshipAnnotation;
				for (;;)
				{
					PdfTextAnnotation pdfTextAnnotation = pdfAnnotation as PdfTextAnnotation;
					if (pdfTextAnnotation == null || pdfTextAnnotation.Relationship != RelationTypes.Reply || !(pdfTextAnnotation.RelationshipAnnotation != null))
					{
						break;
					}
					pdfAnnotation = pdfTextAnnotation.RelationshipAnnotation;
				}
				return pdfAnnotation;
			});
		}

		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x06002715 RID: 10005 RVA: 0x000B8EEF File Offset: 0x000B70EF
		public PdfMarkupAnnotation Annotation
		{
			get
			{
				return this.rawObject;
			}
		}

		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x06002716 RID: 10006 RVA: 0x000B8EF7 File Offset: 0x000B70F7
		public PdfPage Page
		{
			get
			{
				return this.rawObject.Page;
			}
		}

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x06002717 RID: 10007 RVA: 0x000B8F04 File Offset: 0x000B7104
		public PdfAnnotation Parent
		{
			get
			{
				return this.parent.Value;
			}
		}

		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x06002718 RID: 10008 RVA: 0x000B8F11 File Offset: 0x000B7111
		public int AnnotationIndex
		{
			get
			{
				PdfAnnotationCollection annots = this.rawObject.Page.Annots;
				if (annots == null)
				{
					return -1;
				}
				return annots.IndexOf(this.rawObject);
			}
		}

		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x06002719 RID: 10009 RVA: 0x000B8F34 File Offset: 0x000B7134
		// (set) Token: 0x0600271A RID: 10010 RVA: 0x000B8F4C File Offset: 0x000B714C
		public string Contents
		{
			get
			{
				return this.rawObject.Contents ?? "";
			}
			set
			{
				string text = value ?? "";
				if (this.rawObject.Contents != text)
				{
					this.rawObject.Contents = text;
					base.OnPropertyChanged("Contents");
				}
			}
		}

		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x0600271B RID: 10011 RVA: 0x000B8F8E File Offset: 0x000B718E
		public string Text
		{
			get
			{
				return this.rawObject.Text;
			}
		}

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x0600271C RID: 10012 RVA: 0x000B8F9C File Offset: 0x000B719C
		// (set) Token: 0x0600271D RID: 10013 RVA: 0x000B8FC8 File Offset: 0x000B71C8
		public DateTimeOffset? ModificationDate
		{
			get
			{
				DateTimeOffset dateTimeOffset;
				if (this.rawObject.TryGetModificationDate(out dateTimeOffset))
				{
					return new DateTimeOffset?(dateTimeOffset);
				}
				return null;
			}
			set
			{
				this.rawObject.ModificationDate = value.Value.ToModificationDateString();
				base.OnPropertyChanged("ModificationDate");
				base.OnPropertyChanged("ModificationDateText");
				base.OnPropertyChanged("ModificationDateTextShort");
			}
		}

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x0600271E RID: 10014 RVA: 0x000B9004 File Offset: 0x000B7204
		public string ModificationDateText
		{
			get
			{
				DateTimeOffset? modificationDate = this.ModificationDate;
				if (modificationDate != null)
				{
					return modificationDate.Value.ToString("G");
				}
				return string.Empty;
			}
		}

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x0600271F RID: 10015 RVA: 0x000B903C File Offset: 0x000B723C
		public string ModificationDateTextShort
		{
			get
			{
				DateTimeOffset? modificationDate = this.ModificationDate;
				if (modificationDate != null)
				{
					return modificationDate.Value.ToString("d");
				}
				return string.Empty;
			}
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000B9073 File Offset: 0x000B7273
		public void NotifyAnnotationChanged()
		{
			base.OnPropertyChanged("Contents");
			base.OnPropertyChanged("ModificationDate");
			base.OnPropertyChanged("ModificationDateText");
			base.OnPropertyChanged("ModificationDateTextShort");
		}

		// Token: 0x040010DC RID: 4316
		private PdfMarkupAnnotation rawObject;

		// Token: 0x040010DD RID: 4317
		private Lazy<PdfAnnotation> parent;
	}
}
