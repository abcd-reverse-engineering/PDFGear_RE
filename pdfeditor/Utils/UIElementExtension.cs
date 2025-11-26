using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using CommonLib.Common;

namespace pdfeditor.Utils
{
	// Token: 0x020000AE RID: 174
	public static class UIElementExtension
	{
		// Token: 0x06000AC6 RID: 2758 RVA: 0x0003839D File Offset: 0x0003659D
		public static UIElement GetFirstVisualChild(DependencyObject parent)
		{
			if (parent == null)
			{
				return null;
			}
			if (VisualTreeHelper.GetChildrenCount(parent) > 0)
			{
				return VisualTreeHelper.GetChild(parent, 0) as UIElement;
			}
			return null;
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x000383BB File Offset: 0x000365BB
		public static T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
		{
			return UIElementExtension.FindVisualChildCore(parent, typeof(T), name) as T;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x000383D8 File Offset: 0x000365D8
		public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
		{
			return UIElementExtension.FindVisualChild<T>(parent, string.Empty);
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x000383E8 File Offset: 0x000365E8
		private static DependencyObject FindVisualChildCore(DependencyObject parent, Type type, string name)
		{
			if (parent == null)
			{
				return null;
			}
			if (!typeof(DependencyObject).IsAssignableFrom(type))
			{
				return null;
			}
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				if (child == null)
				{
					return null;
				}
				DependencyObject dependencyObject = null;
				if (type.IsAssignableFrom(child.GetType()))
				{
					if (string.IsNullOrEmpty(name))
					{
						dependencyObject = child;
					}
					else
					{
						FrameworkElement frameworkElement = child as FrameworkElement;
						if (frameworkElement == null || !(frameworkElement.Name == name))
						{
							FrameworkContentElement frameworkContentElement = child as FrameworkContentElement;
							if (frameworkContentElement == null || !(frameworkContentElement.Name == name))
							{
								goto IL_0083;
							}
						}
						dependencyObject = child;
					}
				}
				IL_0083:
				if (dependencyObject == null)
				{
					dependencyObject = UIElementExtension.FindVisualChildCore(child, type, name);
				}
				if (dependencyObject != null)
				{
					return dependencyObject;
				}
			}
			return null;
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x00038494 File Offset: 0x00036694
		public static ContextMenu GetExtendContextMenuDataContext(DependencyObject obj)
		{
			return (ContextMenu)obj.GetValue(UIElementExtension.ExtendContextMenuDataContextProperty);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x000384A6 File Offset: 0x000366A6
		public static void SetExtendContextMenuDataContext(DependencyObject obj, ContextMenu value)
		{
			obj.SetValue(UIElementExtension.ExtendContextMenuDataContextProperty, value);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x000384B4 File Offset: 0x000366B4
		public static object GetTraceClickEventTag(DependencyObject obj)
		{
			return obj.GetValue(UIElementExtension.TraceClickEventTagProperty);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x000384C1 File Offset: 0x000366C1
		public static void SetTraceClickEventTag(DependencyObject obj, object value)
		{
			obj.SetValue(UIElementExtension.TraceClickEventTagProperty, value);
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x000384CF File Offset: 0x000366CF
		public static string GetTraceClickEventFormat(DependencyObject obj)
		{
			return (string)obj.GetValue(UIElementExtension.TraceClickEventFormatProperty);
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x000384E1 File Offset: 0x000366E1
		public static void SetTraceClickEventFormat(DependencyObject obj, string value)
		{
			obj.SetValue(UIElementExtension.TraceClickEventFormatProperty, value);
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x000384F0 File Offset: 0x000366F0
		private static void OnTraceClickEventFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ButtonBase buttonBase = d as ButtonBase;
				if (buttonBase != null)
				{
					buttonBase.Click -= UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Btn_Click|13_1;
					if (e.NewValue is string)
					{
						buttonBase.Click += UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Btn_Click|13_1;
						return;
					}
				}
				else
				{
					FrameworkElement frameworkElement = d as FrameworkElement;
					if (frameworkElement != null)
					{
						frameworkElement.PreviewMouseDown -= UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Ele_PreviewMouseDown|13_2;
						if (e.NewValue is string)
						{
							frameworkElement.PreviewMouseDown += UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Ele_PreviewMouseDown|13_2;
						}
					}
				}
			}
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0003858B File Offset: 0x0003678B
		public static double GetRadius(DependencyObject obj)
		{
			return (double)obj.GetValue(UIElementExtension.RadiusProperty);
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0003859D File Offset: 0x0003679D
		public static void SetRadius(DependencyObject obj, double value)
		{
			obj.SetValue(UIElementExtension.RadiusProperty, value);
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x000385B0 File Offset: 0x000367B0
		public static CornerRadius GetCornerRadius(DependencyObject obj)
		{
			return (CornerRadius)obj.GetValue(UIElementExtension.CornerRadiusProperty);
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x000385C2 File Offset: 0x000367C2
		public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
		{
			obj.SetValue(UIElementExtension.CornerRadiusProperty, value);
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x000386EC File Offset: 0x000368EC
		[CompilerGenerated]
		internal static void <OnTraceClickEventFormatPropertyChanged>g__OnElementClick|13_0(FrameworkElement _ele, string _format)
		{
			Log.WriteLog(UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__GetElementEventMessage|13_4(_ele, _format));
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x000386FC File Offset: 0x000368FC
		[CompilerGenerated]
		internal static void <OnTraceClickEventFormatPropertyChanged>g__Btn_Click|13_1(object sender, RoutedEventArgs _e)
		{
			ButtonBase buttonBase = sender as ButtonBase;
			if (buttonBase != null)
			{
				string traceClickEventFormat = UIElementExtension.GetTraceClickEventFormat(buttonBase);
				if (string.IsNullOrEmpty(traceClickEventFormat))
				{
					buttonBase.Click -= UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Btn_Click|13_1;
					return;
				}
				UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__OnElementClick|13_0(buttonBase, traceClickEventFormat);
			}
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0003873C File Offset: 0x0003693C
		[CompilerGenerated]
		internal static void <OnTraceClickEventFormatPropertyChanged>g__Ele_PreviewMouseDown|13_2(object sender, MouseButtonEventArgs _e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			if (frameworkElement != null)
			{
				string traceClickEventFormat = UIElementExtension.GetTraceClickEventFormat(frameworkElement);
				if (string.IsNullOrEmpty(traceClickEventFormat))
				{
					frameworkElement.PreviewMouseDown -= UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__Ele_PreviewMouseDown|13_2;
					return;
				}
				UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__OnElementClick|13_0(frameworkElement, traceClickEventFormat);
			}
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0003877C File Offset: 0x0003697C
		[CompilerGenerated]
		internal static string <OnTraceClickEventFormatPropertyChanged>g__GetViewTypeName|13_3(FrameworkElement _element)
		{
			Window window = Window.GetWindow(_element);
			if (window != null)
			{
				return window.GetType().Name;
			}
			return string.Empty;
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x000387A4 File Offset: 0x000369A4
		[CompilerGenerated]
		internal static string <OnTraceClickEventFormatPropertyChanged>g__GetElementEventMessage|13_4(FrameworkElement _element, string _format)
		{
			return Regex.Replace(_format, "\\$\\{(.+?)\\}", delegate(Match m)
			{
				if (m.Success && m.Groups.Count > 1)
				{
					string text = m.Groups[1].Value.ToUpperInvariant();
					if (!(text == "VIEW"))
					{
						if (!(text == "NAME"))
						{
							if (!(text == "TYPE"))
							{
								if (text == "CONTENT")
								{
									object obj = UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__GetContent|13_5(_element);
									string text2 = ((obj != null) ? obj.ToString() : null) ?? "null";
									if (string.IsNullOrEmpty(text2))
									{
										text2 = "empty";
									}
									return text2;
								}
								if (text == "TAG")
								{
									object tag = _element.Tag;
									string text3 = ((tag != null) ? tag.ToString() : null) ?? "null";
									if (string.IsNullOrEmpty(text3))
									{
										text3 = "empty";
									}
									return text3;
								}
								if (text == "TRACETAG")
								{
									string text4 = UIElementExtension.GetTraceClickEventTag(_element).ToString() ?? "null";
									if (string.IsNullOrEmpty(text4))
									{
										text4 = "empty";
									}
									return text4;
								}
							}
							else
							{
								string name = _element.GetType().Name;
								if (!string.IsNullOrEmpty(name))
								{
									return name;
								}
								return m.Value;
							}
						}
						else
						{
							string name2 = _element.Name;
							if (!string.IsNullOrEmpty(name2))
							{
								return name2;
							}
							return m.Value;
						}
					}
					else
					{
						string text5 = UIElementExtension.<OnTraceClickEventFormatPropertyChanged>g__GetViewTypeName|13_3(_element);
						if (!string.IsNullOrEmpty(text5))
						{
							return text5;
						}
						return m.Value;
					}
				}
				return m.Value;
			});
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x000387D8 File Offset: 0x000369D8
		[CompilerGenerated]
		internal static object <OnTraceClickEventFormatPropertyChanged>g__GetContent|13_5(FrameworkElement _element)
		{
			if (_element == null)
			{
				return null;
			}
			ContentControl contentControl = _element as ContentControl;
			if (contentControl != null)
			{
				return contentControl.Content;
			}
			ContentPresenter contentPresenter = _element as ContentPresenter;
			if (contentPresenter != null)
			{
				return contentPresenter.Content;
			}
			return null;
		}

		// Token: 0x040004B9 RID: 1209
		public static readonly DependencyProperty ExtendContextMenuDataContextProperty = DependencyProperty.RegisterAttached("ExtendContextMenuDataContext", typeof(ContextMenu), typeof(UIElementExtension), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			FrameworkElement frameworkElement = s as FrameworkElement;
			if (frameworkElement != null)
			{
				ContextMenu contextMenu = a.OldValue as ContextMenu;
				if (contextMenu != null)
				{
					contextMenu.DataContext = null;
				}
				ContextMenu contextMenu2 = a.NewValue as ContextMenu;
				if (contextMenu2 != null)
				{
					contextMenu2.DataContext = frameworkElement;
				}
			}
		}));

		// Token: 0x040004BA RID: 1210
		public static readonly DependencyProperty TraceClickEventTagProperty = DependencyProperty.RegisterAttached("TraceClickEventTag", typeof(object), typeof(UIElementExtension), new PropertyMetadata(null));

		// Token: 0x040004BB RID: 1211
		public static readonly DependencyProperty TraceClickEventFormatProperty = DependencyProperty.RegisterAttached("TraceClickEventFormat", typeof(string), typeof(UIElementExtension), new PropertyMetadata(string.Empty, new PropertyChangedCallback(UIElementExtension.OnTraceClickEventFormatPropertyChanged)));

		// Token: 0x040004BC RID: 1212
		public static readonly DependencyProperty RadiusProperty = DependencyProperty.RegisterAttached("Radius", typeof(double), typeof(UIElementExtension), new PropertyMetadata(0.0));

		// Token: 0x040004BD RID: 1213
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(UIElementExtension), new PropertyMetadata(default(CornerRadius)));
	}
}
