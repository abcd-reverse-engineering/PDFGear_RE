using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfconverter.Properties;
using pdfconverter.Utils;

namespace pdfconverter.Models
{
	// Token: 0x0200007E RID: 126
	public class SplitFileItem : BindableBase
	{
		// Token: 0x0600060A RID: 1546 RVA: 0x00016C78 File Offset: 0x00014E78
		public SplitFileItem(string file)
		{
			this._filePath = file;
			this.IsFileSelected = new bool?(true);
			this.PageSplitMode = 0;
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x00016CA6 File Offset: 0x00014EA6
		public string FilePath
		{
			get
			{
				return this._filePath;
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00016CAE File Offset: 0x00014EAE
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

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x00016CCE File Offset: 0x00014ECE
		// (set) Token: 0x0600060E RID: 1550 RVA: 0x00016CD6 File Offset: 0x00014ED6
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

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x00016CEB File Offset: 0x00014EEB
		// (set) Token: 0x06000610 RID: 1552 RVA: 0x00016CF3 File Offset: 0x00014EF3
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

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x00016D08 File Offset: 0x00014F08
		// (set) Token: 0x06000612 RID: 1554 RVA: 0x00016D1B File Offset: 0x00014F1B
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

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x00016D30 File Offset: 0x00014F30
		// (set) Token: 0x06000614 RID: 1556 RVA: 0x00016D38 File Offset: 0x00014F38
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

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x00016D85 File Offset: 0x00014F85
		// (set) Token: 0x06000616 RID: 1558 RVA: 0x00016D90 File Offset: 0x00014F90
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

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x00016DDD File Offset: 0x00014FDD
		// (set) Token: 0x06000618 RID: 1560 RVA: 0x00016DE5 File Offset: 0x00014FE5
		public SplitStatus Status
		{
			get
			{
				return this._status;
			}
			set
			{
				base.SetProperty<SplitStatus>(ref this._status, value, "Status");
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000619 RID: 1561 RVA: 0x00016DFA File Offset: 0x00014FFA
		// (set) Token: 0x0600061A RID: 1562 RVA: 0x00016E02 File Offset: 0x00015002
		public int PageSplitMode
		{
			get
			{
				return this._pageSplitMode;
			}
			set
			{
				base.SetProperty<int>(ref this._pageSplitMode, value, "PageSplitMode");
				this.PageSplitModeStr = "";
				base.OnPropertyChanged("PageSplitModePlaceHolder");
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x00016E2D File Offset: 0x0001502D
		// (set) Token: 0x0600061C RID: 1564 RVA: 0x00016E38 File Offset: 0x00015038
		public string PageSplitModeStr
		{
			get
			{
				return this._pageSplitModeStr;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					if (this.PageSplitMode == 0)
					{
						int[][] array;
						int num;
						if (!PageRangeHelper.TryParsePageRange2(value, out array, out num) || !this.CheckPageRange(array, this.PageCount - 1))
						{
							ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
					}
					else
					{
						if (this.PageSplitMode != 1)
						{
							return;
						}
						if (!new Regex("^\\d+$").IsMatch(value))
						{
							ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						int num2 = Convert.ToInt32(value);
						if (num2 <= 0 || num2 >= this.PageCount)
						{
							ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
					}
				}
				base.SetProperty<string>(ref this._pageSplitModeStr, value, "PageSplitModeStr");
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x00016EF9 File Offset: 0x000150F9
		public string PageSplitModePlaceHolder
		{
			get
			{
				if (this.PageSplitMode == 0)
				{
					return Resources.WinMergeSplitSplitModeCustomRangeHelpAsgMsg;
				}
				if (this.PageSplitMode == 1)
				{
					return Resources.WinMergeSplitSplitModeFixedRangeHelpAsgMsg;
				}
				return "";
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x00016F1D File Offset: 0x0001511D
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x00016F25 File Offset: 0x00015125
		public string OutputPath
		{
			get
			{
				return this._outputPath;
			}
			set
			{
				base.SetProperty<string>(ref this._outputPath, value, "OutputPath");
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00016F3A File Offset: 0x0001513A
		public void parseFile(string passsword)
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
			{
				this.Status = SplitStatus.Loading;
				int result = PdfiumNetHelper.GetPageCountAsync(this._filePath, passsword).GetAwaiter().GetResult();
				if (result > 0)
				{
					this.PageCount = result;
					this.PageTo = result;
					this.PageFrom = 1;
					this.IsFileSelected = new bool?(true);
					this.Status = SplitStatus.Loaded;
					this.Password = passsword;
					return;
				}
				this.PageCount = 0;
				this.IsFileSelected = new bool?(false);
				this.Status = SplitStatus.Unsupport;
			}));
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00016F68 File Offset: 0x00015168
		private bool CheckPageRange(int[][] pageIndexes, int max)
		{
			foreach (int[] array in pageIndexes)
			{
				if (array.Length != 0)
				{
					int num = array.Max();
					int num2 = array.Min();
					if (num > max || num2 < 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x040002E6 RID: 742
		private string _filePath;

		// Token: 0x040002E7 RID: 743
		private int _pageCount;

		// Token: 0x040002E8 RID: 744
		private int _pageFrom;

		// Token: 0x040002E9 RID: 745
		private int _pageTo;

		// Token: 0x040002EA RID: 746
		private SplitStatus _status;

		// Token: 0x040002EB RID: 747
		private int _pageSplitMode;

		// Token: 0x040002EC RID: 748
		private string _pageSplitModeStr;

		// Token: 0x040002ED RID: 749
		private string _outputPath;

		// Token: 0x040002EE RID: 750
		private bool? _isFileSelected = new bool?(true);

		// Token: 0x040002EF RID: 751
		private string _password;
	}
}
