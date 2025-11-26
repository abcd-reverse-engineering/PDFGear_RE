using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfconverter.Properties;
using pdfconverter.ViewModels;

namespace pdfconverter.Controls
{
	// Token: 0x020000AC RID: 172
	public partial class SplitPDFUserControl : UserControl
	{
		// Token: 0x06000768 RID: 1896 RVA: 0x0001B46F File Offset: 0x0001966F
		public SplitPDFUserControl()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<SplitPDFUCViewModel>();
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0001B490 File Offset: 0x00019690
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

		// Token: 0x0600076A RID: 1898 RVA: 0x0001B507 File Offset: 0x00019707
		private void splitModeHelpBtn_Click(object sender, RoutedEventArgs e)
		{
			ModernMessageBox.Show(pdfconverter.Properties.Resources.WinMergeSplitSplitModeCustomRangeHelptipMsg + "\r\n" + pdfconverter.Properties.Resources.WinMergeSplitSplitModeFixedRangeHelptipMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0001B52C File Offset: 0x0001972C
		private void lsvFiles_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				SplitPDFUCViewModel splitPDFUCViewModel = base.DataContext as SplitPDFUCViewModel;
				foreach (string text in array)
				{
					if (string.IsNullOrWhiteSpace(text))
					{
						break;
					}
					DocsPathUtils.WriteFilesPathJson("unknow", text, null);
					splitPDFUCViewModel.AddOneFileToSplitList(text, null);
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0001B5A4 File Offset: 0x000197A4
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

		// Token: 0x06000770 RID: 1904 RVA: 0x0001B69F File Offset: 0x0001989F
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 5)
			{
				((Button)target).Click += this.splitModeHelpBtn_Click;
			}
		}
	}
}
