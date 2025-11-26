using System;
using System.IO;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models;
using pdfeditor.Properties;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000061 RID: 97
	public class ConverterCommands
	{
		// Token: 0x06000552 RID: 1362 RVA: 0x0001B3B0 File Offset: 0x000195B0
		public ConverterCommands(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000553 RID: 1363 RVA: 0x0001B3C0 File Offset: 0x000195C0
		public RelayCommand PDFToWordCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToWordCmd) == null)
				{
					relayCommand = (this.pdfToWordCmd = new RelayCommand(delegate
					{
						this.DoPDFToWord(null);
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0001B410 File Offset: 0x00019610
		public RelayCommand CompressPDF
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.compressPDF) == null)
				{
					relayCommand = (this.compressPDF = new RelayCommand(delegate
					{
						this.DoPDFCompress(null);
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0001B460 File Offset: 0x00019660
		public RelayCommand PDFToExcelCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToExcelCmd) == null)
				{
					relayCommand = (this.pdfToExcelCmd = new RelayCommand(delegate
					{
						this.DoPDFToExcel();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x0001B4B0 File Offset: 0x000196B0
		public RelayCommand PDFToImageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToImageCmd) == null)
				{
					relayCommand = (this.pdfToImageCmd = new RelayCommand(delegate
					{
						this.DoPDFToImage();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x0001B500 File Offset: 0x00019700
		public RelayCommand PDFToJpegCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToJpegCmd) == null)
				{
					relayCommand = (this.pdfToJpegCmd = new RelayCommand(delegate
					{
						this.DoPDFToJpeg();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x0001B550 File Offset: 0x00019750
		public RelayCommand PDFToPPTCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToPPTCmd) == null)
				{
					relayCommand = (this.pdfToPPTCmd = new RelayCommand(delegate
					{
						this.DoPDFToPPT();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x0001B5A0 File Offset: 0x000197A0
		public RelayCommand PDFToRtfCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToRtfCmd) == null)
				{
					relayCommand = (this.pdfToRtfCmd = new RelayCommand(delegate
					{
						this.DoPDFToRtf();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x0001B5F0 File Offset: 0x000197F0
		public RelayCommand PDFToTxtCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToTxtCmd) == null)
				{
					relayCommand = (this.pdfToTxtCmd = new RelayCommand(delegate
					{
						this.DoPDFToTxt();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0001B640 File Offset: 0x00019840
		public RelayCommand PDFToHtmlCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToHtmlCmd) == null)
				{
					relayCommand = (this.pdfToHtmlCmd = new RelayCommand(delegate
					{
						this.DoPDFToHtml();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0001B690 File Offset: 0x00019890
		public RelayCommand PDFToXmlCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pdfToXmlCmd) == null)
				{
					relayCommand = (this.pdfToXmlCmd = new RelayCommand(delegate
					{
						this.DoPDFToXml();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x0001B6E0 File Offset: 0x000198E0
		public RelayCommand WordToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.wordToPdfCmd) == null)
				{
					relayCommand = (this.wordToPdfCmd = new RelayCommand(delegate
					{
						this.DoWordToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x0001B730 File Offset: 0x00019930
		public RelayCommand ExcelToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.excelToPdfCmd) == null)
				{
					relayCommand = (this.excelToPdfCmd = new RelayCommand(delegate
					{
						this.DoExcelToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x0001B780 File Offset: 0x00019980
		public RelayCommand PPTToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.pptToPdfCmd) == null)
				{
					relayCommand = (this.pptToPdfCmd = new RelayCommand(delegate
					{
						this.DoPPTToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001B7D0 File Offset: 0x000199D0
		public RelayCommand ImageToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.imageToPdfCmd) == null)
				{
					relayCommand = (this.imageToPdfCmd = new RelayCommand(delegate
					{
						this.DoImageToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000561 RID: 1377 RVA: 0x0001B820 File Offset: 0x00019A20
		public RelayCommand RtfToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.rtfToPdfCmd) == null)
				{
					relayCommand = (this.rtfToPdfCmd = new RelayCommand(delegate
					{
						this.DoRtfToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x0001B870 File Offset: 0x00019A70
		public RelayCommand TxtToPDFCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.txtToPdfCmd) == null)
				{
					relayCommand = (this.txtToPdfCmd = new RelayCommand(delegate
					{
						this.DoTxtToPDF();
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0001B8C0 File Offset: 0x00019AC0
		public void DoPDFToWord(string from = null)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToWord;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001B930 File Offset: 0x00019B30
		public void DoPDFCompress(ConverterCommands.CompressMode? mode = null)
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeCompressMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (mode != null)
			{
				try
				{
					string text = Path.Combine(AppDataHelper.LocalCacheFolder, "TmpSetting");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					string text2 = Path.Combine(text, "compress");
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
					File.WriteAllText(text2, ((int)mode.Value).ToString());
				}
				catch
				{
				}
			}
			ConvToPDFType convToPDFType = ConvToPDFType.CompressPDF;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterToPdf(convToPDFType, password, array);
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001BA0C File Offset: 0x00019C0C
		public void DoPDFToExcel()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToExcel;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0001BA7C File Offset: 0x00019C7C
		public void DoPDFToImage()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToPng;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001BAEC File Offset: 0x00019CEC
		public void DoPDFToPPT()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToPPT;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0001BB5C File Offset: 0x00019D5C
		public void DoPDFToJpeg()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToJpg;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001BBCC File Offset: 0x00019DCC
		public void DoPDFToRtf()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToRTF;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0001BC3C File Offset: 0x00019E3C
		public void DoPDFToTxt()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToTxt;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001BCAC File Offset: 0x00019EAC
		public void DoPDFToHtml()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToHtml;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0001BD1C File Offset: 0x00019F1C
		public void DoPDFToXml()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			if (this.mainViewModel.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeConvertMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			ConvFromPDFType convFromPDFType = ConvFromPDFType.PDFToXml;
			string password = this.mainViewModel.Password;
			string[] array = new string[1];
			int num = 0;
			DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
			array[num] = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			AppManager.OpenPDFConverterFromPdf(convFromPDFType, password, array);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0001BD8C File Offset: 0x00019F8C
		public void DoWordToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.WordToPDF, null);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001BDA5 File Offset: 0x00019FA5
		public void DoExcelToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.ExcelToPDF, null);
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001BDBE File Offset: 0x00019FBE
		public void DoPPTToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.PPTToPDF, null);
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x0001BDD7 File Offset: 0x00019FD7
		public void DoImageToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.ImageToPDF, null);
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0001BDF0 File Offset: 0x00019FF0
		public void DoRtfToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.RtfToPDF, null);
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0001BE09 File Offset: 0x0001A009
		public void DoTxtToPDF()
		{
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			AppManager.OpenPDFConverterToPdf(ConvToPDFType.TxtToPDF, null);
		}

		// Token: 0x040002D0 RID: 720
		private readonly MainViewModel mainViewModel;

		// Token: 0x040002D1 RID: 721
		private RelayCommand pdfToWordCmd;

		// Token: 0x040002D2 RID: 722
		private RelayCommand pdfToExcelCmd;

		// Token: 0x040002D3 RID: 723
		private RelayCommand pdfToImageCmd;

		// Token: 0x040002D4 RID: 724
		private RelayCommand pdfToJpegCmd;

		// Token: 0x040002D5 RID: 725
		private RelayCommand pdfToPPTCmd;

		// Token: 0x040002D6 RID: 726
		private RelayCommand pdfToRtfCmd;

		// Token: 0x040002D7 RID: 727
		private RelayCommand pdfToTxtCmd;

		// Token: 0x040002D8 RID: 728
		private RelayCommand pdfToHtmlCmd;

		// Token: 0x040002D9 RID: 729
		private RelayCommand pdfToXmlCmd;

		// Token: 0x040002DA RID: 730
		private RelayCommand compressPDF;

		// Token: 0x040002DB RID: 731
		private RelayCommand wordToPdfCmd;

		// Token: 0x040002DC RID: 732
		private RelayCommand excelToPdfCmd;

		// Token: 0x040002DD RID: 733
		private RelayCommand pptToPdfCmd;

		// Token: 0x040002DE RID: 734
		private RelayCommand imageToPdfCmd;

		// Token: 0x040002DF RID: 735
		private RelayCommand rtfToPdfCmd;

		// Token: 0x040002E0 RID: 736
		private RelayCommand txtToPdfCmd;

		// Token: 0x02000346 RID: 838
		public enum CompressMode
		{
			// Token: 0x040013B5 RID: 5045
			High,
			// Token: 0x040013B6 RID: 5046
			Medium,
			// Token: 0x040013B7 RID: 5047
			Low
		}
	}
}
