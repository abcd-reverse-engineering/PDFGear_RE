using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace RegExt.FileAssociations
{
	// Token: 0x02000007 RID: 7
	internal class RegisterFileExtension
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00003BE4 File Offset: 0x00001DE4
		public string FileExtension { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00003BEC File Offset: 0x00001DEC
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00003BF4 File Offset: 0x00001DF4
		public string ContentType { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00003BFD File Offset: 0x00001DFD
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00003C05 File Offset: 0x00001E05
		public string PerceivedType { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00003C0E File Offset: 0x00001E0E
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00003C16 File Offset: 0x00001E16
		public string DefaultProgramId { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00003C1F File Offset: 0x00001E1F
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00003C27 File Offset: 0x00001E27
		public IList<string> OpenWithProgramIds { get; set; } = new List<string>();

		// Token: 0x0600002F RID: 47 RVA: 0x00003C30 File Offset: 0x00001E30
		public RegisterFileExtension(string fileExtension)
		{
			if (string.IsNullOrWhiteSpace(fileExtension))
			{
				throw new ArgumentNullException("fileExtension");
			}
			if (!fileExtension.StartsWith(".", StringComparison.Ordinal))
			{
				throw new ArgumentException(fileExtension + " is not a valid file extension. it must start with \".\"", "fileExtension");
			}
			this.FileExtension = fileExtension;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003C8C File Offset: 0x00001E8C
		public void WriteToCurrentUser()
		{
			this.WriteToRegistry(RegistryHive.CurrentUser);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003C99 File Offset: 0x00001E99
		public void WriteToAllUser()
		{
			this.WriteToRegistry(RegistryHive.LocalMachine);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003CA8 File Offset: 0x00001EA8
		private void WriteToRegistry(RegistryHive registryHive)
		{
			registryHive.Write32(this.BuildRegistryPath(this.FileExtension), this.DefaultProgramId ?? string.Empty);
			if (this.ContentType != null && !string.IsNullOrWhiteSpace(this.ContentType))
			{
				registryHive.Write32(this.BuildRegistryPath(this.FileExtension), "Content Type", this.ContentType);
			}
			if (this.PerceivedType != null && !string.IsNullOrWhiteSpace(this.PerceivedType))
			{
				registryHive.Write32(this.BuildRegistryPath(this.FileExtension), "PerceivedType", this.PerceivedType);
			}
			if (this.OpenWithProgramIds.Count > 0)
			{
				foreach (string text in this.OpenWithProgramIds)
				{
					registryHive.Write32(this.BuildRegistryPath(this.FileExtension + "\\OpenWithProgids"), text, string.Empty);
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003DA4 File Offset: 0x00001FA4
		private string BuildRegistryPath(string relativePath)
		{
			return "Software\\Classes\\" + relativePath;
		}
	}
}
