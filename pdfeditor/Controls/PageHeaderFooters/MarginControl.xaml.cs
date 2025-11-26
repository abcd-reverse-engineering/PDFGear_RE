using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x0200023E RID: 574
	public partial class MarginControl : UserControl
	{
		// Token: 0x06002090 RID: 8336 RVA: 0x00094FF8 File Offset: 0x000931F8
		public MarginControl()
		{
			this.InitializeComponent();
			base.SizeChanged += this.MarginControl_SizeChanged;
			this.LeftTextBlock.SizeChanged += this.TextBlock_SizeChanged;
			this.CenterTextBlock.SizeChanged += this.TextBlock_SizeChanged;
			this.RightTextBlock.SizeChanged += this.TextBlock_SizeChanged;
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x00095068 File Offset: 0x00093268
		private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateLinePosition();
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x00095070 File Offset: 0x00093270
		private void MarginControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateLinePosition();
		}

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06002093 RID: 8339 RVA: 0x00095078 File Offset: 0x00093278
		// (set) Token: 0x06002094 RID: 8340 RVA: 0x0009508A File Offset: 0x0009328A
		public double PreviewFontSize
		{
			get
			{
				return (double)base.GetValue(MarginControl.PreviewFontSizeProperty);
			}
			set
			{
				base.SetValue(MarginControl.PreviewFontSizeProperty, value);
			}
		}

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06002095 RID: 8341 RVA: 0x0009509D File Offset: 0x0009329D
		// (set) Token: 0x06002096 RID: 8342 RVA: 0x000950AF File Offset: 0x000932AF
		public double PageOriginalWidth
		{
			get
			{
				return (double)base.GetValue(MarginControl.PageOriginalWidthProperty);
			}
			set
			{
				base.SetValue(MarginControl.PageOriginalWidthProperty, value);
			}
		}

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06002097 RID: 8343 RVA: 0x000950C2 File Offset: 0x000932C2
		// (set) Token: 0x06002098 RID: 8344 RVA: 0x000950D4 File Offset: 0x000932D4
		public double MarginLeft
		{
			get
			{
				return (double)base.GetValue(MarginControl.MarginLeftProperty);
			}
			set
			{
				base.SetValue(MarginControl.MarginLeftProperty, value);
			}
		}

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06002099 RID: 8345 RVA: 0x000950E7 File Offset: 0x000932E7
		// (set) Token: 0x0600209A RID: 8346 RVA: 0x000950F9 File Offset: 0x000932F9
		public double MarginTop
		{
			get
			{
				return (double)base.GetValue(MarginControl.MarginTopProperty);
			}
			set
			{
				base.SetValue(MarginControl.MarginTopProperty, value);
			}
		}

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x0600209B RID: 8347 RVA: 0x0009510C File Offset: 0x0009330C
		// (set) Token: 0x0600209C RID: 8348 RVA: 0x0009511E File Offset: 0x0009331E
		public double MarginRight
		{
			get
			{
				return (double)base.GetValue(MarginControl.MarginRightProperty);
			}
			set
			{
				base.SetValue(MarginControl.MarginRightProperty, value);
			}
		}

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x0600209D RID: 8349 RVA: 0x00095131 File Offset: 0x00093331
		// (set) Token: 0x0600209E RID: 8350 RVA: 0x00095143 File Offset: 0x00093343
		public double MarginBottom
		{
			get
			{
				return (double)base.GetValue(MarginControl.MarginBottomProperty);
			}
			set
			{
				base.SetValue(MarginControl.MarginBottomProperty, value);
			}
		}

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x0600209F RID: 8351 RVA: 0x00095156 File Offset: 0x00093356
		// (set) Token: 0x060020A0 RID: 8352 RVA: 0x00095168 File Offset: 0x00093368
		public ToreEdge Edge
		{
			get
			{
				return (ToreEdge)base.GetValue(MarginControl.EdgeProperty);
			}
			set
			{
				base.SetValue(MarginControl.EdgeProperty, value);
			}
		}

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x060020A1 RID: 8353 RVA: 0x0009517B File Offset: 0x0009337B
		// (set) Token: 0x060020A2 RID: 8354 RVA: 0x0009518D File Offset: 0x0009338D
		public string LeftString
		{
			get
			{
				return (string)base.GetValue(MarginControl.LeftStringProperty);
			}
			set
			{
				base.SetValue(MarginControl.LeftStringProperty, value);
			}
		}

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x060020A3 RID: 8355 RVA: 0x0009519B File Offset: 0x0009339B
		// (set) Token: 0x060020A4 RID: 8356 RVA: 0x000951AD File Offset: 0x000933AD
		public string CenterString
		{
			get
			{
				return (string)base.GetValue(MarginControl.CenterStringProperty);
			}
			set
			{
				base.SetValue(MarginControl.CenterStringProperty, value);
			}
		}

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x060020A5 RID: 8357 RVA: 0x000951BB File Offset: 0x000933BB
		// (set) Token: 0x060020A6 RID: 8358 RVA: 0x000951CD File Offset: 0x000933CD
		public string RightString
		{
			get
			{
				return (string)base.GetValue(MarginControl.RightStringProperty);
			}
			set
			{
				base.SetValue(MarginControl.RightStringProperty, value);
			}
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x000951DC File Offset: 0x000933DC
		private void UpdatePreviewFontSize()
		{
			if (base.ActualWidth == 0.0 || base.ActualHeight == 0.0)
			{
				return;
			}
			double pageOriginalWidth = this.PageOriginalWidth;
			double num = base.ActualWidth / pageOriginalWidth;
			double num2 = this.PreviewFontSize * num;
			this.LeftTextBlock.FontSize = num2;
			this.CenterTextBlock.FontSize = num2;
			this.RightTextBlock.FontSize = num2;
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x0009524C File Offset: 0x0009344C
		private void UpdateLinePosition()
		{
			if (base.ActualWidth == 0.0 || base.ActualHeight == 0.0)
			{
				return;
			}
			double pageOriginalWidth = this.PageOriginalWidth;
			double actualWidth = base.ActualWidth;
			double num = actualWidth / pageOriginalWidth;
			Line line = this.Line2;
			Line line2 = this.Line3;
			double num2 = this.MarginLeft * num;
			double num3 = this.MarginRight * num;
			line.X1 = num2;
			line.X2 = num2;
			line.Y1 = 0.0;
			line.Y2 = base.ActualHeight;
			line2.X1 = actualWidth - num3;
			line2.X2 = actualWidth - num3;
			line2.Y1 = 0.0;
			line2.Y2 = base.ActualHeight;
			this.Line1.X1 = 0.0;
			this.Line1.X2 = actualWidth;
			this.UpdatePreviewFontSize();
			if (this.Edge == ToreEdge.Top)
			{
				double num4 = this.MarginTop * num;
				this.Line1.Y1 = num4 + 1.0;
				this.Line1.Y2 = num4 + 1.0;
				this.LeftTextBlock.Margin = new Thickness(num2, num4 - MarginControl.GetTextBlockHeight(this.LeftTextBlock), 0.0, 0.0);
				this.CenterTextBlock.Margin = new Thickness(0.0, num4 - MarginControl.GetTextBlockHeight(this.CenterTextBlock), 0.0, 0.0);
				this.RightTextBlock.Margin = new Thickness(0.0, num4 - MarginControl.GetTextBlockHeight(this.RightTextBlock), num3, 0.0);
				return;
			}
			if (this.Edge == ToreEdge.Bottom)
			{
				double num5 = this.MarginBottom * num;
				this.Line1.Y1 = base.ActualHeight - num5 - 1.0;
				this.Line1.Y2 = base.ActualHeight - num5 - 1.0;
				this.LeftTextBlock.Margin = new Thickness(num2, 0.0, 0.0, num5 - MarginControl.GetTextBlockHeight(this.LeftTextBlock));
				this.CenterTextBlock.Margin = new Thickness(0.0, 0.0, 0.0, num5 - MarginControl.GetTextBlockHeight(this.CenterTextBlock));
				this.RightTextBlock.Margin = new Thickness(0.0, 0.0, num3, num5 - MarginControl.GetTextBlockHeight(this.RightTextBlock));
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000954FE File Offset: 0x000936FE
		private static double GetTextBlockHeight(TextBlock block)
		{
			if (!double.IsNaN(block.ActualHeight))
			{
				return block.ActualHeight;
			}
			return 0.0;
		}

		// Token: 0x04000D24 RID: 3364
		public static readonly DependencyProperty PreviewFontSizeProperty = DependencyProperty.Register("PreviewFontSize", typeof(double), typeof(MarginControl), new PropertyMetadata(10.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D25 RID: 3365
		public static readonly DependencyProperty PageOriginalWidthProperty = DependencyProperty.Register("PageOriginalWidth", typeof(double), typeof(MarginControl), new PropertyMetadata(595.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D26 RID: 3366
		public static readonly DependencyProperty MarginLeftProperty = DependencyProperty.Register("MarginLeft", typeof(double), typeof(MarginControl), new PropertyMetadata(0.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D27 RID: 3367
		public static readonly DependencyProperty MarginTopProperty = DependencyProperty.Register("MarginTop", typeof(double), typeof(MarginControl), new PropertyMetadata(0.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D28 RID: 3368
		public static readonly DependencyProperty MarginRightProperty = DependencyProperty.Register("MarginRight", typeof(double), typeof(MarginControl), new PropertyMetadata(0.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D29 RID: 3369
		public static readonly DependencyProperty MarginBottomProperty = DependencyProperty.Register("MarginBottom", typeof(double), typeof(MarginControl), new PropertyMetadata(0.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D2A RID: 3370
		public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register("Edge", typeof(ToreEdge), typeof(MarginControl), new PropertyMetadata(ToreEdge.Top, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			MarginControl marginControl = (MarginControl)s;
			marginControl.LeftTextBlock.VerticalAlignment = (((ToreEdge)a.NewValue == ToreEdge.Top) ? VerticalAlignment.Top : VerticalAlignment.Bottom);
			marginControl.CenterTextBlock.VerticalAlignment = (((ToreEdge)a.NewValue == ToreEdge.Top) ? VerticalAlignment.Top : VerticalAlignment.Bottom);
			marginControl.RightTextBlock.VerticalAlignment = (((ToreEdge)a.NewValue == ToreEdge.Top) ? VerticalAlignment.Top : VerticalAlignment.Bottom);
			marginControl.UpdateLinePosition();
		}));

		// Token: 0x04000D2B RID: 3371
		public static readonly DependencyProperty LeftStringProperty = DependencyProperty.Register("LeftString", typeof(string), typeof(MarginControl), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D2C RID: 3372
		public static readonly DependencyProperty CenterStringProperty = DependencyProperty.Register("CenterString", typeof(string), typeof(MarginControl), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));

		// Token: 0x04000D2D RID: 3373
		public static readonly DependencyProperty RightStringProperty = DependencyProperty.Register("RightString", typeof(string), typeof(MarginControl), new PropertyMetadata("", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			((MarginControl)s).UpdateLinePosition();
		}));
	}
}
