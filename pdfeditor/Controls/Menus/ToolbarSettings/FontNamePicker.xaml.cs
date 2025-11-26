using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000274 RID: 628
	public partial class FontNamePicker : UserControl
	{
		// Token: 0x06002461 RID: 9313 RVA: 0x000A8CBC File Offset: 0x000A6EBC
		public FontNamePicker()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x06002462 RID: 9314 RVA: 0x000A8CCA File Offset: 0x000A6ECA
		// (set) Token: 0x06002463 RID: 9315 RVA: 0x000A8CDC File Offset: 0x000A6EDC
		public ToolbarSettingItemFontNameModel Model
		{
			get
			{
				return (ToolbarSettingItemFontNameModel)base.GetValue(FontNamePicker.ModelProperty);
			}
			set
			{
				base.SetValue(FontNamePicker.ModelProperty, value);
			}
		}

		// Token: 0x06002464 RID: 9316 RVA: 0x000A8CEC File Offset: 0x000A6EEC
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				FontNamePicker fontNamePicker = d as FontNamePicker;
				if (fontNamePicker != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemFontNameModel, EventArgs>.RemoveHandler((ToolbarSettingItemFontNameModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontNamePicker.Model_SelectedValueChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemFontNameModel, EventArgs>.AddHandler((ToolbarSettingItemFontNameModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontNamePicker.Model_SelectedValueChanged));
					}
					fontNamePicker.UpdatePickerComboBoxItems();
					fontNamePicker.UpdateSelectedValue();
				}
			}
		}

		// Token: 0x06002465 RID: 9317 RVA: 0x000A8D75 File Offset: 0x000A6F75
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			this.UpdateSelectedValue();
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x000A8D80 File Offset: 0x000A6F80
		private void UpdateSelectedValue()
		{
			this.comboBox.SelectionChanged -= this.ComboBox_SelectionChanged;
			if (this.Model == null)
			{
				this.comboBox.SelectedIndex = -1;
			}
			else if (this.itemSource != null)
			{
				string text = this.Model.SelectedValue as string;
				if (text != null)
				{
					if (this.itemSource.Count != this.Model.AllFonts.Count)
					{
						this.itemSource.RemoveAt(0);
					}
					if (!this.Model.AllFonts.Contains(text))
					{
						this.itemSource.Insert(0, text);
					}
					this.comboBox.SelectedItem = text;
				}
			}
			this.comboBox.SelectionChanged += this.ComboBox_SelectionChanged;
		}

		// Token: 0x06002467 RID: 9319 RVA: 0x000A8E44 File Offset: 0x000A7044
		private void UpdatePickerComboBoxItems()
		{
			this.comboBox.SelectionChanged -= this.ComboBox_SelectionChanged;
			if (this.Model == null)
			{
				this.comboBox.ItemsSource = null;
				this.itemSource = null;
			}
			else
			{
				this.itemSource = new ObservableCollection<string>(this.Model.AllFonts);
				this.comboBox.ItemsSource = this.itemSource;
			}
			this.comboBox.SelectionChanged += this.ComboBox_SelectionChanged;
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x000A8EC4 File Offset: 0x000A70C4
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.Model == null)
			{
				return;
			}
			string text = e.AddedItems.OfType<string>().FirstOrDefault<string>();
			if (text != null)
			{
				this.Model.SelectedValue = text;
				this.Model.ExecuteCommand();
			}
		}

		// Token: 0x04000F5E RID: 3934
		private ObservableCollection<string> itemSource;

		// Token: 0x04000F5F RID: 3935
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemFontNameModel), typeof(FontNamePicker), new PropertyMetadata(null, new PropertyChangedCallback(FontNamePicker.OnModelPropertyChanged)));
	}
}
