using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NSOCR_NameSpace
{
	// Token: 0x02000006 RID: 6
	public class TNSOCR
	{
		// Token: 0x0600000B RID: 11
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_Initialize();

		// Token: 0x0600000C RID: 12
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_InitializeAdvanced(out int CfgObj, out int OcrObj, out int ImgObj);

		// Token: 0x0600000D RID: 13
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_Uninitialize();

		// Token: 0x0600000E RID: 14
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_SetDataDirectory([MarshalAs(UnmanagedType.LPWStr)] string DirectoryPath);

		// Token: 0x0600000F RID: 15
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_GetVersion([MarshalAs(UnmanagedType.LPWStr)] StringBuilder OptionValue);

		// Token: 0x06000010 RID: 16
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Engine_SetLicenseKey([MarshalAs(UnmanagedType.LPWStr)] string LicenseKey);

		// Token: 0x06000011 RID: 17
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_Create(out int CfgObj);

		// Token: 0x06000012 RID: 18
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_Destroy(int CfgObj);

		// Token: 0x06000013 RID: 19
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_LoadOptions(int CfgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x06000014 RID: 20
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_SaveOptions(int CfgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x06000015 RID: 21
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_LoadOptionsFromString(int CfgObj, [MarshalAs(UnmanagedType.LPWStr)] string XMLString);

		// Token: 0x06000016 RID: 22
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_SaveOptionsToString(int CfgObj, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder XMLString, int MaxLen);

		// Token: 0x06000017 RID: 23
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_GetOption(int CfgObj, int BlockType, [MarshalAs(UnmanagedType.LPWStr)] string OptionPath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder OptionValue, int MaxLen);

		// Token: 0x06000018 RID: 24
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_SetOption(int CfgObj, int BlockType, [MarshalAs(UnmanagedType.LPWStr)] string OptionPath, [MarshalAs(UnmanagedType.LPWStr)] string OptionValue);

		// Token: 0x06000019 RID: 25
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Cfg_DeleteOption(int CfgObj, int BlockType, [MarshalAs(UnmanagedType.LPWStr)] string OptionPath);

		// Token: 0x0600001A RID: 26
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Ocr_Create(int CfgObj, out int OcrObj);

		// Token: 0x0600001B RID: 27
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Ocr_Destroy(int OcrObj);

		// Token: 0x0600001C RID: 28
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Ocr_Destroy(int ImgObj, int SvrObj, int PageIndexStart, int PageIndexEnd, int OcrObjCnt, int Flags);

		// Token: 0x0600001D RID: 29
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Ocr_ProcessPages(int ImgObj, int SvrObj, int PageIndexStart, int PageIndexEnd, int OcrObjCnt, int Flags);

		// Token: 0x0600001E RID: 30
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_Create(int OcrObj, out int ImgObj);

		// Token: 0x0600001F RID: 31
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_Destroy(int ImgObj);

		// Token: 0x06000020 RID: 32
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_LoadFile(int ImgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x06000021 RID: 33
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_LoadFromMemory(int ImgObj, IntPtr Bytes, int DataSize);

		// Token: 0x06000022 RID: 34
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_LoadBmpData(int ImgObj, IntPtr Bytes, int Width, int Height, int Flags, int Stride);

		// Token: 0x06000023 RID: 35
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_Unload(int ImgObj);

		// Token: 0x06000024 RID: 36
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetPageCount(int ImgObj);

		// Token: 0x06000025 RID: 37
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_SetPage(int ImgObj, int PageIndex);

		// Token: 0x06000026 RID: 38
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetSize(int ImgObj, out int Width, out int Height);

		// Token: 0x06000027 RID: 39
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_DrawToDC(int ImgObj, int HandleDC, int X, int Y, ref int Width, ref int Height, int Flags);

		// Token: 0x06000028 RID: 40
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_DeleteAllBlocks(int ImgObj);

		// Token: 0x06000029 RID: 41
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_AddBlock(int ImgObj, int Xpos, int Ypos, int Width, int Height, out int BlkObj);

		// Token: 0x0600002A RID: 42
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_DeleteBlock(int ImgObj, int BlkObj);

		// Token: 0x0600002B RID: 43
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetBlockCnt(int ImgObj);

		// Token: 0x0600002C RID: 44
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetBlock(int ImgObj, int BlockIndex, out int BlkObj);

		// Token: 0x0600002D RID: 45
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetImgText(int ImgObj, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen, int Flags);

		// Token: 0x0600002E RID: 46
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetBmpData(int ImgObj, IntPtr Bits, ref int Width, ref int Height, int Flags);

		// Token: 0x0600002F RID: 47
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_OCR(int ImgObj, int FirstStep, int LastStep, int Flags);

		// Token: 0x06000030 RID: 48
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_SaveBlocks(int ImgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x06000031 RID: 49
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_LoadBlocks(int ImgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x06000032 RID: 50
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetSkewAngle(int ImgObj);

		// Token: 0x06000033 RID: 51
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetPixLineCnt(int ImgObj);

		// Token: 0x06000034 RID: 52
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetPixLine(int ImgObj, int LineInd, out int X1pos, out int Y1pos, out int X2pos, out int Y2pos, out int Width);

		// Token: 0x06000035 RID: 53
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetScaleFactor(int ImgObj);

		// Token: 0x06000036 RID: 54
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_CalcPointPosition(int ImgObj, ref int Xpos, ref int Ypos, int Mode);

		// Token: 0x06000037 RID: 55
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_CopyCurrentPage(int ImgObjSrc, int ImgObjDst, int Flags);

		// Token: 0x06000038 RID: 56
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_GetProperty(int ImgObj, int PropertyID);

		// Token: 0x06000039 RID: 57
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_SaveToFile(int ImgObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName, int Format, int Flags);

		// Token: 0x0600003A RID: 58
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Img_SaveToMemory(int ImgObj, IntPtr Bytes, int BufferSize, int Format, int Flags);

		// Token: 0x0600003B RID: 59
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetType(int BlkObj);

		// Token: 0x0600003C RID: 60
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_SetType(int BlkObj, int BlockType);

		// Token: 0x0600003D RID: 61
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetRect(int BlkObj, out int Xpos, out int Ypos, out int Width, out int Height);

		// Token: 0x0600003E RID: 62
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetText(int BlkObj, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen, int Flags);

		// Token: 0x0600003F RID: 63
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetLineCnt(int BlkObj);

		// Token: 0x06000040 RID: 64
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetLineText(int BlkObj, int LineIndex, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen);

		// Token: 0x06000041 RID: 65
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordCnt(int BlkObj, int LineIndex);

		// Token: 0x06000042 RID: 66
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordText(int BlkObj, int LineIndex, int WordIndex, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen);

		// Token: 0x06000043 RID: 67
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_SetWordText(int BlkObj, int LineIndex, int WordIndex, [MarshalAs(UnmanagedType.LPWStr)] string TextStr);

		// Token: 0x06000044 RID: 68
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetCharCnt(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x06000045 RID: 69
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetCharRect(int BlkObj, int LineIndex, int WordIndex, int CharIndex, out int Xpos, out int Ypos, out int Width, out int Height);

		// Token: 0x06000046 RID: 70
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetCharText(int BlkObj, int LineIndex, int WordIndex, int CharIndex, int ResultIndex, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen);

		// Token: 0x06000047 RID: 71
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetCharQual(int BlkObj, int LineIndex, int WordIndex, int CharIndex, int ResultIndex);

		// Token: 0x06000048 RID: 72
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetTextRect(int BlkObj, int LineIndex, int WordIndex, out int Xpos, out int Ypos, out int Width, out int Height);

		// Token: 0x06000049 RID: 73
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_Inversion(int BlkObj, int Inversion);

		// Token: 0x0600004A RID: 74
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_Rotation(int BlkObj, int Rotation);

		// Token: 0x0600004B RID: 75
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_Mirror(int BlkObj, int Mirror);

		// Token: 0x0600004C RID: 76
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_IsWordInDictionary(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x0600004D RID: 77
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_SetRect(int BlkObj, int Xpos, int Ypos, int Width, int Height);

		// Token: 0x0600004E RID: 78
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordQual(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x0600004F RID: 79
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordFontColor(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x06000050 RID: 80
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordFontSize(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x06000051 RID: 81
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetWordFontStyle(int BlkObj, int LineIndex, int WordIndex);

		// Token: 0x06000052 RID: 82
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetBackgroundColor(int BlkObj);

		// Token: 0x06000053 RID: 83
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_SetWordRegEx(int BlkObj, int LineIndex, int WordIndex, [MarshalAs(UnmanagedType.LPWStr)] string RegEx, int Flags);

		// Token: 0x06000054 RID: 84
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetBarcodeCnt(int BlkObj);

		// Token: 0x06000055 RID: 85
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetBarcodeText(int BlkObj, int BarcodeInd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen);

		// Token: 0x06000056 RID: 86
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetBarcodeRect(int BlkObj, int BarcodeInd, out int Xpos, out int Ypos, out int Width, out int Height);

		// Token: 0x06000057 RID: 87
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Blk_GetBarcodeType(int BlkObj, int BarcodeInd);

		// Token: 0x06000058 RID: 88
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_Create(int CfgObj, int Format, out int SvrObj);

		// Token: 0x06000059 RID: 89
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_Destroy(int SvrObj);

		// Token: 0x0600005A RID: 90
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_NewDocument(int SvrObj);

		// Token: 0x0600005B RID: 91
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_AddPage(int SvrObj, int ImgObj, int Flags);

		// Token: 0x0600005C RID: 92
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_SaveToFile(int SvrObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

		// Token: 0x0600005D RID: 93
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_SaveToMemory(int SvrObj, IntPtr Bytes, int BufferSize);

		// Token: 0x0600005E RID: 94
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_GetText(int SvrObj, int PageIndex, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder TextStr, int MaxLen);

		// Token: 0x0600005F RID: 95
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Svr_SetDocumentInfo(int SvrObj, int InfoID, [MarshalAs(UnmanagedType.LPWStr)] string InfoStr);

		// Token: 0x06000060 RID: 96
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Scan_Create(int CfgObj, out int ScanObj);

		// Token: 0x06000061 RID: 97
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Scan_Destroy(int ScanObj);

		// Token: 0x06000062 RID: 98
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Scan_Enumerate(int ScanObj, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ScannerNames, int MaxLen, int Flags);

		// Token: 0x06000063 RID: 99
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Scan_ScanToImg(int ScanObj, int ImgObj, int ScannerIndex, int Flags);

		// Token: 0x06000064 RID: 100
		[DllImport("NsOCR\\Bin_64\\NSOCR.dll")]
		public static extern int Scan_ScanToFile(int ScanObj, [MarshalAs(UnmanagedType.LPWStr)] string FileName, int ScannerIndex, int Flags);

		// Token: 0x04000003 RID: 3
		public const int ERROR_FIRST = 1879048192;

		// Token: 0x04000004 RID: 4
		public const int ERROR_FILENOTFOUND = 1879048193;

		// Token: 0x04000005 RID: 5
		public const int ERROR_LOADFILE = 1879048194;

		// Token: 0x04000006 RID: 6
		public const int ERROR_SAVEFILE = 1879048195;

		// Token: 0x04000007 RID: 7
		public const int ERROR_MISSEDIMGLOADER = 1879048196;

		// Token: 0x04000008 RID: 8
		public const int ERROR_OPTIONNOTFOUND = 1879048197;

		// Token: 0x04000009 RID: 9
		public const int ERROR_NOBLOCKS = 1879048198;

		// Token: 0x0400000A RID: 10
		public const int ERROR_BLOCKNOTFOUND = 1879048199;

		// Token: 0x0400000B RID: 11
		public const int ERROR_INVALIDINDEX = 1879048200;

		// Token: 0x0400000C RID: 12
		public const int ERROR_INVALIDPARAMETER = 1879048201;

		// Token: 0x0400000D RID: 13
		public const int ERROR_FAILED = 1879048202;

		// Token: 0x0400000E RID: 14
		public const int ERROR_INVALIDBLOCKTYPE = 1879048203;

		// Token: 0x0400000F RID: 15
		public const int ERROR_EMPTYTEXT = 1879048205;

		// Token: 0x04000010 RID: 16
		public const int ERROR_LOADINGDICTIONARY = 1879048206;

		// Token: 0x04000011 RID: 17
		public const int ERROR_LOADCHARBASE = 1879048207;

		// Token: 0x04000012 RID: 18
		public const int ERROR_NOMEMORY = 1879048208;

		// Token: 0x04000013 RID: 19
		public const int ERROR_CANNOTLOADGS = 1879048209;

		// Token: 0x04000014 RID: 20
		public const int ERROR_CANNOTPROCESSPDF = 1879048210;

		// Token: 0x04000015 RID: 21
		public const int ERROR_NOIMAGE = 1879048211;

		// Token: 0x04000016 RID: 22
		public const int ERROR_MISSEDSTEP = 1879048212;

		// Token: 0x04000017 RID: 23
		public const int ERROR_OUTOFIMAGE = 1879048213;

		// Token: 0x04000018 RID: 24
		public const int ERROR_EXCEPTION = 1879048214;

		// Token: 0x04000019 RID: 25
		public const int ERROR_NOTALLOWED = 1879048215;

		// Token: 0x0400001A RID: 26
		public const int ERROR_NODEFAULTDEVICE = 1879048216;

		// Token: 0x0400001B RID: 27
		public const int ERROR_NOTAPPLICABLE = 1879048217;

		// Token: 0x0400001C RID: 28
		public const int ERROR_MISSEDBARCODEDLL = 1879048218;

		// Token: 0x0400001D RID: 29
		public const int ERROR_PENDING = 1879048219;

		// Token: 0x0400001E RID: 30
		public const int ERROR_OPERATIONCANCELLED = 1879048220;

		// Token: 0x0400001F RID: 31
		public const int ERROR_TOOMANYLANGUAGES = 1879048221;

		// Token: 0x04000020 RID: 32
		public const int ERROR_OPERATIONTIMEOUT = 1879048222;

		// Token: 0x04000021 RID: 33
		public const int ERROR_LOAD_ASIAN_MODULE = 1879048223;

		// Token: 0x04000022 RID: 34
		public const int ERROR_LOAD_ASIAN_LANG = 1879048224;

		// Token: 0x04000023 RID: 35
		public const int ERROR_INVALIDOBJECT = 1879113728;

		// Token: 0x04000024 RID: 36
		public const int ERROR_TOOMANYOBJECTS = 1879113729;

		// Token: 0x04000025 RID: 37
		public const int ERROR_DLLNOTLOADED = 1879113730;

		// Token: 0x04000026 RID: 38
		public const int ERROR_DEMO = 1879113731;

		// Token: 0x04000027 RID: 39
		public const int BT_DEFAULT = 0;

		// Token: 0x04000028 RID: 40
		public const int BT_OCRTEXT = 1;

		// Token: 0x04000029 RID: 41
		public const int BT_ICRDIGIT = 2;

		// Token: 0x0400002A RID: 42
		public const int BT_CLEAR = 3;

		// Token: 0x0400002B RID: 43
		public const int BT_PICTURE = 4;

		// Token: 0x0400002C RID: 44
		public const int BT_ZONING = 5;

		// Token: 0x0400002D RID: 45
		public const int BT_OCRDIGIT = 6;

		// Token: 0x0400002E RID: 46
		public const int BT_BARCODE = 7;

		// Token: 0x0400002F RID: 47
		public const int BT_TABLE = 8;

		// Token: 0x04000030 RID: 48
		public const int BT_MRZ = 9;

		// Token: 0x04000031 RID: 49
		public const int BMP_24BIT = 0;

		// Token: 0x04000032 RID: 50
		public const int BMP_8BIT = 1;

		// Token: 0x04000033 RID: 51
		public const int BMP_1BIT = 2;

		// Token: 0x04000034 RID: 52
		public const int BMP_32BIT = 3;

		// Token: 0x04000035 RID: 53
		public const int BMP_BOTTOMTOP = 256;

		// Token: 0x04000036 RID: 54
		public const int FMT_EDITCOPY = 0;

		// Token: 0x04000037 RID: 55
		public const int FMT_EXACTCOPY = 1;

		// Token: 0x04000038 RID: 56
		public const int OCRSTEP_FIRST = 0;

		// Token: 0x04000039 RID: 57
		public const int OCRSTEP_PREFILTERS = 16;

		// Token: 0x0400003A RID: 58
		public const int OCRSTEP_BINARIZE = 32;

		// Token: 0x0400003B RID: 59
		public const int OCRSTEP_POSTFILTERS = 80;

		// Token: 0x0400003C RID: 60
		public const int OCRSTEP_REMOVELINES = 96;

		// Token: 0x0400003D RID: 61
		public const int OCRSTEP_ZONING = 112;

		// Token: 0x0400003E RID: 62
		public const int OCRSTEP_OCR = 128;

		// Token: 0x0400003F RID: 63
		public const int OCRSTEP_LAST = 255;

		// Token: 0x04000040 RID: 64
		public const int OCRFLAG_NONE = 0;

		// Token: 0x04000041 RID: 65
		public const int OCRFLAG_THREAD = 1;

		// Token: 0x04000042 RID: 66
		public const int OCRFLAG_GETRESULT = 2;

		// Token: 0x04000043 RID: 67
		public const int OCRFLAG_GETPROGRESS = 3;

		// Token: 0x04000044 RID: 68
		public const int OCRFLAG_CANCEL = 4;

		// Token: 0x04000045 RID: 69
		public const int DRAW_NORMAL = 0;

		// Token: 0x04000046 RID: 70
		public const int DRAW_BINARY = 1;

		// Token: 0x04000047 RID: 71
		public const int DRAW_GETBPP = 256;

		// Token: 0x04000048 RID: 72
		public const int BLK_INVERSE_GET = -1;

		// Token: 0x04000049 RID: 73
		public const int BLK_INVERSE_SET0 = 0;

		// Token: 0x0400004A RID: 74
		public const int BLK_INVERSE_SET1 = 1;

		// Token: 0x0400004B RID: 75
		public const int BLK_INVERSE_DETECT = 256;

		// Token: 0x0400004C RID: 76
		public const int BLK_ROTATE_GET = -1;

		// Token: 0x0400004D RID: 77
		public const int BLK_ROTATE_NONE = 0;

		// Token: 0x0400004E RID: 78
		public const int BLK_ROTATE_90 = 1;

		// Token: 0x0400004F RID: 79
		public const int BLK_ROTATE_180 = 2;

		// Token: 0x04000050 RID: 80
		public const int BLK_ROTATE_270 = 3;

		// Token: 0x04000051 RID: 81
		public const int BLK_ROTATE_ANGLE = 1048576;

		// Token: 0x04000052 RID: 82
		public const int BLK_ROTATE_DETECT = 256;

		// Token: 0x04000053 RID: 83
		public const int BLK_MIRROR_GET = -1;

		// Token: 0x04000054 RID: 84
		public const int BLK_MIRROR_NONE = 0;

		// Token: 0x04000055 RID: 85
		public const int BLK_MIRROR_H = 1;

		// Token: 0x04000056 RID: 86
		public const int BLK_MIRROR_V = 2;

		// Token: 0x04000057 RID: 87
		public const int SVR_FORMAT_PDF = 1;

		// Token: 0x04000058 RID: 88
		public const int SVR_FORMAT_RTF = 2;

		// Token: 0x04000059 RID: 89
		public const int SVR_FORMAT_TXT_ASCII = 3;

		// Token: 0x0400005A RID: 90
		public const int SVR_FORMAT_TXT_UNICODE = 4;

		// Token: 0x0400005B RID: 91
		public const int SVR_FORMAT_XML = 5;

		// Token: 0x0400005C RID: 92
		public const int SVR_FORMAT_PDFA = 6;

		// Token: 0x0400005D RID: 93
		public const int SCAN_GETDEFAULTDEVICE = 1;

		// Token: 0x0400005E RID: 94
		public const int SCAN_SETDEFAULTDEVICE = 256;

		// Token: 0x0400005F RID: 95
		public const int SCAN_NOUI = 1;

		// Token: 0x04000060 RID: 96
		public const int SCAN_SOURCEADF = 2;

		// Token: 0x04000061 RID: 97
		public const int SCAN_SOURCEAUTO = 4;

		// Token: 0x04000062 RID: 98
		public const int SCAN_DONTCLOSEDS = 8;

		// Token: 0x04000063 RID: 99
		public const int SCAN_FILE_SEPARATE = 16;

		// Token: 0x04000064 RID: 100
		public const int FONT_STYLE_UNDERLINED = 1;

		// Token: 0x04000065 RID: 101
		public const int FONT_STYLE_STRIKED = 2;

		// Token: 0x04000066 RID: 102
		public const int FONT_STYLE_BOLD = 4;

		// Token: 0x04000067 RID: 103
		public const int FONT_STYLE_ITALIC = 8;

		// Token: 0x04000068 RID: 104
		public const int IMG_PROP_DPIX = 1;

		// Token: 0x04000069 RID: 105
		public const int IMG_PROP_DPIY = 2;

		// Token: 0x0400006A RID: 106
		public const int IMG_PROP_BPP = 3;

		// Token: 0x0400006B RID: 107
		public const int IMG_PROP_WIDTH = 4;

		// Token: 0x0400006C RID: 108
		public const int IMG_PROP_HEIGHT = 5;

		// Token: 0x0400006D RID: 109
		public const int IMG_PROP_INVERTED = 6;

		// Token: 0x0400006E RID: 110
		public const int IMG_PROP_SKEW = 7;

		// Token: 0x0400006F RID: 111
		public const int IMG_PROP_SCALE = 8;

		// Token: 0x04000070 RID: 112
		public const int IMG_PROP_PAGEINDEX = 9;

		// Token: 0x04000071 RID: 113
		public const int REGEX_SET = 0;

		// Token: 0x04000072 RID: 114
		public const int REGEX_CLEAR = 1;

		// Token: 0x04000073 RID: 115
		public const int REGEX_CLEAR_ALL = 2;

		// Token: 0x04000074 RID: 116
		public const int REGEX_DISABLE_DICT = 4;

		// Token: 0x04000075 RID: 117
		public const int REGEX_CHECK = 8;

		// Token: 0x04000076 RID: 118
		public const int INFO_PDF_AUTHOR = 1;

		// Token: 0x04000077 RID: 119
		public const int INFO_PDF_CREATOR = 2;

		// Token: 0x04000078 RID: 120
		public const int INFO_PDF_PRODUCER = 3;

		// Token: 0x04000079 RID: 121
		public const int INFO_PDF_TITLE = 4;

		// Token: 0x0400007A RID: 122
		public const int INFO_PDF_SUBJECT = 5;

		// Token: 0x0400007B RID: 123
		public const int INFO_PDF_KEYWORDS = 6;

		// Token: 0x0400007C RID: 124
		public const int BARCODE_TYPE_EAN8 = 1;

		// Token: 0x0400007D RID: 125
		public const int BARCODE_TYPE_UPCE = 2;

		// Token: 0x0400007E RID: 126
		public const int BARCODE_TYPE_ISBN10 = 3;

		// Token: 0x0400007F RID: 127
		public const int BARCODE_TYPE_UPCA = 4;

		// Token: 0x04000080 RID: 128
		public const int BARCODE_TYPE_EAN13 = 5;

		// Token: 0x04000081 RID: 129
		public const int BARCODE_TYPE_ISBN13 = 6;

		// Token: 0x04000082 RID: 130
		public const int BARCODE_TYPE_ZBAR_I25 = 7;

		// Token: 0x04000083 RID: 131
		public const int BARCODE_TYPE_CODE39 = 8;

		// Token: 0x04000084 RID: 132
		public const int BARCODE_TYPE_QRCODE = 9;

		// Token: 0x04000085 RID: 133
		public const int BARCODE_TYPE_CODE128 = 10;

		// Token: 0x04000086 RID: 134
		public const int BARCODE_TYPE_MASK_EAN8 = 1;

		// Token: 0x04000087 RID: 135
		public const int BARCODE_TYPE_MASK_UPCE = 2;

		// Token: 0x04000088 RID: 136
		public const int BARCODE_TYPE_MASK_ISBN10 = 4;

		// Token: 0x04000089 RID: 137
		public const int BARCODE_TYPE_MASK_UPCA = 8;

		// Token: 0x0400008A RID: 138
		public const int BARCODE_TYPE_MASK_EAN13 = 16;

		// Token: 0x0400008B RID: 139
		public const int BARCODE_TYPE_MASK_ISBN13 = 32;

		// Token: 0x0400008C RID: 140
		public const int BARCODE_TYPE_MASK_ZBAR_I25 = 64;

		// Token: 0x0400008D RID: 141
		public const int BARCODE_TYPE_MASK_CODE39 = 128;

		// Token: 0x0400008E RID: 142
		public const int BARCODE_TYPE_MASK_QRCODE = 256;

		// Token: 0x0400008F RID: 143
		public const int BARCODE_TYPE_MASK_CODE128 = 512;

		// Token: 0x04000090 RID: 144
		public const int IMG_FORMAT_BMP = 0;

		// Token: 0x04000091 RID: 145
		public const int IMG_FORMAT_JPEG = 2;

		// Token: 0x04000092 RID: 146
		public const int IMG_FORMAT_PNG = 13;

		// Token: 0x04000093 RID: 147
		public const int IMG_FORMAT_TIFF = 18;

		// Token: 0x04000094 RID: 148
		public const int IMG_FORMAT_FLAG_BINARIZED = 256;

		// Token: 0x04000095 RID: 149
		public const string LIBNAME = "NsOCR\\Bin_64\\NSOCR.dll";
	}
}
