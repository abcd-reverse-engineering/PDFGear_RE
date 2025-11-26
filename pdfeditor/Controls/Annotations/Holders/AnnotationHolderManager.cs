using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002AE RID: 686
	public class AnnotationHolderManager
	{
		// Token: 0x0600279F RID: 10143 RVA: 0x000BA638 File Offset: 0x000B8838
		public AnnotationHolderManager(AnnotationCanvas annotationCanvas)
		{
			this.annotationCanvas = annotationCanvas;
			this.holders = new Dictionary<Type, IAnnotationHolder>();
			this.holders[typeof(PdfLineAnnotation)] = new LineAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfUnderlineAnnotation)] = new UnderlineAnnotationHoder(annotationCanvas);
			this.holders[typeof(PdfStrikeoutAnnotation)] = new StrikeoutAnnotationHoder(annotationCanvas);
			this.holders[typeof(PdfHighlightAnnotation)] = new HighlightAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfFreeTextAnnotation)] = new FreeTextAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfInkAnnotation)] = new InkAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfSquareAnnotation)] = new SquareAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfLinkAnnotation)] = new LinkAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfCircleAnnotation)] = new CircleAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfTextAnnotation)] = new TextAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfStampAnnotation)] = new StampAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfWatermarkAnnotation)] = new WatermarkAnnotationHolder(annotationCanvas);
			this.holders[typeof(AnnotationHolderManager.AreaHighlightAnnotation)] = new HighlightAreaAnnotationHolder(annotationCanvas);
			this.holders[typeof(PdfFileAttachmentAnnotation)] = new AttachmentAnnotationHolder(annotationCanvas);
			foreach (KeyValuePair<Type, IAnnotationHolder> keyValuePair in this.holders)
			{
				Type type;
				IAnnotationHolder annotationHolder;
				keyValuePair.Deconstruct(out type, out annotationHolder);
				IAnnotationHolder annotationHolder2 = annotationHolder;
				annotationHolder2.StateChanged += this.Holder_StateChanged;
				annotationHolder2.SelectedAnnotationChanged += this.Holder_SelectedAnnotationChanged;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x060027A0 RID: 10144 RVA: 0x000BA844 File Offset: 0x000B8A44
		public LineAnnotationHolder Line
		{
			get
			{
				return (LineAnnotationHolder)this.holders[typeof(PdfLineAnnotation)];
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x060027A1 RID: 10145 RVA: 0x000BA860 File Offset: 0x000B8A60
		public UnderlineAnnotationHoder Underline
		{
			get
			{
				return (UnderlineAnnotationHoder)this.holders[typeof(PdfUnderlineAnnotation)];
			}
		}

		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x060027A2 RID: 10146 RVA: 0x000BA87C File Offset: 0x000B8A7C
		public StrikeoutAnnotationHoder Strikeout
		{
			get
			{
				return (StrikeoutAnnotationHoder)this.holders[typeof(PdfStrikeoutAnnotation)];
			}
		}

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x060027A3 RID: 10147 RVA: 0x000BA898 File Offset: 0x000B8A98
		public HighlightAnnotationHolder Highlight
		{
			get
			{
				return (HighlightAnnotationHolder)this.holders[typeof(PdfHighlightAnnotation)];
			}
		}

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x060027A4 RID: 10148 RVA: 0x000BA8B4 File Offset: 0x000B8AB4
		public FreeTextAnnotationHolder FreeText
		{
			get
			{
				return (FreeTextAnnotationHolder)this.holders[typeof(PdfFreeTextAnnotation)];
			}
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x060027A5 RID: 10149 RVA: 0x000BA8D0 File Offset: 0x000B8AD0
		public InkAnnotationHolder Ink
		{
			get
			{
				return (InkAnnotationHolder)this.holders[typeof(PdfInkAnnotation)];
			}
		}

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x060027A6 RID: 10150 RVA: 0x000BA8EC File Offset: 0x000B8AEC
		public SquareAnnotationHolder Square
		{
			get
			{
				return (SquareAnnotationHolder)this.holders[typeof(PdfSquareAnnotation)];
			}
		}

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x060027A7 RID: 10151 RVA: 0x000BA908 File Offset: 0x000B8B08
		public LinkAnnotationHolder Link
		{
			get
			{
				return (LinkAnnotationHolder)this.holders[typeof(PdfLinkAnnotation)];
			}
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x060027A8 RID: 10152 RVA: 0x000BA924 File Offset: 0x000B8B24
		public CircleAnnotationHolder Circle
		{
			get
			{
				return (CircleAnnotationHolder)this.holders[typeof(PdfCircleAnnotation)];
			}
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000BA940 File Offset: 0x000B8B40
		public TextAnnotationHolder Text
		{
			get
			{
				return (TextAnnotationHolder)this.holders[typeof(PdfTextAnnotation)];
			}
		}

		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x060027AA RID: 10154 RVA: 0x000BA95C File Offset: 0x000B8B5C
		public StampAnnotationHolder Stamp
		{
			get
			{
				return (StampAnnotationHolder)this.holders[typeof(PdfStampAnnotation)];
			}
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x060027AB RID: 10155 RVA: 0x000BA978 File Offset: 0x000B8B78
		public WatermarkAnnotationHolder Watermark
		{
			get
			{
				return (WatermarkAnnotationHolder)this.holders[typeof(PdfWatermarkAnnotation)];
			}
		}

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x060027AC RID: 10156 RVA: 0x000BA994 File Offset: 0x000B8B94
		public AttachmentAnnotationHolder Attachment
		{
			get
			{
				return (AttachmentAnnotationHolder)this.holders[typeof(PdfFileAttachmentAnnotation)];
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x060027AD RID: 10157 RVA: 0x000BA9B0 File Offset: 0x000B8BB0
		public HighlightAreaAnnotationHolder HighlightArea
		{
			get
			{
				return (HighlightAreaAnnotationHolder)this.holders[typeof(AnnotationHolderManager.AreaHighlightAnnotation)];
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x060027AE RID: 10158 RVA: 0x000BA9CC File Offset: 0x000B8BCC
		// (set) Token: 0x060027AF RID: 10159 RVA: 0x000BA9ED File Offset: 0x000B8BED
		public IAnnotationHolder CurrentHolder
		{
			get
			{
				IAnnotationHolder annotationHolder = this.currentHolder;
				if (annotationHolder != null && annotationHolder.State == AnnotationHolderState.None)
				{
					return null;
				}
				return this.currentHolder;
			}
			private set
			{
				if (this.currentHolder != value)
				{
					this.currentHolder = value;
					EventHandler currentHolderChanged = this.CurrentHolderChanged;
					if (currentHolderChanged == null)
					{
						return;
					}
					currentHolderChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x060027B0 RID: 10160 RVA: 0x000BAA15 File Offset: 0x000B8C15
		// (set) Token: 0x060027B1 RID: 10161 RVA: 0x000BAA1D File Offset: 0x000B8C1D
		public PdfAnnotation SelectedAnnotation
		{
			get
			{
				return this.selectedAnnotation;
			}
			set
			{
				if (this.selectedAnnotation != value)
				{
					this.selectedAnnotation = value;
					EventHandler selectedAnnotationChanged = this.SelectedAnnotationChanged;
					if (selectedAnnotationChanged == null)
					{
						return;
					}
					selectedAnnotationChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x060027B2 RID: 10162 RVA: 0x000BAA4C File Offset: 0x000B8C4C
		public IAnnotationControl SelectedAnnotationControl
		{
			get
			{
				IAnnotationHolder annotationHolder = this.CurrentHolder;
				if (annotationHolder != null && annotationHolder.State == AnnotationHolderState.Selected)
				{
					PdfAnnotation selectedAnnotation = annotationHolder.SelectedAnnotation;
					return this.annotationCanvas.Children.OfType<IAnnotationControl>().FirstOrDefault((IAnnotationControl c) => c.Annotation == selectedAnnotation);
				}
				return null;
			}
		}

		// Token: 0x060027B3 RID: 10163 RVA: 0x000BAAA4 File Offset: 0x000B8CA4
		public bool IsAnnotationDoubleClicked(MouseEventArgs e)
		{
			FrameworkElement frameworkElement = this.SelectedAnnotationControl as FrameworkElement;
			if (frameworkElement == null)
			{
				PdfAnnotation pdfAnnotation = this.SelectedAnnotation;
				if (pdfAnnotation != null)
				{
					IAnnotationHolder annotationHolder = this.CurrentHolder;
					if (annotationHolder != null && annotationHolder.IsTextMarkupAnnotation)
					{
						Point position = e.GetPosition(frameworkElement);
						return AnnotationHitTestHelper.HitTest(pdfAnnotation, position);
					}
				}
				return false;
			}
			Point position2 = e.GetPosition(frameworkElement);
			if (frameworkElement.InputHitTest(position2) == null)
			{
				return false;
			}
			AnnotationFreeTextEditor annotationFreeTextEditor = frameworkElement as AnnotationFreeTextEditor;
			if (annotationFreeTextEditor != null)
			{
				RichTextBox richTextBox = annotationFreeTextEditor.GetRichTextBox();
				return richTextBox == null || richTextBox.IsReadOnly;
			}
			return true;
		}

		// Token: 0x060027B4 RID: 10164 RVA: 0x000BAB2F File Offset: 0x000B8D2F
		private void UpdateSelectedAnnotation()
		{
			IAnnotationHolder annotationHolder = this.CurrentHolder;
			this.SelectedAnnotation = ((annotationHolder != null && annotationHolder.State == AnnotationHolderState.Selected) ? this.CurrentHolder.SelectedAnnotation : null);
		}

		// Token: 0x060027B5 RID: 10165 RVA: 0x000BAB5C File Offset: 0x000B8D5C
		private void UpdateCurrentHolder()
		{
			this.CurrentHolder = this.holders.Values.FirstOrDefault((IAnnotationHolder c) => c.State > AnnotationHolderState.None);
		}

		// Token: 0x060027B6 RID: 10166 RVA: 0x000BAB94 File Offset: 0x000B8D94
		private void Holder_StateChanged(object sender, EventArgs e)
		{
			if (this.holderStateChanging)
			{
				return;
			}
			this.holderStateChanging = true;
			IAnnotationHolder annotationHolder = sender as IAnnotationHolder;
			if (annotationHolder != null)
			{
				if (annotationHolder.State != AnnotationHolderState.None)
				{
					foreach (KeyValuePair<Type, IAnnotationHolder> keyValuePair in this.holders)
					{
						Type type;
						IAnnotationHolder annotationHolder2;
						keyValuePair.Deconstruct(out type, out annotationHolder2);
						IAnnotationHolder annotationHolder3 = annotationHolder2;
						if (annotationHolder3 != annotationHolder && annotationHolder3.State != AnnotationHolderState.None)
						{
							annotationHolder3.Cancel();
						}
					}
				}
				if (!this.selecting)
				{
					this.UpdateCurrentHolder();
					this.UpdateSelectedAnnotation();
				}
			}
			this.holderStateChanging = false;
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x000BAC3C File Offset: 0x000B8E3C
		private void Holder_SelectedAnnotationChanged(object sender, EventArgs e)
		{
			if (!this.selecting)
			{
				this.UpdateCurrentHolder();
				this.UpdateSelectedAnnotation();
			}
		}

		// Token: 0x060027B8 RID: 10168 RVA: 0x000BAC54 File Offset: 0x000B8E54
		public void Select(PdfAnnotation annotation, bool afterCreate)
		{
			if (this.selecting)
			{
				return;
			}
			PdfAnnotation pdfAnnotation = this.SelectedAnnotation;
			if (pdfAnnotation != null && pdfAnnotation == annotation)
			{
				return;
			}
			this.selecting = true;
			try
			{
				this.CancelAllCore();
				if (annotation != null)
				{
					IAnnotationHolder holder = this.GetHolder(annotation);
					if (holder != null)
					{
						holder.Select(annotation, afterCreate);
					}
				}
				this.UpdateCurrentHolder();
				this.UpdateSelectedAnnotation();
			}
			finally
			{
				this.selecting = false;
			}
		}

		// Token: 0x060027B9 RID: 10169 RVA: 0x000BACD4 File Offset: 0x000B8ED4
		private IAnnotationHolder GetHolder(PdfAnnotation annotation)
		{
			if (annotation != null)
			{
				Type type = annotation.GetType();
				IAnnotationHolder annotationHolder;
				if (type == typeof(PdfHighlightAnnotation))
				{
					if ((annotation as PdfHighlightAnnotation).Subject == "AreaHighlight")
					{
						return (HighlightAreaAnnotationHolder)this.holders[typeof(AnnotationHolderManager.AreaHighlightAnnotation)];
					}
					return (HighlightAnnotationHolder)this.holders[typeof(PdfHighlightAnnotation)];
				}
				else if (this.holders.TryGetValue(type, out annotationHolder))
				{
					return annotationHolder;
				}
			}
			return null;
		}

		// Token: 0x060027BA RID: 10170 RVA: 0x000BAD64 File Offset: 0x000B8F64
		public void CancelAll()
		{
			this.selecting = true;
			try
			{
				this.CancelAllCore();
				this.UpdateCurrentHolder();
				this.UpdateSelectedAnnotation();
			}
			finally
			{
				this.selecting = false;
			}
		}

		// Token: 0x060027BB RID: 10171 RVA: 0x000BADA4 File Offset: 0x000B8FA4
		private void CancelAllCore()
		{
			foreach (KeyValuePair<Type, IAnnotationHolder> keyValuePair in this.holders)
			{
				Type type;
				IAnnotationHolder annotationHolder;
				keyValuePair.Deconstruct(out type, out annotationHolder);
				IAnnotationHolder annotationHolder2 = annotationHolder;
				if (annotationHolder2.State != AnnotationHolderState.None)
				{
					annotationHolder2.Cancel();
					Ioc.Default.GetRequiredService<MainViewModel>();
				}
			}
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x000BAE14 File Offset: 0x000B9014
		public Task DeleteAnnotationAsync(PdfAnnotation annotation, bool batchDeletion = false)
		{
			return this.DeleteAnnotationsAsync(new PdfAnnotation[] { annotation }, batchDeletion);
		}

		// Token: 0x060027BD RID: 10173 RVA: 0x000BAE28 File Offset: 0x000B9028
		public async Task DeleteAnnotationsAsync(global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> annotations, bool batchDeletion = false)
		{
			if (annotations == null)
			{
				throw new ArgumentNullException("annotations");
			}
			if (annotations.Count != 0)
			{
				List<PdfAnnotation> deleteList = new List<PdfAnnotation>();
				PdfDocument doc = null;
				for (int i = 0; i < annotations.Count; i++)
				{
					PdfAnnotation pdfAnnotation = annotations[i];
					bool flag;
					if (pdfAnnotation == null)
					{
						flag = null != null;
					}
					else
					{
						PdfPage page = pdfAnnotation.Page;
						flag = ((page != null) ? page.Document : null) != null;
					}
					if (!flag)
					{
						throw new ArgumentNullException("annotations");
					}
					if (doc == null)
					{
						PdfAnnotation pdfAnnotation2 = annotations[i];
						PdfDocument pdfDocument;
						if (pdfAnnotation2 == null)
						{
							pdfDocument = null;
						}
						else
						{
							PdfPage page2 = pdfAnnotation2.Page;
							pdfDocument = ((page2 != null) ? page2.Document : null);
						}
						doc = pdfDocument;
					}
					else
					{
						PdfAnnotation pdfAnnotation3 = annotations[i];
						PdfDocument pdfDocument2;
						if (pdfAnnotation3 == null)
						{
							pdfDocument2 = null;
						}
						else
						{
							PdfPage page3 = pdfAnnotation3.Page;
							pdfDocument2 = ((page3 != null) ? page3.Document : null);
						}
						if (pdfDocument2 != doc)
						{
							throw new ArgumentNullException("annotations");
						}
					}
					if (annotations[i].Page.Annots != null && annotations[i].Page.Annots.Contains(annotations[i]))
					{
						deleteList.Add(annotations[i]);
					}
				}
				if (deleteList.Count != 0)
				{
					MainViewModel vm = this.annotationCanvas.DataContext as MainViewModel;
					if (vm != null)
					{
						this.SelectedAnnotation = null;
						await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
						await vm.OperationManager.TraceAnnotationRemoveAsync(deleteList.Where((PdfAnnotation c) => !this.IsEmbedSignature(c)).ToArray<PdfAnnotation>(), "");
						this.annotationCanvas.PopupHolder.ClearAnnotationPopup();
						HashSet<int> hashSet = new HashSet<int>();
						for (int j = 0; j < deleteList.Count; j++)
						{
							deleteList[j].DeleteAnnotation();
							if (!batchDeletion && hashSet.Add(deleteList[j].Page.PageIndex))
							{
								PageEditorViewModel pageEditors = vm.PageEditors;
								if (pageEditors != null)
								{
									pageEditors.NotifyPageAnnotationChanged(deleteList[j].Page.PageIndex);
								}
							}
						}
						if (deleteList.Any((PdfAnnotation t) => t is PdfFileAttachmentAnnotation))
						{
							PageEditorViewModel pageEditors2 = vm.PageEditors;
							if (pageEditors2 != null)
							{
								pageEditors2.NotifyAttachmentChanged();
							}
						}
						PdfPage currentPage = doc.Pages.CurrentPage;
						if (currentPage != null)
						{
							this.annotationCanvas.PopupHolder.InitAnnotationPopup(currentPage);
							await currentPage.TryRedrawPageAsync(default(CancellationToken));
						}
					}
				}
			}
		}

		// Token: 0x060027BE RID: 10174 RVA: 0x000BAE7C File Offset: 0x000B907C
		private bool IsEmbedSignature(PdfAnnotation annotation)
		{
			PdfStampAnnotation pdfStampAnnotation = annotation as PdfStampAnnotation;
			return pdfStampAnnotation != null && pdfStampAnnotation.Subject == "Signature" && pdfStampAnnotation.Dictionary.ContainsKey("Embed");
		}

		// Token: 0x060027BF RID: 10175 RVA: 0x000BAEBC File Offset: 0x000B90BC
		public async Task BatchDeleteAnnotationsAsync(global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> annotations, IProgress<double> progress, CancellationToken cancellationToken)
		{
			if (annotations == null)
			{
				throw new ArgumentNullException("annotations");
			}
			if (annotations.Count != 0)
			{
				List<PdfAnnotation> deleteList = new List<PdfAnnotation>();
				PdfDocument doc = null;
				for (int j = 0; j < annotations.Count; j++)
				{
					PdfAnnotation pdfAnnotation = annotations[j];
					bool flag;
					if (pdfAnnotation == null)
					{
						flag = null != null;
					}
					else
					{
						PdfPage page = pdfAnnotation.Page;
						flag = ((page != null) ? page.Document : null) != null;
					}
					if (!flag)
					{
						throw new ArgumentNullException("annotations");
					}
					if (doc == null)
					{
						PdfAnnotation pdfAnnotation2 = annotations[j];
						PdfDocument pdfDocument;
						if (pdfAnnotation2 == null)
						{
							pdfDocument = null;
						}
						else
						{
							PdfPage page2 = pdfAnnotation2.Page;
							pdfDocument = ((page2 != null) ? page2.Document : null);
						}
						doc = pdfDocument;
					}
					else
					{
						PdfAnnotation pdfAnnotation3 = annotations[j];
						PdfDocument pdfDocument2;
						if (pdfAnnotation3 == null)
						{
							pdfDocument2 = null;
						}
						else
						{
							PdfPage page3 = pdfAnnotation3.Page;
							pdfDocument2 = ((page3 != null) ? page3.Document : null);
						}
						if (pdfDocument2 != doc)
						{
							throw new ArgumentNullException("annotations");
						}
					}
					if (annotations[j].Page.Annots != null && annotations[j].Page.Annots.Contains(annotations[j]))
					{
						deleteList.Add(annotations[j]);
					}
				}
				if (deleteList.Count != 0)
				{
					if (this.annotationCanvas.DataContext is MainViewModel)
					{
						this.SelectedAnnotation = null;
						await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
						this.annotationCanvas.PopupHolder.ClearAnnotationPopup();
						new HashSet<int>();
						if (progress != null)
						{
							progress.Report(0.0);
						}
						for (int i = 0; i < deleteList.Count; i++)
						{
							deleteList[i].DeleteAnnotation();
							if (progress != null)
							{
								progress.Report(((double)i + 1.0) / (double)deleteList.Count);
							}
							if (i % 10 == 0)
							{
								await Task.Delay(1);
							}
						}
						PdfPage currentPage = doc.Pages.CurrentPage;
						if (currentPage != null)
						{
							this.annotationCanvas.PopupHolder.InitAnnotationPopup(currentPage);
							await currentPage.TryRedrawPageAsync(default(CancellationToken));
						}
					}
				}
			}
		}

		// Token: 0x060027C0 RID: 10176 RVA: 0x000BAF10 File Offset: 0x000B9110
		public bool OnPropertyChanged(string propertyName)
		{
			int num;
			return this.OnPropertyChanged(propertyName, out num);
		}

		// Token: 0x060027C1 RID: 10177 RVA: 0x000BAF28 File Offset: 0x000B9128
		public bool OnPropertyChanged(string propertyName, out int pageIndex)
		{
			pageIndex = -1;
			if (this.CurrentHolder != null && this.CurrentHolder.State == AnnotationHolderState.Selected)
			{
				IAnnotationHolder annotationHolder = this.CurrentHolder;
				int? num;
				if (annotationHolder == null)
				{
					num = null;
				}
				else
				{
					PdfAnnotation pdfAnnotation = annotationHolder.SelectedAnnotation;
					if (pdfAnnotation == null)
					{
						num = null;
					}
					else
					{
						PdfPage page = pdfAnnotation.Page;
						num = ((page != null) ? new int?(page.PageIndex) : null);
					}
				}
				int? num2 = num;
				int valueOrDefault = num2.GetValueOrDefault(-1);
				if (this.CurrentHolder.OnPropertyChanged(propertyName))
				{
					pageIndex = valueOrDefault;
					this.annotationCanvas.PopupHolder.ClearAnnotationPopup();
					PdfPage pdfPage = this.CurrentHolder.CurrentPage;
					if (pdfPage == null)
					{
						PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
						PdfPage pdfPage2;
						if (pdfViewer == null)
						{
							pdfPage2 = null;
						}
						else
						{
							PdfDocument document = pdfViewer.Document;
							if (document == null)
							{
								pdfPage2 = null;
							}
							else
							{
								PdfPageCollection pages = document.Pages;
								pdfPage2 = ((pages != null) ? pages.CurrentPage : null);
							}
						}
						pdfPage = pdfPage2;
					}
					if (pdfPage != null)
					{
						this.annotationCanvas.PopupHolder.InitAnnotationPopup(pdfPage);
					}
					MainViewModel mainViewModel = this.annotationCanvas.DataContext as MainViewModel;
					if (mainViewModel != null)
					{
						PageEditorViewModel pageEditors = mainViewModel.PageEditors;
						if (pageEditors != null)
						{
							pageEditors.NotifyPageAnnotationChanged(pageIndex);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x060027C2 RID: 10178 RVA: 0x000BB048 File Offset: 0x000B9248
		public async Task WaitForCancelCompletedAsync()
		{
			await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
		}

		// Token: 0x060027C3 RID: 10179 RVA: 0x000BB083 File Offset: 0x000B9283
		public void OnPageClientBoundsChanged()
		{
			IAnnotationHolder annotationHolder = this.CurrentHolder;
			if (((annotationHolder != null) ? annotationHolder.CurrentPage : null) != null)
			{
				IAnnotationHolder annotationHolder2 = this.CurrentHolder;
				if (annotationHolder2 == null)
				{
					return;
				}
				annotationHolder2.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x060027C4 RID: 10180 RVA: 0x000BB0AC File Offset: 0x000B92AC
		// (remove) Token: 0x060027C5 RID: 10181 RVA: 0x000BB0E4 File Offset: 0x000B92E4
		public event EventHandler CurrentHolderChanged;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x060027C6 RID: 10182 RVA: 0x000BB11C File Offset: 0x000B931C
		// (remove) Token: 0x060027C7 RID: 10183 RVA: 0x000BB154 File Offset: 0x000B9354
		public event EventHandler SelectedAnnotationChanged;

		// Token: 0x04001118 RID: 4376
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04001119 RID: 4377
		private Dictionary<Type, IAnnotationHolder> holders;

		// Token: 0x0400111A RID: 4378
		private IAnnotationHolder currentHolder;

		// Token: 0x0400111B RID: 4379
		private bool selecting;

		// Token: 0x0400111C RID: 4380
		private bool holderStateChanging;

		// Token: 0x0400111D RID: 4381
		private PdfAnnotation selectedAnnotation;

		// Token: 0x0200078B RID: 1931
		private class AreaHighlightAnnotation
		{
		}
	}
}
