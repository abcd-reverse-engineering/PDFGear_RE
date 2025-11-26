using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.OcrComponents
{
	// Token: 0x02000258 RID: 600
	public partial class ConvertToSearchablePdfDialog : Window
	{
		// Token: 0x060022B2 RID: 8882 RVA: 0x000A398C File Offset: 0x000A1B8C
		public ConvertToSearchablePdfDialog(PdfDocument document, bool saveChange, Source source)
		{
			this.InitializeComponent();
			this.document = document;
			this.saveChange = saveChange;
			this.CurrentPagesRadioButton.Content = string.Format("{0} ({1} / {2})", pdfeditor.Properties.Resources.PageWinSelectPages, document.Pages.CurrentIndex + 1, document.Pages.Count);
			if (source == Source.Thumbnail)
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				if (requiredService != null)
				{
					TextBox rangeBox = this.RangeBox;
					List<PdfThumbnailModel> selectedThumbnailList = requiredService.SelectedThumbnailList;
					string text;
					if (selectedThumbnailList == null)
					{
						text = null;
					}
					else
					{
						IEnumerable<int> enumerable = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex);
						if (enumerable == null)
						{
							text = null;
						}
						else
						{
							int[] array = enumerable.ToArray<int>();
							int[] array2;
							text = ((array != null) ? array.ConvertToRange(out array2) : null);
						}
					}
					rangeBox.Text = text;
					this.SelectedPagesRadioButton.IsChecked = new bool?(true);
				}
			}
			if (source == Source.Viewer)
			{
				this.CurrentPagesRadioButton.IsChecked = new bool?(true);
			}
			if (!saveChange)
			{
				this.PageRangeGroupPanel.Visibility = Visibility.Collapsed;
				this.AllPagesRadioButton.IsChecked = new bool?(true);
			}
			this.Languages.ItemsSource = (from c in OcrUtils.GetAllOcrLanguages()
				select new ConvertToSearchablePdfDialog.LanguageModel(c)).ToList<ConvertToSearchablePdfDialog.LanguageModel>();
			this.UpdateSelectedLanguages();
			GAManager.SendEvent("OCR", "Show", source.ToString(), 1L);
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x000A3AFA File Offset: 0x000A1CFA
		private void LanguagesItem_Click(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.InvokeAsync(new Action(this.UpdateSelectedLanguages), DispatcherPriority.Send);
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x000A3B16 File Offset: 0x000A1D16
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x000A3B18 File Offset: 0x000A1D18
		private async void btnOk_Click(object sender, RoutedEventArgs e)
		{
			string text = "All";
			bool? flag = this.CurrentPagesRadioButton.IsChecked;
			if (flag != null && flag.GetValueOrDefault())
			{
				text = "Current";
			}
			else
			{
				flag = this.SelectedPagesRadioButton.IsChecked;
				if (flag != null && flag.GetValueOrDefault())
				{
					text = "Selected";
				}
			}
			GAManager.SendEvent("OCR", "OCRBtn", text, 1L);
			if (((List<ConvertToSearchablePdfDialog.LanguageModel>)this.Languages.ItemsSource).Where((ConvertToSearchablePdfDialog.LanguageModel c) => c.IsChecked).ToList<ConvertToSearchablePdfDialog.LanguageModel>().Count < 1)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.OcrDialogLanguageSelectedNone, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				base.IsEnabled = false;
				try
				{
					global::System.Collections.Generic.IReadOnlyList<int> pages = null;
					flag = this.CurrentPagesRadioButton.IsChecked;
					if (flag != null && flag.GetValueOrDefault())
					{
						pages = new int[] { this.document.Pages.CurrentIndex };
					}
					else
					{
						flag = this.SelectedPagesRadioButton.IsChecked;
						if (flag != null && flag.GetValueOrDefault())
						{
							if (this.RangeBox.HasError || this.RangeBox.PageIndexes.Any((int c) => c < 0 || c >= this.document.Pages.Count))
							{
								ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
								return;
							}
							pages = this.RangeBox.PageIndexes.ToArray<int>();
						}
					}
					Path.GetFileName(((MainViewModel)global::PDFKit.PdfControl.GetPdfControl(this.document).DataContext).DocumentWrapper.DocumentPath);
					string text2 = await this.SaveDocumentAsync(this.saveChange);
					if (!string.IsNullOrEmpty(text2))
					{
						GAManager.SendEvent("OCR", "PerformOCR", "Begin", 1L);
						CultureInfo[] array = (from c in (List<ConvertToSearchablePdfDialog.LanguageModel>)this.Languages.ItemsSource
							where c.IsChecked
							select c.CultureInfo).ToArray<CultureInfo>();
						ConvertToSearchablePdfDialog.ProcessDocumentResult processDocumentResult = this.ProcessDocument(text2, pages, array);
						if (processDocumentResult.Success)
						{
							new FileInfo(text2).Attributes &= ~FileAttributes.Hidden;
							GAManager.SendEvent("OCR", "PerformOCR", "Success", 1L);
							base.DialogResult = new bool?(true);
						}
						else if (processDocumentResult.Exception is OperationCanceledException)
						{
							GAManager.SendEvent("OCR", "PerformOCR", "Cancel", 1L);
						}
						else
						{
							GAManager.SendEvent("OCR", "PerformOCR", "Fail", 1L);
							ModernMessageBox.Show(pdfeditor.Properties.Resources.OcrDialogErrorMessage, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						pages = null;
					}
				}
				finally
				{
					base.IsEnabled = true;
				}
			}
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x000A3B50 File Offset: 0x000A1D50
		private void UpdateSelectedLanguages()
		{
			List<ConvertToSearchablePdfDialog.LanguageModel> list = ((List<ConvertToSearchablePdfDialog.LanguageModel>)this.Languages.ItemsSource).Where((ConvertToSearchablePdfDialog.LanguageModel c) => c.IsChecked).ToList<ConvertToSearchablePdfDialog.LanguageModel>();
			if (list.Count > 0)
			{
				this.SelectedLanguagesText.Visibility = Visibility.Visible;
				StringBuilder stringBuilder = list.Aggregate(new StringBuilder(), (StringBuilder s, ConvertToSearchablePdfDialog.LanguageModel c) => s.Append(c.DisplayName).Append(',').Append(' '));
				stringBuilder.Length -= 2;
				this.SelectedLanguagesRun.Text = stringBuilder.ToString();
				if (list.Count > 3)
				{
					this.SelectedTooManyLanguagesTips.Foreground = new SolidColorBrush(Color.FromRgb(220, 20, 60));
				}
				else
				{
					this.SelectedTooManyLanguagesTips.Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85));
				}
				ConvertToSearchablePdfDialog.LanguageModel languageModel = list[list.Count - 1];
				FrameworkElement frameworkElement = this.Languages.ItemContainerGenerator.ContainerFromItem(languageModel) as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.BringIntoView();
					return;
				}
			}
			else
			{
				this.SelectedTooManyLanguagesTips.Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85));
				this.SelectedLanguagesRun.Text = "";
			}
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x000A3C98 File Offset: 0x000A1E98
		private async Task<string> SaveDocumentAsync(bool saveChange)
		{
			MainViewModel.SaveResult saveResult = await ((MainViewModel)global::PDFKit.PdfControl.GetPdfControl(this.document).DataContext).SaveAsync(new MainViewModel.SaveOptions
			{
				ForceSaveAs = true,
				AllowSaveToCurrentFile = false,
				InitialFileNamePostfixOverride = "OCR",
				ShowProgress = true,
				ProgressDelayTime = TimeSpan.FromSeconds(0.0),
				DocumentModified = saveChange,
				ValidCanSaveBeforeActionInvoke = true,
				CreateNewFileAttributes = FileAttributes.Hidden,
				RemoveExistsDigitalSignaturesWhenSaveAs = true
			});
			return (saveResult.FailedResult == MainViewModel.SaveFailedResult.Successed) ? saveResult.File.FullName : string.Empty;
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x000A3CE4 File Offset: 0x000A1EE4
		private ConvertToSearchablePdfDialog.ProcessDocumentResult ProcessDocument(string filePath, global::System.Collections.Generic.IReadOnlyList<int> pages, CultureInfo[] cultureInfoArray)
		{
			ConvertToSearchablePdfDialog.<>c__DisplayClass8_0 CS$<>8__locals1 = new ConvertToSearchablePdfDialog.<>c__DisplayClass8_0();
			CS$<>8__locals1.filePath = filePath;
			CS$<>8__locals1.pages = pages;
			CS$<>8__locals1.cultureInfoArray = cultureInfoArray;
			CS$<>8__locals1.result = false;
			CS$<>8__locals1.resultEx = null;
			CS$<>8__locals1.content = new TextBlock
			{
				Margin = new Thickness(0.0, 8.0, 0.0, 8.0)
			};
			bool flag = ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction action)
			{
				ConvertToSearchablePdfDialog.<>c__DisplayClass8_0.<<ProcessDocument>b__0>d <<ProcessDocument>b__0>d;
				<<ProcessDocument>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ProcessDocument>b__0>d.<>4__this = CS$<>8__locals1;
				<<ProcessDocument>b__0>d.action = action;
				<<ProcessDocument>b__0>d.<>1__state = -1;
				<<ProcessDocument>b__0>d.<>t__builder.Start<ConvertToSearchablePdfDialog.<>c__DisplayClass8_0.<<ProcessDocument>b__0>d>(ref <<ProcessDocument>b__0>d);
				return <<ProcessDocument>b__0>d.<>t__builder.Task;
			}, pdfeditor.Properties.Resources.CerateSearchablePDFWinTitle, CS$<>8__locals1.content, true, this, 0);
			AggregateException ex = CS$<>8__locals1.resultEx as AggregateException;
			if (ex != null)
			{
				CS$<>8__locals1.resultEx = ex.InnerException;
			}
			else if (CS$<>8__locals1.resultEx == null && !flag)
			{
				CS$<>8__locals1.resultEx = new TaskCanceledException();
			}
			return new ConvertToSearchablePdfDialog.ProcessDocumentResult(CS$<>8__locals1.result, CS$<>8__locals1.resultEx);
		}

		// Token: 0x04000EC1 RID: 3777
		private readonly PdfDocument document;

		// Token: 0x04000EC2 RID: 3778
		private readonly bool saveChange;

		// Token: 0x02000714 RID: 1812
		private class ProcessDocumentResult
		{
			// Token: 0x060035B1 RID: 13745 RVA: 0x0010EFDA File Offset: 0x0010D1DA
			public ProcessDocumentResult(bool success, Exception exception)
			{
				this.Success = success;
				this.Exception = exception;
			}

			// Token: 0x17000D54 RID: 3412
			// (get) Token: 0x060035B2 RID: 13746 RVA: 0x0010EFF0 File Offset: 0x0010D1F0
			public bool Success { get; }

			// Token: 0x17000D55 RID: 3413
			// (get) Token: 0x060035B3 RID: 13747 RVA: 0x0010EFF8 File Offset: 0x0010D1F8
			public Exception Exception { get; }
		}

		// Token: 0x02000715 RID: 1813
		private class LanguageModel : ObservableObject
		{
			// Token: 0x060035B4 RID: 13748 RVA: 0x0010F000 File Offset: 0x0010D200
			internal LanguageModel(CultureInfo cultureInfo)
			{
				this.CultureInfo = cultureInfo;
			}

			// Token: 0x17000D56 RID: 3414
			// (get) Token: 0x060035B5 RID: 13749 RVA: 0x0010F00F File Offset: 0x0010D20F
			public CultureInfo CultureInfo { get; }

			// Token: 0x17000D57 RID: 3415
			// (get) Token: 0x060035B6 RID: 13750 RVA: 0x0010F017 File Offset: 0x0010D217
			public string DisplayName
			{
				get
				{
					return this.CultureInfo.NativeName;
				}
			}

			// Token: 0x17000D58 RID: 3416
			// (get) Token: 0x060035B7 RID: 13751 RVA: 0x0010F024 File Offset: 0x0010D224
			// (set) Token: 0x060035B8 RID: 13752 RVA: 0x0010F02C File Offset: 0x0010D22C
			public bool IsChecked
			{
				get
				{
					return this.isChecked;
				}
				set
				{
					base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
				}
			}

			// Token: 0x04002425 RID: 9253
			private bool isChecked;
		}
	}
}
