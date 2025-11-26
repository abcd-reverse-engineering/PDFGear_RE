using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpen;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003A RID: 58
	public sealed class XmpNode : IComparable
	{
		// Token: 0x0600026F RID: 623 RVA: 0x00008CD4 File Offset: 0x00006ED4
		public XmpNode(string name, string value, PropertyOptions options)
		{
			this.Name = name;
			this.Value = value;
			this._options = options;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00008CF1 File Offset: 0x00006EF1
		public XmpNode(string name, PropertyOptions options)
			: this(name, null, options)
		{
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00008CFC File Offset: 0x00006EFC
		public void Clear()
		{
			this._options = null;
			this.Name = null;
			this.Value = null;
			this._children = null;
			this._childrenLookup = null;
			this._qualifier = null;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000272 RID: 626 RVA: 0x00008D28 File Offset: 0x00006F28
		// (set) Token: 0x06000273 RID: 627 RVA: 0x00008D30 File Offset: 0x00006F30
		public XmpNode Parent { get; internal set; }

		// Token: 0x06000274 RID: 628 RVA: 0x00008D39 File Offset: 0x00006F39
		public XmpNode GetChild(int index)
		{
			return this.GetChildren()[index - 1];
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00008D4C File Offset: 0x00006F4C
		public void AddChild(XmpNode node)
		{
			this.AssertChildNotExisting(node.Name);
			node.Parent = this;
			this.GetChildren().Add(node);
			if (this._childrenLookup != null && node.Name != "rdf:li" && node.Name != "[]")
			{
				this._childrenLookup[node.Name] = node;
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00008DB8 File Offset: 0x00006FB8
		public void AddChild(int index, XmpNode node)
		{
			this.AssertChildNotExisting(node.Name);
			node.Parent = this;
			this.GetChildren().Insert(index - 1, node);
			if (this._childrenLookup != null && node.Name != "rdf:li" && node.Name != "[]")
			{
				this._childrenLookup[node.Name] = node;
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00008E25 File Offset: 0x00007025
		public void ReplaceChild(int index, XmpNode node)
		{
			node.Parent = this;
			this.GetChildren()[index - 1] = node;
			if (this._childrenLookup != null)
			{
				this._childrenLookup[node.Name] = node;
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00008E57 File Offset: 0x00007057
		public void RemoveChild(int itemIndex)
		{
			Dictionary<string, XmpNode> childrenLookup = this._childrenLookup;
			if (childrenLookup != null)
			{
				childrenLookup.Remove(this.GetChildren()[itemIndex - 1].Name);
			}
			this.GetChildren().RemoveAt(itemIndex - 1);
			this.CleanupChildren();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00008E92 File Offset: 0x00007092
		public void RemoveChild(XmpNode node)
		{
			Dictionary<string, XmpNode> childrenLookup = this._childrenLookup;
			if (childrenLookup != null)
			{
				childrenLookup.Remove(node.Name);
			}
			this.GetChildren().Remove(node);
			this.CleanupChildren();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00008EBF File Offset: 0x000070BF
		private void CleanupChildren()
		{
			if (this._children.Count == 0)
			{
				this._children = null;
				this._childrenLookup = null;
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00008EDC File Offset: 0x000070DC
		public void RemoveChildren()
		{
			this._children = null;
			this._childrenLookup = null;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00008EEC File Offset: 0x000070EC
		public int GetChildrenLength()
		{
			List<XmpNode> children = this._children;
			if (children == null)
			{
				return 0;
			}
			return children.Count;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00008EFF File Offset: 0x000070FF
		public XmpNode FindChildByName(string expr)
		{
			return XmpNode.FindChild(this.GetChildren(), ref this._childrenLookup, expr);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00008F13 File Offset: 0x00007113
		public XmpNode GetQualifier(int index)
		{
			return this.GetQualifier()[index - 1];
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00008F23 File Offset: 0x00007123
		public int GetQualifierLength()
		{
			List<XmpNode> qualifier = this._qualifier;
			if (qualifier == null)
			{
				return 0;
			}
			return qualifier.Count;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00008F38 File Offset: 0x00007138
		public void AddQualifier(XmpNode qualNode)
		{
			this.AssertQualifierNotExisting(qualNode.Name);
			qualNode.Parent = this;
			qualNode.Options.IsQualifier = true;
			this.Options.HasQualifiers = true;
			if (qualNode.IsLanguageNode)
			{
				this._options.HasLanguage = true;
				this.GetQualifier().Insert(0, qualNode);
				return;
			}
			if (qualNode.IsTypeNode)
			{
				this._options.HasType = true;
				this.GetQualifier().Insert((this._options.HasLanguage > false) ? 1 : 0, qualNode);
				return;
			}
			this.GetQualifier().Add(qualNode);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00008FD0 File Offset: 0x000071D0
		public void RemoveQualifier(XmpNode qualNode)
		{
			PropertyOptions options = this.Options;
			if (qualNode.IsLanguageNode)
			{
				options.HasLanguage = false;
			}
			else if (qualNode.IsTypeNode)
			{
				options.HasType = false;
			}
			this.GetQualifier().Remove(qualNode);
			if (this._qualifier.Count == 0)
			{
				options.HasQualifiers = false;
				this._qualifier = null;
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000902C File Offset: 0x0000722C
		public void RemoveQualifiers()
		{
			PropertyOptions options = this.Options;
			options.HasQualifiers = false;
			options.HasLanguage = false;
			options.HasType = false;
			this._qualifier = null;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000904F File Offset: 0x0000724F
		public XmpNode FindQualifierByName(string expr)
		{
			return XmpNode.Find(this._qualifier, expr);
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000905D File Offset: 0x0000725D
		public bool HasChildren
		{
			get
			{
				return this._children != null && this._children.Count > 0;
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00009078 File Offset: 0x00007278
		public IIterator IterateChildren()
		{
			if (this._children == null)
			{
				return Enumerable.Empty<object>().Iterator<object>();
			}
			return this.GetChildren().Iterator<XmpNode>();
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000286 RID: 646 RVA: 0x000090A7 File Offset: 0x000072A7
		public bool HasQualifier
		{
			get
			{
				return this._qualifier != null && this._qualifier.Count > 0;
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000090C4 File Offset: 0x000072C4
		public IIterator IterateQualifier()
		{
			if (this._qualifier == null)
			{
				return Enumerable.Empty<object>().Iterator<object>();
			}
			return new XmpNode.Iterator391(this.GetQualifier().Iterator<XmpNode>());
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000090F8 File Offset: 0x000072F8
		public object Clone()
		{
			return this.Clone(false);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00009104 File Offset: 0x00007304
		public object Clone(bool skipEmpty)
		{
			PropertyOptions propertyOptions;
			try
			{
				propertyOptions = new PropertyOptions(this.Options.GetOptions());
			}
			catch (XmpException)
			{
				propertyOptions = new PropertyOptions();
			}
			XmpNode xmpNode = new XmpNode(this.Name, this.Value, propertyOptions);
			this.CloneSubtree(xmpNode, skipEmpty);
			if (skipEmpty && string.IsNullOrEmpty(xmpNode.Value) && !xmpNode.HasChildren)
			{
				xmpNode = null;
			}
			return xmpNode;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00009174 File Offset: 0x00007374
		public void CloneSubtree(XmpNode destination, bool skipEmpty)
		{
			try
			{
				IIterator iterator = this.IterateChildren();
				while (iterator.HasNext())
				{
					XmpNode xmpNode = (XmpNode)iterator.Next();
					if (!skipEmpty || !string.IsNullOrEmpty(xmpNode.Value) || xmpNode.HasChildren)
					{
						XmpNode xmpNode2 = (XmpNode)xmpNode.Clone(skipEmpty);
						if (xmpNode2 != null)
						{
							destination.AddChild(xmpNode2);
						}
					}
				}
				IIterator iterator2 = this.IterateQualifier();
				while (iterator2.HasNext())
				{
					XmpNode xmpNode3 = (XmpNode)iterator2.Next();
					if (!skipEmpty || !string.IsNullOrEmpty(xmpNode3.Value) || xmpNode3.HasChildren)
					{
						XmpNode xmpNode4 = (XmpNode)xmpNode3.Clone(skipEmpty);
						if (xmpNode4 != null)
						{
							destination.AddQualifier(xmpNode4);
						}
					}
				}
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00009238 File Offset: 0x00007438
		public string DumpNode(bool recursive)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			this.DumpNode(stringBuilder, recursive, 0, 0);
			return stringBuilder.ToString();
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00009260 File Offset: 0x00007460
		public int CompareTo(object xmpNode)
		{
			if (!this.Options.IsSchemaNode)
			{
				return string.CompareOrdinal(this.Name, ((XmpNode)xmpNode).Name);
			}
			return string.CompareOrdinal(this.Value, ((XmpNode)xmpNode).Value);
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600028E RID: 654 RVA: 0x000092A5 File Offset: 0x000074A5
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000929C File Offset: 0x0000749C
		public string Name { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600028F RID: 655 RVA: 0x000092AD File Offset: 0x000074AD
		// (set) Token: 0x06000290 RID: 656 RVA: 0x000092B5 File Offset: 0x000074B5
		public string Value { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000291 RID: 657 RVA: 0x000092C0 File Offset: 0x000074C0
		// (set) Token: 0x06000292 RID: 658 RVA: 0x000092E5 File Offset: 0x000074E5
		public PropertyOptions Options
		{
			get
			{
				PropertyOptions propertyOptions;
				if ((propertyOptions = this._options) == null)
				{
					propertyOptions = (this._options = new PropertyOptions());
				}
				return propertyOptions;
			}
			set
			{
				this._options = value;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000293 RID: 659 RVA: 0x000092EE File Offset: 0x000074EE
		// (set) Token: 0x06000294 RID: 660 RVA: 0x000092F6 File Offset: 0x000074F6
		public bool IsImplicit { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000295 RID: 661 RVA: 0x000092FF File Offset: 0x000074FF
		// (set) Token: 0x06000296 RID: 662 RVA: 0x00009307 File Offset: 0x00007507
		public bool HasAliases { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00009310 File Offset: 0x00007510
		// (set) Token: 0x06000298 RID: 664 RVA: 0x00009318 File Offset: 0x00007518
		public bool IsAlias { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00009321 File Offset: 0x00007521
		// (set) Token: 0x0600029A RID: 666 RVA: 0x00009329 File Offset: 0x00007529
		public bool HasValueChild { get; set; }

		// Token: 0x0600029B RID: 667 RVA: 0x00009334 File Offset: 0x00007534
		public void Sort()
		{
			if (this.HasQualifier)
			{
				this.GetQualifier().Sort((XmpNode a, XmpNode b) => XmpNode.QualifierOrderComparer.Default.Compare(a.Name, b.Name));
			}
			if (this._children != null)
			{
				if (!this.Options.IsArray)
				{
					this._children.Sort();
				}
				foreach (XmpNode xmpNode in this._children)
				{
					xmpNode.Sort();
				}
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000093D8 File Offset: 0x000075D8
		private void DumpNode(StringBuilder result, bool recursive, int indent, int index)
		{
			for (int i = 0; i < indent; i++)
			{
				result.Append('\t');
			}
			if (this.Parent != null)
			{
				if (this.Options.IsQualifier)
				{
					result.Append('?');
					result.Append(this.Name);
				}
				else if (this.Parent.Options.IsArray)
				{
					result.Append('[');
					result.Append(index);
					result.Append(']');
				}
				else
				{
					result.Append(this.Name);
				}
			}
			else
			{
				result.Append("ROOT NODE");
				if (!string.IsNullOrEmpty(this.Name))
				{
					result.Append(" (");
					result.Append(this.Name);
					result.Append(')');
				}
			}
			if (!string.IsNullOrEmpty(this.Value))
			{
				result.Append(" = \"");
				result.Append(this.Value);
				result.Append('"');
			}
			if (this.Options.ContainsOneOf(-1))
			{
				result.Append("\t(");
				result.Append(this.Options);
				result.Append(" : ");
				result.Append(this.Options.GetOptionsString());
				result.Append(')');
			}
			result.Append('\n');
			if (recursive && this.HasQualifier)
			{
				int num = 0;
				foreach (XmpNode xmpNode in this.GetQualifier().OrderBy((XmpNode q) => q.Name, XmpNode.QualifierOrderComparer.Default))
				{
					xmpNode.DumpNode(result, recursive, indent + 2, ++num);
				}
			}
			if (recursive && this.HasChildren)
			{
				int num2 = 0;
				foreach (XmpNode xmpNode2 in from c in this.GetChildren()
					orderby c
					select c)
				{
					xmpNode2.DumpNode(result, recursive, indent + 1, ++num2);
				}
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00009618 File Offset: 0x00007818
		private bool IsLanguageNode
		{
			get
			{
				return this.Name == "xml:lang";
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0000962A File Offset: 0x0000782A
		private bool IsTypeNode
		{
			get
			{
				return this.Name == "rdf:type";
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000963C File Offset: 0x0000783C
		private List<XmpNode> GetChildren()
		{
			List<XmpNode> list;
			if ((list = this._children) == null)
			{
				list = (this._children = new List<XmpNode>(0));
			}
			return list;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00009662 File Offset: 0x00007862
		public IEnumerable<object> GetUnmodifiableChildren()
		{
			return this.GetChildren().Cast<object>().ToList<object>();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00009674 File Offset: 0x00007874
		private List<XmpNode> GetQualifier()
		{
			List<XmpNode> list;
			if ((list = this._qualifier) == null)
			{
				list = (this._qualifier = new List<XmpNode>(0));
			}
			return list;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000969C File Offset: 0x0000789C
		private static XmpNode Find(IEnumerable<XmpNode> list, string expr)
		{
			if (list == null)
			{
				return null;
			}
			return list.FirstOrDefault((XmpNode node) => node.Name == expr);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x000096D0 File Offset: 0x000078D0
		private static XmpNode FindChild(List<XmpNode> children, ref Dictionary<string, XmpNode> lookup, string expr)
		{
			XmpNode xmpNode = null;
			if (lookup == null)
			{
				if (children.Count > 9)
				{
					lookup = new Dictionary<string, XmpNode>(16);
					using (List<XmpNode>.Enumerator enumerator = children.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XmpNode xmpNode2 = enumerator.Current;
							if (xmpNode2.Name != "rdf:li" && xmpNode2.Name != "[]")
							{
								lookup.Add(xmpNode2.Name, xmpNode2);
							}
						}
						goto IL_007D;
					}
				}
				xmpNode = XmpNode.Find(children, expr);
			}
			IL_007D:
			Dictionary<string, XmpNode> dictionary = lookup;
			if (dictionary != null)
			{
				dictionary.TryGetValue(expr, out xmpNode);
			}
			return xmpNode;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000977C File Offset: 0x0000797C
		private void AssertChildNotExisting(string childName)
		{
			if (childName != "[]" && this.FindChildByName(childName) != null)
			{
				throw new XmpException("Duplicate property or field node '" + childName + "'", XmpErrorCode.BadXmp);
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x000097AF File Offset: 0x000079AF
		private void AssertQualifierNotExisting(string qualifierName)
		{
			if (qualifierName != "[]" && this.FindQualifierByName(qualifierName) != null)
			{
				throw new XmpException("Duplicate '" + qualifierName + "' qualifier", XmpErrorCode.BadXmp);
			}
		}

		// Token: 0x040000FE RID: 254
		private List<XmpNode> _children;

		// Token: 0x040000FF RID: 255
		private Dictionary<string, XmpNode> _childrenLookup;

		// Token: 0x04000100 RID: 256
		private List<XmpNode> _qualifier;

		// Token: 0x04000101 RID: 257
		private PropertyOptions _options;

		// Token: 0x020002CE RID: 718
		private sealed class Iterator391 : IIterator
		{
			// Token: 0x060028CE RID: 10446 RVA: 0x000BFA5F File Offset: 0x000BDC5F
			public Iterator391(IIterator it)
			{
				this._it = it;
			}

			// Token: 0x060028CF RID: 10447 RVA: 0x000BFA6E File Offset: 0x000BDC6E
			public bool HasNext()
			{
				return this._it.HasNext();
			}

			// Token: 0x060028D0 RID: 10448 RVA: 0x000BFA7B File Offset: 0x000BDC7B
			public object Next()
			{
				return this._it.Next();
			}

			// Token: 0x060028D1 RID: 10449 RVA: 0x000BFA88 File Offset: 0x000BDC88
			public void Remove()
			{
				throw new NotSupportedException("remove() is not allowed due to the internal constraints");
			}

			// Token: 0x040011AD RID: 4525
			private readonly IIterator _it;
		}

		// Token: 0x020002CF RID: 719
		private sealed class QualifierOrderComparer : IComparer<string>
		{
			// Token: 0x060028D2 RID: 10450 RVA: 0x000BFA94 File Offset: 0x000BDC94
			public int Compare(string x, string y)
			{
				if (string.Equals(x, y))
				{
					return 0;
				}
				if (x == "xml:lang")
				{
					return -1;
				}
				if (!(x == "rdf:type"))
				{
					return 0;
				}
				if (!(y == "xml:lang"))
				{
					return -1;
				}
				return 1;
			}

			// Token: 0x040011AE RID: 4526
			public static readonly XmpNode.QualifierOrderComparer Default = new XmpNode.QualifierOrderComparer();
		}
	}
}
