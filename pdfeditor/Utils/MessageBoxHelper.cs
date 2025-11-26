using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonLib.Common;
using CommonLib.Common.MessageBoxHelper;
using pdfeditor.Properties;

namespace pdfeditor.Utils
{
	// Token: 0x02000084 RID: 132
	public static class MessageBoxHelper
	{
		// Token: 0x06000902 RID: 2306 RVA: 0x0002CDA4 File Offset: 0x0002AFA4
		public static MessageBoxHelper.RichMessageBoxResult Show(MessageBoxHelper.RichMessageBoxContent messageBoxContent, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxResult defaultResult = MessageBoxResult.None, CultureInfo cultureInfo = null, bool isButtonReversed = false)
		{
			if (messageBoxContent == null)
			{
				throw new ArgumentException(null, "messageBoxContent");
			}
			UIElement uielement = MessageBoxHelper.<Show>g__CreateUIElement|1_0(messageBoxContent.Title, true);
			UIElement uielement2 = MessageBoxHelper.<Show>g__CreateUIElement|1_0(messageBoxContent.Content, false);
			if (uielement == null && uielement2 == null)
			{
				throw new ArgumentException(null, "messageBoxContent");
			}
			if (uielement2 != null)
			{
				Grid.SetRow(uielement2, 1);
				FrameworkElement frameworkElement = uielement2 as FrameworkElement;
				if (frameworkElement != null && uielement != null && object.Equals(frameworkElement.Tag, "MessageBoxHelperCreate"))
				{
					frameworkElement.Margin = new Thickness(0.0, 12.0, 0.0, 12.0);
				}
			}
			CheckBox checkBox = null;
			if (messageBoxContent.ShowLeftBottomCheckbox)
			{
				checkBox = new CheckBox
				{
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Bottom,
					Margin = new Thickness(0.0, 0.0, 0.0, -36.0),
					MaxWidth = 220.0
				};
				checkBox.SetResourceReference(FrameworkElement.StyleProperty, "DefaultCheckBoxStyle");
				if (!string.IsNullOrEmpty(messageBoxContent.LeftBottomCheckboxContent))
				{
					checkBox.Content = messageBoxContent.LeftBottomCheckboxContent;
				}
				Grid.SetRow(checkBox, 2);
			}
			Grid grid = new Grid
			{
				RowDefinitions = 
				{
					new RowDefinition
					{
						Height = new GridLength(1.0, GridUnitType.Auto)
					},
					new RowDefinition
					{
						Height = new GridLength(1.0, GridUnitType.Star)
					},
					new RowDefinition
					{
						Height = new GridLength(1.0, GridUnitType.Auto)
					}
				},
				MinWidth = 384.0
			};
			if (uielement != null)
			{
				grid.Children.Add(uielement);
			}
			if (uielement2 != null)
			{
				grid.Children.Add(uielement2);
			}
			if (checkBox != null)
			{
				grid.Children.Add(checkBox);
			}
			return new MessageBoxHelper.RichMessageBoxResult(ModernMessageBox.Show(new ModernMessageBoxOptions
			{
				Caption = caption,
				MessageBoxContent = grid,
				Button = button,
				DefaultResult = defaultResult,
				CultureInfo = cultureInfo,
				UIOverrides = 
				{
					IsButtonsReversed = isButtonReversed
				}
			}), (checkBox != null) ? checkBox.IsChecked : null);
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0002CFE0 File Offset: 0x0002B1E0
		[CompilerGenerated]
		internal static UIElement <Show>g__CreateUIElement|1_0(object _obj, bool _bold)
		{
			UIElement uielement = _obj as UIElement;
			if (uielement != null)
			{
				return uielement;
			}
			string text = _obj as string;
			if (text != null)
			{
				return new TextBlock
				{
					Text = text,
					LineHeight = 18.0,
					FontWeight = (_bold ? FontWeights.Bold : FontWeights.Normal),
					TextWrapping = TextWrapping.Wrap,
					Tag = "MessageBoxHelperCreate"
				};
			}
			return null;
		}

		// Token: 0x0400045E RID: 1118
		private const string InternalCreateObjectTag = "MessageBoxHelperCreate";

		// Token: 0x02000438 RID: 1080
		public class RichMessageBoxContent
		{
			// Token: 0x17000C8A RID: 3210
			// (get) Token: 0x06002CF2 RID: 11506 RVA: 0x000DBD4E File Offset: 0x000D9F4E
			public static string DefaultLeftBottomCheckboxContent
			{
				get
				{
					return Resources.WinPwdPasswordSaveTipNotshowagainContent;
				}
			}

			// Token: 0x17000C8B RID: 3211
			// (get) Token: 0x06002CF3 RID: 11507 RVA: 0x000DBD55 File Offset: 0x000D9F55
			// (set) Token: 0x06002CF4 RID: 11508 RVA: 0x000DBD5D File Offset: 0x000D9F5D
			public object Title { get; set; }

			// Token: 0x17000C8C RID: 3212
			// (get) Token: 0x06002CF5 RID: 11509 RVA: 0x000DBD66 File Offset: 0x000D9F66
			// (set) Token: 0x06002CF6 RID: 11510 RVA: 0x000DBD6E File Offset: 0x000D9F6E
			public object Content { get; set; }

			// Token: 0x17000C8D RID: 3213
			// (get) Token: 0x06002CF7 RID: 11511 RVA: 0x000DBD77 File Offset: 0x000D9F77
			// (set) Token: 0x06002CF8 RID: 11512 RVA: 0x000DBD7F File Offset: 0x000D9F7F
			public bool ShowLeftBottomCheckbox { get; set; }

			// Token: 0x17000C8E RID: 3214
			// (get) Token: 0x06002CF9 RID: 11513 RVA: 0x000DBD88 File Offset: 0x000D9F88
			// (set) Token: 0x06002CFA RID: 11514 RVA: 0x000DBD90 File Offset: 0x000D9F90
			public string LeftBottomCheckboxContent { get; set; } = MessageBoxHelper.RichMessageBoxContent.DefaultLeftBottomCheckboxContent;
		}

		// Token: 0x02000439 RID: 1081
		public struct RichMessageBoxResult
		{
			// Token: 0x06002CFC RID: 11516 RVA: 0x000DBDAC File Offset: 0x000D9FAC
			public RichMessageBoxResult(MessageBoxResult result, bool? checkboxResult)
			{
				this.Result = result;
				this.CheckboxResult = checkboxResult;
			}

			// Token: 0x17000C8F RID: 3215
			// (get) Token: 0x06002CFD RID: 11517 RVA: 0x000DBDBC File Offset: 0x000D9FBC
			public MessageBoxResult Result { get; }

			// Token: 0x17000C90 RID: 3216
			// (get) Token: 0x06002CFE RID: 11518 RVA: 0x000DBDC4 File Offset: 0x000D9FC4
			public bool? CheckboxResult { get; }
		}
	}
}
