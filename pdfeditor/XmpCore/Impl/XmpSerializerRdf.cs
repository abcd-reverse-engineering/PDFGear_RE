using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sharpen;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003F RID: 63
	public sealed class XmpSerializerRdf
	{
		// Token: 0x060002D5 RID: 725 RVA: 0x0000BB40 File Offset: 0x00009D40
		public void Serialize(IXmpMeta xmp, Stream stream, SerializeOptions options)
		{
			try
			{
				this._stream = stream;
				this._startPos = this._stream.Position;
				this._writer = new StreamWriter(this._stream, options.GetEncoding());
				this._xmp = (XmpMeta)xmp;
				this._options = options;
				this._padding = options.Padding;
				this.CheckOptionsConsistence();
				string text = this.SerializeAsRdf();
				this._writer.Flush();
				this.AddPadding(text.Length);
				this.Write(text);
				this._writer.Flush();
			}
			catch (IOException)
			{
				throw new XmpException("Error writing to the OutputStream", XmpErrorCode.Unknown);
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000BBF0 File Offset: 0x00009DF0
		private void AddPadding(int tailLength)
		{
			if (this._options.ExactPacketLength)
			{
				int num = checked((int)(this._stream.Position - this._startPos)) + tailLength * this._unicodeSize;
				if (num > this._padding)
				{
					throw new XmpException("Can't fit into specified packet size", XmpErrorCode.BadSerialize);
				}
				this._padding -= num;
			}
			this._padding /= this._unicodeSize;
			int length = this._options.Newline.Length;
			if (this._padding >= length)
			{
				this._padding -= length;
				while (this._padding >= 100 + length)
				{
					this.WriteChars(100, ' ');
					this.WriteNewline();
					this._padding -= 100 + length;
				}
				this.WriteChars(this._padding, ' ');
				this.WriteNewline();
				return;
			}
			this.WriteChars(this._padding, ' ');
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000BCD8 File Offset: 0x00009ED8
		private void CheckOptionsConsistence()
		{
			if (this._options.EncodeUtf16Be || this._options.EncodeUtf16Le)
			{
				this._unicodeSize = 2;
			}
			if (this._options.ExactPacketLength)
			{
				if (this._options.OmitPacketWrapper || this._options.IncludeThumbnailPad)
				{
					throw new XmpException("Inconsistent options for exact size serialize", XmpErrorCode.BadOptions);
				}
				if ((this._options.Padding & (this._unicodeSize - 1)) != 0)
				{
					throw new XmpException("Exact size must be a multiple of the Unicode element", XmpErrorCode.BadOptions);
				}
			}
			else if (this._options.ReadOnlyPacket)
			{
				if (this._options.OmitPacketWrapper || this._options.IncludeThumbnailPad)
				{
					throw new XmpException("Inconsistent options for read-only packet", XmpErrorCode.BadOptions);
				}
				this._padding = 0;
				return;
			}
			else if (this._options.OmitPacketWrapper)
			{
				if (this._options.IncludeThumbnailPad)
				{
					throw new XmpException("Inconsistent options for non-packet serialize", XmpErrorCode.BadOptions);
				}
				this._padding = 0;
				return;
			}
			else
			{
				if (this._padding == 0)
				{
					this._padding = 2048 * this._unicodeSize;
				}
				if (this._options.IncludeThumbnailPad && !this._xmp.DoesPropertyExist("http://ns.adobe.com/xap/1.0/", "Thumbnails"))
				{
					this._padding += 10000 * this._unicodeSize;
				}
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000BE24 File Offset: 0x0000A024
		private string SerializeAsRdf()
		{
			int i = 0;
			if (!this._options.OmitPacketWrapper)
			{
				this.WriteIndent(i);
				this.Write("<?xpacket begin=\"\ufeff\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>");
				this.WriteNewline();
			}
			if (!this._options.OmitXmpMetaElement)
			{
				this.WriteIndent(i);
				this.Write("<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"");
				this.Write(XmpMetaFactory.VersionInfo.Message);
				this.Write("\">");
				this.WriteNewline();
				i++;
			}
			this.WriteIndent(i);
			this.Write("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">");
			this.WriteNewline();
			if (this._options.UseCanonicalFormat)
			{
				this.SerializeCanonicalRdfSchemas(i);
			}
			else
			{
				this.SerializeCompactRdfSchemas(i);
			}
			this.WriteIndent(i);
			this.Write("</rdf:RDF>");
			this.WriteNewline();
			if (!this._options.OmitXmpMetaElement)
			{
				i--;
				this.WriteIndent(i);
				this.Write("</x:xmpmeta>");
				this.WriteNewline();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!this._options.OmitPacketWrapper)
			{
				for (i = this._options.BaseIndent; i > 0; i--)
				{
					stringBuilder.Append(this._options.Indent);
				}
				stringBuilder.Append("<?xpacket end=\"");
				stringBuilder.Append(this._options.ReadOnlyPacket ? 'r' : 'w');
				stringBuilder.Append("\"?>");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000BF88 File Offset: 0x0000A188
		private void SerializeCanonicalRdfSchemas(int level)
		{
			if (this._xmp.GetRoot().GetChildrenLength() > 0)
			{
				this.StartOuterRdfDescription(this._xmp.GetRoot(), level);
				IIterator iterator = this._xmp.GetRoot().IterateChildren();
				while (iterator.HasNext())
				{
					XmpNode xmpNode = (XmpNode)iterator.Next();
					this.SerializeCanonicalRdfSchema(xmpNode, level);
				}
				this.EndOuterRdfDescription(level);
				return;
			}
			this.WriteIndent(level + 1);
			this.Write("<rdf:Description rdf:about=");
			this.WriteTreeName();
			this.Write("/>");
			this.WriteNewline();
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000C01C File Offset: 0x0000A21C
		private void WriteTreeName()
		{
			this.Write('"');
			string name = this._xmp.GetRoot().Name;
			if (name != null)
			{
				this.AppendNodeValue(name, true);
			}
			this.Write('"');
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000C058 File Offset: 0x0000A258
		private void SerializeCompactRdfSchemas(int level)
		{
			this.WriteIndent(level + 1);
			this.Write("<rdf:Description rdf:about=");
			this.WriteTreeName();
			ICollection<object> collection = new HashSet<object>();
			collection.Add("xml");
			collection.Add("rdf");
			IIterator iterator = this._xmp.GetRoot().IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				this.DeclareUsedNamespaces(xmpNode, collection, level + 3);
			}
			bool flag = true;
			IIterator iterator2 = this._xmp.GetRoot().IterateChildren();
			while (iterator2.HasNext())
			{
				XmpNode xmpNode2 = (XmpNode)iterator2.Next();
				flag &= this.SerializeCompactRdfAttrProps(xmpNode2, level + 2);
			}
			if (!flag)
			{
				this.Write('>');
				this.WriteNewline();
				IIterator iterator3 = this._xmp.GetRoot().IterateChildren();
				while (iterator3.HasNext())
				{
					XmpNode xmpNode3 = (XmpNode)iterator3.Next();
					this.SerializeCompactRdfElementProps(xmpNode3, level + 2);
				}
				this.WriteIndent(level + 1);
				this.Write("</rdf:Description>");
				this.WriteNewline();
				return;
			}
			this.Write("/>");
			this.WriteNewline();
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000C180 File Offset: 0x0000A380
		private bool SerializeCompactRdfAttrProps(XmpNode parentNode, int indent)
		{
			bool flag = true;
			IIterator iterator = parentNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (XmpSerializerRdf.CanBeRdfAttrProp(xmpNode))
				{
					this.WriteNewline();
					this.WriteIndent(indent);
					this.Write(xmpNode.Name);
					this.Write("=\"");
					this.AppendNodeValue(xmpNode.Value, true);
					this.Write('"');
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000C1F4 File Offset: 0x0000A3F4
		private void SerializeCompactRdfElementProps(XmpNode parentNode, int indent)
		{
			IIterator iterator = parentNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (!XmpSerializerRdf.CanBeRdfAttrProp(xmpNode))
				{
					bool flag = true;
					bool flag2 = true;
					string text = xmpNode.Name;
					if (text == "[]")
					{
						text = "rdf:li";
					}
					this.WriteIndent(indent);
					this.Write('<');
					this.Write(text);
					bool flag3 = false;
					bool flag4 = false;
					IIterator iterator2 = xmpNode.IterateQualifier();
					while (iterator2.HasNext())
					{
						XmpNode xmpNode2 = (XmpNode)iterator2.Next();
						if (!XmpSerializerRdf.RdfAttrQualifier.Contains(xmpNode2.Name))
						{
							flag3 = true;
						}
						else
						{
							flag4 = xmpNode2.Name == "rdf:resource";
							this.Write(' ');
							this.Write(xmpNode2.Name);
							this.Write("=\"");
							this.AppendNodeValue(xmpNode2.Value, true);
							this.Write('"');
						}
					}
					if (flag3)
					{
						this.SerializeCompactRdfGeneralQualifier(indent, xmpNode);
					}
					else if (!xmpNode.Options.IsCompositeProperty)
					{
						object[] array = this.SerializeCompactRdfSimpleProp(xmpNode);
						flag = (bool)array[0];
						flag2 = (bool)array[1];
					}
					else if (xmpNode.Options.IsArray)
					{
						this.SerializeCompactRdfArrayProp(xmpNode, indent);
					}
					else
					{
						flag = this.SerializeCompactRdfStructProp(xmpNode, indent, flag4);
					}
					if (flag)
					{
						if (flag2)
						{
							this.WriteIndent(indent);
						}
						this.Write("</");
						this.Write(text);
						this.Write('>');
						this.WriteNewline();
					}
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000C374 File Offset: 0x0000A574
		private object[] SerializeCompactRdfSimpleProp(XmpNode node)
		{
			bool flag = true;
			bool flag2 = true;
			if (node.Options.IsUri)
			{
				this.Write(" rdf:resource=\"");
				this.AppendNodeValue(node.Value, true);
				this.Write("\"/>");
				this.WriteNewline();
				flag = false;
			}
			else if (string.IsNullOrEmpty(node.Value))
			{
				this.Write("/>");
				this.WriteNewline();
				flag = false;
			}
			else
			{
				this.Write('>');
				this.AppendNodeValue(node.Value, false);
				flag2 = false;
			}
			return new object[] { flag, flag2 };
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000C410 File Offset: 0x0000A610
		private void SerializeCompactRdfArrayProp(XmpNode node, int indent)
		{
			this.Write('>');
			this.WriteNewline();
			this.EmitRdfArrayTag(node, true, indent + 1);
			if (node.Options.IsArrayAltText)
			{
				XmpNodeUtils.NormalizeLangArray(node);
			}
			this.SerializeCompactRdfElementProps(node, indent + 2);
			this.EmitRdfArrayTag(node, false, indent + 1);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000C460 File Offset: 0x0000A660
		private bool SerializeCompactRdfStructProp(XmpNode node, int indent, bool hasRdfResourceQual)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			IIterator iterator = node.IterateChildren();
			while (iterator.HasNext())
			{
				if (XmpSerializerRdf.CanBeRdfAttrProp((XmpNode)iterator.Next()))
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
				if (flag && flag2)
				{
					break;
				}
			}
			if (hasRdfResourceQual && flag2)
			{
				throw new XmpException("Can't mix rdf:resource qualifier and element fields", XmpErrorCode.BadRdf);
			}
			if (!node.HasChildren)
			{
				this.Write(" rdf:parseType=\"Resource\"/>");
				this.WriteNewline();
				flag3 = false;
			}
			else if (!flag2)
			{
				this.SerializeCompactRdfAttrProps(node, indent + 1);
				this.Write("/>");
				this.WriteNewline();
				flag3 = false;
			}
			else if (!flag)
			{
				this.Write(" rdf:parseType=\"Resource\">");
				this.WriteNewline();
				this.SerializeCompactRdfElementProps(node, indent + 1);
			}
			else
			{
				this.Write('>');
				this.WriteNewline();
				this.WriteIndent(indent + 1);
				this.Write("<rdf:Description");
				this.SerializeCompactRdfAttrProps(node, indent + 2);
				this.Write(">");
				this.WriteNewline();
				this.SerializeCompactRdfElementProps(node, indent + 1);
				this.WriteIndent(indent + 1);
				this.Write("</rdf:Description>");
				this.WriteNewline();
			}
			return flag3;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000C580 File Offset: 0x0000A780
		private void SerializeCompactRdfGeneralQualifier(int indent, XmpNode node)
		{
			this.Write(" rdf:parseType=\"Resource\">");
			this.WriteNewline();
			this.SerializeCanonicalRdfProperty(node, false, true, indent + 1);
			IIterator iterator = node.IterateQualifier();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				this.SerializeCanonicalRdfProperty(xmpNode, false, false, indent + 1);
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000C5D4 File Offset: 0x0000A7D4
		private void SerializeCanonicalRdfSchema(XmpNode schemaNode, int level)
		{
			IIterator iterator = schemaNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				this.SerializeCanonicalRdfProperty(xmpNode, this._options.UseCanonicalFormat, false, level + 2);
			}
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000C614 File Offset: 0x0000A814
		private void DeclareUsedNamespaces(XmpNode node, ICollection<object> usedPrefixes, int indent)
		{
			if (node.Options.IsSchemaNode)
			{
				string text = node.Value.Substring(0, node.Value.Length - 1);
				this.DeclareNamespace(text, node.Name, usedPrefixes, indent);
			}
			else if (node.Options.IsStruct)
			{
				IIterator iterator = node.IterateChildren();
				while (iterator.HasNext())
				{
					XmpNode xmpNode = (XmpNode)iterator.Next();
					this.DeclareNamespace(xmpNode.Name, null, usedPrefixes, indent);
				}
			}
			IIterator iterator2 = node.IterateChildren();
			while (iterator2.HasNext())
			{
				XmpNode xmpNode2 = (XmpNode)iterator2.Next();
				this.DeclareUsedNamespaces(xmpNode2, usedPrefixes, indent);
			}
			IIterator iterator3 = node.IterateQualifier();
			while (iterator3.HasNext())
			{
				XmpNode xmpNode3 = (XmpNode)iterator3.Next();
				this.DeclareNamespace(xmpNode3.Name, null, usedPrefixes, indent);
				this.DeclareUsedNamespaces(xmpNode3, usedPrefixes, indent);
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000C6F8 File Offset: 0x0000A8F8
		private void DeclareNamespace(string prefix, string ns, ICollection<object> usedPrefixes, int indent)
		{
			if (ns == null)
			{
				QName qname = new QName(prefix);
				if (!qname.HasPrefix)
				{
					return;
				}
				prefix = qname.Prefix;
				ns = XmpMetaFactory.SchemaRegistry.GetNamespaceUri(prefix + ":");
				this.DeclareNamespace(prefix, ns, usedPrefixes, indent);
			}
			if (!usedPrefixes.Contains(prefix))
			{
				this.WriteNewline();
				this.WriteIndent(indent);
				this.Write("xmlns:");
				this.Write(prefix);
				this.Write("=\"");
				this.Write(ns);
				this.Write('"');
				usedPrefixes.Add(prefix);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000C78C File Offset: 0x0000A98C
		private void StartOuterRdfDescription(XmpNode schemaNode, int level)
		{
			this.WriteIndent(level + 1);
			this.Write("<rdf:Description rdf:about=");
			this.WriteTreeName();
			this.DeclareUsedNamespaces(schemaNode, new HashSet<object> { "xml", "rdf" }, level + 3);
			this.Write('>');
			this.WriteNewline();
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000C7E8 File Offset: 0x0000A9E8
		private void EndOuterRdfDescription(int level)
		{
			this.WriteIndent(level + 1);
			this.Write("</rdf:Description>");
			this.WriteNewline();
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000C804 File Offset: 0x0000AA04
		private void SerializeCanonicalRdfProperty(XmpNode node, bool useCanonicalRdf, bool emitAsRdfValue, int indent)
		{
			bool flag = true;
			bool flag2 = true;
			string text = node.Name;
			if (emitAsRdfValue)
			{
				text = "rdf:value";
			}
			else if (text == "[]")
			{
				text = "rdf:li";
			}
			this.WriteIndent(indent);
			this.Write('<');
			this.Write(text);
			bool flag3 = false;
			bool flag4 = false;
			IIterator iterator = node.IterateQualifier();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (!XmpSerializerRdf.RdfAttrQualifier.Contains(xmpNode.Name))
				{
					flag3 = true;
				}
				else
				{
					flag4 = xmpNode.Name == "rdf:resource";
					if (!emitAsRdfValue)
					{
						this.Write(' ');
						this.Write(xmpNode.Name);
						this.Write("=\"");
						this.AppendNodeValue(xmpNode.Value, true);
						this.Write('"');
					}
				}
			}
			if (flag3 && !emitAsRdfValue)
			{
				if (flag4)
				{
					throw new XmpException("Can't mix rdf:resource and general qualifiers", XmpErrorCode.BadRdf);
				}
				if (useCanonicalRdf)
				{
					this.Write(">");
					this.WriteNewline();
					indent++;
					this.WriteIndent(indent);
					this.Write("<rdf:Description");
					this.Write(">");
				}
				else
				{
					this.Write(" rdf:parseType=\"Resource\">");
				}
				this.WriteNewline();
				this.SerializeCanonicalRdfProperty(node, useCanonicalRdf, true, indent + 1);
				IIterator iterator2 = node.IterateQualifier();
				while (iterator2.HasNext())
				{
					XmpNode xmpNode2 = (XmpNode)iterator2.Next();
					if (!XmpSerializerRdf.RdfAttrQualifier.Contains(xmpNode2.Name))
					{
						this.SerializeCanonicalRdfProperty(xmpNode2, useCanonicalRdf, false, indent + 1);
					}
				}
				if (useCanonicalRdf)
				{
					this.WriteIndent(indent);
					this.Write("</rdf:Description>");
					this.WriteNewline();
					indent--;
				}
			}
			else if (!node.Options.IsCompositeProperty)
			{
				if (node.Options.IsUri)
				{
					this.Write(" rdf:resource=\"");
					this.AppendNodeValue(node.Value, true);
					this.Write("\"/>");
					this.WriteNewline();
					flag = false;
				}
				else if (string.IsNullOrEmpty(node.Value))
				{
					this.Write("/>");
					this.WriteNewline();
					flag = false;
				}
				else
				{
					this.Write('>');
					this.AppendNodeValue(node.Value, false);
					flag2 = false;
				}
			}
			else if (node.Options.IsArray)
			{
				this.Write('>');
				this.WriteNewline();
				this.EmitRdfArrayTag(node, true, indent + 1);
				if (node.Options.IsArrayAltText)
				{
					XmpNodeUtils.NormalizeLangArray(node);
				}
				IIterator iterator3 = node.IterateChildren();
				while (iterator3.HasNext())
				{
					XmpNode xmpNode3 = (XmpNode)iterator3.Next();
					this.SerializeCanonicalRdfProperty(xmpNode3, useCanonicalRdf, false, indent + 2);
				}
				this.EmitRdfArrayTag(node, false, indent + 1);
			}
			else if (!flag4)
			{
				if (!node.HasChildren)
				{
					if (useCanonicalRdf)
					{
						this.Write(">");
						this.WriteNewline();
						this.WriteIndent(indent + 1);
						this.Write("<rdf:Description/>");
					}
					else
					{
						this.Write(" rdf:parseType=\"Resource\"/>");
						flag = false;
					}
					this.WriteNewline();
				}
				else
				{
					if (useCanonicalRdf)
					{
						this.Write(">");
						this.WriteNewline();
						indent++;
						this.WriteIndent(indent);
						this.Write("<rdf:Description");
						this.Write(">");
					}
					else
					{
						this.Write(" rdf:parseType=\"Resource\">");
					}
					this.WriteNewline();
					IIterator iterator4 = node.IterateChildren();
					while (iterator4.HasNext())
					{
						XmpNode xmpNode4 = (XmpNode)iterator4.Next();
						this.SerializeCanonicalRdfProperty(xmpNode4, useCanonicalRdf, false, indent + 1);
					}
					if (useCanonicalRdf)
					{
						this.WriteIndent(indent);
						this.Write("</rdf:Description>");
						this.WriteNewline();
						indent--;
					}
				}
			}
			else
			{
				IIterator iterator5 = node.IterateChildren();
				while (iterator5.HasNext())
				{
					XmpNode xmpNode5 = (XmpNode)iterator5.Next();
					if (!XmpSerializerRdf.CanBeRdfAttrProp(xmpNode5))
					{
						throw new XmpException("Can't mix rdf:resource and complex fields", XmpErrorCode.BadRdf);
					}
					this.WriteNewline();
					this.WriteIndent(indent + 1);
					this.Write(' ');
					this.Write(xmpNode5.Name);
					this.Write("=\"");
					this.AppendNodeValue(xmpNode5.Value, true);
					this.Write('"');
				}
				this.Write("/>");
				this.WriteNewline();
				flag = false;
			}
			if (flag)
			{
				if (flag2)
				{
					this.WriteIndent(indent);
				}
				this.Write("</");
				this.Write(text);
				this.Write('>');
				this.WriteNewline();
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000CC7C File Offset: 0x0000AE7C
		private void EmitRdfArrayTag(XmpNode arrayNode, bool isStartTag, int indent)
		{
			if (isStartTag || arrayNode.HasChildren)
			{
				this.WriteIndent(indent);
				this.Write(isStartTag ? "<rdf:" : "</rdf:");
				if (arrayNode.Options.IsArrayAlternate)
				{
					this.Write("Alt");
				}
				else
				{
					this.Write(arrayNode.Options.IsArrayOrdered ? "Seq" : "Bag");
				}
				if (isStartTag && !arrayNode.HasChildren)
				{
					this.Write("/>");
				}
				else
				{
					this.Write(">");
				}
				this.WriteNewline();
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000CD12 File Offset: 0x0000AF12
		private void AppendNodeValue(string value, bool forAttribute)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			this.Write(Utils.EscapeXml(value, forAttribute, true));
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		private static bool CanBeRdfAttrProp(XmpNode node)
		{
			return !node.HasQualifier && !node.Options.IsUri && !node.Options.IsCompositeProperty && node.Name != "[]";
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000CD64 File Offset: 0x0000AF64
		private void WriteIndent(int times)
		{
			for (int i = this._options.BaseIndent + times; i > 0; i--)
			{
				this._writer.Write(this._options.Indent);
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000CD9F File Offset: 0x0000AF9F
		private void Write(int c)
		{
			this._writer.Write(c);
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000CDAD File Offset: 0x0000AFAD
		private void Write(char c)
		{
			this._writer.Write(c);
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000CDBB File Offset: 0x0000AFBB
		private void Write(string str)
		{
			this._writer.Write(str);
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000CDC9 File Offset: 0x0000AFC9
		private void WriteChars(int number, char c)
		{
			while (number > 0)
			{
				this._writer.Write(c);
				number--;
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000CDE2 File Offset: 0x0000AFE2
		private void WriteNewline()
		{
			this._writer.Write(this._options.Newline);
		}

		// Token: 0x04000115 RID: 277
		private const int DefaultPad = 2048;

		// Token: 0x04000116 RID: 278
		private const string PacketHeader = "<?xpacket begin=\"\ufeff\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>";

		// Token: 0x04000117 RID: 279
		private const string PacketTrailer = "<?xpacket end=\"";

		// Token: 0x04000118 RID: 280
		private const string PacketTrailer2 = "\"?>";

		// Token: 0x04000119 RID: 281
		private const string RdfXmpmetaStart = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"";

		// Token: 0x0400011A RID: 282
		private const string RdfXmpmetaEnd = "</x:xmpmeta>";

		// Token: 0x0400011B RID: 283
		private const string RdfRdfStart = "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">";

		// Token: 0x0400011C RID: 284
		private const string RdfRdfEnd = "</rdf:RDF>";

		// Token: 0x0400011D RID: 285
		private const string RdfSchemaStart = "<rdf:Description rdf:about=";

		// Token: 0x0400011E RID: 286
		private const string RdfSchemaEnd = "</rdf:Description>";

		// Token: 0x0400011F RID: 287
		private const string RdfStructStart = "<rdf:Description";

		// Token: 0x04000120 RID: 288
		private const string RdfStructEnd = "</rdf:Description>";

		// Token: 0x04000121 RID: 289
		private const string RdfEmptyStruct = "<rdf:Description/>";

		// Token: 0x04000122 RID: 290
		private static readonly ICollection<object> RdfAttrQualifier = new HashSet<object>(new string[] { "xml:lang", "rdf:resource", "rdf:ID", "rdf:bagID", "rdf:nodeID" });

		// Token: 0x04000123 RID: 291
		private XmpMeta _xmp;

		// Token: 0x04000124 RID: 292
		private Stream _stream;

		// Token: 0x04000125 RID: 293
		private StreamWriter _writer;

		// Token: 0x04000126 RID: 294
		private SerializeOptions _options;

		// Token: 0x04000127 RID: 295
		private int _unicodeSize = 1;

		// Token: 0x04000128 RID: 296
		private int _padding;

		// Token: 0x04000129 RID: 297
		private long _startPos;
	}
}
