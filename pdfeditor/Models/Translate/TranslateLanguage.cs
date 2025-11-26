using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonLib.Common;
using pdfeditor.Properties;

namespace pdfeditor.Models.Translate
{
	// Token: 0x02000134 RID: 308
	public class TranslateLanguage
	{
		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x0004C13A File Offset: 0x0004A33A
		// (set) Token: 0x060012B3 RID: 4787 RVA: 0x0004C142 File Offset: 0x0004A342
		public string Name { get; private set; }

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x060012B4 RID: 4788 RVA: 0x0004C14B File Offset: 0x0004A34B
		// (set) Token: 0x060012B5 RID: 4789 RVA: 0x0004C153 File Offset: 0x0004A353
		public string DisplayName { get; private set; }

		// Token: 0x060012B6 RID: 4790 RVA: 0x0004C15C File Offset: 0x0004A35C
		public TranslateLanguage(string name)
		{
			this.Name = name;
			this.DisplayName = CultureInfo.GetCultureInfo(name).NativeName;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0004C17C File Offset: 0x0004A37C
		public TranslateLanguage()
		{
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x0004C184 File Offset: 0x0004A384
		public static TranslateLanguage CreateAutoLanguage(bool showLanguageName)
		{
			if (showLanguageName)
			{
				return new TranslateLanguage
				{
					Name = "AutoLanguage",
					DisplayName = Resources.TranslatePanelAuto + " - " + TranslateLanguage.GetAutoLanguage()
				};
			}
			return new TranslateLanguage
			{
				Name = "Auto",
				DisplayName = Resources.TranslatePanelAutoDetect
			};
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x0004C1DA File Offset: 0x0004A3DA
		private static string GetAutoLanguage()
		{
			return CultureInfo.GetCultureInfo(CultureInfoUtils.ActualAppLanguage).NativeName;
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x0004C1EC File Offset: 0x0004A3EC
		public static List<TranslateLanguage> AllLanguageModel
		{
			get
			{
				if (TranslateLanguage.languageModels == null)
				{
					object obj = TranslateLanguage.locker;
					lock (obj)
					{
						if (TranslateLanguage.languageModels == null)
						{
							TranslateLanguage.languageModels = CultureInfoUtils.AllSupportLanguage.Select((string c) => new TranslateLanguage(c)).ToList<TranslateLanguage>();
						}
					}
				}
				return TranslateLanguage.languageModels;
			}
		}

		// Token: 0x040005ED RID: 1517
		private static List<TranslateLanguage> languageModels;

		// Token: 0x040005EE RID: 1518
		private static object locker = new object();
	}
}
