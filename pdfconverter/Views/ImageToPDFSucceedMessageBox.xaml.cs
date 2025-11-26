using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using CommonLib.Common;
using pdfconverter.Controls;
using pdfconverter.Models;
using pdfconverter.Models.Image;

namespace pdfconverter.Views
{
	// Token: 0x02000029 RID: 41
	public partial class ImageToPDFSucceedMessageBox : Window
	{
		// Token: 0x06000235 RID: 565 RVA: 0x00008657 File Offset: 0x00006857
		public ImageToPDFSucceedMessageBox()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00008668 File Offset: 0x00006868
		public ImageToPDFSucceedMessageBox(bool isSingleFile, string failnumber = "", ImageToPDFStatusModel filelList = null)
		{
			this.InitializeComponent();
			if (filelList != null && filelList.Count > 0)
			{
				this.FailedList.Visibility = Visibility.Visible;
				this.lsvFiles.ItemsSource = filelList;
			}
			int num = 0;
			using (IEnumerator<ToPDFFileItem> enumerator = filelList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Status == ToPDFItemStatus.Succ)
					{
						num++;
					}
				}
			}
			this.SucceedFileCount.Text = num.ToString();
			this.FailImageCount.Text = failnumber;
			this.OpenFile.IsChecked = new bool?(ConfigManager.GetImageConvertedFalg("ImageToPDFOpenFileFlag"));
			this.OpenExplorer.IsChecked = new bool?(ConfigManager.GetImageConvertedFalg("ImageToPDFOpenExplorerFlag"));
			base.Closing += this.ImageToPDFSucceedMessageBox_Closing;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000874C File Offset: 0x0000694C
		private void ImageToPDFSucceedMessageBox_Closing(object sender, CancelEventArgs e)
		{
			if (!base.DialogResult.GetValueOrDefault())
			{
				base.DialogResult = new bool?(false);
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00008775 File Offset: 0x00006975
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00008780 File Offset: 0x00006980
		private void PrimaryButton_Click(object sender, RoutedEventArgs e)
		{
			bool value = this.OpenFile.IsChecked.Value;
			bool value2 = this.OpenExplorer.IsChecked.Value;
			if (ConfigManager.GetImageConvertedFalg("ImageToPDFOpenFileFlag") != value)
			{
				ConfigManager.SetImageConvertedFalg("ImageToPDFOpenFileFlag", value);
			}
			if (ConfigManager.GetImageConvertedFalg("ImageToPDFOpenExplorerFlag") != value2)
			{
				ConfigManager.SetImageConvertedFalg("ImageToPDFOpenExplorerFlag", value2);
			}
			base.DialogResult = new bool?(true);
			base.Close();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x000087F8 File Offset: 0x000069F8
		private void openFileInExplorerSplitBtn_Click(object sender, RoutedEventArgs e)
		{
			ToPDFFileItem toPDFFileItem = (sender as Button).DataContext as ToPDFFileItem;
			if (toPDFFileItem != null)
			{
				string filePath = toPDFFileItem.FilePath;
				if (LongPathFile.Exists(filePath))
				{
					UtilsManager.OpenFileInExplore(filePath, true);
				}
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00008830 File Offset: 0x00006A30
		private void openFileWithDefaultApp_Click(object sender, RoutedEventArgs e)
		{
			ToPDFFileItem toPDFFileItem = (sender as Button).DataContext as ToPDFFileItem;
			if (toPDFFileItem != null)
			{
				string filePath = toPDFFileItem.FilePath;
				if (LongPathFile.Exists(filePath))
				{
					UtilsManager.OpenFile(filePath);
				}
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000899F File Offset: 0x00006B9F
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 10)
			{
				((Button)target).Click += this.openFileInExplorerSplitBtn_Click;
				return;
			}
			if (connectionId != 11)
			{
				return;
			}
			((Button)target).Click += this.openFileWithDefaultApp_Click;
		}
	}
}
