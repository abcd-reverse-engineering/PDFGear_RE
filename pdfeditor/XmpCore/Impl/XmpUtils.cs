using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpen;
using XmpCore.Impl.XPath;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x02000040 RID: 64
	public static class XmpUtils
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x0000CE44 File Offset: 0x0000B044
		public static string CatenateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string separator, string quotes, bool allowCommas)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			ParameterAsserts.AssertImplementation(xmp);
			if (string.IsNullOrEmpty(separator))
			{
				separator = "; ";
			}
			if (string.IsNullOrEmpty(quotes))
			{
				quotes = "\"";
			}
			XmpMeta xmpMeta = (XmpMeta)xmp;
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(xmpMeta.GetRoot(), xmpPath, false, null);
			if (xmpNode == null)
			{
				return string.Empty;
			}
			if (!xmpNode.Options.IsArray || xmpNode.Options.IsArrayAlternate)
			{
				throw new XmpException("Named property must be non-alternate array", XmpErrorCode.BadParam);
			}
			XmpUtils.CheckSeparator(separator);
			char c = quotes[0];
			char c2 = XmpUtils.CheckQuotes(quotes, c);
			StringBuilder stringBuilder = new StringBuilder();
			IIterator iterator = xmpNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode2 = (XmpNode)iterator.Next();
				if (xmpNode2.Options.IsCompositeProperty)
				{
					throw new XmpException("Array items must be simple", XmpErrorCode.BadParam);
				}
				string text = XmpUtils.ApplyQuotes(xmpNode2.Value, c, c2, allowCommas);
				stringBuilder.Append(text);
				if (iterator.HasNext())
				{
					stringBuilder.Append(separator);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000CF58 File Offset: 0x0000B158
		public static void SeparateArrayItems(IXmpMeta xmp, string schemaNs, string arrayName, string catedStr, PropertyOptions arrayOptions, bool preserveCommas)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			if (catedStr == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
			ParameterAsserts.AssertImplementation(xmp);
			XmpMeta xmpMeta = (XmpMeta)xmp;
			XmpNode xmpNode = XmpUtils.SeparateFindCreateArray(schemaNs, arrayName, arrayOptions, xmpMeta);
			int num = int.MaxValue;
			if (arrayOptions != null)
			{
				num = arrayOptions.ArrayElementsLimit;
				if (num == -1)
				{
					num = int.MaxValue;
				}
			}
			XmpUtils.UnicodeKind unicodeKind = XmpUtils.UnicodeKind.Normal;
			char c = '\0';
			int i = 0;
			int length = catedStr.Length;
			while (i < length && xmpNode.GetChildrenLength() < num)
			{
				int j;
				for (j = i; j < length; j++)
				{
					c = catedStr[j];
					unicodeKind = XmpUtils.ClassifyCharacter(c);
					if (unicodeKind == XmpUtils.UnicodeKind.Normal || unicodeKind == XmpUtils.UnicodeKind.Quote)
					{
						break;
					}
				}
				if (j >= length)
				{
					break;
				}
				string text;
				if (unicodeKind != XmpUtils.UnicodeKind.Quote)
				{
					for (i = j; i < length; i++)
					{
						c = catedStr[i];
						unicodeKind = XmpUtils.ClassifyCharacter(c);
						if (unicodeKind != XmpUtils.UnicodeKind.Normal && unicodeKind != XmpUtils.UnicodeKind.Quote && (unicodeKind != XmpUtils.UnicodeKind.Comma || !preserveCommas))
						{
							if (unicodeKind != XmpUtils.UnicodeKind.Space || i + 1 >= length)
							{
								break;
							}
							c = catedStr[i + 1];
							XmpUtils.UnicodeKind unicodeKind2 = XmpUtils.ClassifyCharacter(c);
							if (unicodeKind2 != XmpUtils.UnicodeKind.Normal && unicodeKind2 != XmpUtils.UnicodeKind.Quote && (unicodeKind2 != XmpUtils.UnicodeKind.Comma || !preserveCommas))
							{
								break;
							}
						}
					}
					text = catedStr.Substring(j, i - j);
				}
				else
				{
					char c2 = c;
					char closingQuote = XmpUtils.GetClosingQuote(c2);
					j++;
					StringBuilder stringBuilder = new StringBuilder();
					for (i = j; i < length; i++)
					{
						c = catedStr[i];
						unicodeKind = XmpUtils.ClassifyCharacter(c);
						if (unicodeKind != XmpUtils.UnicodeKind.Quote || !XmpUtils.IsSurroundingQuote(c, c2, closingQuote))
						{
							stringBuilder.Append(c);
						}
						else
						{
							char c3;
							if (i + 1 < length)
							{
								c3 = catedStr[i + 1];
							}
							else
							{
								c3 = ';';
							}
							if (c == c3)
							{
								stringBuilder.Append(c);
								i++;
							}
							else
							{
								if (XmpUtils.IsClosingQuote(c, c2, closingQuote))
								{
									i++;
									break;
								}
								stringBuilder.Append(c);
							}
						}
					}
					text = stringBuilder.ToString();
				}
				int num2 = -1;
				for (int k = 1; k <= xmpNode.GetChildrenLength(); k++)
				{
					if (xmpNode.GetChild(k).Value == text)
					{
						num2 = k;
						break;
					}
				}
				if (num2 < 0)
				{
					xmpNode.AddChild(new XmpNode("[]", text, null));
				}
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000D19C File Offset: 0x0000B39C
		private static XmpNode SeparateFindCreateArray(string schemaNs, string arrayName, PropertyOptions arrayOptions, XmpMeta xmp)
		{
			arrayOptions = XmpNodeUtils.VerifySetOptions(arrayOptions, null);
			if (!arrayOptions.IsOnlyArrayOptions)
			{
				throw new XmpException("Options can only provide array form", XmpErrorCode.BadOptions);
			}
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(xmp.GetRoot(), xmpPath, false, null);
			if (xmpNode != null)
			{
				PropertyOptions options = xmpNode.Options;
				if (!options.IsArray || options.IsArrayAlternate)
				{
					throw new XmpException("Named property must be non-alternate array", XmpErrorCode.BadXPath);
				}
				if (arrayOptions.EqualArrayTypes(options))
				{
					throw new XmpException("Mismatch of specified and existing array form", XmpErrorCode.BadXPath);
				}
			}
			else
			{
				arrayOptions.IsArray = true;
				xmpNode = XmpNodeUtils.FindNode(xmp.GetRoot(), xmpPath, true, arrayOptions);
				if (xmpNode == null)
				{
					throw new XmpException("Failed to create named array", XmpErrorCode.BadXPath);
				}
			}
			return xmpNode;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000D244 File Offset: 0x0000B444
		public static void RemoveProperties(IXmpMeta xmp, string schemaNs, string propName, bool doAllProperties, bool includeAliases)
		{
			ParameterAsserts.AssertImplementation(xmp);
			XmpMeta xmpMeta = (XmpMeta)xmp;
			if (!string.IsNullOrEmpty(propName))
			{
				if (string.IsNullOrEmpty(schemaNs))
				{
					throw new XmpException("Property name requires schema namespace", XmpErrorCode.BadParam);
				}
				XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
				XmpNode xmpNode = XmpNodeUtils.FindNode(xmpMeta.GetRoot(), xmpPath, false, null);
				if (xmpNode != null && (doAllProperties || !Utils.IsInternalProperty(xmpPath.GetSegment(0).Name, xmpPath.GetSegment(1).Name)))
				{
					XmpNode parent = xmpNode.Parent;
					parent.RemoveChild(xmpNode);
					if (parent.Options.IsSchemaNode && !parent.HasChildren)
					{
						parent.Parent.RemoveChild(parent);
						return;
					}
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(schemaNs))
				{
					XmpNode xmpNode2 = XmpNodeUtils.FindSchemaNode(xmpMeta.GetRoot(), schemaNs, false);
					if (xmpNode2 != null && XmpUtils.RemoveSchemaChildren(xmpNode2, doAllProperties))
					{
						xmpMeta.GetRoot().RemoveChild(xmpNode2);
					}
					if (!includeAliases)
					{
						return;
					}
					using (IEnumerator<IXmpAliasInfo> enumerator = XmpMetaFactory.SchemaRegistry.FindAliases(schemaNs).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IXmpAliasInfo xmpAliasInfo = enumerator.Current;
							XmpPath xmpPath2 = XmpPathParser.ExpandXPath(xmpAliasInfo.Namespace, xmpAliasInfo.PropName);
							XmpNode xmpNode3 = XmpNodeUtils.FindNode(xmpMeta.GetRoot(), xmpPath2, false, null);
							if (xmpNode3 != null)
							{
								xmpNode3.Parent.RemoveChild(xmpNode3);
							}
						}
						return;
					}
				}
				IIterator iterator = xmpMeta.GetRoot().IterateChildren();
				while (iterator.HasNext())
				{
					if (XmpUtils.RemoveSchemaChildren((XmpNode)iterator.Next(), doAllProperties))
					{
						iterator.Remove();
					}
				}
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000D3E0 File Offset: 0x0000B5E0
		public static void AppendProperties(IXmpMeta source, IXmpMeta destination, bool doAllProperties, bool replaceOldValues, bool deleteEmptyValues)
		{
			ParameterAsserts.AssertImplementation(source);
			ParameterAsserts.AssertImplementation(destination);
			XmpMeta xmpMeta = (XmpMeta)source;
			XmpMeta xmpMeta2 = (XmpMeta)destination;
			IIterator iterator = xmpMeta.GetRoot().IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				XmpNode xmpNode2 = XmpNodeUtils.FindSchemaNode(xmpMeta2.GetRoot(), xmpNode.Name, false);
				bool flag = false;
				if (xmpNode2 == null)
				{
					xmpNode2 = new XmpNode(xmpNode.Name, xmpNode.Value, new PropertyOptions
					{
						IsSchemaNode = true
					});
					xmpMeta2.GetRoot().AddChild(xmpNode2);
					flag = true;
				}
				IIterator iterator2 = xmpNode.IterateChildren();
				while (iterator2.HasNext())
				{
					XmpNode xmpNode3 = (XmpNode)iterator2.Next();
					if (doAllProperties || !Utils.IsInternalProperty(xmpNode.Name, xmpNode3.Name))
					{
						XmpUtils.AppendSubtree(xmpMeta2, xmpNode3, xmpNode2, false, replaceOldValues, deleteEmptyValues);
					}
				}
				if (!xmpNode2.HasChildren && (flag || deleteEmptyValues))
				{
					xmpMeta2.GetRoot().RemoveChild(xmpNode2);
				}
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000D4D4 File Offset: 0x0000B6D4
		private static bool RemoveSchemaChildren(XmpNode schemaNode, bool doAllProperties)
		{
			IIterator iterator = schemaNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (doAllProperties || !Utils.IsInternalProperty(schemaNode.Name, xmpNode.Name))
				{
					iterator.Remove();
				}
			}
			return !schemaNode.HasChildren;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000D524 File Offset: 0x0000B724
		private static void AppendSubtree(XmpMeta destXmp, XmpNode sourceNode, XmpNode destParent, bool mergeCompound, bool replaceOldValues, bool deleteEmptyValues)
		{
			XmpNode xmpNode = XmpNodeUtils.FindChildNode(destParent, sourceNode.Name, false);
			if (sourceNode.Options.IsSimple ? string.IsNullOrEmpty(sourceNode.Value) : (!sourceNode.HasChildren))
			{
				if (deleteEmptyValues && xmpNode != null)
				{
					destParent.RemoveChild(xmpNode);
				}
				return;
			}
			if (xmpNode == null)
			{
				XmpNode xmpNode2 = (XmpNode)sourceNode.Clone(true);
				if (xmpNode2 != null)
				{
					destParent.AddChild(xmpNode2);
				}
				return;
			}
			PropertyOptions options = sourceNode.Options;
			bool flag = replaceOldValues;
			if (mergeCompound && !options.IsSimple)
			{
				flag = false;
			}
			if (flag)
			{
				destParent.RemoveChild(xmpNode);
				XmpNode xmpNode3 = (XmpNode)sourceNode.Clone(true);
				if (xmpNode3 != null)
				{
					destParent.AddChild(xmpNode3);
				}
				return;
			}
			PropertyOptions options2 = xmpNode.Options;
			if (options.GetOptions() != options2.GetOptions() || options.IsSimple)
			{
				return;
			}
			if (options.IsStruct)
			{
				IIterator iterator = sourceNode.IterateChildren();
				while (iterator.HasNext())
				{
					XmpNode xmpNode4 = (XmpNode)iterator.Next();
					XmpUtils.AppendSubtree(destXmp, xmpNode4, xmpNode, mergeCompound, replaceOldValues, deleteEmptyValues);
					if (deleteEmptyValues && !xmpNode.HasChildren)
					{
						destParent.RemoveChild(xmpNode);
					}
				}
				return;
			}
			if (options.IsArrayAltText)
			{
				IIterator iterator2 = sourceNode.IterateChildren();
				while (iterator2.HasNext())
				{
					XmpNode xmpNode5 = (XmpNode)iterator2.Next();
					if (xmpNode5.HasQualifier && !(xmpNode5.GetQualifier(1).Name != "xml:lang"))
					{
						int num = XmpNodeUtils.LookupLanguageItem(xmpNode, xmpNode5.GetQualifier(1).Value);
						if (string.IsNullOrEmpty(xmpNode5.Value))
						{
							if (deleteEmptyValues && num != -1)
							{
								xmpNode.RemoveChild(num);
								if (!xmpNode.HasChildren)
								{
									destParent.RemoveChild(xmpNode);
								}
							}
						}
						else if (num == -1)
						{
							if (xmpNode5.GetQualifier(1).Value != "x-default" || !xmpNode.HasChildren)
							{
								XmpNode xmpNode6 = (XmpNode)xmpNode5.Clone(true);
								if (xmpNode6 != null)
								{
									xmpNode.AddChild(xmpNode6);
								}
							}
							else
							{
								XmpNode xmpNode7 = new XmpNode(xmpNode5.Name, xmpNode5.Value, xmpNode5.Options);
								xmpNode5.CloneSubtree(xmpNode7, true);
								xmpNode.AddChild(1, xmpNode7);
							}
						}
						else if (replaceOldValues)
						{
							xmpNode.GetChild(num).Value = xmpNode5.Value;
						}
					}
				}
				return;
			}
			if (options.IsArray)
			{
				IIterator iterator3 = sourceNode.IterateChildren();
				while (iterator3.HasNext())
				{
					XmpNode xmpNode8 = (XmpNode)iterator3.Next();
					bool flag2 = false;
					IIterator iterator4 = xmpNode.IterateChildren();
					while (iterator4.HasNext())
					{
						XmpNode xmpNode9 = (XmpNode)iterator4.Next();
						if (XmpUtils.ItemValuesMatch(xmpNode8, xmpNode9))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						XmpNode xmpNode10 = (XmpNode)xmpNode8.Clone(true);
						if (xmpNode10 != null)
						{
							xmpNode.AddChild(xmpNode10);
						}
					}
				}
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000D7EC File Offset: 0x0000B9EC
		private static bool ItemValuesMatch(XmpNode leftNode, XmpNode rightNode)
		{
			PropertyOptions options = leftNode.Options;
			PropertyOptions options2 = rightNode.Options;
			if (!options.Equals(options2))
			{
				return false;
			}
			if (options.IsSimple)
			{
				if (leftNode.Value != rightNode.Value)
				{
					return false;
				}
				if (leftNode.Options.HasLanguage != rightNode.Options.HasLanguage)
				{
					return false;
				}
				if (leftNode.Options.HasLanguage && leftNode.GetQualifier(1).Value != rightNode.GetQualifier(1).Value)
				{
					return false;
				}
			}
			else if (options.IsStruct)
			{
				if (leftNode.GetChildrenLength() != rightNode.GetChildrenLength())
				{
					return false;
				}
				IIterator iterator = leftNode.IterateChildren();
				while (iterator.HasNext())
				{
					XmpNode xmpNode = (XmpNode)iterator.Next();
					XmpNode xmpNode2 = XmpNodeUtils.FindChildNode(rightNode, xmpNode.Name, false);
					if (xmpNode2 == null || !XmpUtils.ItemValuesMatch(xmpNode, xmpNode2))
					{
						return false;
					}
				}
			}
			else
			{
				IIterator iterator2 = leftNode.IterateChildren();
				while (iterator2.HasNext())
				{
					XmpNode xmpNode3 = (XmpNode)iterator2.Next();
					bool flag = false;
					IIterator iterator3 = rightNode.IterateChildren();
					while (iterator3.HasNext())
					{
						XmpNode xmpNode4 = (XmpNode)iterator3.Next();
						if (XmpUtils.ItemValuesMatch(xmpNode3, xmpNode4))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000D930 File Offset: 0x0000BB30
		public static void DuplicateSubtree(IXmpMeta source, IXmpMeta dest, string sourceNS, string sourceRoot, string destNS, string destRoot, PropertyOptions options)
		{
			bool flag = false;
			bool flag2 = false;
			ParameterAsserts.AssertNotNull(source);
			ParameterAsserts.AssertSchemaNs(sourceNS);
			ParameterAsserts.AssertSchemaNs(sourceRoot);
			ParameterAsserts.AssertNotNull(dest);
			ParameterAsserts.AssertNotNull(destNS);
			ParameterAsserts.AssertNotNull(destRoot);
			if (destNS.Length == 0)
			{
				destNS = sourceNS;
			}
			if (destRoot.Length == 0)
			{
				destRoot = sourceRoot;
			}
			if (sourceNS == "*")
			{
				flag = true;
			}
			if (destNS == "*")
			{
				flag2 = true;
			}
			if (source == dest && (flag || flag2))
			{
				throw new XmpException("Can't duplicate tree onto itself", XmpErrorCode.BadParam);
			}
			if (flag && flag2)
			{
				throw new XmpException("Use Clone for full tree to full tree", XmpErrorCode.BadParam);
			}
			if (flag)
			{
				XmpPath xmpPath = XmpPathParser.ExpandXPath(destNS, destRoot);
				XmpNode xmpNode = XmpNodeUtils.FindNode(((XmpMeta)dest).GetRoot(), xmpPath, false, null);
				if (xmpNode == null || !xmpNode.Options.IsStruct)
				{
					throw new XmpException("Destination must be an existing struct", XmpErrorCode.BadXPath);
				}
				if (xmpNode.HasChildren)
				{
					if (options == null || (options.GetOptions() & 536870912) == 0)
					{
						throw new XmpException("Destination must be an empty struct", XmpErrorCode.BadXPath);
					}
					xmpNode.RemoveChildren();
				}
				XmpMeta xmpMeta = (XmpMeta)source;
				int i = 1;
				int childrenLength = xmpMeta.GetRoot().GetChildrenLength();
				while (i <= childrenLength)
				{
					XmpNode child = xmpMeta.GetRoot().GetChild(i);
					int j = 1;
					int childrenLength2 = child.GetChildrenLength();
					while (j <= childrenLength2)
					{
						XmpNode xmpNode2 = child.GetChild(j);
						xmpNode.AddChild((XmpNode)xmpNode2.Clone(false));
						j++;
					}
					i++;
				}
				return;
			}
			else if (flag2)
			{
				XmpMeta xmpMeta2 = (XmpMeta)source;
				XmpMeta xmpMeta3 = (XmpMeta)dest;
				XmpPath xmpPath2 = XmpPathParser.ExpandXPath(sourceNS, sourceRoot);
				XmpNode xmpNode2 = XmpNodeUtils.FindNode(xmpMeta2.GetRoot(), xmpPath2, false, null);
				if (xmpNode2 == null || !xmpNode2.Options.IsStruct)
				{
					throw new XmpException("Source must be an existing struct", XmpErrorCode.BadXPath);
				}
				XmpNode xmpNode = xmpMeta3.GetRoot();
				if (xmpNode.HasChildren)
				{
					if (options == null || (options.GetOptions() & 536870912) == 0)
					{
						throw new XmpException("Source must be an existing struct", XmpErrorCode.BadXPath);
					}
					xmpNode.RemoveChildren();
				}
				int k = 1;
				int childrenLength3 = xmpNode2.GetChildrenLength();
				while (k <= childrenLength3)
				{
					XmpNode child2 = xmpNode2.GetChild(k);
					int num = child2.Name.IndexOf(':');
					if (num != -1)
					{
						string text = child2.Name.Substring(0, num + 1);
						string namespaceUri = XmpMetaFactory.SchemaRegistry.GetNamespaceUri(text);
						if (namespaceUri == null)
						{
							throw new XmpException("Source field namespace is not global", XmpErrorCode.BadSchema);
						}
						XmpNode xmpNode3 = XmpNodeUtils.FindSchemaNode(xmpMeta3.GetRoot(), namespaceUri, true);
						if (xmpNode3 == null)
						{
							throw new XmpException("Failed to find destination schema", XmpErrorCode.BadSchema);
						}
						xmpNode3.AddChild((XmpNode)child2.Clone(false));
					}
					k++;
				}
				return;
			}
			else
			{
				XmpPath xmpPath2 = XmpPathParser.ExpandXPath(sourceNS, sourceRoot);
				XmpPath xmpPath = XmpPathParser.ExpandXPath(destNS, destRoot);
				XmpMeta xmpMeta4 = (XmpMeta)source;
				XmpMeta xmpMeta5 = (XmpMeta)dest;
				XmpNode xmpNode2 = XmpNodeUtils.FindNode(xmpMeta4.GetRoot(), xmpPath2, false, null);
				if (xmpNode2 == null)
				{
					throw new XmpException("Can't find source subtree", XmpErrorCode.BadXPath);
				}
				XmpNode xmpNode = XmpNodeUtils.FindNode(xmpMeta5.GetRoot(), xmpPath, false, null);
				if (xmpNode != null)
				{
					throw new XmpException("Destination subtree must not exist", XmpErrorCode.BadXPath);
				}
				xmpNode = XmpNodeUtils.FindNode(xmpMeta5.GetRoot(), xmpPath, true, null);
				if (xmpNode == null)
				{
					throw new XmpException("Can't create destination root node", XmpErrorCode.BadXPath);
				}
				if (source == dest)
				{
					for (XmpNode xmpNode4 = xmpNode; xmpNode4 != null; xmpNode4 = xmpNode4.Parent)
					{
						if (xmpNode4 == xmpNode2)
						{
							throw new XmpException("Destination subtree is within the source subtree", XmpErrorCode.BadXPath);
						}
					}
				}
				xmpNode.Value = xmpNode2.Value;
				xmpNode.Options = xmpNode2.Options;
				xmpNode2.CloneSubtree(xmpNode, false);
				return;
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000DCA8 File Offset: 0x0000BEA8
		private static void CheckSeparator(string separator)
		{
			bool flag = false;
			for (int i = 0; i < separator.Length; i++)
			{
				XmpUtils.UnicodeKind unicodeKind = XmpUtils.ClassifyCharacter(separator[i]);
				if (unicodeKind == XmpUtils.UnicodeKind.Semicolon)
				{
					if (flag)
					{
						throw new XmpException("Separator can have only one semicolon", XmpErrorCode.BadParam);
					}
					flag = true;
				}
				else if (unicodeKind != XmpUtils.UnicodeKind.Space)
				{
					throw new XmpException("Separator can have only spaces and one semicolon", XmpErrorCode.BadParam);
				}
			}
			if (!flag)
			{
				throw new XmpException("Separator must have one semicolon", XmpErrorCode.BadParam);
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0000DD10 File Offset: 0x0000BF10
		private static char CheckQuotes(string quotes, char openQuote)
		{
			if (XmpUtils.ClassifyCharacter(openQuote) != XmpUtils.UnicodeKind.Quote)
			{
				throw new XmpException("Invalid quoting character", XmpErrorCode.BadParam);
			}
			char c;
			if (quotes.Length == 1)
			{
				c = openQuote;
			}
			else
			{
				c = quotes[1];
				if (XmpUtils.ClassifyCharacter(c) != XmpUtils.UnicodeKind.Quote)
				{
					throw new XmpException("Invalid quoting character", XmpErrorCode.BadParam);
				}
			}
			if (c != XmpUtils.GetClosingQuote(openQuote))
			{
				throw new XmpException("Mismatched quote pair", XmpErrorCode.BadParam);
			}
			return c;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000DD74 File Offset: 0x0000BF74
		private static XmpUtils.UnicodeKind ClassifyCharacter(char ch)
		{
			if (" \u3000〿".IndexOf(ch) >= 0 || ('\u2000' <= ch && ch <= '\u200b'))
			{
				return XmpUtils.UnicodeKind.Space;
			}
			if (",，､﹐﹑、،՝".IndexOf(ch) >= 0)
			{
				return XmpUtils.UnicodeKind.Comma;
			}
			if (";；﹔؛;".IndexOf(ch) >= 0)
			{
				return XmpUtils.UnicodeKind.Semicolon;
			}
			if ("\"«»〝〞〟―‹›".IndexOf(ch) >= 0 || ('〈' <= ch && ch <= '』') || ('‘' <= ch && ch <= '‟'))
			{
				return XmpUtils.UnicodeKind.Quote;
			}
			if (ch < ' ' || "\u2028\u2029".IndexOf(ch) >= 0)
			{
				return XmpUtils.UnicodeKind.Control;
			}
			return XmpUtils.UnicodeKind.Normal;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000DE08 File Offset: 0x0000C008
		private static char GetClosingQuote(char openQuote)
		{
			if (openQuote <= '―')
			{
				if (openQuote <= '«')
				{
					if (openQuote == '"')
					{
						return '"';
					}
					if (openQuote == '«')
					{
						return '»';
					}
				}
				else
				{
					if (openQuote == '»')
					{
						return '«';
					}
					if (openQuote == '―')
					{
						return '―';
					}
				}
			}
			else if (openQuote <= '‹')
			{
				switch (openQuote)
				{
				case '‘':
					return '’';
				case '’':
				case '‛':
				case '”':
					break;
				case '‚':
					return '‛';
				case '“':
					return '”';
				case '„':
					return '‟';
				default:
					if (openQuote == '‹')
					{
						return '›';
					}
					break;
				}
			}
			else
			{
				if (openQuote == '›')
				{
					return '‹';
				}
				switch (openQuote)
				{
				case '〈':
					return '〉';
				case '〉':
				case '》':
				case '」':
					break;
				case '《':
					return '》';
				case '「':
					return '」';
				case '『':
					return '』';
				default:
					if (openQuote == '〝')
					{
						return '〟';
					}
					break;
				}
			}
			return '\0';
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000DF28 File Offset: 0x0000C128
		private static string ApplyQuotes(string item, char openQuote, char closeQuote, bool allowCommas)
		{
			if (item == null)
			{
				item = string.Empty;
			}
			bool flag = false;
			int i;
			for (i = 0; i < item.Length; i++)
			{
				XmpUtils.UnicodeKind unicodeKind = XmpUtils.ClassifyCharacter(item[i]);
				if (i == 0 && unicodeKind == XmpUtils.UnicodeKind.Quote)
				{
					break;
				}
				if (unicodeKind == XmpUtils.UnicodeKind.Space)
				{
					if (flag)
					{
						break;
					}
					flag = true;
				}
				else
				{
					flag = false;
					if (unicodeKind == XmpUtils.UnicodeKind.Semicolon || unicodeKind == XmpUtils.UnicodeKind.Control || (unicodeKind == XmpUtils.UnicodeKind.Comma && !allowCommas))
					{
						break;
					}
				}
			}
			if (i < item.Length)
			{
				StringBuilder stringBuilder = new StringBuilder(item.Length + 2);
				int num = 0;
				while (num <= i && XmpUtils.ClassifyCharacter(item[i]) != XmpUtils.UnicodeKind.Quote)
				{
					num++;
				}
				stringBuilder.Append(openQuote).Append(item.Substring(0, num));
				for (int j = num; j < item.Length; j++)
				{
					stringBuilder.Append(item[j]);
					if (XmpUtils.ClassifyCharacter(item[j]) == XmpUtils.UnicodeKind.Quote && XmpUtils.IsSurroundingQuote(item[j], openQuote, closeQuote))
					{
						stringBuilder.Append(item[j]);
					}
				}
				stringBuilder.Append(closeQuote);
				item = stringBuilder.ToString();
			}
			return item;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000E037 File Offset: 0x0000C237
		private static bool IsSurroundingQuote(char ch, char openQuote, char closeQuote)
		{
			return ch == openQuote || XmpUtils.IsClosingQuote(ch, openQuote, closeQuote);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000E047 File Offset: 0x0000C247
		private static bool IsClosingQuote(char ch, char openQuote, char closeQuote)
		{
			return ch == closeQuote || (openQuote == '〝' && ch == '〞') || ch == '〟';
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000E068 File Offset: 0x0000C268
		private static bool MoveOneProperty(XmpMeta stdXMP, XmpMeta extXMP, string schemaURI, string propName)
		{
			XmpNode xmpNode = null;
			XmpNode xmpNode2 = XmpNodeUtils.FindSchemaNode(stdXMP.GetRoot(), schemaURI, false);
			if (xmpNode2 != null)
			{
				xmpNode = XmpNodeUtils.FindChildNode(xmpNode2, propName, false);
			}
			if (xmpNode == null)
			{
				return false;
			}
			XmpNode xmpNode3 = XmpNodeUtils.FindSchemaNode(extXMP.GetRoot(), schemaURI, true);
			xmpNode.Parent = xmpNode3;
			xmpNode3.IsImplicit = false;
			xmpNode3.AddChild(xmpNode);
			xmpNode2.RemoveChild(xmpNode);
			if (!xmpNode2.HasChildren)
			{
				xmpNode2.Parent.RemoveChild(xmpNode2);
			}
			return true;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000E0D8 File Offset: 0x0000C2D8
		private static int EstimateSizeForJPEG(XmpNode xmpNode)
		{
			int num = 0;
			int length = xmpNode.Name.Length;
			bool flag = !xmpNode.Options.IsArray;
			if (xmpNode.Options.IsSimple)
			{
				if (flag)
				{
					num += length + 3;
				}
				num += xmpNode.Value.Length;
			}
			else if (xmpNode.Options.IsArray)
			{
				if (flag)
				{
					num += 2 * length + 5;
				}
				int childrenLength = xmpNode.GetChildrenLength();
				num += 19;
				num += childrenLength * 17;
				for (int i = 1; i <= childrenLength; i++)
				{
					num += XmpUtils.EstimateSizeForJPEG(xmpNode.GetChild(i));
				}
			}
			else
			{
				if (flag)
				{
					num += 2 * length + 5;
				}
				num += 25;
				int childrenLength2 = xmpNode.GetChildrenLength();
				for (int j = 1; j <= childrenLength2; j++)
				{
					num += XmpUtils.EstimateSizeForJPEG(xmpNode.GetChild(j));
				}
			}
			return num;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
		private static void PutObjectsInMultiMap(SortedDictionary<int, List<List<string>>> multiMap, int key, List<string> stringPair)
		{
			if (multiMap == null)
			{
				return;
			}
			List<List<string>> list;
			if (!multiMap.TryGetValue(key, out list))
			{
				list = new List<List<string>>();
				multiMap[key] = list;
			}
			list.Add(stringPair);
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
		private static List<string> GetBiggestEntryInMultiMap(SortedDictionary<int, List<List<string>>> multiMap)
		{
			if (multiMap == null || multiMap.Count == 0)
			{
				return null;
			}
			List<List<string>> list = multiMap[multiMap.Keys.Last<int>()];
			List<string> list2 = list[0];
			list.RemoveAt(0);
			if (list.Count == 0)
			{
				multiMap.Remove(multiMap.Keys.Last<int>());
			}
			return list2;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000E23C File Offset: 0x0000C43C
		private static void CreateEstimatedSizeMap(XmpMeta stdXMP, SortedDictionary<int, List<List<string>>> propSizes)
		{
			for (int i = stdXMP.GetRoot().GetChildrenLength(); i > 0; i--)
			{
				XmpNode child = stdXMP.GetRoot().GetChild(i);
				for (int j = child.GetChildrenLength(); j > 0; j--)
				{
					XmpNode child2 = child.GetChild(j);
					if (!child.Name.Equals("http://ns.adobe.com/xmp/note/") || !child2.Name.Equals("xmpNote:HasExtendedXMP"))
					{
						int num = XmpUtils.EstimateSizeForJPEG(child2);
						List<string> list = new List<string> { child.Name, child2.Name };
						XmpUtils.PutObjectsInMultiMap(propSizes, num, list);
					}
				}
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000E2E0 File Offset: 0x0000C4E0
		private static int MoveLargestProperty(XmpMeta stdXMP, XmpMeta extXMP, SortedDictionary<int, List<List<string>>> propSizes)
		{
			int num = propSizes.Keys.Last<int>();
			List<string> biggestEntryInMultiMap = XmpUtils.GetBiggestEntryInMultiMap(propSizes);
			XmpUtils.MoveOneProperty(stdXMP, extXMP, biggestEntryInMultiMap[0], biggestEntryInMultiMap[1]);
			return num;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0000E318 File Offset: 0x0000C518
		public static void PackageForJPEG(IXmpMeta origXMPImpl, StringBuilder stdStr, StringBuilder extStr, StringBuilder digestStr)
		{
			XmpMeta xmpMeta = (XmpMeta)origXMPImpl;
			int length = "<?xpacket end=\"w\"?>".Length;
			XmpMeta xmpMeta2 = new XmpMeta();
			XmpMeta xmpMeta3 = new XmpMeta();
			SerializeOptions serializeOptions = new SerializeOptions(64)
			{
				Padding = 0,
				Indent = "",
				BaseIndent = 0,
				Newline = " "
			};
			string text = XmpMetaFactory.SerializeToString(xmpMeta, serializeOptions);
			if (text.Length > 65000)
			{
				xmpMeta2.GetRoot().Options = xmpMeta.GetRoot().Options;
				xmpMeta2.GetRoot().Name = xmpMeta.GetRoot().Name;
				xmpMeta2.GetRoot().Value = xmpMeta.GetRoot().Value;
				xmpMeta.GetRoot().CloneSubtree(xmpMeta2.GetRoot(), false);
				if (xmpMeta2.DoesPropertyExist("http://ns.adobe.com/xap/1.0/", "Thumbnails"))
				{
					xmpMeta2.DeleteProperty("http://ns.adobe.com/xap/1.0/", "Thumbnails");
					text = XmpMetaFactory.SerializeToString(xmpMeta2, serializeOptions);
				}
			}
			if (text.Length > 65000)
			{
				xmpMeta2.SetProperty("http://ns.adobe.com/xmp/note/", "HasExtendedXMP", "123456789-123456789-123456789-12", new PropertyOptions(0));
				XmpNode xmpNode = XmpNodeUtils.FindSchemaNode(xmpMeta2.GetRoot(), "http://ns.adobe.com/camera-raw-settings/1.0/", false);
				if (xmpNode != null)
				{
					xmpNode.Parent = xmpMeta3.GetRoot();
					xmpMeta3.GetRoot().AddChild(xmpNode);
					xmpMeta2.GetRoot().RemoveChild(xmpNode);
					text = XmpMetaFactory.SerializeToString(xmpMeta2, serializeOptions);
				}
			}
			if (text.Length > 65000 && XmpUtils.MoveOneProperty(xmpMeta2, xmpMeta3, "http://ns.adobe.com/photoshop/1.0/", "photoshop:History"))
			{
				text = XmpMetaFactory.SerializeToString(xmpMeta2, serializeOptions);
			}
			if (text.Length > 65000)
			{
				SortedDictionary<int, List<List<string>>> sortedDictionary = new SortedDictionary<int, List<List<string>>>();
				XmpUtils.CreateEstimatedSizeMap(xmpMeta2, sortedDictionary);
				while (text.Length > 65000 && sortedDictionary.Count != 0)
				{
					int num = text.Length;
					while (num > 65000 && sortedDictionary.Count != 0)
					{
						int num2 = XmpUtils.MoveLargestProperty(xmpMeta2, xmpMeta3, sortedDictionary);
						if (num2 > num)
						{
							num2 = num;
						}
						num -= num2;
					}
					text = XmpMetaFactory.SerializeToString(xmpMeta2, serializeOptions);
				}
			}
			if (text.Length > 65000)
			{
				throw new XmpException("Can't reduce XMP enough for JPEG file", XmpErrorCode.InternalFailure);
			}
			if (xmpMeta3.GetRoot().GetChildrenLength() == 0)
			{
				stdStr.Append(text);
			}
			else
			{
				text = XmpMetaFactory.SerializeToString(xmpMeta3, new SerializeOptions(80));
				extStr.Append(text);
				xmpMeta2.SetProperty("http://ns.adobe.com/xmp/note/", "HasExtendedXMP", digestStr.ToString(), new PropertyOptions(0));
				text = XmpMetaFactory.SerializeToString(xmpMeta2, serializeOptions);
				stdStr.Append(text);
			}
			int num3 = 65000 - stdStr.Length;
			if (num3 > 2047)
			{
				num3 = 2047;
			}
			int num4 = stdStr.ToString().IndexOf("<?xpacket end=\"w\"?>");
			int num5 = stdStr.Length - num4;
			stdStr.Remove(num4, num5);
			stdStr.Append(' ', num3);
			stdStr.Append("<?xpacket end=\"w\"?>").ToString();
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0000E5F4 File Offset: 0x0000C7F4
		public static void MergeFromJPEG(IXmpMeta fullXMP, IXmpMeta extendedXMP)
		{
			TemplateOptions templateOptions = new TemplateOptions(48);
			XmpUtils.ApplyTemplate((XmpMeta)fullXMP, (XmpMeta)extendedXMP, templateOptions);
			fullXMP.DeleteProperty("http://ns.adobe.com/xmp/note/", "HasExtendedXMP");
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000E62C File Offset: 0x0000C82C
		public static void ApplyTemplate(IXmpMeta origXMP, IXmpMeta tempXMP, TemplateOptions actions)
		{
			XmpMeta xmpMeta = (XmpMeta)origXMP;
			XmpMeta xmpMeta2 = (XmpMeta)tempXMP;
			bool flag = (actions.GetOptions() & 2) != 0;
			bool flag2 = (actions.GetOptions() & 64) != 0;
			bool flag3 = (actions.GetOptions() & 16) != 0;
			bool flag4 = (actions.GetOptions() & 128) != 0;
			flag3 = flag3 || flag4;
			flag4 &= !flag;
			bool flag5 = (actions.GetOptions() & 32) != 0;
			if (flag)
			{
				for (int i = xmpMeta.GetRoot().GetChildrenLength(); i > 0; i--)
				{
					XmpNode child = xmpMeta.GetRoot().GetChild(i);
					XmpNode xmpNode = XmpNodeUtils.FindSchemaNode(xmpMeta2.GetRoot(), child.Name, false);
					if (xmpNode == null)
					{
						if (flag5)
						{
							child.RemoveChildren();
						}
						else
						{
							for (int j = child.GetChildrenLength(); j > 0; j--)
							{
								XmpNode child2 = child.GetChild(j);
								if (!Utils.IsInternalProperty(child.Name, child2.Name))
								{
									child.RemoveChild(j);
								}
							}
						}
					}
					else
					{
						for (int k = child.GetChildrenLength(); k > 0; k--)
						{
							XmpNode child3 = child.GetChild(k);
							if ((flag5 || !Utils.IsInternalProperty(child.Name, child3.Name)) && XmpNodeUtils.FindChildNode(xmpNode, child3.Name, false) == null)
							{
								child.RemoveChild(k);
							}
						}
					}
					if (!child.HasChildren)
					{
						xmpMeta.GetRoot().RemoveChild(i);
					}
				}
			}
			if (flag2 || flag3)
			{
				int l = 0;
				int childrenLength = xmpMeta2.GetRoot().GetChildrenLength();
				while (l < childrenLength)
				{
					XmpNode child4 = xmpMeta2.GetRoot().GetChild(l + 1);
					XmpNode xmpNode2 = XmpNodeUtils.FindSchemaNode(xmpMeta.GetRoot(), child4.Name, false);
					if (xmpNode2 == null)
					{
						xmpNode2 = new XmpNode(child4.Name, child4.Value, new PropertyOptions(int.MinValue));
						xmpMeta.GetRoot().AddChild(xmpNode2);
						xmpNode2.Parent = xmpMeta.GetRoot();
					}
					int m = 1;
					int childrenLength2 = child4.GetChildrenLength();
					while (m <= childrenLength2)
					{
						XmpNode child5 = child4.GetChild(m);
						if (flag5 || !Utils.IsInternalProperty(child4.Name, child5.Name))
						{
							XmpUtils.AppendSubtree(xmpMeta, child5, xmpNode2, flag2, flag3, flag4);
						}
						m++;
					}
					if (!xmpNode2.HasChildren)
					{
						xmpMeta.GetRoot().RemoveChild(xmpNode2);
					}
					l++;
				}
			}
		}

		// Token: 0x0400012A RID: 298
		private const string Spaces = " \u3000〿";

		// Token: 0x0400012B RID: 299
		private const string Commas = ",，､﹐﹑、،՝";

		// Token: 0x0400012C RID: 300
		private const string Semicola = ";；﹔؛;";

		// Token: 0x0400012D RID: 301
		private const string Quotes = "\"«»〝〞〟―‹›";

		// Token: 0x0400012E RID: 302
		private const string Controls = "\u2028\u2029";

		// Token: 0x020002D3 RID: 723
		private enum UnicodeKind
		{
			// Token: 0x040011B9 RID: 4537
			Normal,
			// Token: 0x040011BA RID: 4538
			Space,
			// Token: 0x040011BB RID: 4539
			Comma,
			// Token: 0x040011BC RID: 4540
			Semicolon,
			// Token: 0x040011BD RID: 4541
			Quote,
			// Token: 0x040011BE RID: 4542
			Control
		}
	}
}
