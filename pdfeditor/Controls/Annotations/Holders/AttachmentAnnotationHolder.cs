using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B0 RID: 688
	public class AttachmentAnnotationHolder : BaseAnnotationHolder<PdfFileAttachmentAnnotation>
	{
		// Token: 0x060027C9 RID: 10185 RVA: 0x000BB195 File Offset: 0x000B9395
		public AttachmentAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x060027CA RID: 10186 RVA: 0x000BB19E File Offset: 0x000B939E
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x000BB1A1 File Offset: 0x000B93A1
		public override void OnPageClientBoundsChanged()
		{
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x000BB1A3 File Offset: 0x000B93A3
		public override bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x000BB1A6 File Offset: 0x000B93A6
		protected override void OnCancel()
		{
			IInputElement captured = Mouse.Captured;
			if (captured == null)
			{
				return;
			}
			captured.ReleaseMouseCapture();
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x000BB1B8 File Offset: 0x000B93B8
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfFileAttachmentAnnotation>> OnCompleteCreateNewAsync()
		{
			try
			{
				MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
				FS_POINTF positionFromDocument = vm.ViewerOperationModel.PositionFromDocument;
				PdfPage currentPage = base.CurrentPage;
				if (currentPage.Annots == null)
				{
					currentPage.CreateAnnotations();
				}
				GAManager.SendEvent("PDFAttachment", "Add", "Viewer", 1L);
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					ShowReadOnly = false,
					ReadOnlyChecked = true
				};
				if (openFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
				{
					PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = await AttachmentAnnotationHolder.AddPDFFileAttachmentAnnotation(vm, currentPage, openFileDialog.FileName, positionFromDocument);
					vm.AnnotationMode = AnnotationMode.None;
					if (pdfFileAttachmentAnnotation != null)
					{
						return new PdfFileAttachmentAnnotation[] { pdfFileAttachmentAnnotation };
					}
				}
				vm.AnnotationMode = AnnotationMode.None;
				vm = null;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x000BB1FC File Offset: 0x000B93FC
		public static async Task<PdfFileAttachmentAnnotation> AddPDFFileAttachmentAnnotation(MainViewModel VM, PdfPage page, string filePath, FS_POINTF point)
		{
			try
			{
				LongPathFile longPathFile = filePath;
				if (longPathFile.IsExists)
				{
					string fileName = Path.GetFileName(longPathFile);
					if (longPathFile.FileInfo.Length > (long)((ulong)(-2147483648)))
					{
						ModernMessageBox.Show(Resources.Msg_AttachmentToolLarge.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return null;
					}
					byte[] array = File.ReadAllBytes(longPathFile);
					point.X -= 7f;
					PdfFileAttachmentAnnotation attachmentAnnotation = new PdfFileAttachmentAnnotation(page, FileIconNames.PushPin, Path.GetFileName(longPathFile), array, point.X, point.Y);
					string lastWriteTime = AttachmentFileUtils.GetLastWriteTime(longPathFile);
					attachmentAnnotation.FileSpecification.EmbeddedFile.ModificationDate = lastWriteTime;
					attachmentAnnotation.ModificationDate = lastWriteTime;
					attachmentAnnotation.CreationDate = lastWriteTime;
					attachmentAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
					page.Annots.Add(attachmentAnnotation);
					VM.PageEditors.NotifyAttachmentChanged();
					await VM.OperationManager.TraceAnnotationInsertAsync(attachmentAnnotation, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
					return await Task.FromResult<PdfFileAttachmentAnnotation>(attachmentAnnotation);
				}
			}
			catch (Exception ex)
			{
				ModernMessageBox.Show(ex.Message ?? "", UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			return null;
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000BB257 File Offset: 0x000B9457
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x000BB259 File Offset: 0x000B9459
		protected override bool OnSelecting(PdfFileAttachmentAnnotation annotation, bool afterCreate)
		{
			return true;
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x000BB25C File Offset: 0x000B945C
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			return true;
		}
	}
}
