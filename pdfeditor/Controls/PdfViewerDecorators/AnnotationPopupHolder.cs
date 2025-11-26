using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.Annotations;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000235 RID: 565
	public class AnnotationPopupHolder
	{
		// Token: 0x06002013 RID: 8211 RVA: 0x00090E58 File Offset: 0x0008F058
		public AnnotationPopupHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x06002014 RID: 8212 RVA: 0x00090E81 File Offset: 0x0008F081
		public AnnotationCanvas AnnotationCanvas
		{
			get
			{
				return this.annotationCanvas;
			}
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x00090E8C File Offset: 0x0008F08C
		public void ClearAnnotationPopup()
		{
			foreach (KeyValuePair<int, PopupAnnotationCollection> keyValuePair in this.annotPanels)
			{
				keyValuePair.Value.Loaded -= this.PopupCollection_Loaded;
				this.annotationCanvas.Children.Remove(keyValuePair.Value);
			}
			this.annotPanels.Clear();
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x00090F0C File Offset: 0x0008F10C
		public void InitAnnotationPopup(PdfPage page)
		{
			if (this.annotationCanvas.IsAnnotationVisible && ((page != null) ? page.PageIndex : (-1)) >= 0 && !this.annotPanels.ContainsKey(page.PageIndex))
			{
				PopupAnnotationCollection popupAnnotationCollection = new PopupAnnotationCollection(this.annotationCanvas, page);
				this.annotPanels[page.PageIndex] = popupAnnotationCollection;
				Panel.SetZIndex(popupAnnotationCollection, 2);
				this.annotationCanvas.Children.Add(popupAnnotationCollection);
				popupAnnotationCollection.UpdatePosition();
				PdfAnnotation selectedAnnotation = this.annotationCanvas.HolderManager.SelectedAnnotation;
				if (selectedAnnotation != null && selectedAnnotation.Page == page)
				{
					this.SetPopupSelected(selectedAnnotation, true);
				}
				popupAnnotationCollection.Loaded += this.PopupCollection_Loaded;
			}
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x00090FCC File Offset: 0x0008F1CC
		public void FlushAnnotationPopup()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			bool flag;
			if (pdfViewer == null)
			{
				flag = null != null;
			}
			else
			{
				PdfDocument document = pdfViewer.Document;
				if (document == null)
				{
					flag = null != null;
				}
				else
				{
					PdfPageCollection pages = document.Pages;
					flag = ((pages != null) ? pages.CurrentPage : null) != null;
				}
			}
			if (flag)
			{
				this.ClearAnnotationPopup();
				PdfViewer pdfViewer2 = this.annotationCanvas.PdfViewer;
				PdfPage pdfPage;
				if (pdfViewer2 == null)
				{
					pdfPage = null;
				}
				else
				{
					PdfDocument document2 = pdfViewer2.Document;
					if (document2 == null)
					{
						pdfPage = null;
					}
					else
					{
						PdfPageCollection pages2 = document2.Pages;
						pdfPage = ((pages2 != null) ? pages2.CurrentPage : null);
					}
				}
				this.InitAnnotationPopup(pdfPage);
			}
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x00091045 File Offset: 0x0008F245
		private void PopupCollection_Loaded(object sender, RoutedEventArgs e)
		{
			this.annotationCanvas.UpdateViewerFlyoutExtendWidth();
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x00091054 File Offset: 0x0008F254
		public void SetPopupHovered(PdfAnnotation annot, bool value)
		{
			int? num;
			if (annot == null)
			{
				num = null;
			}
			else
			{
				PdfPage page = annot.Page;
				num = ((page != null) ? new int?(page.PageIndex) : null);
			}
			int? num2 = num;
			int valueOrDefault = num2.GetValueOrDefault(-1);
			PopupAnnotationCollection popupAnnotationCollection;
			if (valueOrDefault >= 0 && this.annotPanels.TryGetValue(valueOrDefault, out popupAnnotationCollection))
			{
				popupAnnotationCollection.SetHovered(annot, value);
			}
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x000910B4 File Offset: 0x0008F2B4
		public void SetPopupSelected(PdfAnnotation annot, bool value)
		{
			int? num;
			if (annot == null)
			{
				num = null;
			}
			else
			{
				PdfPage page = annot.Page;
				num = ((page != null) ? new int?(page.PageIndex) : null);
			}
			int? num2 = num;
			int valueOrDefault = num2.GetValueOrDefault(-1);
			PopupAnnotationCollection popupAnnotationCollection;
			if (valueOrDefault >= 0 && this.annotPanels.TryGetValue(valueOrDefault, out popupAnnotationCollection))
			{
				popupAnnotationCollection.SetSelected(annot, value);
			}
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x00091114 File Offset: 0x0008F314
		public void UpdatePanelsPosition()
		{
			foreach (PopupAnnotationCollection popupAnnotationCollection in this.annotPanels.Values)
			{
				popupAnnotationCollection.UpdatePosition();
			}
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x00091164 File Offset: 0x0008F364
		public void KillFocus()
		{
			foreach (PopupAnnotationCollection popupAnnotationCollection in this.annotPanels.Values)
			{
				popupAnnotationCollection.KillFocus();
			}
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x000911B4 File Offset: 0x0008F3B4
		public double GetMaxPopupWidth()
		{
			return (double)this.annotPanels.Values.SelectMany((PopupAnnotationCollection c) => from x in c.Children.OfType<AnnotationPopupControl>()
				select x.Wrapper.Annotation.GetRECT().Width).DefaultIfEmpty<float>().Max();
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x000911F0 File Offset: 0x0008F3F0
		public bool IsPopupVisible(PdfAnnotation annot)
		{
			PopupAnnotationCollection popupAnnotationCollection;
			return this.annotPanels.TryGetValue(annot.Page.PageIndex, out popupAnnotationCollection) && popupAnnotationCollection.IsPopupVisible(annot);
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x00091220 File Offset: 0x0008F420
		public bool TryShowPopup(PdfAnnotation annot)
		{
			PopupAnnotationCollection popupAnnotationCollection;
			return this.annotPanels.TryGetValue(annot.Page.PageIndex, out popupAnnotationCollection) && !popupAnnotationCollection.IsPopupVisible(annot) && popupAnnotationCollection.ShowPopup(annot, true);
		}

		// Token: 0x06002020 RID: 8224 RVA: 0x0009125C File Offset: 0x0008F45C
		public void FocusPopupTextBox(PdfAnnotation annotation, bool afterCreate)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			PdfPage page = annotation.Page;
			PopupAnnotationCollection panel;
			if (this.annotPanels.TryGetValue(page.PageIndex, out panel))
			{
				Action action = delegate
				{
					panel.TryBringPopupControlIntoView(annotation);
					TextBox popupTextBox = panel.GetPopupTextBox(annotation);
					if (popupTextBox != null)
					{
						popupTextBox.Focus();
						if (afterCreate)
						{
							popupTextBox.SelectAll();
						}
					}
				};
				if (!panel.IsLoaded)
				{
					panel.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, action);
					return;
				}
				action();
			}
		}

		// Token: 0x04000CE3 RID: 3299
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CE4 RID: 3300
		private ConcurrentDictionary<int, PopupAnnotationCollection> annotPanels = new ConcurrentDictionary<int, PopupAnnotationCollection>();
	}
}
