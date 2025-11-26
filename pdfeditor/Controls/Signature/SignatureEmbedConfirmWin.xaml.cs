using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Properties;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001F9 RID: 505
	public partial class SignatureEmbedConfirmWin : Window
	{
		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06001C66 RID: 7270 RVA: 0x00076B01 File Offset: 0x00074D01
		// (set) Token: 0x06001C67 RID: 7271 RVA: 0x00076B13 File Offset: 0x00074D13
		public string ConfirmTitle
		{
			get
			{
				return (string)base.GetValue(SignatureEmbedConfirmWin.ConfirmTitleProperty);
			}
			set
			{
				base.SetValue(SignatureEmbedConfirmWin.ConfirmTitleProperty, value);
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06001C68 RID: 7272 RVA: 0x00076B21 File Offset: 0x00074D21
		// (set) Token: 0x06001C69 RID: 7273 RVA: 0x00076B33 File Offset: 0x00074D33
		public string NoteMsg
		{
			get
			{
				return (string)base.GetValue(SignatureEmbedConfirmWin.NoteMsgProperty);
			}
			set
			{
				base.SetValue(SignatureEmbedConfirmWin.NoteMsgProperty, value);
			}
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00076B44 File Offset: 0x00074D44
		public SignatureEmbedConfirmWin(EmbedType type)
		{
			this.InitializeComponent();
			if (type == EmbedType.InBatch)
			{
				this.ConfirmTitle = pdfeditor.Properties.Resources.WinSignatureFlattenInBatchQuestion;
				this.NoteMsg = pdfeditor.Properties.Resources.WinSignatureFlattenInBatchNoteMsg;
				return;
			}
			if (type == EmbedType.Single)
			{
				this.ConfirmTitle = pdfeditor.Properties.Resources.WinSignatureFlattenInBatchQuestion;
				this.NoteMsg = pdfeditor.Properties.Resources.WinSignatureFlattenInBatchNoteMsg;
				return;
			}
			if (type == EmbedType.All)
			{
				this.ConfirmTitle = pdfeditor.Properties.Resources.WinSaveFilewithUnFlattenSignatureNoteMsg;
				this.NoteMsg = pdfeditor.Properties.Resources.WinSaveFilewithUnFlattenSignatureExplain;
				return;
			}
			if (type == EmbedType.StampSingle || type == EmbedType.StampInBatch)
			{
				this.ConfirmTitle = pdfeditor.Properties.Resources.StampFlattenConfirmTitle;
				this.NoteMsg = pdfeditor.Properties.Resources.StampFlattenConfirmContent;
			}
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x00076BCB File Offset: 0x00074DCB
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00076BDF File Offset: 0x00074DDF
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x04000A66 RID: 2662
		public static readonly DependencyProperty ConfirmTitleProperty = DependencyProperty.Register("ConfirmTitle", typeof(string), typeof(SignatureEmbedConfirmWin), new PropertyMetadata(""));

		// Token: 0x04000A67 RID: 2663
		public static readonly DependencyProperty NoteMsgProperty = DependencyProperty.Register(" NoteMsg", typeof(string), typeof(SignatureEmbedConfirmWin), new PropertyMetadata(""));
	}
}
