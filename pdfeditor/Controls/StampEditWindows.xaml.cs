using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls.ColorPickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using pdfeditor.Controls.Signature;
using pdfeditor.Controls.Stamp;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001DE RID: 478
	public partial class StampEditWindows : Window
	{
		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x06001B07 RID: 6919 RVA: 0x0006CA1A File Offset: 0x0006AC1A
		public SignatureCreateDialogResult ResultModel
		{
			get
			{
				return this.resultModel;
			}
		}

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06001B08 RID: 6920 RVA: 0x0006CA22 File Offset: 0x0006AC22
		// (set) Token: 0x06001B09 RID: 6921 RVA: 0x0006CA2A File Offset: 0x0006AC2A
		public IStampTextModel StampTextModel { get; private set; }

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06001B0A RID: 6922 RVA: 0x0006CA33 File Offset: 0x0006AC33
		// (set) Token: 0x06001B0B RID: 6923 RVA: 0x0006CA45 File Offset: 0x0006AC45
		public bool ClearVisible
		{
			get
			{
				return (bool)base.GetValue(StampEditWindows.ClearVisibleProperty);
			}
			set
			{
				base.SetValue(StampEditWindows.ClearVisibleProperty, value);
			}
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x0006CA58 File Offset: 0x0006AC58
		public StampEditWindows()
		{
			this.InitializeComponent();
			this.MainMenus = new ObservableCollection<MenuModel>();
			this.stampText = new StampTextModel
			{
				FontColor = "#FF20C48F"
			};
			this.stampPreviewText = new StampTextModel
			{
				FontColor = "#FF20C48F"
			};
			this.PreviewImageContainer.StampModel = this.stampPreviewText;
			this.InitMenu();
			this.Menus.ItemsSource = this.MainMenus;
			this.Menus.SelectedIndex = 0;
			this.UpdatePreviewImage();
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x0006CB10 File Offset: 0x0006AD10
		private void InitMenu()
		{
			this.MainMenus.Clear();
			this.MainMenus.Add(new MenuModel
			{
				Title = pdfeditor.Properties.Resources.WinSignatureMenuInputContent,
				Tag = "Type"
			});
			this.MainMenus.Add(new MenuModel
			{
				Title = pdfeditor.Properties.Resources.WinSignatureMenuPictureContent,
				Tag = "Picture"
			});
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				if (!this.CheckOk())
				{
					return;
				}
				Ioc.Default.GetRequiredService<MainViewModel>();
				if (this.Menus.SelectedIndex == 1)
				{
					this.ResultModel.RemoveBackground = this.ckbRemoveBg.IsChecked.Value;
					if (this.SaveCheck.IsChecked.GetValueOrDefault())
					{
						this.SavePictureImg();
						if (this.ResultModel.RemoveBackground)
						{
							this.SaveConfigRemoveBg(this.ResultModel.ImageFilePath);
						}
					}
				}
				else if (this.Menus.SelectedIndex == 0)
				{
					this.stampText.TextContent = this.TypeWriterCtrl.Text.Trim();
					this.stampText.FontColor = ((Color)this.ForegroundPickerList.SelectedItem).ToString();
					this.UpdatePreviewImage();
					if (string.IsNullOrEmpty(this.stampText.TextContent))
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkTextEmptyMsg, UtilManager.GetProductName());
						return;
					}
					if (this.stampText.TextContent.Trim().Length > 50)
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinCustomizeStampMaxCharactersMsg, UtilManager.GetProductName());
						return;
					}
					this.StampTextModel = this.stampText;
				}
				this.isSave = this.SaveCheck.IsChecked.Value;
				string text = ((this.Menus.SelectedIndex == 0) ? "Type-" : "Picture-");
				text += (this.isSave ? "Save" : "NoSave");
				GAManager.SendEvent("PdfStampAnnotation", "CustomStampSet", text, 1L);
				base.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				this.showPicture.Source = null;
				base.DialogResult = new bool?(false);
			};
			this.DateFormatComboBox.ItemsSource = StampEditWindows.GetDateFormats();
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x0006CBB4 File Offset: 0x0006ADB4
		private bool CheckOk()
		{
			return (!(this.SelectedMenuModel.Title == pdfeditor.Properties.Resources.WinSignatureMenuPictureContent) || this.showPicture.Source != null) && !(this.SelectedMenuModel.Title == pdfeditor.Properties.Resources.WinSignatureMenuWriteContent);
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0006CC04 File Offset: 0x0006AE04
		public void SavePictureImg()
		{
			if (string.IsNullOrEmpty(this.FileDiaoligFiePath))
			{
				return;
			}
			Ioc.Default.GetRequiredService<MainViewModel>();
			string text = DateTime.Now.ToString("yyyyMMddHHmmss");
			string text2 = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text2, "StampWrite" + text + ".png");
			File.Copy(this.FileDiaoligFiePath, text3, true);
			this.ResultModel.ImageFilePath = text3;
			this.FileDiaoligFiePath = string.Empty;
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x0006CC98 File Offset: 0x0006AE98
		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			if (this.SelectedMenuModel != null)
			{
				string title = this.SelectedMenuModel.Title;
				string tag = this.SelectedMenuModel.Tag;
				if (tag == "Picture")
				{
					this.showPicture.Source = null;
					this.PictureCtrl.Visibility = Visibility.Visible;
					this.ShowClear();
				}
				if (tag == "Type")
				{
					this.TypeWriterCtrl.Text = string.Empty;
					this.ShowClear();
				}
			}
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0006CD14 File Offset: 0x0006AF14
		private void PictureCtrl_Click(object sender, RoutedEventArgs e)
		{
			Ioc.Default.GetRequiredService<MainViewModel>();
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|Windows Bitmap(*.bmp)|*.bmp|Windows Icon(*.ico)|*.ico|Graphics Interchange Format (*.gif)|(*.gif)|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Tag Image File Format (*.tif)|*.tif;*.tiff",
				ShowReadOnly = false,
				ReadOnlyChecked = true
			};
			if (openFileDialog.ShowDialog().GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
			{
				try
				{
					this.showPicture.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
					this.FileDiaoligFiePath = openFileDialog.FileName;
					this.PictureCtrl.Visibility = Visibility.Collapsed;
					this.ShowClear();
					goto IL_009B;
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
			}
			this.ResultModel.ImageFilePath = string.Empty;
			IL_009B:
			base.Activate();
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x0006CDD4 File Offset: 0x0006AFD4
		private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Selector selector = sender as Selector;
			MenuModel menuModel = ((selector != null) ? selector.SelectedItem : null) as MenuModel;
			if (menuModel != null)
			{
				this.SelectedMenuModel = menuModel;
				this.ShowClear();
				string title = this.SelectedMenuModel.Title;
				string tag = this.SelectedMenuModel.Tag;
				if (tag == "Picture")
				{
					this.ImagePanel.Visibility = Visibility.Visible;
					this.TextPanel.Visibility = Visibility.Collapsed;
					this.isText = false;
					return;
				}
				if (tag == "Type")
				{
					this.ImagePanel.Visibility = Visibility.Collapsed;
					this.TextPanel.Visibility = Visibility.Visible;
					this.TypeWriterCtrl.Focus();
					this.isText = true;
				}
			}
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0006CE89 File Offset: 0x0006B089
		private void txt_Text_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
			this.ShowClear();
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x0006CE98 File Offset: 0x0006B098
		private void ShowClear()
		{
			string title = this.SelectedMenuModel.Title;
			string tag = this.SelectedMenuModel.Tag;
			if (tag == "Picture")
			{
				this.ClearVisible = this.showPicture.Source != null;
				return;
			}
			if (tag == "Type")
			{
				this.ClearVisible = !string.IsNullOrEmpty(this.TypeWriterCtrl.Text);
			}
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x0006CF05 File Offset: 0x0006B105
		private void ForegroundPickerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (base.IsLoaded)
			{
				if (e.AddedItems.Count == 0)
				{
					this.ForegroundPickerList.SelectedIndex = 0;
					return;
				}
				this.UpdatePreviewImage();
			}
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x0006CF30 File Offset: 0x0006B130
		private void ForegroundPicker_SelectedColorChanged(object sender, ColorPickerButtonSelectedColorChangedEventArgs e)
		{
			CompositeCollection compositeCollection = (CompositeCollection)this.colorSelecters.Resources["ColorCollection"];
			compositeCollection[compositeCollection.Count - 1] = e.Color;
			this.ForegroundPickerList.SelectedIndex = compositeCollection.Count - 1;
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x0006CF84 File Offset: 0x0006B184
		private void DateCheck_Click(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x0006CF8C File Offset: 0x0006B18C
		private void DateFormatComboBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x0006CF94 File Offset: 0x0006B194
		private void UpdatePreviewImage()
		{
			this.stampText.TextContent = this.TypeWriterCtrl.Text.Trim();
			this.stampText.FontColor = ((Color)this.ForegroundPickerList.SelectedItem).ToString();
			this.stampPreviewText.TextContent = this.TypeWriterCtrl.Text.Trim();
			this.stampPreviewText.FontColor = ((Color)this.ForegroundPickerList.SelectedItem).ToString();
			if (string.IsNullOrEmpty(this.stampPreviewText.TextContent))
			{
				this.stampPreviewText.TextContent = pdfeditor.Properties.Resources.WinStampSampleText;
			}
			this.PreviewImageContainer.ForceRender();
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x0006D056 File Offset: 0x0006B256
		private void SaveConfigRemoveBg(string fileName)
		{
			ConfigManager.AddSignatureRemoveBg(fileName);
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x0006D060 File Offset: 0x0006B260
		private static string[] GetDateFormats()
		{
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<string> list = new List<string>();
			dictionary["US"] = new string[] { "MM/dd/yyyy hh:mm tt", "MM/dd/yyyy hh:mm:ss tt" };
			dictionary["GB"] = new string[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss" };
			dictionary["ES"] = dictionary["GB"];
			dictionary["IT"] = dictionary["GB"];
			dictionary["PT"] = dictionary["GB"];
			dictionary["FR"] = dictionary["GB"];
			dictionary["RU"] = new string[] { "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm:ss" };
			dictionary["DE"] = dictionary["RU"];
			dictionary["NL"] = new string[] { "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss" };
			dictionary2["CN"] = new string[] { "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH:mm:ss" };
			dictionary2["ZH-CN"] = dictionary2["CN"];
			dictionary2["JA"] = dictionary2["CN"];
			dictionary2["KO"] = new string[] { "yyyy-MM-dd tt hh:mm", "yyyy-MM-dd tt hh:mm:ss" };
			dictionary2["SG"] = new string[] { "dd/MM/yyyy tt hh:mm", "dd/MM/yyyy tt hh:mm:ss" };
			string text = "";
			if (CultureInfoUtils.SystemCultureInfo != null)
			{
				string name = CultureInfoUtils.SystemUICultureInfo.Name;
				string[] array = ((name != null) ? name.Split(new char[] { '-' }) : null);
				if (array != null && array.Length == 2 && array[1].Length == 2 && (dictionary.ContainsKey(array[1]) || dictionary2.ContainsKey(array[1])))
				{
					text = array[1];
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = CultureInfoUtils.ActualAppLanguage;
			}
			text = text.ToUpperInvariant();
			string[] array2;
			string[] array3;
			if (dictionary.TryGetValue(text, out array2))
			{
				list.AddRange(array2);
			}
			else if (dictionary2.TryGetValue(text, out array3))
			{
				list.AddRange(array3);
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in dictionary)
			{
				if (keyValuePair.Key != text)
				{
					list.AddRange(keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<string, string[]> keyValuePair2 in dictionary2)
			{
				if (keyValuePair2.Key != text)
				{
					list.AddRange(keyValuePair2.Value);
				}
			}
			return list.Distinct<string>().ToArray<string>();
		}

		// Token: 0x04000967 RID: 2407
		private ObservableCollection<MenuModel> MainMenus;

		// Token: 0x04000968 RID: 2408
		private MenuModel SelectedMenuModel;

		// Token: 0x04000969 RID: 2409
		internal string FileDiaoligFiePath = string.Empty;

		// Token: 0x0400096A RID: 2410
		private SignatureCreateDialogResult resultModel = new SignatureCreateDialogResult();

		// Token: 0x0400096C RID: 2412
		private StampTextModel stampText;

		// Token: 0x0400096D RID: 2413
		private StampTextModel stampPreviewText;

		// Token: 0x0400096E RID: 2414
		public bool isText = true;

		// Token: 0x0400096F RID: 2415
		public bool isSave;

		// Token: 0x04000970 RID: 2416
		public static readonly DependencyProperty ClearVisibleProperty = DependencyProperty.Register("StampClearVisible", typeof(bool), typeof(StampEditWindows), new PropertyMetadata(false));

		// Token: 0x04000971 RID: 2417
		private double scal = 1.0;
	}
}
