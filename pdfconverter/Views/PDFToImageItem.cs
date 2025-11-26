using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfconverter.Properties;
using PDFKit.Utils;

namespace pdfconverter.Views
{
	// Token: 0x0200002D RID: 45
	internal class PDFToImageItem : ConvertFileItem
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0000A962 File Offset: 0x00008B62
		public PDFToImageItem(string file)
			: base(file)
		{
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000A96B File Offset: 0x00008B6B
		// (set) Token: 0x0600028A RID: 650 RVA: 0x0000A986 File Offset: 0x00008B86
		public string FailedReason
		{
			get
			{
				if (string.IsNullOrEmpty(this.failedReason))
				{
					return Resources.PDFToImageConvertFaileError;
				}
				return this.failedReason;
			}
			set
			{
				this.failedReason = value;
				base.RaisePropertyChanged("FailedReason");
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600028B RID: 651 RVA: 0x0000A99A File Offset: 0x00008B9A
		// (set) Token: 0x0600028C RID: 652 RVA: 0x0000A9A4 File Offset: 0x00008BA4
		public string PageRange
		{
			get
			{
				return this.pageRange;
			}
			set
			{
				IReadOnlyList<PdfPageRange> readOnlyList;
				int num;
				if (PdfPageRange.TryParsePageRange(value, new PdfPageRange.PageRangeParseOptions
				{
					PageCount = base.PageCount,
					BaseIndex = 1
				}, out readOnlyList, out num))
				{
					this.pageRange = value;
					base.RaisePropertyChanged("PageRange");
					return;
				}
				ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600028D RID: 653 RVA: 0x0000AA04 File Offset: 0x00008C04
		// (set) Token: 0x0600028E RID: 654 RVA: 0x0000AA0C File Offset: 0x00008C0C
		public string PageRangePlaceholder
		{
			get
			{
				return this.pageRangePlaceholder;
			}
			set
			{
				this.pageRangePlaceholder = value;
				base.RaisePropertyChanged("PageRangePlaceholder");
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000AA20 File Offset: 0x00008C20
		public string FileNameWithoutExtenstion
		{
			get
			{
				if (!string.IsNullOrEmpty(base.convertFile))
				{
					return Path.GetFileNameWithoutExtension(base.convertFile);
				}
				return null;
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000AA3C File Offset: 0x00008C3C
		public void ParseFile(string password, Action action)
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				this.ConvertStatus = FileCovertStatus.ConvertLoading;
				try
				{
					int result = PdfiumNetHelper.GetPageCountAsync(this.convertFile, password).GetAwaiter().GetResult();
					if (result > 0)
					{
						this.PageCount = result;
						this.PageTo = result;
						this.PageFrom = 1;
						this.FileSelected = new bool?(true);
						this.ConvertStatus = FileCovertStatus.ConvertLoaded;
						this.PassWord = password;
						this.PageRange = ((result == 1) ? "1" : string.Format("1-{0}", result));
						this.PageRangePlaceholder = Resources.WinMergeSplitSplitModeCustomRangeHelpAsgMsg;
					}
					else
					{
						this.PageCount = 0;
						this.FileSelected = new bool?(false);
						this.ConvertStatus = FileCovertStatus.ConvertUnsupport;
					}
				}
				catch (Exception)
				{
					this.PageCount = 0;
					this.FileSelected = new bool?(false);
					this.ConvertStatus = FileCovertStatus.ConvertUnsupport;
				}
				finally
				{
					action();
				}
			}));
		}

		// Token: 0x04000181 RID: 385
		private string failedReason;

		// Token: 0x04000182 RID: 386
		private string pageRange;

		// Token: 0x04000183 RID: 387
		private string pageRangePlaceholder;
	}
}
