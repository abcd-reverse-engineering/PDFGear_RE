using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace pdfeditor.Controls.ProgressWindow
{
	// Token: 0x02000224 RID: 548
	public partial class InternalProgressWindow : Window
	{
		// Token: 0x06001E98 RID: 7832 RVA: 0x00088176 File Offset: 0x00086376
		public InternalProgressWindow()
		{
			this.InitializeComponent();
			this.showDialogCts = new CancellationTokenSource();
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x0008818F File Offset: 0x0008638F
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.UpdateProgressModeState();
			this.UpdateCancelStates();
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06001E9A RID: 7834 RVA: 0x000881A3 File Offset: 0x000863A3
		// (set) Token: 0x06001E9B RID: 7835 RVA: 0x000881B5 File Offset: 0x000863B5
		public bool IsIndeterminate
		{
			get
			{
				return (bool)base.GetValue(InternalProgressWindow.IsIndeterminateProperty);
			}
			set
			{
				base.SetValue(InternalProgressWindow.IsIndeterminateProperty, value);
			}
		}

		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06001E9C RID: 7836 RVA: 0x000881C8 File Offset: 0x000863C8
		// (set) Token: 0x06001E9D RID: 7837 RVA: 0x000881DA File Offset: 0x000863DA
		public double Value
		{
			get
			{
				return (double)base.GetValue(InternalProgressWindow.ValueProperty);
			}
			set
			{
				base.SetValue(InternalProgressWindow.ValueProperty, value);
			}
		}

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x06001E9E RID: 7838 RVA: 0x000881ED File Offset: 0x000863ED
		// (set) Token: 0x06001E9F RID: 7839 RVA: 0x000881FF File Offset: 0x000863FF
		public InternalProgressMode ProgressMode
		{
			get
			{
				return (InternalProgressMode)base.GetValue(InternalProgressWindow.ProgressModeProperty);
			}
			set
			{
				base.SetValue(InternalProgressWindow.ProgressModeProperty, value);
			}
		}

		// Token: 0x06001EA0 RID: 7840 RVA: 0x00088214 File Offset: 0x00086414
		private static void OnProgressModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				InternalProgressWindow internalProgressWindow = d as InternalProgressWindow;
				if (internalProgressWindow != null)
				{
					internalProgressWindow.UpdateProgressModeState();
				}
			}
		}

		// Token: 0x06001EA1 RID: 7841 RVA: 0x00088246 File Offset: 0x00086446
		private void UpdateProgressModeState()
		{
			if (this.ProgressMode == InternalProgressMode.ProgressRing)
			{
				VisualStateManager.GoToState(this, "RingMode", true);
				return;
			}
			VisualStateManager.GoToState(this, "BarMode", true);
		}

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x0008826C File Offset: 0x0008646C
		// (set) Token: 0x06001EA3 RID: 7843 RVA: 0x0008827E File Offset: 0x0008647E
		public bool IsCancellable
		{
			get
			{
				return (bool)base.GetValue(InternalProgressWindow.IsCancellableProperty);
			}
			set
			{
				base.SetValue(InternalProgressWindow.IsCancellableProperty, value);
			}
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x00088294 File Offset: 0x00086494
		private static void OnIsCancellablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				InternalProgressWindow internalProgressWindow = d as InternalProgressWindow;
				if (internalProgressWindow != null)
				{
					internalProgressWindow.UpdateCancelStates();
				}
			}
		}

		// Token: 0x06001EA5 RID: 7845 RVA: 0x000882C6 File Offset: 0x000864C6
		private void UpdateCancelStates()
		{
			if (this.IsCancellable)
			{
				VisualStateManager.GoToState(this, "CanCancel", true);
				return;
			}
			VisualStateManager.GoToState(this, "CannotCancel", true);
		}

		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x000882EB File Offset: 0x000864EB
		// (set) Token: 0x06001EA7 RID: 7847 RVA: 0x000882FD File Offset: 0x000864FD
		public bool IsCompleted
		{
			get
			{
				return (bool)base.GetValue(InternalProgressWindow.IsCompletedProperty);
			}
			private set
			{
				base.SetValue(InternalProgressWindow.IsCompletedPropertyKey, value);
			}
		}

		// Token: 0x06001EA8 RID: 7848 RVA: 0x00088310 File Offset: 0x00086510
		public void Complete()
		{
			this.IsCompleted = true;
			this.showDialogCts.Cancel();
			if (base.IsVisible)
			{
				base.Close();
			}
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x00088332 File Offset: 0x00086532
		protected override void OnClosing(CancelEventArgs e)
		{
			if (!this.IsCompleted && !this.IsCancellable)
			{
				e.Cancel = true;
			}
			this.showDialogCts.Cancel();
			base.OnClosing(e);
		}

		// Token: 0x06001EAA RID: 7850 RVA: 0x00088360 File Offset: 0x00086560
		public bool? ShowDialog(int millisecondsDelay)
		{
			InternalProgressWindow.<>c__DisplayClass31_0 CS$<>8__locals1 = new InternalProgressWindow.<>c__DisplayClass31_0();
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

		// Token: 0x04000BCF RID: 3023
		private CancellationTokenSource showDialogCts;

		// Token: 0x04000BD0 RID: 3024
		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(InternalProgressWindow), new PropertyMetadata(false));

		// Token: 0x04000BD1 RID: 3025
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(InternalProgressWindow), new PropertyMetadata(0.0));

		// Token: 0x04000BD2 RID: 3026
		public static readonly DependencyProperty ProgressModeProperty = DependencyProperty.Register("ProgressMode", typeof(InternalProgressMode), typeof(InternalProgressWindow), new PropertyMetadata(InternalProgressMode.ProgressBar, new PropertyChangedCallback(InternalProgressWindow.OnProgressModePropertyChanged)));

		// Token: 0x04000BD3 RID: 3027
		public static readonly DependencyProperty IsCancellableProperty = DependencyProperty.Register("IsCancellable", typeof(bool), typeof(InternalProgressWindow), new PropertyMetadata(true, new PropertyChangedCallback(InternalProgressWindow.OnIsCancellablePropertyChanged)));

		// Token: 0x04000BD4 RID: 3028
		public static readonly DependencyProperty IsCompletedProperty = InternalProgressWindow.IsCompletedPropertyKey.DependencyProperty;

		// Token: 0x04000BD5 RID: 3029
		public static readonly DependencyPropertyKey IsCompletedPropertyKey = DependencyProperty.RegisterReadOnly("IsCompleted", typeof(bool), typeof(InternalProgressWindow), new PropertyMetadata(false));
	}
}
