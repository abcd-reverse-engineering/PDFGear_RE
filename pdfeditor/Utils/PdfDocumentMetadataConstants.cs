using System;
using System.Collections.Generic;
using Patagames.Pdf.Enums;

namespace pdfeditor.Utils
{
	// Token: 0x0200008D RID: 141
	public static class PdfDocumentMetadataConstants
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x0002DFA8 File Offset: 0x0002C1A8
		public static IReadOnlyList<PdfDocumentMetadata.MetaPropertyInfo> MetaProperties
		{
			get
			{
				IReadOnlyList<PdfDocumentMetadata.MetaPropertyInfo> readOnlyList;
				if ((readOnlyList = PdfDocumentMetadataConstants.metaProperties) == null)
				{
					List<PdfDocumentMetadata.MetaPropertyInfo> list = new List<PdfDocumentMetadata.MetaPropertyInfo>();
					list.Add(PdfDocumentMetadataConstants.Title);
					list.Add(PdfDocumentMetadataConstants.Description);
					list.Add(PdfDocumentMetadataConstants.Author);
					list.Add(PdfDocumentMetadataConstants.Subject);
					list.Add(PdfDocumentMetadataConstants.Keywords);
					list.Add(PdfDocumentMetadataConstants.Creator);
					list.Add(PdfDocumentMetadataConstants.Producer);
					list.Add(PdfDocumentMetadataConstants.CreationDate);
					list.Add(PdfDocumentMetadataConstants.ModificationDate);
					list.Add(PdfDocumentMetadataConstants.Trapped);
					readOnlyList = list;
					PdfDocumentMetadataConstants.metaProperties = list;
				}
				return readOnlyList;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x0002E037 File Offset: 0x0002C237
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Title
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Title),
					InfoDictionaryKey = "Title",
					XmpPropertyKey = "title",
					XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.LocalizedString
				};
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x0002E074 File Offset: 0x0002C274
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Description
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = null,
					InfoDictionaryKey = null,
					XmpPropertyKey = "description",
					XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.LocalizedString
				};
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x0002E0B9 File Offset: 0x0002C2B9
		public static PdfDocumentMetadata.MetaPropertyInfo<string[]> Author
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string[]>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Author),
					InfoDictionaryKey = "Author",
					XmpPropertyKey = "creator",
					XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.ArrayOrdered
				};
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x0002E0F4 File Offset: 0x0002C2F4
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Subject
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Subject),
					InfoDictionaryKey = "Subject",
					XmpPropertyKey = "description",
					XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.LocalizedString
				};
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x0002E12F File Offset: 0x0002C32F
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Keywords
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Keywords),
					InfoDictionaryKey = "Keywords",
					XmpPropertyKey = "Keywords",
					XmpPropertyNameSpace = "http://ns.adobe.com/pdf/1.3/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.String
				};
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0002E16A File Offset: 0x0002C36A
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Creator
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Creator),
					InfoDictionaryKey = "Creator",
					XmpPropertyKey = "CreatorTool",
					XmpPropertyNameSpace = "http://ns.adobe.com/xap/1.0/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.String
				};
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x0002E1A5 File Offset: 0x0002C3A5
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Producer
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Producer),
					InfoDictionaryKey = "Producer",
					XmpPropertyKey = "Producer",
					XmpPropertyNameSpace = "http://ns.adobe.com/pdf/1.3/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.String
				};
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x0002E1E0 File Offset: 0x0002C3E0
		public static PdfDocumentMetadata.MetaPropertyInfo<DateTimeOffset> CreationDate
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<DateTimeOffset>
				{
					DocumentTag = new DocumentTags?(DocumentTags.CreationDate),
					InfoDictionaryKey = "CreationDate",
					XmpPropertyKey = "CreateDate",
					XmpPropertyNameSpace = "http://ns.adobe.com/xap/1.0/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.DateTimeOffset
				};
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x0002E21B File Offset: 0x0002C41B
		public static PdfDocumentMetadata.MetaPropertyInfo<DateTimeOffset> ModificationDate
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<DateTimeOffset>
				{
					DocumentTag = new DocumentTags?(DocumentTags.ModificationDate),
					InfoDictionaryKey = "ModDate",
					XmpPropertyKey = "ModifyDate",
					XmpPropertyNameSpace = "http://ns.adobe.com/xap/1.0/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.DateTimeOffset
				};
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000948 RID: 2376 RVA: 0x0002E256 File Offset: 0x0002C456
		public static PdfDocumentMetadata.MetaPropertyInfo<string> Trapped
		{
			get
			{
				return new PdfDocumentMetadata.MetaPropertyInfo<string>
				{
					DocumentTag = new DocumentTags?(DocumentTags.Trapped),
					InfoDictionaryKey = "Trapped",
					XmpPropertyKey = null,
					XmpPropertyNameSpace = null,
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.None
				};
			}
		}

		// Token: 0x0400046C RID: 1132
		private static IReadOnlyList<PdfDocumentMetadata.MetaPropertyInfo> metaProperties;
	}
}
