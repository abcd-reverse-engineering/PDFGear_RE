using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Commets;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls
{
	// Token: 0x020001B2 RID: 434
	[TemplatePart(Name = "PART_CommetTreeView", Type = typeof(CommetTreeView))]
	public partial class CommetControl : Control
	{
		// Token: 0x060018C6 RID: 6342 RVA: 0x0005F994 File Offset: 0x0005DB94
		static CommetControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommetControl), new FrameworkPropertyMetadata(typeof(CommetControl)));
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x0005FA57 File Offset: 0x0005DC57
		public CommetControl()
		{
			base.IsVisibleChanged += this.CommetControl_IsVisibleChanged;
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x060018C8 RID: 6344 RVA: 0x0005FA71 File Offset: 0x0005DC71
		// (set) Token: 0x060018C9 RID: 6345 RVA: 0x0005FA7C File Offset: 0x0005DC7C
		private CommetTreeView CommetTreeView
		{
			get
			{
				return this.commetTreeView;
			}
			set
			{
				if (this.commetTreeView != value)
				{
					if (this.commetTreeView != null)
					{
						this.commetTreeView.ItemsSource = null;
						this.commetTreeView.SelectedItemChanged -= this.CommetTreeView_SelectedItemChanged;
					}
					this.commetTreeView = value;
					if (this.commetTreeView != null)
					{
						this.commetTreeView.ItemsSource = this.AllPageCommets;
						this.commetTreeView.SelectedItemChanged += this.CommetTreeView_SelectedItemChanged;
					}
				}
			}
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0005FAF4 File Offset: 0x0005DCF4
		private void CommetTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (!this.innerSet && this.Document != null)
			{
				CommetModel commetModel = e.NewValue as CommetModel;
				if (commetModel != null)
				{
					PdfPage pdfPage = this.Document.Pages[commetModel.Annotation.PageIndex];
					if (pdfPage.Annots == null)
					{
						pdfPage.CreateAnnotations();
					}
					if (commetModel.Annotation.AnnotIndex >= pdfPage.Annots.Count)
					{
						return;
					}
					PdfAnnotation selectAnnot = pdfPage.Annots[commetModel.Annotation.AnnotIndex];
					PdfMarkupAnnotation markup = selectAnnot as PdfMarkupAnnotation;
					if (markup != null)
					{
						if (markup.Relationship == RelationTypes.Reply)
						{
							return;
						}
						global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.Document);
						if (viewer != null)
						{
							MainViewModel mainViewModel = viewer.DataContext as MainViewModel;
							if (mainViewModel != null && !mainViewModel.IsAnnotationVisible)
							{
								mainViewModel.IsAnnotationVisible = true;
							}
							FS_RECTF rect = markup.GetRECT();
							bool flag = false;
							if (viewer.PageIndex != pdfPage.PageIndex)
							{
								flag = true;
								viewer.ScrollToPage(pdfPage.PageIndex);
							}
							float num;
							float num2;
							pdfPage.GetEffectiveSize(PageRotate.Normal, false).Deconstruct(out num, out num2);
							float num3 = num;
							float num4 = num2;
							if (rect.left < num3 && rect.right > 0f && rect.top > 0f && rect.bottom < num4)
							{
								if (flag)
								{
									base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
									{
										CommetControl <>4__this = this;
										global::PDFKit.PdfControl viewer2 = viewer;
										<>4__this.ScrollToAnnotation((viewer2 != null) ? viewer2.Viewer : null, markup);
									}));
								}
								else
								{
									global::PDFKit.PdfControl viewer3 = viewer;
									this.ScrollToAnnotation((viewer3 != null) ? viewer3.Viewer : null, markup);
								}
							}
						}
					}
					base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
					{
						try
						{
							this.SelectedAnnotation = selectAnnot;
						}
						catch
						{
						}
					}));
				}
			}
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x0005FCF8 File Offset: 0x0005DEF8
		private void ScrollToAnnotation(PdfViewer viewer, PdfMarkupAnnotation markup)
		{
			try
			{
				Rect deviceBounds = markup.GetDeviceBounds();
				if (deviceBounds.Left > viewer.ActualWidth || deviceBounds.Right < 0.0)
				{
					if (deviceBounds.Left > viewer.ActualWidth)
					{
						viewer.ScrollOwner.ScrollToHorizontalOffset(viewer.ScrollOwner.HorizontalOffset + (deviceBounds.Right - viewer.ActualWidth));
					}
					else
					{
						viewer.ScrollOwner.ScrollToHorizontalOffset(viewer.ScrollOwner.HorizontalOffset + deviceBounds.Left);
					}
				}
				if (deviceBounds.Top > viewer.ActualHeight || deviceBounds.Bottom < 0.0)
				{
					if (deviceBounds.Top > viewer.ActualHeight)
					{
						viewer.ScrollOwner.ScrollToVerticalOffset(viewer.ScrollOwner.VerticalOffset + (deviceBounds.Bottom - viewer.ActualHeight));
					}
					else
					{
						viewer.ScrollOwner.ScrollToVerticalOffset(viewer.ScrollOwner.VerticalOffset + deviceBounds.Top);
					}
				}
			}
			catch
			{
				PdfPage page = markup.Page;
				if (viewer.CurrentIndex != page.PageIndex)
				{
					viewer.ScrollToPage(page.PageIndex);
				}
			}
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0005FE2C File Offset: 0x0005E02C
		public void ExpandAll()
		{
			if (this.AllPageCommets != null)
			{
				PdfViewer pdfViewer = null;
				if (base.IsVisible && this.Document != null)
				{
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
					pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
				}
				int num = ((pdfViewer != null) ? pdfViewer.CurrentIndex : (-1));
				PageCommetCollection pageCommetCollection = null;
				bool flag = false;
				foreach (PageCommetCollection pageCommetCollection2 in this.AllPageCommets)
				{
					pageCommetCollection2.IsExpanded = true;
					if (!flag)
					{
						if (pageCommetCollection2.PageIndex <= num)
						{
							pageCommetCollection = pageCommetCollection2;
						}
						else
						{
							flag = true;
						}
					}
				}
				CommetModel commetModel = this.CommetTreeView.SelectedItem as CommetModel;
				if (commetModel != null)
				{
					this.CommetTreeView.ScrollIntoViewAsync(commetModel, ScrollIntoViewOrientation.Both);
					return;
				}
				if (pageCommetCollection != null && this.CommetTreeView != null && base.IsVisible)
				{
					this.CommetTreeView.ScrollIntoViewAsync(pageCommetCollection, ScrollIntoViewOrientation.Both);
				}
			}
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0005FF24 File Offset: 0x0005E124
		public void CollapseAll()
		{
			this.SelectedAnnotation = null;
			if (this.CommetTreeView != null)
			{
				foreach (object obj in ((IEnumerable)this.CommetTreeView.Items))
				{
					ItemContainerGenerator itemContainerGenerator = this.CommetTreeView.ItemContainerGenerator;
					TreeViewItem treeViewItem = ((itemContainerGenerator != null) ? itemContainerGenerator.ContainerFromItem(obj) : null) as TreeViewItem;
					if (treeViewItem != null)
					{
						treeViewItem.IsExpanded = false;
					}
				}
			}
			if (this.AllPageCommets != null)
			{
				PdfViewer pdfViewer = null;
				if (base.IsVisible && this.Document != null)
				{
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
					pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
				}
				int num = ((pdfViewer != null) ? pdfViewer.CurrentIndex : (-1));
				PageCommetCollection pageCommetCollection = null;
				bool flag = false;
				foreach (PageCommetCollection pageCommetCollection2 in this.AllPageCommets)
				{
					pageCommetCollection2.IsExpanded = false;
					if (!flag)
					{
						if (pageCommetCollection2.PageIndex <= num)
						{
							pageCommetCollection = pageCommetCollection2;
						}
						else
						{
							flag = true;
						}
					}
				}
				if (pageCommetCollection != null && this.CommetTreeView != null && base.IsVisible)
				{
					CommetControl.<CollapseAll>g__ScrollIntoViewAsync|11_0(this.CommetTreeView, pageCommetCollection);
				}
			}
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x00060078 File Offset: 0x0005E278
		private void CommetControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				this.SyncSelectedAnnotation();
			}
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00060088 File Offset: 0x0005E288
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.CommetTreeView = base.GetTemplateChild("PART_CommetTreeView") as CommetTreeView;
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000600A6 File Offset: 0x0005E2A6
		// (set) Token: 0x060018D1 RID: 6353 RVA: 0x000600B8 File Offset: 0x0005E2B8
		public AllPageCommetCollectionView AllPageCommets
		{
			get
			{
				return (AllPageCommetCollectionView)base.GetValue(CommetControl.AllPageCommetsProperty);
			}
			set
			{
				base.SetValue(CommetControl.AllPageCommetsProperty, value);
			}
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x000600C8 File Offset: 0x0005E2C8
		private static void OnAllPageCommetsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				CommetControl commetControl = d as CommetControl;
				if (commetControl != null && commetControl.CommetTreeView != null)
				{
					commetControl.CommetTreeView.ItemsSource = e.NewValue as IEnumerable;
				}
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x060018D3 RID: 6355 RVA: 0x0006010E File Offset: 0x0005E30E
		// (set) Token: 0x060018D4 RID: 6356 RVA: 0x00060120 File Offset: 0x0005E320
		public PdfDocument Document
		{
			get
			{
				return (PdfDocument)base.GetValue(CommetControl.DocumentProperty);
			}
			set
			{
				base.SetValue(CommetControl.DocumentProperty, value);
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x060018D5 RID: 6357 RVA: 0x0006012E File Offset: 0x0005E32E
		// (set) Token: 0x060018D6 RID: 6358 RVA: 0x00060140 File Offset: 0x0005E340
		public PdfAnnotation SelectedAnnotation
		{
			get
			{
				return (PdfAnnotation)base.GetValue(CommetControl.SelectedAnnotationProperty);
			}
			set
			{
				base.SetValue(CommetControl.SelectedAnnotationProperty, value);
			}
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x00060150 File Offset: 0x0005E350
		private static void OnSelectedAnnotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				CommetControl commetControl = d as CommetControl;
				if (commetControl != null)
				{
					commetControl.SyncSelectedAnnotation();
				}
			}
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x00060180 File Offset: 0x0005E380
		private void SyncSelectedAnnotation()
		{
			if (this.AllPageCommets != null)
			{
				this.innerSet = true;
				try
				{
					bool flag = false;
					PdfAnnotation selectedAnnotation = this.SelectedAnnotation;
					if (selectedAnnotation != null)
					{
						foreach (PageCommetCollection pageCommetCollection in this.AllPageCommets)
						{
							if (pageCommetCollection.PageIndex == selectedAnnotation.Page.PageIndex)
							{
								CommetControl.<>c__DisplayClass28_0 CS$<>8__locals1 = new CommetControl.<>c__DisplayClass28_0();
								CS$<>8__locals1.<>4__this = this;
								CS$<>8__locals1.idx = selectedAnnotation.Page.Annots.IndexOf(selectedAnnotation);
								CS$<>8__locals1.model = pageCommetCollection.FirstOrDefault((CommetModel c) => c.Annotation.AnnotIndex == CS$<>8__locals1.idx);
								if (CS$<>8__locals1.model != null)
								{
									CS$<>8__locals1.model.IsSelected = true;
									flag = true;
									if (this.CommetTreeView != null)
									{
										base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
										{
											CommetControl.<>c__DisplayClass28_0.<<SyncSelectedAnnotation>b__1>d <<SyncSelectedAnnotation>b__1>d;
											<<SyncSelectedAnnotation>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
											<<SyncSelectedAnnotation>b__1>d.<>4__this = CS$<>8__locals1;
											<<SyncSelectedAnnotation>b__1>d.<>1__state = -1;
											<<SyncSelectedAnnotation>b__1>d.<>t__builder.Start<CommetControl.<>c__DisplayClass28_0.<<SyncSelectedAnnotation>b__1>d>(ref <<SyncSelectedAnnotation>b__1>d);
										}));
										break;
									}
									break;
								}
							}
						}
					}
					if (!flag)
					{
						CommetTreeView commetTreeView = this.CommetTreeView;
						CommetModel commetModel = ((commetTreeView != null) ? commetTreeView.SelectedItem : null) as CommetModel;
						if (commetModel != null)
						{
							commetModel.IsSelected = false;
						}
					}
				}
				finally
				{
					this.innerSet = false;
				}
			}
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x000602BC File Offset: 0x0005E4BC
		[CompilerGenerated]
		internal static async void <CollapseAll>g__ScrollIntoViewAsync|11_0(TreeView view, PageCommetCollection item)
		{
			await view.ScrollIntoViewAsync(item, ScrollIntoViewOrientation.Both);
			item.IsExpanded = false;
		}

		// Token: 0x04000852 RID: 2130
		private const string CommetTreeViewName = "PART_CommetTreeView";

		// Token: 0x04000853 RID: 2131
		private bool innerSet;

		// Token: 0x04000854 RID: 2132
		private CommetTreeView commetTreeView;

		// Token: 0x04000855 RID: 2133
		public static readonly DependencyProperty AllPageCommetsProperty = DependencyProperty.Register("AllPageCommets", typeof(AllPageCommetCollectionView), typeof(CommetControl), new PropertyMetadata(null, new PropertyChangedCallback(CommetControl.OnAllPageCommetsPropertyChanged)));

		// Token: 0x04000856 RID: 2134
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(PdfDocument), typeof(CommetControl), new PropertyMetadata(null));

		// Token: 0x04000857 RID: 2135
		public static readonly DependencyProperty SelectedAnnotationProperty = DependencyProperty.Register("SelectedAnnotation", typeof(PdfAnnotation), typeof(CommetControl), new PropertyMetadata(null, new PropertyChangedCallback(CommetControl.OnSelectedAnnotationPropertyChanged)));
	}
}
