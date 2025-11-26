using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;

namespace pdfeditor.Utils.Printer
{
	// Token: 0x020000C8 RID: 200
	public static class PrinterDevModeHelper
	{
		// Token: 0x06000B9C RID: 2972 RVA: 0x0003DAD8 File Offset: 0x0003BCD8
		public static void SetHdevmode(this PrinterSettings settings, PrintDevModeHandle devmodeHandle)
		{
			if (settings == null || devmodeHandle == null)
			{
				return;
			}
			bool flag = false;
			try
			{
				devmodeHandle.DangerousAddRef(ref flag);
				settings.DefaultPageSettings.SetHdevmode(devmodeHandle.DangerousGetHandle());
				settings.SetHdevmode(devmodeHandle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					devmodeHandle.DangerousRelease();
				}
			}
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0003DB30 File Offset: 0x0003BD30
		public static PrintDevModeHandle OpenPrinterConfigure(Window window, PrinterSettings settings)
		{
			if (window == null || settings == null)
			{
				return new PrintDevModeHandle(IntPtr.Zero);
			}
			IntPtr handle = new WindowInteropHelper(window).Handle;
			if (handle == IntPtr.Zero)
			{
				return new PrintDevModeHandle(IntPtr.Zero);
			}
			return PrinterDevModeHelper.OpenPrinterConfigure(handle, settings);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0003DB7C File Offset: 0x0003BD7C
		public static PrintDevModeHandle OpenPrinterConfigure(IntPtr parentWindow, PrinterSettings settings)
		{
			if (parentWindow == IntPtr.Zero || settings == null)
			{
				return new PrintDevModeHandle(IntPtr.Zero);
			}
			IntPtr hdevmode = settings.GetHdevmode();
			if (hdevmode != IntPtr.Zero)
			{
				PrintDevModeHandle printDevModeHandle = new PrintDevModeHandle(hdevmode);
				try
				{
					PrintDevModeHandle documentProperties = PrinterDevModeHelper.GetDocumentProperties(parentWindow, settings.PrinterName, hdevmode);
					if (documentProperties != null)
					{
						settings.SetHdevmode(documentProperties);
						return documentProperties;
					}
				}
				finally
				{
					printDevModeHandle.Dispose();
				}
			}
			return new PrintDevModeHandle(IntPtr.Zero);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0003DC00 File Offset: 0x0003BE00
		private static PrintDevModeHandle GetDocumentProperties(IntPtr parentWindow, string printerName, IntPtr hDevModeInput)
		{
			PrinterDevModeHelper.PRINTER_DEFAULTS printer_DEFAULTS = default(PrinterDevModeHelper.PRINTER_DEFAULTS);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				if (PrinterDevModeHelper.OpenPrinter(printerName, out intPtr, ref printer_DEFAULTS))
				{
					if (!(hDevModeInput != IntPtr.Zero))
					{
						goto IL_0088;
					}
					try
					{
						IntPtr intPtr2 = PrinterDevModeHelper.GlobalLock(hDevModeInput);
						int num = PrinterDevModeHelper.DocumentProperties(IntPtr.Zero, intPtr, printerName, IntPtr.Zero, IntPtr.Zero, 0);
						if (num > 0)
						{
							IntPtr intPtr3 = Marshal.AllocHGlobal(num);
							int num2 = 14;
							if (PrinterDevModeHelper.DocumentProperties(parentWindow, intPtr, printerName, intPtr3, intPtr2, num2) == 1)
							{
								return new PrintDevModeHandle(intPtr3);
							}
							Marshal.FreeHGlobal(intPtr3);
						}
						goto IL_009F;
					}
					finally
					{
						PrinterDevModeHelper.GlobalUnlock(hDevModeInput);
					}
				}
				intPtr = IntPtr.Zero;
				IL_0088:;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					PrinterDevModeHelper.ClosePrinter(intPtr);
				}
			}
			IL_009F:
			return null;
		}

		// Token: 0x06000BA0 RID: 2976
		[SuppressUnmanagedCodeSecurity]
		[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPTStr)] string printerName, out IntPtr phPrinter, ref PrinterDevModeHelper.PRINTER_DEFAULTS pd);

