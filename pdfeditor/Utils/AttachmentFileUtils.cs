using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x02000074 RID: 116
	public static class AttachmentFileUtils
	{
		// Token: 0x060008A2 RID: 2210 RVA: 0x0002AB1C File Offset: 0x00028D1C
		public static async Task<bool> AttachmentSaveAsFileFromAnnotation(PdfFileAttachmentAnnotation annot, string initialDirectory, bool openAfterSaved)
		{
			try
			{
				PdfFileSpecification file = ((annot != null) ? annot.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file))
				{
					string fileName2 = file.FileName;
					SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						Filter = "All Files(*.*)|*.*",
						CreatePrompt = false,
						OverwritePrompt = true,
						InitialDirectory = initialDirectory,
						FileName = fileName2
					};
					if (saveFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
					{
						string fileName = saveFileDialog.FileName;
						bool flag = false;
						using (Stream stream = await AttachmentFileUtils.ExtraAttachmentFromAnnotation(annot, fileName).ConfigureAwait(false))
						{
							if (stream != null)
							{
								flag = true;
								List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
								lock (list)
								{
									AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
								}
							}
						}
						if (flag && openAfterSaved)
						{
							await AttachmentFileUtils.OpenFileAsync(fileName);
						}
						return flag;
					}
				}
				file = null;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0002AB70 File Offset: 0x00028D70
		public static async Task<bool> AttachmentSaveAsFileFromAnnotation(PdfFileAttachmentAnnotation annot, string initialDirectory)
		{
			try
			{
				PdfFileSpecification file = ((annot != null) ? annot.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file))
				{
					string fileName2 = file.FileName;
					SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						Filter = "All Files(*.*)|*.*",
						CreatePrompt = false,
						OverwritePrompt = true,
						InitialDirectory = initialDirectory,
						FileName = fileName2
					};
					if (saveFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
					{
						string fileName = saveFileDialog.FileName;
						bool flag = false;
						using (Stream stream = await AttachmentFileUtils.ExtraAttachmentFromAnnotation(annot, fileName).ConfigureAwait(false))
						{
							if (stream != null)
							{
								flag = true;
								List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
								lock (list)
								{
									AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
								}
							}
						}
						if (flag)
						{
							string directoryName = Path.GetDirectoryName(fileName);
							if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(directoryName))
							{
								await AttachmentFileUtils.ShowIfOpenFolderAfterSave(directoryName, new string[] { fileName });
							}
						}
						return flag;
					}
				}
				file = null;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0002ABBC File Offset: 0x00028DBC
		public static async Task<bool> AttachmentSaveAsFileFromAnnotationWithTargetFolder(PdfFileAttachmentAnnotation annot, string targetFolder, bool openAfterSaved)
		{
			try
			{
				PdfFileSpecification file = ((annot != null) ? annot.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file) && Directory.Exists(targetFolder))
				{
					string fileName = Path.Combine(targetFolder, file.FileName);
					if (File.Exists(fileName) && ModernMessageBox.Show("File '" + file.FileName + "' already exist,Whether you want to replace it? ", UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.No)
					{
						return true;
					}
					bool flag = false;
					using (Stream stream = await AttachmentFileUtils.ExtraAttachmentFromAnnotation(annot, fileName).ConfigureAwait(false))
					{
						if (stream != null)
						{
							flag = true;
							List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
							lock (list)
							{
								AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
							}
						}
					}
					if (flag && openAfterSaved)
					{
						await AttachmentFileUtils.OpenFileAsync(fileName);
					}
					return flag;
				}
				else
				{
					file = null;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0002AC10 File Offset: 0x00028E10
		public static async Task<bool> AttachmentSaveAsFileFromPDFAttachment(PdfAttachment attachment, string initialDirectory, bool openAfterSaved)
		{
			try
			{
				PdfFileSpecification file = ((attachment != null) ? attachment.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file))
				{
					string fileName2 = file.FileName;
					SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						Filter = "All Files(*.*)|*.*",
						CreatePrompt = false,
						OverwritePrompt = true,
						InitialDirectory = initialDirectory,
						FileName = fileName2
					};
					if (saveFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
					{
						string fileName = saveFileDialog.FileName;
						bool flag = false;
						using (Stream stream = await AttachmentFileUtils.ExtraEmbeddedFileFromFileSpec(attachment.FileSpecification, fileName).ConfigureAwait(false))
						{
							if (stream != null)
							{
								flag = true;
								List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
								lock (list)
								{
									AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
								}
							}
						}
						if (flag && openAfterSaved)
						{
							await AttachmentFileUtils.OpenFileAsync(fileName);
						}
						return flag;
					}
				}
				file = null;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x0002AC64 File Offset: 0x00028E64
		public static async Task<bool> AttachmentSaveAsFileFromPDFAttachment(PdfAttachment attachment, string initialDirectory)
		{
			try
			{
				PdfFileSpecification file = ((attachment != null) ? attachment.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file))
				{
					string fileName2 = file.FileName;
					SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						Filter = "All Files(*.*)|*.*",
						CreatePrompt = false,
						OverwritePrompt = true,
						InitialDirectory = initialDirectory,
						FileName = fileName2
					};
					if (saveFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
					{
						string fileName = saveFileDialog.FileName;
						bool flag = false;
						using (Stream stream = await AttachmentFileUtils.ExtraEmbeddedFileFromFileSpec(attachment.FileSpecification, fileName).ConfigureAwait(false))
						{
							if (stream != null)
							{
								flag = true;
								List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
								lock (list)
								{
									AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
								}
							}
						}
						if (flag)
						{
							string directoryName = Path.GetDirectoryName(fileName);
							if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(directoryName))
							{
								await AttachmentFileUtils.ShowIfOpenFolderAfterSave(directoryName, new string[] { fileName });
							}
						}
						return flag;
					}
				}
				file = null;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x0002ACB0 File Offset: 0x00028EB0
		public static async Task<bool> AttachmentSaveAsFileFromPDFAttachmentWithTargetFolder(PdfAttachment attachment, string targetFolder, bool openAfterSaved)
		{
			try
			{
				PdfFileSpecification file = ((attachment != null) ? attachment.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(file) && Directory.Exists(targetFolder))
				{
					string fileName = Path.Combine(targetFolder, file.FileName);
					if (File.Exists(fileName) && ModernMessageBox.Show("File '" + file.FileName + "' already exist,Whether you want to replace it? ", UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.No)
					{
						return true;
					}
					bool flag = false;
					using (Stream stream = await AttachmentFileUtils.ExtraEmbeddedFileFromFileSpec(attachment.FileSpecification, fileName).ConfigureAwait(false))
					{
						if (stream != null)
						{
							flag = true;
							List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
							lock (list)
							{
								AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, fileName));
							}
						}
					}
					if (flag && openAfterSaved)
					{
						await AttachmentFileUtils.OpenFileAsync(fileName);
					}
					return flag;
				}
				else
				{
					file = null;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x0002AD04 File Offset: 0x00028F04
		public static async Task ShowIfOpenFolderAfterSave(string folderPath, string[] selectedItems)
		{
			AttachmentFileUtils.<>c__DisplayClass7_0 CS$<>8__locals1 = new AttachmentFileUtils.<>c__DisplayClass7_0();
			CS$<>8__locals1.folderPath = folderPath;
			CS$<>8__locals1.selectedItems = selectedItems;
			await Application.Current.Dispatcher.Invoke<Task>(delegate
			{
				AttachmentFileUtils.<>c__DisplayClass7_0.<<ShowIfOpenFolderAfterSave>b__0>d <<ShowIfOpenFolderAfterSave>b__0>d;
				<<ShowIfOpenFolderAfterSave>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ShowIfOpenFolderAfterSave>b__0>d.<>4__this = CS$<>8__locals1;
				<<ShowIfOpenFolderAfterSave>b__0>d.<>1__state = -1;
				<<ShowIfOpenFolderAfterSave>b__0>d.<>t__builder.Start<AttachmentFileUtils.<>c__DisplayClass7_0.<<ShowIfOpenFolderAfterSave>b__0>d>(ref <<ShowIfOpenFolderAfterSave>b__0>d);
				return <<ShowIfOpenFolderAfterSave>b__0>d.<>t__builder.Task;
			});
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x0002AD50 File Offset: 0x00028F50
		public static async Task<bool> OpenFileSpecAsync(PdfFileSpecification fileSpec)
		{
			bool flag;
			if (fileSpec == null)
			{
				flag = false;
			}
			else
			{
				try
				{
					if (AttachmentFileUtils.IsUrl(fileSpec))
					{
						Uri uri;
						if (!Uri.TryCreate(fileSpec.FileName, UriKind.RelativeOrAbsolute, out uri))
						{
							goto IL_01E8;
						}
						ProcessStartInfo processStartInfo = new ProcessStartInfo
						{
							FileName = uri.ToString(),
							UseShellExecute = true
						};
						try
						{
							new Process
							{
								StartInfo = processStartInfo
							}.Start();
							goto IL_01E8;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							goto IL_01E8;
						}
					}
					if (fileSpec.EmbeddedFile != null)
					{
						return await AttachmentFileUtils.OpenFileAsync(await AttachmentFileUtils.ExtraEmbeddedFileFromFileSpec(fileSpec));
					}
					if (fileSpec.FileName != null)
					{
						return await AttachmentFileUtils.OpenFileAsync(fileSpec.FileName);
					}
					IL_01E8:;
				}
				catch
				{
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0002AD94 File Offset: 0x00028F94
		public static async Task<string> ExtraEmbeddedFileFromFileSpec(PdfFileSpecification fileSpec)
		{
			try
			{
				if (fileSpec == null || AttachmentFileUtils.IsUrl(fileSpec))
				{
					return null;
				}
				string filePath;
				if (AttachmentFileUtils.TryGetFilePathFromCache(fileSpec.EmbeddedFile, out filePath) && File.Exists(filePath))
				{
					return filePath;
				}
				string tempFileName = Path.GetTempFileName();
				File.Delete(tempFileName);
				Directory.CreateDirectory(tempFileName);
				filePath = Path.Combine(tempFileName, fileSpec.FileName);
				bool flag = false;
				using (Stream stream = await AttachmentFileUtils.ExtraEmbeddedFileFromFileSpec(fileSpec, filePath).ConfigureAwait(false))
				{
					if (stream != null)
					{
						List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
						lock (list)
						{
							AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(fileSpec.EmbeddedFile, filePath));
						}
						flag = true;
					}
				}
				if (flag)
				{
					try
					{
						FileAttributes attributes = File.GetAttributes(filePath);
						File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);
					}
					catch
					{
					}
					return filePath;
				}
				filePath = null;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x0002ADD8 File Offset: 0x00028FD8
		public static async Task<Stream> ExtraEmbeddedFileFromFileSpec(PdfFileSpecification fileSpec, string targetFilePath)
		{
			try
			{
				if (fileSpec == null || AttachmentFileUtils.IsUrl(fileSpec))
				{
					return null;
				}
				return await AttachmentFileUtils.ExtraEmbeddedFileToFileAsync((fileSpec != null) ? fileSpec.EmbeddedFile : null, targetFilePath).ConfigureAwait(false);
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x0002AE24 File Offset: 0x00029024
		public static async Task<Stream> ExtraEmbeddedFileToFileAsync(PdfFile pdfFile, string targetFilePath)
		{
			Stream stream;
			if (pdfFile == null)
			{
				stream = null;
			}
			else
			{
				stream = await AttachmentFileUtils.WriteToFileAsync(pdfFile, targetFilePath).ConfigureAwait(false);
			}
			return stream;
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0002AE70 File Offset: 0x00029070
		public static async Task<bool> OpenAttachmentFromAnnotation(PdfFileAttachmentAnnotation annot)
		{
			try
			{
				PdfFileSpecification pdfFileSpecification = ((annot != null) ? annot.FileSpecification : null);
				if (!AttachmentFileUtils.IsUrl(pdfFileSpecification))
				{
					return await AttachmentFileUtils.OpenFileAsync(await AttachmentFileUtils.ExtraAttachmentFromAnnotation(annot));
				}
				Uri uri;
				if (Uri.TryCreate(pdfFileSpecification.FileName, UriKind.RelativeOrAbsolute, out uri))
				{
					Process.Start(uri.ToString());
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0002AEB4 File Offset: 0x000290B4
		public static async Task<string> ExtraAttachmentFromAnnotation(PdfFileAttachmentAnnotation annot)
		{
			try
			{
				PdfFileSpecification file = ((annot != null) ? annot.FileSpecification : null);
				if (file == null || AttachmentFileUtils.IsUrl(file))
				{
					return null;
				}
				string filePath;
				if (AttachmentFileUtils.TryGetFilePathFromCache(file.EmbeddedFile, out filePath) && File.Exists(filePath))
				{
					return filePath;
				}
				string tempFileName = Path.GetTempFileName();
				File.Delete(tempFileName);
				Directory.CreateDirectory(tempFileName);
				filePath = Path.Combine(tempFileName, file.FileName);
				bool flag = false;
				using (Stream stream = await AttachmentFileUtils.ExtraAttachmentFromAnnotation(annot, filePath).ConfigureAwait(false))
				{
					if (stream != null)
					{
						List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
						lock (list)
						{
							AttachmentFileUtils.fileCache.Add(new AttachmentFileUtils.AttachmentFileCache(file.EmbeddedFile, filePath));
						}
						flag = true;
					}
				}
				if (flag)
				{
					try
					{
						FileAttributes attributes = File.GetAttributes(filePath);
						File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);
					}
					catch
					{
					}
					return filePath;
				}
				file = null;
				filePath = null;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0002AEF8 File Offset: 0x000290F8
		private static async Task<bool> OpenFileAsync(string filePath)
		{
			bool flag;
			if (string.IsNullOrEmpty(filePath))
			{
				flag = false;
			}
			else
			{
				try
				{
					FileInfo fileInfo = new FileInfo(filePath);
					if (!fileInfo.Exists || fileInfo.Length == 0L)
					{
						return false;
					}
					if (fileInfo.Extension.ToLowerInvariant() == ".pdf")
					{
						Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdfeditor.exe"), "\"" + filePath + "\"");
					}
					else
					{
						Process.Start(filePath);
					}
					return true;
				}
				catch
				{
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x0002AF3C File Offset: 0x0002913C
		private static bool TryGetFilePathFromCache(PdfFile pdfFile, out string fileName)
		{
			fileName = null;
			if (pdfFile == null)
			{
				return false;
			}
			List<AttachmentFileUtils.AttachmentFileCache> list = AttachmentFileUtils.fileCache;
			lock (list)
			{
				for (int i = AttachmentFileUtils.fileCache.Count - 1; i >= 0; i--)
				{
					AttachmentFileUtils.AttachmentFileCache attachmentFileCache = AttachmentFileUtils.fileCache[i];
					PdfFile pdfFile2 = attachmentFileCache.PdfFile;
					if (pdfFile2 == null)
					{
						AttachmentFileUtils.fileCache.RemoveAt(i);
					}
					else if (pdfFile2 == pdfFile)
					{
						fileName = attachmentFileCache.Path;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002AFE0 File Offset: 0x000291E0
		public static async Task<Stream> ExtraAttachmentFromAnnotation(PdfFileAttachmentAnnotation annot, string targetFilePath)
		{
			try
			{
				PdfFileSpecification pdfFileSpecification = ((annot != null) ? annot.FileSpecification : null);
				if (pdfFileSpecification == null || AttachmentFileUtils.IsUrl(pdfFileSpecification))
				{
					return null;
				}
				PdfFile pdfFile;
				if (annot == null)
				{
					pdfFile = null;
				}
				else
				{
					PdfFileSpecification fileSpecification = annot.FileSpecification;
					pdfFile = ((fileSpecification != null) ? fileSpecification.EmbeddedFile : null);
				}
				return await AttachmentFileUtils.ExtraAttachmentToFileAsync(pdfFile, targetFilePath).ConfigureAwait(false);
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0002B02C File Offset: 0x0002922C
		public static async Task<Stream> ExtraAttachmentToFileAsync(PdfFile pdfFile, string targetFilePath)
		{
			Stream stream;
			if (pdfFile == null)
			{
				stream = null;
			}
			else
			{
				stream = await AttachmentFileUtils.WriteToFileAsync(pdfFile, targetFilePath).ConfigureAwait(false);
			}
			return stream;
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0002B078 File Offset: 0x00029278
		private static async Task<Stream> WriteToFileAsync(PdfFile pdfFile, string targetFilePath)
		{
			Stream stream = null;
			try
			{
				if (pdfFile == null)
				{
					return null;
				}
				byte[] decodedData = pdfFile.Stream.DecodedData;
				stream = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
				await stream.WriteAsync(decodedData, 0, decodedData.Length).ConfigureAwait(false);
				await stream.FlushAsync().ConfigureAwait(false);
				stream.SetLength(stream.Position);
				return stream;
			}
			catch
			{
				Stream stream2 = stream;
				if (stream2 != null)
				{
					stream2.Dispose();
				}
			}
			return null;
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0002B0C4 File Offset: 0x000292C4
		public static bool IsUrl(PdfFileSpecification fileSpec)
		{
			return !(fileSpec == null) && fileSpec.IsExists("FS") && string.Equals(fileSpec.Dictionary["FS"].As<PdfTypeName>(true).Value, "URL", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x0002B114 File Offset: 0x00029314
		public static global::System.Collections.Generic.IReadOnlyList<AttachmentFileUtils.PdfFileAttachmentAnnotationResult> GetAllFileAttachmentAnnotations(PdfDocument document)
		{
			List<AttachmentFileUtils.PdfFileAttachmentAnnotationResult> list = new List<AttachmentFileUtils.PdfFileAttachmentAnnotationResult>();
			int num = Pdfium.FPDF_GetPageCount(document.Handle);
			for (int i = 0; i < num; i++)
			{
				IntPtr intPtr = Pdfium.FPDF_GetPageDictionary(document.Handle, i);
				if (intPtr != IntPtr.Zero)
				{
					PdfTypeDictionary pdfTypeDictionary = new PdfTypeDictionary(intPtr);
					if (pdfTypeDictionary.ContainsKey("Annots") && pdfTypeDictionary["Annots"].Is<PdfTypeArray>())
					{
						PdfTypeArray pdfTypeArray = pdfTypeDictionary["Annots"].As<PdfTypeArray>(true);
						for (int j = 0; j < pdfTypeArray.Count; j++)
						{
							if (pdfTypeArray[j].Is<PdfTypeDictionary>())
							{
								PdfTypeDictionary annotDict = pdfTypeArray[j].As<PdfTypeDictionary>(true);
								if (annotDict.ContainsKey("Type") && annotDict["Type"].Is<PdfTypeName>() && annotDict["Type"].As<PdfTypeName>(true).Value == "Annot" && annotDict.ContainsKey("Subtype") && annotDict["Subtype"].Is<PdfTypeName>() && annotDict["Subtype"].As<PdfTypeName>(true).Value == "FileAttachment" && annotDict.ContainsKey("FS") && annotDict["FS"].Is<PdfTypeDictionary>() && list.All((AttachmentFileUtils.PdfFileAttachmentAnnotationResult c) => c.AnnotationDictionary.Handle != annotDict.Handle))
								{
									PdfTypeDictionary pdfTypeDictionary2 = annotDict["FS"].As<PdfTypeDictionary>(true);
									list.Add(new AttachmentFileUtils.PdfFileAttachmentAnnotationResult(i, annotDict, pdfTypeDictionary, new PdfFileSpecification(document, pdfTypeDictionary2)));
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0002B320 File Offset: 0x00029520
		public static async Task AddPDFAttachmentAsync(global::System.Collections.Generic.IReadOnlyList<string> filePathList, IProgress<double> progress)
		{
			AttachmentFileUtils.<>c__DisplayClass23_0 CS$<>8__locals1 = new AttachmentFileUtils.<>c__DisplayClass23_0();
			CS$<>8__locals1.filePathList = filePathList;
			CS$<>8__locals1.progress = progress;
			if (CS$<>8__locals1.filePathList == null || CS$<>8__locals1.filePathList.Count == 0)
			{
				throw new ArgumentException("filePathList");
			}
			CS$<>8__locals1.vm = Ioc.Default.GetRequiredService<MainViewModel>();
			if (CS$<>8__locals1.vm != null)
			{
				CS$<>8__locals1.addedAttachmentList = new List<PdfAttachment>();
				IProgress<double> progress2 = CS$<>8__locals1.progress;
				if (progress2 != null)
				{
					progress2.Report(0.0);
				}
				await Task.Run<Task>(TaskExceptionHelper.ExceptionBoundary<Task>(delegate
				{
					AttachmentFileUtils.<>c__DisplayClass23_0.<<AddPDFAttachmentAsync>b__0>d <<AddPDFAttachmentAsync>b__0>d;
					<<AddPDFAttachmentAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<Task>.Create();
					<<AddPDFAttachmentAsync>b__0>d.<>4__this = CS$<>8__locals1;
					<<AddPDFAttachmentAsync>b__0>d.<>1__state = -1;
					<<AddPDFAttachmentAsync>b__0>d.<>t__builder.Start<AttachmentFileUtils.<>c__DisplayClass23_0.<<AddPDFAttachmentAsync>b__0>d>(ref <<AddPDFAttachmentAsync>b__0>d);
					return <<AddPDFAttachmentAsync>b__0>d.<>t__builder.Task;
				}));
				CS$<>8__locals1.vm.PageEditors.NotifyAttachmentChanged();
				await CS$<>8__locals1.vm.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
				{
					foreach (PdfAttachment pdfAttachment in CS$<>8__locals1.addedAttachmentList)
					{
						CS$<>8__locals1.vm.Document.Attachments.Remove(pdfAttachment);
					}
					CS$<>8__locals1.vm.PageEditors.NotifyAttachmentChanged();
				}, delegate(PdfDocument doc)
				{
					foreach (PdfAttachment pdfAttachment2 in CS$<>8__locals1.addedAttachmentList)
					{
						CS$<>8__locals1.vm.Document.Attachments.Add(pdfAttachment2);
					}
					CS$<>8__locals1.vm.PageEditors.NotifyAttachmentChanged();
				}, "");
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x0002B36C File Offset: 0x0002956C
		public static string GetLastWriteTime(string filePath)
		{
			string text = null;
			if (string.IsNullOrEmpty(filePath))
			{
				return text;
			}
			try
			{
				DateTime lastWriteTime = File.GetLastWriteTime(filePath);
				text = new DateTimeOffset(lastWriteTime, TimeZoneInfo.Local.GetUtcOffset(lastWriteTime)).ToModificationDateString();
			}
			catch
			{
			}
			return text;
		}

		// Token: 0x04000446 RID: 1094
		private static List<AttachmentFileUtils.AttachmentFileCache> fileCache = new List<AttachmentFileUtils.AttachmentFileCache>();

		// Token: 0x02000409 RID: 1033
		public class PdfFileAttachmentAnnotationResult
		{
			// Token: 0x06002C75 RID: 11381 RVA: 0x000D812A File Offset: 0x000D632A
			public PdfFileAttachmentAnnotationResult(int pageIndex, PdfTypeDictionary annotationDictionary, PdfTypeDictionary pageDictionary, PdfFileSpecification fileSpecification)
			{
				this.PageIndex = pageIndex;
				this.AnnotationDictionary = annotationDictionary;
				this.PageDictionary = pageDictionary;
				this.FileSpecification = fileSpecification;
			}

			// Token: 0x17000C84 RID: 3204
			// (get) Token: 0x06002C76 RID: 11382 RVA: 0x000D814F File Offset: 0x000D634F
			public int PageIndex { get; }

			// Token: 0x17000C85 RID: 3205
			// (get) Token: 0x06002C77 RID: 11383 RVA: 0x000D8157 File Offset: 0x000D6357
			public PdfTypeDictionary AnnotationDictionary { get; }

			// Token: 0x17000C86 RID: 3206
			// (get) Token: 0x06002C78 RID: 11384 RVA: 0x000D815F File Offset: 0x000D635F
			public PdfTypeDictionary PageDictionary { get; }

			// Token: 0x17000C87 RID: 3207
			// (get) Token: 0x06002C79 RID: 11385 RVA: 0x000D8167 File Offset: 0x000D6367
			public PdfFileSpecification FileSpecification { get; }
		}

		// Token: 0x0200040A RID: 1034
		private class AttachmentFileCache
		{
			// Token: 0x06002C7A RID: 11386 RVA: 0x000D816F File Offset: 0x000D636F
			public AttachmentFileCache(PdfFile pdfFile, string path)
			{
				this.weakPdfFile = new WeakReference<PdfFile>(pdfFile);
				this.Path = path;
			}

			// Token: 0x17000C88 RID: 3208
			// (get) Token: 0x06002C7B RID: 11387 RVA: 0x000D818C File Offset: 0x000D638C
			public PdfFile PdfFile
			{
				get
				{
					PdfFile pdfFile;
					if (this.weakPdfFile != null && this.weakPdfFile.TryGetTarget(out pdfFile))
					{
						return pdfFile;
					}
					return null;
				}
			}

			// Token: 0x17000C89 RID: 3209
			// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000D81B3 File Offset: 0x000D63B3
			public string Path { get; }

			// Token: 0x04001744 RID: 5956
			private WeakReference<PdfFile> weakPdfFile;
		}
	}
}
