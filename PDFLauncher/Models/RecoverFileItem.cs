using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PDFLauncher.Models
{
	// Token: 0x0200001B RID: 27
	public class RecoverFileItem : ObservableObject
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000696F File Offset: 0x00004B6F
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00006977 File Offset: 0x00004B77
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

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000698C File Offset: 0x00004B8C
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00006994 File Offset: 0x00004B94
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				base.SetProperty<string>(ref this.displayName, value, "DisplayName");
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600018F RID: 399 RVA: 0x000069A9 File Offset: 0x00004BA9
		// (set) Token: 0x06000190 RID: 400 RVA: 0x000069B1 File Offset: 0x00004BB1
		public string LastTime
		{
			get
			{
				return this.lastTime;
			}
			set
			{
				base.SetProperty<string>(ref this.lastTime, value, "LastTime");
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000191 RID: 401 RVA: 0x000069C6 File Offset: 0x00004BC6
		// (set) Token: 0x06000192 RID: 402 RVA: 0x000069CE File Offset: 0x00004BCE
		public RecoverStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				base.SetProperty<RecoverStatus>(ref this.status, value, "Status");
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000193 RID: 403 RVA: 0x000069E3 File Offset: 0x00004BE3
		// (set) Token: 0x06000194 RID: 404 RVA: 0x000069EB File Offset: 0x00004BEB
		public bool? IsFileSelected
		{
			get
			{
				return this.isFileSelected;
			}
			set
			{
				base.SetProperty<bool?>(ref this.isFileSelected, value, "IsFileSelected");
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00006A00 File Offset: 0x00004C00
		// (set) Token: 0x06000196 RID: 406 RVA: 0x00006A08 File Offset: 0x00004C08
		public string SourceDir
		{
			get
			{
				return this.sourceDir;
			}
			set
			{
				base.SetProperty<string>(ref this.sourceDir, value, "SourceDir");
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00006A1D File Offset: 0x00004C1D
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00006A25 File Offset: 0x00004C25
		public string RecoverDir
		{
			get
			{
				return this.recoverDir;
			}
			set
			{
				base.SetProperty<string>(ref this.recoverDir, value, "RecoverDir");
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00006A3A File Offset: 0x00004C3A
		// (set) Token: 0x0600019A RID: 410 RVA: 0x00006A42 File Offset: 0x00004C42
		public string RecoverFullFileName
		{
			get
			{
				return this.recoverFullFileName;
			}
			set
			{
				base.SetProperty<string>(ref this.recoverFullFileName, value, "RecoverFullFileName");
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00006A57 File Offset: 0x00004C57
		// (set) Token: 0x0600019C RID: 412 RVA: 0x00006A5F File Offset: 0x00004C5F
		public string SourceFullFileName
		{
			get
			{
				return this.sourceFullFileName;
			}
			set
			{
				base.SetProperty<string>(ref this.sourceFullFileName, value, "SourceFullFileName");
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00006A74 File Offset: 0x00004C74
		// (set) Token: 0x0600019E RID: 414 RVA: 0x00006A7C File Offset: 0x00004C7C
		public string FileGuid
		{
			get
			{
				return this.fileGuid;
			}
			set
			{
				base.SetProperty<string>(ref this.fileGuid, value, "FileGuid");
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600019F RID: 415 RVA: 0x00006A91 File Offset: 0x00004C91
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x00006A99 File Offset: 0x00004C99
		public string EditorSourceFullFileName
		{
			get
			{
				return this.editorSourceFullFileName;
			}
			set
			{
				base.SetProperty<string>(ref this.editorSourceFullFileName, value, "EditorSourceFullFileName");
			}
		}

		// Token: 0x040000C1 RID: 193
		private string fileName;

		// Token: 0x040000C2 RID: 194
		private string displayName;

		// Token: 0x040000C3 RID: 195
		private string lastTime;

		// Token: 0x040000C4 RID: 196
		private RecoverStatus status;

		// Token: 0x040000C5 RID: 197
		private bool? isFileSelected = new bool?(true);

		// Token: 0x040000C6 RID: 198
		private string sourceDir;

		// Token: 0x040000C7 RID: 199
		private string recoverDir;

		// Token: 0x040000C8 RID: 200
		private string recoverFullFileName;

		// Token: 0x040000C9 RID: 201
		public string sourceFullFileName;

		// Token: 0x040000CA RID: 202
		public string fileGuid;

		// Token: 0x040000CB RID: 203
		private string editorSourceFullFileName;
	}
}
