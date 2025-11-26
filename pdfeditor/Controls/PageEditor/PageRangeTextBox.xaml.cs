using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfeditor.Utils;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024F RID: 591
	public partial class PageRangeTextBox : TextBox
	{
		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x06002224 RID: 8740 RVA: 0x0009E2CD File Offset: 0x0009C4CD
		private static HashSet<Key> PageRangeKeys
		{
			get
			{
				PageRangeTextBox.InitPageRangeKeys();
				return PageRangeTextBox.pageRangeKeys;
			}
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x0009E2DC File Offset: 0x0009C4DC
		static PageRangeTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PageRangeTextBox), new FrameworkPropertyMetadata(typeof(PageRangeTextBox)));
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x0009E391 File Offset: 0x0009C591
		public PageRangeTextBox()
		{
			InputMethod.SetIsInputMethodEnabled(this, false);
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x0009E3A0 File Offset: 0x0009C5A0
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (PageRangeTextBox.PageRangeKeys.Contains(e.Key))
			{
				base.OnPreviewKeyDown(e);
				return;
			}
			e.Handled = true;
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x0009E3C3 File Offset: 0x0009C5C3
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Return || e.Key == Key.Escape)
			{
				Keyboard.ClearFocus();
			}
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x0009E3E4 File Offset: 0x0009C5E4
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			this.HasError = false;
			this.PageIndexes = Array.Empty<int>();
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x0009E3FF File Offset: 0x0009C5FF
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			this.UpdateState();
			e.Handled = this.HasError;
			base.OnLostFocus(e);
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x0009E41A File Offset: 0x0009C61A
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (!base.IsFocused)
			{
				this.UpdateState();
			}
			base.OnTextChanged(e);
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x0009E434 File Offset: 0x0009C634
		private void UpdateState()
		{
			string text = base.Text;
			if (!string.IsNullOrWhiteSpace(text))
			{
				int[] array;
				int num;
				if (PdfObjectExtensions.TryParsePageRange(text, out array, out num))
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

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x0600222D RID: 8749 RVA: 0x0009E48D File Offset: 0x0009C68D
		// (set) Token: 0x0600222E RID: 8750 RVA: 0x0009E49F File Offset: 0x0009C69F
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

		// Token: 0x0600222F RID: 8751 RVA: 0x0009E4B4 File Offset: 0x0009C6B4
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

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x06002230 RID: 8752 RVA: 0x0009E4E1 File Offset: 0x0009C6E1
		// (set) Token: 0x06002231 RID: 8753 RVA: 0x0009E4F3 File Offset: 0x0009C6F3
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

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06002232 RID: 8754 RVA: 0x0009E504 File Offset: 0x0009C704
		// (remove) Token: 0x06002233 RID: 8755 RVA: 0x0009E53C File Offset: 0x0009C73C
		public event EventHandler HasErrorChanged;

		// Token: 0x06002234 RID: 8756 RVA: 0x0009E574 File Offset: 0x0009C774
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

		// Token: 0x04000E5B RID: 3675
		private static HashSet<Key> pageRangeKeys;

		// Token: 0x04000E5C RID: 3676
		protected static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly("HasError", typeof(bool), typeof(PageRangeTextBox), new PropertyMetadata(false, new PropertyChangedCallback(PageRangeTextBox.OnHasErrorPropertyChanged)));

		// Token: 0x04000E5D RID: 3677
		public static readonly DependencyProperty HasErrorProperty = PageRangeTextBox.HasErrorPropertyKey.DependencyProperty;

		// Token: 0x04000E5E RID: 3678
		protected static readonly DependencyPropertyKey PageIndexesPropertyKey = DependencyProperty.RegisterReadOnly("PageIndexes", typeof(IReadOnlyList<int>), typeof(PageRangeTextBox), new PropertyMetadata(Array.Empty<int>()));

		// Token: 0x04000E5F RID: 3679
		public static readonly DependencyProperty PageIndexesProperty = PageRangeTextBox.PageIndexesPropertyKey.DependencyProperty;
	}
}
