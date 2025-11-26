using System;
using Sharpen;
using XmpCore.Impl.XPath;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003B RID: 59
	public static class XmpNodeUtils
	{
		// Token: 0x060002A6 RID: 678 RVA: 0x000097E2 File Offset: 0x000079E2
		internal static XmpNode FindSchemaNode(XmpNode tree, string namespaceUri, bool createNodes)
		{
			return XmpNodeUtils.FindSchemaNode(tree, namespaceUri, null, createNodes);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000097F0 File Offset: 0x000079F0
		internal static XmpNode FindSchemaNode(XmpNode tree, string namespaceUri, string suggestedPrefix, bool createNodes)
		{
			XmpNode xmpNode = tree.FindChildByName(namespaceUri);
			if (xmpNode == null && createNodes)
			{
				PropertyOptions propertyOptions = new PropertyOptions
				{
					IsSchemaNode = true
				};
				xmpNode = new XmpNode(namespaceUri, propertyOptions)
				{
					IsImplicit = true
				};
				string text = XmpMetaFactory.SchemaRegistry.GetNamespacePrefix(namespaceUri);
				if (text == null)
				{
					if (string.IsNullOrEmpty(suggestedPrefix))
					{
						throw new XmpException("Unregistered schema namespace URI", XmpErrorCode.BadSchema);
					}
					text = XmpMetaFactory.SchemaRegistry.RegisterNamespace(namespaceUri, suggestedPrefix);
				}
				xmpNode.Value = text;
				tree.AddChild(xmpNode);
			}
			return xmpNode;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000986C File Offset: 0x00007A6C
		internal static XmpNode FindChildNode(XmpNode parent, string childName, bool createNodes)
		{
			if (!parent.Options.IsSchemaNode && !parent.Options.IsStruct)
			{
				if (!parent.IsImplicit)
				{
					throw new XmpException("Named children only allowed for schemas and structs", XmpErrorCode.BadXPath);
				}
				if (parent.Options.IsArray)
				{
					throw new XmpException("Named children not allowed for arrays", XmpErrorCode.BadXPath);
				}
				if (createNodes)
				{
					parent.Options.IsStruct = true;
				}
			}
			XmpNode xmpNode = parent.FindChildByName(childName);
			if (xmpNode == null && createNodes)
			{
				PropertyOptions propertyOptions = new PropertyOptions();
				xmpNode = new XmpNode(childName, propertyOptions)
				{
					IsImplicit = true
				};
				parent.AddChild(xmpNode);
			}
			return xmpNode;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00009900 File Offset: 0x00007B00
		internal static XmpNode FindNode(XmpNode xmpTree, XmpPath xpath, bool createNodes, PropertyOptions leafOptions)
		{
			if (xpath == null || xpath.Size() == 0)
			{
				throw new XmpException("Empty XmpPath", XmpErrorCode.BadXPath);
			}
			XmpNode xmpNode = null;
			XmpNode xmpNode2 = XmpNodeUtils.FindSchemaNode(xmpTree, xpath.GetSegment(0).Name, createNodes);
			if (xmpNode2 == null)
			{
				return null;
			}
			if (xmpNode2.IsImplicit)
			{
				xmpNode2.IsImplicit = false;
				xmpNode = xmpNode2;
			}
			try
			{
				for (int i = 1; i < xpath.Size(); i++)
				{
					xmpNode2 = XmpNodeUtils.FollowXPathStep(xmpNode2, xpath.GetSegment(i), createNodes);
					if (xmpNode2 == null)
					{
						if (createNodes)
						{
							XmpNodeUtils.DeleteNode(xmpNode);
						}
						return null;
					}
					if (xmpNode2.IsImplicit)
					{
						xmpNode2.IsImplicit = false;
						if (i == 1 && xpath.GetSegment(i).IsAlias && xpath.GetSegment(i).AliasForm != 0)
						{
							xmpNode2.Options.SetOption(xpath.GetSegment(i).AliasForm, true);
						}
						else if (i < xpath.Size() - 1 && xpath.GetSegment(i).Kind == XmpPathStepType.StructFieldStep && !xmpNode2.Options.IsCompositeProperty)
						{
							xmpNode2.Options.IsStruct = true;
						}
						if (xmpNode == null)
						{
							xmpNode = xmpNode2;
						}
					}
				}
			}
			catch (XmpException)
			{
				if (xmpNode != null)
				{
					XmpNodeUtils.DeleteNode(xmpNode);
				}
				throw;
			}
			if (xmpNode != null)
			{
				xmpNode2.Options.MergeWith(leafOptions);
				xmpNode2.Options = xmpNode2.Options;
			}
			return xmpNode2;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00009A48 File Offset: 0x00007C48
		internal static void DeleteNode(XmpNode node)
		{
			XmpNode parent = node.Parent;
			if (node.Options.IsQualifier)
			{
				parent.RemoveQualifier(node);
			}
			else
			{
				parent.RemoveChild(node);
			}
			if (!parent.HasChildren && parent.Options.IsSchemaNode)
			{
				parent.Parent.RemoveChild(parent);
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00009A9C File Offset: 0x00007C9C
		internal static void SetNodeValue(XmpNode node, object value)
		{
			string text = XmpNodeUtils.SerializeNodeValue(value);
			node.Value = ((node.Options.IsQualifier && node.Name == "xml:lang") ? Utils.NormalizeLangValue(text) : text);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00009AE0 File Offset: 0x00007CE0
		internal static PropertyOptions VerifySetOptions(PropertyOptions options, object itemValue)
		{
			if (options == null)
			{
				options = new PropertyOptions();
			}
			if (options.IsArrayAltText)
			{
				options.IsArrayAlternate = true;
			}
			if (options.IsArrayAlternate)
			{
				options.IsArrayOrdered = true;
			}
			if (options.IsArrayOrdered)
			{
				options.IsArray = true;
			}
			if (options.IsCompositeProperty && itemValue != null && itemValue.ToString().Length > 0)
			{
				throw new XmpException("Structs and arrays can't have values", XmpErrorCode.BadOptions);
			}
			options.AssertConsistency(options.GetOptions());
			return options;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00009B58 File Offset: 0x00007D58
		private static string SerializeNodeValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (value is bool)
			{
				bool flag = (bool)value;
				return XmpUtils.ConvertFromBoolean(flag);
			}
			if (value is int)
			{
				int num = (int)value;
				return XmpUtils.ConvertFromInteger(num);
			}
			if (value is long)
			{
				long num2 = (long)value;
				return XmpUtils.ConvertFromLong(num2);
			}
			if (value is double)
			{
				double num3 = (double)value;
				return XmpUtils.ConvertFromDouble(num3);
			}
			IXmpDateTime xmpDateTime = value as IXmpDateTime;
			string text;
			if (xmpDateTime == null)
			{
				GregorianCalendar gregorianCalendar = value as GregorianCalendar;
				if (gregorianCalendar == null)
				{
					byte[] array = value as byte[];
					text = ((array != null) ? XmpUtils.EncodeBase64(array) : value.ToString());
				}
				else
				{
					text = XmpUtils.ConvertFromDate(XmpDateTimeFactory.CreateFromCalendar(gregorianCalendar));
				}
			}
			else
			{
				text = XmpUtils.ConvertFromDate(xmpDateTime);
			}
			if (text == null)
			{
				return null;
			}
			return Utils.RemoveControlChars(text);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00009C28 File Offset: 0x00007E28
		private static XmpNode FollowXPathStep(XmpNode parentNode, XmpPathSegment nextStep, bool createNodes)
		{
			XmpNode xmpNode = null;
			XmpPathStepType kind = nextStep.Kind;
			if (kind != XmpPathStepType.StructFieldStep)
			{
				if (kind != XmpPathStepType.QualifierStep)
				{
					if (!parentNode.Options.IsArray)
					{
						throw new XmpException("Indexing applied to non-array", XmpErrorCode.BadXPath);
					}
					int num;
					switch (kind)
					{
					case XmpPathStepType.ArrayIndexStep:
						num = XmpNodeUtils.FindIndexedItem(parentNode, nextStep.Name, createNodes);
						break;
					case XmpPathStepType.ArrayLastStep:
						num = parentNode.GetChildrenLength();
						break;
					case XmpPathStepType.QualSelectorStep:
					{
						string text;
						string text2;
						Utils.SplitNameAndValue(nextStep.Name, out text, out text2);
						num = XmpNodeUtils.LookupQualSelector(parentNode, text, text2, nextStep.AliasForm);
						break;
					}
					case XmpPathStepType.FieldSelectorStep:
					{
						string text3;
						string text4;
						Utils.SplitNameAndValue(nextStep.Name, out text3, out text4);
						num = XmpNodeUtils.LookupFieldSelector(parentNode, text3, text4);
						break;
					}
					default:
						throw new XmpException("Unknown array indexing step in FollowXPathStep", XmpErrorCode.InternalFailure);
					}
					if (1 <= num && num <= parentNode.GetChildrenLength())
					{
						xmpNode = parentNode.GetChild(num);
					}
				}
				else
				{
					xmpNode = XmpNodeUtils.FindQualifierNode(parentNode, nextStep.Name.Substring(1), createNodes);
				}
			}
			else
			{
				xmpNode = XmpNodeUtils.FindChildNode(parentNode, nextStep.Name, createNodes);
			}
			return xmpNode;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00009D24 File Offset: 0x00007F24
		private static XmpNode FindQualifierNode(XmpNode parent, string qualName, bool createNodes)
		{
			XmpNode xmpNode = parent.FindQualifierByName(qualName);
			if (xmpNode == null && createNodes)
			{
				xmpNode = new XmpNode(qualName, null)
				{
					IsImplicit = true
				};
				parent.AddQualifier(xmpNode);
			}
			return xmpNode;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00009D58 File Offset: 0x00007F58
		private static int FindIndexedItem(XmpNode arrayNode, string segment, bool createNodes)
		{
			int num;
			if (!int.TryParse(segment.Substring(1, segment.Length - 1 - 1), out num))
			{
				throw new XmpException("Array index not digits.", XmpErrorCode.BadXPath);
			}
			if (createNodes && num == arrayNode.GetChildrenLength() + 1)
			{
				XmpNode xmpNode = new XmpNode("[]", null)
				{
					IsImplicit = true
				};
				arrayNode.AddChild(xmpNode);
			}
			return num;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00009DB8 File Offset: 0x00007FB8
		private static int LookupFieldSelector(XmpNode arrayNode, string fieldName, string fieldValue)
		{
			int num = -1;
			int num2 = 1;
			while (num2 <= arrayNode.GetChildrenLength() && num < 0)
			{
				XmpNode child = arrayNode.GetChild(num2);
				if (!child.Options.IsStruct)
				{
					throw new XmpException("Field selector must be used on array of struct", XmpErrorCode.BadXPath);
				}
				for (int i = 1; i <= child.GetChildrenLength(); i++)
				{
					XmpNode child2 = child.GetChild(i);
					if (!(child2.Name != fieldName) && !(child2.Value != fieldValue))
					{
						num = num2;
						break;
					}
				}
				num2++;
			}
			return num;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00009E3C File Offset: 0x0000803C
		private static int LookupQualSelector(XmpNode arrayNode, string qualName, string qualValue, int aliasForm)
		{
			if (!(qualName == "xml:lang"))
			{
				for (int i = 1; i < arrayNode.GetChildrenLength(); i++)
				{
					IIterator iterator = arrayNode.GetChild(i).IterateQualifier();
					while (iterator.HasNext())
					{
						XmpNode xmpNode = (XmpNode)iterator.Next();
						if (xmpNode.Name == qualName && xmpNode.Value == qualValue)
						{
							return i;
						}
					}
				}
				return -1;
			}
			qualValue = Utils.NormalizeLangValue(qualValue);
			int num = XmpNodeUtils.LookupLanguageItem(arrayNode, qualValue);
			if (num < 0 && (aliasForm & 4096) > 0)
			{
				XmpNode xmpNode2 = new XmpNode("[]", null);
				XmpNode xmpNode3 = new XmpNode("xml:lang", "x-default", null);
				xmpNode2.AddQualifier(xmpNode3);
				arrayNode.AddChild(1, xmpNode2);
				return 1;
			}
			return num;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00009F00 File Offset: 0x00008100
		internal static void NormalizeLangArray(XmpNode arrayNode)
		{
			if (!arrayNode.Options.IsArrayAltText)
			{
				return;
			}
			int i = 2;
			while (i <= arrayNode.GetChildrenLength())
			{
				XmpNode child = arrayNode.GetChild(i);
				if (child.HasQualifier && !(child.GetQualifier(1).Value != "x-default"))
				{
					try
					{
						arrayNode.RemoveChild(i);
						arrayNode.AddChild(1, child);
					}
					catch (XmpException)
					{
					}
					if (i == 2)
					{
						arrayNode.GetChild(2).Value = child.Value;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00009F90 File Offset: 0x00008190
		internal static void DetectAltText(XmpNode arrayNode)
		{
			if (!arrayNode.Options.IsArrayAlternate || !arrayNode.HasChildren)
			{
				return;
			}
			bool flag = false;
			IIterator iterator = arrayNode.IterateChildren();
			while (iterator.HasNext())
			{
				if (((XmpNode)iterator.Next()).Options.HasLanguage)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				arrayNode.Options.IsArrayAltText = true;
				XmpNodeUtils.NormalizeLangArray(arrayNode);
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00009FF8 File Offset: 0x000081F8
		internal static void AppendLangItem(XmpNode arrayNode, string itemLang, string itemValue)
		{
			XmpNode xmpNode = new XmpNode("[]", itemValue, null);
			XmpNode xmpNode2 = new XmpNode("xml:lang", itemLang, null);
			xmpNode.AddQualifier(xmpNode2);
			if (xmpNode2.Value != "x-default")
			{
				arrayNode.AddChild(xmpNode);
				return;
			}
			arrayNode.AddChild(1, xmpNode);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000A048 File Offset: 0x00008248
		internal static object[] ChooseLocalizedText(XmpNode arrayNode, string genericLang, string specificLang)
		{
			if (!arrayNode.Options.IsArrayAltText)
			{
				throw new XmpException("Localized text array is not alt-text", XmpErrorCode.BadXPath);
			}
			if (!arrayNode.HasChildren)
			{
				object[] array = new object[2];
				array[0] = 0;
				return array;
			}
			int num = 0;
			XmpNode xmpNode = null;
			XmpNode xmpNode2 = null;
			IIterator iterator = arrayNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode3 = (XmpNode)iterator.Next();
				if (xmpNode3.Options.IsCompositeProperty)
				{
					throw new XmpException("Alt-text array item is not simple", XmpErrorCode.BadXPath);
				}
				if (!xmpNode3.HasQualifier || xmpNode3.GetQualifier(1).Name != "xml:lang")
				{
					throw new XmpException("Alt-text array item has no language qualifier", XmpErrorCode.BadXPath);
				}
				string value = xmpNode3.GetQualifier(1).Value;
				if (value == specificLang)
				{
					return new object[] { 1, xmpNode3 };
				}
				if (genericLang != null && value.StartsWith(genericLang))
				{
					if (xmpNode == null)
					{
						xmpNode = xmpNode3;
					}
					num++;
				}
				else if (value == "x-default")
				{
					xmpNode2 = xmpNode3;
				}
			}
			if (num == 1)
			{
				return new object[] { 2, xmpNode };
			}
			if (num > 1)
			{
				return new object[] { 3, xmpNode };
			}
			if (xmpNode2 != null)
			{
				return new object[] { 4, xmpNode2 };
			}
			return new object[]
			{
				5,
				arrayNode.GetChild(1)
			};
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000A1B4 File Offset: 0x000083B4
		internal static int LookupLanguageItem(XmpNode arrayNode, string language)
		{
			if (!arrayNode.Options.IsArray)
			{
				throw new XmpException("Language item must be used on array", XmpErrorCode.BadXPath);
			}
			for (int i = 1; i <= arrayNode.GetChildrenLength(); i++)
			{
				XmpNode child = arrayNode.GetChild(i);
				if (child.HasQualifier && !(child.GetQualifier(1).Name != "xml:lang") && child.GetQualifier(1).Value == language)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04000109 RID: 265
		internal const int CltNoValues = 0;

		// Token: 0x0400010A RID: 266
		internal const int CltSpecificMatch = 1;

		// Token: 0x0400010B RID: 267
		internal const int CltSingleGeneric = 2;

		// Token: 0x0400010C RID: 268
		internal const int CltMultipleGeneric = 3;

		// Token: 0x0400010D RID: 269
		internal const int CltXDefault = 4;

		// Token: 0x0400010E RID: 270
		internal const int CltFirstItem = 5;
	}
}
