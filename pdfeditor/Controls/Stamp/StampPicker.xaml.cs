using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Signature;
using pdfeditor.Models.DynamicStamps;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001ED RID: 493
	public partial class StampPicker : Control
	{
		// Token: 0x06001BEB RID: 7147 RVA: 0x000734E0 File Offset: 0x000716E0
		static StampPicker()
		{
			StampPicker.ItemClickEvent = EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Bubble, typeof(EventHandler<StampPickerItemClickEventArgs>), typeof(StampPicker));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(StampPicker), new FrameworkPropertyMetadata(typeof(StampPicker)));
		}

		// Token: 0x06001BEC RID: 7148 RVA: 0x00073571 File Offset: 0x00071771
		public StampPicker()
		{
			base.Loaded += this.SignatureTemplatePreviewControl_Loaded;
			this.stampModels = new ObservableCollection<CustStampModel>();
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06001BED RID: 7149 RVA: 0x00073596 File Offset: 0x00071796
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06001BEE RID: 7150 RVA: 0x000735A2 File Offset: 0x000717A2
		// (set) Token: 0x06001BEF RID: 7151 RVA: 0x000735B4 File Offset: 0x000717B4
		public string GroupName
		{
			get
			{
				return (string)base.GetValue(StampPicker.GroupNameProperty);
			}
			set
			{
				base.SetValue(StampPicker.GroupNameProperty, value);
			}
		}

		// Token: 0x06001BF0 RID: 7152 RVA: 0x000735C4 File Offset: 0x000717C4
		private void SignatureTemplatePreviewControl_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				string text = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				List<string> list = Directory.GetFiles(text).ToList<string>();
				List<DynamicStampTextModel> list2 = ToolbarContextMenuHelper.ReadDynamicStamp(false);
				if (list.Count == 0 && list2 == null)
				{
					return;
				}
				ObservableCollection<CustStampModel> observableCollection = this.stampModels;
				if (observableCollection != null)
				{
					observableCollection.Clear();
				}
				List<CustStampModel> list3 = new List<CustStampModel>();
				foreach (string text2 in list)
				{
					FileInfo fileInfo = new FileInfo(text2);
					CustStampModel custStampModel = new CustStampModel
					{
						ImageFilePath = fileInfo.FullName,
						Image = "Visible",
						Text = "Collapsed",
						dateTime = fileInfo.CreationTime,
						DynamicStampTextModel = ((list2 != null) ? list2.FirstOrDefault((DynamicStampTextModel x) => x.TemplateName == fileInfo.FullName) : null)
					};
					try
					{
						ShellFile shellFile = ShellFile.FromFilePath(fileInfo.FullName);
						custStampModel.StampImageSource = shellFile.Thumbnail.LargeBitmapSource;
						if (custStampModel.DynamicStampTextModel != null && custStampModel.DynamicStampTextModel.GroupName == this.GroupName)
						{
							list3.Add(custStampModel);
						}
					}
					catch
					{
					}
				}
				if (list2 != null)
				{
					foreach (DynamicStampTextModel dynamicStampTextModel in list2)
					{
						if (dynamicStampTextModel.DynamicProperties != null || !string.IsNullOrEmpty(dynamicStampTextModel.TextContent))
						{
							CustStampModel custStampModel2 = new CustStampModel
							{
								TextContent = dynamicStampTextModel.TextContent,
								FontColor = dynamicStampTextModel.FontColor,
								GroupId = dynamicStampTextModel.GroupId,
								Image = "Collapsed",
								Text = "Visible",
								dateTime = dynamicStampTextModel.dateTime,
								DynamicStampTextModel = dynamicStampTextModel
							};
							if (custStampModel2.DynamicStampTextModel.GroupName == this.GroupName)
							{
								list3.Add(custStampModel2);
							}
						}
					}
				}
				foreach (CustStampModel custStampModel3 in list3.OrderByDescending((CustStampModel x) => x.dateTime).Take(50))
				{
					this.stampModels.Add(custStampModel3);
				}
			}
			catch
			{
			}
			base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(this.DelayedAction));
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06001BF1 RID: 7153 RVA: 0x000738EC File Offset: 0x00071AEC
		// (set) Token: 0x06001BF2 RID: 7154 RVA: 0x000738F4 File Offset: 0x00071AF4
		private Button AddNewButton
		{
			get
			{
				return this.addNewButton;
			}
			set
			{
				if (this.addNewButton != value)
				{
					if (this.addNewButton != null)
					{
						this.addNewButton.Click -= this.AddNewButton_Click;
					}
					this.addNewButton = value;
					if (this.addNewButton != null)
					{
						this.addNewButton.Click += this.AddNewButton_Click;
					}
				}
			}
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06001BF3 RID: 7155 RVA: 0x0007394F File Offset: 0x00071B4F
		// (set) Token: 0x06001BF4 RID: 7156 RVA: 0x00073958 File Offset: 0x00071B58
		private ItemsControl PickerItemContainer
		{
			get
			{
				return this.pickerItemContainer;
			}
			set
			{
				if (this.pickerItemContainer != value)
				{
					if (this.pickerItemContainer != null)
					{
						this.pickerItemContainer.ItemsSource = null;
						this.pickerItemContainer.ItemContainerGenerator.StatusChanged -= this.ItemContainerGenerator_StatusChanged;
					}
					this.pickerItemContainer = value;
					if (this.pickerItemContainer != null)
					{
						this.pickerItemContainer.ItemsSource = this.stampModels;
						this.pickerItemContainer.ItemContainerGenerator.StatusChanged += this.ItemContainerGenerator_StatusChanged;
					}
				}
			}
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x000739DC File Offset: 0x00071BDC
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.AddNewButton = base.GetTemplateChild("AddNewButton") as Button;
			if (this.PickerItemContainer != null)
			{
				this.PickerItemContainer.ItemsSource = null;
				this.PickerItemContainer.ItemContainerGenerator.StatusChanged -= this.ItemContainerGenerator_StatusChanged;
			}
			this.PickerItemContainer = base.GetTemplateChild("PickerItemContainer") as ItemsControl;
			if (this.PickerItemContainer != null)
			{
				this.PickerItemContainer.ItemsSource = this.stampModels;
				this.PickerItemContainer.ItemContainerGenerator.StatusChanged += this.ItemContainerGenerator_StatusChanged;
			}
		}

		// Token: 0x06001BF6 RID: 7158 RVA: 0x00073A80 File Offset: 0x00071C80
		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			if (this.PickerItemContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				this.PickerItemContainer.ItemContainerGenerator.StatusChanged -= this.ItemContainerGenerator_StatusChanged;
				base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(this.DelayedAction));
			}
		}

		// Token: 0x06001BF7 RID: 7159 RVA: 0x00073AD8 File Offset: 0x00071CD8
		public static T FindChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
		{
			FrameworkElement frameworkElement = parent as FrameworkElement;
			if (frameworkElement != null && !frameworkElement.IsLoaded)
			{
				return default(T);
			}
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				T t = child as T;
				if (t != null && t.GetValue(FrameworkElement.NameProperty) as string == name)
				{
					return t;
				}
				T t2 = StampPicker.FindChildByName<T>(child, name);
				if (t2 != null)
				{
					return t2;
				}
			}
			return default(T);
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00073B70 File Offset: 0x00071D70
		private void DelayedAction()
		{
			for (int i = 0; i < this.PickerItemContainer.Items.Count; i++)
			{
				DependencyObject dependencyObject = this.PickerItemContainer.ItemContainerGenerator.ContainerFromIndex(i);
				Grid grid = StampPicker.FindSingleVisualChildren<Grid>(dependencyObject);
				Button button = StampPicker.FindChildByName<Button>(dependencyObject, "DelItemButton");
				Button button2 = StampPicker.FindChildByName<Button>(dependencyObject, "EditItemButton");
				if (grid != null)
				{
					grid.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new RoutedEventHandler(this.PickerItem_Click));
					grid.AddHandler(UIElement.MouseLeftButtonUpEvent, new RoutedEventHandler(this.PickerItem_Click));
				}
				if (button != null)
				{
					button.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(this.DelItemButton_Click));
					button.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(this.DelItemButton_Click));
				}
				if (button2 != null)
				{
					button2.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(this.EditItemButton_Click));
					button2.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(this.EditItemButton_Click));
				}
			}
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x00073C60 File Offset: 0x00071E60
		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.stampModels.Count >= 50)
			{
				new SignatureSaveNumTipWin().ShowDialog();
				return;
			}
			this.VM.AnnotationToolbar.DoStampCmd(null);
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x00073C90 File Offset: 0x00071E90
		private void EditItemButton_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
			if (frameworkElement != null)
			{
				object dataContext = frameworkElement.DataContext;
				CustStampModel item = dataContext as CustStampModel;
				if (item != null)
				{
					GAManager.SendEvent("MainWindow_Stamp", "EditCustomStampBtn", "Count", 1L);
					EditStampWindow editStampWindow = new EditStampWindow(item);
					editStampWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					editStampWindow.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
					editStampWindow.ShowDialog();
					MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
					if (requiredService.Document != null && editStampWindow.isSave)
					{
						List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
						if (item.Image == "Visible")
						{
							list.Remove(list.Find((DynamicStampTextModel x) => x.TemplateName == item.ImageFilePath));
						}
						else
						{
							list.Remove(list.Find((DynamicStampTextModel x) => x.GroupId == item.GroupId));
						}
						string text = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
						if (!Directory.Exists(text))
						{
							Directory.CreateDirectory(text);
						}
						string text2 = Path.Combine(text, "Stamp.json");
						try
						{
							using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.ReadWrite))
							{
								using (StreamWriter streamWriter = new StreamWriter(fileStream))
								{
									string text3 = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
									{
										TypeNameHandling = TypeNameHandling.Auto
									});
									streamWriter.Write(text3);
									streamWriter.Close();
								}
								fileStream.Close();
							}
							requiredService.AnnotationToolbar.ReflashStampList();
						}
						catch (Exception)
						{
						}
						requiredService.AnnotationToolbar.SaveStamp(editStampWindow.StampTextModel);
					}
				}
			}
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x00073E70 File Offset: 0x00072070
		private void DelItemButton_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
			if (frameworkElement != null)
			{
				object dataContext = frameworkElement.DataContext;
				CustStampModel item = dataContext as CustStampModel;
				if (item != null)
				{
					GAManager.SendEvent("MainWindow_Stamp", "DeleteCustomStampBtn", "Count", 1L);
					MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
					List<DynamicStampTextModel> list = ToolbarContextMenuHelper.ReadDynamicStamp(false);
					if (item.Image == "Visible")
					{
						if (item != null)
						{
							list.Remove(list.Find((DynamicStampTextModel x) => x.TemplateName == item.ImageFilePath));
							this.stampModels.Remove(item);
						}
						string text = Path.Combine(AppDataHelper.LocalFolder, "Stamp");
						if (!Directory.Exists(text))
						{
							Directory.CreateDirectory(text);
						}
						if (!Directory.GetFiles(text).ToList<string>().Contains(item.ImageFilePath))
						{
							goto IL_014B;
						}
						try
						{
							string imageFilePath = item.ImageFilePath;
							item = null;
							File.Delete(imageFilePath);
							Directory.GetFiles(text).ToList<string>();
							ConfigManager.RemoveSignatureRemoveBg(imageFilePath);
							goto IL_014B;
						}
						catch (Exception)
						{
							goto IL_014B;
						}
					}
					list.Remove(list.Find((DynamicStampTextModel x) => x.GroupId == item.GroupId));
					this.stampModels.Remove(item);
					IL_014B:
					string text2 = Path.Combine(AppDataHelper.LocalCacheFolder, "Config");
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					string text3 = Path.Combine(text2, "Stamp.json");
					try
					{
						using (FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.ReadWrite))
						{
							using (StreamWriter streamWriter = new StreamWriter(fileStream))
							{
								string text4 = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
								{
									TypeNameHandling = TypeNameHandling.Auto
								});
								streamWriter.Write(text4);
								streamWriter.Close();
							}
							fileStream.Close();
						}
						if (this.stampModels.Count <= 0)
						{
							requiredService.AnnotationToolbar.ReflashStampList();
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x06001BFC RID: 7164 RVA: 0x000740A8 File Offset: 0x000722A8
		private async void PickerItem_Click(object sender, RoutedEventArgs e)
		{
			if (this.VM.Document != null)
			{
				FrameworkElement frameworkElement = e.Source as FrameworkElement;
				if (frameworkElement != null)
				{
					CustStampModel custStampModel = frameworkElement.DataContext as CustStampModel;
					if (custStampModel != null)
					{
						GAManager.SendEvent("PdfStampAnnotation", "DoStamp", "Custom", 1L);
						if (custStampModel.Image == "Visible")
						{
							this.RaiseItemClickEvent(custStampModel);
							e.Handled = true;
							this.VM.AnnotationMode = AnnotationMode.Stamp;
							StampImageModel stampImageModel = new StampImageModel
							{
								ImageFilePath = custStampModel.ImageFilePath
							};
							await this.VM.AnnotationToolbar.ProcessStampImageModelAsync(stampImageModel);
						}
						else
						{
							e.Handled = true;
							this.VM.AnnotationMode = AnnotationMode.Stamp;
							if (custStampModel.DynamicStampTextModel.DynamicProperties != null)
							{
								DynamicStampTextModel dynamicStampTextModel = custStampModel.DynamicStampTextModel;
								await this.VM.AnnotationToolbar.ProcessStampTextModelAsync(dynamicStampTextModel);
							}
							else
							{
								DynamicStampProperties dynamicStampProperties = new DynamicStampProperties();
								dynamicStampProperties.FontWeight = DynamicStampProperties.FontWeights.Bold;
								dynamicStampProperties.FontItalic = false;
								dynamicStampProperties.Style = 1;
								dynamicStampProperties.Locale = CultureInfoUtils.ActualAppLanguage;
								dynamicStampProperties.Contents[0].ContentType = DynamicStampProperties.ContentType.Text;
								dynamicStampProperties.Contents[0].Content = custStampModel.TextContent;
								DynamicStampTextModel dynamicStampTextModel2 = new DynamicStampTextModel(dynamicStampProperties, custStampModel.FontColor, "Square", custStampModel.GroupId);
								await this.VM.AnnotationToolbar.ProcessStampTextModelAsync(dynamicStampTextModel2);
							}
						}
					}
				}
			}
			this.VM.AnnotationMode = AnnotationMode.None;
		}

		// Token: 0x06001BFD RID: 7165 RVA: 0x000740E7 File Offset: 0x000722E7
		private void UpdateItemsSource()
		{
		}

		// Token: 0x06001BFE RID: 7166 RVA: 0x000740EC File Offset: 0x000722EC
		private void RaiseItemClickEvent(CustStampModel item)
		{
			if (item == null)
			{
				return;
			}
			ImageStampModel imageStampModel = new ImageStampModel
			{
				ImageFilePath = item.ImageFilePath,
				StampImageSource = item.StampImageSource,
				ImageHeight = item.ImageHeight,
				ImageWidth = item.ImageWidth,
				PageSize = item.PageSize
			};
			StampPickerItemClickEventArgs stampPickerItemClickEventArgs = new StampPickerItemClickEventArgs(this, imageStampModel);
			base.RaiseEvent(stampPickerItemClickEventArgs);
		}

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x06001BFF RID: 7167 RVA: 0x0007414E File Offset: 0x0007234E
		// (remove) Token: 0x06001C00 RID: 7168 RVA: 0x0007415C File Offset: 0x0007235C
		public event EventHandler<StampPickerItemClickEventArgs> ItemClick
		{
			add
			{
				base.AddHandler(StampPicker.ItemClickEvent, value);
			}
			remove
			{
				base.RemoveHandler(StampPicker.ItemClickEvent, value);
			}
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x0007416C File Offset: 0x0007236C
		public static T FindSingleVisualChildren<T>(DependencyObject parentObj) where T : DependencyObject
		{
			T t = default(T);
			if (parentObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(parentObj, i);
					if (child != null && child is T)
					{
						t = child as T;
						break;
					}
					t = StampPicker.FindSingleVisualChildren<T>(child);
					if (t != null)
					{
						break;
					}
				}
			}
			return t;
		}

		// Token: 0x04000A1D RID: 2589
		private ObservableCollection<CustStampModel> stampModels;

		// Token: 0x04000A1E RID: 2590
		public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(StampPicker), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			if (s is StampPicker)
			{
				object newValue = a.NewValue;
				object oldValue = a.OldValue;
			}
		}));

		// Token: 0x04000A1F RID: 2591
		private Button delItemButton;

		// Token: 0x04000A20 RID: 2592
		private Button addNewButton;

		// Token: 0x04000A21 RID: 2593
		private Button editItemButton;

		// Token: 0x04000A22 RID: 2594
		private ItemsControl pickerItemContainer;
	}
}
