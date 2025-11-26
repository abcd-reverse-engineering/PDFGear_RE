using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DC RID: 220
	internal class CertificateStore : IDisposable
	{
		// Token: 0x06000BEC RID: 3052 RVA: 0x0003EE0C File Offset: 0x0003D00C
		private CertificateStore(StoreLocation location, params StoreName[] storeNames)
		{
			if (storeNames == null || storeNames.Length == 0)
			{
				throw new ArgumentException(null, "storeNames");
			}
			this.stores = new List<X509Store>();
			for (int i = 0; i < storeNames.Length; i++)
			{
				try
				{
					this.stores.Add(new X509Store(storeNames[i], location));
				}
				catch
				{
				}
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000BED RID: 3053 RVA: 0x0003EE78 File Offset: 0x0003D078
		public IReadOnlyList<X509Certificate2> Certificates
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.certs == null)
				{
					List<X509Store> list = this.stores;
					lock (list)
					{
						if (this.certs == null)
						{
							this.EnsureStoresOpen();
							List<X509Certificate2> list2 = new List<X509Certificate2>();
							HashSet<string> hashSet = new HashSet<string>();
							for (int i = 0; i < this.openedStores.Count; i++)
							{
								try
								{
									X509Certificate2[] array = this.openedStores[i].Certificates.OfType<X509Certificate2>().ToArray<X509Certificate2>();
									for (int j = 0; j < array.Length; j++)
									{
										try
										{
											if (hashSet.Add(array[j].Thumbprint))
											{
												list2.Add(array[j]);
											}
										}
										catch
										{
										}
									}
								}
								catch
								{
								}
							}
							this.certs = list2;
						}
					}
				}
				return this.certs;
			}
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0003EF7C File Offset: 0x0003D17C
		private void EnsureStoresOpen()
		{
			this.ThrowIfDisposed();
			if (this.openedStores != null)
			{
				return;
			}
			List<X509Store> list = this.stores;
			lock (list)
			{
				if (this.openedStores == null)
				{
					this.openedStores = new List<X509Store>();
					for (int i = 0; i < this.stores.Count; i++)
					{
						try
						{
							this.stores[i].Open(OpenFlags.ReadOnly);
							this.openedStores.Add(this.stores[i]);
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0003F02C File Offset: 0x0003D22C
		public X509CertificateCollection ToCertificateCollection()
		{
			this.ThrowIfDisposed();
			X509Certificate[] array = this.Certificates.ToArray<X509Certificate2>();
			return new X509CertificateCollection(array);
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0003F051 File Offset: 0x0003D251
		private void ThrowIfDisposed()
		{
			if (this.disposedValue)
			{
				throw new ObjectDisposedException("CertificateStore");
			}
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0003F068 File Offset: 0x0003D268
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					List<X509Store> list = this.stores;
					lock (list)
					{
						if (this.certs != null)
						{
							for (int i = 0; i < this.certs.Count; i++)
							{
								this.certs[i].Dispose();
							}
							this.certs = null;
						}
						if (this.openedStores != null)
						{
							for (int j = 0; j < this.openedStores.Count; j++)
							{
								this.openedStores[j].Dispose();
							}
							this.openedStores = null;
						}
					}
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0003F128 File Offset: 0x0003D328
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0003F137 File Offset: 0x0003D337
		public static CertificateStore CreateTrustedStore()
		{
			return new CertificateStore(StoreLocation.CurrentUser, new StoreName[]
			{
				StoreName.TrustedPublisher,
				StoreName.TrustedPeople,
				StoreName.Root
			});
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0003F150 File Offset: 0x0003D350
		public static CertificateStore CreateRootStore()
		{
			return new CertificateStore(StoreLocation.CurrentUser, new StoreName[] { StoreName.Root });
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0003F162 File Offset: 0x0003D362
		public static CertificateStore CreateUserStore()
		{
			return new CertificateStore(StoreLocation.CurrentUser, new StoreName[] { StoreName.My });
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0003F174 File Offset: 0x0003D374
		public static CertificateStore CreateStore(StoreLocation location, params StoreName[] storeNames)
		{
			return new CertificateStore(location, storeNames);
		}

		// Token: 0x0400057C RID: 1404
		public const bool IsWindowsCertificateStoreEnabled = true;

		// Token: 0x0400057D RID: 1405
		private bool disposedValue;

		// Token: 0x0400057E RID: 1406
		private List<X509Store> stores;

		// Token: 0x0400057F RID: 1407
		private List<X509Store> openedStores;

		// Token: 0x04000580 RID: 1408
		private IReadOnlyList<X509Certificate2> certs;
	}
}
