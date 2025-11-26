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
	// Token: 0x020000AF RID: 175
	public partial class WordToPDFUserControl : UserControl
	{
		// Token: 0x0600077A RID: 1914 RVA: 0x0001B917 File Offset: 0x00019B17
		public WordToPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<WordToPDFUCViewModel>();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0001B938 File Offset: 0x00019B38
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

		// Token: 0x0600077C RID: 1916 RVA: 0x0001B9B0 File Offset: 0x00019BB0
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				WordToPDFUCViewModel wordToPDFUCViewModel = base.DataContext as WordToPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					wordToPDFUCViewModel.AddOneFileToFileList(text);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0001BA27 File Offset: 0x00019C27
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
