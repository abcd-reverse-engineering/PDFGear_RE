using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.PageContents
{
	// Token: 0x02000256 RID: 598
	public partial class TextObjectEditDialog : Window
	{
		// Token: 0x060022A2 RID: 8866 RVA: 0x000A3737 File Offset: 0x000A1937
		public TextObjectEditDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x060022A3 RID: 8867 RVA: 0x000A3745 File Offset: 0x000A1945
		// (set) Token: 0x060022A4 RID: 8868 RVA: 0x000A3757 File Offset: 0x000A1957
		public string Text
		{
			get
			{
				return (string)base.GetValue(TextObjectEditDialog.TextProperty);
			}
			set
			{
				base.SetValue(TextObjectEditDialog.TextProperty, value);
			}
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x000A3765 File Offset: 0x000A1965
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x04000EBD RID: 3773
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextObjectEditDialog), new PropertyMetadata(""));
	}
}
