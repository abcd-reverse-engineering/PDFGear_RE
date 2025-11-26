using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.Signature;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A8 RID: 680
	public partial class SignatureDragControl : UserControl, IAnnotationControl<PdfStampAnnotation>, IAnnotationControl
	{
		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x06002722 RID: 10018 RVA: 0x000B90E8 File Offset: 0x000B72E8
		// (set) Token: 0x06002723 RID: 10019 RVA: 0x000B90FA File Offset: 0x000B72FA
		public bool IsApply
		{
			get
			{
				return (bool)base.GetValue(SignatureDragControl.IsApplyProperty);
			}
			set
			{
				base.SetValue(SignatureDragControl.IsApplyProperty, value);
			}
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000B910D File Offset: 0x000B730D
		public SignatureDragControl(PdfStampAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.Annotation = annot;
			this.Holder = holder;
			SignatureDragControl.ApplySignatures = new List<PdfStampAnnotation>();
			base.Loaded += this.SignatureDragControl_Loaded;
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000B9148 File Offset: 0x000B7348
		private void SignatureDragControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.OnPageClientBoundsChanged();
			if (this.Annotation.Dictionary.ContainsKey("ApplyId"))
			{
				this.IsApply = this.Annotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString.Trim().Length > 0;
			}
		}

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06002726 RID: 10022 RVA: 0x000B91A5 File Offset: 0x000B73A5
		public PdfStampAnnotation Annotation { get; }

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06002727 RID: 10023 RVA: 0x000B91AD File Offset: 0x000B73AD
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06002728 RID: 10024 RVA: 0x000B91B5 File Offset: 0x000B73B5
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06002729 RID: 10025 RVA: 0x000B91C2 File Offset: 0x000B73C2
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x000B91CC File Offset: 0x000B73CC
		public void OnPageClientBoundsChanged()
		{
			object dataContext = base.DataContext;
			Rect rect;
			if (!this.ParentCanvas.PdfViewer.TryGetClientRect(this.Annotation.Page.PageIndex, this.Annotation.GetRECT(), out rect))
			{
				return;
			}
			this.AnnotationDrag.Width = rect.Width;
			this.AnnotationDrag.Height = rect.Height;
			this.LayoutRoot.Width = rect.Width;
			this.LayoutRoot.Height = rect.Height;
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			this.ResetDraggers();
		}

		// Token: 0x0600272B RID: 10027 RVA: 0x000B9278 File Offset: 0x000B7478
		public bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x0600272C RID: 10028 RVA: 0x000B927B File Offset: 0x000B747B
		private void ResetDraggers()
		{
			this.DragResizeView.Width = this.LayoutRoot.ActualWidth;
			this.DragResizeView.Height = this.LayoutRoot.ActualHeight;
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x000B92AC File Offset: 0x000B74AC
		private void ResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			if (e.Operation == ResizeViewOperation.Move)
			{
				this.DragResizeView.DragPlaceholderFill = new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(51, 0, 122, 204));
				this.DragResizeView.BorderBrush = global::System.Windows.Media.Brushes.Transparent;
				return;
			}
			this.DragResizeView.DragPlaceholderFill = global::System.Windows.Media.Brushes.Transparent;
			this.DragResizeView.BorderBrush = new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(51, 0, 122, 204));
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x000B9324 File Offset: 0x000B7524
		private async void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			this.DragResizeView.BorderBrush = global::System.Windows.Media.Brushes.Transparent;
			Canvas.SetLeft(this.AnnotationDrag, 0.0);
			Canvas.SetTop(this.AnnotationDrag, 0.0);
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel != null)
			{
				double num = Canvas.GetLeft(this);
				double num2 = Canvas.GetTop(this);
				this.LayoutRoot.Width = e.NewSize.Width;
				this.LayoutRoot.Height = e.NewSize.Height;
				num += e.OffsetX;
				num2 += e.OffsetY;
				Canvas.SetLeft(this, num);
				Canvas.SetTop(this, num2);
				bool pdfViewer = this.ParentCanvas.PdfViewer != null;
				PdfStampAnnotation annotation = this.Annotation;
				PdfPage pdfPage = ((annotation != null) ? annotation.Page : null);
				if (pdfViewer && pdfPage != null)
				{
					PdfPageObjectsCollection normalAppearance = this.Annotation.NormalAppearance;
					PdfImageObject pdfImageObject = ((normalAppearance != null) ? normalAppearance.OfType<PdfImageObject>().FirstOrDefault<PdfImageObject>() : null);
					FS_RECTF? newRect = this.GetNewRect();
					if (newRect != null)
					{
						PdfTypeArray pdfTypeArray = PdfTypeArray.Create();
						pdfTypeArray.AddReal(newRect.Value.Width);
						pdfTypeArray.AddReal(newRect.Value.Height);
						this.Annotation.Dictionary["ChangeSize"] = pdfTypeArray;
						using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
						{
							this.Annotation.Opacity = 1f;
							this.Annotation.Rectangle = newRect.Value;
						}
					}
					mainViewModel.MaybeHaveUnembeddedSignature = true;
					if (pdfImageObject != null)
					{
						await pdfPage.TryRedrawPageAsync(default(CancellationToken));
						base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
						{
							await Task.Delay(10);
							if (this.Holder.State == AnnotationHolderState.None)
							{
								this.Holder.Select(this.Annotation, false);
							}
						}));
					}
					else
					{
						this.Annotation.TryRedrawAnnotation(false);
						this.OnPageClientBoundsChanged();
					}
				}
			}
		}

		// Token: 0x0600272F RID: 10031 RVA: 0x000B9364 File Offset: 0x000B7564
		public static WriteableBitmap BitmapToWriteableBitmap(Bitmap src, int newwidth, int newheight)
		{
			WriteableBitmap writeableBitmap = SignatureDragControl.CreateCompatibleWriteableBitmap(src);
			global::System.Drawing.Imaging.PixelFormat pixelFormat = src.PixelFormat;
			if (writeableBitmap == null)
			{
				writeableBitmap = new WriteableBitmap(newwidth, newheight, 96.0, 96.0, PixelFormats.Bgra32, null);
				pixelFormat = global::System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			}
			SignatureDragControl.BitmapCopyToWriteableBitmap(src, writeableBitmap, new global::System.Drawing.Rectangle(0, 0, newwidth, newheight), 0, 0, pixelFormat);
			return writeableBitmap;
		}

		// Token: 0x06002730 RID: 10032 RVA: 0x000B93BC File Offset: 0x000B75BC
		public static WriteableBitmap CreateCompatibleWriteableBitmap(Bitmap src)
		{
			global::System.Drawing.Imaging.PixelFormat pixelFormat = src.PixelFormat;
			global::System.Windows.Media.PixelFormat pixelFormat2;
			if (pixelFormat <= global::System.Drawing.Imaging.PixelFormat.Format24bppRgb)
			{
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
				{
					pixelFormat2 = PixelFormats.Bgr555;
					goto IL_0075;
				}
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format16bppRgb565)
				{
					pixelFormat2 = PixelFormats.Bgr565;
					goto IL_0075;
				}
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format24bppRgb)
				{
					pixelFormat2 = PixelFormats.Bgr24;
					goto IL_0075;
				}
			}
			else
			{
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format32bppRgb)
				{
					pixelFormat2 = PixelFormats.Bgr32;
					goto IL_0075;
				}
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
				{
					pixelFormat2 = PixelFormats.Pbgra32;
					goto IL_0075;
				}
				if (pixelFormat == global::System.Drawing.Imaging.PixelFormat.Format32bppArgb)
				{
					pixelFormat2 = PixelFormats.Bgra32;
					goto IL_0075;
				}
			}
			return null;
			IL_0075:
			return new WriteableBitmap(src.Width, src.Height, 0.0, 0.0, pixelFormat2, null);
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x000B9464 File Offset: 0x000B7664
		public static void BitmapCopyToWriteableBitmap(Bitmap src, WriteableBitmap dst, global::System.Drawing.Rectangle srcRect, int destinationX, int destinationY, global::System.Drawing.Imaging.PixelFormat srcPixelFormat)
		{
			BitmapData bitmapData = src.LockBits(new global::System.Drawing.Rectangle(new global::System.Drawing.Point(0, 0), src.Size), ImageLockMode.ReadOnly, srcPixelFormat);
			dst.WritePixels(new Int32Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride, destinationX, destinationY);
			src.UnlockBits(bitmapData);
		}

		// Token: 0x06002732 RID: 10034 RVA: 0x000B94D8 File Offset: 0x000B76D8
		public static BitmapSource GetBitmapSource(Bitmap bmp)
		{
			BitmapFrame bitmapFrame = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bmp.Save(memoryStream, ImageFormat.Png);
				bitmapFrame = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
			}
			return bitmapFrame;
		}

		// Token: 0x06002733 RID: 10035 RVA: 0x000B9520 File Offset: 0x000B7720
		private FS_RECTF? GetNewRect()
		{
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			PdfViewer pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			double width = this.LayoutRoot.Width;
			double height = this.LayoutRoot.Height;
			if (width == 0.0 || height == 0.0)
			{
				return null;
			}
			FS_RECTF fs_RECTF;
			if (pdfViewer.TryGetPageRect(this.Annotation.Page.PageIndex, new Rect(left, top, width, height), out fs_RECTF))
			{
				return new FS_RECTF?(fs_RECTF);
			}
			return null;
		}

		// Token: 0x06002734 RID: 10036 RVA: 0x000B95C1 File Offset: 0x000B77C1
		private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResetDraggers();
		}

		// Token: 0x06002735 RID: 10037 RVA: 0x000B95CC File Offset: 0x000B77CC
		private async void Btn_Embed_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfStampAnnotationSignature", "Flatten", "Begin", 1L);
			SignatureEmbedConfirmWin signatureEmbedConfirmWin = new SignatureEmbedConfirmWin(EmbedType.Single);
			signatureEmbedConfirmWin.ShowDialog();
			bool? dialogResult = signatureEmbedConfirmWin.DialogResult;
			bool flag = false;
			if (!((dialogResult.GetValueOrDefault() == flag) & (dialogResult != null)))
			{
				try
				{
					GAManager.SendEvent("PdfStampAnnotationSignature", "Flatten", "Start", 1L);
					MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
					global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(vm.Document);
					PdfObjectExtensions.GetAnnotationHolderManager(viewer);
					this.Annotation.Dictionary["Embed"] = PdfTypeBoolean.Create(true);
					this.Annotation.DeleteAnnotation();
					PageEditorViewModel pageEditors = vm.PageEditors;
					if (pageEditors != null)
					{
						pageEditors.NotifyPageAnnotationChanged(this.Annotation.Page.PageIndex);
					}
					await StampUtil.FlattenAnnotationAsync(this.Annotation);
					vm.SetCanSaveFlag("FlattenSignature", true);
					await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
					GAManager.SendEvent("PdfStampAnnotationSignature", "Flatten", "Done", 1L);
					vm = null;
					viewer = null;
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06002736 RID: 10038 RVA: 0x000B9604 File Offset: 0x000B7804
		private async void Btn_Embed_InBatch_Click(object sender, RoutedEventArgs e)
		{
			SignatureDragControl.<>c__DisplayClass29_0 CS$<>8__locals1 = new SignatureDragControl.<>c__DisplayClass29_0();
			CS$<>8__locals1.<>4__this = this;
			GAManager.SendEvent("PdfStampAnnotationSignature", "BatchFlatten", "Begin", 1L);
			SignatureEmbedConfirmWin signatureEmbedConfirmWin = new SignatureEmbedConfirmWin(EmbedType.InBatch);
			signatureEmbedConfirmWin.ShowDialog();
			bool? dialogResult = signatureEmbedConfirmWin.DialogResult;
			bool flag = false;
			if (!((dialogResult.GetValueOrDefault() == flag) & (dialogResult != null)))
			{
				try
				{
					if (this.Annotation.Dictionary.ContainsKey("ApplyRange") && this.Annotation.Dictionary.ContainsKey("ApplyId"))
					{
						GAManager.SendEvent("PdfStampAnnotationSignature", "BatchFlatten", "Start", 1L);
						PdfTypeBase[] array = this.Annotation.Dictionary["ApplyRange"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>();
						CS$<>8__locals1.applyId = this.Annotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString;
						CS$<>8__locals1.pageIndex = new int[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							CS$<>8__locals1.pageIndex[i] = (array[i] as PdfTypeNumber).IntValue;
						}
						CS$<>8__locals1.vm = Ioc.Default.GetRequiredService<MainViewModel>();
						if (this.Annotation.Dictionary.ContainsKey("ImgSource"))
						{
							string unicodeString = this.Annotation.Dictionary["ImgSource"].As<PdfTypeString>(true).UnicodeString;
						}
						ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
						{
							SignatureDragControl.<>c__DisplayClass29_0.<<Btn_Embed_InBatch_Click>b__0>d <<Btn_Embed_InBatch_Click>b__0>d;
							<<Btn_Embed_InBatch_Click>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<Btn_Embed_InBatch_Click>b__0>d.<>4__this = CS$<>8__locals1;
							<<Btn_Embed_InBatch_Click>b__0>d.c = c;
							<<Btn_Embed_InBatch_Click>b__0>d.<>1__state = -1;
							<<Btn_Embed_InBatch_Click>b__0>d.<>t__builder.Start<SignatureDragControl.<>c__DisplayClass29_0.<<Btn_Embed_InBatch_Click>b__0>d>(ref <<Btn_Embed_InBatch_Click>b__0>d);
							return <<Btn_Embed_InBatch_Click>b__0>d.<>t__builder.Task;
						}, null, pdfeditor.Properties.Resources.WinSignatureFlattenProcess, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
						global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(CS$<>8__locals1.vm.Document);
						CS$<>8__locals1.vm.SetCanSaveFlag("FlattenSignature", true);
						await this.Annotation.Page.TryRedrawPageAsync(default(CancellationToken));
						await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
						GAManager.SendEvent("PdfStampAnnotationSignature", "BatchFlatten", "Done", 1L);
						viewer = null;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06002737 RID: 10039 RVA: 0x000B963C File Offset: 0x000B783C
		private async void Btn_Delete_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfStampAnnotationSignature", "Delete", "Count", 1L);
			await PdfObjectExtensions.GetAnnotationHolderManager(global::PDFKit.PdfControl.GetPdfControl(Ioc.Default.GetRequiredService<MainViewModel>().Document)).DeleteAnnotationAsync(this.Annotation, false);
		}

		// Token: 0x06002738 RID: 10040 RVA: 0x000B9674 File Offset: 0x000B7874
		private async void Btn_Delete_InBatch_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfStampAnnotationSignature", "BatchDelete", "Count", 1L);
			SignatureClearConfirmWin signatureClearConfirmWin = new SignatureClearConfirmWin();
			signatureClearConfirmWin.ShowDialog();
			bool? dialogResult = signatureClearConfirmWin.DialogResult;
			bool flag = false;
			if (!((dialogResult.GetValueOrDefault() == flag) & (dialogResult != null)))
			{
				MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
				await PdfObjectExtensions.GetAnnotationHolderManager(global::PDFKit.PdfControl.GetPdfControl(vm.Document)).DeleteAnnotationAsync(this.Annotation, false);
				if (this.Annotation.Dictionary.ContainsKey("ApplyRange") && this.Annotation.Dictionary.ContainsKey("ApplyId"))
				{
					PdfTypeBase[] array = this.Annotation.Dictionary["ApplyRange"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>();
					string applyId = this.Annotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString;
					int[] pageIndex = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						pageIndex[i] = (array[i] as PdfTypeNumber).IntValue;
					}
					List<PdfStampAnnotation> list = this.TraceRemoveAll(pageIndex);
					if (list != null && list.Count > 0)
					{
						await vm.OperationManager.TraceAnnotationRemoveAsync(list, "");
					}
					await this.ReomveSignatureAsync(vm.Document, pageIndex, applyId);
					applyId = null;
					pageIndex = null;
				}
			}
		}

		// Token: 0x06002739 RID: 10041 RVA: 0x000B96AC File Offset: 0x000B78AC
		private List<PdfStampAnnotation> TraceRemoveAll(int[] pageIndexs)
		{
			PdfDocument document = Ioc.Default.GetRequiredService<MainViewModel>().Document;
			List<PdfStampAnnotation> list = new List<PdfStampAnnotation>();
			foreach (int num in pageIndexs)
			{
				IntPtr intPtr = IntPtr.Zero;
				intPtr = Pdfium.FPDF_LoadPage(document.Handle, num);
				if (intPtr != IntPtr.Zero)
				{
					PdfAnnotationCollection annots = PdfPage.FromHandle(document, intPtr, num, true).Annots;
					List<PdfStampAnnotation> list2 = ((annots != null) ? annots.OfType<PdfStampAnnotation>().ToList<PdfStampAnnotation>() : null);
					if (list2 != null && list2.Count > 0)
					{
						list.AddRange(list2);
					}
				}
			}
			return list;
		}

		// Token: 0x0600273A RID: 10042 RVA: 0x000B9740 File Offset: 0x000B7940
		private async void Btn_Apply_Click(object sender, RoutedEventArgs e)
		{
			SignatureDragControl.<>c__DisplayClass33_0 CS$<>8__locals1 = new SignatureDragControl.<>c__DisplayClass33_0();
			CS$<>8__locals1.<>4__this = this;
			GAManager.SendEvent("PdfStampAnnotationSignature", "ApplyMultiPages", "Begin", 1L);
			SignatureApplyPageWin signatureApplyPageWin = new SignatureApplyPageWin(this.Annotation.Page.PageIndex);
			if (signatureApplyPageWin.ShowDialog().GetValueOrDefault() && signatureApplyPageWin.ApplyPageIndex != null)
			{
				CS$<>8__locals1.vm = Ioc.Default.GetRequiredService<MainViewModel>();
				CS$<>8__locals1.pageIndexs = signatureApplyPageWin.ApplyPageIndex;
				CS$<>8__locals1.rangeArr = PdfTypeArray.Create();
				CS$<>8__locals1.pageIndexs.ToList<int>().ForEach(delegate(int i)
				{
					CS$<>8__locals1.rangeArr.Add(PdfTypeNumber.Create(i));
				});
				IAnnotationHolder currentHolder = this.Holder.AnnotationCanvas.HolderManager.CurrentHolder;
				CS$<>8__locals1.imgSource = (this.Annotation.Dictionary.ContainsKey("ImgSource") ? this.Annotation.Dictionary["ImgSource"].As<PdfTypeString>(true).UnicodeString : string.Empty);
				CS$<>8__locals1.imgSource2 = null;
				CS$<>8__locals1.isRemoveBg = this.Annotation.Dictionary.ContainsKey("IsRemoveBg") && this.Annotation.Dictionary["IsRemoveBg"].As<PdfTypeBoolean>(true).Value;
				PdfTypeBase[] array = (this.Annotation.Dictionary.ContainsKey("ChangeSize") ? this.Annotation.Dictionary["ChangeSize"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>() : null);
				CS$<>8__locals1.changeSize2 = default(global::System.Windows.Size);
				if (array != null)
				{
					float[] array2 = new float[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = (array[j] as PdfTypeNumber).FloatValue;
					}
					if (array2.Length == 2)
					{
						CS$<>8__locals1.changeSize2 = new global::System.Windows.Size((double)array2[0], (double)array2[1]);
					}
				}
				this.Annotation.Dictionary["ApplyRange"] = CS$<>8__locals1.rangeArr;
				CS$<>8__locals1.applyId = Guid.NewGuid().ToString().ToLower();
				this.Annotation.Dictionary["ApplyId"] = PdfTypeString.Create(CS$<>8__locals1.applyId, false, false);
				this.IsApply = true;
				GAManager.SendEvent("PdfStampAnnotationSignature", "ApplyMultiPages", "Start", 1L);
				ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
				{
					SignatureDragControl.<>c__DisplayClass33_0.<<Btn_Apply_Click>b__1>d <<Btn_Apply_Click>b__1>d;
					<<Btn_Apply_Click>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<Btn_Apply_Click>b__1>d.<>4__this = CS$<>8__locals1;
					<<Btn_Apply_Click>b__1>d.c = c;
					<<Btn_Apply_Click>b__1>d.<>1__state = -1;
					<<Btn_Apply_Click>b__1>d.<>t__builder.Start<SignatureDragControl.<>c__DisplayClass33_0.<<Btn_Apply_Click>b__1>d>(ref <<Btn_Apply_Click>b__1>d);
					return <<Btn_Apply_Click>b__1>d.<>t__builder.Task;
				}, null, pdfeditor.Properties.Resources.WinSignatureFlattenApplying, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
				await global::PDFKit.PdfControl.GetPdfControl(CS$<>8__locals1.vm.Document).TryRedrawVisiblePageAsync(default(CancellationToken));
				if (SignatureDragControl.ApplySignatures.Count > 0)
				{
					await CS$<>8__locals1.vm.OperationManager.TraceAnnotationInsertAsync(SignatureDragControl.ApplySignatures, "");
				}
				GAManager.SendEvent("PdfStampAnnotationSignature", "ApplyMultiPages", "Done", 1L);
			}
		}

		// Token: 0x0600273B RID: 10043 RVA: 0x000B9778 File Offset: 0x000B7978
		private async Task<bool> GenerateSignatureAsync(PdfDocument doc, int[] pageIndexs, string imgSource, byte[] imgsSource2, string applyId, IProgress<double> progress, bool isRemoveBg = false, global::System.Windows.Size changeSize = default(global::System.Windows.Size))
		{
			bool flag;
			if (pageIndexs == null || pageIndexs.Length == 0)
			{
				flag = false;
			}
			else
			{
				Func<PdfPage, PdfStampAnnotation> createFunc2 = this.CreateImageStampFunc2(pageIndexs, imgSource, imgsSource2, applyId, isRemoveBg, changeSize);
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
				global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
				int viewportStartPage = valueTuple.Item1;
				int viewportEndPage = valueTuple.Item2;
				if (createFunc2 != null)
				{
					IProgress<double> progress2 = progress;
					if (progress2 != null)
					{
						progress2.Report(0.0);
					}
					SignatureDragControl.ApplySignatures.Clear();
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						for (int i = 0; i < pageIndexs.Length; i++)
						{
							int num = pageIndexs[i];
							if (num != this.Annotation.Page.PageIndex)
							{
								IntPtr intPtr = IntPtr.Zero;
								PdfPage pdfPage = null;
								try
								{
									intPtr = Pdfium.FPDF_LoadPage(doc.Handle, num);
									if (intPtr != IntPtr.Zero)
									{
										pdfPage = PdfPage.FromHandle(doc, intPtr, num, true);
										PdfStampAnnotation pdfStampAnnotation = (PdfStampAnnotation)DispatcherHelper.UIDispatcher.Invoke(createFunc2, new object[] { pdfPage });
										SignatureDragControl.ApplySignatures.Add(pdfStampAnnotation);
									}
								}
								finally
								{
									if (pdfPage != null && (pdfPage.PageIndex > viewportEndPage || pdfPage.PageIndex < viewportStartPage))
									{
										PageDisposeHelper.DisposePage(pdfPage);
									}
									if (intPtr != IntPtr.Zero)
									{
										Pdfium.FPDF_ClosePage(intPtr);
									}
								}
								IProgress<double> progress3 = progress;
								if (progress3 != null)
								{
									progress3.Report(1.0 / (double)pageIndexs.Length * (double)(i + 1));
								}
							}
						}
						IProgress<double> progress4 = progress;
						if (progress4 == null)
						{
							return;
						}
						progress4.Report(1.0);
					})).ConfigureAwait(false);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x0600273C RID: 10044 RVA: 0x000B9800 File Offset: 0x000B7A00
		private Func<PdfPage, PdfStampAnnotation> CreateImageStampFunc2(int[] pageIndex, string imgSource, byte[] imgSource2, string applyId, bool isRemove, global::System.Windows.Size changeSize = default(global::System.Windows.Size))
		{
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			return delegate(PdfPage p)
			{
				ImageStampModel imageStampModel = new ImageStampModel
				{
					ImageFilePath = imgSource
				};
				WriteableBitmap writeableBitmap = null;
				try
				{
					writeableBitmap = new WriteableBitmap(new BitmapImage(new Uri(imgSource))
					{
						CacheOption = BitmapCacheOption.None
					});
				}
				catch (Exception)
				{
				}
				imageStampModel.StampImageSource = writeableBitmap;
				PdfPage currentPage = vm.Document.Pages.CurrentPage;
				FS_RECTF effectiveBox = vm.Document.Pages.CurrentPage.GetEffectiveBox(currentPage.Rotation, false);
				AnnotationCanvas parentCanvas = this.ParentCanvas;
				Rect rect;
				((parentCanvas != null) ? parentCanvas.PdfViewer : null).TryGetClientRect(currentPage.PageIndex, effectiveBox, out rect);
				double num = rect.Width / (double)effectiveBox.Width;
				global::System.Windows.Size signaturePageSize = SignatureDragControl.GetSignaturePageSize(writeableBitmap.Width, writeableBitmap.Height, effectiveBox);
				global::System.Windows.Size size = new global::System.Windows.Size(signaturePageSize.Width * num, signaturePageSize.Height * num);
				imageStampModel.ImageWidth = size.Width;
				imageStampModel.ImageHeight = size.Height;
				imageStampModel.PageSize = new FS_SIZEF(signaturePageSize.Width, signaturePageSize.Height);
				PdfStampAnnotation pdfStampAnnotation = null;
				WriteableBitmap writeableBitmap2 = null;
				FS_RECTF rect2 = this.Annotation.GetRECT();
				if (((imageStampModel != null) ? imageStampModel.StampImageSource : null) != null)
				{
					if (imageStampModel.StampImageSource.Format == PixelFormats.Bgra32)
					{
						writeableBitmap2 = new WriteableBitmap(imageStampModel.StampImageSource);
					}
					else
					{
						writeableBitmap2 = new WriteableBitmap(new FormatConvertedBitmap(imageStampModel.StampImageSource, PixelFormats.Bgra32, null, 0.0));
					}
				}
				global::System.Windows.Size size2 = new global::System.Windows.Size((double)imageStampModel.PageSize.Width, (double)imageStampModel.PageSize.Height);
				using (PdfBitmap pdfBitmap = new PdfBitmap(writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight, true, false))
				{
					int num2 = pdfBitmap.Stride * pdfBitmap.Height;
					writeableBitmap2.CopyPixels(new Int32Rect(0, 0, writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight), pdfBitmap.Buffer, num2, pdfBitmap.Stride);
					PdfImageObject pdfImageObject = PdfImageObject.Create(vm.Document, pdfBitmap, rect2.left, (float)((double)rect2.top - size2.Height));
					FS_MATRIX fs_MATRIX = new FS_MATRIX((float)size2.Width, 0f, 0f, (float)size2.Height, rect2.left, (float)((double)rect2.top - size2.Height));
					if (changeSize != default(global::System.Windows.Size))
					{
						fs_MATRIX = new FS_MATRIX((float)changeSize.Width, 0f, 0f, (float)changeSize.Height, rect2.left, (float)((double)rect2.top - changeSize.Height));
					}
					pdfImageObject.Matrix = fs_MATRIX;
					pdfStampAnnotation = new PdfStampAnnotation(p, "", rect2.left, rect2.top, FS_COLOR.SteelBlue);
					PdfTypeArray rangeArr = PdfTypeArray.Create();
					pageIndex.ToList<int>().ForEach(delegate(int i)
					{
						rangeArr.Add(PdfTypeNumber.Create(i));
					});
					if (isRemove)
					{
						pdfImageObject.BlendMode = BlendTypes.FXDIB_BLEND_MULTIPLY;
						pdfStampAnnotation.Dictionary["IsRemoveBg"] = PdfTypeBoolean.Create(true);
					}
					pdfStampAnnotation.Dictionary["ApplyRange"] = rangeArr;
					pdfStampAnnotation.Dictionary["ApplyId"] = PdfTypeString.Create(applyId, false, false);
					pdfStampAnnotation.Flags |= AnnotationFlags.Print;
					pdfStampAnnotation.NormalAppearance.Clear();
					pdfStampAnnotation.NormalAppearance.Add(pdfImageObject);
					pdfStampAnnotation.Subject = "Signature";
					pdfStampAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					pdfStampAnnotation.CreationDate = pdfStampAnnotation.ModificationDate;
					pdfStampAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
					pdfStampAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
					FS_RECTF rect3 = pdfStampAnnotation.GetRECT();
					FS_RECTF rotatedRect = PdfRotateUtils.GetRotatedRect(rect3, new FS_POINTF(rect3.left, rect3.top), p.Rotation);
					if (p.Rotation != PageRotate.Normal)
					{
						pdfStampAnnotation.Dictionary["Rotate"] = PdfTypeNumber.Create((int)(p.Rotation * (PageRotate)90));
					}
					pdfStampAnnotation.Rectangle = rotatedRect;
					pdfStampAnnotation.RegenerateAppearancesAdvance();
					if (p.Annots == null)
					{
						p.CreateAnnotations();
					}
					p.Annots.Add(pdfStampAnnotation);
					vm.MaybeHaveUnembeddedSignature = true;
					PageEditorViewModel pageEditors = vm.PageEditors;
					if (pageEditors != null)
					{
						pageEditors.NotifyPageAnnotationChanged(p.PageIndex);
					}
				}
				return pdfStampAnnotation;
			};
		}

		// Token: 0x0600273D RID: 10045 RVA: 0x000B985C File Offset: 0x000B7A5C
		private async Task ReomveSignatureAsync(PdfDocument doc, int[] pageIndexs, string applyId)
		{
			if (pageIndexs != null && pageIndexs.Length != 0)
			{
				global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(doc);
				global::PDFKit.PdfControl pdfControl = viewer;
				global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
				int viewportStartPage = valueTuple.Item1;
				int viewportEndPage = valueTuple.Item2;
				foreach (int num in pageIndexs)
				{
					IntPtr pageHandle = IntPtr.Zero;
					PdfPage page = null;
					try
					{
						pageHandle = Pdfium.FPDF_LoadPage(doc.Handle, num);
						if (pageHandle != IntPtr.Zero)
						{
							page = PdfPage.FromHandle(doc, pageHandle, num, true);
							await this.RemoveImageStampFuncAsync(page, applyId);
						}
					}
					finally
					{
						if (page != null && (page.PageIndex > viewportEndPage || page.PageIndex < viewportStartPage))
						{
							PageDisposeHelper.DisposePage(page);
						}
						if (pageHandle != IntPtr.Zero)
						{
							Pdfium.FPDF_ClosePage(pageHandle);
						}
					}
					page = null;
				}
				if (viewer != null)
				{
					await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
				}
			}
		}

		// Token: 0x0600273E RID: 10046 RVA: 0x000B98B8 File Offset: 0x000B7AB8
		private async Task RemoveImageStampFuncAsync(PdfPage page, string applyId)
		{
			await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfAnnotationCollection annots = page.Annots;
			List<PdfStampAnnotation> list = ((annots != null) ? (from x in annots.OfType<PdfStampAnnotation>()
				where x.Subject == "Signature" && x.Dictionary.ContainsKey("ApplyId") && x.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString == applyId
				select x).ToList<PdfStampAnnotation>() : null);
			if (list != null && list.Count > 0)
			{
				foreach (PdfStampAnnotation pdfStampAnnotation in list)
				{
					pdfStampAnnotation.DeleteAnnotation();
				}
				PageEditorViewModel pageEditors = requiredService.PageEditors;
				if (pageEditors != null)
				{
					pageEditors.NotifyPageAnnotationChanged(page.PageIndex);
				}
				await page.TryRedrawPageAsync(default(CancellationToken));
			}
		}

		// Token: 0x0600273F RID: 10047 RVA: 0x000B9904 File Offset: 0x000B7B04
		private async Task ConvertSignatureObj(PdfDocument doc, int[] pageIndexs, string applyId, IProgress<double> progress)
		{
			SignatureDragControl.<>c__DisplayClass38_0 CS$<>8__locals1 = new SignatureDragControl.<>c__DisplayClass38_0();
			CS$<>8__locals1.pageIndexs = pageIndexs;
			CS$<>8__locals1.doc = doc;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.applyId = applyId;
			CS$<>8__locals1.progress = progress;
			if (CS$<>8__locals1.pageIndexs != null && CS$<>8__locals1.pageIndexs.Length != 0)
			{
				IProgress<double> progress2 = CS$<>8__locals1.progress;
				if (progress2 != null)
				{
					progress2.Report(0.0);
				}
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					SignatureDragControl.<>c__DisplayClass38_0.<<ConvertSignatureObj>b__0>d <<ConvertSignatureObj>b__0>d;
					<<ConvertSignatureObj>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<ConvertSignatureObj>b__0>d.<>4__this = CS$<>8__locals1;
					<<ConvertSignatureObj>b__0>d.<>1__state = -1;
					<<ConvertSignatureObj>b__0>d.<>t__builder.Start<SignatureDragControl.<>c__DisplayClass38_0.<<ConvertSignatureObj>b__0>d>(ref <<ConvertSignatureObj>b__0>d);
					return <<ConvertSignatureObj>b__0>d.<>t__builder.Task;
				})).ConfigureAwait(false);
			}
		}

		// Token: 0x06002740 RID: 10048 RVA: 0x000B9968 File Offset: 0x000B7B68
		private async Task AddEmbedSignatureObjAsync(PdfPage page, string applyId)
		{
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfAnnotationCollection annots = page.Annots;
			List<PdfStampAnnotation> imgStamps = ((annots != null) ? (from x in annots.OfType<PdfStampAnnotation>()
				where x.Subject == "Signature" && x.Dictionary.ContainsKey("ApplyId") && x.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString == applyId
				select x).ToList<PdfStampAnnotation>() : null);
			if (imgStamps != null && imgStamps.Count > 0)
			{
				Action <>9__1;
				for (int i = 0; i < imgStamps.Count; i++)
				{
					PdfStampAnnotation pdfStampAnnotation = imgStamps[i];
					pdfStampAnnotation.Dictionary["Embed"] = PdfTypeBoolean.Create(true);
					pdfStampAnnotation.DeleteAnnotation();
					Dispatcher dispatcher = Application.Current.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							PageEditorViewModel pageEditors = vm.PageEditors;
							if (pageEditors == null)
							{
								return;
							}
							pageEditors.NotifyPageAnnotationChanged(page.PageIndex);
						});
					}
					dispatcher.Invoke(action);
					await StampUtil.FlattenAnnotationAsync(pdfStampAnnotation);
				}
			}
		}

		// Token: 0x06002741 RID: 10049 RVA: 0x000B99B4 File Offset: 0x000B7BB4
		private static global::System.Windows.Size GetSignaturePageSize(double bitmapWidth, double bitmapHeight, FS_RECTF pageBounds)
		{
			global::System.Windows.Size size = new global::System.Windows.Size(bitmapWidth * 96.0 / 72.0, bitmapHeight * 96.0 / 72.0);
			if (size.Width != 200.0)
			{
				double num = 200.0 / size.Width;
				size = new global::System.Windows.Size(200.0, size.Height * num);
			}
			if (size.Height > (double)(pageBounds.Height / 2f))
			{
				double num2 = size.Height / (double)(pageBounds.Height / 2f);
				size = new global::System.Windows.Size(size.Width / num2, (double)(pageBounds.Height / 2f));
			}
			return size;
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x000B9A7C File Offset: 0x000B7C7C
		public static byte[] intToBytes(int[] src, int offset)
		{
			byte[] array = new byte[src.Length * 4];
			for (int i = 0; i < src.Length; i++)
			{
				array[offset + 3] = (byte)((src[i] >> 24) & 255);
				array[offset + 2] = (byte)((src[i] >> 16) & 255);
				array[offset + 1] = (byte)((src[i] >> 8) & 255);
				array[offset] = (byte)(src[i] & 255);
				offset += 4;
			}
			return array;
		}

		// Token: 0x040010DE RID: 4318
		public static List<PdfStampAnnotation> ApplySignatures;

		// Token: 0x040010DF RID: 4319
		public static readonly DependencyProperty IsApplyProperty = DependencyProperty.Register("IsApply", typeof(bool), typeof(SignatureDragControl), new PropertyMetadata(false));
	}
}
