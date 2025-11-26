using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Utils.Behaviors;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000275 RID: 629
	public partial class FontSizePicker : UserControl
	{
		// Token: 0x0600246C RID: 9324 RVA: 0x000A8F89 File Offset: 0x000A7189
		public FontSizePicker()
		{
			this.InitializeComponent();
			this.textBox.IsUndoEnabled = false;
		}

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x0600246D RID: 9325 RVA: 0x000A8FA3 File Offset: 0x000A71A3
		// (set) Token: 0x0600246E RID: 9326 RVA: 0x000A8FB5 File Offset: 0x000A71B5
		public ToolbarSettingItemFontSizeModel Model
		{
			get
			{
				return (ToolbarSettingItemFontSizeModel)base.GetValue(FontSizePicker.ModelProperty);
			}
			set
			{
				base.SetValue(FontSizePicker.ModelProperty, value);
			}
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x000A8FC4 File Offset: 0x000A71C4
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				FontSizePicker fontSizePicker = d as FontSizePicker;
				if (fontSizePicker != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemFontSizeModel, EventArgs>.RemoveHandler((ToolbarSettingItemFontSizeModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontSizePicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemFontSizeModel, EventArgs>.RemoveHandler((ToolbarSettingItemFontSizeModel)e.OldValue, "StandardItemsChanged", new EventHandler<EventArgs>(fontSizePicker.Model_StandardItemsChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemFontSizeModel, EventArgs>.AddHandler((ToolbarSettingItemFontSizeModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(fontSizePicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemFontSizeModel, EventArgs>.AddHandler((ToolbarSettingItemFontSizeModel)e.NewValue, "StandardItemsChanged", new EventHandler<EventArgs>(fontSizePicker.Model_StandardItemsChanged));
					}
					fontSizePicker.UpdatePickerComboBoxItems();
					fontSizePicker.UpdateSelectedValue();
				}
			}
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x000A9097 File Offset: 0x000A7297
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			this.UpdateSelectedValue();
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x000A909F File Offset: 0x000A729F
		private void Model_StandardItemsChanged(object sender, EventArgs e)
		{
			this.UpdatePickerComboBoxItems();
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x000A90A8 File Offset: 0x000A72A8
		private void UpdateSelectedValue()
		{
			ToolbarSettingItemFontSizeModel model = this.Model;
			object obj = ((model != null) ? model.SelectedValue : null);
			if (obj is float)
			{
				float num = (float)obj;
				this._TextBoxEditBehavior.Text = string.Format("{0} pt", num);
				return;
			}
			this._TextBoxEditBehavior.Text = null;
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x000A9100 File Offset: 0x000A7300
		private void UpdatePickerComboBoxItems()
		{
			this.comboBox.SelectionChanged -= this.ComboBox_SelectionChanged;
			this.itemSource = null;
			ToolbarSettingItemFontSizeModel model = this.Model;
			if (((model != null) ? model.StandardItems : null) == null || this.Model.StandardItems.Count == 0)
			{
				this.comboBox.Visibility = Visibility.Collapsed;
				this.comboBoxDropButton.Visibility = Visibility.Collapsed;
				this.textBox.Padding = new Thickness(0.0);
			}
			else
			{
				this.comboBox.Visibility = Visibility.Visible;
				this.comboBoxDropButton.Visibility = Visibility.Visible;
				this.textBox.Padding = new Thickness(0.0, 0.0, 20.0, 0.0);
				this.itemSource = new ObservableCollection<string>(this.Model.StandardItems.Select((float c) => string.Format("{0} pt", c)));
			}
			this.comboBox.ItemsSource = this.itemSource;
			this.comboBox.SelectionChanged += this.ComboBox_SelectionChanged;
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x000A9238 File Offset: 0x000A7438
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.Model == null)
			{
				return;
			}
			if (e.AddedItems.Count > 0)
			{
				string text = (string)e.AddedItems[0];
				this._TextBoxEditBehavior.Text = text;
				((ComboBox)sender).SelectedItem = null;
			}
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x000A9286 File Offset: 0x000A7486
		private void FontSizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.comboBox.IsDropDownOpen = true;
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000A9294 File Offset: 0x000A7494
		private void _TextBoxEditBehavior_TextChanged(object sender, EventArgs e)
		{
			float? num = null;
			object selectedValue = this.Model.SelectedValue;
			if (selectedValue is float)
			{
				float num2 = (float)selectedValue;
				num = new float?(num2);
			}
			float num3;
			if (this.TryParseFontSize(this._TextBoxEditBehavior.Text, out num3))
			{
				if (num == null || num.Value != num3)
				{
					this.Model.SelectedValue = num3;
					this.Model.ExecuteCommand();
					return;
				}
			}
			else
			{
				if (num != null)
				{
					this._TextBoxEditBehavior.Text = string.Format("{0} pt", num.Value);
					return;
				}
				this._TextBoxEditBehavior.Text = "";
			}
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x000A934C File Offset: 0x000A754C
		private bool TryParseFontSize(string text, out float value)
		{
			value = 0f;
			text = ((text != null) ? text.Trim() : null);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text.Length > 2 && (text[text.Length - 2] == 'p' || text[text.Length - 2] == 'P') && (text[text.Length - 1] == 't' || text[text.Length - 1] == 'T'))
			{
				text = text.Substring(0, text.Length - 2).Trim();
			}
			return float.TryParse(text, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out value) || float.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x000A93FF File Offset: 0x000A75FF
		private void comboBoxDropButton_Click(object sender, RoutedEventArgs e)
		{
			this.comboBox.IsDropDownOpen = true;
		}

		// Token: 0x04000F62 RID: 3938
		private ObservableCollection<string> itemSource;

		// Token: 0x04000F63 RID: 3939
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemFontSizeModel), typeof(FontSizePicker), new PropertyMetadata(null, new PropertyChangedCallback(FontSizePicker.OnModelPropertyChanged)));
	}
}
