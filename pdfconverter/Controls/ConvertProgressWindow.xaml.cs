using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace pdfconverter.Controls
{
	// Token: 0x0200009C RID: 156
	public partial class ConvertProgressWindow : Window
	{
		// Token: 0x060006D5 RID: 1749 RVA: 0x000185F2 File Offset: 0x000167F2
		public ConvertProgressWindow()
		{
			this.InitializeComponent();
			this.showDialogCts = new CancellationTokenSource();
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0001872E File Offset: 0x0001692E
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.UpdateProgressModeState();
			this.UpdateCancelStates();
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00018742 File Offset: 0x00016942
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00018754 File Offset: 0x00016954
		public bool IsIndeterminate
		{
			get
			{
				return (bool)base.GetValue(ConvertProgressWindow.IsIndeterminateProperty);
			}
			set
			{
				base.SetValue(ConvertProgressWindow.IsIndeterminateProperty, value);
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x00018767 File Offset: 0x00016967
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x00018779 File Offset: 0x00016979
		public double Value
		{
			get
			{
				return (double)base.GetValue(ConvertProgressWindow.ValueProperty);
			}
			set
			{
				base.SetValue(ConvertProgressWindow.ValueProperty, value);
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0001878C File Offset: 0x0001698C
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x0001879E File Offset: 0x0001699E
		public InternalProgressMode ProgressMode
		{
			get
			{
				return (InternalProgressMode)base.GetValue(ConvertProgressWindow.ProgressModeProperty);
			}
			set
			{
				base.SetValue(ConvertProgressWindow.ProgressModeProperty, value);
			}
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x000187B4 File Offset: 0x000169B4
		private static void OnProgressModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ConvertProgressWindow convertProgressWindow = d as ConvertProgressWindow;
				if (convertProgressWindow != null)
				{
					convertProgressWindow.UpdateProgressModeState();
				}
			}
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x000187E6 File Offset: 0x000169E6
		private void UpdateProgressModeState()
		{
			if (this.ProgressMode == InternalProgressMode.ProgressRing)
			{
				VisualStateManager.GoToState(this, "RingMode", true);
				return;
			}
			VisualStateManager.GoToState(this, "BarMode", true);
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0001880C File Offset: 0x00016A0C
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0001881E File Offset: 0x00016A1E
		public bool IsCancellable
		{
			get
			{
				return (bool)base.GetValue(ConvertProgressWindow.IsCancellableProperty);
			}
			set
			{
				base.SetValue(ConvertProgressWindow.IsCancellableProperty, value);
			}
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x00018834 File Offset: 0x00016A34
		private static void OnIsCancellablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				ConvertProgressWindow convertProgressWindow = d as ConvertProgressWindow;
				if (convertProgressWindow != null)
				{
					convertProgressWindow.UpdateCancelStates();
				}
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00018866 File Offset: 0x00016A66
		private void UpdateCancelStates()
		{
			if (this.IsCancellable)
			{
				VisualStateManager.GoToState(this, "CanCancel", true);
				return;
			}
			VisualStateManager.GoToState(this, "CannotCancel", true);
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0001888B File Offset: 0x00016A8B
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0001889D File Offset: 0x00016A9D
		public bool IsCompleted
		{
			get
			{
				return (bool)base.GetValue(ConvertProgressWindow.IsCompletedProperty);
			}
			private set
			{
				base.SetValue(ConvertProgressWindow.IsCompletedPropertyKey, value);
			}
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x000188B0 File Offset: 0x00016AB0
		public void Complete()
		{
			this.IsCompleted = true;
			this.showDialogCts.Cancel();
			if (base.IsVisible)
			{
				base.Close();
			}
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x000188D2 File Offset: 0x00016AD2
		protected override void OnClosing(CancelEventArgs e)
		{
			if (!this.IsCompleted && !this.IsCancellable)
			{
				e.Cancel = true;
			}
			this.showDialogCts.Cancel();
			base.OnClosing(e);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x00018900 File Offset: 0x00016B00
		public bool? ShowDialog(int millisecondsDelay)
		{
			ConvertProgressWindow.<>c__DisplayClass31_0 CS$<>8__locals1 = new ConvertProgressWindow.<>c__DisplayClass31_0();
			CS$<>8__locals1.millisecondsDelay = millisecondsDelay;
			CS$<>8__locals1.<>4__this = this;
			if (CS$<>8__locals1.millisecondsDelay <= 0)
			{
				return base.ShowDialog();
			}
			CS$<>8__locals1.frame = new DispatcherFrame(true)
			{
				Continue = true
			};
			CS$<>8__locals1.<ShowDialog>g__RunCore|0();
			Dispatcher.PushFrame(CS$<>8__locals1.frame);
			if (this.showDialogCts.IsCancellationRequested)
			{
				return null;
			}
			return base.ShowDialog();
		}

		// Token: 0x04000348 RID: 840
		private CancellationTokenSource showDialogCts;

		// Token: 0x04000349 RID: 841
		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ConvertProgressWindow), new PropertyMetadata(false));

		// Token: 0x0400034A RID: 842
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ConvertProgressWindow), new PropertyMetadata(0.0));

		// Token: 0x0400034B RID: 843
		public static readonly DependencyProperty ProgressModeProperty = DependencyProperty.Register("ProgressMode", typeof(InternalProgressMode), typeof(ConvertProgressWindow), new PropertyMetadata(InternalProgressMode.ProgressBar, new PropertyChangedCallback(ConvertProgressWindow.OnProgressModePropertyChanged)));

		// Token: 0x0400034C RID: 844
		public static readonly DependencyProperty IsCancellableProperty = DependencyProperty.Register("IsCancellable", typeof(bool), typeof(ConvertProgressWindow), new PropertyMetadata(true, new PropertyChangedCallback(ConvertProgressWindow.OnIsCancellablePropertyChanged)));

		// Token: 0x0400034D RID: 845
		public static readonly DependencyProperty IsCompletedProperty = ConvertProgressWindow.IsCompletedPropertyKey.DependencyProperty;

		// Token: 0x0400034E RID: 846
		public static readonly DependencyPropertyKey IsCompletedPropertyKey = DependencyProperty.RegisterReadOnly("IsCompleted", typeof(bool), typeof(ConvertProgressWindow), new PropertyMetadata(false));
	}
}
