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
using pdfconverter.ViewModels;

namespace pdfconverter.Controls
{
	// Token: 0x020000A9 RID: 169
	public partial class PagesView : UserControl
	{
		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0001A880 File Offset: 0x00018A80
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0001A892 File Offset: 0x00018A92
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

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0001A8A0 File Offset: 0x00018AA0
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0001A8B2 File Offset: 0x00018AB2
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

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x0001A8C5 File Offset: 0x00018AC5
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x0001A8D7 File Offset: 0x00018AD7
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

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x0001A8EA File Offset: 0x00018AEA
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x0001A8FC File Offset: 0x00018AFC
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

		// Token: 0x0600074C RID: 1868 RVA: 0x0001A90C File Offset: 0x00018B0C
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

		// Token: 0x0600074D RID: 1869 RVA: 0x0001A990 File Offset: 0x00018B90
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

		// Token: 0x0600074E RID: 1870 RVA: 0x0001AA3C File Offset: 0x00018C3C
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

		// Token: 0x0600074F RID: 1871 RVA: 0x0001AAD4 File Offset: 0x00018CD4
		private void PageListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.PageListBox.SelectedItems.Count > 0 && e.OriginalSource is ScrollViewer)
			{
				this.PageListBox.UnselectAll();
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0001AB04 File Offset: 0x00018D04
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

		// Token: 0x06000751 RID: 1873 RVA: 0x0001ABB4 File Offset: 0x00018DB4
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

		// Token: 0x06000752 RID: 1874 RVA: 0x0001AD2C File Offset: 0x00018F2C
		private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				this.PageListBox.ScrollIntoView(this.PageListBox.Items[this.PageListBox.Items.Count - 1]);
			}
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0001AD63 File Offset: 0x00018F63
		private void Button_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0001AD65 File Offset: 0x00018F65
		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			(base.DataContext as ImageToPDFViewModel).ImportCommand.Execute(this);
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x0001AD80 File Offset: 0x00018F80
		private void Grid_Drop(object sender, DragEventArgs e)
		{
			string[] array = e.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (array != null)
			{
				List<string> list = new List<string>();
				foreach (string text in array)
				{
					string extension = Path.GetExtension(text);
					string text2 = ((extension != null) ? extension.ToLowerInvariant() : null);
					if (!string.IsNullOrEmpty(text2) && (PagesView.PdfExtensions.Contains(text2) || PagesView.ImageExtensions.Contains(text2)))
					{
						list.Add(text);
					}
				}
				(base.DataContext as ImageToPDFViewModel).ImportFormGirdDrag((list.Count > 0) ? list.ToArray() : null);
				return;
			}
		}

		// Token: 0x0400038C RID: 908
		private const double aspectRatio = 1.414;

		// Token: 0x0400038D RID: 909
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(PagesView), new PropertyMetadata(null));

		// Token: 0x0400038E RID: 910
		public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(PagesView), new PropertyMetadata(150.0));

		// Token: 0x0400038F RID: 911
		public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(PagesView), new PropertyMetadata(Math.Round(242.1)));

		// Token: 0x04000390 RID: 912
		public static readonly DependencyProperty DragFilesProperty = DependencyProperty.Register("DragFiles", typeof(string[]), typeof(PagesView), new PropertyMetadata(null));

		// Token: 0x04000391 RID: 913
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };

		// Token: 0x04000392 RID: 914
		public static readonly string[] PdfExtensions = new string[] { ".pdf" };

		// Token: 0x04000393 RID: 915
		private object lastItem;
	}
}
