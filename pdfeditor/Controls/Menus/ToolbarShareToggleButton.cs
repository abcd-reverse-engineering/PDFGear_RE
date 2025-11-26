using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200025E RID: 606
	public class ToolbarShareToggleButton : ToggleButton
	{
		// Token: 0x0600230C RID: 8972 RVA: 0x000A583C File Offset: 0x000A3A3C
		static ToolbarShareToggleButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarShareToggleButton), new FrameworkPropertyMetadata(typeof(ToolbarShareToggleButton)));
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x000A58EF File Offset: 0x000A3AEF
		public ToolbarShareToggleButton()
		{
			base.Loaded += this.ToolbarChildToggleButton_Loaded;
			base.Unloaded += this.ToolbarChildToggleButton_Unloaded;
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x0600230E RID: 8974 RVA: 0x000A5921 File Offset: 0x000A3B21
		// (set) Token: 0x0600230F RID: 8975 RVA: 0x000A592C File Offset: 0x000A3B2C
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

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06002310 RID: 8976 RVA: 0x000A59BB File Offset: 0x000A3BBB
		// (set) Token: 0x06002311 RID: 8977 RVA: 0x000A59C4 File Offset: 0x000A3BC4
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

		// Token: 0x06002312 RID: 8978 RVA: 0x000A5A25 File Offset: 0x000A3C25
		private void Indicator_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateIndicatorSize();
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000A5A30 File Offset: 0x000A3C30
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

		// Token: 0x06002314 RID: 8980 RVA: 0x000A5ABC File Offset: 0x000A3CBC
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

		// Token: 0x06002315 RID: 8981 RVA: 0x000A5B2E File Offset: 0x000A3D2E
		private void ToolbarChildToggleButton_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.contextMenuPropertyDesc != null)
			{
				this.contextMenuPropertyDesc.RemoveValueChanged(this, new EventHandler(this.OnContextMenuPropertyChanged));
				this.contextMenuPropertyDesc = null;
			}
			this.InnerContextMenu = null;
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x000A5B5E File Offset: 0x000A3D5E
		private void OnContextMenuPropertyChanged(object sender, EventArgs e)
		{
			this.InnerContextMenu = base.ContextMenu;
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x000A5B6C File Offset: 0x000A3D6C
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Indicator = base.GetTemplateChild("Indicator") as Rectangle;
			ToolbarButtonHelper.UpdateDropDownIconState(this);
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06002318 RID: 8984 RVA: 0x000A5B90 File Offset: 0x000A3D90
		// (set) Token: 0x06002319 RID: 8985 RVA: 0x000A5BA2 File Offset: 0x000A3DA2
		public bool OpenContextMenuOnChecked
		{
			get
			{
				return (bool)base.GetValue(ToolbarShareToggleButton.OpenContextMenuOnCheckedProperty);
			}
			set
			{
				base.SetValue(ToolbarShareToggleButton.OpenContextMenuOnCheckedProperty, value);
			}
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x000A5BB8 File Offset: 0x000A3DB8
		private static void OnOpenContextMenuOnCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarShareToggleButton toolbarShareToggleButton = d as ToolbarShareToggleButton;
				if (toolbarShareToggleButton != null)
				{
					toolbarShareToggleButton.UpdateOpenContextMenuOnChecked();
				}
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x0600231B RID: 8987 RVA: 0x000A5BEF File Offset: 0x000A3DEF
		// (set) Token: 0x0600231C RID: 8988 RVA: 0x000A5C01 File Offset: 0x000A3E01
		public bool IsDropDownIconVisible
		{
			get
			{
				return (bool)base.GetValue(ToolbarShareToggleButton.IsDropDownIconVisibleProperty);
			}
			set
			{
				base.SetValue(ToolbarShareToggleButton.IsDropDownIconVisibleProperty, value);
			}
		}

		// Token: 0x0600231D RID: 8989 RVA: 0x000A5C14 File Offset: 0x000A3E14
		private static void OnIsDropDownIconVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarShareToggleButton toolbarShareToggleButton = d as ToolbarShareToggleButton;
				if (toolbarShareToggleButton != null)
				{
					ToolbarButtonHelper.UpdateDropDownIconState(toolbarShareToggleButton);
				}
			}
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x0600231E RID: 8990 RVA: 0x000A5C4B File Offset: 0x000A3E4B
		// (set) Token: 0x0600231F RID: 8991 RVA: 0x000A5C5D File Offset: 0x000A3E5D
		public Brush IndicatorBrush
		{
			get
			{
				return (Brush)base.GetValue(ToolbarShareToggleButton.IndicatorBrushProperty);
			}
			set
			{
				base.SetValue(ToolbarShareToggleButton.IndicatorBrushProperty, value);
			}
		}

		// Token: 0x06002320 RID: 8992 RVA: 0x000A5C6C File Offset: 0x000A3E6C
		private void UpdateOpenContextMenuOnChecked()
		{
			if (base.ContextMenu != null)
			{
				bool valueOrDefault = base.IsChecked.GetValueOrDefault();
				if (base.ContextMenu.IsOpen != valueOrDefault)
				{
					base.ContextMenu.PlacementTarget = this;
					base.ContextMenu.Placement = PlacementMode.Bottom;
					base.ContextMenu.IsOpen = valueOrDefault;
				}
			}
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x000A5CC2 File Offset: 0x000A3EC2
		protected override void OnChecked(RoutedEventArgs e)
		{
			base.OnChecked(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x06002322 RID: 8994 RVA: 0x000A5CD1 File Offset: 0x000A3ED1
		protected override void OnUnchecked(RoutedEventArgs e)
		{
			base.OnUnchecked(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x000A5CE0 File Offset: 0x000A3EE0
		protected override void OnIndeterminate(RoutedEventArgs e)
		{
			base.OnIndeterminate(e);
			this.UpdateOpenContextMenuOnChecked();
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x000A5CEF File Offset: 0x000A3EEF
		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			base.IsChecked = new bool?(true);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x000A5CFD File Offset: 0x000A3EFD
		private void ContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			base.IsChecked = new bool?(false);
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x000A5D0B File Offset: 0x000A3F0B
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);
			e.Handled = true;
			if (base.ContextMenu != null)
			{
				base.ContextMenu.IsOpen = false;
			}
		}

		// Token: 0x04000EFA RID: 3834
		private ContextMenu innerContextMenu;

		// Token: 0x04000EFB RID: 3835
		private DependencyPropertyDescriptor contextMenuPropertyDesc;

		// Token: 0x04000EFC RID: 3836
		private Rectangle indicator;

		// Token: 0x04000EFD RID: 3837
		public static readonly DependencyProperty OpenContextMenuOnCheckedProperty = DependencyProperty.Register("OpenContextMenuOnChecked", typeof(bool), typeof(ToolbarShareToggleButton), new PropertyMetadata(false, new PropertyChangedCallback(ToolbarShareToggleButton.OnOpenContextMenuOnCheckedPropertyChanged)));

		// Token: 0x04000EFE RID: 3838
		public static readonly DependencyProperty IsDropDownIconVisibleProperty = ToolbarButtonHelper.IsDropDownIconVisibleProperty.AddOwner(typeof(ToolbarShareToggleButton), new PropertyMetadata(true, new PropertyChangedCallback(ToolbarShareToggleButton.OnIsDropDownIconVisiblePropertyChanged)));

		// Token: 0x04000EFF RID: 3839
		public static readonly DependencyProperty IndicatorBrushProperty = ToolbarButtonHelper.IndicatorBrushProperty.AddOwner(typeof(ToolbarShareToggleButton));
	}
}
