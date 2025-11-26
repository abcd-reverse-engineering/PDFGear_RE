using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Controls;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Views;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200006B RID: 107
	public class ShareTabViewModel : ObservableObject
	{
		// Token: 0x060007C3 RID: 1987 RVA: 0x0002549D File Offset: 0x0002369D
		public ShareTabViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x000254AC File Offset: 0x000236AC
		public ToolbarButtonModel FileButtonModel
		{
			get
			{
				ToolbarButtonModel toolbarButtonModel;
				if ((toolbarButtonModel = this.fileButtonModel) == null)
				{
					ToolbarButtonModel toolbarButtonModel2 = new ToolbarButtonModel();
					toolbarButtonModel2.Caption = Resources.MenuShareSendFileContent;
					toolbarButtonModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Share_SendFile.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Share_SendFile.png"));
					toolbarButtonModel2.Command = new RelayCommand(delegate
					{
						if (this.mainViewModel.Document == null)
						{
							return;
						}
						if (this.mainViewModel.CanSave)
						{
							ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
						new ShareSendFileDialog(this.mainViewModel.DocumentWrapper.DocumentPath)
						{
							Owner = mainView,
							WindowStartupLocation = ((mainView == null) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner)
						}.ShowDialog();
					});
					ToolbarButtonModel toolbarButtonModel3 = toolbarButtonModel2;
					this.fileButtonModel = toolbarButtonModel2;
					toolbarButtonModel = toolbarButtonModel3;
				}
				return toolbarButtonModel;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060007C5 RID: 1989 RVA: 0x00025514 File Offset: 0x00023714
		public ToolbarButtonModel ShareButtonModel
		{
			get
			{
				ToolbarButtonModel toolbarButtonModel;
				if ((toolbarButtonModel = this.shareButtonModel) == null)
				{
					ToolbarButtonModel toolbarButtonModel2 = new ToolbarButtonModel();
					toolbarButtonModel2.Caption = Resources.MenuShareShareContent;
					toolbarButtonModel2.Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Share_Share.png"));
					toolbarButtonModel2.Command = new AsyncRelayCommand(async delegate
					{
						if (this.mainViewModel.Document != null)
						{
							if (this.mainViewModel.CanSave)
							{
								ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							else
							{
								await ShareUtils.WindowsShareAsync(this.mainViewModel.DocumentWrapper.DocumentPath);
							}
						}
					});
					ToolbarButtonModel toolbarButtonModel3 = toolbarButtonModel2;
					this.shareButtonModel = toolbarButtonModel2;
					toolbarButtonModel = toolbarButtonModel3;
				}
				return toolbarButtonModel;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060007C6 RID: 1990 RVA: 0x00025570 File Offset: 0x00023770
		public ToolbarButtonModel EmailButtonModel
		{
			get
			{
				ToolbarButtonModel toolbarButtonModel;
				if ((toolbarButtonModel = this.emailButtonModel) == null)
				{
					ToolbarButtonModel toolbarButtonModel2 = new ToolbarButtonModel();
					toolbarButtonModel2.Caption = Resources.MenuShareEmailContent;
					toolbarButtonModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Share_Email.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Share_Email.png"));
					toolbarButtonModel2.Command = new AsyncRelayCommand(async delegate
					{
						if (this.mainViewModel.Document != null)
						{
							if (this.mainViewModel.CanSave)
							{
								ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							else
							{
								await ShareUtils.SendMailAsync(this.mainViewModel.DocumentWrapper.DocumentPath);
							}
						}
					});
					ToolbarButtonModel toolbarButtonModel3 = toolbarButtonModel2;
					this.emailButtonModel = toolbarButtonModel2;
					toolbarButtonModel = toolbarButtonModel3;
				}
				return toolbarButtonModel;
			}
		}

		// Token: 0x040003F9 RID: 1017
		private readonly MainViewModel mainViewModel;

		// Token: 0x040003FA RID: 1018
		private ToolbarButtonModel fileButtonModel;

		// Token: 0x040003FB RID: 1019
		private ToolbarButtonModel shareButtonModel;

		// Token: 0x040003FC RID: 1020
		private ToolbarButtonModel emailButtonModel;
	}
}
