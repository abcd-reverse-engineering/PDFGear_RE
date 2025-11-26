using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Properties;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x020000AB RID: 171
	public static class ToolbarSettingsHelper
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x000379C8 File Offset: 0x00035BC8
		internal static ToolbarSettingsHelper.NestedDefaultValues DefaultValues
		{
			get
			{
				if (ToolbarSettingsHelper._defaultValues == null)
				{
					object obj = ToolbarSettingsHelper.locker;
					lock (obj)
					{
						if (ToolbarSettingsHelper._defaultValues == null)
						{
							ToolbarSettingsHelper._defaultValues = new ToolbarSettingsHelper.NestedDefaultValues();
						}
					}
				}
				return ToolbarSettingsHelper._defaultValues;
			}
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x00037A20 File Offset: 0x00035C20
		public static ToolbarSettingItemModel CreateColor(AnnotationMode mode, ContextMenuItemType type, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			string text = ToolbarSettingConfigHelper.BuildRecentColorKey(mode, type);
			return ToolbarSettingsHelper.CreateColor(mode, type, text, action, icon);
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00037A40 File Offset: 0x00035C40
		public static ToolbarSettingItemModel CreateColor(AnnotationMode mode, ContextMenuItemType type, string recentKey, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			IReadOnlyList<string> readOnlyList = null;
			string text = null;
			string text2 = "";
			string text3 = "";
			if (type == ContextMenuItemType.StrokeColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardStokeColors(mode, out text);
				text2 = "Color";
				text3 = Resources.LabelColorContent;
			}
			else if (type == ContextMenuItemType.FillColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardFillColors(mode, out text);
				text2 = "FillColor";
				text3 = Resources.MenuSubFillColorItem;
			}
			else if (type == ContextMenuItemType.FontColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardFontColors(mode, out text);
				text2 = "FontColor";
				text3 = Resources.MenuSubFontColorItem;
			}
			if (readOnlyList == null)
			{
				return null;
			}
			return new ToolbarSettingItemColorModel(type)
			{
				Name = text2,
				Caption = text3,
				Icon = icon,
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null),
				RecentColorsKey = recentKey,
				SelectedValue = text,
				StandardColors = new ObservableCollection<string>(readOnlyList)
			};
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00037B08 File Offset: 0x00035D08
		private static ImageSource CreateIcon(AnnotationMode model)
		{
			ImageSource imageSource = null;
			switch (model)
			{
			case AnnotationMode.Line:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/line.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/line.png"));
				break;
			case AnnotationMode.Ink:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/ink.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/ink.png"));
				break;
			case AnnotationMode.Shape:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/shape.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/shape.png"));
				break;
			case AnnotationMode.Highlight:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/highlighttext.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/highlighttext.png"));
				break;
			case AnnotationMode.Underline:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/underline.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/underline.png"));
				break;
			case AnnotationMode.Strike:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/strike.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/strike.png"));
				break;
			case AnnotationMode.HighlightArea:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/highlightarea2.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/highlightarea2.png"));
				break;
			case AnnotationMode.Ellipse:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/ellipse.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/ellipse.png"));
				break;
			case AnnotationMode.Text:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/text.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/text.png"));
				break;
			case AnnotationMode.TextBox:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/textbox.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/textbox.png"));
				break;
			}
			return imageSource;
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00037C80 File Offset: 0x00035E80
		public static ImageSource CreateIcon(ContextMenuItemType itemtype)
		{
			ImageSource imageSource = null;
			switch (itemtype)
			{
			case ContextMenuItemType.StrokeColor:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/strokecolor.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/strokecolor.png"));
				break;
			case ContextMenuItemType.FillColor:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/fillcolor.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/fillcolor.png"));
				break;
			case ContextMenuItemType.StrokeThickness:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/strokethickness.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/strokethickness.png"));
				break;
			case ContextMenuItemType.FontColor:
				imageSource = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/fontcolor.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/fontcolor.png"));
				break;
			}
			return imageSource;
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00037D20 File Offset: 0x00035F20
		public static ToolbarSettingItemModel CreateCollapsedColor(AnnotationMode mode, ContextMenuItemType type, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			string text = ToolbarSettingConfigHelper.BuildRecentColorKey(mode, type);
			return ToolbarSettingsHelper.CreateCollapsedColor(mode, type, text, action, icon);
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x00037D3F File Offset: 0x00035F3F
		public static ToolbarSettingItemModel CreateCollapsedColor(AnnotationMode mode, ContextMenuItemType type, string recentKey, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			return ToolbarSettingsHelper.CreateCollapsedColor(ToolbarSettingId.CreateAnnotation(mode), type, recentKey, action, icon);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00037D54 File Offset: 0x00035F54
		public static ToolbarSettingItemModel CreateCollapsedColor(ToolbarSettingId id, ContextMenuItemType type, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			string text = ToolbarSettingConfigHelper.BuildRecentColorKey(id, type);
			return ToolbarSettingsHelper.CreateCollapsedColor(id, type, text, action, icon);
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00037D74 File Offset: 0x00035F74
		public static ToolbarSettingItemModel CreateCollapsedColor(ToolbarSettingId id, ContextMenuItemType type, string recentKey, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			IReadOnlyList<string> readOnlyList = null;
			string text = null;
			string text2 = "";
			string text3 = "";
			if (type == ContextMenuItemType.StrokeColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardStokeColors(id.AnnotationMode, out text);
				text2 = "Color";
				text3 = Resources.LabelColorContent;
				icon = icon ?? new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png"));
			}
			else if (type == ContextMenuItemType.FillColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardFillColors(id.AnnotationMode, out text);
				text2 = "FillColor";
				text3 = Resources.MenuSubFillColorItem;
				icon = icon ?? new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png"));
			}
			else if (type == ContextMenuItemType.FontColor)
			{
				readOnlyList = ToolbarSettingsHelper.DefaultValues.GetStandardFontColors(id.AnnotationMode, out text);
				text2 = "FontColor";
				text3 = Resources.MenuSubFontColorItem;
				icon = icon ?? new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png"));
			}
			if (readOnlyList == null)
			{
				return null;
			}
			return new ToolbarSettingItemColorCollapseModel(type)
			{
				Name = text2,
				Caption = text3,
				Icon = icon,
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null),
				RecentColorsKey = recentKey,
				SelectedValue = text,
				StandardColors = new ObservableCollection<string>(readOnlyList)
			};
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00037E90 File Offset: 0x00036090
		public static ToolbarSettingItemModel CreateStrokeThickness(AnnotationMode mode, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			float num;
			IReadOnlyList<float> standardStrokeThicknesses = ToolbarSettingsHelper.DefaultValues.GetStandardStrokeThicknesses(mode, out num);
			if (standardStrokeThicknesses == null)
			{
				return null;
			}
			return new ToolbarSettingItemStrokeThicknessModel(ContextMenuItemType.StrokeThickness)
			{
				Name = "Thickness",
				Caption = Resources.MenuSubThicknessItem,
				Icon = (icon ?? new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linewidth.png"))),
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null),
				SelectedValue = num,
				StandardItems = new ObservableCollection<float>(standardStrokeThicknesses)
			};
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00037F10 File Offset: 0x00036110
		public static ToolbarSettingItemModel CreateFontSize(AnnotationMode mode, Action<ToolbarSettingItemModel> action, ImageSource icon)
		{
			float num;
			IReadOnlyList<float> standardFontSizes = ToolbarSettingsHelper.DefaultValues.GetStandardFontSizes(mode, out num);
			if (standardFontSizes == null)
			{
				return null;
			}
			return new ToolbarSettingItemFontSizeModel(ContextMenuItemType.FontSize)
			{
				Name = "FontSize",
				Caption = Resources.MenuSubFontSizeItem,
				Icon = (icon ?? new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/linecolor.png"))),
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null),
				SelectedValue = num,
				StandardItems = new ObservableCollection<float>(standardFontSizes)
			};
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00037F90 File Offset: 0x00036190
		public static ToolbarSettingItemModel CreateAnnotationModeIcon(AnnotationMode mode)
		{
			return new ToolbarSettingItemIconModel
			{
				Name = "Icon",
				Icon = ToolbarSettingsHelper.CreateIcon(mode)
			};
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00037FAE File Offset: 0x000361AE
		public static ToolbarSettingItemIconModel CreateMenuItemTypeModelIcon(ContextMenuItemType itemtype)
		{
			return new ToolbarSettingItemIconModel
			{
				Name = "Icon",
				Caption = itemtype.ToString(),
				Icon = ToolbarSettingsHelper.CreateIcon(itemtype)
			};
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00037FDF File Offset: 0x000361DF
		public static ToolbarSettingItemModel CreateExitEdit(Action<ToolbarSettingItemModel> action)
		{
			return new ToolbarSettingItemExitModel
			{
				Name = "Exit",
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null)
			};
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x00038003 File Offset: 0x00036203
		public static ToolbarSettingItemModel CreateImageExitEdit(Action<ToolbarSettingItemModel> action)
		{
			return ToolbarSettingsHelper.CreateImageExitEdit(null, action);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0003800C File Offset: 0x0003620C
		public static ToolbarSettingItemModel CreateImageExitEdit(string text, Action<ToolbarSettingItemModel> action)
		{
			return new ToolbarSettingItemImageExitModel(text)
			{
				Text = text,
				Name = "Exit",
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null)
			};
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x00038038 File Offset: 0x00036238
		public static ToolbarSettingItemModel CreateText(string text)
		{
			return new ToolBarSettingTextBlock(text)
			{
				Text = text,
				Name = "Text"
			};
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x00038052 File Offset: 0x00036252
		public static ToolbarSettingItemApplyToDefaultModel CreateApplyToDefault()
		{
			return new ToolbarSettingItemApplyToDefaultModel
			{
				Name = "ApplyToDefault"
			};
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x00038064 File Offset: 0x00036264
		public static ToolbarSettingItemModel CreteEreserState(AnnotationMode mode, string Name, bool Ischeck, Action<ToolbarSettingItemModel> action)
		{
			int eraserSize = ConfigManager.GetEraserSize();
			return new ToolbarSettingInkEraserModel
			{
				Name = Name,
				IsCheckable = true,
				IsChecked = Ischeck,
				Command = ((action != null) ? new RelayCommand<ToolbarSettingItemModel>(action) : null),
				Caption = Resources.InkToolbarEraserBtn,
				SelectSize = eraserSize
			};
		}

		// Token: 0x040004B3 RID: 1203
		private static object locker = new object();

		// Token: 0x040004B4 RID: 1204
		private static ToolbarSettingsHelper.NestedDefaultValues _defaultValues;

		// Token: 0x020004CA RID: 1226
		internal class NestedDefaultValues
		{
			// Token: 0x06002EB8 RID: 11960 RVA: 0x000E525E File Offset: 0x000E345E
			public NestedDefaultValues()
			{
				this.allValues = new Dictionary<ContextMenuItemType, IDictionary>();
				this.InitStandardStrokeColors();
				this.InitStandardFillColors();
				this.InitStandardStrokeThicknesses();
				this.InitStandardFontSizes();
				this.InitFontColors();
			}

			// Token: 0x06002EB9 RID: 11961 RVA: 0x000E528F File Offset: 0x000E348F
			public IReadOnlyList<string> GetStandardStokeColors(AnnotationMode mode, out string defaultValue)
			{
				return this.GetValues<string>(mode, ContextMenuItemType.StrokeColor, out defaultValue);
			}

			// Token: 0x06002EBA RID: 11962 RVA: 0x000E529A File Offset: 0x000E349A
			public IReadOnlyList<string> GetStandardFillColors(AnnotationMode mode, out string defaultValue)
			{
				return this.GetValues<string>(mode, ContextMenuItemType.FillColor, out defaultValue);
			}

			// Token: 0x06002EBB RID: 11963 RVA: 0x000E52A5 File Offset: 0x000E34A5
			public IReadOnlyList<float> GetStandardStrokeThicknesses(AnnotationMode mode, out float defaultValue)
			{
				return this.GetValues<float>(mode, ContextMenuItemType.StrokeThickness, out defaultValue);
			}

			// Token: 0x06002EBC RID: 11964 RVA: 0x000E52B0 File Offset: 0x000E34B0
			public IReadOnlyList<float> GetStandardFontSizes(AnnotationMode mode, out float defaultValue)
			{
				return this.GetValues<float>(mode, ContextMenuItemType.FontSize, out defaultValue);
			}

			// Token: 0x06002EBD RID: 11965 RVA: 0x000E52BB File Offset: 0x000E34BB
			public IReadOnlyList<string> GetStandardFontColors(AnnotationMode mode, out string defaultValue)
			{
				return this.GetValues<string>(mode, ContextMenuItemType.FontColor, out defaultValue);
			}

			// Token: 0x06002EBE RID: 11966 RVA: 0x000E52C8 File Offset: 0x000E34C8
			private IReadOnlyList<T> GetValues<T>(AnnotationMode mode, ContextMenuItemType type, out T defaultValue)
			{
				defaultValue = default(T);
				IDictionary dictionary;
				if (this.allValues.TryGetValue(type, out dictionary))
				{
					Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<T>>> dictionary2 = dictionary as Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<T>>>;
					if (dictionary2 != null)
					{
						IReadOnlyList<ToolbarSettingsHelper.ValueProxy<T>> readOnlyList;
						if (!dictionary2.TryGetValue(mode, out readOnlyList))
						{
							readOnlyList = dictionary2[AnnotationMode.None];
						}
						bool flag = false;
						List<T> list = new List<T>(readOnlyList.Count);
						for (int i = 0; i < readOnlyList.Count; i++)
						{
							list.Add(readOnlyList[i].Value);
							if (!flag && readOnlyList[i].IsDefault)
							{
								defaultValue = readOnlyList[i].Value;
								flag = true;
							}
						}
						if (!flag && list.Count > 0)
						{
							defaultValue = list[0];
						}
						return list;
					}
				}
				return null;
			}

			// Token: 0x06002EBF RID: 11967 RVA: 0x000E5390 File Offset: 0x000E3590
			private void InitStandardStrokeColors()
			{
				Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>> dictionary = new Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>>();
				dictionary[AnnotationMode.None] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				dictionary[AnnotationMode.Highlight] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA800"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFF400", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#7AF256"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#21A0FF")
				};
				dictionary[AnnotationMode.HighlightArea] = dictionary[AnnotationMode.Highlight];
				dictionary[AnnotationMode.Underline] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FB302F", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA800"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFF400"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#7AF256"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#21A0FF")
				};
				dictionary[AnnotationMode.Strike] = dictionary[AnnotationMode.Underline];
				dictionary[AnnotationMode.Ink] = dictionary[AnnotationMode.Underline];
				this.allValues[ContextMenuItemType.StrokeColor] = dictionary;
			}

			// Token: 0x06002EC0 RID: 11968 RVA: 0x000E54FC File Offset: 0x000E36FC
			private void InitStandardFillColors()
			{
				Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>> dictionary = new Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>>();
				dictionary[AnnotationMode.None] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				dictionary[AnnotationMode.Shape] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#00FFFFFF", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				dictionary[AnnotationMode.Link] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#00FFFFFF", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				dictionary[AnnotationMode.Ellipse] = dictionary[AnnotationMode.Shape];
				dictionary[AnnotationMode.TextBox] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				dictionary[AnnotationMode.Redact] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#00FFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				this.allValues[ContextMenuItemType.FillColor] = dictionary;
			}

			// Token: 0x06002EC1 RID: 11969 RVA: 0x000E57EC File Offset: 0x000E39EC
			private void InitStandardStrokeThicknesses()
			{
				Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<float>>> dictionary = new Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<float>>>();
				dictionary[AnnotationMode.None] = new float[]
				{
					0.25f, 0.5f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f,
					9f, 10f, 11f, 12f
				}.Select((float c) => ToolbarSettingsHelper.NestedDefaultValues.Value<float>(c, c == 1f)).ToArray<ToolbarSettingsHelper.ValueProxy<float>>();
				this.allValues[ContextMenuItemType.StrokeThickness] = dictionary;
			}

			// Token: 0x06002EC2 RID: 11970 RVA: 0x000E5850 File Offset: 0x000E3A50
			private void InitStandardFontSizes()
			{
				Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<float>>> dictionary = new Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<float>>>();
				dictionary[AnnotationMode.None] = new float[]
				{
					2f, 4f, 6f, 8f, 10f, 12f, 14f, 16f, 18f, 20f,
					22f, 24f, 26f, 28f, 36f, 48f, 56f, 72f
				}.Select((float c) => ToolbarSettingsHelper.NestedDefaultValues.Value<float>(c, c == 12f)).ToArray<ToolbarSettingsHelper.ValueProxy<float>>();
				this.allValues[ContextMenuItemType.FontSize] = dictionary;
			}

			// Token: 0x06002EC3 RID: 11971 RVA: 0x000E58B4 File Offset: 0x000E3AB4
			private void InitFontColors()
			{
				Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>> dictionary = new Dictionary<AnnotationMode, IReadOnlyList<ToolbarSettingsHelper.ValueProxy<string>>>();
				dictionary[AnnotationMode.None] = new ToolbarSettingsHelper.ValueProxy<string>[]
				{
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFFFFFF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF000000", true),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFB302F"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFD9927"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFFAFF00"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FFA5DE50"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF43D9EF"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF52AAEC"),
					ToolbarSettingsHelper.NestedDefaultValues.Value<string>("#FF9573E4")
				};
				this.allValues[ContextMenuItemType.FontColor] = dictionary;
			}

			// Token: 0x06002EC4 RID: 11972 RVA: 0x000E5958 File Offset: 0x000E3B58
			private static ToolbarSettingsHelper.ValueProxy<T> Value<T>(T value, bool isDefault)
			{
				return new ToolbarSettingsHelper.ValueProxy<T>(value, isDefault);
			}

			// Token: 0x06002EC5 RID: 11973 RVA: 0x000E5961 File Offset: 0x000E3B61
			private static ToolbarSettingsHelper.ValueProxy<T> Value<T>(T value)
			{
				return ToolbarSettingsHelper.NestedDefaultValues.Value<T>(value, false);
			}

			// Token: 0x04001AD2 RID: 6866
			private Dictionary<ContextMenuItemType, IDictionary> allValues;
		}

		// Token: 0x020004CB RID: 1227
		private class ValueProxy<T>
		{
			// Token: 0x06002EC6 RID: 11974 RVA: 0x000E596A File Offset: 0x000E3B6A
			public ValueProxy(T value, bool isDefault)
			{
				this.Value = value;
				this.IsDefault = isDefault;
			}

			// Token: 0x17000CC2 RID: 3266
			// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x000E5980 File Offset: 0x000E3B80
			public T Value { get; }

			// Token: 0x17000CC3 RID: 3267
			// (get) Token: 0x06002EC8 RID: 11976 RVA: 0x000E5988 File Offset: 0x000E3B88
			public bool IsDefault { get; }
		}
	}
}
