using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Interactivity;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B5 RID: 181
	internal class NumberTextBoxBehavior : Behavior<TextBox>
	{
		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x00038ABD File Offset: 0x00036CBD
		// (set) Token: 0x06000AF1 RID: 2801 RVA: 0x00038ACF File Offset: 0x00036CCF
		public int Minimum
		{
			get
			{
				return (int)base.GetValue(NumberTextBoxBehavior.MinimumProperty);
			}
			set
			{
				base.SetValue(NumberTextBoxBehavior.MinimumProperty, value);
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x00038AE2 File Offset: 0x00036CE2
		// (set) Token: 0x06000AF3 RID: 2803 RVA: 0x00038AF4 File Offset: 0x00036CF4
		public int Maximum
		{
			get
			{
				return (int)base.GetValue(NumberTextBoxBehavior.MaximumProperty);
			}
			set
			{
				base.SetValue(NumberTextBoxBehavior.MaximumProperty, value);
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00038B08 File Offset: 0x00036D08
		protected override void OnAttached()
		{
			base.AssociatedObject.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
			base.AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(this.HandlePreviewKeyDownEvent));
			base.AssociatedObject.AddHandler(UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(this.HandlePreviewTextInputEvent));
			base.AssociatedObject.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(this.HandleLostFocusEvent));
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00038B80 File Offset: 0x00036D80
		protected override void OnDetaching()
		{
			base.AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(this.HandlePreviewKeyDownEvent));
			base.AssociatedObject.RemoveHandler(UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(this.HandlePreviewTextInputEvent));
			base.AssociatedObject.RemoveHandler(UIElement.LostFocusEvent, new RoutedEventHandler(this.HandleLostFocusEvent));
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00038BE1 File Offset: 0x00036DE1
		private void HandlePreviewKeyDownEvent(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.Update();
			}
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00038BF2 File Offset: 0x00036DF2
		private void HandlePreviewTextInputEvent(object sender, TextCompositionEventArgs e)
		{
			if (!this.CheckInput(e.Text))
			{
				e.Handled = true;
			}
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00038C09 File Offset: 0x00036E09
		private void HandleLostFocusEvent(object sender, RoutedEventArgs e)
		{
			this.Update();
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00038C11 File Offset: 0x00036E11
		private bool CheckInput(string text)
		{
			return NumberTextBoxBehavior.digitRegex.IsMatch(text) || (text.Contains("-") && this.Minimum < 0);
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x00038C3C File Offset: 0x00036E3C
		private void Update()
		{
			BindingExpression bindingExpression = base.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
			int num;
			if (!int.TryParse(base.AssociatedObject.Text, out num) || num > this.Maximum || num < this.Minimum)
			{
				this.RestoreDefaults(bindingExpression);
				return;
			}
			bindingExpression.UpdateSource();
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00038C90 File Offset: 0x00036E90
		private void RestoreDefaults(BindingExpression binding)
		{
			base.AssociatedObject.Text = binding.ResolvedSource.GetType().GetProperty(binding.ResolvedSourcePropertyName).GetValue(binding.ResolvedSource)
				.ToString();
			base.AssociatedObject.Select(base.AssociatedObject.Text.Length, 0);
		}

		// Token: 0x040004C2 RID: 1218
		private static readonly Regex digitRegex = new Regex("\\d+");

		// Token: 0x040004C3 RID: 1219
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumberTextBoxBehavior), new PropertyMetadata(0));

		// Token: 0x040004C4 RID: 1220
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumberTextBoxBehavior), new PropertyMetadata(0));
	}
}
