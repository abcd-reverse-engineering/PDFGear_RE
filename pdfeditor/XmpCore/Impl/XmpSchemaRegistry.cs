using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sharpen;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003D RID: 61
	public sealed class XmpSchemaRegistry : IXmpSchemaRegistry
	{
		// Token: 0x060002C4 RID: 708 RVA: 0x0000ACDC File Offset: 0x00008EDC
		public XmpSchemaRegistry()
		{
			try
			{
				this.RegisterStandardNamespaces();
				this.RegisterStandardAliases();
			}
			catch (XmpException ex)
			{
				throw new Exception("The XMPSchemaRegistry cannot be initialized!", ex);
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000AD58 File Offset: 0x00008F58
		public string RegisterNamespace(string namespaceUri, string suggestedPrefix)
		{
			object @lock = this._lock;
			string text2;
			lock (@lock)
			{
				ParameterAsserts.AssertSchemaNs(namespaceUri);
				ParameterAsserts.AssertPrefix(suggestedPrefix);
				if (suggestedPrefix[suggestedPrefix.Length - 1] != ':')
				{
					suggestedPrefix += ":";
				}
				if (!Utils.IsXmlNameNs(suggestedPrefix.Substring(0, suggestedPrefix.Length - 1)))
				{
					throw new XmpException("The prefix is a bad XML name", XmpErrorCode.BadXml);
				}
				string text;
				if (this._namespaceToPrefixMap.TryGetValue(namespaceUri, out text))
				{
					text2 = text;
				}
				else
				{
					if (this._prefixToNamespaceMap.ContainsKey(suggestedPrefix))
					{
						string text3 = suggestedPrefix;
						int num = 1;
						while (this._prefixToNamespaceMap.ContainsKey(text3))
						{
							text3 = suggestedPrefix.Substring(0, suggestedPrefix.Length - 1) + "_" + num.ToString() + "_:";
							num++;
						}
						suggestedPrefix = text3;
					}
					this._prefixToNamespaceMap[suggestedPrefix] = namespaceUri;
					this._namespaceToPrefixMap[namespaceUri] = suggestedPrefix;
					text2 = suggestedPrefix;
				}
			}
			return text2;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000AE6C File Offset: 0x0000906C
		public void DeleteNamespace(string namespaceUri)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				string namespacePrefix = this.GetNamespacePrefix(namespaceUri);
				if (namespacePrefix != null)
				{
					this._namespaceToPrefixMap.Remove(namespaceUri);
					this._prefixToNamespaceMap.Remove(namespacePrefix);
				}
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000AECC File Offset: 0x000090CC
		public string GetNamespacePrefix(string namespaceUri)
		{
			object @lock = this._lock;
			string text;
			lock (@lock)
			{
				string text2;
				text = (this._namespaceToPrefixMap.TryGetValue(namespaceUri, out text2) ? text2 : null);
			}
			return text;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000AF1C File Offset: 0x0000911C
		public string GetNamespaceUri(string namespacePrefix)
		{
			object @lock = this._lock;
			string text;
			lock (@lock)
			{
				if (namespacePrefix != null && !namespacePrefix.EndsWith(":"))
				{
					namespacePrefix += ":";
				}
				string text2;
				text = (this._prefixToNamespaceMap.TryGetValue(namespacePrefix, out text2) ? text2 : null);
			}
			return text;
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x0000AF8C File Offset: 0x0000918C
		public IDictionary<string, string> Namespaces
		{
			get
			{
				object @lock = this._lock;
				IDictionary<string, string> dictionary;
				lock (@lock)
				{
					dictionary = new Dictionary<string, string>(this._namespaceToPrefixMap);
				}
				return dictionary;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000AFD4 File Offset: 0x000091D4
		public IDictionary<string, string> Prefixes
		{
			get
			{
				object @lock = this._lock;
				IDictionary<string, string> dictionary;
				lock (@lock)
				{
					dictionary = new Dictionary<string, string>(this._prefixToNamespaceMap);
				}
				return dictionary;
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000B01C File Offset: 0x0000921C
		private void RegisterStandardNamespaces()
		{
			this.RegisterNamespace("http://www.w3.org/XML/1998/namespace", "xml");
			this.RegisterNamespace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "rdf");
			this.RegisterNamespace("http://purl.org/dc/elements/1.1/", "dc");
			this.RegisterNamespace("http://iptc.org/std/Iptc4xmpCore/1.0/xmlns/", "Iptc4xmpCore");
			this.RegisterNamespace("http://iptc.org/std/Iptc4xmpExt/2008-02-29/", "Iptc4xmpExt");
			this.RegisterNamespace("http://ns.adobe.com/DICOM/", "DICOM");
			this.RegisterNamespace("http://ns.useplus.org/ldf/xmp/1.0/", "plus");
			this.RegisterNamespace("adobe:ns:meta/", "x");
			this.RegisterNamespace("http://ns.adobe.com/iX/1.0/", "iX");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/", "xmp");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/rights/", "xmpRights");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/mm/", "xmpMM");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/bj/", "xmpBJ");
			this.RegisterNamespace("http://ns.adobe.com/xmp/note/", "xmpNote");
			this.RegisterNamespace("http://ns.adobe.com/pdf/1.3/", "pdf");
			this.RegisterNamespace("http://ns.adobe.com/pdfx/1.3/", "pdfx");
			this.RegisterNamespace("http://www.npes.org/pdfx/ns/id/", "pdfxid");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/schema#", "pdfaSchema");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/property#", "pdfaProperty");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/type#", "pdfaType");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/field#", "pdfaField");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/id/", "pdfaid");
			this.RegisterNamespace("http://www.aiim.org/pdfa/ns/extension/", "pdfaExtension");
			this.RegisterNamespace("http://ns.adobe.com/photoshop/1.0/", "photoshop");
			this.RegisterNamespace("http://ns.adobe.com/album/1.0/", "album");
			this.RegisterNamespace("http://ns.adobe.com/exif/1.0/", "exif");
			this.RegisterNamespace("http://cipa.jp/exif/1.0/", "exifEX");
			this.RegisterNamespace("http://ns.adobe.com/exif/1.0/aux/", "aux");
			this.RegisterNamespace("http://ns.adobe.com/tiff/1.0/", "tiff");
			this.RegisterNamespace("http://ns.adobe.com/png/1.0/", "png");
			this.RegisterNamespace("http://ns.adobe.com/jpeg/1.0/", "jpeg");
			this.RegisterNamespace("http://ns.adobe.com/jp2k/1.0/", "jp2k");
			this.RegisterNamespace("http://ns.adobe.com/camera-raw-settings/1.0/", "crs");
			this.RegisterNamespace("http://ns.adobe.com/StockPhoto/1.0/", "bmsp");
			this.RegisterNamespace("http://ns.adobe.com/creatorAtom/1.0/", "creatorAtom");
			this.RegisterNamespace("http://ns.adobe.com/asf/1.0/", "asf");
			this.RegisterNamespace("http://ns.adobe.com/xmp/wav/1.0/", "wav");
			this.RegisterNamespace("http://ns.adobe.com/bwf/bext/1.0/", "bext");
			this.RegisterNamespace("http://ns.adobe.com/riff/info/", "riffinfo");
			this.RegisterNamespace("http://ns.adobe.com/xmp/1.0/Script/", "xmpScript");
			this.RegisterNamespace("http://ns.adobe.com/TransformXMP/", "txmp");
			this.RegisterNamespace("http://ns.adobe.com/swf/1.0/", "swf");
			this.RegisterNamespace("http://ns.adobe.com/ccv/1.0/", "ccv");
			this.RegisterNamespace("http://ns.adobe.com/xmp/1.0/DynamicMedia/", "xmpDM");
			this.RegisterNamespace("http://ns.adobe.com/xmp/transient/1.0/", "xmpx");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/t/", "xmpT");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/t/pg/", "xmpTPg");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/g/", "xmpG");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/g/img/", "xmpGImg");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/Font#", "stFnt");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/Dimensions#", "stDim");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/ResourceEvent#", "stEvt");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/ResourceRef#", "stRef");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/Version#", "stVer");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/Job#", "stJob");
			this.RegisterNamespace("http://ns.adobe.com/xap/1.0/sType/ManifestItem#", "stMfs");
			this.RegisterNamespace("http://ns.adobe.com/xmp/Identifier/qual/1.0/", "xmpidq");
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000B3F4 File Offset: 0x000095F4
		public IXmpAliasInfo ResolveAlias(string aliasNs, string aliasProp)
		{
			object @lock = this._lock;
			IXmpAliasInfo xmpAliasInfo;
			lock (@lock)
			{
				string namespacePrefix = this.GetNamespacePrefix(aliasNs);
				if (namespacePrefix == null)
				{
					xmpAliasInfo = null;
				}
				else
				{
					IXmpAliasInfo xmpAliasInfo2;
					xmpAliasInfo = (this._aliasMap.TryGetValue(namespacePrefix + aliasProp, out xmpAliasInfo2) ? xmpAliasInfo2 : null);
				}
			}
			return xmpAliasInfo;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000B45C File Offset: 0x0000965C
		public IXmpAliasInfo FindAlias(string qname)
		{
			object @lock = this._lock;
			IXmpAliasInfo xmpAliasInfo;
			lock (@lock)
			{
				IXmpAliasInfo xmpAliasInfo2;
				xmpAliasInfo = (this._aliasMap.TryGetValue(qname, out xmpAliasInfo2) ? xmpAliasInfo2 : null);
			}
			return xmpAliasInfo;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000B4AC File Offset: 0x000096AC
		public IEnumerable<IXmpAliasInfo> FindAliases(string aliasNs)
		{
			object @lock = this._lock;
			IEnumerable<IXmpAliasInfo> enumerable;
			lock (@lock)
			{
				string namespacePrefix = this.GetNamespacePrefix(aliasNs);
				List<IXmpAliasInfo> list = new List<IXmpAliasInfo>();
				if (namespacePrefix != null)
				{
					Iterator<string> iterator = this._aliasMap.Keys.Iterator<string>();
					while (iterator.HasNext())
					{
						string text = iterator.Next();
						if (text.StartsWith(namespacePrefix))
						{
							list.Add(this.FindAlias(text));
						}
					}
				}
				enumerable = list;
			}
			return enumerable;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000B53C File Offset: 0x0000973C
		private void RegisterAlias(string aliasNs, string aliasProp, string actualNs, string actualProp, AliasOptions aliasForm)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				ParameterAsserts.AssertSchemaNs(aliasNs);
				ParameterAsserts.AssertPropName(aliasProp);
				ParameterAsserts.AssertSchemaNs(actualNs);
				ParameterAsserts.AssertPropName(actualProp);
				AliasOptions aliasOptions = ((aliasForm != null) ? new AliasOptions(XmpNodeUtils.VerifySetOptions(aliasForm.ToPropertyOptions(), null).GetOptions()) : new AliasOptions());
				if (this._p.IsMatch(aliasProp) || this._p.IsMatch(actualProp))
				{
					throw new XmpException("Alias and actual property names must be simple", XmpErrorCode.BadXPath);
				}
				string namespacePrefix = this.GetNamespacePrefix(aliasNs);
				string namespacePrefix2 = this.GetNamespacePrefix(actualNs);
				if (namespacePrefix == null)
				{
					throw new XmpException("Alias namespace is not registered", XmpErrorCode.BadSchema);
				}
				if (namespacePrefix2 == null)
				{
					throw new XmpException("Actual namespace is not registered", XmpErrorCode.BadSchema);
				}
				string text = namespacePrefix + aliasProp;
				if (this._aliasMap.ContainsKey(text))
				{
					throw new XmpException("Alias is already existing", XmpErrorCode.BadParam);
				}
				if (this._aliasMap.ContainsKey(namespacePrefix2 + actualProp))
				{
					throw new XmpException("Actual property is already an alias, use the base property", XmpErrorCode.BadParam);
				}
				this._aliasMap[text] = new XmpSchemaRegistry.XmpAliasInfo(actualNs, namespacePrefix2, actualProp, aliasOptions);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000B668 File Offset: 0x00009868
		public IDictionary<string, IXmpAliasInfo> Aliases
		{
			get
			{
				object @lock = this._lock;
				IDictionary<string, IXmpAliasInfo> dictionary;
				lock (@lock)
				{
					dictionary = new Dictionary<string, IXmpAliasInfo>(this._aliasMap);
				}
				return dictionary;
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000B6B0 File Offset: 0x000098B0
		private void RegisterStandardAliases()
		{
			AliasOptions aliasOptions = new AliasOptions
			{
				IsArrayOrdered = true
			};
			AliasOptions aliasOptions2 = new AliasOptions
			{
				IsArrayAltText = true
			};
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Author", "http://purl.org/dc/elements/1.1/", "creator", aliasOptions);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Authors", "http://purl.org/dc/elements/1.1/", "creator", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Description", "http://purl.org/dc/elements/1.1/", "description", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Format", "http://purl.org/dc/elements/1.1/", "format", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Keywords", "http://purl.org/dc/elements/1.1/", "subject", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Locale", "http://purl.org/dc/elements/1.1/", "language", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/", "Title", "http://purl.org/dc/elements/1.1/", "title", null);
			this.RegisterAlias("http://ns.adobe.com/xap/1.0/rights/", "Copyright", "http://purl.org/dc/elements/1.1/", "rights", null);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "Author", "http://purl.org/dc/elements/1.1/", "creator", aliasOptions);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "BaseURL", "http://ns.adobe.com/xap/1.0/", "BaseURL", null);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "CreationDate", "http://ns.adobe.com/xap/1.0/", "CreateDate", null);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "Creator", "http://ns.adobe.com/xap/1.0/", "CreatorTool", null);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "ModDate", "http://ns.adobe.com/xap/1.0/", "ModifyDate", null);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "Subject", "http://purl.org/dc/elements/1.1/", "description", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/pdf/1.3/", "Title", "http://purl.org/dc/elements/1.1/", "title", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Author", "http://purl.org/dc/elements/1.1/", "creator", aliasOptions);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Caption", "http://purl.org/dc/elements/1.1/", "description", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Copyright", "http://purl.org/dc/elements/1.1/", "rights", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Keywords", "http://purl.org/dc/elements/1.1/", "subject", null);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Marked", "http://ns.adobe.com/xap/1.0/rights/", "Marked", null);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "Title", "http://purl.org/dc/elements/1.1/", "title", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/photoshop/1.0/", "WebStatement", "http://ns.adobe.com/xap/1.0/rights/", "WebStatement", null);
			this.RegisterAlias("http://ns.adobe.com/tiff/1.0/", "Artist", "http://purl.org/dc/elements/1.1/", "creator", aliasOptions);
			this.RegisterAlias("http://ns.adobe.com/tiff/1.0/", "Copyright", "http://purl.org/dc/elements/1.1/", "rights", null);
			this.RegisterAlias("http://ns.adobe.com/tiff/1.0/", "DateTime", "http://ns.adobe.com/xap/1.0/", "ModifyDate", null);
			this.RegisterAlias("http://ns.adobe.com/exif/1.0/", "DateTimeDigitized", "http://ns.adobe.com/xap/1.0/", "CreateDate", null);
			this.RegisterAlias("http://ns.adobe.com/tiff/1.0/", "ImageDescription", "http://purl.org/dc/elements/1.1/", "description", null);
			this.RegisterAlias("http://ns.adobe.com/tiff/1.0/", "Software", "http://ns.adobe.com/xap/1.0/", "CreatorTool", null);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "Author", "http://purl.org/dc/elements/1.1/", "creator", aliasOptions);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "Copyright", "http://purl.org/dc/elements/1.1/", "rights", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "CreationTime", "http://ns.adobe.com/xap/1.0/", "CreateDate", null);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "Description", "http://purl.org/dc/elements/1.1/", "description", aliasOptions2);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "ModificationTime", "http://ns.adobe.com/xap/1.0/", "ModifyDate", null);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "Software", "http://ns.adobe.com/xap/1.0/", "CreatorTool", null);
			this.RegisterAlias("http://ns.adobe.com/png/1.0/", "Title", "http://purl.org/dc/elements/1.1/", "title", aliasOptions2);
		}

		// Token: 0x04000110 RID: 272
		private readonly Dictionary<string, string> _namespaceToPrefixMap = new Dictionary<string, string>();

		// Token: 0x04000111 RID: 273
		private readonly Dictionary<string, string> _prefixToNamespaceMap = new Dictionary<string, string>();

		// Token: 0x04000112 RID: 274
		private readonly Dictionary<string, IXmpAliasInfo> _aliasMap = new Dictionary<string, IXmpAliasInfo>();

		// Token: 0x04000113 RID: 275
		private readonly Regex _p = new Regex("[/*?\\[\\]]");

		// Token: 0x04000114 RID: 276
		private readonly object _lock = new object();

		// Token: 0x020002D2 RID: 722
		private sealed class XmpAliasInfo : IXmpAliasInfo
		{
			// Token: 0x060028DC RID: 10460 RVA: 0x000BFB37 File Offset: 0x000BDD37
			public XmpAliasInfo(string actualNs, string actualPrefix, string actualProp, AliasOptions aliasOpts)
			{
				this.Namespace = actualNs;
				this.Prefix = actualPrefix;
				this.PropName = actualProp;
				this.AliasForm = aliasOpts;
			}

			// Token: 0x17000C63 RID: 3171
			// (get) Token: 0x060028DD RID: 10461 RVA: 0x000BFB5C File Offset: 0x000BDD5C
			public string Namespace { get; }

			// Token: 0x17000C64 RID: 3172
			// (get) Token: 0x060028DE RID: 10462 RVA: 0x000BFB64 File Offset: 0x000BDD64
			public string Prefix { get; }

			// Token: 0x17000C65 RID: 3173
			// (get) Token: 0x060028DF RID: 10463 RVA: 0x000BFB6C File Offset: 0x000BDD6C
			public string PropName { get; }

			// Token: 0x17000C66 RID: 3174
			// (get) Token: 0x060028E0 RID: 10464 RVA: 0x000BFB74 File Offset: 0x000BDD74
			public AliasOptions AliasForm { get; }

			// Token: 0x060028E1 RID: 10465 RVA: 0x000BFB7C File Offset: 0x000BDD7C
			public override string ToString()
			{
				return string.Format("{0}{1} NS({2}), FORM ({3})", new object[] { this.Prefix, this.PropName, this.Namespace, this.AliasForm });
			}
		}
	}
}
