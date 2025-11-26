using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x020000A7 RID: 167
	public static class TempFileUtils
	{
		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x00034FB0 File Offset: 0x000331B0
		private static string RootTempFolder
		{
			get
			{
				if (string.IsNullOrEmpty(TempFileUtils._rootTempFolder))
				{
					TempFileUtils._rootTempFolder = AppDataHelper.TemporaryFolder;
				}
				return TempFileUtils._rootTempFolder;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000A59 RID: 2649 RVA: 0x00034FD0 File Offset: 0x000331D0
		private static string DocumentTempFolder
		{
			get
			{
				string text = Path.Combine(TempFileUtils.RootTempFolder, "Documents");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x00035000 File Offset: 0x00033200
		private static string MergeDocumentTempFolder
		{
			get
			{
				string text = Path.Combine(TempFileUtils.DocumentTempFolder, "Merge");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x00035030 File Offset: 0x00033230
		public static async Task<Stream> CreateStreamAsync(string fileFullName, FileStream fileStream, bool deleteOnClose, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Stream stream;
			if (fileStream.Length > 20971520L)
			{
				MemoryStream memoryStream = new MemoryStream();
				fileStream.Seek(0L, SeekOrigin.Begin);
				await fileStream.CopyToAsync(memoryStream, (int)fileStream.Length, cancellationToken).ConfigureAwait(false);
				stream = memoryStream;
			}
			else
			{
				FileInfo fileInfo = new FileInfo(fileFullName);
				string tmpFileFullName = "";
				do
				{
					string text = TempFileUtils.GenerateRandomFilename(fileInfo);
					tmpFileFullName = Path.Combine(TempFileUtils.DocumentTempFolder, text);
				}
				while (File.Exists(tmpFileFullName));
				FileStream tmpFileStream = null;
				int num = 0;
				try
				{
					FileOptions fileOptions = FileOptions.None;
					if (deleteOnClose)
					{
						fileOptions |= FileOptions.DeleteOnClose;
					}
					tmpFileStream = new FileStream(tmpFileFullName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, deleteOnClose);
				}
				catch
				{
					num = 1;
				}
				if (num == 1)
				{
					string text = TempFileUtils.GenerateShortRandomFilename(fileInfo);
					tmpFileFullName = Path.Combine(TempFileUtils.DocumentTempFolder, text);
					int num2 = 0;
					try
					{
						tmpFileStream = new FileStream(tmpFileFullName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
					}
					catch
					{
						num2 = 1;
					}
					if (num2 == 1)
					{
						MemoryStream memoryStream = new MemoryStream();
						fileStream.Seek(0L, SeekOrigin.Begin);
						await fileStream.CopyToAsync(memoryStream, (int)fileStream.Length, cancellationToken).ConfigureAwait(false);
						return memoryStream;
					}
				}
				fileStream.Seek(0L, SeekOrigin.Begin);
				try
				{
					await fileStream.CopyToAsync(tmpFileStream, (int)fileStream.Length, cancellationToken).ConfigureAwait(false);
				}
				catch
				{
					try
					{
						tmpFileStream.Dispose();
						tmpFileStream = null;
						File.Delete(tmpFileFullName);
					}
					catch
					{
					}
					throw;
				}
				TempFileUtils.StreamWrapper streamWrapper = new TempFileUtils.StreamWrapper(tmpFileStream);
				streamWrapper.Disposed += delegate(object s, EventArgs a)
				{
					try
					{
						File.Delete(tmpFileFullName);
					}
					catch
					{
					}
				};
				stream = streamWrapper;
			}
			return stream;
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0003508C File Offset: 0x0003328C
		public static async Task<string> SaveMergeSourceFile(PdfDocument document, int[] range)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			if (range == null || range.Length == 0)
			{
				throw new ArgumentNullException("range");
			}
			return await Task.Run<string>(TaskExceptionHelper.ExceptionBoundary<string>(delegate
			{
				string text3;
				try
				{
					string text = "";
					do
					{
						string text2 = Guid.NewGuid().ToString("N").Substring(0, 8) ?? "";
						text = Path.Combine(TempFileUtils.MergeDocumentTempFolder, text2);
					}
					while (File.Exists(text));
					if (range.Length != 0)
					{
						int num = range.Min();
						int num2 = range.Max();
						PageDisposeHelper.TryFixResource(document, num, num2);
					}
					using (FileStream fileStream = File.Create(text))
					{
						using (PdfDocument pdfDocument = PdfDocument.CreateNew(null))
						{
							pdfDocument.Pages.ImportPages(document, range.ConvertToRange(), 0);
							pdfDocument.Save(fileStream, SaveFlags.NoIncremental, 0);
						}
						text3 = text;
					}
				}
				catch
				{
					text3 = string.Empty;
				}
				return text3;
			}));
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000350D8 File Offset: 0x000332D8
		private static string GenerateRandomFilename(FileInfo fileInfo)
		{
			if (fileInfo == null)
			{
				throw new ArgumentException("fileInfo");
			}
			return Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + fileInfo.Name;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0003511C File Offset: 0x0003331C
		private static string GenerateShortRandomFilename(FileInfo fileInfo)
		{
			if (fileInfo == null)
			{
				throw new ArgumentException("fileInfo");
			}
			return Guid.NewGuid().ToString("N").Substring(0, 8) + fileInfo.Extension;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0003515C File Offset: 0x0003335C
		public static void ClearDocuments()
		{
			try
			{
				string[] files = Directory.GetFiles(TempFileUtils.DocumentTempFolder);
				if (files != null)
				{
					string[] array = files;
					for (int i = 0; i < array.Length; i++)
					{
						TempFileUtils.<ClearDocuments>g__TryDeleteFileCore|11_0(array[i]);
					}
				}
				if (Directory.GetFiles(TempFileUtils.MergeDocumentTempFolder) != null)
				{
					string[] array = files;
					for (int i = 0; i < array.Length; i++)
					{
						TempFileUtils.<ClearDocuments>g__TryDeleteFileCore|11_0(array[i]);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000351C8 File Offset: 0x000333C8
		public static async Task<Stream> CloneToTempStream(PdfDocument sourceDocument)
		{
			if (sourceDocument == null)
			{
				throw new ArgumentNullException("sourceDocument");
			}
			long? num = TempFileUtils.TryGetDocumentStreamLength(sourceDocument);
			Stream stream;
			if (num != null && num.Value < 20971520L)
			{
				stream = await TempFileUtils.CloneToTempStreamCore(sourceDocument, () => Task.FromResult<Stream>(new MemoryStream()));
			}
			else
			{
				stream = await TempFileUtils.CloneToTempStreamCore(sourceDocument, delegate
				{
					string tmpFileFullName = "";
					do
					{
						string text = Guid.NewGuid().ToString("N").Substring(0, 8) + "_TEMPDOC";
						tmpFileFullName = Path.Combine(TempFileUtils.DocumentTempFolder, text);
					}
					while (File.Exists(tmpFileFullName));
					TempFileUtils.StreamWrapper streamWrapper = new TempFileUtils.StreamWrapper(new FileStream(tmpFileFullName, FileMode.Create, FileAccess.ReadWrite));
					streamWrapper.Disposed += delegate(object s, EventArgs a)
					{
						try
						{
							File.Delete(tmpFileFullName);
						}
						catch
						{
						}
					};
					return Task.FromResult<Stream>(streamWrapper);
				});
			}
			return stream;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x0003520C File Offset: 0x0003340C
		private static async Task<Stream> CloneToTempStreamCore(PdfDocument sourceDocument, Func<Task<Stream>> tempStream)
		{
			if (sourceDocument == null)
			{
				throw new ArgumentNullException("sourceDocument");
			}
			if (tempStream == null)
			{
				throw new ArgumentNullException("tempStream");
			}
			Stream stream2 = await tempStream();
			Stream stream4;
			using (Stream stream = stream2)
			{
				if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
				{
					throw new ArgumentException(null, "tempStream");
				}
				sourceDocument.Save(stream, SaveFlags.Incremental | SaveFlags.NoIncremental | SaveFlags.RemoveUnusedObjects, 0);
				stream.Seek(0L, SeekOrigin.Begin);
				using (PdfDocument tmpDocument = PdfDocument.Load(stream, new PdfForms(), null, true))
				{
					PageDisposeHelper.TryFixResource(tmpDocument, 0, tmpDocument.Pages.Count - 1);
					for (int i = 0; i < tmpDocument.Pages.Count; i++)
					{
						PageDisposeHelper.TryFixPageAnnotations(tmpDocument, i);
					}
					PdfDocumentUtils.RemoveUnusedObjects(tmpDocument);
					Stream stream3 = await tempStream();
					if (!stream3.CanRead || !stream3.CanWrite || !stream3.CanSeek)
					{
						throw new ArgumentException(null, "tempStream");
					}
					tmpDocument.Save(stream3, SaveFlags.Incremental | SaveFlags.NoIncremental | SaveFlags.RemoveUnusedObjects, 0);
					stream3.Seek(0L, SeekOrigin.Begin);
					stream4 = stream3;
				}
			}
			return stream4;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00035258 File Offset: 0x00033458
		private static long? TryGetDocumentStreamLength(PdfDocument document)
		{
			if (document == null)
			{
				return null;
			}
			if (TempFileUtils.documentPdfStreamGetter == null)
			{
				object obj = TempFileUtils.documentPdfStreamLocker;
				lock (obj)
				{
					if (TempFileUtils.documentPdfStreamGetter == null)
					{
						try
						{
							TempFileUtils.documentPdfStreamGetter = TypeHelper.CreateFieldOrPropertyGetter<PdfDocument, Stream>("_streamPdf", BindingFlags.Instance | BindingFlags.NonPublic);
						}
						catch
						{
						}
						if (TempFileUtils.documentPdfStreamGetter == null)
						{
							TempFileUtils.documentPdfStreamGetter = (PdfDocument c) => null;
						}
					}
				}
			}
			try
			{
				return new long?(TempFileUtils.documentPdfStreamGetter(document).Length);
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00035338 File Offset: 0x00033538
		[CompilerGenerated]
		internal static void <ClearDocuments>g__TryDeleteFileCore|11_0(string filename)
		{
			try
			{
				File.Delete(filename);
			}
			catch
			{
			}
		}

		// Token: 0x040004A1 RID: 1185
		private static string _rootTempFolder;

		// Token: 0x040004A2 RID: 1186
		private static Func<PdfDocument, Stream> documentPdfStreamGetter;

		// Token: 0x040004A3 RID: 1187
		private static object documentPdfStreamLocker = new object();

		// Token: 0x020004B3 RID: 1203
		private class StreamWrapper : Stream
		{
			// Token: 0x06002E59 RID: 11865 RVA: 0x000E36D2 File Offset: 0x000E18D2
			public StreamWrapper(FileStream stream)
			{
				this.stream = stream;
			}

			// Token: 0x17000CB7 RID: 3255
			// (get) Token: 0x06002E5A RID: 11866 RVA: 0x000E36E1 File Offset: 0x000E18E1
			public override bool CanRead
			{
				get
				{
					return this.stream.CanRead;
				}
			}

			// Token: 0x17000CB8 RID: 3256
			// (get) Token: 0x06002E5B RID: 11867 RVA: 0x000E36EE File Offset: 0x000E18EE
			public override bool CanSeek
			{
				get
				{
					return this.stream.CanSeek;
				}
			}

			// Token: 0x17000CB9 RID: 3257
			// (get) Token: 0x06002E5C RID: 11868 RVA: 0x000E36FB File Offset: 0x000E18FB
			public override bool CanWrite
			{
				get
				{
					return this.stream.CanWrite;
				}
			}

			// Token: 0x17000CBA RID: 3258
			// (get) Token: 0x06002E5D RID: 11869 RVA: 0x000E3708 File Offset: 0x000E1908
			public override long Length
			{
				get
				{
					return this.stream.Length;
				}
			}

			// Token: 0x17000CBB RID: 3259
			// (get) Token: 0x06002E5E RID: 11870 RVA: 0x000E3715 File Offset: 0x000E1915
			// (set) Token: 0x06002E5F RID: 11871 RVA: 0x000E3722 File Offset: 0x000E1922
			public override long Position
			{
				get
				{
					return this.stream.Position;
				}
				set
				{
					this.stream.Position = value;
				}
			}

			// Token: 0x06002E60 RID: 11872 RVA: 0x000E3730 File Offset: 0x000E1930
			public override void Flush()
			{
				this.stream.Flush();
			}

			// Token: 0x06002E61 RID: 11873 RVA: 0x000E373D File Offset: 0x000E193D
			public override int Read(byte[] buffer, int offset, int count)
			{
				return this.stream.Read(buffer, offset, count);
			}

			// Token: 0x06002E62 RID: 11874 RVA: 0x000E374D File Offset: 0x000E194D
			public override long Seek(long offset, SeekOrigin origin)
			{
				return this.stream.Seek(offset, origin);
			}

			// Token: 0x06002E63 RID: 11875 RVA: 0x000E375C File Offset: 0x000E195C
			public override void SetLength(long value)
			{
				this.stream.SetLength(value);
			}

			// Token: 0x06002E64 RID: 11876 RVA: 0x000E376A File Offset: 0x000E196A
			public override void Write(byte[] buffer, int offset, int count)
			{
				this.stream.Write(buffer, offset, count);
			}

			// Token: 0x06002E65 RID: 11877 RVA: 0x000E377A File Offset: 0x000E197A
			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return this.stream.BeginRead(buffer, offset, count, callback, state);
			}

			// Token: 0x06002E66 RID: 11878 RVA: 0x000E378E File Offset: 0x000E198E
			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return this.stream.BeginWrite(buffer, offset, count, callback, state);
			}

			// Token: 0x17000CBC RID: 3260
			// (get) Token: 0x06002E67 RID: 11879 RVA: 0x000E37A2 File Offset: 0x000E19A2
			public override bool CanTimeout
			{
				get
				{
					return this.stream.CanTimeout;
				}
			}

			// Token: 0x06002E68 RID: 11880 RVA: 0x000E37AF File Offset: 0x000E19AF
			public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
			{
				return this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
			}

			// Token: 0x06002E69 RID: 11881 RVA: 0x000E37BF File Offset: 0x000E19BF
			public override ObjRef CreateObjRef(Type requestedType)
			{
				return this.stream.CreateObjRef(requestedType);
			}

			// Token: 0x06002E6A RID: 11882 RVA: 0x000E37CD File Offset: 0x000E19CD
			public override int EndRead(IAsyncResult asyncResult)
			{
				return this.stream.EndRead(asyncResult);
			}

			// Token: 0x06002E6B RID: 11883 RVA: 0x000E37DB File Offset: 0x000E19DB
			public override void EndWrite(IAsyncResult asyncResult)
			{
				this.stream.EndWrite(asyncResult);
			}

			// Token: 0x06002E6C RID: 11884 RVA: 0x000E37E9 File Offset: 0x000E19E9
			public override Task FlushAsync(CancellationToken cancellationToken)
			{
				return this.stream.FlushAsync(cancellationToken);
			}

			// Token: 0x06002E6D RID: 11885 RVA: 0x000E37F7 File Offset: 0x000E19F7
			public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
			}

			// Token: 0x06002E6E RID: 11886 RVA: 0x000E3809 File Offset: 0x000E1A09
			public override int ReadByte()
			{
				return this.stream.ReadByte();
			}

			// Token: 0x06002E6F RID: 11887 RVA: 0x000E3816 File Offset: 0x000E1A16
			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
			}

			// Token: 0x06002E70 RID: 11888 RVA: 0x000E3828 File Offset: 0x000E1A28
			public override void WriteByte(byte value)
			{
				this.stream.WriteByte(value);
			}

			// Token: 0x17000CBD RID: 3261
			// (get) Token: 0x06002E71 RID: 11889 RVA: 0x000E3836 File Offset: 0x000E1A36
			// (set) Token: 0x06002E72 RID: 11890 RVA: 0x000E3843 File Offset: 0x000E1A43
			public override int WriteTimeout
			{
				get
				{
					return this.stream.WriteTimeout;
				}
				set
				{
					this.stream.WriteTimeout = value;
				}
			}

			// Token: 0x17000CBE RID: 3262
			// (get) Token: 0x06002E73 RID: 11891 RVA: 0x000E3851 File Offset: 0x000E1A51
			// (set) Token: 0x06002E74 RID: 11892 RVA: 0x000E385E File Offset: 0x000E1A5E
			public override int ReadTimeout
			{
				get
				{
					return this.stream.ReadTimeout;
				}
				set
				{
					this.stream.ReadTimeout = value;
				}
			}

			// Token: 0x06002E75 RID: 11893 RVA: 0x000E386C File Offset: 0x000E1A6C
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				this.stream.Dispose();
				EventHandler disposed = this.Disposed;
				if (disposed != null)
				{
					disposed(this, EventArgs.Empty);
				}
			}

			// Token: 0x06002E76 RID: 11894 RVA: 0x000E389C File Offset: 0x000E1A9C
			~StreamWrapper()
			{
				GC.SuppressFinalize(this);
				this.Dispose(false);
			}

			// Token: 0x1400004F RID: 79
			// (add) Token: 0x06002E77 RID: 11895 RVA: 0x000E38D0 File Offset: 0x000E1AD0
			// (remove) Token: 0x06002E78 RID: 11896 RVA: 0x000E3908 File Offset: 0x000E1B08
			public event EventHandler Disposed;

			// Token: 0x04001A83 RID: 6787
			private readonly FileStream stream;
		}
	}
}
