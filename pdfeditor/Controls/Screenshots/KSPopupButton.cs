using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000210 RID: 528
	public class KSPopupButton : Button
	{
		// Token: 0x06001D3D RID: 7485 RVA: 0x0007E44C File Offset: 0x0007C64C
		static KSPopupButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(KSPopupButton), new FrameworkPropertyMetadata(typeof(KSPopupButton)));
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x0007E629 File Offset: 0x0007C829
		public KSPopupButton()
		{
			base.Loaded += this.KSPopupButton_Loaded;
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x0007E644 File Offset: 0x0007C844
		private void KSPopupButton_Loaded(object sender, RoutedEventArgs e)
		{
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				window.Deactivated -= this.Window_Deactivated;
				window.Deactivated += this.Window_Deactivated;
				window.PreviewMouseDown -= this.Window_PreviewMouseDown;
				window.PreviewMouseDown += this.Window_PreviewMouseDown;
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0007E6A3 File Offset: 0x0007C8A3
		private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.InnerPopup != null && this.InnerPopup.IsOpen && !base.IsMouseOver)
			{
				this.InnerPopup.IsOpen = false;
			}
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0007E6CE File Offset: 0x0007C8CE
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.InnerPopup != null && this.InnerPopup.IsOpen && !base.IsMouseOver && this.PopupOpenMode != EnumKSPopupOpenMode.OpenOnCode)
			{
				this.InnerPopup.IsOpen = false;
			}
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0007E701 File Offset: 0x0007C901
		private void Window_Deactivated(object sender, EventArgs e)
		{
			if (this.InnerPopup != null && this.PopupOpenMode != EnumKSPopupOpenMode.OpenOnCode)
			{
				this.InnerPopup.IsOpen = false;
			}
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x0007E720 File Offset: 0x0007C920
		private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			KSPopupButton kspopupButton = d as KSPopupButton;
			if (kspopupButton.IsOpen)
			{
				kspopupButton.InnerPopup.IsOpen = true;
				return;
			}
			kspopupButton.InnerPopup.IsOpen = false;
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x0007E755 File Offset: 0x0007C955
		private static void OnPopupContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			KSPopupButton kspopupButton = d as KSPopupButton;
			kspopupButton.RemoveLogicalChild(e.OldValue);
			kspopupButton.AddLogicalChild(e.NewValue);
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06001D45 RID: 7493 RVA: 0x0007E776 File Offset: 0x0007C976
		// (set) Token: 0x06001D46 RID: 7494 RVA: 0x0007E788 File Offset: 0x0007C988
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(KSPopupButton.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.CornerRadiusProperty, value);
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06001D47 RID: 7495 RVA: 0x0007E79B File Offset: 0x0007C99B
		// (set) Token: 0x06001D48 RID: 7496 RVA: 0x0007E7AD File Offset: 0x0007C9AD
		public PlacementMode PopupPlacement
		{
			get
			{
				return (PlacementMode)base.GetValue(KSPopupButton.PopupPlacementProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.PopupPlacementProperty, value);
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06001D49 RID: 7497 RVA: 0x0007E7C0 File Offset: 0x0007C9C0
		// (set) Token: 0x06001D4A RID: 7498 RVA: 0x0007E7D2 File Offset: 0x0007C9D2
		public double PopupVerticalOffset
		{
			get
			{
				return (double)base.GetValue(KSPopupButton.PopupVerticalOffsetProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.PopupVerticalOffsetProperty, value);
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06001D4B RID: 7499 RVA: 0x0007E7E5 File Offset: 0x0007C9E5
		// (set) Token: 0x06001D4C RID: 7500 RVA: 0x0007E7F7 File Offset: 0x0007C9F7
		public double PopupHorizontalOffset
		{
			get
			{
				return (double)base.GetValue(KSPopupButton.PopupHorizontalOffsetProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.PopupHorizontalOffsetProperty, value);
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x0007E80A File Offset: 0x0007CA0A
		// (set) Token: 0x06001D4E RID: 7502 RVA: 0x0007E81C File Offset: 0x0007CA1C
		public EnumKSPopupOpenMode PopupOpenMode
		{
			get
			{
				return (EnumKSPopupOpenMode)base.GetValue(KSPopupButton.PopupOpenModeProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.PopupOpenModeProperty, value);
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06001D4F RID: 7503 RVA: 0x0007E82F File Offset: 0x0007CA2F
		// (set) Token: 0x06001D50 RID: 7504 RVA: 0x0007E83C File Offset: 0x0007CA3C
		public object PopupContent
		{
			get
			{
				return base.GetValue(KSPopupButton.PopupContentProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.PopupContentProperty, value);
			}
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06001D51 RID: 7505 RVA: 0x0007E84A File Offset: 0x0007CA4A
		// (set) Token: 0x06001D52 RID: 7506 RVA: 0x0007E85C File Offset: 0x0007CA5C
		public bool IsOpen
		{
			get
			{
				return (bool)base.GetValue(KSPopupButton.IsOpenProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.IsOpenProperty, value);
			}
		}

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06001D53 RID: 7507 RVA: 0x0007E86F File Offset: 0x0007CA6F
		// (set) Token: 0x06001D54 RID: 7508 RVA: 0x0007E881 File Offset: 0x0007CA81
		public bool UseAnimation
		{
			get
			{
				return (bool)base.GetValue(KSPopupButton.UseAnimationProperty);
			}
			set
			{
				base.SetValue(KSPopupButton.UseAnimationProperty, value);
			}
		}

		// Token: 0x06001D55 RID: 7509 RVA: 0x0007E894 File Offset: 0x0007CA94
		private static void OnPopupOpenModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			KSPopupButton kspopupButton = d as KSPopupButton;
			if (kspopupButton.InnerPopup != null)
			{
				kspopupButton.InnerPopup.IsOpen = false;
			}
		}

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06001D56 RID: 7510 RVA: 0x0007E8BC File Offset: 0x0007CABC
		// (set) Token: 0x06001D57 RID: 7511 RVA: 0x0007E8C4 File Offset: 0x0007CAC4
		public Popup InnerPopup { get; private set; }

		// Token: 0x06001D58 RID: 7512 RVA: 0x0007E8D0 File Offset: 0x0007CAD0
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.InnerPopup = base.GetTemplateChild("my_popup") as Popup;
			if (this.InnerPopup != null)
			{
				this.InnerPopup.Opened += this.Popup_Opened;
				this.InnerPopup.Closed += this.Popup_Closed;
			}
			this.bd_content = base.GetTemplateChild("bd_content") as Border;
			if (this.bd_content != null)
			{
				this.bd_content.PreviewMouseDown += this.Bd_content_PreviewMouseDown;
			}
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x0007E964 File Offset: 0x0007CB64
		private void Bd_content_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.PopupOpenMode == EnumKSPopupOpenMode.OpenOnClick)
			{
				if (this.InnerPopup.IsOpen)
				{
					this.InnerPopup.IsOpen = false;
					return;
				}
				this.InnerPopup.IsOpen = true;
				base.Focus();
			}
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x0007E99C File Offset: 0x0007CB9C
		private void Popup_Closed(object sender, EventArgs e)
		{
			this.IsOpen = false;
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x0007E9A5 File Offset: 0x0007CBA5
		private void Popup_Opened(object sender, EventArgs e)
		{
			this.IsOpen = true;
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x0007E9B0 File Offset: 0x0007CBB0
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			if (this.PopupOpenMode == EnumKSPopupOpenMode.OpenOnHover)
			{
				if (this.closeTimer != null)
				{
					this.closeTimer.Change(-1, -1);
					this.closeTimer.Dispose();
					this.closeTimer = null;
				}
				if (this.UseAnimation)
				{
					if (this.openTimer == null)
					{
						this.openTimer = new Timer(delegate(object obj)
						{
							Dispatcher dispatcher = base.Dispatcher;
							if (dispatcher == null)
							{
								return;
							}
							dispatcher.Invoke(delegate
							{
								this.InnerPopup.IsOpen = true;
								try
								{
									if (this.openTimer != null)
									{
										this.openTimer.Change(-1, -1);
										this.openTimer.Dispose();
										this.openTimer = null;
									}
								}
								catch
								{
								}
							});
						}, null, 100, -1);
						return;
					}
				}
				else
				{
					this.InnerPopup.IsOpen = true;
				}
			}
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x0007EA30 File Offset: 0x0007CC30
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.PopupOpenMode == EnumKSPopupOpenMode.OpenOnHover)
			{
				if (this.closeTimer != null)
				{
					this.closeTimer.Change(-1, -1);
					this.closeTimer.Dispose();
					this.closeTimer = null;
				}
				if (this.openTimer == null && !this.InnerPopup.IsOpen)
				{
					this.InnerPopup.IsOpen = true;
				}
			}
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x0007EA98 File Offset: 0x0007CC98
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			if (this.PopupOpenMode == EnumKSPopupOpenMode.OpenOnHover)
			{
				if (this.openTimer != null)
				{
					this.openTimer.Change(-1, -1);
					this.openTimer.Dispose();
					this.openTimer = null;
				}
				if (this.closeTimer != null)
				{
					this.closeTimer.Change(-1, -1);
					this.closeTimer.Dispose();
					this.closeTimer = null;
				}
				this.closeTimer = new Timer(delegate(object obj)
				{
					Dispatcher dispatcher = base.Dispatcher;
					if (dispatcher == null)
					{
						return;
					}
					dispatcher.Invoke(delegate
					{
						this.InnerPopup.IsOpen = false;
					});
				}, null, 300, -1);
			}
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0007EB23 File Offset: 0x0007CD23
		public void OpenPopup()
		{
			if (this.InnerPopup != null)
			{
				this.InnerPopup.IsOpen = true;
			}
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0007EB39 File Offset: 0x0007CD39
		public void ClosePopup()
		{
			if (this.InnerPopup != null)
			{
				this.InnerPopup.IsOpen = false;
			}
		}

		// Token: 0x04000B04 RID: 2820
		private Timer closeTimer;

		// Token: 0x04000B05 RID: 2821
		private Timer openTimer;

		// Token: 0x04000B06 RID: 2822
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(KSPopupButton), new PropertyMetadata(new CornerRadius(0.0)));

		// Token: 0x04000B07 RID: 2823
		public static readonly DependencyProperty PopupHorizontalOffsetProperty = DependencyProperty.Register("PopupHorizontalOffset", typeof(double), typeof(KSPopupButton), new PropertyMetadata(0.0));

		// Token: 0x04000B08 RID: 2824
		public static readonly DependencyProperty PopupVerticalOffsetProperty = DependencyProperty.Register("PopupVerticalOffset", typeof(double), typeof(KSPopupButton), new PropertyMetadata(0.0));

		// Token: 0x04000B09 RID: 2825
		public static readonly DependencyProperty PopupPlacementProperty = DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(KSPopupButton), new PropertyMetadata(PlacementMode.Top));

		// Token: 0x04000B0A RID: 2826
		public static readonly DependencyProperty PopupOpenModeProperty = DependencyProperty.Register("PopupOpenMode", typeof(EnumKSPopupOpenMode), typeof(KSPopupButton), new UIPropertyMetadata(EnumKSPopupOpenMode.OpenOnHover, new PropertyChangedCallback(KSPopupButton.OnPopupOpenModeChanged)));

		// Token: 0x04000B0B RID: 2827
		public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(KSPopupButton), new UIPropertyMetadata(null, new PropertyChangedCallback(KSPopupButton.OnPopupContentChanged)));

		// Token: 0x04000B0C RID: 2828
		public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(KSPopupButton), new PropertyMetadata(false, new PropertyChangedCallback(KSPopupButton.OnIsOpenChanged)));

		// Token: 0x04000B0D RID: 2829
		public static readonly DependencyProperty UseAnimationProperty = DependencyProperty.Register("UseAnimation", typeof(bool), typeof(KSPopupButton), new PropertyMetadata(true));

		// Token: 0x04000B0F RID: 2831
		private Border bd_content;
	}
}
