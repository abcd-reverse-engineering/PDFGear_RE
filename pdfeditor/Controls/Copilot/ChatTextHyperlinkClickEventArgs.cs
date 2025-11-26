using System;
using System.Windows;
using pdfeditor.Utils.Copilot;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x0200028F RID: 655
	public class ChatTextHyperlinkClickEventArgs : RoutedEventArgs
	{
		// Token: 0x060025D1 RID: 9681 RVA: 0x000B0515 File Offset: 0x000AE715
		public ChatTextHyperlinkClickEventArgs(RoutedEvent routedEvent, object source, ChatTextHyperlinkClickAction action, string text, CopilotHelper.AppActionModel appAction)
			: base(routedEvent, source)
		{
			this.Action = action;
			this.Text = text;
			this.AppAction = appAction;
		}

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x060025D2 RID: 9682 RVA: 0x000B0536 File Offset: 0x000AE736
		public string Text { get; }

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x060025D3 RID: 9683 RVA: 0x000B053E File Offset: 0x000AE73E
		public CopilotHelper.AppActionModel AppAction { get; }

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x060025D4 RID: 9684 RVA: 0x000B0546 File Offset: 0x000AE746
		public ChatTextHyperlinkClickAction Action { get; }
	}
}
