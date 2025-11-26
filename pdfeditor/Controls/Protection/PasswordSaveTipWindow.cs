using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfeditor.Controls.Protection
{
	// Token: 0x02000223 RID: 547
	public class PasswordSaveTipWindow : Window, IComponentConnector
	{
		// Token: 0x06001E92 RID: 7826 RVA: 0x00087F58 File Offset: 0x00086158
		public PasswordSaveTipWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x00087F68 File Offset: 0x00086168
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (this.cboxNoMorPrompt.IsChecked.GetValueOrDefault())
			{
				ConfigManager.SetPasswordSaveNoMorePromptFlag(true);
			}
			base.Close();
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x00087F96 File Offset: 0x00086196
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			base.Height = this.grid_root.ActualHeight;
		}

		// Token: 0x06001E95 RID: 7829 RVA: 0x00087FAC File Offset: 0x000861AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/pdfeditor;component/controls/protect/passwordsavetipwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x00087FDC File Offset: 0x000861DC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((PasswordSaveTipWindow)target).Loaded += this.Window_Loaded;
				return;
			case 2:
				this.grid_root = (Grid)target;
				return;
			case 3:
				this.cboxNoMorPrompt = (CheckBox)target;
				return;
			case 4:
				((Button)target).Click += this.Button_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000BCC RID: 3020
		internal Grid grid_root;

		// Token: 0x04000BCD RID: 3021
		internal CheckBox cboxNoMorPrompt;

		// Token: 0x04000BCE RID: 3022
		private bool _contentLoaded;
	}
}
