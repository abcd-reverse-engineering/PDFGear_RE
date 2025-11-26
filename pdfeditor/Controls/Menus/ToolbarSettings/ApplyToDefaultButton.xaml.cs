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
	// Token: 0x02000271 RID: 625
	public partial class ApplyToDefaultButton : UserControl
	{
		// Token: 0x06002447 RID: 9287 RVA: 0x000A87BC File Offset: 0x000A69BC
		public ApplyToDefaultButton()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x06002448 RID: 9288 RVA: 0x000A87CA File Offset: 0x000A69CA
		// (set) Token: 0x06002449 RID: 9289 RVA: 0x000A87DC File Offset: 0x000A69DC
		public ToolbarSettingItemApplyToDefaultModel Model
		{
			get
			{
				return (ToolbarSettingItemApplyToDefaultModel)base.GetValue(ApplyToDefaultButton.ModelProperty);
			}
			set
			{
				base.SetValue(ApplyToDefaultButton.ModelProperty, value);
			}
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x000A87EA File Offset: 0x000A69EA
		private void applyToDefault_Click(object sender, RoutedEventArgs e)
		{
			ToolbarSettingItemApplyToDefaultModel model = this.Model;
			if (model == null)
			{
				return;
			}
			model.ExecuteCommand();
		}

		// Token: 0x04000F54 RID: 3924
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemApplyToDefaultModel), typeof(ApplyToDefaultButton), new PropertyMetadata(null));
	}
}
