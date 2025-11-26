using System;
using System.Threading;

namespace FileWatcher
{
	// Token: 0x02000009 RID: 9
	public static class SingleInstance
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000022BC File Offset: 0x000004BC
		public static bool IsMainInstance
		{
			get
			{
				if (SingleInstance.isMainInstance == null)
				{
					object obj = SingleInstance.locker;
					lock (obj)
					{
						if (SingleInstance.isMainInstance == null)
						{
							bool flag2;
							SingleInstance.mutex = new Mutex(true, "CB79BC3B-4851-4371-BA8E-213000C3AE44", out flag2);
							SingleInstance.isMainInstance = new bool?(flag2);
							if (!flag2)
							{
								SingleInstance.mutex.Dispose();
								SingleInstance.mutex = null;
							}
						}
					}
				}
				return SingleInstance.isMainInstance.Value;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002348 File Offset: 0x00000548
		internal static void TryReleaseMutex()
		{
			Mutex mutex = SingleInstance.mutex;
			if (mutex != null)
			{
				mutex.Dispose();
			}
			SingleInstance.mutex = null;
			SingleInstance.isMainInstance = new bool?(false);
		}

		// Token: 0x0400001F RID: 31
		private const string MutexName = "CB79BC3B-4851-4371-BA8E-213000C3AE44";

		// Token: 0x04000020 RID: 32
		private static Mutex mutex;

		// Token: 0x04000021 RID: 33
		private static bool? isMainInstance;

		// Token: 0x04000022 RID: 34
		private static object locker = new object();
	}
}
