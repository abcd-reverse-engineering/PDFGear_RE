using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Controls;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024D RID: 589
	public partial class InsertPageProperty : Window
	{
		// Token: 0x060021EB RID: 8683 RVA: 0x0009CBC0 File Offset: 0x0009ADC0
		public InsertPageProperty()
		{
			this.InitializeComponent();
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x0009CC38 File Offset: 0x0009AE38
		public InsertPageProperty(IEnumerable<int> selectedPages, PdfDocument doc, bool fromSinglePageCmd = false, bool createNew = false)
		{
			this.InitializeComponent();
			if (createNew)
			{
				this._createNew = createNew;
				this.PagePositionGrid.Visibility = Visibility.Collapsed;
			}
			else
			{
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
					this.PageCount.Text = doc.Pages.Count.ToString();
				}
				this.InitInsertPositionRadioButtons(selectedPages, fromSinglePageCmd);
			}
			this.InitControls();
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x0009CD50 File Offset: 0x0009AF50
		private void InitControls()
		{
			try
			{
				if (this.selectedFirstIndex != -1)
				{
					PdfPage pdfPage = this.doc.Pages[this.selectedFirstIndex];
					FS_SIZEF pageSizeByIndex = this.doc.GetPageSizeByIndex(this.selectedFirstIndex);
					this.DefaultSize.Width = (float)InsertPageProperty.GetMMValueFromPix(pageSizeByIndex.Width);
					this.DefaultSize.Height = (float)InsertPageProperty.GetMMValueFromPix(pageSizeByIndex.Height);
				}
				string text = string.Format("{0} ({1}mm x {2}mm)", pdfeditor.Properties.Resources.WinPageInsertSettingPageSizeItemDefault, this.DefaultSize.Width, this.DefaultSize.Height);
				this.dicItem.Add(text, this.A4SizeF);
				this.dicItem.Add("A4 (210mm x 297mm)", this.A4SizeF);
				this.dicItem.Add("A3 (297mm x 420mm)", this.A3SizeF);
				if (!this._createNew)
				{
					this.dicItem.Add(pdfeditor.Properties.Resources.ScannerWinSamePagesize, default(SizeF));
				}
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

		// Token: 0x060021EE RID: 8686 RVA: 0x0009CEE0 File Offset: 0x0009B0E0
		private void InitInsertPositionRadioButtons(IEnumerable<int> selectedPages, bool fromSinglePageCmd)
		{
			if (fromSinglePageCmd)
			{
				this.InitInsertPositionRadioButtonsFromSinglePage(selectedPages);
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x0009CEF4 File Offset: 0x0009B0F4
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
				this.PageCount.Text = count.ToString();
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x0009CF80 File Offset: 0x0009B180
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
				this.PageCount.Text = count.ToString();
			}
			if (this.lastSelectedPage != -1)
			{
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.InsertPosition.SelectedIndex = 0;
				this.PageCount.Text = count.ToString();
			}
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x060021F1 RID: 8689 RVA: 0x0009D0A0 File Offset: 0x0009B2A0
		// (set) Token: 0x060021F2 RID: 8690 RVA: 0x0009D0A8 File Offset: 0x0009B2A8
		public int PageCounts { get; private set; }

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x060021F3 RID: 8691 RVA: 0x0009D0B1 File Offset: 0x0009B2B1
		// (set) Token: 0x060021F4 RID: 8692 RVA: 0x0009D0B9 File Offset: 0x0009B2B9
		public int InsertPageIndex { get; private set; }

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x060021F5 RID: 8693 RVA: 0x0009D0C2 File Offset: 0x0009B2C2
		// (set) Token: 0x060021F6 RID: 8694 RVA: 0x0009D0CA File Offset: 0x0009B2CA
		public bool InsertBefore { get; private set; }

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x060021F7 RID: 8695 RVA: 0x0009D0D3 File Offset: 0x0009B2D3
		// (set) Token: 0x060021F8 RID: 8696 RVA: 0x0009D0DB File Offset: 0x0009B2DB
		public SizeF PageSize { get; private set; }

		// Token: 0x060021F9 RID: 8697 RVA: 0x0009D0E4 File Offset: 0x0009B2E4
		private static float GetPixValueFromMM(float mm)
		{
			return mm * 7200f / 2540f;
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x0009D0F4 File Offset: 0x0009B2F4
		private static int GetMMValueFromPix(float pix)
		{
			return (int)Math.Round((double)(pix * 2540f / 7200f), 0);
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x0009D10C File Offset: 0x0009B30C
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
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
			}
			base.DialogResult = new bool?(true);
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x0009D16C File Offset: 0x0009B36C
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

		// Token: 0x060021FD RID: 8701 RVA: 0x0009D200 File Offset: 0x0009B400
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

		// Token: 0x060021FE RID: 8702 RVA: 0x0009D260 File Offset: 0x0009B460
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x0009D270 File Offset: 0x0009B470
		private void cbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				int num = 4;
				if (this._createNew)
				{
					num = 3;
				}
				if (this.cbPageSize.SelectedIndex == num)
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

		// Token: 0x06002200 RID: 8704 RVA: 0x0009D310 File Offset: 0x0009B510
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

		// Token: 0x06002201 RID: 8705 RVA: 0x0009D3AC File Offset: 0x0009B5AC
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			int num;
			int.TryParse((sender as TextBox).Text, out num);
			if (num <= this.doc.Pages.Count)
			{
			}
		}

		// Token: 0x04000E23 RID: 3619
		private readonly PdfDocument doc;

		// Token: 0x04000E24 RID: 3620
		private int firstSelectedPage = -1;

		// Token: 0x04000E25 RID: 3621
		private int lastSelectedPage = -1;

		// Token: 0x04000E26 RID: 3622
		private int selectedFirstIndex = -1;

		// Token: 0x04000E27 RID: 3623
		public SizeF DefaultSize = new SizeF(210f, 297f);

		// Token: 0x04000E28 RID: 3624
		private SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x04000E29 RID: 3625
		private SizeF A3SizeF = new SizeF(297f, 420f);

		// Token: 0x04000E2A RID: 3626
		private Dictionary<string, SizeF> dicItem = new Dictionary<string, SizeF>();

		// Token: 0x04000E2B RID: 3627
		private bool _createNew;
	}
}
