using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Interactivity;

namespace pdfconverter.Utils.Image
{
	// Token: 0x0200004B RID: 75
	internal class NumberTextBoxBehavior : Behavior<TextBox>
	{
		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x00015790 File Offset: 0x00013990
		// (set) Token: 0x06000536 RID: 1334 RVA: 0x000157A2 File Offset: 0x000139A2
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

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x000157B5 File Offset: 0x000139B5
		// (set) Token: 0x06000538 RID: 1336 RVA: 0x000157C7 File Offset: 0x000139C7
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

		// Token: 0x06000539 RID: 1337 RVA: 0x000157DC File Offset: 0x000139DC
		protected override void OnAttached()
		{
			base.AssociatedObject.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
			base.AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(this.HandlePreviewKeyDownEvent));
			base.AssociatedObject.AddHandler(UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(this.HandlePreviewTextInputEvent));
			base.AssociatedObject.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(this.HandleLostFocusEvent));
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x00015854 File Offset: 0x00013A54
		protected override void OnDetaching()
		{
			base.AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(this.HandlePreviewKeyDownEvent));
			base.AssociatedObject.RemoveHandler(UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(this.HandlePreviewTextInputEvent));
			base.AssociatedObject.RemoveHandler(UIElement.LostFocusEvent, new RoutedEventHandler(this.HandleLostFocusEvent));
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x000158B5 File Offset: 0x00013AB5
		private void HandlePreviewKeyDownEvent(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.Update();
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x000158C6 File Offset: 0x00013AC6
		private void HandlePreviewTextInputEvent(object sender, TextCompositionEventArgs e)
		{
			if (!this.CheckInput(e.Text))
			{
				e.Handled = true;
			}
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x000158DD File Offset: 0x00013ADD
		private void HandleLostFocusEvent(object sender, RoutedEventArgs e)
		{
			this.Update();
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x000158E5 File Offset: 0x00013AE5
		private bool CheckInput(string text)
		{
			return NumberTextBoxBehavior.digitRegex.IsMatch(text) || (text.Contains("-") && this.Minimum < 0);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00015910 File Offset: 0x00013B10
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

		// Token: 0x06000540 RID: 1344 RVA: 0x00015964 File Offset: 0x00013B64
		private void RestoreDefaults(BindingExpression binding)
		{
			base.AssociatedObject.Text = binding.ResolvedSource.GetType().GetProperty(binding.ResolvedSourcePropertyName).GetValue(binding.ResolvedSource)
				.ToString();
			base.AssociatedObject.Select(base.AssociatedObject.Text.Length, 0);
		}

		// Token: 0x0400027A RID: 634
		private static readonly Regex digitRegex = new Regex("\\d+");

		// Token: 0x0400027B RID: 635
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumberTextBoxBehavior), new PropertyMetadata(0));

		// Token: 0x0400027C RID: 636
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumberTextBoxBehavior), new PropertyMetadata(0));
	}
}
