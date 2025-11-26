using System;
using System.Runtime.InteropServices;
using CommonLib.Common;

namespace FileWatcher
{
	// Token: 0x02000004 RID: 4
	public static class NotificationHelper
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020E0 File Offset: 0x000002E0
		public static bool AcceptsNotifications
		{
			get
			{
				UserNotificationState userNotificationState;
				return NotificationHelper.SHQueryUserNotificationState(out userNotificationState) == 0 && userNotificationState == UserNotificationState.AcceptsNotifications;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020FC File Offset: 0x000002FC
		public static bool IsFocusAssistSupported
		{
			get
			{
				return NotificationHelper.isFocusAssistSupportedLazy.Value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002108 File Offset: 0x00000308
		public static bool IsFocusAssistEnabled
		{
			get
			{
				FocusAssistResult focusAssist = NotificationHelper.FocusAssist;
				return focusAssist != FocusAssistResult.OFF && focusAssist != FocusAssistResult.NOT_SUPPORTED;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002128 File Offset: 0x00000328
		public static FocusAssistResult FocusAssist
		{
			get
			{
				if (NotificationHelper.IsFocusAssistSupported)
				{
					NotificationHelper.WNF_STATE_NAME wnf_STATE_NAME = new NotificationHelper.WNF_STATE_NAME(2747210869U, 226690622U);
					uint num = (uint)Marshal.SizeOf(typeof(IntPtr));
					IntPtr intPtr = IntPtr.Zero;
					try
					{
						intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NotificationHelper.WNF_STATE_NAME)));
						Marshal.StructureToPtr<NotificationHelper.WNF_STATE_NAME>(wnf_STATE_NAME, intPtr, false);
						uint num2;
						IntPtr intPtr2;
						if (NotificationHelper.NtQueryWnfStateData(intPtr, IntPtr.Zero, IntPtr.Zero, out num2, out intPtr2, ref num) == 0U)
						{
							return (FocusAssistResult)(int)intPtr2;
						}
						return FocusAssistResult.FAILED;
					}
					catch (Exception ex)
					{
						GAManager2.SendEvent("Exception", "FocusAssist", ex.Message, 1L);
						return FocusAssistResult.FAILED;
					}
					finally
					{
						if (intPtr != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(intPtr);
						}
					}
					return FocusAssistResult.NOT_SUPPORTED;
				}
				return FocusAssistResult.NOT_SUPPORTED;
			}
		}

		// Token: 0x06000008 RID: 8
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern uint NtQueryWnfStateData(IntPtr pStateName, IntPtr pTypeId, IntPtr pExplicitScope, out uint nChangeStamp, out IntPtr pBuffer, ref uint nBufferSize);

		// Token: 0x06000009 RID: 9
		[DllImport("shell32.dll")]
		private static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);

		// Token: 0x04000009 RID: 9
		private static readonly Lazy<bool> isFocusAssistSupportedLazy = new Lazy<bool>(() => Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(10, 0, 17134, 0), true);

		// Token: 0x02000019 RID: 25
		private struct WNF_TYPE_ID
		{
			// Token: 0x0400004F RID: 79
			public Guid TypeId;
		}

		// Token: 0x0200001A RID: 26
		private struct WNF_STATE_NAME
		{
			// Token: 0x0600007A RID: 122 RVA: 0x0000399C File Offset: 0x00001B9C
			public WNF_STATE_NAME(uint Data1, uint Data2)
			{
				this = default(NotificationHelper.WNF_STATE_NAME);
				this.Data = new uint[] { Data1, Data2 };
			}

			// Token: 0x04000050 RID: 80
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public uint[] Data;
		}
	}
}
