using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Controls;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000253 RID: 595
	public partial class PagesView : UserControl
	{
		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x0600226C RID: 8812 RVA: 0x000A20D4 File Offset: 0x000A02D4
		// (set) Token: 0x0600226D RID: 8813 RVA: 0x000A20E6 File Offset: 0x000A02E6
		public IList SelectedItems
		{
			get
			{
				return (IList)base.GetValue(PagesView.SelectedItemsProperty);
			}
			set
			{
				base.SetValue(PagesView.SelectedItemsProperty, value);
			}
		}

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x0600226E RID: 8814 RVA: 0x000A20F4 File Offset: 0x000A02F4
		// (set) Token: 0x0600226F RID: 8815 RVA: 0x000A2106 File Offset: 0x000A0306
		public double ItemWidth
		{
			get
			{
				return (double)base.GetValue(PagesView.ItemWidthProperty);
			}
			set
			{
				base.SetValue(PagesView.ItemWidthProperty, value);
			}
		}

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06002270 RID: 8816 RVA: 0x000A2119 File Offset: 0x000A0319
		// (set) Token: 0x06002271 RID: 8817 RVA: 0x000A212B File Offset: 0x000A032B
		public double ItemHeight
		{
			get
			{
				return (double)base.GetValue(PagesView.ItemHeightProperty);
			}
			set
			{
				base.SetValue(PagesView.ItemHeightProperty, value);
			}
		}

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x06002272 RID: 8818 RVA: 0x000A213E File Offset: 0x000A033E
		// (set) Token: 0x06002273 RID: 8819 RVA: 0x000A2150 File Offset: 0x000A0350
		public string[] DragFiles
		{
			get
			{
				return (string[])base.GetValue(PagesView.DragFilesProperty);
			}
			set
			{
				base.SetValue(PagesView.DragFilesProperty, value);
			}
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x000A2160 File Offset: 0x000A0360
		public PagesView()
		{
			this.InitializeComponent();
			base.SetBinding(PagesView.SelectedItemsProperty, new Binding("SelectedItems")
			{
				Mode = BindingMode.TwoWay
			});
			base.SetBinding(PagesView.DragFilesProperty, new Binding("DragFiles")
			{
				Mode = BindingMode.OneWayToSource
			});
			base.Loaded += delegate(object s, RoutedEventArgs e)
			{
				this.SelectedItems = this.PageListBox.SelectedItems;
			};
			((INotifyCollectionChanged)this.PageListBox.Items).CollectionChanged += this.Collection_CollectionChanged;
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x000A21E4 File Offset: 0x000A03E4
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			if (!this.PageListBox.HasItems)
			{
				return;
			}
			if ((ModifierKeys.Control & Keyboard.Modifiers) == ModifierKeys.None)
			{
				return;
			}
			if (e.Delta > 0)
			{
				if (this.ItemWidth >= 300.0)
				{
					return;
				}
				this.ItemWidth += 20.0;
			}
			else
			{
				if (this.ItemWidth <= 100.0)
				{
					return;
				}
				this.ItemWidth -= 20.0;
			}
			this.ItemHeight = Math.Round(this.ItemWidth * 1.414) + 30.0;
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000A2290 File Offset: 0x000A0490
		private void PageListBox_DragEnter(object sender, DragEventArgs e)
		{
			string[] array = e.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (array != null)
			{
				List<string> list = new List<string>();
				foreach (string text in array)
				{
					string extension = Path.GetExtension(text);
					if (!string.IsNullOrEmpty(extension) && (PagesView.PdfExtensions.Contains(extension) || PagesView.ImageExtensions.Contains(extension)))
					{
						list.Add(text);
					}
				}
				this.DragFiles = ((list.Count > 0) ? list.ToArray() : null);
				return;
			}
			this.DragFiles = null;
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000A2328 File Offset: 0x000A0528
		private void PageListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.PageListBox.SelectedItems.Count > 0 && e.OriginalSource is ScrollViewer)
			{
				this.PageListBox.UnselectAll();
			}
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x000A2358 File Offset: 0x000A0558
		private void PageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object obj = null;
			if (e.AddedItems.Count > 0)
			{
				obj = e.AddedItems[e.AddedItems.Count - 1];
			}
			else if (e.RemovedItems.Count > 0)
			{
				obj = e.RemovedItems[e.RemovedItems.Count - 1];
			}
			if (obj != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				this.SelectWithShift(obj);
			}
			else
			{
				this.lastItem = obj;
			}
			if (this.PageListBox.SelectedItems.Count == 0)
			{
				this.lastItem = null;
			}
			this.SelectedItems = this.PageListBox.SelectedItems;
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000A2408 File Offset: 0x000A0608
		private void SelectWithShift(object currentItem)
		{
			int num = this.PageListBox.Items.IndexOf(currentItem);
			if (num == -1)
			{
				return;
			}
			int num2 = 0;
			if (this.lastItem != null)
			{
				int num3 = this.PageListBox.Items.IndexOf(this.lastItem);
				if (num3 > 0)
				{
					num2 = num3;
				}
			}
			if (num < num2)
			{
				int num4 = num;
				num = num2;
				num2 = num4;
			}
			try
			{
				this.PageListBox.SelectionChanged -= this.PageListBox_SelectionChanged;
				List<object> list = this.PageListBox.SelectedItems.OfType<object>().ToList<object>();
				for (int i = num2; i <= num; i++)
				{
					ListBoxItem listBoxItem = this.PageListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
					if (listBoxItem != null)
					{
						if (!listBoxItem.IsSelected)
						{
							listBoxItem.IsSelected = true;
						}
						list.Remove(this.PageListBox.Items[i]);
					}
				}
				foreach (object obj in list)
				{
					ListBoxItem listBoxItem2 = this.PageListBox.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
					if (listBoxItem2 != null && listBoxItem2.IsSelected)
					{
						listBoxItem2.IsSelected = false;
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				this.PageListBox.SelectionChanged += this.PageListBox_SelectionChanged;
			}
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000A2580 File Offset: 0x000A0780
		private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				this.PageListBox.ScrollIntoView(this.PageListBox.Items[this.PageListBox.Items.Count - 1]);
			}
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x000A25B7 File Offset: 0x000A07B7
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			InsertFromScannerViewModel insertFromScannerViewModel = base.DataContext as InsertFromScannerViewModel;
			insertFromScannerViewModel.cts.Cancel();
			insertFromScannerViewModel.IsEnabled = false;
		}

		// Token: 0x04000E9A RID: 3738
		private const double aspectRatio = 1.414;

		// Token: 0x04000E9B RID: 3739
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(PagesView), new PropertyMetadata(null));

		// Token: 0x04000E9C RID: 3740
		public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(PagesView), new PropertyMetadata(150.0));

		// Token: 0x04000E9D RID: 3741
		public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(PagesView), new PropertyMetadata(Math.Round(242.1)));

		// Token: 0x04000E9E RID: 3742
		public static readonly DependencyProperty DragFilesProperty = DependencyProperty.Register("DragFiles", typeof(string[]), typeof(PagesView), new PropertyMetadata(null));

		// Token: 0x04000E9F RID: 3743
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };

		// Token: 0x04000EA0 RID: 3744
		public static readonly string[] PdfExtensions = new string[] { ".pdf" };

		// Token: 0x04000EA1 RID: 3745
		private object lastItem;
	}
}
