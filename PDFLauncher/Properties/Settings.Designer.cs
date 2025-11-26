using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace PDFLauncher.Properties
{
	// Token: 0x02000011 RID: 17
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060000EA RID: 234 RVA: 0x000047D6 File Offset: 0x000029D6
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400007D RID: 125
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
