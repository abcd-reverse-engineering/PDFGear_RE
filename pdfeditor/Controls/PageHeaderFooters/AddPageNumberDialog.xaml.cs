using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Controls;
using Patagames.Pdf.Net;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x02000240 RID: 576
	public partial class AddPageNumberDialog : Window
	{
		// Token: 0x060020D6 RID: 8406 RVA: 0x00096794 File Offset: 0x00094994
		public AddPageNumberDialog(PdfDocument pdfDocument, string format, int offset)
		{
			this.InitializeComponent();
			base.Loaded += this.PageNumberDialog_Loaded;
			this.pdfDocument = pdfDocument;
			this.PageNumberOffsetBox.Maximum = 2147483647.0;
			this.PageNumberOffsetBox.Value = (double)offset;
			this.initFormat = format;
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x000967F0 File Offset: 0x000949F0
		private void PageNumberDialog_Loaded(object sender, RoutedEventArgs e)
		{
			this.StyleComboBox.ItemsSource = PagePlaceholderFormatter.AllSupportedPageNumberFormats.ToList<string>();
			if (string.IsNullOrEmpty(this.initFormat) || !PagePlaceholderFormatter.AllSupportedPageNumberFormats.Contains(this.initFormat))
			{
				this.StyleComboBox.SelectedIndex = 0;
				return;
			}
			this.StyleComboBox.SelectedItem = this.initFormat;
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x00096850 File Offset: 0x00094A50
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (base.DialogResult.GetValueOrDefault())
			{
				this.PageNumberFormats = (string)this.StyleComboBox.SelectedItem;
				this.PageNumberOffset = (int)(this.PageNumberOffsetBox.Value + 0.5);
			}
		}

		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x060020D9 RID: 8409 RVA: 0x000968A6 File Offset: 0x00094AA6
		// (set) Token: 0x060020DA RID: 8410 RVA: 0x000968AE File Offset: 0x00094AAE
		public string PageNumberFormats { get; private set; }

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x060020DB RID: 8411 RVA: 0x000968B7 File Offset: 0x00094AB7
		// (set) Token: 0x060020DC RID: 8412 RVA: 0x000968BF File Offset: 0x00094ABF
		public int PageNumberOffset { get; private set; }

		// Token: 0x060020DD RID: 8413 RVA: 0x000968C8 File Offset: 0x00094AC8
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Keyboard.ClearFocus();
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				base.DialogResult = new bool?(true);
			}));
		}

		// Token: 0x04000D52 RID: 3410
		private readonly PdfDocument pdfDocument;

		// Token: 0x04000D53 RID: 3411
		private readonly string initFormat;
	}
}
