using System;
using System.Windows;
using System.Windows.Media;
using CommonLib.Common;
using pdfeditor.Controls.Annotations;
using pdfeditor.ViewModels;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001EA RID: 490
	public class StampDefaultTextPreview : FrameworkElement
	{
		// Token: 0x06001BD7 RID: 7127 RVA: 0x0007314F File Offset: 0x0007134F
		public StampDefaultTextPreview()
		{
			base.HorizontalAlignment = HorizontalAlignment.Stretch;
			base.VerticalAlignment = VerticalAlignment.Stretch;
			base.SizeChanged += this.StampDefaultTextPreview_SizeChanged;
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06001BD8 RID: 7128 RVA: 0x00073177 File Offset: 0x00071377
		// (set) Token: 0x06001BD9 RID: 7129 RVA: 0x00073184 File Offset: 0x00071384
		public object StampModel
		{
			get
			{
				return base.GetValue(StampDefaultTextPreview.StampModelProperty);
			}
			set
			{
				base.SetValue(StampDefaultTextPreview.StampModelProperty, value);
			}
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06001BDA RID: 7130 RVA: 0x00073192 File Offset: 0x00071392
		// (set) Token: 0x06001BDB RID: 7131 RVA: 0x000731A4 File Offset: 0x000713A4
		public double BorderThickness
		{
			get
			{
				return (double)base.GetValue(StampDefaultTextPreview.BorderThicknessProperty);
			}
			set
			{
				base.SetValue(StampDefaultTextPreview.BorderThicknessProperty, value);
			}
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000731B7 File Offset: 0x000713B7
		public void ForceRender()
		{
			this.UpdateStampModel();
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x000731C0 File Offset: 0x000713C0
		private void UpdateStampModel()
		{
			if (this.child != null)
			{
				base.RemoveVisualChild(this.child);
			}
			this.child = null;
			if (base.ActualWidth > 0.0 && base.ActualHeight > 0.0)
			{
				try
				{
					string text = "";
					string text2 = "";
					Color color = default(Color);
					CustStampModel custStampModel = this.StampModel as CustStampModel;
					if (custStampModel != null)
					{
						if (custStampModel != null && custStampModel.Text == "Visible" && !string.IsNullOrEmpty(custStampModel.TextContent))
						{
							text = custStampModel.TextContent;
							color = (Color)ColorConverter.ConvertFromString(custStampModel.FontColor);
						}
					}
					else
					{
						TextStampModel textStampModel = this.StampModel as TextStampModel;
						if (textStampModel != null)
						{
							text = textStampModel.Text;
							color = (Color)ColorConverter.ConvertFromString(textStampModel.Foreground);
						}
						else
						{
							IStampTextModel stampTextModel = this.StampModel as IStampTextModel;
							if (stampTextModel != null)
							{
								text = stampTextModel.TextContent;
								color = (Color)ColorConverter.ConvertFromString(stampTextModel.FontColor);
								StampTextModel stampTextModel2 = stampTextModel as StampTextModel;
							}
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						this.child = StampUtil.CreateDefaultTextPreviewVisual(text, base.ActualWidth, base.ActualHeight, color, text2, CultureInfoUtils.ActualAppLanguage, this.BorderThickness);
					}
				}
				catch
				{
				}
			}
			if (this.child != null)
			{
				base.AddVisualChild(this.child);
			}
			base.InvalidateVisual();
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00073334 File Offset: 0x00071534
		private void StampDefaultTextPreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateStampModel();
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06001BDF RID: 7135 RVA: 0x0007333C File Offset: 0x0007153C
		protected override int VisualChildrenCount
		{
			get
			{
				return (this.child != null) ? 1 : 0;
			}
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x00073347 File Offset: 0x00071547
		protected override Visual GetVisualChild(int index)
		{
			if (index == 0 && this.child != null)
			{
				return this.child;
			}
			throw new ArgumentOutOfRangeException("index");
		}

		// Token: 0x04000A15 RID: 2581
		private Visual child;

		// Token: 0x04000A16 RID: 2582
		public static readonly DependencyProperty StampModelProperty = DependencyProperty.Register("StampModel", typeof(object), typeof(StampDefaultTextPreview), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			StampDefaultTextPreview stampDefaultTextPreview = s as StampDefaultTextPreview;
			if (stampDefaultTextPreview != null && a.NewValue != a.OldValue)
			{
				stampDefaultTextPreview.UpdateStampModel();
			}
		}));

		// Token: 0x04000A17 RID: 2583
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(StampDefaultTextPreview), new PropertyMetadata(2.0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			StampDefaultTextPreview stampDefaultTextPreview2 = s as StampDefaultTextPreview;
			if (stampDefaultTextPreview2 != null && a.NewValue != a.OldValue)
			{
				stampDefaultTextPreview2.UpdateStampModel();
			}
		}));
	}
}
