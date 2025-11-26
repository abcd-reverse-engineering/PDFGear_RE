using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace pdfeditor.Properties
{
	// Token: 0x02000125 RID: 293
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x00049A59 File Offset: 0x00047C59
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040005B1 RID: 1457
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
