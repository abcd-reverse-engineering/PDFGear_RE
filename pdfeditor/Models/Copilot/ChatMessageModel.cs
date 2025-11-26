using System;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfeditor.Utils.Copilot;

namespace pdfeditor.Models.Copilot
{
	// Token: 0x0200018B RID: 395
	internal class ChatMessageModel : ObservableObject
	{
		// Token: 0x06001689 RID: 5769 RVA: 0x00055E65 File Offset: 0x00054065
		private ChatMessageModel()
		{
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x0600168A RID: 5770 RVA: 0x00055E83 File Offset: 0x00054083
		// (set) Token: 0x0600168B RID: 5771 RVA: 0x00055E8B File Offset: 0x0005408B
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				base.SetProperty<string>(ref this.text, value, "Text");
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x0600168C RID: 5772 RVA: 0x00055EA0 File Offset: 0x000540A0
		// (set) Token: 0x0600168D RID: 5773 RVA: 0x00055EA8 File Offset: 0x000540A8
		public ChatMessageModel.Liked Like
		{
			get
			{
				return this.like;
			}
			set
			{
				if (base.SetProperty<ChatMessageModel.Liked>(ref this.like, value, "Like"))
				{
					base.OnPropertyChanged("IsLiked");
					base.OnPropertyChanged("IsDisliked");
					base.OnPropertyChanged("IsLikeButtonVisible");
					base.OnPropertyChanged("IsDislikeButtonVisible");
				}
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x0600168E RID: 5774 RVA: 0x00055EF8 File Offset: 0x000540F8
		public EnumBindingObject<ChatMessageModel.ChatMessageType> MessageType
		{
			get
			{
				EnumBindingObject<ChatMessageModel.ChatMessageType> enumBindingObject;
				if ((enumBindingObject = this.messageType) == null)
				{
					enumBindingObject = (this.messageType = new EnumBindingObject<ChatMessageModel.ChatMessageType>(ChatMessageModel.ChatMessageType.Chat));
				}
				return enumBindingObject;
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x0600168F RID: 5775 RVA: 0x00055F1E File Offset: 0x0005411E
		// (set) Token: 0x06001690 RID: 5776 RVA: 0x00055F26 File Offset: 0x00054126
		public CopilotHelper.AppActionModel AppAction
		{
			get
			{
				return this.appAction;
			}
			set
			{
				base.SetProperty<CopilotHelper.AppActionModel>(ref this.appAction, value, "AppAction");
			}
		}

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06001691 RID: 5777 RVA: 0x00055F3C File Offset: 0x0005413C
		public EnumBindingObject<ChatMessageModel.MessageAppActionState> AppActionState
		{
			get
			{
				EnumBindingObject<ChatMessageModel.MessageAppActionState> enumBindingObject;
				if ((enumBindingObject = this.appActionState) == null)
				{
					enumBindingObject = (this.appActionState = new EnumBindingObject<ChatMessageModel.MessageAppActionState>(ChatMessageModel.MessageAppActionState.None));
				}
				return enumBindingObject;
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06001692 RID: 5778 RVA: 0x00055F62 File Offset: 0x00054162
		// (set) Token: 0x06001693 RID: 5779 RVA: 0x00055F6A File Offset: 0x0005416A
		public ChatMessageModel AskModel
		{
			get
			{
				return this.askModel;
			}
			set
			{
				base.SetProperty<ChatMessageModel>(ref this.askModel, value, "AskModel");
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06001694 RID: 5780 RVA: 0x00055F7F File Offset: 0x0005417F
		// (set) Token: 0x06001695 RID: 5781 RVA: 0x00055F87 File Offset: 0x00054187
		public CopilotHelper.ChatResultError Error
		{
			get
			{
				return this.error;
			}
			set
			{
				if (base.SetProperty<CopilotHelper.ChatResultError>(ref this.error, value, "Error"))
				{
					base.OnPropertyChanged("HasError");
					base.OnPropertyChanged("NoError");
				}
			}
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x00055FB3 File Offset: 0x000541B3
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x00055FBB File Offset: 0x000541BB
		public bool MaybeNotAppAction
		{
			get
			{
				return this.maybeNotAppAction;
			}
			set
			{
				base.SetProperty<bool>(ref this.maybeNotAppAction, value, "MaybeNotAppAction");
			}
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x00055FD0 File Offset: 0x000541D0
		// (set) Token: 0x06001699 RID: 5785 RVA: 0x00055FD8 File Offset: 0x000541D8
		public bool Loading
		{
			get
			{
				return this.loading;
			}
			set
			{
				base.SetProperty<bool>(ref this.loading, value, "Loading");
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x0600169A RID: 5786 RVA: 0x00055FED File Offset: 0x000541ED
		// (set) Token: 0x0600169B RID: 5787 RVA: 0x00055FF5 File Offset: 0x000541F5
		public bool IsEditing
		{
			get
			{
				return this.isEditing;
			}
			set
			{
				base.SetProperty<bool>(ref this.isEditing, value, "IsEditing");
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x0600169C RID: 5788 RVA: 0x0005600A File Offset: 0x0005420A
		// (set) Token: 0x0600169D RID: 5789 RVA: 0x00056012 File Offset: 0x00054212
		public int[] Pages
		{
			get
			{
				return this.pages;
			}
			set
			{
				base.SetProperty<int[]>(ref this.pages, value, "Pages");
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x0600169E RID: 5790 RVA: 0x00056027 File Offset: 0x00054227
		public bool HasError
		{
			get
			{
				return this.Error > CopilotHelper.ChatResultError.None;
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x0600169F RID: 5791 RVA: 0x00056032 File Offset: 0x00054232
		public bool NoError
		{
			get
			{
				return this.Error == CopilotHelper.ChatResultError.None;
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x060016A0 RID: 5792 RVA: 0x0005603D File Offset: 0x0005423D
		public string Role
		{
			get
			{
				return this.role;
			}
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x060016A1 RID: 5793 RVA: 0x00056045 File Offset: 0x00054245
		public bool IsUser
		{
			get
			{
				return this.role == "user";
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x060016A2 RID: 5794 RVA: 0x00056057 File Offset: 0x00054257
		public bool IsAssistant
		{
			get
			{
				return this.role == "assistant";
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x060016A3 RID: 5795 RVA: 0x00056069 File Offset: 0x00054269
		public bool IsLiked
		{
			get
			{
				return this.like == ChatMessageModel.Liked.Like;
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x060016A4 RID: 5796 RVA: 0x00056074 File Offset: 0x00054274
		public bool IsDisliked
		{
			get
			{
				return this.like == ChatMessageModel.Liked.Dislike;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x0005607F File Offset: 0x0005427F
		public bool IsLikeButtonVisible
		{
			get
			{
				return this.IsAssistant && this.MessageType.Value != ChatMessageModel.ChatMessageType.Summary && (this.like == ChatMessageModel.Liked.Like || this.like == ChatMessageModel.Liked.None);
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x060016A6 RID: 5798 RVA: 0x000560AD File Offset: 0x000542AD
		public bool IsDislikeButtonVisible
		{
			get
			{
				return this.IsAssistant && this.MessageType.Value != ChatMessageModel.ChatMessageType.Summary && (this.like == ChatMessageModel.Liked.Dislike || this.like == ChatMessageModel.Liked.None);
			}
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x000560DB File Offset: 0x000542DB
		public static ChatMessageModel CreateUserModel()
		{
			return new ChatMessageModel
			{
				role = "user"
			};
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x000560ED File Offset: 0x000542ED
		public static ChatMessageModel CreateAssistantModel(ChatMessageModel.ChatMessageType type)
		{
			return new ChatMessageModel
			{
				role = "assistant",
				MessageType = 
				{
					Value = type
				}
			};
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x0005610B File Offset: 0x0005430B
		public static ChatMessageModel Create(string role, ChatMessageModel.ChatMessageType type)
		{
			return new ChatMessageModel
			{
				role = role,
				MessageType = 
				{
					Value = type
				}
			};
		}

		// Token: 0x04000780 RID: 1920
		private string text = "";

		// Token: 0x04000781 RID: 1921
		private ChatMessageModel askModel;

		// Token: 0x04000782 RID: 1922
		private string role = "user";

		// Token: 0x04000783 RID: 1923
		private EnumBindingObject<ChatMessageModel.ChatMessageType> messageType;

		// Token: 0x04000784 RID: 1924
		private CopilotHelper.AppActionModel appAction;

		// Token: 0x04000785 RID: 1925
		private EnumBindingObject<ChatMessageModel.MessageAppActionState> appActionState;

		// Token: 0x04000786 RID: 1926
		private bool isEditing;

		// Token: 0x04000787 RID: 1927
		private bool loading;

		// Token: 0x04000788 RID: 1928
		private string errorText;

		// Token: 0x04000789 RID: 1929
		private int[] pages;

		// Token: 0x0400078A RID: 1930
		private ChatMessageModel.Liked like;

		// Token: 0x0400078B RID: 1931
		private CopilotHelper.ChatResultError error;

		// Token: 0x0400078C RID: 1932
		private bool maybeNotAppAction;

		// Token: 0x02000599 RID: 1433
		public enum Liked
		{
			// Token: 0x04001E84 RID: 7812
			None,
			// Token: 0x04001E85 RID: 7813
			Like,
			// Token: 0x04001E86 RID: 7814
			Dislike
		}

		// Token: 0x0200059A RID: 1434
		public enum ChatMessageType
		{
			// Token: 0x04001E88 RID: 7816
			Chat,
			// Token: 0x04001E89 RID: 7817
			Summary,
			// Token: 0x04001E8A RID: 7818
			Welcome,
			// Token: 0x04001E8B RID: 7819
			AppAction
		}

		// Token: 0x0200059B RID: 1435
		public enum MessageAppActionState
		{
			// Token: 0x04001E8D RID: 7821
			None,
			// Token: 0x04001E8E RID: 7822
			Processing,
			// Token: 0x04001E8F RID: 7823
			Done,
			// Token: 0x04001E90 RID: 7824
			Canceled
		}
	}
}
