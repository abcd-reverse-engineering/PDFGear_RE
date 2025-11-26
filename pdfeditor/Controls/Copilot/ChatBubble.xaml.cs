using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x0200028A RID: 650
	public partial class ChatBubble : ContentControl
	{
		// Token: 0x0600256D RID: 9581 RVA: 0x000AE33C File Offset: 0x000AC53C
		static ChatBubble()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatBubble), new FrameworkPropertyMetadata(typeof(ChatBubble)));
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x000AE3D4 File Offset: 0x000AC5D4
		public ChatBubble()
		{
			this.TemplateSettings = new ChatBubble.ChatBubbleTemplateSettings();
			this.hideTimer = new DispatcherTimer();
			this.hideTimer.Interval = TimeSpan.FromSeconds(2.0);
			this.hideTimer.Tick += this.HideTimer_Tick;
			base.Unloaded += this.ChatBubble_Unloaded;
			base.SizeChanged += this.ChatBubble_SizeChanged;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x000AE451 File Offset: 0x000AC651
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.UpdateBackgroundSize();
			this.UpdateVisibleState(false);
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x000AE466 File Offset: 0x000AC666
		public void ShowBubble(TimeSpan? autoHideTime)
		{
			this.hideTimer.Stop();
			this.IsVisibleOverride = true;
			if (autoHideTime != null)
			{
				this.hideTimer.Interval = autoHideTime.Value;
				this.hideTimer.Start();
			}
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x000AE4A0 File Offset: 0x000AC6A0
		private void ChatBubble_Unloaded(object sender, RoutedEventArgs e)
		{
			this.hideTimer.Stop();
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x000AE4AD File Offset: 0x000AC6AD
		private void ChatBubble_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateBackgroundSize();
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x000AE4B5 File Offset: 0x000AC6B5
		private void HideTimer_Tick(object sender, EventArgs e)
		{
			this.hideTimer.Stop();
			this.IsVisibleOverride = false;
		}

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x06002574 RID: 9588 RVA: 0x000AE4C9 File Offset: 0x000AC6C9
		// (set) Token: 0x06002575 RID: 9589 RVA: 0x000AE4DB File Offset: 0x000AC6DB
		protected bool IsVisibleOverride
		{
			get
			{
				return (bool)base.GetValue(ChatBubble.IsVisibleOverrideProperty);
			}
			set
			{
				base.SetValue(ChatBubble.IsVisibleOverrideProperty, value);
			}
		}

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x06002576 RID: 9590 RVA: 0x000AE4EE File Offset: 0x000AC6EE
		// (set) Token: 0x06002577 RID: 9591 RVA: 0x000AE500 File Offset: 0x000AC700
		internal ChatBubble.ChatBubbleTemplateSettings TemplateSettings
		{
			get
			{
				return (ChatBubble.ChatBubbleTemplateSettings)base.GetValue(ChatBubble.TemplateSettingsProperty);
			}
			set
			{
				base.SetValue(ChatBubble.TemplateSettingsProperty, value);
			}
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x000AE50E File Offset: 0x000AC70E
		private void UpdateVisibleState(bool useTransitions)
		{
			if (this.IsVisibleOverride)
			{
				VisualStateManager.GoToState(this, "VisibleState", true);
				return;
			}
			VisualStateManager.GoToState(this, "InvisibleState", true);
		}

		// Token: 0x06002579 RID: 9593 RVA: 0x000AE534 File Offset: 0x000AC734
		private void UpdateBackgroundSize()
		{
			if (base.ActualWidth >= 40.0 && base.ActualHeight >= 10.0)
			{
				this.TemplateSettings.BackgroundPathVisibility = Visibility.Visible;
				this.TemplateSettings.RectGeometryRect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight - 10.0);
				this.TemplateSettings.TriangleGeometryTranslateX = base.ActualWidth - 40.0;
				this.TemplateSettings.TriangleGeometryTranslateY = base.ActualHeight - 10.0;
				this.TemplateSettings.BackgroundScaleTransformCenterX = base.ActualWidth - 40.0;
				this.TemplateSettings.BackgroundScaleTransformCenterY = base.ActualHeight;
				return;
			}
			this.TemplateSettings.BackgroundPathVisibility = Visibility.Collapsed;
		}

		// Token: 0x0400101A RID: 4122
		private DispatcherTimer hideTimer;

		// Token: 0x0400101B RID: 4123
		protected static readonly DependencyProperty IsVisibleOverrideProperty = DependencyProperty.Register("IsVisibleOverride", typeof(bool), typeof(ChatBubble), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatBubble chatBubble = s as ChatBubble;
			if (chatBubble != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatBubble.hideTimer.Stop();
				chatBubble.UpdateVisibleState(chatBubble.IsLoaded);
			}
		}));

		// Token: 0x0400101C RID: 4124
		internal static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register("TemplateSettings", typeof(ChatBubble.ChatBubbleTemplateSettings), typeof(ChatBubble), new PropertyMetadata(null));

		// Token: 0x02000743 RID: 1859
		internal class ChatBubbleTemplateSettings : DependencyObject
		{
			// Token: 0x17000D92 RID: 3474
			// (get) Token: 0x06003689 RID: 13961 RVA: 0x00111ABA File Offset: 0x0010FCBA
			// (set) Token: 0x0600368A RID: 13962 RVA: 0x00111ACC File Offset: 0x0010FCCC
			public double TriangleGeometryTranslateX
			{
				get
				{
					return (double)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.TriangleGeometryTranslateXProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.TriangleGeometryTranslateXProperty, value);
				}
			}

			// Token: 0x17000D93 RID: 3475
			// (get) Token: 0x0600368B RID: 13963 RVA: 0x00111ADF File Offset: 0x0010FCDF
			// (set) Token: 0x0600368C RID: 13964 RVA: 0x00111AF1 File Offset: 0x0010FCF1
			public double TriangleGeometryTranslateY
			{
				get
				{
					return (double)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.TriangleGeometryTranslateYProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.TriangleGeometryTranslateYProperty, value);
				}
			}

			// Token: 0x17000D94 RID: 3476
			// (get) Token: 0x0600368D RID: 13965 RVA: 0x00111B04 File Offset: 0x0010FD04
			// (set) Token: 0x0600368E RID: 13966 RVA: 0x00111B16 File Offset: 0x0010FD16
			public Rect RectGeometryRect
			{
				get
				{
					return (Rect)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.RectGeometryRectProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.RectGeometryRectProperty, value);
				}
			}

			// Token: 0x17000D95 RID: 3477
			// (get) Token: 0x0600368F RID: 13967 RVA: 0x00111B29 File Offset: 0x0010FD29
			// (set) Token: 0x06003690 RID: 13968 RVA: 0x00111B3B File Offset: 0x0010FD3B
			public Visibility BackgroundPathVisibility
			{
				get
				{
					return (Visibility)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundPathVisibilityProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundPathVisibilityProperty, value);
				}
			}

			// Token: 0x17000D96 RID: 3478
			// (get) Token: 0x06003691 RID: 13969 RVA: 0x00111B4E File Offset: 0x0010FD4E
			// (set) Token: 0x06003692 RID: 13970 RVA: 0x00111B60 File Offset: 0x0010FD60
			public double BackgroundScaleTransformCenterX
			{
				get
				{
					return (double)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundScaleTransformCenterXProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundScaleTransformCenterXProperty, value);
				}
			}

			// Token: 0x17000D97 RID: 3479
			// (get) Token: 0x06003693 RID: 13971 RVA: 0x00111B73 File Offset: 0x0010FD73
			// (set) Token: 0x06003694 RID: 13972 RVA: 0x00111B85 File Offset: 0x0010FD85
			public double BackgroundScaleTransformCenterY
			{
				get
				{
					return (double)base.GetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundScaleTransformCenterYProperty);
				}
				set
				{
					base.SetValue(ChatBubble.ChatBubbleTemplateSettings.BackgroundScaleTransformCenterYProperty, value);
				}
			}

			// Token: 0x040024DF RID: 9439
			public static readonly DependencyProperty TriangleGeometryTranslateXProperty = DependencyProperty.Register("TriangleGeometryTranslateX", typeof(double), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(0.0));

			// Token: 0x040024E0 RID: 9440
			public static readonly DependencyProperty TriangleGeometryTranslateYProperty = DependencyProperty.Register("TriangleGeometryTranslateY", typeof(double), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(0.0));

			// Token: 0x040024E1 RID: 9441
			public static readonly DependencyProperty RectGeometryRectProperty = DependencyProperty.Register("RectGeometryRect", typeof(Rect), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(new Rect(0.0, 0.0, 0.0, 0.0)));

			// Token: 0x040024E2 RID: 9442
			public static readonly DependencyProperty BackgroundPathVisibilityProperty = DependencyProperty.Register("BackgroundPathVisibility", typeof(Visibility), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(Visibility.Visible));

			// Token: 0x040024E3 RID: 9443
			public static readonly DependencyProperty BackgroundScaleTransformCenterXProperty = DependencyProperty.Register("BackgroundScaleTransformCenterX", typeof(double), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(0.0));

			// Token: 0x040024E4 RID: 9444
			public static readonly DependencyProperty BackgroundScaleTransformCenterYProperty = DependencyProperty.Register("BackgroundScaleTransformCenterY", typeof(double), typeof(ChatBubble.ChatBubbleTemplateSettings), new PropertyMetadata(0.0));
		}
	}
}
