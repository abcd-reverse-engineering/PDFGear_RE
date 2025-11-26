using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000265 RID: 613
	internal static class ToolbarButtonHelper
	{
		// Token: 0x06002384 RID: 9092 RVA: 0x000A68D8 File Offset: 0x000A4AD8
		public static bool IsContentMouseOver(ButtonBase button, MouseEventArgs e)
		{
			if (button != null && button.IsHitTestVisible)
			{
				bool flag = true;
				FrameworkElement frameworkElement = button.Content as FrameworkElement;
				if (frameworkElement == null)
				{
					frameworkElement = null;
					FrameworkElement frameworkElement2 = ((VisualTreeHelper.GetChildrenCount(button) > 0) ? (VisualTreeHelper.GetChild(button, 0) as FrameworkElement) : null);
					ContentPresenter contentPresenter = ((frameworkElement2 != null) ? frameworkElement2.FindName("contentPresenter") : null) as ContentPresenter;
					if (contentPresenter != null)
					{
						frameworkElement = ((VisualTreeHelper.GetChildrenCount(contentPresenter) > 0) ? (VisualTreeHelper.GetChild(contentPresenter, 0) as FrameworkElement) : null);
					}
				}
				if (frameworkElement is TextBlock)
				{
					object obj = frameworkElement.ReadLocalValue(ToolbarButtonHelper.OverrideMouseOverProperty);
					if (obj is bool)
					{
						bool flag2 = (bool)obj;
						flag = flag2;
					}
					else
					{
						flag = false;
					}
				}
				else if (frameworkElement != null)
				{
					flag = ToolbarButtonHelper.GetOverrideMouseOver(frameworkElement);
				}
				if (frameworkElement != null && frameworkElement.IsHitTestVisible && flag)
				{
					Point position = e.GetPosition(frameworkElement);
					Rect rect = new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight);
					return rect.Contains(position);
				}
			}
			return false;
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000A69D8 File Offset: 0x000A4BD8
		public static void UpdateContentStates(ButtonBase button)
		{
			if (button is ToolbarButton || button is ToolbarToggleButton || button is ToolbarRadioButton)
			{
				if (button.ContentTemplate == null && button.Content is string)
				{
					VisualStateManager.GoToState(button, "ContentIsText", true);
					return;
				}
				VisualStateManager.GoToState(button, "ContentIsElement", true);
			}
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x000A6A30 File Offset: 0x000A4C30
		public static void UpdateHeaderStates(ButtonBase button)
		{
			if (button is ToolbarButton || button is ToolbarToggleButton || button is ToolbarRadioButton)
			{
				bool flag = false;
				bool flag2 = false;
				object value = button.GetValue(ToolbarButton.HeaderProperty);
				object value2 = button.GetValue(ToolbarButton.OrientationProperty);
				Orientation orientation;
				if (value2 is Orientation)
				{
					orientation = (Orientation)value2;
				}
				else
				{
					orientation = Orientation.Vertical;
				}
				object content = button.Content;
				if (value != null)
				{
					flag = true;
				}
				string text = content as string;
				if (text != null && !string.IsNullOrEmpty(text))
				{
					flag2 = true;
				}
				else if (content != null)
				{
					flag2 = true;
				}
				if (flag && flag2)
				{
					VisualStateManager.GoToState(button, orientation.ToString(), true);
					return;
				}
				if (flag)
				{
					VisualStateManager.GoToState(button, "NoContent", true);
					return;
				}
				VisualStateManager.GoToState(button, "NoIcon", true);
			}
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x000A6AF0 File Offset: 0x000A4CF0
		public static void UpdateDropDownIconState(ButtonBase button)
		{
			if (button is ToolbarChildButton || button is ToolbarChildToggleButton)
			{
				object value = button.GetValue(ToolbarButtonHelper.IsDropDownIconVisibleProperty);
				bool flag = value is bool && (bool)value;
				VisualStateManager.GoToState(button, flag ? "DropDownIconVisible" : "DropDownIconNotVisible", true);
			}
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x000A6B44 File Offset: 0x000A4D44
		private static ContentPresenter GetButtonHeaderElement(ButtonBase button)
		{
			if (button != null && VisualTreeHelper.GetChildrenCount(button) > 0)
			{
				FrameworkElement frameworkElement = VisualTreeHelper.GetChild(button, 0) as FrameworkElement;
				if (frameworkElement != null)
				{
					return frameworkElement.FindName("HeaderPresenter") as ContentPresenter;
				}
			}
			return null;
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x000A6B80 File Offset: 0x000A4D80
		private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ButtonBase buttonBase = d as ButtonBase;
				if (buttonBase != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(buttonBase);
				}
			}
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x000A6BB4 File Offset: 0x000A4DB4
		private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ButtonBase buttonBase = d as ButtonBase;
				if (buttonBase != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(buttonBase);
				}
			}
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x000A6BE6 File Offset: 0x000A4DE6
		public static Brush GetIndicatorBrush(DependencyObject obj)
		{
			return (Brush)obj.GetValue(ToolbarButtonHelper.IndicatorBrushProperty);
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x000A6BF8 File Offset: 0x000A4DF8
		public static void SetIndicatorBrush(DependencyObject obj, Brush value)
		{
			obj.SetValue(ToolbarButtonHelper.IndicatorBrushProperty, value);
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x000A6C06 File Offset: 0x000A4E06
		public static DataTemplate GetHeaderTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(ToolbarButtonHelper.HeaderTemplateProperty);
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x000A6C18 File Offset: 0x000A4E18
		public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(ToolbarButtonHelper.HeaderTemplateProperty, value);
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x000A6C28 File Offset: 0x000A4E28
		private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ButtonBase buttonBase = d as ButtonBase;
				if (buttonBase != null)
				{
					ToolbarButtonHelper.UpdateHeaderStates(buttonBase);
				}
			}
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x000A6C5A File Offset: 0x000A4E5A
		public static bool GetIsMouseOverInternal(DependencyObject obj)
		{
			return (bool)obj.GetValue(ToolbarButtonHelper.IsMouseOverInternalProperty);
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x000A6C6C File Offset: 0x000A4E6C
		public static void SetIsMouseOverInternal(DependencyObject obj, bool value)
		{
			obj.SetValue(ToolbarButtonHelper.IsMouseOverInternalProperty, value);
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x000A6C7F File Offset: 0x000A4E7F
		public static bool GetOverrideMouseOver(DependencyObject obj)
		{
			return (bool)obj.GetValue(ToolbarButtonHelper.OverrideMouseOverProperty);
		}

		// Token: 0x06002393 RID: 9107 RVA: 0x000A6C91 File Offset: 0x000A4E91
		public static void SetOverrideMouseOver(DependencyObject obj, bool value)
		{
			obj.SetValue(ToolbarButtonHelper.OverrideMouseOverProperty, value);
		}

		// Token: 0x06002394 RID: 9108 RVA: 0x000A6CA4 File Offset: 0x000A4EA4
		public static bool GetIsKeyboardFocusedInternal(DependencyObject obj)
		{
			return (bool)obj.GetValue(ToolbarButtonHelper.IsKeyboardFocusedInternalProperty);
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x000A6CB6 File Offset: 0x000A4EB6
		public static void SetIsKeyboardFocusedInternal(DependencyObject obj, bool value)
		{
			obj.SetValue(ToolbarButtonHelper.IsKeyboardFocusedInternalProperty, value);
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x000A6CCC File Offset: 0x000A4ECC
		public static void RegisterIsKeyboardFocused(ButtonBase button)
		{
			if (button == null)
			{
				throw new ArgumentNullException("button");
			}
			ToolbarButtonHelper.SetIsKeyboardFocusedInternal(button, false);
			button.PreviewMouseUp += delegate(object s, MouseButtonEventArgs a)
			{
				ToolbarButtonHelper.SetIsKeyboardFocusedInternal(button, false);
			};
			button.GotFocus += delegate(object s, RoutedEventArgs a)
			{
				if (((FrameworkElement)s).IsMouseOver && (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed || Mouse.MiddleButton == MouseButtonState.Pressed || Mouse.XButton1 == MouseButtonState.Pressed || Mouse.XButton2 == MouseButtonState.Pressed))
				{
					ToolbarButtonHelper.SetIsKeyboardFocusedInternal(button, false);
				}
				else
				{
					ToolbarButtonHelper.SetIsKeyboardFocusedInternal(button, true);
				}
				((FrameworkElement)s).BringIntoView();
			};
			button.LostFocus += delegate(object s, RoutedEventArgs a)
			{
				ToolbarButtonHelper.SetIsKeyboardFocusedInternal(button, false);
			};
		}

		// Token: 0x04000F19 RID: 3865
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(object), typeof(ToolbarButtonHelper), new PropertyMetadata(null, new PropertyChangedCallback(ToolbarButtonHelper.OnHeaderPropertyChanged)));

		// Token: 0x04000F1A RID: 3866
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(ToolbarButtonHelper), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(ToolbarButtonHelper.OnOrientationPropertyChanged)));

		// Token: 0x04000F1B RID: 3867
		public static readonly DependencyProperty IsDropDownIconVisibleProperty = DependencyProperty.RegisterAttached("IsDropDownIconVisible", typeof(bool), typeof(ToolbarButtonHelper), new PropertyMetadata(true));

		// Token: 0x04000F1C RID: 3868
		public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.RegisterAttached("IndicatorBrush", typeof(Brush), typeof(ToolbarButtonHelper), new PropertyMetadata(null));

		// Token: 0x04000F1D RID: 3869
		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(ToolbarButtonHelper), new PropertyMetadata(null, new PropertyChangedCallback(ToolbarButtonHelper.OnHeaderTemplatePropertyChanged)));

		// Token: 0x04000F1E RID: 3870
		public static readonly DependencyProperty IsMouseOverInternalProperty = DependencyProperty.RegisterAttached("IsMouseOverInternal", typeof(bool), typeof(ToolbarButtonHelper), new PropertyMetadata(false));

		// Token: 0x04000F1F RID: 3871
		public static readonly DependencyProperty OverrideMouseOverProperty = DependencyProperty.RegisterAttached("OverrideMouseOver", typeof(bool), typeof(ToolbarButtonHelper), new PropertyMetadata(true));

		// Token: 0x04000F20 RID: 3872
		public static readonly DependencyProperty IsKeyboardFocusedInternalProperty = DependencyProperty.RegisterAttached("IsKeyboardFocusedInternal", typeof(bool), typeof(ToolbarButtonHelper), new PropertyMetadata(false));
	}
}
