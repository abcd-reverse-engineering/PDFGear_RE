using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Controls;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024A RID: 586
	public partial class InsertBlankPageDialog : Window
	{
		// Token: 0x0600219D RID: 8605 RVA: 0x0009A8EC File Offset: 0x00098AEC
		public InsertBlankPageDialog(IEnumerable<int> selectedPages, PdfDocument doc, bool fromSinglePageCmd = false)
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
			this.InitControls();
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x0009A9E8 File Offset: 0x00098BE8
		private void InitControls()
		{
			try
			{
				if (this.selectedFirstIndex != -1)
				{
					PdfPage pdfPage = this.doc.Pages[this.selectedFirstIndex];
					FS_SIZEF pageSizeByIndex = this.doc.GetPageSizeByIndex(this.selectedFirstIndex);
					this.DefaultSize.Width = (float)InsertBlankPageDialog.GetMMValueFromPix(pageSizeByIndex.Width);
					this.DefaultSize.Height = (float)InsertBlankPageDialog.GetMMValueFromPix(pageSizeByIndex.Height);
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

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x0600219F RID: 8607 RVA: 0x0009AB58 File Offset: 0x00098D58
		// (set) Token: 0x060021A0 RID: 8608 RVA: 0x0009AB60 File Offset: 0x00098D60
		public int PageCount { get; private set; }

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x060021A1 RID: 8609 RVA: 0x0009AB69 File Offset: 0x00098D69
		// (set) Token: 0x060021A2 RID: 8610 RVA: 0x0009AB71 File Offset: 0x00098D71
		public int InsertPageIndex { get; private set; }

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x060021A3 RID: 8611 RVA: 0x0009AB7A File Offset: 0x00098D7A
		// (set) Token: 0x060021A4 RID: 8612 RVA: 0x0009AB82 File Offset: 0x00098D82
		public bool InsertBefore { get; private set; }

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x060021A5 RID: 8613 RVA: 0x0009AB8B File Offset: 0x00098D8B
		// (set) Token: 0x060021A6 RID: 8614 RVA: 0x0009AB93 File Offset: 0x00098D93
		public SizeF PageSize { get; private set; }

		// Token: 0x060021A7 RID: 8615 RVA: 0x0009AB9C File Offset: 0x00098D9C
		private static float GetPixValueFromMM(float mm)
		{
			return mm * 7200f / 2540f;
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x0009ABAC File Offset: 0x00098DAC
		private static int GetMMValueFromPix(float pix)
		{
			return (int)Math.Round((double)(pix * 2540f / 7200f), 0);
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x0009ABC3 File Offset: 0x00098DC3
		private void InitInsertPositionRadioButtons(IEnumerable<int> selectedPages, bool fromSinglePageCmd)
		{
			if (fromSinglePageCmd)
			{
				this.InitInsertPositionRadioButtonsFromSinglePage(selectedPages);
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x0009ABD8 File Offset: 0x00098DD8
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
			this.EndRadioButton.IsChecked = new bool?(true);
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x0009AC74 File Offset: 0x00098E74
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

		// Token: 0x060021AC RID: 8620 RVA: 0x0009AD94 File Offset: 0x00098F94
		private void cbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.cbPageSize.SelectedIndex == 3)
				{
					this.tbPageHeight.Visibility = Visibility.Visible;
					this.tbPageWidth.Visibility = Visibility.Visible;
					this.PortraitRadioButton.Visibility = Visibility.Collapsed;
					this.LandscapeRadioButton.Visibility = Visibility.Collapsed;
				}
				else
				{
					this.tbPageHeight.Visibility = Visibility.Collapsed;
					this.tbPageWidth.Visibility = Visibility.Collapsed;
					this.PortraitRadioButton.Visibility = Visibility.Visible;
					this.LandscapeRadioButton.Visibility = Visibility.Visible;
				}
			}
			catch
			{
			}
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x0009AE28 File Offset: 0x00099028
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.PageCountNumberBox.IsKeyboardFocusWithin)
				{
					Keyboard.ClearFocus();
				}
				if (this.PageCountNumberBox.IsValid)
				{
					this.PageCount = (int)this.PageCountNumberBox.Value;
					float num;
					float num2;
					if (this.cbPageSize.SelectedIndex == 0)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.DefaultSize.Height);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.DefaultSize.Width);
						}
						else
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.DefaultSize.Width);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.DefaultSize.Height);
						}
					}
					else if (this.cbPageSize.SelectedIndex == 1)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.A4SizeF.Height);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.A4SizeF.Width);
						}
						else
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.A4SizeF.Width);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.A4SizeF.Height);
						}
					}
					else if (this.cbPageSize.SelectedIndex == 2)
					{
						bool? flag = this.PortraitRadioButton.IsChecked;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.A3SizeF.Height);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.A3SizeF.Width);
						}
						else
						{
							num = InsertBlankPageDialog.GetPixValueFromMM(this.A3SizeF.Width);
							num2 = InsertBlankPageDialog.GetPixValueFromMM(this.A3SizeF.Height);
						}
					}
					else
					{
						float num3 = (float)this.tboxPageWidth.Value;
						float num4 = (float)this.tboxPageHeight.Value;
						num = InsertBlankPageDialog.GetPixValueFromMM(num3);
						num2 = InsertBlankPageDialog.GetPixValueFromMM(num4);
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
					if (this.PageRadioButton.IsChecked.GetValueOrDefault())
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

		// Token: 0x060021AE RID: 8622 RVA: 0x0009B114 File Offset: 0x00099314
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

		// Token: 0x060021AF RID: 8623 RVA: 0x0009B1A8 File Offset: 0x000993A8
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

		// Token: 0x060021B0 RID: 8624 RVA: 0x0009B208 File Offset: 0x00099408
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x0009B210 File Offset: 0x00099410
		private void PortraitRadioButton_Checked(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x0009B214 File Offset: 0x00099414
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

		// Token: 0x060021B3 RID: 8627 RVA: 0x0009B2AD File Offset: 0x000994AD
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x04000DCF RID: 3535
		private readonly PdfDocument doc;

		// Token: 0x04000DD0 RID: 3536
		private int firstSelectedPage = -1;

		// Token: 0x04000DD1 RID: 3537
		private int lastSelectedPage = -1;

		// Token: 0x04000DD2 RID: 3538
		private int selectedFirstIndex = -1;

		// Token: 0x04000DD3 RID: 3539
		private SizeF DefaultSize = new SizeF(210f, 297f);

		// Token: 0x04000DD4 RID: 3540
		private SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x04000DD5 RID: 3541
		private SizeF A3SizeF = new SizeF(297f, 420f);

		// Token: 0x04000DD6 RID: 3542
		private Dictionary<string, SizeF> dicItem = new Dictionary<string, SizeF>();
	}
}
