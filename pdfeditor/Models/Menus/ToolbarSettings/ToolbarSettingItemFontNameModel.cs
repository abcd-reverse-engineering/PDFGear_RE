using System;
using System.Collections.Generic;
using System.Linq;
using PDFKit.Contents;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000181 RID: 385
	public class ToolbarSettingItemFontNameModel : ToolbarSettingItemModel
	{
		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x00054E58 File Offset: 0x00053058
		private static IReadOnlyList<string> _AllFontNames
		{
			get
			{
				if (ToolbarSettingItemFontNameModel._allFontNames == null)
				{
					object obj = ToolbarSettingItemFontNameModel.locker;
					lock (obj)
					{
						if (ToolbarSettingItemFontNameModel._allFontNames == null)
						{
							ToolbarSettingItemFontNameModel._allFontNames = (from c in WindowsFonts.GetAllFontFamilies()
								select c.Name).ToList<string>();
						}
					}
				}
				return ToolbarSettingItemFontNameModel._allFontNames;
			}
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x00054ED8 File Offset: 0x000530D8
		public ToolbarSettingItemFontNameModel(ContextMenuItemType type)
			: base(type)
		{
			base.NontransientSelectedValue = "Arial";
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00054EEC File Offset: 0x000530EC
		public ToolbarSettingItemFontNameModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
			base.NontransientSelectedValue = "Arial";
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x00054F01 File Offset: 0x00053101
		public IReadOnlyList<string> AllFonts
		{
			get
			{
				return ToolbarSettingItemFontNameModel._AllFontNames;
			}
		}

		// Token: 0x04000762 RID: 1890
		private static IReadOnlyList<string> _allFontNames;

		// Token: 0x04000763 RID: 1891
		private static object locker = new object();
	}
}
