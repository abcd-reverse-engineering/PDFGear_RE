using System;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Models.Annotations;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Models.Commets
{
	// Token: 0x0200018C RID: 396
	public class CommetModel : ObservableObject, ITreeViewNode
	{
		// Token: 0x060016AA RID: 5802 RVA: 0x00056125 File Offset: 0x00054325
		private CommetModel()
		{
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00056130 File Offset: 0x00054330
		public CommetModel(BaseAnnotation annot, global::System.Collections.Generic.IReadOnlyList<CommetModel> replies, AnnotationMode mode)
		{
			if (annot == null)
			{
				throw new ArgumentNullException("annot");
			}
			this.Annotation = annot;
			this.Replies = replies ?? Array.Empty<CommetModel>();
			this.Contents = CommetModel.GetContent(this.Annotation);
			BaseMarkupAnnotation baseMarkupAnnotation = this.Annotation as BaseMarkupAnnotation;
			if (baseMarkupAnnotation != null)
			{
				this.Text = baseMarkupAnnotation.Text;
				this.IsContentReadOnly = !(baseMarkupAnnotation is FreeTextAnnotation);
			}
			else
			{
				this.IsContentReadOnly = true;
			}
			this.AnnotationMode = mode;
			this.Title = CommetModel.GetTitle(mode, annot);
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x060016AC RID: 5804 RVA: 0x000561C4 File Offset: 0x000543C4
		// (set) Token: 0x060016AD RID: 5805 RVA: 0x000561CC File Offset: 0x000543CC
		public BaseAnnotation Annotation { get; private set; }

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x000561D5 File Offset: 0x000543D5
		// (set) Token: 0x060016AF RID: 5807 RVA: 0x000561DD File Offset: 0x000543DD
		public AnnotationMode AnnotationMode { get; private set; }

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x000561E6 File Offset: 0x000543E6
		// (set) Token: 0x060016B1 RID: 5809 RVA: 0x000561EE File Offset: 0x000543EE
		public string Title { get; private set; }

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x060016B2 RID: 5810 RVA: 0x000561F7 File Offset: 0x000543F7
		// (set) Token: 0x060016B3 RID: 5811 RVA: 0x000561FF File Offset: 0x000543FF
		public string Text { get; private set; }

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x060016B4 RID: 5812 RVA: 0x00056208 File Offset: 0x00054408
		// (set) Token: 0x060016B5 RID: 5813 RVA: 0x00056210 File Offset: 0x00054410
		public bool IsContentReadOnly { get; private set; }

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x0005621C File Offset: 0x0005441C
		public AsyncRelayCommand DeleteSelectedAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteSelectedAnnotCmd) == null)
				{
					asyncRelayCommand = (this.deleteSelectedAnnotCmd = new AsyncRelayCommand(async delegate
					{
						PdfDocument document = Ioc.Default.GetRequiredService<MainViewModel>().Document;
						if (document != null)
						{
							GAManager.SendEvent("AnnotationMgmt", "SignalDeleteBtn", "Count", 1L);
							global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(document);
							if (pdfControl != null)
							{
								AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
								if (annotationHolderManager != null)
								{
									PdfAnnotation pdfAnnotation = document.Pages[this.Annotation.PageIndex].Annots[this.Annotation.AnnotIndex];
									if (pdfAnnotation != null)
									{
										PdfAnnotation pdfAnnotation2 = pdfAnnotation;
										PdfFreeTextAnnotation pdfFreeTextAnnotation = pdfAnnotation2 as PdfFreeTextAnnotation;
										if (pdfFreeTextAnnotation != null && string.IsNullOrEmpty(pdfFreeTextAnnotation.Contents) && pdfFreeTextAnnotation.Intent == AnnotationIntent.FreeTextTypeWriter)
										{
											PdfObjectExtensions.GetAnnotationCanvas(pdfControl).HolderManager.CancelAll();
											return;
										}
									}
									await annotationHolderManager.DeleteAnnotationAsync(pdfAnnotation, false);
								}
							}
						}
					}, () => !this.DeleteSelectedAnnotCmd.IsRunning && this.Annotation != null));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x060016B7 RID: 5815 RVA: 0x0005625C File Offset: 0x0005445C
		public DateTimeOffset? ModificationDate
		{
			get
			{
				string modificationDate = this.Annotation.ModificationDate;
				AttachmentAnnotation attachmentAnnotation = this.Annotation as AttachmentAnnotation;
				if (attachmentAnnotation != null && string.IsNullOrEmpty(modificationDate))
				{
					PdfFileSpecificationModel fileSpecification = attachmentAnnotation.FileSpecification;
					if (fileSpecification != null)
					{
						PdfFileModel embeddedFile = fileSpecification.EmbeddedFile;
						string text = ((embeddedFile != null) ? embeddedFile.ModificationDate : null);
					}
				}
				DateTimeOffset dateTimeOffset;
				if (PdfObjectExtensions.TryParseModificationDate(this.Annotation.ModificationDate, out dateTimeOffset))
				{
					return new DateTimeOffset?(dateTimeOffset);
				}
				return null;
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x060016B8 RID: 5816 RVA: 0x000562D0 File Offset: 0x000544D0
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

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x060016B9 RID: 5817 RVA: 0x00056307 File Offset: 0x00054507
		// (set) Token: 0x060016BA RID: 5818 RVA: 0x0005630F File Offset: 0x0005450F
		public string Contents
		{
			get
			{
				return this.contents;
			}
			set
			{
				base.SetProperty<string>(ref this.contents, value, "Contents");
			}
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060016BB RID: 5819 RVA: 0x00056324 File Offset: 0x00054524
		// (set) Token: 0x060016BC RID: 5820 RVA: 0x0005632C File Offset: 0x0005452C
		public global::System.Collections.Generic.IReadOnlyList<CommetModel> Replies { get; private set; }

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060016BD RID: 5821 RVA: 0x00056335 File Offset: 0x00054535
		// (set) Token: 0x060016BE RID: 5822 RVA: 0x0005633D File Offset: 0x0005453D
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelected, value, "IsSelected");
			}
		}

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x060016BF RID: 5823 RVA: 0x00056352 File Offset: 0x00054552
		// (set) Token: 0x060016C0 RID: 5824 RVA: 0x0005635A File Offset: 0x0005455A
		public bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
				this.judgeSelectall();
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00056378 File Offset: 0x00054578
		private void judgeSelectall()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			bool? flag = new bool?(true);
			foreach (PageCommetCollection pageCommetCollection in requiredService.PageCommetSource)
			{
				using (IEnumerator<CommetModel> enumerator2 = pageCommetCollection.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (!enumerator2.Current.IsChecked)
						{
							flag = null;
						}
					}
				}
			}
			if (flag == null)
			{
				flag = new bool?(false);
				foreach (PageCommetCollection pageCommetCollection2 in requiredService.PageCommetSource)
				{
					using (IEnumerator<CommetModel> enumerator2 = pageCommetCollection2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.IsChecked)
							{
								flag = null;
							}
						}
					}
				}
			}
			requiredService.IsSelectedAll = flag;
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x060016C2 RID: 5826 RVA: 0x00056498 File Offset: 0x00054698
		// (set) Token: 0x060016C3 RID: 5827 RVA: 0x000564A0 File Offset: 0x000546A0
		public ITreeViewNode Parent { get; set; }

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x060016C4 RID: 5828 RVA: 0x000564A9 File Offset: 0x000546A9
		public bool IsDeleteAreaVisible
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>().IsDeleteAreaVisible;
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x000564BC File Offset: 0x000546BC
		public static CommetModel TryCreate(BaseAnnotation annot, global::System.Collections.Generic.IReadOnlyList<CommetModel> replies)
		{
			if (annot is PopupAnnotation)
			{
				return null;
			}
			InkAnnotation inkAnnotation = annot as InkAnnotation;
			if (inkAnnotation != null && inkAnnotation.InkList != null && inkAnnotation.InkList.Count <= 1)
			{
				int num = 0;
				foreach (global::System.Collections.Generic.IReadOnlyList<FS_POINTF> readOnlyList in inkAnnotation.InkList)
				{
					num += readOnlyList.Count;
				}
				if (num <= 1)
				{
					return null;
				}
			}
			global::System.Collections.Generic.IReadOnlyList<AnnotationMode> annotationModes = AnnotationFactory.GetAnnotationModes(annot);
			if (annotationModes.Count == 0 || annotationModes[0] == AnnotationMode.None)
			{
				return null;
			}
			return new CommetModel(annot, replies, annotationModes[0]);
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x00056568 File Offset: 0x00054768
		private static string GetTitle(AnnotationMode mode, BaseAnnotation annot)
		{
			switch (mode)
			{
			case AnnotationMode.Line:
			case AnnotationMode.Arrow:
			{
				LineAnnotation lineAnnotation = annot as LineAnnotation;
				if (lineAnnotation != null && lineAnnotation.LineEnding != null && lineAnnotation.LineEnding.Count > 0)
				{
					if (lineAnnotation.LineEnding.Any((LineEndingStyles c) => c == LineEndingStyles.ClosedArrow || c == LineEndingStyles.OpenArrow || c == LineEndingStyles.RClosedArrow || c == LineEndingStyles.ROpenArrow))
					{
						return Resources.MenuAnnotateArrowContent;
					}
				}
				return Resources.MenuAnnotateLineContent;
			}
			case AnnotationMode.Ink:
				return Resources.MenuAnnotateInkContent;
			case AnnotationMode.Shape:
				return Resources.MenuAnnotateShapeContent;
			case AnnotationMode.Highlight:
			{
				HighlightAnnotation highlightAnnotation = annot as HighlightAnnotation;
				if (highlightAnnotation == null)
				{
					return Resources.WinToolBarBtnHighlightContent;
				}
				if (!(highlightAnnotation.Subject == "AreaHighlight"))
				{
					return Resources.MenuAnnotateHighlightContent;
				}
				return Resources.WinToolBarBtnHighlightContent;
			}
			case AnnotationMode.Underline:
				return Resources.MenuAnnotateUnderlineContent;
			case AnnotationMode.Strike:
				return Resources.MenuAnnotateStrikeContent;
			case AnnotationMode.HighlightArea:
				return Resources.MenuAnnotateHighlight;
			case AnnotationMode.Note:
				return Resources.MenuAnnotateNoteContent;
			case AnnotationMode.Ellipse:
				return Resources.MenuAnnotateEllipseContent;
			case AnnotationMode.Stamp:
			{
				StampAnnotation stampAnnotation = annot as StampAnnotation;
				if (stampAnnotation == null)
				{
					return Resources.MenuAnnotateStampContent;
				}
				if (!(stampAnnotation.Subject == "Signature"))
				{
					return Resources.MenuAnnotateStampContent;
				}
				return Resources.MenuAnnotateSignatureContent;
			}
			case AnnotationMode.Text:
			case AnnotationMode.TextBox:
			{
				FreeTextAnnotation freeTextAnnotation = annot as FreeTextAnnotation;
				if (freeTextAnnotation != null && freeTextAnnotation.Intent == AnnotationIntent.FreeTextTypeWriter)
				{
					return Resources.MenuAnnotateTypeWriterContent;
				}
				return Resources.MenuAnnotateTextBoxContent;
			}
			case AnnotationMode.Signature:
				return Resources.MenuAnnotateSignatureContent;
			case AnnotationMode.Attachment:
				return Resources.AnnotationFileAttachment;
			}
			return null;
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x000566E0 File Offset: 0x000548E0
		private static string GetContent(BaseAnnotation annot)
		{
			StampAnnotation stampAnnotation = annot as StampAnnotation;
			if (stampAnnotation != null)
			{
				PdfTypeBase pdfTypeBase;
				if (stampAnnotation.PDFXExtend != null && stampAnnotation.PDFXExtend.TryGetValue("Type", out pdfTypeBase) && pdfTypeBase.Is<PdfTypeName>() && pdfTypeBase.As<PdfTypeName>(true).Value == "FormControl")
				{
					return "";
				}
				if (string.IsNullOrEmpty(annot.Contents) && stampAnnotation.PDFXExtend != null)
				{
					PDFExtStampDictionary pdfextStampDictionary = new PDFExtStampDictionary();
					if (stampAnnotation.PDFXExtend.ContainsKey("Type") && stampAnnotation.PDFXExtend["Type"].Is<PdfTypeName>())
					{
						pdfextStampDictionary.Type = stampAnnotation.PDFXExtend["Type"].As<PdfTypeName>(true).Value;
					}
					if (stampAnnotation.PDFXExtend.ContainsKey("Content") && stampAnnotation.PDFXExtend["Content"].Is<PdfTypeString>())
					{
						pdfextStampDictionary.Content = stampAnnotation.PDFXExtend["Content"].As<PdfTypeString>(true).UnicodeString;
					}
					if (stampAnnotation.PDFXExtend.ContainsKey("Template") && stampAnnotation.PDFXExtend["Template"].Is<PdfTypeString>())
					{
						pdfextStampDictionary.Template = stampAnnotation.PDFXExtend["Template"].As<PdfTypeString>(true).UnicodeString;
					}
					Dictionary<string, string> contentDictionary = pdfextStampDictionary.GetContentDictionary();
					string text;
					if (contentDictionary != null && contentDictionary.TryGetValue("ContentText", out text))
					{
						return text;
					}
					return stampAnnotation.ExtendedIconName;
				}
			}
			else
			{
				AttachmentAnnotation attachmentAnnotation = annot as AttachmentAnnotation;
				if (attachmentAnnotation != null)
				{
					return attachmentAnnotation.FileSpecification.FileName;
				}
			}
			return annot.Contents ?? "";
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00056888 File Offset: 0x00054A88
		internal CommetModel Clone()
		{
			CommetModel commetModel = new CommetModel
			{
				contents = this.contents,
				isSelected = this.isSelected,
				isChecked = this.isChecked,
				Annotation = this.Annotation,
				AnnotationMode = this.AnnotationMode,
				Title = this.Title,
				Text = this.Text,
				IsContentReadOnly = this.IsContentReadOnly
			};
			if (this.Replies != null)
			{
				List<CommetModel> list = new List<CommetModel>();
				for (int i = 0; i < this.Replies.Count; i++)
				{
					CommetModel commetModel2 = this.Replies[i].Clone();
					commetModel2.Parent = commetModel;
					list.Add(commetModel2);
				}
				commetModel.Replies = list;
			}
			return commetModel;
		}

		// Token: 0x0400078D RID: 1933
		private string contents;

		// Token: 0x0400078E RID: 1934
		private bool isSelected;

		// Token: 0x0400078F RID: 1935
		private bool isChecked;

		// Token: 0x04000795 RID: 1941
		private AsyncRelayCommand deleteSelectedAnnotCmd;
	}
}
