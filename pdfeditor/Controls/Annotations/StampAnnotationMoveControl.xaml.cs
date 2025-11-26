using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using pdfeditor.Controls.Stamp;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002AA RID: 682
	public partial class StampAnnotationMoveControl : UserControl
	{
		// Token: 0x06002759 RID: 10073 RVA: 0x000BA094 File Offset: 0x000B8294
		public StampAnnotationMoveControl(TextStampModel textmodel)
		{
			this.InitializeComponent();
			this.contentImage.Visibility = Visibility.Collapsed;
			this.TextContentBorder.Visibility = Visibility.Visible;
			this.textModel = textmodel;
			this.TextContentBorder.DataContext = this.textModel;
			this.contentImage.DataContext = this.imageModel;
			this.TextContentBorder.Child = new StampDefaultTextPreview
			{
				StampModel = new CustStampModel
				{
					Text = "Visible",
					TextContent = this.textModel.Text,
					FontColor = this.textModel.Foreground,
					TimeFormat = this.textModel.TimeFormat
				}
			};
			this.TextContentBorder.SizeChanged += this.TextContentBorder_SizeChanged;
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000BA160 File Offset: 0x000B8360
		public StampAnnotationMoveControl(DynamicStampTextModel textmodel, Rect stampRect)
		{
			this.InitializeComponent();
			this.contentImage.Visibility = Visibility.Collapsed;
			this.TextContentBorder.Visibility = Visibility.Visible;
			this.dynamicStampTextModel = textmodel;
			this.TextContentBorder.DataContext = this.textModel;
			this.contentImage.DataContext = this.imageModel;
			this.TextContentBorder.Child = new DynamicStampPreview
			{
				DynamicStampProperties = textmodel.DynamicProperties,
				StampColor = (Color)ColorConverter.ConvertFromString(textmodel.FontColor),
				TemplateName = textmodel.TemplateName,
				Width = stampRect.Width,
				Height = stampRect.Height
			};
			this.TextContentBorder.SizeChanged += this.TextContentBorder_SizeChanged;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x000BA22C File Offset: 0x000B842C
		public StampAnnotationMoveControl(ImageStampModel imgmodel)
		{
			this.InitializeComponent();
			this.contentImage.Visibility = Visibility.Visible;
			this.TextContentBorder.Visibility = Visibility.Collapsed;
			this.imageModel = imgmodel;
			this.contentImage.DataContext = this.imageModel;
			this.TextContentBorder.DataContext = this.textModel;
			this.DashBorder.Width = imgmodel.ImageWidth;
			this.DashBorder.Height = imgmodel.ImageHeight;
		}

		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x0600275C RID: 10076 RVA: 0x000BA2A8 File Offset: 0x000B84A8
		public ImageStampModel ImageModel
		{
			get
			{
				return this.imageModel;
			}
		}

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x0600275D RID: 10077 RVA: 0x000BA2B0 File Offset: 0x000B84B0
		public TextStampModel TextModel
		{
			get
			{
				return this.textModel;
			}
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x000BA2B8 File Offset: 0x000B84B8
		private void TextContentBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.DashBorder.Width = e.NewSize.Width;
			this.DashBorder.Height = e.NewSize.Height;
		}

		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x0600275F RID: 10079 RVA: 0x000BA2F8 File Offset: 0x000B84F8
		public Rect Bounds
		{
			get
			{
				if (this.TextModel == null && this.ImageModel == null)
				{
					return Rect.Empty;
				}
				double num = Canvas.GetLeft(this);
				double num2 = Canvas.GetTop(this);
				if (double.IsNaN(num))
				{
					num = 0.0;
				}
				if (double.IsNaN(num2))
				{
					num2 = 0.0;
				}
				if (this.TextModel != null)
				{
					return new Rect(num, num2, this.TextModel.TextWidth, this.TextModel.TextHeight);
				}
				if (this.ImageModel != null)
				{
					return new Rect(num, num2, this.ImageModel.ImageWidth, this.ImageModel.ImageHeight);
				}
				return Rect.Empty;
			}
		}

		// Token: 0x040010F3 RID: 4339
		private TextStampModel textModel;

		// Token: 0x040010F4 RID: 4340
		private ImageStampModel imageModel;

		// Token: 0x040010F5 RID: 4341
		private DynamicStampTextModel dynamicStampTextModel;
	}
}
