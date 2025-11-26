using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using pdfconverter.Controls;

namespace pdfconverter.Utils
{
	// Token: 0x02000043 RID: 67
	public static class ProgressUtils
	{
		// Token: 0x06000513 RID: 1299 RVA: 0x00014EDC File Offset: 0x000130DC
		public static bool ShowProgressRing(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return ProgressUtils.ShowProgressDialogCore(func, title, content, InternalProgressMode.ProgressRing, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x00014EEC File Offset: 0x000130EC
		public static bool ShowProgressBar(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return func != null && ProgressUtils.ShowProgressDialogCore(func, title, content, InternalProgressMode.ProgressBar, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00014F01 File Offset: 0x00013101
		public static bool ShowProgressDialog(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, InternalProgressMode mode, bool isCancellable, Window ownerWindow, int millisecondsDelay = 0)
		{
			return ProgressUtils.ShowProgressDialogCore(func, title, content, mode, isCancellable, ownerWindow, millisecondsDelay);
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x00014F14 File Offset: 0x00013114
		private static bool ShowProgressDialogCore(Func<ProgressUtils.ProgressAction, Task> func, string title, object content, InternalProgressMode mode, bool isCancelEnabled, Window ownerWindow, int millisecondsDelay)
		{
			if (func == null)
			{
				return false;
			}
			title = title ?? string.Empty;
			Progress<double> progress = new Progress<double>();
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner;
			if (ownerWindow == null)
			{
				windowStartupLocation = WindowStartupLocation.CenterScreen;
			}
			ConvertProgressWindow progressWindow = new ConvertProgressWindow
			{
				Title = title,
				Content = content,
				IsCancellable = isCancelEnabled,
				Owner = ownerWindow,
				WindowStartupLocation = windowStartupLocation,
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

		// Token: 0x06000517 RID: 1303 RVA: 0x0001500C File Offset: 0x0001320C
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

		// Token: 0x02000146 RID: 326
		public class ProgressAction
		{
			// Token: 0x060008F7 RID: 2295 RVA: 0x000264D0 File Offset: 0x000246D0
			public ProgressAction(Action setComplete, Func<bool> getIndeterminate, Action<bool> setIndeterminate, IProgress<double> progress, CancellationToken cancellationToken)
			{
				this.setComplete = setComplete;
				this.getIndeterminate = getIndeterminate;
				this.setIndeterminate = setIndeterminate;
				this.progress = progress;
				this.CancellationToken = cancellationToken;
			}

			// Token: 0x060008F8 RID: 2296 RVA: 0x000264FD File Offset: 0x000246FD
			public void Complete()
			{
				this.setComplete();
			}

			// Token: 0x060008F9 RID: 2297 RVA: 0x0002650A File Offset: 0x0002470A
			public void Report(double progress)
			{
				this.progress.Report(progress);
			}

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x060008FA RID: 2298 RVA: 0x00026518 File Offset: 0x00024718
			// (set) Token: 0x060008FB RID: 2299 RVA: 0x00026525 File Offset: 0x00024725
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

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x060008FC RID: 2300 RVA: 0x00026533 File Offset: 0x00024733
			public CancellationToken CancellationToken { get; }

			// Token: 0x04000647 RID: 1607
			private readonly Action setComplete;

			// Token: 0x04000648 RID: 1608
			private readonly Action<bool> setIndeterminate;

			// Token: 0x04000649 RID: 1609
			private readonly Func<bool> getIndeterminate;

			// Token: 0x0400064A RID: 1610
			private readonly IProgress<double> progress;
		}
	}
}
