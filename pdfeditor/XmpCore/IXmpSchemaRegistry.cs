using System;
using System.Collections.Generic;

namespace XmpCore
{
	// Token: 0x0200001C RID: 28
	public interface IXmpSchemaRegistry
	{
		// Token: 0x060000BC RID: 188
		string RegisterNamespace(string namespaceUri, string suggestedPrefix);

		// Token: 0x060000BD RID: 189
		string GetNamespacePrefix(string namespaceUri);

		// Token: 0x060000BE RID: 190
		string GetNamespaceUri(string namespacePrefix);

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000BF RID: 191
		IDictionary<string, string> Namespaces { get; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000C0 RID: 192
		IDictionary<string, string> Prefixes { get; }

		// Token: 0x060000C1 RID: 193
		void DeleteNamespace(string namespaceUri);

		// Token: 0x060000C2 RID: 194
		IXmpAliasInfo ResolveAlias(string aliasNs, string aliasProp);

		// Token: 0x060000C3 RID: 195
		IEnumerable<IXmpAliasInfo> FindAliases(string aliasNs);

		// Token: 0x060000C4 RID: 196
		IXmpAliasInfo FindAlias(string qname);

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000C5 RID: 197
		IDictionary<string, IXmpAliasInfo> Aliases { get; }
	}
}
