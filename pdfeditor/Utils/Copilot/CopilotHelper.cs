using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Config;
using CommonLib.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using LruCacheNet;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Copilot;
using pdfeditor.Utils.Enums;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;

namespace pdfeditor.Utils.Copilot
{
	// Token: 0x020000E0 RID: 224
	public class CopilotHelper : IDisposable
	{
		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000C0D RID: 3085 RVA: 0x0003F84C File Offset: 0x0003DA4C
		public PdfDocument Document
		{
			get
			{
				PdfDocument pdfDocument;
				if (this._document != null && this._document.TryGetTarget(out pdfDocument))
				{
					return pdfDocument;
				}
				return null;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000C0E RID: 3086 RVA: 0x0003F873 File Offset: 0x0003DA73
		// (set) Token: 0x06000C0F RID: 3087 RVA: 0x0003F87B File Offset: 0x0003DA7B
		internal bool Initialized { get; private set; }

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x0003F884 File Offset: 0x0003DA84
		// (set) Token: 0x06000C11 RID: 3089 RVA: 0x0003F88C File Offset: 0x0003DA8C
		internal bool InitializeSucceed { get; private set; }

		// Token: 0x06000C12 RID: 3090 RVA: 0x0003F898 File Offset: 0x0003DA98
		public CopilotHelper(PdfDocument document, string filePath)
		{
			this.cts = new CancellationTokenSource();
			this._document = new WeakReference<PdfDocument>(document);
			this.filePath = filePath;
			this.pageCountForAnalyze = Math.Min(120, document.Pages.Count);
			this.analyzeTask = new CopilotHelper.AnalyzeTask(this);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0003F934 File Offset: 0x0003DB34
		public async Task InitializeAsync(IProgress<double> progressReporter)
		{
			if (this.Document != null)
			{
				await CopilotHelper.AppActionHelper.InitializeAllActions();
				await this.analyzeTask.Start(progressReporter);
				this.Initialized = true;
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0003F980 File Offset: 0x0003DB80
		public async Task<CopilotHelper.CopilotResult> GetSummaryAsync(Func<string, CancellationToken, Task> summaryAction, CancellationToken cancellationToken)
		{
			CopilotHelper.<>c__DisplayClass32_0 CS$<>8__locals1 = new CopilotHelper.<>c__DisplayClass32_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.summaryAction = summaryAction;
			CopilotHelper.CopilotResult copilotResult;
			if (!this.Initialized || this.chatting)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else if (!this.CanChat())
			{
				copilotResult = CopilotHelper.CopilotResult.ChatCountLimitFailed;
			}
			else
			{
				PdfDocument document = this.Document;
				if (document == null)
				{
					copilotResult = CopilotHelper.CopilotResult.ContentEmptyFailed;
				}
				else
				{
					CancellationTokenSource cts2 = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken);
					CopilotHelper.PdfModel pdfModel = null;
					try
					{
						CopilotHelper.<>c__DisplayClass32_1 CS$<>8__locals2 = new CopilotHelper.<>c__DisplayClass32_1();
						CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
						this.chatting = true;
						int num = 5;
						if (num > this.pageCountForAnalyze)
						{
							num = this.pageCountForAnalyze;
						}
						if (num > document.Pages.Count)
						{
							num = document.Pages.Count;
						}
						global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>[] array = await Task.WhenAll<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>(Enumerable.Range(0, num).Select(delegate(int c)
						{
							CopilotHelper.<>c__DisplayClass32_0.<<GetSummaryAsync>b__0>d <<GetSummaryAsync>b__0>d;
							<<GetSummaryAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>.Create();
							<<GetSummaryAsync>b__0>d.<>4__this = CS$<>8__locals2.CS$<>8__locals1;
							<<GetSummaryAsync>b__0>d.c = c;
							<<GetSummaryAsync>b__0>d.<>1__state = -1;
							<<GetSummaryAsync>b__0>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass32_0.<<GetSummaryAsync>b__0>d>(ref <<GetSummaryAsync>b__0>d);
							return <<GetSummaryAsync>b__0>d.<>t__builder.Task;
						}).ToArray<Task<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>>());
						pdfModel = await this.BuildPdfModel(array, cts2.Token);
						CopilotHelper.PdfModel pdfModel2 = pdfModel;
						if (((pdfModel2 != null) ? pdfModel2.Pages : null) == null || pdfModel.Pages.Count == 0)
						{
							GAManager.SendEvent("ChatPdf", "Summary", "Summary_Pdf_Content_Empty", 1L);
							return CopilotHelper.CopilotResult.ContentEmptyFailed;
						}
						CS$<>8__locals2.sb = new StringBuilder();
						GAManager.SendEvent("ChatPdf", "Summary", "Start", 1L);
						CopilotHelper.StreamRequestResult streamRequestResult = await CopilotHelper.InternalCopilotHelper.GetSummaryAsync(pdfModel, true, delegate(string result, CancellationToken ct)
						{
							CopilotHelper.<>c__DisplayClass32_1.<<GetSummaryAsync>b__1>d <<GetSummaryAsync>b__1>d;
							<<GetSummaryAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<GetSummaryAsync>b__1>d.<>4__this = CS$<>8__locals2;
							<<GetSummaryAsync>b__1>d.result = result;
							<<GetSummaryAsync>b__1>d.ct = ct;
							<<GetSummaryAsync>b__1>d.<>1__state = -1;
							<<GetSummaryAsync>b__1>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass32_1.<<GetSummaryAsync>b__1>d>(ref <<GetSummaryAsync>b__1>d);
							return <<GetSummaryAsync>b__1>d.<>t__builder.Task;
						}, cts2.Token);
						if (streamRequestResult.Success)
						{
							string text = CS$<>8__locals2.sb.ToString();
							GAManager.SendEvent("ChatPdf", "Summary", "Done", 1L);
							CopilotHelper.Cache.SetSummaryToCache(this.filePath, CS$<>8__locals2.sb.ToString());
							List<CopilotHelper.PdfPageModel> pages = pdfModel.Pages;
							int[] array2;
							if (pages == null)
							{
								array2 = null;
							}
							else
							{
								array2 = (from c in pages
									select c.PageIndex into c
									orderby c
									select c).ToArray<int>();
							}
							return new CopilotHelper.CopilotResult(array2, text, null, null, false);
						}
						return new CopilotHelper.CopilotResult(null, null, streamRequestResult.Error);
					}
					catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
					{
						return CopilotHelper.CopilotResult.UserCanceledFailed;
					}
					catch (Exception ex2)
					{
						try
						{
							Log.WriteLog("PdfModel: " + JsonConvert.SerializeObject(pdfModel, Formatting.Indented));
						}
						catch
						{
						}
						Log.WriteLog(ex2.ToString());
						GAManager.SendEvent("ChatPdf", "Summary", "Summary_Failed_" + ex2.GetType().Name, 1L);
					}
					finally
					{
						this.chatting = false;
					}
					copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
			}
			return copilotResult;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0003F9D3 File Offset: 0x0003DBD3
		public void LikedAsyne(CopilotHelper.ChatMessage message)
		{
			CopilotHelper.Cache.AppendChatMessagesLikedToCache(this.filePath, message);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0003F9E4 File Offset: 0x0003DBE4
		public async Task<CopilotHelper.CopilotResult> GetAppActionAsync(string message, CancellationToken cancellationToken)
		{
			CopilotHelper.CopilotResult copilotResult;
			if (!this.Initialized || this.chatting)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed;
			}
			else if (string.IsNullOrEmpty(message))
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed;
			}
			else if (!this.CanChat())
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed;
			}
			else
			{
				CancellationTokenSource cts2 = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken);
				try
				{
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel = await this.AnalyzeMessageAsync(message, cts2.Token);
					bool flag;
					if (analyzeResponseModel == null)
					{
						flag = null != null;
					}
					else
					{
						List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> items = analyzeResponseModel.Items;
						if (items == null)
						{
							flag = null != null;
						}
						else
						{
							CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel analyzeResponseItemModel = items.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
							if (analyzeResponseItemModel == null)
							{
								flag = null != null;
							}
							else
							{
								List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> data = analyzeResponseItemModel.Data;
								flag = ((data != null) ? data.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel>() : null) != null;
							}
						}
					}
					if (!flag)
					{
						return CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed;
					}
					float[] array;
					if (analyzeResponseModel == null)
					{
						array = null;
					}
					else
					{
						List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> items2 = analyzeResponseModel.Items;
						if (items2 == null)
						{
							array = null;
						}
						else
						{
							CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel analyzeResponseItemModel2 = items2.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
							if (analyzeResponseItemModel2 == null)
							{
								array = null;
							}
							else
							{
								List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> data2 = analyzeResponseItemModel2.Data;
								if (data2 == null)
								{
									array = null;
								}
								else
								{
									CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel analyzeResponseDataModel = data2.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel>();
									array = ((analyzeResponseDataModel != null) ? analyzeResponseDataModel.Values : null);
								}
							}
						}
					}
					global::System.ValueTuple<CopilotHelper.AppActionModel, bool> valueTuple = await CopilotHelper.AppActionHelper.GetAction(message, array, cts2.Token);
					CopilotHelper.AppActionModel item = valueTuple.Item1;
					bool item2 = valueTuple.Item2;
					if (item != null)
					{
						return new CopilotHelper.CopilotResult(null, item.Confirm, item, null, item2);
					}
					return item2 ? CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed : CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
				catch (OperationCanceledException ex) when (ex.CancellationToken != this.cts.Token)
				{
					return CopilotHelper.CopilotResult.UserCanceledFailed;
				}
				catch (Exception ex2)
				{
					Log.WriteLog(ex2.ToString());
					GAManager.SendEvent("ChatPdf", "GetAppAction_Failed_" + ex2.GetType().Name, "Count", 1L);
				}
				finally
				{
					this.chatting = false;
				}
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownMaybeAppActionFailed;
			}
			return copilotResult;
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0003FA38 File Offset: 0x0003DC38
		public async Task<CopilotHelper.CopilotResult> ChatAsync(string message, Func<string, CancellationToken, Task> chatAction, CancellationToken cancellationToken)
		{
			CopilotHelper.<>c__DisplayClass35_0 CS$<>8__locals1 = new CopilotHelper.<>c__DisplayClass35_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.chatAction = chatAction;
			CopilotHelper.CopilotResult copilotResult;
			if (!this.Initialized || this.chatting)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else if (string.IsNullOrEmpty(message))
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else if (!this.CanChat())
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else
			{
				CopilotHelper.PdfModel pdfModel = null;
				CancellationTokenSource cts2 = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken);
				try
				{
					CopilotHelper.<>c__DisplayClass35_1 CS$<>8__locals2 = new CopilotHelper.<>c__DisplayClass35_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					PdfDocument doc = this.Document;
					if (doc == null)
					{
						return CopilotHelper.CopilotResult.EmptyUnknownFailed;
					}
					this.chatting = true;
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel = await this.AnalyzeMessageAsync(message, cts2.Token);
					CS$<>8__locals2.messageEmbedding = analyzeResponseModel;
					CopilotHelper.AnalyzeResponseModel messageEmbedding = CS$<>8__locals2.messageEmbedding;
					bool flag;
					if (messageEmbedding == null)
					{
						flag = null != null;
					}
					else
					{
						List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> items = messageEmbedding.Items;
						if (items == null)
						{
							flag = null != null;
						}
						else
						{
							CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel analyzeResponseItemModel = items.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
							if (analyzeResponseItemModel == null)
							{
								flag = null != null;
							}
							else
							{
								List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> data = analyzeResponseItemModel.Data;
								flag = ((data != null) ? data.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel>() : null) != null;
							}
						}
					}
					if (!flag)
					{
						return CopilotHelper.CopilotResult.EmptyUnknownFailed;
					}
					int num = Math.Min(this.pageCountForAnalyze, doc.Pages.Count);
					global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>[] array = (await Task.WhenAll<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>(Enumerable.Range(0, num).Select(delegate(int c)
					{
						CopilotHelper.<>c__DisplayClass35_0.<<ChatAsync>b__3>d <<ChatAsync>b__3>d;
						<<ChatAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>.Create();
						<<ChatAsync>b__3>d.<>4__this = CS$<>8__locals2.CS$<>8__locals1;
						<<ChatAsync>b__3>d.c = c;
						<<ChatAsync>b__3>d.<>1__state = -1;
						<<ChatAsync>b__3>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass35_0.<<ChatAsync>b__3>d>(ref <<ChatAsync>b__3>d);
						return <<ChatAsync>b__3>d.<>t__builder.Task;
					}).ToArray<Task<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>>())).Where(delegate([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel> c)
					{
						CopilotHelper.AnalyzeResponseModel item = c.Item2;
						object obj;
						if (item == null)
						{
							obj = null;
						}
						else
						{
							List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> items2 = item.Items;
							if (items2 == null)
							{
								obj = null;
							}
							else
							{
								CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel analyzeResponseItemModel2 = items2.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
								if (analyzeResponseItemModel2 == null)
								{
									obj = null;
								}
								else
								{
									List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> data2 = analyzeResponseItemModel2.Data;
									if (data2 == null)
									{
										obj = null;
									}
									else
									{
										CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel analyzeResponseDataModel = data2.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel>();
										obj = ((analyzeResponseDataModel != null) ? analyzeResponseDataModel.Values : null);
									}
								}
							}
						}
						return obj != null;
					}).SelectMany(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel> c) => c.Item2.Items.Select((CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel x) => new global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>(c.Item1, x))).OrderBy(delegate([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> c)
					{
						float num3 = float.MaxValue;
						foreach (CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel analyzeResponseDataModel2 in c.Item2.Data)
						{
							float[] values = analyzeResponseDataModel2.Values;
							float[] values2 = CS$<>8__locals2.messageEmbedding.Items[0].Data[0].Values;
							if (values != null && values2 != null)
							{
								num3 = Math.Min(CopilotHelper.SimpleCosineSimilarityFloatVersion.ComputeDistance(values, values2), num3);
							}
						}
						return num3;
					})
						.ToArray<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>>();
					pdfModel = await this.BuildPdfModel(array, cts2.Token);
					CopilotHelper.PdfModel pdfModel2 = pdfModel;
					if (((pdfModel2 != null) ? pdfModel2.Pages : null) == null || pdfModel.Pages.Count == 0)
					{
						Log.WriteLog("ChatAsync: PdfModel is Empty");
						GAManager.SendEvent("ChatPdf", "Chat_Pdf_Content_Empty", "Count", 1L);
					}
					if (pdfModel.PageCount == 0)
					{
						return CopilotHelper.CopilotResult.EmptyUnknownFailed;
					}
					List<CopilotHelper.ChatMessage> list = this.cachedChatMessage.ToList<CopilotHelper.ChatMessage>();
					CopilotHelper.ChatMessage userMessage = new CopilotHelper.ChatMessage
					{
						Content = message,
						Role = "user"
					};
					list.Add(userMessage);
					CS$<>8__locals2.sb = new StringBuilder();
					int num2 = CopilotHelper.Cache.IncreaseChatCount(this.filePath);
					this.analyzeTask.Continue();
					GAManager.SendEvent("ChatPdf", "ChatCount", string.Format("{0}", num2), 1L);
					CopilotHelper.StreamRequestResult streamRequestResult = await CopilotHelper.InternalCopilotHelper.ChatAsync(list, pdfModel, true, delegate(string result, CancellationToken ct)
					{
						CopilotHelper.<>c__DisplayClass35_1.<<ChatAsync>b__5>d <<ChatAsync>b__5>d;
						<<ChatAsync>b__5>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<ChatAsync>b__5>d.<>4__this = CS$<>8__locals2;
						<<ChatAsync>b__5>d.result = result;
						<<ChatAsync>b__5>d.ct = ct;
						<<ChatAsync>b__5>d.<>1__state = -1;
						<<ChatAsync>b__5>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass35_1.<<ChatAsync>b__5>d>(ref <<ChatAsync>b__5>d);
						return <<ChatAsync>b__5>d.<>t__builder.Task;
					}, cts2.Token);
					if (streamRequestResult.Success)
					{
						CopilotHelper.ChatMessage chatMessage = new CopilotHelper.ChatMessage();
						chatMessage.Content = CS$<>8__locals2.sb.ToString();
						chatMessage.Role = "assistant";
						chatMessage.Pages = pdfModel.Pages.Select((CopilotHelper.PdfPageModel c) => c.PageIndex).ToArray<int>();
						list.Add(chatMessage);
						while (list.Count > 6)
						{
							list.RemoveAt(0);
						}
						this.cachedChatMessage = list;
						if (!string.IsNullOrEmpty(chatMessage.Content))
						{
							CopilotHelper.Cache.AppendChatMessagesToCache(this.filePath, new CopilotHelper.ChatMessage[] { userMessage, chatMessage });
						}
						return new CopilotHelper.CopilotResult((chatMessage != null) ? chatMessage.Pages : null, chatMessage.Content, null);
					}
					return new CopilotHelper.CopilotResult(null, null, streamRequestResult.Error);
				}
				catch (OperationCanceledException ex) when (ex.CancellationToken != this.cts.Token)
				{
					return CopilotHelper.CopilotResult.UserCanceledFailed;
				}
				catch (Exception ex2)
				{
					try
					{
						Log.WriteLog("PdfModel: " + JsonConvert.SerializeObject(pdfModel, Formatting.Indented));
						Log.WriteLog("Chat records: " + JsonConvert.SerializeObject(this.cachedChatMessage.ToList<CopilotHelper.ChatMessage>()));
					}
					catch
					{
					}
					Log.WriteLog(ex2.ToString());
					GAManager.SendEvent("ChatPdf", "Chat_Failed_" + ex2.GetType().Name, "Count", 1L);
				}
				finally
				{
					this.chatting = false;
				}
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			return copilotResult;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0003FA94 File Offset: 0x0003DC94
		private Task<CopilotHelper.PdfModel> BuildPdfModel([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>[] items, CancellationToken cancellationToken)
		{
			return this.BuildPdfModel(items.Where(delegate([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel> c)
			{
				CopilotHelper.AnalyzeResponseModel item = c.Item2;
				object obj;
				if (item == null)
				{
					obj = null;
				}
				else
				{
					List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> items2 = item.Items;
					if (items2 == null)
					{
						obj = null;
					}
					else
					{
						CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel analyzeResponseItemModel = items2.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
						if (analyzeResponseItemModel == null)
						{
							obj = null;
						}
						else
						{
							List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> data = analyzeResponseItemModel.Data;
							if (data == null)
							{
								obj = null;
							}
							else
							{
								CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel analyzeResponseDataModel = data.FirstOrDefault<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel>();
								obj = ((analyzeResponseDataModel != null) ? analyzeResponseDataModel.Values : null);
							}
						}
					}
				}
				return obj != null;
			}).SelectMany(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel> c) => c.Item2.Items.Select((CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel x) => new global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>(c.Item1, x))).ToArray<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>>(), cancellationToken);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0003FAF8 File Offset: 0x0003DCF8
		private async Task<CopilotHelper.PdfModel> BuildPdfModel([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "data" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>[] items, CancellationToken cancellationToken)
		{
			PdfDocument doc = this.Document;
			CopilotHelper.PdfModel pdfModel2;
			if (doc == null)
			{
				pdfModel2 = null;
			}
			else
			{
				int num = 0;
				CopilotHelper.PdfModel pdfModel = new CopilotHelper.PdfModel
				{
					Pages = new List<CopilotHelper.PdfPageModel>()
				};
				Dictionary<int, List<global::System.ValueTuple<int, int>>> dictionary = new Dictionary<int, List<global::System.ValueTuple<int, int>>>();
				foreach (global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> valueTuple in items)
				{
					int item2 = valueTuple.Item1;
					CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel item3 = valueTuple.Item2;
					num += item3.Usage;
					if (num < 2500)
					{
						List<global::System.ValueTuple<int, int>> list;
						if (!dictionary.TryGetValue(item2, out list))
						{
							list = new List<global::System.ValueTuple<int, int>>();
							dictionary[item2] = list;
						}
						list.Add(new global::System.ValueTuple<int, int>(item3.TextIndex, item3.TextLength));
					}
				}
				if (dictionary.Count > 0)
				{
					foreach (KeyValuePair<int, List<global::System.ValueTuple<int, int>>> item in dictionary)
					{
						int pageIndex = item.Key;
						if (item.Value.Count > 0)
						{
							string text = await this.GetPageContentAsync(pageIndex, cancellationToken);
							if (!string.IsNullOrEmpty(text))
							{
								List<global::System.ValueTuple<int, int>> list2 = item.Value.OrderBy(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "index", "length" })] global::System.ValueTuple<int, int> c) => c.Item1).ToList<global::System.ValueTuple<int, int>>();
								for (int j = list2.Count - 2; j >= 0; j--)
								{
									global::System.ValueTuple<int, int> valueTuple2 = list2[j + 1];
									global::System.ValueTuple<int, int> valueTuple3 = list2[j];
									if (valueTuple3.Item1 + valueTuple3.Item2 >= valueTuple2.Item1)
									{
										list2[j] = new global::System.ValueTuple<int, int>(valueTuple3.Item1, valueTuple2.Item1 + valueTuple2.Item2 - valueTuple3.Item1);
										list2.RemoveAt(j + 1);
									}
								}
								StringBuilder stringBuilder = new StringBuilder();
								for (int k = 0; k < list2.Count; k++)
								{
									try
									{
										int item4 = list2[k].Item1;
										int num2 = Math.Min(text.Length - list2[k].Item1, list2[k].Item2);
										if (item4 >= 0 && num2 > 0 && item4 < text.Length && item4 + num2 <= text.Length)
										{
											stringBuilder.Append(text.Substring(list2[k].Item1, Math.Min(text.Length - list2[k].Item1, list2[k].Item2)));
										}
									}
									catch
									{
									}
								}
								pdfModel.Pages.Add(new CopilotHelper.PdfPageModel
								{
									Content = stringBuilder.ToString(),
									PageIndex = pageIndex
								});
							}
						}
						item = default(KeyValuePair<int, List<global::System.ValueTuple<int, int>>>);
					}
					Dictionary<int, List<global::System.ValueTuple<int, int>>>.Enumerator enumerator = default(Dictionary<int, List<global::System.ValueTuple<int, int>>>.Enumerator);
				}
				pdfModel.PageCount = doc.Pages.Count;
				pdfModel.FileName = Path.GetFileNameWithoutExtension(this.filePath);
				pdfModel2 = pdfModel;
			}
			return pdfModel2;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0003FB4C File Offset: 0x0003DD4C
		public void ClearAsync()
		{
			Dictionary<int, CopilotHelper.AnalyzeResponseModel> dictionary = this.memoryCache;
			lock (dictionary)
			{
				this.memoryCache.Clear();
			}
			LruCache<int, string> lruCache = this.cachedPageContent;
			lock (lruCache)
			{
				this.cachedPageContent.Clear();
			}
			Dictionary<string, CopilotHelper.AnalyzeResponseModel> dictionary2 = this.memoryCache2;
			lock (dictionary2)
			{
				this.memoryCache2.Clear();
			}
			this.cachedChatMessage = new List<CopilotHelper.ChatMessage>();
			try
			{
				CopilotHelper.AnalyzeTask analyzeTask = this.analyzeTask;
				if (analyzeTask != null)
				{
					analyzeTask.Dispose();
				}
				this.analyzeTask = new CopilotHelper.AnalyzeTask(this);
			}
			catch
			{
			}
			try
			{
				CopilotHelper.Cache.RemoveFromCache(this.filePath);
			}
			catch
			{
			}
			try
			{
				CopilotHelper.Cache.ClearChatMessagesInCache(this.filePath);
			}
			catch
			{
			}
			try
			{
				CopilotHelper.Cache.SetSummaryToCache(this.filePath, "");
			}
			catch
			{
			}
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0003FC90 File Offset: 0x0003DE90
		public string GetCachedSummary()
		{
			return CopilotHelper.Cache.GetSummaryFromCache(this.filePath);
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0003FC9D File Offset: 0x0003DE9D
		public List<CopilotHelper.ChatMessage> GetCachedMessageList()
		{
			return CopilotHelper.Cache.GetChatMessagesFromCache(this.filePath);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0003FCAC File Offset: 0x0003DEAC
		public void ClearMessageList()
		{
			this.cachedChatMessage = new List<CopilotHelper.ChatMessage>();
			try
			{
				CopilotHelper.Cache.ClearChatMessagesInCache(this.filePath);
			}
			catch
			{
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0003FCE4 File Offset: 0x0003DEE4
		private async Task<CopilotHelper.AnalyzeResponseModel> AnalyzeMessageAsync(string message, CancellationToken cancellationToken)
		{
			CopilotHelper.AnalyzeResponseModel analyzeResponseModel;
			if (string.IsNullOrWhiteSpace(message))
			{
				analyzeResponseModel = null;
			}
			else
			{
				message = message.Trim();
				Dictionary<string, CopilotHelper.AnalyzeResponseModel> dictionary = this.memoryCache2;
				lock (dictionary)
				{
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel2;
					if (this.memoryCache2.TryGetValue(message, out analyzeResponseModel2))
					{
						return analyzeResponseModel2;
					}
				}
				CopilotHelper.AnalyzeResponseModel analyzeResponseModel3 = await CopilotHelper.InternalCopilotHelper.AnalyzeAsync(message, cancellationToken);
				if (analyzeResponseModel3 == null)
				{
					dictionary = this.memoryCache2;
					lock (dictionary)
					{
						this.memoryCache2[message] = analyzeResponseModel3;
					}
				}
				analyzeResponseModel = analyzeResponseModel3;
			}
			return analyzeResponseModel;
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0003FD38 File Offset: 0x0003DF38
		private async Task<CopilotHelper.AnalyzeResponseModel> AnalyzeAsync(int pageIndex)
		{
			PdfDocument document = this.Document;
			CopilotHelper.AnalyzeResponseModel analyzeResponseModel;
			if (document == null || pageIndex < 0 || pageIndex >= this.pageCountForAnalyze || pageIndex >= document.Pages.Count)
			{
				analyzeResponseModel = null;
			}
			else
			{
				Dictionary<int, CopilotHelper.AnalyzeResponseModel> dictionary = this.memoryCache;
				lock (dictionary)
				{
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel2;
					if (this.memoryCache.TryGetValue(pageIndex, out analyzeResponseModel2))
					{
						return analyzeResponseModel2;
					}
				}
				try
				{
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel3 = await CopilotHelper.Cache.GetModelFromCache(this.filePath, pageIndex);
					if (analyzeResponseModel3 != null)
					{
						dictionary = this.memoryCache;
						lock (dictionary)
						{
							this.memoryCache[pageIndex] = analyzeResponseModel3;
						}
						return analyzeResponseModel3;
					}
				}
				catch
				{
				}
				analyzeResponseModel = null;
			}
			return analyzeResponseModel;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0003FD84 File Offset: 0x0003DF84
		private async Task<string> GetPageContentAsync(int pageIndex, CancellationToken cancellationToken)
		{
			string pageContent = this.GetPageContent(pageIndex);
			string text;
			if (!string.IsNullOrEmpty(pageContent) && pageContent.Length > 200)
			{
				text = pageContent;
			}
			else
			{
				if (!this.ocrGaSended)
				{
					Dictionary<int, string> dictionary = this.cachedPageOcrContent;
					lock (dictionary)
					{
						if (this.cachedPageOcrContent.Count > 3)
						{
							this.ocrGaSended = true;
							GAManager.SendEvent("ChatPdf", "Doc_Ocr", "Count", 1L);
						}
					}
				}
				text = await this.GetOCRPageContentAsync(pageIndex, cancellationToken);
			}
			return text;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0003FDD8 File Offset: 0x0003DFD8
		private string GetPageContent(int pageIndex)
		{
			PdfDocument document = this.Document;
			if (document == null || pageIndex < 0 || pageIndex >= this.pageCountForAnalyze || pageIndex >= document.Pages.Count)
			{
				return null;
			}
			LruCache<int, string> lruCache = this.cachedPageContent;
			lock (lruCache)
			{
				string text;
				if (this.cachedPageContent.Count > 0 && this.cachedPageContent.TryGetValue(pageIndex, out text))
				{
					return text;
				}
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			string text2 = null;
			try
			{
				intPtr = Pdfium.FPDF_LoadPage(document.Handle, pageIndex);
				if (intPtr != IntPtr.Zero)
				{
					intPtr2 = Pdfium.FPDFText_LoadPage(intPtr);
					if (intPtr2 != IntPtr.Zero)
					{
						int num = Pdfium.FPDFText_CountChars(intPtr2);
						text2 = Pdfium.FPDFText_GetText(intPtr2, 0, num);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					Pdfium.FPDFText_ClosePage(intPtr2);
				}
				if (intPtr != IntPtr.Zero)
				{
					Pdfium.FPDF_ClosePage(intPtr);
				}
			}
			if (text2 != null)
			{
				lruCache = this.cachedPageContent;
				lock (lruCache)
				{
					this.cachedPageContent[pageIndex] = text2;
				}
			}
			return text2;
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0003FF3C File Offset: 0x0003E13C
		private async Task<string> GetOCRPageContentAsync(int pageIndex, CancellationToken cancellationToken)
		{
			PdfDocument doc = this.Document;
			string text;
			if (doc == null || pageIndex < 0 || pageIndex >= this.pageCountForAnalyze || pageIndex >= doc.Pages.Count)
			{
				text = null;
			}
			else
			{
				Dictionary<int, string> dictionary = this.cachedPageOcrContent;
				lock (dictionary)
				{
					string text2;
					if (this.cachedPageOcrContent.Count > 0 && this.cachedPageOcrContent.TryGetValue(pageIndex, out text2))
					{
						return text2 ?? string.Empty;
					}
				}
				IntPtr page = IntPtr.Zero;
				IntPtr textPage = IntPtr.Zero;
				string text3 = null;
				try
				{
					page = Pdfium.FPDF_LoadPage(doc.Handle, pageIndex);
					if (page != IntPtr.Zero)
					{
						double num;
						double num2;
						Pdfium.FPDF_GetPageSizeByIndex(doc.Handle, pageIndex, out num, out num2);
						double num3 = Math.Min(num, num2);
						using (PdfPage pageObj = PdfPage.FromHandle(doc, page, pageIndex, true))
						{
							bool flag2 = false;
							PdfPageObject[] array = pageObj.PageObjects.SelectMany((PdfPageObject c) => CopilotHelper.<GetOCRPageContentAsync>g__FlattenCore|46_1(c)).ToArray<PdfPageObject>();
							foreach (PdfImageObject pdfImageObject in array.OfType<PdfImageObject>())
							{
								FS_RECTF boundingBox = pdfImageObject.BoundingBox;
								if ((double)boundingBox.Width > num3 / 4.0 || (double)boundingBox.Height > num3 / 4.0)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2 && array.OfType<PdfPathObject>().Count<PdfPathObject>() >= 20)
							{
								flag2 = true;
							}
							if (flag2)
							{
								double num4 = 1.6666666666666667;
								using (PdfBitmap pdfBitmap = new PdfBitmap((int)(num * num4), (int)(num2 * num4), true, false))
								{
									int num5 = -1;
									Pdfium.FPDFBitmap_FillRect(pdfBitmap.Handle, 0, 0, pdfBitmap.Width, pdfBitmap.Height, num5);
									pageObj.RenderEx(pdfBitmap, 0, 0, pdfBitmap.Width, pdfBitmap.Height, PageRotate.Normal, RenderFlags.FPDF_NONE);
									string text4 = null;
									if (this.ocrCultureInfo == null)
									{
										global::System.ValueTuple<string, CultureInfo> valueTuple = await OcrUtils.GetStringAndCultureAsync(pdfBitmap.Image, cancellationToken);
										string item = valueTuple.Item1;
										CultureInfo item2 = valueTuple.Item2;
										text4 = item;
										if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty((item2 != null) ? item2.Name : null) || this.ocrCultureInfo != null)
										{
											goto IL_048A;
										}
										Dictionary<string, int> dictionary2 = this.ocrCultureInfoCount;
										lock (dictionary2)
										{
											int num6 = ((!this.ocrCultureInfoCount.TryGetValue(item2.Name, out num6)) ? 1 : (num6 + 1));
											this.ocrCultureInfoCount[item2.Name] = num6;
											string key = this.ocrCultureInfoCount.FirstOrDefault((KeyValuePair<string, int> c) => c.Value > 8).Key;
											if (!string.IsNullOrEmpty(key))
											{
												try
												{
													this.ocrCultureInfo = CultureInfo.GetCultureInfo(key);
												}
												catch
												{
												}
											}
											goto IL_048A;
										}
									}
									text4 = await OcrUtils.GetStringAsync(pdfBitmap.Image, this.ocrCultureInfo).WaitAsync(this.cts.Token);
									IL_048A:
									if (!doc.IsDisposed && !string.IsNullOrEmpty(text4))
									{
										textPage = Pdfium.FPDFText_LoadPage(page);
										if (textPage != IntPtr.Zero)
										{
											string text5 = Pdfium.FPDFText_GetText(textPage, 0, Pdfium.FPDFText_CountChars(textPage));
											if (text5 != null && text5.Length > text4.Length / 4 * 3)
											{
												text3 = text5;
											}
											else
											{
												text3 = text4;
											}
										}
										else
										{
											text3 = text4;
										}
										dictionary = this.cachedPageOcrContent;
										lock (dictionary)
										{
											this.cachedPageOcrContent[pageIndex] = text3;
										}
										return text3 ?? string.Empty;
									}
								}
								PdfBitmap pdfBitmap = null;
							}
						}
						PdfPage pageObj = null;
					}
				}
				catch
				{
				}
				finally
				{
					if (!doc.IsDisposed && textPage != IntPtr.Zero)
					{
						Pdfium.FPDFText_ClosePage(textPage);
					}
					if (!doc.IsDisposed && page != IntPtr.Zero)
					{
						Pdfium.FPDF_ClosePage(page);
					}
				}
				text = string.Empty;
			}
			return text;
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x0003FF8F File Offset: 0x0003E18F
		private bool CanChat()
		{
			return CopilotHelper.Cache.GetChatCount(this.filePath) < 50;
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0003FFA0 File Offset: 0x0003E1A0
		public int GetChatRemaining()
		{
			int chatCount = CopilotHelper.Cache.GetChatCount(this.filePath);
			return Math.Max(0, 50 - chatCount);
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x0003FFC4 File Offset: 0x0003E1C4
		public void ShowFeedbackWindow(bool fromDislike)
		{
			string documentPath = Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.DocumentPath;
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.Owner = App.Current.MainWindow;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			feedbackWindow.source = "ChatPdf";
			feedbackWindow.HideFaq();
			if (fromDislike)
			{
				feedbackWindow.SetChatDislike();
			}
			if (!string.IsNullOrEmpty(documentPath))
			{
				feedbackWindow.flist.Add(documentPath);
				feedbackWindow.showAttachmentCB(true);
			}
			feedbackWindow.ShowDialog();
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00040040 File Offset: 0x0003E240
		public async Task<CopilotHelper.CopilotResult> SummarizeAsync(string input, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
		{
			return await this.DoAppActionAsync("Summarize", new CopilotHelper.DoActionRequestModel
			{
				Input = input,
				Stream = true
			}, action, cancellationToken);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0004009C File Offset: 0x0003E29C
		public async Task<CopilotHelper.CopilotResult> TranslateAsync(string input, string language, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken, string sourcelanguage = "")
		{
			return await this.DoTranslateAsync("Translate", new CopilotHelper.TranslateRequestModel
			{
				Input = input,
				Stream = true,
				Language = language,
				SourceLanguage = sourcelanguage
			}, action, cancellationToken);
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x0004010C File Offset: 0x0003E30C
		public async Task<CopilotHelper.CopilotResult> RewriteAsync(string input, string style, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
		{
			return await this.DoAppActionAsync("Rewrite", new CopilotHelper.DoActionRequestModel
			{
				Input = input,
				Stream = true,
				Style = style
			}, action, cancellationToken);
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00040170 File Offset: 0x0003E370
		private async Task<CopilotHelper.CopilotResult> DoTranslateAsync(string actionName, CopilotHelper.TranslateRequestModel model, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
		{
			CopilotHelper.<>c__DisplayClass53_0 CS$<>8__locals1 = new CopilotHelper.<>c__DisplayClass53_0();
			CS$<>8__locals1.action = action;
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken);
			CS$<>8__locals1.sb = new StringBuilder();
			CopilotHelper.StreamRequestResult streamRequestResult = await CopilotHelper.InternalCopilotHelper.DoTranslateAsync(actionName, model, delegate(string s, CancellationToken ct)
			{
				CopilotHelper.<>c__DisplayClass53_0.<<DoTranslateAsync>b__0>d <<DoTranslateAsync>b__0>d;
				<<DoTranslateAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<DoTranslateAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<DoTranslateAsync>b__0>d.s = s;
				<<DoTranslateAsync>b__0>d.ct = ct;
				<<DoTranslateAsync>b__0>d.<>1__state = -1;
				<<DoTranslateAsync>b__0>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass53_0.<<DoTranslateAsync>b__0>d>(ref <<DoTranslateAsync>b__0>d);
				return <<DoTranslateAsync>b__0>d.<>t__builder.Task;
			}, cancellationTokenSource.Token);
			CopilotHelper.CopilotResult copilotResult;
			if (streamRequestResult == null)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else
			{
				copilotResult = new CopilotHelper.CopilotResult(null, CS$<>8__locals1.sb.ToString(), streamRequestResult.Error);
			}
			return copilotResult;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x000401D4 File Offset: 0x0003E3D4
		private async Task<CopilotHelper.CopilotResult> DoAppActionAsync(string actionName, CopilotHelper.DoActionRequestModel model, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
		{
			CopilotHelper.<>c__DisplayClass54_0 CS$<>8__locals1 = new CopilotHelper.<>c__DisplayClass54_0();
			CS$<>8__locals1.action = action;
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, cancellationToken);
			CS$<>8__locals1.sb = new StringBuilder();
			CopilotHelper.StreamRequestResult streamRequestResult = await CopilotHelper.InternalCopilotHelper.DoAppActionAsync("Summarize", model, delegate(string s, CancellationToken ct)
			{
				CopilotHelper.<>c__DisplayClass54_0.<<DoAppActionAsync>b__0>d <<DoAppActionAsync>b__0>d;
				<<DoAppActionAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<DoAppActionAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<DoAppActionAsync>b__0>d.s = s;
				<<DoAppActionAsync>b__0>d.ct = ct;
				<<DoAppActionAsync>b__0>d.<>1__state = -1;
				<<DoAppActionAsync>b__0>d.<>t__builder.Start<CopilotHelper.<>c__DisplayClass54_0.<<DoAppActionAsync>b__0>d>(ref <<DoAppActionAsync>b__0>d);
				return <<DoAppActionAsync>b__0>d.<>t__builder.Task;
			}, cancellationTokenSource.Token);
			CopilotHelper.CopilotResult copilotResult;
			if (streamRequestResult == null)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else
			{
				copilotResult = new CopilotHelper.CopilotResult(null, CS$<>8__locals1.sb.ToString(), streamRequestResult.Error);
			}
			return copilotResult;
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00040230 File Offset: 0x0003E430
		public async Task<bool> ProcessNativeAppAction(CopilotHelper.AppActionModel appAction)
		{
			bool flag;
			if (appAction == null)
			{
				flag = false;
			}
			else
			{
				await CopilotHelper.AppActionHelper.ProcessAction(appAction);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00040273 File Offset: 0x0003E473
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					CopilotHelper.AnalyzeTask analyzeTask = this.analyzeTask;
					if (analyzeTask != null)
					{
						analyzeTask.Dispose();
					}
					this.analyzeTask = null;
				}
				CancellationTokenSource cancellationTokenSource = this.cts;
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Cancel();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x000402B0 File Offset: 0x0003E4B0
		~CopilotHelper()
		{
			this.Dispose(false);
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x000402E0 File Offset: 0x0003E4E0
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x000402F0 File Offset: 0x0003E4F0
		[CompilerGenerated]
		internal static IEnumerable<PdfPageObject> <GetOCRPageContentAsync>g__FlattenCore|46_1(PdfPageObject _pageObject)
		{
			PdfFormObject pdfFormObject = _pageObject as PdfFormObject;
			if (pdfFormObject != null)
			{
				return pdfFormObject.PageObjects.SelectMany((PdfPageObject _c) => CopilotHelper.<GetOCRPageContentAsync>g__FlattenCore|46_1(_c));
			}
			if (_pageObject != null)
			{
				return Enumerable.Repeat<PdfPageObject>(_pageObject, 1);
			}
			return Enumerable.Empty<PdfPageObject>();
		}

		// Token: 0x04000584 RID: 1412
		private const int maxPageCountForAnalyze = 120;

		// Token: 0x04000585 RID: 1413
		public const int MaxSendChatCount = 50;

		// Token: 0x04000586 RID: 1414
		private const bool requestStreamingApi = true;

		// Token: 0x04000587 RID: 1415
		private bool disposedValue;

		// Token: 0x04000588 RID: 1416
		private CancellationTokenSource cts;

		// Token: 0x04000589 RID: 1417
		private WeakReference<PdfDocument> _document;

		// Token: 0x0400058A RID: 1418
		private string filePath;

		// Token: 0x0400058B RID: 1419
		private Dictionary<int, CopilotHelper.AnalyzeResponseModel> memoryCache = new Dictionary<int, CopilotHelper.AnalyzeResponseModel>();

		// Token: 0x0400058C RID: 1420
		private Dictionary<string, CopilotHelper.AnalyzeResponseModel> memoryCache2 = new Dictionary<string, CopilotHelper.AnalyzeResponseModel>();

		// Token: 0x0400058D RID: 1421
		private LruCache<int, string> cachedPageContent = new LruCache<int, string>(50);

		// Token: 0x0400058E RID: 1422
		private Dictionary<int, string> cachedPageOcrContent = new Dictionary<int, string>(50);

		// Token: 0x0400058F RID: 1423
		private List<CopilotHelper.ChatMessage> cachedChatMessage = new List<CopilotHelper.ChatMessage>();

		// Token: 0x04000590 RID: 1424
		private bool chatting;

		// Token: 0x04000591 RID: 1425
		private CopilotHelper.AnalyzeTask analyzeTask;

		// Token: 0x04000592 RID: 1426
		private bool ocrGaSended;

		// Token: 0x04000593 RID: 1427
		private CultureInfo ocrCultureInfo;

		// Token: 0x04000594 RID: 1428
		private Dictionary<string, int> ocrCultureInfoCount = new Dictionary<string, int>();

		// Token: 0x04000595 RID: 1429
		private int pageCountForAnalyze;

		// Token: 0x02000503 RID: 1283
		private class AnalyzeTask : IDisposable
		{
			// Token: 0x17000CCF RID: 3279
			// (get) Token: 0x06002F69 RID: 12137 RVA: 0x000EA0B2 File Offset: 0x000E82B2
			public int ProcessedPageIndex
			{
				get
				{
					return this.curPageIdx;
				}
			}

			// Token: 0x06002F6A RID: 12138 RVA: 0x000EA0BA File Offset: 0x000E82BA
			public AnalyzeTask(CopilotHelper helper)
			{
				this.cts = new CancellationTokenSource();
				this.helper = helper;
			}

			// Token: 0x06002F6B RID: 12139 RVA: 0x000EA0D4 File Offset: 0x000E82D4
			public async Task Start(IProgress<double> progressReporter)
			{
				this.currentTask = this.RunCore(10, progressReporter);
				int num = await this.currentTask;
				this.curToken = num;
				this.Continue();
			}

			// Token: 0x06002F6C RID: 12140 RVA: 0x000EA120 File Offset: 0x000E8320
			public async Task Continue()
			{
				if (this.currentTask == null || this.currentTask.IsCompleted)
				{
					if (!this.cts.IsCancellationRequested)
					{
						if (this.curPageIdx < this.helper.pageCountForAnalyze)
						{
							int chatCount = CopilotHelper.Cache.GetChatCount(this.helper.filePath);
							int num = chatCount / 3 + 1;
							if (chatCount > 8)
							{
								num = 999;
							}
							int num2 = num * 10 + 10;
							if (num2 > this.helper.pageCountForAnalyze)
							{
								num2 = this.helper.pageCountForAnalyze;
							}
							Log.WriteLog(string.Format("AnalyzeTask.Continue() pageCount: {0}, stage: {1}", num2, num));
							if (num2 > this.curPageIdx)
							{
								this.currentTask = this.RunCore(10, null);
								int num3 = await this.currentTask;
								this.curToken += num3;
								if (!this.cts.IsCancellationRequested)
								{
									this.Continue();
								}
							}
						}
					}
				}
			}

			// Token: 0x06002F6D RID: 12141 RVA: 0x000EA164 File Offset: 0x000E8364
			private async Task<int> RunCore(int pageCount, IProgress<double> progressReporter)
			{
				CopilotHelper.AnalyzeTask.<>c__DisplayClass13_0 CS$<>8__locals1 = new CopilotHelper.AnalyzeTask.<>c__DisplayClass13_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.progressReporter = progressReporter;
				CS$<>8__locals1.processPageCount = Math.Min(pageCount, this.helper.pageCountForAnalyze - this.curPageIdx);
				int num;
				if (CS$<>8__locals1.processPageCount <= 0)
				{
					num = 0;
				}
				else
				{
					CS$<>8__locals1.progress = 0.0;
					IEnumerable<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>> enumerable = await Task.WhenAll<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>(Enumerable.Range(this.curPageIdx, CS$<>8__locals1.processPageCount).Select(delegate(int c)
					{
						CopilotHelper.AnalyzeTask.<>c__DisplayClass13_0.<<RunCore>b__0>d <<RunCore>b__0>d;
						<<RunCore>b__0>d.<>t__builder = AsyncTaskMethodBuilder<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>.Create();
						<<RunCore>b__0>d.<>4__this = CS$<>8__locals1;
						<<RunCore>b__0>d.c = c;
						<<RunCore>b__0>d.<>1__state = -1;
						<<RunCore>b__0>d.<>t__builder.Start<CopilotHelper.AnalyzeTask.<>c__DisplayClass13_0.<<RunCore>b__0>d>(ref <<RunCore>b__0>d);
						return <<RunCore>b__0>d.<>t__builder.Task;
					}).ToArray<Task<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>>>());
					this.curPageIdx += CS$<>8__locals1.processPageCount;
					IProgress<double> progressReporter2 = CS$<>8__locals1.progressReporter;
					if (progressReporter2 != null)
					{
						progressReporter2.Report(1.0);
					}
					num = enumerable.SelectMany(delegate([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "model" })] global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel> c)
					{
						CopilotHelper.AnalyzeResponseModel item = c.Item2;
						IEnumerable<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> enumerable2 = ((item != null) ? item.Items : null);
						return enumerable2 ?? Enumerable.Empty<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel>();
					}).Sum(delegate(CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel c)
					{
						if (c == null)
						{
							return 0;
						}
						return c.Usage;
					});
				}
				return num;
			}

			// Token: 0x06002F6E RID: 12142 RVA: 0x000EA1B8 File Offset: 0x000E83B8
			private async Task Run()
			{
				if (!this.cts.IsCancellationRequested)
				{
					int tmp = this.curToken;
					do
					{
						int num = await Task.Run<int>(async () => await this.RunCore(5, null));
						tmp += num;
					}
					while (!this.cts.IsCancellationRequested && this.curPageIdx < this.helper.pageCountForAnalyze);
					this.curToken = tmp;
				}
			}

			// Token: 0x06002F6F RID: 12143 RVA: 0x000EA1FC File Offset: 0x000E83FC
			[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "pageIndex", "model" })]
			private async Task<global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>> AnalyzeAsync(int pageIndex)
			{
				if (!this.cts.IsCancellationRequested && pageIndex >= 0 && pageIndex < this.helper.pageCountForAnalyze)
				{
					CopilotHelper.AnalyzeResponseModel analyzeResponseModel = await CopilotHelper.Cache.GetModelFromCache(this.helper.filePath, pageIndex);
					CopilotHelper.AnalyzeResponseModel model = analyzeResponseModel;
					if (!this.cts.IsCancellationRequested && model == null)
					{
						string text = await this.helper.GetPageContentAsync(pageIndex, this.cts.Token);
						if (!string.IsNullOrEmpty(text))
						{
							try
							{
								analyzeResponseModel = await CopilotHelper.InternalCopilotHelper.AnalyzeAsync(text, this.cts.Token);
								model = analyzeResponseModel;
								await CopilotHelper.Cache.SetModelToCache(this.helper.filePath, pageIndex, model);
								return new global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>(pageIndex, model);
							}
							catch (Exception)
							{
							}
						}
					}
					model = null;
				}
				return new global::System.ValueTuple<int, CopilotHelper.AnalyzeResponseModel>(pageIndex, null);
			}

			// Token: 0x06002F70 RID: 12144 RVA: 0x000EA247 File Offset: 0x000E8447
			protected virtual void Dispose(bool disposing)
			{
				if (!this.disposedValue)
				{
					if (disposing)
					{
						this.cts.Cancel();
					}
					this.disposedValue = true;
				}
			}

			// Token: 0x06002F71 RID: 12145 RVA: 0x000EA266 File Offset: 0x000E8466
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x04001C00 RID: 7168
			private const int StartPageCount = 10;

			// Token: 0x04001C01 RID: 7169
			private const int PageCountPreStage = 10;

			// Token: 0x04001C02 RID: 7170
			private readonly CopilotHelper helper;

			// Token: 0x04001C03 RID: 7171
			private bool disposedValue;

			// Token: 0x04001C04 RID: 7172
			private CancellationTokenSource cts;

			// Token: 0x04001C05 RID: 7173
			private int curToken;

			// Token: 0x04001C06 RID: 7174
			private int curPageIdx;

			// Token: 0x04001C07 RID: 7175
			private Task<int> currentTask;
		}

		// Token: 0x02000504 RID: 1284
		internal static class Cache
		{
			// Token: 0x06002F73 RID: 12147 RVA: 0x000EA2BC File Offset: 0x000E84BC
			public static List<CopilotHelper.ChatMessage> GetChatMessagesFromCache(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return new List<CopilotHelper.ChatMessage>();
				}
				List<CopilotHelper.Cache.ChatMessageCacheModel> chatMessagesFromCacheCore = CopilotHelper.Cache.GetChatMessagesFromCacheCore(id);
				if (chatMessagesFromCacheCore != null)
				{
					return chatMessagesFromCacheCore.Select((CopilotHelper.Cache.ChatMessageCacheModel c) => c.ToChatMessage()).ToList<CopilotHelper.ChatMessage>();
				}
				return null;
			}

			// Token: 0x06002F74 RID: 12148 RVA: 0x000EA314 File Offset: 0x000E8514
			public static void ClearChatMessagesInCache(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return;
				}
				CopilotHelper.Cache.SetChatMessagesToCacheCore(id, null);
			}

			// Token: 0x06002F75 RID: 12149 RVA: 0x000EA338 File Offset: 0x000E8538
			public static bool AppendChatMessagesToCache(string fileName, params CopilotHelper.ChatMessage[] messages)
			{
				if (messages == null || messages.Length == 0)
				{
					return false;
				}
				CopilotHelper.Cache.ChatMessageCacheModel[] array = (from c in messages
					where !string.IsNullOrEmpty(c.Content) && (c.Role == "assistant" || c.Role == "user")
					select new CopilotHelper.Cache.ChatMessageCacheModel(c)).ToArray<CopilotHelper.Cache.ChatMessageCacheModel>();
				if (array.Length == 0)
				{
					return false;
				}
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return false;
				}
				List<CopilotHelper.Cache.ChatMessageCacheModel> chatMessagesFromCacheCore = CopilotHelper.Cache.GetChatMessagesFromCacheCore(id);
				chatMessagesFromCacheCore.AddRange(array);
				CopilotHelper.Cache.SetChatMessagesToCacheCore(id, chatMessagesFromCacheCore);
				return true;
			}

			// Token: 0x06002F76 RID: 12150 RVA: 0x000EA3CC File Offset: 0x000E85CC
			public static bool AppendChatMessagesLikedToCache(string fileName, CopilotHelper.ChatMessage messages)
			{
				if (messages == null)
				{
					return false;
				}
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return false;
				}
				List<CopilotHelper.Cache.ChatMessageCacheModel> chatMessagesFromCacheCore = CopilotHelper.Cache.GetChatMessagesFromCacheCore(id);
				foreach (CopilotHelper.Cache.ChatMessageCacheModel chatMessageCacheModel in chatMessagesFromCacheCore)
				{
					if (chatMessageCacheModel.Content == messages.Content && chatMessageCacheModel.Role == messages.Role)
					{
						chatMessageCacheModel.Liked = messages.Liked;
					}
				}
				CopilotHelper.Cache.SetChatMessagesToCacheCore(id, chatMessagesFromCacheCore);
				return true;
			}

			// Token: 0x06002F77 RID: 12151 RVA: 0x000EA46C File Offset: 0x000E866C
			public static string GetSummaryFromCache(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return "";
				}
				return CopilotHelper.Cache.GetSummaryFromCacheCore(id);
			}

			// Token: 0x06002F78 RID: 12152 RVA: 0x000EA494 File Offset: 0x000E8694
			public static void SetSummaryToCache(string fileName, string summary)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return;
				}
				CopilotHelper.Cache.SetSummaryToCacheCore(id, summary);
			}

			// Token: 0x06002F79 RID: 12153 RVA: 0x000EA4B8 File Offset: 0x000E86B8
			public static int GetChatCount(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return 0;
				}
				return CopilotHelper.Cache.GetChatCountCore(id);
			}

			// Token: 0x06002F7A RID: 12154 RVA: 0x000EA4DC File Offset: 0x000E86DC
			public static int IncreaseChatCount(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(id))
				{
					return 0;
				}
				int num = CopilotHelper.Cache.GetChatCountCore(id);
				num++;
				CopilotHelper.Cache.SetChatCountCore(id, num);
				return num;
			}

			// Token: 0x06002F7B RID: 12155 RVA: 0x000EA510 File Offset: 0x000E8710
			public static async Task<CopilotHelper.AnalyzeResponseModel> GetModelFromCache(string fileName, int pageIndex)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				CopilotHelper.AnalyzeResponseModel analyzeResponseModel;
				if (string.IsNullOrEmpty(id))
				{
					analyzeResponseModel = null;
				}
				else
				{
					analyzeResponseModel = await CopilotHelper.Cache.GetModelFromCacheCoreAsync(id, pageIndex);
				}
				return analyzeResponseModel;
			}

			// Token: 0x06002F7C RID: 12156 RVA: 0x000EA55C File Offset: 0x000E875C
			public static async Task SetModelToCache(string fileName, int pageIndex, CopilotHelper.AnalyzeResponseModel model)
			{
				string text = CopilotHelper.Cache.GetId(fileName);
				if (string.IsNullOrEmpty(text))
				{
					text = Guid.NewGuid().ToString("N");
					CopilotHelper.Cache.SetId(fileName, text);
				}
				await CopilotHelper.Cache.SetModelToCacheCoreAsync(text, pageIndex, model);
			}

			// Token: 0x06002F7D RID: 12157 RVA: 0x000EA5B0 File Offset: 0x000E87B0
			public static void RemoveFromCache(string fileName)
			{
				string id = CopilotHelper.Cache.GetId(fileName);
				if (!string.IsNullOrEmpty(id))
				{
					CopilotHelper.Cache.SetId(fileName, "");
					string text = Path.Combine(CopilotHelper.Cache.GetCacheFolder(), id);
					if (Directory.Exists(text))
					{
						try
						{
							Directory.Delete(text, true);
						}
						catch
						{
						}
					}
				}
			}

			// Token: 0x06002F7E RID: 12158 RVA: 0x000EA608 File Offset: 0x000E8808
			private static async Task<CopilotHelper.AnalyzeResponseModel> GetModelFromCacheCoreAsync(string id, int pageIndex)
			{
				CopilotHelper.AnalyzeResponseModel analyzeResponseModel;
				if (string.IsNullOrEmpty(id))
				{
					analyzeResponseModel = null;
				}
				else
				{
					string text = Path.Combine(CopilotHelper.Cache.GetCacheFolder(), id);
					if (Directory.Exists(text))
					{
						string text2 = Path.Combine(text, string.Format("{0}", pageIndex));
						if (File.Exists(text2))
						{
							try
							{
								using (FileStream fileStream = new FileStream(text2, FileMode.Open, FileAccess.Read))
								{
									using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, true))
									{
										return JsonConvert.DeserializeObject<CopilotHelper.AnalyzeResponseModel>(await reader.ReadToEndAsync());
									}
								}
							}
							catch
							{
							}
						}
					}
					analyzeResponseModel = null;
				}
				return analyzeResponseModel;
			}

			// Token: 0x06002F7F RID: 12159 RVA: 0x000EA654 File Offset: 0x000E8854
			private static async Task SetModelToCacheCoreAsync(string id, int pageIndex, CopilotHelper.AnalyzeResponseModel model)
			{
				if (!string.IsNullOrEmpty(id) && model != null)
				{
					string text = Path.Combine(CopilotHelper.Cache.GetCacheFolder(), id);
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					string text2 = Path.Combine(text, string.Format("{0}", pageIndex));
					try
					{
						string text3 = JsonConvert.SerializeObject(model, Formatting.Indented);
						if (!string.IsNullOrEmpty(text3))
						{
							using (FileStream fileStream = new FileStream(text2, FileMode.OpenOrCreate, FileAccess.Write))
							{
								using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8, 1024, true))
								{
									if (model != null)
									{
										await writer.WriteAsync(text3);
									}
								}
								StreamWriter writer = null;
								fileStream.SetLength(fileStream.Position);
							}
							FileStream fileStream = null;
						}
					}
					catch
					{
					}
				}
			}

			// Token: 0x06002F80 RID: 12160 RVA: 0x000EA6A8 File Offset: 0x000E88A8
			private static List<CopilotHelper.Cache.ChatMessageCacheModel> GetChatMessagesFromCacheCore(string id)
			{
				List<CopilotHelper.Cache.ChatMessageCacheModel> list;
				if (!string.IsNullOrEmpty(id) && ConfigUtils.TryGet<List<CopilotHelper.Cache.ChatMessageCacheModel>>("CopilotChatMessage_" + id, out list) && list != null)
				{
					return list;
				}
				return new List<CopilotHelper.Cache.ChatMessageCacheModel>();
			}

			// Token: 0x06002F81 RID: 12161 RVA: 0x000EA6DB File Offset: 0x000E88DB
			private static void SetChatMessagesToCacheCore(string id, List<CopilotHelper.Cache.ChatMessageCacheModel> messages)
			{
				if (string.IsNullOrEmpty(id))
				{
					return;
				}
				ConfigUtils.TrySet<List<CopilotHelper.Cache.ChatMessageCacheModel>>("CopilotChatMessage_" + id, messages);
			}

			// Token: 0x06002F82 RID: 12162 RVA: 0x000EA6F8 File Offset: 0x000E88F8
			private static string GetSummaryFromCacheCore(string id)
			{
				string text;
				if (!string.IsNullOrEmpty(id) && SqliteUtils.TryGet("CopilotSummaryCache_" + id, out text))
				{
					return text ?? "";
				}
				return "";
			}

			// Token: 0x06002F83 RID: 12163 RVA: 0x000EA731 File Offset: 0x000E8931
			private static void SetSummaryToCacheCore(string id, string summary)
			{
				if (string.IsNullOrEmpty(id))
				{
					return;
				}
				SqliteUtils.TrySet("CopilotSummaryCache_" + id, summary);
			}

			// Token: 0x06002F84 RID: 12164 RVA: 0x000EA750 File Offset: 0x000E8950
			private static int GetChatCountCore(string id)
			{
				string text;
				if (!string.IsNullOrEmpty(id) && SqliteUtils.TryGet("CopilotEditCount_" + id, out text) && !string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[] { '|' });
					long num;
					int num2;
					if (array.Length == 2 && long.TryParse(array[0], out num) && int.TryParse(array[1], out num2))
					{
						DateTimeOffset now = DateTimeOffset.Now;
						DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(num).UtcTicks, now.Offset);
						if (now.Date == dateTimeOffset.Date)
						{
							return num2;
						}
						return 0;
					}
				}
				return 0;
			}

			// Token: 0x06002F85 RID: 12165 RVA: 0x000EA7F4 File Offset: 0x000E89F4
			private static void SetChatCountCore(string id, int count)
			{
				if (string.IsNullOrEmpty(id))
				{
					return;
				}
				string text = "CopilotEditCount_" + id;
				long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				string text2 = string.Format("{0}|{1}", num, count);
				SqliteUtils.TrySet(text, text2);
			}

			// Token: 0x06002F86 RID: 12166 RVA: 0x000EA844 File Offset: 0x000E8A44
			private static string GetCacheFolder()
			{
				string text = Path.Combine(UtilManager.GetLocalCachePath(), "Copilot");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}

			// Token: 0x06002F87 RID: 12167 RVA: 0x000EA874 File Offset: 0x000E8A74
			private static string GetId(string fileName)
			{
				CopilotHelper.Cache.<>c__DisplayClass25_0 CS$<>8__locals1 = new CopilotHelper.Cache.<>c__DisplayClass25_0();
				CS$<>8__locals1.fileName = fileName;
				CopilotHelper.Cache.<>c__DisplayClass25_0 CS$<>8__locals2 = CS$<>8__locals1;
				string fileName2 = CS$<>8__locals1.fileName;
				string text;
				if (fileName2 == null)
				{
					text = null;
				}
				else
				{
					string text2 = fileName2.Trim();
					text = ((text2 != null) ? text2.ToLowerInvariant() : null);
				}
				CS$<>8__locals2.fileName = text;
				if (string.IsNullOrWhiteSpace(CS$<>8__locals1.fileName))
				{
					return null;
				}
				try
				{
					CopilotHelper.Cache.locker.Wait();
					List<CopilotHelper.Cache.CacheModel> list;
					if (ConfigUtils.TryGet<List<CopilotHelper.Cache.CacheModel>>("CopilotAnalyzeCache", out list) && list != null)
					{
						CopilotHelper.Cache.CacheModel cacheModel = list.LastOrDefault((CopilotHelper.Cache.CacheModel c) => c.FileName == CS$<>8__locals1.fileName);
						return (cacheModel != null) ? cacheModel.Id : null;
					}
				}
				finally
				{
					CopilotHelper.Cache.locker.Release();
				}
				return null;
			}

			// Token: 0x06002F88 RID: 12168 RVA: 0x000EA920 File Offset: 0x000E8B20
			private static void SetId(string fileName, string id)
			{
				string text;
				if (fileName == null)
				{
					text = null;
				}
				else
				{
					string text2 = fileName.Trim();
					text = ((text2 != null) ? text2.ToLowerInvariant() : null);
				}
				fileName = text;
				if (string.IsNullOrWhiteSpace(fileName))
				{
					return;
				}
				try
				{
					CopilotHelper.Cache.locker.Wait();
					List<CopilotHelper.Cache.CacheModel> list;
					if (ConfigUtils.TryGet<List<CopilotHelper.Cache.CacheModel>>("CopilotAnalyzeCache", out list) && list != null)
					{
						if (string.IsNullOrEmpty(id))
						{
							for (int i = list.Count - 1; i >= 0; i--)
							{
								if (list[i].Id == id)
								{
									list.RemoveAt(i);
									break;
								}
							}
						}
						else
						{
							bool flag = false;
							for (int j = 0; j < list.Count; j++)
							{
								if (list[j].FileName == fileName)
								{
									flag = true;
									list[j].Id = id;
								}
							}
							if (!flag)
							{
								list.Add(new CopilotHelper.Cache.CacheModel
								{
									FileName = fileName,
									Id = id
								});
							}
						}
						ConfigUtils.TrySet<List<CopilotHelper.Cache.CacheModel>>("CopilotAnalyzeCache", list);
					}
					else if (!string.IsNullOrEmpty(id))
					{
						ConfigUtils.TrySet<List<CopilotHelper.Cache.CacheModel>>("CopilotAnalyzeCache", new List<CopilotHelper.Cache.CacheModel>
						{
							new CopilotHelper.Cache.CacheModel
							{
								FileName = fileName,
								Id = id
							}
						});
					}
				}
				finally
				{
					CopilotHelper.Cache.locker.Release();
				}
			}

			// Token: 0x04001C08 RID: 7176
			private const string CacheKey = "CopilotAnalyzeCache";

			// Token: 0x04001C09 RID: 7177
			private const string ChatMessagesCacheTemplate = "CopilotChatMessage_";

			// Token: 0x04001C0A RID: 7178
			private const string SummaryCacheTemplate = "CopilotSummaryCache_";

			// Token: 0x04001C0B RID: 7179
			private const string EditCountTemplate = "CopilotEditCount_";

			// Token: 0x04001C0C RID: 7180
			private static SemaphoreSlim locker = new SemaphoreSlim(1, 1);

			// Token: 0x0200080F RID: 2063
			private class CacheModel
			{
				// Token: 0x17000DB2 RID: 3506
				// (get) Token: 0x0600386C RID: 14444 RVA: 0x0012767E File Offset: 0x0012587E
				// (set) Token: 0x0600386D RID: 14445 RVA: 0x00127686 File Offset: 0x00125886
				public string FileName { get; set; }

				// Token: 0x17000DB3 RID: 3507
				// (get) Token: 0x0600386E RID: 14446 RVA: 0x0012768F File Offset: 0x0012588F
				// (set) Token: 0x0600386F RID: 14447 RVA: 0x00127697 File Offset: 0x00125897
				public string Id { get; set; }
			}

			// Token: 0x02000810 RID: 2064
			private class ChatMessageCacheModel
			{
				// Token: 0x06003871 RID: 14449 RVA: 0x001276A8 File Offset: 0x001258A8
				public ChatMessageCacheModel()
				{
				}

				// Token: 0x06003872 RID: 14450 RVA: 0x001276B0 File Offset: 0x001258B0
				public ChatMessageCacheModel(CopilotHelper.ChatMessage message)
				{
					this.Role = message.Role;
					this.Content = message.Content;
					this.Pages = message.Pages;
					this.Liked = message.Liked;
				}

				// Token: 0x17000DB4 RID: 3508
				// (get) Token: 0x06003873 RID: 14451 RVA: 0x001276E8 File Offset: 0x001258E8
				// (set) Token: 0x06003874 RID: 14452 RVA: 0x001276F0 File Offset: 0x001258F0
				[JsonProperty("role")]
				public string Role { get; set; }

				// Token: 0x17000DB5 RID: 3509
				// (get) Token: 0x06003875 RID: 14453 RVA: 0x001276F9 File Offset: 0x001258F9
				// (set) Token: 0x06003876 RID: 14454 RVA: 0x00127701 File Offset: 0x00125901
				[JsonProperty("content")]
				public string Content { get; set; }

				// Token: 0x17000DB6 RID: 3510
				// (get) Token: 0x06003877 RID: 14455 RVA: 0x0012770A File Offset: 0x0012590A
				// (set) Token: 0x06003878 RID: 14456 RVA: 0x00127712 File Offset: 0x00125912
				[JsonProperty("pages")]
				public int[] Pages { get; set; }

				// Token: 0x17000DB7 RID: 3511
				// (get) Token: 0x06003879 RID: 14457 RVA: 0x0012771B File Offset: 0x0012591B
				// (set) Token: 0x0600387A RID: 14458 RVA: 0x00127723 File Offset: 0x00125923
				[JsonProperty("like")]
				public string Liked { get; set; }

				// Token: 0x0600387B RID: 14459 RVA: 0x0012772C File Offset: 0x0012592C
				public CopilotHelper.ChatMessage ToChatMessage()
				{
					return new CopilotHelper.ChatMessage
					{
						Role = this.Role,
						Content = this.Content,
						Pages = this.Pages,
						Liked = this.Liked
					};
				}
			}
		}

		// Token: 0x02000505 RID: 1285
		private class InternalCopilotHelper
		{
			// Token: 0x17000CD0 RID: 3280
			// (get) Token: 0x06002F8A RID: 12170 RVA: 0x000EAA6A File Offset: 0x000E8C6A
			private static HttpClient HttpClient
			{
				get
				{
					if (CopilotHelper.InternalCopilotHelper.httpClient == null)
					{
						CopilotHelper.InternalCopilotHelper.httpClient = new HttpClient
						{
							Timeout = TimeSpan.FromSeconds(120.0),
							BaseAddress = new Uri("https://chatapi.pdfgear.com")
						};
					}
					return CopilotHelper.InternalCopilotHelper.httpClient;
				}
			}

			// Token: 0x06002F8B RID: 12171 RVA: 0x000EAAA8 File Offset: 0x000E8CA8
			public static async Task<CopilotHelper.AnalyzeResponseModel> AnalyzeAsync(string text, CancellationToken cancellationToken)
			{
				CopilotHelper.AnalyzeRequestModel analyzeRequestModel = new CopilotHelper.AnalyzeRequestModel
				{
					Text = text,
					User = UtilManager.GetUUID()
				};
				HttpResponseMessage httpResponseMessage = await CopilotHelper.InternalCopilotHelper.HttpClient.PostAsync("/pdf/analyze", CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.AnalyzeRequestModel>(analyzeRequestModel), cancellationToken);
				httpResponseMessage.EnsureSuccessStatusCode();
				return JsonConvert.DeserializeObject<CopilotHelper.ResponseResult<CopilotHelper.AnalyzeResponseModel>>(await httpResponseMessage.Content.ReadAsStringAsync()).Content;
			}

			// Token: 0x06002F8C RID: 12172 RVA: 0x000EAAF4 File Offset: 0x000E8CF4
			public static async Task<CopilotHelper.StreamRequestResult> GetSummaryAsync(CopilotHelper.PdfModel pdf, bool stream, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
			{
				CopilotHelper.ChatRequestModel chatRequestModel = new CopilotHelper.ChatRequestModel
				{
					Pdf = pdf,
					Stream = stream,
					User = UtilManager.GetUUID(),
					Language = CopilotHelper.InternalCopilotHelper.GetLanguage()
				};
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/pdf/getsummary")
				{
					Content = CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.ChatRequestModel>(chatRequestModel)
				};
				return await CopilotHelper.InternalCopilotHelper.ProcessStreamResponse(await CopilotHelper.InternalCopilotHelper.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken), action, cancellationToken).ConfigureAwait(false);
			}

			// Token: 0x06002F8D RID: 12173 RVA: 0x000EAB50 File Offset: 0x000E8D50
			public static async Task<CopilotHelper.StreamRequestResult> ChatAsync(List<CopilotHelper.ChatMessage> messages, CopilotHelper.PdfModel pdf, bool stream, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
			{
				CopilotHelper.ChatRequestModel chatRequestModel = new CopilotHelper.ChatRequestModel
				{
					Messages = messages.ToArray(),
					Pdf = pdf,
					Stream = stream,
					User = UtilManager.GetUUID(),
					Language = CopilotHelper.InternalCopilotHelper.GetLanguage()
				};
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/pdf/chat")
				{
					Content = CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.ChatRequestModel>(chatRequestModel)
				};
				return await CopilotHelper.InternalCopilotHelper.ProcessStreamResponse(await CopilotHelper.InternalCopilotHelper.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken), action, cancellationToken).ConfigureAwait(false);
			}

			// Token: 0x06002F8E RID: 12174 RVA: 0x000EABB4 File Offset: 0x000E8DB4
			public static async Task<CopilotHelper.AppActionModel> GetActionAsync(string input, CopilotHelper.ActionModel[] actionList, CancellationToken cancellationToken)
			{
				CopilotHelper.AppActionModel appActionModel;
				if (string.IsNullOrEmpty(input))
				{
					appActionModel = null;
				}
				else if (actionList == null || actionList.Length == 0)
				{
					appActionModel = null;
				}
				else
				{
					CopilotHelper.GetActionRequestModel getActionRequestModel = new CopilotHelper.GetActionRequestModel();
					getActionRequestModel.Input = input;
					getActionRequestModel.Actions = actionList.Select((CopilotHelper.ActionModel c) => new CopilotHelper.ActionModel
					{
						Description = c.Description,
						Examples = c.Examples,
						Name = c.Name,
						Parameters = c.Parameters
					}).ToArray<CopilotHelper.ActionModel>();
					getActionRequestModel.User = UtilManager.GetUUID();
					CopilotHelper.GetActionRequestModel getActionRequestModel2 = getActionRequestModel;
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/appaction/getaction")
					{
						Content = CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.GetActionRequestModel>(getActionRequestModel2)
					};
					HttpResponseMessage httpResponseMessage = await CopilotHelper.InternalCopilotHelper.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
					httpResponseMessage.EnsureSuccessStatusCode();
					string text = await httpResponseMessage.Content.ReadAsStringAsync();
					try
					{
						CopilotHelper.ResponseResult<CopilotHelper.AppActionModel> responseResult = JsonConvert.DeserializeObject<CopilotHelper.ResponseResult<CopilotHelper.AppActionModel>>(text);
						if (responseResult != null && responseResult.Success)
						{
							return responseResult.Content;
						}
					}
					catch (Exception ex)
					{
						Log.WriteLog(string.Format("Json: {0},\nException: {1}", text, ex));
					}
					appActionModel = null;
				}
				return appActionModel;
			}

			// Token: 0x06002F8F RID: 12175 RVA: 0x000EAC08 File Offset: 0x000E8E08
			public static async Task<CopilotHelper.StreamRequestResult> DoTranslateAsync(string actionName, CopilotHelper.TranslateRequestModel model, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
			{
				CopilotHelper.StreamRequestResult streamRequestResult;
				if (string.IsNullOrEmpty(actionName) || model == null)
				{
					streamRequestResult = CopilotHelper.StreamRequestResult.CreateFailed("unknown");
				}
				else
				{
					model.User = UtilManager.GetUUID();
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/appaction/" + actionName)
					{
						Content = CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.TranslateRequestModel>(model)
					};
					streamRequestResult = await CopilotHelper.InternalCopilotHelper.ProcessStreamResponse(await CopilotHelper.InternalCopilotHelper.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken), action, cancellationToken).ConfigureAwait(false);
				}
				return streamRequestResult;
			}

			// Token: 0x06002F90 RID: 12176 RVA: 0x000EAC64 File Offset: 0x000E8E64
			public static async Task<CopilotHelper.StreamRequestResult> DoAppActionAsync(string actionName, CopilotHelper.DoActionRequestModel model, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
			{
				CopilotHelper.StreamRequestResult streamRequestResult;
				if (string.IsNullOrEmpty(actionName) || model == null)
				{
					streamRequestResult = CopilotHelper.StreamRequestResult.CreateFailed("unknown");
				}
				else
				{
					model.User = UtilManager.GetUUID();
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/appaction/" + actionName)
					{
						Content = CopilotHelper.InternalCopilotHelper.BuildJsonContent<CopilotHelper.DoActionRequestModel>(model)
					};
					streamRequestResult = await CopilotHelper.InternalCopilotHelper.ProcessStreamResponse(await CopilotHelper.InternalCopilotHelper.HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken), action, cancellationToken).ConfigureAwait(false);
				}
				return streamRequestResult;
			}

			// Token: 0x06002F91 RID: 12177 RVA: 0x000EACBF File Offset: 0x000E8EBF
			private static string GetLanguage()
			{
				if (CultureInfo.DefaultThreadCurrentUICulture != null)
				{
					return CultureInfo.DefaultThreadCurrentUICulture.Name;
				}
				if (CultureInfo.CurrentUICulture != null)
				{
					return CultureInfo.CurrentUICulture.Name;
				}
				return "";
			}

			// Token: 0x06002F92 RID: 12178 RVA: 0x000EACEC File Offset: 0x000E8EEC
			private static async Task<CopilotHelper.StreamRequestResult> ProcessStreamResponse(HttpResponseMessage response, Func<string, CancellationToken, Task> action, CancellationToken cancellationToken)
			{
				response.EnsureSuccessStatusCode();
				int textLen = 0;
				CopilotHelper.InternalCopilotHelper.<>c__DisplayClass11_0 CS$<>8__locals1 = new CopilotHelper.InternalCopilotHelper.<>c__DisplayClass11_0();
				Stream stream = await response.Content.ReadAsStreamAsync();
				CS$<>8__locals1.responseStream = stream;
				try
				{
					using (StreamReader reader = new StreamReader(CS$<>8__locals1.responseStream, Encoding.UTF8, true, 1024, true))
					{
						string text = await reader.ReadLineAsync();
						using (cancellationToken.Register(delegate
						{
							try
							{
								CS$<>8__locals1.responseStream.Dispose();
							}
							catch
							{
							}
						}))
						{
							try
							{
								while (text != null)
								{
									CopilotHelper.StreamResult streamResult = null;
									try
									{
										streamResult = JsonConvert.DeserializeObject<CopilotHelper.StreamResult>(text);
									}
									catch
									{
									}
									if (streamResult == null)
									{
										break;
									}
									if (!string.IsNullOrEmpty(streamResult.Error))
									{
										if (!(streamResult.Error == "end"))
										{
											return CopilotHelper.StreamRequestResult.CreateFailed(streamResult.Error);
										}
										if (streamResult.Success == 1)
										{
											int num = textLen;
											string text2 = streamResult.Text;
											textLen = num + ((text2 != null) ? text2.Length : 0);
											if (!string.IsNullOrEmpty(streamResult.Text))
											{
												await action(streamResult.Text, cancellationToken);
											}
										}
										if (textLen == 0)
										{
											return CopilotHelper.StreamRequestResult.CreateFailed("unknown");
										}
										return CopilotHelper.StreamRequestResult.CreateSuccess();
									}
									else if (streamResult.Success == 1)
									{
										int num2 = textLen;
										string text3 = streamResult.Text;
										textLen = num2 + ((text3 != null) ? text3.Length : 0);
										if (!string.IsNullOrEmpty(streamResult.Text))
										{
											await action(streamResult.Text, cancellationToken);
										}
										text = await reader.ReadLineAsync();
									}
									else
									{
										if (streamResult.Text == "end" && textLen != 0)
										{
											return CopilotHelper.StreamRequestResult.CreateSuccess();
										}
										return CopilotHelper.StreamRequestResult.CreateFailed("unknown");
									}
								}
							}
							catch (ObjectDisposedException ex) when (cancellationToken.IsCancellationRequested)
							{
								throw new OperationCanceledException(ex.Message, ex, cancellationToken);
							}
						}
						CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
					}
					StreamReader reader = null;
				}
				finally
				{
					if (CS$<>8__locals1.responseStream != null)
					{
						((IDisposable)CS$<>8__locals1.responseStream).Dispose();
					}
				}
				CS$<>8__locals1 = null;
				return CopilotHelper.StreamRequestResult.CreateFailed("unknown");
			}

			// Token: 0x06002F93 RID: 12179 RVA: 0x000EAD3F File Offset: 0x000E8F3F
			private static HttpContent BuildJsonContent<T>(T model)
			{
				if (!object.Equals(model, null))
				{
					return new StringContent(JsonConvert.SerializeObject(model, Formatting.None), Encoding.UTF8, "application/json");
				}
				return new StringContent("{}", Encoding.UTF8, "application/json");
			}

			// Token: 0x04001C0D RID: 7181
			private const string baseUri = "https://chatapi.pdfgear.com";

			// Token: 0x04001C0E RID: 7182
			private static HttpClient httpClient;
		}

		// Token: 0x02000506 RID: 1286
		private class SimpleCosineSimilarityFloatVersion
		{
			// Token: 0x06002F95 RID: 12181 RVA: 0x000EAD88 File Offset: 0x000E8F88
			public static float[][] ComputeDistances(float[][] dataSet, bool useMultipleThread = false, int maxDegreeOfParallelism = 0)
			{
				int numPoints = dataSet.Length;
				float[][] distances = new float[numPoints][];
				for (int i = 0; i < distances.Length; i++)
				{
					distances[i] = new float[numPoints];
				}
				if (useMultipleThread)
				{
					int num = numPoints * numPoints;
					if (maxDegreeOfParallelism == 0)
					{
						maxDegreeOfParallelism = Environment.ProcessorCount;
					}
					ParallelOptions parallelOptions = new ParallelOptions
					{
						MaxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism)
					};
					Parallel.For(0, num, parallelOptions, delegate(int index)
					{
						int num3 = index % numPoints;
						int num4 = index / numPoints;
						if (num3 < num4)
						{
							float num5 = CopilotHelper.SimpleCosineSimilarityFloatVersion.ComputeDistance(dataSet[num3], dataSet[num4]);
							distances[num3][num4] = num5;
							distances[num4][num3] = num5;
						}
					});
				}
				else
				{
					for (int j = 0; j < numPoints; j++)
					{
						for (int k = 0; k < j; k++)
						{
							float num2 = CopilotHelper.SimpleCosineSimilarityFloatVersion.ComputeDistance(dataSet[j], dataSet[k]);
							distances[j][k] = num2;
							distances[k][j] = num2;
						}
					}
				}
				return distances;
			}

			// Token: 0x06002F96 RID: 12182 RVA: 0x000EAE90 File Offset: 0x000E9090
			public static float ComputeDistance(float[] attributesOne, float[] attributesTwo)
			{
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				int i = 0;
				if (global::System.Numerics.Vector.IsHardwareAccelerated)
				{
					int count = Vector<float>.Count;
					int num4 = attributesOne.Length / count * count;
					for (i = 0; i < num4; i += count)
					{
						Vector<float> vector = new Vector<float>(attributesOne, i);
						Vector<float> vector2 = new Vector<float>(attributesTwo, i);
						num += (double)global::System.Numerics.Vector.Dot<float>(vector, vector2);
						num2 += (double)global::System.Numerics.Vector.Dot<float>(vector, vector);
						num3 += (double)global::System.Numerics.Vector.Dot<float>(vector2, vector2);
					}
				}
				while (i < attributesOne.Length)
				{
					num += (double)(attributesOne[i] * attributesTwo[i]);
					num2 += (double)(attributesOne[i] * attributesOne[i]);
					num3 += (double)(attributesTwo[i] * attributesTwo[i]);
					i++;
				}
				return (float)Math.Max(0.0, 1.0 - num / Math.Sqrt(num2 * num3));
			}
		}

		// Token: 0x02000507 RID: 1287
		private class ResponseResult<T>
		{
			// Token: 0x17000CD1 RID: 3281
			// (get) Token: 0x06002F98 RID: 12184 RVA: 0x000EAF78 File Offset: 0x000E9178
			// (set) Token: 0x06002F99 RID: 12185 RVA: 0x000EAF80 File Offset: 0x000E9180
			public T Content { get; set; }

			// Token: 0x17000CD2 RID: 3282
			// (get) Token: 0x06002F9A RID: 12186 RVA: 0x000EAF89 File Offset: 0x000E9189
			// (set) Token: 0x06002F9B RID: 12187 RVA: 0x000EAF91 File Offset: 0x000E9191
			public bool Success { get; set; }

			// Token: 0x17000CD3 RID: 3283
			// (get) Token: 0x06002F9C RID: 12188 RVA: 0x000EAF9A File Offset: 0x000E919A
			// (set) Token: 0x06002F9D RID: 12189 RVA: 0x000EAFA2 File Offset: 0x000E91A2
			public string Messaage { get; set; } = "";
		}

		// Token: 0x02000508 RID: 1288
		public class StreamResult
		{
			// Token: 0x17000CD4 RID: 3284
			// (get) Token: 0x06002F9F RID: 12191 RVA: 0x000EAFBE File Offset: 0x000E91BE
			// (set) Token: 0x06002FA0 RID: 12192 RVA: 0x000EAFC6 File Offset: 0x000E91C6
			[JsonProperty("s")]
			public int Success { get; set; }

			// Token: 0x17000CD5 RID: 3285
			// (get) Token: 0x06002FA1 RID: 12193 RVA: 0x000EAFCF File Offset: 0x000E91CF
			// (set) Token: 0x06002FA2 RID: 12194 RVA: 0x000EAFD7 File Offset: 0x000E91D7
			[JsonProperty("t")]
			public string Text { get; set; }

			// Token: 0x17000CD6 RID: 3286
			// (get) Token: 0x06002FA3 RID: 12195 RVA: 0x000EAFE0 File Offset: 0x000E91E0
			// (set) Token: 0x06002FA4 RID: 12196 RVA: 0x000EAFE8 File Offset: 0x000E91E8
			[JsonProperty("e")]
			public string Error { get; set; }
		}

		// Token: 0x02000509 RID: 1289
		public class StreamRequestResult
		{
			// Token: 0x06002FA6 RID: 12198 RVA: 0x000EAFF9 File Offset: 0x000E91F9
			public StreamRequestResult(string error)
			{
				this.Error = error;
			}

			// Token: 0x17000CD7 RID: 3287
			// (get) Token: 0x06002FA7 RID: 12199 RVA: 0x000EB008 File Offset: 0x000E9208
			public string Error { get; }

			// Token: 0x17000CD8 RID: 3288
			// (get) Token: 0x06002FA8 RID: 12200 RVA: 0x000EB010 File Offset: 0x000E9210
			public bool Success
			{
				get
				{
					return string.IsNullOrEmpty(this.Error);
				}
			}

			// Token: 0x06002FA9 RID: 12201 RVA: 0x000EB01D File Offset: 0x000E921D
			public static CopilotHelper.StreamRequestResult CreateSuccess()
			{
				return CopilotHelper.StreamRequestResult.successResult;
			}

			// Token: 0x06002FAA RID: 12202 RVA: 0x000EB024 File Offset: 0x000E9224
			public static CopilotHelper.StreamRequestResult CreateFailed(string error = "unknown")
			{
				return new CopilotHelper.StreamRequestResult((!string.IsNullOrEmpty(error)) ? error : "unknown");
			}

			// Token: 0x04001C15 RID: 7189
			private static CopilotHelper.StreamRequestResult successResult = new CopilotHelper.StreamRequestResult(null);
		}

		// Token: 0x0200050A RID: 1290
		public class AnalyzeRequestModel
		{
			// Token: 0x17000CD9 RID: 3289
			// (get) Token: 0x06002FAC RID: 12204 RVA: 0x000EB048 File Offset: 0x000E9248
			// (set) Token: 0x06002FAD RID: 12205 RVA: 0x000EB050 File Offset: 0x000E9250
			[JsonProperty("user")]
			public string User { get; set; }

			// Token: 0x17000CDA RID: 3290
			// (get) Token: 0x06002FAE RID: 12206 RVA: 0x000EB059 File Offset: 0x000E9259
			// (set) Token: 0x06002FAF RID: 12207 RVA: 0x000EB061 File Offset: 0x000E9261
			[JsonProperty("text")]
			public string Text { get; set; }
		}

		// Token: 0x0200050B RID: 1291
		public class AnalyzeResponseModel
		{
			// Token: 0x17000CDB RID: 3291
			// (get) Token: 0x06002FB1 RID: 12209 RVA: 0x000EB072 File Offset: 0x000E9272
			// (set) Token: 0x06002FB2 RID: 12210 RVA: 0x000EB07A File Offset: 0x000E927A
			[JsonProperty("items")]
			public List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseItemModel> Items { get; set; }

			// Token: 0x02000821 RID: 2081
			public class AnalyzeResponseItemModel
			{
				// Token: 0x17000DB8 RID: 3512
				// (get) Token: 0x060038A0 RID: 14496 RVA: 0x00128E9F File Offset: 0x0012709F
				// (set) Token: 0x060038A1 RID: 14497 RVA: 0x00128EA7 File Offset: 0x001270A7
				[JsonProperty("usage")]
				public int Usage { get; set; }

				// Token: 0x17000DB9 RID: 3513
				// (get) Token: 0x060038A2 RID: 14498 RVA: 0x00128EB0 File Offset: 0x001270B0
				// (set) Token: 0x060038A3 RID: 14499 RVA: 0x00128EB8 File Offset: 0x001270B8
				[JsonProperty("textIndex")]
				public int TextIndex { get; set; }

				// Token: 0x17000DBA RID: 3514
				// (get) Token: 0x060038A4 RID: 14500 RVA: 0x00128EC1 File Offset: 0x001270C1
				// (set) Token: 0x060038A5 RID: 14501 RVA: 0x00128EC9 File Offset: 0x001270C9
				[JsonProperty("textLength")]
				public int TextLength { get; set; }

				// Token: 0x17000DBB RID: 3515
				// (get) Token: 0x060038A6 RID: 14502 RVA: 0x00128ED2 File Offset: 0x001270D2
				// (set) Token: 0x060038A7 RID: 14503 RVA: 0x00128EDA File Offset: 0x001270DA
				[JsonProperty("data")]
				public List<CopilotHelper.AnalyzeResponseModel.AnalyzeResponseDataModel> Data { get; set; }
			}

			// Token: 0x02000822 RID: 2082
			public class AnalyzeResponseDataModel
			{
				// Token: 0x17000DBC RID: 3516
				// (get) Token: 0x060038A9 RID: 14505 RVA: 0x00128EEB File Offset: 0x001270EB
				// (set) Token: 0x060038AA RID: 14506 RVA: 0x00128EF3 File Offset: 0x001270F3
				[JsonProperty("values")]
				public float[] Values { get; set; }
			}
		}

		// Token: 0x0200050C RID: 1292
		public class PdfModel
		{
			// Token: 0x17000CDC RID: 3292
			// (get) Token: 0x06002FB4 RID: 12212 RVA: 0x000EB08B File Offset: 0x000E928B
			// (set) Token: 0x06002FB5 RID: 12213 RVA: 0x000EB093 File Offset: 0x000E9293
			[JsonProperty("fileName")]
			public string FileName { get; set; }

			// Token: 0x17000CDD RID: 3293
			// (get) Token: 0x06002FB6 RID: 12214 RVA: 0x000EB09C File Offset: 0x000E929C
			// (set) Token: 0x06002FB7 RID: 12215 RVA: 0x000EB0A4 File Offset: 0x000E92A4
			[JsonProperty("pageCount")]
			public int PageCount { get; set; }

			// Token: 0x17000CDE RID: 3294
			// (get) Token: 0x06002FB8 RID: 12216 RVA: 0x000EB0AD File Offset: 0x000E92AD
			// (set) Token: 0x06002FB9 RID: 12217 RVA: 0x000EB0B5 File Offset: 0x000E92B5
			[JsonProperty("pages")]
			public List<CopilotHelper.PdfPageModel> Pages { get; set; }
		}

		// Token: 0x0200050D RID: 1293
		public class PdfPageModel
		{
			// Token: 0x17000CDF RID: 3295
			// (get) Token: 0x06002FBB RID: 12219 RVA: 0x000EB0C6 File Offset: 0x000E92C6
			// (set) Token: 0x06002FBC RID: 12220 RVA: 0x000EB0CE File Offset: 0x000E92CE
			[JsonProperty("pageIndex")]
			public int PageIndex { get; set; }

			// Token: 0x17000CE0 RID: 3296
			// (get) Token: 0x06002FBD RID: 12221 RVA: 0x000EB0D7 File Offset: 0x000E92D7
			// (set) Token: 0x06002FBE RID: 12222 RVA: 0x000EB0DF File Offset: 0x000E92DF
			[JsonProperty("content")]
			public string Content { get; set; }
		}

		// Token: 0x0200050E RID: 1294
		public class ChatMessage
		{
			// Token: 0x17000CE1 RID: 3297
			// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x000EB0F0 File Offset: 0x000E92F0
			// (set) Token: 0x06002FC1 RID: 12225 RVA: 0x000EB0F8 File Offset: 0x000E92F8
			[JsonProperty("role")]
			public string Role { get; set; }

			// Token: 0x17000CE2 RID: 3298
			// (get) Token: 0x06002FC2 RID: 12226 RVA: 0x000EB101 File Offset: 0x000E9301
			// (set) Token: 0x06002FC3 RID: 12227 RVA: 0x000EB109 File Offset: 0x000E9309
			[JsonProperty("content")]
			public string Content { get; set; }

			// Token: 0x17000CE3 RID: 3299
			// (get) Token: 0x06002FC4 RID: 12228 RVA: 0x000EB112 File Offset: 0x000E9312
			// (set) Token: 0x06002FC5 RID: 12229 RVA: 0x000EB11A File Offset: 0x000E931A
			[JsonIgnore]
			public int[] Pages { get; set; }

			// Token: 0x17000CE4 RID: 3300
			// (get) Token: 0x06002FC6 RID: 12230 RVA: 0x000EB123 File Offset: 0x000E9323
			// (set) Token: 0x06002FC7 RID: 12231 RVA: 0x000EB12B File Offset: 0x000E932B
			[JsonProperty("like")]
			public string Liked { get; set; }
		}

		// Token: 0x0200050F RID: 1295
		public class ChatRequestModel
		{
			// Token: 0x17000CE5 RID: 3301
			// (get) Token: 0x06002FC9 RID: 12233 RVA: 0x000EB13C File Offset: 0x000E933C
			// (set) Token: 0x06002FCA RID: 12234 RVA: 0x000EB144 File Offset: 0x000E9344
			[JsonProperty("user")]
			public string User { get; set; }

			// Token: 0x17000CE6 RID: 3302
			// (get) Token: 0x06002FCB RID: 12235 RVA: 0x000EB14D File Offset: 0x000E934D
			// (set) Token: 0x06002FCC RID: 12236 RVA: 0x000EB155 File Offset: 0x000E9355
			[JsonProperty("messages")]
			public CopilotHelper.ChatMessage[] Messages { get; set; }

			// Token: 0x17000CE7 RID: 3303
			// (get) Token: 0x06002FCD RID: 12237 RVA: 0x000EB15E File Offset: 0x000E935E
			// (set) Token: 0x06002FCE RID: 12238 RVA: 0x000EB166 File Offset: 0x000E9366
			[JsonProperty("pdf")]
			public CopilotHelper.PdfModel Pdf { get; set; }

			// Token: 0x17000CE8 RID: 3304
			// (get) Token: 0x06002FCF RID: 12239 RVA: 0x000EB16F File Offset: 0x000E936F
			// (set) Token: 0x06002FD0 RID: 12240 RVA: 0x000EB177 File Offset: 0x000E9377
			[JsonProperty("stream")]
			public bool Stream { get; set; }

			// Token: 0x17000CE9 RID: 3305
			// (get) Token: 0x06002FD1 RID: 12241 RVA: 0x000EB180 File Offset: 0x000E9380
			// (set) Token: 0x06002FD2 RID: 12242 RVA: 0x000EB188 File Offset: 0x000E9388
			[JsonProperty("language")]
			public string Language { get; set; }
		}

		// Token: 0x02000510 RID: 1296
		public class CopilotResult
		{
			// Token: 0x06002FD4 RID: 12244 RVA: 0x000EB199 File Offset: 0x000E9399
			public CopilotResult(int[] pages, string text, CopilotHelper.AppActionModel appAction, CopilotHelper.ChatResultError error, bool maybeNotAppAction)
			{
				this.Pages = pages;
				this.Text = text;
				this.AppAction = appAction;
				this.Error = error;
				this.MaybeNotAppAction = maybeNotAppAction;
			}

			// Token: 0x06002FD5 RID: 12245 RVA: 0x000EB1C6 File Offset: 0x000E93C6
			public CopilotResult(int[] pages, string text, CopilotHelper.AppActionModel appAction, string error, bool maybeNotAppAction)
				: this(pages, text, appAction, CopilotHelper.CopilotResult.ErrorToEnum(error), maybeNotAppAction)
			{
			}

			// Token: 0x06002FD6 RID: 12246 RVA: 0x000EB1DA File Offset: 0x000E93DA
			public CopilotResult(int[] pages, string text, CopilotHelper.ChatResultError error)
				: this(pages, text, null, error, true)
			{
			}

			// Token: 0x06002FD7 RID: 12247 RVA: 0x000EB1E7 File Offset: 0x000E93E7
			public CopilotResult(int[] pages, string text, string error)
				: this(pages, text, null, CopilotHelper.CopilotResult.ErrorToEnum(error), true)
			{
			}

			// Token: 0x17000CEA RID: 3306
			// (get) Token: 0x06002FD8 RID: 12248 RVA: 0x000EB1F9 File Offset: 0x000E93F9
			public int[] Pages { get; }

			// Token: 0x17000CEB RID: 3307
			// (get) Token: 0x06002FD9 RID: 12249 RVA: 0x000EB201 File Offset: 0x000E9401
			public string Text { get; }

			// Token: 0x17000CEC RID: 3308
			// (get) Token: 0x06002FDA RID: 12250 RVA: 0x000EB209 File Offset: 0x000E9409
			public CopilotHelper.AppActionModel AppAction { get; }

			// Token: 0x17000CED RID: 3309
			// (get) Token: 0x06002FDB RID: 12251 RVA: 0x000EB211 File Offset: 0x000E9411
			public bool MaybeNotAppAction { get; }

			// Token: 0x17000CEE RID: 3310
			// (get) Token: 0x06002FDC RID: 12252 RVA: 0x000EB219 File Offset: 0x000E9419
			public CopilotHelper.ChatResultError Error { get; }

			// Token: 0x17000CEF RID: 3311
			// (get) Token: 0x06002FDD RID: 12253 RVA: 0x000EB221 File Offset: 0x000E9421
			public static CopilotHelper.CopilotResult EmptyUnknownFailed { get; } = new CopilotHelper.CopilotResult(null, null, CopilotHelper.ChatResultError.Unknown);

			// Token: 0x17000CF0 RID: 3312
			// (get) Token: 0x06002FDE RID: 12254 RVA: 0x000EB228 File Offset: 0x000E9428
			public static CopilotHelper.CopilotResult ContentEmptyFailed { get; } = new CopilotHelper.CopilotResult(null, null, CopilotHelper.ChatResultError.ContentEmpty);

			// Token: 0x17000CF1 RID: 3313
			// (get) Token: 0x06002FDF RID: 12255 RVA: 0x000EB22F File Offset: 0x000E942F
			public static CopilotHelper.CopilotResult ChatCountLimitFailed { get; } = new CopilotHelper.CopilotResult(null, null, CopilotHelper.ChatResultError.CountLimit);

			// Token: 0x17000CF2 RID: 3314
			// (get) Token: 0x06002FE0 RID: 12256 RVA: 0x000EB236 File Offset: 0x000E9436
			public static CopilotHelper.CopilotResult UserCanceledFailed { get; } = new CopilotHelper.CopilotResult(null, null, CopilotHelper.ChatResultError.UserCanceled);

			// Token: 0x17000CF3 RID: 3315
			// (get) Token: 0x06002FE1 RID: 12257 RVA: 0x000EB23D File Offset: 0x000E943D
			public static CopilotHelper.CopilotResult EmptyUnknownMaybeAppActionFailed { get; } = new CopilotHelper.CopilotResult(null, null, null, CopilotHelper.ChatResultError.Unknown, true);

			// Token: 0x06002FE2 RID: 12258 RVA: 0x000EB244 File Offset: 0x000E9444
			public static CopilotHelper.ChatResultError ErrorToEnum(string error)
			{
				error = ((error != null) ? error.Trim().ToLowerInvariant() : null);
				if (error == "" || error == null)
				{
					return CopilotHelper.ChatResultError.None;
				}
				if (error == "content_filtered")
				{
					return CopilotHelper.ChatResultError.ContentFiltered;
				}
				if (error == "too_many_request")
				{
					return CopilotHelper.ChatResultError.TooManyRequest;
				}
				if (!(error == "empty"))
				{
					if (!(error == "unknown"))
					{
					}
					return CopilotHelper.ChatResultError.Unknown;
				}
				return CopilotHelper.ChatResultError.ContentEmpty;
			}
		}

		// Token: 0x02000511 RID: 1297
		public enum ChatResultError
		{
			// Token: 0x04001C33 RID: 7219
			None,
			// Token: 0x04001C34 RID: 7220
			Unknown,
			// Token: 0x04001C35 RID: 7221
			TooManyRequest,
			// Token: 0x04001C36 RID: 7222
			AccountBaned,
			// Token: 0x04001C37 RID: 7223
			ResponseEndedPrematurely,
			// Token: 0x04001C38 RID: 7224
			ContentFiltered,
			// Token: 0x04001C39 RID: 7225
			ContentEmpty = 500,
			// Token: 0x04001C3A RID: 7226
			CountLimit,
			// Token: 0x04001C3B RID: 7227
			UserCanceled = 9999
		}

		// Token: 0x02000512 RID: 1298
		public class AppActionModel
		{
			// Token: 0x17000CF4 RID: 3316
			// (get) Token: 0x06002FE4 RID: 12260 RVA: 0x000EB314 File Offset: 0x000E9514
			// (set) Token: 0x06002FE5 RID: 12261 RVA: 0x000EB31C File Offset: 0x000E951C
			[JsonProperty("name")]
			public string Name { get; set; }

			// Token: 0x17000CF5 RID: 3317
			// (get) Token: 0x06002FE6 RID: 12262 RVA: 0x000EB325 File Offset: 0x000E9525
			// (set) Token: 0x06002FE7 RID: 12263 RVA: 0x000EB32D File Offset: 0x000E952D
			[JsonProperty("params")]
			public Dictionary<string, string> Parameters { get; set; }

			// Token: 0x17000CF6 RID: 3318
			// (get) Token: 0x06002FE8 RID: 12264 RVA: 0x000EB338 File Offset: 0x000E9538
			[JsonIgnore]
			public string Confirm
			{
				get
				{
					string text;
					if ((text = this.confirm) == null)
					{
						text = (this.confirm = CopilotHelper.AppActionHelper.GetAppActionConfirm(this));
					}
					return text;
				}
			}

			// Token: 0x04001C3C RID: 7228
			private string confirm;
		}

		// Token: 0x02000513 RID: 1299
		private class ActionModelRoot
		{
			// Token: 0x17000CF7 RID: 3319
			// (get) Token: 0x06002FEA RID: 12266 RVA: 0x000EB366 File Offset: 0x000E9566
			// (set) Token: 0x06002FEB RID: 12267 RVA: 0x000EB36E File Offset: 0x000E956E
			[JsonProperty("actions")]
			public CopilotHelper.ActionModel[] Actions { get; set; }
		}

		// Token: 0x02000514 RID: 1300
		private class ActionModel
		{
			// Token: 0x17000CF8 RID: 3320
			// (get) Token: 0x06002FED RID: 12269 RVA: 0x000EB37F File Offset: 0x000E957F
			// (set) Token: 0x06002FEE RID: 12270 RVA: 0x000EB387 File Offset: 0x000E9587
			[JsonProperty("name")]
			public string Name { get; set; }

			// Token: 0x17000CF9 RID: 3321
			// (get) Token: 0x06002FEF RID: 12271 RVA: 0x000EB390 File Offset: 0x000E9590
			// (set) Token: 0x06002FF0 RID: 12272 RVA: 0x000EB398 File Offset: 0x000E9598
			[JsonProperty("desc")]
			public string Description { get; set; }

			// Token: 0x17000CFA RID: 3322
			// (get) Token: 0x06002FF1 RID: 12273 RVA: 0x000EB3A1 File Offset: 0x000E95A1
			// (set) Token: 0x06002FF2 RID: 12274 RVA: 0x000EB3A9 File Offset: 0x000E95A9
			[JsonProperty("embedding")]
			public float[] Embedding { get; set; }

			// Token: 0x17000CFB RID: 3323
			// (get) Token: 0x06002FF3 RID: 12275 RVA: 0x000EB3B2 File Offset: 0x000E95B2
			// (set) Token: 0x06002FF4 RID: 12276 RVA: 0x000EB3BA File Offset: 0x000E95BA
			[JsonProperty("params")]
			public Dictionary<string, string> Parameters { get; set; }

			// Token: 0x17000CFC RID: 3324
			// (get) Token: 0x06002FF5 RID: 12277 RVA: 0x000EB3C3 File Offset: 0x000E95C3
			// (set) Token: 0x06002FF6 RID: 12278 RVA: 0x000EB3CB File Offset: 0x000E95CB
			[JsonProperty("examples")]
			public CopilotHelper.ExampleModel[] Examples { get; set; }

			// Token: 0x17000CFD RID: 3325
			// (get) Token: 0x06002FF7 RID: 12279 RVA: 0x000EB3D4 File Offset: 0x000E95D4
			// (set) Token: 0x06002FF8 RID: 12280 RVA: 0x000EB3DC File Offset: 0x000E95DC
			[JsonProperty("static")]
			public bool IsStatic { get; set; }

			// Token: 0x17000CFE RID: 3326
			// (get) Token: 0x06002FF9 RID: 12281 RVA: 0x000EB3E5 File Offset: 0x000E95E5
			// (set) Token: 0x06002FFA RID: 12282 RVA: 0x000EB3ED File Offset: 0x000E95ED
			[JsonProperty("confirm")]
			public Dictionary<string, string> Confirm { get; set; }

			// Token: 0x17000CFF RID: 3327
			// (get) Token: 0x06002FFB RID: 12283 RVA: 0x000EB3F6 File Offset: 0x000E95F6
			// (set) Token: 0x06002FFC RID: 12284 RVA: 0x000EB3FE File Offset: 0x000E95FE
			[JsonProperty("disabled")]
			public bool Disabled { get; set; }
		}

		// Token: 0x02000515 RID: 1301
		private class ExampleModel
		{
			// Token: 0x17000D00 RID: 3328
			// (get) Token: 0x06002FFE RID: 12286 RVA: 0x000EB40F File Offset: 0x000E960F
			// (set) Token: 0x06002FFF RID: 12287 RVA: 0x000EB417 File Offset: 0x000E9617
			[JsonProperty("q")]
			public string Question { get; set; }

			// Token: 0x17000D01 RID: 3329
			// (get) Token: 0x06003000 RID: 12288 RVA: 0x000EB420 File Offset: 0x000E9620
			// (set) Token: 0x06003001 RID: 12289 RVA: 0x000EB428 File Offset: 0x000E9628
			[JsonProperty("a")]
			public string Answer { get; set; }
		}

		// Token: 0x02000516 RID: 1302
		private class GetActionRequestModel
		{
			// Token: 0x17000D02 RID: 3330
			// (get) Token: 0x06003003 RID: 12291 RVA: 0x000EB439 File Offset: 0x000E9639
			// (set) Token: 0x06003004 RID: 12292 RVA: 0x000EB441 File Offset: 0x000E9641
			public string Input { get; set; }

			// Token: 0x17000D03 RID: 3331
			// (get) Token: 0x06003005 RID: 12293 RVA: 0x000EB44A File Offset: 0x000E964A
			// (set) Token: 0x06003006 RID: 12294 RVA: 0x000EB452 File Offset: 0x000E9652
			public CopilotHelper.ActionModel[] Actions { get; set; }

			// Token: 0x17000D04 RID: 3332
			// (get) Token: 0x06003007 RID: 12295 RVA: 0x000EB45B File Offset: 0x000E965B
			// (set) Token: 0x06003008 RID: 12296 RVA: 0x000EB463 File Offset: 0x000E9663
			public string User { get; set; }
		}

		// Token: 0x02000517 RID: 1303
		public class DoActionRequestModel
		{
			// Token: 0x17000D05 RID: 3333
			// (get) Token: 0x0600300A RID: 12298 RVA: 0x000EB474 File Offset: 0x000E9674
			// (set) Token: 0x0600300B RID: 12299 RVA: 0x000EB47C File Offset: 0x000E967C
			public string Input { get; set; }

			// Token: 0x17000D06 RID: 3334
			// (get) Token: 0x0600300C RID: 12300 RVA: 0x000EB485 File Offset: 0x000E9685
			// (set) Token: 0x0600300D RID: 12301 RVA: 0x000EB48D File Offset: 0x000E968D
			public string Language { get; set; }

			// Token: 0x17000D07 RID: 3335
			// (get) Token: 0x0600300E RID: 12302 RVA: 0x000EB496 File Offset: 0x000E9696
			// (set) Token: 0x0600300F RID: 12303 RVA: 0x000EB49E File Offset: 0x000E969E
			public string Style { get; set; }

			// Token: 0x17000D08 RID: 3336
			// (get) Token: 0x06003010 RID: 12304 RVA: 0x000EB4A7 File Offset: 0x000E96A7
			// (set) Token: 0x06003011 RID: 12305 RVA: 0x000EB4AF File Offset: 0x000E96AF
			public bool Stream { get; set; }

			// Token: 0x17000D09 RID: 3337
			// (get) Token: 0x06003012 RID: 12306 RVA: 0x000EB4B8 File Offset: 0x000E96B8
			// (set) Token: 0x06003013 RID: 12307 RVA: 0x000EB4C0 File Offset: 0x000E96C0
			public string User { get; set; }
		}

		// Token: 0x02000518 RID: 1304
		public class AppSupportActions
		{
			// Token: 0x04001C52 RID: 7250
			public const string Summarize = "Summarize";

			// Token: 0x04001C53 RID: 7251
			public const string Translate = "Translate";

			// Token: 0x04001C54 RID: 7252
			public const string Rewrite = "Rewrite";
		}

		// Token: 0x02000519 RID: 1305
		public class TranslateRequestModel
		{
			// Token: 0x17000D0A RID: 3338
			// (get) Token: 0x06003016 RID: 12310 RVA: 0x000EB4D9 File Offset: 0x000E96D9
			// (set) Token: 0x06003017 RID: 12311 RVA: 0x000EB4E1 File Offset: 0x000E96E1
			[JsonProperty("input")]
			public string Input { get; set; }

			// Token: 0x17000D0B RID: 3339
			// (get) Token: 0x06003018 RID: 12312 RVA: 0x000EB4EA File Offset: 0x000E96EA
			// (set) Token: 0x06003019 RID: 12313 RVA: 0x000EB4F2 File Offset: 0x000E96F2
			[JsonProperty("sourcelanguage")]
			public string SourceLanguage { get; set; }

			// Token: 0x17000D0C RID: 3340
			// (get) Token: 0x0600301A RID: 12314 RVA: 0x000EB4FB File Offset: 0x000E96FB
			// (set) Token: 0x0600301B RID: 12315 RVA: 0x000EB503 File Offset: 0x000E9703
			[JsonProperty("language")]
			public string Language { get; set; }

			// Token: 0x17000D0D RID: 3341
			// (get) Token: 0x0600301C RID: 12316 RVA: 0x000EB50C File Offset: 0x000E970C
			// (set) Token: 0x0600301D RID: 12317 RVA: 0x000EB514 File Offset: 0x000E9714
			[JsonProperty("style")]
			public string Style { get; set; }

			// Token: 0x17000D0E RID: 3342
			// (get) Token: 0x0600301E RID: 12318 RVA: 0x000EB51D File Offset: 0x000E971D
			// (set) Token: 0x0600301F RID: 12319 RVA: 0x000EB525 File Offset: 0x000E9725
			[JsonProperty("stream")]
			public bool Stream { get; set; }

			// Token: 0x17000D0F RID: 3343
			// (get) Token: 0x06003020 RID: 12320 RVA: 0x000EB52E File Offset: 0x000E972E
			// (set) Token: 0x06003021 RID: 12321 RVA: 0x000EB536 File Offset: 0x000E9736
			[JsonProperty("user")]
			public string User { get; set; }
		}

		// Token: 0x0200051A RID: 1306
		public class TranslateResponseModel
		{
			// Token: 0x17000D10 RID: 3344
			// (get) Token: 0x06003023 RID: 12323 RVA: 0x000EB547 File Offset: 0x000E9747
			// (set) Token: 0x06003024 RID: 12324 RVA: 0x000EB54F File Offset: 0x000E974F
			[JsonProperty("items")]
			public List<CopilotHelper.TranslateResponseModel.TranslateResponseItemModel> Items { get; set; }

			// Token: 0x02000823 RID: 2083
			public class TranslateResponseItemModel
			{
				// Token: 0x17000DBD RID: 3517
				// (get) Token: 0x060038AC RID: 14508 RVA: 0x00128F04 File Offset: 0x00127104
				// (set) Token: 0x060038AD RID: 14509 RVA: 0x00128F0C File Offset: 0x0012710C
				[JsonProperty("usage")]
				public int Usage { get; set; }

				// Token: 0x17000DBE RID: 3518
				// (get) Token: 0x060038AE RID: 14510 RVA: 0x00128F15 File Offset: 0x00127115
				// (set) Token: 0x060038AF RID: 14511 RVA: 0x00128F1D File Offset: 0x0012711D
				[JsonProperty("textIndex")]
				public int TextIndex { get; set; }

				// Token: 0x17000DBF RID: 3519
				// (get) Token: 0x060038B0 RID: 14512 RVA: 0x00128F26 File Offset: 0x00127126
				// (set) Token: 0x060038B1 RID: 14513 RVA: 0x00128F2E File Offset: 0x0012712E
				[JsonProperty("textLength")]
				public int TextLength { get; set; }

				// Token: 0x17000DC0 RID: 3520
				// (get) Token: 0x060038B2 RID: 14514 RVA: 0x00128F37 File Offset: 0x00127137
				// (set) Token: 0x060038B3 RID: 14515 RVA: 0x00128F3F File Offset: 0x0012713F
				[JsonProperty("data")]
				public List<CopilotHelper.TranslateResponseModel.TranslateResponseDataModel> Data { get; set; }
			}

			// Token: 0x02000824 RID: 2084
			public class TranslateResponseDataModel
			{
				// Token: 0x17000DC1 RID: 3521
				// (get) Token: 0x060038B5 RID: 14517 RVA: 0x00128F50 File Offset: 0x00127150
				// (set) Token: 0x060038B6 RID: 14518 RVA: 0x00128F58 File Offset: 0x00127158
				[JsonProperty("values")]
				public float[] Values { get; set; }
			}
		}

		// Token: 0x0200051B RID: 1307
		private class AppActionHelper
		{
			// Token: 0x06003026 RID: 12326 RVA: 0x000EB560 File Offset: 0x000E9760
			[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "action", "maybeNotAppAction" })]
			public static async Task<global::System.ValueTuple<CopilotHelper.AppActionModel, bool>> GetAction(string input, float[] embedding, CancellationToken cancellationToken)
			{
				bool flag = false;
				if (input != null && input.Length < 30)
				{
					flag = true;
				}
				global::System.ValueTuple<CopilotHelper.ActionModel[], bool> valueTuple = await CopilotHelper.AppActionHelper.GetActionsForInput(embedding, flag, cancellationToken);
				CopilotHelper.ActionModel[] actions = valueTuple.Item1;
				bool maybeNotAppAction = valueTuple.Item2;
				CopilotHelper.AppActionModel action = await CopilotHelper.InternalCopilotHelper.GetActionAsync(input, actions, cancellationToken);
				global::System.ValueTuple<CopilotHelper.AppActionModel, bool> valueTuple2;
				if (action == null)
				{
					valueTuple2 = new global::System.ValueTuple<CopilotHelper.AppActionModel, bool>(null, maybeNotAppAction);
				}
				else
				{
					CopilotHelper.ActionModel actionModel = actions.FirstOrDefault((CopilotHelper.ActionModel c) => c.Name == action.Name);
					if (actionModel == null)
					{
						action = null;
					}
					else if (actionModel.Parameters != null)
					{
						if (action.Parameters != null)
						{
							foreach (string text in action.Parameters.Keys)
							{
								if (!actionModel.Parameters.ContainsKey(text))
								{
									action.Parameters.Remove(text);
								}
							}
							if (action.Parameters.Count != actionModel.Parameters.Count)
							{
								action = null;
								goto IL_02BF;
							}
							using (Dictionary<string, string>.Enumerator enumerator2 = action.Parameters.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									KeyValuePair<string, string> keyValuePair = enumerator2.Current;
									if (!CopilotHelper.AppActionHelper.ValidParameter(actionModel.Parameters[keyValuePair.Key], keyValuePair.Value))
									{
										action = null;
										break;
									}
								}
								goto IL_02BF;
							}
						}
						action = null;
					}
					IL_02BF:
					valueTuple2 = new global::System.ValueTuple<CopilotHelper.AppActionModel, bool>(action, maybeNotAppAction);
				}
				return valueTuple2;
			}

			// Token: 0x06003027 RID: 12327 RVA: 0x000EB5B4 File Offset: 0x000E97B4
			public static async Task ProcessAction(CopilotHelper.AppActionModel action)
			{
				MainViewModel vm = Ioc.Default.GetService<MainViewModel>();
				if (vm != null)
				{
					if (vm.Document != null)
					{
						MainView mainView = CopilotHelper.AppActionHelper.GetMainView(vm.Document);
						if (mainView != null)
						{
							string text13;
							if (action.Name == "summary")
							{
								CopilotHelper.AppActionHelper.<>c__DisplayClass3_0 CS$<>8__locals1 = new CopilotHelper.AppActionHelper.<>c__DisplayClass3_0();
								CS$<>8__locals1.chatPanel = mainView.FindName("ChatPanel") as ChatPanel;
								if (CS$<>8__locals1.chatPanel == null)
								{
									return;
								}
								App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
								{
									CopilotHelper.AppActionHelper.<>c__DisplayClass3_0.<<ProcessAction>b__0>d <<ProcessAction>b__0>d;
									<<ProcessAction>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
									<<ProcessAction>b__0>d.<>4__this = CS$<>8__locals1;
									<<ProcessAction>b__0>d.<>1__state = -1;
									<<ProcessAction>b__0>d.<>t__builder.Start<CopilotHelper.AppActionHelper.<>c__DisplayClass3_0.<<ProcessAction>b__0>d>(ref <<ProcessAction>b__0>d);
								}));
							}
							else if (action.Name == "print")
							{
								AsyncRelayCommand printDocCmd = vm.PrintDocCmd;
								if (printDocCmd != null)
								{
									printDocCmd.Execute(null);
								}
							}
							else if (action.Name == "select-page")
							{
								string text;
								int num;
								if (action.Parameters.TryGetValue("page", out text) && int.TryParse(text, out num))
								{
									vm.CurrnetPageIndex = num;
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
								}
							}
							else if (action.Name == "page-display")
							{
								string text2;
								if (action.Parameters.TryGetValue("mode", out text2))
								{
									if (text2 == "actual-size")
									{
										vm.ViewToolbar.DocSizeModeWrap = SizeModesWrap.ZoomActualSize;
									}
									else if (text2 == "fit-page")
									{
										vm.ViewToolbar.DocSizeModeWrap = SizeModesWrap.FitToSize;
									}
									else if (text2 == "fit-width")
									{
										vm.ViewToolbar.DocSizeModeWrap = SizeModesWrap.FitToWidth;
									}
									else if (text2 == "fit-height")
									{
										vm.ViewToolbar.DocSizeModeWrap = SizeModesWrap.FitToHeight;
									}
									else if (text2 == "single-line")
									{
										vm.ViewToolbar.SubViewModePage = SubViewModePage.SinglePage;
									}
									else if (text2 == "double-line")
									{
										vm.ViewToolbar.SubViewModePage = SubViewModePage.DoublePages;
									}
									else if (text2 == "enable-continuous")
									{
										vm.ViewToolbar.SubViewModeContinuous = SubViewModeContinuous.Verticalcontinuous;
									}
									else if (text2 == "disable-continuous")
									{
										vm.ViewToolbar.SubViewModeContinuous = SubViewModeContinuous.Discontinuous;
									}
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
								}
							}
							else if (action.Name == "page-zoom")
							{
								string text3;
								if (action.Parameters.TryGetValue("mode", out text3))
								{
									if (text3 == "zoom-in")
									{
										vm.ViewToolbar.DocZoomIn(false, 0.1f);
									}
									if (text3 == "zoom-out")
									{
										vm.ViewToolbar.DocZoomOut(false, 0.1f);
									}
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
								}
							}
							else if (action.Name == "rotate-page")
							{
								string mode;
								if (action.Parameters.TryGetValue("mode", out mode))
								{
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
										await Task.Delay(500);
									}
									if (mode == "left")
									{
										vm.ViewToolbar.PageRotateLeftCmd.Execute(null);
									}
									if (mode == "right")
									{
										vm.ViewToolbar.PageRotateRightCmd.Execute(null);
									}
								}
								mode = null;
							}
							else if (action.Name == "auto-scroll")
							{
								string text4;
								if (action.Parameters.TryGetValue("mode", out text4))
								{
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
									if (text4 == "on" && !vm.ViewToolbar.AutoScrollButtonModel.IsChecked)
									{
										vm.ViewToolbar.AutoScrollButtonModel.Tap();
									}
									if (text4 == "off")
									{
										vm.ViewToolbar.StopAutoScroll();
									}
								}
							}
							else if (action.Name == "slide-show")
							{
								vm.ViewToolbar.PresentButtonModel.Tap();
							}
							else if (action.Name == "set-background")
							{
								string text5;
								if (action.Parameters.TryGetValue("mode", out text5))
								{
									string text6 = "";
									if (!(text5 == "default"))
									{
										if (!(text5 == "day"))
										{
											if (!(text5 == "night"))
											{
												if (!(text5 == "eye-protection"))
												{
													if (text5 == "yellow-background")
													{
														text6 = "YellowBackground";
													}
												}
												else
												{
													text6 = "EyeProtectionMode";
												}
											}
											else
											{
												text6 = "NightMode";
											}
										}
										else
										{
											text6 = "DayMode";
										}
									}
									else
									{
										text6 = "Default";
									}
									if (!string.IsNullOrEmpty(text6))
									{
										vm.ViewToolbar.SetViewerBackground(text6);
									}
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
								}
							}
							else if (action.Name == "add-annotation")
							{
								if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
								{
									mainView.Menus_SelectItem("Annotate");
									await Task.Delay(500);
								}
								string text7;
								if (action.Parameters.TryGetValue("mode", out text7))
								{
									if (text7 == "highlight-line")
									{
										vm.AnnotationToolbar.HighlightButtonModel.Tap();
									}
									if (text7 == "underline")
									{
										vm.AnnotationToolbar.UnderlineButtonModel.Tap();
									}
									if (text7 == "strike-throught")
									{
										vm.AnnotationToolbar.StrikeButtonModel.Tap();
									}
									if (text7 == "highlight-area")
									{
										vm.AnnotationToolbar.HighlightAreaButtonModel.Tap();
									}
									if (text7 == "line")
									{
										vm.AnnotationToolbar.LineButtonModel.Tap();
									}
									if (text7 == "rectangle")
									{
										vm.AnnotationToolbar.SquareButtonModel.Tap();
									}
									if (text7 == "oval")
									{
										vm.AnnotationToolbar.CircleButtonModel.Tap();
									}
									if (text7 == "ink")
									{
										vm.AnnotationToolbar.InkButtonModel.Tap();
									}
									if (text7 == "textbox")
									{
										vm.AnnotationToolbar.TextBoxButtonModel.Tap();
									}
									if (text7 == "note")
									{
										vm.AnnotationToolbar.NoteButtonModel.Tap();
									}
								}
							}
							else if (action.Name == "show-annotation")
							{
								string text8;
								if (action.Parameters.TryGetValue("mode", out text8))
								{
									if (text8 == "on")
									{
										vm.IsAnnotationVisible = true;
									}
									else if (text8 == "off")
									{
										vm.IsAnnotationVisible = false;
									}
									if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("View");
									}
								}
							}
							else if (action.Name == "edit-text")
							{
								if (!CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
								{
									mainView.Menus_SelectItem("View");
									await Task.Delay(500);
								}
								vm.ViewToolbar.EditDocumentButtomModel.Tap();
							}
							else if (action.Name == "extract-page")
							{
								string mode;
								if (vm.PageEditors.PageEditListItemSource != null && action.Parameters.TryGetValue("page", out mode))
								{
									if (CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("Page");
										await Task.Delay(500);
									}
									int[] array;
									int i;
									if (PdfObjectExtensions.TryParsePageRange(mode, out array, out i))
									{
										vm.PageEditors.PageEditListItemSource.AllItemSelected = new bool?(false);
										int count = vm.PageEditors.PageEditListItemSource.Count;
										foreach (int num2 in array)
										{
											if (num2 >= 0 && num2 < count)
											{
												vm.PageEditors.PageEditListItemSource[num2].Selected = true;
											}
										}
									}
									await vm.PageEditors.PageEditorExtractCmd.ExecuteAsync(null);
								}
								mode = null;
							}
							else if (action.Name == "delete-page")
							{
								string mode;
								if (vm.PageEditors.PageEditListItemSource != null && action.Parameters.TryGetValue("page", out mode))
								{
									if (CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("Page");
										await Task.Delay(500);
									}
									int i;
									int[] array3;
									if (PdfObjectExtensions.TryParsePageRange(mode, out array3, out i))
									{
										vm.PageEditors.PageEditListItemSource.AllItemSelected = new bool?(false);
										int count2 = vm.PageEditors.PageEditListItemSource.Count;
										foreach (int num3 in array3)
										{
											if (num3 >= 0 && num3 < count2)
											{
												vm.PageEditors.PageEditListItemSource[num3].Selected = true;
											}
										}
									}
									await vm.PageEditors.PageEditorDeleteCmd.ExecuteAsync(null);
								}
								mode = null;
							}
							else if (action.Name == "insert-page")
							{
								string mode;
								if (vm.PageEditors.PageEditListItemSource != null && action.Parameters.TryGetValue("mode", out mode))
								{
									if (CopilotHelper.AppActionHelper.IsPdfControlVisible(vm.Document))
									{
										mainView.Menus_SelectItem("Page");
										await Task.Delay(500);
									}
									string text9 = mode;
									if (!(text9 == "blank-page"))
									{
										if (!(text9 == "exist-pdf"))
										{
											if (!(text9 == "word"))
											{
												if (!(text9 == "image"))
												{
													if (!(text9 == "none"))
													{
													}
													vm.PageEditors.InsertPageButtonModel.Tap();
												}
												else
												{
													await vm.PageEditors.PageEditorInsertFromImageCmd.ExecuteAsync(null);
												}
											}
											else
											{
												await vm.PageEditors.PageEditorInsertFromWordCmd.ExecuteAsync(null);
											}
										}
										else
										{
											await vm.PageEditors.PageEditorInsertFromPDFCmd.ExecuteAsync(null);
										}
									}
									else
									{
										await vm.PageEditors.PageEditorInsertBlankCmd.ExecuteAsync(null);
									}
								}
								mode = null;
							}
							else if (action.Name == "crop-page")
							{
								if (vm.PageEditors.PageEditListItemSource.SelectedItems.Count == 0)
								{
									int currnetPageIndex = vm.CurrnetPageIndex;
									vm.PageEditors.PageEditListItemSource[currnetPageIndex].Selected = true;
								}
								vm.PageEditors.CropPageCmd.Execute(null);
							}
							else if (action.Name == "convert-from-pdf")
							{
								string text10;
								if (action.Parameters.TryGetValue("mode", out text10))
								{
									ConvFromPDFType? convFromPDFType = CopilotHelper.AppActionHelper.<ProcessAction>g__MapType|3_1(text10);
									if (convFromPDFType != null)
									{
										AppManager.OpenPDFConverterFromPdf(convFromPDFType.Value, new string[] { vm.DocumentWrapper.DocumentPath });
									}
								}
							}
							else if (action.Name == "convert-to-pdf")
							{
								string text11;
								if (action.Parameters.TryGetValue("mode", out text11))
								{
									ConvToPDFType? convToPDFType = CopilotHelper.AppActionHelper.<ProcessAction>g__MapType|3_2(text11);
									if (convToPDFType != null)
									{
										AppManager.OpenPDFConverterToPdf(convToPDFType.Value, Array.Empty<string>());
									}
								}
							}
							else if (action.Name == "compress")
							{
								string text12;
								if (action.Parameters.TryGetValue("mode", out text12))
								{
									if (text12 == "high")
									{
										vm.ConverterCommands.DoPDFCompress(new ConverterCommands.CompressMode?(ConverterCommands.CompressMode.High));
									}
									else if (text12 == "medium")
									{
										vm.ConverterCommands.DoPDFCompress(new ConverterCommands.CompressMode?(ConverterCommands.CompressMode.Medium));
									}
									else if (text12 == "low")
									{
										vm.ConverterCommands.DoPDFCompress(new ConverterCommands.CompressMode?(ConverterCommands.CompressMode.Low));
									}
									else
									{
										vm.ConverterCommands.DoPDFCompress(null);
									}
									return;
								}
								vm.ConverterCommands.DoPDFCompress(null);
							}
							else if (action.Name == "protect-pdf")
							{
								vm.EncryptCMD.Execute(null);
							}
							else if (action.Name == "remove-password")
							{
								vm.RemovePasswordCMD.Execute(null);
							}
							else if (action.Name == "contact-us")
							{
								vm.FeedBackCmd.Execute(null);
							}
							else if (action.Name == "settings")
							{
								vm.SettingsCmd.Execute(null);
							}
							else if (action.Name == "check-update")
							{
								vm.UpdateCmd.Execute(null);
							}
							else if (action.Name == "about")
							{
								vm.AboutCmd.Execute(null);
							}
							else if (action.Name == "close-pdf")
							{
								mainView.Close();
							}
							else if (action.Name == "find-in-pdf" && action.Parameters.TryGetValue("text", out text13) && !string.IsNullOrWhiteSpace(text13))
							{
								vm.Menus.SearchModel.IsSearchVisible = true;
								vm.Menus.SearchModel.SearchText = text13.Trim();
							}
							await Task.CompletedTask;
						}
					}
				}
			}

			// Token: 0x06003028 RID: 12328 RVA: 0x000EB5F8 File Offset: 0x000E97F8
			private static MainView GetMainView(PdfDocument pdfDocument)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfDocument);
				if (pdfControl != null)
				{
					return Window.GetWindow(pdfControl) as MainView;
				}
				return null;
			}

			// Token: 0x06003029 RID: 12329 RVA: 0x000EB61C File Offset: 0x000E981C
			private static bool IsPdfControlVisible(PdfDocument pdfDocument)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfDocument);
				return pdfControl != null && pdfControl.IsVisible;
			}

