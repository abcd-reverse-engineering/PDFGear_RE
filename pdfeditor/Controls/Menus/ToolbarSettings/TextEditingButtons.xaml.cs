using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000278 RID: 632
	public partial class TextEditingButtons : UserControl
	{
		// Token: 0x06002497 RID: 9367 RVA: 0x000A9CBD File Offset: 0x000A7EBD
		public TextEditingButtons()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06002498 RID: 9368 RVA: 0x000A9CCB File Offset: 0x000A7ECB
		// (set) Token: 0x06002499 RID: 9369 RVA: 0x000A9CDD File Offset: 0x000A7EDD
		public ToolbarSettingItemTextEditingButtonsModel Model
		{
			get
			{
				return (ToolbarSettingItemTextEditingButtonsModel)base.GetValue(TextEditingButtons.ModelProperty);
			}
			set
			{
				base.SetValue(TextEditingButtons.ModelProperty, value);
			}
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x000A9CEB File Offset: 0x000A7EEB
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			ToolbarSettingItemTextEditingButtonsModel model = this.Model;
			if (model == null)
			{
				return;
			}
			model.ExecuteCommand();
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x000A9D00 File Offset: 0x000A7F00
		private void btnSupport_Click(object sender, RoutedEventArgs e)
		{
			string documentPath = Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.DocumentPath;
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.Owner = App.Current.MainWindow;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			feedbackWindow.source = "EditText";
			if (!string.IsNullOrEmpty(documentPath))
			{
				feedbackWindow.flist.Add(documentPath);
				feedbackWindow.showAttachmentCB(true);
			}
			feedbackWindow.ShowDialog();
		}

		// Token: 0x04000F72 RID: 3954
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemTextEditingButtonsModel), typeof(TextEditingButtons), new PropertyMetadata(null));
	}
}
