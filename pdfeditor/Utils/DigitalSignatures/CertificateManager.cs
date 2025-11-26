using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CommonLib.Config;
using Microsoft.Data.Sqlite;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000D9 RID: 217
	public static class CertificateManager
	{
		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x0003EDB3 File Offset: 0x0003CFB3
		public static ICertificateStorage SignatureCertificateStorage
		{
			get
			{
				return CertificateManager.signCertStorage;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x0003EDBA File Offset: 0x0003CFBA
		public static ICertificateStorage VerificationCertificateStorage
		{
			get
			{
				return CertificateManager.validCertStorage;
			}
		}

		// Token: 0x04000576 RID: 1398
		public const bool IsWindowsCertificateStoreEnabled = true;

		// Token: 0x04000577 RID: 1399
		private static readonly ICertificateStorage signCertStorage = new CertificateManager.SignCertStorage();

		// Token: 0x04000578 RID: 1400
		private static readonly ICertificateStorage validCertStorage = new CertificateManager.ValidCertStorage();

		// Token: 0x020004F4 RID: 1268
		private class SignCertStorage : CertificateManager.BaseCertificateStorage
		{
			// Token: 0x06002F3E RID: 12094 RVA: 0x000E8D2F File Offset: 0x000E6F2F
			public SignCertStorage()
				: base("sign_certs")
			{
			}

			// Token: 0x06002F3F RID: 12095 RVA: 0x000E8D3C File Offset: 0x000E6F3C
			protected override bool SaveCertificateCore(X509Certificate2 certificate)
			{
				return SignatureValidateHelper.IsSupportedSignCertificate(certificate) && base.SaveCertificateCore(certificate);
			}
		}

		// Token: 0x020004F5 RID: 1269
		private class ValidCertStorage : CertificateManager.BaseCertificateStorage
		{
			// Token: 0x06002F40 RID: 12096 RVA: 0x000E8D4F File Offset: 0x000E6F4F
			public ValidCertStorage()
				: base("valid_certs")
			{
			}

			// Token: 0x06002F41 RID: 12097 RVA: 0x000E8D5C File Offset: 0x000E6F5C
			protected override bool SaveCertificateCore(X509Certificate2 certificate)
			{
				return SignatureValidateHelper.IsSupportedValidCertificate(certificate) && base.SaveCertificateCore(certificate);
			}
		}

		// Token: 0x020004F6 RID: 1270
		private class BaseCertificateStorage : ICertificateStorage
		{
			// Token: 0x06002F42 RID: 12098 RVA: 0x000E8D6F File Offset: 0x000E6F6F
			public BaseCertificateStorage(string tableName)
			{
				this.tableName = tableName;
			}

			// Token: 0x06002F43 RID: 12099 RVA: 0x000E8D80 File Offset: 0x000E6F80
			public bool DeleteAllCertificates()
			{
				try
				{
					using (SqliteConnection connection = this.GetConnection())
					{
						connection.Open();
						new SqliteCommand("DELETE FROM " + this.tableName + ";", connection).ExecuteNonQuery();
						return true;
					}
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06002F44 RID: 12100 RVA: 0x000E8DEC File Offset: 0x000E6FEC
			public bool DeleteCertificate(string thumbprint)
			{
				if (string.IsNullOrEmpty(thumbprint))
				{
					return false;
				}
				try
				{
					using (SqliteConnection connection = this.GetConnection())
					{
						connection.Open();
						SqliteCommand sqliteCommand = new SqliteCommand("DELETE FROM " + this.tableName + " WHERE thumbprint=@thumbprint;", connection);
						sqliteCommand.Parameters.Add("@thumbprint", SqliteType.Text).Value = thumbprint;
						return sqliteCommand.ExecuteNonQuery() > 0;
					}
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06002F45 RID: 12101 RVA: 0x000E8E7C File Offset: 0x000E707C
			public IReadOnlyList<X509CertificateFile> GetAllCertificates()
			{
				try
				{
					using (SqliteConnection connection = this.GetConnection())
					{
						connection.Open();
						using (SqliteDataReader sqliteDataReader = new SqliteCommand("SELECT raw_data, public_data FROM " + this.tableName + ";", connection).ExecuteReader())
						{
							List<X509CertificateFile> list = null;
							while (sqliteDataReader.Read())
							{
								byte[] array = (byte[])sqliteDataReader["raw_data"];
								byte[] array2 = (byte[])sqliteDataReader["public_data"];
								X509CertificateFile x509CertificateFile = this.CreateCertificate(array2, array);
								if (x509CertificateFile != null)
								{
									if (list == null)
									{
										list = new List<X509CertificateFile>();
									}
									list.Add(x509CertificateFile);
								}
							}
							IReadOnlyList<X509CertificateFile> readOnlyList = list;
							return readOnlyList ?? Array.Empty<X509CertificateFile>();
						}
					}
				}
				catch
				{
				}
				return Array.Empty<X509CertificateFile>();
			}

			// Token: 0x06002F46 RID: 12102 RVA: 0x000E8F60 File Offset: 0x000E7160
			public X509CertificateFile GetCertificate(string thumbprint)
			{
				if (string.IsNullOrEmpty(thumbprint))
				{
					return null;
				}
				try
				{
					using (SqliteConnection connection = this.GetConnection())
					{
						connection.Open();
						SqliteCommand sqliteCommand = new SqliteCommand("SELECT raw_data, public_data FROM " + this.tableName + " WHERE thumbprint=@thumbprint;", connection);
						sqliteCommand.Parameters.Add("@thumbprint", SqliteType.Text).Value = thumbprint;
						using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
						{
							List<X509Certificate2> list = null;
							if (sqliteDataReader.Read())
							{
								if (list == null)
								{
									list = new List<X509Certificate2>();
								}
								byte[] array = (byte[])sqliteDataReader["raw_data"];
								byte[] array2 = (byte[])sqliteDataReader["public_data"];
								return this.CreateCertificate(array2, array);
							}
						}
					}
				}
				catch
				{
				}
				return null;
			}

			// Token: 0x06002F47 RID: 12103 RVA: 0x000E904C File Offset: 0x000E724C
			public bool SaveCertificate(byte[] data, string password)
			{
				if (data == null)
				{
					return false;
				}
				try
				{
					X509Certificate2 x509Certificate = new X509Certificate2(data, password, X509KeyStorageFlags.Exportable);
					if (!this.SaveCertificateCore(x509Certificate))
					{
						return false;
					}
					byte[] array = x509Certificate.Export(X509ContentType.Cert);
					using (SqliteConnection connection = this.GetConnection())
					{
						connection.Open();
						SqliteCommand sqliteCommand = new SqliteCommand("INSERT OR IGNORE INTO " + this.tableName + " (thumbprint, raw_data, public_data) VALUES (@thumbprint, @rawData, @publicData);", connection);
						sqliteCommand.Parameters.Add("@thumbprint", SqliteType.Text).Value = x509Certificate.Thumbprint;
						sqliteCommand.Parameters.Add("@rawData", SqliteType.Blob).Value = data;
						sqliteCommand.Parameters.Add("@publicData", SqliteType.Blob).Value = array;
						return sqliteCommand.ExecuteNonQuery() >= 0;
					}
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06002F48 RID: 12104 RVA: 0x000E912C File Offset: 0x000E732C
			protected virtual X509CertificateFile CreateCertificate(byte[] publicData, byte[] rawData)
			{
				try
				{
					return new X509CertificateFile(new X509Certificate2(publicData), rawData, false);
				}
				catch
				{
				}
				return null;
			}

			// Token: 0x06002F49 RID: 12105 RVA: 0x000E9160 File Offset: 0x000E7360
			protected virtual bool SaveCertificateCore(X509Certificate2 certificate)
			{
				return certificate != null;
			}

			// Token: 0x06002F4A RID: 12106 RVA: 0x000E9168 File Offset: 0x000E7368
			private SqliteConnection GetConnection()
			{
				if (!this.tableCreated)
				{
					using (SqliteConnection connection = SqliteUtils.GetConnection())
					{
						connection.Open();
						new SqliteCommand(string.Concat(new string[] { "CREATE TABLE IF NOT EXISTS ", this.tableName, " (id INTEGER PRIMARY KEY NOT NULL, thumbprint NVARCHAR(50) NOT NULL, raw_data BLOB NOT NULL, public_data BLOB NOT NULL);\nCREATE UNIQUE INDEX IF NOT EXISTS IDX_", this.tableName, "_thumbprint ON ", this.tableName, " (thumbprint);" }), connection).ExecuteNonQuery();
					}
				}
				return SqliteUtils.GetConnection();
			}

			// Token: 0x04001BB9 RID: 7097
			private readonly string tableName;

			// Token: 0x04001BBA RID: 7098
			private bool tableCreated;
		}
	}
}
