using System;
using System.IO;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfconverter.ViewModels;

namespace pdfconverter.Models
{
	// Token: 0x02000056 RID: 86
	public class CompressItem : ObservableObject
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x00015D20 File Offset: 0x00013F20
		public CompressItem(string path, CompressMode mode, CompressPDFUCViewModel model)
		{
			this.FileName = Path.GetFileName(path);
			this.FilePath = path;
			this.sourceSize = this.GetFileSize(path);
			this._status = ToPDFItemStatus.Loaded;
			this._isEnable = new bool?(true);
			this.compressMode = (int)mode;
			this.Parent = model;
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x00015DB7 File Offset: 0x00013FB7
		public string CompressHighFilePath
		{
			get
			{
				return Path.Combine(CompressItem.CompressHigh, this.fileName);
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x00015DC9 File Offset: 0x00013FC9
		public string CompressMediumFilePath
		{
			get
			{
				return Path.Combine(CompressItem.CompressMeidum, this.fileName);
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x00015DDB File Offset: 0x00013FDB
		public string CompressLowFilePath
		{
			get
			{
				return Path.Combine(CompressItem.CompressLow, this.fileName);
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00015DED File Offset: 0x00013FED
		public string CompressHighFolderPath
		{
			get
			{
				return CompressItem.CompressHigh;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x00015DF4 File Offset: 0x00013FF4
		public string CompressMediumFolderPath
		{
			get
			{
				return CompressItem.CompressMeidum;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00015DFB File Offset: 0x00013FFB
		public string CompressLowFolderPath
		{
			get
			{
				return CompressItem.CompressLow;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x00015E02 File Offset: 0x00014002
		// (set) Token: 0x0600056B RID: 1387 RVA: 0x00015E0A File Offset: 0x0001400A
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

		// Token: 0x0600056C RID: 1388 RVA: 0x00015E1F File Offset: 0x0001401F
		private void UpdateSize()
		{
			if (this.compressMode == 0)
			{
				this.CompressedSize = this.CompressedHighSize;
				return;
			}
			if (this.compressMode == 1)
			{
				this.CompressedSize = this.CompressedMediumSize;
				return;
			}
			this.CompressedSize = this.CompressedLowSize;
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00015E58 File Offset: 0x00014058
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x00015E60 File Offset: 0x00014060
		public string OutputPath
		{
			get
			{
				return this.outputPath;
			}
			set
			{
				base.SetProperty<string>(ref this.outputPath, value, "OutputPath");
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00015E75 File Offset: 0x00014075
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x00015E7D File Offset: 0x0001407D
		public int Compress_Mode
		{
			get
			{
				return this.compressMode;
			}
			set
			{
				base.SetProperty<int>(ref this.compressMode, value, "Compress_Mode");
				this.UpdateSize();
				this.Parent.SelectModeChanged();
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x00015EA3 File Offset: 0x000140A3
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x00015EAB File Offset: 0x000140AB
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

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x00015EC0 File Offset: 0x000140C0
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x00015EC8 File Offset: 0x000140C8
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

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x00015EDD File Offset: 0x000140DD
		public string SourceSize
		{
			get
			{
				return this.sourceSize;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00015EE8 File Offset: 0x000140E8
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x00015F3E File Offset: 0x0001413E
		public string CompressedSize
		{
			get
			{
				if (this.compressMode == 0)
				{
					this.compressedSize = this.CompressedHighSize;
					return this.compressedSize;
				}
				if (this.compressMode == 1)
				{
					this.compressedSize = this.CompressedMediumSize;
					return this.compressedSize;
				}
				this.compressedSize = this.CompressedLowSize;
				return this.compressedSize;
			}
			set
			{
				if (this.compressMode == 0)
				{
					this.CompressedHighSize = value;
				}
				else if (this.compressMode == 1)
				{
					this.CompressedMediumSize = value;
				}
				else
				{
					this.CompressedLowSize = value;
				}
				base.SetProperty<string>(ref this.compressedSize, value, "CompressedSize");
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00015F7D File Offset: 0x0001417D
		// (set) Token: 0x06000579 RID: 1401 RVA: 0x00015F85 File Offset: 0x00014185
		private string CompressedHighSize
		{
			get
			{
				return this.compressedHighSize;
			}
			set
			{
				base.SetProperty<string>(ref this.compressedHighSize, value, "CompressedHighSize");
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00015F9A File Offset: 0x0001419A
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x00015FA2 File Offset: 0x000141A2
		private string CompressedMediumSize
		{
			get
			{
				return this.compressedMediumSize;
			}
			set
			{
				base.SetProperty<string>(ref this.compressedMediumSize, value, "CompressedMediumSize");
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x00015FB7 File Offset: 0x000141B7
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x00015FBF File Offset: 0x000141BF
		private string CompressedLowSize
		{
			get
			{
				return this.compressedLowSize;
			}
			set
			{
				base.SetProperty<string>(ref this.compressedLowSize, value, "CompressedLowSize");
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x00015FD4 File Offset: 0x000141D4
		// (set) Token: 0x0600057F RID: 1407 RVA: 0x00015FDC File Offset: 0x000141DC
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

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x00015FF1 File Offset: 0x000141F1
		// (set) Token: 0x06000581 RID: 1409 RVA: 0x00015FF9 File Offset: 0x000141F9
		public bool? IsEnable
		{
			get
			{
				return this._isEnable;
			}
			set
			{
				base.SetProperty<bool?>(ref this._isEnable, value, "IsEnable");
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x0001600E File Offset: 0x0001420E
		// (set) Token: 0x06000583 RID: 1411 RVA: 0x00016016 File Offset: 0x00014216
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

		// Token: 0x06000584 RID: 1412 RVA: 0x0001602C File Offset: 0x0001422C
		public string GetFileSize(string FileName)
		{
			LongPathFile longPathFile = FileName;
			if (!longPathFile.IsExists)
			{
				return "";
			}
			long length = longPathFile.FileInfo.Length;
			double num = 1024.0;
			if ((double)length < num)
			{
				return length.ToString() + "B";
			}
			if ((double)length < Math.Pow(num, 2.0))
			{
				return ((double)length / num).ToString("f2") + "K";
			}
			if ((double)length < Math.Pow(num, 3.0))
			{
				return ((double)length / Math.Pow(num, 2.0)).ToString("f2") + "M";
			}
			if ((double)length < Math.Pow(num, 4.0))
			{
				return ((double)length / Math.Pow(num, 3.0)).ToString("f2") + "G";
			}
			return ((double)length / Math.Pow(num, 4.0)).ToString("f2") + "T";
		}

		// Token: 0x0400027F RID: 639
		private static readonly string CompressTemp = Path.Combine(AppDataHelper.TemporaryFolder, "Compress");

		// Token: 0x04000280 RID: 640
		private static readonly string CompressHigh = Path.Combine(CompressItem.CompressTemp, "Hight");

		// Token: 0x04000281 RID: 641
		private static readonly string CompressMeidum = Path.Combine(CompressItem.CompressTemp, "Meidum");

		// Token: 0x04000282 RID: 642
		private static readonly string CompressLow = Path.Combine(CompressItem.CompressTemp, "Low");

		// Token: 0x04000283 RID: 643
		private CompressPDFUCViewModel Parent;

		// Token: 0x04000284 RID: 644
		private string filePath;

		// Token: 0x04000285 RID: 645
		private string fileName;

		// Token: 0x04000286 RID: 646
		private string outputPath;

		// Token: 0x04000287 RID: 647
		private string sourceSize;

		// Token: 0x04000288 RID: 648
		private string compressedHighSize = "";

		// Token: 0x04000289 RID: 649
		private string compressedMediumSize = "";

		// Token: 0x0400028A RID: 650
		private string compressedLowSize = "";

		// Token: 0x0400028B RID: 651
		private string compressedSize = "";

		// Token: 0x0400028C RID: 652
		private string _password;

		// Token: 0x0400028D RID: 653
		private bool? _isFileSelected = new bool?(true);

		// Token: 0x0400028E RID: 654
		private bool? _isEnable;

		// Token: 0x0400028F RID: 655
		private int compressMode;

		// Token: 0x04000290 RID: 656
		private ToPDFItemStatus _status;

		// Token: 0x04000291 RID: 657
		private object locker = new object();
	}
}
