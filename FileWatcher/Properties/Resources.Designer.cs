using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FileWatcher.Properties
{
	// Token: 0x02000015 RID: 21
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Resources
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00003051 File Offset: 0x00001251
		internal Resources()
		{
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003059 File Offset: 0x00001259
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("FileWatcher.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00003085 File Offset: 0x00001285
		// (set) Token: 0x0600005A RID: 90 RVA: 0x0000308C File Offset: 0x0000128C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003094 File Offset: 0x00001294
		public static string WinRecivedBtnOpenFile
		{
			get
			{
				return Resources.ResourceManager.GetString("WinRecivedBtnOpenFile", Resources.resourceCulture);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000030AA File Offset: 0x000012AA
		public static string WinRecivedFileTitle
		{
			get
			{
				return Resources.ResourceManager.GetString("WinRecivedFileTitle", Resources.resourceCulture);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000030C0 File Offset: 0x000012C0
		public static string WinRecivedNotshowagainContent
		{
			get
			{
				return Resources.ResourceManager.GetString("WinRecivedNotshowagainContent", Resources.resourceCulture);
			}
		}

		// Token: 0x0400003C RID: 60
		private static ResourceManager resourceMan;

		// Token: 0x0400003D RID: 61
		private static CultureInfo resourceCulture;
	}
}
