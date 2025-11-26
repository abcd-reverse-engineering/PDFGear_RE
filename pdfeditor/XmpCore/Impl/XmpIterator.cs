using System;
using System.Linq;
using Sharpen;
using XmpCore.Impl.XPath;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x02000037 RID: 55
	public sealed class XmpIterator : IXmpIterator, IIterator
	{
		// Token: 0x06000215 RID: 533 RVA: 0x00007754 File Offset: 0x00005954
		public XmpIterator(XmpMeta xmp, string schemaNs, string propPath, IteratorOptions options)
		{
			this.Options = options ?? new IteratorOptions();
			string text = null;
			bool flag = !string.IsNullOrEmpty(schemaNs);
			bool flag2 = !string.IsNullOrEmpty(propPath);
			XmpNode xmpNode;
			if (!flag && !flag2)
			{
				xmpNode = xmp.GetRoot();
			}
			else if (flag && flag2)
			{
				XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propPath);
				XmpPath xmpPath2 = new XmpPath();
				for (int i = 0; i < xmpPath.Size() - 1; i++)
				{
					xmpPath2.Add(xmpPath.GetSegment(i));
				}
				xmpNode = XmpNodeUtils.FindNode(xmp.GetRoot(), xmpPath, false, null);
				this.BaseNamespace = schemaNs;
				text = xmpPath2.ToString();
			}
			else
			{
				if (!flag || flag2)
				{
					throw new XmpException("Schema namespace URI is required", XmpErrorCode.BadSchema);
				}
				xmpNode = XmpNodeUtils.FindSchemaNode(xmp.GetRoot(), schemaNs, false);
			}
			IIterator iterator2;
			if (xmpNode == null)
			{
				IIterator iterator = Enumerable.Empty<object>().Iterator<object>();
				iterator2 = iterator;
			}
			else
			{
				IIterator iterator = ((!this.Options.IsJustChildren) ? new XmpIterator.NodeIterator(this, xmpNode, text, 1) : new XmpIterator.NodeIteratorChildren(this, xmpNode, text));
				iterator2 = iterator;
			}
			this._nodeIterator = iterator2;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000785E File Offset: 0x00005A5E
		public void SkipSubtree()
		{
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00007860 File Offset: 0x00005A60
		public void SkipSiblings()
		{
			this.SkipSubtree();
			this._skipSiblings = true;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000786F File Offset: 0x00005A6F
		public bool HasNext()
		{
			return this._nodeIterator.HasNext();
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000787C File Offset: 0x00005A7C
		public object Next()
		{
			return this._nodeIterator.Next();
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00007889 File Offset: 0x00005A89
		public void Remove()
		{
			throw new NotSupportedException("The XMPIterator does not support remove().");
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00007895 File Offset: 0x00005A95
		private IteratorOptions Options { get; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000789D File Offset: 0x00005A9D
		// (set) Token: 0x0600021D RID: 541 RVA: 0x000078A5 File Offset: 0x00005AA5
		private string BaseNamespace { get; set; }

		// Token: 0x040000F7 RID: 247
		private bool _skipSiblings;

		// Token: 0x040000F8 RID: 248
		private readonly IIterator _nodeIterator;

		// Token: 0x020002C7 RID: 711
		private class NodeIterator : IIterator
		{
			// Token: 0x060028AC RID: 10412 RVA: 0x000BF3E0 File Offset: 0x000BD5E0
			protected NodeIterator(XmpIterator enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x060028AD RID: 10413 RVA: 0x000BF400 File Offset: 0x000BD600
			public NodeIterator(XmpIterator enclosing, XmpNode visitedNode, string parentPath, int index)
			{
				this._enclosing = enclosing;
				this._visitedNode = visitedNode;
				this._state = 0;
				if (visitedNode.Options.IsSchemaNode)
				{
					this._enclosing.BaseNamespace = visitedNode.Name;
				}
				this._path = this.AccumulatePath(visitedNode, parentPath, index);
			}

			// Token: 0x060028AE RID: 10414 RVA: 0x000BF468 File Offset: 0x000BD668
			public virtual bool HasNext()
			{
				if (this._returnProperty != null)
				{
					return true;
				}
				int state = this._state;
				if (state == 0)
				{
					return this.ReportNode();
				}
				if (state != 1)
				{
					if (this._childrenIterator == null)
					{
						this._childrenIterator = this._visitedNode.IterateQualifier();
					}
					return this.IterateChildrenMethod(this._childrenIterator);
				}
				if (this._childrenIterator == null)
				{
					this._childrenIterator = this._visitedNode.IterateChildren();
				}
				bool flag = this.IterateChildrenMethod(this._childrenIterator);
				if (!flag && this._visitedNode.HasQualifier && !this._enclosing.Options.IsOmitQualifiers)
				{
					this._state = 2;
					this._childrenIterator = null;
					flag = this.HasNext();
				}
				return flag;
			}

			// Token: 0x060028AF RID: 10415 RVA: 0x000BF51C File Offset: 0x000BD71C
			private bool ReportNode()
			{
				this._state = 1;
				if (this._visitedNode.Parent != null && (!this._enclosing.Options.IsJustLeafNodes || !this._visitedNode.HasChildren))
				{
					this._returnProperty = XmpIterator.NodeIterator.CreatePropertyInfo(this._visitedNode, this._enclosing.BaseNamespace, this._path);
					return true;
				}
				return this.HasNext();
			}

			// Token: 0x060028B0 RID: 10416 RVA: 0x000BF588 File Offset: 0x000BD788
			private bool IterateChildrenMethod(IIterator iterator)
			{
				if (this._enclosing._skipSiblings)
				{
					this._enclosing._skipSiblings = false;
					this._subIterator = Enumerable.Empty<object>().Iterator<object>();
				}
				if (!this._subIterator.HasNext() && iterator.HasNext())
				{
					XmpNode xmpNode = (XmpNode)iterator.Next();
					this._index++;
					this._subIterator = new XmpIterator.NodeIterator(this._enclosing, xmpNode, this._path, this._index);
				}
				if (this._subIterator.HasNext())
				{
					this._returnProperty = (IXmpPropertyInfo)this._subIterator.Next();
					return true;
				}
				return false;
			}

			// Token: 0x060028B1 RID: 10417 RVA: 0x000BF631 File Offset: 0x000BD831
			public virtual object Next()
			{
				if (this.HasNext())
				{
					object returnProperty = this._returnProperty;
					this._returnProperty = null;
					return returnProperty;
				}
				throw new InvalidOperationException("There are no more nodes to return");
			}

			// Token: 0x060028B2 RID: 10418 RVA: 0x000BF653 File Offset: 0x000BD853
			public virtual void Remove()
			{
				throw new NotSupportedException();
			}

			// Token: 0x060028B3 RID: 10419 RVA: 0x000BF65C File Offset: 0x000BD85C
			protected string AccumulatePath(XmpNode currNode, string parentPath, int currentIndex)
			{
				if (currNode.Parent == null || currNode.Options.IsSchemaNode)
				{
					return null;
				}
				string text;
				string text2;
				if (currNode.Parent.Options.IsArray)
				{
					text = string.Empty;
					text2 = "[" + currentIndex.ToString() + "]";
				}
				else
				{
					text = "/";
					text2 = currNode.Name;
				}
				if (string.IsNullOrEmpty(parentPath))
				{
					return text2;
				}
				if (!this._enclosing.Options.IsJustLeafName)
				{
					return parentPath + text + text2;
				}
				if (text2.StartsWith("?"))
				{
					return text2.Substring(1);
				}
				return text2;
			}

			// Token: 0x060028B4 RID: 10420 RVA: 0x000BF6FC File Offset: 0x000BD8FC
			protected static IXmpPropertyInfo CreatePropertyInfo(XmpNode node, string baseNs, string path)
			{
				string text = (node.Options.IsSchemaNode ? null : node.Value);
				return new XmpIterator.NodeIterator.XmpPropertyInfo(node, baseNs, path, text);
			}

			// Token: 0x060028B5 RID: 10421 RVA: 0x000BF729 File Offset: 0x000BD929
			protected IXmpPropertyInfo GetReturnProperty()
			{
				return this._returnProperty;
			}

			// Token: 0x060028B6 RID: 10422 RVA: 0x000BF731 File Offset: 0x000BD931
			protected void SetReturnProperty(IXmpPropertyInfo returnProperty)
			{
				this._returnProperty = returnProperty;
			}

			// Token: 0x04001188 RID: 4488
			private const int IterateNode = 0;

			// Token: 0x04001189 RID: 4489
			private const int IterateChildren = 1;

			// Token: 0x0400118A RID: 4490
			private const int IterateQualifier = 2;

			// Token: 0x0400118B RID: 4491
			private int _state;

			// Token: 0x0400118C RID: 4492
			private readonly XmpNode _visitedNode;

			// Token: 0x0400118D RID: 4493
			private readonly string _path;

			// Token: 0x0400118E RID: 4494
			private IIterator _childrenIterator;

			// Token: 0x0400118F RID: 4495
			private int _index;

			// Token: 0x04001190 RID: 4496
			private IIterator _subIterator = Enumerable.Empty<object>().Iterator<object>();

			// Token: 0x04001191 RID: 4497
			private IXmpPropertyInfo _returnProperty;

			// Token: 0x04001192 RID: 4498
			private readonly XmpIterator _enclosing;

			// Token: 0x020007AD RID: 1965
			private sealed class XmpPropertyInfo : IXmpPropertyInfo, IXmpProperty
			{
				// Token: 0x17000DA9 RID: 3497
				// (get) Token: 0x0600379D RID: 14237 RVA: 0x0011DFF6 File Offset: 0x0011C1F6
				public string Path { get; }

				// Token: 0x17000DAA RID: 3498
				// (get) Token: 0x0600379E RID: 14238 RVA: 0x0011DFFE File Offset: 0x0011C1FE
				public string Value { get; }

				// Token: 0x0600379F RID: 14239 RVA: 0x0011E006 File Offset: 0x0011C206
				public XmpPropertyInfo(XmpNode node, string baseNs, string path, string value)
				{
					this._node = node;
					this._baseNs = baseNs;
					this.Path = path;
					this.Value = value;
				}

				// Token: 0x17000DAB RID: 3499
				// (get) Token: 0x060037A0 RID: 14240 RVA: 0x0011E02C File Offset: 0x0011C22C
				public string Namespace
				{
					get
					{
						if (!this._node.Options.IsSchemaNode)
						{
							QName qname = new QName(this._node.Name);
							return XmpMetaFactory.SchemaRegistry.GetNamespaceUri(qname.Prefix);
						}
						return this._baseNs;
					}
				}

				// Token: 0x17000DAC RID: 3500
				// (get) Token: 0x060037A1 RID: 14241 RVA: 0x0011E073 File Offset: 0x0011C273
				public PropertyOptions Options
				{
					get
					{
						return this._node.Options;
					}
				}

				// Token: 0x17000DAD RID: 3501
				// (get) Token: 0x060037A2 RID: 14242 RVA: 0x0011E080 File Offset: 0x0011C280
				public string Language
				{
					get
					{
						return null;
					}
				}

				// Token: 0x040026D4 RID: 9940
				private readonly XmpNode _node;

				// Token: 0x040026D5 RID: 9941
				private readonly string _baseNs;
			}
		}

		// Token: 0x020002C8 RID: 712
		private sealed class NodeIteratorChildren : XmpIterator.NodeIterator
		{
			// Token: 0x060028B7 RID: 10423 RVA: 0x000BF73C File Offset: 0x000BD93C
			public NodeIteratorChildren(XmpIterator enclosing, XmpNode parentNode, string parentPath)
				: base(enclosing)
			{
				this._enclosing = enclosing;
				if (parentNode.Options.IsSchemaNode)
				{
					this._enclosing.BaseNamespace = parentNode.Name;
				}
				this._parentPath = base.AccumulatePath(parentNode, parentPath, 1);
				this._childrenIterator = parentNode.IterateChildren();
			}

			// Token: 0x060028B8 RID: 10424 RVA: 0x000BF790 File Offset: 0x000BD990
			public override bool HasNext()
			{
				if (base.GetReturnProperty() != null)
				{
					return true;
				}
				if (this._enclosing._skipSiblings)
				{
					return false;
				}
				if (!this._childrenIterator.HasNext())
				{
					return false;
				}
				XmpNode xmpNode = (XmpNode)this._childrenIterator.Next();
				this._index++;
				string text = null;
				if (xmpNode.Options.IsSchemaNode)
				{
					this._enclosing.BaseNamespace = xmpNode.Name;
				}
				else if (xmpNode.Parent != null)
				{
					text = base.AccumulatePath(xmpNode, this._parentPath, this._index);
				}
				if (!this._enclosing.Options.IsJustLeafNodes || !xmpNode.HasChildren)
				{
					base.SetReturnProperty(XmpIterator.NodeIterator.CreatePropertyInfo(xmpNode, this._enclosing.BaseNamespace, text));
					return true;
				}
				return this.HasNext();
			}

			// Token: 0x04001193 RID: 4499
			private readonly string _parentPath;

			// Token: 0x04001194 RID: 4500
			private readonly IIterator _childrenIterator;

			// Token: 0x04001195 RID: 4501
			private int _index;

			// Token: 0x04001196 RID: 4502
			private readonly XmpIterator _enclosing;
		}
	}
}
