using System;
using System.IO;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.OfficeChart;
using Syncfusion.OfficeChartToImageConverter;
using Syncfusion.Pdf;

namespace pdfconverter.Core
{
	// Token: 0x0200008D RID: 141
	public class WordToPDF : IMyInterface
	{
		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00017B92 File Offset: 0x00015D92
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x00017B9A File Offset: 0x00015D9A
		public string OutFolder { get; set; }

		// Token: 0x0600068F RID: 1679 RVA: 0x00017BA3 File Offset: 0x00015DA3
		public WordToPDF(string path)
		{
			this.SourcePath = path;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00017BB4 File Offset: 0x00015DB4
		public void ToPDF()
		{
			WordDocument wordDocument = new WordDocument(this.SourcePath, FormatType.Docx);
			wordDocument.ChartToImageConverter = new ChartToImageConverter();
			wordDocument.ChartToImageConverter.ScalingMode = ScalingMode.Normal;
			PdfDocument pdfDocument = new DocToPDFConverter
			{
				Settings = 
				{
					ImageQuality = 100,
					ImageResolution = 640,
					OptimizeIdenticalImages = true
				}
			}.ConvertToPDF(wordDocument);
			string text = Path.Combine(this.OutFolder, Path.GetFileNameWithoutExtension(this.SourcePath));
			pdfDocument.Save(text);
			pdfDocument.Close(true);
			pdfDocument.Dispose();
			wordDocument.Close();
			wordDocument.Dispose();
		}

		// Token: 0x04000331 RID: 817
		private string SourcePath;
	}
}
