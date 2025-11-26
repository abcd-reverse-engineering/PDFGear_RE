using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Attachments
{
	// Token: 0x02000193 RID: 403
	public class PDFAttachmentWrapper : INotifyPropertyChanged
	{
		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00058904 File Offset: 0x00056B04
		// (set) Token: 0x06001729 RID: 5929 RVA: 0x0005890C File Offset: 0x00056B0C
		public object Attachment { get; set; }

		// Token: 0x0600172A RID: 5930 RVA: 0x00058915 File Offset: 0x00056B15
		public PDFAttachmentWrapper(PdfAttachment pdfAttachment)
		{
			this.Attachment = pdfAttachment;
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x00058924 File Offset: 0x00056B24
		public PDFAttachmentWrapper(PdfFileAttachmentAnnotation attachmentAnnotation)
		{
			this.Attachment = attachmentAnnotation;
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00058934 File Offset: 0x00056B34
		public PDFAttachmentWrapper(AttachmentFileUtils.PdfFileAttachmentAnnotationResult result)
		{
			PdfAnnotation pdfAnnotation = Ioc.Default.GetRequiredService<MainViewModel>().Document.Pages[result.PageIndex].Annots.FirstOrDefault((PdfAnnotation t) => t.Dictionary.Handle == result.AnnotationDictionary.Handle);
			if (pdfAnnotation is PdfFileAttachmentAnnotation)
			{
				this.Attachment = pdfAnnotation;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x000589A0 File Offset: 0x00056BA0
		public string FileName
		{
			get
			{
				try
				{
					PdfFileSpecification pdfFileSpecification = null;
					PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
					if (pdfAttachment != null && !string.IsNullOrEmpty(pdfAttachment.FileSpecification.FileName))
					{
						pdfFileSpecification = pdfAttachment.FileSpecification;
					}
					else
					{
						PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
						if (pdfFileAttachmentAnnotation != null && !string.IsNullOrEmpty(pdfFileAttachmentAnnotation.FileSpecification.FileName))
						{
							pdfFileSpecification = pdfFileAttachmentAnnotation.FileSpecification;
						}
					}
					if (pdfFileSpecification != null)
					{
						return Path.GetFileName(pdfFileSpecification.FileName);
					}
				}
				catch
				{
				}
				return "";
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x0600172E RID: 5934 RVA: 0x00058A34 File Offset: 0x00056C34
		public string ModifyDate
		{
			get
			{
				string text = string.Empty;
				PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
				if (pdfAttachment != null)
				{
					text = pdfAttachment.FileSpecification.EmbeddedFile.ModificationDate;
				}
				else
				{
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
					if (pdfFileAttachmentAnnotation != null)
					{
						text = pdfFileAttachmentAnnotation.FileSpecification.EmbeddedFile.ModificationDate;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					return Resources.UnknownAttachmentModifyDateText;
				}
				DateTimeOffset dateTimeOffset;
				PdfObjectExtensions.TryParseModificationDate(text, out dateTimeOffset);
				return dateTimeOffset.ToString("yyyy/MM/dd HH:mm:ss");
			}
		}

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x00058AAC File Offset: 0x00056CAC
		public int FileSize
		{
			get
			{
				try
				{
					PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
					if (pdfAttachment != null)
					{
						return pdfAttachment.FileSpecification.EmbeddedFile.FileSize;
					}
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
					if (pdfFileAttachmentAnnotation != null)
					{
						return pdfFileAttachmentAnnotation.FileSpecification.EmbeddedFile.FileSize;
					}
				}
				catch
				{
				}
				return 0;
			}
		}

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06001730 RID: 5936 RVA: 0x00058B18 File Offset: 0x00056D18
		public string FileSizeFormat
		{
			get
			{
				return PDFAttachmentWrapper.FormatFileSize((long)this.FileSize);
			}
		}

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x00058B28 File Offset: 0x00056D28
		// (set) Token: 0x06001732 RID: 5938 RVA: 0x00058B88 File Offset: 0x00056D88
		public string Description
		{
			get
			{
				try
				{
					PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
					if (pdfAttachment != null)
					{
						return pdfAttachment.FileSpecification.Description;
					}
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
					if (pdfFileAttachmentAnnotation != null)
					{
						return pdfFileAttachmentAnnotation.Contents;
					}
				}
				catch
				{
				}
				return "";
			}
			set
			{
				PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
				if (pdfAttachment != null)
				{
					PdfFileSpecification fileSpecification = pdfAttachment.FileSpecification;
					if (fileSpecification != null)
					{
						if (value == null && fileSpecification.Dictionary.ContainsKey("Desc"))
						{
							fileSpecification.Dictionary.Remove("Desc");
						}
						else if (value != null)
						{
							fileSpecification.Dictionary["Desc"] = PdfTypeString.Create(value, true, true);
						}
					}
				}
				else
				{
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
					if (pdfFileAttachmentAnnotation != null)
					{
						pdfFileAttachmentAnnotation.Contents = value;
					}
				}
				this.OnPropertyChanged("Description");
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x00058C1C File Offset: 0x00056E1C
		public string PageIndex
		{
			get
			{
				PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
				if (pdfFileAttachmentAnnotation != null)
				{
					return Resources.LeftNavigationAnnotationPageLabelContent.Replace("XXX", (pdfFileAttachmentAnnotation.Page.PageIndex + 1).ToString() ?? "");
				}
				return Resources.AttachmentPanelAttachmentTab;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06001734 RID: 5940 RVA: 0x00058C6C File Offset: 0x00056E6C
		public string ImagePath
		{
			get
			{
				string text = "";
				try
				{
					PdfAttachment pdfAttachment = this.Attachment as PdfAttachment;
					if (pdfAttachment != null && !string.IsNullOrEmpty(pdfAttachment.Name))
					{
						text = Path.GetExtension(pdfAttachment.Name);
					}
					else
					{
						PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
						if (pdfFileAttachmentAnnotation != null && !string.IsNullOrEmpty(pdfFileAttachmentAnnotation.FileSpecification.FileName))
						{
							text = Path.GetExtension(pdfFileAttachmentAnnotation.FileSpecification.FileName);
						}
					}
				}
				catch
				{
				}
				string text2;
				if (text != null && PDFAttachmentWrapper._extensionMap.TryGetValue(text, out text2))
				{
					return text2;
				}
				return "/Style/Resources/ExtIcon/Attachment.png";
			}
		}

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06001735 RID: 5941 RVA: 0x00058D0C File Offset: 0x00056F0C
		public int OrderIndex
		{
			get
			{
				PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Attachment as PdfFileAttachmentAnnotation;
				if (pdfFileAttachmentAnnotation != null)
				{
					return pdfFileAttachmentAnnotation.Page.PageIndex;
				}
				return 0;
			}
		}

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x00058D35 File Offset: 0x00056F35
		public bool IsAttachmentAnnotation
		{
			get
			{
				return this.Attachment is PdfFileAttachmentAnnotation;
			}
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x00058D48 File Offset: 0x00056F48
		public static string FormatFileSize(long fileSize)
		{
			string[] array = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
			if (fileSize == 0L)
			{
				return "0" + array[0];
			}
			int num = (int)Math.Floor(Math.Log((double)fileSize, 1024.0));
			double num2 = Math.Round((double)fileSize / Math.Pow(1024.0, (double)num), 2);
			return string.Format("{0} {1}", num2, array[num]);
		}

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001738 RID: 5944 RVA: 0x00058DFC File Offset: 0x00056FFC
		// (remove) Token: 0x06001739 RID: 5945 RVA: 0x00058E34 File Offset: 0x00057034
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x0600173A RID: 5946 RVA: 0x00058E69 File Offset: 0x00057069
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00058E84 File Offset: 0x00057084
		// Note: this type is marked as 'beforefieldinit'.
		static PDFAttachmentWrapper()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			dictionary[".doc"] = "/Style/Resources/ExtIcon/Word.png";
			dictionary[".docx"] = "/Style/Resources/ExtIcon/Word.png";
			dictionary[".xls"] = "/Style/Resources/ExtIcon/EXCEL.png";
			dictionary[".xlsx"] = "/Style/Resources/ExtIcon/EXCEL.png";
			dictionary[".csv"] = "/Style/Resources/ExtIcon/EXCEL.png";
			dictionary[".ppt"] = "/Style/Resources/ExtIcon/PPT.png";
			dictionary[".pptx"] = "/Style/Resources/ExtIcon/PPT.png";
			dictionary[".txt"] = "/Style/Resources/ExtIcon/TEXT.png";
			dictionary[".rtf"] = "/Style/Resources/ExtIcon/TEXT.png";
			dictionary[".json"] = "/Style/Resources/ExtIcon/TEXT.png";
			dictionary[".xml"] = "/Style/Resources/ExtIcon/TEXT.png";
			dictionary[".html"] = "/Style/Resources/ExtIcon/TEXT.png";
			dictionary[".jpg"] = "/Style/Resources/ExtIcon/IMAGE.png";
			dictionary[".jpeg"] = "/Style/Resources/ExtIcon/IMAGE.png";
			dictionary[".png"] = "/Style/Resources/ExtIcon/IMAGE.png";
			dictionary[".bmp"] = "/Style/Resources/ExtIcon/IMAGE.png";
			dictionary[".ico"] = "/Style/Resources/ExtIcon/IMAGE.png";
			dictionary[".mp3"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".mp4"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".avi"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".mov"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".swf"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".vob"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".mpeg"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".wav"] = "/Style/Resources/ExtIcon/VIDEO.png";
			dictionary[".zip"] = "/Style/Resources/ExtIcon/Archive.png";
			dictionary[".rar"] = "/Style/Resources/ExtIcon/Archive.png";
			dictionary[".7z"] = "/Style/Resources/ExtIcon/Archive.png";
			dictionary[".tar"] = "/Style/Resources/ExtIcon/Archive.png";
			dictionary[".gz"] = "/Style/Resources/ExtIcon/Archive.png";
			dictionary[".pdf"] = "/Style/Resources/ExtIcon/Pdf.png";
			PDFAttachmentWrapper._extensionMap = dictionary;
		}

		// Token: 0x040007BC RID: 1980
		private static readonly Dictionary<string, string> _extensionMap;
	}
}
