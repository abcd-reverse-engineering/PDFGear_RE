using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfconverter.Controls;
using pdfconverter.Models;
using pdfconverter.Models.Image;
using pdfconverter.Properties;
using pdfconverter.Utils;
using pdfconverter.ViewModels;
using PDFKit.GenerateImagePdf;

namespace pdfconverter.Views
{
	// Token: 0x02000028 RID: 40
	public partial class ImageToPDF : Window
	{
		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00007C9B File Offset: 0x00005E9B
		// (set) Token: 0x0600021A RID: 538 RVA: 0x00007CA3 File Offset: 0x00005EA3
		public int InsertPageIndex { get; private set; }

		// Token: 0x0600021B RID: 539 RVA: 0x00007CAC File Offset: 0x00005EAC
		public ImageToPDF()
		{
			this.ImageToPDFViewModel = Ioc.Default.GetRequiredService<ImageToPDFViewModel>();
			base.DataContext = this.ImageToPDFViewModel;
			this.ImageToPDFViewModel._ImagePdfGenerateSettings = new ImagePdfGenerateSettings();
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Portrait;
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = new SizeF?(ImagePdfGeneratePaperSize.A4);
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperMargin = new ImagePdfGeneratePaperMargin(LengthUnit.FromPixel(0f));
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.ImageStretch = ImagePdfGenerateImageStretch.UniformIfNeeded;
			this.InitializeComponent();
			base.Closing += this.CreatePDF_Closing;
			this.ImageStretch.IsChecked = new bool?(ConfigManager.GetImageStretchFalg());
			if (this.ImageStretch.IsChecked.GetValueOrDefault())
			{
				this.ImageToPDFViewModel._ImagePdfGenerateSettings.ImageStretch = ImagePdfGenerateImageStretch.Uniform;
			}
			GAManager.SendEvent("ImageToPDFWindow", "Show", "Count", 1L);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00007DD0 File Offset: 0x00005FD0
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None && this.ImageToPDFViewModel.SelectedItems.Count > 0)
			{
				this.ImageToPDFViewModel.DeleteCommand.Execute(null);
			}
			if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
			{
				e.Handled = true;
				this.ImageToPDFViewModel.IsCheckedAll = new bool?(true);
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00007E43 File Offset: 0x00006043
		private void CreatePDF_Closing(object sender, CancelEventArgs e)
		{
			this.ImageToPDFViewModel.cts.Cancel();
			this.ImageToPDFViewModel.Dispose();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00007E60 File Offset: 0x00006060
		private async void OKButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.ImageToPDFViewModel.Visibility == Visibility.Visible)
				{
					ModernMessageBox.Show(pdfconverter.Properties.Resources.ImageToPDFWinAddingWarning, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					GAManager.SendEvent("ImageToPDFWindow", "OkBtn", "Count", 1L);
					GAManager.SendEvent("ImageToPDFWindow", "ExportedPagesCount", this.ImageToPDFViewModel.PageCount.ToString(), 1L);
					GAManager.SendEvent("ImageToPDFWindow", "OutputInOneFile", this.ImageToPDFViewModel.OurputInOneFile.ToString(), 1L);
					if (this.ImageToPDFViewModel.SelectedCount == this.ImageToPDFViewModel.PageCount || this.ImageToPDFViewModel.SelectedCount == 0 || ModernMessageBox.Show(pdfconverter.Properties.Resources.CreatePDFSelectedCreateContent.Replace("XXX", this.ImageToPDFViewModel.SelectedCount.ToString()), UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
					{
						await this.CreatePDF();
						base.Closing -= this.CreatePDF_Closing;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00007E98 File Offset: 0x00006098
		private async Task CreatePDF()
		{
			ImageToPDF.<>c__DisplayClass17_0 CS$<>8__locals1 = new ImageToPDF.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			try
			{
				GAManager.SendEvent("ImageToPDFWindow", "CreatePDF", "Begin", 1L);
				CS$<>8__locals1.newdoc = PdfDocument.CreateNew(null);
				try
				{
					CS$<>8__locals1.list = this.GetOrderedPagesForExport();
					int count = CS$<>8__locals1.list.Count;
					CS$<>8__locals1.newdoc.Producer = "PDF gear";
					string outputPath = this.ImageToPDFViewModel.OutputPath;
					CS$<>8__locals1.outfloder = this.GetValidOutFolder(outputPath, CS$<>8__locals1.list[0].FilePath);
					CS$<>8__locals1.status = new ImageToPDFStatusModel();
					if (ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
					{
						ImageToPDF.<>c__DisplayClass17_0.<<CreatePDF>b__0>d <<CreatePDF>b__0>d;
						<<CreatePDF>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<CreatePDF>b__0>d.<>4__this = CS$<>8__locals1;
						<<CreatePDF>b__0>d.c = c;
						<<CreatePDF>b__0>d.<>1__state = -1;
						<<CreatePDF>b__0>d.<>t__builder.Start<ImageToPDF.<>c__DisplayClass17_0.<<CreatePDF>b__0>d>(ref <<CreatePDF>b__0>d);
						return <<CreatePDF>b__0>d.<>t__builder.Task;
					}, pdfconverter.Properties.Resources.WinListHeaderImageToPDFText, pdfconverter.Properties.Resources.ImageToPDFWinCreateContent, true, this, 0))
					{
						if (CS$<>8__locals1.list != null && CS$<>8__locals1.list.Count > 0)
						{
							int num = 0;
							using (List<ToimagePage>.Enumerator enumerator = CS$<>8__locals1.list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (enumerator.Current.IsSucceed.GetValueOrDefault())
									{
										num++;
									}
								}
							}
							if (num > 0 && CS$<>8__locals1.status.Count > 0)
							{
								if (this.Saveas.SelectedIndex == 0)
								{
									this.ImageToPDFViewModel.OurputInOneFile = new bool?(true);
								}
								else
								{
									this.ImageToPDFViewModel.OurputInOneFile = new bool?(false);
								}
								ImageToPDFSucceedMessageBox imageToPDFSucceedMessageBox = new ImageToPDFSucceedMessageBox(this.Saveas.SelectedIndex == 0, (CS$<>8__locals1.list.Count - num).ToString(), CS$<>8__locals1.status);
								imageToPDFSucceedMessageBox.Owner = this;
								imageToPDFSucceedMessageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
								imageToPDFSucceedMessageBox.ShowDialog();
								if (imageToPDFSucceedMessageBox.DialogResult.GetValueOrDefault())
								{
									if (imageToPDFSucceedMessageBox.OpenExplorer.IsChecked.GetValueOrDefault())
									{
										for (int i = 0; i < CS$<>8__locals1.status.Count; i++)
										{
											if (CS$<>8__locals1.status[i].Status == ToPDFItemStatus.Succ)
											{
												UtilsManager.OpenFolderInExplore(Path.GetDirectoryName(CS$<>8__locals1.status[i].FilePath));
												break;
											}
										}
									}
									Thread.Sleep(1000);
									if (imageToPDFSucceedMessageBox.OpenFile.IsChecked.GetValueOrDefault())
									{
										foreach (ToPDFFileItem toPDFFileItem in CS$<>8__locals1.status)
										{
											if (toPDFFileItem.Status == ToPDFItemStatus.Succ)
											{
												ImageToPDF.OpenPDFFile(toPDFFileItem.FilePath, null);
											}
										}
									}
								}
								bool? isChecked = this.ImageStretch.IsChecked;
								bool imageStretchFalg = ConfigManager.GetImageStretchFalg();
								if (!((isChecked.GetValueOrDefault() == imageStretchFalg) & (isChecked != null)))
								{
									ConfigManager.SetImageConvertedFalg("ImageToPDFStretchFlag", this.ImageStretch.IsChecked.Value);
								}
							}
							else
							{
								ModernMessageBox.Show(pdfconverter.Properties.Resources.CreatePDFUnexpectedError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
						}
					}
				}
				finally
				{
					if (CS$<>8__locals1.newdoc != null)
					{
						((IDisposable)CS$<>8__locals1.newdoc).Dispose();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00007EDC File Offset: 0x000060DC
		private async Task DoConvertAsync(List<ToimagePage> list, string createPath, PdfDocument newdoc, bool isSingleFile, ImageToPDFStatusModel fileItems, IProgress<double> progress)
		{
			ImageToPDF.<>c__DisplayClass18_0 CS$<>8__locals1 = new ImageToPDF.<>c__DisplayClass18_0();
			CS$<>8__locals1.list = list;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.fileItems = fileItems;
			CS$<>8__locals1.isSingleFile = isSingleFile;
			CS$<>8__locals1.createPath = createPath;
			CS$<>8__locals1.progress = progress;
			CS$<>8__locals1.newdoc = newdoc;
			if (CS$<>8__locals1.list != null && CS$<>8__locals1.list.Count > 0)
			{
				CS$<>8__locals1.pregressIndex = 1.0;
				IProgress<double> progress2 = CS$<>8__locals1.progress;
				if (progress2 != null)
				{
					progress2.Report(0.0);
				}
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						ImageToPDF.<>c__DisplayClass18_0.<<DoConvertAsync>b__0>d <<DoConvertAsync>b__0>d;
						<<DoConvertAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<DoConvertAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<DoConvertAsync>b__0>d.<>1__state = -1;
						<<DoConvertAsync>b__0>d.<>t__builder.Start<ImageToPDF.<>c__DisplayClass18_0.<<DoConvertAsync>b__0>d>(ref <<DoConvertAsync>b__0>d);
						return <<DoConvertAsync>b__0>d.<>t__builder.Task;
					})).ConfigureAwait(false);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00007F54 File Offset: 0x00006154
		private string GetValidOutFolder(string root, string firstFileName)
		{
			string text = Path.GetFileNameWithoutExtension(firstFileName).Trim();
			if (string.IsNullOrWhiteSpace(text))
			{
				text = "PDF Files";
			}
			string text2 = Path.Combine(root, text);
			int num = 1;
			while (Directory.Exists(text2))
			{
				text2 = Path.Combine(root, text + " " + num.ToString());
				num++;
			}
			return text2;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00007FB0 File Offset: 0x000061B0
		private string GetValidOutFileName(List<ToimagePage> list, string parentFolder, ToimagePage item)
		{
			if (!string.IsNullOrWhiteSpace(item.FilePath))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FilePath);
				string text = fileNameWithoutExtension;
				for (int i = 2; i < 100; i++)
				{
					string fname_full = Path.Combine(parentFolder, text + ".pdf");
					if (list.Count((ToimagePage f) => f.FilePath == fname_full) == 0)
					{
						return fname_full;
					}
					text = string.Format("{0}_{1:0}", fileNameWithoutExtension, i);
				}
			}
			return "";
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00008034 File Offset: 0x00006234
		internal static string GetRootDirectory()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00008040 File Offset: 0x00006240
		internal static string GetRootDirectoryFile(params string[] filePath)
		{
			if (filePath == null || filePath.Length == 0)
			{
				return ImageToPDF.GetRootDirectory();
			}
			List<string> list = (from c in filePath
				select c.Trim() into c
				where !string.IsNullOrEmpty(c)
				select c).ToList<string>();
			if (list.Count == 0)
			{
				return ImageToPDF.GetRootDirectory();
			}
			list.Insert(0, ImageToPDF.GetRootDirectory());
			return Path.Combine(list.ToArray());
		}

		// Token: 0x06000225 RID: 549 RVA: 0x000080D0 File Offset: 0x000062D0
		private static void OpenPDFFile(string file, string action = null)
		{
			char[] array = new char[] { '\\', '/', ' ' };
			string fullName = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(array)).FullName;
			string rootDirectoryFile = ImageToPDF.GetRootDirectoryFile(new string[] { "pdfeditor.exe" });
			string text = "\"" + file + "\"";
			if (!string.IsNullOrEmpty(action))
			{
				text = text + " -action " + action.Trim();
			}
			ProcessManager.RunProcess(rootDirectoryFile, text);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00008150 File Offset: 0x00006350
		private List<ToimagePage> GetOrderedPagesForExport()
		{
			ImageToPDFViewModel imageToPDFViewModel = base.DataContext as ImageToPDFViewModel;
			List<ToimagePage> list = imageToPDFViewModel.PageList.ToList<ToimagePage>();
			if (imageToPDFViewModel.IsExportAll || imageToPDFViewModel.PageCount == imageToPDFViewModel.SelectedCount || imageToPDFViewModel.SelectedCount == 0)
			{
				return list;
			}
			List<ToimagePage> list2 = new List<ToimagePage>();
			foreach (ToimagePage toimagePage in list)
			{
				if (imageToPDFViewModel.SelectedItems.Contains(toimagePage))
				{
					list2.Add(toimagePage);
				}
			}
			return list2;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000081F0 File Offset: 0x000063F0
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageToPDFWindow", "CancelBtn", "Count", 1L);
			base.Closing -= this.CreatePDF_Closing;
			if (this.ImageToPDFViewModel.PageList.Count > 0 && ModernMessageBox.Show(pdfconverter.Properties.Resources.ImageToPDFWinCloseWarning, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) == MessageBoxResult.Cancel)
			{
				base.Closing += this.CreatePDF_Closing;
				return;
			}
			this.ImageToPDFViewModel.cts.Cancel();
			this.ImageToPDFViewModel.Dispose();
			base.Close();
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00008282 File Offset: 0x00006482
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("ms-settings:printers");
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00008290 File Offset: 0x00006490
		private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null && this.ImageToPDFViewModel != null && this.ImageToPDFViewModel.PageList.Count >= 1)
			{
				this.ImageStretch.IsEnabled = true;
				if (comboBox.SelectedIndex == 0)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = ImagePdfGeneratePaperSize.Auto;
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Auto;
					this.ImageStretch.IsChecked = new bool?(false);
					this.ImageStretch.IsEnabled = false;
				}
				else if (comboBox.SelectedIndex == 1)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = new SizeF?(ImagePdfGeneratePaperSize.A4);
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Portrait;
				}
				else if (comboBox.SelectedIndex == 2)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = new SizeF?(ImagePdfGeneratePaperSize.A4);
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Landscape;
				}
				else if (comboBox.SelectedIndex == 3)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = new SizeF?(ImagePdfGeneratePaperSize.A3);
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Portrait;
				}
				else if (comboBox.SelectedIndex == 4)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperSize = new SizeF?(ImagePdfGeneratePaperSize.A3);
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperOrientation = ImagePdfGeneratePaperOrientation.Landscape;
				}
				ProgressUtils.ShowProgressBar(async delegate(ProgressUtils.ProgressAction c)
				{
					Progress<double> progress = new Progress<double>();
					progress.ProgressChanged += delegate(object s, double a)
					{
						c.Report(a);
					};
					await this.UpdateThumAsync(progress);
					c.Complete();
				}, pdfconverter.Properties.Resources.WinListHeaderImageToPDFText, pdfconverter.Properties.Resources.ImageToPDFWinUpdateContent, false, this, 0);
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x000082D0 File Offset: 0x000064D0
		private async Task UpdateThumAsync(IProgress<double> progress)
		{
			ImageToPDF.<>c__DisplayClass28_0 CS$<>8__locals1 = new ImageToPDF.<>c__DisplayClass28_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.progress = progress;
			IProgress<double> progress2 = CS$<>8__locals1.progress;
			if (progress2 != null)
			{
				progress2.Report(0.0);
			}
			CS$<>8__locals1.index = 1.0;
			await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				ImageToPDF.<>c__DisplayClass28_0.<<UpdateThumAsync>b__0>d <<UpdateThumAsync>b__0>d;
				<<UpdateThumAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<UpdateThumAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<UpdateThumAsync>b__0>d.<>1__state = -1;
				<<UpdateThumAsync>b__0>d.<>t__builder.Start<ImageToPDF.<>c__DisplayClass28_0.<<UpdateThumAsync>b__0>d>(ref <<UpdateThumAsync>b__0>d);
				return <<UpdateThumAsync>b__0>d.<>t__builder.Task;
			})).ConfigureAwait(false);
			IProgress<double> progress3 = CS$<>8__locals1.progress;
			if (progress3 != null)
			{
				progress3.Report(1.0);
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000831C File Offset: 0x0000651C
		private async void MarginBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null && this.ImageToPDFViewModel != null && this.ImageToPDFViewModel.PageList.Count >= 1)
			{
				if (comboBox.SelectedIndex == 0)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperMargin = new ImagePdfGeneratePaperMargin(LengthUnit.FromCentimeter(0f));
				}
				else if (comboBox.SelectedIndex == 1)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperMargin = new ImagePdfGeneratePaperMargin(LengthUnit.FromCentimeter(1.27f));
				}
				else if (comboBox.SelectedIndex == 2)
				{
					this.ImageToPDFViewModel._ImagePdfGenerateSettings.PaperMargin = new ImagePdfGeneratePaperMargin(LengthUnit.FromCentimeter(3.18f), LengthUnit.FromCentimeter(2.54f));
				}
				ProgressUtils.ShowProgressBar(async delegate(ProgressUtils.ProgressAction c)
				{
					Progress<double> progress = new Progress<double>();
					progress.ProgressChanged += delegate(object s, double a)
					{
						c.Report(a);
					};
					await this.UpdateThumAsync(progress);
					c.Complete();
				}, pdfconverter.Properties.Resources.WinListHeaderImageToPDFText, pdfconverter.Properties.Resources.ImageToPDFWinUpdateContent, false, this, 0);
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000835C File Offset: 0x0000655C
		private void ImageStretch_Checked(object sender, RoutedEventArgs e)
		{
			if (this.ImageToPDFViewModel == null || this.ImageToPDFViewModel.PageList.Count < 1)
			{
				return;
			}
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.ImageStretch = ImagePdfGenerateImageStretch.Uniform;
			ProgressUtils.ShowProgressBar(async delegate(ProgressUtils.ProgressAction c)
			{
				Progress<double> progress = new Progress<double>();
				progress.ProgressChanged += delegate(object s, double a)
				{
					c.Report(a);
				};
				await this.UpdateThumAsync(progress);
				c.Complete();
			}, pdfconverter.Properties.Resources.WinListHeaderImageToPDFText, pdfconverter.Properties.Resources.ImageToPDFWinUpdateContent, false, this, 0);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x000083B8 File Offset: 0x000065B8
		private void ImageStretch_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.ImageToPDFViewModel == null || this.ImageToPDFViewModel.PageList.Count < 1)
			{
				return;
			}
			this.ImageToPDFViewModel._ImagePdfGenerateSettings.ImageStretch = ImagePdfGenerateImageStretch.UniformIfNeeded;
			ProgressUtils.ShowProgressBar(async delegate(ProgressUtils.ProgressAction c)
			{
				Progress<double> progress = new Progress<double>();
				progress.ProgressChanged += delegate(object s, double a)
				{
					c.Report(a);
				};
				await this.UpdateThumAsync(progress);
				c.Complete();
			}, pdfconverter.Properties.Resources.WinListHeaderImageToPDFText, pdfconverter.Properties.Resources.ImageToPDFWinUpdateContent, false, this, 0);
		}

		// Token: 0x04000138 RID: 312
		public ImageToPDFViewModel ImageToPDFViewModel;

		// Token: 0x04000139 RID: 313
		private readonly PdfDocument doc;

		// Token: 0x0400013A RID: 314
		private int firstSelectedPage = -1;

		// Token: 0x0400013B RID: 315
		private int lastSelectedPage = -1;

		// Token: 0x0400013C RID: 316
		private int selectedFirstIndex = -1;

		// Token: 0x0400013D RID: 317
		private Dictionary<string, SizeF> dicItem = new Dictionary<string, SizeF>();

		// Token: 0x0400013E RID: 318
		private bool CreateNew;

		// Token: 0x0400013F RID: 319
		private int marginWidth;

		// Token: 0x04000140 RID: 320
		private int marginHeight;
	}
}
