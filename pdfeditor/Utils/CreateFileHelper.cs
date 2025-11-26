using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using CommonLib.Common;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Controls.Printer;
using pdfeditor.Properties;

namespace pdfeditor.Utils
{
	// Token: 0x02000078 RID: 120
	public static class CreateFileHelper
	{
		// Token: 0x060008C4 RID: 2244 RVA: 0x0002B818 File Offset: 0x00029A18
		public static string CreateBlankPageAsync()
		{
			SizeF defaultSize = CreateFileHelper.GetDefaultSize();
			using (PdfDocument pdfDocument = PdfDocument.CreateNew(null))
			{
				pdfDocument.Pages.InsertPageAt(0, defaultSize.Width, defaultSize.Height);
				pdfDocument.Pages[0].GenerateContent(false);
				pdfDocument.Producer = "PDF Gear";
				string text = Path.Combine(UtilManager.GetTemporaryPath(), "Documents");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				int num = 0;
				string text2 = "";
				do
				{
					num++;
					string text3 = string.Format("{0}{1}", Resources.NewFileName, num);
					text3 += ".pdf";
					text2 = Path.Combine(text, text3);
				}
				while (File.Exists(text2));
				if (Directory.Exists(text))
				{
					try
					{
						using (FileStream fileStream = File.OpenWrite(text2))
						{
							fileStream.Seek(0L, SeekOrigin.Begin);
							pdfDocument.Save(fileStream, SaveFlags.NoIncremental, 0);
							fileStream.SetLength(fileStream.Position);
						}
						GAManager.SendEvent("PageView", "PageEditorCreateBlankCmd", "Success", 1L);
						return text2;
					}
					catch
					{
						return "";
					}
				}
			}
			return "";
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0002B998 File Offset: 0x00029B98
		private static SizeF GetDefaultSize()
		{
			SizeF sizeF = new SizeF(595f, 842f);
			List<PaperSizeInfo> list = Pagesize.paperSizes.Select((PaperSize c) => new PaperSizeInfo
			{
				FriendlyName = c.PaperName,
				PaperSize = c
			}).ToList<PaperSizeInfo>();
			string defaultBlankPageSize = ConfigManager.GetDefaultBlankPageSize();
			PaperSize paperSize = list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A4")].PaperSize;
			if (!(defaultBlankPageSize == "A4"))
			{
				if (!(defaultBlankPageSize == "A3"))
				{
					if (!(defaultBlankPageSize == "Letter"))
					{
						if (!(defaultBlankPageSize == "Tabloid"))
						{
							if (!(defaultBlankPageSize == "Legal"))
							{
								if (defaultBlankPageSize == "Custom")
								{
									sizeF = new SizeF(CreateFileHelper.GetPixValueFromMM(ConfigManager.GetDefaultBlankPageWidth()), CreateFileHelper.GetPixValueFromMM(ConfigManager.GetDefaultBlankPageHeight()));
									return sizeF;
								}
							}
							else
							{
								PaperSize paperSize2 = list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Legal")].PaperSize;
								sizeF = new SizeF(CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.LegalSizeF.Width), CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.LegalSizeF.Height));
							}
						}
						else
						{
							PaperSize paperSize3 = list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Tabloid")].PaperSize;
							sizeF = new SizeF(CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.TabloidSizeF.Width), CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.TabloidSizeF.Height));
						}
					}
					else
					{
						PaperSize paperSize4 = list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Letter")].PaperSize;
						sizeF = new SizeF(CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.LetterSizeF.Width), CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.LetterSizeF.Height));
					}
				}
				else
				{
					PaperSize paperSize5 = list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A3")].PaperSize;
					sizeF = new SizeF(CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.A3SizeF.Width), CreateFileHelper.GetPixValueFromMM((int)CreateFileHelper.A3SizeF.Height));
				}
			}
			if (ConfigManager.GetDefaultBlankPageOrinentation() == "Landscape")
			{
				float height = sizeF.Height;
				sizeF.Height = sizeF.Width;
				sizeF.Width = height;
			}
			return sizeF;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0002BC39 File Offset: 0x00029E39
		private static float GetPixValueFromMM(int mm)
		{
			return (float)((double)(mm * 72) / 25.4);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0002BC4B File Offset: 0x00029E4B
		private static float GetPixValueFrom100inches(int inches)
		{
			return (float)(inches / 100 * 72);
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0002BC58 File Offset: 0x00029E58
		public static void OpenPDFFile(string file, string action = null)
		{
			char[] array = new char[] { '\\', '/', ' ' };
			string text = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(array);
			string fullName = Directory.GetParent(text).FullName;
			string text2 = Path.Combine(text, "pdfeditor.exe");
			string text3 = "\"" + file + "\"";
			if (!string.IsNullOrEmpty(action))
			{
				text3 = text3 + " -action " + action.Trim();
			}
			ProcessManager.RunProcess(text2, text3);
		}

		// Token: 0x04000447 RID: 1095
		private static SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x04000448 RID: 1096
		private static SizeF A3SizeF = new SizeF(279f, 420f);

		// Token: 0x04000449 RID: 1097
		private static SizeF LetterSizeF = new SizeF(216f, 279f);

		// Token: 0x0400044A RID: 1098
		private static SizeF TabloidSizeF = new SizeF(279f, 432f);

		// Token: 0x0400044B RID: 1099
		private static SizeF LegalSizeF = new SizeF(216f, 356f);
	}
}
