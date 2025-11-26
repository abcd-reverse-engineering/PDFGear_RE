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
	// Token: 0x0200027D RID: 637
	public partial class IconPicker : UserControl
	{
		// Token: 0x060024CF RID: 9423 RVA: 0x000AA5B8 File Offset: 0x000A87B8
		public IconPicker()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x060024D0 RID: 9424 RVA: 0x000AA5C6 File Offset: 0x000A87C6
		// (set) Token: 0x060024D1 RID: 9425 RVA: 0x000AA5D8 File Offset: 0x000A87D8
		public ToolbarSettingItemIconModel Model
		{
			get
			{
				return (ToolbarSettingItemIconModel)base.GetValue(IconPicker.ModelProperty);
			}
			set
			{
				base.SetValue(IconPicker.ModelProperty, value);
			}
		}

		// Token: 0x060024D2 RID: 9426 RVA: 0x000AA5E6 File Offset: 0x000A87E6
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		// Token: 0x04000F89 RID: 3977
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemIconModel), typeof(IconPicker), new PropertyMetadata(null, new PropertyChangedCallback(IconPicker.OnModelPropertyChanged)));
	}
}
