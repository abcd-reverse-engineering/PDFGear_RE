using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Patagames.Pdf.Enums;

namespace pdfeditor.Controls.Watermark
{
	// Token: 0x020001E1 RID: 481
	public partial class UIElementAligent : UserControl
	{
		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06001B30 RID: 6960 RVA: 0x0006DA67 File Offset: 0x0006BC67
		// (set) Token: 0x06001B31 RID: 6961 RVA: 0x0006DA6F File Offset: 0x0006BC6F
		public PdfContentAlignment Alignment { get; private set; }

		// Token: 0x06001B32 RID: 6962 RVA: 0x0006DA78 File Offset: 0x0006BC78
		public UIElementAligent()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x0006DA88 File Offset: 0x0006BC88
		private void rbox_Checked(object sender, RoutedEventArgs e)
		{
			string name = (sender as RadioButton).Name;
			if (name != null)
			{
				int length = name.Length;
				if (length == 5)
				{
					switch (name[4])
					{
					case '0':
						if (!(name == "rbox0"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.TopLeft;
						return;
					case '1':
						if (!(name == "rbox1"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.TopCenter;
						return;
					case '2':
						if (!(name == "rbox2"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.TopRight;
						return;
					case '3':
						if (!(name == "rbox3"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.MiddleLeft;
						return;
					case '4':
						if (!(name == "rbox4"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.MiddleCenter;
						return;
					case '5':
						if (!(name == "rbox5"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.MiddleRight;
						return;
					case '6':
						if (!(name == "rbox6"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.BottomLeft;
						return;
					case '7':
						if (!(name == "rbox7"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.BottomCenter;
						return;
					case '8':
						if (!(name == "rbox8"))
						{
							return;
						}
						this.Alignment = PdfContentAlignment.BottomRight;
						break;
					default:
						return;
					}
				}
			}
		}
	}
}
