using System;
using System.Windows;
using pdfeditor.Controls.Annotations;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001FC RID: 508
	public class SignaturePickerItemClickEventArgs : RoutedEventArgs
	{
		// Token: 0x06001C87 RID: 7303 RVA: 0x00077316 File Offset: 0x00075516
		public SignaturePickerItemClickEventArgs(object source, ImageStampModel item)
			: base(SignaturePicker.ItemClickEvent, source)
		{
			if (item == null)
			{
				throw new ArgumentException("item");
			}
			this.Item = item;
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06001C88 RID: 7304 RVA: 0x0007733A File Offset: 0x0007553A
		public ImageStampModel Item { get; }
	}
}
