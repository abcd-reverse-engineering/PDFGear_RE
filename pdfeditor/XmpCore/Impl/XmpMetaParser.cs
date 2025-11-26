using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x02000039 RID: 57
	public static class XmpMetaParser
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000866B File Offset: 0x0000686B
		public static IXmpMeta Parse(Stream stream, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNull(stream);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlDoc(XmpMetaParser.ParseXmlFromInputStream(stream, options), options);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000868C File Offset: 0x0000688C
		public static IXmpMeta Parse(byte[] bytes, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNull(bytes);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlDoc(XmpMetaParser.ParseXmlFromByteBuffer(new ByteBuffer(bytes), options), options);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000086B2 File Offset: 0x000068B2
		public static IXmpMeta Parse(ByteBuffer byteBuffer, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNull(byteBuffer);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlDoc(XmpMetaParser.ParseXmlFromByteBuffer(byteBuffer, options), options);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000086D3 File Offset: 0x000068D3
		public static IXmpMeta Parse(string xmlStr, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNullOrEmpty(xmlStr);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlDoc(XmpMetaParser.ParseXmlString(xmlStr, options), options);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000086F4 File Offset: 0x000068F4
		public static IXmpMeta Parse(XDocument doc, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNull(doc);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlDoc(doc, options);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000870F File Offset: 0x0000690F
		public static XDocument Extract(byte[] bytes, ParseOptions options = null)
		{
			ParameterAsserts.AssertNotNull(bytes);
			options = options ?? new ParseOptions();
			return XmpMetaParser.ParseXmlFromByteBuffer(new ByteBuffer(bytes), options);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00008730 File Offset: 0x00006930
		private static IXmpMeta ParseXmlDoc(XDocument document, ParseOptions options)
		{
			using (IEnumerator<XElement> enumerator = (from d in document.Descendants()
				where d.Attributes().Count<XAttribute>() > 1
				select d).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement node = enumerator.Current;
					IOrderedEnumerable<XAttribute> orderedEnumerable = (from n in node.Attributes()
						orderby !n.IsNamespaceDeclaration
						select n).ThenBy((XAttribute t) => node.GetPrefixOfNamespace(t.Name.Namespace), StringComparer.Ordinal).ThenBy((XAttribute s) => s.Name.LocalName, StringComparer.Ordinal);
					node.ReplaceAttributes(orderedEnumerable);
				}
			}
			object[] array = XmpMetaParser.FindRootNode(document.Nodes(), options.RequireXmpMeta, new object[3]);
			if (array == null || array[1] != XmpMetaParser.XmpRdf)
			{
				return new XmpMeta();
			}
			XmpMeta xmpMeta = ParseRdf.Parse((XElement)array[0], options);
			xmpMeta.SetPacketHeader((string)array[2]);
			if (options.OmitNormalization)
			{
				return xmpMeta;
			}
			return XmpNormalizer.Process(xmpMeta, options);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00008888 File Offset: 0x00006A88
		private static XDocument ParseXmlFromInputStream(Stream stream, ParseOptions options)
		{
			if (!options.AcceptLatin1 && !options.FixControlChars && !options.DisallowDoctype)
			{
				return XmpMetaParser.ParseStream(stream, options);
			}
			XDocument xdocument;
			try
			{
				xdocument = XmpMetaParser.ParseXmlFromByteBuffer(new ByteBuffer(stream), options);
			}
			catch (IOException ex)
			{
				throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, ex);
			}
			return xdocument;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000088E8 File Offset: 0x00006AE8
		private static XDocument ParseXmlFromByteBuffer(ByteBuffer buffer, ParseOptions options)
		{
			XDocument xdocument;
			try
			{
				xdocument = XmpMetaParser.ParseStream(buffer.GetByteStream(), options);
			}
			catch (XmpException ex)
			{
				if (ex.ErrorCode != XmpErrorCode.BadXml && ex.ErrorCode != XmpErrorCode.BadStream)
				{
					throw;
				}
				if (options.AcceptLatin1)
				{
					buffer = Latin1Converter.Convert(buffer);
				}
				if (options.FixControlChars)
				{
					try
					{
						return XmpMetaParser.ParseTextReader(new FixAsciiControlsReader(new StreamReader(buffer.GetByteStream(), buffer.GetEncoding())), options);
					}
					catch
					{
						throw new XmpException("Unsupported Encoding", XmpErrorCode.InternalFailure, ex);
					}
				}
				xdocument = XmpMetaParser.ParseStream(buffer.GetByteStream(), options);
			}
			return xdocument;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00008994 File Offset: 0x00006B94
		private static XDocument ParseXmlString(string input, ParseOptions options)
		{
			XDocument xdocument;
			try
			{
				using (StringReader stringReader = new StringReader(input))
				{
					xdocument = XmpMetaParser.ParseTextReader(stringReader, options);
				}
			}
			catch (XmpException ex)
			{
				if (ex.ErrorCode != XmpErrorCode.BadXml || !options.FixControlChars)
				{
					throw;
				}
				xdocument = XmpMetaParser.ParseTextReader(new FixAsciiControlsReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)))), options);
			}
			return xdocument;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00008A14 File Offset: 0x00006C14
		private static XDocument ParseStream(Stream stream, ParseOptions options)
		{
			XDocument xdocument;
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				if (options.DisallowDoctype)
				{
					xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
				}
				else
				{
					xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
				}
				xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
				using (XmlReader xmlReader = XmlReader.Create(new StreamReader(stream), xmlReaderSettings))
				{
					xdocument = XDocument.Load(xmlReader);
				}
			}
			catch (XmlException ex)
			{
				throw new XmpException("XML parsing failure", XmpErrorCode.BadXml, ex);
			}
			catch (IOException ex2)
			{
				throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, ex2);
			}
			catch (Exception ex3)
			{
				throw new XmpException("XML Parser not correctly configured", XmpErrorCode.Unknown, ex3);
			}
			return xdocument;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00008AD8 File Offset: 0x00006CD8
		private static XDocument ParseTextReader(TextReader reader, ParseOptions options)
		{
			XDocument xdocument;
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				if (options.DisallowDoctype)
				{
					xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
				}
				else
				{
					xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
				}
				xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
				using (XmlReader xmlReader = XmlReader.Create(reader, xmlReaderSettings))
				{
					xdocument = XDocument.Load(xmlReader);
				}
			}
			catch (XmlException ex)
			{
				throw new XmpException("XML parsing failure", XmpErrorCode.BadXml, ex);
			}
			catch (IOException ex2)
			{
				throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, ex2);
			}
			catch (Exception ex3)
			{
				throw new XmpException("XML Parser not correctly configured", XmpErrorCode.Unknown, ex3);
			}
			return xdocument;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00008B98 File Offset: 0x00006D98
		private static object[] FindRootNode(IEnumerable<XNode> nodes, bool xmpmetaRequired, object[] result)
		{
			foreach (XNode xnode in nodes)
			{
				if (XmlNodeType.ProcessingInstruction == xnode.NodeType && ((XProcessingInstruction)xnode).Target == "xpacket")
				{
					result[2] = ((XProcessingInstruction)xnode).Data;
				}
				else if (XmlNodeType.Element == xnode.NodeType)
				{
					XElement xelement = (XElement)xnode;
					string namespaceName = xelement.Name.NamespaceName;
					string localName = xelement.Name.LocalName;
					if ((localName == "xmpmeta" || localName == "xapmeta") && namespaceName == "adobe:ns:meta/")
					{
						return XmpMetaParser.FindRootNode(xelement.Nodes(), false, result);
					}
					if (!xmpmetaRequired && localName == "RDF" && namespaceName == "http://www.w3.org/1999/02/22-rdf-syntax-ns#")
					{
						if (result != null)
						{
							result[0] = xnode;
							result[1] = XmpMetaParser.XmpRdf;
						}
						return result;
					}
					object[] array = XmpMetaParser.FindRootNode(xelement.Nodes(), xmpmetaRequired, result);
					if (array != null)
					{
						return array;
					}
				}
			}
			return null;
		}

		// Token: 0x040000FD RID: 253
		private static readonly object XmpRdf = new object();
	}
}
