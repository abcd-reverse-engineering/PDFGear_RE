using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Nito.AsyncEx;
using Patagames.Pdf.Net;

namespace pdfeditor.Models.Operations
{
	// Token: 0x02000155 RID: 341
	public class OperationManager : ObservableObject, IDisposable
	{
		// Token: 0x0600144D RID: 5197 RVA: 0x00050A69 File Offset: 0x0004EC69
		public OperationManager(PdfDocument pdfDocument)
		{
			this.pdfDocument = pdfDocument;
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x0600144E RID: 5198 RVA: 0x00050AA0 File Offset: 0x0004ECA0
		// (set) Token: 0x0600144F RID: 5199 RVA: 0x00050AA8 File Offset: 0x0004ECA8
		public bool CanGoBack
		{
			get
			{
				return this.canGoBack;
			}
			private set
			{
				if (base.SetProperty<bool>(ref this.canGoBack, value, "CanGoBack"))
				{
					EventHandler stateChanged = this.StateChanged;
					if (stateChanged == null)
					{
						return;
					}
					stateChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06001450 RID: 5200 RVA: 0x00050AD4 File Offset: 0x0004ECD4
		// (set) Token: 0x06001451 RID: 5201 RVA: 0x00050ADC File Offset: 0x0004ECDC
		public bool CanGoForward
		{
			get
			{
				return this.canGoForward;
			}
			private set
			{
				if (base.SetProperty<bool>(ref this.canGoForward, value, "CanGoForward"))
				{
					EventHandler stateChanged = this.StateChanged;
					if (stateChanged == null)
					{
						return;
					}
					stateChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x00050B08 File Offset: 0x0004ED08
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x00050B10 File Offset: 0x0004ED10
		public string Version
		{
			get
			{
				return this.version;
			}
			private set
			{
				if (base.SetProperty<string>(ref this.version, value, "Version"))
				{
					EventHandler stateChanged = this.StateChanged;
					if (stateChanged == null)
					{
						return;
					}
					stateChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x00050B3C File Offset: 0x0004ED3C
		private async Task AddOperationCoreAsync(Func<PdfDocument, Task> goback, Func<PdfDocument, Task> goforward, string tag = "")
		{
			this.ThrowIfDisposed();
			using (await this.asyncLocker.LockAsync())
			{
				this.TryRemoveEndOfQueue();
				this.operations.Add(new OperationManager.OperationItem(goback, goforward, tag, this.scopeId));
				this.currentIndex = this.operations.Count - 1;
				this.UpdateCanState();
			}
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x00050B98 File Offset: 0x0004ED98
		public async Task AddOperationAsync(Func<PdfDocument, Task> goback, Func<PdfDocument, Task> goforward, string tag = "")
		{
			await this.AddOperationCoreAsync(goback, goforward, tag).ConfigureAwait(false);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x00050BF4 File Offset: 0x0004EDF4
		public async Task AddOperationAsync(Action<PdfDocument> goback, Func<PdfDocument, Task> goforward, string tag = "")
		{
			await this.AddOperationCoreAsync((goback != null) ? delegate(PdfDocument d)
			{
				goback(d);
				return Task.CompletedTask;
			} : null, goforward, tag).ConfigureAwait(false);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00050C50 File Offset: 0x0004EE50
		public async Task AddOperationAsync(Func<PdfDocument, Task> goback, Action<PdfDocument> goforward, string tag = "")
		{
			await this.AddOperationCoreAsync(goback, (goforward != null) ? delegate(PdfDocument d)
			{
				goforward(d);
				return Task.CompletedTask;
			} : null, tag).ConfigureAwait(false);
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x00050CAC File Offset: 0x0004EEAC
		public async Task AddOperationAsync(Action<PdfDocument> goback, Action<PdfDocument> goforward, string tag = "")
		{
			await this.AddOperationCoreAsync((goback != null) ? delegate(PdfDocument d)
			{
				goback(d);
				return Task.CompletedTask;
			} : null, (goforward != null) ? delegate(PdfDocument d)
			{
				goforward(d);
				return Task.CompletedTask;
			} : null, tag).ConfigureAwait(false);
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00050D08 File Offset: 0x0004EF08
		public async Task ClearAsync()
		{
			using (await this.asyncLocker.LockAsync().ConfigureAwait(false))
			{
				this.operations.Clear();
				this.currentIndex = -1;
				DispatcherHelper.UIDispatcher.Invoke(delegate
				{
					this.UpdateCanState();
				});
			}
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00050D4C File Offset: 0x0004EF4C
		public async Task GoBackAsync()
		{
			if (!this.CanGoBack)
			{
				throw new ArgumentException("CanGoBack");
			}
			EventHandler beforeOperationInvoked = OperationManager.BeforeOperationInvoked;
			if (beforeOperationInvoked != null)
			{
				beforeOperationInvoked(this, EventArgs.Empty);
			}
			try
			{
				IDisposable disposable = await this.asyncLocker.LockAsync();
				using (disposable)
				{
					OperationManager.OperationItem item = this.operations[this.currentIndex];
					int itemScopeId = item.ScopeId;
					do
					{
						await OperationManager.<GoBackAsync>g__RunCore|25_0(item.GoBack, this.pdfDocument);
						this.currentIndex--;
						if (this.currentIndex > 0)
						{
							item = this.operations[this.currentIndex];
						}
					}
					while (itemScopeId != 0 && this.currentIndex > 0 && itemScopeId == item.ScopeId);
					this.UpdateCanState();
					item = null;
				}
				IDisposable disposable2 = null;
			}
			finally
			{
				EventHandler afterOperationInvoked = OperationManager.AfterOperationInvoked;
				if (afterOperationInvoked != null)
				{
					afterOperationInvoked(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00050D90 File Offset: 0x0004EF90
		public async Task GoForwardAsync()
		{
			if (!this.CanGoForward)
			{
				throw new ArgumentException("CanGoForward");
			}
			EventHandler beforeOperationInvoked = OperationManager.BeforeOperationInvoked;
			if (beforeOperationInvoked != null)
			{
				beforeOperationInvoked(this, EventArgs.Empty);
			}
			try
			{
				IDisposable disposable = await this.asyncLocker.LockAsync();
				using (disposable)
				{
					OperationManager.OperationItem item = this.operations[this.currentIndex + 1];
					int itemScopeId = item.ScopeId;
					do
					{
						await OperationManager.<GoForwardAsync>g__RunCore|26_0(item.GoForward, this.pdfDocument);
						this.currentIndex++;
						if (this.currentIndex + 1 < this.operations.Count)
						{
							item = this.operations[this.currentIndex + 1];
						}
					}
					while (itemScopeId != 0 && this.currentIndex + 1 < this.operations.Count && itemScopeId == item.ScopeId);
					this.UpdateCanState();
					item = null;
				}
				IDisposable disposable2 = null;
			}
			finally
			{
				EventHandler afterOperationInvoked = OperationManager.AfterOperationInvoked;
				if (afterOperationInvoked != null)
				{
					afterOperationInvoked(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00050DD4 File Offset: 0x0004EFD4
		public async Task<IDisposable> CreateScopeAsync()
		{
			IDisposable disposable2;
			using (this.asyncLocker.Lock())
			{
				disposable2 = new OperationManager.OperationScope(this);
			}
			return disposable2;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x00050E18 File Offset: 0x0004F018
		public async Task<bool> ContainsTagAsync(string tag, OperationManagerEntryType type)
		{
			bool flag;
			if (string.IsNullOrEmpty(tag))
			{
				flag = false;
			}
			else
			{
				using (await this.asyncLocker.LockAsync())
				{
					if (type == OperationManagerEntryType.All)
					{
						flag = this.operations.Any((OperationManager.OperationItem c) => c.Tag == tag);
					}
					else if (type == OperationManagerEntryType.GoBack)
					{
						flag = this.operations.Take(this.currentIndex + 1).Any((OperationManager.OperationItem c) => c.Tag == tag);
					}
					else if (type == OperationManagerEntryType.GoForward)
					{
						flag = this.operations.Skip(this.currentIndex + 1).Any((OperationManager.OperationItem c) => c.Tag == tag);
					}
					else
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x00050E6C File Offset: 0x0004F06C
		private void UpdateCanState()
		{
			string empty = string.Empty;
			if (this.operations != null && this.currentIndex > -1)
			{
				empty = this.operations[this.currentIndex].Version;
			}
			if (base.SetProperty<bool>(ref this.canGoBack, this.currentIndex > -1, "CanGoBack") | base.SetProperty<bool>(ref this.canGoForward, this.operations != null && this.currentIndex < this.operations.Count - 1, "CanGoForward") | base.SetProperty<string>(ref this.version, empty, "Version"))
			{
				EventHandler stateChanged = this.StateChanged;
				if (stateChanged == null)
				{
					return;
				}
				stateChanged(this, EventArgs.Empty);
			}
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x0600145F RID: 5215 RVA: 0x00050F20 File Offset: 0x0004F120
		// (remove) Token: 0x06001460 RID: 5216 RVA: 0x00050F58 File Offset: 0x0004F158
		public event EventHandler StateChanged;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001461 RID: 5217 RVA: 0x00050F90 File Offset: 0x0004F190
		// (remove) Token: 0x06001462 RID: 5218 RVA: 0x00050FC4 File Offset: 0x0004F1C4
		public static event EventHandler BeforeOperationInvoked;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001463 RID: 5219 RVA: 0x00050FF8 File Offset: 0x0004F1F8
		// (remove) Token: 0x06001464 RID: 5220 RVA: 0x0005102C File Offset: 0x0004F22C
		public static event EventHandler AfterOperationInvoked;

		// Token: 0x06001465 RID: 5221 RVA: 0x0005105F File Offset: 0x0004F25F
		private void TryRemoveEndOfQueue()
		{
			if (this.currentIndex >= this.operations.Count - 1)
			{
				return;
			}
			this.operations.RemoveRange(this.currentIndex + 1, this.operations.Count - this.currentIndex - 1);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0005109E File Offset: 0x0004F29E
		private void ThrowIfDisposed()
		{
			if (this.disposedValue)
			{
				throw new ObjectDisposedException("OperationManager");
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000510B3 File Offset: 0x0004F2B3
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					this.pdfDocument = null;
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x000510CE File Offset: 0x0004F2CE
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x000510E8 File Offset: 0x0004F2E8
		[CompilerGenerated]
		internal static async Task <GoBackAsync>g__RunCore|25_0(Func<PdfDocument, Task> func, PdfDocument doc)
		{
			await func(doc).ConfigureAwait(false);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00051134 File Offset: 0x0004F334
		[CompilerGenerated]
		internal static async Task <GoForwardAsync>g__RunCore|26_0(Func<PdfDocument, Task> func, PdfDocument doc)
		{
			await func(doc).ConfigureAwait(false);
		}

		// Token: 0x040006BE RID: 1726
		private bool disposedValue;

		// Token: 0x040006BF RID: 1727
		private PdfDocument pdfDocument;

		// Token: 0x040006C0 RID: 1728
		private List<OperationManager.OperationItem> operations = new List<OperationManager.OperationItem>();

		// Token: 0x040006C1 RID: 1729
		private int currentIndex = -1;

		// Token: 0x040006C2 RID: 1730
		private bool canGoBack;

		// Token: 0x040006C3 RID: 1731
		private bool canGoForward;

		// Token: 0x040006C4 RID: 1732
		private string version = string.Empty;

		// Token: 0x040006C5 RID: 1733
		private int scopeId;

		// Token: 0x040006C6 RID: 1734
		private AsyncLock asyncLocker = new AsyncLock();

		// Token: 0x0200056C RID: 1388
		private sealed class OperationScope : IDisposable
		{
			// Token: 0x06003115 RID: 12565 RVA: 0x000F1172 File Offset: 0x000EF372
			public OperationScope(OperationManager manager)
			{
				this.cachedScopeId = manager.scopeId;
				manager.scopeId = this.cachedScopeId + 1;
				this.manager = manager;
			}

			// Token: 0x06003116 RID: 12566 RVA: 0x000F119C File Offset: 0x000EF39C
			public void Dispose()
			{
				if (!this.disposedValue)
				{
					this.disposedValue = true;
					int num = this.manager.scopeId - 1;
					if (num != this.cachedScopeId)
					{
						throw new Exception("OperationScope");
					}
					this.manager.scopeId = num;
				}
			}

			// Token: 0x04001DCB RID: 7627
			private readonly OperationManager manager;

			// Token: 0x04001DCC RID: 7628
			private bool disposedValue;

			// Token: 0x04001DCD RID: 7629
			private int cachedScopeId;

			// Token: 0x04001DCE RID: 7630
			private int scopeId;
		}

		// Token: 0x0200056D RID: 1389
		public class OperationItem
		{
			// Token: 0x06003117 RID: 12567 RVA: 0x000F11E8 File Offset: 0x000EF3E8
			public OperationItem(Func<PdfDocument, Task> goback, Func<PdfDocument, Task> goforward, string tag, int scopeId)
			{
				this.GoBack = goback;
				this.GoForward = goforward;
				this.Tag = tag;
				this.Version = Guid.NewGuid().ToString();
				this.ScopeId = scopeId;
			}

			// Token: 0x17000D17 RID: 3351
			// (get) Token: 0x06003118 RID: 12568 RVA: 0x000F1231 File Offset: 0x000EF431
			public string Version { get; }

			// Token: 0x17000D18 RID: 3352
			// (get) Token: 0x06003119 RID: 12569 RVA: 0x000F1239 File Offset: 0x000EF439
			public string Tag { get; }

			// Token: 0x17000D19 RID: 3353
			// (get) Token: 0x0600311A RID: 12570 RVA: 0x000F1241 File Offset: 0x000EF441
			public int ScopeId { get; }

			// Token: 0x17000D1A RID: 3354
			// (get) Token: 0x0600311B RID: 12571 RVA: 0x000F1249 File Offset: 0x000EF449
			public Func<PdfDocument, Task> GoBack { get; }

			// Token: 0x17000D1B RID: 3355
			// (get) Token: 0x0600311C RID: 12572 RVA: 0x000F1251 File Offset: 0x000EF451
			public Func<PdfDocument, Task> GoForward { get; }

			// Token: 0x0600311D RID: 12573 RVA: 0x000F1259 File Offset: 0x000EF459
			public void Deconstruct(out Func<PdfDocument, Task> goback, out Func<PdfDocument, Task> goforward)
			{
				goback = this.GoBack;
				goforward = this.GoForward;
			}
		}
	}
}
