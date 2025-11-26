using System;

namespace pdfeditor.Models.Protection
{
	// Token: 0x02000149 RID: 329
	public class EncryptManage
	{
		// Token: 0x06001382 RID: 4994 RVA: 0x0004EE6F File Offset: 0x0004D06F
		public void Init()
		{
			this.IsRemoveAllPassword = false;
			this.IsHaveUserPassword = false;
			this.IsChangedPassword = false;
			this.UserPassword = string.Empty;
			this.OwerPassword = string.Empty;
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x0004EE9C File Offset: 0x0004D09C
		public bool NotInputExistOwerpwd
		{
			get
			{
				return this.IsHaveOwerPassword && this.IsRequiredOwerPassword;
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x0004EEAE File Offset: 0x0004D0AE
		// (set) Token: 0x06001385 RID: 4997 RVA: 0x0004EEB6 File Offset: 0x0004D0B6
		public bool IsRemoveAllPassword { get; private set; }

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x0004EEBF File Offset: 0x0004D0BF
		// (set) Token: 0x06001387 RID: 4999 RVA: 0x0004EEC7 File Offset: 0x0004D0C7
		public string UserPassword { get; private set; }

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06001388 RID: 5000 RVA: 0x0004EED0 File Offset: 0x0004D0D0
		// (set) Token: 0x06001389 RID: 5001 RVA: 0x0004EED8 File Offset: 0x0004D0D8
		public string OwerPassword { get; private set; }

		// Token: 0x0600138A RID: 5002 RVA: 0x0004EEE1 File Offset: 0x0004D0E1
		public void UpdateUserPassword(string pwd)
		{
			this.UserPassword = pwd;
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x0004EEEA File Offset: 0x0004D0EA
		public void UpdateOwerPassword(string pwd)
		{
			this.IsRequiredOwerPassword = false;
			this.OwerPassword = pwd;
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x0004EEFA File Offset: 0x0004D0FA
		public void SetPassword(string userpassword, string owerpassword = "")
		{
			if (string.IsNullOrWhiteSpace(owerpassword))
			{
				owerpassword = userpassword;
			}
			this.UserPassword = userpassword;
			this.OwerPassword = owerpassword;
			this.IsRemoveAllPassword = false;
			this.IsRequiredOwerPassword = false;
			this.IsHaveUserPassword = true;
			this.IsHaveOwerPassword = true;
			this.IsChangedPassword = true;
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x0004EF38 File Offset: 0x0004D138
		public void RemoveAllPassword()
		{
			this.IsRemoveAllPassword = true;
			this.UserPassword = string.Empty;
			this.OwerPassword = string.Empty;
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x0600138E RID: 5006 RVA: 0x0004EF57 File Offset: 0x0004D157
		// (set) Token: 0x0600138F RID: 5007 RVA: 0x0004EF5F File Offset: 0x0004D15F
		public bool IsHaveUserPassword
		{
			get
			{
				return this.isHaveUserPassword;
			}
			set
			{
				if (!value)
				{
					this.IsHaveOwerPassword = false;
				}
				this.isHaveUserPassword = value;
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06001390 RID: 5008 RVA: 0x0004EF72 File Offset: 0x0004D172
		// (set) Token: 0x06001391 RID: 5009 RVA: 0x0004EF7A File Offset: 0x0004D17A
		public bool IsChangedPassword
		{
			get
			{
				return this.isChangedPassword;
			}
			set
			{
				this.isChangedPassword = value;
			}
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06001392 RID: 5010 RVA: 0x0004EF83 File Offset: 0x0004D183
		// (set) Token: 0x06001393 RID: 5011 RVA: 0x0004EF8B File Offset: 0x0004D18B
		public bool IsRequiredOwerPassword
		{
			get
			{
				return this.isRequiredOwerPassword;
			}
			set
			{
				this.isRequiredOwerPassword = value;
			}
		}

		// Token: 0x0400065A RID: 1626
		private bool isHaveUserPassword;

		// Token: 0x0400065B RID: 1627
		private bool isChangedPassword;

		// Token: 0x0400065C RID: 1628
		public bool IsHaveOwerPassword;

		// Token: 0x0400065D RID: 1629
		private bool isRequiredOwerPassword;
	}
}
