using System;
using System.Collections.Generic;
using System.Text;

namespace XmpCore.Impl
{
	// Token: 0x02000035 RID: 53
	public static class Utils
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x000067A4 File Offset: 0x000049A4
		static Utils()
		{
			char c = '\0';
			while ((int)c < Utils._xmlNameChars.Length)
			{
				bool flag = c == ':' || ('A' <= c && c <= 'Z') || c == '_' || ('a' <= c && c <= 'z') || ('À' <= c && c <= 'Ö') || ('Ø' <= c && c <= 'ö') || ('ø' <= c && c <= 'ÿ');
				Utils._xmlNameStartChars[(int)c] = flag;
				Utils._xmlNameChars[(int)c] = flag || c == '-' || c == '.' || ('0' <= c && c <= '9') || c == '·';
				c += '\u0001';
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00006A34 File Offset: 0x00004C34
		public static string NormalizeLangValue(string value)
		{
			if (value == "x-default")
			{
				return value;
			}
			int num = 1;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				if (c != ' ')
				{
					if (c == '-' || c == '_')
					{
						stringBuilder.Append('-');
						num++;
					}
					else
					{
						stringBuilder.Append((num != 2) ? char.ToLower(c) : char.ToUpper(c));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00006AB8 File Offset: 0x00004CB8
		internal static void SplitNameAndValue(string selector, out string name, out string value)
		{
			int num = selector.IndexOf('=');
			int i = 1;
			if (selector[i] == '?')
			{
				i++;
			}
			name = selector.Substring(i, num - i);
			i = num + 1;
			char c = selector[i];
			i++;
			int num2 = selector.Length - 2;
			StringBuilder stringBuilder = new StringBuilder(num2 - num);
			while (i < num2)
			{
				stringBuilder.Append(selector[i]);
				i++;
				if (selector[i] == c)
				{
					i++;
				}
			}
			value = stringBuilder.ToString();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00006B40 File Offset: 0x00004D40
		internal static bool IsInternalProperty(string schema, string prop)
		{
			if (schema != null)
			{
				switch (schema.Length)
				{
				case 28:
				{
					char c = schema[20];
					if (c != 'p')
					{
						if (c != 'x')
						{
							return false;
						}
						if (!(schema == "http://ns.adobe.com/xap/1.0/"))
						{
							return false;
						}
						return prop == "xmp:BaseURL" || prop == "xmp:CreatorTool" || prop == "xmp:Format" || prop == "xmp:Locale" || prop == "xmp:MetadataDate" || "xmp:ModifyDate" == prop;
					}
					else
					{
						if (!(schema == "http://ns.adobe.com/pdf/1.3/"))
						{
							return false;
						}
						return prop == "pdf:BaseURL" || prop == "pdf:Creator" || prop == "pdf:ModDate" || prop == "pdf:PDFVersion" || prop == "pdf:Producer";
					}
					break;
				}
				case 29:
				{
					char c = schema[20];
					if (c != 'e')
					{
						if (c != 't')
						{
							return false;
						}
						if (!(schema == "http://ns.adobe.com/tiff/1.0/"))
						{
							return false;
						}
						return prop != "tiff:ImageDescription" && prop != "tiff:Artist" && prop != "tiff:Copyright";
					}
					else
					{
						if (!(schema == "http://ns.adobe.com/exif/1.0/"))
						{
							return false;
						}
						return prop != "exif:UserComment";
					}
					break;
				}
				case 30:
				{
					char c = schema[28];
					if (c != 'g')
					{
						if (c != 't')
						{
							return false;
						}
						if (!(schema == "http://ns.adobe.com/xap/1.0/t/"))
						{
							return false;
						}
					}
					else if (!(schema == "http://ns.adobe.com/xap/1.0/g/"))
					{
						return false;
					}
					break;
				}
				case 31:
					if (!(schema == "http://ns.adobe.com/xap/1.0/mm/"))
					{
						return false;
					}
					break;
				case 32:
					if (!(schema == "http://purl.org/dc/elements/1.1/"))
					{
						return false;
					}
					return prop == "dc:format" || prop == "dc:language";
				case 33:
				{
					char c = schema[20];
					if (c != 'b')
					{
						if (c != 'e')
						{
							if (c != 'x')
							{
								return false;
							}
							if (!(schema == "http://ns.adobe.com/xap/1.0/t/pg/"))
							{
								return false;
							}
						}
						else if (!(schema == "http://ns.adobe.com/exif/1.0/aux/"))
						{
							return false;
						}
					}
					else
					{
						if (!(schema == "http://ns.adobe.com/bwf/bext/1.0/"))
						{
							return false;
						}
						return prop == "bext:version";
					}
					break;
				}
				case 34:
				{
					char c = schema[20];
					if (c != 'p')
					{
						if (c != 'x')
						{
							return false;
						}
						if (!(schema == "http://ns.adobe.com/xap/1.0/g/img/"))
						{
							return false;
						}
					}
					else
					{
						if (!(schema == "http://ns.adobe.com/photoshop/1.0/"))
						{
							return false;
						}
						return prop == "photoshop:ICCProfile" || prop == "photoshop:TextLayers";
					}
					break;
				}
				case 35:
				{
					char c = schema[20];
					if (c != 'S')
					{
						if (c != 'x')
						{
							return false;
						}
						if (!(schema == "http://ns.adobe.com/xmp/1.0/Script/"))
						{
							return false;
						}
						return prop != "xmpScript:action" && prop != "xmpScript:character" && prop != "xmpScript:dialog" && prop != "xmpScript:sceneSetting" && prop != "xmpScript:sceneTimeOfDay";
					}
					else if (!(schema == "http://ns.adobe.com/StockPhoto/1.0/"))
					{
						return false;
					}
					break;
				}
				case 36:
				case 37:
				case 38:
				case 40:
				case 42:
				case 43:
					return false;
				case 39:
					if (!(schema == "http://ns.adobe.com/xap/1.0/sType/Font#"))
					{
						return false;
					}
					break;
				case 41:
					if (!(schema == "http://ns.adobe.com/xmp/1.0/DynamicMedia/"))
					{
						return false;
					}
					return !Utils.EXTERNAL_XMPDM_PROPS.Contains(prop);
				case 44:
					if (!(schema == "http://ns.adobe.com/camera-raw-settings/1.0/"))
					{
						return false;
					}
					return true;
				default:
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00006F40 File Offset: 0x00005140
		internal static bool CheckUuidFormat(string uuid)
		{
			if (uuid == null)
			{
				return false;
			}
			bool flag = true;
			int num = 0;
			int i;
			for (i = 0; i < uuid.Length; i++)
			{
				if (uuid[i] == '-')
				{
					num++;
					flag = flag && (i == 8 || i == 13 || i == 18 || i == 23);
				}
			}
			return flag && 4 == num && 36 == i;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00006FA4 File Offset: 0x000051A4
		public static bool IsXmlName(string name)
		{
			if (name.Length > 0 && !Utils.IsNameStartChar(name[0]))
			{
				return false;
			}
			for (int i = 1; i < name.Length; i++)
			{
				if (!Utils.IsNameChar(name[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00006FEC File Offset: 0x000051EC
		public static bool IsXmlNameNs(string name)
		{
			if (name.Length > 0 && (!Utils.IsNameStartChar(name[0]) || name[0] == ':'))
			{
				return false;
			}
			for (int i = 1; i < name.Length; i++)
			{
				if (!Utils.IsNameChar(name[i]) || name[i] == ':')
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000704A File Offset: 0x0000524A
		internal static bool IsControlChar(char c)
		{
			return (c <= '\u001f' || c == '\u007f') && c != '\t' && c != '\n' && c != '\r';
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000706C File Offset: 0x0000526C
		public static string EscapeXml(string value, bool forAttribute, bool escapeWhitespaces)
		{
			bool flag = value.IndexOfAny(Utils.BasicEscapeCharacters) != -1;
			if (escapeWhitespaces)
			{
				flag |= value.IndexOfAny(Utils.WhiteSpaceEscapeCharacters) != -1;
			}
			if (forAttribute)
			{
				flag |= value.IndexOf('"') != -1;
			}
			if (!flag)
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder(value.Length * 4 / 3);
			foreach (char c in value)
			{
				if (!escapeWhitespaces || (c != '\t' && c != '\n' && c != '\r'))
				{
					if (c <= '&')
					{
						if (c == '"')
						{
							stringBuilder.Append(forAttribute ? "&quot;" : "\"");
							goto IL_00FB;
						}
						if (c == '&')
						{
							stringBuilder.Append("&amp;");
							goto IL_00FB;
						}
					}
					else
					{
						if (c == '<')
						{
							stringBuilder.Append("&lt;");
							goto IL_00FB;
						}
						if (c == '>')
						{
							stringBuilder.Append("&gt;");
							goto IL_00FB;
						}
					}
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.AppendFormat("&#x{0:X};", (int)c);
				}
				IL_00FB:;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000718C File Offset: 0x0000538C
		internal static string RemoveControlChars(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(value);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (Utils.IsControlChar(stringBuilder[i]))
				{
					stringBuilder[i] = ' ';
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000071D0 File Offset: 0x000053D0
		private static bool IsNameStartChar(char ch)
		{
			return (ch <= 'ÿ' && Utils._xmlNameStartChars[(int)ch]) || (ch >= 'Ā' && ch <= '\u02ff') || (ch >= 'Ͱ' && ch <= 'ͽ') || (ch >= 'Ϳ' && ch <= '\u1fff') || (ch >= '\u200c' && ch <= '\u200d') || (ch >= '⁰' && ch <= '\u218f') || (ch >= 'Ⰰ' && ch <= '\u2fef') || (ch >= '、' && ch <= '\ud7ff') || (ch >= '豈' && ch <= '\ufdcf') || (ch >= 'ﷰ' && ch <= '\ufffd');
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000728C File Offset: 0x0000548C
		private static bool IsNameChar(char ch)
		{
			return (ch <= 'ÿ' && Utils._xmlNameChars[(int)ch]) || Utils.IsNameStartChar(ch) || (ch >= '\u0300' && ch <= '\u036f') || (ch >= '\u203f' && ch <= '\u2040');
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x000072D9 File Offset: 0x000054D9
		public static bool IsNullOrWhiteSpace(string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		// Token: 0x040000E3 RID: 227
		public const int UuidSegmentCount = 4;

		// Token: 0x040000E4 RID: 228
		public const int UuidLength = 36;

		// Token: 0x040000E5 RID: 229
		private static readonly bool[] _xmlNameStartChars = new bool[256];

		// Token: 0x040000E6 RID: 230
		private static readonly bool[] _xmlNameChars = new bool[256];

		// Token: 0x040000E7 RID: 231
		private static readonly HashSet<string> EXTERNAL_XMPDM_PROPS = new HashSet<string>
		{
			"xmpDM:album", "xmpDM:altTapeName", "xmpDM:altTimecode", "xmpDM:artist", "xmpDM:cameraAngle", "xmpDM:cameraLabel", "xmpDM:cameraModel", "xmpDM:cameraMove", "xmpDM:client", "xmpDM:comment",
			"xmpDM:composer", "xmpDM:director", "xmpDM:directorPhotography", "xmpDM:engineer", "xmpDM:genre", "xmpDM:good", "xmpDM:instrument", "xmpDM:logComment", "xmpDM:projectName", "xmpDM:releaseDate",
			"xmpDM:scene", "xmpDM:shotDate", "xmpDM:shotDay", "xmpDM:shotLocation", "xmpDM:shotName", "xmpDM:shotNumber", "xmpDM:shotSize", "xmpDM:speakerPlacement", "xmpDM:takeNumber", "xmpDM:tapeName",
			"xmpDM:trackNumber", "xmpDM:videoAlphaMode", "xmpDM:videoAlphaPremultipleColor"
		};

		// Token: 0x040000E8 RID: 232
		private static readonly char[] BasicEscapeCharacters = new char[] { '<', '>', '&' };

		// Token: 0x040000E9 RID: 233
		private static readonly char[] WhiteSpaceEscapeCharacters = new char[] { '\t', '\n', '\r' };
	}
}
