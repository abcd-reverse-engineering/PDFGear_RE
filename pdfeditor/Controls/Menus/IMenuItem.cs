using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200026B RID: 619
	public interface IMenuItem
	{
		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06002402 RID: 9218
		// (set) Token: 0x06002403 RID: 9219
		ImageSource ImageUrl { get; set; }

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06002404 RID: 9220
		// (set) Token: 0x06002405 RID: 9221
		string Caption { get; set; }

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06002406 RID: 9222
		// (set) Token: 0x06002407 RID: 9223
		bool IsBeginGroup { get; set; }

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06002408 RID: 9224
		// (set) Token: 0x06002409 RID: 9225
		bool IsEnable { get; set; }

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x0600240A RID: 9226
		// (set) Token: 0x0600240B RID: 9227
		bool IsVisible { get; set; }

		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x0600240C RID: 9228
		// (set) Token: 0x0600240D RID: 9229
		List<IMenuItem> SubMenus { get; set; }

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x0600240E RID: 9230
		// (set) Token: 0x0600240F RID: 9231
		int level { get; set; }

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06002410 RID: 9232
		// (set) Token: 0x06002411 RID: 9233
		bool IsChecked { get; set; }

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06002412 RID: 9234
		// (set) Token: 0x06002413 RID: 9235
		TagDataModel TagData { get; set; }
	}
}
