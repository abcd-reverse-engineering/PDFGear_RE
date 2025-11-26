using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.Properties;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000213 RID: 531
	public partial class OcrSelectLanguageDialog : Window
	{
		// Token: 0x06001D6B RID: 7531 RVA: 0x0007EC6C File Offset: 0x0007CE6C
		public OcrSelectLanguageDialog(string cultureInfoName, bool supportAutoMode)
		{
			if (cultureInfoName == "zh-CN")
			{
				cultureInfoName = "zh-Hans";
			}
			else if (cultureInfoName == "zh-HK" || cultureInfoName == "zh-TW")
			{
				cultureInfoName = "zh-Hant";
			}
			this.InitializeComponent();
			List<OcrSelectLanguageDialog.LanguageInfo> list = (from c in OcrUtils.GetLanguageList()
				select new OcrSelectLanguageDialog.LanguageInfo(c.Item1, c.Item2)).ToList<OcrSelectLanguageDialog.LanguageInfo>();
			if (supportAutoMode)
			{
				list.Insert(0, new OcrSelectLanguageDialog.LanguageInfo("Auto", pdfeditor.Properties.Resources.AppSettingsLanguageAutoItem));
			}
			this.LanguageListBox.ItemsSource = list;
			this.LanguageListBox.SelectedItem = list.FirstOrDefault((OcrSelectLanguageDialog.LanguageInfo c) => c.CultureInfoName == cultureInfoName);
			this.OKButton.IsEnabled = this.LanguageListBox.SelectedItem is OcrSelectLanguageDialog.LanguageInfo;
			base.Loaded += this.OcrSelectLanguageDialog_Loaded;
		}

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06001D6C RID: 7532 RVA: 0x0007ED81 File Offset: 0x0007CF81
		// (set) Token: 0x06001D6D RID: 7533 RVA: 0x0007ED89 File Offset: 0x0007CF89
		public string SelectedCultureInfoName { get; private set; }

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x0007ED92 File Offset: 0x0007CF92
		// (set) Token: 0x06001D6F RID: 7535 RVA: 0x0007ED9A File Offset: 0x0007CF9A
		public string SelectedDisplayName { get; private set; }

		// Token: 0x06001D70 RID: 7536 RVA: 0x0007EDA3 File Offset: 0x0007CFA3
		private void OcrSelectLanguageDialog_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.LanguageListBox.SelectedItem is OcrSelectLanguageDialog.LanguageInfo)
			{
				this.LanguageListBox.ScrollIntoView(this.LanguageListBox.SelectedItem);
			}
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x0007EDCD File Offset: 0x0007CFCD
		private void LanguageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.OKButton.IsEnabled = this.LanguageListBox.SelectedItem is OcrSelectLanguageDialog.LanguageInfo;
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x0007EDF0 File Offset: 0x0007CFF0
		private async void LanguageListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			await Task.Delay(10);
			this.OKButton_Click(this.OKButton, null);
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x0007EE28 File Offset: 0x0007D028
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			OcrSelectLanguageDialog.LanguageInfo languageInfo = this.LanguageListBox.SelectedItem as OcrSelectLanguageDialog.LanguageInfo;
			if (languageInfo != null)
			{
				this.SelectedCultureInfoName = languageInfo.CultureInfoName;
				this.SelectedDisplayName = languageInfo.DisplayName;
				base.DialogResult = new bool?(true);
			}
		}

		// Token: 0x02000636 RID: 1590
		private class LanguageInfo
		{
			// Token: 0x06003393 RID: 13203 RVA: 0x000FCD5E File Offset: 0x000FAF5E
			public LanguageInfo(string cultureInfoName, string displayName)
			{
				this.CultureInfoName = cultureInfoName;
				this.DisplayName = displayName;
			}

			// Token: 0x17000D3E RID: 3390
			// (get) Token: 0x06003394 RID: 13204 RVA: 0x000FCD74 File Offset: 0x000FAF74
			public string CultureInfoName { get; }

			// Token: 0x17000D3F RID: 3391
			// (get) Token: 0x06003395 RID: 13205 RVA: 0x000FCD7C File Offset: 0x000FAF7C
			public string DisplayName { get; }
		}
	}
}
