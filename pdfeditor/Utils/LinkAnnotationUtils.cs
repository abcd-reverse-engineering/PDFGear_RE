using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Actions;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Services;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x02000081 RID: 129
	internal static class LinkAnnotationUtils
	{
		// Token: 0x060008EB RID: 2283 RVA: 0x0002C6AC File Offset: 0x0002A8AC
		public static void LinkAnnotationop(PdfLinkAnnotation pdfLinkAnnotation, PdfDocument pdfDocument, PdfPage pdfPage, float Doczoom, MainViewModel vm)
		{
			if (pdfLinkAnnotation == null)
			{
				return;
			}
			LinkAnnotationModel linkModel = LinkAnnotationUtils.GetLinkModel(pdfLinkAnnotation, pdfDocument);
			if (linkModel == null)
			{
				return;
			}
			LinkEditWindows linkEditWindows = new LinkEditWindows(linkModel);
			linkEditWindows.Owner = App.Current.MainWindow;
			linkEditWindows.WindowStartupLocation = ((linkEditWindows.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
			bool? flag = linkEditWindows.ShowDialog();
			if (pdfPage.Annots == null)
			{
				pdfPage.CreateAnnotations();
			}
			if (flag.GetValueOrDefault())
			{
				using (vm.OperationManager.TraceAnnotationChange(pdfLinkAnnotation.Page, ""))
				{
					if (linkEditWindows.SelectedType == LinkSelect.ToPage)
					{
						int num = linkEditWindows.Page - 1;
						PdfDestination pdfDestination = PdfDestination.CreateXYZ(pdfDocument, num, null, new float?(pdfDocument.Pages[num].Height), new float?(Doczoom));
						pdfLinkAnnotation.Link.Action = new PdfGoToAction(pdfDocument, pdfDestination);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToWeb)
					{
						pdfLinkAnnotation.Link.Action = new PdfUriAction(pdfDocument, linkEditWindows.UrlFilePath);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToFile)
					{
						PdfFileSpecification pdfFileSpecification = new PdfFileSpecification(pdfDocument);
						pdfFileSpecification.FileName = linkEditWindows.FileDiaoligFiePath;
						pdfLinkAnnotation.Link.Action = new PdfLaunchAction(pdfDocument, pdfFileSpecification);
					}
					Color color = (Color)ColorConverter.ConvertFromString(linkEditWindows.SelectedFontground);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					float num2;
					if (!linkEditWindows.rectangleVis)
					{
						num2 = 0f;
					}
					else
					{
						pdfLinkAnnotation.Color = fs_COLOR;
						num2 = linkEditWindows.BorderWidth;
					}
					PdfBorderStyle pdfBorderStyle = new PdfBorderStyle
					{
						Width = num2,
						Style = linkEditWindows.BorderStyles
					};
					pdfLinkAnnotation.SetBorderStyle(pdfBorderStyle);
				}
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfPage.Document);
				if (pdfControl != null)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
					int? num3;
					if (annotationHolderManager == null)
					{
						num3 = null;
					}
					else
					{
						IAnnotationHolder currentHolder = annotationHolderManager.CurrentHolder;
						if (currentHolder == null)
						{
							num3 = null;
						}
						else
						{
							PdfPage currentPage = currentHolder.CurrentPage;
							num3 = ((currentPage != null) ? new int?(currentPage.PageIndex) : null);
						}
					}
					int? num4 = num3;
					int pageIndex = pdfPage.PageIndex;
					bool flag2 = (num4.GetValueOrDefault() == pageIndex) & (num4 != null);
				}
				for (int i = 0; i < 3; i++)
				{
					bool flag3 = pdfPage.IsDisposed;
					if (!flag3)
					{
						flag3 = PdfDocumentStateService.CanDisposePage(pdfPage);
					}
					ProgressiveStatus progressiveStatus;
					if (!flag3 && PdfObjectExtensions.TryGetProgressiveStatus(pdfPage, out progressiveStatus))
					{
						flag3 = progressiveStatus != ProgressiveStatus.ToBeContinued && progressiveStatus != ProgressiveStatus.Failed;
					}
					if (flag3)
					{
						try
						{
							PageDisposeHelper.DisposePage(pdfPage);
							PdfDocumentStateService.TryRedrawViewerCurrentPage(pdfPage);
						}
						catch
						{
						}
						return;
					}
				}
			}
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0002C970 File Offset: 0x0002AB70
		internal static string GetLinkUrlOrFileName(PdfLink pdfLink)
		{
			PdfUriAction pdfUriAction = pdfLink.Action as PdfUriAction;
			if (pdfUriAction != null && !string.IsNullOrEmpty(pdfUriAction.Uri))
			{
				return pdfUriAction.Uri;
			}
			PdfLaunchAction pdfLaunchAction = pdfLink.Action as PdfLaunchAction;
			if (pdfLaunchAction != null)
			{
				PdfFileSpecification fileSpecification = pdfLaunchAction.FileSpecification;
				if (!string.IsNullOrEmpty((fileSpecification != null) ? fileSpecification.FileName : null))
				{
					return pdfLaunchAction.FileSpecification.FileName;
				}
			}
			return null;
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x0002C9D8 File Offset: 0x0002ABD8
		internal static LinkAnnotationModel GetLinkModel(PdfLinkAnnotation pdfLink, PdfDocument pdfDocument)
		{
			LinkAnnotationModel linkAnnotationModel = new LinkAnnotationModel();
			linkAnnotationModel.Title = Resources.LinkWinTitleEdit;
			linkAnnotationModel.PdfDocument = pdfDocument;
			linkAnnotationModel.BorderColor = pdfLink.Color.ToColor();
			PdfGoToAction pdfGoToAction = pdfLink.Link.Action as PdfGoToAction;
			if (pdfGoToAction != null && pdfGoToAction.Destination != null)
			{
				linkAnnotationModel.Action = LinkSelect.ToPage;
				linkAnnotationModel.Page = pdfGoToAction.Destination.PageIndex + 1;
			}
			else if (pdfLink.Link.Destination != null)
			{
				linkAnnotationModel.Action = LinkSelect.ToPage;
				linkAnnotationModel.Page = pdfLink.Link.Destination.PageIndex + 1;
			}
			else
			{
				PdfUriAction pdfUriAction = pdfLink.Link.Action as PdfUriAction;
				if (pdfUriAction != null)
				{
					linkAnnotationModel.Uri = pdfUriAction.Uri;
					linkAnnotationModel.Action = LinkSelect.ToWeb;
				}
				else
				{
					PdfLaunchAction pdfLaunchAction = pdfLink.Link.Action as PdfLaunchAction;
					if (pdfLaunchAction != null)
					{
						linkAnnotationModel.Action = LinkSelect.ToFile;
						linkAnnotationModel.FileName = pdfLaunchAction.FileSpecification.FileName;
					}
				}
			}
			PdfBorderStyle pdfBorderStyle = new PdfBorderStyle();
			if (pdfLink.Dictionary.ContainsKey("BS"))
			{
				pdfBorderStyle = pdfLink.GetBorderStyle();
				linkAnnotationModel.Width = pdfBorderStyle.Width;
				linkAnnotationModel.BorderStyle = pdfBorderStyle.Style;
			}
			else
			{
				linkAnnotationModel.Width = 1f;
				linkAnnotationModel.BorderStyle = BorderStyles.Solid;
			}
			return linkAnnotationModel;
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0002CB1C File Offset: 0x0002AD1C
		private static bool CheckUri(string uri)
		{
			Regex regex = new Regex("^(http(s)?:\\/\\/)?(www\\.)?[\\w-]+(\\.\\w{2,4})?\\.\\w{2,4}?(\\/)?$");
			string text = uri.Trim();
			return regex.Match(text).Success;
		}
	}
}
