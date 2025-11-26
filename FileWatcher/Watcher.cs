using System;
using System.Collections.Generic;
using System.IO;

namespace FileWatcher
{
	// Token: 0x0200000A RID: 10
	public class Watcher : IDisposable
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002378 File Offset: 0x00000578
		public bool AddPath(string path, string filter = "*.*", bool includeSubdirectories = false)
		{
			this.ThrowIfDisposed();
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			object obj = this.locker;
			lock (obj)
			{
				this.RemovePath(path);
				FileSystemWatcher fileSystemWatcher = null;
				string[] array = filter.Split(new char[] { '|' });
				if (array.Length == 0)
				{
					array = new string[] { "" };
				}
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						fileSystemWatcher = new FileSystemWatcher(path, array[i]);
						fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
						fileSystemWatcher.Created += this.Watcher_Created;
						fileSystemWatcher.Renamed += this.Watcher_Renamed;
						fileSystemWatcher.IncludeSubdirectories = includeSubdirectories;
						fileSystemWatcher.EnableRaisingEvents = true;
						this.watchers.Add(fileSystemWatcher);
						return true;
					}
					catch
					{
						if (fileSystemWatcher != null)
						{
							fileSystemWatcher.Dispose();
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002474 File Offset: 0x00000674
		public void RemovePath(string path)
		{
			this.ThrowIfDisposed();
			object obj = this.locker;
			lock (obj)
			{
				for (int i = this.watchers.Count - 1; i >= 0; i--)
				{
					if (this.watchers[i].Path == path)
					{
						try
						{
							this.watchers[i].Created -= this.Watcher_Created;
							this.watchers[i].Renamed -= this.Watcher_Renamed;
							this.watchers[i].Dispose();
						}
						catch
						{
						}
						this.watchers.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002550 File Offset: 0x00000750
		public void Clear()
		{
			this.ThrowIfDisposed();
			object obj = this.locker;
			lock (obj)
			{
				for (int i = 0; i < this.watchers.Count; i++)
				{
					try
					{
						this.watchers[i].Created -= this.Watcher_Created;
						this.watchers[i].Renamed -= this.Watcher_Renamed;
						this.watchers[i].Dispose();
					}
					catch
					{
					}
				}
				this.watchers.Clear();
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002610 File Offset: 0x00000810
		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			WatcherFileCreatedEventHandler fileCreated = this.FileCreated;
			if (fileCreated != null)
			{
				string path = ((FileSystemWatcher)sender).Path;
				string fullPath = e.FullPath;
				fileCreated(this, new WatcherFileCreatedEventArgs(path, fullPath));
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002648 File Offset: 0x00000848
		private void Watcher_Renamed(object sender, RenamedEventArgs e)
		{
			WatcherFileRenamedEventHandler fileRenamed = this.FileRenamed;
			if (fileRenamed != null)
			{
				string path = ((FileSystemWatcher)sender).Path;
				if (fileRenamed != null)
				{
					fileRenamed(this, new WatcherFileRenamedEventArgs(path, e.OldFullPath, e.FullPath));
				}
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000019 RID: 25 RVA: 0x00002688 File Offset: 0x00000888
		// (remove) Token: 0x0600001A RID: 26 RVA: 0x000026C0 File Offset: 0x000008C0
		public event WatcherFileCreatedEventHandler FileCreated;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600001B RID: 27 RVA: 0x000026F8 File Offset: 0x000008F8
		// (remove) Token: 0x0600001C RID: 28 RVA: 0x00002730 File Offset: 0x00000930
		public event WatcherFileRenamedEventHandler FileRenamed;

		// Token: 0x0600001D RID: 29 RVA: 0x00002765 File Offset: 0x00000965
		private void ThrowIfDisposed()
		{
			if (this.disposedValue)
			{
				throw new ObjectDisposedException("Watcher");
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000277C File Offset: 0x0000097C
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				object obj = this.locker;
				lock (obj)
				{
					for (int i = 0; i < this.watchers.Count; i++)
					{
						try
						{
							this.watchers[i].Created -= this.Watcher_Created;
							this.watchers[i].Renamed -= this.Watcher_Renamed;
							this.watchers[i].Dispose();
						}
						catch
						{
						}
					}
					this.watchers.Clear();
					this.watchers = null;
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002850 File Offset: 0x00000A50
		~Watcher()
		{
			this.Dispose(false);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002880 File Offset: 0x00000A80
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000023 RID: 35
		private bool disposedValue;

		// Token: 0x04000024 RID: 36
		private object locker = new object();

		// Token: 0x04000025 RID: 37
		private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
	}
}
