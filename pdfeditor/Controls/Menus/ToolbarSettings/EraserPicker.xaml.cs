using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x0200026E RID: 622
	public partial class EraserPicker : UserControl
	{
		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x06002433 RID: 9267 RVA: 0x000A851B File Offset: 0x000A671B
		// (set) Token: 0x06002434 RID: 9268 RVA: 0x000A852D File Offset: 0x000A672D
		public ToolbarSettingInkEraserModel Model
		{
			get
			{
				return (ToolbarSettingInkEraserModel)base.GetValue(EraserPicker.ModelProperty);
			}
			set
			{
				base.SetValue(EraserPicker.ModelProperty, value);
			}
		}

		// Token: 0x06002435 RID: 9269 RVA: 0x000A853B File Offset: 0x000A673B
		public EraserPicker()
		{
			this.InitializeComponent();
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x000A8549 File Offset: 0x000A6749
		private void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			this.EraserBarPopup.IsOpen = false;
			this.Model.IsChecked = true;
		}

		// Token: 0x04000F49 RID: 3913
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingInkEraserModel), typeof(EraserPicker), new PropertyMetadata(null));
	}
}
