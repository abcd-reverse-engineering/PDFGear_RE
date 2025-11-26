using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Controls.ColorPickers;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x02000272 RID: 626
	public partial class CollapsedColorPicker : UserControl
	{
		// Token: 0x0600244E RID: 9294 RVA: 0x000A88A6 File Offset: 0x000A6AA6
		public CollapsedColorPicker()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x0600244F RID: 9295 RVA: 0x000A88B4 File Offset: 0x000A6AB4
		// (set) Token: 0x06002450 RID: 9296 RVA: 0x000A88C6 File Offset: 0x000A6AC6
		public ToolbarSettingItemColorCollapseModel Model
		{
			get
			{
				return (ToolbarSettingItemColorCollapseModel)base.GetValue(CollapsedColorPicker.ModelProperty);
			}
			set
			{
				base.SetValue(CollapsedColorPicker.ModelProperty, value);
			}
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x000A88D4 File Offset: 0x000A6AD4
		private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				CollapsedColorPicker collapsedColorPicker = d as CollapsedColorPicker;
				if (collapsedColorPicker != null)
				{
					if (e.OldValue != null)
					{
						WeakEventManager<ToolbarSettingItemColorCollapseModel, EventArgs>.RemoveHandler((ToolbarSettingItemColorCollapseModel)e.OldValue, "SelectedValueChanged", new EventHandler<EventArgs>(collapsedColorPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemColorCollapseModel, EventArgs>.RemoveHandler((ToolbarSettingItemColorCollapseModel)e.OldValue, "ColorsChanged", new EventHandler<EventArgs>(collapsedColorPicker.Model_ColorsChanged));
					}
					if (e.NewValue != null)
					{
						WeakEventManager<ToolbarSettingItemColorCollapseModel, EventArgs>.AddHandler((ToolbarSettingItemColorCollapseModel)e.NewValue, "SelectedValueChanged", new EventHandler<EventArgs>(collapsedColorPicker.Model_SelectedValueChanged));
						WeakEventManager<ToolbarSettingItemColorCollapseModel, EventArgs>.AddHandler((ToolbarSettingItemColorCollapseModel)e.NewValue, "ColorsChanged", new EventHandler<EventArgs>(collapsedColorPicker.Model_ColorsChanged));
					}
					collapsedColorPicker.UpdatePickerSelectedColor();
					collapsedColorPicker.UpdatePickerStandardColors();
				}
			}
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x000A89A7 File Offset: 0x000A6BA7
		private void Model_SelectedValueChanged(object sender, EventArgs e)
		{
			this.UpdatePickerSelectedColor();
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x000A89AF File Offset: 0x000A6BAF
		private void Model_ColorsChanged(object sender, EventArgs e)
		{
			this.UpdatePickerStandardColors();
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x000A89B8 File Offset: 0x000A6BB8
		private void UpdatePickerStandardColors()
		{
			if (this.Model == null)
			{
				this.colorPickerButton.StandardColors = null;
				return;
			}
			ColorPickerButton colorPickerButton = this.colorPickerButton;
			ObservableCollection<string> standardColors = this.Model.StandardColors;
			IReadOnlyCollection<Color> readOnlyCollection;
			if (standardColors == null)
			{
				readOnlyCollection = null;
			}
			else
			{
				readOnlyCollection = (from c in standardColors.Select(delegate(string c)
					{
						try
						{
							return (Color?)ColorConverter.ConvertFromString(c);
						}
						catch
						{
						}
						return null;
					})
					where c != null
					select c.Value).ToArray<Color>();
			}
			colorPickerButton.StandardColors = readOnlyCollection;
		}

		// Token: 0x06002455 RID: 9301 RVA: 0x000A8A68 File Offset: 0x000A6C68
		private void UpdatePickerSelectedColor()
		{
			if (this.Model == null)
			{
				this.colorPickerButton.SelectedColor = Color.FromArgb(byte.MaxValue, 0, 0, 0);
				return;
			}
			try
			{
				Color color = (Color)ColorConverter.ConvertFromString((string)this.Model.SelectedValue);
				this.colorPickerButton.SelectedColor = color;
			}
			catch
			{
			}
		}

		// Token: 0x06002456 RID: 9302 RVA: 0x000A8AD4 File Offset: 0x000A6CD4
		private void ColorPickerButton_ItemClick(object sender, ColorPickerButtonItemClickEventArgs e)
		{
			if (this.Model != null)
			{
				Color color = e.Item.Color;
				this.Model.SelectedValue = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { color.A, color.R, color.G, color.B });
				this.Model.ExecuteCommand();
			}
		}

		// Token: 0x04000F58 RID: 3928
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(ToolbarSettingItemColorCollapseModel), typeof(CollapsedColorPicker), new PropertyMetadata(null, new PropertyChangedCallback(CollapsedColorPicker.OnModelPropertyChanged)));
	}
}
