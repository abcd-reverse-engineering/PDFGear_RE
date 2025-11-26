using System;
using System.Collections.Generic;
using Sharpen;
using XmpCore.Impl.XPath;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x02000038 RID: 56
	public sealed class XmpMeta : IXmpMeta
	{
		// Token: 0x0600021E RID: 542 RVA: 0x000078AE File Offset: 0x00005AAE
		public XmpMeta()
			: this(new XmpNode(null, null, null))
		{
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000078BE File Offset: 0x00005ABE
		public XmpMeta(XmpNode tree)
		{
			this._tree = tree;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000078D0 File Offset: 0x00005AD0
		public void AppendArrayItem(string schemaNs, string arrayName, PropertyOptions arrayOptions, string itemValue, PropertyOptions itemOptions)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			if (arrayOptions == null)
			{
				arrayOptions = new PropertyOptions();
			}
			if (!arrayOptions.IsOnlyArrayOptions)
			{
				throw new XmpException("Only array form flags allowed for arrayOptions", XmpErrorCode.BadOptions);
			}
			arrayOptions = XmpNodeUtils.VerifySetOptions(arrayOptions, null);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode != null)
			{
				if (!xmpNode.Options.IsArray)
				{
					throw new XmpException("The named property is not an array", XmpErrorCode.BadXPath);
				}
			}
			else
			{
				if (!arrayOptions.IsArray)
				{
					throw new XmpException("Explicit arrayOptions required to create new array", XmpErrorCode.BadOptions);
				}
				xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, true, arrayOptions);
				if (xmpNode == null)
				{
					throw new XmpException("Failure creating array node", XmpErrorCode.BadXPath);
				}
			}
			XmpMeta.DoSetArrayItem(xmpNode, -1, itemValue, itemOptions, true);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00007985 File Offset: 0x00005B85
		public void AppendArrayItem(string schemaNs, string arrayName, string itemValue)
		{
			this.AppendArrayItem(schemaNs, arrayName, null, itemValue, null);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00007994 File Offset: 0x00005B94
		public int CountArrayItems(string schemaNs, string arrayName)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode == null)
			{
				return 0;
			}
			if (xmpNode.Options.IsArray)
			{
				return xmpNode.GetChildrenLength();
			}
			throw new XmpException("The named property is not an array", XmpErrorCode.BadXPath);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x000079EC File Offset: 0x00005BEC
		public void DeleteArrayItem(string schemaNs, string arrayName, int itemIndex)
		{
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertArrayName(arrayName);
				string text = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
				this.DeleteProperty(schemaNs, text);
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00007A2C File Offset: 0x00005C2C
		public void DeleteProperty(string schemaNs, string propName)
		{
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertPropName(propName);
				XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
				XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
				if (xmpNode != null)
				{
					XmpNodeUtils.DeleteNode(xmpNode);
				}
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00007A7C File Offset: 0x00005C7C
		public void DeleteQualifier(string schemaNs, string propName, string qualNs, string qualName)
		{
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertPropName(propName);
				string text = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
				this.DeleteProperty(schemaNs, text);
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00007AC4 File Offset: 0x00005CC4
		public void DeleteStructField(string schemaNs, string structName, string fieldNs, string fieldName)
		{
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertStructName(structName);
				string text = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
				this.DeleteProperty(schemaNs, text);
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00007B0C File Offset: 0x00005D0C
		public bool DoesPropertyExist(string schemaNs, string propName)
		{
			bool flag;
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertPropName(propName);
				XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
				flag = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null) != null;
			}
			catch (XmpException)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00007B58 File Offset: 0x00005D58
		public bool DoesArrayItemExist(string schemaNs, string arrayName, int itemIndex)
		{
			bool flag;
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertArrayName(arrayName);
				string text = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
				flag = this.DoesPropertyExist(schemaNs, text);
			}
			catch (XmpException)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00007B9C File Offset: 0x00005D9C
		public bool DoesStructFieldExist(string schemaNs, string structName, string fieldNs, string fieldName)
		{
			bool flag;
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertStructName(structName);
				string text = XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
				flag = this.DoesPropertyExist(schemaNs, structName + text);
			}
			catch (XmpException)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00007BE8 File Offset: 0x00005DE8
		public bool DoesQualifierExist(string schemaNs, string propName, string qualNs, string qualName)
		{
			bool flag;
			try
			{
				ParameterAsserts.AssertSchemaNs(schemaNs);
				ParameterAsserts.AssertPropName(propName);
				string text = XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
				flag = this.DoesPropertyExist(schemaNs, propName + text);
			}
			catch (XmpException)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00007C34 File Offset: 0x00005E34
		public IXmpProperty GetArrayItem(string schemaNs, string arrayName, int itemIndex)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			string text = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
			return this.GetProperty(schemaNs, text);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00007C60 File Offset: 0x00005E60
		public IXmpProperty GetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(altTextName);
			ParameterAsserts.AssertSpecificLang(specificLang);
			genericLang = ((genericLang != null) ? Utils.NormalizeLangValue(genericLang) : null);
			specificLang = Utils.NormalizeLangValue(specificLang);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode == null)
			{
				return null;
			}
			object[] array = XmpNodeUtils.ChooseLocalizedText(xmpNode, genericLang, specificLang);
			int num = (int)array[0];
			XmpNode xmpNode2 = (XmpNode)array[1];
			if (num != 0)
			{
				return new XmpMeta.XmpProperty(xmpNode2);
			}
			return null;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(altTextName);
			ParameterAsserts.AssertSpecificLang(specificLang);
			genericLang = ((genericLang != null) ? Utils.NormalizeLangValue(genericLang) : null);
			specificLang = Utils.NormalizeLangValue(specificLang);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, altTextName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, true, new PropertyOptions(7680));
			if (xmpNode == null)
			{
				throw new XmpException("Failed to find or create array node", XmpErrorCode.BadXPath);
			}
			if (!xmpNode.Options.IsArrayAltText)
			{
				if (xmpNode.HasChildren || !xmpNode.Options.IsArrayAlternate)
				{
					throw new XmpException("Specified property is no alt-text array", XmpErrorCode.BadXPath);
				}
				xmpNode.Options.IsArrayAltText = true;
			}
			bool flag = false;
			XmpNode xmpNode2 = null;
			IIterator iterator = xmpNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode3 = (XmpNode)iterator.Next();
				if (!xmpNode3.HasQualifier || xmpNode3.GetQualifier(1).Name != "xml:lang")
				{
					throw new XmpException("Language qualifier must be first", XmpErrorCode.BadXPath);
				}
				if (xmpNode3.GetQualifier(1).Value == "x-default")
				{
					xmpNode2 = xmpNode3;
					flag = true;
					break;
				}
			}
			if (xmpNode2 != null && xmpNode.GetChildrenLength() > 1)
			{
				xmpNode.RemoveChild(xmpNode2);
				xmpNode.AddChild(1, xmpNode2);
			}
			object[] array = XmpNodeUtils.ChooseLocalizedText(xmpNode, genericLang, specificLang);
			int num = (int)array[0];
			XmpNode xmpNode4 = (XmpNode)array[1];
			bool flag2 = specificLang == "x-default";
			switch (num)
			{
			case 0:
				XmpNodeUtils.AppendLangItem(xmpNode, "x-default", itemValue);
				flag = true;
				if (!flag2)
				{
					XmpNodeUtils.AppendLangItem(xmpNode, specificLang, itemValue);
				}
				break;
			case 1:
				if (!flag2)
				{
					if (flag && xmpNode2 != xmpNode4 && xmpNode2 != null && xmpNode2.Value == xmpNode4.Value)
					{
						xmpNode2.Value = itemValue;
					}
					xmpNode4.Value = itemValue;
				}
				else
				{
					IIterator iterator2 = xmpNode.IterateChildren();
					while (iterator2.HasNext())
					{
						XmpNode xmpNode5 = (XmpNode)iterator2.Next();
						if (xmpNode5 != xmpNode2 && !(xmpNode5.Value != ((xmpNode2 != null) ? xmpNode2.Value : null)))
						{
							xmpNode5.Value = itemValue;
						}
					}
					if (xmpNode2 != null)
					{
						xmpNode2.Value = itemValue;
					}
				}
				break;
			case 2:
				if (flag && xmpNode2 != xmpNode4 && xmpNode2 != null && xmpNode2.Value == xmpNode4.Value)
				{
					xmpNode2.Value = itemValue;
				}
				xmpNode4.Value = itemValue;
				break;
			case 3:
				XmpNodeUtils.AppendLangItem(xmpNode, specificLang, itemValue);
				if (flag2)
				{
					flag = true;
				}
				break;
			case 4:
				if (xmpNode2 != null && xmpNode.GetChildrenLength() == 1)
				{
					xmpNode2.Value = itemValue;
				}
				XmpNodeUtils.AppendLangItem(xmpNode, specificLang, itemValue);
				break;
			case 5:
				XmpNodeUtils.AppendLangItem(xmpNode, specificLang, itemValue);
				if (flag2)
				{
					flag = true;
				}
				break;
			default:
				throw new XmpException("Unexpected result from ChooseLocalizedText", XmpErrorCode.InternalFailure);
			}
			if (!flag && xmpNode.GetChildrenLength() == 1)
			{
				XmpNodeUtils.AppendLangItem(xmpNode, "x-default", itemValue);
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00007FAF File Offset: 0x000061AF
		public void SetLocalizedText(string schemaNs, string altTextName, string genericLang, string specificLang, string itemValue)
		{
			this.SetLocalizedText(schemaNs, altTextName, genericLang, specificLang, itemValue, null);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00007FBF File Offset: 0x000061BF
		public IXmpProperty GetProperty(string schemaNs, string propName)
		{
			return this.GetProperty(schemaNs, propName, XmpMeta.ValueType.String);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00007FCC File Offset: 0x000061CC
		private IXmpProperty GetProperty(string schemaNs, string propName, XmpMeta.ValueType valueType)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertPropName(propName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode == null)
			{
				return null;
			}
			if (valueType != XmpMeta.ValueType.String && xmpNode.Options.IsCompositeProperty)
			{
				throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.BadXPath);
			}
			return new XmpMeta.XmpProperty(XmpMeta.EvaluateNodeValue(valueType, xmpNode), xmpNode);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000802C File Offset: 0x0000622C
		private object GetPropertyObject(string schemaNs, string propName, XmpMeta.ValueType valueType)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertPropName(propName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode == null)
			{
				return null;
			}
			if (valueType != XmpMeta.ValueType.String && xmpNode.Options.IsCompositeProperty)
			{
				throw new XmpException("Property must be simple when a value type is requested", XmpErrorCode.BadXPath);
			}
			return XmpMeta.EvaluateNodeValue(valueType, xmpNode);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00008085 File Offset: 0x00006285
		public bool GetPropertyBoolean(string schemaNs, string propName)
		{
			return (bool)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Boolean);
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00008095 File Offset: 0x00006295
		public void SetPropertyBoolean(string schemaNs, string propName, bool propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue ? "True" : "False", options);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000080B0 File Offset: 0x000062B0
		public void SetPropertyBoolean(string schemaNs, string propName, bool propValue)
		{
			this.SetProperty(schemaNs, propName, propValue ? "True" : "False", null);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x000080CA File Offset: 0x000062CA
		public int GetPropertyInteger(string schemaNs, string propName)
		{
			return (int)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Integer);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x000080DA File Offset: 0x000062DA
		public void SetPropertyInteger(string schemaNs, string propName, int propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x000080EC File Offset: 0x000062EC
		public void SetPropertyInteger(string schemaNs, string propName, int propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x000080FD File Offset: 0x000062FD
		public long GetPropertyLong(string schemaNs, string propName)
		{
			return (long)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Long);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000810D File Offset: 0x0000630D
		public void SetPropertyLong(string schemaNs, string propName, long propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000811F File Offset: 0x0000631F
		public void SetPropertyLong(string schemaNs, string propName, long propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00008130 File Offset: 0x00006330
		public double GetPropertyDouble(string schemaNs, string propName)
		{
			return (double)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Double);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00008140 File Offset: 0x00006340
		public void SetPropertyDouble(string schemaNs, string propName, double propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00008152 File Offset: 0x00006352
		public void SetPropertyDouble(string schemaNs, string propName, double propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00008163 File Offset: 0x00006363
		public IXmpDateTime GetPropertyDate(string schemaNs, string propName)
		{
			return (IXmpDateTime)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Date);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00008173 File Offset: 0x00006373
		public void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00008180 File Offset: 0x00006380
		public void SetPropertyDate(string schemaNs, string propName, IXmpDateTime propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000818C File Offset: 0x0000638C
		public Calendar GetPropertyCalendar(string schemaNs, string propName)
		{
			return (Calendar)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Calendar);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000819C File Offset: 0x0000639C
		public void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x000081A9 File Offset: 0x000063A9
		public void SetPropertyCalendar(string schemaNs, string propName, Calendar propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000081B5 File Offset: 0x000063B5
		public byte[] GetPropertyBase64(string schemaNs, string propName)
		{
			return (byte[])this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.Base64);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000081C5 File Offset: 0x000063C5
		public string GetPropertyString(string schemaNs, string propName)
		{
			return (string)this.GetPropertyObject(schemaNs, propName, XmpMeta.ValueType.String);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000081D5 File Offset: 0x000063D5
		public void SetPropertyBase64(string schemaNs, string propName, byte[] propValue, PropertyOptions options)
		{
			this.SetProperty(schemaNs, propName, propValue, options);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000081E2 File Offset: 0x000063E2
		public void SetPropertyBase64(string schemaNs, string propName, byte[] propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x000081F0 File Offset: 0x000063F0
		public IXmpProperty GetQualifier(string schemaNs, string propName, string qualNs, string qualName)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertPropName(propName);
			string text = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
			return this.GetProperty(schemaNs, text);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00008220 File Offset: 0x00006420
		public IXmpProperty GetStructField(string schemaNs, string structName, string fieldNs, string fieldName)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertStructName(structName);
			string text = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
			return this.GetProperty(schemaNs, text);
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00008250 File Offset: 0x00006450
		public IEnumerable<IXmpPropertyInfo> Properties
		{
			get
			{
				XmpMeta.<get_Properties>d__49 <get_Properties>d__ = new XmpMeta.<get_Properties>d__49(-2);
				<get_Properties>d__.<>4__this = this;
				return <get_Properties>d__;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00008260 File Offset: 0x00006460
		public void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode != null)
			{
				XmpMeta.DoSetArrayItem(xmpNode, itemIndex, itemValue, options, false);
				return;
			}
			throw new XmpException("Specified array does not exist", XmpErrorCode.BadXPath);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x000082AC File Offset: 0x000064AC
		public void SetArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
		{
			this.SetArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x000082BC File Offset: 0x000064BC
		public void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertArrayName(arrayName);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, arrayName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, false, null);
			if (xmpNode != null)
			{
				XmpMeta.DoSetArrayItem(xmpNode, itemIndex, itemValue, options, true);
				return;
			}
			throw new XmpException("Specified array does not exist", XmpErrorCode.BadXPath);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00008308 File Offset: 0x00006508
		public void InsertArrayItem(string schemaNs, string arrayName, int itemIndex, string itemValue)
		{
			this.InsertArrayItem(schemaNs, arrayName, itemIndex, itemValue, null);
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00008318 File Offset: 0x00006518
		public void SetProperty(string schemaNs, string propName, object propValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertPropName(propName);
			options = XmpNodeUtils.VerifySetOptions(options, propValue);
			XmpPath xmpPath = XmpPathParser.ExpandXPath(schemaNs, propName);
			XmpNode xmpNode = XmpNodeUtils.FindNode(this._tree, xmpPath, true, options);
			if (xmpNode != null)
			{
				XmpMeta.SetNode(xmpNode, propValue, options, false);
				return;
			}
			throw new XmpException("Specified property does not exist", XmpErrorCode.BadXPath);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000836D File Offset: 0x0000656D
		public void SetProperty(string schemaNs, string propName, object propValue)
		{
			this.SetProperty(schemaNs, propName, propValue, null);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000837C File Offset: 0x0000657C
		public void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertPropName(propName);
			if (!this.DoesPropertyExist(schemaNs, propName))
			{
				throw new XmpException("Specified property does not exist!", XmpErrorCode.BadXPath);
			}
			string text = propName + XmpPathFactory.ComposeQualifierPath(qualNs, qualName);
			this.SetProperty(schemaNs, text, qualValue, options);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x000083C7 File Offset: 0x000065C7
		public void SetQualifier(string schemaNs, string propName, string qualNs, string qualName, string qualValue)
		{
			this.SetQualifier(schemaNs, propName, qualNs, qualName, qualValue, null);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x000083D8 File Offset: 0x000065D8
		public void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNs(schemaNs);
			ParameterAsserts.AssertStructName(structName);
			string text = structName + XmpPathFactory.ComposeStructFieldPath(fieldNs, fieldName);
			this.SetProperty(schemaNs, text, fieldValue, options);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000840C File Offset: 0x0000660C
		public void SetStructField(string schemaNs, string structName, string fieldNs, string fieldName, string fieldValue)
		{
			this.SetStructField(schemaNs, structName, fieldNs, fieldName, fieldValue, null);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000841C File Offset: 0x0000661C
		public string GetObjectName()
		{
			return this._tree.Name ?? string.Empty;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00008432 File Offset: 0x00006632
		public void SetObjectName(string name)
		{
			this._tree.Name = name;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00008440 File Offset: 0x00006640
		public string GetPacketHeader()
		{
			return this._packetHeader;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00008448 File Offset: 0x00006648
		public void SetPacketHeader(string packetHeader)
		{
			this._packetHeader = packetHeader;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00008451 File Offset: 0x00006651
		public IXmpMeta Clone()
		{
			return new XmpMeta((XmpNode)this._tree.Clone());
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00008468 File Offset: 0x00006668
		public string DumpObject()
		{
			return this.GetRoot().DumpNode(true);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00008476 File Offset: 0x00006676
		public void Sort()
		{
			this._tree.Sort();
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00008483 File Offset: 0x00006683
		public void Normalize(ParseOptions options)
		{
			XmpNormalizer.Process(this, options ?? new ParseOptions());
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00008496 File Offset: 0x00006696
		public XmpNode GetRoot()
		{
			return this._tree;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x000084A0 File Offset: 0x000066A0
		private static void DoSetArrayItem(XmpNode arrayNode, int itemIndex, string itemValue, PropertyOptions itemOptions, bool insert)
		{
			XmpNode xmpNode = new XmpNode("[]", null);
			itemOptions = XmpNodeUtils.VerifySetOptions(itemOptions, itemValue);
			int num = (insert ? (arrayNode.GetChildrenLength() + 1) : arrayNode.GetChildrenLength());
			if (itemIndex == -1)
			{
				itemIndex = num;
			}
			if (1 > itemIndex || itemIndex > num)
			{
				throw new XmpException("Array index out of bounds", XmpErrorCode.BadIndex);
			}
			if (!insert)
			{
				arrayNode.RemoveChild(itemIndex);
			}
			arrayNode.AddChild(itemIndex, xmpNode);
			XmpMeta.SetNode(xmpNode, itemValue, itemOptions, false);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00008510 File Offset: 0x00006710
		internal static void SetNode(XmpNode node, object value, PropertyOptions newOptions, bool deleteExisting)
		{
			int num = 7936;
			if (deleteExisting)
			{
				node.Clear();
			}
			node.Options.MergeWith(newOptions);
			if ((node.Options.GetOptions() & num) == 0)
			{
				XmpNodeUtils.SetNodeValue(node, value);
				return;
			}
			if (value != null && value.ToString().Length > 0)
			{
				throw new XmpException("Composite nodes can't have values", XmpErrorCode.BadXPath);
			}
			if ((node.Options.GetOptions() & num) != 0 && (newOptions.GetOptions() & num) != (node.Options.GetOptions() & num))
			{
				throw new XmpException("Requested and existing composite form mismatch", XmpErrorCode.BadXPath);
			}
			node.RemoveChildren();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x000085A8 File Offset: 0x000067A8
		private static object EvaluateNodeValue(XmpMeta.ValueType valueType, XmpNode propNode)
		{
			switch (valueType)
			{
			case XmpMeta.ValueType.Boolean:
				return XmpUtils.ConvertToBoolean(propNode.Value);
			case XmpMeta.ValueType.Integer:
				return XmpUtils.ConvertToInteger(propNode.Value);
			case XmpMeta.ValueType.Long:
				return XmpUtils.ConvertToLong(propNode.Value);
			case XmpMeta.ValueType.Double:
				return XmpUtils.ConvertToDouble(propNode.Value);
			case XmpMeta.ValueType.Date:
				return XmpUtils.ConvertToDate(propNode.Value);
			case XmpMeta.ValueType.Calendar:
				return XmpUtils.ConvertToDate(propNode.Value).Calendar;
			case XmpMeta.ValueType.Base64:
				return XmpUtils.DecodeBase64(propNode.Value);
			}
			if (propNode.Value == null && !propNode.Options.IsCompositeProperty)
			{
				return string.Empty;
			}
			return propNode.Value;
		}

		// Token: 0x040000FB RID: 251
		private readonly XmpNode _tree;

		// Token: 0x040000FC RID: 252
		private string _packetHeader;

		// Token: 0x020002C9 RID: 713
		public enum ValueType
		{
			// Token: 0x04001198 RID: 4504
			String,
			// Token: 0x04001199 RID: 4505
			Boolean,
			// Token: 0x0400119A RID: 4506
			Integer,
			// Token: 0x0400119B RID: 4507
			Long,
			// Token: 0x0400119C RID: 4508
			Double,
			// Token: 0x0400119D RID: 4509
			Date,
			// Token: 0x0400119E RID: 4510
			Calendar,
			// Token: 0x0400119F RID: 4511
			Base64
		}

		// Token: 0x020002CA RID: 714
		private sealed class XmpProperty : IXmpProperty
		{
			// Token: 0x060028B9 RID: 10425 RVA: 0x000BF85F File Offset: 0x000BDA5F
			public XmpProperty(XmpNode itemNode)
			{
				this._node = itemNode;
				this._proptype = XmpMeta.XmpProperty.XmpPropertyType.item;
			}

			// Token: 0x060028BA RID: 10426 RVA: 0x000BF875 File Offset: 0x000BDA75
			public XmpProperty(object value, XmpNode propNode)
			{
				this._value = value;
				this._node = propNode;
				this._proptype = XmpMeta.XmpProperty.XmpPropertyType.prop;
			}

			// Token: 0x17000C5E RID: 3166
			// (get) Token: 0x060028BB RID: 10427 RVA: 0x000BF892 File Offset: 0x000BDA92
			public string Value
			{
				get
				{
					if (this._proptype == XmpMeta.XmpProperty.XmpPropertyType.item)
					{
						return this._node.Value;
					}
					object value = this._value;
					if (value == null)
					{
						return null;
					}
					return value.ToString();
				}
			}

			// Token: 0x17000C5F RID: 3167
			// (get) Token: 0x060028BC RID: 10428 RVA: 0x000BF8B9 File Offset: 0x000BDAB9
			public PropertyOptions Options
			{
				get
				{
					return this._node.Options;
				}
			}

			// Token: 0x17000C60 RID: 3168
			// (get) Token: 0x060028BD RID: 10429 RVA: 0x000BF8C6 File Offset: 0x000BDAC6
			public string Language
			{
				get
				{
					if (this._proptype != XmpMeta.XmpProperty.XmpPropertyType.item)
					{
						return null;
					}
					return this._node.GetQualifier(1).Value;
				}
			}

			// Token: 0x060028BE RID: 10430 RVA: 0x000BF8E3 File Offset: 0x000BDAE3
			public override string ToString()
			{
				if (this._proptype != XmpMeta.XmpProperty.XmpPropertyType.item)
				{
					return this._value.ToString();
				}
				return this._node.Value;
			}

			// Token: 0x040011A0 RID: 4512
			private readonly XmpMeta.XmpProperty.XmpPropertyType _proptype;

			// Token: 0x040011A1 RID: 4513
			private readonly object _value;

			// Token: 0x040011A2 RID: 4514
			private readonly XmpNode _node;

			// Token: 0x020007AE RID: 1966
			private enum XmpPropertyType
			{
				// Token: 0x040026D9 RID: 9945
				item,
				// Token: 0x040026DA RID: 9946
				prop
			}
		}
	}
}
