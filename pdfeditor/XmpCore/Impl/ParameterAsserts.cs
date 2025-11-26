using System;

namespace XmpCore.Impl
{
	// Token: 0x02000031 RID: 49
	internal static class ParameterAsserts
	{
		// Token: 0x060001BC RID: 444 RVA: 0x000050F9 File Offset: 0x000032F9
		public static void AssertArrayName(string arrayName)
		{
			if (string.IsNullOrEmpty(arrayName))
			{
				throw new XmpException("Empty array name", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000510F File Offset: 0x0000330F
		public static void AssertPropName(string propName)
		{
			if (string.IsNullOrEmpty(propName))
			{
				throw new XmpException("Empty property name", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00005125 File Offset: 0x00003325
		public static void AssertSchemaNs(string schemaNs)
		{
			if (string.IsNullOrEmpty(schemaNs))
			{
				throw new XmpException("Empty schema namespace URI", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000513B File Offset: 0x0000333B
		public static void AssertPrefix(string prefix)
		{
			if (string.IsNullOrEmpty(prefix))
			{
				throw new XmpException("Empty prefix", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00005151 File Offset: 0x00003351
		public static void AssertSpecificLang(string specificLang)
		{
			if (string.IsNullOrEmpty(specificLang))
			{
				throw new XmpException("Empty specific language", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00005167 File Offset: 0x00003367
		public static void AssertStructName(string structName)
		{
			if (string.IsNullOrEmpty(structName))
			{
				throw new XmpException("Empty array name", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000517D File Offset: 0x0000337D
		public static void AssertNotNull(object param)
		{
			if (param == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000518E File Offset: 0x0000338E
		public static void AssertNotNullOrEmpty(string param)
		{
			if (param == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
			if (param.Length == 0)
			{
				throw new XmpException("Parameter must not be an empty string", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x000051B3 File Offset: 0x000033B3
		public static void AssertImplementation(IXmpMeta xmp)
		{
			if (xmp == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
			if (!(xmp is XmpMeta))
			{
				throw new XmpException("The XMPMeta-object is not compatible with this implementation", XmpErrorCode.BadParam);
			}
		}
	}
}
