using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Sharpen;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x02000033 RID: 51
	public static class ParseRdf
	{
		// Token: 0x060001C5 RID: 453 RVA: 0x000051D8 File Offset: 0x000033D8
		internal static XmpMeta Parse(XElement xmlRoot, ParseOptions options)
		{
			XmpMeta xmpMeta = new XmpMeta();
			ParseRdf.Rdf_RDF(xmpMeta, xmlRoot, options);
			return xmpMeta;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000051E7 File Offset: 0x000033E7
		private static void Rdf_RDF(XmpMeta xmp, XElement rdfRdfNode, ParseOptions options)
		{
			if (!rdfRdfNode.Attributes().Any<XAttribute>())
			{
				throw new XmpException("Invalid attributes of rdf:RDF element", XmpErrorCode.BadRdf);
			}
			ParseRdf.Rdf_NodeElementList(xmp, xmp.GetRoot(), rdfRdfNode, options);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00005214 File Offset: 0x00003414
		private static void Rdf_NodeElementList(XmpMeta xmp, XmpNode xmpParent, XElement rdfRdfNode, ParseOptions options)
		{
			foreach (XNode xnode in rdfRdfNode.Nodes())
			{
				XElement xelement = xnode as XElement;
				if (xelement != null)
				{
					ParseRdf.Rdf_NodeElement(xmp, xmpParent, xelement, true, options);
				}
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000526C File Offset: 0x0000346C
		private static void Rdf_NodeElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel, ParseOptions options)
		{
			RdfTerm rdfTermKind = ParseRdf.GetRdfTermKind(xmlNode);
			if (rdfTermKind != RdfTerm.Description && rdfTermKind != RdfTerm.Other)
			{
				throw new XmpException("Node element must be rdf:Description or typed node", XmpErrorCode.BadRdf);
			}
			if (isTopLevel && rdfTermKind == RdfTerm.Other)
			{
				throw new XmpException("Top level typed node not allowed", XmpErrorCode.BadXmp);
			}
			ParseRdf.Rdf_NodeElementAttrs(xmp, xmpParent, xmlNode, isTopLevel, options);
			ParseRdf.Rdf_PropertyElementList(xmp, xmpParent, xmlNode, isTopLevel, options);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x000052C4 File Offset: 0x000034C4
		private static void Rdf_NodeElementAttrs(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel, ParseOptions options)
		{
			int num = 0;
			foreach (XAttribute xattribute in xmlNode.Attributes())
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute.Name.Namespace);
				if (!(prefixOfNamespace == "xmlns") && (prefixOfNamespace != null || !(xattribute.Name == "xmlns")))
				{
					RdfTerm rdfTermKind = ParseRdf.GetRdfTermKind(xattribute);
					switch (rdfTermKind)
					{
					case RdfTerm.Other:
						ParseRdf.AddChildNode(xmp, xmpParent, xattribute, xattribute.Value, isTopLevel);
						continue;
					case RdfTerm.Id:
					case RdfTerm.About:
					case RdfTerm.NodeId:
						if (num > 0)
						{
							throw new XmpException("Mutally exclusive about, ID, nodeID attributes", XmpErrorCode.BadRdf);
						}
						num++;
						if (!isTopLevel || rdfTermKind != RdfTerm.About)
						{
							continue;
						}
						if (string.IsNullOrEmpty(xmpParent.Name))
						{
							xmpParent.Name = xattribute.Value;
							continue;
						}
						if (xattribute.Value != xmpParent.Name)
						{
							throw new XmpException("Mismatched top level rdf:about values", XmpErrorCode.BadXmp);
						}
						continue;
					}
					throw new XmpException("Invalid nodeElement attribute", XmpErrorCode.BadRdf);
				}
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000540C File Offset: 0x0000360C
		private static void Rdf_PropertyElementList(XmpMeta xmp, XmpNode xmpParent, XElement xmlParent, bool isTopLevel, ParseOptions options)
		{
			int num = 0;
			foreach (XNode xnode in xmlParent.Nodes())
			{
				if (!ParseRdf.IsWhitespaceNode(xnode) && xnode.NodeType != XmlNodeType.Comment)
				{
					if (xnode.NodeType != XmlNodeType.Element)
					{
						throw new XmpException("Expected property element node not found", XmpErrorCode.BadRdf);
					}
					if (xmpParent.Options.IsArrayLimited && num > xmpParent.Options.ArrayElementsLimit)
					{
						break;
					}
					ParseRdf.Rdf_PropertyElement(xmp, xmpParent, (XElement)xnode, isTopLevel, options);
					num++;
				}
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x000054B0 File Offset: 0x000036B0
		private static void Rdf_PropertyElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel, ParseOptions options)
		{
			if (!ParseRdf.IsPropertyElementName(ParseRdf.GetRdfTermKind(xmlNode)))
			{
				throw new XmpException("Invalid property element name", XmpErrorCode.BadRdf);
			}
			List<XAttribute> list = xmlNode.Attributes().ToList<XAttribute>();
			List<string> ignoreNodes = new List<string>();
			foreach (XAttribute xattribute in list)
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute.Name.Namespace);
				if (prefixOfNamespace == "xmlns" || (prefixOfNamespace == null && xattribute.Name == "xmlns"))
				{
					ignoreNodes.Add(xattribute.Name.ToString());
				}
			}
			if (list.Count - ignoreNodes.Count > 3)
			{
				ParseRdf.Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
				return;
			}
			XAttribute xattribute2 = (from attribute in list
				let attrLocal = attribute.Name.LocalName
				let attrNs = attribute.Name.NamespaceName
				where "xml:" + attrLocal != "xml:lang" && (!(attrLocal == "ID") || !(attrNs == "http://www.w3.org/1999/02/22-rdf-syntax-ns#")) && !ignoreNodes.Contains(attribute.Name.ToString())
				select attribute).FirstOrDefault<XAttribute>();
			if (xattribute2 != null)
			{
				string localName = xattribute2.Name.LocalName;
				string namespaceName = xattribute2.Name.NamespaceName;
				string value = xattribute2.Value;
				if (localName == "datatype" && namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
				{
					ParseRdf.Rdf_LiteralPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
					return;
				}
				if (!(localName == "parseType") || !(namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#"))
				{
					ParseRdf.Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
					return;
				}
				if (value == "Literal")
				{
					ParseRdf.Rdf_ParseTypeLiteralPropertyElement();
					return;
				}
				if (value == "Resource")
				{
					ParseRdf.Rdf_ParseTypeResourcePropertyElement(xmp, xmpParent, xmlNode, isTopLevel, options);
					return;
				}
				if (value == "Collection")
				{
					ParseRdf.Rdf_ParseTypeCollectionPropertyElement();
					return;
				}
				ParseRdf.Rdf_ParseTypeOtherPropertyElement();
				return;
			}
			else
			{
				if (xmlNode.IsEmpty)
				{
					ParseRdf.Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
					return;
				}
				if (xmlNode.Nodes().FirstOrDefault((XNode t) => t.NodeType != XmlNodeType.Text) == null)
				{
					ParseRdf.Rdf_LiteralPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
					return;
				}
				ParseRdf.Rdf_ResourcePropertyElement(xmp, xmpParent, xmlNode, isTopLevel, options);
				return;
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000573C File Offset: 0x0000393C
		private static void Rdf_ResourcePropertyElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel, ParseOptions options)
		{
			if (isTopLevel && xmlNode.Name == XName.Get("changes", "iX"))
			{
				return;
			}
			XmpNode xmpNode = ParseRdf.AddChildNode(xmp, xmpParent, xmlNode, string.Empty, isTopLevel);
			foreach (XAttribute xattribute in xmlNode.Attributes())
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute.Name.Namespace);
				if (!(prefixOfNamespace == "xmlns") && (prefixOfNamespace != null || !(xattribute.Name == "xmlns")))
				{
					string localName = xattribute.Name.LocalName;
					string namespaceName = xattribute.Name.NamespaceName;
					if ("xml:" + xattribute.Name.LocalName == "xml:lang")
					{
						ParseRdf.AddQualifierNode(xmpNode, "xml:lang", xattribute.Value);
					}
					else if (!(localName == "ID") || !(namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#"))
					{
						throw new XmpException("Invalid attribute for resource property element", XmpErrorCode.BadRdf);
					}
				}
			}
			bool flag = false;
			foreach (XNode xnode in xmlNode.Nodes())
			{
				if (!ParseRdf.IsWhitespaceNode(xnode))
				{
					if (xnode.NodeType != XmlNodeType.Element || flag)
					{
						if (flag)
						{
							throw new XmpException("Invalid child of resource property element", XmpErrorCode.BadRdf);
						}
						throw new XmpException("Children of resource property element must be XML elements", XmpErrorCode.BadRdf);
					}
					else
					{
						XElement xelement = (XElement)xnode;
						bool flag2 = xelement.Name.NamespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
						string localName2 = xelement.Name.LocalName;
						if (flag2 && localName2 == "Bag")
						{
							xmpNode.Options.IsArray = true;
						}
						else if (flag2 && localName2 == "Seq")
						{
							xmpNode.Options.IsArray = true;
							xmpNode.Options.IsArrayOrdered = true;
						}
						else if (flag2 && localName2 == "Alt")
						{
							xmpNode.Options.IsArray = true;
							xmpNode.Options.IsArrayOrdered = true;
							xmpNode.Options.IsArrayAlternate = true;
						}
						else
						{
							xmpNode.Options.IsStruct = true;
							if (!flag2 && localName2 != "Description")
							{
								string text = xelement.Name.NamespaceName;
								text = text + ":" + localName2;
								ParseRdf.AddQualifierNode(xmpNode, "rdf:type", text);
							}
						}
						int num;
						if (xmpNode.Options.IsArray && options.GetXMPNodesToLimit().TryGetValue(xmpNode.Name, out num))
						{
							xmpNode.Options.SetArrayElementLimit(num);
						}
						ParseRdf.Rdf_NodeElement(xmp, xmpNode, xelement, false, options);
						if (xmpNode.HasValueChild)
						{
							ParseRdf.FixupQualifiedNode(xmpNode);
						}
						else if (xmpNode.Options.IsArrayAlternate)
						{
							XmpNodeUtils.DetectAltText(xmpNode);
						}
						flag = true;
					}
				}
			}
			if (!flag)
			{
				throw new XmpException("Missing child of resource property element", XmpErrorCode.BadRdf);
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00005A84 File Offset: 0x00003C84
		private static void Rdf_LiteralPropertyElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel)
		{
			XmpNode xmpNode = ParseRdf.AddChildNode(xmp, xmpParent, xmlNode, null, isTopLevel);
			foreach (XAttribute xattribute in xmlNode.Attributes())
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute.Name.Namespace);
				if (!(prefixOfNamespace == "xmlns") && (prefixOfNamespace != null || !(xattribute.Name == "xmlns")))
				{
					string namespaceName = xattribute.Name.NamespaceName;
					string localName = xattribute.Name.LocalName;
					if ("xml:" + xattribute.Name.LocalName == "xml:lang")
					{
						ParseRdf.AddQualifierNode(xmpNode, "xml:lang", xattribute.Value);
					}
					else if (!(namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#") || (!(localName == "ID") && !(localName == "datatype")))
					{
						throw new XmpException("Invalid attribute for literal property element", XmpErrorCode.BadRdf);
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XNode xnode in xmlNode.Nodes())
			{
				if (xnode.NodeType != XmlNodeType.Text)
				{
					throw new XmpException("Invalid child of literal property element", XmpErrorCode.BadRdf);
				}
				stringBuilder.Append(((XText)xnode).Value);
			}
			xmpNode.Value = stringBuilder.ToString();
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00005C20 File Offset: 0x00003E20
		private static void Rdf_ParseTypeLiteralPropertyElement()
		{
			throw new XmpException("ParseTypeLiteral property element not allowed", XmpErrorCode.BadXmp);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00005C34 File Offset: 0x00003E34
		private static void Rdf_ParseTypeResourcePropertyElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel, ParseOptions options)
		{
			XmpNode xmpNode = ParseRdf.AddChildNode(xmp, xmpParent, xmlNode, string.Empty, isTopLevel);
			xmpNode.Options.IsStruct = true;
			foreach (XAttribute xattribute in xmlNode.Attributes())
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute.Name.Namespace);
				if (!(prefixOfNamespace == "xmlns") && (prefixOfNamespace != null || !(xattribute.Name == "xmlns")))
				{
					string localName = xattribute.Name.LocalName;
					string namespaceName = xattribute.Name.NamespaceName;
					if ("xml:" + xattribute.Name.LocalName == "xml:lang")
					{
						ParseRdf.AddQualifierNode(xmpNode, "xml:lang", xattribute.Value);
					}
					else if (!(namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#") || (!(localName == "ID") && !(localName == "parseType")))
					{
						throw new XmpException("Invalid attribute for ParseTypeResource property element", XmpErrorCode.BadRdf);
					}
				}
			}
			ParseRdf.Rdf_PropertyElementList(xmp, xmpNode, xmlNode, false, options);
			if (xmpNode.HasValueChild)
			{
				ParseRdf.FixupQualifiedNode(xmpNode);
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00005D7C File Offset: 0x00003F7C
		private static void Rdf_ParseTypeCollectionPropertyElement()
		{
			throw new XmpException("ParseTypeCollection property element not allowed", XmpErrorCode.BadXmp);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00005D8D File Offset: 0x00003F8D
		private static void Rdf_ParseTypeOtherPropertyElement()
		{
			throw new XmpException("ParseTypeOther property element not allowed", XmpErrorCode.BadXmp);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00005DA0 File Offset: 0x00003FA0
		private static void Rdf_EmptyPropertyElement(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, bool isTopLevel)
		{
			if (xmlNode.FirstNode != null)
			{
				throw new XmpException("Nested content not allowed with rdf:resource or property attributes", XmpErrorCode.BadRdf);
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			XAttribute xattribute = null;
			foreach (XAttribute xattribute2 in xmlNode.Attributes())
			{
				string prefixOfNamespace = xmlNode.GetPrefixOfNamespace(xattribute2.Name.Namespace);
				if (!(prefixOfNamespace == "xmlns") && (prefixOfNamespace != null || !(xattribute2.Name == "xmlns")))
				{
					switch (ParseRdf.GetRdfTermKind(xattribute2))
					{
					case RdfTerm.Other:
						if (xattribute2.Name.LocalName == "value" && xattribute2.Name.NamespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
						{
							if (flag2)
							{
								throw new XmpException("Empty property element can't have both rdf:value and rdf:resource", XmpErrorCode.BadXmp);
							}
							flag4 = true;
							xattribute = xattribute2;
							continue;
						}
						else
						{
							if ("xml:" + xattribute2.Name.LocalName != "xml:lang")
							{
								flag = true;
								continue;
							}
							continue;
						}
						break;
					case RdfTerm.Id:
						continue;
					case RdfTerm.Resource:
						if (flag3)
						{
							throw new XmpException("Empty property element can't have both rdf:resource and rdf:nodeID", XmpErrorCode.BadRdf);
						}
						if (flag4)
						{
							throw new XmpException("Empty property element can't have both rdf:value and rdf:resource", XmpErrorCode.BadXmp);
						}
						flag2 = true;
						xattribute = xattribute2;
						continue;
					case RdfTerm.NodeId:
						if (flag2)
						{
							throw new XmpException("Empty property element can't have both rdf:resource and rdf:nodeID", XmpErrorCode.BadRdf);
						}
						flag3 = true;
						continue;
					}
					throw new XmpException("Unrecognized attribute of empty property element", XmpErrorCode.BadRdf);
				}
			}
			XmpNode xmpNode = ParseRdf.AddChildNode(xmp, xmpParent, xmlNode, string.Empty, isTopLevel);
			bool flag5 = false;
			if (flag4 || flag2)
			{
				xmpNode.Value = ((xattribute != null) ? xattribute.Value : string.Empty);
				if (!flag4)
				{
					xmpNode.Options.IsUri = true;
				}
			}
			else if (flag)
			{
				xmpNode.Options.IsStruct = true;
				flag5 = true;
			}
			foreach (XAttribute xattribute3 in xmlNode.Attributes())
			{
				string prefixOfNamespace2 = xmlNode.GetPrefixOfNamespace(xattribute3.Name.Namespace);
				if (xattribute3 != xattribute && !(prefixOfNamespace2 == "xmlns") && (prefixOfNamespace2 != null || !(xattribute3.Name == "xmlns")))
				{
					switch (ParseRdf.GetRdfTermKind(xattribute3))
					{
					case RdfTerm.Other:
						if (!flag5)
						{
							ParseRdf.AddQualifierNode(xmpNode, xattribute3.Name.LocalName, xattribute3.Value);
							continue;
						}
						if ("xml:" + xattribute3.Name.LocalName == "xml:lang")
						{
							ParseRdf.AddQualifierNode(xmpNode, "xml:lang", xattribute3.Value);
							continue;
						}
						ParseRdf.AddChildNode(xmp, xmpNode, xattribute3, xattribute3.Value, false);
						continue;
					case RdfTerm.Id:
					case RdfTerm.NodeId:
						continue;
					case RdfTerm.Resource:
						ParseRdf.AddQualifierNode(xmpNode, "rdf:resource", xattribute3.Value);
						continue;
					}
					throw new XmpException("Unrecognized attribute of empty property element", XmpErrorCode.BadRdf);
				}
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00006114 File Offset: 0x00004314
		private static XmpNode AddChildNode(XmpMeta xmp, XmpNode xmpParent, XElement xmlNode, string value, bool isTopLevel)
		{
			return ParseRdf.AddChildNode(xmp, xmpParent, xmlNode.Name, xmlNode.GetPrefixOfNamespace(xmlNode.Name.Namespace), value, isTopLevel);
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00006137 File Offset: 0x00004337
		private static XmpNode AddChildNode(XmpMeta xmp, XmpNode xmpParent, XAttribute xmlNode, string value, bool isTopLevel)
		{
			return ParseRdf.AddChildNode(xmp, xmpParent, xmlNode.Name, xmlNode.Parent.GetPrefixOfNamespace(xmlNode.Name.Namespace), value, isTopLevel);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00006160 File Offset: 0x00004360
		private static XmpNode AddChildNode(XmpMeta xmp, XmpNode xmpParent, XName nodeName, string nodeNamespacePrefix, string value, bool isTopLevel)
		{
			IXmpSchemaRegistry schemaRegistry = XmpMetaFactory.SchemaRegistry;
			string text = nodeName.NamespaceName;
			if (text == string.Empty)
			{
				throw new XmpException("XML namespace required for all elements and attributes", XmpErrorCode.BadRdf);
			}
			if (text == "http://purl.org/dc/1.1/")
			{
				text = "http://purl.org/dc/elements/1.1/";
			}
			string text2 = schemaRegistry.GetNamespacePrefix(text);
			if (text2 == null)
			{
				text2 = nodeNamespacePrefix ?? "_dflt";
				text2 = schemaRegistry.RegisterNamespace(text, text2);
			}
			string text3 = text2 + nodeName.LocalName;
			PropertyOptions propertyOptions = new PropertyOptions();
			bool flag = false;
			if (isTopLevel)
			{
				XmpNode xmpNode = XmpNodeUtils.FindSchemaNode(xmp.GetRoot(), text, "_dflt", true);
				xmpNode.IsImplicit = false;
				xmpParent = xmpNode;
				if (schemaRegistry.FindAlias(text3) != null)
				{
					flag = true;
					xmp.GetRoot().HasAliases = true;
					xmpNode.HasAliases = true;
				}
			}
			bool flag2 = ParseRdf.IsNumberedArrayItemName(text3);
			bool flag3 = text3 == "rdf:value";
			XmpNode xmpNode2 = new XmpNode(text3, value, propertyOptions)
			{
				IsAlias = flag
			};
			if (!flag3)
			{
				xmpParent.AddChild(xmpNode2);
			}
			else
			{
				xmpParent.AddChild(1, xmpNode2);
			}
			if (flag3)
			{
				if (isTopLevel || !xmpParent.Options.IsStruct)
				{
					throw new XmpException("Misplaced rdf:value element", XmpErrorCode.BadRdf);
				}
				xmpParent.HasValueChild = true;
			}
			bool isArray = xmpParent.Options.IsArray;
			if (isArray && flag2)
			{
				xmpNode2.Name = "[]";
			}
			else
			{
				if (!isArray && flag2)
				{
					throw new XmpException("Misplaced rdf:li element", XmpErrorCode.BadRdf);
				}
				if (isArray && !flag2)
				{
					throw new XmpException("Arrays cannot have arbitrary child names", XmpErrorCode.BadRdf);
				}
			}
			return xmpNode2;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000062E1 File Offset: 0x000044E1
		private static void AddQualifierNode(XmpNode xmpParent, string name, string value)
		{
			if (name == "xml:lang")
			{
				value = Utils.NormalizeLangValue(value);
			}
			xmpParent.AddQualifier(new XmpNode(name, value, null));
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00006308 File Offset: 0x00004508
		private static void FixupQualifiedNode(XmpNode xmpParent)
		{
			XmpNode child = xmpParent.GetChild(1);
			if (child.Options.HasLanguage)
			{
				if (xmpParent.Options.HasLanguage)
				{
					throw new XmpException("Redundant xml:lang for rdf:value element", XmpErrorCode.BadXmp);
				}
				XmpNode qualifier = child.GetQualifier(1);
				child.RemoveQualifier(qualifier);
				xmpParent.AddQualifier(qualifier);
			}
			for (int i = 1; i <= child.GetQualifierLength(); i++)
			{
				XmpNode qualifier2 = child.GetQualifier(i);
				xmpParent.AddQualifier(qualifier2);
			}
			for (int j = 2; j <= xmpParent.GetChildrenLength(); j++)
			{
				XmpNode child2 = xmpParent.GetChild(j);
				xmpParent.AddQualifier(child2);
			}
			xmpParent.HasValueChild = false;
			xmpParent.Options.IsStruct = false;
			xmpParent.Options.MergeWith(child.Options);
			xmpParent.Value = child.Value;
			xmpParent.RemoveChildren();
			IIterator iterator = child.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				xmpParent.AddChild(xmpNode);
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00006403 File Offset: 0x00004603
		private static bool IsWhitespaceNode(XNode node)
		{
			return node.NodeType == XmlNodeType.Text && Utils.IsNullOrWhiteSpace(((XText)node).Value);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00006420 File Offset: 0x00004620
		private static bool IsPropertyElementName(RdfTerm term)
		{
			return term != RdfTerm.Description && !ParseRdf.IsOldTerm(term) && !ParseRdf.IsCoreSyntaxTerm(term);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00006439 File Offset: 0x00004639
		private static bool IsOldTerm(RdfTerm term)
		{
			return RdfTerm.AboutEach <= term && term <= RdfTerm.BagId;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000644A File Offset: 0x0000464A
		private static bool IsCoreSyntaxTerm(RdfTerm term)
		{
			return RdfTerm.Rdf <= term && term <= RdfTerm.Datatype;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00006459 File Offset: 0x00004659
		private static RdfTerm GetRdfTermKind(XElement node)
		{
			return ParseRdf.GetRdfTermKind(node.Name, node.NodeType, null);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000646D File Offset: 0x0000466D
		private static RdfTerm GetRdfTermKind(XAttribute node)
		{
			return ParseRdf.GetRdfTermKind(node.Name, node.NodeType, node.Parent.Name);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000648C File Offset: 0x0000468C
		private static RdfTerm GetRdfTermKind(XName name, XmlNodeType nodeType, XName parentName = null)
		{
			string localName = name.LocalName;
			string text = name.NamespaceName;
			string text2 = ((parentName != null) ? parentName.NamespaceName : string.Empty);
			if (text == string.Empty && (localName == "about" || localName == "ID") && nodeType == XmlNodeType.Attribute && text2 == "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
			{
				text = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
			}
			if (text != "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
			{
				return RdfTerm.Other;
			}
			if (localName != null)
			{
				switch (localName.Length)
				{
				case 2:
				{
					char c = localName[0];
					if (c != 'I')
					{
						if (c == 'l')
						{
							if (localName == "li")
							{
								return RdfTerm.Li;
							}
						}
					}
					else if (localName == "ID")
					{
						return RdfTerm.Id;
					}
					break;
				}
				case 3:
					if (localName == "RDF")
					{
						return RdfTerm.Rdf;
					}
					break;
				case 5:
				{
					char c = localName[0];
					if (c != 'a')
					{
						if (c == 'b')
						{
							if (localName == "bagID")
							{
								return RdfTerm.BagId;
							}
						}
					}
					else if (localName == "about")
					{
						return RdfTerm.About;
					}
					break;
				}
				case 6:
					if (localName == "nodeID")
					{
						return RdfTerm.NodeId;
					}
					break;
				case 8:
				{
					char c = localName[0];
					if (c != 'd')
					{
						if (c == 'r')
						{
							if (localName == "resource")
							{
								return RdfTerm.Resource;
							}
						}
					}
					else if (localName == "datatype")
					{
						return RdfTerm.Datatype;
					}
					break;
				}
				case 9:
				{
					char c = localName[0];
					if (c != 'a')
					{
						if (c == 'p')
						{
							if (localName == "parseType")
							{
								return RdfTerm.ParseType;
							}
						}
					}
					else if (localName == "aboutEach")
					{
						return RdfTerm.AboutEach;
					}
					break;
				}
				case 11:
					if (localName == "Description")
					{
						return RdfTerm.Description;
					}
					break;
				case 15:
					if (localName == "aboutEachPrefix")
					{
						return RdfTerm.AboutEachPrefix;
					}
					break;
				}
			}
			return RdfTerm.Other;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x000066C0 File Offset: 0x000048C0
		private static bool IsNumberedArrayItemName(string nodeName)
		{
			bool flag = "rdf:li" == nodeName;
			if (nodeName.StartsWith("rdf:_"))
			{
				flag = true;
				for (int i = 5; i < nodeName.Length; i++)
				{
					flag = flag && nodeName[i] >= '0' && nodeName[i] <= '9';
				}
			}
			return flag;
		}

		// Token: 0x040000E0 RID: 224
		public const string DefaultPrefix = "_dflt";
	}
}
