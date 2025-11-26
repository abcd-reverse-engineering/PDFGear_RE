using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Services
{
	// Token: 0x02000122 RID: 290
	public class AnnotationTooltipService : IDisposable
	{
		// Token: 0x06000D1C RID: 3356 RVA: 0x00042588 File Offset: 0x00040788
		public AnnotationTooltipService(PdfViewer pdfViewer)
		{
			if (pdfViewer == null)
			{
				throw new ArgumentNullException("pdfViewer");
			}
			this.pdfViewer = pdfViewer;
			pdfViewer.Loaded += this.PdfViewer_Loaded;
			pdfViewer.Unloaded += this.PdfViewer_Unloaded;
			this.tooltip = new AnnotationToolTip();
			this.timer = new DispatcherTimer();
			this.timer.Interval = TimeSpan.FromSeconds(0.5);
			this.timer.Tick += this.Timer_Tick;
			this.Window = Window.GetWindow(pdfViewer);
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000D1D RID: 3357 RVA: 0x00042627 File Offset: 0x00040827
		// (set) Token: 0x06000D1E RID: 3358 RVA: 0x00042630 File Offset: 0x00040830
		private Window Window
		{
			get
			{
				return this.window;
			}
			set
			{
				if (this.window != value)
				{
					DispatcherTimer dispatcherTimer = this.timer;
					if (dispatcherTimer != null)
					{
						dispatcherTimer.Stop();
					}
					if (this.window != null)
					{
						this.window.PreviewMouseMove -= this.Window_PreviewMouseMove;
						this.window.GotMouseCapture -= this.Window_GotMouseCapture;
						this.window.LostMouseCapture -= this.Window_LostMouseCapture;
						this.window.Deactivated -= this.Window_Deactivated;
					}
					this.window = value;
					if (this.window != null)
					{
						this.window.PreviewMouseMove += this.Window_PreviewMouseMove;
						this.window.GotMouseCapture += this.Window_GotMouseCapture;
						this.window.LostMouseCapture += this.Window_LostMouseCapture;
						this.window.Deactivated += this.Window_Deactivated;
					}
					this.ResetTimer();
				}
			}
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0004272F File Offset: 0x0004092F
		private void PdfViewer_Loaded(object sender, RoutedEventArgs e)
		{
			this.Window = Window.GetWindow(this.pdfViewer);
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x00042742 File Offset: 0x00040942
		private void PdfViewer_Unloaded(object sender, RoutedEventArgs e)
		{
			this.Window = null;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0004274B File Offset: 0x0004094B
		private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			this.ResetTimer();
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x00042753 File Offset: 0x00040953
		private void Window_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this.ResetTimer();
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0004275B File Offset: 0x0004095B
		private void Window_GotMouseCapture(object sender, MouseEventArgs e)
		{
			this.ResetTimer();
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x00042763 File Offset: 0x00040963
		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.HideTooltip();
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0004276C File Offset: 0x0004096C
		private void ResetTimer()
		{
			this.HideTooltip();
			if (this.Window != null && Mouse.Captured == null && Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
			{
				this.timer.Start();
				return;
			}
			if (this.timer.IsEnabled)
			{
				this.timer.Stop();
			}
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x000427C0 File Offset: 0x000409C0
		private void Timer_Tick(object sender, EventArgs e)
		{
			this.timer.Stop();
			this.ShowTooltip();
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x000427D4 File Offset: 0x000409D4
		private void ShowTooltip()
		{
			if (this.pdfViewer == null)
			{
				return;
			}
			IInputElement directlyOver = Mouse.DirectlyOver;
			if (directlyOver is PdfViewer || directlyOver is AnnotationCanvas || directlyOver is AnnotationFocusControl || AnnotationTooltipService.IsAnnotationControl(directlyOver))
			{
				Point position = Mouse.GetPosition(this.pdfViewer);
				int num;
				PdfAnnotation pointAnnotation = this.pdfViewer.GetPointAnnotation(position, out num);
				string text;
				string text2;
				if (pointAnnotation != null && AnnotationTooltipService.TryBuildTooltipContent(pointAnnotation, this.pdfViewer, out text, out text2))
				{
					Rect deviceBounds = pointAnnotation.GetDeviceBounds();
					this.tooltip.PlacementTarget = this.window;
					this.tooltip.PlacementRectangle = deviceBounds;
					this.tooltip.Header = text;
					this.tooltip.Content = text2;
					this.tooltip.IsOpen = true;
					this.tooltip.VerticalOffset = 2.0;
				}
			}
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x000428AC File Offset: 0x00040AAC
		private static bool IsAnnotationControl(IInputElement element)
		{
			for (FrameworkElement frameworkElement = element as FrameworkElement; frameworkElement != null; frameworkElement = (frameworkElement.Parent as FrameworkElement) ?? (VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement))
			{
				if (frameworkElement is IAnnotationControl)
				{
					return true;
				}
				if (frameworkElement is AnnotationCanvas)
				{
					return false;
				}
				if (frameworkElement is RichTextBox)
				{
					return false;
				}
				if (frameworkElement is TextBox)
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00042909 File Offset: 0x00040B09
		public void HideTooltip()
		{
			this.tooltip.IsOpen = false;
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00042918 File Offset: 0x00040B18
		private static bool TryBuildTooltipContent(PdfAnnotation annot, PdfViewer pdfViewer, out string header, out string content)
		{
			header = null;
			content = null;
			if (annot == null)
			{
				return false;
			}
			bool flag = false;
			MainViewModel mainViewModel = pdfViewer.DataContext as MainViewModel;
			PdfMarkupAnnotation pdfMarkupAnnotation = annot as PdfMarkupAnnotation;
			if (pdfMarkupAnnotation != null)
			{
				PdfPopupAnnotation popup = pdfMarkupAnnotation.Popup;
				if (popup != null)
				{
					flag = !popup.IsOpen;
				}
				else if (annot is PdfTextAnnotation)
				{
					flag = true;
				}
				else
				{
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = annot as PdfFileAttachmentAnnotation;
					if (pdfFileAttachmentAnnotation != null)
					{
						try
						{
							PdfFileSpecification fileSpecification = pdfFileAttachmentAnnotation.FileSpecification;
							string text = ((fileSpecification != null) ? fileSpecification.FileName : null);
							header = Resources.AnnotationFileAttachment;
							content = text;
							return true;
						}
						catch
						{
						}
						return false;
					}
				}
				if (flag && !string.IsNullOrEmpty(pdfMarkupAnnotation.Contents))
				{
					if (!string.IsNullOrEmpty(pdfMarkupAnnotation.Text))
					{
						string text2 = pdfMarkupAnnotation.Text;
						header = ((text2 != null) ? text2.Trim() : null) ?? "";
						string contents = pdfMarkupAnnotation.Contents;
						content = ((contents != null) ? contents.Trim() : null) ?? "";
					}
					else
					{
						header = "";
						string contents2 = annot.Contents;
						content = ((contents2 != null) ? contents2.Trim() : null) ?? "";
					}
					return true;
				}
			}
			else
			{
				PdfLinkAnnotation pdfLinkAnnotation = annot as PdfLinkAnnotation;
				if (pdfLinkAnnotation != null && !mainViewModel.AnnotationToolbar.LinkButtonModel.IsChecked)
				{
					PdfAction action = pdfLinkAnnotation.Link.Action;
					if (action == null || action.ActionType != ActionTypes.Uri)
					{
						PdfAction action2 = pdfLinkAnnotation.Link.Action;
						if (action2 == null || action2.ActionType != ActionTypes.Application)
						{
							return false;
						}
					}
					string linkUrlOrFileName = LinkAnnotationUtils.GetLinkUrlOrFileName(pdfLinkAnnotation.Link);
					if (linkUrlOrFileName != null)
					{
						content = linkUrlOrFileName;
						return true;
					}
				}
				else
				{
					PdfWidgetAnnotation pdfWidgetAnnotation = annot as PdfWidgetAnnotation;
					if (pdfWidgetAnnotation != null)
					{
						PdfDocument document = pdfViewer.Document;
						bool flag2 = ((document != null) ? document.FormFill : null) != null;
						DigitalSignatureHelper digitalSignatureHelper = mainViewModel.DocumentWrapper.DigitalSignatureHelper;
						PdfDigitalSignatureLocation pdfDigitalSignatureLocation = ((digitalSignatureHelper != null) ? digitalSignatureHelper.GetLocation(pdfWidgetAnnotation) : null);
						if (flag2 && ((pdfDigitalSignatureLocation != null) ? pdfDigitalSignatureLocation.SignatureField : null) != null && !pdfDigitalSignatureLocation.HasSigned)
						{
							header = pdfDigitalSignatureLocation.Name;
							content = pdfDigitalSignatureLocation.SignatureField.AlternateName;
							if (string.IsNullOrEmpty(content))
							{
								content = Resources.ResourceManager.GetString("DigSignPropUnsigned");
							}
							if (!string.IsNullOrEmpty(content))
							{
								content += " ";
							}
							content += Resources.ResourceManager.GetString("DigSignUnsignedToolTipPostfix");
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x00042B78 File Offset: 0x00040D78
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					this.Window = null;
					DispatcherTimer dispatcherTimer = this.timer;
					if (dispatcherTimer != null)
					{
						dispatcherTimer.Stop();
					}
					this.timer = null;
					if (this.tooltip != null)
					{
						this.tooltip.IsOpen = false;
						this.tooltip = null;
					}
					if (this.pdfViewer != null)
					{
						this.pdfViewer.Loaded -= this.PdfViewer_Loaded;
						this.pdfViewer.Unloaded -= this.PdfViewer_Unloaded;
						this.pdfViewer = null;
					}
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x00042C11 File Offset: 0x00040E11
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x040005A7 RID: 1447
		private PdfViewer pdfViewer;

		// Token: 0x040005A8 RID: 1448
		private Window window;

		// Token: 0x040005A9 RID: 1449
		private DispatcherTimer timer;

		// Token: 0x040005AA RID: 1450
		private AnnotationToolTip tooltip;

		// Token: 0x040005AB RID: 1451
		private bool disposedValue;
	}
}
