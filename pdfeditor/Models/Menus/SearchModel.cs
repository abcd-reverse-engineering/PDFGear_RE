using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Utils.DocumentOcr;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000160 RID: 352
	public class SearchModel : ObservableObject, IDisposable
	{
		// Token: 0x0600152A RID: 5418 RVA: 0x000524AC File Offset: 0x000506AC
		public SearchModel(DocumentWrapper documentWrapper)
		{
			this.documentWrapper = documentWrapper;
			this.document = ((documentWrapper != null) ? documentWrapper.Document : null);
			if (this.document != null)
			{
				this.searchTimer = new DispatcherTimer();
				this.searchTimer.Interval = TimeSpan.FromMilliseconds(1.0);
				this.searchTimer.Tick += this.SearchTimer_Tick;
			}
			this.HighlightColor = Color.FromArgb(byte.MaxValue, byte.MaxValue, 198, 131);
			this.ActiveRecordColor = Color.FromArgb(byte.MaxValue, 242, 131, 129);
			this.InflateHighlight = new FS_RECTF(2.0, 2.0, 2.0, 2.0);
			documentWrapper.PropertyChanged += this.DocumentWrapper_PropertyChanged;
			this.IsOcrTipsVisible = false;
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x000525D7 File Offset: 0x000507D7
		public bool IsSearchEnabled
		{
			get
			{
				return this.document != null;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600152C RID: 5420 RVA: 0x000525E2 File Offset: 0x000507E2
		// (set) Token: 0x0600152D RID: 5421 RVA: 0x000525EA File Offset: 0x000507EA
		public bool IsSearchVisible
		{
			get
			{
				return this.isSearchVisible;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.isSearchVisible, value, "IsSearchVisible") && value)
				{
					this.DoSearch();
				}
			}
		}

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x0600152E RID: 5422 RVA: 0x00052609 File Offset: 0x00050809
		// (set) Token: 0x0600152F RID: 5423 RVA: 0x00052611 File Offset: 0x00050811
		public string SearchText
		{
			get
			{
				return this.searchText;
			}
			set
			{
				base.SetProperty<string>(ref this.searchText, value, "SearchText");
				this.DoSearch();
			}
		}

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06001530 RID: 5424 RVA: 0x0005262C File Offset: 0x0005082C
		// (set) Token: 0x06001531 RID: 5425 RVA: 0x00052634 File Offset: 0x00050834
		public FindFlags SearchFlag
		{
			get
			{
				return this.searchFlags;
			}
			private set
			{
				base.SetProperty<FindFlags>(ref this.searchFlags, value, "SearchFlag");
				this.DoSearch();
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x0005264F File Offset: 0x0005084F
		// (set) Token: 0x06001533 RID: 5427 RVA: 0x0005265C File Offset: 0x0005085C
		public bool MatchCase
		{
			get
			{
				return (this.SearchFlag & FindFlags.MatchCase) > FindFlags.None;
			}
			set
			{
				if (this.MatchCase != value)
				{
					base.OnPropertyChanging("MatchCase");
					if (value)
					{
						this.SearchFlag |= FindFlags.MatchCase;
					}
					else
					{
						this.SearchFlag &= ~FindFlags.MatchCase;
					}
					base.OnPropertyChanged("MatchCase");
				}
			}
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06001534 RID: 5428 RVA: 0x000526AA File Offset: 0x000508AA
		// (set) Token: 0x06001535 RID: 5429 RVA: 0x000526B8 File Offset: 0x000508B8
		public bool MatchWholeWord
		{
			get
			{
				return (this.SearchFlag & FindFlags.MatchWholeWord) > FindFlags.None;
			}
			set
			{
				if (this.MatchWholeWord != value)
				{
					base.OnPropertyChanging("MatchWholeWord");
					if (value)
					{
						this.SearchFlag |= FindFlags.MatchWholeWord;
					}
					else
					{
						this.SearchFlag &= ~FindFlags.MatchWholeWord;
					}
					base.OnPropertyChanged("MatchWholeWord");
				}
			}
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x00052706 File Offset: 0x00050906
		public Color HighlightColor { get; }

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x0005270E File Offset: 0x0005090E
		public FS_RECTF InflateHighlight { get; }

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06001538 RID: 5432 RVA: 0x00052716 File Offset: 0x00050916
		public Color ActiveRecordColor { get; }

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x0005271E File Offset: 0x0005091E
		// (set) Token: 0x0600153A RID: 5434 RVA: 0x00052728 File Offset: 0x00050928
		public int TotalRecords
		{
			get
			{
				return this.totalRecords;
			}
			private set
			{
				int num = value;
				if (num < 0)
				{
					num = 0;
				}
				if (base.SetProperty<int>(ref this.totalRecords, num, "TotalRecords"))
				{
					if (this.CurrentRecord > this.totalRecords)
					{
						this.scrollToRecord = false;
						this.CurrentRecord = this.totalRecords;
						this.scrollToRecord = true;
					}
					if (num > 0 && this.currentRecord == 0)
					{
						this.CurrentRecord = 1;
					}
					this.SearchUpCmd.NotifyCanExecuteChanged();
					this.SearchDownCmd.NotifyCanExecuteChanged();
				}
			}
		}

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x000527A2 File Offset: 0x000509A2
		// (set) Token: 0x0600153C RID: 5436 RVA: 0x000527AA File Offset: 0x000509AA
		public int CurrentRecord
		{
			get
			{
				return this.currentRecord;
			}
			private set
			{
				base.SetProperty<int>(ref this.currentRecord, value, "CurrentRecord");
				this.OnCurrentRecordChanged(this.CurrentRecord, this.TotalRecords);
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x000527D1 File Offset: 0x000509D1
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x000527D9 File Offset: 0x000509D9
		public double Progress
		{
			get
			{
				return this.progress;
			}
			private set
			{
				base.SetProperty<double>(ref this.progress, value, "Progress");
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x000527EE File Offset: 0x000509EE
		// (set) Token: 0x06001540 RID: 5440 RVA: 0x000527F6 File Offset: 0x000509F6
		public bool IsOcrTipsVisible
		{
			get
			{
				return this.isOcrTipsVisible;
			}
			private set
			{
				base.SetProperty<bool>(ref this.isOcrTipsVisible, value, "IsOcrTipsVisible");
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0005280B File Offset: 0x00050A0B
		protected virtual void OnCurrentRecordChanged(int currentRecord, int totalRecords)
		{
			if (totalRecords <= 0 || currentRecord <= 0 || currentRecord > totalRecords)
			{
				return;
			}
			if (this.scrollToRecord)
			{
				this.ScrollToRecord(currentRecord);
			}
			this.HighlightRecord(this.prevRecord, currentRecord);
			this.prevRecord = currentRecord;
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00052840 File Offset: 0x00050A40
		private void ScrollToRecord(int currentRecord)
		{
			if (currentRecord < 1 || currentRecord > this.foundText.Count)
			{
				return;
			}
			PdfSearch.FoundText foundText = this.foundText[currentRecord - 1];
			PdfViewer pdfViewer = this.GetPdfViewer();
			if (pdfViewer == null)
			{
				return;
			}
			pdfViewer.CurrentIndex = foundText.PageIndex;
			List<Int32Rect> highlightedRects = pdfViewer.GetHighlightedRects(foundText.PageIndex, new HighlightInfo
			{
				CharIndex = foundText.CharIndex,
				CharsCount = foundText.CharsCount,
				Inflate = this.InflateHighlight
			});
			if (highlightedRects.Count > 0)
			{
				DpiScale dpiScale = VisualTreeHelper.GetDpi(pdfViewer);
				Rect[] array = highlightedRects.Select((Int32Rect c) => new Rect((double)c.X / dpiScale.PixelsPerDip, (double)c.Y / dpiScale.PixelsPerDip, (double)c.Width / dpiScale.PixelsPerDip, (double)c.Height / dpiScale.PixelsPerDip)).ToArray<Rect>();
				Rect rect = new Rect(0.0, 0.0, pdfViewer.ViewportWidth, pdfViewer.ViewportHeight);
				Rect rect2 = new Rect(array[0].X, array[0].Y, array[0].Width, array[0].Height);
				if (array.Length != 0 && !rect.Contains(rect2))
				{
					Point point;
					if (pdfViewer.TryGetPagePoint(foundText.PageIndex, new Point(array[0].X, array[0].Y), out point))
					{
						pdfViewer.ScrollToPoint(foundText.PageIndex, point);
						return;
					}
					pdfViewer.ScrollToPage(foundText.PageIndex);
					pdfViewer.UpdateLayout();
					if (pdfViewer.TryGetPagePoint(foundText.PageIndex, new Point(array[0].X, array[0].Y), out point))
					{
						pdfViewer.ScrollToPoint(foundText.PageIndex, point);
					}
				}
			}
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00052A00 File Offset: 0x00050C00
		private void HighlightRecord(int prevRecord, int currentRecord)
		{
			PdfViewer pdfViewer = this.GetPdfViewer();
			if (pdfViewer == null)
			{
				return;
			}
			if (prevRecord >= 1 && prevRecord <= this.foundText.Count)
			{
				PdfSearch.FoundText foundText = this.foundText[prevRecord - 1];
				pdfViewer.HighlightText(foundText.PageIndex, foundText.CharIndex, foundText.CharsCount, this.HighlightColor, this.InflateHighlight);
			}
			if (currentRecord >= 1 && currentRecord <= this.foundText.Count)
			{
				PdfSearch.FoundText foundText2 = this.foundText[currentRecord - 1];
				pdfViewer.HighlightText(foundText2.PageIndex, foundText2.CharIndex, foundText2.CharsCount, this.ActiveRecordColor, this.InflateHighlight);
			}
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00052AA2 File Offset: 0x00050CA2
		private void DoSearch()
		{
			if (this.document == null || this.document.Pages == null)
			{
				return;
			}
			this.StartSearch();
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x00052AC0 File Offset: 0x00050CC0
		private void StartSearch()
		{
			if (!this.IsSearchEnabled || !this.IsSearchVisible)
			{
				return;
			}
			this.StopSearch();
			if (string.IsNullOrEmpty(this.searchText))
			{
				return;
			}
			this.prevRecord = -1;
			this.searchPageIndex = 0;
			PdfDocument pdfDocument = this.document;
			int? num;
			if (pdfDocument == null)
			{
				num = null;
			}
			else
			{
				PdfPageCollection pages = pdfDocument.Pages;
				num = ((pages != null) ? new int?(pages.CurrentIndex) : null);
			}
			int? num2 = num;
			this.searchStartPage = num2.GetValueOrDefault(-1);
			this.autoSelectDisabled = false;
			this.minAfterStartPage = -1;
			if (this.document != null)
			{
				this.document.Pages.CurrentPageChanged -= this.Pages_CurrentPageChanged;
				this.document.Pages.CurrentPageChanged += this.Pages_CurrentPageChanged;
			}
			this.searchTimer.Start();
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x00052B9B File Offset: 0x00050D9B
		private void Pages_CurrentPageChanged(object sender, EventArgs e)
		{
			this.autoSelectDisabled = true;
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x00052BA4 File Offset: 0x00050DA4
		private void StopSearch()
		{
			if (!this.IsSearchEnabled)
			{
				return;
			}
			if (this.document != null)
			{
				this.document.Pages.CurrentPageChanged -= this.Pages_CurrentPageChanged;
			}
			this.Progress = 0.0;
			this.searchPageIndex = -1;
			this.searchStartPage = -1;
			this.minAfterStartPage = -1;
			this.autoSelectDisabled = false;
			this.searchTimer.Stop();
			this.foundText.Clear();
			this.forHighlight.Clear();
			PdfViewer pdfViewer = this.GetPdfViewer();
			if (pdfViewer != null && pdfViewer.Document != null)
			{
				pdfViewer.RemoveHighlightFromText();
			}
			this.CurrentRecord = 0;
			this.TotalRecords = 0;
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x00052C50 File Offset: 0x00050E50
		private void SearchTimer_Tick(object sender, EventArgs e)
		{
			if (this.searchPageIndex < 0)
			{
				this.searchTimer.Stop();
				this.Progress = 0.0;
				return;
			}
			PdfDocument pdfDocument = this.document;
			int count = pdfDocument.Pages.Count;
			if (this.searchPageIndex >= count)
			{
				this.searchTimer.Stop();
				this.Progress = 1.0;
				return;
			}
			IntPtr intPtr = Pdfium.FPDF_LoadPage(pdfDocument.Handle, this.searchPageIndex);
			if (intPtr == IntPtr.Zero)
			{
				this.searchTimer.Stop();
				this.Progress = 1.0;
				return;
			}
			IntPtr intPtr2 = Pdfium.FPDFText_LoadPage(intPtr);
			if (intPtr2 == IntPtr.Zero)
			{
				this.searchTimer.Stop();
				this.Progress = 1.0;
				return;
			}
			IntPtr intPtr3 = Pdfium.FPDFText_FindStart(intPtr2, this.searchText, this.searchFlags, 0);
			if (intPtr3 == IntPtr.Zero)
			{
				this.searchTimer.Stop();
				this.Progress = 1.0;
				return;
			}
			while (Pdfium.FPDFText_FindNext(intPtr3))
			{
				int num = Pdfium.FPDFText_GetSchResultIndex(intPtr3);
				int num2 = Pdfium.FPDFText_GetSchCount(intPtr3);
				PdfSearch.FoundText foundText = new PdfSearch.FoundText
				{
					CharIndex = num,
					CharsCount = num2,
					PageIndex = this.searchPageIndex
				};
				this.foundText.Add(foundText);
				this.forHighlight.Add(foundText);
				if (this.searchStartPage != -2 && this.minAfterStartPage == -1 && this.searchPageIndex >= this.searchStartPage)
				{
					this.minAfterStartPage = this.searchPageIndex;
				}
			}
			Pdfium.FPDFText_FindClose(intPtr3);
			Pdfium.FPDFText_ClosePage(intPtr2);
			Pdfium.FPDF_ClosePage(intPtr);
			this.UpdateResults();
			this.Progress = Math.Max(this.Progress, (double)this.searchPageIndex * 1.0 / (double)count);
			this.searchPageIndex++;
			if (this.searchStartPage != -2)
			{
				if (this.minAfterStartPage != -1)
				{
					int num3 = 0;
					while (num3 < this.foundText.Count && this.foundText[num3].PageIndex != this.minAfterStartPage)
					{
						num3++;
					}
					if (num3 < this.foundText.Count)
					{
						this.searchStartPage = -2;
						this.scrollToRecord = !this.autoSelectDisabled;
						this.CurrentRecord = num3 + 1;
						this.scrollToRecord = true;
						return;
					}
				}
				else
				{
					int num4 = this.searchPageIndex;
					PdfDocument pdfDocument2 = this.document;
					int? num5;
					if (pdfDocument2 == null)
					{
						num5 = null;
					}
					else
					{
						PdfPageCollection pages = pdfDocument2.Pages;
						num5 = ((pages != null) ? new int?(pages.Count) : null);
					}
					int? num6 = num5;
					if ((num4 == num6.GetValueOrDefault()) & (num6 != null))
					{
						this.scrollToRecord = !this.autoSelectDisabled;
						this.CurrentRecord = ((this.TotalRecords != 0) ? 1 : 0);
						this.scrollToRecord = true;
					}
				}
			}
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x00052F38 File Offset: 0x00051138
		private void UpdateResults()
		{
			if (this.foundText != null)
			{
				this.scrollToRecord = false;
				this.TotalRecords = this.foundText.Count;
				this.scrollToRecord = true;
			}
			PdfViewer pdfViewer = this.GetPdfViewer();
			if (pdfViewer != null && pdfViewer.Document != null)
			{
				foreach (PdfSearch.FoundText foundText in this.forHighlight)
				{
					HighlightInfo highlightInfo = new HighlightInfo
					{
						CharIndex = foundText.CharIndex,
						CharsCount = foundText.CharsCount,
						Color = this.HighlightColor,
						Inflate = this.InflateHighlight
					};
					if (!pdfViewer.HighlightedTextInfo.ContainsKey(foundText.PageIndex))
					{
						pdfViewer.HighlightedTextInfo.Add(foundText.PageIndex, new List<HighlightInfo>());
					}
					pdfViewer.HighlightedTextInfo[foundText.PageIndex].Add(highlightInfo);
				}
				this.forHighlight.Clear();
			}
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x00053054 File Offset: 0x00051254
		public RelayCommand SearchDownCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.searchDownCmd) == null)
				{
					relayCommand = (this.searchDownCmd = new RelayCommand(delegate
					{
						this.SearchDown();
					}, () => this.CanSearchDown()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00053094 File Offset: 0x00051294
		private void SearchDown()
		{
			if (this.document == null || this.document.Pages == null)
			{
				return;
			}
			if (this.TotalRecords <= 0)
			{
				this.TotalRecords = 0;
				this.CurrentRecord = 0;
				return;
			}
			this.searchStartPage = -2;
			if (this.CurrentRecord < this.TotalRecords)
			{
				int num = this.CurrentRecord;
				this.CurrentRecord = num + 1;
				return;
			}
			this.CurrentRecord = 1;
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x000530FD File Offset: 0x000512FD
		private bool CanSearchDown()
		{
			return this.document != null && this.TotalRecords > 0;
		}

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x0600154D RID: 5453 RVA: 0x00053114 File Offset: 0x00051314
		public RelayCommand SearchUpCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.searchUpCmd) == null)
				{
					relayCommand = (this.searchUpCmd = new RelayCommand(delegate
					{
						this.SearchUp();
					}, () => this.CanSearchUp()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00053154 File Offset: 0x00051354
		private void SearchUp()
		{
			if (this.document == null || this.document.Pages == null)
			{
				return;
			}
			if (this.TotalRecords <= 0)
			{
				this.TotalRecords = 0;
				this.CurrentRecord = 0;
				return;
			}
			this.searchStartPage = -2;
			if (this.CurrentRecord > 1)
			{
				int num = this.CurrentRecord;
				this.CurrentRecord = num - 1;
				return;
			}
			this.CurrentRecord = this.TotalRecords;
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000531BD File Offset: 0x000513BD
		private bool CanSearchUp()
		{
			return this.document != null && this.TotalRecords > 0;
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x000531D2 File Offset: 0x000513D2
		private PdfViewer GetPdfViewer()
		{
			if (this.document == null)
			{
				return null;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.document);
			if (pdfControl == null)
			{
				return null;
			}
			return pdfControl.Viewer;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x000531F4 File Offset: 0x000513F4
		private void DocumentWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DocumentContentType")
			{
				this.UpdateOcrDocumentTips();
			}
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x00053210 File Offset: 0x00051410
		private async Task UpdateOcrDocumentTips()
		{
			bool flag = false;
			DispatcherTimer dispatcherTimer = this.searchTimer;
			if ((dispatcherTimer == null || !dispatcherTimer.IsEnabled) && this.document != null && this.documentWrapper.DocumentContentType == PdfContentType.ImageOrPath && !this.isOcrTipsClosed)
			{
				TaskAwaiter<string> taskAwaiter = ConfigManager.GetDocumentPropertiesAsync(this.documentWrapper.DocumentPath, "IgnoreSearchRecognitionTip", default(CancellationToken)).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<string> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<string>);
				}
				if (!(taskAwaiter.GetResult() == "1"))
				{
					flag = true;
				}
			}
			this.IsOcrTipsVisible = flag;
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x00053254 File Offset: 0x00051454
		public async void CloseRecognitionTip(bool doNotShowAgain)
		{
			this.isOcrTipsClosed = true;
			if (doNotShowAgain)
			{
				GAManager.SendEvent("OCR", "SearchNotShow", "Count", 1L);
			}
			await this.UpdateOcrDocumentTips();
			if (doNotShowAgain)
			{
				await ConfigManager.SetDocumentPropertiesAsync(this.documentWrapper.DocumentPath, "IgnoreSearchRecognitionTip", "1");
			}
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x00053294 File Offset: 0x00051494
		public void Dispose()
		{
			this.documentWrapper.PropertyChanged -= this.DocumentWrapper_PropertyChanged;
			this.StopSearch();
			if (this.document != null)
			{
				this.document = null;
			}
			base.OnPropertyChanged("IsSearchEnabled");
			if (this.searchTimer != null)
			{
				this.searchTimer.Tick -= this.SearchTimer_Tick;
				this.searchTimer = null;
			}
		}

		// Token: 0x04000700 RID: 1792
		private readonly DocumentWrapper documentWrapper;

		// Token: 0x04000701 RID: 1793
		private PdfDocument document;

		// Token: 0x04000702 RID: 1794
		private bool isSearchVisible;

		// Token: 0x04000703 RID: 1795
		private List<PdfSearch.FoundText> foundText = new List<PdfSearch.FoundText>();

		// Token: 0x04000704 RID: 1796
		private List<PdfSearch.FoundText> forHighlight = new List<PdfSearch.FoundText>();

		// Token: 0x04000705 RID: 1797
		private DispatcherTimer searchTimer;

		// Token: 0x04000706 RID: 1798
		private string searchText = string.Empty;

		// Token: 0x04000707 RID: 1799
		private FindFlags searchFlags;

		// Token: 0x04000708 RID: 1800
		private int prevRecord;

		// Token: 0x04000709 RID: 1801
		private int searchPageIndex;

		// Token: 0x0400070A RID: 1802
		private int totalRecords;

		// Token: 0x0400070B RID: 1803
		private int currentRecord;

		// Token: 0x0400070C RID: 1804
		private RelayCommand searchDownCmd;

		// Token: 0x0400070D RID: 1805
		private RelayCommand searchUpCmd;

		// Token: 0x0400070E RID: 1806
		private double progress;

		// Token: 0x0400070F RID: 1807
		private int searchStartPage = -1;

		// Token: 0x04000710 RID: 1808
		private int minAfterStartPage = -1;

		// Token: 0x04000711 RID: 1809
		private bool scrollToRecord = true;

		// Token: 0x04000712 RID: 1810
		private bool autoSelectDisabled;

		// Token: 0x04000713 RID: 1811
		private bool isOcrTipsClosed;

		// Token: 0x04000714 RID: 1812
		private bool isOcrTipsVisible;
	}
}
