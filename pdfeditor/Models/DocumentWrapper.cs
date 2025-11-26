using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Protection;
using pdfeditor.Models.Protection;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.DocumentOcr;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Models
{
	// Token: 0x02000127 RID: 295
	public class DocumentWrapper : ObservableObject, IDisposable
	{
		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x0600120A RID: 4618 RVA: 0x00049AE0 File Offset: 0x00047CE0
		public EncryptManage EncryptManage
		{
			get
			{
				EncryptManage encryptManage;
				if ((encryptManage = this.encryptManage) == null)
				{
					encryptManage = (this.encryptManage = new EncryptManage());
				}
				return encryptManage;
			}
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x0600120B RID: 4619 RVA: 0x00049B05 File Offset: 0x00047D05
		// (set) Token: 0x0600120C RID: 4620 RVA: 0x00049B13 File Offset: 0x00047D13
		public PdfDocument Document
		{
			get
			{
				this.ThrowIfDisposed();
				return this.document;
			}
			private set
			{
				base.SetProperty<PdfDocument>(ref this.document, value, "Document");
			}
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x0600120D RID: 4621 RVA: 0x00049B28 File Offset: 0x00047D28
		// (set) Token: 0x0600120E RID: 4622 RVA: 0x00049B30 File Offset: 0x00047D30
		public string DocumentPath
		{
			get
			{
				return this.documentPath;
			}
			private set
			{
				base.SetProperty<string>(ref this.documentPath, value, "DocumentPath");
			}
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x00049B45 File Offset: 0x00047D45
		// (set) Token: 0x06001210 RID: 4624 RVA: 0x00049B4D File Offset: 0x00047D4D
		public PdfContentType DocumentContentType
		{
			get
			{
				return this.documentContentType;
			}
			private set
			{
				base.SetProperty<PdfContentType>(ref this.documentContentType, value, "DocumentContentType");
			}
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06001211 RID: 4625 RVA: 0x00049B62 File Offset: 0x00047D62
		public PdfDocumentMetadata Metadata
		{
			get
			{
				return this.pdfMetadata;
			}
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x00049B6A File Offset: 0x00047D6A
		// (set) Token: 0x06001213 RID: 4627 RVA: 0x00049B72 File Offset: 0x00047D72
		public DigitalSignatureHelper DigitalSignatureHelper
		{
			get
			{
				return this.digitalSignatureHelper;
			}
			private set
			{
				base.SetProperty<DigitalSignatureHelper>(ref this.digitalSignatureHelper, value, "DigitalSignatureHelper");
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x00049B87 File Offset: 0x00047D87
		public bool IsOpening
		{
			get
			{
				return this.cts != null;
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x00049B92 File Offset: 0x00047D92
		// (set) Token: 0x06001216 RID: 4630 RVA: 0x00049B9A File Offset: 0x00047D9A
		public bool IsUntitledFile
		{
			get
			{
				return this.isUntitledFile;
			}
			private set
			{
				base.SetProperty<bool>(ref this.isUntitledFile, value, "IsUntitledFile");
			}
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00049BB0 File Offset: 0x00047DB0
		public async Task<bool> OpenAsync(string filePath, string password = null)
		{
			this.TryCancel();
			this.ThrowIfDisposed();
			this.cts = new CancellationTokenSource();
			base.OnPropertyChanged("IsOpening");
			this.EncryptManage.Init();
			try
			{
				global::System.ValueTuple<PdfDocument, FileStream, Stream> valueTuple = await this.OpenCoreAsync(filePath, password, this.cts.Token);
				PdfDocument item = valueTuple.Item1;
				FileStream item2 = valueTuple.Item2;
				Stream item3 = valueTuple.Item3;
				if (item != null)
				{
					PdfDocument pdfDocument = this.document;
					FileStream fileStream = this.sourceFileStream;
					Stream stream = this.openDocStream;
					DigitalSignatureHelper digitalSignatureHelper = this.DigitalSignatureHelper;
					try
					{
						this.Document = item;
						this.sourceFileStream = item2;
						this.openDocStream = item3;
						this.DocumentPath = filePath.FullPathWithoutPrefix;
						this.DocumentContentType = PdfContentType.Text;
						this.IsUntitledFile = false;
						this.pdfMetadata = new PdfDocumentMetadata(this.document);
						this.DigitalSignatureHelper = new DigitalSignatureHelper(item, filePath, this.EncryptManage.UserPassword);
						this.searchableDocumentHelper = new SearchableDocumentHelper(filePath, this.EncryptManage.UserPassword);
						this.<OpenAsync>g__UpdateSearchableDocumentProperties|34_0(this.searchableDocumentHelper);
						this.EncryptManage.IsHaveOwerPassword = this.Document.SecurityRevision > 0;
						this.EncryptManage.IsRequiredOwerPassword = !Pdfium.FPDF_IsOwnerPasswordIsUsed(this.Document.Handle);
						if (!this.EncryptManage.IsRequiredOwerPassword)
						{
							this.EncryptManage.UpdateOwerPassword(this.EncryptManage.UserPassword);
						}
						DigitalSignatureHelper digitalSignatureHelper2 = this.DigitalSignatureHelper;
						if (((digitalSignatureHelper2 != null) ? new bool?(digitalSignatureHelper2.HasDigitalSignatures) : null) ?? false)
						{
							string text = "DigitalSignature";
							string text2 = "CheckHasDigitalSignature";
							DigitalSignatureHelper digitalSignatureHelper3 = this.DigitalSignatureHelper;
							int? num;
							if (digitalSignatureHelper3 == null)
							{
								num = null;
							}
							else
							{
								global::System.Collections.Generic.IReadOnlyList<PdfDigitalSignatureLocation> locations = digitalSignatureHelper3.Locations;
								num = ((locations != null) ? new int?(locations.Count) : null);
							}
							int? num2 = num;
							GAManager.SendEvent(text, text2, num2.GetValueOrDefault().ToString(), 1L);
						}
						this.SendFormData(item);
						return true;
					}
					finally
					{
						this.disposeQueue.Enqueue(pdfDocument);
						this.disposeQueue.Enqueue(stream);
						this.disposeQueue.Enqueue(fileStream);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				CancellationTokenSource cancellationTokenSource = this.cts;
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Dispose();
				}
				this.cts = null;
				base.OnPropertyChanged("IsOpening");
			}
			return false;
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00049C03 File Offset: 0x00047E03
		public void TryCancel()
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this.cts = null;
			base.OnPropertyChanged("IsOpening");
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x00049C28 File Offset: 0x00047E28
		public void CloseDocument()
		{
			this.TryCancel();
			PdfDocument pdfDocument = this.document;
			FileStream fileStream = this.sourceFileStream;
			Stream stream = this.openDocStream;
			if (pdfDocument != null)
			{
				this.disposeQueue.Enqueue(pdfDocument);
			}
			if (stream != null)
			{
				this.disposeQueue.Enqueue(stream);
			}
			if (fileStream != null)
			{
				this.disposeQueue.Enqueue(fileStream);
			}
			if (this.DigitalSignatureHelper != null)
			{
				this.disposeQueue.Enqueue(this.DigitalSignatureHelper);
			}
			this.Document = null;
			this.sourceFileStream = null;
			this.openDocStream = null;
			this.DigitalSignatureHelper = null;
			this.pdfMetadata = null;
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x00049CBC File Offset: 0x00047EBC
		public void ShowEncryptWindow()
		{
			if (this.document == null)
			{
				return;
			}
			EncryptWindow encryptWindow = new EncryptWindow(this);
			if (encryptWindow != null)
			{
				encryptWindow.Owner = (MainView)Application.Current.MainWindow;
				encryptWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				encryptWindow.ShowDialog();
			}
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x00049D00 File Offset: 0x00047F00
		public void ShowDecryptWindow()
		{
			if (this.document == null)
			{
				return;
			}
			if ((!this.EncryptManage.IsHaveUserPassword && !this.EncryptManage.IsHaveOwerPassword) || this.EncryptManage.IsRemoveAllPassword)
			{
				ModernMessageBox.Show(Resources.WinPwdRemoveCheckContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (ModernMessageBox.Show(Resources.WinPwdRemoveConfirmContent, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
			{
				GAManager.SendEvent("Password", "RemovePassword", "Count", 1L);
				this.EncryptManage.RemoveAllPassword();
				this.EncryptManage.IsHaveOwerPassword = false;
				this.EncryptManage.IsHaveUserPassword = false;
				Ioc.Default.GetRequiredService<MainViewModel>().CanSave = true;
			}
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00049DB1 File Offset: 0x00047FB1
		public void SetUntitledFile()
		{
			this.IsUntitledFile = true;
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00049DBC File Offset: 0x00047FBC
		public void TrimMemory()
		{
			if (this.IsOpening)
			{
				return;
			}
			while (this.disposeQueue.Count > 0)
			{
				IDisposable disposable = this.disposeQueue.Dequeue();
				try
				{
					PdfDocument pdfDocument = disposable as PdfDocument;
					if (pdfDocument != null && !pdfDocument.IsDisposed)
					{
						try
						{
							if (!pdfDocument.IsDisposed)
							{
								for (int i = pdfDocument.Pages.Count - 1; i >= 0; i--)
								{
									try
									{
										PageDisposeHelper.DisposePage(pdfDocument.Pages[i]);
									}
									catch
									{
									}
								}
							}
						}
						catch
						{
						}
						pdfDocument.Dispose();
					}
					else if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x00049E78 File Offset: 0x00048078
		public async Task<bool> SaveAsync()
		{
			bool flag;
			if (this.IsOpening)
			{
				flag = false;
			}
			else if (this.Document == null || this.sourceFileStream == null)
			{
				flag = false;
			}
			else
			{
				long length = this.sourceFileStream.Length;
				flag = await Task.Run<bool>(TaskExceptionHelper.ExceptionBoundary<bool>(delegate
				{
					bool flag4;
					try
					{
						FileInfo fileInfo = this.documentPath.FileInfo;
						if (!fileInfo.Exists)
						{
							throw new FileNotFoundException(this.documentPath);
						}
						if (fileInfo.IsReadOnly)
						{
							throw new UnauthorizedAccessException();
						}
						DigitalSignatureHelper digitalSignatureHelper = this.DigitalSignatureHelper;
						bool? flag2;
						if (digitalSignatureHelper == null)
						{
							flag2 = null;
						}
						else
						{
							global::System.Collections.Generic.IReadOnlyList<PdfDigitalSignatureLocation> locations = digitalSignatureHelper.Locations;
							if (locations == null)
							{
								flag2 = null;
							}
							else
							{
								flag2 = new bool?(locations.Any((PdfDigitalSignatureLocation c) => c.HasSigned));
							}
						}
						bool? flag3 = flag2;
						if (flag3 != null)
						{
							flag3.GetValueOrDefault();
						}
						DigitalSignatureHelper digitalSignatureHelper2 = this.DigitalSignatureHelper;
						if (digitalSignatureHelper2 != null)
						{
							digitalSignatureHelper2.Dispose();
						}
						this.DigitalSignatureHelper = null;
						this.sourceFileStream.Dispose();
						this.sourceFileStream.Close();
						this.sourceFileStream = null;
						FileStream fileStream = null;
						try
						{
							for (int i = 0; i < 3; i++)
							{
								try
								{
									fileStream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
									break;
								}
								catch (IOException)
								{
									if (i == 2)
									{
										throw;
									}
									Thread.Sleep(100);
								}
							}
							if (fileStream != null)
							{
								if (fileStream.CanSeek)
								{
									fileStream.Seek(0L, SeekOrigin.Begin);
								}
								if (this.EncryptManage.IsRemoveAllPassword)
								{
									this.document.Save(fileStream, SaveFlags.Incremental | SaveFlags.NoIncremental | SaveFlags.RemoveUnusedObjects, 0);
								}
								else
								{
									this.document.SetPasswordProtection(this.EncryptManage.UserPassword, this.EncryptManage.OwerPassword, (PdfUserAccessPermission)4294967292U, true, EncriptionAlgorithm.AES128);
									this.document.Save(fileStream, SaveFlags.NoIncremental | SaveFlags.RemoveUnusedObjects, 0);
								}
								fileStream.SetLength(fileStream.Position);
							}
						}
						finally
						{
							if (fileStream != null)
							{
								fileStream.Close();
							}
							if (fileStream != null)
							{
								fileStream.Dispose();
							}
							fileStream = null;
							for (int j = 0; j < 3; j++)
							{
								try
								{
									this.sourceFileStream = this.CreateFileStream(fileInfo);
									break;
								}
								catch (IOException)
								{
									if (j < 2)
									{
										Thread.Sleep(100);
									}
								}
							}
						}
						if (this.sourceFileStream == null)
						{
							EventHandler fileError = this.FileError;
							if (fileError != null)
							{
								fileError(this, EventArgs.Empty);
							}
							flag4 = false;
						}
						else
						{
							this.DigitalSignatureHelper = new DigitalSignatureHelper(this.Document, this.DocumentPath, this.EncryptManage.UserPassword);
							flag4 = true;
						}
					}
					catch (Exception ex)
					{
						Log.WriteLog(string.Format("SaveAsync failed: {0}", ex));
						flag4 = false;
					}
					return flag4;
				})).ConfigureAwait(false);
			}
			return flag;
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00049EBC File Offset: 0x000480BC
		public async Task<bool> SaveToStreamAsync(Stream stream, bool removeExistsDigitalSignatures = false)
		{
			DocumentWrapper.<>c__DisplayClass42_0 CS$<>8__locals1 = new DocumentWrapper.<>c__DisplayClass42_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.stream = stream;
			bool flag;
			if (CS$<>8__locals1.stream == null)
			{
				flag = false;
			}
			else if (this.IsOpening)
			{
				flag = false;
			}
			else if (this.Document == null || this.sourceFileStream == null)
			{
				flag = false;
			}
			else
			{
				DocumentWrapper.<>c__DisplayClass42_0 CS$<>8__locals2 = CS$<>8__locals1;
				bool flag3;
				if (!removeExistsDigitalSignatures)
				{
					DigitalSignatureHelper digitalSignatureHelper = this.DigitalSignatureHelper;
					bool? flag2;
					if (digitalSignatureHelper == null)
					{
						flag2 = null;
					}
					else
					{
						global::System.Collections.Generic.IReadOnlyList<PdfDigitalSignatureLocation> locations = digitalSignatureHelper.Locations;
						if (locations == null)
						{
							flag2 = null;
						}
						else
						{
							flag2 = new bool?(locations.Any((PdfDigitalSignatureLocation c) => c.HasSigned));
						}
					}
					flag3 = flag2 ?? false;
				}
				else
				{
					flag3 = false;
				}
				CS$<>8__locals2.hasSignature = flag3;
				flag = await Task.Run<bool>(() => CS$<>8__locals1.<>4__this.SaveToStreamCore(CS$<>8__locals1.stream, CS$<>8__locals1.hasSignature)).ConfigureAwait(false);
			}
			return flag;
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x00049F10 File Offset: 0x00048110
		private bool SaveToStreamCore(Stream stream, bool needIncremental)
		{
			if (stream == null)
			{
				return false;
			}
			bool flag;
			try
			{
				if (stream.CanSeek)
				{
					stream.Seek(0L, SeekOrigin.Begin);
				}
				if (!needIncremental)
				{
					PdfDocumentUtils.RemoveUnusedObjects(this.document);
				}
				SaveFlags saveFlags = (SaveFlags)0;
				if (this.EncryptManage.IsRemoveAllPassword)
				{
					saveFlags |= SaveFlags.NoIncremental;
					saveFlags |= SaveFlags.RemoveSecurity;
				}
				else if (!string.IsNullOrEmpty(this.EncryptManage.UserPassword) || !string.IsNullOrEmpty(this.EncryptManage.OwerPassword))
				{
					this.document.SetPasswordProtection(this.EncryptManage.UserPassword, this.EncryptManage.OwerPassword, (PdfUserAccessPermission)4294967292U, true, EncriptionAlgorithm.AES128);
				}
				if (needIncremental)
				{
					saveFlags |= SaveFlags.Incremental;
				}
				else
				{
					saveFlags |= SaveFlags.NoIncremental;
					saveFlags |= SaveFlags.RemoveUnusedObjects;
				}
				this.Document.Save(stream, saveFlags, 0);
				stream.SetLength(stream.Position);
				flag = true;
			}
			catch (Exception ex)
			{
				Log.WriteLog(string.Format("SaveToStreamCore failed: {0}", ex));
				flag = false;
			}
			return flag;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x00049FFC File Offset: 0x000481FC
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "document", "sourceFileStream", "openDocStream" })]
		private async Task<global::System.ValueTuple<PdfDocument, FileStream, Stream>> OpenCoreAsync(string filePath, string password, CancellationToken cancellationToken)
		{
			DocumentWrapper.<>c__DisplayClass44_0 CS$<>8__locals1 = new DocumentWrapper.<>c__DisplayClass44_0();
			CS$<>8__locals1.filePath = filePath;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.password = password;
			if (string.IsNullOrEmpty(CS$<>8__locals1.filePath))
			{
				throw new ArgumentException("filePath");
			}
			CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
			return await Task.Run<global::System.ValueTuple<PdfDocument, FileStream, Stream>>(TaskExceptionHelper.ExceptionBoundary<global::System.ValueTuple<PdfDocument, FileStream, Stream>>(delegate
			{
				DocumentWrapper.<>c__DisplayClass44_0.<<OpenCoreAsync>b__0>d <<OpenCoreAsync>b__0>d;
				<<OpenCoreAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<global::System.ValueTuple<PdfDocument, FileStream, Stream>>.Create();
				<<OpenCoreAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<OpenCoreAsync>b__0>d.<>1__state = -1;
				<<OpenCoreAsync>b__0>d.<>t__builder.Start<DocumentWrapper.<>c__DisplayClass44_0.<<OpenCoreAsync>b__0>d>(ref <<OpenCoreAsync>b__0>d);
				return <<OpenCoreAsync>b__0>d.<>t__builder.Task;
			}), CS$<>8__locals1.cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0004A057 File Offset: 0x00048257
		private FileStream CreateFileStream(FileInfo fileInfo)
		{
			if (fileInfo == null)
			{
				throw new ArgumentNullException("fileInfo");
			}
			return fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0004A070 File Offset: 0x00048270
		private async Task<PdfDocument> OpenCoreAsync(Stream stream, string password, CancellationToken cancellationToken)
		{
			SynchronizationContext synchronizationContext = await this.GetUIThreadContext().WaitAsync(cancellationToken).ConfigureAwait(false);
			PdfForms pdfForms = new PdfForms
			{
				SynchronizationContext = synchronizationContext
			};
			PdfDocument pdfDocument = null;
			try
			{
				pdfDocument = PdfDocument.Load(stream, pdfForms, password, true);
			}
			finally
			{
				try
				{
					if (pdfDocument == null && pdfForms != null && !pdfForms.IsDisposed)
					{
						pdfForms.Dispose();
						pdfForms = null;
					}
				}
				catch
				{
				}
			}
			return pdfDocument;
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x0004A0CC File Offset: 0x000482CC
		private async Task<SynchronizationContext> GetUIThreadContext()
		{
			TaskCompletionSource<SynchronizationContext> tcs = new TaskCompletionSource<SynchronizationContext>();
			await DispatcherHelper.RunAsync(delegate
			{
				tcs.SetResult(SynchronizationContext.Current);
			});
			return await tcs.Task;
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06001225 RID: 4645 RVA: 0x0004A108 File Offset: 0x00048308
		// (remove) Token: 0x06001226 RID: 4646 RVA: 0x0004A140 File Offset: 0x00048340
		public event EventHandler<DocumentPasswordRequestedEventArgs> PasswordRequested;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06001227 RID: 4647 RVA: 0x0004A178 File Offset: 0x00048378
		// (remove) Token: 0x06001228 RID: 4648 RVA: 0x0004A1B0 File Offset: 0x000483B0
		public event EventHandler FileError;

		// Token: 0x06001229 RID: 4649 RVA: 0x0004A1E8 File Offset: 0x000483E8
		private bool OnPasswordRequested(out string password, string filepath)
		{
			password = string.Empty;
			DocumentPasswordRequestedEventArgs args = new DocumentPasswordRequestedEventArgs();
			args.FileName = filepath;
			this.EncryptManage.IsHaveUserPassword = true;
			DispatcherHelper.UIDispatcher.Invoke(delegate
			{
				EventHandler<DocumentPasswordRequestedEventArgs> passwordRequested = this.PasswordRequested;
				if (passwordRequested == null)
				{
					return;
				}
				passwordRequested(this, args);
			});
			password = args.Password;
			return !args.Cancel;
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0004A25D File Offset: 0x0004845D
		public void ReloadDigitalSignatureHelper()
		{
			if (this.Document == null)
			{
				return;
			}
			DigitalSignatureHelper digitalSignatureHelper = this.DigitalSignatureHelper;
			this.DigitalSignatureHelper = new DigitalSignatureHelper(this.Document, this.DocumentPath, this.EncryptManage.UserPassword);
			if (digitalSignatureHelper == null)
			{
				return;
			}
			digitalSignatureHelper.Dispose();
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0004A29C File Offset: 0x0004849C
		private void SendFormData(PdfDocument document)
		{
			try
			{
				bool flag;
				if (document == null)
				{
					flag = null != null;
				}
				else
				{
					PdfForms formFill = document.FormFill;
					flag = ((formFill != null) ? formFill.InterForm : null) != null;
				}
				if (flag)
				{
					if (document.FormFill.InterForm.HasXFAForm)
					{
						GAManager.SendEvent("MainWindow", "OpenDocumentPDFForm", "XFA", 1L);
					}
					PdfControlCollections controls = document.FormFill.InterForm.Controls;
					if (controls == null || controls.Count <= 0)
					{
						PdfFieldCollections fields = document.FormFill.InterForm.Fields;
						if (fields == null || fields.Count <= 0)
						{
							goto IL_009D;
						}
					}
					GAManager.SendEvent("MainWindow", "OpenDocumentPDFForm", "PDFForm", 1L);
				}
				IL_009D:;
			}
			catch
			{
			}
			try
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty((document != null) ? document.Creator : null))
				{
					text = text + "Creator: " + ((document != null) ? document.Creator : null);
				}
				if (!string.IsNullOrEmpty((document != null) ? document.Producer : null))
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "; ";
					}
					text = text + "Producer: " + document.Producer;
				}
			}
			catch
			{
			}
			if (this.EncryptManage.IsHaveUserPassword || this.EncryptManage.IsHaveOwerPassword)
			{
				if (this.EncryptManage.IsHaveUserPassword && !this.EncryptManage.IsHaveOwerPassword)
				{
					GAManager.SendEvent("MainWindow", "Password", "OnlyUserPassword", 1L);
					return;
				}
				if (!this.EncryptManage.IsHaveUserPassword && this.EncryptManage.IsHaveOwerPassword)
				{
					GAManager.SendEvent("MainWindow", "Password", "OnlyOwerPassword", 1L);
					return;
				}
				if (this.EncryptManage.IsHaveUserPassword && this.EncryptManage.IsHaveOwerPassword)
				{
					GAManager.SendEvent("MainWindow", "Password", "UserOwerPassword", 1L);
				}
			}
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x0004A488 File Offset: 0x00048688
		private void SendAnnotationData(PdfDocument document, string filePath, bool traceAllData)
		{
			try
			{
				this.SendAnnotationDataCore(document);
			}
			catch
			{
			}
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0004A4B4 File Offset: 0x000486B4
		private void SendAnnotationDataCore(PdfDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			foreach (global::System.ValueTuple<string, int> valueTuple in new AnnotationTrace(document).GetAnnotationTypeTraceObjectAllPages())
			{
				string item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				GAManager.SendEvent("DocumentAnnotations", item, "Count", (long)item2);
			}
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x0004A510 File Offset: 0x00048710
		private void SendAnnotationDataCore(PdfDocument document, string filePath, bool traceAllData)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			string text = "";
			try
			{
				text = document.Title;
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text))
			{
				try
				{
					text = new FileInfo(filePath).Name;
				}
				catch
				{
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "no title";
			}
			AnnotationTrace annotationTrace = new AnnotationTrace(document);
			object[] array;
			if (traceAllData)
			{
				array = annotationTrace.GetAnnotationModelTraceObject();
			}
			else
			{
				array = annotationTrace.GetAnnotationTypeTraceObject();
			}
			if (array == null || array.Length == 0)
			{
				JsonConvert.SerializeObject(new
				{
					pdfTitle = text,
					annotTracePageCount = 0,
					maxAnnotTracePageCount = 20,
					annots = "empty"
				});
				return;
			}
			JsonConvert.SerializeObject(new
			{
				pdfTitle = text,
				annotTracePageCount = array.Length,
				maxAnnotTracePageCount = 20,
				annots = array
			});
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0004A5CC File Offset: 0x000487CC
		private async Task SendPdfFileVersion(Stream stream, CancellationToken stopToken, string sourceFrom)
		{
			if (stream != null)
			{
				try
				{
					PdfFileInformationHelper.PdfVersion? pdfVersion = await PdfFileInformationHelper.GetPdfVersionAsync(stream, stopToken);
					if (pdfVersion != null)
					{
						GAManager.SendEvent("OpenPdfFileFail", "PdfFileVersion" + sourceFrom, pdfVersion.Value.ToString(), 1L);
					}
					else
					{
						GAManager.SendEvent("OpenPdfFileFail", "PdfFileVersion" + sourceFrom, "NotPDF", 1L);
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0004A61F File Offset: 0x0004881F
		protected void ThrowIfDisposed()
		{
			if (this.disposedValue)
			{
				throw new ObjectDisposedException("DocumentWrapper");
			}
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x0004A634 File Offset: 0x00048834
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					try
					{
						this.TryCancel();
						this.TrimMemory();
						PdfDocument pdfDocument = this.document;
						FileStream fileStream = this.sourceFileStream;
						this.Document = null;
						if (pdfDocument != null)
						{
							pdfDocument.Dispose();
						}
						if (fileStream != null)
						{
							fileStream.Dispose();
						}
					}
					catch
					{
					}
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0004A6A0 File Offset: 0x000488A0
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0004A6C4 File Offset: 0x000488C4
		[CompilerGenerated]
		private async void <OpenAsync>g__UpdateSearchableDocumentProperties|34_0(SearchableDocumentHelper _helper)
		{
			try
			{
				PdfContentType pdfContentType = await _helper.GetDocumentContentTypeAsync();
				this.DocumentContentType = pdfContentType;
			}
			catch (Exception ex)
			{
				Log.WriteLog(string.Format("UpdateSearchableDocumentProperties failed: {0}", ex));
				this.DocumentContentType = PdfContentType.Text;
			}
			if (this.DocumentContentType != PdfContentType.Text)
			{
				GAManager.SendEvent("MainWindow", "DocumentContentType", this.DocumentContentType.ToString(), 1L);
			}
		}

		// Token: 0x040005B5 RID: 1461
		private FileStream sourceFileStream;

		// Token: 0x040005B6 RID: 1462
		private Stream openDocStream;

		// Token: 0x040005B7 RID: 1463
		private PdfDocument document;

		// Token: 0x040005B8 RID: 1464
		private DigitalSignatureHelper digitalSignatureHelper;

		// Token: 0x040005B9 RID: 1465
		private CancellationTokenSource cts;

		// Token: 0x040005BA RID: 1466
		private bool disposedValue;

		// Token: 0x040005BB RID: 1467
		private string documentPath;

		// Token: 0x040005BC RID: 1468
		private EncryptManage encryptManage;

		// Token: 0x040005BD RID: 1469
		private PdfDocumentMetadata pdfMetadata;

		// Token: 0x040005BE RID: 1470
		private PdfContentType documentContentType;

		// Token: 0x040005BF RID: 1471
		private SearchableDocumentHelper searchableDocumentHelper;

		// Token: 0x040005C0 RID: 1472
		private bool isUntitledFile;

		// Token: 0x040005C1 RID: 1473
		private Queue<IDisposable> disposeQueue = new Queue<IDisposable>();
	}
}
