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
	// Token: 0x020000A2 RID: 162
	public partial class ImageToPDFUserControl : UserControl
	{
		// Token: 0x06000710 RID: 1808 RVA: 0x000198BE File Offset: 0x00017ABE
		public ImageToPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<ImageToPDFUCViewModel>();
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x000198DC File Offset: 0x00017ADC
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

		// Token: 0x06000712 RID: 1810 RVA: 0x00019954 File Offset: 0x00017B54
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				ImageToPDFUCViewModel imageToPDFUCViewModel = base.DataContext as ImageToPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					imageToPDFUCViewModel.AddOneFileToFileList(text);
				}
			}
			e.Handled = true;
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000199CB File Offset: 0x00017BCB
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
