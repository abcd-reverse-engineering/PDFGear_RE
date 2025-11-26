using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B4 RID: 180
	public static class EnumHelper
	{
		// Token: 0x06000AE9 RID: 2793 RVA: 0x00038995 File Offset: 0x00036B95
		public static IEnumerable<KeyValuePair<int, string>> GetValueDescriptionList<T>() where T : Enum
		{
			return new EnumHelper.<GetValueDescriptionList>d__0<T>(-2);
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x0003899E File Offset: 0x00036B9E
		public static T Get<T>(Enum enumName) where T : Attribute
		{
			return EnumHelper.GetAttributes<T>(enumName.GetType().GetField(enumName.ToString())).FirstOrDefault<T>();
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x000389BB File Offset: 0x00036BBB
		public static string GetDescription(Enum enumName)
		{
			DescriptionAttribute descriptionAttribute = EnumHelper.Get<DescriptionAttribute>(enumName);
			if (descriptionAttribute == null)
			{
				return null;
			}
			return descriptionAttribute.Description;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x000389D0 File Offset: 0x00036BD0
		public static Dictionary<TEnum, string> GetDescriptionDictionary<TEnum>() where TEnum : Enum
		{
			return EnumHelper.GetAttributeDictionary<TEnum, DescriptionAttribute>().ToDictionary((KeyValuePair<TEnum, DescriptionAttribute> x) => x.Key, delegate(KeyValuePair<TEnum, DescriptionAttribute> x)
			{
				DescriptionAttribute value = x.Value;
				if (value == null)
				{
					return null;
				}
				return value.Description;
			});
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x00038A28 File Offset: 0x00036C28
		public static Dictionary<TEnum, TAttribute> GetAttributeDictionary<TEnum, TAttribute>() where TEnum : Enum where TAttribute : Attribute
		{
			Type typeFromHandle = typeof(TEnum);
			Type typeFromHandle2 = typeof(TAttribute);
			Dictionary<TEnum, TAttribute> dictionary = new Dictionary<TEnum, TAttribute>();
			foreach (FieldInfo fieldInfo in typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				TEnum tenum = (TEnum)((object)fieldInfo.GetValue(typeFromHandle));
				TAttribute tattribute = fieldInfo.GetCustomAttribute(typeFromHandle2, false) as TAttribute;
				dictionary.Add(tenum, tattribute);
			}
			return dictionary;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x00038A9C File Offset: 0x00036C9C
		public static IEnumerable<KeyValuePair<T, string>> GetNameAndDescriptionList<T>() where T : Enum
		{
			return new EnumHelper.<GetNameAndDescriptionList>d__5<T>(-2);
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00038AA5 File Offset: 0x00036CA5
		private static IEnumerable<T> GetAttributes<T>(FieldInfo fieldInfo) where T : Attribute
		{
			return fieldInfo.GetCustomAttributes(typeof(T), false).Cast<T>();
		}
	}
}
