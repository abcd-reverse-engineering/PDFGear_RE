using System;
using System.Collections.Generic;
using Sharpen;
using XmpCore.Options;

namespace XmpCore
{
	// Token: 0x02000019 RID: 25
	public interface IXmpMeta
	{
		// Token: 0x0600007D RID: 125
		IXmpMeta Clone();

		// Token: 0x0600007E RID: 126
		IXmpProperty GetProperty(string schemaNs, string propName);

		// Token: 0x0600007F RID: 127
		IXmpProperty GetArrayItem(string schemaNs, string arrayName, int itemIndex);

		// Token: 0x06000080 RID: 128
		int CountArrayItems(string schemaNs, string arrayName);

		// Token: 0x06000081 RID: 129
		IXmpProperty GetStructField(string schemaNs, string structName, string fieldNs, string fieldName);

		// Token: 0x06000082 RID: 130
		IXmpProperty GetQualifier(string schemaNs, string propName, string qualNs, string qualName);

		// Token: 0x06000083 RID: 131
		void SetProperty(string schemaNs, string propName, object propValue, PropertyOptions options);

		// Token: 0x06000084 RID: 132
		void SetProperty(string schemaNs, string propName, object propValue);

		// Token: 0x06000085 RID: 133
		void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options);

		// Token: 0x06000086 RID: 134
		void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue);

		// Token: 0x06000087 RID: 135
		void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options);

		// Token: 0x06000088 RID: 136
		void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue);

		// Token: 0x06000089 RID: 137
		void AppendArrayItem(string schemaNs, string arrayName, PropertyOptions arrayOptions, string itemValue, PropertyOptions itemOptions);

		// Token: 0x0600008A RID: 138
		void AppendArrayItem(string schemaNs, string arrayName, string itemValue);

		// Token: 0x0600008B RID: 139
		void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue, PropertyOptions options);

		// Token: 0x0600008C RID: 140
		void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue);

		// Token: 0x0600008D RID: 141
		void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue, PropertyOptions options);

		// Token: 0x0600008E RID: 142
		void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue);

		// Token: 0x0600008F RID: 143
		void DeleteProperty(string schemaNs, string propName);

		// Token: 0x06000090 RID: 144
		void DeleteArrayItem(string schemaNs, string arrayName, int itemIndex);

		// Token: 0x06000091 RID: 145
		void DeleteStructField(string schemaNs, string structName, string fieldNs, string fieldName);

		// Token: 0x06000092 RID: 146
		void DeleteQualifier(string schemaNs, string propName, string qualNs, string qualName);

		// Token: 0x06000093 RID: 147
		bool DoesPropertyExist(string schemaNs, string propName);

		// Token: 0x06000094 RID: 148
		bool DoesArrayItemExist(string schemaNs, string arrayName, int itemIndex);

		// Token: 0x06000095 RID: 149
		bool DoesStructFieldExist(string schemaNs, string structName, string fieldNs, string fieldName);

		// Token: 0x06000096 RID: 150
		bool DoesQualifierExist(string schemaNs, string propName, string qualNs, string qualName);

		// Token: 0x06000097 RID: 151
		IXmpProperty GetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang);

		// Token: 0x06000098 RID: 152
		void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue, PropertyOptions options);

		// Token: 0x06000099 RID: 153
		void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue);

		// Token: 0x0600009A RID: 154
		bool GetPropertyBoolean(string schemaNs, string propName);

		// Token: 0x0600009B RID: 155
		int GetPropertyInteger(string schemaNs, string propName);

		// Token: 0x0600009C RID: 156
		long GetPropertyLong(string schemaNs, string propName);

		// Token: 0x0600009D RID: 157
		double GetPropertyDouble(string schemaNs, string propName);

		// Token: 0x0600009E RID: 158
		IXmpDateTime GetPropertyDate(string schemaNs, string propName);

		// Token: 0x0600009F RID: 159
		Calendar GetPropertyCalendar(string schemaNs, string propName);

		// Token: 0x060000A0 RID: 160
		byte[] GetPropertyBase64(string schemaNs, string propName);

		// Token: 0x060000A1 RID: 161
		string GetPropertyString(string schemaNs, string propName);

		// Token: 0x060000A2 RID: 162
		void SetPropertyBoolean(string schemaNs, string propName, bool propValue, PropertyOptions options);

		// Token: 0x060000A3 RID: 163
		void SetPropertyBoolean(string schemaNs, string propName, bool propValue);

		// Token: 0x060000A4 RID: 164
		void SetPropertyInteger(string schemaNs, string propName, int propValue, PropertyOptions options);

		// Token: 0x060000A5 RID: 165
		void SetPropertyInteger(string schemaNs, string propName, int propValue);

		// Token: 0x060000A6 RID: 166
		void SetPropertyLong(string schemaNs, string propName, long propValue, PropertyOptions options);

		// Token: 0x060000A7 RID: 167
		void SetPropertyLong(string schemaNs, string propName, long propValue);

		// Token: 0x060000A8 RID: 168
		void SetPropertyDouble(string schemaNs, string propName, double propValue, PropertyOptions options);

		// Token: 0x060000A9 RID: 169
		void SetPropertyDouble(string schemaNs, string propName, double propValue);

		// Token: 0x060000AA RID: 170
		void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue, PropertyOptions options);

		// Token: 0x060000AB RID: 171
		void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue);

		// Token: 0x060000AC RID: 172
		void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue, PropertyOptions options);

		// Token: 0x060000AD RID: 173
		void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue);

		// Token: 0x060000AE RID: 174
		void SetPropertyBase64(string schemaNs, string propName, byte[] propValue, PropertyOptions options);

		// Token: 0x060000AF RID: 175
		void SetPropertyBase64(string schemaNs, string propName, byte[] propValue);

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B0 RID: 176
		IEnumerable<IXmpPropertyInfo> Properties { get; }

		// Token: 0x060000B1 RID: 177
		string GetObjectName();

		// Token: 0x060000B2 RID: 178
		void SetObjectName(string name);

		// Token: 0x060000B3 RID: 179
		string GetPacketHeader();

		// Token: 0x060000B4 RID: 180
		void Sort();

		// Token: 0x060000B5 RID: 181
		void Normalize(ParseOptions options);

		// Token: 0x060000B6 RID: 182
		string DumpObject();
	}
}
