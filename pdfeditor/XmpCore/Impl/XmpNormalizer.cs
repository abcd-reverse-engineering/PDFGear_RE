using System;
using System.Collections;
using System.Collections.Generic;
using Sharpen;
using XmpCore.Impl.XPath;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003C RID: 60
	public static class XmpNormalizer
	{
		// Token: 0x060002B8 RID: 696 RVA: 0x0000A22C File Offset: 0x0000842C
		static XmpNormalizer()
		{
			PropertyOptions propertyOptions = new PropertyOptions
			{
				IsArray = true
			};
			PropertyOptions propertyOptions2 = new PropertyOptions
			{
				IsArray = true,
				IsArrayOrdered = true
			};
			PropertyOptions propertyOptions3 = new PropertyOptions
			{
				IsArray = true,
				IsArrayOrdered = true,
				IsArrayAlternate = true,
				IsArrayAltText = true
			};
			Dictionary<string, PropertyOptions> dictionary = new Dictionary<string, PropertyOptions>();
			dictionary["dc:contributor"] = propertyOptions;
			dictionary["dc:language"] = propertyOptions;
			dictionary["dc:publisher"] = propertyOptions;
			dictionary["dc:relation"] = propertyOptions;
			dictionary["dc:subject"] = propertyOptions;
			dictionary["dc:type"] = propertyOptions;
			dictionary["dc:creator"] = propertyOptions2;
			dictionary["dc:date"] = propertyOptions2;
			dictionary["dc:description"] = propertyOptions3;
			dictionary["dc:rights"] = propertyOptions3;
			dictionary["dc:title"] = propertyOptions3;
			XmpNormalizer._dcArrayForms = dictionary;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000A30A File Offset: 0x0000850A
		internal static IXmpMeta Process(XmpMeta xmp, ParseOptions options)
		{
			XmpNode root = xmp.GetRoot();
			XmpNormalizer.TouchUpDataModel(xmp);
			XmpNormalizer.MoveExplicitAliases(root, options);
			XmpNormalizer.TweakOldXmp(root);
			XmpNormalizer.DeleteEmptySchemas(root);
			return xmp;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000A32C File Offset: 0x0000852C
		private static void TweakOldXmp(XmpNode tree)
		{
			if (tree.Name != null && tree.Name.Length >= 36)
			{
				string text = tree.Name.ToLower();
				if (text.StartsWith("uuid:"))
				{
					text = text.Substring(5);
				}
				if (Utils.CheckUuidFormat(text))
				{
					XmpPath xmpPath = XmpPathParser.ExpandXPath("http://ns.adobe.com/xap/1.0/mm/", "InstanceID");
					XmpNode xmpNode = XmpNodeUtils.FindNode(tree, xmpPath, true, null);
					if (xmpNode != null)
					{
						xmpNode.Options = null;
						xmpNode.Value = "uuid:" + text;
						xmpNode.RemoveChildren();
						xmpNode.RemoveQualifiers();
						tree.Name = null;
						return;
					}
					throw new XmpException("Failure creating xmpMM:InstanceID", XmpErrorCode.InternalFailure);
				}
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000A3D4 File Offset: 0x000085D4
		private static void TouchUpDataModel(XmpMeta xmp)
		{
			XmpNodeUtils.FindSchemaNode(xmp.GetRoot(), "http://purl.org/dc/elements/1.1/", true);
			IIterator iterator = xmp.GetRoot().IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				string name = xmpNode.Name;
				if (!(name == "http://purl.org/dc/elements/1.1/"))
				{
					if (!(name == "http://ns.adobe.com/exif/1.0/"))
					{
						if (!(name == "http://ns.adobe.com/xmp/1.0/DynamicMedia/"))
						{
							if (name == "http://ns.adobe.com/xap/1.0/rights/")
							{
								XmpNode xmpNode2 = XmpNodeUtils.FindChildNode(xmpNode, "xmpRights:UsageTerms", false);
								if (xmpNode2 != null)
								{
									XmpNormalizer.RepairAltText(xmpNode2);
								}
							}
						}
						else
						{
							XmpNode xmpNode3 = XmpNodeUtils.FindChildNode(xmpNode, "xmpDM:copyright", false);
							if (xmpNode3 != null)
							{
								XmpNormalizer.MigrateAudioCopyright(xmp, xmpNode3);
							}
						}
					}
					else
					{
						XmpNormalizer.FixGpsTimeStamp(xmpNode);
						XmpNode xmpNode4 = XmpNodeUtils.FindChildNode(xmpNode, "exif:UserComment", false);
						if (xmpNode4 != null)
						{
							if (xmpNode4.Options.IsSimple)
							{
								XmpNode xmpNode5 = new XmpNode("[]", xmpNode4.Value, xmpNode4.Options);
								xmpNode5.Parent = xmpNode4;
								for (int i = xmpNode4.GetQualifierLength(); i > 0; i--)
								{
									xmpNode5.AddQualifier(xmpNode4.GetQualifier(xmpNode4.GetQualifierLength() - i));
								}
								xmpNode4.RemoveQualifiers();
								if (!xmpNode5.Options.HasLanguage)
								{
									PropertyOptions propertyOptions = new PropertyOptions();
									propertyOptions.SetOption(16, true);
									XmpNode xmpNode6 = new XmpNode("xml:lang", "x-default", propertyOptions);
									xmpNode5.AddQualifier(xmpNode6);
									xmpNode5.Options.SetOption(16, true);
									xmpNode5.Options.SetOption(64, true);
								}
								xmpNode4.AddChild(xmpNode5);
								xmpNode4.Options = new PropertyOptions(7680);
								xmpNode4.Value = "";
							}
							XmpNormalizer.RepairAltText(xmpNode4);
						}
					}
				}
				else
				{
					XmpNormalizer.NormalizeDcArrays(xmpNode);
				}
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000A5A0 File Offset: 0x000087A0
		private static void NormalizeDcArrays(XmpNode dcSchema)
		{
			for (int i = 1; i <= dcSchema.GetChildrenLength(); i++)
			{
				XmpNode child = dcSchema.GetChild(i);
				PropertyOptions propertyOptions = (PropertyOptions)XmpNormalizer._dcArrayForms[child.Name];
				if (propertyOptions != null)
				{
					if (child.Options.IsSimple)
					{
						XmpNode xmpNode = new XmpNode(child.Name, propertyOptions);
						child.Name = "[]";
						xmpNode.AddChild(child);
						dcSchema.ReplaceChild(i, xmpNode);
						if (propertyOptions.IsArrayAltText && !child.Options.HasLanguage)
						{
							XmpNode xmpNode2 = new XmpNode("xml:lang", "x-default", null);
							child.AddQualifier(xmpNode2);
						}
					}
					else
					{
						child.Options.SetOption(7680, false);
						child.Options.MergeWith(propertyOptions);
						if (propertyOptions.IsArrayAltText)
						{
							XmpNormalizer.RepairAltText(child);
						}
					}
				}
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000A678 File Offset: 0x00008878
		private static void RepairAltText(XmpNode arrayNode)
		{
			if (arrayNode == null || !arrayNode.Options.IsArray)
			{
				return;
			}
			arrayNode.Options.IsArrayOrdered = true;
			arrayNode.Options.IsArrayAlternate = true;
			arrayNode.Options.IsArrayAltText = true;
			IIterator iterator = arrayNode.IterateChildren();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (xmpNode.Options.IsCompositeProperty)
				{
					iterator.Remove();
				}
				else if (!xmpNode.Options.HasLanguage)
				{
					if (string.IsNullOrEmpty(xmpNode.Value))
					{
						iterator.Remove();
					}
					else
					{
						XmpNode xmpNode2 = new XmpNode("xml:lang", "x-repair", null);
						xmpNode.AddQualifier(xmpNode2);
					}
				}
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000A728 File Offset: 0x00008928
		private static void MoveExplicitAliases(XmpNode tree, ParseOptions options)
		{
			if (!tree.HasAliases)
			{
				return;
			}
			tree.HasAliases = false;
			bool strictAliasing = options.StrictAliasing;
			Iterator<object> iterator = tree.GetUnmodifiableChildren().Iterator<object>();
			while (iterator.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				if (xmpNode.HasAliases)
				{
					IIterator iterator2 = xmpNode.IterateChildren();
					while (iterator2.HasNext())
					{
						XmpNode xmpNode2 = (XmpNode)iterator2.Next();
						if (xmpNode2.IsAlias)
						{
							xmpNode2.IsAlias = false;
							IXmpAliasInfo xmpAliasInfo = XmpMetaFactory.SchemaRegistry.FindAlias(xmpNode2.Name);
							if (xmpAliasInfo != null)
							{
								XmpNode xmpNode3 = XmpNodeUtils.FindSchemaNode(tree, xmpAliasInfo.Namespace, null, true);
								xmpNode3.IsImplicit = false;
								XmpNode xmpNode4 = XmpNodeUtils.FindChildNode(xmpNode3, xmpAliasInfo.Prefix + xmpAliasInfo.PropName, false);
								if (xmpNode4 == null)
								{
									if (xmpAliasInfo.AliasForm.IsSimple())
									{
										string text = xmpAliasInfo.Prefix + xmpAliasInfo.PropName;
										xmpNode2.Name = text;
										xmpNode3.AddChild(xmpNode2);
										iterator2.Remove();
									}
									else
									{
										xmpNode4 = new XmpNode(xmpAliasInfo.Prefix + xmpAliasInfo.PropName, xmpAliasInfo.AliasForm.ToPropertyOptions());
										xmpNode3.AddChild(xmpNode4);
										XmpNormalizer.TransplantArrayItemAlias(iterator2, xmpNode2, xmpNode4);
									}
								}
								else if (xmpAliasInfo.AliasForm.IsSimple())
								{
									if (strictAliasing)
									{
										XmpNormalizer.CompareAliasedSubtrees(xmpNode2, xmpNode4, true);
									}
									iterator2.Remove();
								}
								else
								{
									XmpNode xmpNode5 = null;
									if (xmpAliasInfo.AliasForm.IsArrayAltText)
									{
										int num = XmpNodeUtils.LookupLanguageItem(xmpNode4, "x-default");
										if (num != -1)
										{
											xmpNode5 = xmpNode4.GetChild(num);
										}
									}
									else if (xmpNode4.HasChildren)
									{
										xmpNode5 = xmpNode4.GetChild(1);
									}
									if (xmpNode5 == null)
									{
										XmpNormalizer.TransplantArrayItemAlias(iterator2, xmpNode2, xmpNode4);
									}
									else if (strictAliasing)
									{
										XmpNormalizer.CompareAliasedSubtrees(xmpNode2, xmpNode5, true);
									}
									iterator2.Remove();
								}
							}
						}
					}
					xmpNode.HasAliases = false;
				}
			}
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000A91C File Offset: 0x00008B1C
		private static void TransplantArrayItemAlias(IIterator propertyIt, XmpNode childNode, XmpNode baseArray)
		{
			if (baseArray.Options.IsArrayAltText)
			{
				if (childNode.Options.HasLanguage)
				{
					throw new XmpException("Alias to x-default already has a language qualifier", XmpErrorCode.BadXmp);
				}
				XmpNode xmpNode = new XmpNode("xml:lang", "x-default", null);
				childNode.AddQualifier(xmpNode);
			}
			propertyIt.Remove();
			childNode.Name = "[]";
			baseArray.AddChild(childNode);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000A984 File Offset: 0x00008B84
		private static void FixGpsTimeStamp(XmpNode exifSchema)
		{
			XmpNode xmpNode = XmpNodeUtils.FindChildNode(exifSchema, "exif:GPSTimeStamp", false);
			if (xmpNode == null)
			{
				return;
			}
			try
			{
				IXmpDateTime xmpDateTime = XmpUtils.ConvertToDate(xmpNode.Value);
				if (xmpDateTime.Year == 0 && xmpDateTime.Month == 0 && xmpDateTime.Day == 0)
				{
					XmpNode xmpNode2 = XmpNodeUtils.FindChildNode(exifSchema, "exif:DateTimeOriginal", false) ?? XmpNodeUtils.FindChildNode(exifSchema, "exif:DateTimeDigitized", false);
					if (xmpNode2 != null)
					{
						IXmpDateTime xmpDateTime2 = XmpUtils.ConvertToDate(xmpNode2.Value);
						Calendar calendar = xmpDateTime.Calendar;
						calendar.Set(CalendarEnum.Year, xmpDateTime2.Year);
						calendar.Set(CalendarEnum.Month, xmpDateTime2.Month);
						calendar.Set(CalendarEnum.DayOfMonth, xmpDateTime2.Day);
						xmpDateTime = new XmpDateTime(calendar);
						xmpNode.Value = XmpUtils.ConvertFromDate(xmpDateTime);
					}
				}
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000AA4C File Offset: 0x00008C4C
		private static void DeleteEmptySchemas(XmpNode tree)
		{
			IIterator iterator = tree.IterateChildren();
			while (iterator.HasNext())
			{
				if (!((XmpNode)iterator.Next()).HasChildren)
				{
					iterator.Remove();
				}
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000AA84 File Offset: 0x00008C84
		private static void CompareAliasedSubtrees(XmpNode aliasNode, XmpNode baseNode, bool outerCall)
		{
			if (baseNode.Value != aliasNode.Value || aliasNode.GetChildrenLength() != baseNode.GetChildrenLength())
			{
				throw new XmpException("Mismatch between alias and base nodes", XmpErrorCode.BadXmp);
			}
			if (!outerCall && (baseNode.Name != aliasNode.Name || !aliasNode.Options.Equals(baseNode.Options) || aliasNode.GetQualifierLength() != baseNode.GetQualifierLength()))
			{
				throw new XmpException("Mismatch between alias and base nodes", XmpErrorCode.BadXmp);
			}
			IIterator iterator = aliasNode.IterateChildren();
			IIterator iterator2 = baseNode.IterateChildren();
			while (iterator.HasNext() && iterator2.HasNext())
			{
				XmpNode xmpNode = (XmpNode)iterator.Next();
				XmpNode xmpNode2 = (XmpNode)iterator2.Next();
				XmpNormalizer.CompareAliasedSubtrees(xmpNode, xmpNode2, false);
			}
			IIterator iterator3 = aliasNode.IterateQualifier();
			IIterator iterator4 = baseNode.IterateQualifier();
			while (iterator3.HasNext() && iterator4.HasNext())
			{
				XmpNode xmpNode3 = (XmpNode)iterator3.Next();
				XmpNode xmpNode4 = (XmpNode)iterator4.Next();
				XmpNormalizer.CompareAliasedSubtrees(xmpNode3, xmpNode4, false);
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000AB8C File Offset: 0x00008D8C
		private static void MigrateAudioCopyright(IXmpMeta xmp, XmpNode dmCopyright)
		{
			try
			{
				XmpNode xmpNode = XmpNodeUtils.FindSchemaNode(((XmpMeta)xmp).GetRoot(), "http://purl.org/dc/elements/1.1/", true);
				string text = dmCopyright.Value;
				XmpNode xmpNode2 = XmpNodeUtils.FindChildNode(xmpNode, "dc:rights", false);
				if (xmpNode2 == null || !xmpNode2.HasChildren)
				{
					text = "\n\n" + text;
					xmp.SetLocalizedText("http://purl.org/dc/elements/1.1/", "rights", string.Empty, "x-default", text, null);
				}
				else
				{
					int num = XmpNodeUtils.LookupLanguageItem(xmpNode2, "x-default");
					if (num < 0)
					{
						string value = xmpNode2.GetChild(1).Value;
						xmp.SetLocalizedText("http://purl.org/dc/elements/1.1/", "rights", string.Empty, "x-default", value, null);
						num = XmpNodeUtils.LookupLanguageItem(xmpNode2, "x-default");
					}
					XmpNode child = xmpNode2.GetChild(num);
					string value2 = child.Value;
					int num2 = value2.IndexOf("\n\n");
					if (num2 < 0)
					{
						if (value2 != text)
						{
							child.Value = value2 + "\n\n" + text;
						}
					}
					else if (text != value2.Substring(num2 + 2))
					{
						child.Value = value2.Substring(0, num2 + 2) + text;
					}
				}
				dmCopyright.Parent.RemoveChild(dmCopyright);
			}
			catch (XmpException)
			{
			}
		}

		// Token: 0x0400010F RID: 271
		private static readonly IDictionary _dcArrayForms;
	}
}
