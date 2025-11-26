using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfconverter.Controls;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.ViewModels;

namespace pdfconverter.Views
{
	// Token: 0x0200002A RID: 42
	public partial class MainWindow2 : Window
	{
		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000240 RID: 576 RVA: 0x000089DB File Offset: 0x00006BDB
		public MainWindow2ViewModel VM
		{
			get
			{
				return base.DataContext as MainWindow2ViewModel;
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000089E8 File Offset: 0x00006BE8
		public MainWindow2()
		{
			this.InitializeComponent();
			base.DataContext = Ioc.Default.GetRequiredService<MainWindow2ViewModel>();
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00008A08 File Offset: 0x00006C08
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				switch ((ConvToPDFType)App.convertType)
				{
				case ConvToPDFType.MergePDF:
					this.HidenUC(this.UCMergePDF);
					this.Menus.SelectedIndex = 0;
					break;
				case ConvToPDFType.SplitPDF:
					this.Menus.SelectedIndex = 1;
					this.HidenUC(this.UCSplitPDF);
					break;
				case ConvToPDFType.CompressPDF:
					this.Menus.SelectedIndex = 2;
					this.HidenUC(this.CompressPDF);
					break;
				case ConvToPDFType.WordToPDF:
					this.Menus.SelectedIndex = 3;
					this.HidenUC(this.UCWordToPDF);
					break;
				case ConvToPDFType.ExcelToPDF:
					this.Menus.SelectedIndex = 4;
					this.HidenUC(this.UCExcelToPDF);
					break;
				case ConvToPDFType.PPTToPDF:
					this.Menus.SelectedIndex = 5;
					this.HidenUC(this.UCPPTToPDF);
					break;
				case ConvToPDFType.ImageToPDF:
					this.Menus.SelectedIndex = 6;
					this.HidenUC(this.UCImageToPDF);
					break;
				case ConvToPDFType.RtfToPDF:
					this.Menus.SelectedIndex = 7;
					this.HidenUC(this.UCRTFToPDF);
					break;
				case ConvToPDFType.TxtToPDF:
					this.Menus.SelectedIndex = 8;
					this.HidenUC(this.UCTXTToPDF);
					break;
				}
			}
			catch (Exception ex)
			{
				Log.Instance.Error<Exception>(ex);
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00008B70 File Offset: 0x00006D70
		private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				string tag = (this.Menus.SelectedItem as ActionMenuGroup).Tag;
				if (tag != null)
				{
					switch (tag.Length)
					{
					case 8:
						switch (tag[0])
						{
						case 'm':
							if (tag == "mergepdf")
							{
								this.HidenUC(this.UCMergePDF);
							}
							break;
						case 'p':
							if (tag == "ppttopdf")
							{
								this.HidenUC(this.UCPPTToPDF);
							}
							break;
						case 'r':
							if (tag == "rtftopdf")
							{
								this.HidenUC(this.UCRTFToPDF);
							}
							break;
						case 's':
							if (tag == "splitpdf")
							{
								this.HidenUC(this.UCSplitPDF);
							}
							break;
						case 't':
							if (tag == "txttopdf")
							{
								this.HidenUC(this.UCTXTToPDF);
							}
							break;
						}
						break;
					case 9:
					{
						char c = tag[0];
						if (c != 'h')
						{
							if (c == 'w')
							{
								if (tag == "wordtopdf")
								{
									this.HidenUC(this.UCWordToPDF);
								}
							}
						}
						else if (tag == "htmltopdf")
						{
							this.HidenUC(this.UCHtmlToPDF);
						}
						break;
					}
					case 10:
					{
						char c = tag[0];
						if (c != 'e')
						{
							if (c == 'i')
							{
								if (tag == "imagetopdf")
								{
									this.HidenUC(this.UCImageToPDF);
								}
							}
						}
						else if (tag == "exceltopdf")
						{
							this.HidenUC(this.UCExcelToPDF);
						}
						break;
					}
					case 11:
						if (tag == "compresspdf")
						{
							this.HidenUC(this.CompressPDF);
						}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Instance.Error<Exception>(ex);
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00008DAC File Offset: 0x00006FAC
		private void HidenUC(FrameworkElement element)
		{
			this.UCSplitPDF.Visibility = Visibility.Hidden;
			this.UCMergePDF.Visibility = Visibility.Hidden;
			this.UCWordToPDF.Visibility = Visibility.Hidden;
			this.UCExcelToPDF.Visibility = Visibility.Hidden;
			this.UCPPTToPDF.Visibility = Visibility.Hidden;
			this.UCRTFToPDF.Visibility = Visibility.Hidden;
			this.UCTXTToPDF.Visibility = Visibility.Hidden;
			this.UCHtmlToPDF.Visibility = Visibility.Hidden;
			this.UCImageToPDF.Visibility = Visibility.Hidden;
			this.CompressPDF.Visibility = Visibility.Hidden;
			element.Visibility = Visibility.Visible;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00008E38 File Offset: 0x00007038
		private void AddOneFileToMergeList(string file)
		{
			LongPathFile.Exists(file);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00008E41 File Offset: 0x00007041
		private void splitModeHelpBtn_Click(object sender, RoutedEventArgs e)
		{
			ModernMessageBox.Show(pdfconverter.Properties.Resources.WinMergeSplitSplitModeCustomRangeHelpAsgMsg + "\r\n" + pdfconverter.Properties.Resources.WinMergeSplitSplitModeFixedRangeHelpAsgMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
		}
	}
}
