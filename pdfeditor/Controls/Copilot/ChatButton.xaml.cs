using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x0200028B RID: 651
	public partial class ChatButton : Button
	{
		// Token: 0x0600257A RID: 9594 RVA: 0x000AE61C File Offset: 0x000AC81C
		static ChatButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatButton), new FrameworkPropertyMetadata(typeof(ChatButton)));
		}

		// Token: 0x0600257B RID: 9595 RVA: 0x000AE6C8 File Offset: 0x000AC8C8
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.UpdateVisibleState(false);
			this.UpdateTipsShowStates(false);
		}

		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x0600257C RID: 9596 RVA: 0x000AE6DE File Offset: 0x000AC8DE
		// (set) Token: 0x0600257D RID: 9597 RVA: 0x000AE6F0 File Offset: 0x000AC8F0
		public bool IsTipsShow
		{
			get
			{
				return (bool)base.GetValue(ChatButton.IsTipsShowProperty);
			}
			set
			{
				base.SetValue(ChatButton.IsTipsShowProperty, value);
			}
		}

		// Token: 0x0600257E RID: 9598 RVA: 0x000AE703 File Offset: 0x000AC903
		public static bool GetIsVisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(ChatButton.IsVisibleProperty);
		}

		// Token: 0x0600257F RID: 9599 RVA: 0x000AE715 File Offset: 0x000AC915
		public static void SetIsVisible(DependencyObject obj, bool value)
		{
			obj.SetValue(ChatButton.IsVisibleProperty, value);
		}

		// Token: 0x06002580 RID: 9600 RVA: 0x000AE728 File Offset: 0x000AC928
		private void UpdateVisibleState(bool useTransitions = true)
		{
			if (ChatButton.GetIsVisible(this))
			{
				VisualStateManager.GoToState(this, "Visible", useTransitions);
				base.Focusable = true;
				return;
			}
			VisualStateManager.GoToState(this, "Invisible", useTransitions);
			base.Focusable = false;
			DispatcherTimer dispatcherTimer = this.timer;
			if (dispatcherTimer != null)
			{
				dispatcherTimer.Stop();
			}
			this.timer = null;
			this.IsTipsShow = false;
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x000AE785 File Offset: 0x000AC985
		private void UpdateTipsShowStates(bool useTransitions = true)
		{
			if (this.IsTipsShow)
			{
				VisualStateManager.GoToState(this, "TipsShowedState", useTransitions);
				return;
			}
			VisualStateManager.GoToState(this, "TipsHidedState", useTransitions);
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x000AE7AC File Offset: 0x000AC9AC
		public void ShowTips()
		{
			this.IsTipsShow = true;
			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer.Start();
				return;
			}
			this.timer = new DispatcherTimer(DispatcherPriority.Normal);
			this.timer.Interval = TimeSpan.FromSeconds(5.0);
			this.timer.Tick += this.Timer_Tick;
			this.timer.Start();
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x000AE827 File Offset: 0x000ACA27
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer = null;
			}
			this.IsTipsShow = false;
		}

		// Token: 0x0400101D RID: 4125
		public static readonly DependencyProperty IsTipsShowProperty = DependencyProperty.Register("IsTipsShow", typeof(bool), typeof(ChatButton), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatButton chatButton = s as ChatButton;
			if (chatButton != null && !object.Equals(a.NewValue, a.OldValue))
			{
				chatButton.UpdateTipsShowStates(chatButton.IsLoaded);
			}
		}));

		// Token: 0x0400101E RID: 4126
		public new static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(ChatButton), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ChatButton chatButton2 = s as ChatButton;
			if (chatButton2 != null)
			{
				chatButton2.UpdateVisibleState(true);
			}
		}));

		// Token: 0x0400101F RID: 4127
		private DispatcherTimer timer;
	}
}
