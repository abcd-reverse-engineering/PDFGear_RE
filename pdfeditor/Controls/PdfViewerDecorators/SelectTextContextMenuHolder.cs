using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Copilot.Popups;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.Translation.Popups;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000239 RID: 569
	internal class SelectTextContextMenuHolder
	{
		// Token: 0x0600205D RID: 8285 RVA: 0x000933D3 File Offset: 0x000915D3
		public SelectTextContextMenuHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.InitContextMenu();
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x0600205E RID: 8286 RVA: 0x000933F7 File Offset: 0x000915F7
		private MainViewModel VM
		{
			get
			{
				return this.annotationCanvas.DataContext as MainViewModel;
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x0600205F RID: 8287 RVA: 0x00093409 File Offset: 0x00091609
		// (set) Token: 0x06002060 RID: 8288 RVA: 0x00093411 File Offset: 0x00091611
		public bool ShowRecentColorInContextMenu { get; set; }

		// Token: 0x06002061 RID: 8289 RVA: 0x0009341C File Offset: 0x0009161C
		private void InitContextMenu()
		{
			if (SelectTextContextMenuHolder.IsDesignMode)
			{
				return;
			}
			string text;
			IEnumerable<string> standardStokeColors = ToolbarSettingsHelper.DefaultValues.GetStandardStokeColors(AnnotationMode.Highlight, out text);
			global::System.Collections.Generic.IReadOnlyList<string> standardStokeColors2 = ToolbarSettingsHelper.DefaultValues.GetStandardStokeColors(AnnotationMode.Strike, out text);
			global::System.Collections.Generic.IReadOnlyList<string> standardStokeColors3 = ToolbarSettingsHelper.DefaultValues.GetStandardStokeColors(AnnotationMode.Underline, out text);
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel
			{
				Name = "Copy",
				Caption = Resources.WinScreenshotToolbarCopyContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Copy.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Copy.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextMenuClick))
			};
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.StrokeColor)
			{
				Name = "Highlight",
				Caption = Resources.MenuAnnotateHighlightContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Highlight.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Highlight.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextMenuClick))
			};
			TypedContextMenuItemModel typedContextMenuItemModel2 = new TypedContextMenuItemModel(ContextMenuItemType.StrokeColor)
			{
				Name = "Strike",
				Caption = Resources.MenuAnnotateStrikeContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Strike.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Strike.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextMenuClick))
			};
			TypedContextMenuItemModel typedContextMenuItemModel3 = new TypedContextMenuItemModel(ContextMenuItemType.StrokeColor)
			{
				Name = "Underline",
				Caption = Resources.MenuAnnotateUnderlineContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_Underline.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_Underline.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextMenuClick))
			};
			this.removeTextItem = new ContextMenuItemModel
			{
				Name = "RemoveText",
				Caption = Resources.MenuAnnotateRemoveTextContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Eraser.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Eraser.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextMenuClick))
			};
			ContextMenuTranslateModel contextMenuTranslateModel = new ContextMenuTranslateModel
			{
				Name = "Translate",
				Caption = Resources.MenuAnnotateTranslateContent,
				IsCheckable = true,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/TranslateWords.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/TranslateWords.png")),
				Command = new AsyncRelayCommand<ContextMenuTranslateModel>(new Func<ContextMenuTranslateModel, Task>(this.OnContextMenuClick))
			};
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel();
			contextMenuItemModel2.Name = "AI";
			contextMenuItemModel2.Caption = "AI";
			contextMenuItemModel2.Icon = null;
			contextMenuItemModel2.Add(new ContextMenuItemModel
			{
				Name = "Summarize",
				Caption = "Summarize",
				Icon = null,
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnAIItemClick))
			});
			contextMenuItemModel2.Add(new ContextMenuItemModel
			{
				Name = "Translate",
				Caption = "Translate",
				Icon = null,
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnAIItemClick))
			});
			contextMenuItemModel2.Add(new ContextMenuItemModel
			{
				Name = "Rewrite",
				Caption = "Rewrite",
				Icon = null,
				Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.OnAIItemClick))
			});
			foreach (string text2 in standardStokeColors)
			{
				ContextMenuItemModel contextMenuItemModel3 = ToolbarContextMenuHelper.CreateContextMenuItem(AnnotationMode.Highlight, ContextMenuItemType.StrokeColor, text2, false, new Action<ContextMenuItemModel>(this.OnContextMenuItemClick));
				typedContextMenuItemModel.Add(contextMenuItemModel3);
			}
			foreach (string text3 in standardStokeColors2)
			{
				ContextMenuItemModel contextMenuItemModel4 = ToolbarContextMenuHelper.CreateContextMenuItem(AnnotationMode.Strike, ContextMenuItemType.StrokeColor, text3, false, new Action<ContextMenuItemModel>(this.OnContextMenuItemClick));
				typedContextMenuItemModel2.Add(contextMenuItemModel4);
			}
			foreach (string text4 in standardStokeColors3)
			{
				ContextMenuItemModel contextMenuItemModel5 = ToolbarContextMenuHelper.CreateContextMenuItem(AnnotationMode.Underline, ContextMenuItemType.StrokeColor, text4, false, new Action<ContextMenuItemModel>(this.OnContextMenuItemClick));
				typedContextMenuItemModel3.Add(contextMenuItemModel5);
			}
			ContextMenuModel contextMenuModel = new ContextMenuModel { contextMenuItemModel, typedContextMenuItemModel, typedContextMenuItemModel3, typedContextMenuItemModel2, this.removeTextItem, contextMenuTranslateModel };
			this.selectTextContextMenu = new PdfViewerContextMenu
			{
				ItemsSource = contextMenuModel,
				PlacementTarget = this.annotationCanvas
			};
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x0009389C File Offset: 0x00091A9C
		private void OnContextMenuItemClick(ContextMenuItemModel model)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = model.Parent as TypedContextMenuItemModel;
			if (typedContextMenuItemModel != null)
			{
				ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = null;
				if (typedContextMenuItemModel.Name == "Highlight")
				{
					toolbarAnnotationButtonModel = this.VM.AnnotationToolbar.HighlightButtonModel;
				}
				else if (typedContextMenuItemModel.Name == "Strike")
				{
					toolbarAnnotationButtonModel = this.VM.AnnotationToolbar.StrikeButtonModel;
				}
				else if (typedContextMenuItemModel.Name == "Underline")
				{
					toolbarAnnotationButtonModel = this.VM.AnnotationToolbar.UnderlineButtonModel;
				}
				TagDataModel tagData = model.TagData;
				object obj = ((tagData != null) ? tagData.MenuItemValue : null);
				if (obj == null)
				{
					return;
				}
				ToolbarSettingItemModel toolbarSettingItemModel = toolbarAnnotationButtonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.StrokeColor);
				if (toolbarSettingItemModel != null)
				{
					toolbarSettingItemModel.SelectedValue = obj;
				}
				this.OnContextMenuClick(typedContextMenuItemModel);
			}
		}

		// Token: 0x06002063 RID: 8291 RVA: 0x00093980 File Offset: 0x00091B80
		private async Task OnContextMenuClick(ContextMenuItemModel model)
		{
			this.selectTextContextMenu.IsOpen = false;
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			if (pdfViewer != null && this.annotationCanvas.HasSelectedText())
			{
				if (model.Name == "Copy")
				{
					string selectedText = pdfViewer.SelectedText;
					try
					{
						Clipboard.SetDataObject(selectedText);
					}
					catch
					{
					}
					pdfViewer.DeselectText();
					GAManager.SendEvent("ContextMenu", "Copy", "Count", 1L);
				}
				else if (model.Name == "Highlight")
				{
					this.VM.AnnotationToolbar.HighlightButtonModel.Tap();
					GAManager.SendEvent("ContextMenu", "Highlight", "Count", 1L);
				}
				else if (model.Name == "Strike")
				{
					this.VM.AnnotationToolbar.StrikeButtonModel.Tap();
					GAManager.SendEvent("ContextMenu", "Strike", "Count", 1L);
				}
				else if (model.Name == "Underline")
				{
					this.VM.AnnotationToolbar.UnderlineButtonModel.Tap();
					GAManager.SendEvent("ContextMenu", "Underline", "Count", 1L);
				}
				else if (model.Name == "Translate" && model.IsChecked)
				{
					GAManager.SendEvent("ContextMenu", "Translate", "Enable", 1L);
					GAManager.SendEvent("Translate", "TranslateShow", "ContextMenu", 1L);
					GAManager.SendEvent("Translate", "AutoTranslateSelText", "ContextEnable", 1L);
					this.VM.TranslatePanelVisible = true;
					this.VM.TranslateWords = true;
					string selectedText2 = pdfViewer.SelectedText;
					this.VM.TranslateViewModel.translateAsync(selectedText2, true);
				}
				else if (model.Name == "Translate" && !model.IsChecked)
				{
					GAManager.SendEvent("ContextMenu", "Translate", "Disable", 1L);
					GAManager.SendEvent("Translate", "AutoTranslateSelText", "ContextDisable", 1L);
					this.VM.TranslateWords = false;
				}
				else if (model.Name == "RemoveText")
				{
					GAManager.SendEvent("ContextMenu", "EraseText", "Count", 1L);
					PdfPage currentPage = this.VM.Document.Pages.CurrentPage;
					int startIndex = pdfViewer.SelectInfo.StartIndex;
					int endIndex = pdfViewer.SelectInfo.EndIndex;
					pdfViewer.DeselectText();
					if (startIndex != -1 && endIndex != -1)
					{
						GAManager.SendEvent("EraseText", "EraseText", "ContextMenu", 1L);
						await this.VM.OperationManager.RemoveSelectedTextAsync(currentPage, Math.Min(startIndex, endIndex), Math.Max(startIndex, endIndex), "RemoveSelectedText");
					}
				}
			}
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x000939CC File Offset: 0x00091BCC
		private void OnAIItemClick(ContextMenuItemModel model)
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			if (pdfViewer != null && this.annotationCanvas.HasSelectedText())
			{
				string selectedText = pdfViewer.SelectedText;
				if (!string.IsNullOrEmpty(selectedText) && selectedText.Length > 0 && selectedText.Length < 2000 && model.Name == "Summarize")
				{
					new SummarizePopup(this.VM.CopilotHelper, selectedText).Show();
				}
			}
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x00093A42 File Offset: 0x00091C42
		public void Hide()
		{
			this.selectTextContextMenu.IsOpen = false;
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x00093A50 File Offset: 0x00091C50
		public async Task<bool> ShowAsync(bool autoOpen)
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
				if (!this.annotationCanvas.HasSelectedText())
				{
					flag = false;
				}
				else if (this.selectTextContextMenu.IsOpen)
				{
					flag = false;
				}
				else
				{
					foreach (TypedContextMenuItemModel typedContextMenuItemModel in (this.selectTextContextMenu.ItemsSource as ContextMenuModel).OfType<TypedContextMenuItemModel>())
					{
						AnnotationMode mode = AnnotationMode.None;
						object selectedValue = null;
						string[] array = null;
						if (typedContextMenuItemModel.Name == "Highlight")
						{
							mode = AnnotationMode.Highlight;
							selectedValue = SelectTextContextMenuHolder.<ShowAsync>g__GetToolbarButtonSelectedValue|16_1(this.VM.AnnotationToolbar.HighlightButtonModel, out array);
						}
						else if (typedContextMenuItemModel.Name == "Strike")
						{
							mode = AnnotationMode.Strike;
							selectedValue = SelectTextContextMenuHolder.<ShowAsync>g__GetToolbarButtonSelectedValue|16_1(this.VM.AnnotationToolbar.StrikeButtonModel, out array);
						}
						else if (typedContextMenuItemModel.Name == "Underline")
						{
							mode = AnnotationMode.Underline;
							selectedValue = SelectTextContextMenuHolder.<ShowAsync>g__GetToolbarButtonSelectedValue|16_1(this.VM.AnnotationToolbar.UnderlineButtonModel, out array);
						}
						if (selectedValue != null)
						{
							for (int i = typedContextMenuItemModel.Count - 1; i >= 0; i--)
							{
								ContextMenuItemModel contextMenuItemModel = typedContextMenuItemModel[i] as ContextMenuItemModel;
								if (contextMenuItemModel != null)
								{
									TagDataModel tagData = contextMenuItemModel.TagData;
									if (tagData != null && tagData.IsTransient)
									{
										typedContextMenuItemModel.RemoveAt(i);
									}
								}
							}
							if (this.ShowRecentColorInContextMenu && array != null)
							{
								foreach (string text in array)
								{
									typedContextMenuItemModel.Add(ToolbarContextMenuHelper.CreateContextMenuItem(mode, ContextMenuItemType.StrokeColor, text, true, new Action<ContextMenuItemModel>(this.OnContextMenuItemClick)));
								}
							}
							ContextMenuItemModel contextMenuItemModel2 = typedContextMenuItemModel.OfType<ContextMenuItemModel>().FirstOrDefault(delegate(ContextMenuItemModel c)
							{
								AnnotationMode mode2 = mode;
								ContextMenuItemType contextMenuItemType = ContextMenuItemType.StrokeColor;
								TagDataModel tagData2 = c.TagData;
								return ToolbarContextMenuValueEqualityComparer.MenuValueEquals(mode2, contextMenuItemType, (tagData2 != null) ? tagData2.MenuItemValue : null, selectedValue);
							});
							if (contextMenuItemModel2 != null)
							{
								contextMenuItemModel2.IsChecked = true;
							}
							else
							{
								contextMenuItemModel2 = ToolbarContextMenuHelper.CreateContextMenuItem(mode, ContextMenuItemType.StrokeColor, selectedValue, true, new Action<ContextMenuItemModel>(this.OnContextMenuItemClick));
								typedContextMenuItemModel.Add(contextMenuItemModel2);
								contextMenuItemModel2.IsChecked = true;
							}
						}
					}
					if (autoOpen && !this.VM.TranslateWords)
					{
						this.selectTextContextMenu.AutoCloseOnMouseLeave = true;
						this.UpdateSelectedTextContextMenuPlacementRect();
					}
					else
					{
						this.selectTextContextMenu.AutoCloseOnMouseLeave = false;
						this.selectTextContextMenu.PlacementRectangle = Rect.Empty;
					}
					ContextMenuModel contextMenuModel = this.selectTextContextMenu.ItemsSource as ContextMenuModel;
					contextMenuModel.Remove(this.removeTextItem);
					if (this.IsRemoveTextMenuItemVisible())
					{
						contextMenuModel.Add(this.removeTextItem);
					}
					this.selectTextContextMenu.IsOpen = true;
					string selectedText = this.annotationCanvas.PdfViewer.SelectedText;
					ContextMenuItemModel contextMenuItemModel3 = (this.selectTextContextMenu.ItemsSource as ContextMenuModel).OfType<ContextMenuItemModel>().FirstOrDefault((ContextMenuItemModel x) => x.Name == "Translate");
					if (contextMenuItemModel3 != null)
					{
						if (this.VM.TranslateWords)
						{
							contextMenuItemModel3.IsChecked = true;
						}
						else
						{
							contextMenuItemModel3.IsChecked = false;
						}
						if (contextMenuItemModel3.IsChecked)
						{
							if (!this.VM.TranslatePanelVisible)
							{
								this.VM.TranslatePanelVisible = true;
							}
							this.VM.TranslateViewModel.translateAsync(selectedText, true);
						}
					}
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x00093A9C File Offset: 0x00091C9C
		private void UpdateSelectedTextContextMenuPlacementRect()
		{
			Rect[] selectedTextRect = this.GetSelectedTextRect();
			if (selectedTextRect.Length != 0)
			{
				Point position = Mouse.GetPosition(this.annotationCanvas.PdfViewer);
				bool flag = false;
				Rect rect = Rect.Empty;
				double num = double.MaxValue;
				foreach (Rect rect2 in selectedTextRect)
				{
					if (rect2.Contains(position))
					{
						this.selectTextContextMenu.PlacementRectangle = rect2;
						flag = true;
						break;
					}
					double num2 = Math.Min(Math.Min(Math.Abs(rect2.Left - position.X), Math.Abs(rect2.Right - position.X)), Math.Min(Math.Abs(rect2.Top - position.Y), Math.Abs(rect2.Bottom - position.Y)));
					if (num2 < num)
					{
						num = num2;
						rect = rect2;
					}
				}
				if (!flag)
				{
					this.selectTextContextMenu.PlacementRectangle = rect;
				}
			}
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x00093B9C File Offset: 0x00091D9C
		private Rect[] GetSelectedTextRect()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			PdfDocument pdfDocument = ((pdfViewer != null) ? pdfViewer.Document : null);
			if (pdfDocument == null)
			{
				return Array.Empty<Rect>();
			}
			if (!this.annotationCanvas.HasSelectedText())
			{
				return Array.Empty<Rect>();
			}
			SelectInfo selectInfo = pdfViewer.SelectInfo;
			List<Rect> list = new List<Rect>();
			for (int i = selectInfo.StartPage; i <= selectInfo.EndPage; i++)
			{
				try
				{
					int num = 0;
					if (i == selectInfo.StartPage)
					{
						num = selectInfo.StartIndex;
					}
					int num2;
					if (i == selectInfo.EndPage)
					{
						num2 = selectInfo.EndIndex;
					}
					else
					{
						num2 = pdfDocument.Pages[i].Text.CountChars - 1;
					}
					int num3 = num2 - num + 1;
					if (num3 > 0)
					{
						PdfTextInfo textInfo = pdfViewer.Document.Pages[i].Text.GetTextInfo(num, num3);
						if (textInfo.Rects.Count > 0)
						{
							FS_RECTF fs_RECTF = textInfo.Rects[0];
							for (int j = 1; j < textInfo.Rects.Count; j++)
							{
								FS_RECTF fs_RECTF2 = textInfo.Rects[j];
								fs_RECTF.left = Math.Min(fs_RECTF.left, fs_RECTF2.left);
								fs_RECTF.right = Math.Max(fs_RECTF.right, fs_RECTF2.right);
								fs_RECTF.top = Math.Max(fs_RECTF.top, fs_RECTF2.top);
								fs_RECTF.bottom = Math.Min(fs_RECTF.bottom, fs_RECTF2.bottom);
							}
							Rect rect;
							if (pdfViewer.TryGetClientRect(i, fs_RECTF, out rect))
							{
								list.Add(rect);
							}
						}
					}
				}
				catch
				{
				}
			}
			return list.ToArray();
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06002069 RID: 8297 RVA: 0x00093D7C File Offset: 0x00091F7C
		private static bool IsDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x00093D9C File Offset: 0x00091F9C
		private bool IsRemoveTextMenuItemVisible()
		{
			AnnotationCanvas annotationCanvas = this.annotationCanvas;
			PdfViewer pdfViewer = ((annotationCanvas != null) ? annotationCanvas.PdfViewer : null);
			if (((pdfViewer != null) ? pdfViewer.Document : null) == null)
			{
				return false;
			}
			SelectInfo selectInfo = pdfViewer.SelectInfo;
			return selectInfo.StartPage == selectInfo.EndPage && selectInfo.StartPage >= 0 && selectInfo.StartPage < pdfViewer.Document.Pages.Count && PageContentUtils.CanRemoveSelectedText(pdfViewer.Document.Pages[selectInfo.StartPage], Math.Min(selectInfo.StartIndex, selectInfo.EndPage), Math.Max(selectInfo.StartIndex, selectInfo.EndPage));
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x00093E44 File Offset: 0x00092044
		[CompilerGenerated]
		internal static object <ShowAsync>g__GetToolbarButtonSelectedValue|16_1(ToolbarAnnotationButtonModel _button, out string[] recentColors)
		{
			recentColors = null;
			ToolbarSettingModel toolbarSettingModel = _button.ToolbarSettingModel;
			object obj;
			if (toolbarSettingModel == null)
			{
				obj = null;
			}
			else
			{
				obj = toolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.StrokeColor);
			}
			object obj2 = obj;
			ToolbarSettingItemColorModel toolbarSettingItemColorModel = obj2 as ToolbarSettingItemColorModel;
			if (toolbarSettingItemColorModel != null)
			{
				ObservableCollection<string> recentColors2 = toolbarSettingItemColorModel.RecentColors;
				recentColors = ((recentColors2 != null) ? recentColors2.ToArray<string>() : null);
			}
			return ((obj2 != null) ? obj2.SelectedValue : null) ?? ToolbarContextMenuHelper.GetDefaultValue(_button.Mode, ContextMenuItemType.StrokeColor);
		}

		// Token: 0x04000CF6 RID: 3318
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CF7 RID: 3319
		private PdfViewerContextMenu selectTextContextMenu;

		// Token: 0x04000CF8 RID: 3320
		private ContextMenuItemModel removeTextItem;

		// Token: 0x04000CF9 RID: 3321
		private TranslationWin translationWin;
	}
}
