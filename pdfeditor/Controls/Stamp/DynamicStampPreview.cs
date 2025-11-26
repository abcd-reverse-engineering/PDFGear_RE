using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CommonLib.Common;
using pdfeditor.Controls.Annotations;
using pdfeditor.Models.DynamicStamps;
using pdfeditor.ViewModels;
using PDFKit.Utils.StampUtils;
using PDFKit.Utils.StampUtils.StampTemplates;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001E9 RID: 489
	public class DynamicStampPreview : FrameworkElement
	{
		// Token: 0x06001BC7 RID: 7111 RVA: 0x00072C89 File Offset: 0x00070E89
		public DynamicStampPreview()
		{
			base.HorizontalAlignment = HorizontalAlignment.Stretch;
			base.VerticalAlignment = VerticalAlignment.Stretch;
			base.SizeChanged += this.StampDefaultTextPreview_SizeChanged;
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06001BC8 RID: 7112 RVA: 0x00072CB1 File Offset: 0x00070EB1
		// (set) Token: 0x06001BC9 RID: 7113 RVA: 0x00072CC3 File Offset: 0x00070EC3
		public string TemplateName
		{
			get
			{
				return (string)base.GetValue(DynamicStampPreview.TemplateNameProperty);
			}
			set
			{
				base.SetValue(DynamicStampPreview.TemplateNameProperty, value);
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06001BCA RID: 7114 RVA: 0x00072CD1 File Offset: 0x00070ED1
		// (set) Token: 0x06001BCB RID: 7115 RVA: 0x00072CE3 File Offset: 0x00070EE3
		public DynamicStampProperties DynamicStampProperties
		{
			get
			{
				return (DynamicStampProperties)base.GetValue(DynamicStampPreview.DynamicStampPropertiesProperty);
			}
			set
			{
				base.SetValue(DynamicStampPreview.DynamicStampPropertiesProperty, value);
			}
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06001BCC RID: 7116 RVA: 0x00072CF1 File Offset: 0x00070EF1
		// (set) Token: 0x06001BCD RID: 7117 RVA: 0x00072CFE File Offset: 0x00070EFE
		public object StampModel
		{
			get
			{
				return base.GetValue(DynamicStampPreview.StampModelProperty);
			}
			set
			{
				base.SetValue(DynamicStampPreview.StampModelProperty, value);
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06001BCE RID: 7118 RVA: 0x00072D0C File Offset: 0x00070F0C
		// (set) Token: 0x06001BCF RID: 7119 RVA: 0x00072D1E File Offset: 0x00070F1E
		public Color StampColor
		{
			get
			{
				return (Color)base.GetValue(DynamicStampPreview.StampColorProperty);
			}
			set
			{
				base.SetValue(DynamicStampPreview.StampColorProperty, value);
			}
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x00072D34 File Offset: 0x00070F34
		private void UpdateStampModel()
		{
			if (base.ActualWidth > 0.0 && base.ActualHeight > 0.0)
			{
				try
				{
					string text = "";
					Color color = default(Color);
					CustStampModel custStampModel = this.StampModel as CustStampModel;
					if (custStampModel != null)
					{
						if (custStampModel != null && custStampModel.Text == "Visible")
						{
							if (!string.IsNullOrEmpty(custStampModel.TextContent))
							{
								text = custStampModel.TextContent;
								color = (Color)ColorConverter.ConvertFromString(custStampModel.FontColor);
							}
							if (custStampModel.DynamicStampTextModel.DynamicProperties != null)
							{
								this.DynamicStampProperties = custStampModel.DynamicStampTextModel.DynamicProperties;
								this.TemplateName = custStampModel.DynamicStampTextModel.TemplateName;
								this.StampColor = (Color)ColorConverter.ConvertFromString(custStampModel.DynamicStampTextModel.FontColor);
								this.Refresh();
								return;
							}
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
						this.TemplateName = "Square";
						this.StampColor = color;
						this.DynamicStampProperties = new DynamicStampProperties
						{
							FontWeight = DynamicStampProperties.FontWeights.Bold,
							FontItalic = false,
							Style = 1,
							Locale = CultureInfoUtils.ActualAppLanguage
						};
						this.DynamicStampProperties.Contents[0].ContentType = DynamicStampProperties.ContentType.Text;
						this.DynamicStampProperties.Contents[0].Content = text;
						this.Refresh();
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x00072F1C File Offset: 0x0007111C
		public void Refresh()
		{
			this.UpdateStampPreview();
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x00072F24 File Offset: 0x00071124
		private void UpdateStampPreview()
		{
			if (this.child != null)
			{
				base.RemoveVisualChild(this.child);
			}
			this.child = null;
			double actualWidth = base.ActualWidth;
			double actualHeight = base.ActualHeight;
			DynamicStampProperties dynamicStampProperties = this.DynamicStampProperties;
			Dictionary<string, string> dictionary = ((dynamicStampProperties != null) ? dynamicStampProperties.Data : null);
			if (actualWidth > 0.0 && actualHeight > 0.0 && dictionary != null)
			{
				try
				{
					this.child = StampUtil.CreateStampPreviewVisual(this.TemplateName, new StampAnnotationData
					{
						Width = actualWidth,
						Height = actualHeight,
						Color = this.StampColor,
						ContentDictionary = dictionary,
						CreateDate = new DateTimeOffset?(DateTimeOffset.Now)
					});
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

		// Token: 0x06001BD3 RID: 7123 RVA: 0x00073000 File Offset: 0x00071200
		private void StampDefaultTextPreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.StampModel != null)
			{
				this.UpdateStampModel();
			}
			this.UpdateStampPreview();
		}

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06001BD4 RID: 7124 RVA: 0x00073016 File Offset: 0x00071216
		protected override int VisualChildrenCount
		{
			get
			{
				return (this.child != null) ? 1 : 0;
			}
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00073021 File Offset: 0x00071221
		protected override Visual GetVisualChild(int index)
		{
			if (index == 0 && this.child != null)
			{
				return this.child;
			}
			throw new ArgumentOutOfRangeException("index");
		}

		// Token: 0x04000A10 RID: 2576
		private Visual child;

		// Token: 0x04000A11 RID: 2577
		public static readonly DependencyProperty TemplateNameProperty = DependencyProperty.Register("TemplateName", typeof(string), typeof(DynamicStampPreview), new PropertyMetadata("Chop1", delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DynamicStampPreview dynamicStampPreview = s as DynamicStampPreview;
			if (dynamicStampPreview != null && a.NewValue != a.OldValue)
			{
				dynamicStampPreview.UpdateStampPreview();
			}
		}));

		// Token: 0x04000A12 RID: 2578
		public static readonly DependencyProperty DynamicStampPropertiesProperty = DependencyProperty.Register("DynamicStampProperties", typeof(DynamicStampProperties), typeof(DynamicStampPreview), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DynamicStampPreview dynamicStampPreview2 = s as DynamicStampPreview;
			if (dynamicStampPreview2 != null && a.NewValue != a.OldValue)
			{
				dynamicStampPreview2.UpdateStampPreview();
			}
		}));

		// Token: 0x04000A13 RID: 2579
		public static readonly DependencyProperty StampModelProperty = DependencyProperty.Register("StampModel", typeof(object), typeof(DynamicStampPreview), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DynamicStampPreview dynamicStampPreview3 = s as DynamicStampPreview;
			if (dynamicStampPreview3 != null && a.NewValue != a.OldValue)
			{
				dynamicStampPreview3.UpdateStampModel();
			}
		}));

		// Token: 0x04000A14 RID: 2580
		public static readonly DependencyProperty StampColorProperty = DependencyProperty.Register("StampColor", typeof(Color), typeof(DynamicStampPreview), new PropertyMetadata(Color.FromArgb(byte.MaxValue, 32, 196, 143), delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DynamicStampPreview dynamicStampPreview4 = s as DynamicStampPreview;
			if (dynamicStampPreview4 != null && a.NewValue != a.OldValue)
			{
				dynamicStampPreview4.UpdateStampPreview();
			}
		}));
	}
}
