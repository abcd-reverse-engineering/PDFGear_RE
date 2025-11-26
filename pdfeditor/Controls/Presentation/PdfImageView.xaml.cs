using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using Patagames.Pdf.Net;
using PDFKit;

namespace pdfeditor.Controls.Presentation
{
	// Token: 0x02000232 RID: 562
	public partial class PdfImageView : UserControl
	{
		// Token: 0x06001FC9 RID: 8137 RVA: 0x0008F898 File Offset: 0x0008DA98
		public PdfImageView()
		{
			this.InitializeComponent();
			base.Loaded += this.PdfImageView_Loaded;
			base.Unloaded += this.PdfImageView_Unloaded;
			base.SizeChanged += this.PdfImageView_SizeChanged;
			base.IsVisibleChanged += this.PdfImageView_IsVisibleChanged;
		}

		// Token: 0x06001FCA RID: 8138 RVA: 0x0008F8F9 File Offset: 0x0008DAF9
		private void PdfImageView_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdateImage();
		}

		// Token: 0x06001FCB RID: 8139 RVA: 0x0008F901 File Offset: 0x0008DB01
		private void PdfImageView_Unloaded(object sender, RoutedEventArgs e)
		{
			this.UpdateImage();
			this.cts = null;
		}

		// Token: 0x06001FCC RID: 8140 RVA: 0x0008F910 File Offset: 0x0008DB10
		private void PdfImageView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateImage();
		}

		// Token: 0x06001FCD RID: 8141 RVA: 0x0008F918 File Offset: 0x0008DB18
		private async void PdfImageView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			try
			{
				int num = 20;
				CancellationTokenSource cancellationTokenSource = this.cts;
				await Task.Delay(num, (cancellationTokenSource != null) ? cancellationTokenSource.Token : default(CancellationToken));
				this.UpdateImage();
			}
			catch (OperationCanceledException)
			{
			}
		}

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06001FCE RID: 8142 RVA: 0x0008F94F File Offset: 0x0008DB4F
		// (set) Token: 0x06001FCF RID: 8143 RVA: 0x0008F961 File Offset: 0x0008DB61
		public PdfDocument Document
		{
			get
			{
				return (PdfDocument)base.GetValue(PdfImageView.DocumentProperty);
			}
			set
			{
				base.SetValue(PdfImageView.DocumentProperty, value);
			}
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x0008F970 File Offset: 0x0008DB70
		private static void OnDocumentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				PdfImageView pdfImageView = d as PdfImageView;
				if (pdfImageView != null)
				{
					pdfImageView.UpdateImage();
				}
			}
		}

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x0008F99D File Offset: 0x0008DB9D
		// (set) Token: 0x06001FD2 RID: 8146 RVA: 0x0008F9AF File Offset: 0x0008DBAF
		public int PageIndex
		{
			get
			{
				return (int)base.GetValue(PdfImageView.PageIndexProperty);
			}
			set
			{
				base.SetValue(PdfImageView.PageIndexProperty, value);
			}
		}

		// Token: 0x06001FD3 RID: 8147 RVA: 0x0008F9C4 File Offset: 0x0008DBC4
		private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				PdfImageView pdfImageView = d as PdfImageView;
				if (pdfImageView != null)
				{
					pdfImageView.UpdateImage();
				}
			}
		}

		// Token: 0x06001FD4 RID: 8148 RVA: 0x0008F9F8 File Offset: 0x0008DBF8
		private async void UpdateImage()
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this.cts = new CancellationTokenSource();
			if (!base.IsLoaded || !base.IsVisible)
			{
				this.MainImage.Source = null;
			}
			else
			{
				try
				{
					WriteableBitmap writeableBitmap = await this.GetPdfImageAsync(this.PageIndex);
					if (writeableBitmap != null)
					{
						this.MainImage.Source = writeableBitmap;
					}
				}
				catch (OperationCanceledException)
				{
				}
			}
		}

		// Token: 0x06001FD5 RID: 8149 RVA: 0x0008FA30 File Offset: 0x0008DC30
		private async Task<WriteableBitmap> GetPdfImageAsync(int pageIndex)
		{
			PdfImageView.<>c__DisplayClass17_0 CS$<>8__locals1 = new PdfImageView.<>c__DisplayClass17_0();
			CS$<>8__locals1.pageIndex = pageIndex;
			WriteableBitmap writeableBitmap;
			if (CS$<>8__locals1.pageIndex < 0 || this.Document == null || CS$<>8__locals1.pageIndex >= this.Document.Pages.Count)
			{
				writeableBitmap = null;
			}
			else if (!base.IsLoaded || !base.IsVisible || this.cts == null)
			{
				writeableBitmap = null;
			}
			else
			{
				CS$<>8__locals1.token = this.cts.Token;
				double actualWidth = base.ActualWidth;
				double actualHeight = base.ActualHeight;
				CS$<>8__locals1.dpiScale = VisualTreeHelper.GetDpi(this);
				if (actualWidth <= 1.0 || actualHeight <= 1.0)
				{
					writeableBitmap = null;
				}
				else
				{
					CS$<>8__locals1.widthPixel = actualWidth * CS$<>8__locals1.dpiScale.PixelsPerDip;
					CS$<>8__locals1.heightPixel = actualHeight * CS$<>8__locals1.dpiScale.PixelsPerDip;
					CS$<>8__locals1.document = this.Document;
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(CS$<>8__locals1.document);
					CS$<>8__locals1.showAnnot = pdfControl == null || pdfControl.IsAnnotationVisible;
					writeableBitmap = await Task.Run<WriteableBitmap>(TaskExceptionHelper.ExceptionBoundary<WriteableBitmap>(delegate
					{
						PdfImageView.<>c__DisplayClass17_0.<<GetPdfImageAsync>b__0>d <<GetPdfImageAsync>b__0>d;
						<<GetPdfImageAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<WriteableBitmap>.Create();
						<<GetPdfImageAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<GetPdfImageAsync>b__0>d.<>1__state = -1;
						<<GetPdfImageAsync>b__0>d.<>t__builder.Start<PdfImageView.<>c__DisplayClass17_0.<<GetPdfImageAsync>b__0>d>(ref <<GetPdfImageAsync>b__0>d);
						return <<GetPdfImageAsync>b__0>d.<>t__builder.Task;
					}), CS$<>8__locals1.token);
				}
			}
			return writeableBitmap;
		}

		// Token: 0x04000CBC RID: 3260
		private CancellationTokenSource cts;

		// Token: 0x04000CBD RID: 3261
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(PdfDocument), typeof(PdfImageView), new PropertyMetadata(null, new PropertyChangedCallback(PdfImageView.OnDocumentPropertyChanged)));

		// Token: 0x04000CBE RID: 3262
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(PdfImageView), new PropertyMetadata(-1, new PropertyChangedCallback(PdfImageView.OnPageIndexPropertyChanged)));
	}
}
