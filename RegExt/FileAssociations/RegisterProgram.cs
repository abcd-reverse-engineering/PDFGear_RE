using System;
using Microsoft.Win32;

namespace RegExt.FileAssociations
{
	// Token: 0x02000008 RID: 8
	internal class RegisterProgram
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00003DB1 File Offset: 0x00001FB1
		public string ProgramId { get; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003DB9 File Offset: 0x00001FB9
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00003DC1 File Offset: 0x00001FC1
		public string TypeName { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003DCA File Offset: 0x00001FCA
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00003DD2 File Offset: 0x00001FD2
		public string FriendlyTypeName { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00003DDB File Offset: 0x00001FDB
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00003DE3 File Offset: 0x00001FE3
		public string DefaultIcon { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003DEC File Offset: 0x00001FEC
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00003DF4 File Offset: 0x00001FF4
		public bool? IsAlwaysShowExt { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003DFD File Offset: 0x00001FFD
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00003E05 File Offset: 0x00002005
		public string Operation { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003E0E File Offset: 0x0000200E
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003E16 File Offset: 0x00002016
		public string Command { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003E1F File Offset: 0x0000201F
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00003E27 File Offset: 0x00002027
		public string FriendlyAppName { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003E30 File Offset: 0x00002030
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00003E38 File Offset: 0x00002038
		public string AppUserModelID { get; set; }

		// Token: 0x06000045 RID: 69 RVA: 0x00003E41 File Offset: 0x00002041
		public RegisterProgram(string programId)
		{
			if (string.IsNullOrWhiteSpace(programId))
			{
				throw new ArgumentNullException("programId");
			}
			this.ProgramId = programId;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003E63 File Offset: 0x00002063
		public void WriteToCurrentUser()
		{
			this.WriteToRegistry(RegistryHive.CurrentUser);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003E70 File Offset: 0x00002070
		public void WriteToAllUser()
		{
			this.WriteToRegistry(RegistryHive.LocalMachine);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003E80 File Offset: 0x00002080
		public void RemoveFromCurrentUser()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
			{
				registryKey.DeleteSubKeyTree(this.BuildRegistryPath(this.ProgramId), false);
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003ECC File Offset: 0x000020CC
		public void RemoveFromAllUser()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			{
				registryKey.DeleteSubKeyTree(this.BuildRegistryPath(this.ProgramId), false);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003F18 File Offset: 0x00002118
		private void WriteToRegistry(RegistryHive registryHive)
		{
			registryHive.Write32(this.BuildRegistryPath(this.ProgramId), this.TypeName ?? string.Empty);
			if (this.FriendlyTypeName != null && !string.IsNullOrWhiteSpace(this.FriendlyTypeName))
			{
				registryHive.Write32(this.BuildRegistryPath(this.ProgramId), "FriendlyTypeName", this.FriendlyTypeName);
			}
			if (!string.IsNullOrWhiteSpace(this.AppUserModelID))
			{
				registryHive.Write32(this.BuildRegistryPath(this.ProgramId), "AppUserModelID", this.AppUserModelID);
			}
			if (this.IsAlwaysShowExt != null)
			{
				registryHive.Write32(this.BuildRegistryPath(this.ProgramId), "IsAlwaysShowExt", this.IsAlwaysShowExt.Value ? "1" : "0");
			}
			if (this.DefaultIcon != null && !string.IsNullOrWhiteSpace(this.DefaultIcon))
			{
				registryHive.Write32(this.BuildRegistryPath(this.ProgramId + "\\DefaultIcon"), this.DefaultIcon);
			}
			if (this.Operation != null && !string.IsNullOrWhiteSpace(this.Operation))
			{
				registryHive.Write32(this.BuildRegistryPath(this.ProgramId + "\\shell\\" + this.Operation + "\\command"), this.Command ?? string.Empty);
				if (!string.IsNullOrWhiteSpace(this.FriendlyAppName))
				{
					registryHive.Write32(this.BuildRegistryPath(this.ProgramId + "\\shell\\" + this.Operation), "FriendlyAppName", this.FriendlyAppName);
				}
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000040A1 File Offset: 0x000022A1
		private string BuildRegistryPath(string relativePath)
		{
			return "Software\\Classes\\" + relativePath;
		}
	}
}
