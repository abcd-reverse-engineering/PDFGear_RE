using System;
using System.Globalization;
using System.Windows.Data;
using CommonLib.Common.HotKeys;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200011E RID: 286
	internal class BooleanToHotkeyInvokeActionConverter : IValueConverter
	{
		// Token: 0x06000CE7 RID: 3303 RVA: 0x00041964 File Offset: 0x0003FB64
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			HotKeyInvokeAction hotKeyInvokeAction = HotKeyInvokeAction.None;
			if (value is bool && (bool)value)
			{
				return hotKeyInvokeAction;
			}
			if (string.IsNullOrEmpty((string)parameter))
			{
				hotKeyInvokeAction = HotKeyInvokeAction.Invoke;
			}
			else if (!Enum.TryParse<HotKeyInvokeAction>((string)parameter, true, out hotKeyInvokeAction))
			{
				hotKeyInvokeAction = HotKeyInvokeAction.None;
			}
			return hotKeyInvokeAction;
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000419B3 File Offset: 0x0003FBB3
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
