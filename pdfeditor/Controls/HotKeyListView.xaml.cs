using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common.HotKeys;

namespace pdfeditor.Controls
{
	// Token: 0x020001BA RID: 442
	public partial class HotKeyListView : UserControl
	{
		// Token: 0x06001922 RID: 6434 RVA: 0x00061770 File Offset: 0x0005F970
		public HotKeyListView()
		{
			this.InitializeComponent();
			this.itemsControl.ItemsSource = (from c in HotKeyManager.Names
				select HotKeyManager.GetOrCreate(c) into c
				where c.IsVisible
				select c).ToArray<HotKeyModel>();
		}
	}
}
