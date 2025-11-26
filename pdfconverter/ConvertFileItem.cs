using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfconverter.Properties;
using PDFKit.Utils;

namespace pdfconverter
{
	// Token: 0x02000012 RID: 18
	internal class ConvertFileItem : INotifyPropertyChanged
	{
		// Token: 0x06000084 RID: 132 RVA: 0x000023A0 File Offset: 0x000005A0
		public ConvertFileItem(string file)
		{
			this._filePath = file;
			this._pageCount = -1;
			this.ConvertStatus = FileCovertStatus.ConvertInit;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000023EC File Offset: 0x000005EC
		public void parseFile(string password)
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				this.ConvertStatus = FileCovertStatus.ConvertLoading;
				int result = PdfiumNetHelper.GetPageCountAsync(this._filePath, password).GetAwaiter().GetResult();
				if (result > 0)
				{
					GAManager.SendEvent("PDFConvert", "Loaded", "Count", 1L);
					this.PageCount = result;
					this.PageTo = result;
					this.PageFrom = 1;
					this.FileSelected = new bool?(true);
					this.WithOCR = new bool?(true);
					this.ConvertStatus = FileCovertStatus.ConvertLoaded;
					this.PassWord = password;
					return;
				}
				GAManager.SendEvent("PDFConvert", "Unsupport", "Count", 1L);
				this.PageCount = 0;
				this.FileSelected = new bool?(false);
				this.ConvertStatus = FileCovertStatus.ConvertUnsupport;
			}));
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00002417 File Offset: 0x00000617
		public string convertFile
		{
			get
			{
				return this._filePath;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000087 RID: 135 RVA: 0x0000241F File Offset: 0x0000061F
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00002427 File Offset: 0x00000627
		public string PassWord
		{
			get
			{
				return this._password;
			}
			set
			{
				this._password = value;
				this.RaisePropertyChanged("PassWord");
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000089 RID: 137 RVA: 0x0000243B File Offset: 0x0000063B
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00002443 File Offset: 0x00000643
		public FileCovertStatus ConvertStatus
		{
			get
			{
				return this._status;
			}
			set
			{
				this._status = value;
				this.RaisePropertyChanged("ConvertStatus");
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00002457 File Offset: 0x00000657
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00002477 File Offset: 0x00000677
		// (set) Token: 0x0600008D RID: 141 RVA: 0x0000248A File Offset: 0x0000068A
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
				this._pageCount = value;
				this.RaisePropertyChanged("PageCount");
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000249E File Offset: 0x0000069E
		// (set) Token: 0x0600008F RID: 143 RVA: 0x000024A8 File Offset: 0x000006A8
		public int PageFrom
		{
			get
			{
				return this._pageFrom;
			}
			set
			{
				if (value < 0 || value > this._pageCount || value > this._pageTo)
				{
					ModernMessageBox.Show(Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					this.RaisePropertyChanged("PageFrom");
					return;
				}
				this._pageFrom = value;
				this.RaisePropertyChanged("PageFrom");
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000090 RID: 144 RVA: 0x000024FF File Offset: 0x000006FF
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00002508 File Offset: 0x00000708
		public int PageTo
		{
			get
			{
				return this._pageTo;
			}
			set
			{
				if (value < 0 || value > this._pageCount || value < this._pageFrom)
				{
					ModernMessageBox.Show(Resources.FileConvertMsgInvaildPageNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					this.RaisePropertyChanged("PageTo");
					return;
				}
				this._pageTo = value;
				this.RaisePropertyChanged("PageTo");
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000092 RID: 146 RVA: 0x0000255F File Offset: 0x0000075F
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00002567 File Offset: 0x00000767
		public List<PdfPageRange> PDFPangeRanges
		{
			get
			{
				return this._pdfPageRanges;
			}
			set
			{
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00002569 File Offset: 0x00000769
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00002571 File Offset: 0x00000771
		public bool? FileSelected
		{
			get
			{
				return this._isFileSelected;
			}
			set
			{
				this._isFileSelected = value;
				this.RaisePropertyChanged("FileSelected");
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00002585 File Offset: 0x00000785
		// (set) Token: 0x06000097 RID: 151 RVA: 0x0000258D File Offset: 0x0000078D
		public bool? WithOCR
		{
			get
			{
				return this._withOCR;
			}
			set
			{
				this._withOCR = value;
				this.RaisePropertyChanged("WithOCR");
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000025A1 File Offset: 0x000007A1
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000025A9 File Offset: 0x000007A9
		public bool? SingleSheet
		{
			get
			{
				return this._singleSheet;
			}
			set
			{
				this._singleSheet = value;
				this.RaisePropertyChanged("SingleSheet");
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000025BD File Offset: 0x000007BD
		// (set) Token: 0x0600009B RID: 155 RVA: 0x000025C5 File Offset: 0x000007C5
		public string outputFile { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000025CE File Offset: 0x000007CE
		// (set) Token: 0x0600009D RID: 157 RVA: 0x000025D6 File Offset: 0x000007D6
		public bool outputFileIsDir { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600009E RID: 158 RVA: 0x000025E0 File Offset: 0x000007E0
		// (remove) Token: 0x0600009F RID: 159 RVA: 0x00002618 File Offset: 0x00000818
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x060000A0 RID: 160 RVA: 0x00002650 File Offset: 0x00000850
		protected void RaisePropertyChanged([CallerMemberName] string name = "")
		{
			try
			{
				if (this.PropertyChanged != null)
				{
					this.PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
			}
			catch
			{
			}
		}

		// Token: 0x0400009F RID: 159
		private bool? _isFileSelected = new bool?(true);

		// Token: 0x040000A0 RID: 160
		private string _filePath;

		// Token: 0x040000A1 RID: 161
		private FileCovertStatus _status;

		// Token: 0x040000A2 RID: 162
		private int _pageCount;

		// Token: 0x040000A3 RID: 163
		private int _pageFrom;

		// Token: 0x040000A4 RID: 164
		private int _pageTo;

		// Token: 0x040000A5 RID: 165
		private bool? _withOCR = new bool?(false);

		// Token: 0x040000A6 RID: 166
		private bool? _singleSheet = new bool?(true);

		// Token: 0x040000A7 RID: 167
		private string _password;

		// Token: 0x040000A8 RID: 168
		private List<PdfPageRange> _pdfPageRanges;
	}
}
