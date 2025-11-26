using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using CommonLib.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.AutoSaveRestore;
using pdfeditor.Controls;
using pdfeditor.Controls.Bookmarks;
using pdfeditor.Controls.Copilot;
using pdfeditor.Controls.DigitalSignatures;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.Menus.ToolbarSettings;
using pdfeditor.Controls.Screenshots;
using pdfeditor.Controls.Translation;
using pdfeditor.Models;
using pdfeditor.Models.Attachments;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Views
{
	// Token: 0x0200004C RID: 76
	public partial class MainView : Window
	{
		// Token: 0x0600036C RID: 876 RVA: 0x0001098C File Offset: 0x0000EB8C
		public MainView()
		{
			base.DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
			this.InitializeComponent();
			this.PdfControl.Viewer.IsVisibleChanged += this.Viewer_IsVisibleChanged;
			base.Loaded += this.MainView_Loaded;
			WSUtils.LoadWindowInfo(null);
			this.micaHelper = MicaHelper.Create(this);
			if (this.micaHelper != null)
			{
				this.micaHelper.IsMicaEnabled = true;
				this.TitlebarPlaceholder.Visibility = Visibility.Visible;
				this.micaHelper.TitlebarPlaceholder = this.TitlebarPlaceholder;
			}
			else
			{
				base.Background = new SolidColorBrush(Colors.White);
			}
			LaunchUtils.LaunchActionInvoked += this.LaunchUtils_LaunchActionInvoked;
			this.InitViewerBackgroundColorValues();
			this.PdfControl.Viewer.IsFillFormHighlighted = ConfigManager.GetIsFillFormHighlightedFlag();
			this.PdfControl.Viewer.DeferredSigningPreviewText = pdfeditor.Properties.Resources.ResourceManager.GetString("DeferredDigitSignPreviewText");
			this.InitViewerThemeValues();
			this.SetFooterVisible(ConfigManager.GetLaunchAPPShowFlag("LaunchStatusbar"));
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00010A9C File Offset: 0x0000EC9C
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
			if (hwndSource != null)
			{
				hwndSource.AddHook(new HwndSourceHook(MainView.<OnSourceInitialized>g__WndProc|6_0));
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00010AD4 File Offset: 0x0000ECD4
		private void LaunchUtils_LaunchActionInvoked(PdfDocument sender, LaunchActionInvokedEventArgs args)
		{
			string action = args.Action;
			if (action == "tab:fillform")
			{
				GAManager.SendEvent("ToolbarAction", "FillForm", "LaunchAction", 1L);
				this.Menus.SelectedItem = this.VM.Menus.MainMenus.FirstOrDefault((MainMenuGroup c) => c.Tag == "FillForm");
				return;
			}
			if (action == "new:CreatedBlankFile")
			{
				this.VM.CreatePdfFileAsync(CreatePdfFileType.Blank);
				return;
			}
			if (action == "open:CreatedFile")
			{
				this.VM.DocumentWrapper.SetUntitledFile();
				this.VM.SetCanSaveFlag("CreateNew", false);
				pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().LastOperationVersion = "CreateNew";
				return;
			}
			if (!(action == "new:CreatedFileFromScanner"))
			{
				return;
			}
			GAManager.SendEvent("ToolbarAction", "NewScannerPDF", "LaunchAction", 1L);
			this.VM.CreatePdfFileAsync(CreatePdfFileType.FromScanner);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00010BD8 File Offset: 0x0000EDD8
		private void Viewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AppHotKeyManager appHotKeyManager = App.Current.AppHotKeyManager;
			if (appHotKeyManager == null)
			{
				return;
			}
			appHotKeyManager.UpdateHotKeyEnabledStates();
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00010BF0 File Offset: 0x0000EDF0
		public void Menus_SelectItem(string tag)
		{
			if (this.Menus == null || this.Menus.Items.Count <= 0)
			{
				return;
			}
			foreach (object obj in ((IEnumerable)this.Menus.Items))
			{
				MainMenuGroup mainMenuGroup = (MainMenuGroup)obj;
				if (mainMenuGroup != null && mainMenuGroup.Tag == tag)
				{
					this.Menus.SelectedItem = mainMenuGroup;
					return;
				}
			}
			this.Menus.SelectedIndex = 0;
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000371 RID: 881 RVA: 0x00010C90 File Offset: 0x0000EE90
		protected MainViewModel VM
		{
			get
			{
				return base.DataContext as MainViewModel;
			}
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00010CA0 File Offset: 0x0000EEA0
		private async void MainView_Loaded(object sender, RoutedEventArgs e)
		{
			this.Menus_SelectItem("View");
			this.UpdateBookmarkWrapMode();
			await this.VM.OpenStartUpFileCmd.ExecuteAsync(null);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00010CD7 File Offset: 0x0000EED7
		private void VM_SelectedPageIndexChanged(object sender, EventArgs e)
		{
			this.PdfControl.ScrollToPage(this.VM.SelectedPageIndex);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00010CF0 File Offset: 0x0000EEF0
		private void ThumbnailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			if (listBox != null)
			{
				try
				{
					IList selectedItems = listBox.SelectedItems;
					this.VM.SelectedThumbnailList = selectedItems.Cast<PdfThumbnailModel>().ToList<PdfThumbnailModel>();
				}
				catch
				{
				}
			}
			if (e.AddedItems.OfType<PdfThumbnailModel>().FirstOrDefault<PdfThumbnailModel>() != null)
			{
				if (this.ThumbnailList.SelectedItems.Count == 1)
				{
					this.ThumbnailList.BringSelectedIndexIntoView();
				}
				if (this.PagesEditorContainer.Visibility == Visibility.Collapsed)
				{
					int selectedIndex = ((PdfPagePreviewListView)sender).SelectedIndex;
					if (selectedIndex != -1)
					{
						PageEditorViewModel pageEditors = this.VM.PageEditors;
						if (((pageEditors != null) ? pageEditors.PageEditListItemSource : null) != null && selectedIndex < this.VM.PageEditors.PageEditListItemSource.Count)
						{
							PageEditorViewModel pageEditors2 = this.VM.PageEditors;
							PdfPageEditListModel pdfPageEditListModel = ((pageEditors2 != null) ? pageEditors2.PageEditListItemSource[selectedIndex] : null);
							this.PageGridView.ScrollIntoView(pdfPageEditListModel);
						}
					}
				}
			}
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00010DE8 File Offset: 0x0000EFE8
		private void PageNum_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBoxBase tb = sender as TextBoxBase;
			if (tb != null)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
				{
					tb.SelectAll();
					if (Keyboard.FocusedElement != tb)
					{
						Keyboard.Focus(tb);
					}
				}));
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00010E2C File Offset: 0x0000F02C
		private void ZoomCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				ComboBox comboBox = sender as ComboBox;
				if (comboBox != null && comboBox.SelectedValue != null)
				{
					string text = comboBox.SelectedValue.ToString();
					this.UpdateDocZoom(text);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00010E78 File Offset: 0x0000F078
		private void ZoomCB_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				try
				{
					string text = "";
					ComboBox comboBox = sender as ComboBox;
					if (comboBox != null && !string.IsNullOrWhiteSpace(comboBox.Text))
					{
						text = comboBox.Text;
					}
					if (string.IsNullOrWhiteSpace(text))
					{
						TextBox textBox = sender as TextBox;
						if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
						{
							text = textBox.Text;
						}
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						this.UpdateDocZoom(text);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00010F00 File Offset: 0x0000F100
		private void ZoomCB_LostFocus(object sender, RoutedEventArgs e)
		{
			BindingExpression bindingExpression = BindingOperations.GetBindingExpression((DependencyObject)sender, ComboBox.TextProperty);
			if (bindingExpression != null)
			{
				bindingExpression.UpdateTarget();
			}
			BindingExpression bindingExpression2 = BindingOperations.GetBindingExpression((DependencyObject)sender, TextBox.TextProperty);
			if (bindingExpression2 == null)
			{
				return;
			}
			bindingExpression2.UpdateTarget();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00010F38 File Offset: 0x0000F138
		private void UpdateDocZoom(string zoomStr)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(zoomStr))
				{
					string[] array = zoomStr.Split(new char[] { '%' });
					if (!string.IsNullOrWhiteSpace(array[0]))
					{
						if (zoomStr == pdfeditor.Properties.Resources.ShortcutTextFullSize)
						{
							this.VM.ViewToolbar.DocSizeMode = SizeModes.Zoom;
							this.VM.ViewToolbar.UpdateDocToZoom(1f, false, null);
						}
						else if (zoomStr == pdfeditor.Properties.Resources.ShortcutTextFitPage)
						{
							this.VM.ViewToolbar.DocSizeMode = SizeModes.FitToSize;
						}
						else if (zoomStr == pdfeditor.Properties.Resources.ShortcutTextFitWidth)
						{
							this.VM.ViewToolbar.DocSizeMode = SizeModes.FitToWidth;
						}
						else if (zoomStr == pdfeditor.Properties.Resources.ShortcutTextFitHeight)
						{
							this.VM.ViewToolbar.DocSizeMode = SizeModes.FitToHeight;
						}
						else
						{
							int num = 0;
							try
							{
								num = Convert.ToInt32(array[0]);
							}
							catch
							{
							}
							if (num > 0)
							{
								float num2 = (float)num / 100f;
								this.VM.ViewToolbar.UpdateDocToZoom(num2, false, null);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00011088 File Offset: 0x0000F288
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			this.isClosed = true;
			LaunchUtils.LaunchActionInvoked -= this.LaunchUtils_LaunchActionInvoked;
			ComparisonWindow comparisonWindow = Application.Current.Windows.OfType<ComparisonWindow>().FirstOrDefault<ComparisonWindow>();
			if (comparisonWindow != null)
			{
				comparisonWindow.Close();
			}
			AppSettingsViewModel service = Ioc.Default.GetService<AppSettingsViewModel>();
			if (service != null && service.LanguageChangedFlag)
			{
				FileWatcherHelper.Instance.TryRestart();
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000110F8 File Offset: 0x0000F2F8
		protected override async void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!this.isReadyToClose)
			{
				e.Cancel = true;
				if (!this.isClosing)
				{
					this.isClosing = true;
					WSUtils.SaveWindowInfo(null);
					string currentOpenedFile = this.VM.DocumentWrapper.DocumentPath;
					await this.VM.ReleaseViewerFocusAsync(true);
					if (this.VM.CanSave)
					{
						if (this.VM.ViewToolbar.IsDocumentEdited)
						{
							GAManager.SendEvent("TextEditor2", "ExitEditing", "CloseBtnDocEdited", 1L);
						}
						TaskAwaiter<bool> taskAwaiter = this.VM.TrySaveBeforeCloseDocumentAsync().GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (taskAwaiter.GetResult())
						{
							RateUtils.CheckAndShowRate(currentOpenedFile);
							base.Hide();
							await this.VM.AnnotationToolbar.SaveToolbarSettingsConfigAsync();
							await this.VM.CloseDocumentAsync();
							await DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
							{
								this.isReadyToClose = true;
								this.VM.DelAutoSaveFile(currentOpenedFile);
								this.Close();
							}));
						}
						else
						{
							this.isClosing = false;
						}
					}
					else
					{
						if (this.VM.ViewToolbar.IsDocumentEdited)
						{
							GAManager.SendEvent("TextEditor2", "ExitEditing", "CloseBtnDocNotEdited", 1L);
						}
						RateUtils.CheckAndShowRate(currentOpenedFile);
						base.Hide();
						await this.VM.AnnotationToolbar.SaveToolbarSettingsConfigAsync();
						await this.VM.CloseDocumentAsync();
						this.isReadyToClose = true;
						this.isClosing = false;
						this.VM.DelAutoSaveFile(currentOpenedFile);
						base.Close();
					}
				}
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00011138 File Offset: 0x0000F338
		protected override async void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (!(e.OriginalSource is TextBoxBase))
			{
				if (e.Key == Key.Escape)
				{
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
					if (this.AnnotationEditorCanvas.TextObjectHolder.SelectedObject != null)
					{
						e.Handled = true;
						this.AnnotationEditorCanvas.TextObjectHolder.CancelTextObject();
					}
					else if (this.AnnotationEditorCanvas.HolderManager.CurrentHolder != null)
					{
						e.Handled = true;
						if (this.VM.AnnotationMode == AnnotationMode.Link)
						{
							ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = this.VM.AnnotationToolbar.LinkButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel;
							if (toolbarChildCheckableButtonModel != null)
							{
								TypedContextMenuModel typedContextMenuModel = toolbarChildCheckableButtonModel.ContextMenu as TypedContextMenuModel;
								if (typedContextMenuModel != null)
								{
									ContextMenuItemModel contextMenuItemModel = typedContextMenuModel[0] as ContextMenuItemModel;
									if (contextMenuItemModel != null)
									{
										contextMenuItemModel.Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png"));
									}
								}
							}
							pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
							if (this.VM.SelectedAnnotation != null)
							{
								this.VM.SelectedAnnotation = null;
							}
							this.VM.AnnotationMode = AnnotationMode.None;
						}
						this.AnnotationEditorCanvas.HolderManager.CancelAll();
					}
					else if (this.VM.AnnotationMode != AnnotationMode.None)
					{
						e.Handled = true;
						if (this.VM.AnnotationMode == AnnotationMode.Link)
						{
							ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel2 = this.VM.AnnotationToolbar.LinkButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel;
							if (toolbarChildCheckableButtonModel2 != null)
							{
								TypedContextMenuModel typedContextMenuModel2 = toolbarChildCheckableButtonModel2.ContextMenu as TypedContextMenuModel;
								if (typedContextMenuModel2 != null)
								{
									ContextMenuItemModel contextMenuItemModel2 = typedContextMenuModel2[0] as ContextMenuItemModel;
									if (contextMenuItemModel2 != null)
									{
										contextMenuItemModel2.Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png"));
									}
								}
							}
							pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
							if (this.VM.SelectedAnnotation != null)
							{
								this.VM.SelectedAnnotation = null;
							}
						}
						else if (this.VM.AnnotationMode == AnnotationMode.Ink)
						{
							ToolbarSettingInkEraserModel toolbarSettingInkEraserModel = this.VM.AnnotationToolbar.InkButtonModel.ToolbarSettingModel[1] as ToolbarSettingInkEraserModel;
							if (toolbarSettingInkEraserModel != null && toolbarSettingInkEraserModel.IsChecked)
							{
								toolbarSettingInkEraserModel.IsChecked = false;
							}
						}
						this.VM.AnnotationMode = AnnotationMode.None;
					}
					else if (FullScreenHelper.GetIsFullScreenEnabled(this))
					{
						e.Handled = true;
						FullScreenHelper.SetIsFullScreenEnabled(this, false);
						this.FullScreenToolbarToggleButton_Click(this.FullScreenButton, null);
					}
					else if (this.VM.ViewToolbar != null)
					{
						this.VM.ViewToolbar.StopAutoScroll();
					}
				}
				else
				{
					if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None)
					{
						if (this.VM.SelectedAnnotation != null)
						{
							e.Handled = true;
							await this.VM.DeleteSelectedAnnotCmd.ExecuteAsync(null);
						}
						else if (this.AnnotationEditorCanvas.TextObjectHolder.SelectedObject != null)
						{
							GAManager.SendEvent("TextEditor", "DeleteSelectedObject", "DeleteKey", 1L);
							e.Handled = true;
							await this.AnnotationEditorCanvas.TextObjectHolder.DeleteSelectedObjectAsync();
						}
						global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
						AnnotationCanvas annotCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl2);
						if (annotCanvas != null && annotCanvas.ImageControl.Visibility == Visibility.Visible)
						{
							if (ModernMessageBox.Show(pdfeditor.Properties.Resources.ImageControl_DeleteConfirm, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
							{
								return;
							}
							PdfImageObject pdfImageObject = this.VM.Document.Pages[annotCanvas.ImageControl.Pageindex].PageObjects[annotCanvas.ImageControl.imageindex] as PdfImageObject;
							await annotCanvas.ImageControl.DeleteImageCmd(pdfImageObject, annotCanvas.ImageControl.Pageindex);
							annotCanvas.ImageControl.Visibility = Visibility.Collapsed;
						}
						annotCanvas = null;
					}
					if (Keyboard.Modifiers == ModifierKeys.Control)
					{
						if (this.PdfControl.IsEditing && (e.Key == Key.O || e.Key == Key.P || e.Key == Key.S))
						{
							e.Handled = true;
							return;
						}
						if (e.Key == Key.C && !string.IsNullOrEmpty(this.PdfControl.Viewer.SelectedText))
						{
							try
							{
								e.Handled = true;
								Clipboard.SetDataObject(this.PdfControl.Viewer.SelectedText);
							}
							catch
							{
							}
						}
						if (e.Key == Key.A && this.PdfControl.Document != null && !this.PdfControl.IsEditing)
						{
							e.Handled = true;
							if (this.PagesEditorContainer.Visibility == Visibility.Visible)
							{
								MainViewModel vm = this.VM;
								bool flag;
								if (vm == null)
								{
									flag = null != null;
								}
								else
								{
									PageEditorViewModel pageEditors = vm.PageEditors;
									flag = ((pageEditors != null) ? pageEditors.PageEditListItemSource : null) != null;
								}
								if (flag)
								{
									this.VM.PageEditors.PageEditListItemSource.AllItemSelected = new bool?(true);
								}
							}
							else
							{
								this.PdfControl.Viewer.SelectText(0, 0, this.PdfControl.Document.Pages.Count - 1, this.PdfControl.Document.Pages[this.PdfControl.Document.Pages.Count - 1].Text.CountChars);
							}
						}
						if (e.Key == Key.Up && this.PdfControl.Document != null)
						{
							e.Handled = true;
							if (this.VM.CurrnetPageIndex > 0)
							{
								this.VM.CurrnetPageIndex--;
							}
						}
						if (e.Key == Key.Down && this.PdfControl.Document != null)
						{
							e.Handled = true;
							if (this.VM.CurrnetPageIndex <= this.VM.Document.Pages.Count)
							{
								this.VM.CurrnetPageIndex++;
							}
						}
					}
					if (Keyboard.Modifiers == ModifierKeys.None)
					{
						if (e.Key == Key.F11 && !e.IsRepeat)
						{
							e.Handled = true;
							this.FullScreenButton.IsChecked = !this.FullScreenButton.IsChecked;
							this.EnterOrExitFullScreen();
						}
						if (!this.PdfControl.IsEditing && string.IsNullOrEmpty(this.PdfControl.Viewer.FillForms.FocusedText))
						{
							if (e.Key == Key.Left && this.PdfControl.Document != null && (this.PdfControl.ScrollViewer == null || this.PdfControl.ScrollViewer.HorizontalOffset == 0.0))
							{
								e.Handled = true;
								if (this.VM.SelectedPageIndex > 0)
								{
									this.VM.SelectedPageIndex--;
								}
							}
							if (e.Key == Key.Right && this.PdfControl.Document != null && (this.PdfControl.ScrollViewer == null || this.PdfControl.ScrollViewer.HorizontalOffset == this.PdfControl.ScrollViewer.ScrollableWidth))
							{
								e.Handled = true;
								if (this.VM.SelectedPageIndex < this.VM.Document.Pages.Count - 1)
								{
									this.VM.SelectedPageIndex++;
								}
							}
							if (e.Key == Key.Home && this.PdfControl.Document != null)
							{
								e.Handled = true;
								this.VM.SelectedPageIndex = 0;
							}
							if (e.Key == Key.End && this.PdfControl.Document != null)
							{
								e.Handled = true;
								this.VM.SelectedPageIndex = this.VM.TotalPagesCount - 1;
							}
							if (e.Key == Key.F5 && this.PdfControl.Document != null && this.PdfControl.Viewer.IsVisible)
							{
								e.Handled = true;
								this.VM.ViewToolbar.Present();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00011178 File Offset: 0x0000F378
		public async void ToolbarCropScreenShot(RadioButton btn, int[] pages = null)
		{
			this.VM.ExitTransientMode(true, false, false, false, false);
			if (this.VM.Document == null)
			{
				btn.IsChecked = new bool?(false);
			}
			else if (btn.IsChecked.GetValueOrDefault())
			{
				this.VM.AnnotationMode = AnnotationMode.None;
				await this.VM.ReleaseViewerFocusAsync(true);
				ScreenshotDialogMode screenshotDialogMode = ScreenshotDialogMode.CropPage;
				this.AnnotationEditorCanvas.CloseScreenShot();
				btn.IsChecked = new bool?(true);
				await this.AnnotationEditorCanvas.StartScreenShotAsync(screenshotDialogMode, pages);
			}
			else
			{
				this.AnnotationEditorCanvas.CloseScreenShot();
			}
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000111C0 File Offset: 0x0000F3C0
		public async void ToolbarScreenShotButton_Click(object sender, RoutedEventArgs e)
		{
			this.SearchBox.CloseSearchBox();
			this.VM.ExitTransientMode(true, false, false, false, false);
			RadioButton btn = (RadioButton)sender;
			if (this.VM.Document == null)
			{
				btn.IsChecked = new bool?(false);
			}
			else if (btn.IsChecked.GetValueOrDefault())
			{
				this.VM.AnnotationMode = AnnotationMode.None;
				await this.VM.ReleaseViewerFocusAsync(true);
				bool flag = false;
				string text = "";
				ScreenshotDialogMode screenshotDialogMode = ScreenshotDialogMode.Screenshot;
				if (sender == this.screenshotBtn)
				{
					screenshotDialogMode = ScreenshotDialogMode.Screenshot;
					GAManager.SendEvent("MainWindow", "ScreenshotBtn", "Count", 1L);
					flag = true;
					text = pdfeditor.Properties.Resources.ScreenshotTip;
				}
				else if (sender == this.cropPageBtn)
				{
					screenshotDialogMode = ScreenshotDialogMode.CropPage;
					GAManager.SendEvent("PageView", "CropPage", "Main", 1L);
				}
				if (flag && !ConfigManager.GetDoNotShowFlag("NotShowScreenshotTipFlag", false))
				{
					bool? checkboxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
					{
						Content = text,
						ShowLeftBottomCheckbox = true,
						LeftBottomCheckboxContent = pdfeditor.Properties.Resources.WinPwdPasswordSaveTipNotshowagainContent
					}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false).CheckboxResult;
					if (checkboxResult != null && checkboxResult.GetValueOrDefault())
					{
						ConfigManager.SetDoNotShowFlag("NotShowScreenshotTipFlag", true);
					}
				}
				this.AnnotationEditorCanvas.CloseScreenShot();
				btn.IsChecked = new bool?(true);
				await this.AnnotationEditorCanvas.StartScreenShotAsync(screenshotDialogMode, null);
			}
			else
			{
				GAManager.SendEvent("CropPage", "CancelCropPageBtn", "Count", 1L);
				this.AnnotationEditorCanvas.CloseScreenShot();
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x000111FF File Offset: 0x0000F3FF
		private void AnnotationEditorCanvas_ScreenshotDialogClosed(object sender, EventArgs e)
		{
			this.screenshotBtn.IsChecked = new bool?(false);
			this.cropPageBtn.IsChecked = new bool?(false);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00011224 File Offset: 0x0000F424
		private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateMenuScrollStates();
			MainMenuGroup mainMenuGroup = e.AddedItems.OfType<MainMenuGroup>().FirstOrDefault<MainMenuGroup>();
			if (mainMenuGroup != null)
			{
				MainViewModel vm = this.VM;
				if (((vm != null) ? vm.Document : null) != null)
				{
					if (mainMenuGroup.Tag == "Page" && this.PagesEditorContainer.Visibility == Visibility.Collapsed)
					{
						this.VM.ExitTransientMode(false, false, false, false, false);
						this.AnnotationEditorCanvas.HolderManager.CancelAll();
						this.menuFooterShow.Visibility = Visibility.Collapsed;
						this.PagesEditorContainer.Visibility = Visibility.Visible;
						this.PagesEditorFooterContainer.Visibility = Visibility.Visible;
						this.FooterContainer.Visibility = Visibility.Collapsed;
						this.PdfContentContainer.Visibility = Visibility.Collapsed;
						this.FooterContainer.Visibility = Visibility.Collapsed;
					}
					else if ((mainMenuGroup.Tag == "View" || mainMenuGroup.Tag == "Annotate" || mainMenuGroup.Tag == "FillForm" || mainMenuGroup.Tag == "Insert" || mainMenuGroup.Tag == "Tools" || mainMenuGroup.Tag == "Protection") && this.PdfContentContainer.Visibility == Visibility.Collapsed)
					{
						this.menuFooterShow.Visibility = Visibility.Visible;
						this.PagesEditorContainer.Visibility = Visibility.Collapsed;
						this.PagesEditorFooterContainer.Visibility = Visibility.Collapsed;
						this.FooterContainer.Visibility = Visibility.Visible;
						this.PdfContentContainer.Visibility = Visibility.Visible;
						this.FooterContainer.Visibility = Visibility.Visible;
					}
					GAManager.SendEvent("ToolbarAction", mainMenuGroup.Tag, "SelectionChanged", 1L);
					return;
				}
				if (mainMenuGroup.Tag == "Insert" || mainMenuGroup.Tag == "Annotate" || mainMenuGroup.Tag == "FillForm" || mainMenuGroup.Tag == "Page" || mainMenuGroup.Tag == "Protection" || mainMenuGroup.Tag == "Share" || mainMenuGroup.Tag == "Tools")
				{
					((Selector)sender).SelectedItem = e.RemovedItems.OfType<MainMenuGroup>().FirstOrDefault<MainMenuGroup>() ?? this.VM.Menus.MainMenus[0];
					return;
				}
			}
			else
			{
				((Selector)sender).SelectedItem = e.RemovedItems.OfType<MainMenuGroup>().FirstOrDefault<MainMenuGroup>() ?? this.VM.Menus.MainMenus[0];
			}
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000114B8 File Offset: 0x0000F6B8
		public void ShowPdfViewerView()
		{
			MainMenuGroup mainMenuGroup = this.Menus.SelectedItem as MainMenuGroup;
			if (mainMenuGroup != null && mainMenuGroup.Tag == "Page")
			{
				this.Menus_SelectItem("View");
			}
			this.menuFooterShow.Visibility = Visibility.Visible;
			this.PagesEditorContainer.Visibility = Visibility.Collapsed;
			this.PagesEditorFooterContainer.Visibility = Visibility.Collapsed;
			this.FooterContainer.Visibility = Visibility.Visible;
			this.PdfContentContainer.Visibility = Visibility.Visible;
			this.FooterContainer.Visibility = Visibility.Visible;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001153E File Offset: 0x0000F73E
		private void SearchToolbarButton_Click(object sender, RoutedEventArgs e)
		{
			this.ShowSearchBox();
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00011548 File Offset: 0x0000F748
		public bool ShowSearchBox()
		{
			this.VM.ExitTransientMode(false, false, false, false, false);
			if (!this.VM.ViewToolbar.EditDocumentButtomModel.IsChecked && this.VM.Menus.SearchModel != null && this.VM.Menus.SearchModel.IsSearchEnabled)
			{
				string selectedText = this.PdfControl.Viewer.SelectedText;
				if (!string.IsNullOrEmpty(selectedText) && selectedText.Length < 200)
				{
					this.VM.Menus.SearchModel.SearchText = selectedText;
				}
				if (!this.VM.Menus.SearchModel.IsSearchVisible)
				{
					this.VM.Menus.SearchModel.IsSearchVisible = true;
				}
				else
				{
					this.SearchBox.Focus();
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00011625 File Offset: 0x0000F825
		private void MenuScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.UpdateMenuScrollStates();
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00011630 File Offset: 0x0000F830
		private void UpdateMenuScrollStates()
		{
			StackPanel[] array = new StackPanel[] { this.ExitAnnotPanel1, this.ExitAnnotPanel2 };
			foreach (StackPanel stackPanel in array)
			{
				TranslateTransform translateTransform = stackPanel.RenderTransform as TranslateTransform;
				if (translateTransform == null)
				{
					translateTransform = new TranslateTransform();
					stackPanel.RenderTransform = translateTransform;
				}
				translateTransform.X = this.MenuScrollViewer.HorizontalOffset;
			}
			if (this.MenuScrollViewer.HorizontalOffset == 0.0)
			{
				this.MenuScrollLeftMask.Visibility = Visibility.Collapsed;
			}
			else
			{
				double num = 0.0;
				MainMenuGroup mainMenuGroup = this.Menus.SelectedItem as MainMenuGroup;
				if (mainMenuGroup != null && (mainMenuGroup.Tag == "View" || mainMenuGroup.Tag == "Annotate"))
				{
					num = array.Select((StackPanel c) => c.ActualWidth).Max();
				}
				this.MenuScrollLeftMask.Margin = new Thickness(num - 2.0, 0.0, 0.0, 0.0);
				this.MenuScrollLeftMask.Visibility = Visibility.Visible;
			}
			if (this.MenuScrollViewer.HorizontalOffset > this.MenuScrollViewer.ScrollableWidth - 5.0)
			{
				this.MenuScrollRightMask.Visibility = Visibility.Collapsed;
				return;
			}
			this.MenuScrollRightMask.Visibility = Visibility.Visible;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x000117B4 File Offset: 0x0000F9B4
		private void MenuScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (((ScrollViewer)sender).ScrollableWidth > 0.0)
			{
				double horizontalOffset = ((ScrollViewer)sender).HorizontalOffset;
				MouseTiltEventArgs mouseTiltEventArgs = e as MouseTiltEventArgs;
				int num;
				if (mouseTiltEventArgs != null)
				{
					num = mouseTiltEventArgs.Delta / 2;
				}
				else
				{
					num = -e.Delta / 2;
				}
				((ScrollViewer)sender).ScrollToHorizontalOffset(horizontalOffset + (double)num);
			}
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00011814 File Offset: 0x0000FA14
		private void MenuNavigationButton_Click(object sender, RoutedEventArgs e)
		{
			int num = 40;
			if (((FrameworkElement)sender).HorizontalAlignment == HorizontalAlignment.Left)
			{
				num = -num;
			}
			double horizontalOffset = this.MenuScrollViewer.HorizontalOffset;
			this.MenuScrollViewer.ScrollToHorizontalOffset(horizontalOffset + (double)num);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00011850 File Offset: 0x0000FA50
		private async void Viewer_LostFocus(object sender, RoutedEventArgs e)
		{
			IInputElement focusedElement = Keyboard.FocusedElement;
			if (focusedElement != this.PdfControl.Editor && focusedElement != this.PdfControl.Viewer)
			{
				ScrollViewer scrollViewer = focusedElement as ScrollViewer;
				if (scrollViewer == null || (scrollViewer.Content != this.PdfControl.Editor && scrollViewer.Content != this.PdfControl.Viewer))
				{
					FrameworkElement frameworkElement = focusedElement as FrameworkElement;
					if (frameworkElement == null || !MainView.<Viewer_LostFocus>g__IsChild|35_0<AnnotationCanvas>(frameworkElement))
					{
						await this.VM.ReleaseViewerFocusAsync(false);
					}
				}
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00011888 File Offset: 0x0000FA88
		private void PagesEditorContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta != 0 && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
			{
				e.Handled = true;
				if (e.Delta < 0)
				{
					this.PagesEditorThumbnailScaleSlider.Value -= 0.1;
					return;
				}
				this.PagesEditorThumbnailScaleSlider.Value += 0.1;
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x000118F0 File Offset: 0x0000FAF0
		private void PagesEditorCheckboxButton_Click(object sender, RoutedEventArgs e)
		{
			PageEditorViewModel pageEditors = this.VM.PageEditors;
			if (((pageEditors != null) ? pageEditors.PageEditListItemSource : null) == null)
			{
				return;
			}
			bool? flag = null;
			PageEditorViewModel pageEditors2 = this.VM.PageEditors;
			bool? flag2 = ((pageEditors2 != null) ? pageEditors2.PageEditListItemSource.AllItemSelected : null);
			if (flag2 != null)
			{
				if (flag2.GetValueOrDefault())
				{
					flag = new bool?(false);
				}
				else
				{
					flag = new bool?(true);
				}
			}
			else
			{
				flag = new bool?(true);
			}
			this.VM.PageEditors.PageEditListItemSource.AllItemSelected = flag;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00011988 File Offset: 0x0000FB88
		private void CommetMenuContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 130.0)
			{
				this.CommetExpandButtonContainer.Visibility = Visibility.Visible;
				return;
			}
			this.CommetExpandButtonContainer.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x000119C7 File Offset: 0x0000FBC7
		private void CommetExpandButton_Click(object sender, RoutedEventArgs e)
		{
			this.CommetMenuControl.ExpandAll();
		}

		// Token: 0x0600038D RID: 909 RVA: 0x000119D4 File Offset: 0x0000FBD4
		private void CommetCollapseButton_Click(object sender, RoutedEventArgs e)
		{
			this.CommetMenuControl.CollapseAll();
		}

		// Token: 0x0600038E RID: 910 RVA: 0x000119E1 File Offset: 0x0000FBE1
		private void MenuGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.VM.Menus.IsShowToolbar = false;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x000119F4 File Offset: 0x0000FBF4
		private void MenuGroup_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.VM.Menus.IsShowToolbar = true;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00011A08 File Offset: 0x0000FC08
		private void FooterShowHideButton_Click(object sender, RoutedEventArgs e)
		{
			this.VM.Menus.IsShowFooter = !this.VM.Menus.IsShowFooter;
			if (this.VM.Menus.IsShowFooter)
			{
				Grid.SetRowSpan(this.PdfContentContainer, 1);
				Panel.SetZIndex(this.FooterContainer, 0);
				this.FooterContainerHairline.Visibility = Visibility.Visible;
				return;
			}
			Grid.SetRowSpan(this.PdfContentContainer, 2);
			Panel.SetZIndex(this.FooterContainer, -1);
			this.FooterContainerHairline.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00011A93 File Offset: 0x0000FC93
		public void SetFooterVisible(bool value)
		{
			if (value == this.VM.Menus.IsShowFooter)
			{
				return;
			}
			this.FooterShowHideButton_Click(null, null);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00011AB1 File Offset: 0x0000FCB1
		private void FullScreenToolbarToggleButton_Click(object sender, RoutedEventArgs e)
		{
			this.EnterOrExitFullScreen();
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00011ABC File Offset: 0x0000FCBC
		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
			{
				e.Handled = true;
				if (e.Delta < 0)
				{
					this.PdfScrollViewerByMouseWheel(false);
				}
				else if (e.Delta > 0)
				{
					this.PdfScrollViewerByMouseWheel(true);
				}
			}
			if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
			{
				e.Handled = true;
				MouseTiltEventArgs mouseTiltEventArgs = e as MouseTiltEventArgs;
				int num;
				if (mouseTiltEventArgs != null)
				{
					num = mouseTiltEventArgs.Delta / 2;
				}
				else
				{
					num = -e.Delta / 2;
				}
				double horizontalOffset = this.PdfControl.ScrollViewer.HorizontalOffset;
				this.PdfControl.ScrollViewer.ScrollToHorizontalOffset(horizontalOffset + (double)num);
			}
			if (!e.Handled && this.PdfControl.ScrollViewer.VerticalOffset < 1.0)
			{
				MainViewModel vm = this.VM;
				if (vm == null)
				{
					return;
				}
				AnnotationToolbarViewModel annotationToolbar = vm.AnnotationToolbar;
				if (annotationToolbar == null)
				{
					return;
				}
				annotationToolbar.UpdateViewerToobarPadding();
			}
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00011B90 File Offset: 0x0000FD90
		private void PdfScrollViewerByMouseWheel(bool zoomin)
		{
			float docZoom = this.VM.ViewToolbar.DocZoom;
			int num = 9;
			this.VM.ViewToolbar.DocSizeMode = SizeModes.Zoom;
			int num2 = 0;
			while (num2 < 25 && (float)MouseWheelZoomStepModel.MouseWheelZoomStepModels[num2] <= docZoom * 100f)
			{
				num = num2;
				num2++;
			}
			if (zoomin)
			{
				if (docZoom >= 64f)
				{
					return;
				}
				this.VM.ViewToolbar.UpdateDocToZoom((float)MouseWheelZoomStepModel.MouseWheelZoomStepModels[num + 1] / 100f, false, null);
				return;
			}
			else
			{
				if ((double)docZoom <= 0.01)
				{
					return;
				}
				this.VM.ViewToolbar.UpdateDocToZoom((float)MouseWheelZoomStepModel.MouseWheelZoomStepModels[num - 1] / 100f, false, null);
				return;
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00011C60 File Offset: 0x0000FE60
		private void PdfViewerScrollViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			if (e.IsInertial)
			{
				return;
			}
			Vector scale = e.DeltaManipulation.Scale;
			if (scale.X != 1.0 || scale.Y != 1.0)
			{
				e.Handled = true;
				double num = (double)this.VM.ViewToolbar.DocZoom + scale.Length - 1.4142135623730951;
				this.VM.ViewToolbar.UpdateDocToZoom((float)num, true, new Point?(e.ManipulationOrigin));
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00011CF0 File Offset: 0x0000FEF0
		private void PageGridView_ItemDoubleClick(object sender, PdfPagePreviewGridViewItemEventArgs e)
		{
			PdfPageEditListModel pdfPageEditListModel = e.Item as PdfPageEditListModel;
			if (pdfPageEditListModel != null)
			{
				this.VM.SelectedPageIndex = pdfPageEditListModel.PageIndex;
				this.Menus_SelectItem("View");
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00011D28 File Offset: 0x0000FF28
		private async void PageGridView_ItemsDragStart(object sender, PdfPagePreviewGridViewItemDragStartEventArgs e)
		{
			PdfPageEditListModel pdfPageEditListModel = e.DragContainer.DataContext as PdfPageEditListModel;
			if (pdfPageEditListModel != null)
			{
				WriteableBitmap writeableBitmap = await Ioc.Default.GetRequiredService<PdfThumbnailService>().TryGetPdfBitmapAsync(pdfPageEditListModel.Document.Pages[pdfPageEditListModel.PageIndex], Colors.White, PageRotate.Normal, 0, 100, default(CancellationToken));
				double width = writeableBitmap.Width;
				double height = writeableBitmap.Height;
				int num = Math.Min(e.DragItems.Length, 3);
				Grid grid = new Grid();
				for (int i = 0; i < num; i++)
				{
					Border border = new Border();
					border.Width = width;
					border.Height = height;
					border.Background = new SolidColorBrush(Colors.White);
					border.BorderBrush = new SolidColorBrush(Colors.Black);
					border.BorderThickness = new Thickness(1.0);
					border.Margin = new Thickness((double)(i * 10), (double)(i * 10), 0.0, 0.0);
					if (i == num - 1)
					{
						border.Child = new Image
						{
							Source = writeableBitmap
						};
					}
					grid.Children.Add(border);
				}
				e.UIOverride = grid;
			}
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00011D60 File Offset: 0x0000FF60
		private void PageGridView_ItemsDragCompleted(object sender, PdfPagePreviewGridViewItemDragCompletedEventArgs e)
		{
			MainViewModel vm = this.VM;
			if (((vm != null) ? vm.PageEditors : null) != null && e.Reordered)
			{
				MainViewModel vm2 = this.VM;
				if (vm2 == null)
				{
					return;
				}
				PageEditorViewModel pageEditors = vm2.PageEditors;
				if (pageEditors == null)
				{
					return;
				}
				PdfPageEditListModel pdfPageEditListModel = e.BeforeItem as PdfPageEditListModel;
				PdfPageEditListModel pdfPageEditListModel2 = e.AfterItem as PdfPageEditListModel;
				object[] dragItems = e.DragItems;
				pageEditors.ReorderPages(pdfPageEditListModel, pdfPageEditListModel2, (dragItems != null) ? dragItems.OfType<PdfPageEditListModel>().ToArray<PdfPageEditListModel>() : null);
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00011DD0 File Offset: 0x0000FFD0
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			MainViewModel vm = this.VM;
			if (vm == null)
			{
				return;
			}
			AnnotationToolbarViewModel annotationToolbar = vm.AnnotationToolbar;
			if (annotationToolbar == null)
			{
				return;
			}
			annotationToolbar.UpdateViewerToobarPadding();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00011DEC File Offset: 0x0000FFEC
		private void BookmarkExpandAll_Click(object sender, RoutedEventArgs e)
		{
			this.bookmarkControl.ExpandAll();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00011DF9 File Offset: 0x0000FFF9
		private void BookmarkCollapseAll_Click(object sender, RoutedEventArgs e)
		{
			this.bookmarkControl.CollapseAll();
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00011E08 File Offset: 0x00010008
		private void bookmarkControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				e.Handled = true;
				this.VM.BookmarkRemoveCommand.Execute(null);
				return;
			}
			if (e.Key == Key.Insert)
			{
				e.Handled = true;
				this.VM.BookmarkAddCommand.Execute(null);
			}
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00011E5C File Offset: 0x0001005C
		private void bookmarkControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
			BookmarkModel bookmarkModel = ((frameworkElement != null) ? frameworkElement.DataContext : null) as BookmarkModel;
			if (bookmarkModel != null)
			{
				this.bookmarkContextMenuData = bookmarkModel;
			}
			else
			{
				this.bookmarkContextMenuData = null;
			}
			if (this.bookmarkContextMenuData != null)
			{
				this.BookmarkMenuSeparator.Visibility = Visibility.Visible;
				this.AddChildBookmarkMenuItem.Visibility = Visibility.Visible;
				this.EditBookmarkMenuItem.Visibility = Visibility.Visible;
				this.DeleteBookmarkMenuItem.Visibility = Visibility.Visible;
				return;
			}
			this.BookmarkMenuSeparator.Visibility = Visibility.Collapsed;
			this.AddChildBookmarkMenuItem.Visibility = Visibility.Collapsed;
			this.EditBookmarkMenuItem.Visibility = Visibility.Collapsed;
			this.DeleteBookmarkMenuItem.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00011F04 File Offset: 0x00010104
		private async void BookmarkContextMenuAddBookmark_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("Bookmark", "AddBookmark", "BookmarkContextMenu", 1L);
			AsyncRelayCommand<BookmarkModel> bookmarkAddCommand = this.VM.BookmarkAddCommand;
			BookmarkModel bookmarkModel;
			if ((bookmarkModel = this.bookmarkContextMenuData) == null)
			{
				BookmarkModel bookmarks = this.VM.Bookmarks;
				bookmarkModel = ((bookmarks != null) ? bookmarks.Children.LastOrDefault<BookmarkModel>() : null);
			}
			await bookmarkAddCommand.ExecuteAsync(bookmarkModel);
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00011F3C File Offset: 0x0001013C
		private async void BookmarkContextMenuAddChidBookmark_Click(object sender, RoutedEventArgs e)
		{
			BookmarkModel bookmarkModel = this.bookmarkContextMenuData;
			if (bookmarkModel != null)
			{
				await this.VM.BookmarkAddChildCommand.ExecuteAsync(bookmarkModel);
			}
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00011F74 File Offset: 0x00010174
		private async void BookmarkContextMenuDeleteBookmark_Click(object sender, RoutedEventArgs e)
		{
			BookmarkModel bookmarkModel = this.bookmarkContextMenuData;
			if (bookmarkModel != null)
			{
				await this.VM.BookmarkRemoveCommand.ExecuteAsync(bookmarkModel);
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00011FAC File Offset: 0x000101AC
		private void BookmarkContextMenuRenameBookmark_Click(object sender, RoutedEventArgs e)
		{
			BookmarkModel bookmarkModel = this.bookmarkContextMenuData;
			if (bookmarkModel != null)
			{
				GAManager.SendEvent("Bookmark", "RenameBookmark", "BookmarkContextMenu", 1L);
				BookmarkRenameDialog.Create(bookmarkModel).ShowDialog();
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00011FE8 File Offset: 0x000101E8
		private void WrapBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("Bookmark", "RenameBookmark", "BookmarkContextMenu", 1L);
			MenuItem menuItem = (MenuItem)sender;
			base.Dispatcher.InvokeAsync(delegate
			{
				ConfigManager.SetBookmarkWrapFlag(menuItem.IsChecked);
				this.UpdateBookmarkWrapMode();
			}, DispatcherPriority.Loaded);
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00012040 File Offset: 0x00010240
		private void UpdateBookmarkWrapMode()
		{
			bool bookmarkWrapFlag = ConfigManager.GetBookmarkWrapFlag();
			this.WrapBookmarkMenuItem.IsChecked = bookmarkWrapFlag;
			ScrollViewer.SetHorizontalScrollBarVisibility(this.bookmarkControl, bookmarkWrapFlag ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00012071 File Offset: 0x00010271
		private void PdfControl_EditorUndoStateChanged(object sender, EventArgs e)
		{
			this.VM.UpdateCanSaveFlagState();
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0001207E File Offset: 0x0001027E
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (Keyboard.FocusedElement == null)
			{
				base.Focus();
			}
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00012098 File Offset: 0x00010298
		private async void DigitalSignatureBanner_ShowButtonClick(object sender, EventArgs e)
		{
			if (this.VM.Document != null)
			{
				await this.VM.Menus.ShowLeftNavMenuAsync("DigitalSignatures");
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000120D0 File Offset: 0x000102D0
		private void ChatButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("MainWindowChatButton", "Click", "Count", 1L);
			GAManager.SendEvent("ChatPdf", "ChatAgainButton", "Count", 1L);
			this.VM.ChatPanelVisible = true;
			this.ChatPanel.FocusUserInputTextBox();
			ConfigManager.SetChatPanelClosed(false);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00012128 File Offset: 0x00010328
		private void ChatPanel_CloseButtonClick(object sender, EventArgs e)
		{
			if (!ConfigManager.GetChatPanelFirstClose())
			{
				ConfigManager.SetChatPanelFirstClose(true);
				this.ChatButton.ShowTips();
				this.RightNavigationView.IsAnimationEnabled = false;
			}
			this.VM.ChatPanelVisible = false;
			this.RightNavigationView.IsAnimationEnabled = true;
			ConfigManager.SetChatPanelClosed(true);
			if (this.VM.TranslatePanelState)
			{
				this.VM.TranslatePanelVisible = true;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00012190 File Offset: 0x00010390
		private void BackgroundModeButton_Click(object sender, RoutedEventArgs e)
		{
			BackgroundModel result = ConfigManager.GetBackgroundModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (result == null)
			{
				this.PaperColorListBox.SelectedIndex = 0;
				this.BackgroundModeMenu.IsOpen = true;
				return;
			}
			for (int i = 0; i < MainView.viewerBackgroundColorValues.Length; i++)
			{
				if (result.BackgroundName == MainView.viewerBackgroundColorValues[i].Name)
				{
					this.PaperColorListBox.SelectedIndex = i;
					this.BackgroundModeMenu.IsOpen = true;
					return;
				}
			}
			this.PaperColorListBox.SelectedIndex = 0;
			this.BackgroundModeMenu.IsOpen = true;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00012234 File Offset: 0x00010434
		private void InitViewerThemeValues()
		{
			ListBoxItem listBoxItem = new ListBoxItem();
			listBoxItem.Content = pdfeditor.Properties.Resources.ToolAppThemeAutoMode;
			listBoxItem.Tag = "Auto";
			ListBoxItem listBoxItem2 = new ListBoxItem();
			listBoxItem2.Content = pdfeditor.Properties.Resources.ToolAppThemeLightMode;
			listBoxItem2.Tag = "Light";
			ListBoxItem listBoxItem3 = new ListBoxItem();
			listBoxItem3.Content = pdfeditor.Properties.Resources.ToolAppThemeDarkMode;
			listBoxItem3.Tag = "Dark";
			this.ThemeListBox.Items.Add(listBoxItem);
			this.ThemeListBox.Items.Add(listBoxItem2);
			this.ThemeListBox.Items.Add(listBoxItem3);
			this.UpdateViewerThemeValues();
		}

		// Token: 0x060003AB RID: 939 RVA: 0x000122D4 File Offset: 0x000104D4
		internal void UpdateViewerThemeValues()
		{
			string currentAppTheme = ConfigManager.GetCurrentAppTheme();
			if (currentAppTheme == "Auto")
			{
				this.ThemeListBox.SelectedIndex = 0;
				return;
			}
			if (currentAppTheme == "Light")
			{
				this.ThemeListBox.SelectedIndex = 1;
				return;
			}
			this.ThemeListBox.SelectedIndex = 2;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00012328 File Offset: 0x00010528
		private void InitViewerBackgroundColorValues()
		{
			string text = ((App.Current.GetCurrentActualAppTheme() == "Dark") ? "#444444" : "#E2E2E2");
			MainView.viewerBackgroundColorValues = new BackgroundColorSetting[]
			{
				new BackgroundColorSetting("Default", "", pdfeditor.Properties.Resources.WinViewToolBackgroundDefaultText, text, "#00FFFFFF"),
				new BackgroundColorSetting("DayMode", "", pdfeditor.Properties.Resources.WinViewToolBackgroundDayModeText, "#FCFCFC", "#00FFFFFF"),
				new BackgroundColorSetting("NightMode", "", pdfeditor.Properties.Resources.WinViewToolBackgroundNightModeText, "#CECECE", "#400F0F0F"),
				new BackgroundColorSetting("EyeProtectionMode", "", pdfeditor.Properties.Resources.WinViewToolBackgroundEyeProtectionModeText, "#D2E2C8", "#404B7430"),
				new BackgroundColorSetting("YellowBackground", "", pdfeditor.Properties.Resources.WinViewToolBackgroundYellowBackgroundText, "#E4DDC4", "#40775F13")
			};
			MainView.viewerBackgroundColorDict = MainView.viewerBackgroundColorValues.ToDictionary((BackgroundColorSetting c) => c.Name, (BackgroundColorSetting c) => c);
			foreach (BackgroundColorSetting backgroundColorSetting in MainView.viewerBackgroundColorValues)
			{
				ListBoxItem listBoxItem = new ListBoxItem();
				listBoxItem.Content = backgroundColorSetting.DisplayName;
				listBoxItem.Tag = backgroundColorSetting.Name;
				this.PaperColorListBox.Items.Add(listBoxItem);
			}
			BackgroundModel result = ConfigManager.GetBackgroundModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (result == null)
			{
				this.PaperColorListBox.SelectedIndex = 0;
				return;
			}
			for (int j = 0; j < MainView.viewerBackgroundColorValues.Length; j++)
			{
				if (result.BackgroundName == MainView.viewerBackgroundColorValues[j].Name)
				{
					this.PaperColorListBox.SelectedIndex = j;
					return;
				}
			}
			this.PaperColorListBox.SelectedIndex = 0;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00012518 File Offset: 0x00010718
		private void DoBackgroundMenuItemCmd()
		{
			if (this.PaperColorListBox == null || this.PaperColorListBox.SelectedIndex == -1)
			{
				return;
			}
			BackgroundColorSetting backgroundColorSetting = MainView.viewerBackgroundColorValues[this.PaperColorListBox.SelectedIndex];
			if (backgroundColorSetting != null && backgroundColorSetting.Name == "Default")
			{
				string text = "#E2E2E2";
				if (App.Current.GetCurrentActualAppTheme() == "Dark")
				{
					text = "#444444";
				}
				backgroundColorSetting.BackgroundColor = (Color)ColorConverter.ConvertFromString(text);
			}
			this.TryUpdateViewerBackground();
			BackgroundColorSetting setting = MainView.viewerBackgroundColorValues[this.PaperColorListBox.SelectedIndex];
			if (setting != null)
			{
				ConfigManager.SetBackgroundAsync(setting.Name, setting.PageMaskColor.ToString(), setting.BackgroundColor.ToString());
				if (this.VM.ViewToolbar != null)
				{
					IContextMenuModel contextMenuModel = this.VM.ViewToolbar.BackgroundMenuItems.ToList<IContextMenuModel>().Find((IContextMenuModel x) => x.Name.Equals(setting.Name));
					if (contextMenuModel != null)
					{
						BackgroundContextMenuItemModel backgroundContextMenuItemModel = contextMenuModel as BackgroundContextMenuItemModel;
						if (backgroundContextMenuItemModel != null)
						{
							backgroundContextMenuItemModel.IsChecked = true;
						}
					}
				}
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00012658 File Offset: 0x00010858
		public void TryUpdateViewerBackground()
		{
			if (this.VM.Document == null)
			{
				return;
			}
			BackgroundColorSetting backgroundColorSetting = MainView.viewerBackgroundColorValues[this.PaperColorListBox.SelectedIndex];
			if (backgroundColorSetting != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
				if (pdfControl != null)
				{
					pdfControl.PageMaskBrush = new SolidColorBrush(backgroundColorSetting.PageMaskColor);
					pdfControl.PageBackground = new SolidColorBrush(backgroundColorSetting.BackgroundColor);
				}
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000126BE File Offset: 0x000108BE
		private void PaperColorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.DoBackgroundMenuItemCmd();
			this.BackgroundModeMenu.IsOpen = false;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000126D4 File Offset: 0x000108D4
		private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.ThemeListBox == null || this.ThemeListBox.SelectedIndex == -1)
			{
				return;
			}
			string text = (e.AddedItems[0] as ListBoxItem).Tag.ToString();
			if (text == "Light")
			{
				ConfigManager.SetCurrentAppTheme("Light");
			}
			else if (text == "Dark")
			{
				ConfigManager.SetCurrentAppTheme("Dark");
			}
			else
			{
				ConfigManager.SetCurrentAppTheme("Auto");
				new SystemThemeListener(base.Dispatcher);
			}
			ThemeResourceDictionary.GetForCurrentApp().Theme = App.Current.GetCurrentActualAppTheme();
			BackgroundColorSetting backgroundColorSetting = MainView.viewerBackgroundColorValues[this.PaperColorListBox.SelectedIndex];
			if (backgroundColorSetting != null && backgroundColorSetting.Name == "Default")
			{
				this.DoBackgroundMenuItemCmd();
			}
			ProcessMessageHelper.SendMessageAsync(0UL, "UpdateTheme");
			this.BackgroundModeMenu.IsOpen = false;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000127B8 File Offset: 0x000109B8
		private void ThemeListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			DependencyObject dependencyObject = (DependencyObject)e.OriginalSource;
			while (dependencyObject != null && !(dependencyObject is ListBoxItem))
			{
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			ListBoxItem listBoxItem = dependencyObject as ListBoxItem;
			if (listBoxItem != null && listBoxItem.IsSelected)
			{
				this.BackgroundModeMenu.IsOpen = false;
			}
			ListBoxItem listBoxItem2 = dependencyObject as ListBoxItem;
			if (listBoxItem2 != null)
			{
				GAManager.SendEvent("MainWindow", "UpdateTheme", listBoxItem2.Tag.ToString(), 1L);
			}
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001282C File Offset: 0x00010A2C
		private void MenuHeaderContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width > 0.0 && this.menuHeaderMaxWidth == 0.0)
			{
				MainView.SetIsHeaderMenuCompactModeEnabled(this.MenuHeaderScrollViewer, true);
				this.MenuHeaderScrollViewer.InvalidateMeasure();
				this.MenuHeaderScrollViewer.Measure(new Size(double.MaxValue, e.NewSize.Height));
				this.menuHeaderMaxWidth = this.MenuHeaderScrollViewer.DesiredSize.Width;
			}
			double num = this.menuHeaderMaxWidth;
			MainView.SetIsHeaderMenuCompactModeEnabled(this.MenuHeaderScrollViewer, e.NewSize.Width <= num);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000128E0 File Offset: 0x00010AE0
		public static bool GetIsHeaderMenuCompactModeEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(MainView.IsHeaderMenuCompactModeEnabledProperty);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000128F2 File Offset: 0x00010AF2
		public static void SetIsHeaderMenuCompactModeEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(MainView.IsHeaderMenuCompactModeEnabledProperty, value);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00012905 File Offset: 0x00010B05
		private void SelectTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.AnnotationFilterbtn.IsChecked = new bool?(false);
				this.UsersFilterbtn.IsChecked = new bool?(true);
				this.selectTimer.Dispose();
			});
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x00012922 File Offset: 0x00010B22
		private void SelectTimer_Elapsed2(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.UsersFilterbtn.IsChecked = new bool?(false);
				this.AnnotationFilterbtn.IsChecked = new bool?(true);
				this.selectTimer.Dispose();
			});
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00012940 File Offset: 0x00010B40
		private void UsersFilterbtn_MouseEnter(object sender, MouseEventArgs e)
		{
			if ((sender as ToggleButton).IsChecked.GetValueOrDefault())
			{
				return;
			}
			this.selectTimer = new global::System.Timers.Timer(500.0);
			this.selectTimer.Elapsed += this.SelectTimer_Elapsed;
			this.selectTimer.Start();
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00012999 File Offset: 0x00010B99
		private void UsersFilterbtn_MouseLeave(object sender, MouseEventArgs e)
		{
			global::System.Timers.Timer timer = this.selectTimer;
			if (timer != null)
			{
				timer.Stop();
			}
			this.selectTimer.Dispose();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000129B8 File Offset: 0x00010BB8
		private void AnnotationFilterbtn_MouseEnter(object sender, MouseEventArgs e)
		{
			if ((sender as ToggleButton).IsChecked.GetValueOrDefault())
			{
				return;
			}
			this.selectTimer = new global::System.Timers.Timer(500.0);
			this.selectTimer.Elapsed += this.SelectTimer_Elapsed2;
			this.selectTimer.Start();
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00012A11 File Offset: 0x00010C11
		private void UsersFilterbtn_Checked(object sender, RoutedEventArgs e)
		{
			this.AnnotationFilterbtn.IsChecked = new bool?(false);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00012A24 File Offset: 0x00010C24
		private void AnnotationFilterbtn_Checked(object sender, RoutedEventArgs e)
		{
			this.UsersFilterbtn.IsChecked = new bool?(false);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00012A37 File Offset: 0x00010C37
		private void ExpandCollapseBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.InvokeAsync(delegate
			{
				if (this.bookmarkControl.CheckIfCollapseAll())
				{
					this.bookmarkControl.CollapseAll();
					return;
				}
				this.bookmarkControl.ExpandAll();
			}, DispatcherPriority.Loaded);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00012A54 File Offset: 0x00010C54
		private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				PdfPagePreviewListViewItem parent = VisualHelper.GetParent<PdfPagePreviewListViewItem>((DependencyObject)sender);
				if (parent != null && parent.IsSelected)
				{
					e.Handled = true;
				}
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00012A88 File Offset: 0x00010C88
		private void EnterOrExitFullScreen()
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate
			{
				bool isFullScreenEnabled = FullScreenHelper.GetIsFullScreenEnabled(this);
				this.SetFooterVisible(!isFullScreenEnabled);
				this.VM.Menus.IsShowToolbar = !isFullScreenEnabled;
				if (this.VM.Menus.IsShowToolbar)
				{
					this.FullScreenButton.ToolTip = pdfeditor.Properties.Resources.HeaderToolFullScreenTips;
					return;
				}
				this.FullScreenButton.ToolTip = pdfeditor.Properties.Resources.HeaderToolExitFullScreenTips;
			}));
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00012AA4 File Offset: 0x00010CA4
		private void ZoomCombobox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				ComboBoxItem parent = VisualHelper.GetParent<ComboBoxItem>((DependencyObject)e.OriginalSource);
				if (parent == null)
				{
					return;
				}
				if (parent.Content == comboBox.SelectedValue && comboBox.SelectedValue.ToString() != comboBox.Text)
				{
					this.UpdateDocZoom(comboBox.SelectedValue.ToString());
				}
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00012B07 File Offset: 0x00010D07
		private void NeedRecognitionBanner_RecognitionRequested(object sender, EventArgs e)
		{
			GAManager.SendEvent("OCR", "BannerOCRBtn", "Count", 1L);
			this.VM.ViewToolbar.ConvertToSearchableDocumentButtonModel.Tap();
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00012B34 File Offset: 0x00010D34
		private void SearchBox_RecognitionRequested(object sender, EventArgs e)
		{
			GAManager.SendEvent("OCR", "SearchBoxOCRBtn", "Count", 1L);
			this.VM.ViewToolbar.ConvertToSearchableDocumentButtonModel.Tap();
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00012B64 File Offset: 0x00010D64
		private void PdfViewerContextMenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.VM.FileBtnIsChecked = false;
			string name = (sender as PdfViewerContextMenuItem).Name;
			if (!string.IsNullOrEmpty(name))
			{
				GAManager.SendEvent("FileMenu", name, "Count", 1L);
			}
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00012BA3 File Offset: 0x00010DA3
		private void FileSelectTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				if (this.menuItem1 != null)
				{
					this.menuItem1.IsSubmenuOpen = true;
				}
			});
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00012BC0 File Offset: 0x00010DC0
		private void PdfViewerContextMenuItem_MouseLeave(object sender, MouseEventArgs e)
		{
			global::System.Timers.Timer timer = this.selectTimer;
			if (timer != null)
			{
				timer.Stop();
			}
			this.selectTimer.Dispose();
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00012BE0 File Offset: 0x00010DE0
		private async void PdfViewerContextMenuItem_MouseEnter(object sender, MouseEventArgs e)
		{
			this.menuItem1 = sender as PdfViewerContextMenuItem;
			this.selectTimer = new global::System.Timers.Timer(500.0);
			this.selectTimer.Elapsed += this.FileSelectTimer_Elapsed;
			this.selectTimer.Start();
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00012C20 File Offset: 0x00010E20
		private async void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			string name = (sender as PdfViewerContextMenuItem).Name;
			GAManager.SendEvent("FileMenu", "CloseBtn", "Count", 1L);
			this.FileBtnPop.Visibility = Visibility.Hidden;
			this.FileBtn.IsChecked = new bool?(false);
			this.FileBtnPop.IsOpen = false;
			await Task.Delay(300);
			base.Close();
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00012C60 File Offset: 0x00010E60
		private async void Btn_OpenAttachment_Click(object sender, RoutedEventArgs e)
		{
			PDFAttachmentWrapper pdfattachmentWrapper = this.ListBox_Attachment.SelectedItem as PDFAttachmentWrapper;
			if (pdfattachmentWrapper != null)
			{
				await this.VM.OpenAttachmentCmd.ExecuteAsync(pdfattachmentWrapper);
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00012C98 File Offset: 0x00010E98
		private async void Btn_AddAttachment_Click(object sender, RoutedEventArgs e)
		{
			await this.VM.AddAttachmentCmd.ExecuteAsync(null);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00012CD0 File Offset: 0x00010ED0
		private async void Button_EditDescription_Click(object sender, RoutedEventArgs e)
		{
			PDFAttachmentWrapper pdfattachmentWrapper = this.ListBox_Attachment.SelectedItem as PDFAttachmentWrapper;
			if (pdfattachmentWrapper != null)
			{
				await this.VM.EditAttachmentDescriptionCmd.ExecuteAsync(pdfattachmentWrapper);
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00012D08 File Offset: 0x00010F08
		private async void Btn_SaveAttachment_Click(object sender, RoutedEventArgs e)
		{
			if (this.ListBox_Attachment.SelectedItems != null && this.ListBox_Attachment.SelectedItems.Count > 0)
			{
				await this.VM.SaveAttachmentCmd.ExecuteAsync(this.ListBox_Attachment.SelectedItems.Cast<PDFAttachmentWrapper>().ToList<PDFAttachmentWrapper>());
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00012D40 File Offset: 0x00010F40
		private async void Btn_DeleteAttachment_Click(object sender, RoutedEventArgs e)
		{
			await this.VM.DeleteAttachmentCmd.ExecuteAsync(this.ListBox_Attachment.SelectedItems.Cast<PDFAttachmentWrapper>().ToList<PDFAttachmentWrapper>());
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00012D78 File Offset: 0x00010F78
		private async void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListBoxItem listBoxItem = sender as ListBoxItem;
			if (listBoxItem != null)
			{
				PDFAttachmentWrapper pdfattachmentWrapper = listBoxItem.Content as PDFAttachmentWrapper;
				if (pdfattachmentWrapper != null)
				{
					await this.VM.OpenAttachmentCmd.ExecuteAsync(pdfattachmentWrapper);
				}
			}
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00012DB8 File Offset: 0x00010FB8
		private async void SaveToPCMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (this.ListBox_Attachment.SelectedItems != null && this.ListBox_Attachment.SelectedItems.Count > 0)
			{
				await this.VM.SaveAttachmentCmd.ExecuteAsync(this.ListBox_Attachment.SelectedItems.Cast<PDFAttachmentWrapper>().ToList<PDFAttachmentWrapper>());
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00012DF0 File Offset: 0x00010FF0
		private async void AddAttachmentsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			await this.VM.AddAttachmentCmd.ExecuteAsync(null);
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00012E28 File Offset: 0x00011028
		private async void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			PDFAttachmentWrapper pdfattachmentWrapper = this.ListBox_Attachment.SelectedItem as PDFAttachmentWrapper;
			if (pdfattachmentWrapper != null)
			{
				await this.VM.OpenAttachmentCmd.ExecuteAsync(pdfattachmentWrapper);
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00012E60 File Offset: 0x00011060
		private async void EditDescriptionsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			PDFAttachmentWrapper pdfattachmentWrapper = this.ListBox_Attachment.SelectedItem as PDFAttachmentWrapper;
			if (pdfattachmentWrapper != null)
			{
				await this.VM.EditAttachmentDescriptionCmd.ExecuteAsync(pdfattachmentWrapper);
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00012E98 File Offset: 0x00011098
		private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			await this.VM.DeleteAttachmentCmd.ExecuteAsync(this.ListBox_Attachment.SelectedItems.Cast<PDFAttachmentWrapper>().ToList<PDFAttachmentWrapper>());
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00012ED0 File Offset: 0x000110D0
		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			ContextMenu contextMenu = sender as ContextMenu;
			if (contextMenu != null)
			{
				Point position = Mouse.GetPosition(this.ListBox_Attachment);
				HitTestResult hitTestResult = VisualTreeHelper.HitTest(this.ListBox_Attachment, position);
				bool flag = ((hitTestResult != null) ? VisualHelper.GetParent<ListBoxItem>(hitTestResult.VisualHit) : null) != null;
				foreach (object obj in ((IEnumerable)contextMenu.Items))
				{
					MenuItem menuItem = (MenuItem)obj;
					string name = menuItem.Name;
					if (!(name == "SaveToPCMenuItem") && !(name == "DeleteMenuItem"))
					{
						if (!(name == "AddAttachmentsMenuItem"))
						{
							if (name == "OpenFileMenuItem" || name == "EditDescriptionsMenuItem")
							{
								bool flag2 = flag && this.ListBox_Attachment.SelectedItems.Count == 1;
								this.SetMenuItemState(menuItem, flag2);
							}
						}
						else
						{
							Visibility visibility = (flag ? Visibility.Collapsed : Visibility.Visible);
							this.SetMenuItemVisibility(menuItem, visibility);
						}
					}
					else
					{
						this.SetMenuItemState(menuItem, flag);
					}
				}
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00013004 File Offset: 0x00011204
		private void SetMenuItemState(MenuItem item, bool isEnabled)
		{
			item.IsEnabled = isEnabled;
			item.Opacity = (isEnabled ? 1.0 : 0.5);
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0001302A File Offset: 0x0001122A
		private void SetMenuItemVisibility(MenuItem item, Visibility visibility)
		{
			item.Visibility = visibility;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00013034 File Offset: 0x00011234
		private void HyperLink_Attachment_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink hyperlink = sender as Hyperlink;
			if (hyperlink != null)
			{
				PDFAttachmentWrapper pdfattachmentWrapper = hyperlink.DataContext as PDFAttachmentWrapper;
				if (pdfattachmentWrapper != null)
				{
					object obj = pdfattachmentWrapper.Attachment;
					PdfFileAttachmentAnnotation annotation = obj as PdfFileAttachmentAnnotation;
					if (annotation != null)
					{
						PdfPage pdfPage = this.VM.Document.Pages[annotation.Page.PageIndex];
						global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
						if (pdfPage != null && viewer != null)
						{
							if (!this.VM.IsAnnotationVisible)
							{
								this.VM.IsAnnotationVisible = true;
							}
							bool flag = false;
							if (viewer.PageIndex != pdfPage.PageIndex)
							{
								flag = true;
								viewer.ScrollToPage(pdfPage.PageIndex);
							}
							float num;
							float num2;
							pdfPage.GetEffectiveSize(PageRotate.Normal, false).Deconstruct(out num, out num2);
							float num3 = num;
							float num4 = num2;
							FS_RECTF rect = annotation.GetRECT();
							if (rect.left < num3 && rect.right > 0f && rect.top > 0f && rect.bottom < num4)
							{
								if (flag)
								{
									base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
									{
										MainView <>4__this = this;
										global::PDFKit.PdfControl viewer2 = viewer;
										<>4__this.ScrollToAnnotation((viewer2 != null) ? viewer2.Viewer : null, annotation);
									}));
									return;
								}
								global::PDFKit.PdfControl viewer3 = viewer;
								this.ScrollToAnnotation((viewer3 != null) ? viewer3.Viewer : null, annotation);
							}
						}
					}
				}
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000131B4 File Offset: 0x000113B4
		private void ScrollToAnnotation(PdfViewer viewer, PdfMarkupAnnotation markup)
		{
			try
			{
				Rect deviceBounds = markup.GetDeviceBounds();
				if (deviceBounds.Left > viewer.ActualWidth || deviceBounds.Right < 0.0)
				{
					if (deviceBounds.Left > viewer.ActualWidth)
					{
						viewer.ScrollOwner.ScrollToHorizontalOffset(viewer.ScrollOwner.HorizontalOffset + (deviceBounds.Right - viewer.ActualWidth));
					}
					else
					{
						viewer.ScrollOwner.ScrollToHorizontalOffset(viewer.ScrollOwner.HorizontalOffset + deviceBounds.Left);
					}
				}
				if (deviceBounds.Top > viewer.ActualHeight || deviceBounds.Bottom < 0.0)
				{
					if (deviceBounds.Top > viewer.ActualHeight)
					{
						viewer.ScrollOwner.ScrollToVerticalOffset(viewer.ScrollOwner.VerticalOffset + (deviceBounds.Bottom - viewer.ActualHeight));
					}
					else
					{
						viewer.ScrollOwner.ScrollToVerticalOffset(viewer.ScrollOwner.VerticalOffset + deviceBounds.Top);
					}
				}
			}
			catch
			{
				PdfPage page = markup.Page;
				if (viewer.CurrentIndex != page.PageIndex)
				{
					viewer.ScrollToPage(page.PageIndex);
				}
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000132E8 File Offset: 0x000114E8
		public async Task InitialRecentFileItem(PdfViewerContextMenuItem menuItem)
		{
			try
			{
				menuItem.Items.Clear();
				PdfViewerContextMenuItem clearItem = null;
				clearItem = new PdfViewerContextMenuItem();
				clearItem.Header = pdfeditor.Properties.Resources.WinSignatureDeleteInBatchBtn;
				clearItem.Command = new RelayCommand(delegate
				{
					if (HistoryManager.ReadHistory().Count == 0)
					{
						return;
					}
					if (ModernMessageBox.Show(pdfeditor.Properties.Resources.WinHistoryClearLabelClickTips, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
					{
						HistoryManager.RemoveAllItems();
					}
				});
				clearItem.MinWidth = 180.0;
				clearItem.IsEnabled = false;
				menuItem.Items.Add(clearItem);
				BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/ExtIcon/Pdf.png"));
				bitmapImage.Freeze();
				IEnumerable<OpenedInfo> enumerable = await HistoryManager.ReadHistoryAsync(default(CancellationToken));
				string text = global::System.IO.Path.Combine(UtilManager.GetTemporaryPath(), "Documents");
				ObservableCollection<RecentFileItem> observableCollection = new ObservableCollection<RecentFileItem>();
				using (IEnumerator<OpenedInfo> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MainView.<>c__DisplayClass119_0 CS$<>8__locals1 = new MainView.<>c__DisplayClass119_0();
						CS$<>8__locals1.<>4__this = this;
						CS$<>8__locals1.path = enumerator.Current;
						if (global::System.IO.Path.GetDirectoryName(CS$<>8__locals1.path.FilePath) == text)
						{
							bool flag;
							HistoryManager.RemoveHistoryItem(CS$<>8__locals1.path.FilePath, out flag);
						}
						else
						{
							observableCollection.Add(new RecentFileItem
							{
								FilePath = CS$<>8__locals1.path.FilePath,
								FileName = global::System.IO.Path.GetFileName(CS$<>8__locals1.path.FilePath),
								Icon = bitmapImage,
								OpenCommand = new AsyncRelayCommand(delegate
								{
									MainView.<>c__DisplayClass119_0.<<InitialRecentFileItem>b__1>d <<InitialRecentFileItem>b__1>d;
									<<InitialRecentFileItem>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
									<<InitialRecentFileItem>b__1>d.<>4__this = CS$<>8__locals1;
									<<InitialRecentFileItem>b__1>d.<>1__state = -1;
									<<InitialRecentFileItem>b__1>d.<>t__builder.Start<MainView.<>c__DisplayClass119_0.<<InitialRecentFileItem>b__1>d>(ref <<InitialRecentFileItem>b__1>d);
									return <<InitialRecentFileItem>b__1>d.<>t__builder.Task;
								})
							});
						}
					}
				}
				if (observableCollection.Count > 0)
				{
					ContextMenuSeparator contextMenuSeparator = new ContextMenuSeparator();
					menuItem.Items.Add(contextMenuSeparator);
					ContextMenuRecentFileList contextMenuRecentFileList = new ContextMenuRecentFileList();
					contextMenuRecentFileList.Name = "RecentFileList";
					contextMenuRecentFileList.RecentFiles = observableCollection;
					menuItem.Items.Add(contextMenuRecentFileList);
				}
				clearItem.IsEnabled = observableCollection.Count > 0;
				clearItem = null;
				bitmapImage = null;
			}
			catch
			{
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00013334 File Offset: 0x00011534
		private async void FileBtnPop_Opened(object sender, EventArgs e)
		{
			PdfViewerContextMenuItem pdfViewerContextMenuItem = new PdfViewerContextMenuItem();
			this.ContextMenuItem_RecentFile.Items.Add(pdfViewerContextMenuItem);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0001336C File Offset: 0x0001156C
		private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				global::System.Timers.Timer timer = this.selectTimer;
				if (timer != null)
				{
					timer.Stop();
				}
				global::System.Timers.Timer timer2 = this.selectTimer;
				if (timer2 != null)
				{
					timer2.Dispose();
				}
				this.selectTimer = new global::System.Timers.Timer(500.0);
				this.selectTimer.Elapsed += this.DocZoomInTimer_Elapsed;
				this.selectTimer.Start();
			}
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000133DA File Offset: 0x000115DA
		private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			global::System.Timers.Timer timer = this.selectTimer;
			if (timer != null)
			{
				timer.Stop();
			}
			global::System.Timers.Timer timer2 = this.selectTimer;
			if (timer2 == null)
			{
				return;
			}
			timer2.Dispose();
		}

		// Token: 0x060003DB RID: 987 RVA: 0x000133FD File Offset: 0x000115FD
		private void DocZoomOutTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.selectTimer.Interval = 100.0;
				this.VM.ViewToolbar.DocZoomOutCmd.Execute(null);
			});
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001341A File Offset: 0x0001161A
		private void DocZoomInTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.selectTimer.Interval = 100.0;
				this.VM.ViewToolbar.DocZoomInCmd.Execute(null);
			});
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00013438 File Offset: 0x00011638
		private void Button_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				global::System.Timers.Timer timer = this.selectTimer;
				if (timer != null)
				{
					timer.Stop();
				}
				global::System.Timers.Timer timer2 = this.selectTimer;
				if (timer2 != null)
				{
					timer2.Dispose();
				}
				this.selectTimer = new global::System.Timers.Timer(500.0);
				this.selectTimer.Elapsed += this.DocZoomOutTimer_Elapsed;
				this.selectTimer.Start();
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x000134A6 File Offset: 0x000116A6
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			global::System.Timers.Timer timer = this.selectTimer;
			if (timer != null)
			{
				timer.Stop();
			}
			global::System.Timers.Timer timer2 = this.selectTimer;
			if (timer2 == null)
			{
				return;
			}
			timer2.Dispose();
		}

		// Token: 0x060003DF RID: 991 RVA: 0x000134CC File Offset: 0x000116CC
		private void Btn_Login_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("MainWindow", "LoginBtnClick", "Count", 1L);
			new UserLoginWin().ShowDialog();
			if (this.VM.UserInfoModel != null)
			{
				Window window = Window.GetWindow(this.Popup_UserInfo);
				if (window != null)
				{
					window.SetForegroundWindow();
				}
				this.Popup_UserInfo.IsOpen = true;
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0001352A File Offset: 0x0001172A
		private void TranslatePanel_CloseButtonClick(object sender, EventArgs e)
		{
			this.VM.TranslatePanelVisible = false;
			this.RightNavigationTranslateView.IsAnimationEnabled = true;
			if (this.VM.ChatPanelState)
			{
				this.VM.ChatPanelVisible = true;
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0001355D File Offset: 0x0001175D
		private void Slider_ThumbnailScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			PdfPagePreviewHScrollGridView thumbnailList = this.ThumbnailList;
			if (thumbnailList == null)
			{
				return;
			}
			thumbnailList.BringSelectedIndexIntoView();
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00013570 File Offset: 0x00011770
		private void VirtualThumbnailPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is VirtualThumbnailPanel && e.OriginalSource == sender)
			{
				object selectedItem = this.ThumbnailList.SelectedItem;
				this.ThumbnailList.SelectedItems.Clear();
				this.ThumbnailList.SelectedItems.Add(selectedItem);
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x000135BC File Offset: 0x000117BC
		private async void ContextMenuItem_RecentFile_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			await this.InitialRecentFileItem(this.ContextMenuItem_RecentFile);
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00014990 File Offset: 0x00012B90
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = Control.MouseDoubleClickEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.MenuGroup_MouseDoubleClick);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.PreviewMouseDownEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.MenuGroup_PreviewMouseDown);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 2:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = Control.MouseDoubleClickEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.ListBoxItem_MouseDoubleClick);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 3:
				((Hyperlink)target).Click += this.HyperLink_Attachment_Click;
				return;
			default:
				if (connectionId != 208)
				{
					return;
				}
				((Border)target).PreviewMouseDown += this.Border_PreviewMouseDown;
				return;
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00014ABC File Offset: 0x00012CBC
		[CompilerGenerated]
		internal static IntPtr <OnSourceInitialized>g__WndProc|6_0(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 17)
			{
				Log.WriteLog("WM_QUERYENDSESSION");
				pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().TrySaveImmediately();
				handled = true;
				return (IntPtr)1;
			}
			if (msg == 22)
			{
				Log.WriteLog("WM_ENDSESSION");
				handled = true;
				Application.Current.Shutdown();
				return (IntPtr)1;
			}
			return IntPtr.Zero;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00014B28 File Offset: 0x00012D28
		[CompilerGenerated]
		internal static bool <Viewer_LostFocus>g__IsChild|35_0<T>(FrameworkElement _ele) where T : DependencyObject
		{
			return MainView.<Viewer_LostFocus>g__GetParent|35_1<T>(_ele) != null;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00014B38 File Offset: 0x00012D38
		[CompilerGenerated]
		internal static T <Viewer_LostFocus>g__GetParent|35_1<T>(FrameworkElement _ele) where T : DependencyObject
		{
			while (_ele != null)
			{
				T t = _ele as T;
				if (t != null)
				{
					return t;
				}
				_ele = (_ele.Parent as FrameworkElement) ?? (VisualTreeHelper.GetParent(_ele) as FrameworkElement);
			}
			return default(T);
		}

		// Token: 0x0400017A RID: 378
		private bool isClosing;

		// Token: 0x0400017B RID: 379
		private bool isReadyToClose;

		// Token: 0x0400017C RID: 380
		private bool isClosed;

		// Token: 0x0400017D RID: 381
		private static BackgroundColorSetting[] viewerBackgroundColorValues;

		// Token: 0x0400017E RID: 382
		private static IReadOnlyDictionary<string, BackgroundColorSetting> viewerBackgroundColorDict;

		// Token: 0x0400017F RID: 383
		private MicaHelper micaHelper;

		// Token: 0x04000180 RID: 384
		private BookmarkModel bookmarkContextMenuData;

		// Token: 0x04000181 RID: 385
		private double menuHeaderMaxWidth;

		// Token: 0x04000182 RID: 386
		public static readonly DependencyProperty IsHeaderMenuCompactModeEnabledProperty = DependencyProperty.RegisterAttached("IsHeaderMenuCompactModeEnabled", typeof(bool), typeof(MainView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.Inherits));

		// Token: 0x04000183 RID: 387
		private global::System.Timers.Timer selectTimer;

		// Token: 0x04000184 RID: 388
		private PdfViewerContextMenuItem menuItem1;
	}
}
