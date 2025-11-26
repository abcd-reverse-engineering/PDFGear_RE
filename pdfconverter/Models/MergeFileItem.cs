using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfconverter.Properties;
using pdfconverter.Utils;

namespace pdfconverter.Models
{
	// Token: 0x02000066 RID: 102
	public class MergeFileItem : BindableBase
	{
		// Token: 0x060005C1 RID: 1473 RVA: 0x00016630 File Offset: 0x00014830
		public MergeFileItem(string file)
		{
			this._filePath = file;
			this.IsFileSelected = new bool?(true);
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x00016657 File Offset: 0x00014857
		public string FilePath
		{
			get
			{
				return this._filePath;
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0001665F File Offset: 0x0001485F
		public string FileName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this._filePath))
				{
					return "";
				}
				return Path.GetFileName(this._filePath);
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060005C4 RID: 1476 RVA: 0x0001667F File Offset: 0x0001487F
		// (set) Token: 0x060005C5 RID: 1477 RVA: 0x00016687 File Offset: 0x00014887
		public bool? IsFileSelected
		{
			get
			{
				return this._isFileSelected;
			}
			set
			{
				base.SetProperty<bool?>(ref this._isFileSelected, value, "IsFileSelected");
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001669C File Offset: 0x0001489C
		// (set) Token: 0x060005C7 RID: 1479 RVA: 0x000166A4 File Offset: 0x000148A4
		public string Passwrod
		{
			get
			{
				return this._password;
			}
			set
			{
				base.SetProperty<string>(ref this._password, value, "Passwrod");
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x000166B9 File Offset: 0x000148B9
		// (set) Token: 0x060005C9 RID: 1481 RVA: 0x000166CC File Offset: 0x000148CC
		public int PageCount
		{
			get
			{
				if (this._pageCount > 0)
				{
					return this._pageCount;
				}
				return 0;
			}
			set
			{
				base.SetProperty<int>(ref this._pageCount, value, "PageCount");
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060005CA RID: 1482 RVA: 0x000166E1 File Offset: 0x000148E1
		// (set) Token: 0x060005CB RID: 1483 RVA: 0x000166EC File Offset: 0x000148EC
		public int PageFrom
		{
			get
			{
				return this._pageFrom;
			}
			set
			{
				if (value <= 0 || value > this._pageCount || value > this._pageTo)
				{
					ModernMessageBox.Show(Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				base.SetProperty<int>(ref this._pageFrom, value, "PageFrom");
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060005CC RID: 1484 RVA: 0x00016739 File Offset: 0x00014939
		// (set) Token: 0x060005CD RID: 1485 RVA: 0x00016744 File Offset: 0x00014944
		public int PageTo
		{
			get
			{
				return this._pageTo;
			}
			set
			{
				if (value <= 0 || value > this._pageCount || value < this._pageFrom)
				{
					ModernMessageBox.Show(Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				base.SetProperty<int>(ref this._pageTo, value, "PageTo");
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x00016791 File Offset: 0x00014991
		// (set) Token: 0x060005CF RID: 1487 RVA: 0x00016799 File Offset: 0x00014999
		public MergeStatus Status
		{
			get
			{
				return this._status;
			}
			set
			{
				base.SetProperty<MergeStatus>(ref this._status, value, "Status");
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x000167AE File Offset: 0x000149AE
		public void parseFile(string password)
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				this.Status = MergeStatus.Loading;
				int result = PdfiumNetHelper.GetPageCountAsync(this._filePath, password).GetAwaiter().GetResult();
				if (result > 0)
				{
					this.PageCount = result;
					this.PageTo = result;
					this.PageFrom = 1;
					this.IsFileSelected = new bool?(true);
					this.Status = MergeStatus.Loaded;
					this.Passwrod = password;
					return;
				}
				this.PageCount = 0;
				this.IsFileSelected = new bool?(false);
				this.Status = MergeStatus.Unsupport;
			}));
		}

		// Token: 0x040002AD RID: 685
		private string _filePath;

		// Token: 0x040002AE RID: 686
		private int _pageCount;

		// Token: 0x040002AF RID: 687
		private int _pageFrom;

		// Token: 0x040002B0 RID: 688
		private int _pageTo;

		// Token: 0x040002B1 RID: 689
		private MergeStatus _status;

		// Token: 0x040002B2 RID: 690
		private bool? _isFileSelected = new bool?(true);

		// Token: 0x040002B3 RID: 691
		private string _password;
	}
}
