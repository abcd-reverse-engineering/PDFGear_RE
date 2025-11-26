using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Patagames.Pdf;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x020000A9 RID: 169
	public static class ToolbarContextMenuValueEqualityComparer
	{
		// Token: 0x06000A9F RID: 2719 RVA: 0x0003767A File Offset: 0x0003587A
		public static bool MenuValueEquals(AnnotationMode mode, ContextMenuItemType type, object x, object y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (object.Equals(x, y))
			{
				return true;
			}
			if (x == null)
			{
				object obj = y;
				y = x;
				x = obj;
			}
			if (type == ContextMenuItemType.FillColor || type == ContextMenuItemType.StrokeColor)
			{
				return ToolbarContextMenuValueEqualityComparer.ColorValueEquals(mode, type, x, y);
			}
			return type == ContextMenuItemType.StrokeThickness && ToolbarContextMenuValueEqualityComparer.StrokeThicknessValueEquals(mode, type, x, y);
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x000376BC File Offset: 0x000358BC
		private static bool ColorValueEquals(AnnotationMode mode, ContextMenuItemType type, object x, object y)
		{
			Color? color = ToolbarContextMenuValueEqualityComparer.<ColorValueEquals>g__ConvertToColor|1_0(x);
			Color? color2 = ToolbarContextMenuValueEqualityComparer.<ColorValueEquals>g__ConvertToColor|1_0(y);
			return object.Equals(color, color2) || (color != null && color2 != null && ((color.Value.A == 0 && color2.Value.A == 0) || (Math.Abs((int)(color.Value.R - color2.Value.R)) <= 2 && Math.Abs((int)(color.Value.G - color2.Value.G)) <= 2 && Math.Abs((int)(color.Value.B - color2.Value.B)) <= 2 && Math.Abs((int)(color.Value.A - color2.Value.A)) <= 2)));
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x000377C8 File Offset: 0x000359C8
		private static bool StrokeThicknessValueEquals(AnnotationMode mode, ContextMenuItemType type, object x, object y)
		{
			try
			{
				float num = Convert.ToSingle(x);
				float num2 = Convert.ToSingle(y);
				return num == num2;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00037800 File Offset: 0x00035A00
		[CompilerGenerated]
		internal static Color? <ColorValueEquals>g__ConvertToColor|1_0(object obj)
		{
			Color? color = null;
			if (obj is FS_COLOR)
			{
				FS_COLOR fs_COLOR = (FS_COLOR)obj;
				color = new Color?(fs_COLOR.ToColor());
			}
			else if (obj is Color)
			{
				Color color2 = (Color)obj;
				color = new Color?(color2);
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					text = text.Trim().ToLowerInvariant();
					try
					{
						color = new Color?((Color)ColorConverter.ConvertFromString(text));
					}
					catch
					{
					}
				}
			}
			return color;
		}
	}
}
