using System;
using XmpCore.Impl;
using XmpCore.Impl.XPath;

namespace XmpCore
{
	// Token: 0x02000023 RID: 35
	public static class XmpPathFactory
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x000032DA File Offset: 0x000014DA
		public static string ComposeArrayItemPath(string arrayName, int itemIndex)
		{
			if (itemIndex > 0)
			{
				return string.Format("{0}[{1}]", arrayName, itemIndex);
			}
			if (itemIndex == -1)
			{
				return arrayName + "[last()]";
			}
			throw new XmpException("Array index must be larger than zero", XmpErrorCode.BadIndex);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00003310 File Offset: 0x00001510
		public static string ComposeStructFieldPath(string fieldNs, string fieldName)
		{
			XmpPathFactory.AssertFieldNs(fieldNs);
			XmpPathFactory.AssertFieldName(fieldName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(fieldNs, fieldName);
			if (xmpPath.Size() != 2)
			{
				throw new XmpException("The field name must be simple", XmpErrorCode.BadXPath);
			}
			return "/" + xmpPath.GetSegment(1).Name;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00003360 File Offset: 0x00001560
		public static string ComposeQualifierPath(string qualNs, string qualName)
		{
			XmpPathFactory.AssertQualNs(qualNs);
			XmpPathFactory.AssertQualName(qualName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(qualNs, qualName);
			if (xmpPath.Size() != 2)
			{
				throw new XmpException("The qualifier name must be simple", XmpErrorCode.BadXPath);
			}
			return "/?" + xmpPath.GetSegment(1).Name;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000033AD File Offset: 0x000015AD
		public static string ComposeLangSelector(string arrayName, string langName)
		{
			return arrayName + "[?xml:lang=\"" + Utils.NormalizeLangValue(langName) + "\"]";
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000033C8 File Offset: 0x000015C8
		public static string ComposeFieldSelector(string arrayName, string fieldNs, string fieldName, string fieldValue)
		{
			XmpPath xmpPath = XmpPathParser.ExpandXPath(fieldNs, fieldName);
			if (xmpPath.Size() != 2)
			{
				throw new XmpException("The fieldName name must be simple", XmpErrorCode.BadXPath);
			}
			return string.Concat(new string[]
			{
				arrayName,
				"[",
				xmpPath.GetSegment(1).Name,
				"=\"",
				fieldValue,
				"\"]"
			});
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000342D File Offset: 0x0000162D
		private static void AssertQualNs(string qualNs)
		{
			if (string.IsNullOrEmpty(qualNs))
			{
				throw new XmpException("Empty qualifier namespace URI", XmpErrorCode.BadSchema);
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00003444 File Offset: 0x00001644
		private static void AssertQualName(string qualName)
		{
			if (string.IsNullOrEmpty(qualName))
			{
				throw new XmpException("Empty qualifier name", XmpErrorCode.BadXPath);
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000345B File Offset: 0x0000165B
		private static void AssertFieldNs(string fieldNs)
		{
			if (string.IsNullOrEmpty(fieldNs))
			{
				throw new XmpException("Empty field namespace URI", XmpErrorCode.BadSchema);
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00003472 File Offset: 0x00001672
		private static void AssertFieldName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				throw new XmpException("Empty f name", XmpErrorCode.BadXPath);
			}
		}
	}
}
