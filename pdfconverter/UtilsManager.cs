using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CommonLib.Common;

namespace pdfconverter
{
	// Token: 0x02000020 RID: 32
	internal class UtilsManager
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x00004064 File Offset: 0x00002264
		public static void OpenFile(string file)
		{
			if (!string.IsNullOrWhiteSpace(file))
			{
				try
				{
					Process.Start(file);
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004098 File Offset: 0x00002298
		public static void OpenFileInExplore(string file, bool isFile)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				return;
			}
			try
			{
				char[] array = new char[] { '\\', '/', ' ' };
				ExplorerUtils.SelectItemInExplorerAsync(file.TrimEnd(array), default(CancellationToken));
			}
			catch
			{
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000040EC File Offset: 0x000022EC
		public static void OpenFolderInExplore(string folder)
		{
			if (string.IsNullOrWhiteSpace(folder))
			{
				return;
			}
			try
			{
				char[] array = new char[] { '\\', '/', ' ' };
				string text = folder.TrimEnd(array);
				string text2 = text;
				if (text2.Length > 240)
				{
					string text3 = Directory.GetFiles(text2).FirstOrDefault<string>();
					ExplorerUtils.OpenFolderAsync(text, new string[] { text3 }, default(CancellationToken));
				}
				else
				{
					Process.Start("explorer.exe", text2);
				}
			}
			catch
			{
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004188 File Offset: 0x00002388
		public static bool IsPDFFile(string file)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				return false;
			}
			string extension = Path.GetExtension(file);
			return !string.IsNullOrWhiteSpace(extension) && extension.Equals(".pdf", StringComparison.CurrentCultureIgnoreCase);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000041C4 File Offset: 0x000023C4
		public static bool IsnotSupportFile(string file, string[] supportExt)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				return true;
			}
			string extension = Path.GetExtension(file);
			return string.IsNullOrWhiteSpace(extension) || Array.IndexOf<string>(supportExt, extension.ToLower()) < 0;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004200 File Offset: 0x00002400
		public static string getValidFileName(string path, string fileName, string extention = ".pdf")
		{
			if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(fileName))
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string text = fileName;
				for (int i = 2; i < 100; i++)
				{
					string text2 = path + "\\" + text + extention;
					if (!Directory.Exists(text2) && !File.Exists(text2))
					{
						return text;
					}
					text = fileName + string.Format(" {0}", i);
				}
			}
			return "";
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000042A4 File Offset: 0x000024A4
		public static string getPDFFileName(string path, string fileName)
		{
			if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(fileName))
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
				string text = fileNameWithoutExtension;
				for (int i = 2; i < 100; i++)
				{
					string text2 = path + "\\" + text + ".pdf";
					if (!File.Exists(text2))
					{
						return text2;
					}
					text = fileNameWithoutExtension + string.Format(" {0}", i);
				}
			}
			return "";
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000433C File Offset: 0x0000253C
		public static string getPDFResult(string path, string fileName)
		{
			if (!string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(fileName))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
				string text = fileNameWithoutExtension;
				for (int i = 2; i < 100; i++)
				{
					if (!File.Exists(path + "\\" + text + ".pdf"))
					{
						text = ((i == 3) ? fileNameWithoutExtension : (fileNameWithoutExtension + string.Format(" {0}", i - 2)));
						return path + "\\" + text + ".pdf";
					}
					text = fileNameWithoutExtension + string.Format(" {0}", i);
				}
			}
			return "";
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000043DC File Offset: 0x000025DC
		public static string getValidFolder(string path, string foldName)
		{
			if (!string.IsNullOrWhiteSpace(path))
			{
				if (string.IsNullOrWhiteSpace(foldName))
				{
					foldName = "PDF Files";
				}
				string text = foldName.Trim();
				for (int i = 1; i < 100; i++)
				{
					string text2 = path + "\\" + text;
					if (!Directory.Exists(text2) && !File.Exists(text2))
					{
						return text;
					}
					text = foldName + string.Format(" {0}", i);
				}
			}
			return "";
		}
	}
}
