using System;
using System.Windows;
using System.Windows.Controls;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x0200027C RID: 636
	internal class ToolbarSettingContentTemplateSelector : DataTemplateSelector
	{
		// Token: 0x060024B1 RID: 9393 RVA: 0x000AA370 File Offset: 0x000A8570
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is ToolbarSettingItemStrokeThicknessModel)
			{
				return this.StrokeThicknessPicker;
			}
			if (item is ToolbarSettingItemFontSizeModel)
			{
				return this.FontSizePicker;
			}
			if (item is ToolbarSettingItemIconModel)
			{
				return this.IconPicker;
			}
			if (item is ToolbarSettingItemTextEditingButtonsModel)
			{
				return this.TextEditingButtons;
			}
			if (item is ToolbarSettingItemExitModel)
			{
				return this.ExitEditingButton;
			}
			if (item is ToolbarSettingItemImageExitModel)
			{
				return this.ImageExitEditingButton;
			}
			if (item is ToolBarSettingTextBlock)
			{
				return this.TextBlock;
			}
			if (item is ToolbarSettingItemApplyToDefaultModel)
			{
				return this.ApplyToDefaultButton;
			}
			if (item is ToolbarSettingItemBoldModel)
			{
				return this.FontStyleButton;
			}
			if (item is ToolbarSettingItemItalicModel)
			{
				return this.FontStyleButton;
			}
			if (item is ToolbarSettingItemFontNameModel)
			{
				return this.FontNamePicker;
			}
			if (item is ToolbarSettingInkEraserModel)
			{
				return this.EraserPicker;
			}
			ToolbarSettingItemModel toolbarSettingItemModel = item as ToolbarSettingItemModel;
			if (toolbarSettingItemModel == null || !this.IsColorModel(toolbarSettingItemModel))
			{
				return base.SelectTemplate(item, container);
			}
			if (item is ToolbarSettingItemColorModel)
			{
				return this.TextMarkupColorPicker;
			}
			return this.ColorPicker;
		}

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x060024B2 RID: 9394 RVA: 0x000AA462 File Offset: 0x000A8662
		// (set) Token: 0x060024B3 RID: 9395 RVA: 0x000AA46A File Offset: 0x000A866A
		public DataTemplate TextMarkupColorPicker { get; set; }

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x060024B4 RID: 9396 RVA: 0x000AA473 File Offset: 0x000A8673
		// (set) Token: 0x060024B5 RID: 9397 RVA: 0x000AA47B File Offset: 0x000A867B
		public DataTemplate ColorPicker { get; set; }

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x060024B6 RID: 9398 RVA: 0x000AA484 File Offset: 0x000A8684
		// (set) Token: 0x060024B7 RID: 9399 RVA: 0x000AA48C File Offset: 0x000A868C
		public DataTemplate StrokeThicknessPicker { get; set; }

		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x060024B8 RID: 9400 RVA: 0x000AA495 File Offset: 0x000A8695
		// (set) Token: 0x060024B9 RID: 9401 RVA: 0x000AA49D File Offset: 0x000A869D
		public DataTemplate FontSizePicker { get; set; }

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x060024BA RID: 9402 RVA: 0x000AA4A6 File Offset: 0x000A86A6
		// (set) Token: 0x060024BB RID: 9403 RVA: 0x000AA4AE File Offset: 0x000A86AE
		public DataTemplate IconPicker { get; set; }

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x060024BC RID: 9404 RVA: 0x000AA4B7 File Offset: 0x000A86B7
		// (set) Token: 0x060024BD RID: 9405 RVA: 0x000AA4BF File Offset: 0x000A86BF
		public DataTemplate ExitEditingButton { get; set; }

		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x060024BE RID: 9406 RVA: 0x000AA4C8 File Offset: 0x000A86C8
		// (set) Token: 0x060024BF RID: 9407 RVA: 0x000AA4D0 File Offset: 0x000A86D0
		public DataTemplate ImageExitEditingButton { get; set; }

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x060024C0 RID: 9408 RVA: 0x000AA4D9 File Offset: 0x000A86D9
		// (set) Token: 0x060024C1 RID: 9409 RVA: 0x000AA4E1 File Offset: 0x000A86E1
		public DataTemplate TextBlock { get; set; }

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x060024C2 RID: 9410 RVA: 0x000AA4EA File Offset: 0x000A86EA
		// (set) Token: 0x060024C3 RID: 9411 RVA: 0x000AA4F2 File Offset: 0x000A86F2
		public DataTemplate EraserPicker { get; set; }

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x060024C4 RID: 9412 RVA: 0x000AA4FB File Offset: 0x000A86FB
		// (set) Token: 0x060024C5 RID: 9413 RVA: 0x000AA503 File Offset: 0x000A8703
		public DataTemplate ApplyToDefaultButton { get; set; }

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x060024C6 RID: 9414 RVA: 0x000AA50C File Offset: 0x000A870C
		// (set) Token: 0x060024C7 RID: 9415 RVA: 0x000AA514 File Offset: 0x000A8714
		public DataTemplate FontStyleButton { get; set; }

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x060024C8 RID: 9416 RVA: 0x000AA51D File Offset: 0x000A871D
		// (set) Token: 0x060024C9 RID: 9417 RVA: 0x000AA525 File Offset: 0x000A8725
		public DataTemplate FontNamePicker { get; set; }

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x060024CA RID: 9418 RVA: 0x000AA52E File Offset: 0x000A872E
		// (set) Token: 0x060024CB RID: 9419 RVA: 0x000AA536 File Offset: 0x000A8736
		public DataTemplate TextEditingButtons { get; set; }

		// Token: 0x060024CC RID: 9420 RVA: 0x000AA53F File Offset: 0x000A873F
		private bool IsColorModel(ToolbarSettingItemModel model)
		{
			return model != null && (model.Type == ContextMenuItemType.StrokeColor || model.Type == ContextMenuItemType.FontColor || model.Type == ContextMenuItemType.FillColor);
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x000AA564 File Offset: 0x000A8764
		private bool IsTextMarkupModel(ToolbarSettingItemModel model)
		{
			return model != null && (model.Id.AnnotationMode == AnnotationMode.Highlight || model.Id.AnnotationMode == AnnotationMode.Underline || model.Id.AnnotationMode == AnnotationMode.Strike || model.Id.AnnotationMode == AnnotationMode.HighlightArea);
		}
	}
}
