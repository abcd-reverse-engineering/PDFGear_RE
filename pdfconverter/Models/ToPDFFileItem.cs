using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommonLib.ConvertUtils;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfconverter.Properties;

namespace pdfconverter.Models
{
	// Token: 0x02000083 RID: 131
	public class ToPDFFileItem : ObservableObject
	{
		// Token: 0x0600065C RID: 1628 RVA: 0x000176AC File Offset: 0x000158AC
		public ToPDFFileItem(string file, ConvToPDFType type)
		{
			this.FilePath = file;
			this.IsFileSelected = new bool?(true);
			this.ConvType = type;
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x000176E1 File Offset: 0x000158E1
		public string FilePath { get; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x000176E9 File Offset: 0x000158E9
		// (set) Token: 0x0600065F RID: 1631 RVA: 0x000176F1 File Offset: 0x000158F1
		public string OutputPath { get; set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x000176FA File Offset: 0x000158FA
		public string Extention
		{
			get
			{
				return new FileInfo(this.FilePath).Extension;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x0001770C File Offset: 0x0001590C
		// (set) Token: 0x06000662 RID: 1634 RVA: 0x00017714 File Offset: 0x00015914
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				base.SetProperty<string>(ref this._password, value, "Password");
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x00017729 File Offset: 0x00015929
		public string FileName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.FilePath))
				{
					return "";
				}
				return Path.GetFileName(this.FilePath);
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x00017749 File Offset: 0x00015949
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x00017751 File Offset: 0x00015951
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

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x00017766 File Offset: 0x00015966
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x0001776E File Offset: 0x0001596E
		public bool? IsEnable
		{
			get
			{
				return this._isFileSelected;
			}
			set
			{
				base.SetProperty<bool?>(ref this._isFileSelected, value, "IsEnable");
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x00017783 File Offset: 0x00015983
		// (set) Token: 0x06000669 RID: 1641 RVA: 0x0001778B File Offset: 0x0001598B
		public int PageCount
		{
			get
			{
				return this._pageCount;
			}
			set
			{
				base.SetProperty<int>(ref this._pageCount, value, "PageCount");
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x000177A0 File Offset: 0x000159A0
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x000177A8 File Offset: 0x000159A8
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

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x000177F5 File Offset: 0x000159F5
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x00017800 File Offset: 0x00015A00
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

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0001784D File Offset: 0x00015A4D
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x00017855 File Offset: 0x00015A55
		public ToPDFItemStatus Status
		{
			get
			{
				return this._status;
			}
			set
			{
				base.SetProperty<ToPDFItemStatus>(ref this._status, value, "Status");
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0001786A File Offset: 0x00015A6A
		public void ParseFile()
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(async delegate
			{
				this.Status = ToPDFItemStatus.Loading;
				this.GetPagesNum();
			}));
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00017884 File Offset: 0x00015A84
		private async void GetPagesNum()
		{
			await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				string extension = Path.GetExtension(this.FilePath);
				if (string.IsNullOrWhiteSpace(extension))
				{
					this.IsFileSelected = new bool?(false);
					this.Status = ToPDFItemStatus.Unsupport;
					return;
				}
				int num;
				if (ConvertUtils.ExtEquals(extension, UtilManager.WordExtention))
				{
					num = ((this.ConvType == ConvToPDFType.WordToPDF) ? 1 : (-1));
				}
				else if (ConvertUtils.ExtEquals(extension, UtilManager.ExcelExtention))
				{
					num = ((this.ConvType == ConvToPDFType.ExcelToPDF) ? 1 : (-1));
				}
				else if (ConvertUtils.ExtEquals(extension, UtilManager.RtfExtention))
				{
					num = ((this.ConvType == ConvToPDFType.RtfToPDF) ? 1 : (-1));
				}
				else if (ConvertUtils.ExtEquals(extension, UtilManager.PPTExtention))
				{
					num = ((this.ConvType == ConvToPDFType.PPTToPDF) ? 1 : (-1));
				}
				else if (ConvertUtils.ExtEquals(extension, UtilManager.TxtExtention))
				{
					num = ((this.ConvType == ConvToPDFType.TxtToPDF) ? 1 : (-1));
				}
				else if (ConvertUtils.ExtEquals(extension, UtilManager.ImageExtention))
				{
					num = ((this.ConvType == ConvToPDFType.ImageToPDF) ? 1 : (-1));
				}
				else
				{
					num = -1;
				}
				if (num > 0)
				{
					this.IsFileSelected = new bool?(true);
					this.Status = ToPDFItemStatus.Loaded;
					return;
				}
				this.IsFileSelected = new bool?(false);
				this.Status = ToPDFItemStatus.Unsupport;
			}));
		}

		// Token: 0x0400030D RID: 781
		private int _pageCount;

		// Token: 0x0400030E RID: 782
		private int _pageFrom;

		// Token: 0x0400030F RID: 783
		private int _pageTo;

		// Token: 0x04000310 RID: 784
		private ToPDFItemStatus _status;

		// Token: 0x04000311 RID: 785
		private bool? _isFileSelected = new bool?(true);

		// Token: 0x04000312 RID: 786
		private bool _isEnable = true;

		// Token: 0x04000313 RID: 787
		private ConvToPDFType ConvType;

		// Token: 0x04000314 RID: 788
		private string _password;
	}
}
