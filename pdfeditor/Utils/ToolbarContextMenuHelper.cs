using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.Speech;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using PDFKit.Utils;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Utils
{
	// Token: 0x020000A8 RID: 168
	public static class ToolbarContextMenuHelper
	{
		// Token: 0x06000A65 RID: 2661 RVA: 0x00035360 File Offset: 0x00033560
		static ToolbarContextMenuHelper()
		{
			ToolbarContextMenuHelper.InitAllAnnotationMode();
			ToolbarContextMenuHelper.InitFillColorMenuValues();
			ToolbarContextMenuHelper.InitStrokeColorMenuValues();
			ToolbarContextMenuHelper.InitStrokeThicknessMenuValues();
			ToolbarContextMenuHelper.InitFontColorMenuValues();
			ToolbarContextMenuHelper.InitFontSizeMenuValue();
			ToolbarContextMenuHelper.InitStampPresetsMenuValues();
			ToolbarContextMenuHelper.InitViewerBackgroundColorValues();
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x0003538C File Offset: 0x0003358C
		public static IContextMenuModel CreateStrokeColorMenu(AnnotationMode mode, Action<ContextMenuItemModel> action, bool addMoreItem = false)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.StrokeColor)
			{
				Name = "Color",
				Caption = Resources.LabelColorContent,
				IsChecked = true,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode,
					MenuItemType = ContextMenuItemType.StrokeColor
				}
			};
			foreach (IContextMenuModel contextMenuModel in ToolbarContextMenuHelper.CreateContextMenuItems(mode, ContextMenuItemType.StrokeColor, action))
			{
				typedContextMenuItemModel.Add(contextMenuModel);
			}
			if (addMoreItem)
			{
				typedContextMenuItemModel.Add(ToolbarContextMenuHelper.CreateColorMoreItem(mode, ContextMenuItemType.StrokeColor, action));
			}
			return typedContextMenuItemModel;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00035440 File Offset: 0x00033640
		public static IContextMenuModel CreateThicknessMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.StrokeThickness)
			{
				Name = "Thickness",
				Caption = Resources.MenuSubThicknessItem,
				IsChecked = true,
				TagData = new TagDataModel
				{
					AnnotationMode = mode,
					MenuItemType = ContextMenuItemType.StrokeThickness
				},
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linewidth.png"))
			};
			foreach (IContextMenuModel contextMenuModel in ToolbarContextMenuHelper.CreateContextMenuItems(mode, ContextMenuItemType.StrokeThickness, action))
			{
				typedContextMenuItemModel.Add(contextMenuModel);
			}
			return typedContextMenuItemModel;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x000354E4 File Offset: 0x000336E4
		public static IContextMenuModel CreateFillMenu(AnnotationMode mode, Action<ContextMenuItemModel> action, bool addMoreItem = false)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.FillColor)
			{
				Name = "FillColor",
				Caption = Resources.MenuSubFillColorItem,
				IsChecked = true,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode,
					MenuItemType = ContextMenuItemType.FillColor
				}
			};
			foreach (IContextMenuModel contextMenuModel in ToolbarContextMenuHelper.CreateContextMenuItems(mode, ContextMenuItemType.FillColor, action))
			{
				typedContextMenuItemModel.Add(contextMenuModel);
			}
			if (addMoreItem)
			{
				typedContextMenuItemModel.Add(ToolbarContextMenuHelper.CreateColorMoreItem(mode, ContextMenuItemType.FillColor, action));
			}
			return typedContextMenuItemModel;
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00035598 File Offset: 0x00033798
		public static IContextMenuModel CreateFontColorMenu(AnnotationMode mode, Action<ContextMenuItemModel> action, bool addMoreItem = false)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.FontColor)
			{
				Name = "FontColor",
				Caption = Resources.MenuSubFontColorItem,
				IsChecked = true,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode,
					MenuItemType = ContextMenuItemType.FontColor
				}
			};
			foreach (IContextMenuModel contextMenuModel in ToolbarContextMenuHelper.CreateContextMenuItems(mode, ContextMenuItemType.FontColor, action))
			{
				typedContextMenuItemModel.Add(contextMenuModel);
			}
			if (addMoreItem)
			{
				typedContextMenuItemModel.Add(ToolbarContextMenuHelper.CreateColorMoreItem(mode, ContextMenuItemType.FontColor, action));
			}
			return typedContextMenuItemModel;
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x0003564C File Offset: 0x0003384C
		public static IContextMenuModel CreateFontSizeMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			TypedContextMenuItemModel typedContextMenuItemModel = new TypedContextMenuItemModel(ContextMenuItemType.FontSize)
			{
				Name = "FontSize",
				Caption = Resources.MenuSubFontSizeItem,
				IsChecked = true,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode,
					MenuItemType = ContextMenuItemType.FontSize
				}
			};
			foreach (IContextMenuModel contextMenuModel in ToolbarContextMenuHelper.CreateContextMenuItems(mode, ContextMenuItemType.FontSize, action))
			{
				typedContextMenuItemModel.Add(contextMenuModel);
			}
			return typedContextMenuItemModel;
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x000356F0 File Offset: 0x000338F0
		public static ContextMenuItemModel GetDefaultMenuItem(AnnotationMode mode, TypedContextMenuItemModel menu)
		{
			if (mode == AnnotationMode.None)
			{
				return null;
			}
			if (menu == null)
			{
				return null;
			}
			ContextMenuItemType type = menu.Type;
			object defaultValue = ToolbarContextMenuHelper.GetDefaultValue(mode, type);
			return menu.OfType<ContextMenuItemModel>().FirstOrDefault(delegate(ContextMenuItemModel c)
			{
				AnnotationMode mode2 = mode;
				ContextMenuItemType type2 = type;
				object defaultValue2 = defaultValue;
				TagDataModel tagData = c.TagData;
				return ToolbarContextMenuValueEqualityComparer.MenuValueEquals(mode2, type2, defaultValue2, (tagData != null) ? tagData.MenuItemValue : null);
			});
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00035754 File Offset: 0x00033954
		public static IContextMenuModel CreateAddImgMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Insert local image",
				Caption = Resources.MenuStampSubInsertlocalImageContent,
				IsChecked = false,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/addimg.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x000357BC File Offset: 0x000339BC
		public static IContextMenuModel CreatePresetsMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			StampContextMenuItemModel stampContextMenuItemModel = new StampContextMenuItemModel
			{
				Name = "Presets",
				Caption = Resources.MenuStampSubPresetsContent,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/presets.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/presets.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				}
			};
			foreach (IStampTextModel stampTextModel in ToolbarContextMenuHelper.stampPresetsMenuValues)
			{
				PresetsItemContextMenuItemModel presetsItemContextMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItemCore2<PresetsItemContextMenuItemModel>(mode, ContextMenuItemType.None, stampTextModel.TextContent, stampTextModel, false, false, action);
				stampContextMenuItemModel.Add(presetsItemContextMenuItemModel);
			}
			return stampContextMenuItemModel;
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00035854 File Offset: 0x00033A54
		public static IContextMenuModel ManageStampMenu(string CategroyName, AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			StampContextMenuItemModel stampContextMenuItemModel = new StampContextMenuItemModel();
			stampContextMenuItemModel.Name = "CustomStamp";
			stampContextMenuItemModel.Caption = CategroyName;
			stampContextMenuItemModel.IsChecked = false;
			stampContextMenuItemModel.TagData = new TagDataModel
			{
				AnnotationMode = mode
			};
			StampCustomMenuItemModel stampCustomMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItemCore2<StampCustomMenuItemModel>(mode, ContextMenuItemType.None, CategroyName, null, false, false, action);
			stampContextMenuItemModel.Add(stampCustomMenuItemModel);
			return stampContextMenuItemModel;
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x000358A8 File Offset: 0x00033AA8
		public static IContextMenuModel CreteStampMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "CustomStamp",
				Caption = Resources.EditStampWinTitle,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Stamp/MenuStampAdd.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Stamp/MenuStampAdd.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0003591C File Offset: 0x00033B1C
		public static IContextMenuModel CreteManageStampMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "ManageStamp",
				Caption = Resources.ManageStampWinTitle,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Stamp/ManageImage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Stamp/ManageImage.png")),
				IsChecked = false,
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00035990 File Offset: 0x00033B90
		public static IContextMenuModel SpeakCurrent(Action<ContextMenuItemModel> action)
		{
			SpeedContextMenuItemModel speedContextMenuItemModel = new SpeedContextMenuItemModel();
			speedContextMenuItemModel.Name = "Read Current Page";
			speedContextMenuItemModel.Caption = Resources.ReadCurrentBtn;
			speedContextMenuItemModel.IsChecked = false;
			speedContextMenuItemModel.IsEnabled = false;
			speedContextMenuItemModel.Command = new RelayCommand(delegate
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
				ContextMenuModel contextMenuModel = (requiredService.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = true;
				if (requiredService.speechUtils != null)
				{
					requiredService.speechUtils.Activated();
					requiredService.speechUtils.SpeakCurrentPage(requiredService.CurrnetPageIndex - 1);
				}
				else
				{
					requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
					PdfDocument pdfDocument;
					if (requiredService.Document != null)
					{
						pdfDocument = requiredService.Document;
					}
					else
					{
						pdfDocument = null;
					}
					requiredService.IsReading = true;
					SpeechUtils speechUtils = requiredService.speechUtils;
					if (speechUtils != null)
					{
						speechUtils.Dispose();
					}
					requiredService.speechUtils = new SpeechUtils(pdfDocument);
					if (requiredService.speechControl != null)
					{
						requiredService.speechUtils.Rate = requiredService.speechControl.SpeedSli.Value * 2.0 - 10.0;
						requiredService.speechUtils.SpeechVolume = (float)Convert.ToInt32(requiredService.speechControl.VolumeSlider.Value);
						requiredService.speechUtils.Pitch = (double)Convert.ToInt32(requiredService.speechControl.ToneSli.Value - 5.0);
						if (requiredService.speechControl.CultureListBox.SelectedIndex < 0)
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechUtils.GetcultureIndex();
						}
						else
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechControl.CultureListBox.SelectedIndex;
						}
					}
					requiredService.speechUtils.SpeakCurrentPage(requiredService.CurrnetPageIndex - 1);
				}
				GAManager.SendEvent("PDFReader", "Read", "CurrentPage", 1L);
			});
			return speedContextMenuItemModel;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x000359F0 File Offset: 0x00033BF0
		public static IContextMenuModel SpeakFormCurrent(Action<ContextMenuItemModel> action)
		{
			SpeedContextMenuItemModel speedContextMenuItemModel = new SpeedContextMenuItemModel();
			speedContextMenuItemModel.Name = "Read From Current Page";
			speedContextMenuItemModel.Caption = Resources.ReadFromCurrentBtn;
			speedContextMenuItemModel.IsChecked = false;
			speedContextMenuItemModel.IsEnabled = false;
			speedContextMenuItemModel.Command = new RelayCommand(delegate
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
				ContextMenuModel contextMenuModel = (requiredService.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = true;
				if (requiredService.speechUtils != null)
				{
					requiredService.speechUtils.Activated();
					requiredService.speechUtils.SpeakPages(requiredService.CurrnetPageIndex - 1, requiredService.Document.Pages.Count - 1);
				}
				else
				{
					requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
					PdfDocument pdfDocument;
					if (requiredService.Document != null)
					{
						pdfDocument = requiredService.Document;
					}
					else
					{
						pdfDocument = null;
					}
					requiredService.IsReading = true;
					SpeechUtils speechUtils = requiredService.speechUtils;
					if (speechUtils != null)
					{
						speechUtils.Dispose();
					}
					requiredService.speechUtils = new SpeechUtils(pdfDocument);
					if (requiredService.speechControl != null)
					{
						requiredService.speechUtils.Rate = requiredService.speechControl.SpeedSli.Value * 2.0 - 10.0;
						requiredService.speechUtils.SpeechVolume = (float)Convert.ToInt32(requiredService.speechControl.VolumeSlider.Value);
						requiredService.speechUtils.Pitch = (double)Convert.ToInt32(requiredService.speechControl.ToneSli.Value - 5.0);
						if (requiredService.speechControl.CultureListBox.SelectedIndex < 0)
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechUtils.GetcultureIndex();
						}
						else
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechControl.CultureListBox.SelectedIndex;
						}
					}
					requiredService.speechUtils.SpeakPages(requiredService.CurrnetPageIndex - 1, requiredService.Document.Pages.Count - 1);
				}
				GAManager.SendEvent("PDFReader", "Read", "FromCurrentPage", 1L);
			});
			return speedContextMenuItemModel;
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x00035A50 File Offset: 0x00033C50
		public static IContextMenuModel SpeakAll(Action<ContextMenuItemModel> action)
		{
			SpeedContextMenuItemModel speedContextMenuItemModel = new SpeedContextMenuItemModel();
			speedContextMenuItemModel.Name = "Read All Pages";
			speedContextMenuItemModel.Caption = Resources.ReadAllBtn;
			speedContextMenuItemModel.IsChecked = false;
			speedContextMenuItemModel.IsEnabled = false;
			speedContextMenuItemModel.Command = new RelayCommand(delegate
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
				ContextMenuModel contextMenuModel = (requiredService.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = false;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = false;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = true;
				if (requiredService.speechUtils != null)
				{
					requiredService.speechUtils.Activated();
					requiredService.speechUtils.SpeakPages(0, requiredService.Document.Pages.Count - 1);
				}
				else
				{
					if (requiredService.Document == null)
					{
						return;
					}
					requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
					PdfDocument pdfDocument;
					if (requiredService.Document != null)
					{
						pdfDocument = requiredService.Document;
					}
					else
					{
						pdfDocument = null;
					}
					requiredService.IsReading = true;
					SpeechUtils speechUtils = requiredService.speechUtils;
					if (speechUtils != null)
					{
						speechUtils.Dispose();
					}
					requiredService.speechUtils = new SpeechUtils(pdfDocument);
					if (requiredService.speechControl != null)
					{
						requiredService.speechUtils.Rate = requiredService.speechControl.SpeedSli.Value * 2.0 - 10.0;
						requiredService.speechUtils.SpeechVolume = (float)Convert.ToInt32(requiredService.speechControl.VolumeSlider.Value);
						requiredService.speechUtils.Pitch = (double)Convert.ToInt32(requiredService.speechControl.ToneSli.Value - 5.0);
						if (requiredService.speechControl.CultureListBox.SelectedIndex < 0)
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechUtils.GetcultureIndex();
						}
						else
						{
							requiredService.speechUtils.CultureIndex = requiredService.speechControl.CultureListBox.SelectedIndex;
						}
					}
					requiredService.speechUtils.SpeakPages(0, requiredService.Document.Pages.Count - 1);
				}
				GAManager.SendEvent("PDFReader", "Read", "AllPages", 1L);
			});
			return speedContextMenuItemModel;
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00035AB0 File Offset: 0x00033CB0
		public static IContextMenuModel SpeechToolbarMenu(Action<ContextMenuItemModel> action)
		{
			SpeechToolContextMenuItemModel speechToolContextMenuItemModel = new SpeechToolContextMenuItemModel();
			speechToolContextMenuItemModel.Name = "ToolBar";
			speechToolContextMenuItemModel.Caption = Resources.ReadToolBarBtn;
			speechToolContextMenuItemModel.IsEnabled = true;
			speechToolContextMenuItemModel.IsChecked = false;
			speechToolContextMenuItemModel.Command = new RelayCommand(delegate
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				bool flag = false;
				using (IEnumerator enumerator = Application.Current.Windows.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((Window)enumerator.Current).GetType() == typeof(SpeechControl))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					requiredService.speechControl.Close();
					return;
				}
				requiredService.speechControl = new SpeechControl();
				requiredService.speechControl.Owner = App.Current.MainWindow;
				if (requiredService.speechControl.Owner.WindowState == WindowState.Normal)
				{
					requiredService.speechControl.Top = requiredService.speechControl.Owner.Top + 152.0;
					requiredService.speechControl.Left = requiredService.speechControl.Owner.Left + requiredService.speechControl.Owner.ActualWidth - 520.0;
				}
				else if (requiredService.speechControl.Owner.WindowState == WindowState.Maximized)
				{
					requiredService.speechControl.Top = 152.0;
					requiredService.speechControl.Left = requiredService.speechControl.Owner.ActualWidth - 520.0;
				}
				requiredService.speechControl.Show();
			});
			return speechToolContextMenuItemModel;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x00035B10 File Offset: 0x00033D10
		public static IContextMenuModel CreateCustomMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			StampContextMenuItemModel stampContextMenuItemModel = new StampContextMenuItemModel
			{
				IsChecked = false,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/add.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
			if (mode == AnnotationMode.Stamp)
			{
				stampContextMenuItemModel.Name = "Customize stamp";
				stampContextMenuItemModel.Caption = Resources.MenuStampSubCustomizeContent;
			}
			else
			{
				stampContextMenuItemModel.Name = "Customize";
				stampContextMenuItemModel.Caption = "Customize";
			}
			return stampContextMenuItemModel;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x00035B98 File Offset: 0x00033D98
		public static IContextMenuModel CreateShareEmailMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Email",
				Caption = Resources.MenuShareEmailContent,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/mail.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/mail.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x00035C0C File Offset: 0x00033E0C
		public static IContextMenuModel CreateShareMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Share",
				Caption = Resources.MenuShareShareContent,
				IsChecked = false,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/share.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x00035C74 File Offset: 0x00033E74
		public static IContextMenuModel CreateAddLinkMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Create/Edit Link",
				Caption = Resources.LinkCreateBtn,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/LinkCE.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action)),
				HotKeyInvokeWhen = "Editor_CreateOrEditLink"
			};
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00035CF4 File Offset: 0x00033EF4
		public static IContextMenuModel CreateDeleteAllLinkMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "DeleteAllLink",
				Caption = Resources.LinkDeleteAllBtn,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/deleteLink.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/deleteLink.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action)),
				HotKeyInvokeWhen = "Editor_DeleteLink"
			};
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x00035D74 File Offset: 0x00033F74
		public static IContextMenuModel TranslateForWordsMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Translation of words",
				Caption = "Translation of words",
				IsChecked = false,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00035DDC File Offset: 0x00033FDC
		public static IContextMenuModel OpenTranslateMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Translations",
				Caption = Resources.TranslatePanelOutputPlaceHolder,
				IsChecked = false,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00035E44 File Offset: 0x00034044
		public static IContextMenuModel CreateShareFileMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Sharefile",
				Caption = Resources.MenuShareSendFileContent,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/file.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/file.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x00035EB6 File Offset: 0x000340B6
		public static IContextMenuModel CreateSignatureMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return ToolbarContextMenuHelper.CreateSignatureItem(mode, ContextMenuItemType.None, action);
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x00035EC0 File Offset: 0x000340C0
		public static IContextMenuModel CreateAddWatermarkMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Create Watermark",
				Caption = Resources.MenuWatermarkSubCreateContent,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/addwatermark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/addwatermark.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action)),
				HotKeyInvokeWhen = "Editor_CreateWatermark"
			};
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00035F40 File Offset: 0x00034140
		public static IContextMenuModel CreateDeleteCurrentPageWatermarkMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "DeleteCurrentPageWatermark",
				Caption = "Delete current page watermark",
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/addwatermark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/addwatermark.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00035FB4 File Offset: 0x000341B4
		public static IContextMenuModel CreateDeleteAllWatermarkMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "DeleteAllWatermark",
				Caption = Resources.MenuWatermarkSubDeleteContent,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/addwatermark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/addwatermark.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action)),
				HotKeyInvokeWhen = "Editor_DeleteWatermark"
			};
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x00036034 File Offset: 0x00034234
		public static IContextMenuModel CreateAddAttachmentMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Add Attachment",
				Caption = Resources.AttachmentPanelBtnAddAttachmentsText,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/AddAttachment.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/AddAttachment.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x000360A8 File Offset: 0x000342A8
		public static IContextMenuModel OpenAttachmentManagerMenu(AnnotationMode mode, Action<ContextMenuItemModel> action)
		{
			return new StampContextMenuItemModel
			{
				Name = "Open Attachment Manager",
				Caption = Resources.AttachmentToolbarBtnContextMenuOpenAttachmentManager,
				IsChecked = false,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/AttachmentManager.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/AttachmentManager.png")),
				TagData = new TagDataModel
				{
					AnnotationMode = mode
				},
				Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
			};
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0003611C File Offset: 0x0003431C
		public static SelectableContextMenuItemModel CreateConverterContextMenu(Action<ContextMenuItemModel> action)
		{
			BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/wordmenu.png"));
			BitmapImage bitmapImage2 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/excelmenu.png"));
			BitmapImage bitmapImage3 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/pptmenu.png"));
			BitmapImage bitmapImage4 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/imagemenu.png"));
			BitmapImage bitmapImage5 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Rtfmenu.png"));
			BitmapImage bitmapImage6 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/txtmenu.png"));
			BitmapImage bitmapImage7 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Htmlmenu.png"));
			BitmapImage bitmapImage8 = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Xmlmenu.png"));
			return new SelectableContextMenuItemModel
			{
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoWord", Resources.MenuConvertPdfToWordContent, bitmapImage, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoExcel", Resources.MenuConvertPdfToExcelContent, bitmapImage2, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoPPT", Resources.MenuConvertPdfToPPTContent, bitmapImage3, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoImage", Resources.MenuConvertPdfToImageContent, bitmapImage4, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoJpeg", Resources.MenuConvertPdfToJpegContent, bitmapImage4, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoText", Resources.MenuConvertPdfToTxtContent, bitmapImage6, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoHtml", Resources.MenuConvertPdfToHtmlContent, bitmapImage7, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoRtf", Resources.MenuConvertPdfToRtfContent, bitmapImage5, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PDFtoXml", Resources.MenuConvertPdfToXmlContent, bitmapImage8, action),
				new ContextMenuSeparator(),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("WordtoPDF", Resources.WinConvertToolBtnWordToPDF, bitmapImage, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("ExceltoPDF", Resources.WinConvertToolBtnExcelToPDFText, bitmapImage2, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("ImagetoPDF", Resources.WinConvertToolBtnImageToPDFText, bitmapImage4, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("PPTtoPDF", Resources.WinConvertToolBtnPPTToPDFText, bitmapImage3, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("RtftoPDF", Resources.WinConvertToolBtnRTFToPDFText, bitmapImage5, action),
				ToolbarContextMenuHelper.<CreateConverterContextMenu>g__CreateItem|34_0("TxttoPDF", Resources.WinConvertToolBtnTXTToPDFText, bitmapImage6, action)
			};
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0003631C File Offset: 0x0003451C
		public static SelectableContextMenuItemModel CreateBackgroundContextMenu(string selectedName, Action<ContextMenuItemModel> action)
		{
			SelectableContextMenuItemModel selectableContextMenuItemModel = new SelectableContextMenuItemModel();
			if (!ToolbarContextMenuHelper.viewerBackgroundColorDict.ContainsKey(selectedName))
			{
				selectedName = ToolbarContextMenuHelper.viewerBackgroundColorValues[0].Name;
			}
			foreach (BackgroundColorSetting backgroundColorSetting in ToolbarContextMenuHelper.viewerBackgroundColorValues)
			{
				selectableContextMenuItemModel.Add(new BackgroundContextMenuItemModel
				{
					Name = backgroundColorSetting.Name,
					Caption = backgroundColorSetting.DisplayName,
					IsChecked = (backgroundColorSetting.Name == selectedName),
					Icon = null,
					TagData = new TagDataModel
					{
						MenuItemValue = backgroundColorSetting
					},
					Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action))
				});
			}
			return selectableContextMenuItemModel;
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x000363C4 File Offset: 0x000345C4
		public static SelectableContextMenuItemModel CreateAutoScrollContextMenu(int selectedValue, Action<ContextMenuItemModel> action)
		{
			ToolbarContextMenuHelper.<>c__DisplayClass36_0 CS$<>8__locals1;
			CS$<>8__locals1.selectedValue = selectedValue;
			CS$<>8__locals1.action = action;
			CS$<>8__locals1.model = new SelectableContextMenuItemModel();
			if (CS$<>8__locals1.selectedValue < -4 || CS$<>8__locals1.selectedValue == 0 || CS$<>8__locals1.selectedValue > 4)
			{
				CS$<>8__locals1.selectedValue = 1;
			}
			for (int i = -4; i < 0; i++)
			{
				ToolbarContextMenuHelper.<CreateAutoScrollContextMenu>g__AddItem|36_0(i, ref CS$<>8__locals1);
			}
			for (int j = 1; j <= 4; j++)
			{
				ToolbarContextMenuHelper.<CreateAutoScrollContextMenu>g__AddItem|36_0(j, ref CS$<>8__locals1);
			}
			return CS$<>8__locals1.model;
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00036440 File Offset: 0x00034640
		public static object GetDefaultValue(AnnotationMode mode, ContextMenuItemType type)
		{
			global::System.Collections.Generic.IReadOnlyList<ToolbarContextMenuHelper.MenuValueProvider> values = ToolbarContextMenuHelper.GetValues(type);
			if (values == null || values.Count == 0)
			{
				return null;
			}
			ToolbarContextMenuHelper.MenuValueProvider menuValueProvider = values.FirstOrDefault((ToolbarContextMenuHelper.MenuValueProvider c) => c.IsDefaultValue(mode));
			if (menuValueProvider == null)
			{
				return null;
			}
			return menuValueProvider.Value;
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0003648C File Offset: 0x0003468C
		public static ContextMenuItemModel CreateContextMenuItem(AnnotationMode mode, ContextMenuItemType type, object value, bool isTransient, Action<ContextMenuItemModel> action)
		{
			string text;
			object obj;
			if (ToolbarContextMenuHelper.TryParseMenuValue(mode, type, value, out text, out obj))
			{
				return ToolbarContextMenuHelper.CreateContextMenuItemCore(mode, type, text, obj, false, isTransient, action);
			}
			return null;
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x000364B6 File Offset: 0x000346B6
		private static ContextMenuItemModel CreateContextMenuItemCore(AnnotationMode mode, ContextMenuItemType type, string caption, object value, bool isChecked, bool isTransient, Action<ContextMenuItemModel> action)
		{
			return ToolbarContextMenuHelper.CreateContextMenuItemCore2<ContextMenuItemModel>(mode, type, caption, value, isChecked, isTransient, action);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x000364C8 File Offset: 0x000346C8
		private static T CreateContextMenuItemCore2<T>(AnnotationMode mode, ContextMenuItemType type, string caption, object value, bool isChecked, bool isTransient, Action<ContextMenuItemModel> action) where T : ContextMenuItemModel, new()
		{
			T t = new T();
			t.Name = "";
			t.Caption = caption;
			t.IsChecked = isChecked;
			t.TagData = new TagDataModel(isTransient)
			{
				MenuItemType = type,
				MenuItemValue = value,
				AnnotationMode = mode
			};
			t.Command = ((action == null) ? null : new RelayCommand<ContextMenuItemModel>(action));
			return t;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00036544 File Offset: 0x00034744
		public static IEnumerable<IContextMenuModel> CreateContextMenuItems(AnnotationMode mode, ContextMenuItemType type, Action<ContextMenuItemModel> action)
		{
			global::System.Collections.Generic.IReadOnlyList<ToolbarContextMenuHelper.MenuValueProvider> values = ToolbarContextMenuHelper.GetValues(type);
			if (values == null || values.Count == 0)
			{
				return Enumerable.Empty<IContextMenuModel>();
			}
			return from c in values
				where c.IsValueOwner(mode)
				select ToolbarContextMenuHelper.CreateContextMenuItemCore(mode, type, c.Caption, c.Value, c.IsDefaultValue(mode), false, action);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x000365AC File Offset: 0x000347AC
		public static ContextMenuItemModel CreateColorMoreItem(AnnotationMode mode, ContextMenuItemType type, Action<ContextMenuItemModel> action)
		{
			ColorMoreItemContextMenuItemModel colorMoreItemContextMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItemCore2<ColorMoreItemContextMenuItemModel>(mode, type, "More", "More", false, false, action);
			colorMoreItemContextMenuItemModel.Name = "More";
			colorMoreItemContextMenuItemModel.RecentColorsKey = string.Format("{0}_{1}", mode, type);
			if (type == ContextMenuItemType.StrokeColor || type == ContextMenuItemType.FillColor || type == ContextMenuItemType.FontColor)
			{
				ContextMenuItemModel contextMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItemCore(mode, type, "ColorPicker", "ColorPicker", false, false, null);
				contextMenuItemModel.Name = "ColorPicker";
				colorMoreItemContextMenuItemModel.Add(contextMenuItemModel);
			}
			return colorMoreItemContextMenuItemModel;
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00036629 File Offset: 0x00034829
		private static ContextMenuItemModel CreateSignatureItem(AnnotationMode mode, ContextMenuItemType type, Action<ContextMenuItemModel> action)
		{
			ContextMenuItemModel contextMenuItemModel = ToolbarContextMenuHelper.CreateContextMenuItemCore(mode, type, "SignaturePicker", "SignaturePicker", false, false, null);
			contextMenuItemModel.Name = "SignaturePicker";
			return contextMenuItemModel;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0003664C File Offset: 0x0003484C
		private static global::System.Collections.Generic.IReadOnlyList<ToolbarContextMenuHelper.MenuValueProvider> GetValues(ContextMenuItemType type)
		{
			ToolbarContextMenuHelper.MenuValueProvider[] array = null;
			if (type == ContextMenuItemType.FillColor)
			{
				array = ToolbarContextMenuHelper.fillColorMenuValues;
			}
			else if (type == ContextMenuItemType.StrokeColor)
			{
				array = ToolbarContextMenuHelper.strokeColorMenuValues;
			}
			else if (type == ContextMenuItemType.StrokeThickness)
			{
				array = ToolbarContextMenuHelper.strokeThicknessMenuValues;
			}
			else if (type == ContextMenuItemType.FontColor)
			{
				array = ToolbarContextMenuHelper.fontColorMenuValues;
			}
			else if (type == ContextMenuItemType.FontSize)
			{
				array = ToolbarContextMenuHelper.fontSizeMenuValues;
			}
			if (array == null)
			{
				return Array.Empty<ToolbarContextMenuHelper.MenuValueProvider>();
			}
			return array;
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x000366A0 File Offset: 0x000348A0
		public static List<StampTextModel> ReadStamp()
		{
			string text = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Path.Combine(text, "Stamp.json");
			if (!File.Exists(text2))
			{
				return null;
			}
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
			jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
			jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			List<StampTextModel> list;
			using (StreamReader streamReader = new StreamReader(text2))
			{
				list = JsonConvert.DeserializeObject<List<StampTextModel>>(streamReader.ReadToEnd(), new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
			}
			return list;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00036734 File Offset: 0x00034934
		public static List<DynamicStampTextModel> ReadDynamicStamp(bool Loaded = false)
		{
			string text = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Path.Combine(text, "Stamp.json");
			if (File.Exists(text2))
			{
				JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
				jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				List<DynamicStampTextModel> list;
				using (StreamReader streamReader = new StreamReader(text2))
				{
					list = JsonConvert.DeserializeObject<List<DynamicStampTextModel>>(streamReader.ReadToEnd(), new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.Auto
					});
				}
				if (Loaded)
				{
					string text3 = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
					if (!Directory.Exists(text3))
					{
						Directory.CreateDirectory(text3);
					}
					List<string> list2 = Directory.GetFiles(text3).ToList<string>();
					if (list2.Count > 0)
					{
						List<FileInfo> fileInfos2 = new List<FileInfo>();
						list2.ForEach(delegate(string f)
						{
							fileInfos2.Add(new FileInfo(f));
						});
						using (List<FileInfo>.Enumerator enumerator = fileInfos2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								FileInfo item = enumerator.Current;
								DynamicStampTextModel dynamicStampTextModel = list.FirstOrDefault((DynamicStampTextModel x) => x.TemplateName == item.FullName);
								try
								{
									if (dynamicStampTextModel == null)
									{
										DynamicStampTextModel dynamicStampTextModel2 = new DynamicStampTextModel(null, null, item.FullName, null);
										list.Add(dynamicStampTextModel2);
										dynamicStampTextModel2.GroupId = Guid.NewGuid().ToString();
										dynamicStampTextModel2.dateTime = DateTime.Now;
										List<DynamicStampTextModel> list3 = ToolbarContextMenuHelper.ReadDynamicStamp(false);
										if (list3 == null)
										{
											list3 = new List<DynamicStampTextModel>();
										}
										list3.Add(dynamicStampTextModel2);
										using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.ReadWrite))
										{
											using (StreamWriter streamWriter = new StreamWriter(fileStream))
											{
												string text4 = JsonConvert.SerializeObject(list3, Formatting.Indented, new JsonSerializerSettings
												{
													TypeNameHandling = TypeNameHandling.Auto
												});
												streamWriter.Write(text4);
												streamWriter.Close();
											}
											fileStream.Close();
										}
									}
								}
								catch
								{
								}
							}
						}
					}
				}
				ToolbarContextMenuHelper.VerifyLocalCategory(list);
				return list;
			}
			if (Loaded)
			{
				string text5 = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
				if (!Directory.Exists(text5))
				{
					Directory.CreateDirectory(text5);
				}
				List<string> list4 = Directory.GetFiles(text5).ToList<string>();
				List<DynamicStampTextModel> list5 = new List<DynamicStampTextModel>();
				if (list4.Count > 0)
				{
					List<FileInfo> fileInfos = new List<FileInfo>();
					list4.ForEach(delegate(string f)
					{
						fileInfos.Add(new FileInfo(f));
					});
					foreach (FileInfo fileInfo in fileInfos)
					{
						try
						{
							DynamicStampTextModel dynamicStampTextModel3 = new DynamicStampTextModel(null, null, fileInfo.FullName, null);
							list5.Add(dynamicStampTextModel3);
							dynamicStampTextModel3.GroupId = Guid.NewGuid().ToString();
							dynamicStampTextModel3.dateTime = DateTime.Now;
							List<DynamicStampTextModel> list6 = ToolbarContextMenuHelper.ReadDynamicStamp(false);
							if (list6 == null)
							{
								list6 = new List<DynamicStampTextModel>();
							}
							list6.Add(dynamicStampTextModel3);
							using (FileStream fileStream2 = new FileStream(text2, FileMode.Create, FileAccess.ReadWrite))
							{
								using (StreamWriter streamWriter2 = new StreamWriter(fileStream2))
								{
									string text6 = JsonConvert.SerializeObject(list6, Formatting.Indented, new JsonSerializerSettings
									{
										TypeNameHandling = TypeNameHandling.Auto
									});
									streamWriter2.Write(text6);
									streamWriter2.Close();
								}
								fileStream2.Close();
							}
						}
						catch
						{
						}
					}
				}
				ToolbarContextMenuHelper.VerifyLocalCategory(list5);
				return list5;
			}
			return null;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x00036B98 File Offset: 0x00034D98
		private static void VerifyLocalCategory(List<DynamicStampTextModel> filesArgs)
		{
			List<string> list = new List<string>();
			list = filesArgs.Select((DynamicStampTextModel f) => f.GroupName).Distinct<string>().ToList<string>();
			string stampCategory = ConfigManager.GetStampCategory();
			if (string.IsNullOrEmpty(stampCategory))
			{
				string text = JsonConvert.SerializeObject(list);
				if (!string.IsNullOrEmpty(text))
				{
					ConfigManager.SetStampCategory(text);
				}
				return;
			}
			List<string> list2 = JsonConvert.DeserializeObject<List<string>>(stampCategory);
			int num = list2.Count<string>();
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string category = enumerator.Current;
					if (list2.FindIndex((string x) => x == category) < 0)
					{
						list2.Add(category);
					}
				}
			}
			if (list2.Count > num)
			{
				string text2 = JsonConvert.SerializeObject(list2);
				if (!string.IsNullOrEmpty(text2))
				{
					ConfigManager.SetStampCategory(text2);
				}
			}
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00036C9C File Offset: 0x00034E9C
		private static void InitAllAnnotationMode()
		{
			ToolbarContextMenuHelper.allAnnotationMode = EnumHelper<AnnotationMode>.AllValues.Where((AnnotationMode c) => c != AnnotationMode.None).ToArray<AnnotationMode>();
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x00036CD1 File Offset: 0x00034ED1
		private static void InitStrokeThicknessMenuValues()
		{
			ToolbarContextMenuHelper.strokeThicknessMenuValues = Enumerable.Range(1, 12).Select(delegate(int c)
			{
				AnnotationMode[] array = ((c == 1) ? ToolbarContextMenuHelper.allAnnotationMode : null);
				return new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeThickness, (float)c, string.Format("{0} pt", c), array);
			}).ToArray<ToolbarContextMenuHelper.MenuValueProvider>();
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x00036D0C File Offset: 0x00034F0C
		private static void InitStrokeColorMenuValues()
		{
			ToolbarContextMenuHelper.MenuValueProvider[] array = new ToolbarContextMenuHelper.MenuValueProvider[9];
			array[0] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFFFFFF", "", new AnnotationMode[] { AnnotationMode.Shape });
			array[1] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF000000", "", Array.Empty<AnnotationMode>());
			array[2] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFB302F", "", ToolbarContextMenuHelper.allAnnotationMode.Where((AnnotationMode c) => c != AnnotationMode.Highlight && c != AnnotationMode.Text).ToArray<AnnotationMode>());
			array[3] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFD9927", "", Array.Empty<AnnotationMode>());
			array[4] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFAFF00", "", new AnnotationMode[]
			{
				AnnotationMode.Highlight,
				AnnotationMode.HighlightArea
			});
			array[5] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFA5DE50", "", Array.Empty<AnnotationMode>());
			array[6] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF43D9EF", "", Array.Empty<AnnotationMode>());
			array[7] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF52AAEC", "", Array.Empty<AnnotationMode>());
			array[8] = new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF9573E4", "", Array.Empty<AnnotationMode>());
			ToolbarContextMenuHelper.strokeColorMenuValues = array;
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00036E34 File Offset: 0x00035034
		private static void InitFillColorMenuValues()
		{
			AnnotationMode[] transparentOwner = new AnnotationMode[]
			{
				AnnotationMode.Ellipse,
				AnnotationMode.Shape
			};
			AnnotationMode[] array = ToolbarContextMenuHelper.allAnnotationMode.Where((AnnotationMode c) => !transparentOwner.Contains(c) && c != AnnotationMode.TextBox && c != AnnotationMode.Text).ToArray<AnnotationMode>();
			ToolbarContextMenuHelper.fillColorMenuValues = new ToolbarContextMenuHelper.MenuValueProvider[]
			{
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#00FFFFFF", "", transparentOwner, transparentOwner),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFFFFFF", "", new AnnotationMode[] { AnnotationMode.TextBox }),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF000000", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFB302F", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFD9927", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFFAFF00", "", array),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FFA5DE50", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF43D9EF", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF52AAEC", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeColor, "#FF9573E4", "", Array.Empty<AnnotationMode>())
			};
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00036F80 File Offset: 0x00035180
		private static void InitFontColorMenuValues()
		{
			ToolbarContextMenuHelper.fontColorMenuValues = new ToolbarContextMenuHelper.MenuValueProvider[]
			{
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FFFFFFFF", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FF000000", "", new AnnotationMode[]
				{
					AnnotationMode.TextBox,
					AnnotationMode.Text
				}),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FFFB302F", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FFFD9927", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FFFAFF00", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FFA5DE50", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FF43D9EF", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FF52AAEC", "", Array.Empty<AnnotationMode>()),
				new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.FontColor, "#FF9573E4", "", Array.Empty<AnnotationMode>())
			};
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x0003707C File Offset: 0x0003527C
		private static void InitFontSizeMenuValue()
		{
			AnnotationMode[] annots = new AnnotationMode[]
			{
				AnnotationMode.TextBox,
				AnnotationMode.Text
			};
			ToolbarContextMenuHelper.fontSizeMenuValues = new int[] { 8, 10, 12, 14, 18, 24, 36 }.Select(delegate(int c)
			{
				AnnotationMode[] array = ((c == 12) ? annots : null);
				return new ToolbarContextMenuHelper.MenuValueProvider(ContextMenuItemType.StrokeThickness, (float)c, string.Format("{0} pt", c), array);
			}).ToArray<ToolbarContextMenuHelper.MenuValueProvider>();
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x000370D4 File Offset: 0x000352D4
		private static void InitStampPresetsMenuValues()
		{
			ToolbarContextMenuHelper.stampPresetsMenuValues = new IStampTextModel[]
			{
				new PresetStampTextModel(StampIconNames.Approved, "#20C48F", "Approved"),
				new PresetStampTextModel(StampIconNames.Final, "#20C48F", "Approved"),
				new PresetStampTextModel(StampIconNames.ForPublicRelease, "#20C48F", "Approved"),
				new PresetStampTextModel(StampIconNames.Draft, "#298FEE", "Draft"),
				new PresetStampTextModel(StampIconNames.AsIs, "#298FEE", "Draft"),
				new PresetStampTextModel(StampIconNames.Experimental, "#298FEE", "Draft"),
				new PresetStampTextModel(StampIconNames.NotApproved, "#FF6932", "NotApproved"),
				new PresetStampTextModel(StampIconNames.Expired, "#FF6932", "NotApproved"),
				new PresetStampTextModel(StampIconNames.NotForPublicRelease, "#FF6932", "NotApproved"),
				new PresetStampTextModel(StampIconNames.Confidential, "#FF6932", "Confidential"),
				new PresetStampTextModel(StampIconNames.Sold, "#FF6932", "Confidential"),
				new PresetStampTextModel(StampIconNames.TopSecret, "#FF6932", "Confidential")
			};
			StampUtil.InitStandardIconNameContent(ToolbarContextMenuHelper.stampPresetsMenuValues.OfType<PresetStampTextModel>().ToDictionary((PresetStampTextModel c) => c.IconName, (PresetStampTextModel c) => ToolbarContextMenuHelper.GetPresetStampTextContext(c.IconName)));
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0003722C File Offset: 0x0003542C
		private static void InitViewerBackgroundColorValues()
		{
			string text = ((App.Current.GetCurrentActualAppTheme() == "Dark") ? "#444444" : "#E2E2E2");
			ToolbarContextMenuHelper.viewerBackgroundColorValues = new BackgroundColorSetting[]
			{
				new BackgroundColorSetting("Default", "", Resources.WinViewToolBackgroundDefaultText, text, "#00FFFFFF"),
				new BackgroundColorSetting("DayMode", "", Resources.WinViewToolBackgroundDayModeText, "#FCFCFC", "#00FFFFFF"),
				new BackgroundColorSetting("NightMode", "", Resources.WinViewToolBackgroundNightModeText, "#CECECE", "#400F0F0F"),
				new BackgroundColorSetting("EyeProtectionMode", "", Resources.WinViewToolBackgroundEyeProtectionModeText, "#D2E2C8", "#404B7430"),
				new BackgroundColorSetting("YellowBackground", "", Resources.WinViewToolBackgroundYellowBackgroundText, "#E4DDC4", "#40775F13")
			};
			ToolbarContextMenuHelper.viewerBackgroundColorDict = ToolbarContextMenuHelper.viewerBackgroundColorValues.ToDictionary((BackgroundColorSetting c) => c.Name, (BackgroundColorSetting c) => c);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00037358 File Offset: 0x00035558
		public static string GetPresetStampTextContext(StampIconNames stampIconName)
		{
			switch (stampIconName)
			{
			case StampIconNames.Draft:
				return Resources.MenuStampPresetsDraftItemName;
			case StampIconNames.Approved:
				return Resources.MenuStampPresetsApprovedItemName;
			case StampIconNames.Experimental:
				return Resources.MenuStampPresetsExperimentalItemName;
			case StampIconNames.NotApproved:
				return Resources.MenuStampPresetsNotApprovedItemName;
			case StampIconNames.AsIs:
				return Resources.MenuStampPresetsAsIsItemName;
			case StampIconNames.Expired:
				return Resources.MenuStampPresetsExpiredItemName;
			case StampIconNames.NotForPublicRelease:
				return Resources.MenuStampPresetsNotForPublicReleaseItemName;
			case StampIconNames.Confidential:
				return Resources.MenuStampPresetsConfidentialItemName;
			case StampIconNames.Final:
				return Resources.MenuStampPresetsFinalItemName;
			case StampIconNames.Sold:
				return Resources.MenuStampPresetsSoldItemName;
			case StampIconNames.TopSecret:
				return Resources.MenuStampPresetsTopSecretItemName;
			case StampIconNames.ForPublicRelease:
				return Resources.MenuStampPresetsNotForPublicReleaseItemName;
			}
			return string.Empty;
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000373F2 File Offset: 0x000355F2
		public static bool TryParseMenuValue(AnnotationMode mode, ContextMenuItemType type, object value, out string caption, out object result)
		{
			caption = null;
			result = null;
			if (value == null || type == ContextMenuItemType.None)
			{
				return false;
			}
			if (type == ContextMenuItemType.FillColor || type == ContextMenuItemType.StrokeColor || type == ContextMenuItemType.FontColor)
			{
				return ToolbarContextMenuHelper.TryParseMenuColorValue(mode, type, value, out caption, out result);
			}
			return (type == ContextMenuItemType.StrokeThickness || type == ContextMenuItemType.FontSize) && ToolbarContextMenuHelper.TryParseMenuThicknessValue(mode, type, value, out caption, out result);
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00037430 File Offset: 0x00035630
		private static bool TryParseMenuColorValue(AnnotationMode mode, ContextMenuItemType type, object value, out string caption, out object result)
		{
			caption = null;
			result = null;
			if (value is FS_COLOR)
			{
				FS_COLOR fs_COLOR = (FS_COLOR)value;
				caption = "";
				if (fs_COLOR.A == 0)
				{
					result = "#00FFFFFF";
				}
				else
				{
					result = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { fs_COLOR.A, fs_COLOR.R, fs_COLOR.G, fs_COLOR.B });
				}
				return true;
			}
			string text = value as string;
			if (text != null)
			{
				try
				{
					Color color = (Color)ColorConverter.ConvertFromString(text);
					caption = "";
					result = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color.A, color.R, color.G, color.B });
					return true;
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00037544 File Offset: 0x00035744
		private static bool TryParseMenuThicknessValue(AnnotationMode mode, ContextMenuItemType type, object value, out string caption, out object result)
		{
			caption = null;
			result = null;
			try
			{
				float num = Convert.ToSingle(value);
				caption = string.Format("{0} pt", num);
				result = num;
				return true;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00037594 File Offset: 0x00035794
		[CompilerGenerated]
		internal static IContextMenuModel <CreateConverterContextMenu>g__CreateItem|34_0(string _name, string _caption, BitmapImage _icon, Action<ContextMenuItemModel> _action)
		{
			return new ConvertContextMenuItemModel
			{
				Name = _name,
				Caption = _caption,
				IsChecked = false,
				Icon = _icon,
				TagData = new TagDataModel
				{
					AnnotationMode = AnnotationMode.None
				},
				Command = ((_action == null) ? null : new RelayCommand<ContextMenuItemModel>(_action))
			};
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x000375E8 File Offset: 0x000357E8
		[CompilerGenerated]
		internal static void <CreateAutoScrollContextMenu>g__AddItem|36_0(int i, ref ToolbarContextMenuHelper.<>c__DisplayClass36_0 A_1)
		{
			A_1.model.Add(new ContextMenuItemModel
			{
				Name = string.Format("{0}", i),
				Caption = string.Format("{0}", i),
				IsChecked = (i == A_1.selectedValue),
				Icon = null,
				TagData = new TagDataModel
				{
					MenuItemValue = i
				},
				Command = ((A_1.action == null) ? null : new RelayCommand<ContextMenuItemModel>(A_1.action))
			});
		}

		// Token: 0x040004A4 RID: 1188
		private const string StampTextApproved = "Approved";

		// Token: 0x040004A5 RID: 1189
		private const string StampTextDraft = "Draft";

		// Token: 0x040004A6 RID: 1190
		private const string StampTextNotApproved = "NotApproved";

		// Token: 0x040004A7 RID: 1191
		private const string StampTextConfidential = "Confidential";

		// Token: 0x040004A8 RID: 1192
		private static AnnotationMode[] allAnnotationMode;

		// Token: 0x040004A9 RID: 1193
		private static ToolbarContextMenuHelper.MenuValueProvider[] strokeThicknessMenuValues;

		// Token: 0x040004AA RID: 1194
		private static ToolbarContextMenuHelper.MenuValueProvider[] strokeColorMenuValues;

		// Token: 0x040004AB RID: 1195
		private static ToolbarContextMenuHelper.MenuValueProvider[] fillColorMenuValues;

		// Token: 0x040004AC RID: 1196
		private static ToolbarContextMenuHelper.MenuValueProvider[] fontColorMenuValues;

		// Token: 0x040004AD RID: 1197
		private static ToolbarContextMenuHelper.MenuValueProvider[] fontSizeMenuValues;

		// Token: 0x040004AE RID: 1198
		private static IStampTextModel[] stampPresetsMenuValues;

		// Token: 0x040004AF RID: 1199
		private static BackgroundColorSetting[] viewerBackgroundColorValues;

		// Token: 0x040004B0 RID: 1200
		private static IReadOnlyDictionary<string, BackgroundColorSetting> viewerBackgroundColorDict;

		// Token: 0x020004BC RID: 1212
		private class MenuValueProvider
		{
			// Token: 0x06002E8C RID: 11916 RVA: 0x000E4566 File Offset: 0x000E2766
			public MenuValueProvider(ContextMenuItemType type, object value, string caption, params AnnotationMode[] defaultValueOwner)
				: this(type, value, caption, null, defaultValueOwner)
			{
			}

			// Token: 0x06002E8D RID: 11917 RVA: 0x000E4574 File Offset: 0x000E2774
			public MenuValueProvider(ContextMenuItemType type, object value, string caption, AnnotationMode[] valueOwner, params AnnotationMode[] defaultValueOwner)
			{
				this.Type = type;
				this.Value = value;
				this.Caption = caption;
				if (valueOwner != null && valueOwner.Length != 0)
				{
					this.valueOwner = valueOwner.Distinct<AnnotationMode>().ToImmutableHashSet<AnnotationMode>();
				}
				if (defaultValueOwner != null && defaultValueOwner.Length != 0)
				{
					this.defaultValueOwner = defaultValueOwner.Distinct<AnnotationMode>().ToImmutableHashSet<AnnotationMode>();
				}
			}

			// Token: 0x17000CBF RID: 3263
			// (get) Token: 0x06002E8E RID: 11918 RVA: 0x000E45D2 File Offset: 0x000E27D2
			public ContextMenuItemType Type { get; }

			// Token: 0x17000CC0 RID: 3264
			// (get) Token: 0x06002E8F RID: 11919 RVA: 0x000E45DA File Offset: 0x000E27DA
			public object Value { get; }

			// Token: 0x17000CC1 RID: 3265
			// (get) Token: 0x06002E90 RID: 11920 RVA: 0x000E45E2 File Offset: 0x000E27E2
			public string Caption { get; }

			// Token: 0x06002E91 RID: 11921 RVA: 0x000E45EA File Offset: 0x000E27EA
			public bool IsValueOwner(AnnotationMode mode)
			{
				return this.valueOwner == null || this.valueOwner.Contains(mode);
			}

			// Token: 0x06002E92 RID: 11922 RVA: 0x000E4602 File Offset: 0x000E2802
			public bool IsDefaultValue(AnnotationMode mode)
			{
				return this.defaultValueOwner != null && this.defaultValueOwner.Contains(mode);
			}

			// Token: 0x04001AA7 RID: 6823
			private ImmutableHashSet<AnnotationMode> valueOwner;

			// Token: 0x04001AA8 RID: 6824
			private ImmutableHashSet<AnnotationMode> defaultValueOwner;
		}
	}
}
