using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.Controls;
using CommonLib.Controls.ColorPickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Services;
using PDFKit.Utils;
using PDFKit.Utils.PageHeaderFooters;
using PDFKit.Utils.XObjects;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x02000241 RID: 577
	public partial class PageNumberDialog : Window
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x000969DC File Offset: 0x00094BDC
		public PageNumberDialog(PdfDocument doc, HeaderFooterSettings settings, int[] selectedIndexes = null)
		{
			this.InitializeComponent();
			if (doc == null)
			{
				throw new ArgumentException("doc");
			}
			this.document = doc;
			ResultModel resultModel;
			if (settings != null)
			{
				resultModel = ResultModel.FromSettings(doc, settings);
			}
			else
			{
				resultModel = new ResultModel();
			}
			this.StyleComboBox.ItemsSource = PagePlaceholderFormatter.AllSupportedPageNumberFormats.ToList<string>();
			if (!PagePlaceholderFormatter.AllSupportedPageNumberFormats.Contains(resultModel.PageNumberFormat))
			{
				resultModel.PageNumberFormat = PagePlaceholderFormatter.AllSupportedPageNumberFormats[0];
			}
			this.StyleComboBox.SelectedItem = resultModel.PageNumberFormat;
			this.PageNumberOffsetBox.Maximum = 2147483647.0;
			this.PageNumberOffsetBox.Value = (double)resultModel.PageNumberOffset;
			this.UpdateSelectedPageRange();
			if (resultModel.PageRange == ResultModel.PageRangeEnum.None)
			{
				if (selectedIndexes != null && selectedIndexes.Length != 0)
				{
					resultModel.PageRange = ResultModel.PageRangeEnum.SelectedPages;
					resultModel.SelectedPagesStart = selectedIndexes[0] + 1;
					resultModel.SelectedPagesEnd = selectedIndexes[selectedIndexes.Length - 1] + 1;
				}
				else
				{
					resultModel.PageRange = ResultModel.PageRangeEnum.AllPages;
					resultModel.SelectedPagesStart = 1;
					resultModel.SelectedPagesEnd = this.document.Pages.Count;
				}
			}
			if (resultModel.PageRange == ResultModel.PageRangeEnum.AllPages)
			{
				this.AllPageRadioButton.IsChecked = new bool?(true);
			}
			else if (resultModel.PageRange == ResultModel.PageRangeEnum.SelectedPages)
			{
				this.SelectedPagesRadioButton.IsChecked = new bool?(true);
			}
			this.SubsetComboBox.SelectedIndex = (int)resultModel.Subset;
			string fontSizeStr = string.Format("{0:0.#}pt", resultModel.FontSize);
			ComboBoxItem comboBoxItem = this.FontSizeComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault((ComboBoxItem c) => object.Equals(fontSizeStr, c.Content));
			if (comboBoxItem != null)
			{
				comboBoxItem.IsSelected = true;
			}
			else
			{
				fontSizeStr = "10pt";
				ComboBoxItem comboBoxItem2 = this.FontSizeComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault((ComboBoxItem c) => object.Equals(fontSizeStr, c));
				if (comboBoxItem2 != null)
				{
					comboBoxItem2.IsSelected = true;
				}
			}
			this.Result = resultModel;
			this.UpdateResultText();
			this.UpdatePagePreviewAsync(0);
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x00096BCD File Offset: 0x00094DCD
		private void UpdateSelectedPageRange()
		{
			this.SelectedPageEndBox.Maximum = (double)this.document.Pages.Count;
			this.PreviewPageIndexBox.Maximum = (double)this.document.Pages.Count;
		}

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x060020E4 RID: 8420 RVA: 0x00096C07 File Offset: 0x00094E07
		// (set) Token: 0x060020E5 RID: 8421 RVA: 0x00096C19 File Offset: 0x00094E19
		public ResultModel Result
		{
			get
			{
				return (ResultModel)base.GetValue(PageNumberDialog.ResultProperty);
			}
			private set
			{
				base.SetValue(PageNumberDialog.ResultPropertyKey, value);
			}
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x00096C27 File Offset: 0x00094E27
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x00096C30 File Offset: 0x00094E30
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Keyboard.ClearFocus();
			base.IsHitTestVisible = false;
			this.UpdateResultText();
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				base.DialogResult = new bool?(true);
			}));
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00096C5D File Offset: 0x00094E5D
		private void TextColorButton_SelectedColorChanged(object sender, ColorPickerButtonSelectedColorChangedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				this.UpdatePreviewTextContent();
			}));
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x00096C78 File Offset: 0x00094E78
		private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateFontSize();
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x00096C80 File Offset: 0x00094E80
		private void HeaderFooterDialogPageRange_Checked(object sender, RoutedEventArgs e)
		{
			if (this.Result == null)
			{
				return;
			}
			if (e.Source == this.AllPageRadioButton)
			{
				this.Result.PageRange = ResultModel.PageRangeEnum.AllPages;
				return;
			}
			if (e.Source == this.SelectedPagesRadioButton)
			{
				this.Result.PageRange = ResultModel.PageRangeEnum.SelectedPages;
			}
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00096CC0 File Offset: 0x00094EC0
		private void SubsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateSubset();
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00096CC8 File Offset: 0x00094EC8
		private async void PreviewPageIndexBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int num = (int)((RangeBase)sender).Value;
			if (this.document != null && num > 0 && num <= this.document.Pages.Count)
			{
				await this.UpdatePagePreviewAsync(num - 1);
			}
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00096D07 File Offset: 0x00094F07
		private void StyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.Result != null)
			{
				this.Result.PageNumberFormat = (string)e.AddedItems[0];
			}
			this.UpdateResultText();
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x00096D39 File Offset: 0x00094F39
		private void PageNumberOffsetBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.Result != null)
			{
				this.Result.PageNumberOffset = (int)(e.NewValue + 0.5);
			}
			this.UpdateResultText();
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x00096D6B File Offset: 0x00094F6B
		private void PositionRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			this.UpdateResultText();
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00096D79 File Offset: 0x00094F79
		private void AlignmentRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			this.UpdateResultText();
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x00096D88 File Offset: 0x00094F88
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			FrameworkElement frameworkElement = Keyboard.FocusedElement as FrameworkElement;
			if (frameworkElement != null)
			{
				TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
				frameworkElement.MoveFocus(traversalRequest);
			}
			Keyboard.ClearFocus();
			this.UpdateResultText();
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x00096DCC File Offset: 0x00094FCC
		private void UpdateResultText()
		{
			if (this.Result == null)
			{
				return;
			}
			this.Result.Text.LeftHeaderText = "";
			this.Result.Text.CenterHeaderText = "";
			this.Result.Text.RightHeaderText = "";
			this.Result.Text.LeftFooterText = "";
			this.Result.Text.CenterFooterText = "";
			this.Result.Text.RightFooterText = "";
			string pageNumberFormat = this.Result.PageNumberFormat;
			string text = "<<" + pageNumberFormat + ">>";
			if (this.PositionHeaderRadioButton.IsChecked.GetValueOrDefault())
			{
				if (this.AlignmentLeftRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.LeftHeaderText = text;
				}
				if (this.AlignmentCenterRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.CenterHeaderText = text;
				}
				if (this.AlignmentRightRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.RightHeaderText = text;
					return;
				}
			}
			else
			{
				if (this.AlignmentLeftRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.LeftFooterText = text;
				}
				if (this.AlignmentCenterRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.CenterFooterText = text;
				}
				if (this.AlignmentRightRadioButton.IsChecked.GetValueOrDefault())
				{
					this.Result.Text.RightFooterText = text;
				}
			}
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x00096F7C File Offset: 0x0009517C
		private void UpdateSubset()
		{
			if (this.Result == null)
			{
				return;
			}
			switch (this.SubsetComboBox.SelectedIndex)
			{
			case 0:
				this.Result.Subset = ResultModel.SubsetEnum.AllPages;
				return;
			case 1:
				this.Result.Subset = ResultModel.SubsetEnum.Odd;
				return;
			case 2:
				this.Result.Subset = ResultModel.SubsetEnum.Even;
				return;
			default:
				this.Result.Subset = ResultModel.SubsetEnum.AllPages;
				return;
			}
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x00096FE8 File Offset: 0x000951E8
		private void UpdateFontSize()
		{
			if (this.Result == null)
			{
				return;
			}
			string text = string.Empty;
			string text2 = this.FontSizeComboBox.SelectedItem as string;
			if (text2 != null)
			{
				text = text2;
			}
			else
			{
				ComboBoxItem comboBoxItem = this.FontSizeComboBox.SelectedItem as ComboBoxItem;
				if (comboBoxItem != null)
				{
					string text3 = comboBoxItem.Content as string;
					if (text3 != null)
					{
						text = text3;
					}
				}
			}
			float num = 14f;
			if (!string.IsNullOrEmpty(text))
			{
				if (text.Length >= 2 && text[text.Length - 2] == 'p' && text[text.Length - 1] == 't')
				{
					text = text.Substring(0, text.Length - 2);
				}
				if (!float.TryParse(text, out num))
				{
					num = 14f;
				}
			}
			this.Result.FontSize = num;
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x000970B4 File Offset: 0x000952B4
		private async Task UpdatePagePreviewAsync(int pageIndex)
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			CancellationTokenSource cancellationTokenSource2 = this.cts;
			if (cancellationTokenSource2 != null)
			{
				cancellationTokenSource2.Dispose();
			}
			this.cts = new CancellationTokenSource();
			PdfThumbnailService service = Ioc.Default.GetService<PdfThumbnailService>();
			try
			{
				WriteableBitmap writeableBitmap = await service.TryGetPdfBitmapAsync(this.document.Pages[pageIndex], Colors.White, this.document.Pages[pageIndex].Rotation, 600, 0, this.cts.Token);
				ImageBrush imageBrush = new ImageBrush(writeableBitmap);
				imageBrush.AlignmentY = AlignmentY.Top;
				imageBrush.AlignmentX = AlignmentX.Left;
				imageBrush.Stretch = Stretch.UniformToFill;
				this.Tore1.ContentBrush = imageBrush;
				ImageBrush imageBrush2 = new ImageBrush(writeableBitmap);
				imageBrush2.AlignmentY = AlignmentY.Bottom;
				imageBrush2.AlignmentX = AlignmentX.Left;
				imageBrush2.Stretch = Stretch.UniformToFill;
				this.Tore2.ContentBrush = imageBrush2;
				FS_SIZEF effectiveSize = this.document.Pages[pageIndex].GetEffectiveSize(PageRotate.Normal, false);
				this.MarginControl1.PageOriginalWidth = (double)effectiveSize.Width;
				this.MarginControl2.PageOriginalWidth = (double)effectiveSize.Width;
				this.UpdatePreviewTextContentCore(this.document.Pages[pageIndex]);
			}
			catch (OperationCanceledException)
			{
			}
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x00097100 File Offset: 0x00095300
		private void UpdatePreviewTextContentCore(PdfPage page)
		{
			if (this.Result == null)
			{
				return;
			}
			HeaderFooterSettings headerFooterSettings = this.Result.ToSettings(this.document, page.PageIndex);
			int pageIndex = page.PageIndex;
			int count = page.Document.Pages.Count;
			this.MarginControl1.LeftString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.HeaderLeft, DateTimeOffset.Now);
			this.MarginControl1.CenterString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.HeaderCenter, DateTimeOffset.Now);
			this.MarginControl1.RightString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.HeaderRight, DateTimeOffset.Now);
			this.MarginControl2.LeftString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.FooterLeft, DateTimeOffset.Now);
			this.MarginControl2.CenterString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.FooterCenter, DateTimeOffset.Now);
			this.MarginControl2.RightString = PageHeaderFooterUtils.GetContent(page, pageIndex, count, headerFooterSettings, PageHeaderFooterUtils.LocationEnum.FooterRight, DateTimeOffset.Now);
			this.MarginControl1.PreviewFontSize = headerFooterSettings.Font.Size;
			this.MarginControl1.FontFamily = new FontFamily("Arial, Microsoft Yahei");
			this.MarginControl1.Foreground = new SolidColorBrush(this.Result.Color);
			this.MarginControl2.PreviewFontSize = headerFooterSettings.Font.Size;
			this.MarginControl2.FontFamily = new FontFamily("Arial, Microsoft Yahei");
			this.MarginControl2.Foreground = new SolidColorBrush(this.Result.Color);
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x00097270 File Offset: 0x00095470
		private void UpdatePreviewTextContent()
		{
			if (this.PreviewPageIndexBox == null)
			{
				return;
			}
			int num = (int)this.PreviewPageIndexBox.Value;
			if (this.document != null && num > 0 && num <= this.document.Pages.Count)
			{
				this.UpdatePreviewTextContentCore(this.document.Pages[num - 1]);
			}
		}

		// Token: 0x04000D59 RID: 3417
		private PdfDocument document;

		// Token: 0x04000D5A RID: 3418
		public static readonly DependencyProperty ResultProperty = PageNumberDialog.ResultPropertyKey.DependencyProperty;

		// Token: 0x04000D5B RID: 3419
		private static readonly DependencyPropertyKey ResultPropertyKey = DependencyProperty.RegisterReadOnly("Texts", typeof(ResultModel), typeof(PageNumberDialog), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PageNumberDialog pageNumberDialog = s as PageNumberDialog;
			if (pageNumberDialog != null && a.NewValue != a.OldValue)
			{
				pageNumberDialog.UpdateSubset();
				pageNumberDialog.UpdateFontSize();
			}
		}));

		// Token: 0x04000D5C RID: 3420
		private CancellationTokenSource cts;
	}
}
