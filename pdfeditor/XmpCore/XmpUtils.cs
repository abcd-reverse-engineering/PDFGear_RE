using System;
using System.Globalization;
using System.Text;
using XmpCore.Impl;
using XmpCore.Options;

namespace XmpCore
{
	// Token: 0x02000024 RID: 36
	public static class XmpUtils
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00003489 File Offset: 0x00001689
		public static string CatenateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string separator, string quotes, bool allowCommas)
		{
			return XmpUtils.CatenateArrayItems(xmp, schemaNs, arrayName, separator, quotes, allowCommas);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00003498 File Offset: 0x00001698
		public static void SeparateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string catedStr, PropertyOptions arrayOptions, bool preserveCommas)
		{
			XmpUtils.SeparateArrayItems(xmp, schemaNs, arrayName, catedStr, arrayOptions, preserveCommas);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000034A7 File Offset: 0x000016A7
		public static void RemoveProperties(IXmpMeta xmp, string schemaNs, string propName, bool doAllProperties, bool includeAliases)
		{
			XmpUtils.RemoveProperties(xmp, schemaNs, propName, doAllProperties, includeAliases);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000034B4 File Offset: 0x000016B4
		public static void AppendProperties(IXmpMeta source, IXmpMeta dest, bool doAllProperties, bool replaceOldValues, bool deleteEmptyValues = false)
		{
			XmpUtils.AppendProperties(source, dest, doAllProperties, replaceOldValues, deleteEmptyValues);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000034C4 File Offset: 0x000016C4
		public static bool ConvertToBoolean(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
			}
			int num;
			if (int.TryParse(value, out num))
			{
				return num != 0;
			}
			return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "t", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "on", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000352C File Offset: 0x0000172C
		public static string ConvertFromBoolean(bool value)
		{
			if (!value)
			{
				return "False";
			}
			return "True";
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000353C File Offset: 0x0000173C
		public static int ConvertToInteger(string rawValue)
		{
			if (string.IsNullOrEmpty(rawValue))
			{
				throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
			}
			int num;
			if (!(rawValue.StartsWith("0x") ? int.TryParse(rawValue.Substring(2), NumberStyles.HexNumber, null, out num) : int.TryParse(rawValue, out num)))
			{
				throw new XmpException("Invalid integer string", XmpErrorCode.BadValue);
			}
			return num;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00003597 File Offset: 0x00001797
		public static string ConvertFromInteger(int value)
		{
			return value.ToString();
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000035A0 File Offset: 0x000017A0
		public static long ConvertToLong(string rawValue)
		{
			if (string.IsNullOrEmpty(rawValue))
			{
				throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
			}
			long num;
			if (!(rawValue.StartsWith("0x") ? long.TryParse(rawValue.Substring(2), NumberStyles.HexNumber, null, out num) : long.TryParse(rawValue, out num)))
			{
				throw new XmpException("Invalid long string", XmpErrorCode.BadValue);
			}
			return num;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000035FB File Offset: 0x000017FB
		public static string ConvertFromLong(long value)
		{
			return value.ToString();
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00003604 File Offset: 0x00001804
		public static double ConvertToDouble(string rawValue)
		{
			if (string.IsNullOrEmpty(rawValue))
			{
				throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
			}
			double num;
			if (!double.TryParse(rawValue, out num))
			{
				throw new XmpException("Invalid double string", XmpErrorCode.BadValue);
			}
			return num;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000363C File Offset: 0x0000183C
		public static string ConvertFromDouble(double value)
		{
			return value.ToString();
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00003645 File Offset: 0x00001845
		public static IXmpDateTime ConvertToDate(string rawValue)
		{
			if (string.IsNullOrEmpty(rawValue))
			{
				throw new XmpException("Empty convert-string", XmpErrorCode.BadValue);
			}
			return Iso8601Converter.Parse(rawValue);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00003661 File Offset: 0x00001861
		public static string ConvertFromDate(IXmpDateTime value)
		{
			return Iso8601Converter.Render(value);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00003669 File Offset: 0x00001869
		public static string EncodeBase64(byte[] buffer)
		{
			return Convert.ToBase64String(buffer);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00003674 File Offset: 0x00001874
		public static byte[] DecodeBase64(string base64String)
		{
			byte[] array;
			try
			{
				array = Convert.FromBase64String(base64String);
			}
			catch (Exception ex)
			{
				throw new XmpException("Invalid base64 string", XmpErrorCode.BadValue, ex);
			}
			return array;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000036AC File Offset: 0x000018AC
		public static void PackageForJPEG(IXmpMeta origXMP, StringBuilder stdStr, StringBuilder extStr, StringBuilder digestStr)
		{
			XmpUtils.PackageForJPEG(origXMP, stdStr, extStr, digestStr);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000036B7 File Offset: 0x000018B7
		public static void MergeFromJPEG(IXmpMeta fullXMP, IXmpMeta extendedXMP)
		{
			XmpUtils.MergeFromJPEG(fullXMP, extendedXMP);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000036C0 File Offset: 0x000018C0
		public static void ApplyTemplate(IXmpMeta workingXMP, IXmpMeta templateXMP, TemplateOptions options)
		{
			XmpUtils.ApplyTemplate(workingXMP, templateXMP, options);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000036CA File Offset: 0x000018CA
		public static void DuplicateSubtree(IXmpMeta source, IXmpMeta dest, string sourceNS, string sourceRoot, string destNS, string destRoot, PropertyOptions options)
		{
			XmpUtils.DuplicateSubtree(source, dest, sourceNS, sourceRoot, destNS, destRoot, options);
		}
	}
}
