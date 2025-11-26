using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace pdfeditor.Utils
{
	// Token: 0x020000A0 RID: 160
	public static class ScrollUtils
	{
		// Token: 0x06000A17 RID: 2583 RVA: 0x00033248 File Offset: 0x00031448
		public static ScrollViewer GetScrollViewer(ListBox listBox)
		{
			if (listBox == null)
			{
				return null;
			}
			if (!listBox.IsLoaded)
			{
				return null;
			}
			if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
			{
				Border border = VisualTreeHelper.GetChild(listBox, 0) as Border;
				ScrollViewer scrollViewer = ((border != null) ? border.Child : null) as ScrollViewer;
				if (scrollViewer != null)
				{
					return scrollViewer;
				}
			}
			return null;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00033294 File Offset: 0x00031494
		public static ScrollViewer GetScrollViewerFromItemContainer(DependencyObject container)
		{
			ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
			if (itemsControl != null && VisualTreeHelper.GetChildrenCount(itemsControl) > 0)
			{
				FrameworkElement frameworkElement = VisualTreeHelper.GetChild(itemsControl, 0) as FrameworkElement;
				if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
				{
					ScrollViewer scrollViewer = VisualTreeHelper.GetChild(frameworkElement, 0) as ScrollViewer;
					if (scrollViewer == null)
					{
						scrollViewer = frameworkElement.FindName("ScrollViewer") as ScrollViewer;
					}
					return scrollViewer;
				}
			}
			return null;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x000332F4 File Offset: 0x000314F4
		public static void SmoothScrollToHorizontalOffset(this ScrollViewer scrollViewer, double offset, double maxSmoothScrollLength = 500.0, IEasingFunction easingFunction = null)
		{
			if (scrollViewer == null)
			{
				return;
			}
			ScrollUtils.TryStopScroll(scrollViewer, Orientation.Horizontal);
			if (offset < 0.0)
			{
				offset = 0.0;
			}
			if (offset > scrollViewer.ScrollableWidth)
			{
				offset = scrollViewer.ScrollableWidth;
			}
			double horizontalOffset = scrollViewer.HorizontalOffset;
			if ((horizontalOffset + 8.0 > offset && horizontalOffset - 8.0 < offset) || Math.Abs(horizontalOffset - offset) > Math.Abs(maxSmoothScrollLength))
			{
				scrollViewer.ScrollToHorizontalOffset(offset);
				return;
			}
			ScrollUtils.SmoothScroll(scrollViewer, offset, easingFunction, Orientation.Horizontal);
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00033378 File Offset: 0x00031578
		public static void SmoothScrollToVerticalOffset(this ScrollViewer scrollViewer, double offset, double maxSmoothScrollLength = 500.0, IEasingFunction easingFunction = null)
		{
			if (scrollViewer == null)
			{
				return;
			}
			ScrollUtils.TryStopScroll(scrollViewer, Orientation.Vertical);
			if (offset < 0.0)
			{
				offset = 0.0;
			}
			if (offset > scrollViewer.ScrollableHeight)
			{
				offset = scrollViewer.ScrollableHeight;
			}
			double verticalOffset = scrollViewer.VerticalOffset;
			if ((verticalOffset + 8.0 > offset && verticalOffset - 8.0 < offset) || Math.Abs(verticalOffset - offset) > Math.Abs(maxSmoothScrollLength))
			{
				scrollViewer.ScrollToVerticalOffset(offset);
				return;
			}
			ScrollUtils.SmoothScroll(scrollViewer, offset, easingFunction, Orientation.Vertical);
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x000333FC File Offset: 0x000315FC
		private static void TryStopScroll(ScrollViewer scrollViewer, Orientation orientation)
		{
			if (scrollViewer == null)
			{
				return;
			}
			if (ScrollUtils.animatingScrollViewers == null)
			{
				return;
			}
			ConcurrentDictionary<WeakReference<ScrollViewer>, Storyboard> concurrentDictionary = ScrollUtils.animatingScrollViewers;
			lock (concurrentDictionary)
			{
				WeakReference<ScrollViewer> weakReference = null;
				foreach (KeyValuePair<WeakReference<ScrollViewer>, Storyboard> keyValuePair in ScrollUtils.animatingScrollViewers)
				{
					ScrollViewer scrollViewer2;
					if (keyValuePair.Key != null && keyValuePair.Key.TryGetTarget(out scrollViewer2) && scrollViewer2 == scrollViewer)
					{
						weakReference = keyValuePair.Key;
						Storyboard value = keyValuePair.Value;
						if (value.GetCurrentState() == ClockState.Active)
						{
							value.Pause();
							if (orientation == Orientation.Horizontal)
							{
								double horizontalOffset = scrollViewer2.HorizontalOffset;
								value.Stop();
								if (scrollViewer2.HorizontalOffset != horizontalOffset)
								{
									scrollViewer2.ScrollToHorizontalOffset(horizontalOffset);
									scrollViewer2.UpdateLayout();
								}
							}
							else
							{
								double verticalOffset = scrollViewer2.VerticalOffset;
								value.Stop();
								if (scrollViewer2.VerticalOffset != verticalOffset)
								{
									scrollViewer2.ScrollToVerticalOffset(verticalOffset);
									scrollViewer2.UpdateLayout();
								}
							}
						}
					}
				}
				if (weakReference != null)
				{
					Storyboard storyboard;
					ScrollUtils.animatingScrollViewers.TryRemove(weakReference, out storyboard);
				}
			}
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00033530 File Offset: 0x00031730
		private static void SmoothScroll(ScrollViewer scrollViewer, double offset, IEasingFunction easingFunction, Orientation orientation)
		{
			IEasingFunction easingFunction2 = easingFunction ?? ScrollUtils.<SmoothScroll>g__CreateDefaultEasingFunction|7_3();
			Orientation orientation2 = orientation;
			double num;
			Action<double> action;
			if (orientation2 != Orientation.Horizontal && orientation2 == Orientation.Vertical)
			{
				num = scrollViewer.VerticalOffset;
				action = delegate(double d)
				{
					scrollViewer.ScrollToVerticalOffset(d);
				};
			}
			else
			{
				num = scrollViewer.HorizontalOffset;
				action = delegate(double d)
				{
					scrollViewer.ScrollToHorizontalOffset(d);
				};
			}
			ScrollUtils.ValueWrapper<double> valueWrapper = new ScrollUtils.ValueWrapper<double>(num, action);
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				To = new double?(offset),
				FillBehavior = FillBehavior.HoldEnd,
				EasingFunction = easingFunction2,
				Duration = new Duration(TimeSpan.FromSeconds(0.15))
			};
			Storyboard.SetTarget(doubleAnimation, valueWrapper);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Value", Array.Empty<object>()));
			Storyboard storyboard = new Storyboard
			{
				Children = { doubleAnimation }
			};
			if (ScrollUtils.animatingScrollViewers == null)
			{
				ScrollUtils.animatingScrollViewers = new ConcurrentDictionary<WeakReference<ScrollViewer>, Storyboard>();
			}
			ScrollUtils.animatingScrollViewers[new WeakReference<ScrollViewer>(scrollViewer)] = storyboard;
			storyboard.Completed += delegate(object s, EventArgs a)
			{
				ScrollUtils.TryStopScroll(scrollViewer, orientation);
			};
			storyboard.Begin();
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00033665 File Offset: 0x00031865
		[CompilerGenerated]
		internal static IEasingFunction <SmoothScroll>g__CreateDefaultEasingFunction|7_3()
		{
			if (ScrollUtils.defaultSmoothScrollEasingFunction == null)
			{
				ExponentialEase exponentialEase = new ExponentialEase();
				exponentialEase.EasingMode = EasingMode.EaseOut;
				exponentialEase.Exponent = 7.0;
				exponentialEase.Freeze();
				ScrollUtils.defaultSmoothScrollEasingFunction = exponentialEase;
			}
			return ScrollUtils.defaultSmoothScrollEasingFunction;
		}

		// Token: 0x04000487 RID: 1159
		private static ExponentialEase defaultSmoothScrollEasingFunction;

		// Token: 0x04000488 RID: 1160
		private static ConcurrentDictionary<WeakReference<ScrollViewer>, Storyboard> animatingScrollViewers;

		// Token: 0x0200049A RID: 1178
		private class ValueWrapper<T> : DependencyObject
		{
			// Token: 0x06002E1F RID: 11807 RVA: 0x000E184C File Offset: 0x000DFA4C
			public ValueWrapper(T initValue, Action<T> action)
			{
				this.Value = initValue;
				this.action = action;
			}

			// Token: 0x17000CB6 RID: 3254
			// (get) Token: 0x06002E20 RID: 11808 RVA: 0x000E1862 File Offset: 0x000DFA62
			// (set) Token: 0x06002E21 RID: 11809 RVA: 0x000E1874 File Offset: 0x000DFA74
			public T Value
			{
				get
				{
					return (T)((object)base.GetValue(ScrollUtils.ValueWrapper<T>.ValueProperty));
				}
				set
				{
					base.SetValue(ScrollUtils.ValueWrapper<T>.ValueProperty, value);
				}
			}

			// Token: 0x06002E22 RID: 11810 RVA: 0x000E1888 File Offset: 0x000DFA88
			private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
			{
				if (!object.Equals(e.NewValue, e.OldValue))
				{
					ScrollUtils.ValueWrapper<T> valueWrapper = d as ScrollUtils.ValueWrapper<T>;
					if (valueWrapper != null)
					{
						Action<T> action = valueWrapper.action;
						if (action == null)
						{
							return;
						}
						action((T)((object)e.NewValue));
					}
				}
			}

			// Token: 0x04001A0D RID: 6669
			private readonly Action<T> action;

			// Token: 0x04001A0E RID: 6670
			public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(ScrollUtils.ValueWrapper<T>), new PropertyMetadata(default(T), new PropertyChangedCallback(ScrollUtils.ValueWrapper<T>.OnValuePropertyChanged)));
		}
	}
}
