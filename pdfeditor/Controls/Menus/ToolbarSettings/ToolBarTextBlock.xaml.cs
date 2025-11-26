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
	// Token: 0x02000270 RID: 624
	public partial class ToolBarTextBlock : UserControl
	{
		// Token: 0x06002441 RID: 9281 RVA: 0x000A8728 File Offset: 0x000A6928
		public ToolBarTextBlock()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x06002442 RID: 9282 RVA: 0x000A8736 File Offset: 0x000A6936
		// (set) Token: 0x06002443 RID: 9283 RVA: 0x000A8748 File Offset: 0x000A6948
		public ToolBarSettingTextBlock Model
		{
			get
			{
				return (ToolBarSettingTextBlock)base.GetValue(ToolBarTextBlock.ModelProperty);
			}
			set
			{
				base.SetValue(ToolBarTextBlock.ModelProperty, value);
			}
		}

		// Token: 0x04000F52 RID: 3922
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolBarSettingTextBlock), typeof(ToolBarTextBlock), new PropertyMetadata(null));
	}
}
