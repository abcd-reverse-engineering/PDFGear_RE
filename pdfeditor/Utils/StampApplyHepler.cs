using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Signature;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Utils
{
	// Token: 0x020000A5 RID: 165
	public class StampApplyHepler
	{
		// Token: 0x06000A49 RID: 2633 RVA: 0x00034A2C File Offset: 0x00032C2C
		public StampApplyHepler(AnnotationCanvas canvas)
		{
			this.ParentCanvas = canvas;
			this.ApplySignatures = new List<PdfStampAnnotation>();
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x00034A48 File Offset: 0x00032C48
		public async Task EmbedInBatch(PdfStampAnnotation Annotation)
		{
			StampApplyHepler.<>c__DisplayClass3_0 CS$<>8__locals1 = new StampApplyHepler.<>c__DisplayClass3_0();
			CS$<>8__locals1.<>4__this = this;
			GAManager.SendEvent("PdfStampAnnotation", "BatchFlatten", "Begin", 1L);
			SignatureEmbedConfirmWin signatureEmbedConfirmWin = new SignatureEmbedConfirmWin(EmbedType.StampInBatch);
			signatureEmbedConfirmWin.ShowDialog();
			bool? dialogResult = signatureEmbedConfirmWin.DialogResult;
			bool flag = false;
			if (!((dialogResult.GetValueOrDefault() == flag) & (dialogResult != null)))
			{
				try
				{
					if (Annotation.Dictionary.ContainsKey("ApplyRange") && Annotation.Dictionary.ContainsKey("ApplyId"))
					{
						GAManager.SendEvent("PdfStampAnnotation", "BatchFlatten", "Start", 1L);
						PdfTypeBase[] array = Annotation.Dictionary["ApplyRange"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>();
						CS$<>8__locals1.applyId = Annotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString;
						CS$<>8__locals1.pageIndex = new int[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							CS$<>8__locals1.pageIndex[i] = (array[i] as PdfTypeNumber).IntValue;
						}
						CS$<>8__locals1.vm = Ioc.Default.GetRequiredService<MainViewModel>();
						if (Annotation.Dictionary.ContainsKey("ImgSource"))
						{
							string unicodeString = Annotation.Dictionary["ImgSource"].As<PdfTypeString>(true).UnicodeString;
						}
						ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
						{
							StampApplyHepler.<>c__DisplayClass3_0.<<EmbedInBatch>b__0>d <<EmbedInBatch>b__0>d;
							<<EmbedInBatch>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<EmbedInBatch>b__0>d.<>4__this = CS$<>8__locals1;
							<<EmbedInBatch>b__0>d.c = c;
							<<EmbedInBatch>b__0>d.<>1__state = -1;
							<<EmbedInBatch>b__0>d.<>t__builder.Start<StampApplyHepler.<>c__DisplayClass3_0.<<EmbedInBatch>b__0>d>(ref <<EmbedInBatch>b__0>d);
							return <<EmbedInBatch>b__0>d.<>t__builder.Task;
						}, null, Resources.StampProgressFlattenContent, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
						global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(CS$<>8__locals1.vm.Document);
						CS$<>8__locals1.vm.SetCanSaveFlag("FlattenSignature", true);
						await Annotation.Page.TryRedrawPageAsync(default(CancellationToken));
						await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
						GAManager.SendEvent("PdfStampAnnotation", "BatchFlatten", "Done", 1L);
						viewer = null;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x00034A94 File Offset: 0x00032C94
		private async Task ConvertStampObj(PdfDocument doc, int[] pageIndexs, string applyId, IProgress<double> progress)
		{
			StampApplyHepler.<>c__DisplayClass4_0 CS$<>8__locals1 = new StampApplyHepler.<>c__DisplayClass4_0();
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
					StampApplyHepler.<>c__DisplayClass4_0.<<ConvertStampObj>b__0>d <<ConvertStampObj>b__0>d;
					<<ConvertStampObj>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<ConvertStampObj>b__0>d.<>4__this = CS$<>8__locals1;
					<<ConvertStampObj>b__0>d.<>1__state = -1;
					<<ConvertStampObj>b__0>d.<>t__builder.Start<StampApplyHepler.<>c__DisplayClass4_0.<<ConvertStampObj>b__0>d>(ref <<ConvertStampObj>b__0>d);
					return <<ConvertStampObj>b__0>d.<>t__builder.Task;
				})).ConfigureAwait(false);
			}
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00034AF8 File Offset: 0x00032CF8
		private async Task AddEmbedStampObjAsync(PdfPage page, string applyId)
		{
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfAnnotationCollection annots = page.Annots;
			List<PdfStampAnnotation> imgStamps = ((annots != null) ? (from x in annots.OfType<PdfStampAnnotation>()
				where x.Subject == "Stamp" && x.Dictionary.ContainsKey("ApplyId") && x.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString == applyId
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

		// Token: 0x06000A4D RID: 2637 RVA: 0x00034B44 File Offset: 0x00032D44
		public async Task DeleteInBatch(PdfStampAnnotation Annotation)
		{
			GAManager.SendEvent("PdfStampAnnotation", "BatchDelete", "Count", 1L);
			if (ModernMessageBox.Show(Resources.StampRightMenuDeleteAllStamp, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.No)
			{
				MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
				await PdfObjectExtensions.GetAnnotationHolderManager(global::PDFKit.PdfControl.GetPdfControl(vm.Document)).DeleteAnnotationAsync(Annotation, false);
				if (Annotation.Dictionary.ContainsKey("ApplyRange") && Annotation.Dictionary.ContainsKey("ApplyId"))
				{
					PdfTypeBase[] array = Annotation.Dictionary["ApplyRange"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>();
					string applyId = Annotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString;
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
					await this.ReomveStampAsync(vm.Document, pageIndex, applyId);
					applyId = null;
					pageIndex = null;
				}
			}
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00034B90 File Offset: 0x00032D90
		private async Task ReomveStampAsync(PdfDocument doc, int[] pageIndexs, string applyId)
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

		// Token: 0x06000A4F RID: 2639 RVA: 0x00034BEC File Offset: 0x00032DEC
		private async Task RemoveImageStampFuncAsync(PdfPage page, string applyId)
		{
			await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfAnnotationCollection annots = page.Annots;
			List<PdfStampAnnotation> list = ((annots != null) ? (from x in annots.OfType<PdfStampAnnotation>()
				where x.Subject == "Stamp" && x.Dictionary.ContainsKey("ApplyId") && x.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString == applyId
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

		// Token: 0x06000A50 RID: 2640 RVA: 0x00034C38 File Offset: 0x00032E38
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

		// Token: 0x06000A51 RID: 2641 RVA: 0x00034CC9 File Offset: 0x00032EC9
		private Func<PdfPage, PdfStampAnnotation> CreateTextStampFunc2(int[] pageIndex, string applyId, bool isRemove, PdfStampAnnotation Annotation, Size changeSize = default(Size))
		{
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			return delegate(PdfPage p)
			{
				PDFExtStampDictionary pdfextStampDictionary = new PDFExtStampDictionary();
				pdfextStampDictionary.Type = "Stamp";
				pdfextStampDictionary.Template = "Default";
				FS_RECTF rect = Annotation.GetRECT();
				PdfStampAnnotation pdfStampAnnotation = new PdfStampAnnotation(p, Annotation.Contents, rect.left, rect.top, Annotation.Color);
				pdfStampAnnotation.Contents = null;
				PDFExtStampDictionary pdfextStampDictionary2 = StampUtil.GetPDFExtStampDictionary(Annotation);
				StampUtil.SetPDFXStampDictionary(pdfStampAnnotation, pdfextStampDictionary2);
				pdfStampAnnotation.Rectangle = Annotation.Rectangle;
				pdfStampAnnotation.ExtendedIconName = Annotation.Contents;
				pdfStampAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				pdfStampAnnotation.CreationDate = pdfStampAnnotation.ModificationDate;
				pdfStampAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
				PdfTypeArray rangeArr = PdfTypeArray.Create();
				pageIndex.ToList<int>().ForEach(delegate(int i)
				{
					rangeArr.Add(PdfTypeNumber.Create(i));
				});
				pdfStampAnnotation.Dictionary["ApplyRange"] = rangeArr;
				pdfStampAnnotation.Dictionary["ApplyId"] = PdfTypeString.Create(applyId, false, false);
				pdfStampAnnotation.Subject = "Stamp";
				pdfStampAnnotation.Flags |= AnnotationFlags.Print;
				if (p.Annots == null)
				{
					p.CreateAnnotations();
				}
				p.Annots.Add(pdfStampAnnotation);
				pdfStampAnnotation.TryRedrawAnnotation(false);
				vm.MaybeHaveUnembeddedSignature = true;
				PageEditorViewModel pageEditors = vm.PageEditors;
				if (pageEditors != null)
				{
					pageEditors.NotifyPageAnnotationChanged(p.PageIndex);
				}
				return pdfStampAnnotation;
			};
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00034D04 File Offset: 0x00032F04
		private Func<PdfPage, PdfStampAnnotation> CreateImageStampFunc2(int[] pageIndex, string imgSource, byte[] imgSource2, string applyId, bool isRemove, PdfStampAnnotation Annotation, Size changeSize = default(Size))
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
				Size stampPageSize = StampApplyHepler.GetStampPageSize(writeableBitmap.Width, writeableBitmap.Height, effectiveBox);
				Size size = new Size(stampPageSize.Width * num, stampPageSize.Height * num);
				imageStampModel.ImageWidth = size.Width;
				imageStampModel.ImageHeight = size.Height;
				imageStampModel.PageSize = new FS_SIZEF(stampPageSize.Width, stampPageSize.Height);
				PdfStampAnnotation pdfStampAnnotation = null;
				WriteableBitmap writeableBitmap2 = null;
				FS_RECTF rect2 = Annotation.GetRECT();
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
				Size size2 = new Size((double)rect2.Width, (double)rect2.Height);
				using (PdfBitmap pdfBitmap = new PdfBitmap(writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight, true, false))
				{
					int num2 = pdfBitmap.Stride * pdfBitmap.Height;
					writeableBitmap2.CopyPixels(new Int32Rect(0, 0, writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight), pdfBitmap.Buffer, num2, pdfBitmap.Stride);
					PdfImageObject pdfImageObject = PdfImageObject.Create(vm.Document, pdfBitmap, rect2.left, (float)((double)rect2.top - size2.Height));
					FS_MATRIX fs_MATRIX = new FS_MATRIX((float)size2.Width, 0f, 0f, (float)size2.Height, rect2.left, (float)((double)rect2.top - size2.Height));
					if (changeSize != default(Size))
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
					pdfStampAnnotation.Subject = "Stamp";
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

		// Token: 0x06000A53 RID: 2643 RVA: 0x00034D68 File Offset: 0x00032F68
		public async Task<bool> GenerateStampAsync(PdfDocument doc, int[] pageIndexs, string imgSource, byte[] imgsSource2, string applyId, IProgress<double> progress, PdfStampAnnotation Annotation, bool isRemoveBg = false, Size changeSize = default(Size))
		{
			bool flag;
			if (pageIndexs == null || pageIndexs.Length == 0)
			{
				flag = false;
			}
			else
			{
				Func<PdfPage, PdfStampAnnotation> createFunc2 = null;
				if (string.IsNullOrEmpty(imgSource))
				{
					createFunc2 = this.CreateTextStampFunc2(pageIndexs, applyId, isRemoveBg, Annotation, changeSize);
				}
				else
				{
					createFunc2 = this.CreateImageStampFunc2(pageIndexs, imgSource, imgsSource2, applyId, isRemoveBg, Annotation, changeSize);
				}
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
					this.ApplySignatures.Clear();
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						for (int i = 0; i < pageIndexs.Length; i++)
						{
							int num = pageIndexs[i];
							if (num != Annotation.Page.PageIndex)
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
										this.ApplySignatures.Add(pdfStampAnnotation);
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

		// Token: 0x06000A54 RID: 2644 RVA: 0x00034DFC File Offset: 0x00032FFC
		private static Size GetStampPageSize(double bitmapWidth, double bitmapHeight, FS_RECTF pageBounds)
		{
			Size size = new Size(bitmapWidth * 96.0 / 72.0, bitmapHeight * 96.0 / 72.0);
			if (size.Width != 200.0)
			{
				double num = 200.0 / size.Width;
				size = new Size(200.0, size.Height * num);
			}
			if (size.Height > (double)(pageBounds.Height / 2f))
			{
				double num2 = size.Height / (double)(pageBounds.Height / 2f);
				size = new Size(size.Width / num2, (double)(pageBounds.Height / 2f));
			}
			return size;
		}

		// Token: 0x0400049F RID: 1183
		public List<PdfStampAnnotation> ApplySignatures;

		// Token: 0x040004A0 RID: 1184
		public AnnotationCanvas ParentCanvas;
	}
}
