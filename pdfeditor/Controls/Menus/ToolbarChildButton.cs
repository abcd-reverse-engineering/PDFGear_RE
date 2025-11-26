using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000267 RID: 615
	public class ToolbarChildButton : Button
	{
		// Token: 0x060023B4 RID: 9140 RVA: 0x000A74FC File Offset: 0x000A56FC
		static ToolbarChildButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarChildButton), new FrameworkPropertyMetadata(typeof(ToolbarChildButton)));
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x000A7575 File Offset: 0x000A5775
		public ToolbarChildButton()
		{
			ToolbarButtonHelper.RegisterIsKeyboardFocused(this);
		}

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x060023B6 RID: 9142 RVA: 0x000A7583 File Offset: 0x000A5783
		// (set) Token: 0x060023B7 RID: 9143 RVA: 0x000A758C File Offset: 0x000A578C
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

		// Token: 0x060023B8 RID: 9144 RVA: 0x000A75ED File Offset: 0x000A57ED
		private void Indicator_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateIndicatorSize();
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x000A75F8 File Offset: 0x000A57F8
		private void UpdateIndicatorSize()
		{
			if (this.Indicator != null)
			{
				RectangleGeometry rectangleGeometry = this.Indicator.Clip as RectangleGeometry;
				if (rectangleGeometry == null)
				{
					rectangleGeometry = new RectangleGeometry();
					this.Indicator.Clip = rectangleGeometry;
				}
				rectangleGeometry.Rect = new Rect(0.0, this.Indicator.ActualHeight / 2.0, this.Indicator.ActualWidth, this.Indicator.ActualHeight / 2.0);
			}
		}

		// Token: 0x060023BA RID: 9146 RVA: 0x000A767C File Offset: 0x000A587C
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Indicator = base.GetTemplateChild("Indicator") as Rectangle;
			ToolbarButtonHelper.UpdateDropDownIconState(this);
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x060023BB RID: 9147 RVA: 0x000A76A0 File Offset: 0x000A58A0
		// (set) Token: 0x060023BC RID: 9148 RVA: 0x000A76B2 File Offset: 0x000A58B2
		public bool IsDropDownIconVisible
		{
			get
			{
				return (bool)base.GetValue(ToolbarChildButton.IsDropDownIconVisibleProperty);
			}
			set
			{
				base.SetValue(ToolbarChildButton.IsDropDownIconVisibleProperty, value);
			}
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x000A76C8 File Offset: 0x000A58C8
		private static void OnIsDropDownIconVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ToolbarChildButton toolbarChildButton = d as ToolbarChildButton;
				if (toolbarChildButton != null)
				{
					ToolbarButtonHelper.UpdateDropDownIconState(toolbarChildButton);
				}
			}
		}

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x060023BE RID: 9150 RVA: 0x000A76FF File Offset: 0x000A58FF
		// (set) Token: 0x060023BF RID: 9151 RVA: 0x000A7711 File Offset: 0x000A5911
		public Brush IndicatorBrush
		{
			get
			{
				return (Brush)base.GetValue(ToolbarChildButton.IndicatorBrushProperty);
			}
			set
			{
				base.SetValue(ToolbarChildButton.IndicatorBrushProperty, value);
			}
		}

		// Token: 0x04000F29 RID: 3881
		private Rectangle indicator;

		// Token: 0x04000F2A RID: 3882
		public static readonly DependencyProperty IsDropDownIconVisibleProperty = ToolbarButtonHelper.IsDropDownIconVisibleProperty.AddOwner(typeof(ToolbarChildButton), new PropertyMetadata(true, new PropertyChangedCallback(ToolbarChildButton.OnIsDropDownIconVisiblePropertyChanged)));

		// Token: 0x04000F2B RID: 3883
		public static readonly DependencyProperty IndicatorBrushProperty = ToolbarButtonHelper.IndicatorBrushProperty.AddOwner(typeof(ToolbarChildButton));
	}
}
