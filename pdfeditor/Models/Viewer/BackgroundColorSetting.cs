using System;
using System.Windows.Media;
using pdfeditor.Properties;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x0200012B RID: 299
	public class BackgroundColorSetting
	{
		// Token: 0x06001250 RID: 4688 RVA: 0x0004ADC0 File Offset: 0x00048FC0
		public BackgroundColorSetting(string name, string displayNameResourceKey, string defaultDisplayName, Color backgroundColor, Color pageMaskColor)
		{
			this.Name = name;
			try
			{
				this.DisplayName = Resources.ResourceManager.GetString(displayNameResourceKey, Resources.Culture);
			}
			catch
			{
			}
			if (this.DisplayName == null)
			{
				this.DisplayName = defaultDisplayName;
			}
			this.BackgroundColor = backgroundColor;
			this.PageMaskColor = pageMaskColor;
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0004AE24 File Offset: 0x00049024
		public BackgroundColorSetting(string name, string displayNameResourceKey, string defaultDisplayName, string backgroundColor, string pageMaskColor)
			: this(name, displayNameResourceKey, defaultDisplayName, (Color)ColorConverter.ConvertFromString(backgroundColor), (Color)ColorConverter.ConvertFromString(pageMaskColor))
		{
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x0004AE47 File Offset: 0x00049047
		public string Name { get; }

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06001253 RID: 4691 RVA: 0x0004AE4F File Offset: 0x0004904F
		public string DisplayName { get; }

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06001254 RID: 4692 RVA: 0x0004AE57 File Offset: 0x00049057
		// (set) Token: 0x06001255 RID: 4693 RVA: 0x0004AE5F File Offset: 0x0004905F
		public Color BackgroundColor { get; set; }

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06001256 RID: 4694 RVA: 0x0004AE68 File Offset: 0x00049068
		public Color PageMaskColor { get; }
	}
}