		// Token: 0x06000BA1 RID: 2977
		[SuppressUnmanagedCodeSecurity]
		[DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool ClosePrinter(IntPtr phPrinter);

		// Token: 0x06000BA2 RID: 2978
		[DllImport("winspool.drv")]
		private static extern int PrinterProperties(IntPtr hwnd, IntPtr hPrinter);

		// Token: 0x06000BA3 RID: 2979
		[DllImport("winspool.drv", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DocumentPropertiesW", SetLastError = true)]
		private static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

		// Token: 0x06000BA4 RID: 2980
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GlobalLock(IntPtr handle);

		// Token: 0x06000BA5 RID: 2981
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		private static extern bool GlobalUnlock(IntPtr handle);

		// Token: 0x0400051B RID: 1307
		private const int IDOK = 1;

		// Token: 0x0400051C RID: 1308
		private const int IDCANCEL = 2;

		// Token: 0x020004E2 RID: 1250
		private struct PRINTER_DEFAULTS
		{
			// Token: 0x04001B2D RID: 6957
			public IntPtr pDatatype;

			// Token: 0x04001B2E RID: 6958
			public IntPtr pDevMode;

			// Token: 0x04001B2F RID: 6959
			public int DesiredAccess;
		}

		// Token: 0x020004E3 RID: 1251
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private class DEVMODE
		{
			// Token: 0x06002F12 RID: 12050 RVA: 0x000E731C File Offset: 0x000E551C
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"[DEVMODE: dmDeviceName=",
					this.dmDeviceName,
					", dmSpecVersion=",
					this.dmSpecVersion.ToString(),
					", dmDriverVersion=",
					this.dmDriverVersion.ToString(),
					", dmSize=",
					this.dmSize.ToString(),
					", dmDriverExtra=",
					this.dmDriverExtra.ToString(),
					", dmFields=",
					this.dmFields.ToString(),
					", dmOrientation=",
					this.dmOrientation.ToString(),
					", dmPaperSize=",
					this.dmPaperSize.ToString(),
					", dmPaperLength=",
					this.dmPaperLength.ToString(),
					", dmPaperWidth=",
					this.dmPaperWidth.ToString(),
					", dmScale=",
					this.dmScale.ToString(),
					", dmCopies=",
					this.dmCopies.ToString(),
					", dmDefaultSource=",
					this.dmDefaultSource.ToString(),
					", dmPrintQuality=",
					this.dmPrintQuality.ToString(),
					", dmColor=",
					this.dmColor.ToString(),
					", dmDuplex=",
					this.dmDuplex.ToString(),
					", dmYResolution=",
					this.dmYResolution.ToString(),
					", dmTTOption=",
					this.dmTTOption.ToString(),
					", dmCollate=",
					this.dmCollate.ToString(),
					", dmFormName=",
					this.dmFormName,
					", dmLogPixels=",
					this.dmLogPixels.ToString(),
					", dmBitsPerPel=",
					this.dmBitsPerPel.ToString(),
					", dmPelsWidth=",
					this.dmPelsWidth.ToString(),
					", dmPelsHeight=",
					this.dmPelsHeight.ToString(),
					", dmDisplayFlags=",
					this.dmDisplayFlags.ToString(),
					", dmDisplayFrequency=",
					this.dmDisplayFrequency.ToString(),
					", dmICMMethod=",
					this.dmICMMethod.ToString(),
					", dmICMIntent=",
					this.dmICMIntent.ToString(),
					", dmMediaType=",
					this.dmMediaType.ToString(),
					", dmDitherType=",
					this.dmDitherType.ToString(),
					", dmICCManufacturer=",
					this.dmICCManufacturer.ToString(),
					", dmICCModel=",
					this.dmICCModel.ToString(),
					", dmPanningWidth=",
					this.dmPanningWidth.ToString(),
					", dmPanningHeight=",
					this.dmPanningHeight.ToString(),
					"]"
				});
			}

			// Token: 0x04001B30 RID: 6960
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmDeviceName;

			// Token: 0x04001B31 RID: 6961
			public short dmSpecVersion;

			// Token: 0x04001B32 RID: 6962
			public short dmDriverVersion;

			// Token: 0x04001B33 RID: 6963
			public short dmSize;

			// Token: 0x04001B34 RID: 6964
			public short dmDriverExtra;

			// Token: 0x04001B35 RID: 6965
			public int dmFields;

			// Token: 0x04001B36 RID: 6966
			public short dmOrientation;

			// Token: 0x04001B37 RID: 6967
			public short dmPaperSize;

			// Token: 0x04001B38 RID: 6968
			public short dmPaperLength;

			// Token: 0x04001B39 RID: 6969
			public short dmPaperWidth;

			// Token: 0x04001B3A RID: 6970
			public short dmScale;

			// Token: 0x04001B3B RID: 6971
			public short dmCopies;

			// Token: 0x04001B3C RID: 6972
			public short dmDefaultSource;

			// Token: 0x04001B3D RID: 6973
			public short dmPrintQuality;

			// Token: 0x04001B3E RID: 6974
			public short dmColor;

			// Token: 0x04001B3F RID: 6975
			public short dmDuplex;

			// Token: 0x04001B40 RID: 6976
			public short dmYResolution;

			// Token: 0x04001B41 RID: 6977
			public short dmTTOption;

			// Token: 0x04001B42 RID: 6978
			public short dmCollate;

			// Token: 0x04001B43 RID: 6979
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;

			// Token: 0x04001B44 RID: 6980
			public short dmLogPixels;

			// Token: 0x04001B45 RID: 6981
			public int dmBitsPerPel;

			// Token: 0x04001B46 RID: 6982
			public int dmPelsWidth;

			// Token: 0x04001B47 RID: 6983
			public int dmPelsHeight;

			// Token: 0x04001B48 RID: 6984
			public int dmDisplayFlags;

			// Token: 0x04001B49 RID: 6985
			public int dmDisplayFrequency;

			// Token: 0x04001B4A RID: 6986
			public int dmICMMethod;

			// Token: 0x04001B4B RID: 6987
			public int dmICMIntent;

			// Token: 0x04001B4C RID: 6988
			public int dmMediaType;

			// Token: 0x04001B4D RID: 6989
			public int dmDitherType;

			// Token: 0x04001B4E RID: 6990
			public int dmICCManufacturer;

			// Token: 0x04001B4F RID: 6991
			public int dmICCModel;

			// Token: 0x04001B50 RID: 6992
			public int dmPanningWidth;

			// Token: 0x04001B51 RID: 6993
			public int dmPanningHeight;
		}

		// Token: 0x020004E4 RID: 1252
		private enum DM
		{
			// Token: 0x04001B53 RID: 6995
			DMDUP_UNKNOWN,
			// Token: 0x04001B54 RID: 6996
			DMDUP_SIMPLEX,
			// Token: 0x04001B55 RID: 6997
			DMDUP_VERTICAL,
			// Token: 0x04001B56 RID: 6998
			DMDUP_HORIZONTAL
		}

		// Token: 0x020004E5 RID: 1253
		[Flags]
		private enum DocumentPropertiesMode
		{
			// Token: 0x04001B58 RID: 7000
			DM_OUT_BUFFER = 2,
			// Token: 0x04001B59 RID: 7001
			DM_IN_PROMPT = 4,
			// Token: 0x04001B5A RID: 7002
			DM_IN_BUFFER = 8
		}

		// Token: 0x020004E6 RID: 1254
		private struct POINTL
		{
			// Token: 0x04001B5B RID: 7003
			public int x;

			// Token: 0x04001B5C RID: 7004
			public int y;
		}
	}
}
