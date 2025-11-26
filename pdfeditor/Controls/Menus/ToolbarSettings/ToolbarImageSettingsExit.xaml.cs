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
	// Token: 0x0200026F RID: 623
	public partial class ToolbarImageSettingsExit : UserControl
	{
		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x0600243A RID: 9274 RVA: 0x000A8659 File Offset: 0x000A6859
		// (set) Token: 0x0600243B RID: 9275 RVA: 0x000A866B File Offset: 0x000A686B
		public ToolbarSettingItemImageExitModel Model
		{
			get
			{
				return (ToolbarSettingItemImageExitModel)base.GetValue(ToolbarImageSettingsExit.ModelProperty);
			}
			set
			{
				base.SetValue(ToolbarImageSettingsExit.ModelProperty, value);
			}
		}

		// Token: 0x0600243C RID: 9276 RVA: 0x000A8679 File Offset: 0x000A6879
		public ToolbarImageSettingsExit()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600243D RID: 9277 RVA: 0x000A8687 File Offset: 0x000A6887
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			ToolbarSettingItemImageExitModel model = this.Model;
			if (model == null)
			{
				return;
			}
			model.ExecuteCommand();
		}

		// Token: 0x04000F4F RID: 3919
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemImageExitModel), typeof(ToolbarImageSettingsExit), new PropertyMetadata(null));
	}
}
