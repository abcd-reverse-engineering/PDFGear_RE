using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Patagames.Pdf;
using Patagames.Pdf.Net.AcroForms;
using pdfeditor.Controls.PdfViewerDecorators;
using pdfeditor.Models;
using pdfeditor.Models.Protection;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000285 RID: 645
	internal partial class DigitalSignatureControl : Control
	{
		// Token: 0x06002541 RID: 9537 RVA: 0x000AD6D0 File Offset: 0x000AB8D0
		static DigitalSignatureControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DigitalSignatureControl), new FrameworkPropertyMetadata(typeof(DigitalSignatureControl)));
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x000AD739 File Offset: 0x000AB939
		public DigitalSignatureControl()
		{
			base.Loaded += delegate(object s, RoutedEventArgs a)
			{
				StrongReferenceMessenger.Default.Register(this, "DeferrerSignaturePropertiesChanged", delegate(object s1, [Nullable(new byte[] { 1, 0 })] ValueChangedMessage<PdfDigitalSignatureLocation> a1)
				{
					for (int i = 0; i < this.InternalItemsSource.Count; i++)
					{
						PdfDigitalSignatureField field = this.InternalItemsSource[i].Field;
						if (((field != null) ? field.Location : null) == a1.Value)
						{
							this.InternalItemsSource[i].RaiseNameChanged();
						}
					}
				});
			};
			base.Unloaded += delegate(object s, RoutedEventArgs a)
			{
				StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<PdfDigitalSignatureLocation>, string>(this, "DeferrerSignaturePropertiesChanged");
			};
		}

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06002543 RID: 9539 RVA: 0x000AD765 File Offset: 0x000AB965
		// (set) Token: 0x06002544 RID: 9540 RVA: 0x000AD76D File Offset: 0x000AB96D
		private IReadOnlyList<DigitalSignatureControl.InternalModel> InternalItemsSource
		{
			get
			{
				return this.internalItemsSource;
			}
			set
			{
				if (this.internalItemsSource != value)
				{
					this.internalItemsSource = value;
					if (this.InternalList != null)
					{
						this.InternalList.ItemsSource = this.internalItemsSource;
					}
				}
			}
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x000AD798 File Offset: 0x000AB998
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.InternalList = base.GetTemplateChild("InternalList") as HeaderedItemsControl;
			if (this.InternalList != null)
			{
				this.InternalList.ItemsSource = this.internalItemsSource;
			}
		}

		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x06002546 RID: 9542 RVA: 0x000AD7CF File Offset: 0x000AB9CF
		// (set) Token: 0x06002547 RID: 9543 RVA: 0x000AD7E1 File Offset: 0x000AB9E1
		public DigitalSignatureHelper DigitalSignatureHelper
		{
			get
			{
				return (DigitalSignatureHelper)base.GetValue(DigitalSignatureControl.DigitalSignatureHelperProperty);
			}
			set
			{
				base.SetValue(DigitalSignatureControl.DigitalSignatureHelperProperty, value);
			}
		}

		// Token: 0x04000FF2 RID: 4082
		private IReadOnlyList<DigitalSignatureControl.InternalModel> internalItemsSource;

		// Token: 0x04000FF3 RID: 4083
		private HeaderedItemsControl InternalList;

		// Token: 0x04000FF4 RID: 4084
		public static readonly DependencyProperty DigitalSignatureHelperProperty = DependencyProperty.Register("DigitalSignatureHelper", typeof(DigitalSignatureHelper), typeof(DigitalSignatureControl), new PropertyMetadata(null, async delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DigitalSignatureControl sender = s as DigitalSignatureControl;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				DigitalSignatureHelper digitalSignatureHelper = a.NewValue as DigitalSignatureHelper;
				if (digitalSignatureHelper != null)
				{
					IReadOnlyList<PdfDigitalSignatureField> readOnlyList = await digitalSignatureHelper.GetDigitalSignaturesAsync();
					sender.InternalItemsSource = readOnlyList.Select((PdfDigitalSignatureField c) => new DigitalSignatureControl.InternalModel(c, (DigitalSignatureHelper)a.NewValue)).ToList<DigitalSignatureControl.InternalModel>();
				}
				else
				{
					sender.InternalItemsSource = null;
				}
			}
		}));

		// Token: 0x0200073C RID: 1852
		private class InternalModel : ObservableObject
		{
			// Token: 0x06003650 RID: 13904 RVA: 0x00110D24 File Offset: 0x0010EF24
			public InternalModel(PdfDigitalSignatureField field, DigitalSignatureHelper digitalSignatureHelper)
			{
				this.Field = field;
				this.digitalSignatureHelper = digitalSignatureHelper;
				this.RaiseNameChanged();
				this.SignedName = this.Field.SignedName;
				this.HasSigned = this.Field.IsSigned;
				this.DisplayLocationPageIndex = field.Location.PageIndex + 1;
				PdfDigitalSignatureField field2 = this.Field;
				bool? flag = ((field2 != null) ? new bool?(field2.IsBroken) : null);
				if (flag != null && flag.GetValueOrDefault())
				{
					this.IsValid = new bool?(false);
				}
				else
				{
					this.IsValid = null;
				}
				this.IsFieldPageVisible = this.Field.Location.PageIndex >= 0;
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(this.Field, "PropertyChanged", delegate(object s, PropertyChangedEventArgs a)
				{
					if (a.PropertyName == "LastValidateResult")
					{
						this.UpdateResultInfo();
					}
				});
			}

			// Token: 0x17000D7C RID: 3452
			// (get) Token: 0x06003651 RID: 13905 RVA: 0x00110E1D File Offset: 0x0010F01D
			public PdfDigitalSignatureField Field { get; }

			// Token: 0x17000D7D RID: 3453
			// (get) Token: 0x06003652 RID: 13906 RVA: 0x00110E25 File Offset: 0x0010F025
			public PdfDigitalSignatureLocation Location { get; }

			// Token: 0x17000D7E RID: 3454
			// (get) Token: 0x06003653 RID: 13907 RVA: 0x00110E2D File Offset: 0x0010F02D
			public string Name
			{
				get
				{
					this.ValidateSignatureFirstTimeCommand.Execute(null);
					return this.name;
				}
			}

			// Token: 0x17000D7F RID: 3455
			// (get) Token: 0x06003654 RID: 13908 RVA: 0x00110E41 File Offset: 0x0010F041
			public string SignedName { get; }

			// Token: 0x17000D80 RID: 3456
			// (get) Token: 0x06003655 RID: 13909 RVA: 0x00110E49 File Offset: 0x0010F049
			public Visibility SignedNameVisibility
			{
				get
				{
					if (!string.IsNullOrEmpty(this.SignedName))
					{
						return Visibility.Visible;
					}
					return Visibility.Collapsed;
				}
			}

			// Token: 0x17000D81 RID: 3457
			// (get) Token: 0x06003656 RID: 13910 RVA: 0x00110E5B File Offset: 0x0010F05B
			public int DisplayLocationPageIndex { get; }

			// Token: 0x17000D82 RID: 3458
			// (get) Token: 0x06003657 RID: 13911 RVA: 0x00110E63 File Offset: 0x0010F063
			// (set) Token: 0x06003658 RID: 13912 RVA: 0x00110E6B File Offset: 0x0010F06B
			public PdfSignatureValidationResult Result { get; private set; }

			// Token: 0x17000D83 RID: 3459
			// (get) Token: 0x06003659 RID: 13913 RVA: 0x00110E74 File Offset: 0x0010F074
			// (set) Token: 0x0600365A RID: 13914 RVA: 0x00110E7C File Offset: 0x0010F07C
			public bool IsFieldPageVisible { get; private set; }

			// Token: 0x17000D84 RID: 3460
			// (get) Token: 0x0600365B RID: 13915 RVA: 0x00110E85 File Offset: 0x0010F085
			// (set) Token: 0x0600365C RID: 13916 RVA: 0x00110E8D File Offset: 0x0010F08D
			public bool HasSigned { get; private set; }

			// Token: 0x0600365D RID: 13917 RVA: 0x00110E98 File Offset: 0x0010F098
			private void UpdateResultInfo()
			{
				PdfSignatureValidationResult lastValidateResult = this.Field.LastValidateResult;
				if (lastValidateResult != null)
				{
					this.IsValid = new bool?(lastValidateResult.IsValid(true));
					StringBuilder stringBuilder = new StringBuilder();
					foreach (DigitalSignatureFormattedItem digitalSignatureFormattedItem in SignatureFormatHelper.BuildValidateResultStatusContent(this.Field, lastValidateResult))
					{
						stringBuilder.AppendLine(digitalSignatureFormattedItem.Text);
					}
					StringBuilder stringBuilder2 = stringBuilder.Append(pdfeditor.Properties.Resources.DigSignPropLocation);
					PdfSignature signature = this.Field.Signature;
					stringBuilder2.AppendLine(((signature != null) ? signature.LocationInfo : null) ?? "");
					StringBuilder stringBuilder3 = stringBuilder.Append(pdfeditor.Properties.Resources.DigSignPropReason);
					PdfSignature signature2 = this.Field.Signature;
					stringBuilder3.AppendLine(((signature2 != null) ? signature2.Reason : null) ?? "");
					IReadOnlyList<DigitalSignatureFormattedItem> readOnlyList = SignatureFormatHelper.BuildDigitalSignatureInfosContent(this.Field, lastValidateResult);
					if (readOnlyList == null || readOnlyList.Count <= 0)
					{
						goto IL_0128;
					}
					using (IEnumerator<DigitalSignatureFormattedItem> enumerator = readOnlyList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DigitalSignatureFormattedItem digitalSignatureFormattedItem2 = enumerator.Current;
							stringBuilder.AppendLine(digitalSignatureFormattedItem2.Text);
						}
						goto IL_0128;
					}
					IL_0116:
					StringBuilder stringBuilder4 = stringBuilder;
					int length = stringBuilder4.Length;
					stringBuilder4.Length = length - 1;
					IL_0128:
					if (stringBuilder.Length > 0 && (stringBuilder[stringBuilder.Length - 1] == '\n' || stringBuilder[stringBuilder.Length - 1] == '\r'))
					{
						goto IL_0116;
					}
					this.SignValidatePropertiesText = SignatureFormatHelper.BuildValidateResultTitle(this.Field, lastValidateResult);
					this.SignPropertiesText = stringBuilder.ToString();
				}
				else
				{
					PdfDigitalSignatureField field = this.Field;
					bool? flag = ((field != null) ? new bool?(field.IsBroken) : null);
					if (flag != null && flag.GetValueOrDefault())
					{
						this.IsValid = new bool?(false);
					}
					else
					{
						this.IsValid = null;
					}
					this.SignValidatePropertiesText = "";
					this.SignPropertiesText = "";
				}
				base.OnPropertyChanged("SignPropertiesTextVisibility");
				this.Result = null;
				base.OnPropertyChanged("Result");
				this.Result = lastValidateResult;
				base.OnPropertyChanged("Result");
			}

			// Token: 0x17000D85 RID: 3461
			// (get) Token: 0x0600365E RID: 13918 RVA: 0x001110D0 File Offset: 0x0010F2D0
			// (set) Token: 0x0600365F RID: 13919 RVA: 0x001110D8 File Offset: 0x0010F2D8
			public bool? IsValid
			{
				get
				{
					return this.isValid;
				}
				private set
				{
					base.SetProperty<bool?>(ref this.isValid, value, "IsValid");
				}
			}

			// Token: 0x17000D86 RID: 3462
			// (get) Token: 0x06003660 RID: 13920 RVA: 0x001110ED File Offset: 0x0010F2ED
			public Visibility SignPropertiesTextVisibility
			{
				get
				{
					if (!string.IsNullOrEmpty(this.SignPropertiesText) || !string.IsNullOrEmpty(this.SignValidatePropertiesText))
					{
						return Visibility.Visible;
					}
					return Visibility.Collapsed;
				}
			}

			// Token: 0x17000D87 RID: 3463
			// (get) Token: 0x06003661 RID: 13921 RVA: 0x0011110C File Offset: 0x0010F30C
			// (set) Token: 0x06003662 RID: 13922 RVA: 0x00111114 File Offset: 0x0010F314
			public string SignPropertiesText
			{
				get
				{
					return this.signPropertiesText;
				}
				set
				{
					base.SetProperty<string>(ref this.signPropertiesText, value, "SignPropertiesText");
				}
			}

			// Token: 0x17000D88 RID: 3464
			// (get) Token: 0x06003663 RID: 13923 RVA: 0x00111129 File Offset: 0x0010F329
			// (set) Token: 0x06003664 RID: 13924 RVA: 0x00111131 File Offset: 0x0010F331
			public string SignValidatePropertiesText
			{
				get
				{
					return this.signValidatePropertiesText;
				}
				set
				{
					base.SetProperty<string>(ref this.signValidatePropertiesText, value, "SignValidatePropertiesText");
				}
			}

			// Token: 0x17000D89 RID: 3465
			// (get) Token: 0x06003665 RID: 13925 RVA: 0x00111148 File Offset: 0x0010F348
			public ICommand GoToFieldLocationCommand
			{
				get
				{
					ICommand command;
					if ((command = this.goToFieldLocationCommand) == null)
					{
						command = (this.goToFieldLocationCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignature", "GoToDigitalSignatureField", "Count", 1L);
							PdfControl pdfControl = PdfControl.GetPdfControl(this.Field.Location.SignatureField.InterForms.FillForms.Document);
							if (pdfControl != null)
							{
								pdfControl.ScrollToPage(this.Field.Location.PageIndex);
								pdfControl.UpdateDocLayout();
								pdfControl.UpdateLayout();
								FS_RECTF bounds = this.Field.Location.Bounds;
								Rect rect;
								if (pdfControl.TryGetClientRect(this.Field.Location.PageIndex, this.Field.Location.Bounds, out rect))
								{
									FS_RECTF mediaBox = pdfControl.Document.Pages[this.Field.Location.PageIndex].MediaBox;
									if (bounds.top > 0f && bounds.top < mediaBox.top && bounds.left > 0f && bounds.left < mediaBox.right)
									{
										double horizontalOffset = pdfControl.ScrollViewer.HorizontalOffset;
										double verticalOffset = pdfControl.ScrollViewer.VerticalOffset;
										pdfControl.ScrollViewer.ScrollToHorizontalOffset(horizontalOffset + rect.Left);
										pdfControl.ScrollViewer.ScrollToVerticalOffset(verticalOffset + rect.Top);
										pdfControl.UpdateDocLayout();
										pdfControl.UpdateLayout();
									}
								}
							}
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8A RID: 3466
			// (get) Token: 0x06003666 RID: 13926 RVA: 0x0011117C File Offset: 0x0010F37C
			public ICommand ValidateSignatureCommand
			{
				get
				{
					ICommand command;
					if ((command = this.validateSignatureCommand) == null)
					{
						command = (this.validateSignatureCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignature", "ValidateDigitalSignature", "LeftPanel", 1L);
							global::System.ValueTuple<bool, PdfSignatureValidationResult> valueTuple = await SignatureValidateHelper.ShowSignatureStatusDialogAsync(this.Field, false, true);
							bool item = valueTuple.Item1;
							PdfSignatureValidationResult item2 = valueTuple.Item2;
							if (item2 != null)
							{
								this.UpdateResultInfo();
								if (item)
								{
									GAManager.SendEvent("DigitalSignature", "ShowSignatureProperties", "ValidateDigitalSignature_LeftPanel", 1L);
									SignatureValidateHelper.ShowSignaturePropertiesDialog(this.Field, item2);
								}
							}
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8B RID: 3467
			// (get) Token: 0x06003667 RID: 13927 RVA: 0x001111B0 File Offset: 0x0010F3B0
			public ICommand ShowSignaturePropertiesCommand
			{
				get
				{
					ICommand command;
					if ((command = this.showSignaturePropertiesCommand) == null)
					{
						command = (this.showSignaturePropertiesCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignature", "ShowSignatureProperties", "LeftPanel_OnDigitalSignature", 1L);
							PdfSignatureValidationResult pdfSignatureValidationResult = await this.Field.ValidateSignatureAsync(false, true);
							if (pdfSignatureValidationResult != null)
							{
								this.UpdateResultInfo();
								SignatureValidateHelper.ShowSignaturePropertiesDialog(this.Field, pdfSignatureValidationResult);
							}
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8C RID: 3468
			// (get) Token: 0x06003668 RID: 13928 RVA: 0x001111E4 File Offset: 0x0010F3E4
			public ICommand ShowSignatureCertificateCommand
			{
				get
				{
					ICommand command;
					if ((command = this.showSignatureCertificateCommand) == null)
					{
						command = (this.showSignatureCertificateCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignature", "ShowCertificate", "LeftPanel", 1L);
							PdfSignatureValidationResult pdfSignatureValidationResult = await this.Field.ValidateSignatureAsync(true, true);
							if (pdfSignatureValidationResult != null && pdfSignatureValidationResult.Certificates.Count > 0)
							{
								this.UpdateResultInfo();
								IntPtr intPtr = IntPtr.Zero;
								Window mainWindow = App.Current.MainWindow;
								if (mainWindow != null)
								{
									HwndSource hwndSource = PresentationSource.FromVisual(mainWindow) as HwndSource;
									if (hwndSource != null)
									{
										intPtr = hwndSource.Handle;
									}
								}
								pdfSignatureValidationResult.Certificates[0].ShowCertificateDialog(intPtr, null);
							}
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8D RID: 3469
			// (get) Token: 0x06003669 RID: 13929 RVA: 0x00111218 File Offset: 0x0010F418
			public ICommand ValidateSignatureFirstTimeCommand
			{
				get
				{
					ICommand command;
					if ((command = this.validateSignatureFirstTimeCommand) == null)
					{
						command = (this.validateSignatureFirstTimeCommand = new AsyncRelayCommand(async delegate
						{
							await this.Field.ValidateSignatureAsync(true, true);
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8E RID: 3470
			// (get) Token: 0x0600366A RID: 13930 RVA: 0x0011124C File Offset: 0x0010F44C
			public ICommand RemoveSignatureCommand
			{
				get
				{
					ICommand command;
					if ((command = this.removeSignatureCommand) == null)
					{
						command = (this.removeSignatureCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignature", "RemoveDigitalSignature", "LeftPanel", 1L);
							await this.RemoveSignatureAsync();
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D8F RID: 3471
			// (get) Token: 0x0600366B RID: 13931 RVA: 0x00111280 File Offset: 0x0010F480
			public ICommand SignDeferredSignatureCommand
			{
				get
				{
					ICommand command;
					if ((command = this.signDeferredSignatureCommand) == null)
					{
						command = (this.signDeferredSignatureCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignatureFiled", "SignInDSFiled", "SignaturePannel", 1L);
							await Ioc.Default.GetRequiredService<MainViewModel>().AnnotationToolbar.SignDeferredDigitalSignature(this.Field.Location);
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D90 RID: 3472
			// (get) Token: 0x0600366C RID: 13932 RVA: 0x001112B4 File Offset: 0x0010F4B4
			public ICommand ShowDeferredSignaturePropertiesCommand
			{
				get
				{
					ICommand command;
					if ((command = this.showDeferredSignaturePropertiesCommand) == null)
					{
						command = (this.showDeferredSignaturePropertiesCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignatureFiled", "ShowFiledProperty", "SignaturePannel", 1L);
							PdfDigitalSignatureField field = this.Field;
							PdfDigitalSignatureLocation pdfDigitalSignatureLocation = ((field != null) ? field.Location : null);
							if (pdfDigitalSignatureLocation != null)
							{
								DigitalSignatureContextMenuHolder.ShowDeferrerSignProperties(pdfDigitalSignatureLocation);
							}
						}));
					}
					return command;
				}
			}

			// Token: 0x17000D91 RID: 3473
			// (get) Token: 0x0600366D RID: 13933 RVA: 0x001112E8 File Offset: 0x0010F4E8
			public ICommand RemoveDeferredSignatureCommand
			{
				get
				{
					ICommand command;
					if ((command = this.removeDeferredSignatureCommand) == null)
					{
						command = (this.removeDeferredSignatureCommand = new AsyncRelayCommand(async delegate
						{
							GAManager.SendEvent("DigitalSignatureFiled", "RemoveDSFiled", "SignaturePannel", 1L);
							await Ioc.Default.GetRequiredService<MainViewModel>().AnnotationToolbar.RemoveDeferredDigitalSignature(this.Field.Location);
						}));
					}
					return command;
				}
			}

			// Token: 0x0600366E RID: 13934 RVA: 0x0011131C File Offset: 0x0010F51C
			private async Task RemoveSignatureAsync()
			{
				MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
				DocumentWrapper documentWrapper = vm.DocumentWrapper;
				string text;
				if (documentWrapper == null)
				{
					text = null;
				}
				else
				{
					EncryptManage encryptManage = documentWrapper.EncryptManage;
					text = ((encryptManage != null) ? encryptManage.UserPassword : null);
				}
				string password = text;
				if (string.IsNullOrEmpty(password))
				{
					password = null;
				}
				MainViewModel.SaveResult saveResult = await this.digitalSignatureHelper.RemoveDigitalSignatureAndSaveAsAsync(this.Field, vm.CanSave);
				if (saveResult.FailedResult == MainViewModel.SaveFailedResult.Successed)
				{
					await vm.OpenDocumentCoreAsync(saveResult.File.FullName, password, true);
				}
				else
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.DigSigRemoveMessage_Failed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
			}

			// Token: 0x0600366F RID: 13935 RVA: 0x00111360 File Offset: 0x0010F560
			public void RaiseNameChanged()
			{
				PdfSignatureField signatureField = this.Field.Location.SignatureField;
				string text = ((signatureField != null) ? signatureField.FullName : null);
				if (string.IsNullOrEmpty(text))
				{
					text = pdfeditor.Properties.Resources.DigSignPanelDefaultName;
				}
				this.name = text;
				base.OnPropertyChanged("Name");
			}

			// Token: 0x040024BB RID: 9403
			private readonly DigitalSignatureHelper digitalSignatureHelper;

			// Token: 0x040024BC RID: 9404
			private string name;

			// Token: 0x040024BD RID: 9405
			private bool? isValid;

			// Token: 0x040024BE RID: 9406
			private string signValidatePropertiesText = "";

			// Token: 0x040024BF RID: 9407
			private string signPropertiesText = "";

			// Token: 0x040024C0 RID: 9408
			private ICommand goToFieldLocationCommand;

			// Token: 0x040024C1 RID: 9409
			private ICommand validateSignatureCommand;

			// Token: 0x040024C2 RID: 9410
			private ICommand showSignaturePropertiesCommand;

			// Token: 0x040024C3 RID: 9411
			private ICommand showSignatureCertificateCommand;

			// Token: 0x040024C4 RID: 9412
			private ICommand validateSignatureFirstTimeCommand;

			// Token: 0x040024C5 RID: 9413
			private ICommand removeSignatureCommand;

			// Token: 0x040024C6 RID: 9414
			private ICommand signDeferredSignatureCommand;

			// Token: 0x040024C7 RID: 9415
			private ICommand showDeferredSignaturePropertiesCommand;

			// Token: 0x040024C8 RID: 9416
			private ICommand removeDeferredSignatureCommand;
		}
	}
}
