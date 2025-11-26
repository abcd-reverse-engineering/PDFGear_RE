using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Controls.ColorPickers;
using pdfeditor.Models.Menus;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000261 RID: 609
	public class ToolbarButtonContextMenuItem : MenuItem
	{
		// Token: 0x0600233D RID: 9021 RVA: 0x000A6014 File Offset: 0x000A4214
		static ToolbarButtonContextMenuItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarButtonContextMenuItem), new FrameworkPropertyMetadata(typeof(ToolbarButtonContextMenuItem)));
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x000A6074 File Offset: 0x000A4274
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ContextMenuItemModel contextMenuItemModel = base.DataContext as ContextMenuItemModel;
			if (contextMenuItemModel != null)
			{
				TagDataModel tagData = contextMenuItemModel.TagData;
				if (object.Equals((tagData != null) ? tagData.MenuItemValue : null, "ColorPicker"))
				{
					ColorPicker colorPicker = base.GetTemplateChild("ColorPicker") as ColorPicker;
					colorPicker.ItemClick += this.ColorPicker_ItemClick;
					ColorMoreItemContextMenuItemModel colorMoreItemContextMenuItemModel = contextMenuItemModel.Parent as ColorMoreItemContextMenuItemModel;
					if (colorMoreItemContextMenuItemModel != null)
					{
						colorPicker.RecentColorsKey = colorMoreItemContextMenuItemModel.RecentColorsKey;
						colorPicker.DefaultColor = colorMoreItemContextMenuItemModel.DefaultColor;
						colorPicker.Tag = base.DataContext;
					}
				}
			}
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x000A610C File Offset: 0x000A430C
		private void ColorPicker_ItemClick(object sender, ColorPickerItemClickEventArgs e)
		{
			ContextMenuItemModel contextMenuItemModel = ((FrameworkElement)sender).Tag as ContextMenuItemModel;
			if (contextMenuItemModel != null)
			{
				ColorMoreItemContextMenuItemModel colorMoreItemContextMenuItemModel = contextMenuItemModel.Parent as ColorMoreItemContextMenuItemModel;
				if (colorMoreItemContextMenuItemModel != null)
				{
					colorMoreItemContextMenuItemModel.TagData.MenuItemValue = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
					{
						e.Item.Color.A,
						e.Item.Color.R,
						e.Item.Color.G,
						e.Item.Color.B
					});
					colorMoreItemContextMenuItemModel.Command.Execute(colorMoreItemContextMenuItemModel);
					FrameworkElement frameworkElement = this;
					while (frameworkElement != null)
					{
						frameworkElement = (frameworkElement.Parent as FrameworkElement) ?? (VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement);
						ContextMenu contextMenu = frameworkElement as ContextMenu;
						if (contextMenu != null)
						{
							contextMenu.IsOpen = false;
							return;
						}
					}
				}
			}
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06002340 RID: 9024 RVA: 0x000A620C File Offset: 0x000A440C
		// (set) Token: 0x06002341 RID: 9025 RVA: 0x000A621E File Offset: 0x000A441E
		public bool IsToggleEnabled
		{
			get
			{
				return (bool)base.GetValue(ToolbarButtonContextMenuItem.IsToggleEnabledProperty);
			}
			set
			{
				base.SetValue(ToolbarButtonContextMenuItem.IsToggleEnabledProperty, value);
			}
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000A6234 File Offset: 0x000A4434
		protected override void OnClick()
		{
			bool isChecked = base.IsChecked;
			base.OnClick();
			if (!this.IsToggleEnabled && base.IsCheckable && isChecked && !base.IsChecked)
			{
				base.IsChecked = true;
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000A6272 File Offset: 0x000A4472
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ToolbarButtonContextMenuItem();
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x000A6279 File Offset: 0x000A4479
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is ToolbarButtonContextMenuItem;
		}

		// Token: 0x04000F02 RID: 3842
		public static readonly DependencyProperty IsToggleEnabledProperty = DependencyProperty.Register("IsToggleEnabled", typeof(bool), typeof(ToolbarButtonContextMenuItem), new PropertyMetadata(false));
	}
}
