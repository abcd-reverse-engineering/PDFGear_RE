using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PDFLauncher.Models
{
	// Token: 0x0200001A RID: 26
	public class OpenHistoryModel : ObservableObject
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000687C File Offset: 0x00004A7C
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00006884 File Offset: 0x00004A84
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				base.SetProperty<string>(ref this.fileName, value, "FileName");
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00006899 File Offset: 0x00004A99
		public string Extension
		{
			get
			{
				if (!string.IsNullOrEmpty(this.filePath))
				{
					return new FileInfo(this.filePath).Extension.ToLower();
				}
				return string.Empty;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000180 RID: 384 RVA: 0x000068C3 File Offset: 0x00004AC3
		// (set) Token: 0x06000181 RID: 385 RVA: 0x000068CB File Offset: 0x00004ACB
		public string FilePath
		{
			get
			{
				return this.filePath;
			}
			set
			{
				base.SetProperty<string>(ref this.filePath, value, "FilePath");
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000182 RID: 386 RVA: 0x000068E0 File Offset: 0x00004AE0
		// (set) Token: 0x06000183 RID: 387 RVA: 0x000068E8 File Offset: 0x00004AE8
		public string FileSize
		{
			get
			{
				return this.fileSize;
			}
			set
			{
				base.SetProperty<string>(ref this.fileSize, value, "FileSize");
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000184 RID: 388 RVA: 0x000068FD File Offset: 0x00004AFD
		// (set) Token: 0x06000185 RID: 389 RVA: 0x00006905 File Offset: 0x00004B05
		public string FileLastOpen
		{
			get
			{
				return this.fileLastOpen;
			}
			set
			{
				base.SetProperty<string>(ref this.fileLastOpen, value, "FileLastOpen");
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000186 RID: 390 RVA: 0x0000691A File Offset: 0x00004B1A
		// (set) Token: 0x06000187 RID: 391 RVA: 0x00006922 File Offset: 0x00004B22
		public bool IsSelect
		{
			get
			{
				return this.isSelect;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelect, value, "IsSelect");
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00006938 File Offset: 0x00004B38
		public string DisplayFilePath
		{
			get
			{
				return this.FilePath.FullPathWithoutPrefix;
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00006958 File Offset: 0x00004B58
		public OpenHistoryModel(string filePath)
		{
			this.FilePath = filePath;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00006967 File Offset: 0x00004B67
		public OpenHistoryModel()
		{
		}

		// Token: 0x040000BC RID: 188
		private string fileName;

		// Token: 0x040000BD RID: 189
		private string filePath;

		// Token: 0x040000BE RID: 190
		private string fileSize;

		// Token: 0x040000BF RID: 191
		private string fileLastOpen;

		// Token: 0x040000C0 RID: 192
		private bool isSelect;
	}
}
