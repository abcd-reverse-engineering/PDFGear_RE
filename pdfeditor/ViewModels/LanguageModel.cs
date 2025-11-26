using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommonLib.Common;
using pdfeditor.Properties;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000060 RID: 96
	public class LanguageModel
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x0001B197 File Offset: 0x00019397
		public static LanguageModel Fallback
		{
			get
			{
				LanguageModel languageModel;
				if ((languageModel = LanguageModel.fallbackLanguageModel) == null)
				{
					languageModel = (LanguageModel.fallbackLanguageModel = new LanguageModel.FallbackLanguageModel());
				}
				return languageModel;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000547 RID: 1351 RVA: 0x0001B1B0 File Offset: 0x000193B0
		public static IReadOnlyList<string> AllLanguages
		{
			get
			{
				if (LanguageModel.allLanguages == null)
				{
					object obj = LanguageModel.locker;
					lock (obj)
					{
						if (LanguageModel.allLanguages == null)
						{
							LanguageModel.allLanguages = LanguageModel.GetAllLanguages();
						}
					}
				}
				return LanguageModel.allLanguages;
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x0001B208 File Offset: 0x00019408
		public static IReadOnlyList<LanguageModel> AllLanguageModel
		{
			get
			{
				if (LanguageModel.languageModels == null)
				{
					object obj = LanguageModel.locker;
					lock (obj)
					{
						if (LanguageModel.languageModels == null)
						{
							LanguageModel.languageModels = LanguageModel.AllLanguages.Select((string c) => new LanguageModel(c)).ToList<LanguageModel>();
						}
					}
				}
				return LanguageModel.languageModels;
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001B288 File Offset: 0x00019488
		protected LanguageModel()
		{
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001B290 File Offset: 0x00019490
		public LanguageModel(string name)
		{
			this.Name = name;
			this.CultureInfo = CultureInfo.GetCultureInfo(name);
			this.NativeName = this.CultureInfo.NativeName;
			this.EnglishName = this.CultureInfo.EnglishName;
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x0001B2CD File Offset: 0x000194CD
		public virtual string NativeName { get; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001B2D5 File Offset: 0x000194D5
		public virtual string EnglishName { get; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x0001B2DD File Offset: 0x000194DD
		public virtual string ResourceName
		{
			get
			{
				return this.Name;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001B2E5 File Offset: 0x000194E5
		public virtual CultureInfo CultureInfo { get; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x0001B2ED File Offset: 0x000194ED
		public virtual string Name { get; }

		// Token: 0x06000550 RID: 1360 RVA: 0x0001B2F8 File Offset: 0x000194F8
		private static IReadOnlyList<string> GetAllLanguages()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			return new string[] { "en" }.Concat(from c in directoryInfo.GetDirectories().Where(delegate(DirectoryInfo c)
				{
					try
					{
						return c.GetFiles().Any((FileInfo x) => x.Name == "pdfeditor.resources.dll");
					}
					catch
					{
					}
					return false;
				})
				select c.Name).OrderBy(delegate(string c)
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(c);
				if (cultureInfo == null)
				{
					return null;
				}
				return cultureInfo.EnglishName;
			}, StringComparer.OrdinalIgnoreCase).ToList<string>();
		}

		// Token: 0x040002C8 RID: 712
		private static LanguageModel fallbackLanguageModel;

		// Token: 0x040002C9 RID: 713
		private static IReadOnlyList<string> allLanguages;

		// Token: 0x040002CA RID: 714
		private static IReadOnlyList<LanguageModel> languageModels;

		// Token: 0x040002CB RID: 715
		private static object locker = new object();

		// Token: 0x02000344 RID: 836
		private class FallbackLanguageModel : LanguageModel
		{
			// Token: 0x060029F5 RID: 10741 RVA: 0x000C9B94 File Offset: 0x000C7D94
			public FallbackLanguageModel()
			{
				foreach (LanguageModel languageModel in LanguageModel.AllLanguageModel)
				{
					if (languageModel.Name == CultureInfoUtils.SuggestAppLanguage)
					{
						this.englishName = languageModel.EnglishName;
						break;
					}
				}
			}

			// Token: 0x17000C67 RID: 3175
			// (get) Token: 0x060029F6 RID: 10742 RVA: 0x000C9C00 File Offset: 0x000C7E00
			public override string Name
			{
				get
				{
					return "";
				}
			}

			// Token: 0x17000C68 RID: 3176
			// (get) Token: 0x060029F7 RID: 10743 RVA: 0x000C9C07 File Offset: 0x000C7E07
			public override CultureInfo CultureInfo
			{
				get
				{
					return CultureInfoUtils.SystemUICultureInfo;
				}
			}

			// Token: 0x17000C69 RID: 3177
			// (get) Token: 0x060029F8 RID: 10744 RVA: 0x000C9C0E File Offset: 0x000C7E0E
			public override string NativeName
			{
				get
				{
					return Resources.AppSettingsLanguageAutoItem;
				}
			}

			// Token: 0x17000C6A RID: 3178
			// (get) Token: 0x060029F9 RID: 10745 RVA: 0x000C9C15 File Offset: 0x000C7E15
			public override string EnglishName
			{
				get
				{
					return this.englishName;
				}
			}

			// Token: 0x040013AD RID: 5037
			private string englishName;
		}
	}
}
