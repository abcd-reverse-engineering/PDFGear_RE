using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Controls;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024B RID: 587
	public partial class InsertPageFromImage : Window
	{
		// Token: 0x060021B7 RID: 8631 RVA: 0x0009B45C File Offset: 0x0009965C
		public InsertPageFromImage(string sourceFile, PdfDocument doc, IEnumerable<int> selectedPages, bool fromSinglePageCmd = false)
		{
			this.InitializeComponent();
			this.doc = doc;
			List<int> list = selectedPages.ToList<int>();
			list.Sort();
			if (list.Count > 0)
			{
				this.selectedFirstIndex = list[0];
				this.PageindexNumbox.Text = selectedPages.ConvertToRange();
			}
			else
			{
				this.selectedFirstIndex = 0;
				this.PageindexNumbox.Text = "";
			}
			if (doc != null)
			{
				this.PageNumber.Text = doc.Pages.Count.ToString();
			}
			this.InitInsertPositionRadioButtons(selectedPages, fromSinglePageCmd);
			if (!string.IsNullOrEmpty(sourceFile))
			{
				try
				{
					FileInfo fileInfo = new FileInfo(sourceFile);
					this.LocationTextBox.Text = fileInfo.FullName;
					this.sourceFile = sourceFile;
				}
				catch
				{
				}
			}
			this.LocationTextBox.TextChanged += this.LocationTextBox_TextChanged;
			this.InitControls();
		}

		// Token: 0x060021B8 RID: 8632 RVA: 0x0009B5AC File Offset: 0x000997AC
		private void LocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.sourceFile = this.LocationTextBox.Text;
			if (!string.IsNullOrEmpty(this.sourceFile))
			{
				try
				{
					FileInfo fileInfo = new FileInfo(this.sourceFile);
					this.LocationTextBox.Text = fileInfo.FullName;
				}
				catch
				{
				}
			}
		}

		// Token: 0x060021B9 RID: 8633 RVA: 0x0009B60C File Offset: 0x0009980C
		private void InitInsertPositionRadioButtons(IEnumerable<int> selectedPages, bool fromSinglePageCmd)
		{
			if (fromSinglePageCmd)
			{
				this.InitInsertPositionRadioButtonsFromSinglePage(selectedPages);
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x0009B620 File Offset: 0x00099820
		private void InitInsertPositionRadioButtonsFromSinglePage(IEnumerable<int> selectedPages)
		{
			int count = this.doc.Pages.Count;
			int[] array = ((selectedPages != null) ? selectedPages.ToArray<int>() : null);
			if (array != null && array.Length == 1 && array[0] >= 0 && array[0] < count)
			{
				this.firstSelectedPage = array[0];
				this.lastSelectedPage = array[0];
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.PageNumber.Text = count.ToString();
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x0009B6AC File Offset: 0x000998AC
		private void InitInsertPositionRadioButtonsFromMultiPages(IEnumerable<int> selectedPages)
		{
			int count = this.doc.Pages.Count;
			int[] array = ((selectedPages != null) ? selectedPages.ToArray<int>() : null);
			if (array != null)
			{
				int num = int.MaxValue;
				int num2 = int.MinValue;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] >= 0 && array[i] < count)
					{
						num = Math.Min(array[i], num);
						num2 = Math.Max(array[i], num2);
					}
				}
				if (num != 2147483647 && num >= 0)
				{
					this.firstSelectedPage = num;
				}
				if (num2 != -2147483648 && num2 <= count - 1)
				{
					this.lastSelectedPage = num2;
				}
			}
			if (this.firstSelectedPage != -1)
			{
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.InsertPosition.SelectedIndex = 1;
				this.PageNumber.Text = count.ToString();
			}
			if (this.lastSelectedPage != -1)
			{
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.InsertPosition.SelectedIndex = 0;
				this.PageNumber.Text = count.ToString();
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x0009B7CC File Offset: 0x000999CC
		private void InitControls()
		{
			try
			{
				if (this.selectedFirstIndex != -1)
				{
					FS_SIZEF pageSizeByIndex = this.doc.GetPageSizeByIndex(this.selectedFirstIndex);
					this.DefaultSize.Width = (float)InsertPageFromImage.GetMMValueFromPix(pageSizeByIndex.Width);
					this.DefaultSize.Height = (float)InsertPageFromImage.GetMMValueFromPix(pageSizeByIndex.Height);
				}
				string text = string.Format("{0} ({1}mm x {2}mm)", pdfeditor.Properties.Resources.WinPageInsertSettingPageSizeItemDefault, this.DefaultSize.Width, this.DefaultSize.Height);
				this.dicItem.Add(text, this.A4SizeF);
				this.dicItem.Add("A4 (210mm x 297mm)", this.A4SizeF);
				this.dicItem.Add("A3 (297mm x 420mm)", this.A3SizeF);
				this.dicItem.Add(pdfeditor.Properties.Resources.WinPageInsertSettingPageSizeItemCustomize, this.DefaultSize);
				this.cbPageSize.ItemsSource = this.dicItem;
				this.cbPageSize.SelectedValuePath = "Value";
				this.cbPageSize.SelectedIndex = 0;
				this.tboxPageWidth.Value = (double)this.DefaultSize.Width;
				this.tboxPageHeight.Value = (double)this.DefaultSize.Height;
			}
			catch
			{
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x0009B924 File Offset: 0x00099B24
		private void cbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.cbPageSize.SelectedIndex == 3)
				{
					this.tbPageHeightUnit.Visibility = Visibility.Visible;
					this.tbPageHeight.Visibility = Visibility.Visible;
					this.tboxPageHeight.Visibility = Visibility.Visible;
					this.tbPageWidthUnit.Visibility = Visibility.Visible;
					this.tbPageWidth.Visibility = Visibility.Visible;
					this.tboxPageWidth.Visibility = Visibility.Visible;
					this.PortraitRadioButton.Visibility = Visibility.Collapsed;
					this.LandscapeRadioButton.Visibility = Visibility.Collapsed;
				}
				else
				{
					this.tbPageHeightUnit.Visibility = Visibility.Collapsed;
					this.tbPageHeight.Visibility = Visibility.Collapsed;
					this.tboxPageHeight.Visibility = Visibility.Collapsed;
					this.tbPageWidthUnit.Visibility = Visibility.Collapsed;
					this.tbPageWidth.Visibility = Visibility.Collapsed;
					this.tboxPageWidth.Visibility = Visibility.Collapsed;
					this.PortraitRadioButton.Visibility = Visibility.Visible;
					this.LandscapeRadioButton.Visibility = Visibility.Visible;
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x060021BE RID: 8638 RVA: 0x0009BA18 File Offset: 0x00099C18
		// (set) Token: 0x060021BF RID: 8639 RVA: 0x0009BA20 File Offset: 0x00099C20
		public int InsertPageIndex { get; private set; }

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x060021C0 RID: 8640 RVA: 0x0009BA29 File Offset: 0x00099C29
		// (set) Token: 0x060021C1 RID: 8641 RVA: 0x0009BA31 File Offset: 0x00099C31
		public bool InsertBefore { get; private set; }

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x060021C2 RID: 8642 RVA: 0x0009BA3A File Offset: 0x00099C3A
		// (set) Token: 0x060021C3 RID: 8643 RVA: 0x0009BA42 File Offset: 0x00099C42
		public SizeF PageSize { get; private set; }

		// Token: 0x060021C4 RID: 8644 RVA: 0x0009BA4B File Offset: 0x00099C4B
		private static float GetPixValueFromMM(float mm)
		{
			return (float)((double)(mm * 72f) / 25.4);
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x0009BA60 File Offset: 0x00099C60
		private static int GetMMValueFromPix(float pix)
		{
			return (int)Math.Round((double)pix * 25.4 / 72.0, 0);
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x0009BA80 File Offset: 0x00099C80
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!IAPUtils.IsPaidUserWrapper())
				{
					IAPUtils.ShowPurchaseWindows("insertimage", ".pdf");
				}
				else
				{
					float num;
					float num2;
					if (this.cbPageSize.SelectedIndex == 0)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertPageFromImage.GetPixValueFromMM(this.DefaultSize.Height);
							num2 = InsertPageFromImage.GetPixValueFromMM(this.DefaultSize.Width);
						}
						else
						{
							num = InsertPageFromImage.GetPixValueFromMM(this.DefaultSize.Width);
							num2 = InsertPageFromImage.GetPixValueFromMM(this.DefaultSize.Height);
						}
					}
					else if (this.cbPageSize.SelectedIndex == 1)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A4SizeF.Height));
							num2 = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A4SizeF.Width));
						}
						else
						{
							num = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A4SizeF.Width));
							num2 = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A4SizeF.Height));
						}
					}
					else if (this.cbPageSize.SelectedIndex == 2)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A3SizeF.Height));
							num2 = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A3SizeF.Width));
						}
						else
						{
							num = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A3SizeF.Width));
							num2 = InsertPageFromImage.GetPixValueFromMM((float)((int)this.A3SizeF.Height));
						}
					}
					else
					{
						float num3 = (float)this.tboxPageWidth.Value;
						float num4 = (float)this.tboxPageHeight.Value;
						num = InsertPageFromImage.GetPixValueFromMM(num3);
						num2 = InsertPageFromImage.GetPixValueFromMM(num4);
					}
					this.PageSize = new SizeF(num, num2);
					if (this.BeginingRadioButton.IsChecked.GetValueOrDefault())
					{
						this.InsertPageIndex = 0;
						this.InsertBefore = true;
					}
					else if (this.EndRadioButton.IsChecked.GetValueOrDefault())
					{
						this.InsertPageIndex = this.doc.Pages.Count - 1;
						this.InsertBefore = false;
					}
					else if (this.PageRadioButton.IsChecked.GetValueOrDefault())
					{
						if (this.InsertPosition.SelectedIndex == 0)
						{
							this.InsertBefore = false;
						}
						else
						{
							this.InsertBefore = true;
						}
						this.InsertPageIndex = this.GetInsertPageIndex(this.InsertBefore);
						if (this.InsertPageIndex == -1)
						{
							ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
					}
					base.DialogResult = new bool?(true);
				}
			}
			catch
			{
				base.DialogResult = new bool?(false);
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x0009BD68 File Offset: 0x00099F68
		private int GetInsertPageIndex(bool InsertBefore)
		{
			int count = this.doc.Pages.Count;
			int[] importPageRange = this.GetImportPageRange();
			if (importPageRange == null)
			{
				return -1;
			}
			int num = int.MaxValue;
			int num2 = int.MinValue;
			for (int i = 0; i < importPageRange.Length; i++)
			{
				if (importPageRange[i] >= 0 && importPageRange[i] < count)
				{
					num = Math.Min(importPageRange[i], num);
					num2 = Math.Max(importPageRange[i], num2);
				}
			}
			if (!InsertBefore)
			{
				if (num2 != -2147483648 && num2 <= count - 1)
				{
					return num2;
				}
			}
			else if (num != 2147483647 && num >= 0)
			{
				return num;
			}
			return this.InsertPageIndex;
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x0009BDFC File Offset: 0x00099FFC
		private int[] GetImportPageRange()
		{
			int[] array = null;
			if (string.IsNullOrEmpty(this.PageindexNumbox.Text))
			{
				return null;
			}
			int[] array2;
			int num;
			PdfObjectExtensions.TryParsePageRange(this.PageindexNumbox.Text, out array2, out num);
			if (array2 == null)
			{
				return null;
			}
			if (array2.Length != 0)
			{
				array = array2;
			}
			if (array.Any((int c) => c < 0 || c >= this.doc.Pages.Count))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x0009BE5C File Offset: 0x0009A05C
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x0009BE64 File Offset: 0x0009A064
		private void PageindexNumbox_TextChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.doc == null)
			{
				return;
			}
			int num = (int)(e.NewValue - 1.0);
			if (num < 0)
			{
				this.PageindexNumbox.Text = "0";
				num = 0;
			}
			if (num > this.doc.Pages.Count - 1)
			{
				this.PageindexNumbox.Text = string.Format("{0}", this.doc.Pages.Count - 1);
				num = this.doc.Pages.Count - 1;
			}
			this.InsertPageIndex = num;
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x0009BEFD File Offset: 0x0009A0FD
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x04000DEA RID: 3562
		public string sourceFile;

		// Token: 0x04000DEB RID: 3563
		private readonly PdfDocument doc;

		// Token: 0x04000DEC RID: 3564
		private int firstSelectedPage = -1;

		// Token: 0x04000DED RID: 3565
		private int lastSelectedPage = -1;

		// Token: 0x04000DEE RID: 3566
		private int selectedFirstIndex = -1;

		// Token: 0x04000DEF RID: 3567
		private SizeF DefaultSize = new SizeF(210f, 297f);

		// Token: 0x04000DF0 RID: 3568
		private SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x04000DF1 RID: 3569
		private SizeF A3SizeF = new SizeF(297f, 420f);

		// Token: 0x04000DF2 RID: 3570
		private Dictionary<string, SizeF> dicItem = new Dictionary<string, SizeF>();
	}
}
