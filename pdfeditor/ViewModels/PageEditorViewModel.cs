using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Common.HotKeys;
using CommonLib.Common.MessageBoxHelper;
using CommonLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Controls;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Controls.PageHeaderFooters;
using pdfeditor.Controls.PageOperation;
using pdfeditor.Models;
using pdfeditor.Models.Attachments;
using pdfeditor.Models.Commets;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.PageHeaderFooters;
using PDFKit.Utils.XObjects;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200006A RID: 106
	public class PageEditorViewModel : ObservableObject
	{
		// Token: 0x0600074D RID: 1869 RVA: 0x000223C8 File Offset: 0x000205C8
		public PageEditorViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
			this.headerFooterButtonModel = new ToolbarButtonModel
			{
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HeaderFooter.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HeaderFooter.png")),
				Caption = Resources.MenuInsertHeaderFooterContent,
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					IsDropDownIconVisible = true,
					ContextMenu = new ContextMenuModel
					{
						new ContextMenuItemModel
						{
							Name = "Add",
							Caption = Resources.MenuInsertAddContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFAdd.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFAdd.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.HeaderFooterContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_AddPageHeaderAndFooter"
						},
						new ContextMenuItemModel
						{
							Name = "Update",
							Caption = Resources.MenuInsertEditContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFUpdate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFUpdate.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.HeaderFooterContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_UpdatePageHeaderAndFooter"
						},
						new ContextMenuItemModel
						{
							Name = "Delete",
							Caption = Resources.MenuInsertDelContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFDelete.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.HeaderFooterContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_DeletePageHeaderAndFooter"
						}
					}
				},
				Command = new RelayCommand<ToolbarButtonModel>(new Action<ToolbarButtonModel>(this.OpenContextMenuCommandFunc))
			};
			this.pageNumberButtonModel = new ToolbarButtonModel
			{
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/PageNumber.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/PageNumber.png")),
				Caption = Resources.MenuInsertPageNumberContent,
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					IsDropDownIconVisible = true,
					ContextMenu = new ContextMenuModel
					{
						new ContextMenuItemModel
						{
							Name = "Add",
							Caption = Resources.MenuInsertAddContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFAdd.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFAdd.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.PageNumberContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_AddPageNumber"
						},
						new ContextMenuItemModel
						{
							Name = "Update",
							Caption = Resources.MenuInsertEditContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFUpdate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFUpdate.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.PageNumberContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_UpdatePageNumber"
						},
						new ContextMenuItemModel
						{
							Name = "Delete",
							Caption = Resources.MenuInsertDelContent,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/HFDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/HFDelete.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.PageNumberContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_DeletePageNumber"
						}
					}
				},
				Command = new RelayCommand<ToolbarButtonModel>(new Action<ToolbarButtonModel>(this.OpenContextMenuCommandFunc))
			};
			this.insertPageButtonModel = new ToolbarButtonModel
			{
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Insert.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Insert.png")),
				Caption = Resources.MenuPageInsertContent,
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					IsDropDownIconVisible = true,
					ContextMenu = new ContextMenuModel
					{
						new ContextMenuItemModel
						{
							Name = "BlankPage",
							Caption = Resources.MenuPageSubInsertBlankPage,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/BlankPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/BlankPage.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_InsertBlankPage",
							HotKeyInvokeAction = HotKeyInvokeAction.None
						},
						new ContextMenuItemModel
						{
							Name = "FromPDF",
							Caption = Resources.MenuPageSubInsertFromPDF,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/FromPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/FromPDF.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_InsertPDF",
							HotKeyInvokeAction = HotKeyInvokeAction.None
						},
						new ContextMenuItemModel
						{
							Name = "FromScanner",
							Caption = Resources.MenuPageSubInsertFromScanner,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/FromScanner.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/FromScanner.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke)),
							IsCheckable = false
						},
						new ContextMenuItemModel
						{
							Name = "FromWord",
							Caption = Resources.MenuPageSubInsertFromWord,
							Icon = new BitmapImage(new Uri("/Style/Resources/PageEditor/FromWord.png", UriKind.Relative)),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_InsertWord",
							HotKeyInvokeAction = HotKeyInvokeAction.None
						},
						new ContextMenuItemModel
						{
							Name = "FromImage",
							Caption = Resources.MenuPageSubInsertFromImage,
							Icon = new BitmapImage(new Uri("/Style/Resources/PageEditor/FromImage.png", UriKind.Relative)),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke)),
							IsCheckable = false,
							HotKeyInvokeWhen = "Editor_InsertImage",
							HotKeyInvokeAction = HotKeyInvokeAction.None
						}
					}
				},
				Command = new RelayCommand<ToolbarButtonModel>(new Action<ToolbarButtonModel>(this.OpenContextMenuCommandFunc))
			};
			this.insertPageButtonModel2 = new ToolbarButtonModel
			{
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Insert.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Insert.png")),
				Caption = Resources.MenuPageInsertContent,
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					IsDropDownIconVisible = true,
					ContextMenu = new ContextMenuModel
					{
						new ContextMenuItemModel
						{
							Name = "BlankPage",
							Caption = Resources.MenuPageSubInsertBlankPage,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/BlankPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/BlankPage.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke2)),
							IsCheckable = false
						},
						new ContextMenuItemModel
						{
							Name = "FromPDF",
							Caption = Resources.MenuPageSubInsertFromPDF,
							Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/FromPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/FromPDF.png")),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke2)),
							IsCheckable = false
						},
						new ContextMenuItemModel
						{
							Name = "FromScanner",
							Caption = Resources.MenuPageSubInsertFromScanner,
							Icon = new BitmapImage(new Uri("/Style/Resources/PageEditor/FromScanner.png", UriKind.Relative)),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke2)),
							IsCheckable = false
						},
						new ContextMenuItemModel
						{
							Name = "FromWord",
							Caption = Resources.MenuPageSubInsertFromWord,
							Icon = new BitmapImage(new Uri("/Style/Resources/PageEditor/FromWord.png", UriKind.Relative)),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke2)),
							IsCheckable = false
						},
						new ContextMenuItemModel
						{
							Name = "FromImage",
							Caption = Resources.MenuPageSubInsertFromImage,
							Icon = new BitmapImage(new Uri("/Style/Resources/PageEditor/FromImage.png", UriKind.Relative)),
							Command = new RelayCommand<ContextMenuItemModel>(new Action<ContextMenuItemModel>(this.InsertPageContextMenuItemInvoke2)),
							IsCheckable = false
						}
					}
				},
				Command = new RelayCommand<ToolbarButtonModel>(new Action<ToolbarButtonModel>(this.OpenContextMenuCommandFunc))
			};
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600074E RID: 1870 RVA: 0x00022BF0 File Offset: 0x00020DF0
		public ToolbarButtonModel HeaderFooterButtonModel
		{
			get
			{
				return this.headerFooterButtonModel;
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600074F RID: 1871 RVA: 0x00022BF8 File Offset: 0x00020DF8
		public ToolbarButtonModel PageNumberButtonModel
		{
			get
			{
				return this.pageNumberButtonModel;
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000750 RID: 1872 RVA: 0x00022C00 File Offset: 0x00020E00
		public ToolbarButtonModel InsertPageButtonModel
		{
			get
			{
				return this.insertPageButtonModel;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000751 RID: 1873 RVA: 0x00022C08 File Offset: 0x00020E08
		public ToolbarButtonModel InsertPageButtonModel2
		{
			get
			{
				return this.insertPageButtonModel2;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x00022C10 File Offset: 0x00020E10
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x00022C18 File Offset: 0x00020E18
		public PdfPageEditList PageEditListItemSource
		{
			get
			{
				return this.pageEditListItemSource;
			}
			set
			{
				base.SetProperty<PdfPageEditList>(ref this.pageEditListItemSource, value, "PageEditListItemSource");
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00022C2D File Offset: 0x00020E2D
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x00022C35 File Offset: 0x00020E35
		public double PageEditorMinThumbnailScale
		{
			get
			{
				return this.pageEditorMinThumbnailScale;
			}
			set
			{
				if (base.SetProperty<double>(ref this.pageEditorMinThumbnailScale, value, "PageEditorMinThumbnailScale") && this.PageEditerThumbnailScale < value)
				{
					this.PageEditerThumbnailScale = value;
				}
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x00022C5B File Offset: 0x00020E5B
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x00022C63 File Offset: 0x00020E63
		public double PageEditorMaxThumbnailScale
		{
			get
			{
				return this.pageEditorMaxThumbnailScale;
			}
			set
			{
				if (base.SetProperty<double>(ref this.pageEditorMaxThumbnailScale, value, "PageEditorMaxThumbnailScale") && this.PageEditerThumbnailScale > value)
				{
					this.PageEditerThumbnailScale = value;
				}
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x00022C89 File Offset: 0x00020E89
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x00022C94 File Offset: 0x00020E94
		public double PageEditerThumbnailScale
		{
			get
			{
				return this.pageEditerThumbnailScale;
			}
			set
			{
				double num = Math.Max(this.PageEditorMinThumbnailScale, Math.Min(this.PageEditorMaxThumbnailScale, value));
				if (base.SetProperty<double>(ref this.pageEditerThumbnailScale, num, "PageEditerThumbnailScale"))
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					if (pdfPageEditList != null)
					{
						pdfPageEditList.Scale = num;
					}
				}
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x00022CE0 File Offset: 0x00020EE0
		public RelayCommand AllPageRotateRightCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.allPageRotateRightCmd) == null)
				{
					relayCommand = (this.allPageRotateRightCmd = new RelayCommand(delegate
					{
						this.RotateAllPageCoreCmd(true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x00022D14 File Offset: 0x00020F14
		public RelayCommand AllPageRotateLeftCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.allPageRotateLeftCmd) == null)
				{
					relayCommand = (this.allPageRotateLeftCmd = new RelayCommand(delegate
					{
						this.RotateAllPageCoreCmd(false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x00022D48 File Offset: 0x00020F48
		public RelayCommand<PdfPageEditListModel> PageEditorRotateRightCmd
		{
			get
			{
				RelayCommand<PdfPageEditListModel> relayCommand;
				if ((relayCommand = this.pageEditorRotateRightCmd) == null)
				{
					relayCommand = (this.pageEditorRotateRightCmd = new RelayCommand<PdfPageEditListModel>(delegate(PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "PageEditorRotateRightCmd", "Count", 1L);
						this.RotatePageCoreCmd(model, true, false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600075D RID: 1885 RVA: 0x00022D7C File Offset: 0x00020F7C
		public RelayCommand<PdfPageEditListModel> PageEditorRotateLeftCmd
		{
			get
			{
				RelayCommand<PdfPageEditListModel> relayCommand;
				if ((relayCommand = this.pageEditorRotateLeftCmd) == null)
				{
					relayCommand = (this.pageEditorRotateLeftCmd = new RelayCommand<PdfPageEditListModel>(delegate(PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "PageEditorRotateLeftCmd", "Count", 1L);
						this.RotatePageCoreCmd(model, false, false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x00022DB0 File Offset: 0x00020FB0
		public RelayCommand SiderbarRotateRightCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.siderbarRotateRightCmd) == null)
				{
					relayCommand = (this.siderbarRotateRightCmd = new RelayCommand(delegate
					{
						GAManager.SendEvent("PageView", "PageEditorRotateRightCmd", "Siderbar", 1L);
						this.RotatePageCoreCmd(null, true, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x00022DE4 File Offset: 0x00020FE4
		public RelayCommand SiderbarRotateLeftCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.siderbarRotateLeftCmd) == null)
				{
					relayCommand = (this.siderbarRotateLeftCmd = new RelayCommand(delegate
					{
						GAManager.SendEvent("PageView", "PageEditorRotateLeftCmd", "Siderbar", 1L);
						this.RotatePageCoreCmd(null, false, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00022E18 File Offset: 0x00021018
		public RelayCommand CropPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.cropPageCmd) == null)
				{
					relayCommand = (this.cropPageCmd = new RelayCommand(delegate
					{
						GAManager.SendEvent("PageView", "CropPageCmd", "Page", 1L);
						this.CropPageCoreCmd(false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x00022E4C File Offset: 0x0002104C
		public RelayCommand SiderbarCropPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.siderbarCropPageCmd) == null)
				{
					relayCommand = (this.siderbarCropPageCmd = new RelayCommand(delegate
					{
						GAManager.SendEvent("PageView", "CropPageCmd", "Siderbar", 1L);
						this.CropPageCoreCmd(true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x00022E80 File Offset: 0x00021080
		public RelayCommand<PdfPageEditListModel> CropPageCmd2
		{
			get
			{
				RelayCommand<PdfPageEditListModel> relayCommand;
				if ((relayCommand = this.cropPageCmd2) == null)
				{
					relayCommand = (this.cropPageCmd2 = new RelayCommand<PdfPageEditListModel>(delegate(PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "CropPageCmd", "HotkeyContextMenu", 1L);
						if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
						{
							return;
						}
						if (model == null)
						{
							return;
						}
						if (!ConfigManager.GetDoNotShowFlag("NotShowCropTipFlag", false))
						{
							bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
							{
								Content = Resources.CropCreateTip,
								ShowLeftBottomCheckbox = true,
								LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
							}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
							if (checkboxResult != null && checkboxResult.GetValueOrDefault())
							{
								ConfigManager.SetDoNotShowFlag("NotShowCropTipFlag", true);
							}
						}
						this.mainViewModel.CurrnetPageIndex = model.DisplayPageIndex;
						MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
						ToolbarRadioButton cropPageBtn = mainView.cropPageBtn;
						cropPageBtn.IsChecked = new bool?(true);
						MainMenuGroup mainMenuGroup = this.mainViewModel.Menus.MainMenus.FirstOrDefault((MainMenuGroup x) => x.Tag == "View");
						int num = mainView.Menus.Items.IndexOf(mainMenuGroup);
						mainView.Menus.SelectedIndex = num;
						mainView.ToolbarScreenShotButton_Click(cropPageBtn, null);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00022EB4 File Offset: 0x000210B4
		private void CropPageCoreCmd(bool IsSiderbar = false)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int[] array2;
			if (IsSiderbar)
			{
				MainViewModel mainViewModel = this.mainViewModel;
				int[] array;
				if (mainViewModel == null)
				{
					array = null;
				}
				else
				{
					List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
					if (selectedThumbnailList == null)
					{
						array = null;
					}
					else
					{
						array = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
					}
				}
				array2 = array;
			}
			else
			{
				PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
				int[] array3;
				if (pdfPageEditList == null)
				{
					array3 = null;
				}
				else
				{
					IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
					if (selectedItems == null)
					{
						array3 = null;
					}
					else
					{
						array3 = selectedItems.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
				}
				array2 = array3;
			}
			MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			if (!ConfigManager.GetDoNotShowFlag("NotShowCropTipFlag", false))
			{
				bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
				{
					Content = Resources.CropCreateTip,
					ShowLeftBottomCheckbox = true,
					LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
				}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
				if (checkboxResult != null && checkboxResult.GetValueOrDefault())
				{
					ConfigManager.SetDoNotShowFlag("NotShowCropTipFlag", true);
				}
			}
			ToolbarRadioButton cropPageBtn = mainView.cropPageBtn;
			cropPageBtn.IsChecked = new bool?(true);
			MainMenuGroup mainMenuGroup = this.mainViewModel.Menus.MainMenus.FirstOrDefault((MainMenuGroup x) => x.Tag == "View");
			int num = mainView.Menus.Items.IndexOf(mainMenuGroup);
			mainView.Menus.SelectedIndex = num;
			if (array2 != null && array2.Length != 0)
			{
				Array.Sort<int>(array2);
				if (!IsSiderbar)
				{
					this.mainViewModel.SelectedPageIndex = array2[0];
				}
				mainView.ToolbarCropScreenShot(cropPageBtn, array2);
				return;
			}
			mainView.ToolbarCropScreenShot(cropPageBtn, null);
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x00023080 File Offset: 0x00021280
		public RelayCommand<PdfPageEditListModel> ResizePageCmd
		{
			get
			{
				RelayCommand<PdfPageEditListModel> relayCommand;
				if ((relayCommand = this.resizePageCmd) == null)
				{
					relayCommand = (this.resizePageCmd = new RelayCommand<PdfPageEditListModel>(delegate(PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "ResizePageCmd", "Page", 1L);
						this.ResizePageCoreCmd(model, false, Source.Default);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000765 RID: 1893 RVA: 0x000230B4 File Offset: 0x000212B4
		public RelayCommand<PdfPageEditListModel> viewerResizePageCmd
		{
			get
			{
				RelayCommand<PdfPageEditListModel> relayCommand;
				if ((relayCommand = this._viewerResizePageCmd) == null)
				{
					relayCommand = (this._viewerResizePageCmd = new RelayCommand<PdfPageEditListModel>(delegate(PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "ResizePageCmd", "Page", 1L);
						this.ResizePageCoreCmd(model, false, Source.Viewer);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x000230E8 File Offset: 0x000212E8
		public RelayCommand SiderbarResizePageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.siderbarResizePageCmd) == null)
				{
					relayCommand = (this.siderbarResizePageCmd = new RelayCommand(delegate
					{
						GAManager.SendEvent("PageView", "ResizePageCmd", "Page", 1L);
						this.ResizePageCoreCmd(null, true, Source.Default);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x0002311C File Offset: 0x0002131C
		private void ResizePageCoreCmd(PdfPageEditListModel model, bool IsSiderbar = false, Source source = Source.Default)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			try
			{
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				int[] array;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					List<PdfThumbnailModel> selectedThumbnailList = this.mainViewModel.SelectedThumbnailList;
					PdfThumbnailModel[] array2 = ((selectedThumbnailList != null) ? selectedThumbnailList.ToArray() : null);
					int[] array3;
					if (array2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = array2.Select((PdfThumbnailModel t) => t.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array4;
					if (obj2 == null)
					{
						array4 = null;
					}
					else
					{
						array4 = obj2.Select((PdfPageEditListModel t) => t.PageIndex).ToArray<int>();
					}
					array = array4;
				}
				PdfDocument pdfDocument = PdfDocument.CreateNew(null);
				pdfDocument.Pages.ImportPages(this.mainViewModel.Document, string.Format("1-{0}", this.mainViewModel.Document.Pages.Count), 0);
				if (array != null && array.Length != 0)
				{
					Array.Sort<int>(array);
					int num = this.mainViewModel.SelectedPageIndex;
					if (array != null)
					{
						num = array[0];
					}
					PageResizeWindow pageResizeWindow = new PageResizeWindow(pdfDocument, num, array, source);
					pageResizeWindow.ShowDialog();
					pageResizeWindow.Owner = App.Current.MainWindow;
					pageResizeWindow.WindowStartupLocation = ((pageResizeWindow.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
				}
				else
				{
					PageResizeWindow pageResizeWindow2 = new PageResizeWindow(pdfDocument, this.mainViewModel.SelectedPageIndex, null, Source.Default);
					pageResizeWindow2.ShowDialog();
					pageResizeWindow2.Owner = App.Current.MainWindow;
					pageResizeWindow2.WindowStartupLocation = ((pageResizeWindow2.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
				}
			}
			catch
			{
				ModernMessageBox.Show(Resources.ImprotPagesUnexpectedError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00023318 File Offset: 0x00021518
		private void RotatePagesCore(IEnumerable<int> pageNumbers, bool rotateRight)
		{
			if (pageNumbers == null || this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int[] nums = pageNumbers.ToArray<int>();
			if (nums.Length != 0)
			{
				PageEditorViewModel.<RotatePagesCore>g__RotateCore|91_0(this.mainViewModel.Document, nums, rotateRight);
				this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
				{
					PageEditorViewModel.<RotatePagesCore>g__RotateCore|91_0(this.mainViewModel.Document, nums, !rotateRight);
					global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					if (pdfControl2 != null && pdfControl2.Document != null)
					{
						pdfControl2.UpdateDocLayout();
					}
				}, delegate(PdfDocument doc)
				{
					PageEditorViewModel.<RotatePagesCore>g__RotateCore|91_0(this.mainViewModel.Document, nums, rotateRight);
					global::PDFKit.PdfControl pdfControl3 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					if (pdfControl3 != null && pdfControl3.Document != null)
					{
						pdfControl3.UpdateDocLayout();
					}
				}, "");
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				if (pdfControl != null && pdfControl.Document != null)
				{
					pdfControl.UpdateDocLayout();
				}
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x000233E0 File Offset: 0x000215E0
		private void RotateAllPageCoreCmd(bool rotateRight)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int count = this.mainViewModel.Document.Pages.Count;
			this.RotatePagesCore(Enumerable.Range(0, count), rotateRight);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00023434 File Offset: 0x00021634
		private void RotatePageCoreCmd(PdfPageEditListModel model, bool rotateRight, bool IsSiderbar = false)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			IEnumerable<int> enumerable;
			if (model != null)
			{
				enumerable = new int[] { model.PageIndex };
			}
			else if (IsSiderbar)
			{
				MainViewModel mainViewModel = this.mainViewModel;
				IEnumerable<int> enumerable2;
				if (mainViewModel == null)
				{
					enumerable2 = null;
				}
				else
				{
					List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
					if (selectedThumbnailList == null)
					{
						enumerable2 = null;
					}
					else
					{
						enumerable2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex);
					}
				}
				enumerable = enumerable2;
			}
			else
			{
				PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
				IEnumerable<PdfPageEditListModel> enumerable3;
				if (pdfPageEditList == null)
				{
					enumerable3 = null;
				}
				else
				{
					IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
					enumerable3 = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
				}
				enumerable = enumerable3.Select((PdfPageEditListModel c) => c.PageIndex);
			}
			if (enumerable != null && enumerable.Count<int>() > 0)
			{
				this.RotatePagesCore(enumerable, rotateRight);
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x00023518 File Offset: 0x00021718
		public RelayCommand FormBlankPage
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.formBlankPage) == null)
				{
					relayCommand = (this.formBlankPage = new RelayCommand(delegate
					{
						this.DoInsertBlankPage(null, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0002354C File Offset: 0x0002174C
		public RelayCommand CreateBlankPage
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.createBlankPage) == null)
				{
					relayCommand = (this.createBlankPage = new RelayCommand(delegate
					{
						this.DoCreateBlankPageAsync(null, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x00023580 File Offset: 0x00021780
		public RelayCommand FormScanner
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.formScanner) == null)
				{
					relayCommand = (this.formScanner = new RelayCommand(delegate
					{
						this.DoInsertFormScanner(null, true, false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x000235B4 File Offset: 0x000217B4
		public RelayCommand FormPDF
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.formPDF) == null)
				{
					relayCommand = (this.formPDF = new RelayCommand(delegate
					{
						this.DoInsertPDF(null, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x000235E8 File Offset: 0x000217E8
		public RelayCommand FormWord
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.formWord) == null)
				{
					relayCommand = (this.formWord = new RelayCommand(delegate
					{
						this.DoInsertWord(null, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x0002361C File Offset: 0x0002181C
		public RelayCommand FormImage
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.formImage) == null)
				{
					relayCommand = (this.formImage = new RelayCommand(delegate
					{
						this.DoInsertImage(null, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x00023650 File Offset: 0x00021850
		public RelayCommand CreateNewFormScanner
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.createNewFormScanner) == null)
				{
					relayCommand = (this.createNewFormScanner = new RelayCommand(delegate
					{
						this.DoInsertFormScanner(null, true, true);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x00023684 File Offset: 0x00021884
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorDeleteCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorDeleteCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorDeleteCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DeletePages(model, false);
					}, (PdfPageEditListModel _) => !this.PageEditorDeleteCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x000236C4 File Offset: 0x000218C4
		public AsyncRelayCommand SiderbarDeleteCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.siderbarDeleteCmd) == null)
				{
					asyncRelayCommand = (this.siderbarDeleteCmd = new AsyncRelayCommand(async delegate
					{
						this.DeletePages(null, true);
					}, () => !this.SiderbarDeleteCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00023704 File Offset: 0x00021904
		private void DeletePages(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			GAManager.SendEvent("PageView", "PageEditorDeleteCmd", "PageToolbar", 1L);
			if (!IAPUtils.IsPaidUserWrapper())
			{
				IAPUtils.ShowPurchaseWindows("deletepages", ".pdf");
				return;
			}
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			this.mainViewModel.ViewJumpManager.ClearStack();
			int[] array;
			if (model != null)
			{
				array = new int[] { model.PageIndex };
			}
			else if (IsSiderbar)
			{
				MainViewModel mainViewModel = this.mainViewModel;
				List<PdfThumbnailModel> list = ((mainViewModel != null) ? mainViewModel.SelectedThumbnailList : null);
				int[] array2;
				if (list == null)
				{
					array2 = null;
				}
				else
				{
					array2 = list.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
				}
				array = array2;
			}
			else
			{
				PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
				IEnumerable<PdfPageEditListModel> enumerable;
				if (pdfPageEditList == null)
				{
					enumerable = null;
				}
				else
				{
					IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
					enumerable = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
				}
				array = enumerable.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
			}
			DeletePagesWindow deletePagesWindow = ((array != null && array.Length != 0) ? new DeletePagesWindow(this.mainViewModel, array) : new DeletePagesWindow(this.mainViewModel));
			deletePagesWindow.Owner = App.Current.MainWindow;
			deletePagesWindow.WindowStartupLocation = ((deletePagesWindow.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
			bool? flag = deletePagesWindow.ShowDialog();
			if (IsSiderbar && flag.GetValueOrDefault())
			{
				StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(new global::System.ValueTuple<int, int>(this.mainViewModel.SelectedPageIndex, this.mainViewModel.SelectedPageIndex)), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x000238AC File Offset: 0x00021AAC
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorExtractCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorExtractCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorExtractCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						GAManager.SendEvent("PageView", "PageEditorExtractCmd", "PageToolbar", 1L);
						this.ExtractPages(model, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x000238E0 File Offset: 0x00021AE0
		public AsyncRelayCommand SiderbarExtractCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.siderbarExtractCmd) == null)
				{
					asyncRelayCommand = (this.siderbarExtractCmd = new AsyncRelayCommand(async delegate
					{
						GAManager.SendEvent("PageView", "SiderbarExtractCmd", "Siderbar", 1L);
						this.ExtractPages(null, true);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00023914 File Offset: 0x00021B14
		private void ExtractPages(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			if (!IAPUtils.IsPaidUserWrapper())
			{
				IAPUtils.ShowPurchaseWindows("extractpages", ".pdf");
				return;
			}
			int[] array;
			if (model != null)
			{
				array = new int[] { model.PageIndex };
			}
			else if (IsSiderbar)
			{
				MainViewModel mainViewModel = this.mainViewModel;
				List<PdfThumbnailModel> list = ((mainViewModel != null) ? mainViewModel.SelectedThumbnailList : null);
				int[] array2;
				if (list == null)
				{
					array2 = null;
				}
				else
				{
					array2 = list.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
				}
				array = array2;
			}
			else
			{
				PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
				object obj;
				if (pdfPageEditList == null)
				{
					obj = null;
				}
				else
				{
					IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
					obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
				}
				object obj2 = obj;
				int[] array3;
				if (obj2 == null)
				{
					array3 = null;
				}
				else
				{
					array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
				}
				array = array3;
			}
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (array != null && array.Length != 0)
			{
				int[] array4;
				string text = array.ConvertToRange(out array4);
				ExtractPagesWindow extractPagesWindow = new ExtractPagesWindow(array, text, folderPath, this.mainViewModel);
				extractPagesWindow.ShowDialog();
				extractPagesWindow.Owner = App.Current.MainWindow;
				extractPagesWindow.WindowStartupLocation = ((extractPagesWindow.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
				return;
			}
			ExtractPagesWindow extractPagesWindow2 = new ExtractPagesWindow(folderPath, this.mainViewModel);
			extractPagesWindow2.ShowDialog();
			extractPagesWindow2.Owner = App.Current.MainWindow;
			extractPagesWindow2.WindowStartupLocation = ((extractPagesWindow2.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x00023A9C File Offset: 0x00021C9C
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorMergeCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorMergeCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorMergeCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.mainViewModel.ExitTransientMode(false, false, false, false, false);
						if (this.mainViewModel.CanSave)
						{
							ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						else
						{
							this.mainViewModel.ViewJumpManager.ClearStack();
							GAManager.SendEvent("PageView", "PageEditorMergeCmd", "Count", 1L);
							ConvToPDFType convToPDFType = ConvToPDFType.MergePDF;
							string password = this.mainViewModel.Password;
							string[] array = new string[1];
							int num = 0;
							DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
							array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
							AppManager.OpenPDFConverterToPdf(convToPDFType, password, array);
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x00023AD0 File Offset: 0x00021CD0
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorInsertBlankCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorInsertBlankCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorInsertBlankCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DoInsertBlankPage(model, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x00023B04 File Offset: 0x00021D04
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorInsertFromPDFCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorInsertFromPDFCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorInsertFromPDFCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DoInsertPDF(model, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x00023B38 File Offset: 0x00021D38
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorInsertFromScannerCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorInsertFromScannerCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorInsertFromScannerCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DoInsertFormScanner(model, false, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x00023B6C File Offset: 0x00021D6C
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorInsertFromWordCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorInsertFromWordCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorInsertFromWordCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DoInsertWord(model, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00023BA0 File Offset: 0x00021DA0
		public AsyncRelayCommand<PdfPageEditListModel> PageEditorInsertFromImageCmd
		{
			get
			{
				AsyncRelayCommand<PdfPageEditListModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorInsertFromImgCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorInsertFromImgCmd = new AsyncRelayCommand<PdfPageEditListModel>(async delegate([Nullable(2)] PdfPageEditListModel model)
					{
						this.DoInsertImage(model, false);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00023BD4 File Offset: 0x00021DD4
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "startPage", "endPage" })]
		private global::System.ValueTuple<int, int> InsertPagesIntoDocument(PdfDocument doc, string sourceFilePath, int insertIndex, bool isBefore)
		{
			global::System.ValueTuple<int, int> valueTuple;
			using (FileStream fileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (PdfDocument pdfDocument = PdfDocument.Load(fileStream, null, null, true))
				{
					PageDisposeHelper.TryFixResource(pdfDocument, 0, pdfDocument.Pages.Count - 1);
					int num = (isBefore ? insertIndex : (insertIndex + 1));
					doc.Pages.ImportPages(pdfDocument, string.Format("1-{0}", pdfDocument.Pages.Count), num);
					valueTuple = new global::System.ValueTuple<int, int>(num, num + pdfDocument.Pages.Count - 1);
				}
			}
			return valueTuple;
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x00023C84 File Offset: 0x00021E84
		public AsyncRelayCommand PageEditorMergeCmd2
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorMergeCmd2) == null)
				{
					asyncRelayCommand = (this.pageEditorMergeCmd2 = new AsyncRelayCommand(async delegate
					{
						this.mainViewModel.ExitTransientMode(false, false, false, false, false);
						if (this.mainViewModel.CanSave)
						{
							ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						else
						{
							this.mainViewModel.ViewJumpManager.ClearStack();
							GAManager.SendEvent("PageView", "PageEditorMergeCmd", "Count", 1L);
							ConvToPDFType convToPDFType = ConvToPDFType.MergePDF;
							string password = this.mainViewModel.Password;
							string[] array = new string[1];
							int num = 0;
							DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
							array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
							AppManager.OpenPDFConverterToPdf(convToPDFType, password, array);
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x00023CB8 File Offset: 0x00021EB8
		public AsyncRelayCommand PageEditorSplitCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.pageEditorSplitCmd) == null)
				{
					asyncRelayCommand = (this.pageEditorSplitCmd = new AsyncRelayCommand(async delegate
					{
						this.mainViewModel.ExitTransientMode(false, false, false, false, false);
						if (this.mainViewModel.CanSave)
						{
							ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						else
						{
							this.mainViewModel.ViewJumpManager.ClearStack();
							GAManager.SendEvent("PageView", "PageEditorSplitCmd", "Count", 1L);
							ConvToPDFType convToPDFType = ConvToPDFType.SplitPDF;
							string password = this.mainViewModel.Password;
							string[] array = new string[1];
							int num = 0;
							DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
							array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
							AppManager.OpenPDFConverterToPdf(convToPDFType, password, array);
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x00023CEC File Offset: 0x00021EEC
		public RelayCommand PageEditorZoomOutCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pageEditorZoomOutCmd) == null)
				{
					relayCommand = (this.pageEditorZoomOutCmd = new RelayCommand(delegate
					{
						if (this.mainViewModel.Document != null)
						{
							this.PageEditerThumbnailScale -= 0.1;
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00023D20 File Offset: 0x00021F20
		public RelayCommand PageEditorZoomInCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pageEditorZoomInCmd) == null)
				{
					relayCommand = (this.pageEditorZoomInCmd = new RelayCommand(delegate
					{
						if (this.mainViewModel.Document != null)
						{
							this.PageEditerThumbnailScale += 0.1;
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00023D51 File Offset: 0x00021F51
		public void NotifyPageAnnotationChanged(int pageIndex)
		{
			AllPageCommetCollectionView pageCommetSource = this.mainViewModel.PageCommetSource;
			if (pageCommetSource == null)
			{
				return;
			}
			pageCommetSource.NotifyPageAnnotationChanged(pageIndex);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x00023D69 File Offset: 0x00021F69
		public void NotifyAttachmentChanged()
		{
			AttachmentWrappersCollection attachmentSource = this.mainViewModel.AttachmentSource;
			if (attachmentSource == null)
			{
				return;
			}
			attachmentSource.NotifyAttachmentChanged();
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00023D80 File Offset: 0x00021F80
		private void OpenContextMenuCommandFunc(ToolbarButtonModel model)
		{
			PageEditorViewModel.<>c__DisplayClass141_0 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass141_0();
			CS$<>8__locals1.model = model;
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				PageEditorViewModel.<>c__DisplayClass141_0.<<OpenContextMenuCommandFunc>b__0>d <<OpenContextMenuCommandFunc>b__0>d;
				<<OpenContextMenuCommandFunc>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<OpenContextMenuCommandFunc>b__0>d.<>4__this = CS$<>8__locals1;
				<<OpenContextMenuCommandFunc>b__0>d.<>1__state = -1;
				<<OpenContextMenuCommandFunc>b__0>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass141_0.<<OpenContextMenuCommandFunc>b__0>d>(ref <<OpenContextMenuCommandFunc>b__0>d);
			}));
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00023DB4 File Offset: 0x00021FB4
		private bool GetHeaderFooterSettings(out global::System.Collections.Generic.IReadOnlyList<HeaderFooterData> hfData)
		{
			PageEditorViewModel.<>c__DisplayClass142_0 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass142_0();
			CS$<>8__locals1.<>4__this = this;
			hfData = null;
			CS$<>8__locals1._hfData = hfData;
			bool flag = ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
			{
				PageEditorViewModel.<>c__DisplayClass142_0.<<GetHeaderFooterSettings>b__0>d <<GetHeaderFooterSettings>b__0>d;
				<<GetHeaderFooterSettings>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<GetHeaderFooterSettings>b__0>d.<>4__this = CS$<>8__locals1;
				<<GetHeaderFooterSettings>b__0>d.c = c;
				<<GetHeaderFooterSettings>b__0>d.<>1__state = -1;
				<<GetHeaderFooterSettings>b__0>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass142_0.<<GetHeaderFooterSettings>b__0>d>(ref <<GetHeaderFooterSettings>b__0>d);
				return <<GetHeaderFooterSettings>b__0>d.<>t__builder.Task;
			}, null, Resources.WinPageLoadingHeaderorFooterContent, true, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
			hfData = CS$<>8__locals1._hfData;
			return flag;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00023E10 File Offset: 0x00022010
		private void HeaderFooterContextMenuItemInvoke(ContextMenuItemModel model)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.mainViewModel.ReleaseViewerFocusAsync(true);
			string text = ((model != null) ? model.Name : null);
			if (text == "Add")
			{
				this.AddHeaderFooter();
				return;
			}
			if (text == "Update")
			{
				this.UpdateHeaderFooter();
				return;
			}
			if (!(text == "Delete"))
			{
				return;
			}
			this.DeleteHeaderFooter();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00023E8F File Offset: 0x0002208F
		public void AddHeaderFooter2()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.mainViewModel.ReleaseViewerFocusAsync(true);
			this.AddHeaderFooter();
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00023EC0 File Offset: 0x000220C0
		private void AddHeaderFooter()
		{
			GAManager.SendEvent("PageView", "PageEditorAddHeaderFooterCmd", "Count", 1L);
			this.AddHeaderFooterOrPageNumber(false);
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x00023EDF File Offset: 0x000220DF
		private void UpdateHeaderFooter()
		{
			GAManager.SendEvent("PageView", "PageEditorUpdateHeaderFooterCmd", "Count", 1L);
			this.UpdateHeaderFooterOrPageNumber(false);
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x00023EFE File Offset: 0x000220FE
		private void DeleteHeaderFooter()
		{
			GAManager.SendEvent("PageView", "PageEditorDeleteHeaderFooterCmd", "Count", 1L);
			this.DeleteHeaderFooterOrPageNumber(false);
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x00023F20 File Offset: 0x00022120
		private void PageNumberContextMenuItemInvoke(ContextMenuItemModel model)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.mainViewModel.ReleaseViewerFocusAsync(true);
			string text = ((model != null) ? model.Name : null);
			if (text == "Add")
			{
				this.AddPageNumber();
				return;
			}
			if (text == "Update")
			{
				this.UpdatePageNumber();
				return;
			}
			if (!(text == "Delete"))
			{
				return;
			}
			this.DeletePageNumber();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x00023FA0 File Offset: 0x000221A0
		private async void InsertPageContextMenuItemInvoke(ContextMenuItemModel model)
		{
			this.mainViewModel.ViewJumpManager.ClearStack();
			string text = ((model != null) ? model.Name : null);
			if (!(text == "BlankPage"))
			{
				if (!(text == "FromPDF"))
				{
					if (!(text == "FromScanner"))
					{
						if (!(text == "FromWord"))
						{
							if (text == "FromImage")
							{
								await this.PageEditorInsertFromImageCmd.ExecuteAsync(null);
							}
						}
						else
						{
							await this.PageEditorInsertFromWordCmd.ExecuteAsync(null);
						}
					}
					else
					{
						await this.PageEditorInsertFromScannerCmd.ExecuteAsync(null);
					}
				}
				else
				{
					await this.PageEditorInsertFromPDFCmd.ExecuteAsync(null);
				}
			}
			else
			{
				await this.PageEditorInsertBlankCmd.ExecuteAsync(null);
			}
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x00023FE0 File Offset: 0x000221E0
		private void InsertPageContextMenuItemInvoke2(ContextMenuItemModel model)
		{
			int selectedPageIndex = this.mainViewModel.SelectedPageIndex;
			((MainView)App.Current.MainWindow).Menus_SelectItem("Page");
			this.PageEditListItemSource.AllItemSelected = new bool?(false);
			if (selectedPageIndex >= 0 && selectedPageIndex < this.PageEditListItemSource.Count)
			{
				this.PageEditListItemSource[selectedPageIndex].Selected = true;
			}
			this.InsertPageContextMenuItemInvoke(model);
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x0002404E File Offset: 0x0002224E
		public void AddPageNumber()
		{
			GAManager.SendEvent("PageView", "PageEditorAddPageNumberCmd", "Count", 1L);
			this.AddHeaderFooterOrPageNumber(true);
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x00024070 File Offset: 0x00022270
		public void AddPageNumber2()
		{
			GAManager.SendEvent("PageView", "PageEditorAddPageNumberCmd", "Count", 1L);
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			this.mainViewModel.AnnotationMode = AnnotationMode.None;
			this.mainViewModel.ReleaseViewerFocusAsync(true);
			this.AddHeaderFooterOrPageNumber(true);
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x000240C3 File Offset: 0x000222C3
		private void UpdatePageNumber()
		{
			GAManager.SendEvent("PageView", "PageEditorUpdatePageNumberCmd", "Count", 1L);
			this.UpdateHeaderFooterOrPageNumber(true);
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x000240E2 File Offset: 0x000222E2
		private void DeletePageNumber()
		{
			GAManager.SendEvent("PageView", "PageEditorDeletePageNumberCmd", "Count", 1L);
			this.DeleteHeaderFooterOrPageNumber(true);
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00024104 File Offset: 0x00022304
		private void AddHeaderFooterOrPageNumber(bool isPageNumber)
		{
			PageEditorViewModel.<>c__DisplayClass155_0 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass155_0();
			CS$<>8__locals1.<>4__this = this;
			PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
			object obj;
			if (pdfPageEditList == null)
			{
				obj = null;
			}
			else
			{
				IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
				obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
			}
			object obj2 = obj;
			if (obj2 != null && obj2.Length != 0 && (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null))
			{
				return;
			}
			if (!this.GetHeaderFooterSettings(out CS$<>8__locals1.hfData))
			{
				return;
			}
			MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
			if (CS$<>8__locals1.hfData != null && CS$<>8__locals1.hfData.Count > 0)
			{
				TextBlock textBlock = new TextBlock
				{
					Text = Resources.WinPageInsertPageNumorHeaderFooterAddChekContent,
					TextWrapping = TextWrapping.Wrap,
					FontWeight = FontWeights.SemiBold
				};
				TextBlock textBlock2 = new TextBlock
				{
					Text = Resources.WinPageInsertPageNumorHeaderFooterAddNoteContent,
					TextWrapping = TextWrapping.Wrap,
					Margin = new Thickness(0.0, 8.0, 0.0, 0.0),
					Foreground = new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(204, 0, 0, 0))
				};
				StackPanel stackPanel = new StackPanel
				{
					Children = { textBlock, textBlock2 }
				};
				messageBoxResult = ModernMessageBox.Show(new ModernMessageBoxOptions
				{
					Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(),
					Caption = UtilManager.GetProductName(),
					MessageBoxContent = stackPanel,
					Button = MessageBoxButton.YesNoCancel,
					UIOverrides = 
					{
						YesButtonContent = Resources.WinPageInsertPageNumorHeaderFooterAddBtnUpdateContent,
						NoButtonContent = Resources.WinPageInsertPageNumorHeaderFooterAddBtnAddNewContent,
						IsButtonsReversed = true,
						HighlightPrimaryButton = true
					}
				});
			}
			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return;
			}
			CS$<>8__locals1.isReplace = messageBoxResult == MessageBoxResult.Yes;
			HeaderFooterSettings headerFooterSettings = null;
			if (CS$<>8__locals1.isReplace)
			{
				global::System.Collections.Generic.IReadOnlyList<HeaderFooterData> hfData = CS$<>8__locals1.hfData;
				HeaderFooterSettings headerFooterSettings2;
				if (hfData == null)
				{
					headerFooterSettings2 = null;
				}
				else
				{
					HeaderFooterData headerFooterData = hfData.FirstOrDefault(delegate(HeaderFooterData c)
					{
						HeaderFooterData.HeaderFooterSettingsData settingsData = c.SettingsData;
						return ((settingsData != null) ? settingsData.Settings : null) != null;
					});
					headerFooterSettings2 = ((headerFooterData != null) ? headerFooterData.SettingsData.Settings : null);
				}
				headerFooterSettings = headerFooterSettings2;
			}
			string documentPath = this.mainViewModel.DocumentWrapper.DocumentPath;
			string text = "";
			if (!string.IsNullOrEmpty(documentPath))
			{
				text = Path.GetFileName(documentPath);
			}
			bool? flag = null;
			CS$<>8__locals1.settings = null;
			if (isPageNumber)
			{
				PageNumberDialog pageNumberDialog = new PageNumberDialog(this.mainViewModel.Document, headerFooterSettings, null);
				pageNumberDialog.Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				pageNumberDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				flag = pageNumberDialog.ShowDialog();
				if (flag.GetValueOrDefault())
				{
					CS$<>8__locals1.settings = pageNumberDialog.Result.ToSettings(this.mainViewModel.Document, -1);
				}
			}
			else
			{
				PageHeaderFooterDialog pageHeaderFooterDialog = new PageHeaderFooterDialog(this.mainViewModel.Document, text, headerFooterSettings, null);
				pageHeaderFooterDialog.Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				pageHeaderFooterDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				flag = pageHeaderFooterDialog.ShowDialog();
				if (flag.GetValueOrDefault())
				{
					CS$<>8__locals1.settings = pageHeaderFooterDialog.Result.ToSettings(this.mainViewModel.Document, -1);
				}
			}
			if (flag.GetValueOrDefault())
			{
				ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
				{
					PageEditorViewModel.<>c__DisplayClass155_0.<<AddHeaderFooterOrPageNumber>b__1>d <<AddHeaderFooterOrPageNumber>b__1>d;
					<<AddHeaderFooterOrPageNumber>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<AddHeaderFooterOrPageNumber>b__1>d.<>4__this = CS$<>8__locals1;
					<<AddHeaderFooterOrPageNumber>b__1>d.c = c;
					<<AddHeaderFooterOrPageNumber>b__1>d.<>1__state = -1;
					<<AddHeaderFooterOrPageNumber>b__1>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass155_0.<<AddHeaderFooterOrPageNumber>b__1>d>(ref <<AddHeaderFooterOrPageNumber>b__1>d);
					return <<AddHeaderFooterOrPageNumber>b__1>d.<>t__builder.Task;
				}, null, Resources.WinPageApplyingHeaderorFooterContent, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
				this.mainViewModel.SetCanSaveFlag();
				this.FlushViewerAndThumbnail(false);
			}
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x0002446C File Offset: 0x0002266C
		private void UpdateHeaderFooterOrPageNumber(bool isPageNumber)
		{
			PageEditorViewModel.<>c__DisplayClass156_0 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass156_0();
			CS$<>8__locals1.<>4__this = this;
			PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
			object obj;
			if (pdfPageEditList == null)
			{
				obj = null;
			}
			else
			{
				IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
				obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
			}
			object obj2 = obj;
			if (obj2 != null && obj2.Length != 0 && (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null))
			{
				return;
			}
			if (!this.GetHeaderFooterSettings(out CS$<>8__locals1.hfData))
			{
				return;
			}
			if (CS$<>8__locals1.hfData.Count == 0)
			{
				ModernMessageBox.Show(App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), Resources.WinPageInsertPageNumorHeaderFooterUpdateCheckEmptyContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			global::System.Collections.Generic.IReadOnlyList<HeaderFooterData> hfData = CS$<>8__locals1.hfData;
			HeaderFooterSettings headerFooterSettings;
			if (hfData == null)
			{
				headerFooterSettings = null;
			}
			else
			{
				HeaderFooterData headerFooterData = hfData.FirstOrDefault(delegate(HeaderFooterData c)
				{
					HeaderFooterData.HeaderFooterSettingsData settingsData = c.SettingsData;
					return ((settingsData != null) ? settingsData.Settings : null) != null;
				});
				headerFooterSettings = ((headerFooterData != null) ? headerFooterData.SettingsData.Settings : null);
			}
			HeaderFooterSettings headerFooterSettings2 = headerFooterSettings;
			string documentPath = this.mainViewModel.DocumentWrapper.DocumentPath;
			string text = "";
			if (!string.IsNullOrEmpty(documentPath))
			{
				text = Path.GetFileName(documentPath);
			}
			bool? flag = null;
			CS$<>8__locals1.settings = null;
			if (isPageNumber)
			{
				PageNumberDialog pageNumberDialog = new PageNumberDialog(this.mainViewModel.Document, headerFooterSettings2, null);
				pageNumberDialog.Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				pageNumberDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				flag = pageNumberDialog.ShowDialog();
				if (flag.GetValueOrDefault())
				{
					CS$<>8__locals1.settings = pageNumberDialog.Result.ToSettings(this.mainViewModel.Document, -1);
				}
			}
			else
			{
				PageHeaderFooterDialog pageHeaderFooterDialog = new PageHeaderFooterDialog(this.mainViewModel.Document, text, headerFooterSettings2, null);
				pageHeaderFooterDialog.Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				pageHeaderFooterDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				flag = pageHeaderFooterDialog.ShowDialog();
				if (flag.GetValueOrDefault())
				{
					CS$<>8__locals1.settings = pageHeaderFooterDialog.Result.ToSettings(this.mainViewModel.Document, -1);
				}
			}
			if (flag.GetValueOrDefault())
			{
				ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
				{
					PageEditorViewModel.<>c__DisplayClass156_0.<<UpdateHeaderFooterOrPageNumber>b__1>d <<UpdateHeaderFooterOrPageNumber>b__1>d;
					<<UpdateHeaderFooterOrPageNumber>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<UpdateHeaderFooterOrPageNumber>b__1>d.<>4__this = CS$<>8__locals1;
					<<UpdateHeaderFooterOrPageNumber>b__1>d.c = c;
					<<UpdateHeaderFooterOrPageNumber>b__1>d.<>1__state = -1;
					<<UpdateHeaderFooterOrPageNumber>b__1>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass156_0.<<UpdateHeaderFooterOrPageNumber>b__1>d>(ref <<UpdateHeaderFooterOrPageNumber>b__1>d);
					return <<UpdateHeaderFooterOrPageNumber>b__1>d.<>t__builder.Task;
				}, null, Resources.WinPageApplyingHeaderorFooterContent, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
				this.mainViewModel.SetCanSaveFlag();
				this.FlushViewerAndThumbnail(false);
			}
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x000246B0 File Offset: 0x000228B0
		private void DeleteHeaderFooterOrPageNumber(bool isPageNumber)
		{
			PageEditorViewModel.<>c__DisplayClass157_0 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass157_0();
			CS$<>8__locals1.<>4__this = this;
			if (!this.GetHeaderFooterSettings(out CS$<>8__locals1.hfData))
			{
				return;
			}
			if (CS$<>8__locals1.hfData.Count == 0)
			{
				ModernMessageBox.Show(App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), Resources.WinPageInsertPageNumorHeaderFooterDelCheckEmptyContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (ModernMessageBox.Show(App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), Resources.WinPageInsertPageNumorHeaderFooterDelCheckExistContent, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, true) == MessageBoxResult.Yes)
			{
				ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
				{
					PageEditorViewModel.<>c__DisplayClass157_0.<<DeleteHeaderFooterOrPageNumber>b__0>d <<DeleteHeaderFooterOrPageNumber>b__0>d;
					<<DeleteHeaderFooterOrPageNumber>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<DeleteHeaderFooterOrPageNumber>b__0>d.<>4__this = CS$<>8__locals1;
					<<DeleteHeaderFooterOrPageNumber>b__0>d.c = c;
					<<DeleteHeaderFooterOrPageNumber>b__0>d.<>1__state = -1;
					<<DeleteHeaderFooterOrPageNumber>b__0>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass157_0.<<DeleteHeaderFooterOrPageNumber>b__0>d>(ref <<DeleteHeaderFooterOrPageNumber>b__0>d);
					return <<DeleteHeaderFooterOrPageNumber>b__0>d.<>t__builder.Task;
				}, null, Resources.WinPageApplyingHeaderorFooterContent, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
				this.mainViewModel.SetCanSaveFlag();
				this.FlushViewerAndThumbnail(false);
			}
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x0002477C File Offset: 0x0002297C
		public void FlushViewerAndThumbnail(bool forceRedraw = false)
		{
			Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (pdfControl != null)
			{
				pdfControl.Redraw(forceRedraw);
			}
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl);
			if (annotationCanvas != null && annotationCanvas.ImageControl.Visibility == Visibility.Visible)
			{
				annotationCanvas.ImageControl.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x000247D8 File Offset: 0x000229D8
		public void ReorderPages(PdfPageEditListModel beforeItem, PdfPageEditListModel afterItem, PdfPageEditListModel[] selectedItems)
		{
			if ((beforeItem == null && afterItem == null) || selectedItems == null || selectedItems.Length == 0)
			{
				return;
			}
			int num = -1;
			if (beforeItem != null)
			{
				num = beforeItem.PageIndex + 1;
			}
			else if (afterItem != null)
			{
				num = afterItem.PageIndex;
			}
			PdfPageEditListModel[] array = selectedItems.OrderBy((PdfPageEditListModel c) => c.PageIndex).ToArray<PdfPageEditListModel>();
			if (this.mainViewModel.CanSave && this.mainViewModel.ExtraSaveOperationNames.LastOrDefault<string>() != "Reorder")
			{
				ModernMessageBox.Show(Resources.PageSplitMergeCheckMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (!ConfigManager.GetDoNotShowFlag("PageReorderDontShow", false))
			{
				MessageBoxHelper.RichMessageBoxResult richMessageBoxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
				{
					Content = Resources.WinPageReorderToolTipContent,
					ShowLeftBottomCheckbox = true,
					LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
				}, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false);
				if (richMessageBoxResult.Result == MessageBoxResult.Yes)
				{
					bool? checkboxResult = richMessageBoxResult.CheckboxResult;
					if (checkboxResult != null && checkboxResult.GetValueOrDefault())
					{
						ConfigManager.SetDoNotShowFlag("PageReorderDontShow", true);
					}
				}
				if (richMessageBoxResult.Result != MessageBoxResult.Yes)
				{
					return;
				}
			}
			GAManager.SendEvent("PageView", "ReorderPages", "Drag", 1L);
			if (!IAPUtils.IsPaidUserWrapper())
			{
				IAPUtils.ShowPurchaseWindows("ReorderPages", ".pdf");
				return;
			}
			int num2 = num;
			PdfPageEditListModel[] array2 = array;
			int num3 = 0;
			while (num3 < array2.Length && array2[num3].PageIndex < num2)
			{
				num--;
				num3++;
			}
			if (num < 0)
			{
				num = 0;
			}
			int[] array3 = array.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
			int[] array4;
			string text = array3.ConvertToRange(out array4);
			PageDisposeHelper.TryFixResource(this.mainViewModel.Document, array3.Min(), array3.Max());
			using (PdfDocument pdfDocument = PdfDocument.CreateNew(null))
			{
				pdfDocument.Pages.ImportPages(this.mainViewModel.Document, text, 0);
				for (int i = array4.Length - 1; i >= 0; i--)
				{
					this.mainViewModel.Document.Pages.DeleteAt(array4[i]);
				}
				this.mainViewModel.Document.Pages.ImportPages(pdfDocument, string.Format("1-{0}", pdfDocument.Pages.Count), num);
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (pdfControl != null && pdfControl.Document != null)
			{
				pdfControl.UpdateDocLayout();
			}
			this.mainViewModel.UpdateDocumentCore();
			for (int j = 0; j < selectedItems.Length; j++)
			{
				int num4 = j + num;
				if (num4 >= 0 && num4 < this.PageEditListItemSource.Count)
				{
					this.PageEditListItemSource[num4].Selected = true;
				}
			}
			this.mainViewModel.SetCanSaveFlag("Reorder", true);
			GAManager.SendEvent("PageView", "ReorderPages", "Done", 1L);
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00024AD8 File Offset: 0x00022CD8
		public async Task DoCreateBlankPageAsync(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			GAManager.SendEvent("PageView", "PageEditorCreateBlankCmd", "Begin", 1L);
			this.mainViewModel.CreatePdfFileAsync(CreatePdfFileType.Blank);
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00024B1C File Offset: 0x00022D1C
		private async void DoMergePDF()
		{
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00024B4C File Offset: 0x00022D4C
		private async void DoInsertBlankPage(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (IsSiderbar)
			{
				GAManager.SendEvent("PageView", "PageEditorInsertBlankCmd", "Siderbar", 1L);
			}
			else
			{
				GAManager.SendEvent("PageView", "PageEditorInsertBlankCmd", "PageToolbar", 1L);
			}
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				int[] array;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					MainViewModel mainViewModel = this.mainViewModel;
					int[] array2;
					if (mainViewModel == null)
					{
						array2 = null;
					}
					else
					{
						List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
						if (selectedThumbnailList == null)
						{
							array2 = null;
						}
						else
						{
							array2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
						}
					}
					array = array2;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array3;
					if (obj2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				InsertBlankPageDialog insertBlankPageDialog = new InsertBlankPageDialog(array, this.mainViewModel.Document, model != null);
				if (insertBlankPageDialog.ShowDialog().GetValueOrDefault())
				{
					SizeF size = insertBlankPageDialog.PageSize;
					bool insertBefore = insertBlankPageDialog.InsertBefore;
					int insertIndex = (insertBefore ? insertBlankPageDialog.InsertPageIndex : (insertBlankPageDialog.InsertPageIndex + 1));
					int pageCount = insertBlankPageDialog.PageCount;
					for (int i = 0; i < pageCount; i++)
					{
						this.mainViewModel.Document.Pages.InsertPageAt(insertIndex, size.Width, size.Height);
						this.mainViewModel.Document.Pages[insertIndex].GenerateContent(false);
					}
					this.mainViewModel.LastViewPage = this.mainViewModel.Document.Pages[insertIndex];
					this.mainViewModel.UpdateDocumentCore();
					this.FlushViewerAndThumbnail(false);
					StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(new global::System.ValueTuple<int, int>(insertIndex, insertIndex + pageCount - 1)), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
					this.mainViewModel.SelectedPageIndex = insertIndex;
					await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
					{
						for (int j = 0; j < pageCount; j++)
						{
							doc.Pages.DeleteAt(insertIndex);
						}
						this.mainViewModel.UpdateDocumentCore();
						this.FlushViewerAndThumbnail(false);
					}, delegate(PdfDocument doc)
					{
						for (int k = 0; k < pageCount; k++)
						{
							this.mainViewModel.Document.Pages.InsertPageAt(insertIndex, size.Width, size.Height);
							this.mainViewModel.Document.Pages[insertIndex].GenerateContent(false);
						}
						this.mainViewModel.UpdateDocumentCore();
					}, "");
				}
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00024B94 File Offset: 0x00022D94
		private async void DoInsertPDF(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (IsSiderbar)
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromPDFCmd", "Siderbar", 1L);
			}
			else
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromPDFCmd", "PageToolbar", 1L);
			}
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "pdf|*.pdf"
				};
				int[] array;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					MainViewModel mainViewModel = this.mainViewModel;
					int[] array2;
					if (mainViewModel == null)
					{
						array2 = null;
					}
					else
					{
						List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
						if (selectedThumbnailList == null)
						{
							array2 = null;
						}
						else
						{
							array2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
						}
					}
					array = array2;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array3;
					if (obj2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				if (openFileDialog.ShowDialog().GetValueOrDefault())
				{
					PageMergeDialog pageMergeDialog = new PageMergeDialog(openFileDialog.FileName, this.mainViewModel.Document, array, model != null, InsertSourceFileType.PDF);
					pageMergeDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
					pageMergeDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					if (pageMergeDialog.ShowDialog().GetValueOrDefault())
					{
						string sourceFilePath = pageMergeDialog.MergeTempFilePath;
						int insertIndex = pageMergeDialog.InsertPageIndex;
						bool isBefore = pageMergeDialog.InsertBefore;
						int sourceFilePageCount = pageMergeDialog.MergePageCount;
						global::System.ValueTuple<int, int> valueTuple = this.InsertPagesIntoDocument(this.mainViewModel.Document, sourceFilePath, insertIndex, isBefore);
						this.mainViewModel.LastViewPage = this.mainViewModel.Document.Pages[valueTuple.Item1];
						this.mainViewModel.UpdateDocumentCore();
						this.FlushViewerAndThumbnail(false);
						if (!isBefore)
						{
							int insertIndex3 = insertIndex;
						}
						else
						{
							int insertIndex2 = insertIndex;
						}
						StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(valueTuple), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
						await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
						{
							int num = (isBefore ? insertIndex : (insertIndex + 1));
							for (int i = num + sourceFilePageCount - 1; i >= num; i--)
							{
								doc.Pages.DeleteAt(i);
							}
							this.mainViewModel.UpdateDocumentCore();
							this.FlushViewerAndThumbnail(false);
						}, delegate(PdfDocument doc)
						{
							this.InsertPagesIntoDocument(doc, sourceFilePath, insertIndex, isBefore);
							this.mainViewModel.UpdateDocumentCore();
						}, "");
					}
				}
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00024BDC File Offset: 0x00022DDC
		private async void DoInsertWord(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (IsSiderbar)
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromWordCmd", "Siderbar", 1L);
			}
			else
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromWordCmd", "PageToolbar", 1L);
			}
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "Microsoft Office Word|" + UtilManager.WordExtention
				};
				int[] array;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					MainViewModel mainViewModel = this.mainViewModel;
					int[] array2;
					if (mainViewModel == null)
					{
						array2 = null;
					}
					else
					{
						List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
						if (selectedThumbnailList == null)
						{
							array2 = null;
						}
						else
						{
							array2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
						}
					}
					array = array2;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array3;
					if (obj2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				if (openFileDialog.ShowDialog().GetValueOrDefault())
				{
					PageMergeDialog pageMergeDialog = new PageMergeDialog(openFileDialog.FileName, this.mainViewModel.Document, array, model != null, InsertSourceFileType.Doc);
					pageMergeDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
					pageMergeDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					if (pageMergeDialog.ShowDialog().GetValueOrDefault())
					{
						string sourceFilePath = pageMergeDialog.MergeTempFilePath;
						int insertIndex = pageMergeDialog.InsertPageIndex;
						bool isBefore = pageMergeDialog.InsertBefore;
						int sourceFilePageCount = pageMergeDialog.MergePageCount;
						global::System.ValueTuple<int, int> valueTuple = this.InsertPagesIntoDocument(this.mainViewModel.Document, sourceFilePath, insertIndex, isBefore);
						this.mainViewModel.LastViewPage = this.mainViewModel.Document.Pages[valueTuple.Item1];
						this.mainViewModel.UpdateDocumentCore();
						this.FlushViewerAndThumbnail(false);
						if (!isBefore)
						{
							int insertIndex3 = insertIndex;
						}
						else
						{
							int insertIndex2 = insertIndex;
						}
						StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(valueTuple), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
						await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
						{
							int num = (isBefore ? insertIndex : (insertIndex + 1));
							for (int i = num + sourceFilePageCount - 1; i >= num; i--)
							{
								doc.Pages.DeleteAt(i);
							}
							this.mainViewModel.UpdateDocumentCore();
						}, delegate(PdfDocument doc)
						{
							this.InsertPagesIntoDocument(doc, sourceFilePath, insertIndex, isBefore);
							this.mainViewModel.UpdateDocumentCore();
							this.FlushViewerAndThumbnail(false);
						}, "");
					}
				}
			}
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00024C24 File Offset: 0x00022E24
		private async void DoInsertImage(PdfPageEditListModel model = null, bool IsSiderbar = false)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (IsSiderbar)
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromImageCmd", "Siderbar", 1L);
			}
			else
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromImageCmd", "PageToolbar", 1L);
			}
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "Image|" + UtilManager.ImageExtention
				};
				int[] array = null;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					MainViewModel mainViewModel = this.mainViewModel;
					int[] array2;
					if (mainViewModel == null)
					{
						array2 = null;
					}
					else
					{
						List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
						if (selectedThumbnailList == null)
						{
							array2 = null;
						}
						else
						{
							array2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
						}
					}
					array = array2;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array3;
					if (obj2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				if (openFileDialog.ShowDialog().GetValueOrDefault())
				{
					string file = openFileDialog.FileName;
					Bitmap image;
					if (ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(file, out image) == null)
					{
						DrawUtils.ShowUnsupportedImageMessage();
					}
					else
					{
						try
						{
							InsertPageFromImage insertPageFromImage = new InsertPageFromImage(file, this.mainViewModel.Document, array, model != null);
							insertPageFromImage.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
							insertPageFromImage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
							if (!insertPageFromImage.ShowDialog().GetValueOrDefault())
							{
								return;
							}
							PdfBitmap pdfBitmap = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(insertPageFromImage.sourceFile, out image);
							if (pdfBitmap == null)
							{
								DrawUtils.ShowUnsupportedImageMessage();
								return;
							}
							SizeF size = insertPageFromImage.PageSize;
							bool insertBefore = insertPageFromImage.InsertBefore;
							int insertIndex = (insertBefore ? insertPageFromImage.InsertPageIndex : (insertPageFromImage.InsertPageIndex + 1));
							PdfPage pdfPage = this.mainViewModel.Document.Pages.InsertPageAt(insertIndex, size.Width, size.Height);
							float renderwidth = size.Width - 180f;
							float renderheight = size.Height - 144f;
							float num = (float)(image.Height * 72) / image.VerticalResolution;
							float num2 = (float)(image.Width * 72) / image.HorizontalResolution;
							float num3 = Math.Min(renderwidth / num2, renderheight / num);
							if (num3 < 1f)
							{
								num *= num3;
								num2 *= num3;
							}
							float num4 = (renderwidth - num2) / 2f + 90f;
							float num5 = (renderheight - num) / 2f + 72f;
							num4 = ((num4 < 0f) ? 0f : num4);
							num5 = ((num5 < 0f) ? 0f : num5);
							PdfImageObject pdfImageObject = PdfImageObject.Create(this.mainViewModel.Document, pdfBitmap, 0f, 0f);
							pdfPage.PageObjects.Add(pdfImageObject);
							pdfImageObject.Matrix = new FS_MATRIX(num2, 0f, 0f, num, num4, num5);
							pdfPage.GenerateContent(false);
							this.mainViewModel.LastViewPage = pdfPage;
							this.mainViewModel.UpdateDocumentCore();
							this.FlushViewerAndThumbnail(false);
							StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(new global::System.ValueTuple<int, int>(insertIndex, insertIndex)), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
							this.mainViewModel.SelectedPageIndex = insertIndex;
							await this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
							{
								doc.Pages.DeleteAt(insertIndex);
								global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
								MainViewModel mainViewModel2 = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
								if (mainViewModel2 == null)
								{
									return;
								}
								mainViewModel2.UpdateDocumentCore();
							}, delegate(PdfDocument doc)
							{
								PdfPage pdfPage2 = doc.Pages.InsertPageAt(insertIndex, size.Width, size.Height);
								Bitmap bitmap;
								PdfBitmap pdfBitmap2 = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(file, out bitmap);
								if (pdfBitmap2 == null)
								{
									DrawUtils.ShowUnsupportedImageMessage();
									return;
								}
								try
								{
									float num6 = (float)(bitmap.Height * 72) / bitmap.VerticalResolution;
									float num7 = (float)(bitmap.Width * 72) / bitmap.HorizontalResolution;
									float num8 = Math.Min(renderwidth / num7, renderheight / num6);
									if (num8 < 1f)
									{
										num6 *= num8;
										num7 *= num8;
									}
									float num9 = (renderwidth - num7) / 2f + 90f;
									float num10 = (renderheight - num6) / 2f + 72f;
									num9 = ((num9 < 0f) ? 0f : num9);
									num10 = ((num10 < 0f) ? 0f : num10);
									PdfImageObject pdfImageObject2 = PdfImageObject.Create(doc, pdfBitmap2, 0f, 0f);
									pdfPage2.PageObjects.Add(pdfImageObject2);
									pdfImageObject2.Matrix = new FS_MATRIX(num7, 0f, 0f, num6, num9, num10);
									pdfPage2.GenerateContent(false);
									global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(doc);
									MainViewModel mainViewModel3 = ((pdfControl2 != null) ? pdfControl2.DataContext : null) as MainViewModel;
									if (mainViewModel3 != null)
									{
										mainViewModel3.UpdateDocumentCore();
									}
									if (mainViewModel3 != null)
									{
										PageEditorViewModel pageEditors = mainViewModel3.PageEditors;
										if (pageEditors != null)
										{
											pageEditors.FlushViewerAndThumbnail(false);
										}
									}
								}
								catch
								{
								}
								finally
								{
									if (pdfBitmap2 != null)
									{
										pdfBitmap2.Dispose();
									}
									if (bitmap != null)
									{
										bitmap.Dispose();
									}
								}
							}, "");
						}
						catch
						{
							DrawUtils.ShowUnsupportedImageMessage();
							return;
						}
						finally
						{
							if (image != null)
							{
								image.Dispose();
							}
						}
						image = null;
					}
				}
			}
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00024C6C File Offset: 0x00022E6C
		private void DoInsertFormScanner(PdfPageEditListModel model = null, bool IsSiderbar = false, bool creatNew = false)
		{
			if (!creatNew)
			{
				GAManager.SendEvent("PageView", "PageEditorInsertFromScannerCmd", "Count", 1L);
			}
			if ((this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null) && !creatNew)
			{
				return;
			}
			if (!creatNew)
			{
				int[] array;
				if (model != null)
				{
					array = new int[] { model.PageIndex };
				}
				else if (IsSiderbar)
				{
					MainViewModel mainViewModel = this.mainViewModel;
					int[] array2;
					if (mainViewModel == null)
					{
						array2 = null;
					}
					else
					{
						List<PdfThumbnailModel> selectedThumbnailList = mainViewModel.SelectedThumbnailList;
						if (selectedThumbnailList == null)
						{
							array2 = null;
						}
						else
						{
							array2 = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex).ToArray<int>();
						}
					}
					array = array2;
				}
				else
				{
					PdfPageEditList pdfPageEditList = this.PageEditListItemSource;
					object obj;
					if (pdfPageEditList == null)
					{
						obj = null;
					}
					else
					{
						IReadOnlyCollection<PdfPageEditListModel> selectedItems = pdfPageEditList.SelectedItems;
						obj = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
					}
					object obj2 = obj;
					int[] array3;
					if (obj2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = obj2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
					}
					array = array3;
				}
				new InsertPageFromScanner(array, this.mainViewModel.Document, model != null, creatNew, this.mainViewModel)
				{
					Owner = Application.Current.MainWindow
				}.ShowDialog();
				return;
			}
			new InsertPageFromScanner(null, this.mainViewModel.Document, model != null, creatNew, this.mainViewModel)
			{
				Owner = Application.Current.MainWindow
			}.ShowDialog();
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00025048 File Offset: 0x00023248
		[CompilerGenerated]
		internal static void <RotatePagesCore>g__RotateCore|91_0(PdfDocument _doc, int[] _nums, bool _rotateRight)
		{
			PageEditorViewModel.<>c__DisplayClass91_1 CS$<>8__locals1 = new PageEditorViewModel.<>c__DisplayClass91_1();
			CS$<>8__locals1._doc = _doc;
			CS$<>8__locals1._nums = _nums;
			CS$<>8__locals1._rotateRight = _rotateRight;
			ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction p)
			{
				PageEditorViewModel.<>c__DisplayClass91_1.<<RotatePagesCore>b__3>d <<RotatePagesCore>b__3>d;
				<<RotatePagesCore>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<RotatePagesCore>b__3>d.<>4__this = CS$<>8__locals1;
				<<RotatePagesCore>b__3>d.p = p;
				<<RotatePagesCore>b__3>d.<>1__state = -1;
				<<RotatePagesCore>b__3>d.<>t__builder.Start<PageEditorViewModel.<>c__DisplayClass91_1.<<RotatePagesCore>b__3>d>(ref <<RotatePagesCore>b__3>d);
				return <<RotatePagesCore>b__3>d.<>t__builder.Task;
			}, null, null, false, Application.Current.MainWindow, 0);
		}

		// Token: 0x040003CD RID: 973
		private const float ImageLandscapeMargin = 90f;

		// Token: 0x040003CE RID: 974
		private const float ImagePortraitMargin = 72f;

		// Token: 0x040003CF RID: 975
		private readonly MainViewModel mainViewModel;

		// Token: 0x040003D0 RID: 976
		private PdfPageEditList pageEditListItemSource;

		// Token: 0x040003D1 RID: 977
		private double pageEditerThumbnailScale = 0.5;

		// Token: 0x040003D2 RID: 978
		private double pageEditorMinThumbnailScale = 0.3;

		// Token: 0x040003D3 RID: 979
		private double pageEditorMaxThumbnailScale = 2.0;

		// Token: 0x040003D4 RID: 980
		private ToolbarButtonModel headerFooterButtonModel;

		// Token: 0x040003D5 RID: 981
		private ToolbarButtonModel insertPageButtonModel;

		// Token: 0x040003D6 RID: 982
		private ToolbarButtonModel insertPageButtonModel2;

		// Token: 0x040003D7 RID: 983
		private ToolbarButtonModel pageNumberButtonModel;

		// Token: 0x040003D8 RID: 984
		private RelayCommand<PdfPageEditListModel> pageEditorRotateRightCmd;

		// Token: 0x040003D9 RID: 985
		private RelayCommand<PdfPageEditListModel> pageEditorRotateLeftCmd;

		// Token: 0x040003DA RID: 986
		private RelayCommand siderbarRotateRightCmd;

		// Token: 0x040003DB RID: 987
		private RelayCommand siderbarRotateLeftCmd;

		// Token: 0x040003DC RID: 988
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorDeleteCmd;

		// Token: 0x040003DD RID: 989
		private AsyncRelayCommand siderbarDeleteCmd;

		// Token: 0x040003DE RID: 990
		private RelayCommand formBlankPage;

		// Token: 0x040003DF RID: 991
		private RelayCommand formPDF;

		// Token: 0x040003E0 RID: 992
		private RelayCommand formScanner;

		// Token: 0x040003E1 RID: 993
		private RelayCommand formWord;

		// Token: 0x040003E2 RID: 994
		private RelayCommand formImage;

		// Token: 0x040003E3 RID: 995
		private RelayCommand createBlankPage;

		// Token: 0x040003E4 RID: 996
		private RelayCommand createNewFormScanner;

		// Token: 0x040003E5 RID: 997
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorExtractCmd;

		// Token: 0x040003E6 RID: 998
		private AsyncRelayCommand siderbarExtractCmd;

		// Token: 0x040003E7 RID: 999
		private RelayCommand cropPageCmd;

		// Token: 0x040003E8 RID: 1000
		private RelayCommand siderbarCropPageCmd;

		// Token: 0x040003E9 RID: 1001
		private RelayCommand<PdfPageEditListModel> cropPageCmd2;

		// Token: 0x040003EA RID: 1002
		private RelayCommand<PdfPageEditListModel> resizePageCmd;

		// Token: 0x040003EB RID: 1003
		private RelayCommand siderbarResizePageCmd;

		// Token: 0x040003EC RID: 1004
		private RelayCommand<PdfPageEditListModel> _viewerResizePageCmd;

		// Token: 0x040003ED RID: 1005
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorMergeCmd;

		// Token: 0x040003EE RID: 1006
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorInsertBlankCmd;

		// Token: 0x040003EF RID: 1007
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorInsertFromPDFCmd;

		// Token: 0x040003F0 RID: 1008
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorInsertFromScannerCmd;

		// Token: 0x040003F1 RID: 1009
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorInsertFromWordCmd;

		// Token: 0x040003F2 RID: 1010
		private AsyncRelayCommand<PdfPageEditListModel> pageEditorInsertFromImgCmd;

		// Token: 0x040003F3 RID: 1011
		private AsyncRelayCommand pageEditorSplitCmd;

		// Token: 0x040003F4 RID: 1012
		private AsyncRelayCommand pageEditorMergeCmd2;

		// Token: 0x040003F5 RID: 1013
		private RelayCommand pageEditorZoomOutCmd;

		// Token: 0x040003F6 RID: 1014
		private RelayCommand pageEditorZoomInCmd;

		// Token: 0x040003F7 RID: 1015
		private RelayCommand allPageRotateRightCmd;

		// Token: 0x040003F8 RID: 1016
		private RelayCommand allPageRotateLeftCmd;
	}
}
