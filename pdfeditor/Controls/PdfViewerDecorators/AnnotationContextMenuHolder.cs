using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.Signature;
using pdfeditor.Models.Attachments;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000234 RID: 564
	internal class AnnotationContextMenuHolder
	{
		// Token: 0x06001FFB RID: 8187 RVA: 0x0009046E File Offset: 0x0008E66E
		public AnnotationContextMenuHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.InitContextMenu();
		}

		// Token: 0x06001FFC RID: 8188 RVA: 0x00090494 File Offset: 0x0008E694
		private void InitContextMenu()
		{
			if (AnnotationContextMenuHolder.IsDesignMode)
			{
				return;
			}
			this.duplicateAnnotItem = new ContextMenuItemModel
			{
				Name = "Duplicate",
				Caption = Resources.MenuRightAnnotateDuplicate,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Duplicate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Duplicate.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDuplicateAnnotation))
			};
			this.copyTextItem = new ContextMenuItemModel
			{
				Name = "CopyText",
				Caption = Resources.AnnotationContextMenuCopyText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Copy.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Copy.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextCopyAnnotationText))
			};
			this.deleteTextItem = new ContextMenuItemModel
			{
				Name = "DeleteText",
				Caption = Resources.MenuAnnotateRemoveTextContent,
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeleteAnnotationText)),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_DeleteText.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_DeleteText.png"))
			};
			this.deleteAnnotItem = new ContextMenuItemModel
			{
				Name = "Delete",
				Caption = Resources.RightMenuDeleteAnnotationItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_AnnotDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_AnnotDelete.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeleteAnnotation)),
				HotKeyString = "Delete"
			};
			this.openFileItem = new ContextMenuItemModel
			{
				Name = "OpenFile",
				Caption = Resources.AttachmentPanelBtnOpenAttachmentText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/OpenFile.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/OpenFile.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextOpenFileAnnotation))
			};
			this.saveToPCItem = new ContextMenuItemModel
			{
				Name = "SaveToPC",
				Caption = Resources.AttachmentPanelBtnSaveAttachmentText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/SaveToPC.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/SaveToPC.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextSaveToPCAnnotation))
			};
			this.editDescriptionItem = new ContextMenuItemModel
			{
				Name = "EditDescription",
				Caption = Resources.AttachmentPanelBtnEditDescriptionText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/EditDescription.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/EditDescription.png")),
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnContextEditDescriptionAnnotation))
			};
			this.deleteAttachmentItem = new ContextMenuItemModel
			{
				Name = "DeleteAttachment",
				Caption = Resources.AttachmentPanelBtnDeleteAttachmentText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_AnnotDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_AnnotDelete.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeleteAnnotation)),
				HotKeyString = "Delete"
			};
			this.stampFlattenItem = new ContextMenuItemModel
			{
				Name = "FlattenStamp",
				Caption = Resources.WinSignatureContextMenuFlatten,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Signature/Embed.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Signature/Embed.png")),
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnFlattenAnnotation))
			};
			this.stampApplyMulPageItem = new ContextMenuItemModel
			{
				Name = "ApplyStampMulPage",
				Caption = Resources.WinSignatureContextMenuApplyMulPage,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Signature/Apply.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Signature/Apply.png")),
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnApplyMulPageAnnotation))
			};
			this.batchStampFlattenItem = new ContextMenuItemModel
			{
				Name = "FlattenStampInBatch",
				Caption = Resources.WinSignatureContextMenuFlattenInBatch,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Signature/EmbedInBatch.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Signature/EmbedInBatch.png")),
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnFlattenAnnotationInBatch))
			};
			this.stampDeleteItem = new ContextMenuItemModel
			{
				Name = "DeleteStamp",
				Caption = Resources.WinSignatureContextMenuDetele,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Signature/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Signature/Delete.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnStampDelete)),
				HotKeyString = "Delete"
			};
			this.batchStampDeleteItem = new ContextMenuItemModel
			{
				Name = "DeleteStampInBatch",
				Caption = Resources.WinSignatureContextMenuDeteleInBatch,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Signature/DeleteInBatch.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Signature/DeleteInBatch.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnStampDeleteInBatch))
			};
			this.contextMenus = new ContextMenuModel { this.duplicateAnnotItem, this.deleteAnnotItem };
			this.selectAnnotationContextMenu = new PdfViewerContextMenu
			{
				ItemsSource = this.contextMenus,
				PlacementTarget = this.annotationCanvas,
				AutoCloseOnMouseLeave = false
			};
		}

		// Token: 0x06001FFD RID: 8189 RVA: 0x00090984 File Offset: 0x0008EB84
		private async Task OnContextDuplicateAnnotation(ContextMenuItemModel model)
		{
			PdfAnnotation selectedAnnotation = this.annotationCanvas.SelectedAnnotation;
			if (AnnotationContextMenuHolder.CanDuplicate(this.annotationCanvas.SelectedAnnotation))
			{
				GAManager.SendEvent("ContextRightMenu", "DuplicateAnnotation", "Count", 1L);
				PdfAnnotation pdfAnnotation = await selectedAnnotation.DuplicateAnnotationAsync(null, null);
				if (pdfAnnotation != null)
				{
					this.annotationCanvas.HolderManager.Select(pdfAnnotation, false);
					try
					{
						PdfHighlightAnnotation pdfHighlightAnnotation = pdfAnnotation as PdfHighlightAnnotation;
						if (pdfHighlightAnnotation != null)
						{
							if (!string.IsNullOrWhiteSpace(pdfHighlightAnnotation.Subject) && pdfHighlightAnnotation.Subject == "AreaHighlight")
							{
								GAManager.SendEvent("AnnotationAction", "PdfAreaHighlightAnnotation", "Duplicate", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfHighlightAnnotation", "Duplicate", 1L);
							}
						}
						if (pdfAnnotation is PdfStrikeoutAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfStrikeoutAnnotation", "Duplicate", 1L);
						}
						if (pdfAnnotation is PdfUnderlineAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfUnderlineAnnotation", "Duplicate", 1L);
						}
						if (pdfAnnotation is PdfLineAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfLineAnnotation", "Duplicate", 1L);
						}
						if (pdfAnnotation is PdfSquareAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfSquareAnnotation", "Duplicate", 1L);
						}
						if (pdfAnnotation is PdfCircleAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfCircleAnnotation", "Duplicate", 1L);
						}
						if (pdfAnnotation is PdfInkAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfInkAnnotation", "Duplicate", 1L);
						}
						PdfFreeTextAnnotation pdfFreeTextAnnotation = pdfAnnotation as PdfFreeTextAnnotation;
						if (pdfFreeTextAnnotation != null)
						{
							if (pdfFreeTextAnnotation.Intent == AnnotationIntent.FreeTextTypeWriter)
							{
								GAManager.SendEvent("AnnotationAction", "PdfFreeTextAnnotationTransparent", "Duplicate", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfFreeTextAnnotation", "Duplicate", 1L);
							}
						}
						if (pdfAnnotation is PdfTextAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfTextAnnotation", "Duplicate", 1L);
						}
						PdfStampAnnotation pdfStampAnnotation = pdfAnnotation as PdfStampAnnotation;
						if (pdfStampAnnotation != null)
						{
							if (!string.IsNullOrWhiteSpace(pdfStampAnnotation.Subject) && pdfStampAnnotation.Subject == "Signature")
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotationSignature", "Duplicate", 1L);
							}
							else if (!string.IsNullOrWhiteSpace(pdfStampAnnotation.Subject) && pdfStampAnnotation.Subject == "FormControl")
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotationForm", "Duplicate", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotation", "Duplicate", 1L);
							}
						}
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06001FFE RID: 8190 RVA: 0x000909C8 File Offset: 0x0008EBC8
		private async Task OnContextDeleteAnnotation(ContextMenuItemModel model)
		{
			PdfAnnotation selectedAnnot = this.annotationCanvas.SelectedAnnotation;
			if (selectedAnnot != null)
			{
				GAManager.SendEvent("ContextRightMenu", "DeleteAnnotation", "Count", 1L);
				PdfPage page = selectedAnnot.Page;
				this.annotationCanvas.HolderManager.CancelAll();
				await this.annotationCanvas.HolderManager.WaitForCancelCompletedAsync();
				await this.annotationCanvas.HolderManager.DeleteAnnotationAsync(selectedAnnot, false);
			}
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x00090A0C File Offset: 0x0008EC0C
		private async Task OnStampDelete(ContextMenuItemModel model)
		{
			PdfAnnotation selectedAnnot = this.annotationCanvas.SelectedAnnotation;
			if (selectedAnnot != null)
			{
				GAManager.SendEvent("PdfStampAnnotation", "Delete", "Count", 1L);
				PdfPage page = selectedAnnot.Page;
				this.annotationCanvas.HolderManager.CancelAll();
				await this.annotationCanvas.HolderManager.WaitForCancelCompletedAsync();
				await this.annotationCanvas.HolderManager.DeleteAnnotationAsync(selectedAnnot, false);
			}
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x00090A50 File Offset: 0x0008EC50
		private async Task OnStampDeleteInBatch(ContextMenuItemModel model)
		{
			PdfStampAnnotation pdfStampAnnotation = this.annotationCanvas.SelectedAnnotation as PdfStampAnnotation;
			if (pdfStampAnnotation != null)
			{
				await new StampApplyHepler(this.annotationCanvas).DeleteInBatch(pdfStampAnnotation);
				Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
			}
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x00090A94 File Offset: 0x0008EC94
		private async Task OnContextCopyAnnotationText(ContextMenuItemModel model)
		{
			PdfTextMarkupAnnotation pdfTextMarkupAnnotation = this.annotationCanvas.SelectedAnnotation as PdfTextMarkupAnnotation;
			if (pdfTextMarkupAnnotation != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				PdfText text = pdfTextMarkupAnnotation.Page.Text;
				using (IEnumerator<FS_QUADPOINTSF> enumerator = pdfTextMarkupAnnotation.QuadPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FS_QUADPOINTSF fs_QUADPOINTSF = enumerator.Current;
						FS_RECTF fs_RECTF = fs_QUADPOINTSF.ToPdfRect();
						string boundedText = text.GetBoundedText(fs_RECTF.left - 2f, fs_RECTF.top + 2f, fs_RECTF.right - 2f, fs_RECTF.bottom - 2f);
						stringBuilder.AppendLine(boundedText);
					}
					goto IL_00CE;
				}
				IL_00BC:
				StringBuilder stringBuilder2 = stringBuilder;
				int num = stringBuilder2.Length;
				stringBuilder2.Length = num - 1;
				IL_00CE:
				if (stringBuilder.Length > 0 && (stringBuilder[stringBuilder.Length - 1] == '\r' || stringBuilder[stringBuilder.Length - 1] == '\n'))
				{
					goto IL_00BC;
				}
				string str = stringBuilder.ToString();
				for (int i = 0; i < 3; i = num + 1)
				{
					num = 0;
					try
					{
						Clipboard.SetDataObject(str);
					}
					catch
					{
						num = 1;
					}
					if (num == 1 && i != 2)
					{
						await Task.Delay(100);
					}
					num = i;
				}
				str = null;
			}
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x00090AD8 File Offset: 0x0008ECD8
		private async Task OnContextDeleteAnnotationText(ContextMenuItemModel model)
		{
			GAManager.SendEvent("ContextRightMenu", "EraseText", "Count", 1L);
			PdfAnnotation selectedAnnotation = this.annotationCanvas.SelectedAnnotation;
			PdfTextMarkupAnnotation selectedAnnot = selectedAnnotation as PdfTextMarkupAnnotation;
			if (selectedAnnot != null)
			{
				PdfPage page = selectedAnnot.Page;
				MainViewModel vm = (MainViewModel)this.annotationCanvas.DataContext;
				IDisposable disposable = await vm.OperationManager.CreateScopeAsync();
				using (disposable)
				{
					if (selectedAnnot.Subject == "AreaHighlight")
					{
						PdfQuadPointsCollection quadPoints = selectedAnnot.QuadPoints;
						if (quadPoints != null && quadPoints.Count == 1)
						{
							FS_RECTF fs_RECTF = selectedAnnot.QuadPoints[0].ToPdfRect();
							await vm.OperationManager.RedactionTextObjectAsync(page, fs_RECTF, null, "DeleteAnnotationText");
							goto IL_0410;
						}
					}
					foreach (FS_QUADPOINTSF fs_QUADPOINTSF in selectedAnnot.QuadPoints)
					{
						FS_RECTF fs_RECTF2 = fs_QUADPOINTSF.ToPdfRect();
						float num = fs_RECTF2.bottom + fs_RECTF2.Height / 2f;
						float left = fs_RECTF2.left;
						float right = fs_RECTF2.right;
						float num2 = Math.Max(fs_RECTF2.top - 0.5f, num);
						PdfText text = page.Text;
						int num3 = -1;
						int num4 = -1;
						while (num2 > fs_RECTF2.bottom + 0.5f)
						{
							num3 = text.GetCharIndexAtPos(left, num2, 2f, 2f);
							num4 = text.GetCharIndexAtPos(right, num2, 2f, 2f);
							if (num3 >= 0 && num4 >= 0)
							{
								break;
							}
							num2 -= 2f;
						}
						if (num3 >= 0 && num4 >= 0)
						{
							if (num3 > num4)
							{
								int num5 = num4;
								num4 = num3;
								num3 = num5;
							}
							int num6 = 0;
							for (int i = num3; i <= num4; i++)
							{
								FS_RECTF charBox = text.GetCharBox(i);
								charBox.left -= 2f;
								charBox.top += 2f;
								charBox.right += 2f;
								charBox.bottom -= 2f;
								if (charBox.IntersectsWith(fs_RECTF2))
								{
									num6++;
								}
							}
							if (num6 >= (num4 - num3) / 3)
							{
								GAManager.SendEvent("EraseText", "EraseText", "ContextRightMenu", 1L);
								await vm.OperationManager.RemoveSelectedTextAsync(page, num3, num4, "RemoveSelectedText");
							}
						}
					}
					IEnumerator<FS_QUADPOINTSF> enumerator = null;
					IL_0410:
					await this.annotationCanvas.HolderManager.DeleteAnnotationAsync(selectedAnnot, false);
				}
				IDisposable disposable2 = null;
				page = null;
				vm = null;
			}
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x00090B1C File Offset: 0x0008ED1C
		private async Task OnContextSaveToPCAnnotation(ContextMenuItemModel model)
		{
			PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.annotationCanvas.SelectedAnnotation as PdfFileAttachmentAnnotation;
			if (pdfFileAttachmentAnnotation != null)
			{
				GAManager.SendEvent("PDFAttachment", "Save", "ViewerContextMenu", 1L);
				await AttachmentFileUtils.AttachmentSaveAsFileFromAnnotation(pdfFileAttachmentAnnotation, null);
			}
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x00090B60 File Offset: 0x0008ED60
		private async Task OnContextOpenFileAnnotation(ContextMenuItemModel model)
		{
			PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.annotationCanvas.SelectedAnnotation as PdfFileAttachmentAnnotation;
			if (pdfFileAttachmentAnnotation != null)
			{
				GAManager.SendEvent("PDFAttachment", "Open", "ViewerContextMenu", 1L);
				if (ModernMessageBox.Show(Resources.AnnotationFileAttachmentOpenWarning, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
				{
					TaskAwaiter<bool> taskAwaiter = AttachmentFileUtils.OpenAttachmentFromAnnotation(pdfFileAttachmentAnnotation).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						ModernMessageBox.Show(Resources.MsgAttachmentOpenFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
				}
			}
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x00090BA4 File Offset: 0x0008EDA4
		private async void OnApplyMulPageAnnotation(ContextMenuItemModel model)
		{
			AnnotationContextMenuHolder.<>c__DisplayClass26_0 CS$<>8__locals1 = new AnnotationContextMenuHolder.<>c__DisplayClass26_0();
			PdfAnnotation selectedAnnotation = this.annotationCanvas.SelectedAnnotation;
			CS$<>8__locals1.Annotation = selectedAnnotation as PdfStampAnnotation;
			if (CS$<>8__locals1.Annotation != null)
			{
				object dataContext = this.annotationCanvas.DataContext;
				CS$<>8__locals1.vm = dataContext as MainViewModel;
				if (CS$<>8__locals1.vm != null)
				{
					GAManager.SendEvent("PdfStampAnnotation", "ApplyMultiPages", "Begin", 1L);
					SignatureApplyPageWin signatureApplyPageWin = new SignatureApplyPageWin(CS$<>8__locals1.Annotation.Page.PageIndex);
					if (signatureApplyPageWin.ShowDialog().GetValueOrDefault() && signatureApplyPageWin.ApplyPageIndex != null)
					{
						CS$<>8__locals1.stampApplyHepler = new StampApplyHepler(this.annotationCanvas);
						CS$<>8__locals1.pageIndexs = signatureApplyPageWin.ApplyPageIndex;
						CS$<>8__locals1.rangeArr = PdfTypeArray.Create();
						CS$<>8__locals1.pageIndexs.ToList<int>().ForEach(delegate(int i)
						{
							CS$<>8__locals1.rangeArr.Add(PdfTypeNumber.Create(i));
						});
						IAnnotationHolder currentHolder = this.annotationCanvas.HolderManager.CurrentHolder;
						CS$<>8__locals1.imgSource = (CS$<>8__locals1.Annotation.Dictionary.ContainsKey("ImgSource") ? CS$<>8__locals1.Annotation.Dictionary["ImgSource"].As<PdfTypeString>(true).UnicodeString : string.Empty);
						CS$<>8__locals1.imgSource2 = null;
						CS$<>8__locals1.isRemoveBg = CS$<>8__locals1.Annotation.Dictionary.ContainsKey("IsRemoveBg") && CS$<>8__locals1.Annotation.Dictionary["IsRemoveBg"].As<PdfTypeBoolean>(true).Value;
						PdfTypeBase[] array = (CS$<>8__locals1.Annotation.Dictionary.ContainsKey("ChangeSize") ? CS$<>8__locals1.Annotation.Dictionary["ChangeSize"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>() : null);
						CS$<>8__locals1.changeSize2 = default(Size);
						if (array != null)
						{
							float[] array2 = new float[array.Length];
							for (int j = 0; j < array.Length; j++)
							{
								array2[j] = (array[j] as PdfTypeNumber).FloatValue;
							}
							if (array2.Length == 2)
							{
								CS$<>8__locals1.changeSize2 = new Size((double)array2[0], (double)array2[1]);
							}
						}
						CS$<>8__locals1.Annotation.Dictionary["ApplyRange"] = CS$<>8__locals1.rangeArr;
						CS$<>8__locals1.applyId = Guid.NewGuid().ToString().ToLower();
						CS$<>8__locals1.Annotation.Dictionary["ApplyId"] = PdfTypeString.Create(CS$<>8__locals1.applyId, false, false);
						GAManager.SendEvent("PdfStampAnnotation", "ApplyMultiPages", "Start", 1L);
						ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
						{
							AnnotationContextMenuHolder.<>c__DisplayClass26_0.<<OnApplyMulPageAnnotation>b__1>d <<OnApplyMulPageAnnotation>b__1>d;
							<<OnApplyMulPageAnnotation>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<OnApplyMulPageAnnotation>b__1>d.<>4__this = CS$<>8__locals1;
							<<OnApplyMulPageAnnotation>b__1>d.c = c;
							<<OnApplyMulPageAnnotation>b__1>d.<>1__state = -1;
							<<OnApplyMulPageAnnotation>b__1>d.<>t__builder.Start<AnnotationContextMenuHolder.<>c__DisplayClass26_0.<<OnApplyMulPageAnnotation>b__1>d>(ref <<OnApplyMulPageAnnotation>b__1>d);
							return <<OnApplyMulPageAnnotation>b__1>d.<>t__builder.Task;
						}, null, Resources.StampProgressContent, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
						await global::PDFKit.PdfControl.GetPdfControl(CS$<>8__locals1.vm.Document).TryRedrawVisiblePageAsync(default(CancellationToken));
						List<PdfStampAnnotation> applySignatures = CS$<>8__locals1.stampApplyHepler.ApplySignatures;
						if (applySignatures.Count > 0)
						{
							await CS$<>8__locals1.vm.OperationManager.TraceAnnotationInsertAsync(applySignatures, "");
						}
						GAManager.SendEvent("PdfStampAnnotation", "ApplyMultiPages", "Done", 1L);
						Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
					}
				}
			}
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x00090BDC File Offset: 0x0008EDDC
		private async void OnFlattenAnnotationInBatch(ContextMenuItemModel model)
		{
			PdfStampAnnotation pdfStampAnnotation = this.annotationCanvas.SelectedAnnotation as PdfStampAnnotation;
			if (pdfStampAnnotation != null)
			{
				await new StampApplyHepler(this.annotationCanvas).EmbedInBatch(pdfStampAnnotation);
				Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x00090C14 File Offset: 0x0008EE14
		private async void OnFlattenAnnotation(ContextMenuItemModel model)
		{
			PdfStampAnnotation pdfStampAnnotation = this.annotationCanvas.SelectedAnnotation as PdfStampAnnotation;
			if (pdfStampAnnotation != null)
			{
				object dataContext = this.annotationCanvas.DataContext;
				MainViewModel vm = dataContext as MainViewModel;
				if (vm != null)
				{
					GAManager.SendEvent("PdfStampAnnotation", "Flatten", "Begin", 1L);
					SignatureEmbedConfirmWin signatureEmbedConfirmWin = new SignatureEmbedConfirmWin(EmbedType.StampSingle);
					signatureEmbedConfirmWin.ShowDialog();
					bool? dialogResult = signatureEmbedConfirmWin.DialogResult;
					bool flag = false;
					if (!((dialogResult.GetValueOrDefault() == flag) & (dialogResult != null)))
					{
						try
						{
							GAManager.SendEvent("PdfStampAnnotation", "Flatten", "Start", 1L);
							global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(vm.Document);
							PdfObjectExtensions.GetAnnotationHolderManager(viewer);
							pdfStampAnnotation.Dictionary["Embed"] = PdfTypeBoolean.Create(true);
							pdfStampAnnotation.DeleteAnnotation();
							PageEditorViewModel pageEditors = vm.PageEditors;
							if (pageEditors != null)
							{
								pageEditors.NotifyPageAnnotationChanged(pdfStampAnnotation.Page.PageIndex);
							}
							await StampUtil.FlattenAnnotationAsync(pdfStampAnnotation);
							vm.SetCanSaveFlag("FlattenStamp", true);
							await viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
							GAManager.SendEvent("PdfStampAnnotation", "Flatten", "Done", 1L);
							Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
							viewer = null;
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x00090C4C File Offset: 0x0008EE4C
		private async void OnContextEditDescriptionAnnotation(ContextMenuItemModel model)
		{
			PdfAnnotation selectedAnnot = this.annotationCanvas.SelectedAnnotation;
			if (selectedAnnot != null)
			{
				MainViewModel mainViewModel = this.annotationCanvas.DataContext as MainViewModel;
				if (mainViewModel != null)
				{
					PDFAttachmentWrapper pdfattachmentWrapper = mainViewModel.AttachmentSource.FirstOrDefault(delegate(PDFAttachmentWrapper wrapper)
					{
						PdfAnnotation pdfAnnotation = wrapper.Attachment as PdfAnnotation;
						return pdfAnnotation != null && pdfAnnotation.Dictionary.Handle == selectedAnnot.Dictionary.Handle;
					});
					if (pdfattachmentWrapper != null)
					{
						GAManager.SendEvent("PDFAttachment", "EditDescription", "ViewerContextMenu", 1L);
						await mainViewModel.EditAttachmentDescriptionCmd.ExecuteAsync(pdfattachmentWrapper);
					}
				}
			}
		}

		// Token: 0x06002009 RID: 8201 RVA: 0x00090C84 File Offset: 0x0008EE84
		public async Task<bool> ShowAsync()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			bool flag;
			if (pdfViewer != null && pdfViewer.MouseMode == MouseModes.PanTool)
			{
				flag = false;
			}
			else
			{
				await Task.Delay(50);
				if (!AnnotationContextMenuHolder.CanShow(this.annotationCanvas.SelectedAnnotation))
				{
					flag = false;
				}
				else
				{
					PdfViewer pdfViewer2 = this.annotationCanvas.PdfViewer;
					if (pdfViewer2 != null)
					{
						pdfViewer2.DeselectText();
					}
					if (this.selectAnnotationContextMenu.IsOpen)
					{
						flag = true;
					}
					else
					{
						this.contextMenus.Remove(this.duplicateAnnotItem);
						if (AnnotationContextMenuHolder.CanDuplicate(this.annotationCanvas.SelectedAnnotation))
						{
							this.contextMenus.Remove(this.deleteAnnotItem);
							this.contextMenus.Insert(0, this.duplicateAnnotItem);
							this.contextMenus.Add(this.deleteAnnotItem);
						}
						this.contextMenus.Remove(this.copyTextItem);
						this.contextMenus.Remove(this.deleteTextItem);
						if (AnnotationContextMenuHolder.CanCopyOrDeleteText(this.annotationCanvas.SelectedAnnotation))
						{
							this.contextMenus.Remove(this.deleteAnnotItem);
							this.contextMenus.Add(this.copyTextItem);
							this.contextMenus.Add(this.deleteAnnotItem);
							this.contextMenus.Add(this.deleteTextItem);
						}
						this.contextMenus.Remove(this.openFileItem);
						this.contextMenus.Remove(this.saveToPCItem);
						this.contextMenus.Remove(this.editDescriptionItem);
						this.contextMenus.Remove(this.deleteAttachmentItem);
						if (this.IsAttachmentAnnotation(this.annotationCanvas.SelectedAnnotation))
						{
							this.contextMenus.Remove(this.deleteAnnotItem);
							this.contextMenus.Add(this.openFileItem);
							this.contextMenus.Add(this.saveToPCItem);
							this.contextMenus.Add(this.editDescriptionItem);
							this.contextMenus.Add(this.deleteAttachmentItem);
						}
						this.contextMenus.Remove(this.stampDeleteItem);
						this.contextMenus.Remove(this.stampFlattenItem);
						this.contextMenus.Remove(this.batchStampFlattenItem);
						this.contextMenus.Remove(this.stampApplyMulPageItem);
						this.contextMenus.Remove(this.batchStampDeleteItem);
						if (this.IsStampAnnotation(this.annotationCanvas.SelectedAnnotation))
						{
							this.contextMenus.Remove(this.deleteAnnotItem);
							this.contextMenus.Remove(this.stampDeleteItem);
							PdfStampAnnotation pdfStampAnnotation = this.annotationCanvas.SelectedAnnotation as PdfStampAnnotation;
							if (pdfStampAnnotation != null && pdfStampAnnotation.Subject == "FormControl")
							{
								this.contextMenus.Add(this.deleteAnnotItem);
							}
							else
							{
								if (!this.IsApply(this.annotationCanvas.SelectedAnnotation))
								{
									this.contextMenus.Add(this.stampFlattenItem);
									this.contextMenus.Add(this.stampDeleteItem);
								}
								else
								{
									this.contextMenus.Add(this.stampFlattenItem);
									this.contextMenus.Add(this.batchStampFlattenItem);
									this.contextMenus.Add(this.stampDeleteItem);
									this.contextMenus.Add(this.batchStampDeleteItem);
								}
								if (!this.IsApply(this.annotationCanvas.SelectedAnnotation) && this.IsGearStamp(this.annotationCanvas.SelectedAnnotation))
								{
									this.contextMenus.Add(this.stampApplyMulPageItem);
								}
							}
						}
						this.selectAnnotationContextMenu.IsOpen = true;
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x00090CC7 File Offset: 0x0008EEC7
		private bool IsAttachmentAnnotation(PdfAnnotation selectedAnnotation)
		{
			return selectedAnnotation is PdfFileAttachmentAnnotation;
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x00090CD4 File Offset: 0x0008EED4
		private bool IsStampAnnotation(PdfAnnotation selectedAnnotation)
		{
			PdfStampAnnotation pdfStampAnnotation = selectedAnnotation as PdfStampAnnotation;
			return pdfStampAnnotation != null && pdfStampAnnotation.Subject != "Signature";
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x00090D00 File Offset: 0x0008EF00
		private bool IsApply(PdfAnnotation selectedAnnotation)
		{
			PdfStampAnnotation pdfStampAnnotation = selectedAnnotation as PdfStampAnnotation;
			return pdfStampAnnotation != null && pdfStampAnnotation.Dictionary.ContainsKey("ApplyId") && pdfStampAnnotation.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString.Trim().Length > 0;
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x00090D54 File Offset: 0x0008EF54
		private bool IsGearStamp(PdfAnnotation selectedAnnotation)
		{
			PdfStampAnnotation pdfStampAnnotation = selectedAnnotation as PdfStampAnnotation;
			return pdfStampAnnotation != null && (pdfStampAnnotation.Dictionary.ContainsKey("ImgSource") || pdfStampAnnotation.Dictionary.ContainsKey("PDFXExtend"));
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x00090D92 File Offset: 0x0008EF92
		public void Hide()
		{
			this.selectAnnotationContextMenu.IsOpen = false;
		}

		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x0600200F RID: 8207 RVA: 0x00090DA0 File Offset: 0x0008EFA0
		private static bool IsDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x00090DC0 File Offset: 0x0008EFC0
		private static bool CanShow(PdfAnnotation annot)
		{
			if (annot == null)
			{
				return false;
			}
			PdfStampAnnotation pdfStampAnnotation = annot as PdfStampAnnotation;
			return pdfStampAnnotation == null || !(pdfStampAnnotation.Subject == "Signature");
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x00090DF8 File Offset: 0x0008EFF8
		private static bool CanDuplicate(PdfAnnotation annot)
		{
			if (annot == null)
			{
				return false;
			}
			if (annot is PdfTextMarkupAnnotation)
			{
				return false;
			}
			if (annot is PdfTextAnnotation)
			{
				return false;
			}
			PdfStampAnnotation pdfStampAnnotation = annot as PdfStampAnnotation;
			return (pdfStampAnnotation == null || !(pdfStampAnnotation.Subject == "Signature")) && !(annot is PdfFileAttachmentAnnotation);
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x00090E4D File Offset: 0x0008F04D
		private static bool CanCopyOrDeleteText(PdfAnnotation annot)
		{
			return annot is PdfTextMarkupAnnotation;
		}

		// Token: 0x04000CD3 RID: 3283
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CD4 RID: 3284
		private PdfViewerContextMenu selectAnnotationContextMenu;

		// Token: 0x04000CD5 RID: 3285
		private ContextMenuModel contextMenus;

		// Token: 0x04000CD6 RID: 3286
		private ContextMenuItemModel duplicateAnnotItem;

		// Token: 0x04000CD7 RID: 3287
		private ContextMenuItemModel deleteAnnotItem;

		// Token: 0x04000CD8 RID: 3288
		private ContextMenuItemModel deleteAttachmentItem;

		// Token: 0x04000CD9 RID: 3289
		private ContextMenuItemModel copyTextItem;

		// Token: 0x04000CDA RID: 3290
		private ContextMenuItemModel deleteTextItem;

		// Token: 0x04000CDB RID: 3291
		private ContextMenuItemModel saveToPCItem;

		// Token: 0x04000CDC RID: 3292
		private ContextMenuItemModel openFileItem;

		// Token: 0x04000CDD RID: 3293
		private ContextMenuItemModel editDescriptionItem;

		// Token: 0x04000CDE RID: 3294
		private ContextMenuItemModel stampFlattenItem;

		// Token: 0x04000CDF RID: 3295
		private ContextMenuItemModel stampApplyMulPageItem;

		// Token: 0x04000CE0 RID: 3296
		private ContextMenuItemModel batchStampFlattenItem;

		// Token: 0x04000CE1 RID: 3297
		private ContextMenuItemModel stampDeleteItem;

		// Token: 0x04000CE2 RID: 3298
		private ContextMenuItemModel batchStampDeleteItem;
	}
}
