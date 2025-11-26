using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Patagames.Pdf.Net;

namespace pdfeditor.Controls.Presentation
{
	// Token: 0x02000231 RID: 561
	public partial class PdfImageFlipView : UserControl
	{
		// Token: 0x06001FB8 RID: 8120 RVA: 0x0008F2D8 File Offset: 0x0008D4D8
		public PdfImageFlipView()
		{
			this.InitializeComponent();
			this.prevImage = this.PdfImageView1;
			this.curImage = this.PdfImageView2;
			this.nextImage = this.PdfImageView3;
			base.IsVisibleChanged += this.PdfImageFlipView_IsVisibleChanged;
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x0008F327 File Offset: 0x0008D527
		private void PdfImageFlipView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateDocument();
		}

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06001FBA RID: 8122 RVA: 0x0008F32F File Offset: 0x0008D52F
		// (set) Token: 0x06001FBB RID: 8123 RVA: 0x0008F341 File Offset: 0x0008D541
		public PdfDocument Document
		{
			get
			{
				return (PdfDocument)base.GetValue(PdfImageFlipView.DocumentProperty);
			}
			set
			{
				base.SetValue(PdfImageFlipView.DocumentProperty, value);
			}
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x0008F350 File Offset: 0x0008D550
		private static void OnDocumentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				PdfImageFlipView pdfImageFlipView = d as PdfImageFlipView;
				if (pdfImageFlipView != null)
				{
					pdfImageFlipView.UpdateDocument();
				}
			}
		}

		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06001FBD RID: 8125 RVA: 0x0008F37D File Offset: 0x0008D57D
		// (set) Token: 0x06001FBE RID: 8126 RVA: 0x0008F38F File Offset: 0x0008D58F
		public int PageIndex
		{
			get
			{
				return (int)base.GetValue(PdfImageFlipView.PageIndexProperty);
			}
			set
			{
				base.SetValue(PdfImageFlipView.PageIndexProperty, value);
			}
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x0008F3A4 File Offset: 0x0008D5A4
		private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				PdfImageFlipView pdfImageFlipView = d as PdfImageFlipView;
				if (pdfImageFlipView != null)
				{
					pdfImageFlipView.UpdatePage((int)e.NewValue, (int)e.OldValue);
				}
			}
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x0008F3F0 File Offset: 0x0008D5F0
		private void UpdateDocument()
		{
			Storyboard storyboard = this.curSb;
			if (storyboard != null)
			{
				storyboard.SkipToFill();
			}
			this.curSb = null;
			this.prevImage.Opacity = 0.0;
			this.curImage.Opacity = 0.0;
			this.nextImage.Opacity = 0.0;
			this.prevImage.Document = this.Document;
			this.curImage.Document = this.Document;
			this.nextImage.Document = this.Document;
			this.prevImage.PageIndex = this.PageIndex - 1;
			this.curImage.PageIndex = this.PageIndex;
			this.nextImage.PageIndex = this.PageIndex + 1;
			Panel.SetZIndex(this.prevImage, 0);
			Panel.SetZIndex(this.nextImage, 1);
			Panel.SetZIndex(this.curImage, 2);
			this.AnimationShow(null);
		}

		// Token: 0x06001FC1 RID: 8129 RVA: 0x0008F4F0 File Offset: 0x0008D6F0
		private void UpdatePage(int newPage, int oldPage)
		{
			this.curSb.SkipToFill();
			this.curSb = null;
			int num = newPage - oldPage;
			if (num == 0 || num > 1 || num < -1)
			{
				this.UpdateDocument();
				return;
			}
			PdfImageView pdfImageView = this.curImage;
			Panel.SetZIndex(pdfImageView, 1);
			PdfImageView pdfImageView2 = null;
			if (num == -1)
			{
				pdfImageView2 = this.prevImage;
				PdfImageView pdfImageView3 = this.nextImage;
				this.nextImage = this.curImage;
				this.curImage = this.prevImage;
				this.prevImage = pdfImageView3;
				this.prevImage.PageIndex = newPage - 1;
				Panel.SetZIndex(this.curImage, 2);
				Panel.SetZIndex(this.prevImage, 0);
			}
			else if (num == 1)
			{
				pdfImageView2 = this.nextImage;
				PdfImageView pdfImageView4 = this.prevImage;
				this.prevImage = this.curImage;
				this.curImage = this.nextImage;
				this.nextImage = pdfImageView4;
				this.nextImage.PageIndex = newPage + 1;
				Panel.SetZIndex(this.curImage, 2);
				Panel.SetZIndex(this.nextImage, 0);
			}
			this.AnimationBetween(pdfImageView, pdfImageView2, null);
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x0008F5FC File Offset: 0x0008D7FC
		private void AnimationBetween(PdfImageView from, PdfImageView to, TimeSpan? duration = null)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(0.2);
			if (duration != null)
			{
				timeSpan = duration.Value;
			}
			to.Opacity = 0.0;
			Storyboard storyboard = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(0.0),
				To = new double?((double)1),
				Duration = new Duration(TimeSpan.FromSeconds(timeSpan.TotalSeconds))
			};
			Storyboard.SetTarget(doubleAnimation, to);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			storyboard.Children.Add(doubleAnimation);
			storyboard.Begin();
			this.curSb = storyboard;
		}

		// Token: 0x06001FC3 RID: 8131 RVA: 0x0008F6AC File Offset: 0x0008D8AC
		private void AnimationShow(TimeSpan? duration = null)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(0.2);
			if (duration != null)
			{
				timeSpan = duration.Value;
			}
			Storyboard storyboard = new Storyboard
			{
				Children = 
				{
					PdfImageFlipView.<AnimationShow>g__CreateAnimationCore|19_0(this.PdfImageView1, timeSpan),
					PdfImageFlipView.<AnimationShow>g__CreateAnimationCore|19_0(this.PdfImageView2, timeSpan),
					PdfImageFlipView.<AnimationShow>g__CreateAnimationCore|19_0(this.PdfImageView3, timeSpan)
				}
			};
			storyboard.Begin();
			this.curSb = storyboard;
		}

		// Token: 0x06001FC8 RID: 8136 RVA: 0x0008F840 File Offset: 0x0008DA40
		[CompilerGenerated]
		internal static DoubleAnimation <AnimationShow>g__CreateAnimationCore|19_0(DependencyObject _obj, TimeSpan _duration)
		{
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			doubleAnimation.From = new double?(0.0);
			doubleAnimation.To = new double?((double)1);
			doubleAnimation.Duration = new Duration(_duration);
			Storyboard.SetTarget(doubleAnimation, _obj);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			return doubleAnimation;
		}

		// Token: 0x04000CB2 RID: 3250
		private PdfImageView prevImage;

		// Token: 0x04000CB3 RID: 3251
		private PdfImageView curImage;

		// Token: 0x04000CB4 RID: 3252
		private PdfImageView nextImage;

		// Token: 0x04000CB5 RID: 3253
		private Storyboard curSb;

		// Token: 0x04000CB6 RID: 3254
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(PdfDocument), typeof(PdfImageFlipView), new PropertyMetadata(null, new PropertyChangedCallback(PdfImageFlipView.OnDocumentPropertyChanged)));

		// Token: 0x04000CB7 RID: 3255
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(PdfImageFlipView), new PropertyMetadata(-1, new PropertyChangedCallback(PdfImageFlipView.OnPageIndexPropertyChanged)));
	}
}
