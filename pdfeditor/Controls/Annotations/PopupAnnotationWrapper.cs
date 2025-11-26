using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A6 RID: 678
	public class PopupAnnotationWrapper : ObservableObject
	{
		// Token: 0x060026FC RID: 9980 RVA: 0x000B89F0 File Offset: 0x000B6BF0
		public PopupAnnotationWrapper(PdfPopupAnnotation annot)
		{
			this.rawObject = annot;
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x060026FD RID: 9981 RVA: 0x000B89FF File Offset: 0x000B6BFF
		public PdfPopupAnnotation Annotation
		{
			get
			{
				return this.rawObject;
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x060026FE RID: 9982 RVA: 0x000B8A07 File Offset: 0x000B6C07
		// (set) Token: 0x060026FF RID: 9983 RVA: 0x000B8A0F File Offset: 0x000B6C0F
		public ObservableCollection<PopupAnnotationReplyWrapper> Replies
		{
			get
			{
				return this.replies;
			}
			set
			{
				base.SetProperty<ObservableCollection<PopupAnnotationReplyWrapper>>(ref this.replies, value, "Replies");
			}
		}

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x06002700 RID: 9984 RVA: 0x000B8A24 File Offset: 0x000B6C24
		public PdfPage Page
		{
			get
			{
				return this.rawObject.Page;
			}
		}

		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x06002701 RID: 9985 RVA: 0x000B8A31 File Offset: 0x000B6C31
		public PdfAnnotation Parent
		{
			get
			{
				return this.rawObject.Parent;
			}
		}

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x06002702 RID: 9986 RVA: 0x000B8A3E File Offset: 0x000B6C3E
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

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06002703 RID: 9987 RVA: 0x000B8A61 File Offset: 0x000B6C61
		// (set) Token: 0x06002704 RID: 9988 RVA: 0x000B8A6C File Offset: 0x000B6C6C
		public string Contents
		{
			get
			{
				return this.GetContent();
			}
			set
			{
				string text = value ?? "";
				if (this.GetContent() != text)
				{
					this.rawObject.Contents = text;
					base.OnPropertyChanged("Contents");
				}
			}
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x000B8AAC File Offset: 0x000B6CAC
		private string GetContent()
		{
			try
			{
				return this.rawObject.Contents ?? "";
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x06002706 RID: 9990 RVA: 0x000B8AEC File Offset: 0x000B6CEC
		// (set) Token: 0x06002707 RID: 9991 RVA: 0x000B8AF9 File Offset: 0x000B6CF9
		public bool IsOpen
		{
			get
			{
				return this.rawObject.IsOpen;
			}
			set
			{
				if (this.rawObject.IsOpen != value)
				{
					this.rawObject.IsOpen = value;
					base.OnPropertyChanged("IsOpen");
				}
			}
		}

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x06002708 RID: 9992 RVA: 0x000B8B20 File Offset: 0x000B6D20
		// (set) Token: 0x06002709 RID: 9993 RVA: 0x000B8CCF File Offset: 0x000B6ECF
		public FS_RECTF Rectangle
		{
			get
			{
				PdfTypeBase pdfTypeBase;
				if (this.rawObject.Dictionary.TryGetValue("Rect", out pdfTypeBase))
				{
					PdfTypeArray pdfTypeArray = pdfTypeBase.As<PdfTypeArray>(true);
					float floatValue = pdfTypeArray[0].As<PdfTypeNumber>(true).FloatValue;
					float floatValue2 = pdfTypeArray[1].As<PdfTypeNumber>(true).FloatValue;
					float floatValue3 = pdfTypeArray[2].As<PdfTypeNumber>(true).FloatValue;
					float floatValue4 = pdfTypeArray[3].As<PdfTypeNumber>(true).FloatValue;
					return new FS_RECTF(Math.Min(floatValue, floatValue3), Math.Max(floatValue2, floatValue4), Math.Max(floatValue, floatValue3), Math.Min(floatValue2, floatValue4));
				}
				PdfAnnotation parent = this.rawObject.Parent;
				PdfTypeBase pdfTypeBase2;
				if (((parent != null) ? parent.Dictionary : null) != null && this.rawObject.Parent.Dictionary.TryGetValue("Rect", out pdfTypeBase2))
				{
					FS_RECTF fs_RECTF = new FS_RECTF(pdfTypeBase2);
					FS_RECTF fs_RECTF2 = new FS_RECTF(fs_RECTF.right + 40f, fs_RECTF.top, fs_RECTF.right + 40f + 180f, fs_RECTF.top - 140f);
					this.rawObject.Dictionary["Rect"] = fs_RECTF2.ToArray();
					return fs_RECTF2;
				}
				PdfPage page = this.rawObject.Page;
				FS_RECTF fs_RECTF3 = new FS_RECTF(page.Width - 200f, page.Height / 2f + 70f, page.Width + 20f, page.Height / 2f - 70f);
				this.rawObject.Dictionary["Rect"] = fs_RECTF3.ToArray();
				return fs_RECTF3;
			}
			set
			{
				if (this.rawObject.Rectangle != value)
				{
					this.rawObject.Rectangle = value;
					base.OnPropertyChanged("Rectangle");
				}
			}
		}

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x0600270A RID: 9994 RVA: 0x000B8CFB File Offset: 0x000B6EFB
		// (set) Token: 0x0600270B RID: 9995 RVA: 0x000B8D03 File Offset: 0x000B6F03
		public FS_COLOR Color
		{
			get
			{
				return this.GetColor();
			}
			set
			{
				if (this.GetColor() != value)
				{
					this.rawObject.Color = value;
					base.OnPropertyChanged("Color");
					base.OnPropertyChanged("BackgroundColor");
				}
			}
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x000B8D38 File Offset: 0x000B6F38
		private FS_COLOR GetColor()
		{
			try
			{
				return this.rawObject.Color;
			}
			catch
			{
			}
			return FS_COLOR.White;
		}

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x0600270D RID: 9997 RVA: 0x000B8D70 File Offset: 0x000B6F70
		public Color BackgroundColor
		{
			get
			{
				return this.Color.ToColor();
			}
		}

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x0600270E RID: 9998 RVA: 0x000B8D7D File Offset: 0x000B6F7D
		public string Text
		{
			get
			{
				return this.rawObject.Text;
			}
		}

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x0600270F RID: 9999 RVA: 0x000B8D8C File Offset: 0x000B6F8C
		// (set) Token: 0x06002710 RID: 10000 RVA: 0x000B8DB8 File Offset: 0x000B6FB8
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

		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06002711 RID: 10001 RVA: 0x000B8DF4 File Offset: 0x000B6FF4
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

		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06002712 RID: 10002 RVA: 0x000B8E2C File Offset: 0x000B702C
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

		// Token: 0x06002713 RID: 10003 RVA: 0x000B8E64 File Offset: 0x000B7064
		public void NotifyAnnotationChanged()
		{
			base.OnPropertyChanged("Contents");
			base.OnPropertyChanged("IsOpen");
			base.OnPropertyChanged("Rectangle");
			base.OnPropertyChanged("Color");
			base.OnPropertyChanged("BackgroundColor");
			base.OnPropertyChanged("ModificationDate");
			base.OnPropertyChanged("ModificationDateText");
			base.OnPropertyChanged("ModificationDateTextShort");
		}

		// Token: 0x040010DA RID: 4314
		private PdfPopupAnnotation rawObject;

		// Token: 0x040010DB RID: 4315
		private ObservableCollection<PopupAnnotationReplyWrapper> replies;
	}
}
