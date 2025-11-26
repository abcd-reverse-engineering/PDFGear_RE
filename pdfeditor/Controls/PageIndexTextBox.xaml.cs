using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using pdfeditor.Utils.Behaviors;

namespace pdfeditor.Controls
{
	// Token: 0x020001BD RID: 445
	public partial class PageIndexTextBox : Control
	{
		// Token: 0x06001963 RID: 6499 RVA: 0x00064B60 File Offset: 0x00062D60
		static PageIndexTextBox()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PageIndexTextBox), new FrameworkPropertyMetadata(typeof(PageIndexTextBox)));
		}

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x06001964 RID: 6500 RVA: 0x00064C0C File Offset: 0x00062E0C
		// (set) Token: 0x06001965 RID: 6501 RVA: 0x00064C14 File Offset: 0x00062E14
		private TextBoxEditBehavior PageIndexTextBoxBehavior
		{
			get
			{
				return this.pageIndexTextBoxBehavior;
			}
			set
			{
				if (this.pageIndexTextBoxBehavior != value)
				{
					if (this.pageIndexTextBoxBehavior != null)
					{
						this.pageIndexTextBoxBehavior.TextChanged -= this.PageIndexTextBoxBehavior_TextChanged;
					}
					this.pageIndexTextBoxBehavior = value;
					if (this.pageIndexTextBoxBehavior != null)
					{
						this.pageIndexTextBoxBehavior.TextChanged += this.PageIndexTextBoxBehavior_TextChanged;
					}
					this.UpdatePageIndex();
				}
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06001966 RID: 6502 RVA: 0x00064C75 File Offset: 0x00062E75
		// (set) Token: 0x06001967 RID: 6503 RVA: 0x00064C80 File Offset: 0x00062E80
		private TextBlock PageCountTextBlock
		{
			get
			{
				return this.pageCountTextBlock;
			}
			set
			{
				if (this.pageCountTextBlock != value)
				{
					if (this.pageCountTextBlock != null)
					{
						this.pageCountTextBlock.SizeChanged += this.PageCountTextBlock_SizeChanged;
					}
					this.pageCountTextBlock = value;
					if (this.pageCountTextBlock != null)
					{
						this.pageCountTextBlock.SizeChanged += this.PageCountTextBlock_SizeChanged;
					}
					this.UpdateTextBoxSize();
				}
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x00064CE9 File Offset: 0x00062EE9
		// (set) Token: 0x0600196A RID: 6506 RVA: 0x00064CFB File Offset: 0x00062EFB
		public int PageIndex
		{
			get
			{
				return (int)base.GetValue(PageIndexTextBox.PageIndexProperty);
			}
			set
			{
				base.SetValue(PageIndexTextBox.PageIndexProperty, value);
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x0600196B RID: 6507 RVA: 0x00064D0E File Offset: 0x00062F0E
		// (set) Token: 0x0600196C RID: 6508 RVA: 0x00064D20 File Offset: 0x00062F20
		public int PageCount
		{
			get
			{
				return (int)base.GetValue(PageIndexTextBox.PageCountProperty);
			}
			set
			{
				base.SetValue(PageIndexTextBox.PageCountProperty, value);
			}
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x00064D34 File Offset: 0x00062F34
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.PageCountRun = base.GetTemplateChild("PageCountRun") as Run;
			this.PageIndexTextBoxBehavior = base.GetTemplateChild("PageIndexTextBoxBehavior") as TextBoxEditBehavior;
			this._PageIndexTextBox = base.GetTemplateChild("PageIndexTextBox") as TextBox;
			this.PageCountTextBlock = base.GetTemplateChild("PageCountTextBlock") as TextBlock;
			this.UpdatePageCount();
			this.UpdateTextBoxSize();
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x00064DAC File Offset: 0x00062FAC
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (this._PageIndexTextBox != null && e.Source != this._PageIndexTextBox)
			{
				this._PageIndexTextBox.Focus();
				Keyboard.Focus(this._PageIndexTextBox);
				this._PageIndexTextBox.SelectAll();
			}
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x00064DFC File Offset: 0x00062FFC
		private void PageIndexTextBoxBehavior_TextChanged(object sender, EventArgs e)
		{
			int num;
			if (int.TryParse(this.PageIndexTextBoxBehavior.Text, out num))
			{
				this.PageIndex = num;
			}
			this.UpdatePageIndex();
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x00064E2A File Offset: 0x0006302A
		private void PageCountTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateTextBoxSize();
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x00064E32 File Offset: 0x00063032
		private void UpdatePageCount()
		{
			if (this.PageCountRun != null)
			{
				this.PageCountRun.Text = string.Format("{0}", this.PageCount);
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x00064E5C File Offset: 0x0006305C
		private void UpdatePageIndex()
		{
			if (this.PageIndexTextBoxBehavior != null)
			{
				this.PageIndexTextBoxBehavior.Text = string.Format("{0}", this.PageIndex);
			}
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x00064E86 File Offset: 0x00063086
		private void UpdateTextBoxSize()
		{
			if (this.PageCountTextBlock != null && this._PageIndexTextBox != null)
			{
				this._PageIndexTextBox.MaxWidth = this.PageCountTextBlock.ActualWidth;
			}
		}

		// Token: 0x040008BB RID: 2235
		private Run PageCountRun;

		// Token: 0x040008BC RID: 2236
		private TextBoxEditBehavior pageIndexTextBoxBehavior;

		// Token: 0x040008BD RID: 2237
		private TextBox _PageIndexTextBox;

		// Token: 0x040008BE RID: 2238
		private TextBlock pageCountTextBlock;

		// Token: 0x040008BF RID: 2239
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(PageIndexTextBox), new PropertyMetadata(0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PageIndexTextBox pageIndexTextBox = s as PageIndexTextBox;
			if (pageIndexTextBox != null && !object.Equals(a.NewValue, a.OldValue))
			{
				pageIndexTextBox.UpdatePageIndex();
			}
		}));

		// Token: 0x040008C0 RID: 2240
		public static readonly DependencyProperty PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(PageIndexTextBox), new PropertyMetadata(0, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PageIndexTextBox pageIndexTextBox2 = s as PageIndexTextBox;
			if (pageIndexTextBox2 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				pageIndexTextBox2.UpdatePageCount();
			}
		}));
	}
}
