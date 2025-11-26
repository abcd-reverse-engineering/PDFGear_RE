using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfconverter.ViewModels;

namespace pdfconverter.Controls
{
	// Token: 0x020000AB RID: 171
	public partial class RTFToPDFUserControl : UserControl
	{
		// Token: 0x06000761 RID: 1889 RVA: 0x0001B267 File Offset: 0x00019467
		public RTFToPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<RTFToPDFUCViewModel>();
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0001B288 File Offset: 0x00019488
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			GridView gridView = this.lsvFiles.View as GridView;
			double num = 0.0;
			for (int i = gridView.Columns.Count - 1; i > 0; i--)
			{
				num += gridView.Columns[i].ActualWidth;
			}
			gridView.Columns[0].Width = base.ActualWidth - num - 10.0;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0001B300 File Offset: 0x00019500
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				RTFToPDFUCViewModel rtftoPDFUCViewModel = base.DataContext as RTFToPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					rtftoPDFUCViewModel.AddOneFileToFileList(text);
				}
			}
			e.Handled = true;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0001B377 File Offset: 0x00019577
		private void lsvFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}
	}
}
