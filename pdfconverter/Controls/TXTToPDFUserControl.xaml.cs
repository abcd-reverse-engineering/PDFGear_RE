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
	// Token: 0x020000AE RID: 174
	public partial class TXTToPDFUserControl : UserControl
	{
		// Token: 0x06000773 RID: 1907 RVA: 0x0001B712 File Offset: 0x00019912
		public TXTToPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<TXTToPDFUCViewModel>();
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x0001B730 File Offset: 0x00019930
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

		// Token: 0x06000775 RID: 1909 RVA: 0x0001B7A8 File Offset: 0x000199A8
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				TXTToPDFUCViewModel txttoPDFUCViewModel = base.DataContext as TXTToPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					txttoPDFUCViewModel.AddOneFileToFileList(text);
				}
			}
			e.Handled = true;
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x0001B81F File Offset: 0x00019A1F
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
