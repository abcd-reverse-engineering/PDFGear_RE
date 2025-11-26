using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000266 RID: 614
	public class ToolbarChildToggleButton : ToggleButton
	{
		// Token: 0x06002398 RID: 9112 RVA: 0x000A6EE0 File Offset: 0x000A50E0
		static ToolbarChildToggleButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarChildToggleButton), new FrameworkPropertyMetadata(typeof(ToolbarChildToggleButton)));
			ToolbarChildToggleButton.readyToShowContextElements = new List<WeakReference<ToolbarChildToggleButton>>();
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x000A6FA0 File Offset: 0x000A51A0
		public ToolbarChildToggleButton()
		{
			base.Loaded += this.ToolbarChildToggleButton_Loaded;
			base.Unloaded += this.ToolbarChildToggleButton_Unloaded;
			base.IsVisibleChanged += this.ToolbarChildToggleButton_IsVisibleChanged;
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x0600239A RID: 9114 RVA: 0x000A6FEF File Offset: 0x000A51EF
		// (set) Token: 0x0600239B RID: 9115 RVA: 0x000A6FF8 File Offset: 0x000A51F8
		private ContextMenu InnerContextMenu
		{
			get
			{
				return this.innerContextMenu;
			}
			set
			{
				if (this.innerContextMenu != value)
				{
					if (this.innerContextMenu != null)
					{
						this.innerContextMenu.Opened -= this.ContextMenu_Opened;
						this.innerContextMenu.Closed -= this.ContextMenu_Closed;
					}
					this.attached = false;
					this.innerContextMenu = value;
					if (this.innerContextMenu != null)
					{
						this.innerContextMenu.Opened += this.ContextMenu_Opened;
						this.innerContextMenu.Closed += this.ContextMenu_Closed;
					}
					this.UpdateOpenContextMenuOnChecked();
				}
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x0600239C RID: 9116 RVA: 0x000A7091 File Offset: 0x000A5291
		// (set) Token: 0x0600239D RID: 9117 RVA: 0x000A709C File Offset: 0x000A529C
		private Rectangle Indicator
		{
			get
			{
				return this.indicator;
			}
			set
			{
				if (this.indicator != value)
				{
					if (this.indicator != null)
					{
						this.indicator.SizeChanged -= this.Indicator_SizeChanged;
					}
					this.indicator = value;
					if (this.indicator != null)
					{
						this.indicator.SizeChanged += this.Indicator_SizeChanged;
					}
					this.UpdateIndicatorSize();
				}
			}
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x000A70FD File Offset: 0x000A52FD
		private void Indicator_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateIndicatorSize();
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x000A7108 File Offset: 0x000A5308
		private void UpdateIndicatorSize()
		{
			if (this.Indicator != null)
			{
				RectangleGeometry rectangleGeometry = this.Indicator.Clip as RectangleGeometry;
				if (rectangleGeometry == null || rectangleGeometry.IsFrozen)
				{
					rectangleGeometry = new RectangleGeometry();
					this.Indicator.Clip = rectangleGeometry;
				}
				rectangleGeometry.Rect = new Rect(0.0, this.Indicator.ActualHeight / 2.0, this.Indicator.ActualWidth, this.Indicator.ActualHeight / 2.0);
			}
		}

		// Token: 0x060023A0 RID: 9120 RVA: 0x000A7194 File Offset: 0x000A5394
		private void ToolbarChildToggleButton_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.contextMenuPropertyDesc != null)
			{
				this.contextMenuPropertyDesc.RemoveValueChanged(this, new EventHandler(this.OnContextMenuPropertyChanged));
				this.contextMenuPropertyDesc = null;
			}
			this.contextMenuPropertyDesc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ContextMenuProperty, typeof(FrameworkElement));
			this.contextMenuPropertyDesc.AddValueChanged(this, new EventHandler(this.OnContextMenuPropertyChanged));
			this.InnerContextMenu = base.ContextMenu;
		}

		// Token: 0x060023A1 RID: 9121 RVA: 0x000A7206 File Offset: 0x000A5406
		private void ToolbarChildToggleButton_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.contextMenuPropertyDesc != null)
			{
				this.contextMenuPropertyDesc.RemoveValueChanged(this, new EventHandler(this.OnContextMenuPropertyChanged));
				this.contextMenuPropertyDesc = null;
			}
			this.InnerContextMenu = null;
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x000A7238 File Offset: 0x000A5438
		private void ToolbarChildToggleButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!base.IsVisible)
			{
				this.attached = false;
				if (base.IsChecked.GetValueOrDefault())
				{
					base.IsChecked = new bool?(false);
				}
				ContextMenu contextMenu = this.InnerContextMenu;
				if (contextMenu != null)
				{
					contextMenu.IsOpen = false;
				}
			}
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x000A7281 File Offset: 0x000A5481
		private void OnContextMenuPropertyChanged(object sender, EventArgs e)
		{
			this.InnerContextMenu = base.ContextMenu;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x000A728F File Offset: 0x000A548F
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Indicator = base.GetTemplateChild("Indicator") as Rectangle;
			ToolbarButtonHelper.UpdateDropDownIconState(this);
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x060023A5 RID: 9125 RVA: 0x000A72B3 File Offset: 0x000A54B3
		// (set) Token: 0x060023A6 RID: 9126 RVA: 0x000A72C5 File Offset: 0x000A54C5
		public bool OpenContextMenuOnChecked
		{
			get
			{
				return (bool)base.GetValue(ToolbarChildToggleButton.OpenContextMenuOnCheckedProperty);
			}
			set
			{
				base.SetValue(ToolbarChildToggleButton.OpenContextMenuOnCheckedProperty, value);
			}
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x000A72D8 File Offset: 0x000A54D8
		private static void OnOpenContextMenuOnCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarChildToggleButton toolbarChildToggleButton = d as ToolbarChildToggleButton;
				if (toolbarChildToggleButton != null)
				{
					toolbarChildToggleButton.UpdateOpenContextMenuOnChecked();
				}
			}
		}

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x060023A8 RID: 9128 RVA: 0x000A730F File Offset: 0x000A550F
		// (set) Token: 0x060023A9 RID: 9129 RVA: 0x000A7321 File Offset: 0x000A5521
		public bool IsDropDownIconVisible
		{
			get
			{
				return (bool)base.GetValue(ToolbarChildToggleButton.IsDropDownIconVisibleProperty);
			}
			set
			{
				base.SetValue(ToolbarChildToggleButton.IsDropDownIconVisibleProperty, value);
			}
		}

		// Token: 0x060023AA RID: 9130 RVA: 0x000A7334 File Offset: 0x000A5534
		private static void OnIsDropDownIconVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarChildToggleButton toolbarChildToggleButton = d as ToolbarChildToggleButton;
				if (toolbarChildToggleButton != null)
				{
					ToolbarButtonHelper.UpdateDropDownIconState(toolbarChildToggleButton);
				}
			}
		}

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x060023AB RID: 9131 RVA: 0x000A736B File Offset: 0x000A556B
		// (set) Token: 0x060023AC RID: 9132 RVA: 0x000A737D File Offset: 0x000A557D
		public Brush IndicatorBrush
		{
			get
			{
				return (Brush)base.GetValue(ToolbarChildToggleButton.IndicatorBrushProperty);
			}
			set
			{
				base.SetValue(ToolbarChildToggleButton.IndicatorBrushProperty, value);
			}
		}

		// Token: 0x060023AD RID: 9133 RVA: 0x000A738C File Offset: 0x000A558C
		private void UpdateOpenContextMenuOnChecked()
		{
			if (base.ContextMenu != null && base.IsVisible)
			{
				base.IsChecked.GetValueOrDefault();
				if (base.IsChecked.GetValueOrDefault())
				{
					List<WeakReference<ToolbarChildToggleButton>> list = ToolbarChildToggleButton.readyToShowContextElements;
					lock (list)
					{
						ToolbarChildToggleButton.readyToShowContextElements.Add(new WeakReference<ToolbarChildToggleButton>(this));
					}
					base.ContextMenu.PlacementTarget = this;
					base.Dispatcher.InvokeAsync(delegate
					{
						List<ToolbarChildToggleButton> list2 = new List<ToolbarChildToggleButton>();
						List<WeakReference<ToolbarChildToggleButton>> list3 = ToolbarChildToggleButton.readyToShowContextElements;
						lock (list3)
						{
							for (int i = ToolbarChildToggleButton.readyToShowContextElements.Count - 1; i >= 0; i--)
							{
								ToolbarChildToggleButton toolbarChildToggleButton;
								if (ToolbarChildToggleButton.readyToShowContextElements[i].TryGetTarget(out toolbarChildToggleButton) && toolbarChildToggleButton.IsLoaded && toolbarChildToggleButton.IsVisible && toolbarChildToggleButton.InnerContextMenu != null)
								{
									list2.Add(toolbarChildToggleButton);
								}
								ToolbarChildToggleButton.readyToShowContextElements.RemoveAt(i);
							}
						}
						ToolbarChildToggleButton toolbarChildToggleButton2 = null;
						double num = double.MaxValue;
						for (int j = 0; j < list2.Count; j++)
						{
							if (list2[j].IsMouseOver)
							{
								toolbarChildToggleButton2 = list2[j];
								break;
							}
							double num2 = list2[j].ActualWidth / 2.0;
							double num3 = list2[j].ActualHeight / 2.0;
							Point position = Mouse.GetPosition(list2[j]);
							double num4 = num2 - position.X;
							double num5 = num3 - position.Y;
							double num6 = num4 * num4 + num5 * num5;
							if (num6 < num)
							{
								toolbarChildToggleButton2 = list2[j];
								num = num6;
							}
						}
						if (num > 2500.0)
						{
							toolbarChildToggleButton2 = list2.FirstOrDefault<ToolbarChildToggleButton>();
						}
						if (toolbarChildToggleButton2 != null)
						{
							toolbarChildToggleButton2.attached = true;
							ContextMenu contextMenu = toolbarChildToggleButton2.innerContextMenu;
							contextMenu.Placement = PlacementMode.Bottom;
							contextMenu.IsOpen = true;
						}
					}, DispatcherPriority.Render);
					return;
				}
				if (this.attached)
				{
					this.attached = false;
					base.IsChecked = new bool?(false);
				}
				base.ContextMenu.IsOpen = false;
				base.ContextMenu.PlacementTarget = null;
			}
		}

		// Token: 0x060023AE RID: 9134 RVA: 0x000A7478 File Offset: 0x000A5678
		protected override void OnChecked(RoutedEventArgs e)
		{
			base.OnChecked(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x000A7487 File Offset: 0x000A5687
		protected override void OnUnchecked(RoutedEventArgs e)
		{
			base.OnUnchecked(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x000A7496 File Offset: 0x000A5696
		protected override void OnIndeterminate(RoutedEventArgs e)
		{
			base.OnIndeterminate(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x000A74A5 File Offset: 0x000A56A5
		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			if (this.attached)
			{
				base.IsChecked = new bool?(true);
			}
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x000A74BB File Offset: 0x000A56BB
		private void ContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			if (this.attached)
			{
				this.attached = false;
				base.IsChecked = new bool?(false);
			}
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x000A74D8 File Offset: 0x000A56D8
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);
			e.Handled = true;
			if (base.ContextMenu != null)
			{
				base.ContextMenu.IsOpen = false;
			}
		}

		// Token: 0x04000F21 RID: 3873
		private static List<WeakReference<ToolbarChildToggleButton>> readyToShowContextElements;

		// Token: 0x04000F22 RID: 3874
		private ContextMenu innerContextMenu;

		// Token: 0x04000F23 RID: 3875
		private DependencyPropertyDescriptor contextMenuPropertyDesc;

		// Token: 0x04000F24 RID: 3876
		private Rectangle indicator;

		// Token: 0x04000F25 RID: 3877
		private bool attached;

		// Token: 0x04000F26 RID: 3878
		public static readonly DependencyProperty OpenContextMenuOnCheckedProperty = DependencyProperty.Register("OpenContextMenuOnChecked", typeof(bool), typeof(ToolbarChildToggleButton), new PropertyMetadata(false, new PropertyChangedCallback(ToolbarChildToggleButton.OnOpenContextMenuOnCheckedPropertyChanged)));

		// Token: 0x04000F27 RID: 3879
		public static readonly DependencyProperty IsDropDownIconVisibleProperty = ToolbarButtonHelper.IsDropDownIconVisibleProperty.AddOwner(typeof(ToolbarChildToggleButton), new PropertyMetadata(true, new PropertyChangedCallback(ToolbarChildToggleButton.OnIsDropDownIconVisiblePropertyChanged)));

		// Token: 0x04000F28 RID: 3880
		public static readonly DependencyProperty IndicatorBrushProperty = ToolbarButtonHelper.IndicatorBrushProperty.AddOwner(typeof(ToolbarChildToggleButton));
	}
}
