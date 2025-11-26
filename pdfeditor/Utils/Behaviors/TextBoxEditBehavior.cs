using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace pdfeditor.Utils.Behaviors
{
	// Token: 0x02000120 RID: 288
	public class TextBoxEditBehavior : Behavior<TextBox>
	{
		// Token: 0x06000CF9 RID: 3321 RVA: 0x00041D20 File Offset: 0x0003FF20
		protected override void OnAttached()
		{
			base.OnAttached();
			this.innerSet = true;
			this.Text = base.AssociatedObject.Text;
			this.innerSet = false;
			base.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
			base.AssociatedObject.LostFocus += this.AssociatedObject_LostFocus;
			base.AssociatedObject.PreviewKeyDown += this.AssociatedObject_PreviewKeyDown;
			this.UpdateRootVisualEvent();
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00041DA0 File Offset: 0x0003FFA0
		protected override void OnDetaching()
		{
			if (this.rootVisual != null)
			{
				this.rootVisual.PreviewMouseDown -= this.RootVisual_PreviewMouseDown;
				this.rootVisual = null;
			}
			base.AssociatedObject.Loaded -= this.AssociatedObject_Loaded;
			base.AssociatedObject.LostFocus -= this.AssociatedObject_LostFocus;
			base.AssociatedObject.PreviewKeyDown -= this.AssociatedObject_PreviewKeyDown;
			base.OnDetaching();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00041E1E File Offset: 0x0004001E
		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdateRootVisualEvent();
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00041E28 File Offset: 0x00040028
		private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			bool flag = e.Key == Key.Return;
			TextBox associatedObject = base.AssociatedObject;
			if (associatedObject != null && associatedObject.AcceptsReturn)
			{
				flag = false;
			}
			if (e.Key == Key.Escape || flag)
			{
				e.Handled = true;
				this.Apply();
			}
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00041E70 File Offset: 0x00040070
		public void Apply()
		{
			if (base.AssociatedObject.IsMouseCaptured)
			{
				base.AssociatedObject.ReleaseMouseCapture();
			}
			this.Text = base.AssociatedObject.Text;
			if (base.AssociatedObject.IsFocused)
			{
				TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
				if (!base.AssociatedObject.MoveFocus(traversalRequest))
				{
					Keyboard.ClearFocus();
				}
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00041ECD File Offset: 0x000400CD
		private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
		{
			this.Apply();
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x00041ED8 File Offset: 0x000400D8
		private void RootVisual_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			TextBox associatedObject = base.AssociatedObject;
			if (associatedObject == null)
			{
				return;
			}
			Point position = e.GetPosition(associatedObject);
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(associatedObject, position);
			if (((hitTestResult != null) ? hitTestResult.VisualHit : null) == null)
			{
				this.Apply();
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x00041F14 File Offset: 0x00040114
		private void UpdateRootVisualEvent()
		{
			if (this.rootVisual != null)
			{
				this.rootVisual.PreviewMouseDown -= this.RootVisual_PreviewMouseDown;
				this.rootVisual = null;
			}
			TextBox associatedObject = base.AssociatedObject;
			if (associatedObject != null && this.ApplyWhenClickEmpty)
			{
				PresentationSource presentationSource = PresentationSource.FromVisual(associatedObject);
				if (presentationSource != null)
				{
					this.rootVisual = presentationSource.RootVisual as UIElement;
				}
				if (this.rootVisual != null)
				{
					this.rootVisual.PreviewMouseDown += this.RootVisual_PreviewMouseDown;
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000D01 RID: 3329 RVA: 0x00041F93 File Offset: 0x00040193
		// (set) Token: 0x06000D02 RID: 3330 RVA: 0x00041FA5 File Offset: 0x000401A5
		public string Text
		{
			get
			{
				return (string)base.GetValue(TextBoxEditBehavior.TextProperty);
			}
			set
			{
				base.SetValue(TextBoxEditBehavior.TextProperty, value);
			}
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x00041FB4 File Offset: 0x000401B4
		private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TextBoxEditBehavior textBoxEditBehavior = d as TextBoxEditBehavior;
			if (textBoxEditBehavior != null)
			{
				if (textBoxEditBehavior.AssociatedObject == null)
				{
					throw new ArgumentException("AssociatedObject");
				}
				if (textBoxEditBehavior.AssociatedObject.Text != (string)e.NewValue)
				{
					textBoxEditBehavior.AssociatedObject.Text = (string)e.NewValue;
				}
				if (!textBoxEditBehavior.innerSet)
				{
					EventHandler textChanged = textBoxEditBehavior.TextChanged;
					if (textChanged == null)
					{
						return;
					}
					textChanged(textBoxEditBehavior, EventArgs.Empty);
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000D04 RID: 3332 RVA: 0x00042031 File Offset: 0x00040231
		// (set) Token: 0x06000D05 RID: 3333 RVA: 0x00042043 File Offset: 0x00040243
		public bool ApplyWhenClickEmpty
		{
			get
			{
				return (bool)base.GetValue(TextBoxEditBehavior.ApplyWhenClickEmptyProperty);
			}
			set
			{
				base.SetValue(TextBoxEditBehavior.ApplyWhenClickEmptyProperty, value);
			}
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000D06 RID: 3334 RVA: 0x00042058 File Offset: 0x00040258
		// (remove) Token: 0x06000D07 RID: 3335 RVA: 0x00042090 File Offset: 0x00040290
		public event EventHandler TextChanged;

		// Token: 0x0400059C RID: 1436
		private bool innerSet;

		// Token: 0x0400059D RID: 1437
		private UIElement rootVisual;

		// Token: 0x0400059E RID: 1438
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBoxEditBehavior), new PropertyMetadata("", new PropertyChangedCallback(TextBoxEditBehavior.OnTextPropertyChanged)));

		// Token: 0x0400059F RID: 1439
		public static readonly DependencyProperty ApplyWhenClickEmptyProperty = DependencyProperty.Register("ApplyWhenClickEmpty", typeof(bool), typeof(TextBoxEditBehavior), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TextBoxEditBehavior textBoxEditBehavior = s as TextBoxEditBehavior;
			if (textBoxEditBehavior != null && !object.Equals(a.NewValue, a.OldValue))
			{
				textBoxEditBehavior.UpdateRootVisualEvent();
			}
		}));
	}
}
