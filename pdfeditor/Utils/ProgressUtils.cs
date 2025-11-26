using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using pdfeditor.Controls.ProgressWindow;

namespace pdfeditor.Utils
{
	// Token: 0x0200009C RID: 156
	public static class ProgressUtils
	{
		// Token: 0x060009F7 RID: 2551 RVA: 0x000328E1 File Offset: 0x00030AE1
		public static bool ShowProgressRing(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return ProgressUtils.ShowProgressDialogCore(func, title, content, InternalProgressMode.ProgressRing, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x000328F1 File Offset: 0x00030AF1
		public static bool ShowProgressBar(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return func != null && ProgressUtils.ShowProgressDialogCore(func, title, content, InternalProgressMode.ProgressBar, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x00032906 File Offset: 0x00030B06
		public static bool ShowProgressDialog(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, InternalProgressMode mode, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return ProgressUtils.ShowProgressDialogCore(func, title, content, mode, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00032918 File Offset: 0x00030B18
		private static bool ShowProgressDialogCore(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, InternalProgressMode mode, bool isCancelEnabled, Window ownerWindow, int millisecondsDelay)
		{
			if (func == null)
			{
				return false;
			}
			title = title ?? string.Empty;
			Progress<double> progress = new Progress<double>();
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			InternalProgressWindow progressWindow = new InternalProgressWindow
			{
				Title = title,
				Content = content,
				IsCancellable = isCancelEnabled,
				Owner = ownerWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ProgressMode = mode
			};
			ProgressUtils.ProgressAction progressAction = new ProgressUtils.ProgressAction(delegate
			{
				progressWindow.Complete();
			}, () => progressWindow.IsIndeterminate, delegate(bool c)
			{
				progressWindow.IsIndeterminate = c;
			}, progress, cancellationTokenSource.Token);
			ProgressUtils.<ShowProgressDialogCore>g__RunCore|3_4(progressWindow.Dispatcher, func, progressAction);
			progress.ProgressChanged += delegate(object s, double a)
			{
				progressWindow.Value = a;
			};
			progressWindow.ShowDialog(millisecondsDelay);
			if (!progressWindow.IsCompleted)
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource.Dispose();
			}
			return progressWindow.IsCompleted;
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00032A08 File Offset: 0x00030C08
		[CompilerGenerated]
		internal static async void <ShowProgressDialogCore>g__RunCore|3_4(Dispatcher _dispatcher, Func<ProgressUtils.ProgressAction, Task> _func, ProgressUtils.ProgressAction data)
		{
			ProgressUtils.<>c__DisplayClass3_1 CS$<>8__locals1 = new ProgressUtils.<>c__DisplayClass3_1();
			CS$<>8__locals1._func = _func;
			CS$<>8__locals1.data = data;
			await _dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				ProgressUtils.<>c__DisplayClass3_1.<<ShowProgressDialogCore>b__5>d <<ShowProgressDialogCore>b__5>d;
				<<ShowProgressDialogCore>b__5>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<ShowProgressDialogCore>b__5>d.<>4__this = CS$<>8__locals1;
				<<ShowProgressDialogCore>b__5>d.<>1__state = -1;
				<<ShowProgressDialogCore>b__5>d.<>t__builder.Start<ProgressUtils.<>c__DisplayClass3_1.<<ShowProgressDialogCore>b__5>d>(ref <<ShowProgressDialogCore>b__5>d);
			}));
		}

		// Token: 0x02000489 RID: 1161
		public class ProgressAction
		{
			// Token: 0x06002DF7 RID: 11767 RVA: 0x000E10C6 File Offset: 0x000DF2C6
			public ProgressAction(Action setComplete, Func<bool> getIndeterminate, Action<bool> setIndeterminate, IProgress<double> progress, CancellationToken cancellationToken)
			{
				this.setComplete = setComplete;
				this.getIndeterminate = getIndeterminate;
				this.setIndeterminate = setIndeterminate;
				this.progress = progress;
				this.CancellationToken = cancellationToken;
			}

			// Token: 0x06002DF8 RID: 11768 RVA: 0x000E10F3 File Offset: 0x000DF2F3
			public void Complete()
			{
				this.setComplete();
			}

			// Token: 0x06002DF9 RID: 11769 RVA: 0x000E1100 File Offset: 0x000DF300
			public void Report(double progress)
			{
				this.progress.Report(progress);
			}

			// Token: 0x17000CAF RID: 3247
			// (get) Token: 0x06002DFA RID: 11770 RVA: 0x000E110E File Offset: 0x000DF30E
			// (set) Token: 0x06002DFB RID: 11771 RVA: 0x000E111B File Offset: 0x000DF31B
			public bool IsIndeterminate
			{
				get
				{
					return this.getIndeterminate();
				}
				set
				{
					this.setIndeterminate(value);
				}
			}

			// Token: 0x17000CB0 RID: 3248
			// (get) Token: 0x06002DFC RID: 11772 RVA: 0x000E1129 File Offset: 0x000DF329
			public CancellationToken CancellationToken { get; }

			// Token: 0x040019D5 RID: 6613
			private readonly Action setComplete;

			// Token: 0x040019D6 RID: 6614
			private readonly Action<bool> setIndeterminate;

			// Token: 0x040019D7 RID: 6615
			private readonly Func<bool> getIndeterminate;

			// Token: 0x040019D8 RID: 6616
			private readonly IProgress<double> progress;
		}
	}
}
