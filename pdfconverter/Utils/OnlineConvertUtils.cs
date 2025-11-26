using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;
using Newtonsoft.Json;
using pdfconverter.Models;

namespace pdfconverter.Utils
{
	// Token: 0x02000040 RID: 64
	public static class OnlineConvertUtils
	{
		// Token: 0x060004F3 RID: 1267 RVA: 0x00014488 File Offset: 0x00012688
		public static async Task<OnlineAuthResponsModel> IsServiceOnline(ConnectModel model)
		{
			return await OnlineConvertUtils.IsServiceOnline(JsonConvert.SerializeObject(model), OnlineConvertUtils.ServiceURLForAuth).ConfigureAwait(false);
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x000144CC File Offset: 0x000126CC
		public static async Task<OnlineAuthResponsModel> IsServiceOnline(ConnectModel model, string url)
		{
			return await OnlineConvertUtils.IsServiceOnline(JsonConvert.SerializeObject(model), url).ConfigureAwait(false);
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00014518 File Offset: 0x00012718
		public static async Task<OnlineAuthResponsModel> IsServiceOnline(string strPara, string url)
		{
			OnlineAuthResponsModel onlineAuthResponsModel;
			try
			{
				string text = EncryptUtils.EncryptStringToBase64_Aes(strPara);
				MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent("----------------------------" + DateTime.Now.Ticks.ToString("x"));
				multipartFormDataContent.Add(new StringContent(text), "parameter");
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
				httpRequestMessage.Content = multipartFormDataContent;
				onlineAuthResponsModel = JsonConvert.DeserializeObject<OnlineAuthResponsModel>(await (await new HttpClient().SendAsync(httpRequestMessage)).Content.ReadAsStringAsync());
			}
			catch
			{
				onlineAuthResponsModel = new OnlineAuthResponsModel
				{
					Success = false
				};
			}
			return onlineAuthResponsModel;
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x00014564 File Offset: 0x00012764
		public static async Task<bool> RequestConvertFile(string SrcFilePath, string DesFilePath, string _token, ConnectModel model, string fileNameWithoutExtension)
		{
			bool flag;
			try
			{
				string text = EncryptUtils.EncryptStringToBase64_Aes(JsonConvert.SerializeObject(new ConvertPara
				{
					converttype = model.convertType,
					token = _token
				}), EncryptUtils.key, EncryptUtils.iv);
				LongPathFile longPathFile = SrcFilePath;
				if (!longPathFile.IsExists)
				{
					flag = false;
				}
				else
				{
					bool flag2;
					using (FileStream stream = new FileStream(longPathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent("----------------------------" + DateTime.Now.Ticks.ToString("x"));
						if (stream.CanSeek)
						{
							stream.Seek(0L, SeekOrigin.Begin);
						}
						multipartFormDataContent.Add(new StreamContent(stream), "attachment", model.fileName);
						multipartFormDataContent.Add(new StringContent(text), "parameter");
						HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, OnlineConvertUtils.ServiceURLConvert);
						httpRequestMessage.Content = multipartFormDataContent;
						httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _token);
						flag2 = await OnlineConvertUtils.DownLoadFile(await (await new HttpClient().SendAsync(httpRequestMessage)).Content.ReadAsStreamAsync(), DesFilePath, model, fileNameWithoutExtension);
					}
					FileStream stream = null;
					flag = flag2;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000145C8 File Offset: 0x000127C8
		public static string GetRandomFileName()
		{
			return Guid.NewGuid().ToString("N");
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x000145E8 File Offset: 0x000127E8
		private static async Task<bool> DownLoadFile(Stream stream, string OutPath, ConnectModel model, string fileNameWithoutExtension)
		{
			bool flag;
			try
			{
				string extension = MapOnlineConverttype.GetOnlineExtensionStr(model.convertType);
				string DownLoadPath = OutPath;
				if (extension == ".zip")
				{
					DownLoadPath = Path.Combine(OnlineConvertUtils.GetDataPath(), OnlineConvertUtils.GetRandomFileName() + ".zip");
				}
				await OnlineConvertUtils.SaveFile(stream, DownLoadPath);
				if (extension == ".zip")
				{
					flag = OnlineConvertUtils.UnPackFileToDes(DownLoadPath, OutPath, model, fileNameWithoutExtension);
				}
				else
				{
					flag = true;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x00014644 File Offset: 0x00012844
		private static bool UnPackFileToDes(string sourceFile, string OutFolder, ConnectModel model, string fileNameWithoutExtension)
		{
			bool flag;
			try
			{
				string text = Path.Combine(Directory.GetParent(sourceFile).FullName, Path.GetFileNameWithoutExtension(sourceFile));
				ZipFile.ExtractToDirectory(sourceFile, text);
				FileInfo[] files = new DirectoryInfo(text).GetFiles();
				Array.Sort<FileInfo>(files, delegate(FileInfo x, FileInfo y)
				{
					if (OnlineConvertUtils.GetNum(x.Name) > OnlineConvertUtils.GetNum(y.Name))
					{
						return 1;
					}
					return -1;
				});
				foreach (FileInfo fileInfo in files)
				{
					string extension = Path.GetExtension(fileInfo.FullName);
					if (string.IsNullOrWhiteSpace(extension))
					{
						return false;
					}
					string newPath = OnlineConvertUtils.GetNewPath(fileNameWithoutExtension, extension, OutFolder, model.pageFrom, model.pageTo);
					fileInfo.CopyTo(newPath);
				}
				Directory.Delete(text, true);
				File.Delete(sourceFile);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00014718 File Offset: 0x00012918
		private static int GetNum(string name)
		{
			int num;
			try
			{
				string[] array = name.Split(new char[] { '_' });
				num = int.Parse(array[array.Length - 1].Split(new char[] { '.' })[0]);
			}
			catch
			{
				num = 0;
			}
			return num;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001476C File Offset: 0x0001296C
		private static string GetNewPath(string fileName, string extention, string OutFolder, int from, int to)
		{
			string text = string.Format("{0} {1}{2}", fileName, from, extention);
			string text2 = Path.Combine(OutFolder, text);
			while (File.Exists(text2))
			{
				from++;
				text = string.Format("{0} {1}{2}", fileName, from, extention);
				text2 = Path.Combine(OutFolder, text);
			}
			return text2;
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x000147C0 File Offset: 0x000129C0
		private static async Task SaveFile(Stream stream, string LocalPath)
		{
			using (FileStream fileStream = File.Create(LocalPath))
			{
				if (stream.CanSeek)
				{
					stream.Seek(0L, SeekOrigin.Begin);
				}
				byte[] buffer = ArrayPool<byte>.Shared.Rent(16384);
				try
				{
					int num;
					while ((num = await stream.ReadAsync(buffer, 0, 4096, default(CancellationToken))) != 0)
					{
						await fileStream.WriteAsync(buffer, 0, num, default(CancellationToken));
					}
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(buffer, false);
				}
				buffer = null;
			}
			FileStream fileStream = null;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001480C File Offset: 0x00012A0C
		private static string GetDataPath()
		{
			string text = Path.Combine(AppDataHelper.TemporaryFolder, "Online");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		// Token: 0x04000272 RID: 626
		private static readonly string ServiceURLForAuth = "https://apiw.pdfgear.com/v1/auth";

		// Token: 0x04000273 RID: 627
		private static readonly string ServiceURLConvert = "https://apiw.pdfgear.com/v1/convert";
	}
}
