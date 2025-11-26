using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommonLib.Common;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using pdfconverter.Properties;
using pdfconverter.Views;

namespace pdfconverter
{
	// Token: 0x02000017 RID: 23
	internal class ConvertManager
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x0000268C File Offset: 0x0000088C
		public static string selectPDFFile()
		{
			string text = "";
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			openFileDialog.Filter = "Portable Document Format(*.pdf)|*.pdf";
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				text = openFileDialog.FileName;
			}
			return text;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000026DC File Offset: 0x000008DC
		public static string[] selectMultiPDFFiles()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			openFileDialog.Filter = "Portable Document Format(*.pdf)|*.pdf";
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				return openFileDialog.FileNames;
			}
			return null;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00002728 File Offset: 0x00000928
		public static string[] selectMultiFiles(string typeName, string extention)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			openFileDialog.Filter = string.Concat(new string[] { typeName, "(*", extention, ")|*", extention });
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				return openFileDialog.FileNames;
			}
			return null;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00002794 File Offset: 0x00000994
		public static string selectOutputFolder(string defaultPath)
		{
			string text = "";
			string text2;
			if (!string.IsNullOrWhiteSpace(defaultPath) && Directory.Exists(defaultPath))
			{
				text2 = defaultPath;
			}
			else
			{
				text2 = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			}
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
			commonOpenFileDialog.IsFolderPicker = true;
			commonOpenFileDialog.InitialDirectory = text2;
			if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				text = commonOpenFileDialog.FileName.FullPathWithoutPrefix;
			}
			return text;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000027FC File Offset: 0x000009FC
		public static bool IsFileAdded(ObservableCollection<ConvertFileItem> list, string file)
		{
			if (list.Count > 0)
			{
				using (IEnumerator<ConvertFileItem> enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.convertFile.Equals(file))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00002864 File Offset: 0x00000A64
		public static bool IsFileAdded(ObservableCollection<PDFToImageItem> list, string file)
		{
			if (list.Count > 0)
			{
				using (IEnumerator<PDFToImageItem> enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.convertFile.Equals(file))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000028CC File Offset: 0x00000ACC
		public static FileCovertType parseConvertType(string typeStr)
		{
			FileCovertType fileCovertType = FileCovertType.Invalid;
			if (typeStr.Equals("pdftoword", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToWord;
			}
			else if (typeStr.Equals("pdftortf", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToRtf;
			}
			else if (typeStr.Equals("pdftoxls", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToXls;
			}
			else if (typeStr.Equals("pdftohtml", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToHtml;
			}
			else if (typeStr.Equals("pdftoxml", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToXml;
			}
			else if (typeStr.Equals("pdftotext", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToText;
			}
			else if (typeStr.Equals("pdftopng", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToPng;
			}
			else if (typeStr.Equals("pdftojpeg", StringComparison.CurrentCulture))
			{
				fileCovertType = FileCovertType.PDFToJpeg;
			}
			return fileCovertType;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000296C File Offset: 0x00000B6C
		public static string getTitle(ConvFromPDFType type)
		{
			string text = "";
			switch (type)
			{
			case ConvFromPDFType.PDFToWord:
				text = Resources.PDFtoWordText;
				break;
			case ConvFromPDFType.PDFToExcel:
				text = Resources.PDFtoExcelText;
				break;
			case ConvFromPDFType.PDFToPng:
				text = Resources.PDFtoPngText;
				break;
			case ConvFromPDFType.PDFToJpg:
				text = Resources.PDFtoJpegText;
				break;
			case ConvFromPDFType.PDFToTxt:
				text = Resources.PDFtoTextText;
				break;
			case ConvFromPDFType.PDFToHtml:
				text = Resources.PDFtoWebText;
				break;
			case ConvFromPDFType.PDFToXml:
				text = Resources.PDFtoXMLText;
				break;
			case ConvFromPDFType.PDFToRTF:
				text = Resources.PDFtoRTFText;
				break;
			case ConvFromPDFType.PDFToPPT:
				text = Resources.PDFtoPPTText;
				break;
			}
			return text;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000029F4 File Offset: 0x00000BF4
		public static string getOutputExt(OutputFormat format)
		{
			string text = "";
			switch (format)
			{
			case OutputFormat.Docx:
				text = ".docx";
				break;
			case OutputFormat.Rtf:
				text = ".rtf";
				break;
			case OutputFormat.Xls:
				text = ".xls";
				break;
			case OutputFormat.Html:
				text = ".html";
				break;
			case OutputFormat.Xml:
				text = ".xml";
				break;
			case OutputFormat.Text:
				text = ".txt";
				break;
			case OutputFormat.Png:
				text = ".png";
				break;
			case OutputFormat.Jpeg:
				text = ".jpeg";
				break;
			case OutputFormat.Ppt:
				text = ".pptx";
				break;
			}
			return text;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00002A7C File Offset: 0x00000C7C
		public static OCRLanguageID GetOCRLanguageID()
		{
			int num = ConfigManager.GetOCRLanguageID();
			OCRLanguageID ocrlanguageID = OCRLanguageID.Arabic;
			int num2 = Enum.GetNames(ocrlanguageID.GetType()).Count<string>();
			if (num < 0 || num >= num2)
			{
				num = 9;
			}
			if (!ConvertManager.OCRLanguages.ContainsKey((OCRLanguageID)num))
			{
				return OCRLanguageID.English;
			}
			return (OCRLanguageID)num;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00002AC4 File Offset: 0x00000CC4
		public static string GetOCROnlineLanguage()
		{
			int num = ConfigManager.GetOCRLanguageID();
			OCRLanguageID ocrlanguageID = OCRLanguageID.Arabic;
			int num2 = Enum.GetNames(ocrlanguageID.GetType()).Count<string>();
			if (num < 0 || num >= num2)
			{
				num = 9;
			}
			if (!ConvertManager.OCROnlineLanguages.ContainsKey((OCRLanguageID)num))
			{
				return string.Empty;
			}
			return ConvertManager.OCROnlineLanguages[(OCRLanguageID)num];
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00002B18 File Offset: 0x00000D18
		public static string getOCRLanguage()
		{
			OCRLanguageID ocrlanguageID = ConvertManager.GetOCRLanguageID();
			string text = ConvertManager.OCRLanguages[ocrlanguageID];
			return string.Format("Languages/{0}", text);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00002B44 File Offset: 0x00000D44
		public static string getOCRLanguageL10N()
		{
			OCRLanguageID ocrlanguageID = ConvertManager.GetOCRLanguageID();
			return ConvertManager.OCRLanguagesL10n[ocrlanguageID];
		}

		// Token: 0x040000E6 RID: 230
		public static readonly Dictionary<OCRLanguageID, string> OCRLanguages = new Dictionary<OCRLanguageID, string>(31)
		{
			{
				OCRLanguageID.Arabic,
				"Arabic"
			},
			{
				OCRLanguageID.Bulgarian,
				"Bulgarian"
			},
			{
				OCRLanguageID.Catalan,
				"Catalan"
			},
			{
				OCRLanguageID.ChineseSimplified,
				"Chinese_Simplified"
			},
			{
				OCRLanguageID.ChineseTraditional,
				"Chinese_Traditional"
			},
			{
				OCRLanguageID.Croatian,
				"Croatian"
			},
			{
				OCRLanguageID.Czech,
				"Czech"
			},
			{
				OCRLanguageID.Danish,
				"Danish"
			},
			{
				OCRLanguageID.Dutch,
				"Dutch"
			},
			{
				OCRLanguageID.English,
				"English"
			},
			{
				OCRLanguageID.Estonian,
				"Estonian"
			},
			{
				OCRLanguageID.Finish,
				"Finish"
			},
			{
				OCRLanguageID.French,
				"French"
			},
			{
				OCRLanguageID.German,
				"German"
			},
			{
				OCRLanguageID.Hungarian,
				"Hungarian"
			},
			{
				OCRLanguageID.Indonesian,
				"Indonesian"
			},
			{
				OCRLanguageID.Italian,
				"Italian"
			},
			{
				OCRLanguageID.Japanese,
				"Japanese"
			},
			{
				OCRLanguageID.Korean,
				"Korean"
			},
			{
				OCRLanguageID.Latvian,
				"Latvian"
			},
			{
				OCRLanguageID.Lithuanian,
				"Lithuanian"
			},
			{
				OCRLanguageID.Norwegian,
				"Norwegian"
			},
			{
				OCRLanguageID.Polish,
				"Polish"
			},
			{
				OCRLanguageID.Portuguese,
				"Portuguese"
			},
			{
				OCRLanguageID.Romanian,
				"Romanian"
			},
			{
				OCRLanguageID.Russian,
				"Russian"
			},
			{
				OCRLanguageID.Slovak,
				"Slovak"
			},
			{
				OCRLanguageID.Slovenian,
				"Slovenian"
			},
			{
				OCRLanguageID.Spanish,
				"Spanish"
			},
			{
				OCRLanguageID.Swedish,
				"Swedish"
			},
			{
				OCRLanguageID.Turkish,
				"Turkish"
			}
		};

		// Token: 0x040000E7 RID: 231
		public static readonly Dictionary<OCRLanguageID, string> OCROnlineLanguages = new Dictionary<OCRLanguageID, string>(26)
		{
			{
				OCRLanguageID.Bulgarian,
				"BG_BG"
			},
			{
				OCRLanguageID.ChineseSimplified,
				"ZH_CN"
			},
			{
				OCRLanguageID.ChineseTraditional,
				"ZH_HK"
			},
			{
				OCRLanguageID.Croatian,
				"HR_HR"
			},
			{
				OCRLanguageID.Czech,
				"CS_CZ"
			},
			{
				OCRLanguageID.Danish,
				"DA_DK"
			},
			{
				OCRLanguageID.English,
				"EN_US"
			},
			{
				OCRLanguageID.Estonian,
				"ET_EE"
			},
			{
				OCRLanguageID.Finish,
				"FI_FI"
			},
			{
				OCRLanguageID.French,
				"FR_FR"
			},
			{
				OCRLanguageID.German,
				"DE_DE"
			},
			{
				OCRLanguageID.Hungarian,
				"HU_HU"
			},
			{
				OCRLanguageID.Italian,
				"IT_IT"
			},
			{
				OCRLanguageID.Japanese,
				"JA_JP"
			},
			{
				OCRLanguageID.Korean,
				"KO_KR"
			},
			{
				OCRLanguageID.Latvian,
				"LV_LV"
			},
			{
				OCRLanguageID.Lithuanian,
				"LT_LT"
			},
			{
				OCRLanguageID.Norwegian,
				"NO_NO"
			},
			{
				OCRLanguageID.Polish,
				"PL_PL"
			},
			{
				OCRLanguageID.Romanian,
				"RO_RO"
			},
			{
				OCRLanguageID.Russian,
				"RU_RU"
			},
			{
				OCRLanguageID.Slovak,
				"SK_SK"
			},
			{
				OCRLanguageID.Slovenian,
				"SL_SI"
			},
			{
				OCRLanguageID.Spanish,
				"ES_ES"
			},
			{
				OCRLanguageID.Swedish,
				"SV_SE"
			},
			{
				OCRLanguageID.Turkish,
				"TR_TR"
			}
		};

		// Token: 0x040000E8 RID: 232
		public static readonly Dictionary<OCRLanguageID, string> OCRLanguagesL10n = new Dictionary<OCRLanguageID, string>(31)
		{
			{
				OCRLanguageID.Arabic,
				"عربى"
			},
			{
				OCRLanguageID.Bulgarian,
				"български"
			},
			{
				OCRLanguageID.Catalan,
				"Català"
			},
			{
				OCRLanguageID.ChineseSimplified,
				"中文 (简体)"
			},
			{
				OCRLanguageID.ChineseTraditional,
				"中文 (繁體)"
			},
			{
				OCRLanguageID.Croatian,
				"Hrvatski"
			},
			{
				OCRLanguageID.Czech,
				"čeština"
			},
			{
				OCRLanguageID.Danish,
				"dansk"
			},
			{
				OCRLanguageID.Dutch,
				"Nederlands"
			},
			{
				OCRLanguageID.English,
				"English"
			},
			{
				OCRLanguageID.Estonian,
				"Eesti"
			},
			{
				OCRLanguageID.Finish,
				"Suomi"
			},
			{
				OCRLanguageID.French,
				"français"
			},
			{
				OCRLanguageID.German,
				"Deutsch"
			},
			{
				OCRLanguageID.Hungarian,
				"Magyar"
			},
			{
				OCRLanguageID.Indonesian,
				"bahasa Indonesia"
			},
			{
				OCRLanguageID.Italian,
				"Italiano"
			},
			{
				OCRLanguageID.Japanese,
				"日本語"
			},
			{
				OCRLanguageID.Korean,
				"한국어"
			},
			{
				OCRLanguageID.Latvian,
				"latviešu"
			},
			{
				OCRLanguageID.Lithuanian,
				"Lietuvių"
			},
			{
				OCRLanguageID.Norwegian,
				"norsk"
			},
			{
				OCRLanguageID.Polish,
				"Polski"
			},
			{
				OCRLanguageID.Portuguese,
				"Português"
			},
			{
				OCRLanguageID.Romanian,
				"Română"
			},
			{
				OCRLanguageID.Russian,
				"русский"
			},
			{
				OCRLanguageID.Slovak,
				"Slovenčina"
			},
			{
				OCRLanguageID.Slovenian,
				"Slovenski"
			},
			{
				OCRLanguageID.Spanish,
				"Español"
			},
			{
				OCRLanguageID.Swedish,
				"svenska"
			},
			{
				OCRLanguageID.Turkish,
				"Türkçe"
			}
		};
	}
}
