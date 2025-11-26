using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000273 RID: 627
	public partial class ExitEditingButton : UserControl
	{
		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x0600245A RID: 9306 RVA: 0x000A8BF0 File Offset: 0x000A6DF0
		// (set) Token: 0x0600245B RID: 9307 RVA: 0x000A8C02 File Offset: 0x000A6E02
		public ToolbarSettingItemExitModel Model
		{
			get
			{
				return (ToolbarSettingItemExitModel)base.GetValue(ExitEditingButton.ModelProperty);
			}
			set
			{
				base.SetValue(ExitEditingButton.ModelProperty, value);
			}
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x000A8C10 File Offset: 0x000A6E10
		public ExitEditingButton()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x000A8C1E File Offset: 0x000A6E1E
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			ToolbarSettingItemExitModel model = this.Model;
			if (model == null)
			{
				return;
			}
			model.ExecuteCommand();
		}

		// Token: 0x04000F5B RID: 3931
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemExitModel), typeof(ExitEditingButton), new PropertyMetadata(null));
	}
}
