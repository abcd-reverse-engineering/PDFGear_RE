using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CommonLib.Common;
using Microsoft.Win32;
using pdfeditor.Models.Copilot;
using pdfeditor.Properties;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x02000292 RID: 658
	public partial class ExportChatMessagesDialog : Window
	{
		// Token: 0x060025DB RID: 9691 RVA: 0x000B06F4 File Offset: 0x000AE8F4
		internal ExportChatMessagesDialog(IEnumerable<ChatMessageModel> chatMessages)
		{
			this.InitializeComponent();
			this.showToastAnimation = this.LayoutRoot.Resources["ShowToastAnimation"] as Storyboard;
			ExportChatMessagesDialog.BuildContentText(this.ContentTextBox.Document, chatMessages);
			base.Loaded += delegate(object s, RoutedEventArgs a)
			{
				this.ContentTextBox.Document.PagePadding = new Thickness(8.0, 4.0, 8.0, 4.0);
				this.ContentTextBox.Focus();
				Keyboard.Focus(this.ContentTextBox);
				this.ContentTextBox.SelectAll();
			};
		}

		// Token: 0x060025DC RID: 9692 RVA: 0x000B0750 File Offset: 0x000AE950
		private static void BuildContentText(FlowDocument doc, IEnumerable<ChatMessageModel> chatMessages)
		{
			doc.Blocks.Clear();
			foreach (ChatMessageModel chatMessageModel in chatMessages)
			{
				if (chatMessageModel != null && chatMessageModel.NoError && !string.IsNullOrEmpty(chatMessageModel.Text))
				{
					string text = "";
					if (chatMessageModel.MessageType.Value == ChatMessageModel.ChatMessageType.Summary)
					{
						text = "PDFgear: ";
					}
					else if (chatMessageModel.IsAssistant)
					{
						text = "PDFgear: ";
					}
					else if (chatMessageModel.IsUser)
					{
						text = "Me: ";
					}
					Paragraph paragraph = new Paragraph
					{
						LineHeight = 16.0,
						FontSize = 12.0,
						TextIndent = -20.0,
						Margin = new Thickness(20.0, 0.0, 0.0, 10.0),
						Inlines = 
						{
							new Run(text)
							{
								FontWeight = FontWeights.Bold
							},
							new Run(chatMessageModel.Text)
						}
					};
					if (chatMessageModel.Pages != null && chatMessageModel.Pages.Length != 0)
					{
						paragraph.Inlines.Add(new LineBreak());
						paragraph.Inlines.Add(pdfeditor.Properties.Resources.CopilotViewboxPages);
						paragraph.Inlines.Add(chatMessageModel.Pages.Aggregate(new StringBuilder(), (StringBuilder sb, int c) => sb.Append(c + 1).Append(' ')).ToString());
					}
					doc.Blocks.Add(paragraph);
				}
			}
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x000B091C File Offset: 0x000AEB1C
		private string GetContentText()
		{
			string text;
			try
			{
				text = new TextRange(this.ContentTextBox.Document.ContentStart, this.ContentTextBox.Document.ContentEnd).Text;
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x000B0970 File Offset: 0x000AEB70
		private async void CopyBtn_Click(object sender, RoutedEventArgs e)
		{
			string contentText = this.GetContentText();
			if (!string.IsNullOrEmpty(contentText))
			{
				((Button)sender).IsEnabled = false;
				try
				{
					Clipboard.SetDataObject(contentText);
					this.showToastAnimation.SkipToFill();
					this.showToastAnimation.Begin();
					await Task.Delay(300);
				}
				catch
				{
					this.showToastAnimation.SkipToFill();
				}
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x000B09B0 File Offset: 0x000AEBB0
		private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
		{
			string contentText = this.GetContentText();
			if (!string.IsNullOrEmpty(contentText))
			{
				((Button)sender).IsEnabled = false;
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					AddExtension = true,
					Filter = "txt|*.txt",
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
					FileName = "Export.txt"
				};
				if (saveFileDialog.ShowDialog().GetValueOrDefault())
				{
					string fileName = saveFileDialog.FileName;
					try
					{
						File.WriteAllText(fileName, contentText, Encoding.UTF8);
						Mouse.OverrideCursor = Cursors.AppStarting;
						await ExplorerUtils.SelectItemInExplorerAsync(fileName, default(CancellationToken));
						Mouse.OverrideCursor = null;
					}
					catch
					{
					}
				}
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x04001050 RID: 4176
		private const string SummaryTitle = "PDFgear: ";

		// Token: 0x04001051 RID: 4177
		private const string UserMessageTitle = "Me: ";

		// Token: 0x04001052 RID: 4178
		private const string AssistantMessageTitle = "PDFgear: ";

		// Token: 0x04001053 RID: 4179
		private Storyboard showToastAnimation;
	}
}
