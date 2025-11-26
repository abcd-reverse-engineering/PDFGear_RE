using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Printer;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Models.Print
{
	// Token: 0x0200014A RID: 330
	public class BatchPrintItemModel : ObservableObject, IDisposable
	{
		// Token: 0x06001394 RID: 5012 RVA: 0x0004EF94 File Offset: 0x0004D194
		public BatchPrintItemModel(string filePath, PdfDocument doc)
		{
			this.FilePath = filePath;
			this.Document = doc;
			this.FileName = Path.GetFileName(filePath);
			this.pageRangeMode = BatchPrintItemModel.PageRangeEnum.SelectedPages;
			this.subsetMode = BatchPrintItemModel.SubsetEnum.AllPages;
			this.pageRange = ((doc.Pages.Count == 1) ? "1" : string.Format("1-{0}", doc.Pages.Count));
			this.DocumentTotalPageCount = doc.Pages.Count;
			this.UpdateSelectedPageCount();
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0004F030 File Offset: 0x0004D230
		public BatchPrintItemModel(DocumentWrapper wrapper)
			: this(Path.GetFileName(wrapper.DocumentPath), wrapper.Document)
		{
			this.DocumentWrapper = wrapper;
			this.filePath = wrapper.DocumentPath;
		}

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x0004F05C File Offset: 0x0004D25C
		// (set) Token: 0x06001397 RID: 5015 RVA: 0x0004F064 File Offset: 0x0004D264
		public PdfDocument Document { get; private set; }

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x06001398 RID: 5016 RVA: 0x0004F06D File Offset: 0x0004D26D
		// (set) Token: 0x06001399 RID: 5017 RVA: 0x0004F075 File Offset: 0x0004D275
		public DocumentWrapper DocumentWrapper { get; private set; }

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0004F07E File Offset: 0x0004D27E
		// (set) Token: 0x0600139B RID: 5019 RVA: 0x0004F086 File Offset: 0x0004D286
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				base.SetProperty<string>(ref this.fileName, value, "FileName");
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x0600139C RID: 5020 RVA: 0x0004F09B File Offset: 0x0004D29B
		// (set) Token: 0x0600139D RID: 5021 RVA: 0x0004F0A3 File Offset: 0x0004D2A3
		public string FilePath
		{
			get
			{
				return this.filePath;
			}
			set
			{
				base.SetProperty<string>(ref this.filePath, value, "FilePath");
			}
		}

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x0600139E RID: 5022 RVA: 0x0004F0B8 File Offset: 0x0004D2B8
		// (set) Token: 0x0600139F RID: 5023 RVA: 0x0004F0C0 File Offset: 0x0004D2C0
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelected, value, "IsSelected");
			}
		}

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x060013A0 RID: 5024 RVA: 0x0004F0D5 File Offset: 0x0004D2D5
		public int DocumentTotalPageCount { get; }

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x060013A1 RID: 5025 RVA: 0x0004F0DD File Offset: 0x0004D2DD
		// (set) Token: 0x060013A2 RID: 5026 RVA: 0x0004F0E5 File Offset: 0x0004D2E5
		public int SelectedPageCount
		{
			get
			{
				return this.selectedPageCount;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedPageCount, value, "SelectedPageCount");
			}
		}

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x060013A3 RID: 5027 RVA: 0x0004F0FA File Offset: 0x0004D2FA
		// (set) Token: 0x060013A4 RID: 5028 RVA: 0x0004F104 File Offset: 0x0004D304
		public string PageRange
		{
			get
			{
				return this.pageRange;
			}
			set
			{
				int[] array;
				int num;
				if (!PdfObjectExtensions.TryParsePageRange(value, out array, out num) || !array.All((int c) => c >= 0 && c < this.Document.Pages.Count))
				{
					ModernMessageBox.Show(Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				if (base.SetProperty<string>(ref this.pageRange, value, "PageRange"))
				{
					this.UpdateSelectedPageCount();
					return;
				}
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x060013A5 RID: 5029 RVA: 0x0004F161 File Offset: 0x0004D361
		// (set) Token: 0x060013A6 RID: 5030 RVA: 0x0004F169 File Offset: 0x0004D369
		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				if (base.SetProperty<int>(ref this.currentPage, value, "CurrentPage"))
				{
					this.UpdateSelectedPageCount();
				}
			}
		}

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x060013A7 RID: 5031 RVA: 0x0004F185 File Offset: 0x0004D385
		// (set) Token: 0x060013A8 RID: 5032 RVA: 0x0004F18D File Offset: 0x0004D38D
		public BatchPrintItemModel.PageRangeEnum PageRangeMode
		{
			get
			{
				return this.pageRangeMode;
			}
			set
			{
				if (base.SetProperty<BatchPrintItemModel.PageRangeEnum>(ref this.pageRangeMode, value, "PageRangeMode"))
				{
					this.UpdateSelectedPageCount();
				}
			}
		}

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x060013A9 RID: 5033 RVA: 0x0004F1A9 File Offset: 0x0004D3A9
		// (set) Token: 0x060013AA RID: 5034 RVA: 0x0004F1B1 File Offset: 0x0004D3B1
		public BatchPrintItemModel.SubsetEnum SubsetMode
		{
			get
			{
				return this.subsetMode;
			}
			set
			{
				if (base.SetProperty<BatchPrintItemModel.SubsetEnum>(ref this.subsetMode, value, "SubsetMode"))
				{
					this.UpdateSelectedPageCount();
				}
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x060013AB RID: 5035 RVA: 0x0004F1CD File Offset: 0x0004D3CD
		// (set) Token: 0x060013AC RID: 5036 RVA: 0x0004F1D5 File Offset: 0x0004D3D5
		public int PrintStatus
		{
			get
			{
				return this.printStatus;
			}
			set
			{
				base.SetProperty<int>(ref this.printStatus, value, "PrintStatus");
			}
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x060013AD RID: 5037 RVA: 0x0004F1EA File Offset: 0x0004D3EA
		// (set) Token: 0x060013AE RID: 5038 RVA: 0x0004F1F2 File Offset: 0x0004D3F2
		public PrintArgs _PrintArgs
		{
			get
			{
				return this._printArgs;
			}
			set
			{
				base.SetProperty<PrintArgs>(ref this._printArgs, value, "_PrintArgs");
			}
		}

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x0004F207 File Offset: 0x0004D407
		// (set) Token: 0x060013B0 RID: 5040 RVA: 0x0004F20F File Offset: 0x0004D40F
		public int Copies
		{
			get
			{
				return this.copies;
			}
			set
			{
				if (value >= 1)
				{
					base.SetProperty<int>(ref this.copies, value, "Copies");
				}
			}
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x060013B1 RID: 5041 RVA: 0x0004F228 File Offset: 0x0004D428
		// (set) Token: 0x060013B2 RID: 5042 RVA: 0x0004F230 File Offset: 0x0004D430
		public bool _PrintAnnotations
		{
			get
			{
				return this._printAnnotations;
			}
			set
			{
				base.SetProperty<bool>(ref this._printAnnotations, value, "_PrintAnnotations");
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x060013B3 RID: 5043 RVA: 0x0004F245 File Offset: 0x0004D445
		// (set) Token: 0x060013B4 RID: 5044 RVA: 0x0004F24D File Offset: 0x0004D44D
		public ICommand RemoveCmd
		{
			get
			{
				return this.removeCmd;
			}
			set
			{
				base.SetProperty<ICommand>(ref this.removeCmd, value, "RemoveCmd");
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x0004F262 File Offset: 0x0004D462
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x0004F26A File Offset: 0x0004D46A
		public ICommand SettingCmd
		{
			get
			{
				return this.settingCmd;
			}
			set
			{
				base.SetProperty<ICommand>(ref this.settingCmd, value, "SettingCmd");
			}
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0004F280 File Offset: 0x0004D480
		private void UpdateSelectedPageCount()
		{
			int pageCount = this.Document.Pages.Count;
			if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.AllPages)
			{
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
				{
					this.SelectedPageCount = pageCount;
					return;
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
				{
					this.SelectedPageCount = pageCount / 2 + pageCount % 2;
					return;
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
				{
					this.SelectedPageCount = pageCount / 2;
					return;
				}
			}
			else if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.CurrentPage)
			{
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
				{
					this.SelectedPageCount = 1;
					return;
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
				{
					this.SelectedPageCount = ((this.CurrentPage % 2 == 1) ? 1 : 0);
					return;
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
				{
					this.SelectedPageCount = ((this.CurrentPage % 2 == 0) ? 1 : 0);
					return;
				}
			}
			else if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.SelectedPages)
			{
				int[] array;
				int num;
				if (PdfObjectExtensions.TryParsePageRange(this.PageRange, out array, out num) && array.All((int c) => c < pageCount))
				{
					if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
					{
						this.SelectedPageCount = array.Count<int>();
						return;
					}
					if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
					{
						this.SelectedPageCount = array.Count((int c) => (c + 1) % 2 == 1);
						return;
					}
					if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
					{
						this.SelectedPageCount = array.Count((int c) => (c + 1) % 2 == 0);
						return;
					}
				}
				else
				{
					this.SelectedPageCount = 0;
				}
			}
		}

		// Token: 0x060013B8 RID: 5048 RVA: 0x0004F410 File Offset: 0x0004D610
		public string GetActualPageRange(out int[] indexes)
		{
			int pageCount = this.Document.Pages.Count;
			indexes = null;
			int num;
			if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.AllPages)
			{
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
				{
					indexes = Enumerable.Range(1, pageCount).ToArray<int>();
					if (pageCount != 1)
					{
						return string.Format("1-{0}", pageCount);
					}
					return "1";
				}
				else
				{
					if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
					{
						indexes = (from c in Enumerable.Range(0, pageCount)
							where c % 2 == 0
							select c).ToArray<int>();
						return indexes.ConvertToRange();
					}
					if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
					{
						indexes = (from c in Enumerable.Range(0, pageCount)
							where c % 2 == 1
							select c).ToArray<int>();
						return indexes.ConvertToRange();
					}
				}
			}
			else if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.CurrentPage)
			{
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
				{
					indexes = new int[] { this.CurrentPage };
					return string.Format("{0}", this.CurrentPage);
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
				{
					object obj;
					if (this.CurrentPage % 2 != 1)
					{
						obj = null;
					}
					else
					{
						(obj = new int[1])[0] = this.CurrentPage;
					}
					indexes = obj;
					if (this.CurrentPage % 2 != 1)
					{
						return string.Empty;
					}
					return string.Format("{0}", this.CurrentPage);
				}
				else if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
				{
					object obj2;
					if (this.CurrentPage % 2 != 0)
					{
						obj2 = null;
					}
					else
					{
						(obj2 = new int[1])[0] = this.CurrentPage;
					}
					indexes = obj2;
					if (this.CurrentPage % 2 != 0)
					{
						return string.Empty;
					}
					return string.Format("{0}", this.CurrentPage);
				}
			}
			else if (this.PageRangeMode == BatchPrintItemModel.PageRangeEnum.SelectedPages && PdfObjectExtensions.TryParsePageRange(this.PageRange, out indexes, out num) && indexes.All((int c) => c < pageCount && c >= 0))
			{
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.AllPages)
				{
					return indexes.ConvertToRange();
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Odd)
				{
					indexes = indexes.Where((int c) => c % 2 == 0).ToArray<int>();
					return indexes.ConvertToRange();
				}
				if (this.SubsetMode == BatchPrintItemModel.SubsetEnum.Even)
				{
					indexes = indexes.Where((int c) => c % 2 == 1).ToArray<int>();
					return indexes.ConvertToRange();
				}
			}
			return string.Empty;
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x0004F6BC File Offset: 0x0004D8BC
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					DocumentWrapper documentWrapper = this.DocumentWrapper;
					if (documentWrapper != null)
					{
						documentWrapper.Dispose();
					}
					this.DocumentWrapper = null;
					this.Document = null;
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0004F6EF File Offset: 0x0004D8EF
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0400065E RID: 1630
		private string fileName;

		// Token: 0x0400065F RID: 1631
		private bool isSelected = true;

		// Token: 0x04000660 RID: 1632
		private int selectedPageCount;

		// Token: 0x04000661 RID: 1633
		private string pageRange;

		// Token: 0x04000662 RID: 1634
		private int currentPage;

		// Token: 0x04000663 RID: 1635
		private BatchPrintItemModel.PageRangeEnum pageRangeMode;

		// Token: 0x04000664 RID: 1636
		private BatchPrintItemModel.SubsetEnum subsetMode;

		// Token: 0x04000665 RID: 1637
		private ICommand removeCmd;

		// Token: 0x04000666 RID: 1638
		private ICommand settingCmd;

		// Token: 0x04000667 RID: 1639
		private bool disposedValue;

		// Token: 0x04000668 RID: 1640
		private PrintArgs _printArgs;

		// Token: 0x04000669 RID: 1641
		private bool _printAnnotations = true;

		// Token: 0x0400066A RID: 1642
		private int printStatus;

		// Token: 0x0400066B RID: 1643
		private int copies = 1;

		// Token: 0x0400066C RID: 1644
		private string filePath;

		// Token: 0x02000563 RID: 1379
		public enum PageRangeEnum
		{
			// Token: 0x04001DB1 RID: 7601
			AllPages,
			// Token: 0x04001DB2 RID: 7602
			CurrentPage,
			// Token: 0x04001DB3 RID: 7603
			SelectedPages
		}

		// Token: 0x02000564 RID: 1380
		public enum SubsetEnum
		{
			// Token: 0x04001DB5 RID: 7605
			AllPages,
			// Token: 0x04001DB6 RID: 7606
			Odd,
			// Token: 0x04001DB7 RID: 7607
			Even
		}
	}
}
