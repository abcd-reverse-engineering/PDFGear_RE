using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfconverter
{
	// Token: 0x0200001A RID: 26
	public partial class OCRSettingsWindow : Window
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000034DA File Offset: 0x000016DA
		public OCRSettingsWindow()
		{
			this.InitializeComponent();
			this.InitLanguages();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000034F0 File Offset: 0x000016F0
		private void InitLanguages()
		{
			foreach (KeyValuePair<OCRLanguageID, string> keyValuePair in ConvertManager.OCRLanguagesL10n)
			{
				this.languagesListView.Items.Add(keyValuePair.Value);
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00003554 File Offset: 0x00001754
		private void updateSelectedLanguage()
		{
			base.Dispatcher.Invoke(delegate
			{
				OCRLanguageID ocrlanguageID = ConvertManager.GetOCRLanguageID();
				this.languagesListView.SelectedIndex = (int)ocrlanguageID;
				int num = (int)(ocrlanguageID + 5);
				if (num > this.languagesListView.Items.Count - 1)
				{
					num = this.languagesListView.Items.Count - 1;
				}
				this.languagesListView.ScrollIntoView(this.languagesListView.SelectedItem);
				ListViewItem listViewItem = (ListViewItem)this.languagesListView.ItemContainerGenerator.ContainerFromIndex(this.languagesListView.SelectedIndex);
				if (listViewItem != null)
				{
					listViewItem.Focus();
				}
				object obj = this.languagesListView.Items[num];
				this.languagesListView.ScrollIntoView(obj);
			});
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003570 File Offset: 0x00001770
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				this.updateSelectedLanguage();
			}
			catch
			{
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003598 File Offset: 0x00001798
		private void OkBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int selectedIndex = this.languagesListView.SelectedIndex;
				if (selectedIndex < 0 || selectedIndex >= ConvertManager.OCRLanguages.Count)
				{
					return;
				}
				if (!ConvertManager.OCRLanguages.ContainsKey((OCRLanguageID)selectedIndex))
				{
					return;
				}
				ConfigManager.SetOCRLanguageID(selectedIndex);
			}
			catch
			{
			}
			this.ret = true;
			base.Close();
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000035FC File Offset: 0x000017FC
		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			this.ret = false;
			base.Close();
		}

		// Token: 0x040000F5 RID: 245
		public bool ret;
	}
}
