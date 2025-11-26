using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace FileWatcher.Properties
{
	// Token: 0x02000016 RID: 22
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000030D6 File Offset: 0x000012D6
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400003E RID: 62
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
