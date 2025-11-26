using System;
using System.Windows;
using pdfeditor.Controls.Annotations;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001EE RID: 494
	public class StampPickerItemClickEventArgs : RoutedEventArgs
	{
		// Token: 0x06001C02 RID: 7170 RVA: 0x000741C6 File Offset: 0x000723C6
		public StampPickerItemClickEventArgs(object source, ImageStampModel item)
			: base(StampPicker.ItemClickEvent, source)
		{
			if (item == null)
			{
				throw new ArgumentException("item");
			}
			this.Item = item;
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x000741EA File Offset: 0x000723EA
		public ImageStampModel Item { get; }
	}
}
