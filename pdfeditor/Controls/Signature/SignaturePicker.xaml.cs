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
using pdfeditor.Controls.Annotations;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001FB RID: 507
	public partial class SignaturePicker : Control
	{
		// Token: 0x06001C70 RID: 7280 RVA: 0x00076CD8 File Offset: 0x00074ED8
		static SignaturePicker()
		{
			SignaturePicker.ItemClickEvent = EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Bubble, typeof(EventHandler<SignaturePickerItemClickEventArgs>), typeof(SignaturePicker));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SignaturePicker), new FrameworkPropertyMetadata(typeof(SignaturePicker)));
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x00076D87 File Offset: 0x00074F87
		public SignaturePicker()
		{
			base.Loaded += this.SignatureTemplatePreviewControl_Loaded;
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06001C72 RID: 7282 RVA: 0x00076DA1 File Offset: 0x00074FA1
		// (set) Token: 0x06001C73 RID: 7283 RVA: 0x00076DB3 File Offset: 0x00074FB3
		public ObservableCollection<ImageStampModel> ImageSignaturesTemplates
		{
			get
			{
				return (ObservableCollection<ImageStampModel>)base.GetValue(SignaturePicker.ImageSignaturesTemplatesProperty);
			}
			set
			{
				base.SetValue(SignaturePicker.ImageSignaturesTemplatesProperty, value);
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06001C74 RID: 7284 RVA: 0x00076DC1 File Offset: 0x00074FC1
		// (set) Token: 0x06001C75 RID: 7285 RVA: 0x00076DD3 File Offset: 0x00074FD3
		public bool IsExistTemplate
		{
			get
			{
				return (bool)base.GetValue(SignaturePicker.IsExistTemplateProperty);
			}
			set
			{
				base.SetValue(SignaturePicker.IsExistTemplateProperty, value);
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06001C76 RID: 7286 RVA: 0x00076DE6 File Offset: 0x00074FE6
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x06001C77 RID: 7287 RVA: 0x00076DF4 File Offset: 0x00074FF4
		private void SignatureTemplatePreviewControl_Loaded(object sender, RoutedEventArgs e)
		{
			string text = Path.Combine(AppDataHelper.LocalFolder, "Signature");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			List<string> list = Directory.GetFiles(text).ToList<string>();
			this.IsExistTemplate = list.Count<string>() > 0;
			if (list.Count == 0)
			{
				return;
			}
			ObservableCollection<ImageStampModel> imageSignaturesTemplates = this.ImageSignaturesTemplates;
			if (imageSignaturesTemplates != null)
			{
				imageSignaturesTemplates.Clear();
			}
			List<FileInfo> fileInfos = new List<FileInfo>();
			list.ForEach(delegate(string f)
			{
				fileInfos.Add(new FileInfo(f));
			});
			foreach (FileInfo fileInfo in fileInfos.OrderByDescending((FileInfo f) => f.CreationTime).ToList<FileInfo>().Take(50))
			{
				ImageStampModel imageStampModel = new ImageStampModel
				{
					ImageFilePath = fileInfo.FullName
				};
				try
				{
					ShellFile shellFile = ShellFile.FromFilePath(fileInfo.FullName);
					imageStampModel.StampImageSource = shellFile.Thumbnail.LargeBitmapSource;
					this.ImageSignaturesTemplates.Add(imageStampModel);
				}
				catch
				{
				}
			}
			base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(this.DelayedAction));
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06001C78 RID: 7288 RVA: 0x00076F50 File Offset: 0x00075150
		// (set) Token: 0x06001C79 RID: 7289 RVA: 0x00076F58 File Offset: 0x00075158
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

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06001C7A RID: 7290 RVA: 0x00076FB3 File Offset: 0x000751B3
		// (set) Token: 0x06001C7B RID: 7291 RVA: 0x00076FBB File Offset: 0x000751BB
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
					this.pickerItemContainer = value;
				}
			}
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x00076FD0 File Offset: 0x000751D0
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.AddNewButton = base.GetTemplateChild("AddNewButton") as Button;
			this.PickerItemContainer = base.GetTemplateChild("PickerItemContainer") as ItemsControl;
			this.PickerItemContainer.ItemContainerGenerator.StatusChanged -= this.ItemContainerGenerator_StatusChanged;
			this.PickerItemContainer.ItemContainerGenerator.StatusChanged += this.ItemContainerGenerator_StatusChanged;
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x00077048 File Offset: 0x00075248
		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			if (this.PickerItemContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
			{
				this.PickerItemContainer.ItemContainerGenerator.StatusChanged -= this.ItemContainerGenerator_StatusChanged;
				base.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(this.DelayedAction));
			}
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x000770A0 File Offset: 0x000752A0
		private void DelayedAction()
		{
			for (int i = 0; i < this.PickerItemContainer.Items.Count; i++)
			{
				DependencyObject dependencyObject = this.PickerItemContainer.ItemContainerGenerator.ContainerFromIndex(i);
				Grid grid = SignaturePicker.FindSingleVisualChildren<Grid>(dependencyObject);
				Button button = SignaturePicker.FindSingleVisualChildren<Button>(dependencyObject);
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
			}
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x0007714E File Offset: 0x0007534E
		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.ImageSignaturesTemplates != null && this.ImageSignaturesTemplates.Count >= 50)
			{
				new SignatureSaveNumTipWin().ShowDialog();
				return;
			}
			this.VM.AnnotationToolbar.DoSignatureMenuCmd(null);
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x00077184 File Offset: 0x00075384
		private void DelItemButton_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
			if (frameworkElement != null)
			{
				ImageStampModel imageStampModel = frameworkElement.DataContext as ImageStampModel;
				if (imageStampModel != null)
				{
					string text = Path.Combine(AppDataHelper.LocalFolder, "Signature");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					if (Directory.GetFiles(text).ToList<string>().Contains(imageStampModel.ImageFilePath))
					{
						try
						{
							this.ImageSignaturesTemplates.Remove(imageStampModel);
							string imageFilePath = imageStampModel.ImageFilePath;
							File.Delete(imageFilePath);
							List<string> list = Directory.GetFiles(text).ToList<string>();
							this.IsExistTemplate = list.Count<string>() > 0;
							ConfigManager.RemoveSignatureRemoveBg(imageFilePath);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x0007723C File Offset: 0x0007543C
		private async void PickerItem_Click(object sender, RoutedEventArgs e)
		{
			if (this.VM.Document != null)
			{
				FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
				if (frameworkElement != null)
				{
					ImageStampModel imageStampModel = frameworkElement.DataContext as ImageStampModel;
					if (imageStampModel != null)
					{
						this.RaiseItemClickEvent(imageStampModel);
						e.Handled = true;
						this.VM.AnnotationMode = AnnotationMode.Signature;
						StampImageModel stampImageModel = new StampImageModel
						{
							ImageFilePath = imageStampModel.ImageFilePath,
							IsSignature = true
						};
						await this.VM.AnnotationToolbar.ProcessStampImageModelAsync(stampImageModel);
					}
				}
			}
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x0007727B File Offset: 0x0007547B
		private void UpdateItemsSource()
		{
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x00077280 File Offset: 0x00075480
		private void RaiseItemClickEvent(ImageStampModel item)
		{
			if (item == null)
			{
				return;
			}
			SignaturePickerItemClickEventArgs signaturePickerItemClickEventArgs = new SignaturePickerItemClickEventArgs(this, item);
			base.RaiseEvent(signaturePickerItemClickEventArgs);
		}

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x06001C84 RID: 7300 RVA: 0x000772A0 File Offset: 0x000754A0
		// (remove) Token: 0x06001C85 RID: 7301 RVA: 0x000772AE File Offset: 0x000754AE
		public event EventHandler<SignaturePickerItemClickEventArgs> ItemClick
		{
			add
			{
				base.AddHandler(SignaturePicker.ItemClickEvent, value);
			}
			remove
			{
				base.RemoveHandler(SignaturePicker.ItemClickEvent, value);
			}
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x000772BC File Offset: 0x000754BC
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
					t = SignaturePicker.FindSingleVisualChildren<T>(child);
					if (t != null)
					{
						break;
					}
				}
			}
			return t;
		}

		// Token: 0x04000A72 RID: 2674
		public static readonly DependencyProperty ImageSignaturesTemplatesProperty = DependencyProperty.Register(" ImageSignaturesTemplates", typeof(ObservableCollection<ImageStampModel>), typeof(SignaturePicker), new PropertyMetadata(new ObservableCollection<ImageStampModel>()));

		// Token: 0x04000A73 RID: 2675
		public static readonly DependencyProperty IsExistTemplateProperty = DependencyProperty.Register("IsExistTemplate", typeof(bool), typeof(SignaturePicker), new PropertyMetadata(false));

		// Token: 0x04000A74 RID: 2676
		private Button delItemButton;

		// Token: 0x04000A75 RID: 2677
		private Button addNewButton;

		// Token: 0x04000A76 RID: 2678
		private ItemsControl pickerItemContainer;
	}
}
