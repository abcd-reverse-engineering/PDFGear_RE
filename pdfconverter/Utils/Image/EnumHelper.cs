using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace pdfconverter.Utils.Image
{
	// Token: 0x02000048 RID: 72
	public static class EnumHelper
	{
		// Token: 0x06000529 RID: 1321 RVA: 0x000153DB File Offset: 0x000135DB
		public static IEnumerable<KeyValuePair<int, string>> GetValueDescriptionList<T>() where T : Enum
		{
			return new EnumHelper.<GetValueDescriptionList>d__0<T>(-2);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x000153E4 File Offset: 0x000135E4
		public static T Get<T>(Enum enumName) where T : Attribute
		{
			return EnumHelper.GetAttributes<T>(enumName.GetType().GetField(enumName.ToString())).FirstOrDefault<T>();
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00015401 File Offset: 0x00013601
		public static string GetDescription(Enum enumName)
		{
			DescriptionAttribute descriptionAttribute = EnumHelper.Get<DescriptionAttribute>(enumName);
			if (descriptionAttribute == null)
			{
				return null;
			}
			return descriptionAttribute.Description;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00015414 File Offset: 0x00013614
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

		// Token: 0x0600052D RID: 1325 RVA: 0x0001546C File Offset: 0x0001366C
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

		// Token: 0x0600052E RID: 1326 RVA: 0x000154E0 File Offset: 0x000136E0
		public static IEnumerable<KeyValuePair<T, string>> GetNameAndDescriptionList<T>() where T : Enum
		{
			return new EnumHelper.<GetNameAndDescriptionList>d__5<T>(-2);
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x000154E9 File Offset: 0x000136E9
		private static IEnumerable<T> GetAttributes<T>(FieldInfo fieldInfo) where T : Attribute
		{
			return fieldInfo.GetCustomAttributes(typeof(T), false).Cast<T>();
		}
	}
}
