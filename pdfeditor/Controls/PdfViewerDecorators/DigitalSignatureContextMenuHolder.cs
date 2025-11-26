using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.DigitalSignatures;
using pdfeditor.Models;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Protection;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000236 RID: 566
	internal class DigitalSignatureContextMenuHolder
	{
		// Token: 0x06002021 RID: 8225 RVA: 0x000912E9 File Offset: 0x0008F4E9
		public DigitalSignatureContextMenuHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.InitContextMenu();
		}

		// Token: 0x06002022 RID: 8226 RVA: 0x00091310 File Offset: 0x0008F510
		private void InitContextMenu()
		{
			if (DesignTimeHelpers.IsRunningInDesignerMode)
			{
				return;
			}
			this.validateItem = new ContextMenuItemModel
			{
				Name = "ValidateSignature",
				Caption = Resources.DigSignPanelContextMenuValidateSign,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_Validate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_Validate.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextValidateSignature))
			};
			this.showPropertiesItem = new ContextMenuItemModel
			{
				Name = "ShowSignatureProperties",
				Caption = Resources.DigSignPanelContextMenuShowProps,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_ShowProperties.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_ShowProperties.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextShowSignatureProperties))
			};
			this.removeItem = new ContextMenuItemModel
			{
				Name = "RemoveSignature",
				Caption = Resources.DigSignPanelContextMenuRemoveSign,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_Remove.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_Remove.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextRemoveSignature))
			};
			this.contextMenus = new ContextMenuModel { this.validateItem, this.showPropertiesItem };
			this.signNow = new ContextMenuItemModel
			{
				Name = "SignNow",
				Caption = Resources.ResourceManager.GetString("DigSignPanelContextMenuUnsignedSignNow"),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_Validate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_Validate.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeferrerSignNow))
			};
			this.deferrerSignPropertiesItem = new ContextMenuItemModel
			{
				Name = "DeferrerSignProperties",
				Caption = Resources.ResourceManager.GetString("DigSignPanelContextMenuUnsignedShowProps"),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_ShowProperties.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_ShowProperties.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeferrerSignProperties))
			};
			this.deferrerSignRemoveItem = new ContextMenuItemModel
			{
				Name = "DeferrerSignRemove",
				Caption = Resources.ResourceManager.GetString("DigSignPanelContextMenuUnsignedRemove"),
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DigitalSignature_Remove.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DigitalSignature_Remove.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeferrerSignRemove))
			};
			this.deferrerSignContextMenus = new ContextMenuModel
			{
				this.signNow,
				this.deferrerSignPropertiesItem,
				new ContextMenuSeparator(),
				this.deferrerSignRemoveItem
			};
			this.digitalSignatureContextMenu = new PdfViewerContextMenu
			{
				ItemsSource = this.contextMenus,
				PlacementTarget = this.annotationCanvas,
				AutoCloseOnMouseLeave = false
			};
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x000915D4 File Offset: 0x0008F7D4
		private async Task OnContextValidateSignature(ContextMenuItemModel model)
		{
			GAManager.SendEvent("DigitalSignature", "ValidateDigitalSignature", "ContextMenu", 1L);
			PdfDigitalSignatureField field = this.currentField;
			if (field != null)
			{
				global::System.ValueTuple<bool, PdfSignatureValidationResult> valueTuple = await SignatureValidateHelper.ShowSignatureStatusDialogAsync(field, false, true);
				bool item = valueTuple.Item1;
				PdfSignatureValidationResult item2 = valueTuple.Item2;
				if (item && item2 != null)
				{
					GAManager.SendEvent("DigitalSignature", "ShowSignatureProperties", "ValidateDigitalSignature_ContextMenu", 1L);
					SignatureValidateHelper.ShowSignaturePropertiesDialog(field, item2);
				}
			}
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x00091618 File Offset: 0x0008F818
		private async Task OnContextShowSignatureProperties(ContextMenuItemModel model)
		{
			GAManager.SendEvent("DigitalSignature", "ShowSignatureProperties", "ContextMenu_OnDigitalSignature", 1L);
			PdfDigitalSignatureField pdfDigitalSignatureField = this.currentField;
			if (pdfDigitalSignatureField != null)
			{
				await SignatureValidateHelper.ShowSignaturePropertiesDialogAsync(pdfDigitalSignatureField, false, true);
			}
		}

		// Token: 0x06002025 RID: 8229 RVA: 0x0009165C File Offset: 0x0008F85C
		private async Task OnContextRemoveSignature(ContextMenuItemModel model)
		{
			GAManager.SendEvent("DigitalSignature", "RemoveDigitalSignature", "ContextMenu", 1L);
			PdfDigitalSignatureField pdfDigitalSignatureField = this.currentField;
			if (pdfDigitalSignatureField != null)
			{
				object dataContext = this.annotationCanvas.DataContext;
				MainViewModel vm = dataContext as MainViewModel;
				if (vm != null)
				{
					DocumentWrapper documentWrapper = vm.DocumentWrapper;
					if (((documentWrapper != null) ? documentWrapper.DigitalSignatureHelper : null) != null)
					{
						DocumentWrapper documentWrapper2 = vm.DocumentWrapper;
						string text;
						if (documentWrapper2 == null)
						{
							text = null;
						}
						else
						{
							EncryptManage encryptManage = documentWrapper2.EncryptManage;
							text = ((encryptManage != null) ? encryptManage.UserPassword : null);
						}
						string password = text;
						if (string.IsNullOrEmpty(password))
						{
							password = null;
						}
						MainViewModel.SaveResult saveResult = await vm.DocumentWrapper.DigitalSignatureHelper.RemoveDigitalSignatureAndSaveAsAsync(pdfDigitalSignatureField, vm.CanSave);
						if (saveResult.FailedResult == MainViewModel.SaveFailedResult.Successed)
						{
							await vm.OpenDocumentCoreAsync(saveResult.File.FullName, password, true);
						}
						else
						{
							ModernMessageBox.Show(Resources.DigSigRemoveMessage_Failed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						password = null;
					}
				}
			}
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x000916A0 File Offset: 0x0008F8A0
		private async Task OnContextDeferrerSignProperties(ContextMenuItemModel model)
		{
			PdfDigitalSignatureField pdfDigitalSignatureField = this.currentField;
			PdfDigitalSignatureLocation pdfDigitalSignatureLocation = ((pdfDigitalSignatureField != null) ? pdfDigitalSignatureField.Location : null);
			if (pdfDigitalSignatureLocation != null)
			{
				GAManager.SendEvent("DigitalSignatureFiled", "ShowFiledProperty", "ContextMenu", 1L);
				DigitalSignatureContextMenuHolder.ShowDeferrerSignProperties(pdfDigitalSignatureLocation);
			}
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x000916E4 File Offset: 0x0008F8E4
		public static void ShowDeferrerSignProperties(PdfDigitalSignatureLocation location)
		{
			if (location != null)
			{
				bool? flag = new DeferrerSignaturePropertiesDialog(location)
				{
					Owner = App.Current.MainWindow,
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				}.ShowDialog();
				if (flag != null && flag.GetValueOrDefault())
				{
					StrongReferenceMessenger.Default.Send<ValueChangedMessage<PdfDigitalSignatureLocation>, string>(new ValueChangedMessage<PdfDigitalSignatureLocation>(location), "DeferrerSignaturePropertiesChanged");
				}
			}
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x00091740 File Offset: 0x0008F940
		private async Task OnContextDeferrerSignRemove(ContextMenuItemModel model)
		{
			PdfDigitalSignatureField pdfDigitalSignatureField = this.currentField;
			if (pdfDigitalSignatureField != null)
			{
				MainViewModel mainViewModel = this.annotationCanvas.DataContext as MainViewModel;
				if (mainViewModel != null)
				{
					GAManager.SendEvent("DigitalSignatureFiled", "RemoveDSFiled", "ContextMenu", 1L);
					await mainViewModel.AnnotationToolbar.RemoveDeferredDigitalSignature(pdfDigitalSignatureField.Location);
				}
			}
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x00091784 File Offset: 0x0008F984
		private async Task OnContextDeferrerSignNow(ContextMenuItemModel model)
		{
			PdfDigitalSignatureField pdfDigitalSignatureField = this.currentField;
			if (pdfDigitalSignatureField != null)
			{
				MainViewModel mainViewModel = this.annotationCanvas.DataContext as MainViewModel;
				if (mainViewModel != null)
				{
					GAManager.SendEvent("DigitalSignatureFiled", "SignInDSFiled", "ContextMenu", 1L);
					await mainViewModel.AnnotationToolbar.SignDeferredDigitalSignature(pdfDigitalSignatureField.Location);
				}
			}
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000917C8 File Offset: 0x0008F9C8
		public async Task<bool> ShowAsync()
		{
			this.currentField = null;
			if (this.digitalSignatureContextMenu.IsOpen)
			{
				this.digitalSignatureContextMenu.IsOpen = false;
				await Task.Delay(50);
			}
			PdfDigitalSignatureField pdfDigitalSignatureField = await this.GetPdfDigitalSignatureField();
			bool flag;
			if (pdfDigitalSignatureField == null)
			{
				flag = false;
			}
			else
			{
				this.currentField = pdfDigitalSignatureField;
				if (pdfDigitalSignatureField.IsSigned)
				{
					this.digitalSignatureContextMenu.ItemsSource = this.contextMenus;
				}
				else
				{
					this.digitalSignatureContextMenu.ItemsSource = this.deferrerSignContextMenus;
				}
				this.digitalSignatureContextMenu.IsOpen = true;
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x0009180B File Offset: 0x0008FA0B
		public void Hide()
		{
			this.digitalSignatureContextMenu.IsOpen = false;
			this.currentField = null;
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x00091820 File Offset: 0x0008FA20
		public async Task<PdfDigitalSignatureField> GetPdfDigitalSignatureField()
		{
			MainViewModel mainViewModel = this.annotationCanvas.DataContext as MainViewModel;
			if (mainViewModel != null)
			{
				DocumentWrapper documentWrapper = mainViewModel.DocumentWrapper;
				if (((documentWrapper != null) ? documentWrapper.DigitalSignatureHelper : null) != null)
				{
					PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
					PdfPage pdfPage = ((pdfViewer != null) ? pdfViewer.CurrentPage : null);
					if (((pdfPage != null) ? pdfPage.Annots : null) == null)
					{
						return null;
					}
					Point position = Mouse.GetPosition(this.annotationCanvas.PdfViewer);
					int i = 0;
					while (i < pdfPage.Annots.Count)
					{
						if (AnnotationHitTestHelper.HitTest(pdfPage.Annots[i], position))
						{
							if (pdfPage.Annots[i].IsDigitalSignatureAnnotation())
							{
								return await mainViewModel.DocumentWrapper.DigitalSignatureHelper.GetFieldInfo((PdfWidgetAnnotation)pdfPage.Annots[i]);
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x04000CE5 RID: 3301
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CE6 RID: 3302
		private PdfViewerContextMenu digitalSignatureContextMenu;

		// Token: 0x04000CE7 RID: 3303
		private ContextMenuModel contextMenus;

		// Token: 0x04000CE8 RID: 3304
		private ContextMenuItemModel validateItem;

		// Token: 0x04000CE9 RID: 3305
		private ContextMenuItemModel showPropertiesItem;

		// Token: 0x04000CEA RID: 3306
		private ContextMenuItemModel removeItem;

		// Token: 0x04000CEB RID: 3307
		private ContextMenuModel deferrerSignContextMenus;

		// Token: 0x04000CEC RID: 3308
		private ContextMenuItemModel signNow;

		// Token: 0x04000CED RID: 3309
		private ContextMenuItemModel deferrerSignRemoveItem;

		// Token: 0x04000CEE RID: 3310
		private ContextMenuItemModel deferrerSignPropertiesItem;

		// Token: 0x04000CEF RID: 3311
		private PdfDigitalSignatureField currentField;
	}
}
