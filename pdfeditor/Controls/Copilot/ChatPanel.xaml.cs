using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using CommonLib.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Models.Copilot;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Copilot;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x0200028C RID: 652
	public partial class ChatPanel : UserControl
	{
		// Token: 0x06002585 RID: 9605 RVA: 0x000AE852 File Offset: 0x000ACA52
		public ChatPanel()
		{
			this.InitializeComponent();
			this.chatMessages = new ObservableCollection<ChatMessageModel>();
			this.ChatItemsControl.ItemsSource = this.chatMessages;
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x000AE887 File Offset: 0x000ACA87
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ChatPdf", "Close", "Count", 1L);
			EventHandler closeButtonClick = this.CloseButtonClick;
			if (closeButtonClick == null)
			{
				return;
			}
			closeButtonClick(this, e);
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06002587 RID: 9607 RVA: 0x000AE8B4 File Offset: 0x000ACAB4
		// (remove) Token: 0x06002588 RID: 9608 RVA: 0x000AE8EC File Offset: 0x000ACAEC
		public event EventHandler CloseButtonClick;

		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x06002589 RID: 9609 RVA: 0x000AE921 File Offset: 0x000ACB21
		// (set) Token: 0x0600258A RID: 9610 RVA: 0x000AE933 File Offset: 0x000ACB33
		public CopilotHelper CopilotHelper
		{
			get
			{
				return (CopilotHelper)base.GetValue(ChatPanel.CopilotHelperProperty);
			}
			set
			{
				base.SetValue(ChatPanel.CopilotHelperProperty, value);
			}
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x000AE941 File Offset: 0x000ACB41
		public void FocusUserInputTextBox()
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				if (base.IsVisible && this.ChatPage.Visibility == Visibility.Visible)
				{
					this.ScrollToBottom(true, true);
					this.UserInputTextBox.Focus();
					Keyboard.Focus(this.UserInputTextBox);
				}
			}));
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000AE95C File Offset: 0x000ACB5C
		private void UpdateCopilotHelper()
		{
			this.Reset();
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000AE964 File Offset: 0x000ACB64
		private void Reset()
		{
			this.ChatNowButton.Content = pdfeditor.Properties.Resources.ResourceManager.GetString("CopilotWelcomePageChatNowButtonText");
			this.AnalysisProgressBar.Visibility = Visibility.Collapsed;
			this.AnalysisProgressBar.Value = 0.0;
			this.AnalysisProgressBar.IsIndeterminate = false;
			this.WelcomePage.Visibility = Visibility.Visible;
			this.ChatPage.Visibility = Visibility.Collapsed;
			this.StopButton.Visibility = Visibility.Collapsed;
			this.chatMessages.Clear();
		}

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x0600258E RID: 9614 RVA: 0x000AE9E6 File Offset: 0x000ACBE6
		public bool Chatting
		{
			get
			{
				return this.ChatPage.Visibility == Visibility.Visible;
			}
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x000AE9F8 File Offset: 0x000ACBF8
		private async void NavigatedFromWelcome(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ChatPdf", "ChatNowButton", "Count", 1L);
			try
			{
				this.SendButton.IsEnabled = false;
				((Button)sender).IsEnabled = false;
				CopilotHelper helper = this.CopilotHelper;
				if (helper != null)
				{
					Progress<double> progress = new Progress<double>(delegate(double p)
					{
						this.AnalysisProgressBar.Value = p;
					});
					this.ChatNowButton.Content = pdfeditor.Properties.Resources.ResourceManager.GetString("CopilotWelcomePageAnalysingText");
					this.AnalysisProgressBar.Visibility = Visibility.Visible;
					await helper.InitializeAsync(progress);
					this.AnalysisProgressBar.IsIndeterminate = true;
					ChatMessageModel chatMessageModel = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Welcome);
					this.chatMessages.Add(chatMessageModel);
					this.LoadMessageListFromCache();
					if (helper == this.CopilotHelper)
					{
						this.WelcomePage.Visibility = Visibility.Collapsed;
						this.ChatPage.Visibility = Visibility.Visible;
						this.AnalysisProgressBar.IsIndeterminate = false;
						this.FocusUserInputTextBox();
					}
				}
				helper = null;
			}
			finally
			{
				((Button)sender).IsEnabled = true;
				this.UpdateCanChatState();
			}
		}

		// Token: 0x06002590 RID: 9616 RVA: 0x000AEA38 File Offset: 0x000ACC38
		private bool LoadMessageListFromCache()
		{
			CopilotHelper copilotHelper = this.CopilotHelper;
			if (copilotHelper != null)
			{
				List<CopilotHelper.ChatMessage> cachedMessageList = copilotHelper.GetCachedMessageList();
				if (cachedMessageList != null && cachedMessageList.Count > 0)
				{
					foreach (CopilotHelper.ChatMessage chatMessage in cachedMessageList)
					{
						ChatMessageModel chatMessageModel = ChatMessageModel.Create(chatMessage.Role, ChatMessageModel.ChatMessageType.Chat);
						chatMessageModel.Text = chatMessage.Content;
						chatMessageModel.Pages = chatMessage.Pages;
						if (chatMessage.Liked == "Like")
						{
							chatMessageModel.Like = ChatMessageModel.Liked.Like;
						}
						else if (chatMessage.Liked == "Dislike")
						{
							chatMessageModel.Like = ChatMessageModel.Liked.Dislike;
						}
						else
						{
							chatMessageModel.Like = ChatMessageModel.Liked.None;
						}
						this.chatMessages.Add(chatMessageModel);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002591 RID: 9617 RVA: 0x000AEB20 File Offset: 0x000ACD20
		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			await this.SendAsync(true);
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x000AEB58 File Offset: 0x000ACD58
		public async Task RequestSummaryAsync()
		{
			ChatPanel.<>c__DisplayClass20_0 CS$<>8__locals1 = new ChatPanel.<>c__DisplayClass20_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.SendButton.IsEnabled)
			{
				CopilotHelper helper = this.CopilotHelper;
				if (helper != null)
				{
					this.SendButton.IsEnabled = false;
					try
					{
						CS$<>8__locals1.summaryChatModel = null;
						string cachedSummary = helper.GetCachedSummary();
						if (!string.IsNullOrEmpty(cachedSummary))
						{
							CS$<>8__locals1.summaryChatModel = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Summary);
							CS$<>8__locals1.summaryChatModel.Text = cachedSummary;
							this.chatMessages.Add(CS$<>8__locals1.summaryChatModel);
							await base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
							{
								FrameworkElement frameworkElement = CS$<>8__locals1.<>4__this.ChatItemsControl.ItemContainerGenerator.ContainerFromItem(CS$<>8__locals1.summaryChatModel) as FrameworkElement;
								if (frameworkElement != null)
								{
									frameworkElement.BringIntoView();
									return;
								}
								CS$<>8__locals1.<>4__this.ScrollToBottom(true, true);
							}));
						}
						else
						{
							ChatPanel.<>c__DisplayClass20_1 CS$<>8__locals2 = new ChatPanel.<>c__DisplayClass20_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							this.UpdateCanChatState();
							this.SendButton.IsEnabled = false;
							CS$<>8__locals2.sb = new StringBuilder();
							CS$<>8__locals2.CS$<>8__locals1.summaryChatModel = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Summary);
							CS$<>8__locals2.CS$<>8__locals1.summaryChatModel.Loading = true;
							this.chatMessages.Add(CS$<>8__locals2.CS$<>8__locals1.summaryChatModel);
							CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
							CancellationTokenSource cancellationTokenSource2 = this.cts;
							if (cancellationTokenSource2 != null)
							{
								cancellationTokenSource2.Cancel();
							}
							this.cts = cancellationTokenSource;
							CopilotHelper.CopilotResult copilotResult = await helper.GetSummaryAsync(delegate(string text, CancellationToken ct)
							{
								StringBuilder sb = CS$<>8__locals2.sb;
								lock (sb)
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.WelcomePage.Visibility = Visibility.Collapsed;
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.ChatPage.Visibility = Visibility.Visible;
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.AnalysisProgressBar.IsIndeterminate = false;
									CS$<>8__locals2.sb.Append(text);
									CS$<>8__locals2.CS$<>8__locals1.summaryChatModel.Text = CS$<>8__locals2.sb.ToString();
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.ScrollToBottom(false, true);
								}
								return Task.CompletedTask;
							}, cancellationTokenSource.Token);
							if (CS$<>8__locals2.CS$<>8__locals1.summaryChatModel != null)
							{
								CS$<>8__locals2.CS$<>8__locals1.summaryChatModel.Loading = false;
								CS$<>8__locals2.CS$<>8__locals1.summaryChatModel.Error = copilotResult.Error;
								if (CS$<>8__locals2.CS$<>8__locals1.summaryChatModel.HasError)
								{
									GAManager.SendEvent("ChatPdf", "Summary_Failed", "Count", 1L);
								}
							}
							CS$<>8__locals2 = null;
						}
					}
					finally
					{
						this.UpdateCanChatState();
						if (helper == this.CopilotHelper)
						{
							this.SendButton.IsEnabled = true;
							this.StopButton.Visibility = Visibility.Collapsed;
						}
					}
				}
			}
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x000AEB9C File Offset: 0x000ACD9C
		private async Task SendAsync(bool useAppAction)
		{
			if (this.SendButton.IsEnabled)
			{
				if (string.IsNullOrEmpty(this.UserInputTextBox.Text.TrimStart(Array.Empty<char>()).TrimEnd(Array.Empty<char>())))
				{
					this.ChatEmptytip.ShowBubble(new TimeSpan?(TimeSpan.FromSeconds(2.0)));
					this.UserInputTextBox.Text = "";
					this.UserInputTextBox.Focus();
				}
				else
				{
					GAManager.SendEvent("ChatPdf", "SendButton", "Count", 1L);
					string text = this.UserInputTextBox.Text;
					this.UserInputTextBox.Text = "";
					ChatMessageModel userModel = ChatMessageModel.CreateUserModel();
					userModel.Text = text;
					this.chatMessages.Add(userModel);
					this.ScrollToBottom(true, true);
					CancellationTokenSource cts2 = new CancellationTokenSource();
					CancellationTokenSource cancellationTokenSource = this.cts;
					if (cancellationTokenSource != null)
					{
						cancellationTokenSource.Cancel();
					}
					this.cts = cts2;
					CopilotHelper.CopilotResult copilotResult = null;
					if (useAppAction)
					{
						copilotResult = await this.DirectGetAppActionAsync(text, userModel, cts2.Token);
					}
					if (copilotResult == null || (copilotResult.AppAction == null && copilotResult.MaybeNotAppAction))
					{
						GAManager.SendEvent("ChatPdf", "Chat_DirectChat", "Count", 1L);
						await this.DirectChatAsync(text, userModel, cts2.Token);
					}
					else
					{
						GAManager.SendEvent("ChatPdf", "Chat_AppAction", copilotResult.AppAction.Name ?? "", 1L);
					}
				}
			}
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x000AEBE8 File Offset: 0x000ACDE8
		private async Task<CopilotHelper.CopilotResult> DirectGetAppActionAsync(string message, ChatMessageModel askModel, CancellationToken cancellationToken)
		{
			CopilotHelper.CopilotResult copilotResult;
			if (!this.SendButton.IsEnabled)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else
			{
				CopilotHelper helper = this.CopilotHelper;
				if (helper == null)
				{
					copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
				else if (string.IsNullOrEmpty((message != null) ? message.Trim() : null))
				{
					copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
				else
				{
					try
					{
						this.UpdateCanChatState();
						this.SendButton.IsEnabled = false;
						this.StopButton.Visibility = Visibility.Visible;
						ChatMessageModel model = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Chat);
						model.Loading = true;
						model.AskModel = askModel;
						this.chatMessages.Add(model);
						this.ScrollToBottom(true, true);
						CopilotHelper.CopilotResult copilotResult2 = await helper.GetAppActionAsync(message, cancellationToken);
						CopilotHelper.CopilotResult appActionResult = copilotResult2;
						if (appActionResult != null && appActionResult.Error == CopilotHelper.ChatResultError.None && appActionResult.AppAction != null)
						{
							GAManager.SendEvent("ChatPdf", "AppAction_DirectGetAppActionAsync", appActionResult.AppAction.Name ?? "", 1L);
							if (appActionResult.Text == null)
							{
								this.chatMessages.Remove(model);
								if (helper != null && this.CopilotHelper == helper)
								{
									await helper.ProcessNativeAppAction(appActionResult.AppAction);
								}
							}
							else
							{
								model.MessageType.Value = ChatMessageModel.ChatMessageType.AppAction;
								model.Loading = false;
								model.AskModel = askModel;
								model.Text = appActionResult.Text;
								model.AppAction = appActionResult.AppAction;
								model.MaybeNotAppAction = appActionResult.MaybeNotAppAction;
							}
							this.ScrollToBottom(true, true);
						}
						else
						{
							this.chatMessages.Remove(model);
						}
						copilotResult = appActionResult ?? CopilotHelper.CopilotResult.EmptyUnknownFailed;
					}
					finally
					{
						this.UpdateCanChatState();
						if (helper == this.CopilotHelper)
						{
							this.SendButton.IsEnabled = true;
							this.StopButton.Visibility = Visibility.Collapsed;
						}
					}
				}
			}
			return copilotResult;
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x000AEC44 File Offset: 0x000ACE44
		private async Task<CopilotHelper.CopilotResult> DirectChatAsync(string message, ChatMessageModel askModel, CancellationToken cancellationToken)
		{
			CopilotHelper.CopilotResult copilotResult;
			if (!this.SendButton.IsEnabled)
			{
				copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
			}
			else
			{
				CopilotHelper helper = this.CopilotHelper;
				if (helper == null)
				{
					copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
				else if (string.IsNullOrEmpty((message != null) ? message.Trim() : null))
				{
					copilotResult = CopilotHelper.CopilotResult.EmptyUnknownFailed;
				}
				else
				{
					try
					{
						this.UpdateCanChatState();
						this.SendButton.IsEnabled = false;
						this.StopButton.Visibility = Visibility.Visible;
						ChatMessageModel model = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Chat);
						model.Loading = true;
						model.AskModel = askModel;
						this.chatMessages.Add(model);
						this.ScrollToBottom(true, true);
						StringBuilder sb = new StringBuilder();
						CopilotHelper.CopilotResult copilotResult2 = await helper.ChatAsync(message, delegate(string result, CancellationToken ct)
						{
							StringBuilder sb2 = sb;
							lock (sb2)
							{
								sb.Append(result);
								model.Text = sb.ToString();
								this.ScrollToBottom(false, false);
							}
							return Task.CompletedTask;
						}, cancellationToken);
						if (copilotResult2 != null)
						{
							model.Loading = false;
							model.Error = copilotResult2.Error;
							model.Pages = copilotResult2.Pages;
							if (model.HasError)
							{
								GAManager.SendEvent("ChatPdf", "Chat_Failed", "Count", 1L);
							}
						}
						this.ScrollToBottom(true, true);
						copilotResult = copilotResult2 ?? CopilotHelper.CopilotResult.EmptyUnknownFailed;
					}
					finally
					{
						this.UpdateCanChatState();
						if (helper == this.CopilotHelper)
						{
							this.SendButton.IsEnabled = true;
							this.StopButton.Visibility = Visibility.Collapsed;
						}
					}
				}
			}
			return copilotResult;
		}

		// Token: 0x06002596 RID: 9622 RVA: 0x000AECA0 File Offset: 0x000ACEA0
		private async void ChatItemsControl_HyperlinkClick(object sender, ChatTextHyperlinkClickEventArgs e)
		{
			if (e.Action == ChatTextHyperlinkClickAction.SuggestionAppAction)
			{
				if (!string.IsNullOrEmpty(e.Text) && e.AppAction != null)
				{
					GAManager.SendEvent("ChatPdf", "SuggestionAppActionBtnClick", e.AppAction.Name ?? "", 1L);
					if (e.AppAction.Confirm == null)
					{
						ChatMessageModel chatMessageModel = ChatMessageModel.CreateUserModel();
						chatMessageModel.Text = e.Text;
						this.chatMessages.Add(chatMessageModel);
						CopilotHelper copilotHelper = this.CopilotHelper;
						if (copilotHelper != null)
						{
							await copilotHelper.ProcessNativeAppAction(e.AppAction);
						}
					}
					else
					{
						ChatMessageModel chatMessageModel2 = ChatMessageModel.CreateUserModel();
						chatMessageModel2.Text = e.Text;
						ChatMessageModel chatMessageModel3 = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.AppAction);
						chatMessageModel3.Text = e.AppAction.Confirm;
						chatMessageModel3.AppAction = e.AppAction;
						this.chatMessages.Add(chatMessageModel2);
						this.chatMessages.Add(chatMessageModel3);
						this.ScrollToBottom(true, true);
					}
				}
			}
			else if (e.Action == ChatTextHyperlinkClickAction.SummaryQuestion)
			{
				GAManager.SendEvent("ChatPdf", "HyperlinkClick", "SummaryQuestion", 1L);
				this.SendUserInputMessage(e.Text, false);
			}
			else if (e.Action == ChatTextHyperlinkClickAction.ErrorTryAgain)
			{
				GAManager.SendEvent("ChatPdf", "HyperlinkClick", "ErrorTryAgain", 1L);
				ChatTextControl chatTextControl = e.OriginalSource as ChatTextControl;
				if (chatTextControl != null)
				{
					ChatMessageModel chatMessageModel4 = chatTextControl.DataContext as ChatMessageModel;
					if (chatMessageModel4 != null)
					{
						this.SendUserInputMessage(chatMessageModel4.AskModel.Text, true);
					}
				}
			}
			else if (e.Action == ChatTextHyperlinkClickAction.GoToPage)
			{
				GAManager.SendEvent("ChatPdf", "HyperlinkClick", "GoToPage", 1L);
				int num;
				if (int.TryParse(e.Text, out num))
				{
					CopilotHelper copilotHelper2 = this.CopilotHelper;
					PdfDocument pdfDocument = ((copilotHelper2 != null) ? copilotHelper2.Document : null);
					if (num >= 0 && num < pdfDocument.Pages.Count)
					{
						global::PDFKit.PdfControl.GetPdfControl(pdfDocument).ScrollToPage(num);
					}
				}
			}
		}

		// Token: 0x06002597 RID: 9623 RVA: 0x000AECDF File Offset: 0x000ACEDF
		private void SendUserInputMessage(string text, bool useAppAction)
		{
			if (!string.IsNullOrEmpty(text) && this.SendButton.IsEnabled)
			{
				this.UserInputTextBox.Text = text;
				this.SendAsync(useAppAction);
			}
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x000AED0C File Offset: 0x000ACF0C
		private void ScrollToBottom(bool force, bool animated = true)
		{
			DateTime now = DateTime.Now;
			if (force || (now - this.lastScroll).TotalSeconds > 1.0)
			{
				this.lastScroll = now;
				if (animated)
				{
					this.ScrollToBottomAnimated();
					return;
				}
				this.ScrollViewer.ScrollToEnd();
			}
		}

		// Token: 0x06002599 RID: 9625 RVA: 0x000AED5D File Offset: 0x000ACF5D
		private void ScrollToBottomAnimated()
		{
			this.ScrollViewer.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				if (base.IsLoaded)
				{
					double scrollableHeight = this.ScrollViewer.ScrollableHeight;
					if (scrollableHeight > 0.0)
					{
						this.ScrollViewer.SmoothScrollToVerticalOffset(scrollableHeight, 500.0, null);
					}
				}
			}));
		}

		// Token: 0x0600259A RID: 9626 RVA: 0x000AED80 File Offset: 0x000ACF80
		private async void UserInputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			bool flag = (Keyboard.Modifiers & ModifierKeys.Control) > ModifierKeys.None;
			bool flag2 = (Keyboard.Modifiers & ModifierKeys.Shift) > ModifierKeys.None;
			if (e.Key == Key.Return && (flag || flag2))
			{
				e.Handled = true;
				await this.SendAsync(true);
			}
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x000AEDC0 File Offset: 0x000ACFC0
		private void UpdateCanChatState()
		{
			CopilotHelper copilotHelper = this.CopilotHelper;
			if (copilotHelper != null)
			{
				int chatRemaining = copilotHelper.GetChatRemaining();
				if (chatRemaining > 0)
				{
					this.SendButton.IsEnabled = true;
					this.UserInputTextBox.IsEnabled = true;
					this.Overchance.Visibility = Visibility.Collapsed;
					this.InputPanel.Visibility = Visibility.Visible;
				}
				else
				{
					this.InputPanel.Visibility = Visibility.Collapsed;
					this.Overchance.Visibility = Visibility.Visible;
				}
				string @string = pdfeditor.Properties.Resources.ResourceManager.GetString("CopilotMessagesRemainingTips");
				string text = string.Format("{0}/{1}", chatRemaining, 50);
				this.ChatTips.Text = @string.Replace("XXX", text);
			}
		}

		// Token: 0x0600259C RID: 9628 RVA: 0x000AEE6E File Offset: 0x000AD06E
		private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClearHistoryAsync();
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x000AEE78 File Offset: 0x000AD078
		private void FeedbackButton_Click(object sender, RoutedEventArgs e)
		{
			string documentPath = Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.DocumentPath;
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.Owner = App.Current.MainWindow;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			feedbackWindow.source = "ChatPdf";
			feedbackWindow.HideFaq();
			if (!string.IsNullOrEmpty(documentPath))
			{
				feedbackWindow.flist.Add(documentPath);
				feedbackWindow.showAttachmentCB(true);
			}
			feedbackWindow.ShowDialog();
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x000AEEEC File Offset: 0x000AD0EC
		private async void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			await this.ExportHistory();
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x000AEF24 File Offset: 0x000AD124
		private void DisLike_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ChatPdf", "DisLikeBtn", "Count", 1L);
			ChatMessageModel chatMessageModel = ((FrameworkElement)sender).DataContext as ChatMessageModel;
			chatMessageModel.Like = ChatMessageModel.Liked.Dislike;
			this.SetLikedState(chatMessageModel);
			this.CopilotHelper.ShowFeedbackWindow(true);
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x000AEF74 File Offset: 0x000AD174
		private void Like_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ChatPdf", "LikeBtn", "Count", 1L);
			ChatMessageModel chatMessageModel = ((FrameworkElement)sender).DataContext as ChatMessageModel;
			chatMessageModel.Like = ChatMessageModel.Liked.Like;
			this.SetLikedState(chatMessageModel);
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x000AEFB8 File Offset: 0x000AD1B8
		private void SetLikedState(ChatMessageModel data)
		{
			CopilotHelper.ChatMessage chatMessage = new CopilotHelper.ChatMessage
			{
				Role = data.Role,
				Content = data.Text,
				Pages = data.Pages,
				Liked = data.Like.ToString()
			};
			this.CopilotHelper.LikedAsyne(chatMessage);
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x000AF018 File Offset: 0x000AD218
		private void ClearHistoryAsync()
		{
			if (!this.SendButton.IsEnabled)
			{
				return;
			}
			if (ModernMessageBox.Show(pdfeditor.Properties.Resources.CopilotClearHistoryTip, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) == MessageBoxResult.Cancel)
			{
				return;
			}
			GAManager.SendEvent("ChatPdf", "ClearHistory", "Count", 1L);
			CopilotHelper copilotHelper = this.CopilotHelper;
			if (copilotHelper != null)
			{
				this.chatMessages.Clear();
				copilotHelper.ClearMessageList();
			}
			ChatMessageModel chatMessageModel = ChatMessageModel.CreateAssistantModel(ChatMessageModel.ChatMessageType.Welcome);
			this.chatMessages.Add(chatMessageModel);
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x000AF08E File Offset: 0x000AD28E
		private Task ExportHistory()
		{
			new ExportChatMessagesDialog(this.chatMessages)
			{
				Owner = App.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog();
			return Task.CompletedTask;
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x000AF0BD File Offset: 0x000AD2BD
		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this.cts = null;
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x000AF0D8 File Offset: 0x000AD2D8
		private async void AppActionCancelButton_Click(object sender, RoutedEventArgs e)
		{
			ChatMessageModel chatMessageModel = ((FrameworkElement)sender).DataContext as ChatMessageModel;
			if (chatMessageModel != null && chatMessageModel.AppAction != null)
			{
				if (chatMessageModel.MaybeNotAppAction && chatMessageModel.AskModel != null && this.chatMessages.LastOrDefault<ChatMessageModel>() == chatMessageModel)
				{
					this.chatMessages.Remove(chatMessageModel);
					CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
					CancellationTokenSource cancellationTokenSource2 = this.cts;
					if (cancellationTokenSource2 != null)
					{
						cancellationTokenSource2.Cancel();
					}
					this.cts = cancellationTokenSource;
					GAManager.SendEvent("ChatPdf", "AppAction_CancelAndChat", chatMessageModel.AppAction.Name ?? "", 1L);
					await this.DirectChatAsync(chatMessageModel.AskModel.Text, chatMessageModel.AskModel, cancellationTokenSource.Token);
				}
				else
				{
					GAManager.SendEvent("ChatPdf", "AppAction_Cancel", chatMessageModel.AppAction.Name ?? "", 1L);
					chatMessageModel.AppActionState.Value = ChatMessageModel.MessageAppActionState.Canceled;
				}
			}
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x000AF118 File Offset: 0x000AD318
		private async void AppActionYesButton_Click(object sender, RoutedEventArgs e)
		{
			ChatMessageModel model = ((FrameworkElement)sender).DataContext as ChatMessageModel;
			if (model != null && model.AppAction != null)
			{
				model.AppActionState.Value = ChatMessageModel.MessageAppActionState.Processing;
				try
				{
					GAManager.SendEvent("ChatPdf", "AppAction_Yes", model.AppAction.Name ?? "", 1L);
					await this.CopilotHelper.ProcessNativeAppAction(model.AppAction);
				}
				finally
				{
					model.AppActionState.Value = ChatMessageModel.MessageAppActionState.Done;
				}
			}
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x000AF3B8 File Offset: 0x000AD5B8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 11:
				((ToggleButton)target).Click += this.Like_Click;
				return;
			case 12:
				((ToggleButton)target).Click += this.DisLike_Click;
				return;
			case 13:
				((Button)target).Click += this.AppActionCancelButton_Click;
				return;
			case 14:
				((Button)target).Click += this.AppActionCancelButton_Click;
				return;
			case 15:
				((Button)target).Click += this.AppActionYesButton_Click;
				return;
			default:
				return;
			}
		}

		// Token: 0x04001020 RID: 4128
		private DateTime lastScroll;

		// Token: 0x04001021 RID: 4129
		private ObservableCollection<ChatMessageModel> chatMessages;

		// Token: 0x04001022 RID: 4130
		private CancellationTokenSource cts;

		// Token: 0x04001024 RID: 4132
		public static readonly DependencyProperty CopilotHelperProperty = DependencyProperty.Register("CopilotHelper", typeof(CopilotHelper), typeof(ChatPanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatPanel chatPanel = s as ChatPanel;
			if (chatPanel != null && a.NewValue != a.OldValue)
			{
				chatPanel.UpdateCopilotHelper();
			}
		}));

		// Token: 0x04001025 RID: 4133
		private string text = "";
	}
}
