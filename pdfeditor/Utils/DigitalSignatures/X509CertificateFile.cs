using System;
using System.Security.Cryptography.X509Certificates;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DB RID: 219
	public class X509CertificateFile
	{
		// Token: 0x06000BE8 RID: 3048 RVA: 0x0003EDD7 File Offset: 0x0003CFD7
		internal X509CertificateFile(X509Certificate2 certificate, byte[] fileData, bool hasPrivateKey)
		{
			this.Certificate = certificate;
			this.FileData = fileData;
			this.HasPrivateKey = hasPrivateKey;
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x0003EDF4 File Offset: 0x0003CFF4
		public X509Certificate2 Certificate { get; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000BEA RID: 3050 RVA: 0x0003EDFC File Offset: 0x0003CFFC
		public byte[] FileData { get; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000BEB RID: 3051 RVA: 0x0003EE04 File Offset: 0x0003D004
		public bool HasPrivateKey { get; }
	}
}
