using System;
using System.Windows;
using CommonLib.Common;
using pdfeditor.Models.LeftNavigations;

namespace pdfeditor.Controls
{
	// Token: 0x020001D5 RID: 469
	public class NavigationViewItemClickEventArgs : RoutedEventArgs
	{
		// Token: 0x06001A8D RID: 6797 RVA: 0x0006A7B4 File Offset: 0x000689B4
		public NavigationViewItemClickEventArgs(object clickedItem, RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
			this.ClickedItem = clickedItem;
			NavigationModel navigationModel = clickedItem as NavigationModel;
			if (navigationModel != null)
			{
				GAManager.SendEvent("MainWindow", navigationModel.Name, "Count", 1L);
				Log.WriteLog("LeftNavigationViewItem: " + navigationModel.Name);
			}
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06001A8E RID: 6798 RVA: 0x0006A806 File Offset: 0x00068A06
		public object ClickedItem { get; }
	}
}
