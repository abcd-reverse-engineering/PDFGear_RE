using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Controls.ColorPickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000279 RID: 633
	public partial class TextMarkupColorPicker : UserControl
	{
		// Token: 0x0600249F RID: 9375 RVA: 0x000A9E2D File Offset: 0x000A802D
		public TextMarkupColorPicker()
		{
			this.InitializeComponent();
			this.standardColors = new ObservableCollection<TextMarkupColorPicker.NestedItemsControlModel>();
			this.recentColors = new ObservableCollection<TextMarkupColorPicker.NestedItemsControlModel>();
		}

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x060024A0 RID: 9376 RVA: 0x000A9E51 File Offset: 0x000A8051
		// (set) Token: 0x060024A1 RID: 9377 RVA: 0x000A9E63 File Offset: 0x000A8063
		public ToolbarSettingItemColorModel Model
		{
			get
			{
				return (ToolbarSettingItemColorModel)base.GetValue(TextMarkupColorPicker.ModelProperty);
			}
			set
			{
				base.SetValue(TextMarkupColorPicker.ModelProperty, value);
			}
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000A9E74 File Offset: 0x000A8074
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				TextMarkupColorPicker textMarkupColorPicker = d as TextMarkupColorPicker;
				if (textMarkupColorPicker != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemColorModel, EventArgs>.RemoveHandler((ToolbarSettingItemColorModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(textMarkupColorPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemColorModel, EventArgs>.RemoveHandler((ToolbarSettingItemColorModel)e.OldValue, "ColorsChanged", new EventHandler<EventArgs>(textMarkupColorPicker.Model_ColorsChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemColorModel, EventArgs>.AddHandler((ToolbarSettingItemColorModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(textMarkupColorPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemColorModel, EventArgs>.AddHandler((ToolbarSettingItemColorModel)e.NewValue, "ColorsChanged", new EventHandler<EventArgs>(textMarkupColorPicker.Model_ColorsChanged));
					}
					textMarkupColorPicker.UpdateColorCollection();
				}
			}
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000A9F41 File Offset: 0x000A8141
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			this.UpdateRadioButtonCheckedState();
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000A9F49 File Offset: 0x000A8149
		private void Model_ColorsChanged(object sender, EventArgs e)
		{
			this.UpdateColorCollection();
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x000A9F54 File Offset: 0x000A8154
		private void ColorPickerButton_ItemClick(object sender, ColorPickerButtonItemClickEventArgs e)
		{
			if (this.Model != null)
			{
				Color color = e.Item.Color;
				this.Model.SelectedValue = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color.A, color.R, color.G, color.B });
				this.Model.ExecuteCommand();
			}
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x000A9FD8 File Offset: 0x000A81D8
		private void UpdateColorCollection()
		{
			if (this.Model != null)
			{
				this.StandardColorItemsControl.ItemsSource = null;
				this.RecentColorItemsControl.ItemsSource = null;
				this.standardColors.Clear();
				this.recentColors.Clear();
				if (this.Model.StandardColors != null)
				{
					foreach (string text in this.Model.StandardColors)
					{
						this.standardColors.Add(new TextMarkupColorPicker.NestedItemsControlModel(text, this.Model));
					}
				}
				if (this.Model.RecentColors != null)
				{
					foreach (string text2 in this.Model.RecentColors)
					{
						this.recentColors.Add(new TextMarkupColorPicker.NestedItemsControlModel(text2, this.Model));
					}
				}
				this.StandardColorItemsControl.ItemsSource = this.standardColors;
				this.RecentColorItemsControl.ItemsSource = this.recentColors;
			}
			this.UpdateRadioButtonCheckedState();
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x000AA108 File Offset: 0x000A8308
		private void UpdateRadioButtonCheckedState()
		{
			ToolbarSettingItemColorModel model = this.Model;
			string text = ((model != null) ? model.SelectedValue : null) as string;
			Color color;
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				color = (Color)ColorConverter.ConvertFromString(text);
			}
			catch
			{
				return;
			}
			foreach (TextMarkupColorPicker.NestedItemsControlModel nestedItemsControlModel in this.standardColors.Concat(this.recentColors))
			{
				try
				{
					Color color2 = (Color)ColorConverter.ConvertFromString(nestedItemsControlModel.Value);
					nestedItemsControlModel.IsChecked = color2 == color;
				}
				catch
				{
					nestedItemsControlModel.IsChecked = false;
				}
			}
		}

		// Token: 0x04000F76 RID: 3958
		private ObservableCollection<TextMarkupColorPicker.NestedItemsControlModel> standardColors;

		// Token: 0x04000F77 RID: 3959
		private ObservableCollection<TextMarkupColorPicker.NestedItemsControlModel> recentColors;

		// Token: 0x04000F78 RID: 3960
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemColorModel), typeof(TextMarkupColorPicker), new PropertyMetadata(null, new PropertyChangedCallback(TextMarkupColorPicker.OnModelPropertyChanged)));

		// Token: 0x02000728 RID: 1832
		private class NestedItemsControlModel : ObservableObject
		{
			// Token: 0x060035F3 RID: 13811 RVA: 0x0010FDB1 File Offset: 0x0010DFB1
			public NestedItemsControlModel(string value, ToolbarSettingItemColorModel model)
			{
				this.Value = value;
				this.Model = model;
				this.Command = new RelayCommand(delegate
				{
					this.Model.SelectedValue = this.Value;
					this.Model.ExecuteCommand();
				});
			}

			// Token: 0x17000D5C RID: 3420
			// (get) Token: 0x060035F4 RID: 13812 RVA: 0x0010FDDE File Offset: 0x0010DFDE
			public string Value { get; }

			// Token: 0x17000D5D RID: 3421
			// (get) Token: 0x060035F5 RID: 13813 RVA: 0x0010FDE6 File Offset: 0x0010DFE6
			public ToolbarSettingItemColorModel Model { get; }

			// Token: 0x17000D5E RID: 3422
			// (get) Token: 0x060035F6 RID: 13814 RVA: 0x0010FDEE File Offset: 0x0010DFEE
			public ICommand Command { get; }

			// Token: 0x17000D5F RID: 3423
			// (get) Token: 0x060035F7 RID: 13815 RVA: 0x0010FDF6 File Offset: 0x0010DFF6
			// (set) Token: 0x060035F8 RID: 13816 RVA: 0x0010FDFE File Offset: 0x0010DFFE
			public bool IsChecked
			{
				get
				{
					return this.isChecked;
				}
				set
				{
					base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
				}
			}

			// Token: 0x04002466 RID: 9318
			private bool isChecked;
		}
	}
}
