using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfconverter.Controls
{
	// Token: 0x020000A8 RID: 168
	public class PageRangeTextBox : TextBox
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0001A4B8 File Offset: 0x000186B8
		private static HashSet<Key> PageRangeKeys
		{
			get
			{
				PageRangeTextBox.InitPageRangeKeys();
				return PageRangeTextBox.pageRangeKeys;
			}
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0001A4C4 File Offset: 0x000186C4
		static PageRangeTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PageRangeTextBox), new FrameworkPropertyMetadata(typeof(PageRangeTextBox)));
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0001A579 File Offset: 0x00018779
		public PageRangeTextBox()
		{
			InputMethod.SetIsInputMethodEnabled(this, false);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x0001A588 File Offset: 0x00018788
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (PageRangeTextBox.PageRangeKeys.Contains(e.Key))
			{
				base.OnPreviewKeyDown(e);
				return;
			}
			e.Handled = true;
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0001A5AB File Offset: 0x000187AB
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Return || e.Key == Key.Escape)
			{
				Keyboard.ClearFocus();
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0001A5CC File Offset: 0x000187CC
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			this.HasError = false;
			this.PageIndexes = Array.Empty<int>();
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0001A5E7 File Offset: 0x000187E7
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			this.UpdateState();
			e.Handled = this.HasError;
			if (e.Handled)
			{
				BindingExpression bindingExpression = BindingOperations.GetBindingExpression(this, TextBox.TextProperty);
				if (bindingExpression != null)
				{
					bindingExpression.UpdateSource();
				}
			}
			base.OnLostFocus(e);
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x0001A620 File Offset: 0x00018820
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (!base.IsFocused)
			{
				this.UpdateState();
			}
			base.OnTextChanged(e);
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0001A638 File Offset: 0x00018838
		private void UpdateState()
		{
			string text = base.Text;
			if (!string.IsNullOrWhiteSpace(text))
			{
				int[] array;
				int num;
				if (PageRangeHelper.TryParsePageRange(text, out array, out num))
				{
					this.HasError = false;
					this.PageIndexes = array;
					return;
				}
				if (num == -1)
				{
					base.SelectAll();
				}
				else
				{
					base.Select(num, text.Length - num);
				}
				this.HasError = true;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0001A691 File Offset: 0x00018891
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x0001A6A3 File Offset: 0x000188A3
		public bool HasError
		{
			get
			{
				return (bool)base.GetValue(PageRangeTextBox.HasErrorProperty);
			}
			protected set
			{
				base.SetValue(PageRangeTextBox.HasErrorPropertyKey, value);
			}
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0001A6B8 File Offset: 0x000188B8
		private static void OnHasErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PageRangeTextBox pageRangeTextBox = d as PageRangeTextBox;
			if (pageRangeTextBox != null)
			{
				EventHandler hasErrorChanged = pageRangeTextBox.HasErrorChanged;
				if (hasErrorChanged == null)
				{
					return;
				}
				hasErrorChanged(pageRangeTextBox, EventArgs.Empty);
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0001A6E5 File Offset: 0x000188E5
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x0001A6F7 File Offset: 0x000188F7
		public IReadOnlyList<int> PageIndexes
		{
			get
			{
				return (IReadOnlyList<int>)base.GetValue(PageRangeTextBox.PageIndexesProperty);
			}
			protected set
			{
				base.SetValue(PageRangeTextBox.PageIndexesPropertyKey, value);
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000741 RID: 1857 RVA: 0x0001A708 File Offset: 0x00018908
		// (remove) Token: 0x06000742 RID: 1858 RVA: 0x0001A740 File Offset: 0x00018940
		public event EventHandler HasErrorChanged;

		// Token: 0x06000743 RID: 1859 RVA: 0x0001A778 File Offset: 0x00018978
		private static void InitPageRangeKeys()
		{
			if (PageRangeTextBox.pageRangeKeys == null)
			{
				Type typeFromHandle = typeof(PageRangeTextBox);
				lock (typeFromHandle)
				{
					if (PageRangeTextBox.pageRangeKeys == null)
					{
						PageRangeTextBox.pageRangeKeys = new HashSet<Key>
						{
							Key.Space,
							Key.Return,
							Key.Escape,
							Key.Left,
							Key.Up,
							Key.Right,
							Key.Down,
							Key.Tab,
							Key.Subtract,
							Key.OemMinus,
							Key.OemComma,
							Key.Back,
							Key.Delete
						};
						for (Key key = Key.D0; key <= Key.D9; key++)
						{
							PageRangeTextBox.pageRangeKeys.Add(key);
						}
						for (Key key2 = Key.NumPad0; key2 <= Key.NumPad9; key2++)
						{
							PageRangeTextBox.pageRangeKeys.Add(key2);
						}
					}
				}
			}
		}

		// Token: 0x04000386 RID: 902
		private static HashSet<Key> pageRangeKeys;

		// Token: 0x04000387 RID: 903
		protected static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly("HasError", typeof(bool), typeof(PageRangeTextBox), new PropertyMetadata(false, new PropertyChangedCallback(PageRangeTextBox.OnHasErrorPropertyChanged)));

		// Token: 0x04000388 RID: 904
		public static readonly DependencyProperty HasErrorProperty = PageRangeTextBox.HasErrorPropertyKey.DependencyProperty;

		// Token: 0x04000389 RID: 905
		protected static readonly DependencyPropertyKey PageIndexesPropertyKey = DependencyProperty.RegisterReadOnly("PageIndexes", typeof(IReadOnlyList<int>), typeof(PageRangeTextBox), new PropertyMetadata(Array.Empty<int>()));

		// Token: 0x0400038A RID: 906
		public static readonly DependencyProperty PageIndexesProperty = PageRangeTextBox.PageIndexesPropertyKey.DependencyProperty;
	}
}