			// Token: 0x0600302A RID: 12330 RVA: 0x000EB63C File Offset: 0x000E983C
			private static bool ValidParameter(string type, string value)
			{
				string text;
				if (type == null)
				{
					text = null;
				}
				else
				{
					string text2 = type.Trim();
					text = ((text2 != null) ? text2.ToLowerInvariant() : null);
				}
				type = text;
				if (string.IsNullOrEmpty(type))
				{
					return false;
				}
				if (type == "string")
				{
					return !string.IsNullOrEmpty(value);
				}
				if (type == "int")
				{
					int num;
					return int.TryParse(value, out num);
				}
				if (type == "int-array")
				{
					int num;
					int[] array;
					return PdfObjectExtensions.TryParsePageRange(value, out array, out num);
				}
				if (!type.Contains(" | "))
				{
					return false;
				}
				if (string.IsNullOrEmpty(value) || value.Contains("|"))
				{
					return false;
				}
				return new HashSet<string>((from c in type.Split(new char[] { '|' })
					select c.Trim().ToLowerInvariant()).Distinct<string>()).Contains(value.Trim().ToLowerInvariant());
			}

			// Token: 0x0600302B RID: 12331 RVA: 0x000EB728 File Offset: 0x000E9928
			[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "actions", "maybeNotAppAction" })]
			private static async Task<global::System.ValueTuple<CopilotHelper.ActionModel[], bool>> GetActionsForInput(float[] embedding, bool appendFindAction, CancellationToken cancellationToken)
			{
				CopilotHelper.ActionModel[] array = await CopilotHelper.AppActionHelper.GetAllActions();
				global::System.ValueTuple<CopilotHelper.ActionModel[], bool> valueTuple;
				if (embedding == null || embedding.Length == 0)
				{
					valueTuple = new global::System.ValueTuple<CopilotHelper.ActionModel[], bool>(Array.Empty<CopilotHelper.ActionModel>(), true);
				}
				else
				{
					List<global::System.ValueTuple<CopilotHelper.ActionModel, float>> list = (from c in array
						select new global::System.ValueTuple<CopilotHelper.ActionModel, float>(c, CopilotHelper.SimpleCosineSimilarityFloatVersion.ComputeDistance(c.Embedding, embedding)) into c
						orderby c.Item2
						select c).ToList<global::System.ValueTuple<CopilotHelper.ActionModel, float>>();
					IEnumerable<CopilotHelper.ActionModel> enumerable = array.Where((CopilotHelper.ActionModel c) => c.IsStatic);
					List<CopilotHelper.ActionModel> list2 = new List<CopilotHelper.ActionModel>();
					bool flag = true;
					foreach (global::System.ValueTuple<CopilotHelper.ActionModel, float> valueTuple2 in list)
					{
						if ((double)valueTuple2.Item2 < 0.08)
						{
							flag = false;
						}
						if (valueTuple2.Item1.Confirm != null && valueTuple2.Item1.Confirm.Count > 0)
						{
							if ((double)valueTuple2.Item2 < 0.3)
							{
								list2.Add(valueTuple2.Item1);
							}
						}
						else if ((double)valueTuple2.Item2 < 0.15)
						{
							list2.Add(valueTuple2.Item1);
						}
						if (list2.Count == 5)
						{
							break;
						}
					}
					if (appendFindAction)
					{
						list2.AddRange(enumerable);
					}
					valueTuple = new global::System.ValueTuple<CopilotHelper.ActionModel[], bool>(list2.ToArray(), flag);
				}
				return valueTuple;
			}

			// Token: 0x0600302C RID: 12332 RVA: 0x000EB774 File Offset: 0x000E9974
			private static async Task<CopilotHelper.ActionModel[]> GetAllActions()
			{
				CopilotHelper.ActionModel[] array;
				if (CopilotHelper.AppActionHelper.actionModels != null)
				{
					array = CopilotHelper.AppActionHelper.actionModels;
				}
				else
				{
					IDisposable disposable = await CopilotHelper.AppActionHelper.asyncLock.LockAsync();
					using (disposable)
					{
						CopilotHelper.AppActionHelper.<>c__DisplayClass8_0 CS$<>8__locals1 = new CopilotHelper.AppActionHelper.<>c__DisplayClass8_0();
						if (CopilotHelper.AppActionHelper.actionModels != null)
						{
							return CopilotHelper.AppActionHelper.actionModels;
						}
						CS$<>8__locals1.tcs = new TaskCompletionSource<bool>();
						DispatcherHelper.CheckBeginInvokeOnUI(delegate
						{
							CopilotHelper.AppActionHelper.<>c__DisplayClass8_0.<<GetAllActions>b__0>d <<GetAllActions>b__0>d;
							<<GetAllActions>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
							<<GetAllActions>b__0>d.<>4__this = CS$<>8__locals1;
							<<GetAllActions>b__0>d.<>1__state = -1;
							<<GetAllActions>b__0>d.<>t__builder.Start<CopilotHelper.AppActionHelper.<>c__DisplayClass8_0.<<GetAllActions>b__0>d>(ref <<GetAllActions>b__0>d);
						});
						await CS$<>8__locals1.tcs.Task;
					}
					IDisposable disposable2 = null;
					array = CopilotHelper.AppActionHelper.actionModels;
				}
				return array;
			}

			// Token: 0x0600302D RID: 12333 RVA: 0x000EB7B0 File Offset: 0x000E99B0
			internal static string GetAppActionConfirm(CopilotHelper.ActionModel action)
			{
				if (action == null)
				{
					return null;
				}
				string resourceName = Ioc.Default.GetService<AppSettingsViewModel>().ActualLanguage.ResourceName;
				string text;
				if (action.Confirm != null && action.Confirm.Count > 0 && (action.Confirm.TryGetValue(resourceName, out text) || action.Confirm.TryGetValue("en", out text)) && !string.IsNullOrEmpty(text))
				{
					if (action.Parameters != null && action.Parameters.Count > 0)
					{
						foreach (KeyValuePair<string, string> keyValuePair in action.Parameters)
						{
							text = text.Replace("{$" + keyValuePair.Key + "}", keyValuePair.Value);
						}
					}
					return text;
				}
				return null;
			}

			// Token: 0x0600302E RID: 12334 RVA: 0x000EB89C File Offset: 0x000E9A9C
			internal static async Task InitializeAllActions()
			{
				await CopilotHelper.AppActionHelper.GetAllActions();
			}

			// Token: 0x0600302F RID: 12335 RVA: 0x000EB8D8 File Offset: 0x000E9AD8
			internal static string GetAppActionConfirm(CopilotHelper.AppActionModel action)
			{
				if (action == null)
				{
					return null;
				}
				if (CopilotHelper.AppActionHelper.actionModels == null)
				{
					return null;
				}
				CopilotHelper.ActionModel actionModel = CopilotHelper.AppActionHelper.actionModels.FirstOrDefault((CopilotHelper.ActionModel c) => c.Name == action.Name);
				if (actionModel == null)
				{
					return null;
				}
				string resourceName = Ioc.Default.GetService<AppSettingsViewModel>().ActualLanguage.ResourceName;
				string text;
				if (actionModel.Confirm != null && actionModel.Confirm.Count > 0 && (actionModel.Confirm.TryGetValue(resourceName, out text) || actionModel.Confirm.TryGetValue("en", out text)) && !string.IsNullOrEmpty(text))
				{
					if (action.Parameters != null && action.Parameters.Count > 0)
					{
						foreach (KeyValuePair<string, string> keyValuePair in action.Parameters)
						{
							text = text.Replace("{$" + keyValuePair.Key + "}", keyValuePair.Value);
						}
					}
					return text;
				}
				return null;
			}

			// Token: 0x06003032 RID: 12338 RVA: 0x000EBA24 File Offset: 0x000E9C24
			[CompilerGenerated]
			internal static ConvFromPDFType? <ProcessAction>g__MapType|3_1(string _type)
			{
				if (_type != null)
				{
					switch (_type.Length)
					{
					case 3:
					{
						char c = _type[1];
						switch (c)
						{
						case 'm':
							if (_type == "xml")
							{
								return new ConvFromPDFType?(ConvFromPDFType.PDFToXml);
							}
							break;
						case 'n':
							if (_type == "png")
							{
								return new ConvFromPDFType?(ConvFromPDFType.PDFToPng);
							}
							break;
						case 'o':
							break;
						case 'p':
							if (_type == "ppt")
							{
								return new ConvFromPDFType?(ConvFromPDFType.PDFToPPT);
							}
							break;
						default:
							if (c != 't')
							{
								if (c == 'x')
								{
									if (_type == "txt")
									{
										return new ConvFromPDFType?(ConvFromPDFType.PDFToTxt);
									}
								}
							}
							else if (_type == "rtf")
							{
								return new ConvFromPDFType?(ConvFromPDFType.PDFToRTF);
							}
							break;
						}
						break;
					}
					case 4:
					{
						char c = _type[0];
						if (c != 'h')
						{
							if (c != 'j')
							{
								if (c == 'w')
								{
									if (_type == "word")
									{
										return new ConvFromPDFType?(ConvFromPDFType.PDFToWord);
									}
								}
							}
							else if (_type == "jpeg")
							{
								return new ConvFromPDFType?(ConvFromPDFType.PDFToJpg);
							}
						}
						else if (_type == "html")
						{
							return new ConvFromPDFType?(ConvFromPDFType.PDFToHtml);
						}
						break;
					}
					case 5:
						if (_type == "excel")
						{
							return new ConvFromPDFType?(ConvFromPDFType.PDFToExcel);
						}
						break;
					}
				}
				return null;
			}

			// Token: 0x06003033 RID: 12339 RVA: 0x000EBB8C File Offset: 0x000E9D8C
			[CompilerGenerated]
			internal static ConvToPDFType? <ProcessAction>g__MapType|3_2(string _type)
			{
				if (_type == "word")
				{
					return new ConvToPDFType?(ConvToPDFType.WordToPDF);
				}
				if (_type == "excel")
				{
					return new ConvToPDFType?(ConvToPDFType.ExcelToPDF);
				}
				if (_type == "image")
				{
					return new ConvToPDFType?(ConvToPDFType.ImageToPDF);
				}
				if (_type == "ppt")
				{
					return new ConvToPDFType?(ConvToPDFType.PPTToPDF);
				}
				if (_type == "rtf")
				{
					return new ConvToPDFType?(ConvToPDFType.RtfToPDF);
				}
				if (!(_type == "txt"))
				{
					return null;
				}
				return new ConvToPDFType?(ConvToPDFType.TxtToPDF);
			}

			// Token: 0x04001C5C RID: 7260
			private static CopilotHelper.ActionModel[] actionModels;

			// Token: 0x04001C5D RID: 7261
			private static AsyncLock asyncLock = new AsyncLock();
		}
	}
}
