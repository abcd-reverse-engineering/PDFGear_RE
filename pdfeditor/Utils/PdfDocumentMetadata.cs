using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;
using XmpCore;
using XmpCore.Options;

namespace pdfeditor.Utils
{
	// Token: 0x0200008E RID: 142
	public class PdfDocumentMetadata
	{
		// Token: 0x06000949 RID: 2377 RVA: 0x0002E289 File Offset: 0x0002C489
		public PdfDocumentMetadata(PdfDocument document)
		{
			this.document = document;
			this.ThrowIfDisposed();
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x0002E2A0 File Offset: 0x0002C4A0
		// (set) Token: 0x0600094B RID: 2379 RVA: 0x0002E344 File Offset: 0x0002C544
		public int PdfFileVersion
		{
			get
			{
				PdfTypeDictionary root = this.document.Root;
				if (root.ContainsKey("Version") && root["Version"].Is<PdfTypeName>())
				{
					string value = root["Version"].As<PdfTypeName>(true).Value;
					int num = 0;
					for (int i = 0; i < value.Length; i++)
					{
						char c = value[i];
						if (c != '.')
						{
							if (c < '0' || c > '9')
							{
								num = 0;
								break;
							}
							num = num * 10 + (int)(value[i] - '0');
						}
					}
					if (num > 0)
					{
						return num;
					}
				}
				return this.document.Version;
			}
			set
			{
				if (value >= 10 && value < 100)
				{
					PdfTypeDictionary root = this.document.Root;
					string text = string.Format("{0}.{1}", value / 10, value % 10);
					root["Version"] = PdfTypeName.Create(text);
				}
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x0002E394 File Offset: 0x0002C594
		// (set) Token: 0x0600094D RID: 2381 RVA: 0x0002E3B3 File Offset: 0x0002C5B3
		public string Title
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Title, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Title, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x0600094E RID: 2382 RVA: 0x0002E3C8 File Offset: 0x0002C5C8
		// (set) Token: 0x0600094F RID: 2383 RVA: 0x0002E3E7 File Offset: 0x0002C5E7
		public string Description
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Description, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Description, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000950 RID: 2384 RVA: 0x0002E3FC File Offset: 0x0002C5FC
		// (set) Token: 0x06000951 RID: 2385 RVA: 0x0002E41B File Offset: 0x0002C61B
		public string[] Author
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string[]>(PdfDocumentMetadataConstants.Author, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Author, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000952 RID: 2386 RVA: 0x0002E430 File Offset: 0x0002C630
		// (set) Token: 0x06000953 RID: 2387 RVA: 0x0002E44F File Offset: 0x0002C64F
		public string Subject
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Subject, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Subject, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000954 RID: 2388 RVA: 0x0002E464 File Offset: 0x0002C664
		// (set) Token: 0x06000955 RID: 2389 RVA: 0x0002E484 File Offset: 0x0002C684
		public string Keywords
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Keywords, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				string text = "";
				string[] array = Array.Empty<string>();
				if (!string.IsNullOrEmpty(value))
				{
					text = value.Replace("\r", "").Replace("\n", " ").Trim();
					string[] array2 = text.Split(new char[] { ',', ';', '，', '；' });
					if (array2.Length != 0)
					{
						array = (from c in array2
							select c.Trim() into c
							where !string.IsNullOrWhiteSpace(c)
							select c).ToArray<string>();
					}
				}
				if (this.SetMetaValue(PdfDocumentMetadataConstants.Keywords, CultureInfo.InvariantCulture, text))
				{
					PdfDocumentMetadata.MetaPropertyInfo<string[]> metaPropertyInfo = new PdfDocumentMetadata.MetaPropertyInfo<string[]>
					{
						XmpPropertyKey = "subject",
						XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
						XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.ArrayAlternate
					};
					this.SetMetaValue(metaPropertyInfo, CultureInfo.InvariantCulture, array);
				}
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x0002E57C File Offset: 0x0002C77C
		public string[] XmpSubjects
		{
			get
			{
				PdfDocumentMetadata.MetaPropertyInfo<string[]> metaPropertyInfo = new PdfDocumentMetadata.MetaPropertyInfo<string[]>
				{
					XmpPropertyKey = "subject",
					XmpPropertyNameSpace = "http://purl.org/dc/elements/1.1/",
					XmpPropertyType = PdfDocumentMetadata.XmpPropertyType.ArrayAlternate
				};
				bool flag;
				return this.GetMetaValue<string[]>(metaPropertyInfo, CultureInfo.CurrentUICulture, out flag) ?? Array.Empty<string>();
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x0002E5C4 File Offset: 0x0002C7C4
		// (set) Token: 0x06000958 RID: 2392 RVA: 0x0002E5E3 File Offset: 0x0002C7E3
		public string Creator
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Creator, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Creator, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x0002E5F8 File Offset: 0x0002C7F8
		// (set) Token: 0x0600095A RID: 2394 RVA: 0x0002E617 File Offset: 0x0002C817
		public string Producer
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Producer, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Producer, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600095B RID: 2395 RVA: 0x0002E62C File Offset: 0x0002C82C
		// (set) Token: 0x0600095C RID: 2396 RVA: 0x0002E64B File Offset: 0x0002C84B
		public DateTimeOffset CreationDate
		{
			get
			{
				bool flag;
				return this.GetMetaValue<DateTimeOffset>(PdfDocumentMetadataConstants.CreationDate, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.CreationDate, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x0002E664 File Offset: 0x0002C864
		// (set) Token: 0x0600095E RID: 2398 RVA: 0x0002E683 File Offset: 0x0002C883
		public DateTimeOffset ModificationDate
		{
			get
			{
				bool flag;
				return this.GetMetaValue<DateTimeOffset>(PdfDocumentMetadataConstants.ModificationDate, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.ModificationDate, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0002E69C File Offset: 0x0002C89C
		// (set) Token: 0x06000960 RID: 2400 RVA: 0x0002E6BB File Offset: 0x0002C8BB
		public string Trapped
		{
			get
			{
				bool flag;
				return this.GetMetaValue<string>(PdfDocumentMetadataConstants.Trapped, CultureInfo.CurrentUICulture, out flag);
			}
			set
			{
				this.SetMetaValue(PdfDocumentMetadataConstants.Trapped, CultureInfo.InvariantCulture, value);
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0002E6D0 File Offset: 0x0002C8D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetMetaValue<T>(PdfDocumentMetadata.MetaPropertyInfo<T> propertyInfo, CultureInfo cultureInfo, out bool isXmpProperty)
		{
			object metaValueCore = this.GetMetaValueCore(propertyInfo, cultureInfo, out isXmpProperty);
			if (metaValueCore == null)
			{
				return default(T);
			}
			return (T)((object)metaValueCore);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0002E6FA File Offset: 0x0002C8FA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object GetMetaValue(PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, out bool isXmpProperty)
		{
			return this.GetMetaValueCore(propertyInfo, cultureInfo, out isXmpProperty);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0002E705 File Offset: 0x0002C905
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetMetaValue(PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, object value)
		{
			return this.SetMetaValueCore(propertyInfo, cultureInfo, value);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0002E710 File Offset: 0x0002C910
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetMetaText<T>(PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, T value)
		{
			return this.SetMetaValueCore(propertyInfo, cultureInfo, value);
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0002E720 File Offset: 0x0002C920
		public bool RemoveMetaValue(PdfDocumentMetadata.MetaPropertyInfo propertyInfo)
		{
			this.ThrowIfDisposed();
			if (propertyInfo == null)
			{
				return false;
			}
			IXmpMeta xmpMeta = this.GetXmpMeta();
			if (xmpMeta == null)
			{
				xmpMeta = XmpMetaFactory.Create();
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(propertyInfo.XmpPropertyKey) && propertyInfo.XmpPropertyType != PdfDocumentMetadata.XmpPropertyType.None && xmpMeta.GetProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey) != null)
			{
				xmpMeta.DeleteProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
				if (!this.SetXmpMeta(xmpMeta))
				{
					return false;
				}
				flag = true;
			}
			if (propertyInfo.DocumentTag != null)
			{
				Pdfium.FPDF_SetMetaText(this.document.Handle, propertyInfo.DocumentTag.Value, null);
			}
			return flag;
		}

		// Token: 0x06000966 RID: 2406
		[DllImport("pdfium", CharSet = CharSet.Unicode, EntryPoint = "FPDF_GetMetaText", SetLastError = true)]
		private static extern int FPDF_GetMetaText_native(IntPtr document, [MarshalAs(UnmanagedType.LPStr)] string tag, [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int buflen);

		// Token: 0x06000967 RID: 2407 RVA: 0x0002E7CC File Offset: 0x0002C9CC
		private object GetMetaValueCore(PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, out bool isXmpProperty)
		{
			this.ThrowIfDisposed();
			isXmpProperty = false;
			IXmpMeta xmpMeta = this.GetXmpMeta();
			object xmpValue = this.GetXmpValue(xmpMeta, propertyInfo, cultureInfo);
			if (xmpValue != null)
			{
				isXmpProperty = true;
				return xmpValue;
			}
			string text = Pdfium.FPDF_GetMetaText(this.document.Handle, propertyInfo.InfoDictionaryKey);
			switch (propertyInfo.XmpPropertyType)
			{
			case PdfDocumentMetadata.XmpPropertyType.String:
			case PdfDocumentMetadata.XmpPropertyType.LocalizedString:
				return text ?? string.Empty;
			case PdfDocumentMetadata.XmpPropertyType.ArrayOrdered:
			case PdfDocumentMetadata.XmpPropertyType.ArrayAlternate:
				if (text == null)
				{
					return Array.Empty<string>();
				}
				return new string[] { text };
			case PdfDocumentMetadata.XmpPropertyType.DateTimeOffset:
			{
				DateTimeOffset dateTimeOffset;
				if (PdfObjectExtensions.TryParseModificationDate(text, out dateTimeOffset))
				{
					return dateTimeOffset;
				}
				return null;
			}
			}
			return null;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0002E870 File Offset: 0x0002CA70
		private bool SetMetaValueCore(PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, object value)
		{
			this.ThrowIfDisposed();
			if (propertyInfo == null)
			{
				return false;
			}
			IXmpMeta xmpMeta = this.GetXmpMeta();
			if (xmpMeta == null)
			{
				xmpMeta = XmpMetaFactory.Create();
			}
			if (!string.IsNullOrEmpty(propertyInfo.XmpPropertyKey) && propertyInfo.XmpPropertyType != PdfDocumentMetadata.XmpPropertyType.None)
			{
				if (!this.SetXmpValue(xmpMeta, propertyInfo, cultureInfo, value, string.IsNullOrEmpty((cultureInfo != null) ? cultureInfo.Name : null)))
				{
					return false;
				}
				if (!this.SetXmpMeta(xmpMeta))
				{
					return false;
				}
			}
			if (propertyInfo.DocumentTag != null)
			{
				string text = "";
				if (value != null)
				{
					switch (propertyInfo.XmpPropertyType)
					{
					case PdfDocumentMetadata.XmpPropertyType.ArrayOrdered:
					case PdfDocumentMetadata.XmpPropertyType.ArrayAlternate:
					{
						string[] array = (string[])value;
						text = ((array != null) ? array.FirstOrDefault<string>() : null) ?? "";
						goto IL_00D6;
					}
					case PdfDocumentMetadata.XmpPropertyType.DateTimeOffset:
						text = ((DateTimeOffset)value).ToModificationDateString();
						goto IL_00D6;
					}
					text = value.ToString();
				}
				IL_00D6:
				Pdfium.FPDF_SetMetaText(this.document.Handle, propertyInfo.DocumentTag.Value, text);
			}
			return true;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0002E974 File Offset: 0x0002CB74
		private bool SetXmpValue(IXmpMeta xmpMeta, PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo, object value, bool removeOtherLanguageValues)
		{
			if (xmpMeta == null || propertyInfo == null)
			{
				return false;
			}
			if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.None)
			{
				return false;
			}
			if ((propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.String || propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.LocalizedString) && !(value is string))
			{
				return false;
			}
			if ((propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayOrdered || propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayAlternate) && !(value is string[]))
			{
				return false;
			}
			if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.DateTimeOffset && !(value is DateTimeOffset))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(propertyInfo.XmpPropertyKey))
			{
				try
				{
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.String)
					{
						xmpMeta.SetProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, value);
						return true;
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.LocalizedString)
					{
						if (removeOtherLanguageValues)
						{
							xmpMeta.DeleteProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
						}
						string text;
						string text2;
						this.GetLanguages(cultureInfo, out text, out text2);
						xmpMeta.SetLocalizedText(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, text, text2, (string)value);
						return true;
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayOrdered || propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayAlternate)
					{
						xmpMeta.DeleteProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
						string[] array = (string[])value;
						xmpMeta.SetProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, null, new PropertyOptions
						{
							IsArray = true,
							IsArrayAlternate = (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayAlternate),
							IsArrayOrdered = (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayOrdered)
						});
						for (int i = 0; i < array.Length; i++)
						{
							xmpMeta.InsertArrayItem(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, i + 1, array[i]);
						}
						return true;
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.DateTimeOffset)
					{
						string text3 = PdfDocumentMetadata.FormatISO8601((DateTimeOffset)value);
						xmpMeta.SetProperty(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, text3);
						return true;
					}
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0002EB58 File Offset: 0x0002CD58
		private object GetXmpValue(IXmpMeta xmpMeta, PdfDocumentMetadata.MetaPropertyInfo propertyInfo, CultureInfo cultureInfo)
		{
			if (xmpMeta == null || propertyInfo == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(propertyInfo.XmpPropertyKey))
			{
				try
				{
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.LocalizedString)
					{
						string text;
						string text2;
						this.GetLanguages(cultureInfo, out text, out text2);
						IXmpProperty localizedText = xmpMeta.GetLocalizedText(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, text, text2);
						return (localizedText != null) ? localizedText.Value : null;
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.String)
					{
						return xmpMeta.GetPropertyString(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayOrdered || propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.ArrayAlternate)
					{
						int num = xmpMeta.CountArrayItems(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
						List<string> list = new List<string>(num);
						for (int i = 1; i <= num; i++)
						{
							IXmpProperty arrayItem = xmpMeta.GetArrayItem(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey, i);
							string text3 = ((arrayItem != null) ? arrayItem.Value : null);
							if (!string.IsNullOrEmpty(text3))
							{
								list.Add(text3);
							}
						}
						return (list.Count == 0) ? null : list.ToArray();
					}
					if (propertyInfo.XmpPropertyType == PdfDocumentMetadata.XmpPropertyType.DateTimeOffset)
					{
						string propertyString = xmpMeta.GetPropertyString(propertyInfo.XmpPropertyNameSpace, propertyInfo.XmpPropertyKey);
						DateTimeOffset dateTimeOffset;
						if (!string.IsNullOrEmpty(propertyString) && PdfDocumentMetadata.TryParseISO8601(propertyString, out dateTimeOffset))
						{
							return dateTimeOffset;
						}
					}
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0002ECC8 File Offset: 0x0002CEC8
		private void GetLanguages(CultureInfo cultureInfo, out string genericLang, out string specificLang)
		{
			genericLang = string.Empty;
			specificLang = string.Empty;
			while (cultureInfo != null && !(cultureInfo.Name == string.Empty))
			{
				if (string.IsNullOrEmpty(specificLang))
				{
					specificLang = cultureInfo.Name;
				}
				if (cultureInfo.IsNeutralCulture)
				{
					genericLang = cultureInfo.Name;
				}
				cultureInfo = cultureInfo.Parent;
			}
			if (string.IsNullOrEmpty(specificLang))
			{
				specificLang = "x-default";
				genericLang = "x-default";
			}
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0002ED3C File Offset: 0x0002CF3C
		private IXmpMeta GetXmpMeta()
		{
			this.ThrowIfDisposed();
			try
			{
				if (this.document.Root.ContainsKey("Metadata") && this.document.Root["Metadata"].Is<PdfTypeStream>())
				{
					PdfTypeStream pdfTypeStream = this.document.Root["Metadata"].As<PdfTypeStream>(true);
					if (pdfTypeStream.Dictionary.ContainsKey("Type") && pdfTypeStream.Dictionary["Type"].Is<PdfTypeName>() && pdfTypeStream.Dictionary["Type"].As<PdfTypeName>(true).Value == "Metadata" && pdfTypeStream.Dictionary.ContainsKey("Subtype") && pdfTypeStream.Dictionary["Subtype"].Is<PdfTypeName>() && pdfTypeStream.Dictionary["Subtype"].As<PdfTypeName>(true).Value == "XML")
					{
						return XmpMetaFactory.ParseFromString(PdfDocumentMetadata.GetDecodedText(pdfTypeStream), null);
					}
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0002EE80 File Offset: 0x0002D080
		private bool SetXmpMeta(IXmpMeta xmpMeta)
		{
			this.ThrowIfDisposed();
			try
			{
				PdfTypeStream pdfTypeStream = null;
				if (this.document.Root.ContainsKey("Metadata") && this.document.Root["Metadata"].Is<PdfTypeStream>())
				{
					pdfTypeStream = this.document.Root["Metadata"].As<PdfTypeStream>(true);
				}
				if (pdfTypeStream == null)
				{
					pdfTypeStream = PdfTypeStream.Create();
					pdfTypeStream.InitEmpty();
					pdfTypeStream.Dictionary["Type"] = PdfTypeName.Create("Metadata");
					pdfTypeStream.Dictionary["Subtype"] = PdfTypeName.Create("XML");
					PdfIndirectList pdfIndirectList = PdfIndirectList.FromPdfDocument(this.document);
					int num = pdfIndirectList.Add(pdfTypeStream);
					this.document.Root.SetIndirectAt("Metadata", pdfIndirectList, num);
				}
				byte[] array = XmpMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions
				{
					Indent = "",
					UseCanonicalFormat = true,
					EncodeUtf16Be = false,
					EncodeUtf16Le = false,
					EncodeUtf8WithBom = false
				});
				pdfTypeStream.SetContent(array, false);
				return true;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0002EFAC File Offset: 0x0002D1AC
		private static string GetDecodedText(PdfTypeStream stream)
		{
			byte[] decodedData = stream.DecodedData;
			Span<byte> span = decodedData.AsSpan<byte>();
			byte[] preamble = Encoding.UTF8.GetPreamble();
			byte[] preamble2 = Encoding.Unicode.GetPreamble();
			byte[] preamble3 = Encoding.BigEndianUnicode.GetPreamble();
			byte[] preamble4 = Encoding.UTF32.GetPreamble();
			if (span.Slice(0, Math.Min(30, decodedData.Length)).IndexOf(preamble) != -1)
			{
				return Encoding.UTF8.GetString(decodedData);
			}
			if (span.Slice(0, Math.Min(60, decodedData.Length)).IndexOf(preamble2) != -1)
			{
				return Encoding.Unicode.GetString(decodedData);
			}
			if (span.Slice(0, Math.Min(60, decodedData.Length)).IndexOf(preamble3) != -1)
			{
				return Encoding.BigEndianUnicode.GetString(decodedData);
			}
			if (span.Slice(0, Math.Min(120, decodedData.Length)).IndexOf(preamble4) != -1)
			{
				return Encoding.UTF32.GetString(decodedData);
			}
			return Encoding.UTF8.GetString(decodedData);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0002F0B2 File Offset: 0x0002D2B2
		private void ThrowIfDisposed()
		{
			if (this.document.IsDisposed)
			{
				throw new ObjectDisposedException("document");
			}
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0002F0CC File Offset: 0x0002D2CC
		private static string FormatISO8601(DateTimeOffset dateTimeOffset)
		{
			string text = ((dateTimeOffset.Offset == TimeSpan.Zero) ? "yyyy-MM-ddTHH:mm:ss.fffZ" : "yyyy-MM-ddTHH:mm:ss.fffzzz");
			return dateTimeOffset.ToString(text, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0002F106 File Offset: 0x0002D306
		private static bool TryParseISO8601(string iso8601String, out DateTimeOffset result)
		{
			return DateTimeOffset.TryParseExact(iso8601String, new string[] { "yyyy-MM-dd'T'HH:mm:ss.FFFK" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
		}

		// Token: 0x0400046D RID: 1133
		private readonly PdfDocument document;

		// Token: 0x0200045B RID: 1115
		public class MetaPropertyInfo<T> : PdfDocumentMetadata.MetaPropertyInfo, IEquatable<PdfDocumentMetadata.MetaPropertyInfo<T>>
		{
			// Token: 0x06002D6C RID: 11628 RVA: 0x000DE5D9 File Offset: 0x000DC7D9
			public bool Equals(PdfDocumentMetadata.MetaPropertyInfo<T> other)
			{
				return base.Equals(other);
			}
		}

		// Token: 0x0200045C RID: 1116
		public class MetaPropertyInfo : IEquatable<PdfDocumentMetadata.MetaPropertyInfo>
		{
			// Token: 0x17000C9C RID: 3228
			// (get) Token: 0x06002D6E RID: 11630 RVA: 0x000DE5EA File Offset: 0x000DC7EA
			// (set) Token: 0x06002D6F RID: 11631 RVA: 0x000DE5F2 File Offset: 0x000DC7F2
			public DocumentTags? DocumentTag { get; set; }

			// Token: 0x17000C9D RID: 3229
			// (get) Token: 0x06002D70 RID: 11632 RVA: 0x000DE5FB File Offset: 0x000DC7FB
			// (set) Token: 0x06002D71 RID: 11633 RVA: 0x000DE603 File Offset: 0x000DC803
			public string InfoDictionaryKey { get; set; }

			// Token: 0x17000C9E RID: 3230
			// (get) Token: 0x06002D72 RID: 11634 RVA: 0x000DE60C File Offset: 0x000DC80C
			// (set) Token: 0x06002D73 RID: 11635 RVA: 0x000DE614 File Offset: 0x000DC814
			public string XmpPropertyKey { get; set; }

			// Token: 0x17000C9F RID: 3231
			// (get) Token: 0x06002D74 RID: 11636 RVA: 0x000DE61D File Offset: 0x000DC81D
			// (set) Token: 0x06002D75 RID: 11637 RVA: 0x000DE625 File Offset: 0x000DC825
			public string XmpPropertyNameSpace { get; set; }

			// Token: 0x17000CA0 RID: 3232
			// (get) Token: 0x06002D76 RID: 11638 RVA: 0x000DE62E File Offset: 0x000DC82E
			// (set) Token: 0x06002D77 RID: 11639 RVA: 0x000DE636 File Offset: 0x000DC836
			public PdfDocumentMetadata.XmpPropertyType XmpPropertyType { get; set; }

			// Token: 0x06002D78 RID: 11640 RVA: 0x000DE640 File Offset: 0x000DC840
			public bool Equals(PdfDocumentMetadata.MetaPropertyInfo other)
			{
				if (other != null)
				{
					DocumentTags? documentTag = this.DocumentTag;
					DocumentTags? documentTag2 = other.DocumentTag;
					if (((documentTag.GetValueOrDefault() == documentTag2.GetValueOrDefault()) & (documentTag != null == (documentTag2 != null))) && this.InfoDictionaryKey == other.InfoDictionaryKey && this.XmpPropertyKey == other.XmpPropertyKey && this.XmpPropertyNameSpace == other.XmpPropertyNameSpace)
					{
						return this.XmpPropertyType == other.XmpPropertyType;
					}
				}
				return false;
			}

			// Token: 0x06002D79 RID: 11641 RVA: 0x000DE6D0 File Offset: 0x000DC8D0
			public override bool Equals(object obj)
			{
				PdfDocumentMetadata.MetaPropertyInfo metaPropertyInfo = obj as PdfDocumentMetadata.MetaPropertyInfo;
				return metaPropertyInfo != null && this.Equals(metaPropertyInfo);
			}

			// Token: 0x06002D7A RID: 11642 RVA: 0x000DE6F0 File Offset: 0x000DC8F0
			public override int GetHashCode()
			{
				return HashCode.Combine<DocumentTags?, string, string, string, PdfDocumentMetadata.XmpPropertyType>(this.DocumentTag, this.InfoDictionaryKey, this.XmpPropertyKey, this.XmpPropertyNameSpace, this.XmpPropertyType);
			}

			// Token: 0x06002D7B RID: 11643 RVA: 0x000DE715 File Offset: 0x000DC915
			public static bool operator ==(PdfDocumentMetadata.MetaPropertyInfo left, PdfDocumentMetadata.MetaPropertyInfo right)
			{
				return (left == null && right == null) || (left != null && right != null && left.Equals(right));
			}

			// Token: 0x06002D7C RID: 11644 RVA: 0x000DE72E File Offset: 0x000DC92E
			public static bool operator !=(PdfDocumentMetadata.MetaPropertyInfo left, PdfDocumentMetadata.MetaPropertyInfo right)
			{
				return !(left == right);
			}
		}

		// Token: 0x0200045D RID: 1117
		public enum XmpPropertyType
		{
			// Token: 0x040018FE RID: 6398
			None,
			// Token: 0x040018FF RID: 6399
			String,
			// Token: 0x04001900 RID: 6400
			LocalizedString,
			// Token: 0x04001901 RID: 6401
			ArrayOrdered,
			// Token: 0x04001902 RID: 6402
			ArrayAlternate,
			// Token: 0x04001903 RID: 6403
			DateTimeOffset
		}
	}
}
