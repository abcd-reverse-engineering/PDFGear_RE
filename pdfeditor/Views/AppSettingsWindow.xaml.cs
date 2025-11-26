using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Common.HotKeys;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models.Viewer;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Views
{
	// Token: 0x02000048 RID: 72
	public partial class AppSettingsWindow : Window
	{
		// Token: 0x06000348 RID: 840 RVA: 0x0000FB0C File Offset: 0x0000DD0C
		public AppSettingsWindow()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<AppSettingsViewModel>();
			base.Loaded += this.AppSettingsWindow_Loaded;
			AppSettingsWindow.SetHeaderedControlSplitLineVisible(this.ItemsStackPanel.Children.OfType<HeaderedContentControl>().LastOrDefault<HeaderedContentControl>(), false);
			this.AuthorTextBox.Text = AnnotationAuthorUtil.GetAuthorName();
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000FB72 File Offset: 0x0000DD72
		public AppSettingsViewModel VM
		{
			get
			{
				return base.DataContext as AppSettingsViewModel;
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000FB7F File Offset: 0x0000DD7F
		private void AppSettingsWindow_Loaded(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("SettingsWindow", "Show", "Count", 1L);
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0000FB97 File Offset: 0x0000DD97
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			Ioc.Default.GetRequiredService<AppSettingsViewModel>().RefreshSettingsAsync();
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000FBB0 File Offset: 0x0000DDB0
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("SettingsWindow", "OkBtn", "Count", 1L);
			base.DialogResult = new bool?(true);
			this.VM.UpdateAppSettingsState();
			AnnotationAuthorUtil.SetAuthorName(this.AuthorTextBox.Text.Trim());
			Log.WriteLog("AuthorName Changed: " + (string.IsNullOrWhiteSpace(this.AuthorTextBox.Text) ? "Empty" : "Custom"));
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000FC2C File Offset: 0x0000DE2C
		public static bool GetHeaderedControlSplitLineVisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(AppSettingsWindow.HeaderedControlSplitLineVisibleProperty);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000FC3E File Offset: 0x0000DE3E
		public static void SetHeaderedControlSplitLineVisible(DependencyObject obj, bool value)
		{
			obj.SetValue(AppSettingsWindow.HeaderedControlSplitLineVisibleProperty, value);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000FC51 File Offset: 0x0000DE51
		private void RestoreButton_Click(object sender, RoutedEventArgs e)
		{
			HotKeyManager.ResetAllKeysToDefault();
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000FC58 File Offset: 0x0000DE58
		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000FC5A File Offset: 0x0000DE5A
		private void ThemesCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0000FC5C File Offset: 0x0000DE5C
		private void InitialThemes()
		{
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000FC5E File Offset: 0x0000DE5E
		private void InitialBackgroundColors()
		{
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000FC60 File Offset: 0x0000DE60
		private void InitialShowStatusBarCheckBox()
		{
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000FC62 File Offset: 0x0000DE62
		private void BackgroundColorsCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000FC64 File Offset: 0x0000DE64
		private void ShowStatusBarCheckBox_Click(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate
			{
				CheckBox checkBox = sender as CheckBox;
				if (checkBox != null)
				{
					((MainView)App.Current.MainWindow).SetFooterVisible(checkBox.IsChecked.Value);
				}
			}));
		}

		// Token: 0x0400014B RID: 331
		public List<string> ThemesList;

		// Token: 0x0400014C RID: 332
		public List<string> BackgroundColorNamesList;

		// Token: 0x0400014D RID: 333
		private static List<BackgroundColorSetting> viewerBackgroundColorValues;

		// Token: 0x0400014E RID: 334
		public static readonly DependencyProperty HeaderedControlSplitLineVisibleProperty = DependencyProperty.RegisterAttached("HeaderedControlSplitLineVisible", typeof(bool), typeof(AppSettingsWindow), new PropertyMetadata(true));
	}
}
