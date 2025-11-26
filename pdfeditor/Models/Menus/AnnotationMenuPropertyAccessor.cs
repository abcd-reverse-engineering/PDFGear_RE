using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000156 RID: 342
	public class AnnotationMenuPropertyAccessor : ObservableObject
	{
		// Token: 0x0600146C RID: 5228 RVA: 0x0005117F File Offset: 0x0004F37F
		public AnnotationMenuPropertyAccessor(AnnotationToolbarViewModel vm)
		{
			if (vm == null)
			{
				throw new ArgumentNullException("vm");
			}
			this.vm = vm;
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600146D RID: 5229 RVA: 0x0005119D File Offset: 0x0004F39D
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Line, ContextMenuItemType.StrokeColor)]
		public string LineStroke
		{
			get
			{
				return this.StrokeColor(this.vm.LineButtonModel);
			}
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x0600146E RID: 5230 RVA: 0x000511B0 File Offset: 0x0004F3B0
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Line, ContextMenuItemType.StrokeThickness)]
		public float LineWidth
		{
			get
			{
				return this.StrokeThickness(this.vm.LineButtonModel, 1f);
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x0600146F RID: 5231 RVA: 0x000511C8 File Offset: 0x0004F3C8
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Underline, ContextMenuItemType.StrokeColor)]
		public string UnderlineStroke
		{
			get
			{
				return this.StrokeColor(this.vm.UnderlineButtonModel);
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06001470 RID: 5232 RVA: 0x000511DB File Offset: 0x0004F3DB
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Highlight, ContextMenuItemType.StrokeColor)]
		public string HighlightStroke
		{
			get
			{
				return this.StrokeColor(this.vm.HighlightButtonModel);
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06001471 RID: 5233 RVA: 0x000511EE File Offset: 0x0004F3EE
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.HighlightArea, ContextMenuItemType.StrokeColor)]
		public string HighlightAreaStroke
		{
			get
			{
				return this.StrokeColor(this.vm.HighlightAreaButtonModel);
			}
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06001472 RID: 5234 RVA: 0x00051201 File Offset: 0x0004F401
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Strike, ContextMenuItemType.StrokeColor)]
		public string StrikeStroke
		{
			get
			{
				return this.StrokeColor(this.vm.StrikeButtonModel);
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06001473 RID: 5235 RVA: 0x00051214 File Offset: 0x0004F414
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Ink, ContextMenuItemType.StrokeColor)]
		public string InkStroke
		{
			get
			{
				return this.StrokeColor(this.vm.InkButtonModel);
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x06001474 RID: 5236 RVA: 0x00051227 File Offset: 0x0004F427
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Ink, ContextMenuItemType.StrokeThickness)]
		public float InkWidth
		{
			get
			{
				return this.StrokeThickness(this.vm.InkButtonModel, 1f);
			}
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06001475 RID: 5237 RVA: 0x0005123F File Offset: 0x0004F43F
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Shape, ContextMenuItemType.StrokeColor)]
		public string ShapeStroke
		{
			get
			{
				return this.StrokeColor(this.vm.SquareButtonModel);
			}
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x06001476 RID: 5238 RVA: 0x00051252 File Offset: 0x0004F452
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Shape, ContextMenuItemType.FillColor)]
		public string ShapeFill
		{
			get
			{
				return this.FillColor(this.vm.SquareButtonModel);
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x06001477 RID: 5239 RVA: 0x00051265 File Offset: 0x0004F465
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Shape, ContextMenuItemType.StrokeThickness)]
		public float ShapeThickness
		{
			get
			{
				return this.StrokeThickness(this.vm.SquareButtonModel, 1f);
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06001478 RID: 5240 RVA: 0x0005127D File Offset: 0x0004F47D
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Ellipse, ContextMenuItemType.FillColor)]
		public string EllipseFill
		{
			get
			{
				return this.FillColor(this.vm.CircleButtonModel);
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x06001479 RID: 5241 RVA: 0x00051290 File Offset: 0x0004F490
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Ellipse, ContextMenuItemType.StrokeColor)]
		public string EllipseStroke
		{
			get
			{
				return this.StrokeColor(this.vm.CircleButtonModel);
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600147A RID: 5242 RVA: 0x000512A3 File Offset: 0x0004F4A3
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Ellipse, ContextMenuItemType.StrokeThickness)]
		public float EllipseThickness
		{
			get
			{
				return this.StrokeThickness(this.vm.CircleButtonModel, 1f);
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600147B RID: 5243 RVA: 0x000512BB File Offset: 0x0004F4BB
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.StrokeColor)]
		public string TextBoxStroke
		{
			get
			{
				return this.StrokeColor(this.vm.TextBoxButtonModel);
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x0600147C RID: 5244 RVA: 0x000512CE File Offset: 0x0004F4CE
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.FillColor)]
		public string TextBoxFill
		{
			get
			{
				return this.FillColor(this.vm.TextBoxButtonModel);
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x0600147D RID: 5245 RVA: 0x000512E1 File Offset: 0x0004F4E1
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.StrokeThickness)]
		public float TextBoxThickness
		{
			get
			{
				return this.StrokeThickness(this.vm.TextBoxButtonModel, 1f);
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x0600147E RID: 5246 RVA: 0x000512F9 File Offset: 0x0004F4F9
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.FontSize)]
		public float TextBoxFontSize
		{
			get
			{
				return this.FontSize(this.vm.TextBoxButtonModel, 12f);
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x0600147F RID: 5247 RVA: 0x00051311 File Offset: 0x0004F511
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.FontColor)]
		public string TextBoxFontColor
		{
			get
			{
				return this.FontColor(this.vm.TextBoxButtonModel);
			}
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x06001480 RID: 5248 RVA: 0x00051324 File Offset: 0x0004F524
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.TextBox, ContextMenuItemType.FontName)]
		public string TextBoxFontName
		{
			get
			{
				return "Arial";
			}
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x06001481 RID: 5249 RVA: 0x0005132B File Offset: 0x0004F52B
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Text, ContextMenuItemType.FontSize)]
		public float TextFontSize
		{
			get
			{
				return this.FontSize(this.vm.TextButtonModel, 12f);
			}
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x06001482 RID: 5250 RVA: 0x00051343 File Offset: 0x0004F543
		[AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute(AnnotationMode.Text, ContextMenuItemType.FontColor)]
		public string TextFontColor
		{
			get
			{
				return this.FontColor(this.vm.TextButtonModel);
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00051356 File Offset: 0x0004F556
		private string StrokeColor(ToolbarAnnotationButtonModel buttonModel)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = buttonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.StrokeColor);
			return ((toolbarSettingItemModel != null) ? toolbarSettingItemModel.SelectedValue : null) as string;
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00051393 File Offset: 0x0004F593
		private string FillColor(ToolbarAnnotationButtonModel buttonModel)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = buttonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.FillColor);
			return ((toolbarSettingItemModel != null) ? toolbarSettingItemModel.SelectedValue : null) as string;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000513D0 File Offset: 0x0004F5D0
		private string FontColor(ToolbarAnnotationButtonModel buttonModel)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = buttonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.FontColor);
			return ((toolbarSettingItemModel != null) ? toolbarSettingItemModel.SelectedValue : null) as string;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x0005140D File Offset: 0x0004F60D
		private float StrokeThickness(ToolbarAnnotationButtonModel buttonModel, float defaultValue = 1f)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = buttonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.StrokeThickness);
			return this.ToFloat((toolbarSettingItemModel != null) ? toolbarSettingItemModel.SelectedValue : null, defaultValue);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x0005144C File Offset: 0x0004F64C
		private float FontSize(ToolbarAnnotationButtonModel buttonModel, float defaultValue = 12f)
		{
			ToolbarSettingItemModel toolbarSettingItemModel = buttonModel.ToolbarSettingModel.FirstOrDefault((ToolbarSettingItemModel c) => c.Type == ContextMenuItemType.FontSize);
			return this.ToFloat((toolbarSettingItemModel != null) ? toolbarSettingItemModel.SelectedValue : null, defaultValue);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0005148C File Offset: 0x0004F68C
		private float ToFloat(object value, float defaultValue = 1f)
		{
			try
			{
				string text = value as string;
				if (text != null)
				{
					if (text.ToLowerInvariant().EndsWith("pt"))
					{
						text = text.Substring(0, text.Length - 2);
					}
					return Convert.ToSingle(text);
				}
				return Convert.ToSingle(value);
			}
			catch
			{
			}
			return defaultValue;
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000514F0 File Offset: 0x0004F6F0
		public object GetTagDataValue(AnnotationMode mode, ContextMenuItemType type)
		{
			AnnotationMenuPropertyAccessor.InitPropertiesTable();
			Func<AnnotationMenuPropertyAccessor, object> func;
			if (AnnotationMenuPropertyAccessor.dict.TryGetValue(new global::System.ValueTuple<AnnotationMode, ContextMenuItemType>(mode, type), out func))
			{
				return func(this);
			}
			return null;
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x00051520 File Offset: 0x0004F720
		public static string BuildPropertyName(ToolbarAnnotationButtonModel model, SelectedAccessorSelectionChangedEventArgs args)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			return AnnotationMenuPropertyAccessor.BuildPropertyNameCore(model.Mode, args.Type);
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0005154F File Offset: 0x0004F74F
		public static string BuildPropertyName(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode == AnnotationMode.None)
			{
				throw new ArgumentNullException("mode");
			}
			return AnnotationMenuPropertyAccessor.BuildPropertyNameCore(mode, type);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00051568 File Offset: 0x0004F768
		private static string BuildPropertyNameCore(AnnotationMode mode, ContextMenuItemType type)
		{
			string text = mode.ToString();
			string text2;
			if (type == ContextMenuItemType.StrokeColor)
			{
				text2 = "Stroke";
			}
			else if (type == ContextMenuItemType.FillColor)
			{
				text2 = "Fill";
			}
			else if (type == ContextMenuItemType.FontName)
			{
				text2 = "FontName";
			}
			else if (type == ContextMenuItemType.FontSize)
			{
				text2 = "FontSize";
			}
			else if (type == ContextMenuItemType.FontColor)
			{
				text2 = "FontColor";
			}
			else if (type == ContextMenuItemType.StrokeThickness)
			{
				if (mode == AnnotationMode.Line || mode == AnnotationMode.Arrow || mode == AnnotationMode.Ink)
				{
					text2 = "Width";
				}
				else
				{
					text2 = "Thickness";
				}
			}
			else
			{
				text2 = "_err";
			}
			return text + text2;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x000515F0 File Offset: 0x0004F7F0
		private static void InitPropertiesTable()
		{
			if (AnnotationMenuPropertyAccessor.dict == null)
			{
				Type typeFromHandle = typeof(AnnotationMenuPropertyAccessor);
				lock (typeFromHandle)
				{
					if (AnnotationMenuPropertyAccessor.dict == null)
					{
						AnnotationMenuPropertyAccessor.dict = (from c in (from c in typeof(AnnotationMenuPropertyAccessor).GetProperties()
								select new global::System.ValueTuple<PropertyInfo, AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>(c, c.GetCustomAttribute<AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>()) into c
								where c.Item2 != null
								select c).ToArray<global::System.ValueTuple<PropertyInfo, AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>>()
							select new global::System.ValueTuple<global::System.ValueTuple<PropertyInfo, AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>, Func<AnnotationMenuPropertyAccessor, object>>(c, AnnotationMenuPropertyAccessor.<InitPropertiesTable>g__BuildFunc|57_0(c.Item1)) into c
							where c.Item2 != null
							select c).ToDictionary(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "k", "v", "property", "attribute" })] global::System.ValueTuple<global::System.ValueTuple<PropertyInfo, AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>, Func<AnnotationMenuPropertyAccessor, object>> c) => new global::System.ValueTuple<AnnotationMode, ContextMenuItemType>(c.Item1.Item2.Mode, c.Item1.Item2.Type), ([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "k", "v", "property", "attribute" })] global::System.ValueTuple<global::System.ValueTuple<PropertyInfo, AnnotationMenuPropertyAccessor.MenuPropertyBindAttribute>, Func<AnnotationMenuPropertyAccessor, object>> c) => c.Item2);
					}
				}
			}
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x0005173C File Offset: 0x0004F93C
		[CompilerGenerated]
		internal static Func<AnnotationMenuPropertyAccessor, object> <InitPropertiesTable>g__BuildFunc|57_0(PropertyInfo property)
		{
			return TypeHelper.CreateFieldOrPropertyGetter<AnnotationMenuPropertyAccessor, object>(property.Name, BindingFlags.Instance | BindingFlags.Public);
		}

		// Token: 0x040006CA RID: 1738
		private static Dictionary<global::System.ValueTuple<AnnotationMode, ContextMenuItemType>, Func<AnnotationMenuPropertyAccessor, object>> dict;

		// Token: 0x040006CB RID: 1739
		private readonly AnnotationToolbarViewModel vm;

		// Token: 0x0200057E RID: 1406
		[AttributeUsage(AttributeTargets.Property)]
		public class MenuPropertyBindAttribute : Attribute
		{
			// Token: 0x06003141 RID: 12609 RVA: 0x000F22B2 File Offset: 0x000F04B2
			public MenuPropertyBindAttribute(AnnotationMode mode, ContextMenuItemType type)
			{
				this.Mode = mode;
				this.Type = type;
			}

			// Token: 0x17000D1C RID: 3356
			// (get) Token: 0x06003142 RID: 12610 RVA: 0x000F22C8 File Offset: 0x000F04C8
			public AnnotationMode Mode { get; }

			// Token: 0x17000D1D RID: 3357
			// (get) Token: 0x06003143 RID: 12611 RVA: 0x000F22D0 File Offset: 0x000F04D0
			public ContextMenuItemType Type { get; }
		}
	}
}
