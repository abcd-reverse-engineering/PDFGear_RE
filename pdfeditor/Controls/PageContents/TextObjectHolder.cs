using System;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using Patagames.Pdf.Net;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.PageContents
{
	// Token: 0x02000257 RID: 599
	public class TextObjectHolder
	{
		// Token: 0x060022A9 RID: 8873 RVA: 0x000A37F8 File Offset: 0x000A19F8
		public TextObjectHolder(AnnotationCanvas annotationCanvas)
		{
			this.AnnotationCanvas = annotationCanvas;
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x060022AA RID: 8874 RVA: 0x000A3807 File Offset: 0x000A1A07
		public AnnotationCanvas AnnotationCanvas { get; }

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x060022AB RID: 8875 RVA: 0x000A380F File Offset: 0x000A1A0F
		public PdfTextObject SelectedObject
		{
			get
			{
				TextObjectEditControl textObjectEditControl = this.textObjectEditControl;
				if (textObjectEditControl == null)
				{
					return null;
				}
				return textObjectEditControl.TextObject;
			}
		}

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x060022AC RID: 8876 RVA: 0x000A3822 File Offset: 0x000A1A22
		public TextObjectEditControl TextObjectEditControl
		{
			get
			{
				return this.textObjectEditControl;
			}
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x000A382C File Offset: 0x000A1A2C
		public async Task DeleteSelectedObjectAsync()
		{
			if (this.textObjectEditControl != null)
			{
				int pageIndex = this.textObjectEditControl.PageIndex;
				PdfTextObject textObject = this.textObjectEditControl.TextObject;
				if (textObject != null)
				{
					PdfPage pdfPage = this.AnnotationCanvas.PdfViewer.Document.Pages[pageIndex];
					MainViewModel mainViewModel = this.AnnotationCanvas.DataContext as MainViewModel;
					this.CancelTextObject();
					GAManager.SendEvent("TextEditor", "DeleteSelectedObject", "Count", 1L);
					await mainViewModel.OperationManager.DeleteTextObjectAsync(pdfPage, textObject, "");
				}
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x000A386F File Offset: 0x000A1A6F
		public void OnPageClientBoundsChanged()
		{
			TextObjectEditControl textObjectEditControl = this.textObjectEditControl;
			if (textObjectEditControl == null)
			{
				return;
			}
			textObjectEditControl.UpdatePosition();
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x000A3881 File Offset: 0x000A1A81
		public bool CancelTextObject()
		{
			if (this.textObjectEditControl != null)
			{
				this.AnnotationCanvas.Children.Remove(this.textObjectEditControl);
				this.textObjectEditControl = null;
				return true;
			}
			return false;
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x000A38AC File Offset: 0x000A1AAC
		public void SelectTextObject(PdfPage page, PdfTextObject textObject, bool showContextMenu = true)
		{
			if (page == null || textObject == null)
			{
				this.CancelTextObject();
				return;
			}
			if (this.AnnotationCanvas.EditingPageObjectType != PageObjectType.Text)
			{
				return;
			}
			this.AnnotationCanvas.UpdateHoverPageObjectRect(Rect.Empty);
			if (this.textObjectEditControl != null)
			{
				this.AnnotationCanvas.Children.Remove(this.textObjectEditControl);
			}
			this.textObjectEditControl = new TextObjectEditControl(this.AnnotationCanvas, page.PageIndex, textObject);
			this.AnnotationCanvas.Children.Add(this.textObjectEditControl);
			if (showContextMenu)
			{
				this.AnnotationCanvas.TextObjectContextMenuHolder.ShowAtSelectedObejctRightAsync();
			}
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x000A3948 File Offset: 0x000A1B48
		public async Task EditSelectedTextObjectAsync()
		{
			if (this.textObjectEditControl != null)
			{
				int pageIndex = this.textObjectEditControl.PageIndex;
				PdfTextObject textObject = this.textObjectEditControl.TextObject;
				if (textObject != null)
				{
					GAManager.SendEvent("TextEditor", "EditSelectedTextObject", "Show", 1L);
					PdfPage page = this.AnnotationCanvas.PdfViewer.Document.Pages[pageIndex];
					TextObjectEditDialog textObjectEditDialog = new TextObjectEditDialog();
					textObjectEditDialog.Text = textObject.TextUnicode;
					textObjectEditDialog.Owner = Application.Current.MainWindow;
					textObjectEditDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					if (textObjectEditDialog.ShowDialog().GetValueOrDefault())
					{
						string text = textObjectEditDialog.Text;
						if (!(text == textObject.TextUnicode))
						{
							GAManager.SendEvent("TextEditor", "EditSelectedTextObject", "EditText", 1L);
							MainViewModel mainViewModel = this.AnnotationCanvas.DataContext as MainViewModel;
							this.CancelTextObject();
							if (mainViewModel != null)
							{
								if (string.IsNullOrWhiteSpace(text))
								{
									await mainViewModel.OperationManager.DeleteTextObjectAsync(page, textObject, "");
								}
								else
								{
									PdfTextObject[] array = await mainViewModel.OperationManager.ModifyTextObjectAsync(page, textObject, text, "");
									if (array != null && array.Length != 0)
									{
										this.SelectTextObject(page, array[0], false);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04000EBF RID: 3775
		private TextObjectEditControl textObjectEditControl;
	}
}
