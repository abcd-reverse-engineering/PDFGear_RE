using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommonLib.Common;
using CommonLib.IAP;

namespace pdfeditor.Controls.Users
{
	// Token: 0x020001E3 RID: 483
	public partial class UserInfoControl : Control
	{
		// Token: 0x06001B55 RID: 6997 RVA: 0x0006F32C File Offset: 0x0006D52C
		static UserInfoControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(UserInfoControl), new FrameworkPropertyMetadata(typeof(UserInfoControl)));
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x0006F3A0 File Offset: 0x0006D5A0
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.PlanButton != null)
			{
				this.PlanButton.Click -= this.PlanButton_Click;
			}
			if (this.LogoutButton != null)
			{
				this.LogoutButton.Click -= this.LogoutButton_Click;
			}
			if (this.popup != null)
			{
				this.popup.Opened -= this.Popup_Opened;
			}
			this.LayoutRoot = base.GetTemplateChild("LayoutRoot") as Grid;
			this.ContentText = base.GetTemplateChild("ContentText") as ContentControl;
			this.PremiumBadge1 = base.GetTemplateChild("PremiumBadge1") as Image;
			this.PremiumBadge2 = base.GetTemplateChild("PremiumBadge2") as Image;
			this.EmailText = base.GetTemplateChild("EmailText") as TextBlock;
			this.PlanText = base.GetTemplateChild("PlanText") as TextBlock;
			this.ExpireText = base.GetTemplateChild("ExpireText") as TextBlock;
			this.PlanButton = base.GetTemplateChild("PlanButton") as Button;
			this.LogoutButton = base.GetTemplateChild("LogoutButton") as Button;
			this.popup = base.GetTemplateChild("popup") as Popup;
			if (this.PlanButton != null)
			{
				this.PlanButton.Click += this.PlanButton_Click;
			}
			if (this.LogoutButton != null)
			{
				this.LogoutButton.Click += this.LogoutButton_Click;
			}
			if (this.popup != null)
			{
				this.popup.Opened += this.Popup_Opened;
			}
			this.UpdateUserInfo();
		}

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06001B58 RID: 7000 RVA: 0x0006F54F File Offset: 0x0006D74F
		// (set) Token: 0x06001B59 RID: 7001 RVA: 0x0006F561 File Offset: 0x0006D761
		public UserInfo UserInfo
		{
			get
			{
				return (UserInfo)base.GetValue(UserInfoControl.UserInfoProperty);
			}
			set
			{
				base.SetValue(UserInfoControl.UserInfoProperty, value);
			}
		}

		// Token: 0x06001B5A RID: 7002 RVA: 0x0006F570 File Offset: 0x0006D770
		private void UpdateUserInfo()
		{
			UserInfo userInfo = this.UserInfo;
			if (!string.IsNullOrEmpty((userInfo != null) ? userInfo.Email : null))
			{
				if (this.LayoutRoot != null)
				{
					this.LayoutRoot.Visibility = Visibility.Visible;
				}
				string nextTextElement = StringInfo.GetNextTextElement(userInfo.Email);
				if (this.ContentText != null)
				{
					this.ContentText.Content = nextTextElement;
				}
				bool flag = userInfo.Premium;
				if (flag && userInfo.IsSubscription && (userInfo.ExpireTime == null || (userInfo.ExpireTime.Value - DateTime.UtcNow).TotalSeconds < 0.0))
				{
					flag = false;
				}
				if (this.PremiumBadge1 != null)
				{
					this.PremiumBadge1.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
				}
				if (this.PremiumBadge2 != null)
				{
					this.PremiumBadge2.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
				}
				if (this.EmailText != null)
				{
					this.EmailText.Text = userInfo.Email;
				}
				if (flag)
				{
					if (this.PlanText != null)
					{
						this.PlanText.Text = "Plan";
					}
					if (userInfo.IsSubscription)
					{
						if (this.ExpireText != null)
						{
							this.ExpireText.Text = string.Format("Expire on {0:d}", userInfo.ExpireTime.Value);
							return;
						}
					}
					else if (this.ExpireText != null)
					{
						this.ExpireText.Text = "Life-time";
						return;
					}
				}
				else
				{
					if (this.PlanText != null)
					{
						this.PlanText.Text = "Buy Plan";
					}
					if (this.ExpireText != null)
					{
						this.ExpireText.Text = "";
						return;
					}
				}
			}
			else if (this.LayoutRoot != null)
			{
				this.LayoutRoot.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x0006F720 File Offset: 0x0006D920
		public void Open()
		{
			if (this.popup != null && !this.popup.IsOpen && this.LayoutRoot.Visibility == Visibility.Visible)
			{
				this.popup.IsOpen = true;
			}
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x0006F750 File Offset: 0x0006D950
		private async void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("MainWindow", "Logout", "Count", 1L);
			this.popup.IsOpen = false;
			await IAPHelper.LogoutAsync();
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x0006F787 File Offset: 0x0006D987
		private void PlanButton_Click(object sender, RoutedEventArgs e)
		{
			string text = null;
			UserInfo userInfo = this.UserInfo;
			IAPHelper.LaunchBuyPlanUri(text, (userInfo != null) ? userInfo.Email : null);
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x0006F7A1 File Offset: 0x0006D9A1
		private void Popup_Opened(object sender, EventArgs e)
		{
			GAManager.SendEvent("MainWindow", "ShowPurchaseInfo", "Count", 1L);
		}

		// Token: 0x040009B3 RID: 2483
		private Grid LayoutRoot;

		// Token: 0x040009B4 RID: 2484
		private ContentControl ContentText;

		// Token: 0x040009B5 RID: 2485
		private Image PremiumBadge1;

		// Token: 0x040009B6 RID: 2486
		private Image PremiumBadge2;

		// Token: 0x040009B7 RID: 2487
		private TextBlock EmailText;

		// Token: 0x040009B8 RID: 2488
		private TextBlock PlanText;

		// Token: 0x040009B9 RID: 2489
		private TextBlock ExpireText;

		// Token: 0x040009BA RID: 2490
		private Button PlanButton;

		// Token: 0x040009BB RID: 2491
		private Button LogoutButton;

		// Token: 0x040009BC RID: 2492
		private Popup popup;

		// Token: 0x040009BD RID: 2493
		public static readonly DependencyProperty UserInfoProperty = DependencyProperty.Register("UserInfo", typeof(UserInfo), typeof(UserInfoControl), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			UserInfoControl userInfoControl = s as UserInfoControl;
			if (userInfoControl != null && !object.Equals(a.NewValue, a.OldValue))
			{
				userInfoControl.UpdateUserInfo();
			}
		}));
	}
}
