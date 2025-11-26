using System;
using System.Collections.Generic;
using pdfeditor.Properties;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000148 RID: 328
	public class WiaErrorCodes
	{
		// Token: 0x0600137E RID: 4990 RVA: 0x0004ECE9 File Offset: 0x0004CEE9
		public static string GetMessage(uint code)
		{
			if (WiaErrorCodes.allErrors.ContainsKey(code))
			{
				return WiaErrorCodes.allErrors[code];
			}
			return null;
		}

		// Token: 0x04000656 RID: 1622
		private static readonly Dictionary<uint, string> allErrors = new Dictionary<uint, string>
		{
			{
				2149646342U,
				Resources.WIA_ERROR_BUSY
			},
			{
				2149646358U,
				Resources.WIA_ERROR_COVER_OPEN
			},
			{
				2149646346U,
				Resources.WIA_ERROR_DEVICE_COMMUNICATION
			},
			{
				2149646349U,
				Resources.WIA_ERROR_DEVICE_LOCKED
			},
			{
				2149646350U,
				Resources.WIA_ERROR_EXCEPTION_IN_DRIVER
			},
			{
				2149646337U,
				Resources.WIA_ERROR_GENERAL_ERROR
			},
			{
				2149646348U,
				Resources.WIA_ERROR_INCORRECT_HARDWARE_SETTING
			},
			{
				2149646347U,
				Resources.WIA_ERROR_INVALID_COMMAND
			},
			{
				2149646351U,
				Resources.WIA_ERROR_INVALID_DRIVER_RESPONSE
			},
			{
				2149646345U,
				Resources.WIA_ERROR_ITEM_DELETED
			},
			{
				2149646359U,
				Resources.WIA_ERROR_LAMP_OFF
			},
			{
				2149646369U,
				Resources.WIA_ERROR_MAXIMUM_PRINTER_ENDORSER_COUNTER
			},
			{
				2149646368U,
				Resources.WIA_ERROR_MULTI_FEED
			},
			{
				2149646341U,
				Resources.WIA_ERROR_OFFLINE
			},
			{
				2149646339U,
				Resources.ScannerWinScannerBusyContent
			},
			{
				2149646338U,
				Resources.WIA_ERROR_PAPER_JAM
			},
			{
				2149646340U,
				Resources.WIA_ERROR_PAPER_PROBLEM
			},
			{
				2149646343U,
				Resources.WIA_ERROR_WARMING_UP
			},
			{
				2149646344U,
				Resources.WIA_ERROR_USER_INTERVENTION
			},
			{
				2149646357U,
				Resources.WIA_S_NO_DEVICE_AVAILABLE
			}
		};
	}
}
