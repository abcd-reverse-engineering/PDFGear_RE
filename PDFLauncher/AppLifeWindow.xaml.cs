using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace PDFLauncher
{
	// Token: 0x02000006 RID: 6
	public partial class AppLifeWindow : Window
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000020FE File Offset: 0x000002FE
		public AppLifeWindow()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.AppLifeWindow_IsVisibleChanged;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000211E File Offset: 0x0000031E
		private void AppLifeWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					base.Hide();
				}));
			}
		}
	}
}
