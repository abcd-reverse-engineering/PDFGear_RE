using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Common;
using CommonLib.Controls.ColorPickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001BC RID: 444
	public partial class LinkEditWindows : Window
	{
		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x0600194F RID: 6479 RVA: 0x00063F35 File Offset: 0x00062135
		// (set) Token: 0x06001950 RID: 6480 RVA: 0x00063F3D File Offset: 0x0006213D
		public LinkSelect SelectedType { get; set; }

		// Token: 0x06001951 RID: 6481 RVA: 0x00063F48 File Offset: 0x00062148
		public LinkEditWindows(PdfDocument pdfDocument)
		{
			this.InitializeComponent();
			GAManager.SendEvent("PDFLink", "WindowShow", "Create", 1L);
			base.Loaded += this.LinkEditWindows_Loaded;
			this.Document = pdfDocument;
			this.pagenum.Text = "/ " + pdfDocument.Pages.Count.ToString();
			this.SelectedType = LinkSelect.ToPage;
			this.InitMenu();
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x00064008 File Offset: 0x00062208
		public LinkEditWindows(LinkAnnotationModel linkAnnotationModel)
		{
			this.InitializeComponent();
			GAManager.SendEvent("PDFLink", "WindowShow", "Edit", 1L);
			this.Editorloaded(linkAnnotationModel);
			this.InitMenu();
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x00064088 File Offset: 0x00062288
		private void LinkEditWindows_Loaded(object sender, RoutedEventArgs e)
		{
			Color color = (Color)ColorConverter.ConvertFromString(this.SelectedFontground);
			this.WatermarkColorPicker.SelectedColor = color;
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x000640B2 File Offset: 0x000622B2
		private void InitMenu()
		{
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				if (!this.CheckOk())
				{
					return;
				}
				Ioc.Default.GetRequiredService<MainViewModel>();
				if (this.SelectedType == LinkSelect.ToWeb && string.IsNullOrEmpty(this.UrlFilePath))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkUriEmpty, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				if (this.SelectedType == LinkSelect.ToFile && string.IsNullOrEmpty(this.FileDiaoligFiePath))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkUriEmpty, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				Uri uri;
				if (this.SelectedType == LinkSelect.ToWeb && !Uri.TryCreate(this.UrlFilePath, UriKind.Absolute, out uri))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkUriError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				Color selectedColor = this.WatermarkColorPicker.SelectedColor;
				this.SelectedFontground = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B });
				ContentControl contentControl = this.linkstylyCombobox.SelectedItem as ComboBoxItem;
				float.TryParse((this.BorderstyleCombobox.SelectedItem as ComboBoxItem).Tag.ToString(), out this.BorderWidth);
				int selectedIndex = this.LinestyleCombobox.SelectedIndex;
				this.BorderStyles = LinkEditWindows.GetBorder(selectedIndex);
				if (contentControl.Content.ToString() == pdfeditor.Properties.Resources.LinkWinInvisibleRect)
				{
					this.rectangleVis = false;
				}
				GAManager.SendEvent("PDFLink", "WindowCreateEdit", this.SelectedType.ToString(), 1L);
				base.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				base.DialogResult = new bool?(false);
			};
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x000640E2 File Offset: 0x000622E2
		private static BorderStyles GetBorder(int Selectindex)
		{
			switch (Selectindex)
			{
			case 0:
				return BorderStyles.Solid;
			case 1:
				return BorderStyles.Dashed;
			case 2:
				return BorderStyles.Underline;
			default:
				return BorderStyles.Solid;
			}
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x000640FF File Offset: 0x000622FF
		private static int SetBorder(BorderStyles borderStyles)
		{
			switch (borderStyles)
			{
			case BorderStyles.Solid:
				return 0;
			case BorderStyles.Dashed:
				return 1;
			case BorderStyles.Underline:
				return 2;
			}
			return 0;
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x00064124 File Offset: 0x00062324
		public static bool isInputNumber(KeyEventArgs e)
		{
			if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
			{
				if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
				{
					return true;
				}
				e.Handled = true;
			}
			else
			{
				e.Handled = true;
			}
			return false;
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x000641A0 File Offset: 0x000623A0
		private bool CheckOk()
		{
			if (this.SelectedType == LinkSelect.ToPage && !string.IsNullOrEmpty(this.pagecur.Text))
			{
				if (this.Page > 0 && this.Page <= this.Document.Pages.Count)
				{
					return true;
				}
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return false;
			}
			else
			{
				if (this.SelectedType == LinkSelect.ToWeb && !string.IsNullOrEmpty(this.urlcur.Text))
				{
					this.UrlFilePath = this.urlcur.Text;
					return true;
				}
				if (this.SelectedType == LinkSelect.ToFile && !string.IsNullOrEmpty(this.localcur.Text))
				{
					this.FileDiaoligFiePath = this.localcur.Text;
					return true;
				}
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkUriEmpty, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return false;
			}
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x00064274 File Offset: 0x00062474
		private void openfolder_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "All Files(*.*)|*.*",
				ShowReadOnly = false,
				ReadOnlyChecked = true
			};
			if (openFileDialog.ShowDialog().GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
			{
				try
				{
					this.FileDiaoligFiePath = openFileDialog.FileName;
					this.localcur.Text = openFileDialog.FileName;
					goto IL_006E;
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
			}
			this.FileDiaoligFiePath = string.Empty;
			IL_006E:
			base.Activate();
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x00064308 File Offset: 0x00062508
		private void CurrentPageRadioButton_Click(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if (radioButton.IsChecked.Value)
			{
				string name = radioButton.Name;
				if (name == "ToFileRadioButton")
				{
					this.SelectedType = LinkSelect.ToFile;
					this.localcur.IsEnabled = true;
					this.urlcur.IsEnabled = false;
					this.pagecur.IsEnabled = false;
					this.openfolder.IsEnabled = true;
					this.pagecur.SetResourceReference(Control.BackgroundProperty, "SignaturePickerBackground");
					return;
				}
				if (name == "ToWebRadioButton")
				{
					this.SelectedType = LinkSelect.ToWeb;
					this.localcur.IsEnabled = false;
					this.urlcur.IsEnabled = true;
					this.pagecur.IsEnabled = false;
					this.openfolder.IsEnabled = false;
					this.pagecur.SetResourceReference(Control.BackgroundProperty, "SignaturePickerBackground");
					return;
				}
				if (!(name == "ToPageRadioButton"))
				{
					return;
				}
				this.SelectedType = LinkSelect.ToPage;
				this.localcur.IsEnabled = false;
				this.urlcur.IsEnabled = false;
				this.pagecur.IsEnabled = true;
				this.openfolder.IsEnabled = false;
				if (this.Page > 0 && this.Page <= this.Document.Pages.Count)
				{
					this.pagecur.SetResourceReference(Control.BackgroundProperty, "SignaturePickerBackground");
					return;
				}
				this.pagecur.SetResourceReference(Control.BackgroundProperty, "LinkEditWindowsErrorPage");
			}
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0006447E File Offset: 0x0006267E
		private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			LinkEditWindows.isInputNumber(e);
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x00064488 File Offset: 0x00062688
		private void pagecur_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (this.pagecur.Text.Length > 0)
				{
					int num = int.Parse(this.pagecur.Text);
					if (num > 0 && num <= this.Document.Pages.Count)
					{
						this.pagecur.SetResourceReference(Control.BackgroundProperty, "SignaturePickerBackground");
					}
					else if (num <= 0)
					{
						this.pagecur.SetResourceReference(Control.BackgroundProperty, "LinkEditWindowsErrorPage");
					}
					else if (num > this.Document.Pages.Count)
					{
						this.pagecur.SetResourceReference(Control.BackgroundProperty, "LinkEditWindowsErrorPage");
					}
					this.Page = int.Parse(this.pagecur.Text);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x00064558 File Offset: 0x00062758
		private void Editorloaded(LinkAnnotationModel linkAnnotationModel)
		{
			if (linkAnnotationModel == null)
			{
				return;
			}
			base.Title = linkAnnotationModel.Title;
			this.Document = linkAnnotationModel.PdfDocument;
			this.pagenum.Text = "/ " + linkAnnotationModel.PdfDocument.Pages.Count.ToString();
			this.SelectedType = linkAnnotationModel.Action;
			this.WatermarkColorPicker.SelectedColor = linkAnnotationModel.BorderColor;
			if (this.SelectedType == LinkSelect.ToWeb)
			{
				this.urlcur.Text = linkAnnotationModel.Uri;
				this.ToWebRadioButton.IsChecked = new bool?(true);
				this.localcur.IsEnabled = false;
				this.urlcur.IsEnabled = true;
				this.pagecur.IsEnabled = false;
				this.openfolder.IsEnabled = false;
			}
			else if (this.SelectedType == LinkSelect.ToFile)
			{
				this.localcur.Text = linkAnnotationModel.FileName;
				this.ToFileRadioButton.IsChecked = new bool?(true);
				this.localcur.IsEnabled = true;
				this.urlcur.IsEnabled = false;
				this.pagecur.IsEnabled = false;
				this.openfolder.IsEnabled = true;
			}
			else
			{
				this.pagecur.Text = linkAnnotationModel.Page.ToString();
			}
			if (linkAnnotationModel.Width == 0f)
			{
				this.linkstylyCombobox.SelectedIndex = 1;
				this.BorderstyleCombobox.SelectedIndex = 2;
				return;
			}
			if ((double)linkAnnotationModel.Width == 0.25)
			{
				this.linkstylyCombobox.SelectedIndex = 0;
				this.BorderstyleCombobox.SelectedIndex = 0;
				return;
			}
			if ((double)linkAnnotationModel.Width == 0.5)
			{
				this.linkstylyCombobox.SelectedIndex = 0;
				this.BorderstyleCombobox.SelectedIndex = 1;
				return;
			}
			this.BorderstyleCombobox.SelectedIndex = (int)(linkAnnotationModel.Width + 1f);
			this.LinestyleCombobox.SelectedIndex = LinkEditWindows.SetBorder(linkAnnotationModel.BorderStyle);
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x00064748 File Offset: 0x00062948
		private void linkstylyCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int selectedIndex = (sender as ComboBox).SelectedIndex;
			if (this.LinestyleCombobox != null && this.BorderstyleCombobox != null && this.WatermarkColorPicker != null)
			{
				if (selectedIndex == 0)
				{
					this.LinestyleCombobox.IsEnabled = true;
					this.BorderstyleCombobox.IsEnabled = true;
					this.WatermarkColorPicker.IsEnabled = true;
					return;
				}
				this.LinestyleCombobox.IsEnabled = false;
				this.BorderstyleCombobox.IsEnabled = false;
				this.WatermarkColorPicker.IsEnabled = false;
			}
		}

		// Token: 0x040008A3 RID: 2211
		internal string FileDiaoligFiePath = string.Empty;

		// Token: 0x040008A4 RID: 2212
		public bool RactVisible = true;

		// Token: 0x040008A5 RID: 2213
		public string UrlFilePath = string.Empty;

		// Token: 0x040008A6 RID: 2214
		public int Page = 1;

		// Token: 0x040008A7 RID: 2215
		private PdfDocument Document;

		// Token: 0x040008A8 RID: 2216
		public string SelectedFontground = "#FF000000";

		// Token: 0x040008A9 RID: 2217
		public bool rectangleVis = true;

		// Token: 0x040008AA RID: 2218
		public float BorderWidth = 1f;

		// Token: 0x040008AB RID: 2219
		public BorderStyles BorderStyles;
	}
}
