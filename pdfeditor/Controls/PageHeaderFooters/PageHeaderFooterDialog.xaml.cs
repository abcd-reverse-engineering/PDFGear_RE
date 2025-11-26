using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
using Patagames.Pdf.Net;
using pdfeditor.Services;
using PDFKit.Utils;
using PDFKit.Utils.PageHeaderFooters;
using PDFKit.Utils.XObjects;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x0200023F RID: 575
	public partial class PageHeaderFooterDialog : Window
	{
		// Token: 0x060020AE RID: 8366 RVA: 0x000958D4 File Offset: 0x00093AD4
		public PageHeaderFooterDialog(PdfDocument doc, string fileName, HeaderFooterSettings settings, int[] selectedIndexes = null)
		{
			this.InitializeComponent();
			if (doc == null)
			{
				throw new ArgumentException("doc");
			}
			this.document = doc;
			this.fileName = fileName;
			ResultModel resultModel;
			if (settings != null)
			{
				resultModel = ResultModel.FromSettings(doc, settings);
			}
			else
			{
				resultModel = new ResultModel();
			}
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
			this.DateButtonContextMenu.ItemsSource = PagePlaceholderFormatter.AllSupportedDateFormats.ToList<string>();
			if (string.IsNullOrEmpty(fileName))
			{
				this.InsertFileNameButton.Visibility = Visibility.Collapsed;
			}
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
			this.UpdatePagePreviewAsync(0);
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x00095A85 File Offset: 0x00093C85
		public PageHeaderFooterDialog(PdfDocument doc, string fileName, int[] selectedIndexes = null)
			: this(doc, fileName, null, selectedIndexes)
		{
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x00095A91 File Offset: 0x00093C91
		private void UpdateSelectedPageRange()
		{
			this.SelectedPageEndBox.Maximum = (double)this.document.Pages.Count;
			this.PreviewPageIndexBox.Maximum = (double)this.document.Pages.Count;
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x00095ACB File Offset: 0x00093CCB
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00095AD4 File Offset: 0x00093CD4
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Keyboard.ClearFocus();
			base.IsHitTestVisible = false;
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				base.DialogResult = new bool?(true);
			}));
		}

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x060020B3 RID: 8371 RVA: 0x00095AFB File Offset: 0x00093CFB
		// (set) Token: 0x060020B4 RID: 8372 RVA: 0x00095B0D File Offset: 0x00093D0D
		public ResultModel Result
		{
			get
			{
				return (ResultModel)base.GetValue(PageHeaderFooterDialog.ResultProperty);
			}
			private set
			{
				base.SetValue(PageHeaderFooterDialog.ResultPropertyKey, value);
			}
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x00095B1B File Offset: 0x00093D1B
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

		// Token: 0x060020B6 RID: 8374 RVA: 0x00095B5B File Offset: 0x00093D5B
		private void SubsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateSubset();
		}

		// Token: 0x060020B7 RID: 8375 RVA: 0x00095B63 File Offset: 0x00093D63
		private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateFontSize();
		}

		// Token: 0x060020B8 RID: 8376 RVA: 0x00095B6C File Offset: 0x00093D6C
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

		// Token: 0x060020B9 RID: 8377 RVA: 0x00095BD8 File Offset: 0x00093DD8
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

		// Token: 0x060020BA RID: 8378 RVA: 0x00095CA1 File Offset: 0x00093EA1
		private void DateButton_Click(object sender, RoutedEventArgs e)
		{
			this.DateButtonContextMenu.PlacementTarget = this.DateButton;
			this.DateButtonContextMenu.Placement = PlacementMode.Bottom;
			this.DateButtonContextMenu.IsOpen = true;
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x00095CCC File Offset: 0x00093ECC
		private void DateButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				e.Handled = true;
			}
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x00095CE0 File Offset: 0x00093EE0
		private void DateButtonContextMenu_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = (FrameworkElement)e.OriginalSource;
			string text = ((frameworkElement != null) ? frameworkElement.DataContext : null) as string;
			if (text != null)
			{
				bool flag;
				Action<string> action;
				TextBox lastFocusedTextBox = this.GetLastFocusedTextBox(out flag, out action);
				this.Result.DateFormat = text;
				string text2 = "<<" + text + ">>";
				PageHeaderFooterDialog.InsertString(lastFocusedTextBox, text2, flag, true, action);
				this.UpdatePreviewTextContent();
			}
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x00095D44 File Offset: 0x00093F44
		private void InsertFileNameButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.fileName))
			{
				return;
			}
			string fileNameWithoutExtension = this.fileName;
			try
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.fileName);
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(fileNameWithoutExtension))
			{
				fileNameWithoutExtension = this.fileName;
			}
			bool flag;
			Action<string> action;
			PageHeaderFooterDialog.InsertString(this.GetLastFocusedTextBox(out flag, out action), fileNameWithoutExtension, flag, true, action);
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x00095DB0 File Offset: 0x00093FB0
		private void InsertPageNumberButton_Click(object sender, RoutedEventArgs e)
		{
			AddPageNumberDialog addPageNumberDialog = new AddPageNumberDialog(this.document, this.Result.PageNumberFormat, this.Result.PageNumberOffset);
			addPageNumberDialog.Owner = this;
			addPageNumberDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (addPageNumberDialog.ShowDialog().GetValueOrDefault())
			{
				this.Result.PageNumberFormat = addPageNumberDialog.PageNumberFormats;
				this.Result.PageNumberOffset = addPageNumberDialog.PageNumberOffset;
				bool flag;
				Action<string> action;
				TextBox lastFocusedTextBox = this.GetLastFocusedTextBox(out flag, out action);
				string text = "<<" + addPageNumberDialog.PageNumberFormats + ">>";
				PageHeaderFooterDialog.InsertString(lastFocusedTextBox, text, flag, true, action);
				this.UpdatePreviewTextContent();
			}
		}

		// Token: 0x060020BF RID: 8383 RVA: 0x00095E50 File Offset: 0x00094050
		private void ContentContainer_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox textBox = e.OriginalSource as TextBox;
			if (textBox != null)
			{
				this.lastLostFocusTextbox = textBox;
				this.UpdatePreviewTextContent();
			}
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x00095E7C File Offset: 0x0009407C
		private TextBox GetLastFocusedTextBox(out bool adjusted, out Action<string> setTextFunc)
		{
			adjusted = false;
			setTextFunc = null;
			TextBox leftHeaderTextBox = this.lastLostFocusTextbox;
			if (leftHeaderTextBox == this.LeftHeaderTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.LeftHeaderText = s;
				};
			}
			else if (leftHeaderTextBox == this.CenterHeaderTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.CenterHeaderText = s;
				};
			}
			else if (leftHeaderTextBox == this.RightHeaderTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.RightHeaderText = s;
				};
			}
			else if (leftHeaderTextBox == this.LeftFooterTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.LeftFooterText = s;
				};
			}
			else if (leftHeaderTextBox == this.CenterFooterTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.CenterFooterText = s;
				};
			}
			else if (leftHeaderTextBox == this.RightFooterTextBox)
			{
				setTextFunc = delegate(string s)
				{
					this.Result.Text.RightFooterText = s;
				};
			}
			else
			{
				adjusted = true;
				leftHeaderTextBox = this.LeftHeaderTextBox;
				setTextFunc = delegate(string s)
				{
					this.Result.Text.LeftHeaderText = s;
				};
			}
			return leftHeaderTextBox;
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x00095F48 File Offset: 0x00094148
		private static void InsertString(TextBox textBox, string insertText, bool adjusted, bool insertSpace, Action<string> setTextFunc)
		{
			if (textBox == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(insertText))
			{
				return;
			}
			if (setTextFunc == null)
			{
				setTextFunc = delegate(string s)
				{
					textBox.Text = s;
				};
			}
			if (!string.IsNullOrEmpty(textBox.Text))
			{
				insertText = " " + insertText;
			}
			int num = textBox.SelectionStart;
			int num2 = textBox.SelectionLength;
			if (adjusted)
			{
				num = textBox.Text.Length - 1;
				num2 = 0;
			}
			if (num == -1)
			{
				num = 0;
			}
			setTextFunc(textBox.Text.Substring(0, num) + insertText + textBox.Text.Substring(num + num2));
			textBox.SelectionLength = 0;
			textBox.SelectionStart += insertText.Length;
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x00096034 File Offset: 0x00094234
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
				FS_SIZEF effectiveSize = this.document.Pages[pageIndex].GetEffectiveSize(this.document.Pages[pageIndex].Rotation, false);
				this.MarginControl1.PageOriginalWidth = (double)effectiveSize.Width;
				this.MarginControl2.PageOriginalWidth = (double)effectiveSize.Width;
				this.UpdatePreviewTextContentCore(this.document.Pages[pageIndex]);
			}
			catch (OperationCanceledException)
			{
			}
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x00096080 File Offset: 0x00094280
		private void UpdatePreviewTextContentCore(PdfPage page)
		{
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

		// Token: 0x060020C4 RID: 8388 RVA: 0x000961E8 File Offset: 0x000943E8
		private void UpdatePreviewTextContent()
		{
			int num = (int)this.PreviewPageIndexBox.Value;
			if (this.document != null && num > 0 && num <= this.document.Pages.Count)
			{
				this.UpdatePreviewTextContentCore(this.document.Pages[num - 1]);
			}
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x0009623C File Offset: 0x0009443C
		private async void PreviewPageIndexBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int num = (int)((RangeBase)sender).Value;
			if (this.document != null && num > 0 && num <= this.document.Pages.Count)
			{
				await this.UpdatePagePreviewAsync(num - 1);
			}
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x0009627B File Offset: 0x0009447B
		private void TextColorButton_SelectedColorChanged(object sender, ColorPickerButtonSelectedColorChangedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				this.UpdatePreviewTextContent();
			}));
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x00096298 File Offset: 0x00094498
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
			this.UpdatePreviewTextContent();
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x000962D4 File Offset: 0x000944D4
		private void LeftHeaderTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x000962D8 File Offset: 0x000944D8
		private void LeftHeaderTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox.Text.Length > 0)
			{
				textBox.Text = textBox.Text.TrimStart(new char[] { '\r', '\n' });
				textBox.Text = Regex.Replace(textBox.Text, "(\\r\\n)+", "\r\n");
				textBox.SelectionStart = textBox.Text.Length;
				textBox.SelectionLength = 0;
			}
		}

		// Token: 0x04000D35 RID: 3381
		private PdfDocument document;

		// Token: 0x04000D36 RID: 3382
		private readonly string fileName;

		// Token: 0x04000D37 RID: 3383
		private TextBox lastLostFocusTextbox;

		// Token: 0x04000D38 RID: 3384
		public static readonly DependencyProperty ResultProperty = PageHeaderFooterDialog.ResultPropertyKey.DependencyProperty;

		// Token: 0x04000D39 RID: 3385
		private static readonly DependencyPropertyKey ResultPropertyKey = DependencyProperty.RegisterReadOnly("Texts", typeof(ResultModel), typeof(PageHeaderFooterDialog), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PageHeaderFooterDialog pageHeaderFooterDialog = s as PageHeaderFooterDialog;
			if (pageHeaderFooterDialog != null && a.NewValue != a.OldValue)
			{
				pageHeaderFooterDialog.UpdateSubset();
				pageHeaderFooterDialog.UpdateFontSize();
			}
		}));

		// Token: 0x04000D3A RID: 3386
		private CancellationTokenSource cts;
	}
}
