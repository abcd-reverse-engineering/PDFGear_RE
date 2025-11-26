using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;

namespace pdfconverter
{
	// Token: 0x0200001C RID: 28
	public static class PdfiumNetHelper
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x000037B0 File Offset: 0x000019B0
		public static async Task<int> GetPageCountAsync(string filePath, string pwd)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException("filePath");
			}
			return await Task.Run<int>(delegate
			{
				int count;
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (PdfDocument pdfDocument = PdfDocument.Load(fileStream, null, pwd, true))
					{
						count = pdfDocument.Pages.Count;
					}
				}
				return count;
			}).ConfigureAwait(false);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000037FC File Offset: 0x000019FC
		public static async Task<bool> MergeAsync(global::System.Collections.Generic.IReadOnlyList<PdfiumPdfRange> inputFiles, string outputFile, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			bool flag;
			if (string.IsNullOrEmpty(outputFile) || inputFiles == null || inputFiles.Count == 0)
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				int num = 0;
				using (PdfDocument outputDoc = PdfDocument.CreateNew(null))
				{
					for (int i = 0; i < inputFiles.Count; i++)
					{
						cancellationToken.ThrowIfCancellationRequested();
						PdfiumPdfRange pdfiumPdfRange = inputFiles[i];
						if (pdfiumPdfRange != null && !string.IsNullOrEmpty(pdfiumPdfRange.FilePath) && pdfiumPdfRange.EndPageIndex >= pdfiumPdfRange.StartPageIndex)
						{
							try
							{
								string text = ((pdfiumPdfRange.Password == "") ? null : pdfiumPdfRange.Password);
								using (FileStream fileStream = new FileStream(pdfiumPdfRange.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
								{
									using (PdfDocument pdfDocument = PdfDocument.Load(fileStream, null, text, true))
									{
										PdfiumNetHelper.TryFixResource(pdfDocument, pdfiumPdfRange.StartPageIndex, pdfiumPdfRange.EndPageIndex);
										outputDoc.Pages.ImportPages(pdfDocument, string.Format("{0}-{1}", pdfiumPdfRange.StartPageIndex + 1, pdfiumPdfRange.EndPageIndex + 1), outputDoc.Pages.Count);
									}
								}
								num++;
							}
							catch
							{
							}
						}
						if (progress != null)
						{
							progress.Report(0.9 / (double)inputFiles.Count * (double)(i + 1));
						}
					}
					cancellationToken.ThrowIfCancellationRequested();
					if (num != 0)
					{
						try
						{
							if (File.Exists(outputFile))
							{
								File.Delete(outputFile);
							}
							cancellationToken.ThrowIfCancellationRequested();
							using (FileStream outputStream = File.OpenWrite(outputFile))
							{
								outputDoc.Save(outputStream, SaveFlags.NoIncremental, 0);
								await outputStream.FlushAsync(cancellationToken);
							}
							FileStream outputStream = null;
							if (progress != null)
							{
								progress.Report(1.0);
							}
							return true;
						}
						catch
						{
						}
					}
				}
				PdfDocument outputDoc = null;
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00003858 File Offset: 0x00001A58
		public static async Task<PdfiumNetHelper.SplitResult> SplitByRangeAsync(string inputFile, string password, string outputFolder, string splitRanges, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			PdfiumNetHelper.SplitResult splitResult;
			int[][] array;
			int num;
			if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFolder) || string.IsNullOrEmpty(splitRanges))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			else if (!PageRangeHelper.TryParsePageRange2(splitRanges, out array, out num))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			else
			{
				if (array != null && array.Length != 0)
				{
					if (!array.All((int[] c) => c.Length == 0))
					{
						try
						{
							string text = ((password == "") ? null : password);
							using (FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
							{
								using (PdfDocument doc = PdfDocument.Load(stream, null, text, true))
								{
									int[] array2 = (from c in array.SelectMany((int[] c) => c)
										orderby c
										select c).ToArray<int>();
									PdfiumNetHelper.TryFixResource(doc, array2[0], array2[array2.Length - 1]);
									FileInfo fileInfo = new FileInfo(inputFile);
									string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
									if (string.IsNullOrEmpty(text2))
									{
										text2 = "PDFgear Split";
									}
									return await PdfiumNetHelper.SplitCore(doc, text2, outputFolder, array, progress, cancellationToken);
								}
							}
						}
						catch (Exception ex) when (!(ex is OperationCanceledException))
						{
						}
						if (progress != null)
						{
							progress.Report(1.0);
						}
						return new PdfiumNetHelper.SplitResult(false, null);
					}
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			return splitResult;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000038C8 File Offset: 0x00001AC8
		public static async Task<PdfiumNetHelper.SplitResult> SplitByMaxPageCountAsync(string inputFile, string password, string outputFolder, int maxPageCount, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			PdfiumNetHelper.SplitResult splitResult;
			if (maxPageCount <= 0)
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			else if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFolder))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			else
			{
				try
				{
					string text = ((password == "") ? null : password);
					using (FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						using (PdfDocument doc = PdfDocument.Load(stream, null, text, true))
						{
							PdfiumNetHelper.TryFixResource(doc, 0, doc.Pages.Count - 1);
							FileInfo fileInfo = new FileInfo(inputFile);
							string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
							if (string.IsNullOrEmpty(text2))
							{
								text2 = "PDFgear Split";
							}
							List<int[]> list = new List<int[]>();
							for (int i = 0; i < doc.Pages.Count; i += maxPageCount)
							{
								int num = i;
								int num2 = Math.Min(i + maxPageCount - 1, doc.Pages.Count - 1);
								string.Format("{0}-{1}", num + 1, num2 + 1);
								list.Add(Enumerable.Range(num, num2 - num + 1).ToArray<int>());
							}
							return await PdfiumNetHelper.SplitCore(doc, text2, outputFolder, list.ToArray(), progress, cancellationToken);
						}
					}
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				splitResult = new PdfiumNetHelper.SplitResult(false, null);
			}
			return splitResult;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00003938 File Offset: 0x00001B38
		private static async Task<PdfiumNetHelper.SplitResult> SplitCore(PdfDocument doc, string fileNameWithoutExt, string outputFolder, int[][] pageIndexes, IProgress<double> progress, CancellationToken cancellationToken)
		{
			if (doc != null && !string.IsNullOrEmpty(fileNameWithoutExt) && !string.IsNullOrEmpty(outputFolder) && pageIndexes != null && pageIndexes.Length != 0)
			{
				if (!pageIndexes.All((int[] c) => c.Length == 0))
				{
					cancellationToken.ThrowIfCancellationRequested();
					List<string> outputFileList = new List<string>();
					Func<int, bool> <>9__1;
					for (int i = 0; i < pageIndexes.Length; i++)
					{
						IEnumerable<int> enumerable = pageIndexes[i];
						Func<int, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (int c) => c >= 0 && c < doc.Pages.Count);
						}
						int[] array = enumerable.Where(func).ToArray<int>();
						if (array.Length != 0)
						{
							try
							{
								string outputFullPath;
								using (PdfDocument tmpDoc = PdfDocument.CreateNew(null))
								{
									string text = array.ConvertToRange();
									tmpDoc.Pages.ImportPages(doc, text, 0);
									string text2 = fileNameWithoutExt + " [" + text + "].pdf";
									outputFullPath = Path.Combine(outputFolder, text2);
									if (File.Exists(outputFullPath))
									{
										File.Delete(outputFullPath);
									}
									cancellationToken.ThrowIfCancellationRequested();
									using (FileStream tmpStream = File.OpenWrite(outputFullPath))
									{
										tmpDoc.Save(tmpStream, SaveFlags.NoIncremental, 0);
										await tmpStream.FlushAsync(cancellationToken);
									}
									FileStream tmpStream = null;
								}
								PdfDocument tmpDoc = null;
								outputFileList.Add(outputFullPath);
								outputFullPath = null;
							}
							catch
							{
							}
						}
						if (progress != null)
						{
							progress.Report(1.0 / (double)pageIndexes.Length * (double)(i + 1));
						}
					}
					return new PdfiumNetHelper.SplitResult(outputFileList.Count > 0, outputFileList);
				}
			}
			return new PdfiumNetHelper.SplitResult(false, null);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000039A8 File Offset: 0x00001BA8
		private static void TryFixResource(PdfDocument doc, int startPage, int endPage)
		{
			if (doc == null)
			{
				return;
			}
			if (startPage < 0 || endPage < 0)
			{
				return;
			}
			endPage = Math.Min(endPage, doc.Pages.Count - 1);
			if (endPage < startPage)
			{
				return;
			}
			try
			{
				for (int i = startPage; i <= endPage; i++)
				{
					try
					{
						PdfPage pdfPage = doc.Pages[i];
						if (!pdfPage.Dictionary.ContainsKey("Resources"))
						{
							PdfTypeBase pdfTypeBase = PdfiumNetHelper.FindParentResources(pdfPage);
							if (pdfTypeBase != null)
							{
								pdfPage.Dictionary["Resources"] = PdfiumNetHelper.DeepClone(pdfTypeBase);
							}
						}
						pdfPage.Dispose();
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00003A58 File Offset: 0x00001C58
		private static PdfTypeBase FindParentResources(PdfPage page)
		{
			if (((page != null) ? page.Dictionary : null) == null)
			{
				return null;
			}
			for (PdfTypeDictionary pdfTypeDictionary = PdfiumNetHelper.<FindParentResources>g__GetParentPagesNode|6_0(page.Dictionary); pdfTypeDictionary != null; pdfTypeDictionary = PdfiumNetHelper.<FindParentResources>g__GetParentPagesNode|6_0(pdfTypeDictionary))
			{
				PdfTypeBase pdfTypeBase = PdfiumNetHelper.<FindParentResources>g__GetResources|6_1(pdfTypeDictionary);
				if (pdfTypeBase != null)
				{
					return pdfTypeBase;
				}
			}
			return null;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00003A9C File Offset: 0x00001C9C
		private static PdfTypeBase DeepClone(PdfTypeBase obj)
		{
			if (obj == null)
			{
				return null;
			}
			PdfTypeIndirect pdfTypeIndirect = obj as PdfTypeIndirect;
			if (pdfTypeIndirect != null)
			{
				return pdfTypeIndirect.Clone(false);
			}
			if (obj.Is<PdfTypeBoolean>() || obj.Is<PdfTypeName>() || obj.Is<PdfTypeNull>() || obj.Is<PdfTypeNumber>() || obj.Is<PdfTypeString>() || obj.Is<PdfTypeUnknown>() || obj.Is<PdfTypeStream>())
			{
				return obj.Clone(false);
			}
			if (obj.Is<PdfTypeArray>())
			{
				PdfTypeArray pdfTypeArray = PdfTypeArray.Create();
				foreach (PdfTypeBase pdfTypeBase in obj.As<PdfTypeArray>(true))
				{
					pdfTypeArray.Add(PdfiumNetHelper.DeepClone(pdfTypeBase));
				}
				return pdfTypeArray;
			}
			if (obj.Is<PdfTypeDictionary>())
			{
				PdfTypeDictionary pdfTypeDictionary = PdfTypeDictionary.Create();
				foreach (KeyValuePair<string, PdfTypeBase> keyValuePair in obj.As<PdfTypeDictionary>(true))
				{
					pdfTypeDictionary[keyValuePair.Key] = PdfiumNetHelper.DeepClone(keyValuePair.Value);
				}
				return pdfTypeDictionary;
			}
			return null;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00003BC0 File Offset: 0x00001DC0
		[CompilerGenerated]
		internal static PdfTypeDictionary <FindParentResources>g__GetParentPagesNode|6_0(PdfTypeDictionary _dict)
		{
			PdfTypeBase pdfTypeBase;
			if (_dict.TryGetValue("Parent", out pdfTypeBase) && pdfTypeBase.Is<PdfTypeDictionary>())
			{
				return pdfTypeBase.As<PdfTypeDictionary>(true);
			}
			return null;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00003BF0 File Offset: 0x00001DF0
		[CompilerGenerated]
		internal static PdfTypeBase <FindParentResources>g__GetResources|6_1(PdfTypeBase _pagesNode)
		{
			PdfTypeBase pdfTypeBase;
			if (_pagesNode.Is<PdfTypeDictionary>() && _pagesNode.As<PdfTypeDictionary>(true).TryGetValue("Resources", out pdfTypeBase) && pdfTypeBase.Is<PdfTypeDictionary>())
			{
				return pdfTypeBase;
			}
			return null;
		}

		// Token: 0x020000B4 RID: 180
		public class SplitResult
		{
			// Token: 0x0600078A RID: 1930 RVA: 0x0001C246 File Offset: 0x0001A446
			public SplitResult(bool success, global::System.Collections.Generic.IReadOnlyList<string> outputFiles)
			{
				this.Success = success;
				this.OutputFiles = outputFiles ?? Array.Empty<string>();
			}

			// Token: 0x1700025E RID: 606
			// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001C265 File Offset: 0x0001A465
			public bool Success { get; }

			// Token: 0x1700025F RID: 607
			// (get) Token: 0x0600078C RID: 1932 RVA: 0x0001C26D File Offset: 0x0001A46D
			public global::System.Collections.Generic.IReadOnlyList<string> OutputFiles { get; }
		}
	}
}
