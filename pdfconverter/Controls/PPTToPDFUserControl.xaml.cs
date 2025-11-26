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
	// Token: 0x020000AA RID: 170
	public partial class PPTToPDFUserControl : UserControl
	{
		// Token: 0x0600075A RID: 1882 RVA: 0x0001B062 File Offset: 0x00019262
		public PPTToPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<PPTToPDFUCViewModel>();
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x0001B080 File Offset: 0x00019280
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

		// Token: 0x0600075C RID: 1884 RVA: 0x0001B0F8 File Offset: 0x000192F8
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				PPTToPDFUCViewModel ppttoPDFUCViewModel = base.DataContext as PPTToPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					ppttoPDFUCViewModel.AddOneFileToFileList(text);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0001B16F File Offset: 0x0001936F
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
