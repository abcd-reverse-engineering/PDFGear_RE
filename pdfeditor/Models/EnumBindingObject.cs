using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using PDFKit.Utils;

namespace pdfeditor.Models
{
	// Token: 0x02000129 RID: 297
	public class EnumBindingObject<T> : DynamicObject, INotifyPropertyChanged where T : struct, Enum
	{
		// Token: 0x0600123D RID: 4669 RVA: 0x0004A9D4 File Offset: 0x00048BD4
		public EnumBindingObject(T defaultValue)
		{
			if (EnumBindingObject<T>._enumNames == null)
			{
				object obj = EnumBindingObject<T>.locker;
				lock (obj)
				{
					if (EnumBindingObject<T>._enumNames == null)
					{
						EnumBindingObject<T>._enumNames = EnumHelper<T>.NameValueDict.Keys.ToArray<string>();
						EnumBindingObject<T>._enumValues = EnumHelper<T>.NameValueDict.ToDictionary((KeyValuePair<string, T> c) => c.Key, (KeyValuePair<string, T> c) => Convert.ToInt64(c.Value));
					}
				}
			}
			this.defaultValue = defaultValue;
			this.value = defaultValue;
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0004AA94 File Offset: 0x00048C94
		// (set) Token: 0x0600123F RID: 4671 RVA: 0x0004AA9C File Offset: 0x00048C9C
		public T Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (!object.Equals(this.value, value))
				{
					long num = Convert.ToInt64(this.value);
					this.value = value;
					this.OnPropertyChanged("Value");
					this.RaiseValueChanged(num);
					this.RaiseValueChanged(Convert.ToInt64(this.value));
				}
			}
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x0004AB04 File Offset: 0x00048D04
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = null;
			if (binder.Name == "Value" && (binder.ReturnType == typeof(T) || binder.ReturnType == typeof(object)))
			{
				result = this.Value;
				return true;
			}
			long num;
			if ((binder.ReturnType == typeof(bool) || binder.ReturnType == typeof(object)) && EnumBindingObject<T>._enumValues.TryGetValue(binder.Name, out num))
			{
				result = Convert.ToInt64(this.Value) == num;
				return true;
			}
			return false;
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x0004ABC4 File Offset: 0x00048DC4
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (!(binder.Name == "Value"))
			{
				if (value is bool)
				{
					bool flag = (bool)value;
					long num;
					if (EnumBindingObject<T>._enumValues.TryGetValue(binder.Name, out num))
					{
						T t;
						if (Convert.ToInt64(this.Value) == num)
						{
							if (!flag)
							{
								this.Value = this.defaultValue;
							}
						}
						else if (flag && Enum.TryParse<T>(binder.Name, out t))
						{
							this.Value = t;
						}
						return true;
					}
				}
				return false;
			}
			if (value is T)
			{
				T t2 = (T)((object)value);
				Convert.ToInt64(this.Value);
				this.Value = t2;
				return true;
			}
			throw new ArgumentException("value");
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06001242 RID: 4674 RVA: 0x0004AC7C File Offset: 0x00048E7C
		// (remove) Token: 0x06001243 RID: 4675 RVA: 0x0004ACB4 File Offset: 0x00048EB4
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001244 RID: 4676 RVA: 0x0004ACE9 File Offset: 0x00048EE9
		protected void OnPropertyChanged([CallerMemberName] string propName = "")
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0004AD04 File Offset: 0x00048F04
		protected void RaiseValueChanged(long value)
		{
			foreach (KeyValuePair<string, long> keyValuePair in EnumBindingObject<T>._enumValues)
			{
				if (keyValuePair.Value == value)
				{
					this.OnPropertyChanged(keyValuePair.Key);
				}
			}
		}

		// Token: 0x040005C7 RID: 1479
		private static IReadOnlyCollection<string> _enumNames;

		// Token: 0x040005C8 RID: 1480
		private static object locker = new object();

		// Token: 0x040005C9 RID: 1481
		private static Dictionary<string, long> _enumValues;

		// Token: 0x040005CA RID: 1482
		private T value;

		// Token: 0x040005CB RID: 1483
		private readonly T defaultValue;
	}
}
