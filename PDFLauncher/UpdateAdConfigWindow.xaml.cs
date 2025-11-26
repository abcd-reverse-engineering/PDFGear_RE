using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Common;

namespace PDFLauncher
{
	// Token: 0x0200000C RID: 12
	public partial class UpdateAdConfigWindow : Window
	{
		// Token: 0x06000033 RID: 51 RVA: 0x0000281C File Offset: 0x00000A1C
		public UpdateAdConfigWindow()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.UpdateAdConfigWindow_IsVisibleChanged;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000283C File Offset: 0x00000A3C
		private void UpdateAdConfigWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					base.Hide();
				}));
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000285F File Offset: 0x00000A5F
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Task.Run(TaskExceptionHelper.ExceptionBoundary(async delegate
			{
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
				{
					await Task.Delay(10000);
					base.Close();
				}));
			}));
		}
	}
}
