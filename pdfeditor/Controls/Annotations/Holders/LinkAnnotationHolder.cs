using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Actions;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B9 RID: 697
	public class LinkAnnotationHolder : BaseAnnotationHolder<PdfLinkAnnotation>
	{
		// Token: 0x06002838 RID: 10296 RVA: 0x000BCE72 File Offset: 0x000BB072
		public LinkAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x06002839 RID: 10297 RVA: 0x000BCE7B File Offset: 0x000BB07B
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x000BCE80 File Offset: 0x000BB080
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (this.newSquareControl == null)
				{
					throw new ArgumentException("newSquareControl");
				}
				if (base.CurrentPage == null)
				{
					throw new ArgumentException("CurrentPage");
				}
			}
			else if (base.State == AnnotationHolderState.Selected)
			{
				AnnotationDragEditControl annotationDragEditControl = this.editControl;
				if (annotationDragEditControl == null)
				{
					return;
				}
				annotationDragEditControl.OnPageClientLinkBoundsChanged();
			}
		}

		// Token: 0x0600283B RID: 10299 RVA: 0x000BCED8 File Offset: 0x000BB0D8
		protected override void OnCancel()
		{
			if (this.newSquareControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newSquareControl);
				this.newSquareControl = null;
			}
			if (this.editControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.editControl);
				this.editControl = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x000BCF48 File Offset: 0x000BB148
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfLinkAnnotation>> OnCompleteCreateNewAsync()
		{
			base.AnnotationCanvas.Children.Remove(this.newSquareControl);
			if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) > 5f || Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) > 5f)
			{
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				LinkEditWindows linkEditWindows = new LinkEditWindows(mainViewModel.Document);
				linkEditWindows.Owner = App.Current.MainWindow;
				linkEditWindows.WindowStartupLocation = ((linkEditWindows.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
				bool? flag = linkEditWindows.ShowDialog();
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				PdfPage page = base.CurrentPage;
				if (page.Annots == null)
				{
					page.CreateAnnotations();
				}
				if (flag.GetValueOrDefault())
				{
					PdfLinkAnnotation LinkAnnot = new PdfLinkAnnotation(page);
					if (linkEditWindows.SelectedType == LinkSelect.ToPage)
					{
						this.Page = linkEditWindows.Page - 1;
						PdfDestination pdfDestination = PdfDestination.CreateXYZ(mainViewModel.Document, this.Page, null, new float?(mainViewModel.Document.Pages[this.Page].Height), null);
						LinkAnnot.Link.Action = new PdfGoToAction(mainViewModel.Document, pdfDestination);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToWeb)
					{
						LinkAnnot.Link.Action = new PdfUriAction(mainViewModel.Document, linkEditWindows.UrlFilePath);
					}
					else if (linkEditWindows.SelectedType == LinkSelect.ToFile)
					{
						PdfFileSpecification pdfFileSpecification = new PdfFileSpecification(mainViewModel.Document);
						pdfFileSpecification.FileName = linkEditWindows.FileDiaoligFiePath;
						LinkAnnot.Link.Action = new PdfLaunchAction(mainViewModel.Document, pdfFileSpecification);
					}
					Color color = (Color)ColorConverter.ConvertFromString(linkEditWindows.SelectedFontground);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					float num;
					if (!linkEditWindows.rectangleVis)
					{
						num = 0f;
					}
					else
					{
						LinkAnnot.Color = fs_COLOR;
						num = linkEditWindows.BorderWidth;
					}
					float num2 = Math.Min(this.createStartPoint.X, this.createEndPoint.X);
					float num3 = Math.Max(this.createStartPoint.Y, this.createEndPoint.Y);
					float num4 = Math.Max(this.createStartPoint.X, this.createEndPoint.X);
					float num5 = Math.Min(this.createStartPoint.Y, this.createEndPoint.Y);
					LinkAnnot.Rectangle = new FS_RECTF(num2, num3, num4, num5);
					LinkAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					LinkAnnot.Flags |= AnnotationFlags.Print;
					PdfBorderStyle pdfBorderStyle = new PdfBorderStyle
					{
						Width = num,
						Style = linkEditWindows.BorderStyles,
						DashPattern = new float[] { 2f, 4f }
					};
					LinkAnnot.SetBorderStyle(pdfBorderStyle);
					page.Annots.Add(LinkAnnot);
					await requiredService.OperationManager.TraceAnnotationInsertAsync(LinkAnnot, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
					this.createStartPoint = default(FS_POINTF);
					this.createEndPoint = default(FS_POINTF);
					if (LinkAnnot != null)
					{
						return new PdfLinkAnnotation[] { LinkAnnot };
					}
					return null;
				}
				else
				{
					page = null;
				}
			}
			return null;
		}

		// Token: 0x0600283D RID: 10301 RVA: 0x000BCF8C File Offset: 0x000BB18C
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (page != base.CurrentPage)
			{
				return;
			}
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				double num = Math.Min(point.X, this.startPoint.X);
				double num2 = Math.Min(point.Y, this.startPoint.Y);
				double num3 = Math.Max(point.X, this.startPoint.X) - num;
				double num4 = Math.Max(point.Y, this.startPoint.Y) - num2;
				if (this.newSquareControl != null)
				{
					this.newSquareControl.Width = num3;
					this.newSquareControl.Height = num4;
					Canvas.SetLeft(this.newSquareControl, num);
					Canvas.SetTop(this.newSquareControl, num2);
				}
			}
		}

		// Token: 0x0600283E RID: 10302 RVA: 0x000BD06C File Offset: 0x000BB26C
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.startPoint = point;
				object dataContext = base.AnnotationCanvas.DataContext;
				Color color = (Color)ColorConverter.ConvertFromString("#000000");
				this.newSquareControl = new Rectangle
				{
					Stroke = new SolidColorBrush(color),
					StrokeThickness = 1.0
				};
				Canvas.SetLeft(this.newSquareControl, this.startPoint.X);
				Canvas.SetTop(this.newSquareControl, this.startPoint.Y);
				base.AnnotationCanvas.Children.Add(this.newSquareControl);
				return true;
			}
			return false;
		}

		// Token: 0x0600283F RID: 10303 RVA: 0x000BD139 File Offset: 0x000BB339
		public override bool OnPropertyChanged(string propertyName)
		{
			if (propertyName == "ShapeFill" || propertyName == "ShapeStroke" || propertyName == "ShapeThickness")
			{
				AnnotationDragEditControl annotationDragEditControl = this.editControl;
				return annotationDragEditControl != null && annotationDragEditControl.OnPropertyChanged(propertyName);
			}
			return false;
		}

		// Token: 0x06002840 RID: 10304 RVA: 0x000BD178 File Offset: 0x000BB378
		protected override bool OnSelecting(PdfLinkAnnotation annotation, bool afterCreate)
		{
			bool flag;
			try
			{
				this.editControl = new AnnotationDragEditControl(annotation, this);
				base.AnnotationCanvas.Children.Add(this.editControl);
				flag = true;
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x04001147 RID: 4423
		private Rectangle newSquareControl;

		// Token: 0x04001148 RID: 4424
		private FS_POINTF createStartPoint;

		// Token: 0x04001149 RID: 4425
		private FS_POINTF createEndPoint;

		// Token: 0x0400114A RID: 4426
		public Point startPoint;

		// Token: 0x0400114B RID: 4427
		private AnnotationDragEditControl editControl;

		// Token: 0x0400114C RID: 4428
		private int Page;

		// Token: 0x0400114D RID: 4429
		private string Filepath;

		// Token: 0x0400114E RID: 4430
		private string Linkurl;
	}
}
