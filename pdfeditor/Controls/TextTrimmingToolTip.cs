using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfeditor.Controls
{
	// Token: 0x020001DF RID: 479
	public class TextTrimmingToolTip
	{
		// Token: 0x06001B22 RID: 6946 RVA: 0x0006D7CD File Offset: 0x0006B9CD
		public static bool GetEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(TextTrimmingToolTip.EnabledProperty);
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x0006D7DF File Offset: 0x0006B9DF
		public static void SetEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(TextTrimmingToolTip.EnabledProperty, value);
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x0006D7F2 File Offset: 0x0006B9F2
		public static string GetTooltipText(DependencyObject obj)
		{
			return (string)obj.GetValue(TextTrimmingToolTip.TooltipTextProperty);
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x0006D804 File Offset: 0x0006BA04
		public static void SetTooltipText(DependencyObject obj, string value)
		{
			obj.SetValue(TextTrimmingToolTip.TooltipTextProperty, value);
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x0006D814 File Offset: 0x0006BA14
		private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextBlock textBlock = d as TextBlock;
			if (textBlock != null)
			{
				if ((bool)e.NewValue)
				{
					textBlock.SizeChanged += TextTrimmingToolTip.TextBlock_SizeChanged;
					TextTrimmingToolTip.UpdateToolTip(textBlock);
					return;
				}
				textBlock.SizeChanged -= TextTrimmingToolTip.TextBlock_SizeChanged;
				textBlock.ToolTip = null;
			}
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x0006D86C File Offset: 0x0006BA6C
		private static void OnTooltipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextBlock textBlock = d as TextBlock;
			if (textBlock != null && TextTrimmingToolTip.GetEnabled(textBlock))
			{
				TextTrimmingToolTip.UpdateToolTip(textBlock);
			}
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x0006D894 File Offset: 0x0006BA94
		private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			TextBlock textBlock = sender as TextBlock;
			if (textBlock != null)
			{
				TextTrimmingToolTip.UpdateToolTip(textBlock);
			}
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x0006D8B4 File Offset: 0x0006BAB4
		private static void UpdateToolTip(TextBlock textBlock)
		{
			Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
			if (new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground, VisualTreeHelper.GetDpi(textBlock).PixelsPerDip).Width > textBlock.ActualWidth)
			{
				textBlock.ToolTip = new ToolTip
				{
					Content = new TextBlock
					{
						Text = TextTrimmingToolTip.GetTooltipText(textBlock),
						MaxWidth = 300.0,
						TextWrapping = TextWrapping.Wrap
					}
				};
				return;
			}
			textBlock.ToolTip = null;
		}

		// Token: 0x04000987 RID: 2439
		public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(TextTrimmingToolTip), new PropertyMetadata(false, new PropertyChangedCallback(TextTrimmingToolTip.OnEnabledChanged)));

		// Token: 0x04000988 RID: 2440
		public static readonly DependencyProperty TooltipTextProperty = DependencyProperty.RegisterAttached("TooltipText", typeof(string), typeof(TextTrimmingToolTip), new PropertyMetadata(null, new PropertyChangedCallback(TextTrimmingToolTip.OnTooltipTextChanged)));
	}
}
