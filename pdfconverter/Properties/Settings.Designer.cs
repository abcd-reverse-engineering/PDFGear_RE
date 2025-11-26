using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace pdfconverter.Properties
{
	// Token: 0x02000025 RID: 37
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000204 RID: 516 RVA: 0x000078EE File Offset: 0x00005AEE
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400012C RID: 300
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
