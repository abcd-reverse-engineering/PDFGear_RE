using System;
using System.Collections.Generic;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DA RID: 218
	public interface ICertificateStorage
	{
		// Token: 0x06000BE3 RID: 3043
		X509CertificateFile GetCertificate(string thumbprint);

		// Token: 0x06000BE4 RID: 3044
		bool SaveCertificate(byte[] data, string password);

		// Token: 0x06000BE5 RID: 3045
		bool DeleteCertificate(string thumbprint);

		// Token: 0x06000BE6 RID: 3046
		IReadOnlyList<X509CertificateFile> GetAllCertificates();

		// Token: 0x06000BE7 RID: 3047
		bool DeleteAllCertificates();
	}
}
