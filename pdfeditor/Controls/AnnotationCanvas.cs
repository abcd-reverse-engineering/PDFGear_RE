using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.CSharp.RuntimeBinder;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Actions;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.EventArguments;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.PageContents;
using pdfeditor.Controls.PdfViewerDecorators;
using pdfeditor.Controls.Screenshots;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Models.Operations;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Events;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Controls
{
	// Token: 0x020001B0 RID: 432
	public class AnnotationCanvas : Canvas
	{
		// Token: 0x06001867 RID: 6247 RVA: 0x0005D0EC File Offset: 0x0005B2EC
		public AnnotationCanvas()
		{
			base.HorizontalAlignment = HorizontalAlignment.Stretch;
			base.VerticalAlignment = VerticalAlignment.Stretch;
			base.ClipToBounds = true;
			this.doubleClickHelper = new DoubleClickHelper(this);
			this.doubleClickHelper.MouseDoubleClick += this.DoubleClickHelper_MouseDoubleClick;
			this.hitTestElement = new Rectangle
			{
				IsHitTestVisible = false,
				Fill = Brushes.Transparent,
				UseLayoutRounding = false
			};
			this.ImageControl = new ImageControl();
			this.ImageControl.Visibility = Visibility.Collapsed;
			base.Children.Add(this.ImageControl);
			base.InternalChildren.Add(this.hitTestElement);
			Panel.SetZIndex(this.hitTestElement, -1);
			this.holders = new AnnotationHolderManager(this);
			this.holders.SelectedAnnotationChanged += this.Holders_SelectedAnnotationChanged;
			this.holders.CurrentHolderChanged += this.Holders_CurrentHolderChanged;
			this.focusControl = new AnnotationFocusControl(this);
			base.InternalChildren.Add(this.focusControl);
			Panel.SetZIndex(this.focusControl, 1);
			OperationManager.BeforeOperationInvoked += this.OperationManager_BeforeOperationInvoked;
			OperationManager.AfterOperationInvoked += this.OperationManager_AfterOperationInvoked;
			SystemParameters.StaticPropertyChanged += this.SystemParameters_StaticPropertyChanged;
			this.UpdateViewerFlyoutExtendWidth();
			this.annotationContextMenuHolder = new AnnotationContextMenuHolder(this);
			this.selectTextContextMenuHolder = new SelectTextContextMenuHolder(this)
			{
				ShowRecentColorInContextMenu = false
			};
			this.textObjectContextMenuHolder = new TextObjectContextMenuHolder(this);
			this.pageDefaultContextMenuHolder = new PageDefaultContextMenuHolder(this);
			this.digitalSignatureContextMenuHolder = new DigitalSignatureContextMenuHolder(this);
			this.screenshotDialog = new ScreenshotDialog();
			this.screenshotDialog.Visibility = Visibility.Collapsed;
			this.screenshotDialog.IsVisibleChanged += this.ScreenshotDialog_IsVisibleChanged;
			base.InternalChildren.Add(this.screenshotDialog);
			Panel.SetZIndex(this.screenshotDialog, 2);
			base.SizeChanged += this.AnnotationCanvas_SizeChanged;
			this.textObjRect = new Rectangle
			{
				Stroke = new SolidColorBrush(Colors.Black),
				StrokeThickness = 1.0
			};
			base.Children.Add(this.textObjRect);
			this.popupHolder = new AnnotationPopupHolder(this);
			this.textObjectHolder = new TextObjectHolder(this);
			this.pdfEraserUtil = new PDFEraserUtil();
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06001868 RID: 6248 RVA: 0x0005D346 File Offset: 0x0005B546
		protected PdfDocument Document
		{
			get
			{
				PdfViewer pdfViewer = this.PdfViewer;
				if (pdfViewer == null)
				{
					return null;
				}
				return pdfViewer.Document;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06001869 RID: 6249 RVA: 0x0005D359 File Offset: 0x0005B559
		public AnnotationHolderManager HolderManager
		{
			get
			{
				return this.holders;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x0600186A RID: 6250 RVA: 0x0005D361 File Offset: 0x0005B561
		public AnnotationPopupHolder PopupHolder
		{
			get
			{
				return this.popupHolder;
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x0600186B RID: 6251 RVA: 0x0005D369 File Offset: 0x0005B569
		public TextObjectHolder TextObjectHolder
		{
			get
			{
				return this.textObjectHolder;
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x0600186C RID: 6252 RVA: 0x0005D371 File Offset: 0x0005B571
		public PdfViewerAutoScrollHelper AutoScrollHelper
		{
			get
			{
				return this.pdfViewerAutoScrollHelper;
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x0600186D RID: 6253 RVA: 0x0005D379 File Offset: 0x0005B579
		internal TextObjectContextMenuHolder TextObjectContextMenuHolder
		{
			get
			{
				return this.textObjectContextMenuHolder;
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0005D381 File Offset: 0x0005B581
		public ScreenshotDialog ScreenshotDialog
		{
			get
			{
				return this.screenshotDialog;
			}
		}

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x0600186F RID: 6255 RVA: 0x0005D389 File Offset: 0x0005B589
		private MainViewModel VM
		{
			get
			{
				return base.DataContext as MainViewModel;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06001870 RID: 6256 RVA: 0x0005D396 File Offset: 0x0005B596
		// (set) Token: 0x06001871 RID: 6257 RVA: 0x0005D3A8 File Offset: 0x0005B5A8
		public PdfViewer PdfViewer
		{
			get
			{
				return (PdfViewer)base.GetValue(AnnotationCanvas.PdfViewerProperty);
			}
			set
			{
				base.SetValue(AnnotationCanvas.PdfViewerProperty, value);
			}
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0005D3B8 File Offset: 0x0005B5B8
		private static void OnPdfViewerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				AnnotationCanvas annotationCanvas = d as AnnotationCanvas;
				if (annotationCanvas != null)
				{
					AnnotationTooltipService annotationTooltipService = annotationCanvas.tooltipService;
					if (annotationTooltipService != null)
					{
						annotationTooltipService.Dispose();
					}
					annotationCanvas.tooltipService = null;
					annotationCanvas.popupHolder.ClearAnnotationPopup();
					annotationCanvas.focusControl.Annotation = null;
					PdfViewer pdfViewer = e.OldValue as PdfViewer;
					if (pdfViewer != null)
					{
						annotationCanvas.RemoveViewerEventHandler(pdfViewer);
					}
					PdfViewer pdfViewer2 = e.NewValue as PdfViewer;
					if (pdfViewer2 != null)
					{
						annotationCanvas.AddViewerEventHandler(pdfViewer2);
						AnnotationPopupHolder annotationPopupHolder = annotationCanvas.popupHolder;
						PdfDocument document = pdfViewer2.Document;
						PdfPage pdfPage;
						if (document == null)
						{
							pdfPage = null;
						}
						else
						{
							PdfPageCollection pages = document.Pages;
							pdfPage = ((pages != null) ? pages.CurrentPage : null);
						}
						annotationPopupHolder.InitAnnotationPopup(pdfPage);
						annotationCanvas.tooltipService = new AnnotationTooltipService(pdfViewer2);
					}
					annotationCanvas.UpdateAutoScrollHelper();
				}
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06001873 RID: 6259 RVA: 0x0005D47D File Offset: 0x0005B67D
		// (set) Token: 0x06001874 RID: 6260 RVA: 0x0005D48F File Offset: 0x0005B68F
		public PdfAnnotation SelectedAnnotation
		{
			get
			{
				return (PdfAnnotation)base.GetValue(AnnotationCanvas.SelectedAnnotationProperty);
			}
			set
			{
				base.SetValue(AnnotationCanvas.SelectedAnnotationProperty, value);
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0005D4A0 File Offset: 0x0005B6A0
		private static void OnSelectedAnnotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				AnnotationCanvas annotationCanvas = d as AnnotationCanvas;
				if (annotationCanvas != null)
				{
					annotationCanvas.holders.Select(e.NewValue as PdfAnnotation, false);
					if (e.NewValue is PdfAnnotation)
					{
						annotationCanvas.focusControl.Annotation = null;
					}
					annotationCanvas.UpdateHoverAnnotationBorder();
					EventHandler selectedAnnotationChanged = annotationCanvas.SelectedAnnotationChanged;
					if (selectedAnnotationChanged != null)
					{
						selectedAnnotationChanged(annotationCanvas, EventArgs.Empty);
					}
					annotationCanvas.popupHolder.SetPopupSelected((PdfAnnotation)e.OldValue, false);
					annotationCanvas.popupHolder.SetPopupSelected((PdfAnnotation)e.NewValue, true);
					annotationCanvas.UpdateViewerFlyoutExtendWidth();
				}
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x0005D552 File Offset: 0x0005B752
		// (set) Token: 0x06001877 RID: 6263 RVA: 0x0005D564 File Offset: 0x0005B764
		public bool IsAnnotationVisible
		{
			get
			{
				return (bool)base.GetValue(AnnotationCanvas.IsAnnotationVisibleProperty);
			}
			set
			{
				base.SetValue(AnnotationCanvas.IsAnnotationVisibleProperty, value);
			}
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x0005D578 File Offset: 0x0005B778
		private static void OnIsAnnotationVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				AnnotationCanvas annotationCanvas = d as AnnotationCanvas;
				if (annotationCanvas != null)
				{
					object newValue = e.NewValue;
					if (newValue is bool && (bool)newValue)
					{
						PdfViewer pdfViewer = annotationCanvas.PdfViewer;
						PdfPage pdfPage;
						if (pdfViewer == null)
						{
							pdfPage = null;
						}
						else
						{
							PdfDocument document = pdfViewer.Document;
							if (document == null)
							{
								pdfPage = null;
							}
							else
							{
								PdfPageCollection pages = document.Pages;
								pdfPage = ((pages != null) ? pages.CurrentPage : null);
							}
						}
						PdfPage pdfPage2 = pdfPage;
						if (pdfPage2 != null)
						{
							annotationCanvas.popupHolder.InitAnnotationPopup(pdfPage2);
							return;
						}
					}
					else
					{
						annotationCanvas.popupHolder.ClearAnnotationPopup();
						annotationCanvas.holders.CancelAll();
						annotationCanvas.UpdateHoverAnnotationBorder();
					}
				}
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06001879 RID: 6265 RVA: 0x0005D613 File Offset: 0x0005B813
		// (set) Token: 0x0600187A RID: 6266 RVA: 0x0005D625 File Offset: 0x0005B825
		public PageObjectType EditingPageObjectType
		{
			get
			{
				return (PageObjectType)base.GetValue(AnnotationCanvas.EditingPageObjectTypeProperty);
			}
			set
			{
				base.SetValue(AnnotationCanvas.EditingPageObjectTypeProperty, value);
			}
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x0005D638 File Offset: 0x0005B838
		private static void OnEditingPageObjectTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((PageObjectType)e.NewValue != (PageObjectType)e.OldValue)
			{
				AnnotationCanvas annotationCanvas = d as AnnotationCanvas;
				if (annotationCanvas != null)
				{
					annotationCanvas.textObjectHolder.CancelTextObject();
					annotationCanvas.editingPageObjectTypes = null;
					switch ((PageObjectType)e.NewValue)
					{
					case PageObjectType.Text:
						annotationCanvas.editingPageObjectTypes = new PageObjectTypes[] { PageObjectTypes.PDFPAGE_TEXT };
						break;
					case PageObjectType.Path:
						annotationCanvas.editingPageObjectTypes = new PageObjectTypes[] { PageObjectTypes.PDFPAGE_PATH };
						break;
					case PageObjectType.Image:
						annotationCanvas.editingPageObjectTypes = new PageObjectTypes[] { PageObjectTypes.PDFPAGE_IMAGE };
						break;
					case PageObjectType.Form:
						annotationCanvas.editingPageObjectTypes = new PageObjectTypes[] { PageObjectTypes.PDFPAGE_FORM };
						break;
					}
					annotationCanvas.UpdateHoverAnnotationBorder();
				}
			}
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x0600187C RID: 6268 RVA: 0x0005D6F9 File Offset: 0x0005B8F9
		// (set) Token: 0x0600187D RID: 6269 RVA: 0x0005D70B File Offset: 0x0005B90B
		public int AutoScrollSpeed
		{
			get
			{
				return (int)base.GetValue(AnnotationCanvas.AutoScrollSpeedProperty);
			}
			set
			{
				base.SetValue(AnnotationCanvas.AutoScrollSpeedProperty, value);
			}
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x0005D720 File Offset: 0x0005B920
		private static void OnAutoScrollSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AnnotationCanvas annotationCanvas = d as AnnotationCanvas;
			if (annotationCanvas != null)
			{
				object newValue = e.NewValue;
				if (newValue is int)
				{
					int num = (int)newValue;
					if (annotationCanvas.pdfViewerAutoScrollHelper != null)
					{
						annotationCanvas.pdfViewerAutoScrollHelper.Speed = (double)num;
					}
				}
			}
		}

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x0600187F RID: 6271 RVA: 0x0005D764 File Offset: 0x0005B964
		// (remove) Token: 0x06001880 RID: 6272 RVA: 0x0005D79C File Offset: 0x0005B99C
		public event EventHandler SelectedAnnotationChanged;

		// Token: 0x06001881 RID: 6273 RVA: 0x0005D7D1 File Offset: 0x0005B9D1
		private void Holders_SelectedAnnotationChanged(object sender, EventArgs e)
		{
			this.SelectedAnnotation = this.holders.SelectedAnnotation;
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x0005D7E4 File Offset: 0x0005B9E4
		private void Holders_CurrentHolderChanged(object sender, EventArgs e)
		{
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x0005D7E6 File Offset: 0x0005B9E6
		private void OperationManager_BeforeOperationInvoked(object sender, EventArgs e)
		{
			this.holders.CancelAll();
			this.focusControl.Annotation = null;
			this.textObjectHolder.CancelTextObject();
			this.UpdateHoverAnnotationBorder();
			this.popupHolder.ClearAnnotationPopup();
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x0005D81C File Offset: 0x0005BA1C
		private void OperationManager_AfterOperationInvoked(object sender, EventArgs e)
		{
			PdfPage currentPage = this.PdfViewer.CurrentPage;
			this.popupHolder.InitAnnotationPopup(currentPage);
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0005D844 File Offset: 0x0005BA44
		private void AddViewerEventHandler(PdfViewer viewer)
		{
			if (viewer == null)
			{
				return;
			}
			viewer.MouseDown += this.Viewer_MouseDown;
			viewer.PreviewMouseDown += this.Viewer_PreviewMouseDown;
			viewer.PreviewMouseMove += this.Viewer_PreviewMouseMove;
			viewer.PreviewMouseUp += this.Viewer_PreviewMouseUp;
			viewer.MouseUp += this.Viewer_MouseUp;
			viewer.AnnotationMouseMoved += this.Viewer_AnnotationMouseMoved;
			viewer.AnnotationMouseEntered += this.Viewer_AnnotationMouseEntered;
			viewer.AnnotationMouseExited += this.Viewer_AnnotationMouseExited;
			viewer.AnnotationMouseClick += this.Viewer_AnnotationMouseClick;
			viewer.SizeChanged += this.Viewer_SizeChanged;
			viewer.ScrollOwnerChanged += this.Viewer_ScrollOwnerChanged;
			viewer.BeforeDocumentChanged += this.Viewer_BeforeDocumentChanged;
			viewer.AfterDocumentChanged += this.Viewer_AfterDocumentChanged;
			viewer.CurrentPageChanged += this.Viewer_CurrentPageChanged;
			viewer.MouseModeChanged += this.Viewer_MouseModeChanged;
			viewer.PreviewMouseWheel += this.Viewer_PreviewMouseWheel;
			viewer.BeforeLinkClicked += this.Viewer_BeforeLinkClicked;
			viewer.LostMouseCapture += this.Viewer_LostMouseCapture;
			this.scrollViewer = viewer.ScrollOwner;
			if (this.scrollViewer != null)
			{
				MouseMiddleButtonScrollExtensions.SetIsEnabled(this.scrollViewer, true);
				MouseMiddleButtonScrollExtensions.SetShowCursorAtStartPoint(this.scrollViewer, true);
				this.scrollViewer.ScrollChanged += this.ScrollOwner_ScrollChanged;
			}
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x0005D9DC File Offset: 0x0005BBDC
		private void RemoveViewerEventHandler(PdfViewer viewer)
		{
			if (viewer == null)
			{
				return;
			}
			viewer.MouseDown -= this.Viewer_MouseDown;
			viewer.PreviewMouseDown -= this.Viewer_PreviewMouseDown;
			viewer.PreviewMouseMove -= this.Viewer_PreviewMouseMove;
			viewer.PreviewMouseUp -= this.Viewer_PreviewMouseUp;
			viewer.MouseUp -= this.Viewer_MouseUp;
			viewer.LostMouseCapture -= this.Viewer_LostMouseCapture;
			viewer.AnnotationMouseMoved -= this.Viewer_AnnotationMouseMoved;
			viewer.AnnotationMouseEntered -= this.Viewer_AnnotationMouseEntered;
			viewer.AnnotationMouseExited -= this.Viewer_AnnotationMouseExited;
			viewer.AnnotationMouseClick -= this.Viewer_AnnotationMouseClick;
			viewer.SizeChanged -= this.Viewer_SizeChanged;
			viewer.ScrollOwnerChanged -= this.Viewer_ScrollOwnerChanged;
			viewer.BeforeDocumentChanged -= this.Viewer_BeforeDocumentChanged;
			viewer.AfterDocumentChanged -= this.Viewer_AfterDocumentChanged;
			viewer.CurrentPageChanged -= this.Viewer_CurrentPageChanged;
			viewer.MouseModeChanged -= this.Viewer_MouseModeChanged;
			viewer.PreviewMouseWheel -= this.Viewer_PreviewMouseWheel;
			viewer.BeforeLinkClicked -= this.Viewer_BeforeLinkClicked;
			if (this.scrollViewer != null)
			{
				MouseMiddleButtonScrollExtensions.SetIsEnabled(this.scrollViewer, false);
				MouseMiddleButtonScrollExtensions.SetShowCursorAtStartPoint(this.scrollViewer, false);
				this.scrollViewer.ScrollChanged -= this.ScrollOwner_ScrollChanged;
			}
			this.scrollViewer = null;
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x0005DB6F File Offset: 0x0005BD6F
		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			if (e.OriginalSource == this)
			{
				this.hitTestElement.IsHitTestVisible = true;
				base.OnGotMouseCapture(e);
			}
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0005DB8D File Offset: 0x0005BD8D
		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			if (e.OriginalSource == this)
			{
				this.hitTestElement.IsHitTestVisible = false;
				base.OnLostMouseCapture(e);
			}
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0005DBAB File Offset: 0x0005BDAB
		private void AnnotationCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateHoverAnnotationBorder();
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0005DBB4 File Offset: 0x0005BDB4
		private void UpdateHoverAnnotationBorder()
		{
			if (!this.IsAnnotationVisible)
			{
				this.focusControl.Annotation = null;
			}
			this.focusControl.InvalidateMeasure();
			this.holders.OnPageClientBoundsChanged();
			this.ImageControl.UpdateImageborder();
			this.popupHolder.UpdatePanelsPosition();
			this.textObjectHolder.OnPageClientBoundsChanged();
			this.screenshotDialog.Width = base.ActualWidth;
			this.screenshotDialog.Height = base.ActualHeight;
			this.UpdateHoverPageObjectRect(Rect.Empty);
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0005DC3C File Offset: 0x0005BE3C
		private void Viewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.viewerDragged = false;
			if (e.ChangedButton == MouseButton.Left && this.doubleClickHelper.ProcessMouseClick(e))
			{
				e.Handled = true;
				return;
			}
			ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = this.VM.AnnotationToolbar.InkButtonModel.ToolbarSettingModel.OfType<ToolbarSettingInkEraserModel>().FirstOrDefault<ToolbarSettingInkEraserModel>();
			if (toolbarSettingInkEraserModel != null && toolbarSettingInkEraserModel.IsChecked && this.VM.AnnotationToolbar.InkButtonModel.IsChecked && this.PdfViewer.CaptureMouse())
			{
				e.Handled = true;
				Point position = e.GetPosition(this);
				Point point;
				int num = this.PdfViewer.DeviceToPage(position.X, position.Y, out point);
				this.pdfEraserUtil.MouseDownRecord(num, this.VM, this.Document, toolbarSettingInkEraserModel, position, point);
				return;
			}
			if (this.holders.CurrentHolder != null && this.holders.CurrentHolder.State != AnnotationHolderState.Selected)
			{
				e.Handled = true;
				if (e.ChangedButton == MouseButton.Right)
				{
					this.holders.CancelAll();
				}
				return;
			}
			if (!this.holders.IsAnnotationDoubleClicked(e))
			{
				this.holders.CancelAll();
			}
			this.textObjectHolder.CancelTextObject();
			Point position2 = e.GetPosition(this);
			Point point2;
			int num2 = this.PdfViewer.DeviceToPage(position2.X, position2.Y, out point2);
			if (num2 == -1)
			{
				return;
			}
			this.viewerPreviewPressedPageIndex = num2;
			if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed && this.holders.CurrentHolder == null)
			{
				bool flag = false;
				if ((this.VM.AnnotationToolbar.LinkButtonModel.IsChecked || this.VM.AnnotationToolbar.TextBoxButtonModel.IsChecked || this.VM.AnnotationToolbar.TextButtonModel.IsChecked) && this.Document.Pages[num2].Annots != null)
				{
					foreach (PdfAnnotation pdfAnnotation in this.Document.Pages[num2].Annots)
					{
						if (pdfAnnotation is PdfLinkAnnotation && AnnotationHitTestHelper.HitTest(pdfAnnotation, position2))
						{
							flag = true;
							break;
						}
						if (pdfAnnotation is PdfFreeTextAnnotation && AnnotationHitTestHelper.HitTest(pdfAnnotation, position2))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					if (this.StartCreateNewAnnot(this.Document.Pages[num2], point2.ToPdfPoint()))
					{
						if (this.holders.CurrentHolder.IsTextMarkupAnnotation)
						{
							this.PdfViewer.CaptureMouse();
						}
						else
						{
							e.Handled = true;
							base.CaptureMouse();
						}
					}
					e.Handled = e.LeftButton != MouseButtonState.Pressed;
				}
			}
			if (AnnotationCanvas.<>o__80.<>p__2 == null)
			{
				AnnotationCanvas.<>o__80.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = AnnotationCanvas.<>o__80.<>p__2.Target;
			CallSite <>p__ = AnnotationCanvas.<>o__80.<>p__2;
			if (AnnotationCanvas.<>o__80.<>p__1 == null)
			{
				AnnotationCanvas.<>o__80.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, MouseModes, object> target2 = AnnotationCanvas.<>o__80.<>p__1.Target;
			CallSite <>p__2 = AnnotationCanvas.<>o__80.<>p__1;
			if (AnnotationCanvas.<>o__80.<>p__0 == null)
			{
				AnnotationCanvas.<>o__80.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			if (target(<>p__, target2(<>p__2, AnnotationCanvas.<>o__80.<>p__0.Target(AnnotationCanvas.<>o__80.<>p__0, this.VM.ViewerMouseMode), MouseModes.PanTool)))
			{
				this.VM.ViewToolbar.PauseAutoScroll(0);
			}
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0005E008 File Offset: 0x0005C208
		private void Viewer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.viewerPressedPoint = new Point?(e.GetPosition(this));
			Point position = e.GetPosition(this);
			Point point;
			int clientPoint = this.GetClientPoint(position.X, position.Y, out point);
			if (e.MiddleButton == MouseButtonState.Pressed && !this.ImageControl.ImageControlState && this.VM.AnnotationMode != AnnotationMode.Link)
			{
				PdfViewerAutoScrollHelper pdfViewerAutoScrollHelper = this.pdfViewerAutoScrollHelper;
				if (pdfViewerAutoScrollHelper != null)
				{
					pdfViewerAutoScrollHelper.StopAutoScroll();
				}
				this.VM.ExitTransientMode(false, false, false, false, false);
				this.VM.ReleaseViewerFocusAsync(false);
				if (this.scrollViewer != null)
				{
					MouseMiddleButtonScrollExtensions.TryEnterScrollMode(this.scrollViewer);
				}
			}
			if (clientPoint >= 0)
			{
				PdfPage pdfPage = this.Document.Pages[clientPoint];
				PageObjectTypes[] array = this.editingPageObjectTypes;
				if (array != null && array.Length != 0)
				{
					e.Handled = true;
					this.PdfViewer.DeselectText();
					PdfPageObject[] pointObjects = PageObjectHitTestHelper.GetPointObjects(pdfPage, point, array);
					if (pointObjects.Length != 0)
					{
						PdfTextObject pdfTextObject = pointObjects[0] as PdfTextObject;
						if (pdfTextObject != null)
						{
							this.textObjectHolder.SelectTextObject(pdfPage, pdfTextObject, true);
						}
					}
				}
				int num;
				if (this.ImageControl.Visibility == Visibility.Visible && PageImageUtils.ImageTestHitTest(this.Document.Pages[clientPoint], point, out num))
				{
					if (AnnotationCanvas.<>o__81.<>p__3 == null)
					{
						AnnotationCanvas.<>o__81.<>p__3 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, bool> target = AnnotationCanvas.<>o__81.<>p__3.Target;
					CallSite <>p__ = AnnotationCanvas.<>o__81.<>p__3;
					bool flag = num != this.ImageControl.imageindex;
					object obj;
					if (flag)
					{
						if (AnnotationCanvas.<>o__81.<>p__2 == null)
						{
							AnnotationCanvas.<>o__81.<>p__2 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, bool, object, object> target2 = AnnotationCanvas.<>o__81.<>p__2.Target;
						CallSite <>p__2 = AnnotationCanvas.<>o__81.<>p__2;
						bool flag2 = flag;
						if (AnnotationCanvas.<>o__81.<>p__1 == null)
						{
							AnnotationCanvas.<>o__81.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						Func<CallSite, object, MouseModes, object> target3 = AnnotationCanvas.<>o__81.<>p__1.Target;
						CallSite <>p__3 = AnnotationCanvas.<>o__81.<>p__1;
						if (AnnotationCanvas.<>o__81.<>p__0 == null)
						{
							AnnotationCanvas.<>o__81.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						obj = target2(<>p__2, flag2, target3(<>p__3, AnnotationCanvas.<>o__81.<>p__0.Target(AnnotationCanvas.<>o__81.<>p__0, this.VM.ViewerMouseMode), MouseModes.PanTool));
					}
					else
					{
						obj = flag;
					}
					if (target(<>p__, obj))
					{
						this.ImageControl.CreateImageborder(this, this.Document, clientPoint, num, this.PdfViewer, false);
						this.ImageControl.Visibility = Visibility.Visible;
					}
					this.ImageControl.clickStartPosition = position;
				}
			}
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0005E2DF File Offset: 0x0005C4DF
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			if (e.ChangedButton == MouseButton.Left && this.doubleClickHelper.ProcessMouseClick(e))
			{
				e.Handled = true;
			}
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0005E308 File Offset: 0x0005C508
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Pressed && this.holders.CurrentHolder != null && this.holders.CurrentHolder.State == AnnotationHolderState.CreatingNew)
			{
				base.ReleaseMouseCapture();
				this.holders.CurrentHolder.Cancel();
			}
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0005E364 File Offset: 0x0005C564
		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point position = e.GetPosition(this);
			Point point;
			int clientPoint = this.GetClientPoint(position.X, position.Y, out point);
			if (this.ProcessMouseMove(clientPoint, point.ToPdfPoint()))
			{
				e.Handled = true;
			}
			base.OnMouseMove(e);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0005E3B0 File Offset: 0x0005C5B0
		protected override async void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			try
			{
				if (this.isAllowClick())
				{
					if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
					{
						Point position = e.GetPosition(this);
						Point point;
						int clientPoint = this.GetClientPoint(position.X, position.Y, out point);
						TaskAwaiter<bool> taskAwaiter = this.ProcessMouseUpAsync(clientPoint, point.ToPdfPoint(), delegate
						{
							e.Handled = true;
							this.ReleaseMouseCapture();
						}).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (taskAwaiter.GetResult())
						{
							this.RouteClickEventToPdfViewer(e);
						}
						else
						{
							base.ReleaseMouseCapture();
						}
					}
					else if (e.ChangedButton == MouseButton.Right)
					{
						PdfLinkAnnotation pdfLinkAnnotation = this.SelectedAnnotation as PdfLinkAnnotation;
						if (pdfLinkAnnotation != null && this.VM.AnnotationToolbar.LinkButtonModel.IsChecked)
						{
							new LinkRightMenu(this, pdfLinkAnnotation, this.Document).Show();
						}
						if (this.holders.IsAnnotationDoubleClicked(e))
						{
							await this.annotationContextMenuHolder.ShowAsync();
						}
					}
				}
			}
			finally
			{
				this.viewerPreviewPressedPageIndex = -1;
				this.viewerDragged = false;
				this.viewerPressedPoint = null;
			}
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0005E3F0 File Offset: 0x0005C5F0
		private void Viewer_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(this);
			Point point;
			int num = this.GetClientPoint(position.X, position.Y, out point);
			if (this.VM.ViewerOperationModel != null)
			{
				return;
			}
			ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = this.VM.AnnotationToolbar.InkButtonModel.ToolbarSettingModel.OfType<ToolbarSettingInkEraserModel>().FirstOrDefault<ToolbarSettingInkEraserModel>();
			if (this.VM.AnnotationToolbar.LinkButtonModel.IsChecked)
			{
				base.Cursor = Cursors.Cross;
				global::PDFKit.PdfControl.GetPdfControl(this.Document).Viewer.OverrideCursor = Cursors.Cross;
				return;
			}
			if (toolbarSettingInkEraserModel != null && toolbarSettingInkEraserModel.IsChecked && this.VM.AnnotationToolbar.InkButtonModel.IsChecked)
			{
				try
				{
					this.pdfEraserUtil.MouseStyle(this.Document, this, toolbarSettingInkEraserModel, this.VM);
					if (num < 0)
					{
						num = this.VM.SelectedPageIndex;
					}
					if (this.Document.Pages[num].Annots != null && num >= 0 && e.LeftButton == MouseButtonState.Pressed)
					{
						bool flag = false;
						for (int i = 0; i < this.Document.Pages[num].Annots.Count; i++)
						{
							if (this.Document.Pages[num].Annots[i] is PdfInkAnnotation)
							{
								flag = this.pdfEraserUtil.DeleteInk(this.Document, num, position, toolbarSettingInkEraserModel) || flag;
							}
						}
						if (flag)
						{
							this.Document.Pages[num].TryRedrawPageAsync(default(CancellationToken));
						}
					}
					return;
				}
				catch
				{
					return;
				}
			}
			if (this.VM.AnnotationMode == AnnotationMode.Ink)
			{
				Cursor cursor = CursorHelper.CreateCursor(global::System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style\\\\Resources\\\\MousePen.png"), 0U, 32U);
				base.Cursor = cursor;
				global::PDFKit.PdfControl.GetPdfControl(this.Document).Viewer.OverrideCursor = cursor;
				return;
			}
			base.Cursor = null;
			if (this.Document != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				if (pdfControl != null && pdfControl.Viewer.OverrideCursor != Cursors.Hand)
				{
					pdfControl.Viewer.OverrideCursor = null;
				}
			}
			this.ProcessMouseMove(num, point.ToPdfPoint());
			if (num >= 0)
			{
				PdfPage pdfPage = this.Document.Pages[num];
				PageObjectTypes[] array = this.editingPageObjectTypes;
				int num2;
				if ((this.ImageControl.Visibility == Visibility.Visible && PageImageUtils.ImageTestHitTest(this.Document.Pages[num], point, out num2)) || this.ImageControl.IsMoved || this.ImageControl.mousePosition != ImageControl.MousePosition.None)
				{
					base.Cursor = Cursors.SizeAll;
					global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.Document);
					if (this.ImageControl.ImageControlState)
					{
						if (pdfControl2 != null && pdfControl2.Viewer.OverrideCursor != Cursors.Hand)
						{
							pdfControl2.Viewer.OverrideCursor = Cursors.SizeAll;
						}
						this.PdfViewer.DeselectText();
						if (e.LeftButton == MouseButtonState.Pressed && this.ImageControl.mousePosition != ImageControl.MousePosition.None)
						{
							this.ImageControl.ImageControlReSizeImage(position);
							return;
						}
						if (e.LeftButton == MouseButtonState.Pressed)
						{
							this.ImageControl.MoveImageBorder(position);
						}
						return;
					}
				}
				if (array != null)
				{
					PdfPageObject[] pointObjects = PageObjectHitTestHelper.GetPointObjects(pdfPage, point, array);
					Rect rect;
					if (!this.textObjectContextMenuHolder.IsOpen && pointObjects.Length != 0 && this.PdfViewer.TryGetClientRect(num, pointObjects[0].BoundingBox, out rect))
					{
						this.UpdateHoverPageObjectRect(rect);
						return;
					}
				}
			}
			this.UpdateHoverPageObjectRect(Rect.Empty);
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0005E798 File Offset: 0x0005C998
		private async void Viewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.isAllowClick())
			{
				ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = this.VM.AnnotationToolbar.InkButtonModel.ToolbarSettingModel.OfType<ToolbarSettingInkEraserModel>().FirstOrDefault<ToolbarSettingInkEraserModel>();
				if (toolbarSettingInkEraserModel.IsChecked && toolbarSettingInkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Partial && this.VM.AnnotationToolbar.InkButtonModel.IsChecked)
				{
					IInputElement captured = Mouse.Captured;
					if (captured != null)
					{
						captured.ReleaseMouseCapture();
					}
					this.pdfEraserUtil.CommitRedoRecords(this.VM, this.Document);
				}
				if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
				{
					Point position = e.GetPosition(this);
					Point point;
					int clientPoint = this.GetClientPoint(position.X, position.Y, out point);
					TaskAwaiter<bool> taskAwaiter = this.ProcessMouseUpAsync(clientPoint, point.ToPdfPoint(), delegate
					{
						this.PdfViewer.DeselectText();
						this.PdfViewer.ReleaseMouseCapture();
					}).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						this.PdfViewer.ReleaseMouseCapture();
					}
				}
				if (AnnotationCanvas.<>o__87.<>p__2 == null)
				{
					AnnotationCanvas.<>o__87.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				Func<CallSite, object, bool> target = AnnotationCanvas.<>o__87.<>p__2.Target;
				CallSite <>p__ = AnnotationCanvas.<>o__87.<>p__2;
				if (AnnotationCanvas.<>o__87.<>p__1 == null)
				{
					AnnotationCanvas.<>o__87.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, MouseModes, object> target2 = AnnotationCanvas.<>o__87.<>p__1.Target;
				CallSite <>p__2 = AnnotationCanvas.<>o__87.<>p__1;
				if (AnnotationCanvas.<>o__87.<>p__0 == null)
				{
					AnnotationCanvas.<>o__87.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				if (target(<>p__, target2(<>p__2, AnnotationCanvas.<>o__87.<>p__0.Target(AnnotationCanvas.<>o__87.<>p__0, this.VM.ViewerMouseMode), MouseModes.PanTool)))
				{
					this.VM.ViewToolbar.PauseAutoScroll(1);
				}
				this.viewerPreviewPressedPageIndex = -1;
			}
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0005E7D8 File Offset: 0x0005C9D8
		private async void Viewer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
			{
				bool dragged = this.viewerDragged;
				if (!dragged && this.viewerPressedPoint != null)
				{
					Point value = this.viewerPressedPoint.Value;
					Point position = e.GetPosition(this);
					if (Math.Abs(position.X - value.X) > 2.0 || Math.Abs(position.Y - value.Y) > 2.0)
					{
						dragged = true;
					}
				}
				this.viewerPressedPoint = null;
				if (this.ImageControl.Visibility == Visibility.Visible && this.ImageControl.IsMoved)
				{
					this.ImageControl.ImageControlMoveImage(e.GetPosition(this));
				}
				if (e.ChangedButton == MouseButton.Right || (dragged && ConfigManager.GetSelectTextShowMenuFlag()))
				{
					this.viewerDragged = false;
					this.viewerPressedPoint = null;
					await this.selectTextContextMenuHolder.ShowAsync(e.ChangedButton == MouseButton.Left);
				}
				if (!dragged)
				{
					Point position2 = e.GetPosition(this);
					Point point;
					int num = this.PdfViewer.DeviceToPage(position2.X, position2.Y, out point);
					if (e.ChangedButton == MouseButton.Right)
					{
						this.tooltipService.HideTooltip();
						if (num >= 0)
						{
							TaskAwaiter<PdfDigitalSignatureField> taskAwaiter = this.digitalSignatureContextMenuHolder.GetPdfDigitalSignatureField().GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								TaskAwaiter<PdfDigitalSignatureField> taskAwaiter2;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<PdfDigitalSignatureField>);
							}
							if (taskAwaiter.GetResult() != null)
							{
								this.focusControl.Annotation = null;
								this.focusControl.InvalidateVisual();
								await this.digitalSignatureContextMenuHolder.ShowAsync();
							}
							else
							{
								this.pageDefaultContextMenuHolder.Show();
							}
						}
					}
					else
					{
						if (AnnotationCanvas.<>o__88.<>p__4 == null)
						{
							AnnotationCanvas.<>o__88.<>p__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						Func<CallSite, object, bool> target = AnnotationCanvas.<>o__88.<>p__4.Target;
						CallSite <>p__ = AnnotationCanvas.<>o__88.<>p__4;
						bool flag = e.ChangedButton == MouseButton.Left && this.Document != null && num != -1;
						object obj;
						if (flag)
						{
							if (AnnotationCanvas.<>o__88.<>p__3 == null)
							{
								AnnotationCanvas.<>o__88.<>p__3 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, bool, object, object> target2 = AnnotationCanvas.<>o__88.<>p__3.Target;
							CallSite <>p__2 = AnnotationCanvas.<>o__88.<>p__3;
							if (AnnotationCanvas.<>o__88.<>p__2 == null)
							{
								AnnotationCanvas.<>o__88.<>p__2 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.And, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, bool, object, object> target3 = AnnotationCanvas.<>o__88.<>p__2.Target;
							CallSite <>p__3 = AnnotationCanvas.<>o__88.<>p__2;
							bool flag2 = this.VM.AnnotationMode == AnnotationMode.Image || this.VM.AnnotationMode == AnnotationMode.None;
							if (AnnotationCanvas.<>o__88.<>p__1 == null)
							{
								AnnotationCanvas.<>o__88.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(AnnotationCanvas), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, MouseModes, object> target4 = AnnotationCanvas.<>o__88.<>p__1.Target;
							CallSite <>p__4 = AnnotationCanvas.<>o__88.<>p__1;
							if (AnnotationCanvas.<>o__88.<>p__0 == null)
							{
								AnnotationCanvas.<>o__88.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(AnnotationCanvas), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							obj = target2(<>p__2, flag, target3(<>p__3, flag2, target4(<>p__4, AnnotationCanvas.<>o__88.<>p__0.Target(AnnotationCanvas.<>o__88.<>p__0, this.VM.ViewerMouseMode), MouseModes.PanTool)));
						}
						else
						{
							obj = flag;
						}
						if (target(<>p__, obj))
						{
							int num2;
							if (PageImageUtils.ImageTestHitTest(this.Document.Pages[num], point, out num2))
							{
								this.ImageControl.CreateImageborder(this, this.Document, num, num2, this.PdfViewer, false);
								this.ImageControl.Visibility = Visibility.Visible;
							}
							else if (!this.ImageControl.IsMoved)
							{
								this.ImageControl.Visibility = Visibility.Collapsed;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0005E818 File Offset: 0x0005CA18
		private void Viewer_LostMouseCapture(object sender, MouseEventArgs e)
		{
			ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = this.VM.AnnotationToolbar.InkButtonModel.ToolbarSettingModel.OfType<ToolbarSettingInkEraserModel>().FirstOrDefault<ToolbarSettingInkEraserModel>();
			if (toolbarSettingInkEraserModel != null && toolbarSettingInkEraserModel.IsChecked && toolbarSettingInkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Partial && this.VM.AnnotationToolbar.InkButtonModel.IsChecked)
			{
				GAManager.SendEvent("InkEraser", "PartialEraser", "EraseInk", 1L);
				this.pdfEraserUtil.CommitRedoRecords(this.VM, this.Document);
			}
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0005E89D File Offset: 0x0005CA9D
		public bool HasSelectedText()
		{
			return AnnotationCanvas.HasSelectedText(this.PdfViewer);
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0005E8AC File Offset: 0x0005CAAC
		private static bool HasSelectedText(PdfViewer viewer)
		{
			return viewer != null && viewer.SelectInfo.StartPage != -1 && viewer.SelectInfo.EndPage != -1 && (viewer.SelectInfo.StartIndex != -1 || viewer.SelectInfo.EndIndex != -1) && !string.IsNullOrWhiteSpace(viewer.SelectedText);
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0005E904 File Offset: 0x0005CB04
		private int GetClientPoint(double clientPosX, double clientPosY, out Point pagePoint)
		{
			pagePoint = default(Point);
			if (this.Document == null)
			{
				return -1;
			}
			int num = this.viewerPreviewPressedPageIndex;
			if (num >= 0 && num < this.Document.Pages.Count && this.PdfViewer.TryGetPagePoint(num, new Point(clientPosX, clientPosY), out pagePoint))
			{
				return num;
			}
			return this.PdfViewer.DeviceToPage(clientPosX, clientPosY, out pagePoint);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0005E968 File Offset: 0x0005CB68
		private bool ProcessMouseMove(int pageIndex, FS_POINTF pagePoint)
		{
			if (pageIndex >= 0 && this.Document != null && pageIndex < this.Document.Pages.Count)
			{
				PdfPage pdfPage = this.Document.Pages[pageIndex];
				return this.ProcessCreateNewAnnot(pdfPage, pagePoint);
			}
			return false;
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x0005E9B0 File Offset: 0x0005CBB0
		private async Task<bool> ProcessMouseUpAsync(int pageIndex, FS_POINTF pagePoint, Action setHandledFunc)
		{
			if (pageIndex >= 0 && this.Document != null && pageIndex < this.Document.Pages.Count)
			{
				PdfPage pdfPage = this.Document.Pages[pageIndex];
				if (this.ProcessCreateNewAnnot(pdfPage, pagePoint))
				{
					if (setHandledFunc != null)
					{
						setHandledFunc();
					}
					TaskAwaiter<bool> taskAwaiter = this.CompleteCreateNewAnnotAsync().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					return !taskAwaiter.GetResult();
				}
			}
			return false;
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x0005EA0C File Offset: 0x0005CC0C
		private bool StartCreateNewAnnot(PdfPage page, FS_POINTF pagePoint)
		{
			if ((this.holders.CurrentHolder == null || this.holders.CurrentHolder.State == AnnotationHolderState.None) && this.Document != null)
			{
				switch (this.VM.AnnotationMode)
				{
				case AnnotationMode.Line:
					this.holders.Line.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Ink:
					this.holders.Ink.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Shape:
					this.holders.Square.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Highlight:
					this.holders.Highlight.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Underline:
					this.holders.Underline.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Strike:
					this.holders.Strikeout.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.HighlightArea:
					this.holders.HighlightArea.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Note:
					this.holders.Text.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Ellipse:
					this.holders.Circle.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Stamp:
					goto IL_0181;
				case AnnotationMode.Text:
					this.holders.FreeText.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.TextBox:
					this.holders.FreeText.StartCreateNew(page, pagePoint);
					goto IL_0181;
				case AnnotationMode.Link:
					this.holders.Link.StartCreateNew(page, pagePoint);
					goto IL_0181;
				}
				return false;
				IL_0181:
				return this.holders.CurrentHolder != null;
			}
			return false;
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0005EBAC File Offset: 0x0005CDAC
		private bool ProcessCreateNewAnnot(PdfPage page, FS_POINTF pagePoint)
		{
			if (page == null)
			{
				return false;
			}
			IAnnotationHolder currentHolder = this.holders.CurrentHolder;
			if (currentHolder == null || currentHolder.State != AnnotationHolderState.CreatingNew)
			{
				return false;
			}
			currentHolder.ProcessCreateNew(page, pagePoint);
			return true;
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x0005EBEC File Offset: 0x0005CDEC
		private async Task<bool> CompleteCreateNewAnnotAsync()
		{
			IAnnotationHolder holder = this.holders.CurrentHolder;
			bool flag;
			if (holder == null)
			{
				flag = false;
			}
			else
			{
				global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> readOnlyList = await holder.CompleteCreateNewAsync();
				if (readOnlyList != null && readOnlyList.Count > 0)
				{
					if (!this.VM.IsAnnotationVisible)
					{
						this.VM.IsAnnotationVisible = true;
					}
					if (!holder.IsTextMarkupAnnotation && readOnlyList.Count == 1 && holder != this.holders.Ink)
					{
						this.holders.Select(readOnlyList[0], true);
					}
					this.focusControl.Annotation = null;
					this.UpdateHoverAnnotationBorder();
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x0005EC30 File Offset: 0x0005CE30
		private void RouteClickEventToPdfViewer(MouseButtonEventArgs e)
		{
			MouseButtonEventArgs mouseButtonEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
			mouseButtonEventArgs.RoutedEvent = UIElement.MouseDownEvent;
			mouseButtonEventArgs.Source = this.PdfViewer;
			this.PdfViewer.RaiseEvent(mouseButtonEventArgs);
			MouseButtonEventArgs mouseButtonEventArgs2 = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
			mouseButtonEventArgs2.RoutedEvent = UIElement.MouseUpEvent;
			mouseButtonEventArgs2.Source = this.PdfViewer;
			this.doubleClickHelper.MouseDoubleClick -= this.DoubleClickHelper_MouseDoubleClick;
			this.PdfViewer.RaiseEvent(mouseButtonEventArgs2);
			this.doubleClickHelper.MouseDoubleClick += this.DoubleClickHelper_MouseDoubleClick;
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x0005ECE4 File Offset: 0x0005CEE4
		private void Viewer_AnnotationMouseMoved(object sender, AnnotationMouseEventArgs e)
		{
			if (this.ImageControl.IsMoved)
			{
				return;
			}
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				e.Handled = true;
			}
			if (!(e.PdfAnnotation is PdfLinkAnnotation) || !this.VM.AnnotationToolbar.LinkButtonModel.IsChecked)
			{
				if (this.VM.AnnotationToolbar.LinkButtonModel.IsChecked)
				{
					this.PdfViewer.DeselectText();
					if (e.PdfAnnotation is PdfFileAttachmentAnnotation)
					{
						return;
					}
					if (this.holders.CurrentHolder != null && this.holders.CurrentHolder.State == AnnotationHolderState.CreatingNew)
					{
						return;
					}
					e.Handled = true;
					this.holders.Select(null, false);
					this.focusControl.Annotation = null;
					this.UpdateHoverAnnotationBorder();
				}
				return;
			}
			this.PdfViewer.DeselectText();
			if (e.PdfAnnotation is PdfFileAttachmentAnnotation)
			{
				return;
			}
			e.Handled = true;
			this.holders.Select(e.PdfAnnotation, false);
			this.focusControl.Annotation = null;
			this.UpdateHoverAnnotationBorder();
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x0005EDF0 File Offset: 0x0005CFF0
		private void Viewer_AnnotationMouseEntered(object sender, AnnotationMouseEventArgs e)
		{
			if (this.ImageControl.IsMoved)
			{
				return;
			}
			if (this.VM.AnnotationToolbar.InkButtonModel.IsChecked || (this.VM.AnnotationToolbar.LinkButtonModel.IsChecked && !(e.PdfAnnotation is PdfLinkAnnotation)) || this.VM.AnnotationMode == AnnotationMode.Image || this.ImageControl.ImageControlState)
			{
				return;
			}
			if (e.PdfAnnotation.IsDigitalSignatureAnnotation() && this.VM.ViewerOperationModel == null)
			{
				this.PdfViewer.OverrideCursor = Cursors.Hand;
			}
			if (this.EditingPageObjectType == PageObjectType.None && this.SelectedAnnotation != e.PdfAnnotation && (this.holders.CurrentHolder == null || !this.holders.CurrentHolder.IsTextMarkupAnnotation || this.holders.CurrentHolder.State != AnnotationHolderState.CreatingNew))
			{
				this.focusControl.Annotation = e.PdfAnnotation;
			}
			else
			{
				this.focusControl.Annotation = null;
			}
			if (this.VM.AnnotationToolbar.LinkButtonModel.IsChecked && !(this.focusControl.Annotation is PdfLinkAnnotation))
			{
				return;
			}
			this.UpdateHoverAnnotationBorder();
			this.popupHolder.SetPopupHovered(e.PdfAnnotation, true);
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x0005EF3C File Offset: 0x0005D13C
		private void Viewer_AnnotationMouseExited(object sender, AnnotationMouseEventArgs e)
		{
			if (this.ImageControl.IsMoved)
			{
				return;
			}
			this.focusControl.Annotation = null;
			this.UpdateHoverAnnotationBorder();
			if (e.PdfAnnotation.IsDigitalSignatureAnnotation() && this.VM.ViewerOperationModel == null)
			{
				this.PdfViewer.OverrideCursor = null;
			}
			if (this.VM.AnnotationToolbar.InkButtonModel.IsChecked || (this.VM.AnnotationToolbar.LinkButtonModel.IsChecked && !(e.PdfAnnotation is PdfLinkAnnotation)))
			{
				return;
			}
			this.popupHolder.SetPopupHovered(e.PdfAnnotation, false);
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0005EFE0 File Offset: 0x0005D1E0
		private async void Viewer_AnnotationMouseClick(object sender, AnnotationMouseClickEventArgs e)
		{
			if (!this.VM.AnnotationToolbar.InkButtonModel.IsChecked && !this.ImageControl.IsMoved && (!this.VM.AnnotationToolbar.LinkButtonModel.IsChecked || e.PdfAnnotation is PdfLinkAnnotation) && this.VM.AnnotationMode != AnnotationMode.Image && !this.ImageControl.ImageControlState)
			{
				if (!this.VM.AnnotationToolbar.LinkButtonModel.IsChecked && e.PdfAnnotation is PdfLinkAnnotation)
				{
					if (e.ChangeButton == MouseButton.Right)
					{
						e.Handled = true;
					}
				}
				else if (!this.VM.AnnotationToolbar.LinkButtonModel.IsChecked || e.PdfAnnotation is PdfLinkAnnotation)
				{
					if (this.EditingPageObjectType == PageObjectType.None)
					{
						if (e.ChangeButton == MouseButton.Left)
						{
							this.PdfViewer.DeselectText();
							if (!(e.PdfAnnotation is PdfFileAttachmentAnnotation))
							{
								if (e.PdfAnnotation.IsDigitalSignatureAnnotation())
								{
									e.Handled = true;
									PdfDigitalSignatureLocation location = this.VM.DocumentWrapper.DigitalSignatureHelper.GetLocation((PdfWidgetAnnotation)e.PdfAnnotation);
									if (location != null)
									{
										if (location.HasSigned)
										{
											await SignatureValidateHelper.ShowSignaturePropertiesDialogAsync(await this.VM.DocumentWrapper.DigitalSignatureHelper.GetFieldInfo((PdfWidgetAnnotation)e.PdfAnnotation), true, true);
										}
										else
										{
											GAManager.SendEvent("DigitalSignatureFiled", "SignInDSFiled", "MouseClick", 1L);
											await this.VM.AnnotationToolbar.SignDeferredDigitalSignature(location);
										}
									}
								}
								else
								{
									e.Handled = true;
									this.holders.Select(e.PdfAnnotation, false);
									if (this.VM.AnnotationToolbar.ImageButtonModel.IsChecked)
									{
										this.ImageControl.quitImageControl();
									}
									this.focusControl.Annotation = null;
									this.UpdateHoverAnnotationBorder();
								}
							}
						}
						else if (e.ChangeButton == MouseButton.Right)
						{
							e.Handled = true;
							this.PdfViewer.DeselectText();
							if (!e.PdfAnnotation.IsDigitalSignatureAnnotation())
							{
								this.holders.Select(e.PdfAnnotation, false);
								this.focusControl.Annotation = null;
								this.UpdateHoverAnnotationBorder();
								if (!(e.PdfAnnotation is PdfLinkAnnotation))
								{
									TaskAwaiter<bool> taskAwaiter = this.annotationContextMenuHolder.ShowAsync().GetAwaiter();
									if (!taskAwaiter.IsCompleted)
									{
										await taskAwaiter;
										TaskAwaiter<bool> taskAwaiter2;
										taskAwaiter = taskAwaiter2;
										taskAwaiter2 = default(TaskAwaiter<bool>);
									}
									if (!taskAwaiter.GetResult() && e.PdfAnnotation is PdfFileAttachmentAnnotation)
									{
										e.Handled = true;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x0005F020 File Offset: 0x0005D220
		private async void DoubleClickHelper_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (this.holders.IsAnnotationDoubleClicked(e))
			{
				PdfAnnotation selectedAnnotation = this.holders.SelectedAnnotation;
				if (selectedAnnotation != null && !this.popupHolder.IsPopupVisible(selectedAnnotation) && this.popupHolder.TryShowPopup(selectedAnnotation))
				{
					e.Handled = true;
				}
			}
			else
			{
				int num;
				PdfAnnotation pointAnnotation = this.PdfViewer.GetPointAnnotation(e.GetPosition(this.PdfViewer), out num);
				PdfFileAttachmentAnnotation attachAnnot = pointAnnotation as PdfFileAttachmentAnnotation;
				if (attachAnnot != null)
				{
					GAManager.SendEvent("PDFAttachment", "Open", "ViewerDoubleClick", 1L);
					bool flag = true;
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = attachAnnot;
					if (!AttachmentFileUtils.IsUrl((pdfFileAttachmentAnnotation != null) ? pdfFileAttachmentAnnotation.FileSpecification : null))
					{
						TaskAwaiter<string> taskAwaiter = AttachmentFileUtils.ExtraAttachmentFromAnnotation(attachAnnot).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<string> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<string>);
						}
						if (string.IsNullOrEmpty(taskAwaiter.GetResult()))
						{
							flag = false;
						}
					}
					if (flag)
					{
						AnnotationTooltipService annotationTooltipService = this.tooltipService;
						if (annotationTooltipService != null)
						{
							annotationTooltipService.HideTooltip();
						}
						if (ModernMessageBox.Show(pdfeditor.Properties.Resources.AnnotationFileAttachmentOpenWarning, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							await AttachmentFileUtils.OpenAttachmentFromAnnotation(attachAnnot).ConfigureAwait(false);
						}
					}
					e.Handled = true;
				}
				else
				{
					PdfLinkAnnotation pdfLinkAnnotation = pointAnnotation as PdfLinkAnnotation;
					if (pdfLinkAnnotation != null && this.VM.AnnotationToolbar.LinkButtonModel.IsChecked)
					{
						GAManager.SendEvent("PDFLink", "Editor", "Count", 1L);
						float docZoom = Ioc.Default.GetRequiredService<MainViewModel>().ViewToolbar.DocZoom;
						LinkAnnotationUtils.LinkAnnotationop(pdfLinkAnnotation, this.Document, pointAnnotation.Page, docZoom, this.VM);
					}
					await base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
					{
						this.viewerDragged = false;
						this.viewerPressedPoint = null;
						this.selectTextContextMenuHolder.ShowAsync(true);
					}));
					attachAnnot = null;
				}
			}
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0005F05F File Offset: 0x0005D25F
		private void Viewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateHoverAnnotationBorder();
			this.UpdateViewerFlyoutExtendWidth();
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0005F070 File Offset: 0x0005D270
		private void Viewer_ScrollOwnerChanged(object sender, EventArgs e)
		{
			if (this.scrollViewer != null)
			{
				MouseMiddleButtonScrollExtensions.SetIsEnabled(this.scrollViewer, false);
				MouseMiddleButtonScrollExtensions.SetShowCursorAtStartPoint(this.scrollViewer, false);
				this.scrollViewer.ScrollChanged -= this.ScrollOwner_ScrollChanged;
			}
			PdfViewer pdfViewer = this.PdfViewer;
			this.scrollViewer = ((pdfViewer != null) ? pdfViewer.ScrollOwner : null);
			if (this.scrollViewer != null)
			{
				MouseMiddleButtonScrollExtensions.SetIsEnabled(this.scrollViewer, true);
				MouseMiddleButtonScrollExtensions.SetShowCursorAtStartPoint(this.scrollViewer, true);
				this.scrollViewer.ScrollChanged += this.ScrollOwner_ScrollChanged;
			}
			this.UpdateHoverAnnotationBorder();
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0005F10C File Offset: 0x0005D30C
		public void UpdateViewerFlyoutExtendWidth()
		{
			if (this.PdfViewer != null)
			{
				DpiScale dpi = VisualTreeHelper.GetDpi(this);
				double maxPopupWidth = this.popupHolder.GetMaxPopupWidth();
				this.PdfViewer.FlyoutExtentWidth = maxPopupWidth * dpi.PixelsPerDip + 20.0;
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0005F152 File Offset: 0x0005D352
		private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "HorizontalScrollBarHeight" || e.PropertyName == "VerticalScrollBarWidth")
			{
				this.UpdateViewerFlyoutExtendWidth();
			}
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0005F17E File Offset: 0x0005D37E
		private void ScrollOwner_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				this.viewerDragged = true;
			}
			this.UpdateHoverAnnotationBorder();
			if (e.ExtentWidthChange != 0.0)
			{
				this.UpdateViewerFlyoutExtendWidth();
			}
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0005F1AC File Offset: 0x0005D3AC
		private void Viewer_BeforeDocumentChanged(object sender, DocumentClosingEventArgs e)
		{
			this.SelectedAnnotation = null;
			this.focusControl.Annotation = null;
			this.UpdateHoverAnnotationBorder();
			this.popupHolder.ClearAnnotationPopup();
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0005F1D2 File Offset: 0x0005D3D2
		private void Viewer_AfterDocumentChanged(object sender, EventArgs e)
		{
			AnnotationPopupHolder annotationPopupHolder = this.popupHolder;
			PdfDocument document = this.PdfViewer.Document;
			PdfPage pdfPage;
			if (document == null)
			{
				pdfPage = null;
			}
			else
			{
				PdfPageCollection pages = document.Pages;
				pdfPage = ((pages != null) ? pages.CurrentPage : null);
			}
			annotationPopupHolder.InitAnnotationPopup(pdfPage);
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0005F204 File Offset: 0x0005D404
		private void Viewer_CurrentPageChanged(object sender, EventArgs e)
		{
			this.popupHolder.ClearAnnotationPopup();
			if (this.PdfViewer.Document == null)
			{
				return;
			}
			PdfPage pdfPage = null;
			try
			{
				pdfPage = this.PdfViewer.CurrentPage;
			}
			catch
			{
				GAManager.SendEvent("AnnotCanvas", "PageChanged", "Crash", 1L);
			}
			if (pdfPage == null)
			{
				return;
			}
			this.popupHolder.InitAnnotationPopup(pdfPage);
			IAnnotationHolder currentHolder = this.holders.CurrentHolder;
			if (currentHolder != null && !currentHolder.IsTextMarkupAnnotation && this.holders.Stamp.State != AnnotationHolderState.CreatingNew)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					this.holders.CancelAll();
				}));
			}
			this.textObjectHolder.CancelTextObject();
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0005F2C8 File Offset: 0x0005D4C8
		private void Viewer_MouseModeChanged(object sender, EventArgs e)
		{
			PdfViewer pdfViewer = this.PdfViewer;
			if (pdfViewer != null)
			{
				pdfViewer.DeselectText();
			}
			this.holders.CancelAll();
			this.UpdateHoverAnnotationBorder();
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0005F2EC File Offset: 0x0005D4EC
		private void Viewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			PdfViewerAutoScrollHelper pdfViewerAutoScrollHelper = this.pdfViewerAutoScrollHelper;
			if (pdfViewerAutoScrollHelper == null)
			{
				return;
			}
			pdfViewerAutoScrollHelper.Pause(200);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x0005F304 File Offset: 0x0005D504
		private async void Viewer_BeforeLinkClicked(object sender, PdfBeforeLinkClickedEventArgs e)
		{
			PdfLink link = e.Link;
			if (((link != null) ? link.Action : null) != null)
			{
				ActionTypes actionType = e.Link.Action.ActionType;
				if (actionType != ActionTypes.CurrentDoc && actionType - ActionTypes.Uri > 1UL)
				{
					e.Cancel = true;
				}
				else if (e.Link.Action.ActionType == ActionTypes.Uri || e.Link.Action.ActionType == ActionTypes.Application)
				{
					this.tooltipService.HideTooltip();
					string linkUrlOrFileName = LinkAnnotationUtils.GetLinkUrlOrFileName(e.Link);
					if (ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkActionToUri.Replace("XXX", linkUrlOrFileName), UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
					{
						e.Cancel = true;
					}
					else
					{
						PdfLaunchAction pdfLaunchAction = e.Link.Action as PdfLaunchAction;
						if (pdfLaunchAction != null)
						{
							try
							{
								TaskAwaiter<bool> taskAwaiter = AttachmentFileUtils.OpenFileSpecAsync((pdfLaunchAction != null) ? pdfLaunchAction.FileSpecification : null).GetAwaiter();
								if (!taskAwaiter.IsCompleted)
								{
									await taskAwaiter;
									TaskAwaiter<bool> taskAwaiter2;
									taskAwaiter = taskAwaiter2;
									taskAwaiter2 = default(TaskAwaiter<bool>);
								}
								if (!taskAwaiter.GetResult())
								{
									ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkOpenFileFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
								}
							}
							catch
							{
								ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkOpenFileFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0005F344 File Offset: 0x0005D544
		public async Task StartScreenShotAsync(ScreenshotDialogMode mode, int[] pages = null)
		{
			if (this.Document != null)
			{
				if (this.screenshotDialog.Visibility == Visibility.Visible)
				{
					this.screenshotDialog.Close(null);
				}
				base.Margin = default(Thickness);
				ScreenshotDialogResult screenshotDialogResult = await this.screenshotDialog.ShowDialogAsync(mode, pages);
				if (screenshotDialogResult != null)
				{
					if (screenshotDialogResult.Mode == ScreenshotDialogMode.ExtractText || screenshotDialogResult.Mode == ScreenshotDialogMode.Ocr)
					{
						if (screenshotDialogResult.Completed)
						{
							ExtractTextResultDialog extractTextResultDialog = ExtractTextResultDialog.FromPage(this.Document, screenshotDialogResult);
							extractTextResultDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
							extractTextResultDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
							extractTextResultDialog.ShowDialog();
						}
					}
					else if (screenshotDialogResult.Mode == ScreenshotDialogMode.Screenshot)
					{
						if (screenshotDialogResult.Completed)
						{
							ComparisonWindow comparisonWindow = Application.Current.Windows.OfType<ComparisonWindow>().FirstOrDefault<ComparisonWindow>();
							if (comparisonWindow == null)
							{
								comparisonWindow = new ComparisonWindow();
							}
							comparisonWindow.SetContent(this.Document, screenshotDialogResult);
						}
					}
					else if (screenshotDialogResult.Mode == ScreenshotDialogMode.CropPage)
					{
						if (screenshotDialogResult.Completed && screenshotDialogResult.ApplyPageIndex != null)
						{
							AnnotationCanvas.<>c__DisplayClass115_0 CS$<>8__locals1 = new AnnotationCanvas.<>c__DisplayClass115_0();
							GAManager.SendEvent("CropPage", "DoCrop", string.Format("{0}", screenshotDialogResult.ApplyPageIndex.Length), 1L);
							CS$<>8__locals1.list = new List<global::System.ValueTuple<int, FS_RECTF, FS_RECTF>>();
							for (int i = 0; i < screenshotDialogResult.ApplyPageIndex.Length; i++)
							{
								PdfPage pdfPage = this.Document.Pages[screenshotDialogResult.ApplyPageIndex[i]];
								FS_RECTF fs_RECTF = screenshotDialogResult.BeforeRects[i];
								FS_RECTF fs_RECTF2 = screenshotDialogResult.SelectedRects[i];
								CS$<>8__locals1.list.Add(new global::System.ValueTuple<int, FS_RECTF, FS_RECTF>(screenshotDialogResult.ApplyPageIndex[i], fs_RECTF, fs_RECTF2));
								pdfPage.SetPageCropBox(screenshotDialogResult.SelectedRects[i]);
								pdfPage.ReloadPage();
							}
							this.VM.PageEditors.FlushViewerAndThumbnail(false);
							this.PdfViewer.UpdateLayout();
							this.PdfViewer.UpdateDocLayout();
							this.PdfViewer.TryRedrawVisiblePageAsync(default(CancellationToken));
							this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
							{
								AnnotationCanvas.<>c__DisplayClass115_0.<<StartScreenShotAsync>b__0>d <<StartScreenShotAsync>b__0>d;
								<<StartScreenShotAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<StartScreenShotAsync>b__0>d.<>4__this = CS$<>8__locals1;
								<<StartScreenShotAsync>b__0>d.doc = doc;
								<<StartScreenShotAsync>b__0>d.<>1__state = -1;
								<<StartScreenShotAsync>b__0>d.<>t__builder.Start<AnnotationCanvas.<>c__DisplayClass115_0.<<StartScreenShotAsync>b__0>d>(ref <<StartScreenShotAsync>b__0>d);
								return <<StartScreenShotAsync>b__0>d.<>t__builder.Task;
							}, delegate(PdfDocument doc)
							{
								AnnotationCanvas.<>c__DisplayClass115_0.<<StartScreenShotAsync>b__1>d <<StartScreenShotAsync>b__1>d;
								<<StartScreenShotAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<StartScreenShotAsync>b__1>d.<>4__this = CS$<>8__locals1;
								<<StartScreenShotAsync>b__1>d.doc = doc;
								<<StartScreenShotAsync>b__1>d.<>1__state = -1;
								<<StartScreenShotAsync>b__1>d.<>t__builder.Start<AnnotationCanvas.<>c__DisplayClass115_0.<<StartScreenShotAsync>b__1>d>(ref <<StartScreenShotAsync>b__1>d);
								return <<StartScreenShotAsync>b__1>d.<>t__builder.Task;
							}, "");
						}
					}
					else if (screenshotDialogResult.Mode == ScreenshotDialogMode.ResizePage && screenshotDialogResult.Completed && screenshotDialogResult.ApplyPageIndex != null)
					{
						AnnotationCanvas.<>c__DisplayClass115_1 CS$<>8__locals2 = new AnnotationCanvas.<>c__DisplayClass115_1();
						GAManager.SendEvent("ResizePage", "DoResize", "Count", 1L);
						CS$<>8__locals2.list = new List<global::System.ValueTuple<int, FS_RECTF, FS_RECTF>>();
						for (int j = 0; j < screenshotDialogResult.ApplyPageIndex.Length; j++)
						{
							PdfPage pdfPage2 = this.Document.Pages[screenshotDialogResult.ApplyPageIndex[j]];
							FS_RECTF fs_RECTF3 = screenshotDialogResult.BeforeRects[j];
							FS_RECTF fs_RECTF4 = screenshotDialogResult.SelectedRects[j];
							CS$<>8__locals2.list.Add(new global::System.ValueTuple<int, FS_RECTF, FS_RECTF>(screenshotDialogResult.ApplyPageIndex[j], fs_RECTF3, fs_RECTF4));
							pdfPage2.MediaBox = screenshotDialogResult.SelectedRects[j];
							pdfPage2.SetPageCropBox(screenshotDialogResult.SelectedRects[j]);
							pdfPage2.ReloadPage();
						}
						this.VM.ViewToolbar.DocSizeMode = SizeModes.Zoom;
						this.VM.ViewToolbar.DocZoom = 1f;
						this.VM.PageEditors.FlushViewerAndThumbnail(false);
						this.PdfViewer.UpdateLayout();
						this.PdfViewer.UpdateDocLayout();
						this.PdfViewer.TryRedrawVisiblePageAsync(default(CancellationToken));
						this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
						{
							AnnotationCanvas.<>c__DisplayClass115_1.<<StartScreenShotAsync>b__2>d <<StartScreenShotAsync>b__2>d;
							<<StartScreenShotAsync>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<StartScreenShotAsync>b__2>d.<>4__this = CS$<>8__locals2;
							<<StartScreenShotAsync>b__2>d.doc = doc;
							<<StartScreenShotAsync>b__2>d.<>1__state = -1;
							<<StartScreenShotAsync>b__2>d.<>t__builder.Start<AnnotationCanvas.<>c__DisplayClass115_1.<<StartScreenShotAsync>b__2>d>(ref <<StartScreenShotAsync>b__2>d);
							return <<StartScreenShotAsync>b__2>d.<>t__builder.Task;
						}, delegate(PdfDocument doc)
						{
							AnnotationCanvas.<>c__DisplayClass115_1.<<StartScreenShotAsync>b__3>d <<StartScreenShotAsync>b__3>d;
							<<StartScreenShotAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<StartScreenShotAsync>b__3>d.<>4__this = CS$<>8__locals2;
							<<StartScreenShotAsync>b__3>d.doc = doc;
							<<StartScreenShotAsync>b__3>d.<>1__state = -1;
							<<StartScreenShotAsync>b__3>d.<>t__builder.Start<AnnotationCanvas.<>c__DisplayClass115_1.<<StartScreenShotAsync>b__3>d>(ref <<StartScreenShotAsync>b__3>d);
							return <<StartScreenShotAsync>b__3>d.<>t__builder.Task;
						}, "");
					}
				}
			}
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0005F397 File Offset: 0x0005D597
		public void CloseScreenShot()
		{
			if (this.screenshotDialog.Visibility != Visibility.Visible)
			{
				return;
			}
			this.screenshotDialog.Close(null);
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0005F3B3 File Offset: 0x0005D5B3
		private void ScreenshotDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.screenshotDialog;
			if (screenshotDialog == null || screenshotDialog.Visibility > Visibility.Visible)
			{
				this.UpdateViewerFlyoutExtendWidth();
				EventHandler screenshotDialogClosed = this.ScreenshotDialogClosed;
				if (screenshotDialogClosed == null)
				{
					return;
				}
				screenshotDialogClosed(this, EventArgs.Empty);
			}
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0005F3E8 File Offset: 0x0005D5E8
		private bool isAllowClick()
		{
			return this.VM.AnnotationMode != AnnotationMode.Stamp || DateTime.Now.Subtract(this.VM.AnnotationToolbar.StampImgFileOkTime).TotalSeconds >= 1.0;
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0005F43C File Offset: 0x0005D63C
		public void UpdateHoverPageObjectRect(Rect rect)
		{
			if (rect.IsEmpty || rect.Width == 0.0 || rect.Height == 0.0)
			{
				this.textObjRect.Visibility = Visibility.Collapsed;
				return;
			}
			this.textObjRect.Visibility = Visibility.Visible;
			Panel.SetZIndex(this.textObjRect, 2);
			Canvas.SetLeft(this.textObjRect, rect.Left - 1.0);
			Canvas.SetTop(this.textObjRect, rect.Top - 1.0);
			this.textObjRect.Width = rect.Width + 2.0;
			this.textObjRect.Height = rect.Height + 2.0;
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0005F50C File Offset: 0x0005D70C
		public void UpdateAutoScrollHelper()
		{
			PdfViewerAutoScrollHelper pdfViewerAutoScrollHelper = this.pdfViewerAutoScrollHelper;
			if (pdfViewerAutoScrollHelper != null)
			{
				pdfViewerAutoScrollHelper.Dispose();
			}
			this.pdfViewerAutoScrollHelper = null;
			ViewToolbarViewModel viewToolbar = this.VM.ViewToolbar;
			if (((viewToolbar != null) ? viewToolbar.AutoScrollButtonModel : null) != null)
			{
				this.VM.ViewToolbar.AutoScrollButtonModel.IsChecked = false;
			}
			if (this.PdfViewer != null)
			{
				this.pdfViewerAutoScrollHelper = new PdfViewerAutoScrollHelper(this.PdfViewer)
				{
					Speed = (double)this.AutoScrollSpeed
				};
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0005F586 File Offset: 0x0005D786
		public void TryShowTextObjectContextMenu()
		{
			TextObjectContextMenuHolder textObjectContextMenuHolder = this.textObjectContextMenuHolder;
			if (textObjectContextMenuHolder == null)
			{
				return;
			}
			textObjectContextMenuHolder.ShowAsync();
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060018B5 RID: 6325 RVA: 0x0005F59C File Offset: 0x0005D79C
		// (remove) Token: 0x060018B6 RID: 6326 RVA: 0x0005F5D4 File Offset: 0x0005D7D4
		public event EventHandler ScreenshotDialogClosed;

		// Token: 0x04000829 RID: 2089
		private PDFEraserUtil pdfEraserUtil;

		// Token: 0x0400082A RID: 2090
		private DoubleClickHelper doubleClickHelper;

		// Token: 0x0400082B RID: 2091
		private AnnotationTooltipService tooltipService;

		// Token: 0x0400082C RID: 2092
		private Rectangle hitTestElement;

		// Token: 0x0400082D RID: 2093
		private ScrollViewer scrollViewer;

		// Token: 0x0400082E RID: 2094
		private AnnotationHolderManager holders;

		// Token: 0x0400082F RID: 2095
		private AnnotationFocusControl focusControl;

		// Token: 0x04000830 RID: 2096
		private ScreenshotDialog screenshotDialog;

		// Token: 0x04000831 RID: 2097
		private bool viewerDragged;

		// Token: 0x04000832 RID: 2098
		private Point? viewerPressedPoint;

		// Token: 0x04000833 RID: 2099
		private int viewerPreviewPressedPageIndex;

		// Token: 0x04000834 RID: 2100
		public FS_RECTF ImageRect;

		// Token: 0x04000835 RID: 2101
		public int ImageIndex = -1;

		// Token: 0x04000836 RID: 2102
		public ImageControl ImageControl;

		// Token: 0x04000837 RID: 2103
		private AnnotationContextMenuHolder annotationContextMenuHolder;

		// Token: 0x04000838 RID: 2104
		private SelectTextContextMenuHolder selectTextContextMenuHolder;

		// Token: 0x04000839 RID: 2105
		private TextObjectContextMenuHolder textObjectContextMenuHolder;

		// Token: 0x0400083A RID: 2106
		private PageDefaultContextMenuHolder pageDefaultContextMenuHolder;

		// Token: 0x0400083B RID: 2107
		private DigitalSignatureContextMenuHolder digitalSignatureContextMenuHolder;

		// Token: 0x0400083C RID: 2108
		private AnnotationPopupHolder popupHolder;

		// Token: 0x0400083D RID: 2109
		private TextObjectHolder textObjectHolder;

		// Token: 0x0400083E RID: 2110
		private PdfViewerAutoScrollHelper pdfViewerAutoScrollHelper;

		// Token: 0x0400083F RID: 2111
		private Rectangle textObjRect;

		// Token: 0x04000840 RID: 2112
		public Point StampStartPoint;

		// Token: 0x04000841 RID: 2113
		public static readonly DependencyProperty PdfViewerProperty = DependencyProperty.Register("PdfViewer", typeof(PdfViewer), typeof(AnnotationCanvas), new PropertyMetadata(null, new PropertyChangedCallback(AnnotationCanvas.OnPdfViewerPropertyChanged)));

		// Token: 0x04000842 RID: 2114
		public static readonly DependencyProperty SelectedAnnotationProperty = DependencyProperty.Register("SelectedAnnotation", typeof(PdfAnnotation), typeof(AnnotationCanvas), new PropertyMetadata(null, new PropertyChangedCallback(AnnotationCanvas.OnSelectedAnnotationPropertyChanged)));

		// Token: 0x04000843 RID: 2115
		public static readonly DependencyProperty IsAnnotationVisibleProperty = DependencyProperty.Register("IsAnnotationVisible", typeof(bool), typeof(AnnotationCanvas), new PropertyMetadata(true, new PropertyChangedCallback(AnnotationCanvas.OnIsAnnotationVisiblePropertyChanged)));

		// Token: 0x04000844 RID: 2116
		private PageObjectTypes[] editingPageObjectTypes;

		// Token: 0x04000845 RID: 2117
		public static readonly DependencyProperty EditingPageObjectTypeProperty = DependencyProperty.Register("EditingPageObjectType", typeof(PageObjectType), typeof(AnnotationCanvas), new PropertyMetadata(PageObjectType.None, new PropertyChangedCallback(AnnotationCanvas.OnEditingPageObjectTypePropertyChanged)));

		// Token: 0x04000846 RID: 2118
		public static readonly DependencyProperty AutoScrollSpeedProperty = DependencyProperty.Register("AutoScrollSpeed", typeof(int), typeof(AnnotationCanvas), new PropertyMetadata(1, new PropertyChangedCallback(AnnotationCanvas.OnAutoScrollSpeedPropertyChanged)));
	}
}
