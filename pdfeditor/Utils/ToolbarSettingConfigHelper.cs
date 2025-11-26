using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Config;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x020000AA RID: 170
	internal static class ToolbarSettingConfigHelper
	{
		// Token: 0x06000AA3 RID: 2723 RVA: 0x0003788C File Offset: 0x00035A8C
		public static string BuildConfigKey(ToolbarSettingId id, ContextMenuItemType type)
		{
			if (id.Type == ToolbarSettingType.Annotation)
			{
				return string.Format("ToolbarSettingConfig_{0}_{1}", (int)id.AnnotationMode, (int)type);
			}
			if (id.Type == ToolbarSettingType.EditDocument)
			{
				return string.Format("ToolbarSettingConfig_{0}_{1}", "editdoc", (int)type);
			}
			return string.Empty;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x000378E2 File Offset: 0x00035AE2
		public static string BuildRecentColorKey(ToolbarSettingId id, ContextMenuItemType type)
		{
			if (id.Type == ToolbarSettingType.Annotation)
			{
				return ToolbarSettingConfigHelper.BuildRecentColorKey(id.AnnotationMode, type);
			}
			if (id.Type == ToolbarSettingType.EditDocument)
			{
				return string.Format("TS_RecentColors_{0}_{1}", "editdoc", (int)type);
			}
			return string.Empty;
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0003791E File Offset: 0x00035B1E
		public static string BuildRecentColorKey(AnnotationMode mode, ContextMenuItemType type)
		{
			return string.Format("TS_RecentColors_{0}_{1}", (int)mode, (int)type);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00037938 File Offset: 0x00035B38
		public static async Task SaveConfigAsync(string key, Dictionary<string, string> dict)
		{
			if (!string.IsNullOrEmpty(key) && key.StartsWith("ToolbarSettingConfig_"))
			{
				try
				{
					await ConfigUtils.TrySetAsync<Dictionary<string, string>>(key, dict, default(CancellationToken)).ConfigureAwait(false);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00037984 File Offset: 0x00035B84
		public static async Task<Dictionary<string, string>> LoadConfigAsync(string key)
		{
			Dictionary<string, string> dictionary;
			if (string.IsNullOrEmpty(key) || !key.StartsWith("ToolbarSettingConfig_"))
			{
				dictionary = null;
			}
			else
			{
				try
				{
					global::System.ValueTuple<bool, Dictionary<string, string>> valueTuple = await ConfigUtils.TryGetAsync<Dictionary<string, string>>(key, default(CancellationToken)).ConfigureAwait(false);
					if (valueTuple.Item1)
					{
						return valueTuple.Item2;
					}
				}
				catch
				{
				}
				dictionary = null;
			}
			return dictionary;
		}

		// Token: 0x040004B1 RID: 1201
		private const string ToolbarSettingConfigTemplate = "ToolbarSettingConfig_{0}_{1}";

		// Token: 0x040004B2 RID: 1202
		private const string ToolbarSettingColorPickerRecentKeyTemplate = "TS_RecentColors_{0}_{1}";
	}
}
