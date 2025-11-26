using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using PDFLauncher.CustomControl;
using PDFLauncher.Models;
using PDFLauncher.Utils;
using PDFLauncher.ViewModels;

namespace PDFLauncher
{
	// Token: 0x0200000F RID: 15
	public partial class MainWindow : Window
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00002E24 File Offset: 0x00001024
		public MainWindow()
		{
			this.InitializeComponent();
			WSUtils.LoadWindowInfo("Launcher");
			base.DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
			GAManager.SendEvent("WelcomeWindow", "Show", "Count", 1L);
			base.SizeChanged += this.MainWindow_SizeChanged;
			if (!ConfigManager.GetWelcomeOpenBtnTipsFlag())
			{
				int welcomeOpenBtnShowTipsCount = ConfigManager.GetWelcomeOpenBtnShowTipsCount();
				if (welcomeOpenBtnShowTipsCount <= 2)
				{
					ConfigManager.SetWelcomeOpenBtnShowTipsCount(welcomeOpenBtnShowTipsCount + 1);
					this.ShowAllTips();
				}
				else
				{
					this.CloseAllTips();
				}
			}
			else
			{
				this.CloseAllTips();
			}
			base.Activated += this.MainWindow_Activated;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002ED0 File Offset: 0x000010D0
		private void MainWindow_Deactivated(object sender, EventArgs e)
		{
			if (this.OpenButtonTipsGrid.Visibility == Visibility.Visible)
			{
				((Storyboard)this.OpenButtonTipsGrid.Resources["OpenButtonTipsAnimation"]).Stop();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002EFE File Offset: 0x000010FE
		private void MainWindow_Activated(object sender, EventArgs e)
		{
			if (this.OpenButtonTipsGrid.Visibility == Visibility.Visible)
			{
				((Storyboard)this.OpenButtonTipsGrid.Resources["OpenButtonTipsAnimation"]).Begin();
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002F2C File Offset: 0x0000112C
		private void openFileBtn_Click(object sender, RoutedEventArgs e)
		{
			string text = FileManager.SelectFileForOpen();
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			DocsPathUtils.WriteFilesPathJson("unknow", text, null);
			FileManager.OpenOneFile(text, null);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002F60 File Offset: 0x00001160
		private void openFileGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDEDE"));
			this.openFileGrid.Background = brush;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002F90 File Offset: 0x00001190
		private void openFileGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0F0"));
			this.openFileGrid.Background = brush;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002FBE File Offset: 0x000011BE
		private void FastStartClick(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002FC0 File Offset: 0x000011C0
		private void Image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.VM.OpenOneFileCMD.Execute(null);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002FD3 File Offset: 0x000011D3
		private void fileItemListSelectAll(object sender, RoutedEventArgs e)
		{
			object dataContext = this.lsvOpenHistory.DataContext;
			this.VM.SelectAll();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002FEC File Offset: 0x000011EC
		private void fileItemListSelectNone(object sender, RoutedEventArgs e)
		{
			this.VM.SelectNone();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002FF9 File Offset: 0x000011F9
		private void fileItemChecked(object sender, RoutedEventArgs e)
		{
			this.VM.SelectItemsPropertyChange();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003006 File Offset: 0x00001206
		private void fileItemUnchecked(object sender, RoutedEventArgs e)
		{
			this.VM.SelectItemsPropertyChange();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003014 File Offset: 0x00001214
		private void lstBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.stpHotTools.Visibility = Visibility.Collapsed;
			this.stpConvert.Visibility = Visibility.Collapsed;
			this.stpConToPDF.Visibility = Visibility.Collapsed;
			this.stpMergeSplit.Visibility = Visibility.Collapsed;
			this.stpAllTools.Visibility = Visibility.Collapsed;
			switch (this.lstBoxMenu.SelectedIndex)
			{
			case 0:
				this.stpHotTools.Visibility = Visibility.Visible;
				return;
			case 1:
				this.stpConvert.Visibility = Visibility.Visible;
				return;
			case 2:
				this.stpConToPDF.Visibility = Visibility.Visible;
				return;
			case 3:
				this.stpMergeSplit.Visibility = Visibility.Visible;
				return;
			case 4:
				this.stpAllTools.Visibility = Visibility.Visible;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000030C4 File Offset: 0x000012C4
		private void Window_Closing(object sender, CancelEventArgs e)
		{
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000030C8 File Offset: 0x000012C8
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			this.VM.SelectFilesSupportPropertChange(button.DataContext as OpenHistoryModel);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000030F2 File Offset: 0x000012F2
		private void pdfto_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000030F4 File Offset: 0x000012F4
		private void singleFileOperateSub_Closed(object sender, EventArgs e)
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000030F8 File Offset: 0x000012F8
		private void tbFilePath_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OpenHistoryModel openHistoryModel = (sender as TextBlock).DataContext as OpenHistoryModel;
			Process.Start("explorer.exe", "/select," + openHistoryModel.FilePath);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003131 File Offset: 0x00001331
		private void LBtnRemoveClick(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003133 File Offset: 0x00001333
		private void Grid_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				base.DragMove();
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003144 File Offset: 0x00001344
		private void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.Source is CheckBox)
			{
				return;
			}
			try
			{
				OpenHistoryModel openHistoryModel = (sender as ListBoxItem).DataContext as OpenHistoryModel;
				if (!string.IsNullOrWhiteSpace(openHistoryModel.FilePath))
				{
					GAManager.SendEvent("WelcomeWindow", "RecentOpenFile", "Count", 1L);
					DocsPathUtils.WriteFilesPathJson("unknow", openHistoryModel.FilePath, null);
					FileManager.OpenOneFile(openHistoryModel.FilePath, null);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000031CC File Offset: 0x000013CC
		private void AllToolsSwitch_Click(object sender, RoutedEventArgs e)
		{
			SwitchButton switchButton = sender as SwitchButton;
			switchButton.ContextMenu.DataContext = switchButton.DataContext;
			if (switchButton.ContextMenu.IsOpen)
			{
				switchButton.ContextMenu.PlacementTarget = switchButton;
				switchButton.ContextMenu.Placement = PlacementMode.Bottom;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003216 File Offset: 0x00001416
		private void ContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			this.VM.AllToolsSwitchIsChecked = false;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003224 File Offset: 0x00001424
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			WSUtils.SaveWindowInfo("Launcher");
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003237 File Offset: 0x00001437
		private void fileItemCB_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003240 File Offset: 0x00001440
		private void Button_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003249 File Offset: 0x00001449
		private void openFileTipsCloseBtn_Click(object sender, RoutedEventArgs e)
		{
			ConfigManager.SetWelcomeOpenBtnTipsFlag(true);
			this.CloseAllTips();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003258 File Offset: 0x00001458
		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 0.0 && e.NewSize.Height > 0.0)
			{
				Rect rect = this.openFileBtn.TransformToVisual(this.OpenFileBorder).TransformBounds(new Rect(0.0, 0.0, this.openFileBtn.ActualWidth, this.openFileBtn.ActualHeight));
				Canvas.SetLeft(this.OpenButtonTips, rect.Right);
				Canvas.SetTop(this.OpenButtonTips, rect.Top + (rect.Height - this.OpenButtonTips.ActualHeight) / 2.0);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003323 File Offset: 0x00001523
		private void CloseAllTips()
		{
			this.VM.OpenBtnTipsVisibility = Visibility.Collapsed;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003331 File Offset: 0x00001531
		private void ShowAllTips()
		{
			this.VM.OpenBtnTipsVisibility = Visibility.Visible;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000333F File Offset: 0x0000153F
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (ThemeResourceDictionary.GetForCurrentApp().Theme == "Light")
			{
				ThemeResourceDictionary.GetForCurrentApp().Theme = "Dark";
				return;
			}
			ThemeResourceDictionary.GetForCurrentApp().Theme = "Light";
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003376 File Offset: 0x00001576
		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003378 File Offset: 0x00001578
		private void Window_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				foreach (string text in (string[])e.Data.GetData(DataFormats.FileDrop))
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					GAManager.SendEvent("WelcomeWindow", "OpenOneFileBtnClick", "Count", 1L);
					FileManager.OpenOneFile(text, null);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000033FC File Offset: 0x000015FC
		private void Btn_Login_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("WelcomeWindow", "LoginBtnClick", "Count", 1L);
			new UserLoginWin().ShowDialog();
			if (this.VM.UserInfoModel != null)
			{
				Window window = Window.GetWindow(this.Popup_UserInfo);
				if (window != null)
				{
					window.SetForegroundWindow();
				}
				this.Popup_UserInfo.IsOpen = true;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000345C File Offset: 0x0000165C
		private void Btn_Feedback_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("WelcomeWindow", "FeedbackBtnClick", "Count", 1L);
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.HideFaq();
			feedbackWindow.Owner = Application.Current.MainWindow;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			feedbackWindow.source = "Welcome";
			feedbackWindow.ShowDialog();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000034B2 File Offset: 0x000016B2
		private void Btn_Setting_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000034B4 File Offset: 0x000016B4
		private void Btn_CheckUpgrade_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("WelcomeWindow", "UpdateBtnClick", "Count", 1L);
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
			{
				FileWatcherHelper.Instance.UpdateState(700);
				await UpdateHelper.UpdateAndExit(true);
			}));
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003503 File Offset: 0x00001703
		private void Btn_AppAbout_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("WelcomeWindow", "AboutBtnClick", "Count", 1L);
			AboutWindow aboutWindow = new AboutWindow();
			if (aboutWindow == null)
			{
				return;
			}
			aboutWindow.ShowDialog();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000352B File Offset: 0x0000172B
		private void ScanDocument_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("Ads", "ScanPDF", "BtnClick", 1L);
			new GearForPDFScan
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003D1C File Offset: 0x00001F1C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 92:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = Control.MouseDoubleClickEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.OnListViewItemDoubleClick);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 93:
				break;
			case 94:
				((CheckBox)target).Checked += this.fileItemListSelectAll;
				((CheckBox)target).Unchecked += this.fileItemListSelectNone;
				return;
			case 95:
				((CheckBox)target).Checked += this.fileItemChecked;
				((CheckBox)target).Unchecked += this.fileItemUnchecked;
				((CheckBox)target).PreviewMouseDoubleClick += this.fileItemCB_PreviewMouseDoubleClick;
				return;
			case 96:
				((Button)target).Click += this.Button_Click;
				((Button)target).PreviewMouseDoubleClick += this.Button_PreviewMouseDoubleClick;
				break;
			default:
				return;
			}
		}

		// Token: 0x0400001C RID: 28
		private MainViewModel VM = Ioc.Default.GetRequiredService<MainViewModel>();
	}
}
