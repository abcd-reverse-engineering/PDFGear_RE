using System;
using System.Windows;
using System.Windows.Media;
using pdfeditor.Controls.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000164 RID: 356
	public class ToolbarAnnotationButtonModel : ToolbarButtonModel
	{
		// Token: 0x06001569 RID: 5481 RVA: 0x000535EC File Offset: 0x000517EC
		public ToolbarAnnotationButtonModel(AnnotationMode mode)
		{
			this.Mode = mode;
			base.Name = mode.ToString();
		}

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x0005360E File Offset: 0x0005180E
		public AnnotationMode Mode { get; }

		// Token: 0x0600156B RID: 5483 RVA: 0x00053618 File Offset: 0x00051818
		protected override void OnChildButtonModelChanged(ToolbarChildButtonModel newValue, ToolbarChildButtonModel oldValue)
		{
			base.OnChildButtonModelChanged(newValue, oldValue);
			ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = oldValue as ToolbarChildCheckableButtonModel;
			if (toolbarChildCheckableButtonModel != null)
			{
				WeakEventManager<ToolbarChildCheckableButtonModel, SelectedAccessorSelectionChangedEventArgs>.RemoveHandler(toolbarChildCheckableButtonModel, "ContextMenuSelectionChanged", new EventHandler<SelectedAccessorSelectionChangedEventArgs>(this.ChildButtonModel_ContextMenuSelectionChanged));
			}
			ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel2 = newValue as ToolbarChildCheckableButtonModel;
			if (toolbarChildCheckableButtonModel2 != null)
			{
				WeakEventManager<ToolbarChildCheckableButtonModel, SelectedAccessorSelectionChangedEventArgs>.AddHandler(toolbarChildCheckableButtonModel2, "ContextMenuSelectionChanged", new EventHandler<SelectedAccessorSelectionChangedEventArgs>(this.ChildButtonModel_ContextMenuSelectionChanged));
			}
			this.UpdateIndicatorBrush();
		}

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x00053675 File Offset: 0x00051875
		// (set) Token: 0x0600156D RID: 5485 RVA: 0x0005367D File Offset: 0x0005187D
		public ToolbarSettingModel ToolbarSettingModel
		{
			get
			{
				return this.toolbarSettingModel;
			}
			set
			{
				base.SetProperty<ToolbarSettingModel>(ref this.toolbarSettingModel, value, "ToolbarSettingModel");
			}
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00053692 File Offset: 0x00051892
		private void ChildButtonModel_ContextMenuSelectionChanged(object sender, SelectedAccessorSelectionChangedEventArgs e)
		{
			this.UpdateIndicatorBrush();
			EventHandler<SelectedAccessorSelectionChangedEventArgs> contextMenuSelectionChanged = this.ContextMenuSelectionChanged;
			if (contextMenuSelectionChanged == null)
			{
				return;
			}
			contextMenuSelectionChanged(this, e);
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x000536AC File Offset: 0x000518AC
		private void UpdateIndicatorBrush()
		{
			ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = base.ChildButtonModel as ToolbarChildCheckableButtonModel;
			if (toolbarChildCheckableButtonModel != null)
			{
				TypedContextMenuModel typedContextMenuModel = toolbarChildCheckableButtonModel.ContextMenu as TypedContextMenuModel;
				if (typedContextMenuModel != null)
				{
					string text = "";
					switch (this.Mode)
					{
					case AnnotationMode.Line:
					case AnnotationMode.Arrow:
					case AnnotationMode.Ink:
					case AnnotationMode.Highlight:
					case AnnotationMode.Underline:
					case AnnotationMode.Strike:
					case AnnotationMode.HighlightArea:
					{
						ContextMenuItemModel strokeColor = typedContextMenuModel.SelectedItems.StrokeColor;
						object obj;
						if (strokeColor == null)
						{
							obj = null;
						}
						else
						{
							TagDataModel tagData = strokeColor.TagData;
							obj = ((tagData != null) ? tagData.MenuItemValue : null);
						}
						string text2 = obj as string;
						if (text2 != null)
						{
							text = text2;
						}
						break;
					}
					case AnnotationMode.Shape:
					case AnnotationMode.Ellipse:
					case AnnotationMode.TextBox:
					case AnnotationMode.Link:
					{
						ContextMenuItemModel strokeColor2 = typedContextMenuModel.SelectedItems.StrokeColor;
						object obj2;
						if (strokeColor2 == null)
						{
							obj2 = null;
						}
						else
						{
							TagDataModel tagData2 = strokeColor2.TagData;
							obj2 = ((tagData2 != null) ? tagData2.MenuItemValue : null);
						}
						string text3 = obj2 as string;
						if (text3 != null)
						{
							text = text3;
						}
						break;
					}
					case AnnotationMode.Text:
					{
						ContextMenuItemModel fontColor = typedContextMenuModel.SelectedItems.FontColor;
						object obj3;
						if (fontColor == null)
						{
							obj3 = null;
						}
						else
						{
							TagDataModel tagData3 = fontColor.TagData;
							obj3 = ((tagData3 != null) ? tagData3.MenuItemValue : null);
						}
						string text4 = obj3 as string;
						if (text4 != null)
						{
							text = text4;
						}
						break;
					}
					}
					if (string.IsNullOrEmpty(text))
					{
						base.IndicatorBrush = null;
						return;
					}
					try
					{
						Color color = (Color)ColorConverter.ConvertFromString(text);
						base.IndicatorBrush = new SolidColorBrush(color);
						return;
					}
					catch
					{
						base.IndicatorBrush = null;
						return;
					}
				}
			}
			base.IndicatorBrush = null;
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001570 RID: 5488 RVA: 0x00053818 File Offset: 0x00051A18
		// (remove) Token: 0x06001571 RID: 5489 RVA: 0x00053850 File Offset: 0x00051A50
		public event EventHandler<SelectedAccessorSelectionChangedEventArgs> ContextMenuSelectionChanged;

		// Token: 0x0400071B RID: 1819
		private ToolbarChildButtonModel childButtonModel;

		// Token: 0x0400071C RID: 1820
		private ToolbarSettingModel toolbarSettingModel;
	}
}
