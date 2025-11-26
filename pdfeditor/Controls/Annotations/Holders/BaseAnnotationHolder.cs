using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B2 RID: 690
	public abstract class BaseAnnotationHolder<TCreateAnnotReturn> : IAnnotationHolder where TCreateAnnotReturn : PdfAnnotation
	{
		// Token: 0x060027E5 RID: 10213 RVA: 0x000BB25F File Offset: 0x000B945F
		public BaseAnnotationHolder(AnnotationCanvas annotationCanvas)
		{
			this.AnnotationCanvas = annotationCanvas;
		}

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x060027E6 RID: 10214
		public abstract bool IsTextMarkupAnnotation { get; }

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x060027E7 RID: 10215 RVA: 0x000BB26E File Offset: 0x000B946E
		public AnnotationCanvas AnnotationCanvas { get; }

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x060027E8 RID: 10216 RVA: 0x000BB276 File Offset: 0x000B9476
		// (set) Token: 0x060027E9 RID: 10217 RVA: 0x000BB27E File Offset: 0x000B947E
		public PdfPage CurrentPage { get; protected set; }

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x060027EA RID: 10218 RVA: 0x000BB287 File Offset: 0x000B9487
		// (set) Token: 0x060027EB RID: 10219 RVA: 0x000BB28F File Offset: 0x000B948F
		public PdfAnnotation SelectedAnnotation
		{
			get
			{
				return this.currentAnnotation;
			}
			private set
			{
				if (this.currentAnnotation != value)
				{
					this.currentAnnotation = value;
					EventHandler selectedAnnotationChanged = this.SelectedAnnotationChanged;
					if (selectedAnnotationChanged == null)
					{
						return;
					}
					selectedAnnotationChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x060027EC RID: 10220 RVA: 0x000BB2BC File Offset: 0x000B94BC
		// (set) Token: 0x060027ED RID: 10221 RVA: 0x000BB2C4 File Offset: 0x000B94C4
		public AnnotationHolderState State
		{
			get
			{
				return this.state;
			}
			protected set
			{
				if (this.state != value)
				{
					this.state = value;
					EventHandler stateChanged = this.StateChanged;
					if (stateChanged == null)
					{
						return;
					}
					stateChanged(this, EventArgs.Empty);
				}
			}
		}

		// Token: 0x060027EE RID: 10222 RVA: 0x000BB2EC File Offset: 0x000B94EC
		public virtual void StartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (this.State != AnnotationHolderState.None)
			{
				throw new ArgumentException(this.State.ToString());
			}
			if (this.CurrentPage != null)
			{
				throw new ArgumentException("CurrentPage");
			}
			this.State = AnnotationHolderState.CreateNewStarting;
			this.CurrentPage = page;
			bool flag = false;
			try
			{
				flag = this.OnStartCreateNew(page, pagePoint);
			}
			finally
			{
				if (flag)
				{
					this.State = AnnotationHolderState.CreatingNew;
				}
				else
				{
					this.State = AnnotationHolderState.None;
					this.CurrentPage = null;
				}
			}
		}

		// Token: 0x060027EF RID: 10223 RVA: 0x000BB374 File Offset: 0x000B9574
		public virtual void ProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (this.State != AnnotationHolderState.CreatingNew)
			{
				throw new ArgumentException(this.State.ToString());
			}
			if (this.CurrentPage == null)
			{
				throw new ArgumentException("CurrentPage");
			}
			this.OnProcessCreateNew(page, pagePoint);
		}

		// Token: 0x060027F0 RID: 10224 RVA: 0x000BB3C0 File Offset: 0x000B95C0
		async Task<global::System.Collections.Generic.IReadOnlyList<PdfAnnotation>> IAnnotationHolder.CompleteCreateNewAsync()
		{
			return await this.CompleteCreateNewAsync();
		}

		// Token: 0x060027F1 RID: 10225 RVA: 0x000BB404 File Offset: 0x000B9604
		public virtual async Task<global::System.Collections.Generic.IReadOnlyList<TCreateAnnotReturn>> CompleteCreateNewAsync()
		{
			if (this.CurrentPage == null)
			{
				throw new ArgumentException("CurrentPage");
			}
			this.State = AnnotationHolderState.CreateNewCompleting;
			global::System.Collections.Generic.IReadOnlyList<TCreateAnnotReturn> readOnlyList2;
			try
			{
				global::System.Collections.Generic.IReadOnlyList<TCreateAnnotReturn> readOnlyList = await this.OnCompleteCreateNewAsync();
				if (readOnlyList != null)
				{
					MainViewModel mainViewModel = this.AnnotationCanvas.DataContext as MainViewModel;
					if (mainViewModel != null)
					{
						foreach (TCreateAnnotReturn tcreateAnnotReturn in readOnlyList)
						{
							if (tcreateAnnotReturn is PdfMarkupAnnotation)
							{
								if (mainViewModel != null)
								{
									PageEditorViewModel pageEditors = mainViewModel.PageEditors;
									if (pageEditors != null)
									{
										pageEditors.NotifyPageAnnotationChanged(tcreateAnnotReturn.Page.PageIndex);
									}
								}
								if (tcreateAnnotReturn is PdfFileAttachmentAnnotation && mainViewModel != null)
								{
									mainViewModel.PageEditors.NotifyAttachmentChanged();
								}
							}
						}
					}
					try
					{
						TCreateAnnotReturn tcreateAnnotReturn2 = ((readOnlyList.Count > 0) ? readOnlyList[0] : default(TCreateAnnotReturn));
						PdfHighlightAnnotation pdfHighlightAnnotation = tcreateAnnotReturn2 as PdfHighlightAnnotation;
						if (pdfHighlightAnnotation != null)
						{
							if (!string.IsNullOrWhiteSpace(pdfHighlightAnnotation.Subject) && pdfHighlightAnnotation.Subject == "AreaHighlight")
							{
								GAManager.SendEvent("AnnotationAction", "PdfAreaHighlightAnnotation", "New", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfHighlightAnnotation", "New", 1L);
							}
						}
						if (tcreateAnnotReturn2 is PdfStrikeoutAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfStrikeoutAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfUnderlineAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfUnderlineAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfLineAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfLineAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfSquareAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfSquareAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfCircleAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfCircleAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfInkAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfInkAnnotation", "New", 1L);
						}
						if (tcreateAnnotReturn2 is PdfLinkAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfLinkAnnotation", "New", 1L);
						}
						PdfFreeTextAnnotation pdfFreeTextAnnotation = tcreateAnnotReturn2 as PdfFreeTextAnnotation;
						if (pdfFreeTextAnnotation != null)
						{
							if (pdfFreeTextAnnotation.Intent == AnnotationIntent.FreeTextTypeWriter)
							{
								GAManager.SendEvent("AnnotationAction", "PdfFreeTextAnnotationTransparent", "New", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfFreeTextAnnotation", "New", 1L);
							}
						}
						if (tcreateAnnotReturn2 is PdfTextAnnotation)
						{
							GAManager.SendEvent("AnnotationAction", "PdfTextAnnotation", "New", 1L);
						}
						PdfStampAnnotation pdfStampAnnotation = tcreateAnnotReturn2 as PdfStampAnnotation;
						if (pdfStampAnnotation != null)
						{
							if (!string.IsNullOrWhiteSpace(pdfStampAnnotation.Subject) && pdfStampAnnotation.Subject == "Signature")
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotationSignature", "New", 1L);
							}
							else if (!string.IsNullOrWhiteSpace(pdfStampAnnotation.Subject) && pdfStampAnnotation.Subject == "FormControl")
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotationForm", "New", 1L);
							}
							else
							{
								GAManager.SendEvent("AnnotationAction", "PdfStampAnnotation", "New", 1L);
							}
						}
					}
					catch
					{
					}
				}
				readOnlyList2 = readOnlyList;
			}
			finally
			{
				this.State = AnnotationHolderState.None;
				this.CurrentPage = null;
			}
			return readOnlyList2;
		}

		// Token: 0x060027F2 RID: 10226 RVA: 0x000BB448 File Offset: 0x000B9648
		public void Select(PdfAnnotation annotation, bool afterCreate)
		{
			if (!(annotation is TCreateAnnotReturn))
			{
				throw new ArgumentException("annotation");
			}
			if (this.State != AnnotationHolderState.None)
			{
				throw new ArgumentException(this.State.ToString());
			}
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			this.CurrentPage = annotation.Page;
			bool flag = false;
			try
			{
				flag = this.OnSelecting((TCreateAnnotReturn)((object)annotation), afterCreate);
			}
			finally
			{
				if (flag)
				{
					this.State = AnnotationHolderState.Selected;
					this.SelectedAnnotation = annotation;
				}
				else
				{
					this.State = AnnotationHolderState.None;
					this.SelectedAnnotation = null;
					this.CurrentPage = null;
				}
			}
		}

		// Token: 0x060027F3 RID: 10227 RVA: 0x000BB4F0 File Offset: 0x000B96F0
		public virtual void Cancel()
		{
			if (this.AnnotationCanvas.Dispatcher.CheckAccess())
			{
				this.<Cancel>g__CancelCore|23_0();
				return;
			}
			this.AnnotationCanvas.Dispatcher.Invoke(new Action(this.<Cancel>g__CancelCore|23_0));
		}

		// Token: 0x060027F4 RID: 10228
		protected abstract bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint);

		// Token: 0x060027F5 RID: 10229
		protected abstract void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint);

		// Token: 0x060027F6 RID: 10230
		protected abstract Task<global::System.Collections.Generic.IReadOnlyList<TCreateAnnotReturn>> OnCompleteCreateNewAsync();

		// Token: 0x060027F7 RID: 10231
		public abstract void OnPageClientBoundsChanged();

		// Token: 0x060027F8 RID: 10232
		protected abstract void OnCancel();

		// Token: 0x060027F9 RID: 10233
		protected abstract bool OnSelecting(TCreateAnnotReturn annotation, bool afterCreate);

		// Token: 0x060027FA RID: 10234
		public abstract bool OnPropertyChanged(string propertyName);

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x060027FB RID: 10235 RVA: 0x000BB528 File Offset: 0x000B9728
		// (remove) Token: 0x060027FC RID: 10236 RVA: 0x000BB560 File Offset: 0x000B9760
		public event EventHandler Canceled;

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x060027FD RID: 10237 RVA: 0x000BB598 File Offset: 0x000B9798
		// (remove) Token: 0x060027FE RID: 10238 RVA: 0x000BB5D0 File Offset: 0x000B97D0
		public event EventHandler StateChanged;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x060027FF RID: 10239 RVA: 0x000BB608 File Offset: 0x000B9808
		// (remove) Token: 0x06002800 RID: 10240 RVA: 0x000BB640 File Offset: 0x000B9840
		public event EventHandler SelectedAnnotationChanged;

		// Token: 0x06002801 RID: 10241 RVA: 0x000BB675 File Offset: 0x000B9875
		[CompilerGenerated]
		private void <Cancel>g__CancelCore|23_0()
		{
			this.OnCancel();
			this.State = AnnotationHolderState.None;
			this.CurrentPage = null;
			this.SelectedAnnotation = null;
			EventHandler canceled = this.Canceled;
			if (canceled == null)
			{
				return;
			}
			canceled(this, EventArgs.Empty);
		}

		// Token: 0x04001126 RID: 4390
		private AnnotationHolderState state;

		// Token: 0x04001127 RID: 4391
		private PdfAnnotation currentAnnotation;
	}
}
