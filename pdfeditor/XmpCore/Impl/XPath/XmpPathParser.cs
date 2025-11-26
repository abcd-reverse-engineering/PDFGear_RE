using System;

namespace XmpCore.Impl.XPath
{
	// Token: 0x02000043 RID: 67
	public static class XmpPathParser
	{
		// Token: 0x06000311 RID: 785 RVA: 0x0000E93C File Offset: 0x0000CB3C
		public static XmpPath ExpandXPath(string schemaNs, string path)
		{
			if (schemaNs == null || path == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
			XmpPath xmpPath = new XmpPath();
			PathPosition pathPosition = new PathPosition
			{
				Path = path
			};
			XmpPathParser.ParseRootNode(schemaNs, pathPosition, xmpPath);
			while (pathPosition.StepEnd < path.Length)
			{
				pathPosition.StepBegin = pathPosition.StepEnd;
				XmpPathParser.SkipPathDelimiter(path, pathPosition);
				pathPosition.StepEnd = pathPosition.StepBegin;
				XmpPathSegment xmpPathSegment = ((path[pathPosition.StepBegin] != '[') ? XmpPathParser.ParseStructSegment(pathPosition) : XmpPathParser.ParseIndexSegment(pathPosition));
				XmpPathStepType kind = xmpPathSegment.Kind;
				if (kind != XmpPathStepType.StructFieldStep)
				{
					if (kind == XmpPathStepType.FieldSelectorStep)
					{
						if (xmpPathSegment.Name[1] == '@')
						{
							xmpPathSegment.Name = "[?" + xmpPathSegment.Name.Substring(2);
							if (!xmpPathSegment.Name.StartsWith("[?xml:lang="))
							{
								throw new XmpException("Only xml:lang allowed with '@'", XmpErrorCode.BadXPath);
							}
						}
						if (xmpPathSegment.Name[1] == '?')
						{
							pathPosition.NameStart++;
							xmpPathSegment.Kind = XmpPathStepType.QualSelectorStep;
							XmpPathParser.VerifyQualName(pathPosition.Path.Substring(pathPosition.NameStart, pathPosition.NameEnd - pathPosition.NameStart));
						}
					}
				}
				else
				{
					if (xmpPathSegment.Name[0] == '@')
					{
						xmpPathSegment.Name = "?" + xmpPathSegment.Name.Substring(1);
						if (xmpPathSegment.Name != "?xml:lang")
						{
							throw new XmpException("Only xml:lang allowed with '@'", XmpErrorCode.BadXPath);
						}
					}
					if (xmpPathSegment.Name[0] == '?')
					{
						pathPosition.NameStart++;
						xmpPathSegment.Kind = XmpPathStepType.QualifierStep;
					}
					XmpPathParser.VerifyQualName(pathPosition.Path.Substring(pathPosition.NameStart, pathPosition.NameEnd - pathPosition.NameStart));
				}
				xmpPath.Add(xmpPathSegment);
			}
			return xmpPath;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000EB14 File Offset: 0x0000CD14
		private static void SkipPathDelimiter(string path, PathPosition pos)
		{
			if (path[pos.StepBegin] == '/')
			{
				pos.StepBegin++;
				if (pos.StepBegin >= path.Length)
				{
					throw new XmpException("Empty XmpPath segment", XmpErrorCode.BadXPath);
				}
			}
			if (path[pos.StepBegin] == '*')
			{
				pos.StepBegin++;
				if (pos.StepBegin >= path.Length || path[pos.StepBegin] != '[')
				{
					throw new XmpException("Missing '[' after '*'", XmpErrorCode.BadXPath);
				}
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000EBA4 File Offset: 0x0000CDA4
		private static XmpPathSegment ParseStructSegment(PathPosition pos)
		{
			pos.NameStart = pos.StepBegin;
			while (pos.StepEnd < pos.Path.Length && "/[*".IndexOf(pos.Path[pos.StepEnd]) < 0)
			{
				pos.StepEnd++;
			}
			pos.NameEnd = pos.StepEnd;
			if (pos.StepEnd == pos.StepBegin)
			{
				throw new XmpException("Empty XmpPath segment", XmpErrorCode.BadXPath);
			}
			return new XmpPathSegment(pos.Path.Substring(pos.StepBegin, pos.StepEnd - pos.StepBegin), XmpPathStepType.StructFieldStep);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000EC4C File Offset: 0x0000CE4C
		private static XmpPathSegment ParseIndexSegment(PathPosition pos)
		{
			pos.StepEnd++;
			XmpPathSegment xmpPathSegment;
			if ('0' <= pos.Path[pos.StepEnd] && pos.Path[pos.StepEnd] <= '9')
			{
				while (pos.StepEnd < pos.Path.Length && '0' <= pos.Path[pos.StepEnd] && pos.Path[pos.StepEnd] <= '9')
				{
					pos.StepEnd++;
				}
				xmpPathSegment = new XmpPathSegment(null, XmpPathStepType.ArrayIndexStep);
			}
			else
			{
				while (pos.StepEnd < pos.Path.Length && pos.Path[pos.StepEnd] != ']' && pos.Path[pos.StepEnd] != '=')
				{
					pos.StepEnd++;
				}
				if (pos.StepEnd >= pos.Path.Length)
				{
					throw new XmpException("Missing ']' or '=' for array index", XmpErrorCode.BadXPath);
				}
				if (pos.Path[pos.StepEnd] == ']')
				{
					if (pos.Path.Substring(pos.StepBegin, pos.StepEnd - pos.StepBegin) != "[last()")
					{
						throw new XmpException("Invalid non-numeric array index", XmpErrorCode.BadXPath);
					}
					xmpPathSegment = new XmpPathSegment(null, XmpPathStepType.ArrayLastStep);
				}
				else
				{
					pos.NameStart = pos.StepBegin + 1;
					pos.NameEnd = pos.StepEnd;
					pos.StepEnd++;
					char c = pos.Path[pos.StepEnd];
					if (c != '\'' && c != '"')
					{
						throw new XmpException("Invalid quote in array selector", XmpErrorCode.BadXPath);
					}
					pos.StepEnd++;
					while (pos.StepEnd < pos.Path.Length)
					{
						if (pos.Path[pos.StepEnd] == c)
						{
							if (pos.StepEnd + 1 >= pos.Path.Length || pos.Path[pos.StepEnd + 1] != c)
							{
								break;
							}
							pos.StepEnd++;
						}
						pos.StepEnd++;
					}
					if (pos.StepEnd >= pos.Path.Length)
					{
						throw new XmpException("No terminating quote for array selector", XmpErrorCode.BadXPath);
					}
					pos.StepEnd++;
					xmpPathSegment = new XmpPathSegment(null, XmpPathStepType.FieldSelectorStep);
				}
			}
			if (pos.StepEnd >= pos.Path.Length || pos.Path[pos.StepEnd] != ']')
			{
				throw new XmpException("Missing ']' for array index", XmpErrorCode.BadXPath);
			}
			pos.StepEnd++;
			xmpPathSegment.Name = pos.Path.Substring(pos.StepBegin, pos.StepEnd - pos.StepBegin);
			return xmpPathSegment;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000EF1C File Offset: 0x0000D11C
		private static void ParseRootNode(string schemaNs, PathPosition pos, XmpPath expandedXPath)
		{
			while (pos.StepEnd < pos.Path.Length && "/[*".IndexOf(pos.Path[pos.StepEnd]) < 0)
			{
				pos.StepEnd++;
			}
			if (pos.StepEnd == pos.StepBegin)
			{
				throw new XmpException("Empty initial XmpPath step", XmpErrorCode.BadXPath);
			}
			string text = XmpPathParser.VerifyXPathRoot(schemaNs, pos.Path.Substring(pos.StepBegin, pos.StepEnd - pos.StepBegin));
			IXmpAliasInfo xmpAliasInfo = XmpMetaFactory.SchemaRegistry.FindAlias(text);
			if (xmpAliasInfo == null)
			{
				expandedXPath.Add(new XmpPathSegment(schemaNs, XmpPathStepType.SchemaNode));
				XmpPathSegment xmpPathSegment = new XmpPathSegment(text, XmpPathStepType.StructFieldStep);
				expandedXPath.Add(xmpPathSegment);
				return;
			}
			expandedXPath.Add(new XmpPathSegment(xmpAliasInfo.Namespace, XmpPathStepType.SchemaNode));
			expandedXPath.Add(new XmpPathSegment(XmpPathParser.VerifyXPathRoot(xmpAliasInfo.Namespace, xmpAliasInfo.PropName), XmpPathStepType.StructFieldStep)
			{
				IsAlias = true,
				AliasForm = xmpAliasInfo.AliasForm.GetOptions()
			});
			if (xmpAliasInfo.AliasForm.IsArrayAltText)
			{
				expandedXPath.Add(new XmpPathSegment("[?xml:lang='x-default']", XmpPathStepType.QualSelectorStep)
				{
					IsAlias = true,
					AliasForm = xmpAliasInfo.AliasForm.GetOptions()
				});
				return;
			}
			if (xmpAliasInfo.AliasForm.IsArray)
			{
				expandedXPath.Add(new XmpPathSegment("[1]", XmpPathStepType.ArrayIndexStep)
				{
					IsAlias = true,
					AliasForm = xmpAliasInfo.AliasForm.GetOptions()
				});
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000F094 File Offset: 0x0000D294
		private static void VerifyQualName(string qualName)
		{
			int num = qualName.IndexOf(':');
			if (num <= 0)
			{
				throw new XmpException("Ill-formed qualified name", XmpErrorCode.BadXPath);
			}
			string text = qualName.Substring(0, num);
			if (!Utils.IsXmlNameNs(text))
			{
				throw new XmpException("Ill-formed qualified name", XmpErrorCode.BadXPath);
			}
			if (XmpMetaFactory.SchemaRegistry.GetNamespaceUri(text) == null)
			{
				throw new XmpException("Unknown namespace prefix for qualified name", XmpErrorCode.BadXPath);
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000F0F3 File Offset: 0x0000D2F3
		private static void VerifySimpleXmlName(string name)
		{
			if (!Utils.IsXmlName(name))
			{
				throw new XmpException("Bad XML name", XmpErrorCode.BadXPath);
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000F10C File Offset: 0x0000D30C
		private static string VerifyXPathRoot(string schemaNs, string rootProp)
		{
			if (string.IsNullOrEmpty(schemaNs))
			{
				throw new XmpException("Schema namespace URI is required", XmpErrorCode.BadSchema);
			}
			if (rootProp[0] == '?' || rootProp[0] == '@')
			{
				throw new XmpException("Top level name must not be a qualifier", XmpErrorCode.BadXPath);
			}
			if (rootProp.IndexOf('/') >= 0 || rootProp.IndexOf('[') >= 0)
			{
				throw new XmpException("Top level name must be simple", XmpErrorCode.BadXPath);
			}
			string text = XmpMetaFactory.SchemaRegistry.GetNamespacePrefix(schemaNs);
			if (text == null)
			{
				throw new XmpException("Unregistered schema namespace URI", XmpErrorCode.BadSchema);
			}
			int num = rootProp.IndexOf(':');
			if (num < 0)
			{
				XmpPathParser.VerifySimpleXmlName(rootProp);
				return text + rootProp;
			}
			XmpPathParser.VerifySimpleXmlName(rootProp.Substring(0, num));
			XmpPathParser.VerifySimpleXmlName(rootProp.Substring(num));
			text = rootProp.Substring(0, num + 1);
			string namespacePrefix = XmpMetaFactory.SchemaRegistry.GetNamespacePrefix(schemaNs);
			if (namespacePrefix == null)
			{
				throw new XmpException("Unknown schema namespace prefix", XmpErrorCode.BadSchema);
			}
			if (text != namespacePrefix)
			{
				throw new XmpException("Schema namespace URI and prefix mismatch", XmpErrorCode.BadSchema);
			}
			return rootProp;
		}
	}
}
