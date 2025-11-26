using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace pdfeditor.Controls
{
	// Token: 0x020001AF RID: 431
	public partial class AnimationExtentButton : Button
	{
		// Token: 0x06001851 RID: 6225 RVA: 0x0005CB9C File Offset: 0x0005AD9C
		static AnimationExtentButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationExtentButton), new FrameworkPropertyMetadata(typeof(AnimationExtentButton)));
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x0005CC2A File Offset: 0x0005AE2A
		public AnimationExtentButton()
		{
			base.Unloaded += this.AnimationExtentButton_Unloaded;
			this.backgroundAnimationFunc = new ExponentialEase
			{
				EasingMode = EasingMode.EaseInOut,
				Exponent = 7.0
			};
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06001853 RID: 6227 RVA: 0x0005CC65 File Offset: 0x0005AE65
		// (set) Token: 0x06001854 RID: 6228 RVA: 0x0005CC70 File Offset: 0x0005AE70
		private Border ContentContainer
		{
			get
			{
				return this.contentContainer;
			}
			set
			{
				if (this.contentContainer != value)
				{
					if (this.contentContainer != null)
					{
						this.contentContainer.SizeChanged -= this.ContentContainer_SizeChanged;
					}
					this.contentContainer = value;
					if (this.contentContainer != null)
					{
						this.contentContainer.SizeChanged += this.ContentContainer_SizeChanged;
					}
					this.UpdateContentSize();
				}
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x0005CCD4 File Offset: 0x0005AED4
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.ContentContainer = base.GetTemplateChild("ContentContainer") as Border;
			this.StartFigure = base.GetTemplateChild("StartFigure") as PathFigure;
			this.LeftArc = base.GetTemplateChild("LeftArc") as ArcSegment;
			this.CenterLine1 = base.GetTemplateChild("CenterLine1") as LineSegment;
			this.RightArc = base.GetTemplateChild("RightArc") as ArcSegment;
			this.CenterLine2 = base.GetTemplateChild("CenterLine2") as LineSegment;
			this.ContentBackground = base.GetTemplateChild("ContentBackground") as Path;
			this.UpdateMouseOverState(false);
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0005CD88 File Offset: 0x0005AF88
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			this.UpdateMouseOverState(false);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x0005CD98 File Offset: 0x0005AF98
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.UpdateMouseOverState(false);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x0005CDA8 File Offset: 0x0005AFA8
		protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsMouseDirectlyOverChanged(e);
			this.UpdateMouseOverState(false);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0005CDB8 File Offset: 0x0005AFB8
		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			base.OnPreviewMouseMove(e);
			this.UpdateMouseOverState(false);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0005CDC8 File Offset: 0x0005AFC8
		private void AnimationExtentButton_Unloaded(object sender, RoutedEventArgs e)
		{
			this.UpdateMouseOverState(false);
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0005CDD1 File Offset: 0x0005AFD1
		private void ContentContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateContentSize();
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x0600185C RID: 6236 RVA: 0x0005CDD9 File Offset: 0x0005AFD9
		// (set) Token: 0x0600185D RID: 6237 RVA: 0x0005CDE6 File Offset: 0x0005AFE6
		public object Header
		{
			get
			{
				return base.GetValue(AnimationExtentButton.HeaderProperty);
			}
			set
			{
				base.SetValue(AnimationExtentButton.HeaderProperty, value);
			}
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x0005CDF4 File Offset: 0x0005AFF4
		private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				AnimationExtentButton animationExtentButton = d as AnimationExtentButton;
				if (animationExtentButton != null)
				{
					animationExtentButton.RemoveLogicalChild(e.OldValue);
					DependencyObject dependencyObject = e.NewValue as DependencyObject;
					if (dependencyObject != null)
					{
						animationExtentButton.AddLogicalChild(dependencyObject);
					}
				}
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x0600185F RID: 6239 RVA: 0x0005CE3F File Offset: 0x0005B03F
		// (set) Token: 0x06001860 RID: 6240 RVA: 0x0005CE51 File Offset: 0x0005B051
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(AnimationExtentButton.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(AnimationExtentButton.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0005CE60 File Offset: 0x0005B060
		private void UpdateMouseOverState(bool disableAnimation = false)
		{
			bool isMouseOver = base.IsMouseOver;
			if (isMouseOver != this.isMouseOverInternal)
			{
				if (base.IsMouseOver)
				{
					VisualStateManager.GoToState(this, "IsMouseOverState", true);
					Storyboard storyboard = this.currentSb;
					if (storyboard != null)
					{
						storyboard.Stop();
					}
					this.currentSb = this.BuildShowStoryboard();
					if (disableAnimation)
					{
						this.currentSb.SkipToFill();
					}
					else
					{
						this.currentSb.Begin();
					}
				}
				else
				{
					VisualStateManager.GoToState(this, "Normal", true);
					Storyboard storyboard2 = this.currentSb;
					if (storyboard2 != null)
					{
						storyboard2.Stop();
					}
					this.currentSb = this.BuildHideStoryboard();
					if (disableAnimation)
					{
						this.currentSb.SkipToFill();
					}
					else
					{
						this.currentSb.Begin();
					}
				}
				this.isMouseOverInternal = isMouseOver;
			}
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x0005CF1B File Offset: 0x0005B11B
		private void UpdateContentSize()
		{
			if (this.ContentContainer != null)
			{
				Canvas.SetLeft(this.ContentContainer, -this.ContentContainer.ActualWidth);
			}
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0005CF3C File Offset: 0x0005B13C
		private Storyboard BuildShowStoryboard()
		{
			if (this.ContentContainer != null)
			{
				return this.BuildStoryboardCore(this.ContentContainer.ActualWidth, TimeSpan.FromSeconds(0.3), TimeSpan.Zero);
			}
			return null;
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x0005CF6C File Offset: 0x0005B16C
		private Storyboard BuildHideStoryboard()
		{
			if (this.ContentContainer != null)
			{
				return this.BuildStoryboardCore(0.0, TimeSpan.FromSeconds(0.3), TimeSpan.Zero);
			}
			return null;
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0005CF9C File Offset: 0x0005B19C
		private Storyboard BuildStoryboardCore(double width, TimeSpan duration, TimeSpan beginTime)
		{
			if (this.ContentContainer != null && this.StartFigure != null && this.LeftArc != null && this.CenterLine1 != null && this.RightArc != null && this.CenterLine2 != null)
			{
				Storyboard storyboard = new Storyboard();
				this.<BuildStoryboardCore>g__AddPointAnimation|35_0(storyboard, new Point(20.0 - width, 0.0), this.StartFigure, PathFigure.StartPointProperty, duration, beginTime);
				this.<BuildStoryboardCore>g__AddPointAnimation|35_0(storyboard, new Point(20.0 - width, 40.0), this.LeftArc, ArcSegment.PointProperty, duration, beginTime);
				this.<BuildStoryboardCore>g__AddPointAnimation|35_0(storyboard, new Point(20.0 - width, 0.0), this.CenterLine2, LineSegment.PointProperty, duration, beginTime);
				storyboard.Freeze();
				return storyboard;
			}
			return null;
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0005D084 File Offset: 0x0005B284
		[CompilerGenerated]
		private void <BuildStoryboardCore>g__AddPointAnimation|35_0(Storyboard _sb, Point _to, DependencyObject _target, DependencyProperty _property, TimeSpan _duration, TimeSpan _beginTime)
		{
			PointAnimation pointAnimation = new PointAnimation
			{
				To = new Point?(_to),
				EasingFunction = this.backgroundAnimationFunc,
				Duration = _duration,
				BeginTime = new TimeSpan?(_beginTime)
			};
			Storyboard.SetTarget(pointAnimation, _target);
			Storyboard.SetTargetProperty(pointAnimation, new PropertyPath(_property));
			_sb.Children.Add(pointAnimation);
		}

		// Token: 0x0400081D RID: 2077
		private bool isMouseOverInternal;

		// Token: 0x0400081E RID: 2078
		private Border contentContainer;

		// Token: 0x0400081F RID: 2079
		private PathFigure StartFigure;

		// Token: 0x04000820 RID: 2080
		private ArcSegment LeftArc;

		// Token: 0x04000821 RID: 2081
		private LineSegment CenterLine1;

		// Token: 0x04000822 RID: 2082
		private ArcSegment RightArc;

		// Token: 0x04000823 RID: 2083
		private LineSegment CenterLine2;

		// Token: 0x04000824 RID: 2084
		private Path ContentBackground;

		// Token: 0x04000825 RID: 2085
		private EasingFunctionBase backgroundAnimationFunc;

		// Token: 0x04000826 RID: 2086
		private Storyboard currentSb;

		// Token: 0x04000827 RID: 2087
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(AnimationExtentButton), new PropertyMetadata(null, new PropertyChangedCallback(AnimationExtentButton.OnHeaderPropertyChanged)));

		// Token: 0x04000828 RID: 2088
		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(AnimationExtentButton), new PropertyMetadata(null));
	}
}
