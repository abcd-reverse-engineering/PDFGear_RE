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
	// Token: 0x0200009B RID: 155
	public partial class CompressPDFUserControl : UserControl
	{
		// Token: 0x060006CF RID: 1743 RVA: 0x0001844C File Offset: 0x0001664C
		public CompressPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<CompressPDFUCViewModel>();
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0001846C File Offset: 0x0001666C
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				CompressPDFUCViewModel compressPDFUCViewModel = base.DataContext as CompressPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					compressPDFUCViewModel.AddOneFileToFileList(text, null);
				}
			}
			e.Handled = true;
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x000184E4 File Offset: 0x000166E4
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
