using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
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
using Newtonsoft.Json;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Signature;
using pdfeditor.Models.DynamicStamps;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001E8 RID: 488
	public partial class EditStampWindow : Window
	{
		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06001BA2 RID: 7074 RVA: 0x000709F8 File Offset: 0x0006EBF8
		public SignatureCreateDialogResult ResultModel
		{
			get
			{
				return this.resultModel;
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x00070A00 File Offset: 0x0006EC00
		// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x00070A08 File Offset: 0x0006EC08
		public DynamicStampTextModel StampTextModel { get; private set; }

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06001BA5 RID: 7077 RVA: 0x00070A11 File Offset: 0x0006EC11
		// (set) Token: 0x06001BA6 RID: 7078 RVA: 0x00070A23 File Offset: 0x0006EC23
		public bool ClearVisible
		{
			get
			{
				return (bool)base.GetValue(EditStampWindow.ClearVisibleProperty);
			}
			set
			{
				base.SetValue(EditStampWindow.ClearVisibleProperty, value);
			}
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x00070A38 File Offset: 0x0006EC38
		public EditStampWindow(CustStampModel item)
		{
			GAManager.SendEvent("CustomStampWindow", "Show", "Edit", 1L);
			this.InitializeComponent();
			this.EditItem = item;
			base.Title = pdfeditor.Properties.Resources.EditCustomStampWinTitle;
			List<string> list = (from n in new InstalledFontCollection().Families.Select((global::System.Drawing.FontFamily f) => f.Name).Distinct<string>()
				orderby n
				select n).ToList<string>();
			this.FontComboBox.ItemsSource = list;
			string systemFont = global::System.Windows.SystemFonts.MessageFontFamily.Source;
			this.FontComboBox.SelectedItem = list.FirstOrDefault((string f) => f.Equals(systemFont));
			this.GroupItemInitMenu(item);
			this.SaveCheck.IsChecked = new bool?(true);
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BA8 RID: 7080 RVA: 0x00070B50 File Offset: 0x0006ED50
		public EditStampWindow(string GroupName)
		{
			GAManager.SendEvent("CustomStampWindow", "Show", "NewFromMgmt", 1L);
			this.InitializeComponent();
			List<string> list = (from n in new InstalledFontCollection().Families.Select((global::System.Drawing.FontFamily f) => f.Name).Distinct<string>()
				orderby n
				select n).ToList<string>();
			this.FontComboBox.ItemsSource = list;
			string systemFont = global::System.Windows.SystemFonts.MessageFontFamily.Source;
			this.FontComboBox.SelectedItem = list.FirstOrDefault((string f) => f.Equals(systemFont));
			this.InitMenu(GroupName);
			this.SaveCheck.IsChecked = new bool?(true);
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BA9 RID: 7081 RVA: 0x00070C58 File Offset: 0x0006EE58
		public EditStampWindow()
		{
			GAManager.SendEvent("CustomStampWindow", "Show", "New", 1L);
			this.InitializeComponent();
			List<string> list = (from n in new InstalledFontCollection().Families.Select((global::System.Drawing.FontFamily f) => f.Name).Distinct<string>()
				orderby n
				select n).ToList<string>();
			this.FontComboBox.ItemsSource = list;
			string systemFont = global::System.Windows.SystemFonts.MessageFontFamily.Source;
			this.FontComboBox.SelectedItem = list.FirstOrDefault((string f) => f.Equals(systemFont));
			this.InitMenu(null);
			this.SaveCheck.IsChecked = new bool?(true);
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BAA RID: 7082 RVA: 0x00070D60 File Offset: 0x0006EF60
		private void InitMenu(string groupName = null)
		{
			List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
			if (list != null)
			{
				List<string> list2 = new List<string>();
				list2 = list.Select((DynamicStampTextModel f) => f.GroupName).Distinct<string>().ToList<string>();
				string stampCategory = ConfigManager.GetStampCategory();
				if (!string.IsNullOrEmpty(stampCategory))
				{
					using (List<string>.Enumerator enumerator = JsonConvert.DeserializeObject<List<string>>(stampCategory).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string localCategory = enumerator.Current;
							if (list2.FindIndex((string x) => x == localCategory) < 0)
							{
								list2.Add(localCategory);
							}
						}
					}
				}
				if (list2.Count < 5)
				{
					list2.Insert(0, pdfeditor.Properties.Resources.EditStampWinCategoryListNewContent);
					this.GetDefaultCategoryName(list2);
				}
				else
				{
					this.NewCategoryName.Visibility = Visibility.Collapsed;
				}
				this.CategoryComboBox.Items.Clear();
				this.CategoryComboBox.ItemsSource = list2;
				if (!string.IsNullOrEmpty(groupName))
				{
					this.SaveCheck.IsChecked = new bool?(true);
					if (list2.FindIndex((string x) => x.Contains(groupName)) < 0)
					{
						list2.Add(groupName);
					}
					this.CategoryComboBox.SelectedItem = list2.FirstOrDefault((string x) => x == groupName);
				}
				else if (list2.Count >= 2)
				{
					string lastGroupName = ConfigManager.GetLastStampCategory();
					if (string.IsNullOrWhiteSpace(list2.FirstOrDefault((string x) => x == lastGroupName)))
					{
						this.CategoryComboBox.SelectedIndex = 1;
					}
					else
					{
						this.CategoryComboBox.SelectedItem = list2.FirstOrDefault((string x) => x == lastGroupName);
					}
				}
				else
				{
					this.CategoryComboBox.SelectedIndex = 0;
				}
			}
			else
			{
				List<string> list3 = new List<string>();
				list3.Insert(0, pdfeditor.Properties.Resources.EditStampWinCategoryListNewContent);
				this.GetDefaultCategoryName(list3);
				this.CategoryComboBox.Items.Clear();
				this.CategoryComboBox.ItemsSource = list3;
				string stampCategory2 = ConfigManager.GetStampCategory();
				if (!string.IsNullOrEmpty(stampCategory2))
				{
					foreach (string text in JsonConvert.DeserializeObject<List<string>>(stampCategory2))
					{
						string localgroupName = text;
						if (list3.FindIndex((string x) => x.Contains(localgroupName)) < 0)
						{
							list3.Add(localgroupName);
						}
					}
				}
				if (!string.IsNullOrEmpty(groupName))
				{
					list3.FirstOrDefault((string x) => x.Contains(groupName));
					if (list3.FindIndex((string x) => x.Contains(groupName)) < 0)
					{
						list3.Add(groupName);
					}
					this.CategoryComboBox.SelectedItem = list3.FirstOrDefault((string x) => x == groupName);
				}
			}
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				if (!this.CheckOk())
				{
					return;
				}
				Ioc.Default.GetRequiredService<MainViewModel>();
				if (this.lstBoxMenu.SelectedIndex == 1)
				{
					if (string.IsNullOrEmpty(this.ImageFilePath.Text))
					{
						MessageBox.Show(pdfeditor.Properties.Resources.EditStampWinImageEmptyErrorMessage, UtilManager.GetProductName());
						return;
					}
					this.ResultModel.RemoveBackground = this.ckbRemoveBg.IsChecked.Value;
					if (this.SaveCheck.IsChecked.GetValueOrDefault())
					{
						this.SavePictureImg();
						this.UpdateConfigRemoveBg(this.ResultModel.ImageFilePath, this.ResultModel.RemoveBackground);
						this.StampTextModel = new DynamicStampTextModel(null, null, this.ResultModel.ImageFilePath, null);
					}
					this.isText = false;
					GAManager.SendEvent("CustomStampWindow", "NewImageStamp", "Count", 1L);
				}
				else if (this.lstBoxMenu.SelectedIndex == 0)
				{
					this.UpdatePreviewImage();
					if (this.lstBoxMenu.SelectedIndex == 0)
					{
						this.StampTextModel = new DynamicStampTextModel(this.PreviewImageContainer.DynamicStampProperties, this.PreviewImageContainer.StampColor.ToString(), this.PreviewImageContainer.TemplateName, null);
					}
					if (string.IsNullOrEmpty(this.StampTextModel.DynamicProperties.Contents[0].Content))
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkTextEmptyMsg, UtilManager.GetProductName());
						return;
					}
					if (this.StampTextModel.DynamicProperties.Contents[0].Content.Trim().Length > 50)
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinCustomizeStampMaxCharactersMsg, UtilManager.GetProductName());
						return;
					}
					DynamicStampProperties.ContentType contentType = this.StampTextModel.DynamicProperties.Contents[0].ContentType;
					DynamicStampProperties.ContentType contentType2 = this.StampTextModel.DynamicProperties.Contents[1].ContentType;
					DynamicStampProperties.ContentType contentType3 = this.StampTextModel.DynamicProperties.Contents[2].ContentType;
					GAManager.SendEvent("CustomStampWindow", "NewTextStamp", string.Format("{0},{1},{2}", contentType, contentType2, contentType3), 1L);
				}
				this.isSave = this.SaveCheck.IsChecked.Value;
				if (this.isSave)
				{
					if (this.CategoryComboBox.SelectedIndex == 0 && this.CategoryComboBox.Items.Count <= 5 && this.NewCategoryName.Visibility == Visibility.Visible)
					{
						this.StampTextModel.GroupName = this.NewCategoryName.Text;
					}
					else
					{
						this.StampTextModel.GroupName = this.CategoryComboBox.SelectedItem.ToString();
					}
					this.StampTextModel.Name = this.StampName.Text;
					ConfigManager.SetLastStampCategory(this.StampTextModel.GroupName);
				}
				GAManager.SendEvent("CustomStampWindow", "OkBtn", this.isSave ? "Save" : "NotSave", 1L);
				this.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				this.DialogResult = new bool?(false);
				GAManager.SendEvent("CustomStampWindow", "CancelBtn", "Count", 1L);
			};
			this.DateFormatComboBox1.ItemsSource = EditStampWindow.GetDateFormats();
			this.DateFormatComboBox2.ItemsSource = EditStampWindow.GetDateFormats();
			this.DateFormatComboBox3.ItemsSource = EditStampWindow.GetDateFormats();
		}

		// Token: 0x06001BAB RID: 7083 RVA: 0x000710E0 File Offset: 0x0006F2E0
		private void GroupItemInitMenu(CustStampModel item)
		{
			List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
			if (list != null)
			{
				List<string> list2 = new List<string>();
				list2 = list.Select((DynamicStampTextModel f) => f.GroupName).Distinct<string>().ToList<string>();
				string stampCategory = ConfigManager.GetStampCategory();
				if (!string.IsNullOrEmpty(stampCategory))
				{
					using (List<string>.Enumerator enumerator = JsonConvert.DeserializeObject<List<string>>(stampCategory).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string localCategory = enumerator.Current;
							if (list2.FindIndex((string x) => x == localCategory) < 0)
							{
								list2.Add(localCategory);
							}
						}
					}
				}
				if (list2.Count < 5)
				{
					list2.Insert(0, pdfeditor.Properties.Resources.EditStampWinCategoryListNewContent);
					this.GetDefaultCategoryName(list2);
				}
				else
				{
					this.NewCategoryName.Visibility = Visibility.Collapsed;
				}
				this.CategoryComboBox.Items.Clear();
				this.CategoryComboBox.ItemsSource = list2;
				this.CategoryComboBox.SelectedItem = list2.FirstOrDefault((string x) => x.Contains(item.DynamicStampTextModel.GroupName));
				this.StampName.Text = item.DynamicStampTextModel.Name;
			}
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				if (!this.CheckOk())
				{
					return;
				}
				Ioc.Default.GetRequiredService<MainViewModel>();
				if (this.lstBoxMenu.SelectedIndex == 1)
				{
					if (string.IsNullOrEmpty(this.ImageFilePath.Text))
					{
						MessageBox.Show(pdfeditor.Properties.Resources.EditStampWinImageEmptyErrorMessage, UtilManager.GetProductName());
						return;
					}
					this.ResultModel.RemoveBackground = this.ckbRemoveBg.IsChecked.Value;
					if (this.SaveCheck.IsChecked.GetValueOrDefault())
					{
						if (this.FileDiaoligFiePath != this.EditItem.ImageFilePath)
						{
							this.SavePictureImg();
						}
						this.UpdateConfigRemoveBg(this.FileDiaoligFiePath, this.ResultModel.RemoveBackground);
					}
					this.StampTextModel = new DynamicStampTextModel(null, null, this.FileDiaoligFiePath, null);
					this.isText = false;
				}
				else if (this.lstBoxMenu.SelectedIndex == 0)
				{
					this.UpdatePreviewImage();
					if (this.lstBoxMenu.SelectedIndex == 0)
					{
						this.StampTextModel = new DynamicStampTextModel(this.PreviewImageContainer.DynamicStampProperties, this.PreviewImageContainer.StampColor.ToString(), this.PreviewImageContainer.TemplateName, null);
					}
					if (string.IsNullOrEmpty(this.StampTextModel.DynamicProperties.Contents[0].Content))
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkTextEmptyMsg, UtilManager.GetProductName());
						return;
					}
					if (this.StampTextModel.DynamicProperties.Contents[0].Content.Trim().Length > 50)
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinCustomizeStampMaxCharactersMsg, UtilManager.GetProductName());
						return;
					}
				}
				this.isSave = this.SaveCheck.IsChecked.Value;
				if (this.isSave)
				{
					if (this.CategoryComboBox.SelectedIndex == 0 && this.CategoryComboBox.Items.Count <= 5 && this.NewCategoryName.Visibility == Visibility.Visible)
					{
						this.StampTextModel.GroupName = this.NewCategoryName.Text;
					}
					else
					{
						this.StampTextModel.GroupName = this.CategoryComboBox.SelectedItem.ToString();
					}
					this.StampTextModel.Name = this.StampName.Text;
				}
				this.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				this.isSave = false;
				this.DialogResult = new bool?(false);
			};
			this.DateFormatComboBox1.ItemsSource = EditStampWindow.GetDateFormats();
			this.DateFormatComboBox2.ItemsSource = EditStampWindow.GetDateFormats();
			this.DateFormatComboBox3.ItemsSource = EditStampWindow.GetDateFormats();
			this.ApplySettings();
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x000712A4 File Offset: 0x0006F4A4
		private void GetDefaultCategoryName(List<string> CategoryNames)
		{
			string categoryName = pdfeditor.Properties.Resources.EditStampWinDefaultCategoryName;
			Predicate<string> <>9__0;
			for (int i = 0; i < CategoryNames.Count; i++)
			{
				if (i > 0)
				{
					categoryName = string.Format("{0} {1}", pdfeditor.Properties.Resources.EditStampWinDefaultCategoryName, i);
				}
				Predicate<string> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = (string x) => x == categoryName);
				}
				if (CategoryNames.FindIndex(predicate) <= 0)
				{
					this.NewCategoryName.Text = categoryName;
					return;
				}
			}
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x0007132C File Offset: 0x0006F52C
		private void ApplySettings()
		{
			this.applying = true;
			this.SaveCheck.Visibility = Visibility.Collapsed;
			this.SaveCheck.IsChecked = new bool?(true);
			if (this.EditItem.Image == "Visible")
			{
				this.lstBoxMenu.SelectedIndex = 1;
				BitmapImage bitmapImage = this.showPicture.Source as BitmapImage;
				if (bitmapImage != null)
				{
					Stream streamSource = bitmapImage.StreamSource;
					if (streamSource != null)
					{
						streamSource.Dispose();
					}
					this.showPicture.Source = null;
				}
				BitmapImage bitmapImage2 = new BitmapImage();
				bitmapImage2.BeginInit();
				bitmapImage2.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage2.UriSource = new Uri(this.EditItem.ImageFilePath, UriKind.Absolute);
				bitmapImage2.EndInit();
				bitmapImage2.Freeze();
				this.showPicture.Source = bitmapImage2;
				this.FileDiaoligFiePath = this.EditItem.ImageFilePath;
				this.ImageFilePath.Text = this.FileDiaoligFiePath;
				if (!string.IsNullOrEmpty(this.FileDiaoligFiePath))
				{
					this.ckbRemoveBg.IsChecked = new bool?(ConfigManager.IsRemoveSignatureBg(this.FileDiaoligFiePath));
				}
				this.PictureCtrl.IsEnabled = false;
			}
			else
			{
				this.lstBoxMenu.SelectedIndex = 0;
				if (this.EditItem.DynamicStampTextModel.DynamicProperties != null)
				{
					this.PreviewImageContainer.DynamicStampProperties = this.EditItem.DynamicStampTextModel.DynamicProperties;
					for (int i = 0; i < this.EditItem.DynamicStampTextModel.DynamicProperties.Contents.Count; i++)
					{
						if (this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].ContentType == DynamicStampProperties.ContentType.Text)
						{
							if (i == 0)
							{
								this.Content1ComboBox.SelectedIndex = 0;
								this.TypeWriterCtrl1.Text = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
							else if (i == 1)
							{
								this.Content2ComboBox.SelectedIndex = 0;
								this.TypeWriterCtrl2.Text = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
							else if (i == 2)
							{
								this.Content3ComboBox.SelectedIndex = 0;
								this.TypeWriterCtrl3.Text = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
						}
						else if (this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].ContentType == DynamicStampProperties.ContentType.Time)
						{
							if (i == 0)
							{
								this.Content1ComboBox.SelectedIndex = 1;
								this.DateFormatComboBox1.SelectedItem = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
							else if (i == 1)
							{
								this.Content2ComboBox.SelectedIndex = 1;
								this.DateFormatComboBox2.SelectedItem = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
							else if (i == 2)
							{
								this.Content3ComboBox.SelectedIndex = 1;
								this.DateFormatComboBox3.SelectedItem = this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].Content;
							}
						}
						else if (this.EditItem.DynamicStampTextModel.DynamicProperties.Contents[i].ContentType == DynamicStampProperties.ContentType.None)
						{
							if (i == 1)
							{
								this.Content2ComboBox.SelectedIndex = 2;
							}
							else if (i == 2)
							{
								this.Content3ComboBox.SelectedIndex = 2;
							}
						}
					}
					if (this.EditItem.DynamicStampTextModel.TemplateName == "Chop1")
					{
						this.ShapeEllipse.IsChecked = new bool?(true);
					}
					else if (this.EditItem.DynamicStampTextModel.TemplateName == "Square")
					{
						this.ShapeRechangle.IsChecked = new bool?(true);
					}
					else if (this.EditItem.DynamicStampTextModel.TemplateName == "Arrow1")
					{
						if (this.EditItem.DynamicStampTextModel.DynamicProperties.ArrowDirection == DynamicStampProperties.ArrowDirections.Right)
						{
							this.ShapeSingleArrow.IsChecked = new bool?(true);
						}
						else
						{
							this.ShapeSingleReverseArrow.IsChecked = new bool?(true);
						}
					}
					else if (this.EditItem.DynamicStampTextModel.TemplateName == "Arrow2")
					{
						if (this.EditItem.DynamicStampTextModel.DynamicProperties.ArrowDirection == DynamicStampProperties.ArrowDirections.Right)
						{
							this.ShapeDoubleArrow.IsChecked = new bool?(true);
						}
						else
						{
							this.ShapeDoubleReverseArrow.IsChecked = new bool?(true);
						}
					}
					if (this.EditItem.DynamicStampTextModel.FontColor == "#FF20C48F")
					{
						this.ForegroundPickerList.SelectedIndex = 0;
					}
					else if (this.EditItem.DynamicStampTextModel.FontColor == "#FF298FEE")
					{
						this.ForegroundPickerList.SelectedIndex = 1;
					}
					else if (this.EditItem.DynamicStampTextModel.FontColor == "#FFFF6932")
					{
						this.ForegroundPickerList.SelectedIndex = 2;
					}
					else if (this.EditItem.DynamicStampTextModel.FontColor == "#FFB80000")
					{
						this.ForegroundPickerList.SelectedIndex = 3;
					}
					else
					{
						CompositeCollection compositeCollection = (CompositeCollection)this.colorSelecters.Resources["ColorCollection"];
						compositeCollection[compositeCollection.Count - 1] = global::System.Windows.Media.ColorConverter.ConvertFromString(this.EditItem.DynamicStampTextModel.FontColor);
						this.ForegroundPickerList.SelectedIndex = compositeCollection.Count - 1;
					}
					this.PreviewImageContainer.StampColor = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(this.EditItem.DynamicStampTextModel.FontColor);
					this.FontComboBox.SelectedItem = this.EditItem.DynamicStampTextModel.DynamicProperties.FontFamily;
					double? num = this.EditItem.DynamicStampTextModel.DynamicProperties.FontSize;
					double num2 = (double)20;
					if ((num.GetValueOrDefault() == num2) & (num != null))
					{
						this.FontSizeComboBox.SelectedIndex = 1;
					}
					else
					{
						num = this.EditItem.DynamicStampTextModel.DynamicProperties.FontSize;
						num2 = (double)12;
						if ((num.GetValueOrDefault() == num2) & (num != null))
						{
							this.FontSizeComboBox.SelectedIndex = 2;
						}
						else
						{
							num = this.EditItem.DynamicStampTextModel.DynamicProperties.FontSize;
							num2 = (double)28;
							if ((num.GetValueOrDefault() == num2) & (num != null))
							{
								this.FontSizeComboBox.SelectedIndex = 3;
							}
							else
							{
								this.FontSizeComboBox.SelectedIndex = 0;
							}
						}
					}
				}
				else
				{
					this.Content2ComboBox.SelectedIndex = 2;
					this.TypeWriterCtrl1.Text = this.EditItem.TextContent;
					this.ShapeRechangle.IsChecked = new bool?(true);
					if (this.EditItem.FontColor == "#FF20C48F")
					{
						this.ForegroundPickerList.SelectedIndex = 0;
					}
					else if (this.EditItem.FontColor == "#FF298FEE")
					{
						this.ForegroundPickerList.SelectedIndex = 1;
					}
					else if (this.EditItem.FontColor == "#FFFF6932")
					{
						this.ForegroundPickerList.SelectedIndex = 2;
					}
					else if (this.EditItem.FontColor == "#FFB80000")
					{
						this.ForegroundPickerList.SelectedIndex = 3;
					}
					else
					{
						CompositeCollection compositeCollection2 = (CompositeCollection)this.colorSelecters.Resources["ColorCollection"];
						compositeCollection2[compositeCollection2.Count - 1] = global::System.Windows.Media.ColorConverter.ConvertFromString(this.EditItem.FontColor);
						this.ForegroundPickerList.SelectedIndex = compositeCollection2.Count - 1;
					}
					this.PreviewImageContainer.StampColor = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(this.EditItem.FontColor);
				}
			}
			this.applying = false;
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x00071B3C File Offset: 0x0006FD3C
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

		// Token: 0x06001BAF RID: 7087 RVA: 0x00071BCF File Offset: 0x0006FDCF
		private void UpdateConfigRemoveBg(string fileName, bool removeBg)
		{
			if (removeBg)
			{
				ConfigManager.AddSignatureRemoveBg(fileName);
				return;
			}
			ConfigManager.RemoveSignatureRemoveBg(fileName);
		}

		// Token: 0x06001BB0 RID: 7088 RVA: 0x00071BE3 File Offset: 0x0006FDE3
		private bool CheckOk()
		{
			return true;
		}

		// Token: 0x06001BB1 RID: 7089 RVA: 0x00071BE8 File Offset: 0x0006FDE8
		private static string[] GetDateFormats()
		{
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<string> list = new List<string>();
			dictionary["US"] = new string[] { "MM/dd/yyyy", "MM/dd/yyyy HH:mm", "MM.dd.yyyy", "MM.dd.yyyy HH:mm" };
			dictionary["GB"] = new string[] { "dd/MM/yyyy", "dd/MM/yyyy", "dd/MM/yyyy HH:mm" };
			dictionary["ES"] = dictionary["GB"];
			dictionary["IT"] = dictionary["GB"];
			dictionary["PT"] = dictionary["GB"];
			dictionary["FR"] = dictionary["GB"];
			dictionary["RU"] = new string[] { "dd.MM.yyyy", "dd.MM.yyyy", "dd.MM.yyyy HH:mm" };
			dictionary["DE"] = dictionary["RU"];
			dictionary["NL"] = new string[] { "dd-MM-yyyy", "dd-MM-yyyy", "dd-MM-yyyy HH:mm" };
			dictionary2["CN"] = new string[] { "yyyy/MM/dd", "yyyy/MM/dd", "yyyy/MM/dd HH:mm", "yyyy-MM-dd", "yyyy-MM-dd HH:mm" };
			dictionary2["ZH-CN"] = dictionary2["CN"];
			dictionary2["JA"] = dictionary2["CN"];
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

		// Token: 0x06001BB2 RID: 7090 RVA: 0x00071EE0 File Offset: 0x000700E0
		private void ForegroundPicker_SelectedColorChanged(object sender, ColorPickerButtonSelectedColorChangedEventArgs e)
		{
			CompositeCollection compositeCollection = (CompositeCollection)this.colorSelecters.Resources["ColorCollection"];
			compositeCollection[compositeCollection.Count - 1] = e.Color;
			this.ForegroundPickerList.SelectedIndex = compositeCollection.Count - 1;
		}

		// Token: 0x06001BB3 RID: 7091 RVA: 0x00071F34 File Offset: 0x00070134
		private void ForegroundPickerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (base.IsLoaded)
			{
				if (e.AddedItems.Count == 0)
				{
					this.ForegroundPickerList.SelectedIndex = 0;
					return;
				}
				this.PreviewImageContainer.StampColor = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(this.ForegroundPickerList.SelectedItem.ToString());
				this.UpdatePreviewImage();
			}
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x00071F90 File Offset: 0x00070190
		private void UpdatePreviewImage()
		{
			if (this.PreviewImageContainer == null || this.applying)
			{
				return;
			}
			DynamicStampProperties dynamicStampProperties = this.PreviewImageContainer.DynamicStampProperties;
			if (dynamicStampProperties == null)
			{
				dynamicStampProperties = new DynamicStampProperties
				{
					FontFamily = this.FontComboBox.SelectedItem.ToString(),
					FontWeight = DynamicStampProperties.FontWeights.Bold,
					FontItalic = false,
					Style = 1,
					Locale = CultureInfoUtils.ActualAppLanguage
				};
			}
			if (this.Content1ComboBox.SelectedIndex == 0)
			{
				dynamicStampProperties.Contents[0].ContentType = DynamicStampProperties.ContentType.Text;
				if (string.IsNullOrEmpty(this.TypeWriterCtrl1.Text.Trim()))
				{
					dynamicStampProperties.Contents[0].Content = pdfeditor.Properties.Resources.WinStampSampleText;
				}
				else
				{
					dynamicStampProperties.Contents[0].Content = this.TypeWriterCtrl1.Text.Trim();
				}
			}
			else
			{
				dynamicStampProperties.Contents[0].ContentType = DynamicStampProperties.ContentType.Time;
				DynamicStampProperties.StampContent stampContent = dynamicStampProperties.Contents[0];
				object selectedItem = this.DateFormatComboBox1.SelectedItem;
				stampContent.Content = ((selectedItem != null) ? selectedItem.ToString() : null);
			}
			if (this.Content2ComboBox.SelectedIndex != 2)
			{
				if (this.Content2ComboBox.SelectedIndex == 0)
				{
					dynamicStampProperties.Contents[1].ContentType = DynamicStampProperties.ContentType.Text;
					dynamicStampProperties.Contents[1].Content = this.TypeWriterCtrl2.Text.Trim();
				}
				else
				{
					dynamicStampProperties.Contents[1].ContentType = DynamicStampProperties.ContentType.Time;
					DynamicStampProperties.StampContent stampContent2 = dynamicStampProperties.Contents[1];
					object selectedItem2 = this.DateFormatComboBox2.SelectedItem;
					stampContent2.Content = ((selectedItem2 != null) ? selectedItem2.ToString() : null);
				}
			}
			else
			{
				dynamicStampProperties.Contents[1].ContentType = DynamicStampProperties.ContentType.None;
			}
			if (this.Content3ComboBox.SelectedIndex != 2)
			{
				if (this.Content3ComboBox.SelectedIndex == 0)
				{
					dynamicStampProperties.Contents[2].ContentType = DynamicStampProperties.ContentType.Text;
					dynamicStampProperties.Contents[2].Content = this.TypeWriterCtrl3.Text.Trim();
				}
				else
				{
					dynamicStampProperties.Contents[2].ContentType = DynamicStampProperties.ContentType.Time;
					DynamicStampProperties.StampContent stampContent3 = dynamicStampProperties.Contents[2];
					object selectedItem3 = this.DateFormatComboBox3.SelectedItem;
					stampContent3.Content = ((selectedItem3 != null) ? selectedItem3.ToString() : null);
				}
			}
			else
			{
				dynamicStampProperties.Contents[2].ContentType = DynamicStampProperties.ContentType.None;
			}
			if (this.PreviewImageContainer.TemplateName == "Chop1")
			{
				for (int i = 0; i < dynamicStampProperties.Contents.Count; i++)
				{
					dynamicStampProperties.Contents[i].TextMargin = 5.0;
				}
			}
			this.PreviewImageContainer.DynamicStampProperties = dynamicStampProperties;
			this.PreviewImageContainer.Refresh();
		}

		// Token: 0x06001BB5 RID: 7093 RVA: 0x00072242 File Offset: 0x00070442
		private void lstBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x00072244 File Offset: 0x00070444
		private void txt_Text_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x0007224C File Offset: 0x0007044C
		private void StackPanel_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = e.Source as RadioButton;
			if (radioButton != null && this.PreviewImageContainer != null)
			{
				this.PreviewImageBorder.Height = 55.0;
				this.TypeWriterCtrl3.AcceptsReturn = false;
				this.TypeWriterCtrl2.AcceptsReturn = false;
				if (radioButton.Name == "ShapeEllipse")
				{
					this.PreviewImageContainer.TemplateName = "Chop1";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Left;
					this.PreviewImageBorder.Height = 200.0;
					this.TypeWriterCtrl3.AcceptsReturn = true;
					this.TypeWriterCtrl2.AcceptsReturn = true;
				}
				else if (radioButton.Name == "ShapeRechangle")
				{
					this.PreviewImageContainer.TemplateName = "Square";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Left;
				}
				else if (radioButton.Name == "ShapeSingleArrow")
				{
					this.PreviewImageContainer.TemplateName = "Arrow1";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Right;
				}
				else if (radioButton.Name == "ShapeSingleReverseArrow")
				{
					this.PreviewImageContainer.TemplateName = "Arrow1";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Left;
				}
				else if (radioButton.Name == "ShapeDoubleArrow")
				{
					this.PreviewImageContainer.TemplateName = "Arrow2";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Right;
				}
				else if (radioButton.Name == "ShapeDoubleReverseArrow")
				{
					this.PreviewImageContainer.TemplateName = "Arrow2";
					this.PreviewImageContainer.DynamicStampProperties.ArrowDirection = DynamicStampProperties.ArrowDirections.Left;
				}
			}
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x00072419 File Offset: 0x00070619
		private void DateFormatComboBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x00072421 File Offset: 0x00070621
		private void DateFormatComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x0007242C File Offset: 0x0007062C
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = e.Source as ComboBox;
			if (comboBox != null && this.PreviewImageContainer != null)
			{
				if (comboBox.SelectedIndex == 1)
				{
					this.PreviewImageContainer.DynamicStampProperties.FontSize = new double?((double)20);
				}
				else if (comboBox.SelectedIndex == 2)
				{
					this.PreviewImageContainer.DynamicStampProperties.FontSize = new double?((double)12);
				}
				else if (comboBox.SelectedIndex == 3)
				{
					this.PreviewImageContainer.DynamicStampProperties.FontSize = new double?((double)28);
				}
				else
				{
					this.PreviewImageContainer.DynamicStampProperties.FontSize = null;
				}
				this.UpdatePreviewImage();
			}
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x000724E0 File Offset: 0x000706E0
		private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = e.Source as ComboBox;
			if (comboBox != null && this.PreviewImageContainer.DynamicStampProperties != null)
			{
				this.PreviewImageContainer.DynamicStampProperties.FontFamily = comboBox.SelectedItem.ToString();
				this.UpdatePreviewImage();
			}
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x0007252C File Offset: 0x0007072C
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
					this.ImageFilePath.Text = this.FileDiaoligFiePath;
					goto IL_009A;
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
			}
			this.ResultModel.ImageFilePath = string.Empty;
			IL_009A:
			base.Activate();
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x000725EC File Offset: 0x000707EC
		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			this.showPicture.Source = null;
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x000725FC File Offset: 0x000707FC
		private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = e.Source as ComboBox;
			if (comboBox != null && comboBox.SelectedIndex == 0 && comboBox.Items.Count < 5 && this.NewCategoryName.Visibility == Visibility.Visible)
			{
				if (string.IsNullOrEmpty(this.NewCategoryName.Text) && this.SaveCheck.IsChecked.GetValueOrDefault())
				{
					this.btnOk.IsEnabled = false;
				}
				if (!string.IsNullOrEmpty(this.NewCategoryName.Text))
				{
					this.NewCategoryName.Focus();
					base.Dispatcher.BeginInvoke(new Action(delegate
					{
						this.NewCategoryName.SelectAll();
					}), Array.Empty<object>());
					return;
				}
			}
			else
			{
				this.btnOk.IsEnabled = true;
			}
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x000726BC File Offset: 0x000708BC
		private void SaveCheck_Checked(object sender, RoutedEventArgs e)
		{
			if (this.CategoryComboBox.SelectedIndex == 0 && string.IsNullOrEmpty(this.NewCategoryName.Text) && this.CategoryComboBox.Items.Count <= 5 && this.NewCategoryName.Visibility == Visibility.Visible)
			{
				this.btnOk.IsEnabled = false;
				return;
			}
			this.btnOk.IsEnabled = true;
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x00072721 File Offset: 0x00070921
		private void SaveCheck_Unchecked(object sender, RoutedEventArgs e)
		{
			this.btnOk.IsEnabled = true;
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x00072730 File Offset: 0x00070930
		private void NewCategoryName_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = e.Source as TextBox;
			if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text) && this.CategoryComboBox.SelectedIndex == 0 && this.CategoryComboBox.Items.Count <= 5)
			{
				this.btnOk.IsEnabled = false;
				return;
			}
			this.btnOk.IsEnabled = true;
		}

		// Token: 0x040009E3 RID: 2531
		private MenuModel SelectedMenuModel;

		// Token: 0x040009E4 RID: 2532
		internal string FileDiaoligFiePath = string.Empty;

		// Token: 0x040009E5 RID: 2533
		private SignatureCreateDialogResult resultModel = new SignatureCreateDialogResult();

		// Token: 0x040009E7 RID: 2535
		private StampTextModel stampText;

		// Token: 0x040009E8 RID: 2536
		private StampTextModel stampPreviewText;

		// Token: 0x040009E9 RID: 2537
		public bool isText = true;

		// Token: 0x040009EA RID: 2538
		public bool isSave;

		// Token: 0x040009EB RID: 2539
		public static readonly DependencyProperty ClearVisibleProperty = DependencyProperty.Register("StampClearVisible", typeof(bool), typeof(StampEditWindows), new PropertyMetadata(false));

		// Token: 0x040009EC RID: 2540
		private CustStampModel EditItem;

		// Token: 0x040009ED RID: 2541
		private bool applying;
	}
}
