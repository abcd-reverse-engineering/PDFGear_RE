using System;
using System.Windows;
using System.Windows.Controls;
using pdfeditor.Models.Menus;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000264 RID: 612
	public class ToolbarButtonContextMenuStyleSelector : StyleSelector
	{
		// Token: 0x06002366 RID: 9062 RVA: 0x000A6698 File Offset: 0x000A4898
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is ContextMenuSeparator)
			{
				return this.SeparatorStyle;
			}
			if (item is SpeedContextMenuItemModel || item is SpeechToolContextMenuItemModel)
			{
				return this.SpeedPresetsMenuItem;
			}
			if (item is StampContextMenuItemModel)
			{
				return this.StampItemStyle;
			}
			if (item is PresetsItemContextMenuItemModel)
			{
				return this.StampPresetsMenuItem;
			}
			if (item is StampCustomMenuItemModel)
			{
				return this.StampPickerItemStyle;
			}
			if (item is ConvertContextMenuItemModel)
			{
				return this.ConvertItemStyle;
			}
			if (item is BackgroundContextMenuItemModel)
			{
				return this.BackgroundStyle;
			}
			if (item is ColorMoreItemContextMenuItemModel)
			{
				return this.ColorMoreItemStyle;
			}
			ContextMenuItemModel contextMenuItemModel = item as ContextMenuItemModel;
			if (contextMenuItemModel != null && object.Equals(contextMenuItemModel.Name, "ColorPicker"))
			{
				return this.ColorPickerItemStyle;
			}
			if (item is ContextMenuTranslateModel)
			{
				return this.TranslateStyle;
			}
			ContextMenuItemModel contextMenuItemModel2 = item as ContextMenuItemModel;
			if (contextMenuItemModel2 != null && object.Equals(contextMenuItemModel2.Name, "SignaturePicker"))
			{
				return this.SignaturePickerItemStyle;
			}
			ContextMenuItemModel contextMenuItemModel3 = item as ContextMenuItemModel;
			if (contextMenuItemModel3 != null)
			{
				TypedContextMenuItemModel typedContextMenuItemModel = contextMenuItemModel3.Parent as TypedContextMenuItemModel;
				if (typedContextMenuItemModel != null)
				{
					switch (typedContextMenuItemModel.Type)
					{
					case ContextMenuItemType.StrokeColor:
					case ContextMenuItemType.FillColor:
					case ContextMenuItemType.FontColor:
						return this.ColorItemStyle;
					case ContextMenuItemType.StrokeThickness:
					case ContextMenuItemType.FontSize:
						return this.StrokeThicknessItemStyle;
					}
				}
			}
			return this.DefaultItemStyle ?? base.SelectStyle(item, container);
		}

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x06002367 RID: 9063 RVA: 0x000A67E0 File Offset: 0x000A49E0
		// (set) Token: 0x06002368 RID: 9064 RVA: 0x000A67E8 File Offset: 0x000A49E8
		public Style StrokeThicknessItemStyle { get; set; }

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06002369 RID: 9065 RVA: 0x000A67F1 File Offset: 0x000A49F1
		// (set) Token: 0x0600236A RID: 9066 RVA: 0x000A67F9 File Offset: 0x000A49F9
		public Style ColorItemStyle { get; set; }

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x0600236B RID: 9067 RVA: 0x000A6802 File Offset: 0x000A4A02
		// (set) Token: 0x0600236C RID: 9068 RVA: 0x000A680A File Offset: 0x000A4A0A
		public Style DefaultItemStyle { get; set; }

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x0600236D RID: 9069 RVA: 0x000A6813 File Offset: 0x000A4A13
		// (set) Token: 0x0600236E RID: 9070 RVA: 0x000A681B File Offset: 0x000A4A1B
		public Style StampItemStyle { get; set; }

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x0600236F RID: 9071 RVA: 0x000A6824 File Offset: 0x000A4A24
		// (set) Token: 0x06002370 RID: 9072 RVA: 0x000A682C File Offset: 0x000A4A2C
		public Style ConvertItemStyle { get; set; }

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06002371 RID: 9073 RVA: 0x000A6835 File Offset: 0x000A4A35
		// (set) Token: 0x06002372 RID: 9074 RVA: 0x000A683D File Offset: 0x000A4A3D
		public Style BackgroundStyle { get; set; }

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06002373 RID: 9075 RVA: 0x000A6846 File Offset: 0x000A4A46
		// (set) Token: 0x06002374 RID: 9076 RVA: 0x000A684E File Offset: 0x000A4A4E
		public Style StampPresetsMenuItem { get; set; }

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x06002375 RID: 9077 RVA: 0x000A6857 File Offset: 0x000A4A57
		// (set) Token: 0x06002376 RID: 9078 RVA: 0x000A685F File Offset: 0x000A4A5F
		public Style ColorMoreItemStyle { get; set; }

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06002377 RID: 9079 RVA: 0x000A6868 File Offset: 0x000A4A68
		// (set) Token: 0x06002378 RID: 9080 RVA: 0x000A6870 File Offset: 0x000A4A70
		public Style ColorPickerItemStyle { get; set; }

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06002379 RID: 9081 RVA: 0x000A6879 File Offset: 0x000A4A79
		// (set) Token: 0x0600237A RID: 9082 RVA: 0x000A6881 File Offset: 0x000A4A81
		public Style SignaturePickerItemStyle { get; set; }

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x0600237B RID: 9083 RVA: 0x000A688A File Offset: 0x000A4A8A
		// (set) Token: 0x0600237C RID: 9084 RVA: 0x000A6892 File Offset: 0x000A4A92
		public Style SeparatorStyle { get; set; }

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x0600237D RID: 9085 RVA: 0x000A689B File Offset: 0x000A4A9B
		// (set) Token: 0x0600237E RID: 9086 RVA: 0x000A68A3 File Offset: 0x000A4AA3
		public Style TranslateStyle { get; set; }

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x0600237F RID: 9087 RVA: 0x000A68AC File Offset: 0x000A4AAC
		// (set) Token: 0x06002380 RID: 9088 RVA: 0x000A68B4 File Offset: 0x000A4AB4
		public Style StampPickerItemStyle { get; set; }

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06002381 RID: 9089 RVA: 0x000A68BD File Offset: 0x000A4ABD
		// (set) Token: 0x06002382 RID: 9090 RVA: 0x000A68C5 File Offset: 0x000A4AC5
		public Style SpeedPresetsMenuItem { get; set; }
	}
}
