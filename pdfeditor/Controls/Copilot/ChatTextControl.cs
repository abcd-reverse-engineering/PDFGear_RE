using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using pdfeditor.Models.Copilot;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Copilot;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x0200028D RID: 653
	internal class ChatTextControl : ContentControl
	{
		// Token: 0x060025AF RID: 9647 RVA: 0x000AF520 File Offset: 0x000AD720
		public ChatTextControl()
		{
			this.rtb = new RichTextBox
			{
				IsReadOnly = true,
				IsDocumentEnabled = true,
				Padding = default(Thickness),
				BorderThickness = default(Thickness),
				Background = null
			};
			this.rtb.SetResourceReference(Control.ForegroundProperty, "TextBrushWhiteAndBlack");
			base.Content = this.rtb;
		}

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x060025B0 RID: 9648 RVA: 0x000AF592 File Offset: 0x000AD792
		// (set) Token: 0x060025B1 RID: 9649 RVA: 0x000AF5A4 File Offset: 0x000AD7A4
		public CopilotHelper CopilotHelper
		{
			get
			{
				return (CopilotHelper)base.GetValue(ChatTextControl.CopilotHelperProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.CopilotHelperProperty, value);
			}
		}

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x060025B2 RID: 9650 RVA: 0x000AF5B2 File Offset: 0x000AD7B2
		// (set) Token: 0x060025B3 RID: 9651 RVA: 0x000AF5C4 File Offset: 0x000AD7C4
		public string Text
		{
			get
			{
				return (string)base.GetValue(ChatTextControl.TextProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.TextProperty, value);
			}
		}

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x060025B4 RID: 9652 RVA: 0x000AF5D2 File Offset: 0x000AD7D2
		// (set) Token: 0x060025B5 RID: 9653 RVA: 0x000AF5E4 File Offset: 0x000AD7E4
		public ChatMessageModel.ChatMessageType MessageType
		{
			get
			{
				return (ChatMessageModel.ChatMessageType)base.GetValue(ChatTextControl.MessageTypeProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.MessageTypeProperty, value);
			}
		}

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x060025B6 RID: 9654 RVA: 0x000AF5F7 File Offset: 0x000AD7F7
		// (set) Token: 0x060025B7 RID: 9655 RVA: 0x000AF609 File Offset: 0x000AD809
		public CopilotHelper.ChatResultError Error
		{
			get
			{
				return (CopilotHelper.ChatResultError)base.GetValue(ChatTextControl.ErrorProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.ErrorProperty, value);
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x060025B8 RID: 9656 RVA: 0x000AF61C File Offset: 0x000AD81C
		// (set) Token: 0x060025B9 RID: 9657 RVA: 0x000AF62E File Offset: 0x000AD82E
		public bool Loading
		{
			get
			{
				return (bool)base.GetValue(ChatTextControl.LoadingProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.LoadingProperty, value);
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x060025BA RID: 9658 RVA: 0x000AF641 File Offset: 0x000AD841
		// (set) Token: 0x060025BB RID: 9659 RVA: 0x000AF653 File Offset: 0x000AD853
		public string Role
		{
			get
			{
				return (string)base.GetValue(ChatTextControl.RoleProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.RoleProperty, value);
			}
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x060025BC RID: 9660 RVA: 0x000AF661 File Offset: 0x000AD861
		// (set) Token: 0x060025BD RID: 9661 RVA: 0x000AF673 File Offset: 0x000AD873
		public IEnumerable<int> Pages
		{
			get
			{
				return (IEnumerable<int>)base.GetValue(ChatTextControl.PagesProperty);
			}
			set
			{
				base.SetValue(ChatTextControl.PagesProperty, value);
			}
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x060025BE RID: 9662 RVA: 0x000AF684 File Offset: 0x000AD884
		public string ErrorText
		{
			get
			{
				CopilotHelper.ChatResultError error = this.Error;
				if (error == CopilotHelper.ChatResultError.ContentFiltered)
				{
					return pdfeditor.Properties.Resources.ResourceManager.GetString("ChatPanelMessageError_ContentFiltered");
				}
				if (error != CopilotHelper.ChatResultError.UserCanceled)
				{
					return pdfeditor.Properties.Resources.ResourceManager.GetString("ChatPanelMessageErrorText");
				}
				return pdfeditor.Properties.Resources.ResourceManager.GetString("ChatPanelMessageError_UserCanceled");
			}
		}

		// Token: 0x060025BF RID: 9663 RVA: 0x000AF6D8 File Offset: 0x000AD8D8
		private void UpdateDocument()
		{
			string text = this.Text;
			FlowDocument document = this.rtb.Document;
			document.Blocks.Clear();
			if (this.MessageType == ChatMessageModel.ChatMessageType.Welcome)
			{
				this.ProcessWelcomeDocument(document);
				return;
			}
			if (this.Error != CopilotHelper.ChatResultError.None)
			{
				this.ProcessErrorDocument(document);
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				if (this.Loading)
				{
					Paragraph paragraph = new Paragraph
					{
						LineHeight = 18.0,
						Inlines = 
						{
							new LoadingEllipsis(false)
						}
					};
					document.Blocks.Add(paragraph);
				}
				return;
			}
			this.ProcessTextDocument(document);
			if (this.MessageType != ChatMessageModel.ChatMessageType.Summary && !this.Loading && this.Error == CopilotHelper.ChatResultError.None && this.Pages != null && this.Role == "assistant")
			{
				int[] array = this.Pages.Distinct<int>().ToArray<int>();
				if (array.Length != 0)
				{
					Paragraph paragraph2 = new Paragraph
					{
						LineHeight = 18.0
					};
					paragraph2.Inlines.Add(pdfeditor.Properties.Resources.CopilotViewboxPages);
					int[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						int page = array2[i];
						Hyperlink hyperlink = new Hyperlink(new Run(string.Format("{0}", page + 1)))
						{
							Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 41, 143, 238))
						};
						hyperlink.Click += delegate(object s, RoutedEventArgs a)
						{
							this.OnHyperlinkClick(ChatTextHyperlinkClickAction.GoToPage, string.Format("{0}", page));
						};
						paragraph2.Inlines.Add(hyperlink);
						paragraph2.Inlines.Add(new Run("   "));
					}
					document.Blocks.Add(paragraph2);
				}
			}
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x000AF8B0 File Offset: 0x000ADAB0
		private void ProcessWelcomeDocument(FlowDocument document)
		{
			Run run = new Run(pdfeditor.Properties.Resources.ChatPanelMessageWelcomeText1);
			document.Blocks.Add(new Paragraph(run)
			{
				LineHeight = 18.0,
				Margin = new Thickness(0.0, 0.0, 0.0, 4.0)
			});
			Paragraph paragraph = new Paragraph
			{
				LineHeight = 18.0,
				Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
				Inlines = 
				{
					new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(pdfeditor.Properties.Resources.ChatPanelMessageWelcomeAction_summary, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2("summary", Array.Empty<global::System.ValueTuple<string, string>>()))),
					new LineBreak()
				}
			};
			foreach (string text in (from c in ChatTextControl.WelcomeSampleActionNames
				where c != "summary"
				orderby ChatTextControl.rnd.Next()
				select c).Take(4).ToArray<string>())
			{
				string text2 = "ChatPanelMessageWelcomeAction_" + text.Replace("-", "_");
				string @string = pdfeditor.Properties.Resources.ResourceManager.GetString(text2);
				if (text == "convert-from-pdf")
				{
					paragraph.Inlines.Add(new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(@string, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2(text, new global::System.ValueTuple<string, string>[]
					{
						new global::System.ValueTuple<string, string>("mode", "word")
					}))));
				}
				else if (text == "compress")
				{
					paragraph.Inlines.Add(new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(@string, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2(text, new global::System.ValueTuple<string, string>[]
					{
						new global::System.ValueTuple<string, string>("mode", "high")
					}))));
				}
				else if (text == "protect-pdf")
				{
					paragraph.Inlines.Add(new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(@string, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2(text, Array.Empty<global::System.ValueTuple<string, string>>()))));
				}
				else if (text == "slide-show")
				{
					paragraph.Inlines.Add(new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(@string, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2(text, Array.Empty<global::System.ValueTuple<string, string>>()))));
				}
				else if (text == "page-zoom")
				{
					paragraph.Inlines.Add(new InlineUIContainer(this.<ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(@string, ChatTextControl.<ProcessWelcomeDocument>g__CreateAction|37_2(text, new global::System.ValueTuple<string, string>[]
					{
						new global::System.ValueTuple<string, string>("mode", "zoom-in")
					}))));
				}
				if (paragraph.Inlines.OfType<InlineUIContainer>().Count<InlineUIContainer>() != 5)
				{
					paragraph.Inlines.Add(new LineBreak());
				}
			}
			document.Blocks.Add(paragraph);
			document.Blocks.Add(new Paragraph(new Run(pdfeditor.Properties.Resources.ChatPanelMessageWelcomeText2))
			{
				LineHeight = 18.0,
				Margin = new Thickness(0.0, 0.0, 0.0, 0.0)
			});
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x000AFC08 File Offset: 0x000ADE08
		private void ProcessErrorDocument(FlowDocument document)
		{
			if (this.Error == CopilotHelper.ChatResultError.UserCanceled && !string.IsNullOrEmpty(this.Text))
			{
				this.ProcessTextDocument(document);
			}
			Paragraph paragraph = new Paragraph
			{
				LineHeight = 18.0
			};
			paragraph.Inlines.Add(this.ErrorText);
			if (this.MessageType != ChatMessageModel.ChatMessageType.Summary && this.Error != CopilotHelper.ChatResultError.ContentFiltered && this.Error != CopilotHelper.ChatResultError.CountLimit && this.Error != CopilotHelper.ChatResultError.AccountBaned)
			{
				Hyperlink hyperlink = new Hyperlink
				{
					Inlines = 
					{
						new Run(pdfeditor.Properties.Resources.ChatPanelMessageErrorRetryText)
					},
					Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 41, 143, 238))
				};
				hyperlink.Click += delegate(object s, RoutedEventArgs a)
				{
					this.OnHyperlinkClick(ChatTextHyperlinkClickAction.ErrorTryAgain, "Try again");
				};
				paragraph.Inlines.Add(new Run(" "));
				paragraph.Inlines.Add(hyperlink);
			}
			if (this.Error != CopilotHelper.ChatResultError.UserCanceled)
			{
				Hyperlink hyperlink2 = new Hyperlink
				{
					Inlines = 
					{
						new Run(pdfeditor.Properties.Resources.MenuHelpContactUsContent)
					},
					Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 41, 143, 238))
				};
				hyperlink2.Click += delegate(object s, RoutedEventArgs a)
				{
					CopilotHelper copilotHelper = this.CopilotHelper;
					if (copilotHelper == null)
					{
						return;
					}
					copilotHelper.ShowFeedbackWindow(false);
				};
				paragraph.Inlines.Add(new Run(" "));
				paragraph.Inlines.Add(hyperlink2);
			}
			document.Blocks.Add(paragraph);
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x000AFD84 File Offset: 0x000ADF84
		private void ProcessTextDocument(FlowDocument document)
		{
			string text = this.Text;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			foreach (string text2 in from c in text.Replace("\r", "").Split(new char[] { '\n' })
				where !string.IsNullOrEmpty(c) && c != "\n"
				select c)
			{
				Paragraph paragraph = new Paragraph
				{
					LineHeight = 18.0
				};
				if (this.MessageType == ChatMessageModel.ChatMessageType.Summary && (text2.StartsWith("1.") || text2.StartsWith("2.") || text2.StartsWith("3.")))
				{
					Geometry geometry = (Geometry)new GeometryConverter().ConvertFromString("M-0.0271606 -0.0274658L35.9728 17.7589V19.3509L-0.0271606 36.9725V-0.0274658ZM3.55655 20.3116V31.2084L26.6063 20.3116H3.55655ZM3.55655 16.6884H26.1176L3.55655 5.81897V16.6884Z");
					new Path().Data = geometry;
					InlineUIContainer inlineUIContainer = new InlineUIContainer
					{
						Child = new Border
						{
							Child = new Viewbox
							{
								Stretch = Stretch.Uniform,
								Child = new Path
								{
									Data = geometry,
									Fill = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 41, 143, 238))
								},
								Width = 12.0,
								Height = 12.0,
								Margin = new Thickness(0.0, 0.0, 4.0, -2.0)
							}
						}
					};
					string inlineText = text2.Substring(2);
					Hyperlink hyperlink = new Hyperlink
					{
						Inlines = 
						{
							inlineUIContainer,
							new Run
							{
								Text = inlineText
							}
						},
						TextDecorations = null,
						Foreground = Brushes.Black
					};
					hyperlink.SetResourceReference(TextElement.ForegroundProperty, "TextBrushWhiteAndBlack");
					hyperlink.Click += delegate(object s, RoutedEventArgs a)
					{
						this.OnHyperlinkClick(ChatTextHyperlinkClickAction.SummaryQuestion, inlineText);
					};
					paragraph.Inlines.Add(hyperlink);
				}
				else
				{
					paragraph.Inlines.Add(new Run
					{
						Text = text2.Trim()
					});
				}
				document.Blocks.Add(paragraph);
			}
			if (this.Loading)
			{
				Paragraph paragraph2 = document.Blocks.LastOrDefault<Block>() as Paragraph;
				if (paragraph2 != null)
				{
					paragraph2.Inlines.Add(new LoadingEllipsis(true));
				}
			}
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x000B002C File Offset: 0x000AE22C
		private void OnHyperlinkClick(ChatTextHyperlinkClickAction action, string text)
		{
			ChatTextHyperlinkClickEventArgs chatTextHyperlinkClickEventArgs = new ChatTextHyperlinkClickEventArgs(ChatTextControl.HyperlinkClickEvent, this, action, text, null);
			base.RaiseEvent(chatTextHyperlinkClickEventArgs);
		}

		// Token: 0x060025C4 RID: 9668 RVA: 0x000B0050 File Offset: 0x000AE250
		private void OnSuggestionAppActionClick(string text, CopilotHelper.AppActionModel action)
		{
			ChatTextHyperlinkClickEventArgs chatTextHyperlinkClickEventArgs = new ChatTextHyperlinkClickEventArgs(ChatTextControl.HyperlinkClickEvent, this, ChatTextHyperlinkClickAction.SuggestionAppAction, text, action);
			base.RaiseEvent(chatTextHyperlinkClickEventArgs);
		}

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x060025C5 RID: 9669 RVA: 0x000B0073 File Offset: 0x000AE273
		// (remove) Token: 0x060025C6 RID: 9670 RVA: 0x000B0081 File Offset: 0x000AE281
		public event ChatTextHyperlinkClickEventHandler HyperlinkClick
		{
			add
			{
				base.AddHandler(ChatTextControl.HyperlinkClickEvent, value);
			}
			remove
			{
				base.RemoveHandler(ChatTextControl.HyperlinkClickEvent, value);
			}
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x000B0090 File Offset: 0x000AE290
		// Note: this type is marked as 'beforefieldinit'.
		static ChatTextControl()
		{
			ChatTextControl.HyperlinkClickEvent = EventManager.RegisterRoutedEvent("HyperlinkClick", RoutingStrategy.Bubble, typeof(ChatTextHyperlinkClickEventHandler), typeof(ChatTextControl));
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x000B02C0 File Offset: 0x000AE4C0
		[CompilerGenerated]
		internal static CopilotHelper.AppActionModel <ProcessWelcomeDocument>g__CreateAction|37_2(string _name, [global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "key", "value" })] global::System.ValueTuple<string, string>[] parameters)
		{
			CopilotHelper.AppActionModel appActionModel = new CopilotHelper.AppActionModel();
			appActionModel.Name = _name;
			appActionModel.Parameters = (from c in parameters
				group c by c.Item1).ToDictionary(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "key", "value" })] IGrouping<string, global::System.ValueTuple<string, string>> c) => c.Key, ([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "key", "value" })] IGrouping<string, global::System.ValueTuple<string, string>> c) => c.FirstOrDefault<global::System.ValueTuple<string, string>>().Item2);
			return appActionModel;
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x000B0348 File Offset: 0x000AE548
		[CompilerGenerated]
		private Button <ProcessWelcomeDocument>g__CreateWelcomeContentButton|37_3(string _content, CopilotHelper.AppActionModel _appAction)
		{
			Button button = new Button
			{
				Tag = new object[] { _content, _appAction },
				Content = new TextBlock
				{
					Text = _content,
					TextWrapping = TextWrapping.Wrap,
					FontSize = 12.0,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					TextAlignment = TextAlignment.Center
				},
				Margin = new Thickness(-4.0, 4.0, 0.0, 4.0),
				Padding = new Thickness(8.0, 6.0, 8.0, 3.0),
				Background = Brushes.White,
				BorderBrush = ChatTextControl.WelcomeContentButtonBorderBrush,
				BorderThickness = new Thickness(1.0),
				MinWidth = 0.0,
				MinHeight = 0.0,
				Style = (Style)App.Current.Resources["DialogButtonStyle"]
			};
			button.SetResourceReference(Control.BackgroundProperty, "SignaturePickerBackground");
			UIElementExtension.SetCornerRadius(button, new CornerRadius(4.0));
			button.Click += this.<ProcessWelcomeDocument>g___button_Click|37_4;
			return button;
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x000B04A8 File Offset: 0x000AE6A8
		[CompilerGenerated]
		private void <ProcessWelcomeDocument>g___button_Click|37_4(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			if (frameworkElement != null)
			{
				object[] array = frameworkElement.Tag as object[];
				if (array != null && array.Length == 2)
				{
					string text = array[0] as string;
					if (text != null)
					{
						CopilotHelper.AppActionModel appActionModel = array[1] as CopilotHelper.AppActionModel;
						if (appActionModel != null)
						{
							this.OnSuggestionAppActionClick(text, appActionModel);
						}
					}
				}
			}
		}

		// Token: 0x04001039 RID: 4153
		private RichTextBox rtb;

		// Token: 0x0400103A RID: 4154
		public static readonly DependencyProperty CopilotHelperProperty = DependencyProperty.Register("CopilotHelper", typeof(CopilotHelper), typeof(ChatTextControl), new PropertyMetadata(null));

		// Token: 0x0400103B RID: 4155
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ChatTextControl), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl = s as ChatTextControl;
			if (chatTextControl != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl.UpdateDocument();
			}
		}));

		// Token: 0x0400103C RID: 4156
		public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register("MessageType", typeof(ChatMessageModel.ChatMessageType), typeof(ChatTextControl), new PropertyMetadata(ChatMessageModel.ChatMessageType.Chat, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl2 = s as ChatTextControl;
			if (chatTextControl2 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl2.UpdateDocument();
			}
		}));

		// Token: 0x0400103D RID: 4157
		public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register("Error", typeof(CopilotHelper.ChatResultError), typeof(ChatTextControl), new PropertyMetadata(CopilotHelper.ChatResultError.None, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl3 = s as ChatTextControl;
			if (chatTextControl3 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl3.UpdateDocument();
			}
		}));

		// Token: 0x0400103E RID: 4158
		public static readonly DependencyProperty LoadingProperty = DependencyProperty.Register("Loading", typeof(bool), typeof(ChatTextControl), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl4 = s as ChatTextControl;
			if (chatTextControl4 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl4.UpdateDocument();
			}
		}));

		// Token: 0x0400103F RID: 4159
		public static readonly DependencyProperty RoleProperty = DependencyProperty.Register("Role", typeof(string), typeof(ChatTextControl), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl5 = s as ChatTextControl;
			if (chatTextControl5 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl5.UpdateDocument();
			}
		}));

		// Token: 0x04001040 RID: 4160
		public static readonly DependencyProperty PagesProperty = DependencyProperty.Register("Pages", typeof(IEnumerable<int>), typeof(ChatTextControl), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatTextControl chatTextControl6 = s as ChatTextControl;
			if (chatTextControl6 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatTextControl6.UpdateDocument();
			}
		}));

		// Token: 0x04001041 RID: 4161
		private const int MaxWelcomeContentButtonCount = 5;

		// Token: 0x04001042 RID: 4162
		private static string[] WelcomeSampleActionNames = new string[] { "summary", "convert-from-pdf", "compress", "protect-pdf", "slide-show", "page-zoom" };

		// Token: 0x04001043 RID: 4163
		private static Random rnd = new Random();

		// Token: 0x04001044 RID: 4164
		private static Brush WelcomeContentButtonBorderBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 216, 216, 216));
	}
}
