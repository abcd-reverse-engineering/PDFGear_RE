using System;
using System.Windows;
using System.Windows.Controls;

namespace PDFLauncher.Utils
{
	// Token: 0x02000017 RID: 23
	public static class UIElementExtension
	{
		// Token: 0x06000176 RID: 374 RVA: 0x0000662D File Offset: 0x0000482D
		public static void SetExtendContextMenuDataContext(DependencyObject obj, ContextMenu value)
		{
			obj.SetValue(UIElementExtension.ExtendContextMenuDataContextProperty, value);
		}

		// Token: 0x040000BB RID: 187
		public static readonly DependencyProperty ExtendContextMenuDataContextProperty = DependencyProperty.RegisterAttached("ExtendContextMenuDataContext", typeof(ContextMenu), typeof(UIElementExtension), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			FrameworkElement frameworkElement = s as FrameworkElement;
			if (frameworkElement != null)
			{
				ContextMenu contextMenu = a.OldValue as ContextMenu;
				if (contextMenu != null)
				{
					contextMenu.DataContext = null;
				}
				ContextMenu contextMenu2 = a.NewValue as ContextMenu;
				if (contextMenu2 != null)
				{
					contextMenu2.DataContext = frameworkElement;
				}
			}
		}));
	}
}
