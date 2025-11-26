using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Utils;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x0200014E RID: 334
	public class MarkedContentModel : IEquatable<MarkedContentModel>
	{
		// Token: 0x06001410 RID: 5136 RVA: 0x0004FFAF File Offset: 0x0004E1AF
		public MarkedContentModel(string tag, Dictionary<string, PdfTypeBase> parameters, PropertyListTypes paramType, bool hasMCID)
		{
			this.Tag = tag;
			this.Parameters = parameters;
			this.ParamType = paramType;
			this.HasMCID = hasMCID;
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06001411 RID: 5137 RVA: 0x0004FFD4 File Offset: 0x0004E1D4
		public string Tag { get; }

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x06001412 RID: 5138 RVA: 0x0004FFDC File Offset: 0x0004E1DC
		public Dictionary<string, PdfTypeBase> Parameters { get; }

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06001413 RID: 5139 RVA: 0x0004FFE4 File Offset: 0x0004E1E4
		public PropertyListTypes ParamType { get; }

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06001414 RID: 5140 RVA: 0x0004FFEC File Offset: 0x0004E1EC
		public bool HasMCID { get; }

		// Token: 0x06001415 RID: 5141 RVA: 0x0004FFF4 File Offset: 0x0004E1F4
		public PdfMarkedContent ToMarkedContent()
		{
			PdfTypeDictionary pdfTypeDictionary = PdfTypeDictionary.Create();
			if (this.Parameters != null && this.Parameters.Count > 0)
			{
				foreach (KeyValuePair<string, PdfTypeBase> keyValuePair in this.Parameters)
				{
					pdfTypeDictionary[keyValuePair.Key] = keyValuePair.Value.DeepClone<PdfTypeBase>();
				}
			}
			return new PdfMarkedContent(this.Tag, this.HasMCID, this.ParamType, pdfTypeDictionary);
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x00050090 File Offset: 0x0004E290
		public static MarkedContentModel Create(PdfMarkedContent markedContent)
		{
			if (markedContent == null)
			{
				return null;
			}
			PdfTypeDictionary parameters = markedContent.Parameters;
			Dictionary<string, PdfTypeBase> dictionary;
			if (parameters == null)
			{
				dictionary = null;
			}
			else
			{
				dictionary = parameters.ToDictionary((KeyValuePair<string, PdfTypeBase> c) => c.Key, (KeyValuePair<string, PdfTypeBase> c) => c.Value.DeepClone<PdfTypeBase>());
			}
			Dictionary<string, PdfTypeBase> dictionary2 = dictionary;
			return new MarkedContentModel(markedContent.Tag, dictionary2, markedContent.ParamType, markedContent.HasMCID);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0005010C File Offset: 0x0004E30C
		public bool Equals(MarkedContentModel other)
		{
			if (other == null || !(this.Tag == other.Tag) || this.ParamType != other.ParamType || this.HasMCID != other.HasMCID)
			{
				return false;
			}
			if (this.Parameters == null && other.Parameters == null)
			{
				return true;
			}
			if (this.Parameters == null || other.Parameters == null)
			{
				return false;
			}
			if (this.Parameters.Count != other.Parameters.Count)
			{
				return false;
			}
			return this.Parameters.Select((KeyValuePair<string, PdfTypeBase> c) => new global::System.ValueTuple<string, PdfTypeBase>(c.Key, c.Value)).SequenceEqual(other.Parameters.Select((KeyValuePair<string, PdfTypeBase> c) => new global::System.ValueTuple<string, PdfTypeBase>(c.Key, c.Value)));
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x000501EC File Offset: 0x0004E3EC
		public override bool Equals(object obj)
		{
			MarkedContentModel markedContentModel = obj as MarkedContentModel;
			return markedContentModel != null && this.Equals(markedContentModel);
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x0005020C File Offset: 0x0004E40C
		public override int GetHashCode()
		{
			return HashCode.Combine<string, Dictionary<string, PdfTypeBase>, PropertyListTypes, bool>(this.Tag, this.Parameters, this.ParamType, this.HasMCID);
		}
	}
}
