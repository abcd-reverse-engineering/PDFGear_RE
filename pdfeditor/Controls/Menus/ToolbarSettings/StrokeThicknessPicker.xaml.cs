using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000277 RID: 631
	public partial class StrokeThicknessPicker : UserControl
	{
		// Token: 0x06002487 RID: 9351 RVA: 0x000A9824 File Offset: 0x000A7A24
		public StrokeThicknessPicker()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06002488 RID: 9352 RVA: 0x000A9832 File Offset: 0x000A7A32
		// (set) Token: 0x06002489 RID: 9353 RVA: 0x000A9844 File Offset: 0x000A7A44
		public ToolbarSettingItemStrokeThicknessModel Model
		{
			get
			{
				return (ToolbarSettingItemStrokeThicknessModel)base.GetValue(StrokeThicknessPicker.ModelProperty);
			}
			set
			{
				base.SetValue(StrokeThicknessPicker.ModelProperty, value);
			}
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x000A9854 File Offset: 0x000A7A54
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				StrokeThicknessPicker strokeThicknessPicker = d as StrokeThicknessPicker;
				if (strokeThicknessPicker != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.RemoveHandler((ToolbarSettingItemStrokeThicknessModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.RemoveHandler((ToolbarSettingItemStrokeThicknessModel)e.OldValue, "StandardItemsChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_StandardItemsChanged));
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.RemoveHandler((ToolbarSettingItemStrokeThicknessModel)e.OldValue, "TransientItemChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_TransientItemChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.AddHandler((ToolbarSettingItemStrokeThicknessModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.AddHandler((ToolbarSettingItemStrokeThicknessModel)e.NewValue, "StandardItemsChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_StandardItemsChanged));
						WeakEventManager<ToolbarSettingItemStrokeThicknessModel, EventArgs>.AddHandler((ToolbarSettingItemStrokeThicknessModel)e.NewValue, "TransientItemChanged", new EventHandler<EventArgs>(strokeThicknessPicker.Model_TransientItemChanged));
					}
					strokeThicknessPicker.UpdatePickerComboBoxItems();
					strokeThicknessPicker.UpdateSelectedValue();
				}
			}
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x000A996B File Offset: 0x000A7B6B
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			this.UpdateSelectedValue();
			if (this.Model.Id.AnnotationMode == AnnotationMode.Ink)
			{
				(this.Model.Parent[3] as ToolbarSettingInkEraserModel).IsChecked = false;
			}
		}

		// Token: 0x0600248C RID: 9356 RVA: 0x000A99A2 File Offset: 0x000A7BA2
		private void Model_StandardItemsChanged(object sender, EventArgs e)
		{
			this.UpdatePickerComboBoxItems();
		}

		// Token: 0x0600248D RID: 9357 RVA: 0x000A99AA File Offset: 0x000A7BAA
		private void Model_TransientItemChanged(object sender, EventArgs e)
		{
			this.UpdatePickerComboBoxItems();
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x000A99B4 File Offset: 0x000A7BB4
		private void UpdateSelectedValue()
		{
			this.comboBox.SelectionChanged -= this.ComboBox_SelectionChanged;
			if (this.Model == null)
			{
				this.comboBox.SelectedIndex = -1;
			}
			else
			{
				List<StrokeThicknessPicker.NestedModel> list = this.comboBox.ItemsSource as List<StrokeThicknessPicker.NestedModel>;
				if (list != null)
				{
					object selectedValue = this.Model.SelectedValue;
					if (selectedValue is float)
					{
						float num = (float)selectedValue;
						foreach (StrokeThicknessPicker.NestedModel nestedModel in list)
						{
							if (nestedModel.Value == num)
							{
								this.comboBox.SelectedItem = nestedModel;
								break;
							}
						}
					}
				}
			}
			this.comboBox.SelectionChanged += this.ComboBox_SelectionChanged;
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x000A9A88 File Offset: 0x000A7C88
		private void UpdatePickerComboBoxItems()
		{
			this.comboBox.SelectionChanged -= this.ComboBox_SelectionChanged;
			if (this.Model == null)
			{
				this.comboBox.ItemsSource = null;
			}
			else
			{
				ObservableCollection<float> standardItems = this.Model.StandardItems;
				List<StrokeThicknessPicker.NestedModel> list = ((standardItems != null) ? standardItems.Select((float c) => new StrokeThicknessPicker.NestedModel(c, this.Model)).ToList<StrokeThicknessPicker.NestedModel>() : null);
				if (list == null)
				{
					list = new List<StrokeThicknessPicker.NestedModel>();
				}
				if (this.Model.TransientItem != null && list.Find((StrokeThicknessPicker.NestedModel c) => c.Model.TransientItem.Value == this.Model.TransientItem.Value) == null)
				{
					list.Add(new StrokeThicknessPicker.NestedModel(this.Model.TransientItem.Value, this.Model));
				}
				this.comboBox.ItemsSource = list;
			}
			this.comboBox.SelectionChanged += this.ComboBox_SelectionChanged;
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x000A9B68 File Offset: 0x000A7D68
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.Model == null)
			{
				return;
			}
			StrokeThicknessPicker.NestedModel nestedModel = e.AddedItems.OfType<StrokeThicknessPicker.NestedModel>().FirstOrDefault<StrokeThicknessPicker.NestedModel>();
			if (nestedModel != null)
			{
				this.Model.SelectedValue = nestedModel.Value;
				this.Model.ExecuteCommand();
			}
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x000A9BB3 File Offset: 0x000A7DB3
		private void ThicknessIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.comboBox.IsDropDownOpen = true;
		}

		// Token: 0x04000F6E RID: 3950
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemStrokeThicknessModel), typeof(StrokeThicknessPicker), new PropertyMetadata(null, new PropertyChangedCallback(StrokeThicknessPicker.OnModelPropertyChanged)));

		// Token: 0x02000727 RID: 1831
		private class NestedModel : ObservableObject
		{
			// Token: 0x060035EF RID: 13807 RVA: 0x0010FD74 File Offset: 0x0010DF74
			public NestedModel(float value, ToolbarSettingItemStrokeThicknessModel model)
			{
				this.Value = value;
				this.Model = model;
			}

			// Token: 0x17000D59 RID: 3417
			// (get) Token: 0x060035F0 RID: 13808 RVA: 0x0010FD8A File Offset: 0x0010DF8A
			public float Value { get; }

			// Token: 0x17000D5A RID: 3418
			// (get) Token: 0x060035F1 RID: 13809 RVA: 0x0010FD92 File Offset: 0x0010DF92
			public string Caption
			{
				get
				{
					return string.Format("{0} pt", this.Value);
				}
			}

			// Token: 0x17000D5B RID: 3419
			// (get) Token: 0x060035F2 RID: 13810 RVA: 0x0010FDA9 File Offset: 0x0010DFA9
			public ToolbarSettingItemStrokeThicknessModel Model { get; }
		}
	}
}
