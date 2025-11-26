using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.AutoSaveRestore;
using pdfeditor.ViewModels;

namespace pdfeditor.Views
{
	// Token: 0x02000050 RID: 80
	public partial class SettingWindow : Window
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x000150B2 File Offset: 0x000132B2
		// (set) Token: 0x06000406 RID: 1030 RVA: 0x000150C4 File Offset: 0x000132C4
		public bool IsAutoSave
		{
			get
			{
				return (bool)base.GetValue(SettingWindow.IsAutoSaveProperty);
			}
			set
			{
				base.SetValue(SettingWindow.IsAutoSaveProperty, value);
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x000150D7 File Offset: 0x000132D7
		// (set) Token: 0x06000408 RID: 1032 RVA: 0x000150E9 File Offset: 0x000132E9
		public string SpanMinutes
		{
			get
			{
				return (string)base.GetValue(SettingWindow.SpanMinutesProperty);
			}
			set
			{
				base.SetValue(SettingWindow.SpanMinutesProperty, value);
			}
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x000150F7 File Offset: 0x000132F7
		public SettingWindow()
		{
			this.InitializeComponent();
			this.GetConfig();
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0001510C File Offset: 0x0001330C
		private void GetConfig()
		{
			CommonLib.Config.ConfigModels.AutoSaveModel result = ConfigManager.GetAutoSaveAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (result != null)
			{
				this.IsAutoSave = result.IsAutoSave;
				this.SpanMinutes = result.FrequencyMins.ToString();
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00015158 File Offset: 0x00013358
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00015160 File Offset: 0x00013360
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			bool isAutoSave = this.IsAutoSave;
			int num = 5;
			if (!int.TryParse(this.SpanMinutes, out num))
			{
				ModernMessageBox.Show("the number is not valid.", UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				this.SpanMinutes = "5";
				return;
			}
			if (isAutoSave && num == 0)
			{
				ModernMessageBox.Show("Set the numbers greater than 0.", UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				this.SpanMinutes = "5";
				return;
			}
			ConfigManager.SetAutoSaveAsync(isAutoSave, num);
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (isAutoSave)
			{
				requiredService.AutoSaveModel.IsAuto = isAutoSave;
				requiredService.AutoSaveModel.SpanMinutes = num;
				if (requiredService.CanSave)
				{
					pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().Start(num);
				}
			}
			else
			{
				requiredService.AutoSaveModel.IsAuto = false;
				pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().Stop();
			}
			base.Close();
		}

		// Token: 0x0400024D RID: 589
		public static readonly DependencyProperty IsAutoSaveProperty = DependencyProperty.Register("IsAutoSave", typeof(bool), typeof(SettingWindow), new PropertyMetadata(true));

		// Token: 0x0400024E RID: 590
		public static readonly DependencyProperty SpanMinutesProperty = DependencyProperty.Register("SpanMinutesProperty", typeof(string), typeof(SettingWindow), new PropertyMetadata("5"));
	}
}
