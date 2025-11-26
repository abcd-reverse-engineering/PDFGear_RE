using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace pdfeditor.Utils.Behaviors
{
	// Token: 0x0200011F RID: 287
	public class ResizeAnimationBehavior : Behavior<FrameworkElement>
	{
		// Token: 0x06000CEA RID: 3306 RVA: 0x000419C2 File Offset: 0x0003FBC2
		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.SizeChanged += this.AssociatedObject_SizeChanged;
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x000419E1 File Offset: 0x0003FBE1
		protected override void OnDetaching()
		{
			this.TryStopAnimation();
			base.OnDetaching();
			base.AssociatedObject.SizeChanged -= this.AssociatedObject_SizeChanged;
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x00041A06 File Offset: 0x0003FC06
		// (set) Token: 0x06000CED RID: 3309 RVA: 0x00041A18 File Offset: 0x0003FC18
		public bool KeepAspectRatio
		{
			get
			{
				return (bool)base.GetValue(ResizeAnimationBehavior.KeepAspectRatioProperty);
			}
			set
			{
				base.SetValue(ResizeAnimationBehavior.KeepAspectRatioProperty, value);
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000CEE RID: 3310 RVA: 0x00041A2B File Offset: 0x0003FC2B
		// (set) Token: 0x06000CEF RID: 3311 RVA: 0x00041A3D File Offset: 0x0003FC3D
		public double HorizontalAlignmentRatio
		{
			get
			{
				return (double)base.GetValue(ResizeAnimationBehavior.HorizontalAlignmentRatioProperty);
			}
			set
			{
				base.SetValue(ResizeAnimationBehavior.HorizontalAlignmentRatioProperty, value);
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000CF0 RID: 3312 RVA: 0x00041A50 File Offset: 0x0003FC50
		// (set) Token: 0x06000CF1 RID: 3313 RVA: 0x00041A62 File Offset: 0x0003FC62
		public double VerticalAlignmentRatio
		{
			get
			{
				return (double)base.GetValue(ResizeAnimationBehavior.VerticalAlignmentRatioProperty);
			}
			set
			{
				base.SetValue(ResizeAnimationBehavior.VerticalAlignmentRatioProperty, value);
			}
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x00041A78 File Offset: 0x0003FC78
		private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.TryStopAnimation();
			Matrix startMatrix = this.GetStartMatrix(e.PreviousSize, e.NewSize);
			if (startMatrix.IsIdentity)
			{
				return;
			}
			MatrixTransform matrixTransform = new MatrixTransform(startMatrix);
			Storyboard storyboard = this.CreateAnimation(TimeSpan.FromSeconds(0.15), startMatrix, matrixTransform);
			storyboard.Completed += delegate(object s, EventArgs a)
			{
				this.TryStopAnimation();
			};
			base.AssociatedObject.RenderTransform = matrixTransform;
			storyboard.Begin();
			this.currentSb = storyboard;
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x00041AF1 File Offset: 0x0003FCF1
		private void TryStopAnimation()
		{
			if (this.currentSb != null)
			{
				this.currentSb.Stop();
				this.currentSb = null;
				base.AssociatedObject.RenderTransform = null;
			}
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x00041B1C File Offset: 0x0003FD1C
		private Matrix GetStartMatrix(Size oldSize, Size newSize)
		{
			if (newSize.Width == 0.0 || newSize.Height == 0.0)
			{
				return Matrix.Identity;
			}
			double num = oldSize.Width / newSize.Width;
			double num2 = oldSize.Height / newSize.Height;
			if (this.KeepAspectRatio && num != num2)
			{
				if (num == 1.0)
				{
					num = num2;
				}
				else if (num2 == 1.0)
				{
					num2 = num;
				}
			}
			double num3 = (newSize.Width - oldSize.Width) * this.HorizontalAlignmentRatio;
			double num4 = (newSize.Height - oldSize.Height) * this.VerticalAlignmentRatio;
			Matrix identity = Matrix.Identity;
			identity.Scale(num, num2);
			identity.Translate(num3, num4);
			return identity;
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x00041BE8 File Offset: 0x0003FDE8
		private Storyboard CreateAnimation(TimeSpan duration, Matrix startValue, MatrixTransform matrixTransform)
		{
			if (duration.TotalSeconds <= 0.0)
			{
				return null;
			}
			ResizeAnimationBehavior.LinearMatrixAnimation linearMatrixAnimation = new ResizeAnimationBehavior.LinearMatrixAnimation
			{
				Duration = new Duration(duration),
				From = new Matrix?(startValue),
				To = new Matrix?(Matrix.Identity)
			};
			Storyboard.SetTarget(linearMatrixAnimation, matrixTransform);
			Storyboard.SetTargetProperty(linearMatrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));
			linearMatrixAnimation.Freeze();
			return new Storyboard
			{
				Children = { linearMatrixAnimation }
			};
		}

		// Token: 0x04000598 RID: 1432
		public static readonly DependencyProperty KeepAspectRatioProperty = DependencyProperty.Register("KeepAspectRatio", typeof(bool), typeof(ResizeAnimationBehavior), new PropertyMetadata(false));

		// Token: 0x04000599 RID: 1433
		public static readonly DependencyProperty HorizontalAlignmentRatioProperty = DependencyProperty.Register("HorizontalAlignmentRatio", typeof(double), typeof(ResizeAnimationBehavior), new PropertyMetadata(0.0));

		// Token: 0x0400059A RID: 1434
		public static readonly DependencyProperty VerticalAlignmentRatioProperty = DependencyProperty.Register("VerticalAlignmentRatio", typeof(double), typeof(ResizeAnimationBehavior), new PropertyMetadata(0.0));

		// Token: 0x0400059B RID: 1435
		private Storyboard currentSb;

		// Token: 0x02000534 RID: 1332
		public class LinearMatrixAnimation : AnimationTimeline
		{
			// Token: 0x17000D11 RID: 3345
			// (get) Token: 0x06003071 RID: 12401 RVA: 0x000EE921 File Offset: 0x000ECB21
			// (set) Token: 0x06003070 RID: 12400 RVA: 0x000EE90E File Offset: 0x000ECB0E
			public Matrix? From
			{
				get
				{
					return new Matrix?((Matrix)base.GetValue(ResizeAnimationBehavior.LinearMatrixAnimation.FromProperty));
				}
				set
				{
					base.SetValue(ResizeAnimationBehavior.LinearMatrixAnimation.FromProperty, value);
				}
			}

			// Token: 0x17000D12 RID: 3346
			// (get) Token: 0x06003073 RID: 12403 RVA: 0x000EE94B File Offset: 0x000ECB4B
			// (set) Token: 0x06003072 RID: 12402 RVA: 0x000EE938 File Offset: 0x000ECB38
			public Matrix? To
			{
				get
				{
					return new Matrix?((Matrix)base.GetValue(ResizeAnimationBehavior.LinearMatrixAnimation.ToProperty));
				}
				set
				{
					base.SetValue(ResizeAnimationBehavior.LinearMatrixAnimation.ToProperty, value);
				}
			}

			// Token: 0x06003074 RID: 12404 RVA: 0x000EE962 File Offset: 0x000ECB62
			public LinearMatrixAnimation()
			{
			}

			// Token: 0x06003075 RID: 12405 RVA: 0x000EE96A File Offset: 0x000ECB6A
			public LinearMatrixAnimation(Matrix from, Matrix to, Duration duration)
			{
				base.Duration = duration;
				this.From = new Matrix?(from);
				this.To = new Matrix?(to);
			}

			// Token: 0x06003076 RID: 12406 RVA: 0x000EE994 File Offset: 0x000ECB94
			public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
			{
				if (animationClock.CurrentProgress == null)
				{
					return null;
				}
				double value = animationClock.CurrentProgress.Value;
				Matrix matrix = this.From ?? ((Matrix)defaultOriginValue);
				if (this.To != null)
				{
					Matrix value2 = this.To.Value;
					return new Matrix((value2.M11 - matrix.M11) * value + matrix.M11, 0.0, 0.0, (value2.M22 - matrix.M22) * value + matrix.M22, (value2.OffsetX - matrix.OffsetX) * value + matrix.OffsetX, (value2.OffsetY - matrix.OffsetY) * value + matrix.OffsetY);
				}
				return Matrix.Identity;
			}

			// Token: 0x06003077 RID: 12407 RVA: 0x000EEA90 File Offset: 0x000ECC90
			protected override Freezable CreateInstanceCore()
			{
				return new ResizeAnimationBehavior.LinearMatrixAnimation();
			}

			// Token: 0x17000D13 RID: 3347
			// (get) Token: 0x06003078 RID: 12408 RVA: 0x000EEA97 File Offset: 0x000ECC97
			public override Type TargetPropertyType
			{
				get
				{
					return typeof(Matrix);
				}
			}

			// Token: 0x04001CF6 RID: 7414
			public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(ResizeAnimationBehavior.LinearMatrixAnimation), new PropertyMetadata(null));

			// Token: 0x04001CF7 RID: 7415
			public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(ResizeAnimationBehavior.LinearMatrixAnimation), new PropertyMetadata(null));
		}
	}
}
