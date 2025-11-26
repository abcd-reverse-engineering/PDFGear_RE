using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000276 RID: 630
	public partial class FontStyleButton : UserControl
	{
		// Token: 0x0600247D RID: 9341 RVA: 0x000A9535 File Offset: 0x000A7735
		public FontStyleButton()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x0600247E RID: 9342 RVA: 0x000A9543 File Offset: 0x000A7743
		// (set) Token: 0x0600247F RID: 9343 RVA: 0x000A9555 File Offset: 0x000A7755
		public ToolbarSettingItemModel Model
		{
			get
			{
				return (ToolbarSettingItemModel)base.GetValue(FontStyleButton.ModelProperty);
			}
			set
			{
				base.SetValue(FontStyleButton.ModelProperty, value);
			}
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x000A9564 File Offset: 0x000A7764
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				FontStyleButton fontStyleButton = d as FontStyleButton;
				if (fontStyleButton != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemModel, EventArgs>.RemoveHandler((ToolbarSettingItemModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontStyleButton.Model_SelectedValueChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemModel, EventArgs>.AddHandler((ToolbarSettingItemModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontStyleButton.Model_SelectedValueChanged));
					}
					fontStyleButton.UpdateModel();
				}
			}
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x000A95E8 File Offset: 0x000A77E8
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			ToggleButton button = this.Button;
			ToolbarSettingItemModel model = this.Model;
			object obj = ((model != null) ? model.SelectedValue : null);
			button.IsChecked = new bool?(obj is bool && (bool)obj);
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x000A962C File Offset: 0x000A782C
		private void UpdateModel()
		{
			if (this.Model is ToolbarSettingItemBoldModel)
			{
				this.Button.Content = "B";
				this.Button.FontStyle = FontStyles.Normal;
				this.Button.FontWeight = FontWeights.Bold;
				this.Button.Padding = default(Thickness);
				this.Button.FontFamily = new FontFamily("Arial");
				return;
			}
			if (this.Model is ToolbarSettingItemItalicModel)
			{
				this.Button.Content = "I";
				this.Button.FontStyle = FontStyles.Italic;
				this.Button.FontWeight = FontWeights.Normal;
				this.Button.Padding = new Thickness(0.0, 0.0, 2.0, 0.0);
				this.Button.FontFamily = new FontFamily("Times New Roman");
			}
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x000A9728 File Offset: 0x000A7928
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			object selectedValue = this.Model.SelectedValue;
			if (selectedValue is bool && (bool)selectedValue)
			{
				this.Model.SelectedValue = false;
				this.Model.ExecuteCommand();
				return;
			}
			this.Model.SelectedValue = true;
			this.Model.ExecuteCommand();
		}

		// Token: 0x04000F6B RID: 3947
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemModel), typeof(FontStyleButton), new PropertyMetadata(null, new PropertyChangedCallback(FontStyleButton.OnModelPropertyChanged)));
	}
}
