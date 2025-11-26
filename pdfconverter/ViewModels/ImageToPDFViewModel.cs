using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using pdfconverter.Controls;
using pdfconverter.Models;
using pdfconverter.Models.Image;
using pdfconverter.Properties;
using pdfconverter.Utils;
using pdfconverter.Utils.Image;
using pdfconverter.Views;
using PDFKit.GenerateImagePdf;

namespace pdfconverter.ViewModels
{
	// Token: 0x02000031 RID: 49
	public class ImageToPDFViewModel : ObservableObject
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0000D4EC File Offset: 0x0000B6EC
		public bool AddImage
		{
			get
			{
				ObservableCollection<ToimagePage> pageList = this.PageList;
				return pageList != null && pageList.Count < 1;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000D502 File Offset: 0x0000B702
		public List<KeyValuePair<PageZoomLevel, string>> ZoomList { get; } = new List<KeyValuePair<PageZoomLevel, string>>(EnumHelper.GetDescriptionDictionary<PageZoomLevel>());

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0000D50A File Offset: 0x0000B70A
		// (set) Token: 0x06000341 RID: 833 RVA: 0x0000D512 File Offset: 0x0000B712
		public ObservableCollection<ToimagePage> PageList { get; set; } = new ObservableCollection<ToimagePage>();

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000342 RID: 834 RVA: 0x0000D51B File Offset: 0x0000B71B
		public int PageCount
		{
			get
			{
				return this.PageList.Count;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000D528 File Offset: 0x0000B728
		// (set) Token: 0x06000344 RID: 836 RVA: 0x0000D530 File Offset: 0x0000B730
		public bool IsNeedConfirmOnClose { get; set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000D539 File Offset: 0x0000B739
		// (set) Token: 0x06000346 RID: 838 RVA: 0x0000D541 File Offset: 0x0000B741
		public ImageToPDFViewModel.DropHandler DropTarget { get; private set; }

		// Token: 0x06000347 RID: 839 RVA: 0x0000D54C File Offset: 0x0000B74C
		public ImageToPDFViewModel()
		{
			this.InitializeEnv();
			this.PageList.CollectionChanged += this.OnPageListCollectionChanged;
			PageService.PageAdded += delegate(ToimagePage x)
			{
				ImageDispatcherHelper.Invoke(delegate
				{
					this.PageList.Add(x);
				});
				if (x.ImageBitmap != null)
				{
					ImageDispatcherHelper.Invoke(delegate
					{
						this.selectedItems.Add(x);
					});
				}
				this.IsNeedConfirmOnClose = true;
			};
			this.DropTarget = new ImageToPDFViewModel.DropHandler(this);
			this.imageService.PreviewImageChanged += this.OnPreviewImageChanged;
			this.imageService.PreviewImageChanged2 += this.OnPreviewImageChanged2;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0000D643 File Offset: 0x0000B843
		private void OnPageListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.OnPropertyChanged("PageCount");
			this.UpdateCheckedAll();
			if (!this.isRemovePages)
			{
				this.RefreshList();
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000D664 File Offset: 0x0000B864
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0000D66C File Offset: 0x0000B86C
		public PageSizeItem PDFPageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				base.SetProperty<PageSizeItem>(ref this.pageSize, value, "PDFPageSize");
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000D681 File Offset: 0x0000B881
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0000D689 File Offset: 0x0000B889
		public PageMarginItem ContentMargin
		{
			get
			{
				return this.contentMargin;
			}
			set
			{
				base.SetProperty<PageMarginItem>(ref this.contentMargin, value, "ContentMargin");
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0000D69E File Offset: 0x0000B89E
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0000D6A6 File Offset: 0x0000B8A6
		public PageSaveItem SaveFile
		{
			get
			{
				return this.saveFile;
			}
			set
			{
				base.SetProperty<PageSaveItem>(ref this.saveFile, value, "SaveFile");
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600034F RID: 847 RVA: 0x0000D6BB File Offset: 0x0000B8BB
		// (set) Token: 0x06000350 RID: 848 RVA: 0x0000D6C3 File Offset: 0x0000B8C3
		public ImagePdfGenerateSettings _ImagePdfGenerateSettings
		{
			get
			{
				return this._imagePdfGenerateSettings;
			}
			set
			{
				base.SetProperty<ImagePdfGenerateSettings>(ref this._imagePdfGenerateSettings, value, "_ImagePdfGenerateSettings");
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
		// (set) Token: 0x06000352 RID: 850 RVA: 0x0000D6E0 File Offset: 0x0000B8E0
		public string OutputPath
		{
			get
			{
				return this._OutputPath;
			}
			set
			{
				base.SetProperty<string>(ref this._OutputPath, value, "OutputPath");
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0000D6F5 File Offset: 0x0000B8F5
		// (set) Token: 0x06000354 RID: 852 RVA: 0x0000D700 File Offset: 0x0000B900
		public bool? OurputInOneFile
		{
			get
			{
				return this._OutputInOneFile;
			}
			set
			{
				if (value.GetValueOrDefault())
				{
					foreach (ToPDFFileItem toPDFFileItem in this.FileList)
					{
						toPDFFileItem.Status = ToPDFItemStatus.Loaded;
					}
				}
				base.SetProperty<bool?>(ref this._OutputInOneFile, value, "OurputInOneFile");
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000D768 File Offset: 0x0000B968
		// (set) Token: 0x06000356 RID: 854 RVA: 0x0000D770 File Offset: 0x0000B970
		public string OutputFileFullName
		{
			get
			{
				return this._OutputFileFullName;
			}
			set
			{
				base.SetProperty<string>(ref this._OutputFileFullName, value, "OutputFileFullName");
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000D785 File Offset: 0x0000B985
		private void OnPreviewImageChanged(ToimagePage page, Bitmap bitmap, bool isAdjust)
		{
			ImageViewerSource source = new ImageViewerSource(bitmap, (double)page.Rotate);
			ImageDispatcherHelper.Invoke(delegate
			{
				this.PreviewSource = source;
			});
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000D7B8 File Offset: 0x0000B9B8
		private void OnPreviewImageChanged2(ToimagePage page, ImageSource imageSource, bool isAdjust)
		{
			ImageViewerSource source = new ImageViewerSource(imageSource, (double)page.Rotate);
			ImageDispatcherHelper.Invoke(delegate
			{
				this.PreviewSource = source;
			});
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0000D7EC File Offset: 0x0000B9EC
		private void InitializeEnv()
		{
			LongPathDirectory longPathDirectory = ConfigManager.GetConvertPath();
			try
			{
				if (string.IsNullOrEmpty(longPathDirectory))
				{
					longPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PDFgear");
				}
				if (!Directory.Exists(longPathDirectory))
				{
					Directory.CreateDirectory(longPathDirectory);
				}
			}
			catch
			{
			}
			if (!string.IsNullOrEmpty(longPathDirectory) && Directory.Exists(longPathDirectory))
			{
				this.OutputPath = longPathDirectory.FullPathWithoutPrefix;
			}
			if (App.convertType.Equals(ConvToPDFType.ImageToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text in App.selectedFile)
				{
				}
			}
			if (this.pageSizeItems.Count == 0)
			{
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeMatchSource, pdfconverter.Models.PDFPageSize.MatchSource));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA4Portrait, pdfconverter.Models.PDFPageSize.A4_Portrait));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA4Landscape, pdfconverter.Models.PDFPageSize.A4_Landscape));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA3Portrait, pdfconverter.Models.PDFPageSize.A3_Portrait));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA3Landscape, pdfconverter.Models.PDFPageSize.A3_Landscape));
			}
			if (this.contentMargins.Count == 0)
			{
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsNoMargin, pdfconverter.Models.ContentMargin.NoMargin));
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsSmallMargin, pdfconverter.Models.ContentMargin.SmallMargin));
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsBigMargin, pdfconverter.Models.ContentMargin.BigMargin));
			}
			if (this.saveFileWays.Count == 0)
			{
				this.saveFileWays.Add(new PageSaveItem(Resources.ImageToPDFWinSinglePDF, SaveFileWay.Single));
				this.saveFileWays.Add(new PageSaveItem(Resources.ImageToPDFWinMultiplePDF, SaveFileWay.Multiple));
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600035A RID: 858 RVA: 0x0000D9B8 File Offset: 0x0000BBB8
		public List<PageSizeItem> PageSizeItems
		{
			get
			{
				return this.pageSizeItems;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000D9C0 File Offset: 0x0000BBC0
		public List<PageMarginItem> PageMarginItems
		{
			get
			{
				return this.contentMargins;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0000D9C8 File Offset: 0x0000BBC8
		public List<PageSaveItem> SaveFileWays
		{
			get
			{
				return this.saveFileWays;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0000D9D0 File Offset: 0x0000BBD0
		public ToPDFFileItemCollection FileList
		{
			get
			{
				ToPDFFileItemCollection toPDFFileItemCollection;
				if ((toPDFFileItemCollection = this._toPDFItemList) == null)
				{
					toPDFFileItemCollection = (this._toPDFItemList = new ToPDFFileItemCollection());
				}
				return toPDFFileItemCollection;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0000D9F5 File Offset: 0x0000BBF5
		// (set) Token: 0x0600035F RID: 863 RVA: 0x0000D9FD File Offset: 0x0000BBFD
		public ToPDFFileItem SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				base.SetProperty<ToPDFFileItem>(ref this._selectedItem, value, "SelectedItem");
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0000DA12 File Offset: 0x0000BC12
		// (set) Token: 0x06000361 RID: 865 RVA: 0x0000DA1A File Offset: 0x0000BC1A
		public Visibility Visibility
		{
			get
			{
				return this.visibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.visibility, value, "Visibility");
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0000DA2F File Offset: 0x0000BC2F
		// (set) Token: 0x06000363 RID: 867 RVA: 0x0000DA37 File Offset: 0x0000BC37
		public bool? IsCheckedAll
		{
			get
			{
				return this.isCheckedAll;
			}
			set
			{
				if (base.SetProperty<bool?>(ref this.isCheckedAll, value, "IsCheckedAll"))
				{
					this.OnIsCheckedAllChanged();
				}
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000364 RID: 868 RVA: 0x0000DA53 File Offset: 0x0000BC53
		// (set) Token: 0x06000365 RID: 869 RVA: 0x0000DA5B File Offset: 0x0000BC5B
		public ImageViewerSource PreviewSource
		{
			get
			{
				return this.previewSource;
			}
			set
			{
				if (base.SetProperty<ImageViewerSource>(ref this.previewSource, value, "PreviewSource"))
				{
					ImageViewerSource imageViewerSource = this.previewSource;
					this.PreviewPageRotate = ((imageViewerSource != null) ? imageViewerSource.Rotate : 0.0);
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000366 RID: 870 RVA: 0x0000DA91 File Offset: 0x0000BC91
		// (set) Token: 0x06000367 RID: 871 RVA: 0x0000DA99 File Offset: 0x0000BC99
		public string[] DragFiles
		{
			get
			{
				return this.dragFiles;
			}
			set
			{
				base.SetProperty<string[]>(ref this.dragFiles, value, "DragFiles");
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0000DAB0 File Offset: 0x0000BCB0
		public AsyncRelayCommand ImportCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.importCommand) == null)
				{
					asyncRelayCommand = (this.importCommand = new AsyncRelayCommand(new Func<Task>(this.Import), new Func<bool>(this.CanImport)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000369 RID: 873 RVA: 0x0000DAF0 File Offset: 0x0000BCF0
		public AsyncRelayCommand<string> RotateCommand
		{
			get
			{
				AsyncRelayCommand<string> asyncRelayCommand;
				if ((asyncRelayCommand = this.rotateCommand) == null)
				{
					asyncRelayCommand = (this.rotateCommand = new AsyncRelayCommand<string>(new Func<string, Task>(this.Rotate), new Predicate<string>(this.CanRotate)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600036A RID: 874 RVA: 0x0000DB30 File Offset: 0x0000BD30
		public AsyncRelayCommand<string> RotatePreviewCommand
		{
			get
			{
				AsyncRelayCommand<string> asyncRelayCommand;
				if ((asyncRelayCommand = this.rotatePreviewCommand) == null)
				{
					asyncRelayCommand = (this.rotatePreviewCommand = new AsyncRelayCommand<string>(new Func<string, Task>(this.RotatePreview), new Predicate<string>(this.CanRotatePreview)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600036B RID: 875 RVA: 0x0000DB70 File Offset: 0x0000BD70
		public AsyncRelayCommand DeleteCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteCommand) == null)
				{
					asyncRelayCommand = (this.deleteCommand = new AsyncRelayCommand(new Func<Task>(this.Delete), new Func<bool>(this.CanDelete)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600036C RID: 876 RVA: 0x0000DBB0 File Offset: 0x0000BDB0
		public AsyncRelayCommand DeletePreviewCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.deletePreviewCommand) == null)
				{
					asyncRelayCommand = (this.deletePreviewCommand = new AsyncRelayCommand(new Func<Task>(this.DeletePreview), new Func<bool>(this.CanDeletePreview)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600036D RID: 877 RVA: 0x0000DBF0 File Offset: 0x0000BDF0
		public AsyncRelayCommand<object> PreviewCommand
		{
			get
			{
				AsyncRelayCommand<object> asyncRelayCommand;
				if ((asyncRelayCommand = this.previewCommand) == null)
				{
					asyncRelayCommand = (this.previewCommand = new AsyncRelayCommand<object>(new Func<object, Task>(this.Preview)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600036E RID: 878 RVA: 0x0000DC24 File Offset: 0x0000BE24
		public AsyncRelayCommand PreviousPageCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.previousPageCommand) == null)
				{
					asyncRelayCommand = (this.previousPageCommand = new AsyncRelayCommand(new Func<Task>(this.PreviousPage), new Func<bool>(this.CanPreviousPage)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600036F RID: 879 RVA: 0x0000DC64 File Offset: 0x0000BE64
		public AsyncRelayCommand NextPageCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.nextPageCommand) == null)
				{
					asyncRelayCommand = (this.nextPageCommand = new AsyncRelayCommand(new Func<Task>(this.NextPage), new Func<bool>(this.CanNextPage)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000370 RID: 880 RVA: 0x0000DCA4 File Offset: 0x0000BEA4
		public AsyncRelayCommand OpenCropCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.openCropCommand) == null)
				{
					asyncRelayCommand = (this.openCropCommand = new AsyncRelayCommand(new Func<Task>(this.OpenCrop)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0000DCD8 File Offset: 0x0000BED8
		public AsyncRelayCommand<Rect> CropCommand
		{
			get
			{
				AsyncRelayCommand<Rect> asyncRelayCommand;
				if ((asyncRelayCommand = this.cropCommand) == null)
				{
					asyncRelayCommand = (this.cropCommand = new AsyncRelayCommand<Rect>(new Func<Rect, Task>(this.Crop)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0000DD0C File Offset: 0x0000BF0C
		public AsyncRelayCommand RevertCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.revertCommand) == null)
				{
					asyncRelayCommand = (this.revertCommand = new AsyncRelayCommand(new Func<Task>(this.Revert)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0000DD40 File Offset: 0x0000BF40
		public RelayCommand SelectPath
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.selectPath) == null)
				{
					relayCommand = (this.selectPath = new RelayCommand(delegate
					{
						string text = ConvertManager.selectOutputFolder(this.OutputPath);
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.OutputPath = text;
							ConfigManager.SetConvertPath(this.OutputPath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0000DD74 File Offset: 0x0000BF74
		public RelayCommand<ToPDFFileItem> OpenInExplorer
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						string filePath = model.FilePath;
						if (LongPathFile.Exists(filePath))
						{
							UtilsManager.OpenFileInExplore(filePath, true);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000375 RID: 885 RVA: 0x0000DDB8 File Offset: 0x0000BFB8
		public RelayCommand<ToPDFFileItem> OpenWithEditor
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openWithEditor) == null)
				{
					relayCommand = (this.openWithEditor = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						string filePath = model.FilePath;
						if (LongPathFile.Exists(filePath))
						{
							UtilsManager.OpenFile(filePath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0000DDFC File Offset: 0x0000BFFC
		public RelayCommand OpenOneFileInExplorer
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openOneFileInExplorer) == null)
				{
					relayCommand = (this.openOneFileInExplorer = new RelayCommand(delegate
					{
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0000DE40 File Offset: 0x0000C040
		// (set) Token: 0x06000378 RID: 888 RVA: 0x0000DE48 File Offset: 0x0000C048
		public double PreviewPageRotate
		{
			get
			{
				return this.previewPageRotate;
			}
			set
			{
				if (base.SetProperty<double>(ref this.previewPageRotate, value, "PreviewPageRotate") && this.PreviewSource != null)
				{
					this.PreviewSource.Rotate = this.previewPageRotate;
				}
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0000DE78 File Offset: 0x0000C078
		public string PageText
		{
			get
			{
				int num = this.PageList.IndexOf(this.PreviewPage);
				if (num >= 0)
				{
					return string.Format("{0}/{1}", num + 1, this.PageList.Count);
				}
				return string.Empty;
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000DEC4 File Offset: 0x0000C0C4
		private async Task Import()
		{
			ImageToPDFViewModel.<>c__DisplayClass143_0 CS$<>8__locals1 = new ImageToPDFViewModel.<>c__DisplayClass143_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.CanImport())
			{
				this.Visibility = Visibility.Visible;
				GAManager.SendEvent("ImageToPDFWindow", "ImportBtn", "Count", 1L);
				CS$<>8__locals1.filePaths = ImageToPDFViewModel.ShowImportDialog();
				if (CS$<>8__locals1.filePaths != null)
				{
					ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
					{
						ImageToPDFViewModel.<>c__DisplayClass143_0.<<Import>b__0>d <<Import>b__0>d;
						<<Import>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<Import>b__0>d.<>4__this = CS$<>8__locals1;
						<<Import>b__0>d.c = c;
						<<Import>b__0>d.<>1__state = -1;
						<<Import>b__0>d.<>t__builder.Start<ImageToPDFViewModel.<>c__DisplayClass143_0.<<Import>b__0>d>(ref <<Import>b__0>d);
						return <<Import>b__0>d.<>t__builder.Task;
					}, Resources.WinListHeaderImageToPDFText, Resources.ImageToPDFWinImportContent, false, null, 0);
				}
				this.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600037B RID: 891 RVA: 0x0000DF07 File Offset: 0x0000C107
		// (set) Token: 0x0600037C RID: 892 RVA: 0x0000DF10 File Offset: 0x0000C110
		public int PageNumber
		{
			get
			{
				return this.pageNumber;
			}
			set
			{
				if (base.SetProperty<int>(ref this.pageNumber, value, "PageNumber"))
				{
					int index = this.pageNumber - 1;
					if (index != this.PageList.IndexOf(this.PreviewPage))
					{
						Application.Current.Dispatcher.InvokeAsync(delegate
						{
							this.UpdatePreviewPage(index);
						});
					}
				}
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600037D RID: 893 RVA: 0x0000DF81 File Offset: 0x0000C181
		// (set) Token: 0x0600037E RID: 894 RVA: 0x0000DF8C File Offset: 0x0000C18C
		public ToimagePage PreviewPage
		{
			get
			{
				return this.previewPage;
			}
			set
			{
				ToimagePage toimagePage = this.previewPage;
				if (base.SetProperty<ToimagePage>(ref this.previewPage, value, "PreviewPage"))
				{
					if (toimagePage != null)
					{
						toimagePage.Adjusted -= this.OnPageAdjusted;
					}
					if (this.previewPage == null)
					{
						return;
					}
					this.previewPage.Adjusted += this.OnPageAdjusted;
					base.OnPropertyChanged("PageText");
					this.PreviousPageCommand.NotifyCanExecuteChanged();
					this.NextPageCommand.NotifyCanExecuteChanged();
					this.PageNumber = this.PageList.IndexOf(this.previewPage) + 1;
					Task.Run(delegate
					{
						this.imageService.Preview(this.previewPage);
					});
				}
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000E038 File Offset: 0x0000C238
		private void OnPageAdjusted(ToimagePage page, AdjustType type)
		{
			AdjustSettings settings = new AdjustSettings
			{
				OriginalPage = page,
				Page = page.Clone(),
				AdjustType = type
			};
			this.debounceAdjust.Invoke(100, delegate(object state)
			{
				this.imageService.Adjust(settings);
			}, settings);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000E098 File Offset: 0x0000C298
		public static string[] ShowImportDialog()
		{
			string text = "*" + string.Join(";*", ImageToPDFViewModel.ImageExtensions);
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = string.Concat(new string[]
				{
					Resources.ImportDialogFilterSupported,
					" (",
					text,
					")|",
					text,
					";|",
					Resources.ImportDialogFilterImage,
					" (",
					text,
					")|",
					text
				})
			};
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				return openFileDialog.FileNames;
			}
			return null;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000E142 File Offset: 0x0000C342
		private void ImportFormDrag()
		{
			string[] array = this.dragFiles;
			if (array != null && array.Length != 0)
			{
				this.RunImportWorker(this.dragFiles);
			}
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0000E164 File Offset: 0x0000C364
		public void ImportFormGirdDrag(string[] dragFiles)
		{
			if (dragFiles != null && dragFiles.Length != 0)
			{
				this.RunImportWorker(dragFiles);
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000E175 File Offset: 0x0000C375
		private Task RunImportWorker(IEnumerable<string> filePaths)
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			CancellationTokenSource cancellationTokenSource2 = this.cts;
			if (cancellationTokenSource2 != null)
			{
				cancellationTokenSource2.Dispose();
			}
			this.cts = new CancellationTokenSource();
			return PageService.Import(filePaths, this.cts.Token);
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000384 RID: 900 RVA: 0x0000E1B5 File Offset: 0x0000C3B5
		// (set) Token: 0x06000385 RID: 901 RVA: 0x0000E1BD File Offset: 0x0000C3BD
		public bool IsExportAll
		{
			get
			{
				return this.isExportAll;
			}
			set
			{
				base.SetProperty<bool>(ref this.isExportAll, value, "IsExportAll");
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		private async Task ShowExport()
		{
			if (this.CanShowExport())
			{
				await Task.Yield();
				this.IsExportAll = this.SelectedCount == 0 || this.SelectedCount == this.PageCount;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000E217 File Offset: 0x0000C417
		// (set) Token: 0x06000388 RID: 904 RVA: 0x0000E21F File Offset: 0x0000C41F
		public IList SelectedItems
		{
			get
			{
				return this.selectedItems;
			}
			set
			{
				if (base.SetProperty<IList>(ref this.selectedItems, value, "SelectedItems"))
				{
					this.OnSelectedItemsChanged();
				}
				base.OnPropertyChanged("SelectedCount");
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0000E246 File Offset: 0x0000C446
		public int SelectedCount
		{
			get
			{
				IList list = this.SelectedItems;
				if (list == null)
				{
					return 0;
				}
				return list.Count;
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0000E259 File Offset: 0x0000C459
		private bool CanShowExport()
		{
			return this.PageList.Count > 0;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000E269 File Offset: 0x0000C469
		private bool CanImport()
		{
			return this.visibility == Visibility.Collapsed;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000E274 File Offset: 0x0000C474
		private async Task Rotate(string direction)
		{
			await Task.Yield();
			if (this.CanRotate(direction))
			{
				GAManager.SendEvent("ImageToPDFWindow", "RotateImageBtn", "Count", 1L);
				if (direction == "left")
				{
					this.RotateLeft(this.SelectedItems.Cast<ToimagePage>().ToArray<ToimagePage>());
				}
				else if (direction == "right")
				{
					this.RotateRight(this.SelectedItems.Cast<ToimagePage>().ToArray<ToimagePage>());
				}
			}
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000E2C0 File Offset: 0x0000C4C0
		private void RotateLeft(params ToimagePage[] pages)
		{
			foreach (ToimagePage toimagePage in pages)
			{
				int num = toimagePage.Rotate - 90;
				if (num < 0)
				{
					num += 360;
				}
				toimagePage.Rotate = num;
			}
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0000E2FC File Offset: 0x0000C4FC
		private void RotateRight(params ToimagePage[] pages)
		{
			foreach (ToimagePage toimagePage in pages)
			{
				int num = toimagePage.Rotate + 90;
				if (num >= 360)
				{
					num -= 360;
				}
				toimagePage.Rotate = num;
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0000E33C File Offset: 0x0000C53C
		private bool CanRotate(string direction)
		{
			IList list = this.SelectedItems;
			return list != null && list.Count > 0;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0000E354 File Offset: 0x0000C554
		private async Task RotatePreview(string direction)
		{
			await Task.Yield();
			if (this.CanRotatePreview(direction))
			{
				GAManager.SendEvent("ImageToPDFWindowImageEdit", "RotateImageBtn", "Count", 1L);
				if (direction == "left")
				{
					this.RotateLeft(new ToimagePage[] { this.PreviewPage });
				}
				else if (direction == "right")
				{
					this.RotateRight(new ToimagePage[] { this.PreviewPage });
				}
				this.PreviewPageRotate = (double)this.PreviewPage.Rotate;
			}
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000E39F File Offset: 0x0000C59F
		private bool CanRotatePreview(string direction)
		{
			return this.PreviewPage != null;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0000E3AC File Offset: 0x0000C5AC
		private async Task Delete()
		{
			await Task.Yield();
			if (this.CanDelete())
			{
				GAManager.SendEvent("ImageToPDFWindow", "DeleteImageBtn", "Count", 1L);
				if (ModernMessageBox.Show(Resources.CreatePDFDeleteContent.Replace("XXX", this.SelectedItems.Count.ToString()), UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
				{
					try
					{
						this.isRemovePages = true;
						foreach (ToimagePage toimagePage in this.SelectedItems.Cast<ToimagePage>().ToList<ToimagePage>())
						{
							this.PageList.Remove(toimagePage);
						}
					}
					finally
					{
						this.isRemovePages = false;
					}
					this.SelectedItems.Clear();
					this.RefreshList();
				}
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0000E3F0 File Offset: 0x0000C5F0
		private void RefreshList()
		{
			for (int i = 0; i < this.PageList.Count; i++)
			{
				ToimagePage toimagePage = this.PageList[i];
				string text = string.Format("{0}", i + 1);
				if (text != toimagePage.IndexString)
				{
					toimagePage.IndexString = text;
				}
			}
			base.OnPropertyChanged("PageCount");
			base.OnPropertyChanged("AddImage");
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0000E45E File Offset: 0x0000C65E
		private bool CanDelete()
		{
			IList list = this.SelectedItems;
			return list != null && list.Count > 0;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0000E474 File Offset: 0x0000C674
		private async Task DeletePreview()
		{
			await Task.Yield();
			if (this.CanDeletePreview())
			{
				GAManager.SendEvent("ImageToPDFWindowImageEdit", "DeleteImageBtn", "Count", 1L);
				if (ModernMessageBox.Show(Resources.ImageToPDFWinDeletePreview, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
				{
					int num = this.PageList.IndexOf(this.PreviewPage);
					this.SelectedItems.Remove(this.PreviewPage);
					this.PageList.Remove(this.PreviewPage);
					if (this.PageList.Count == 0)
					{
						ImagePreview imagePreview = Application.Current.Windows.OfType<ImagePreview>().FirstOrDefault<ImagePreview>();
						if (imagePreview != null)
						{
							imagePreview.Close();
						}
					}
					else
					{
						this.UpdatePreviewPage(num);
					}
				}
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0000E4B7 File Offset: 0x0000C6B7
		private bool CanDeletePreview()
		{
			return this.PreviewPage != null;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0000E4C4 File Offset: 0x0000C6C4
		private async Task Preview(object obj)
		{
			await Task.Yield();
			GAManager.SendEvent("ImageToPDFWindow", "PreviewImage", "Count", 1L);
			ToimagePage toimagePage = obj as ToimagePage;
			if (toimagePage != null)
			{
				this.PreviewPage = toimagePage;
				new ImagePreview(this)
				{
					Owner = Application.Current.Windows.OfType<ImageToPDF>().FirstOrDefault<ImageToPDF>()
				}.ShowDialog();
				this.PreviewSource = null;
				this.PreviewPage = null;
				GC.Collect();
			}
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0000E510 File Offset: 0x0000C710
		private async Task PreviousPage()
		{
			await Task.Yield();
			if (this.CanPreviousPage())
			{
				GAManager.SendEvent("ImageToPDFWindowImageEdit", "PreviousImage", "Count", 1L);
				this.PreviewPage = this.PageList[this.PageList.IndexOf(this.PreviewPage) - 1];
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0000E553 File Offset: 0x0000C753
		private bool CanPreviousPage()
		{
			return this.PageList.Count > 1 && this.PageList.IndexOf(this.PreviewPage) > 0;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0000E57C File Offset: 0x0000C77C
		private async Task NextPage()
		{
			await Task.Yield();
			if (this.CanNextPage())
			{
				GAManager.SendEvent("ImageToPDFWindow", "NextImage", "Count", 1L);
				this.PreviewPage = this.PageList[this.PageList.IndexOf(this.PreviewPage) + 1];
			}
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0000E5BF File Offset: 0x0000C7BF
		private bool CanNextPage()
		{
			return this.PageList.Count > 1 && this.PageList.IndexOf(this.PreviewPage) < this.PageList.Count - 1;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0000E5F4 File Offset: 0x0000C7F4
		private async Task OpenCrop()
		{
			await Task.Yield();
		}

		// Token: 0x0600039D RID: 925 RVA: 0x0000E630 File Offset: 0x0000C830
		private async Task Crop(Rect rect)
		{
			await Task.Run(delegate
			{
				this.imageService.Crop(this.PreviewPage, this.PreviewSource.OriginalImage, rect);
			});
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0000E67C File Offset: 0x0000C87C
		private async Task Revert()
		{
			GAManager.SendEvent("ImageToPDFWindowImageEdit", "RevertBtn", "Count", 1L);
			ToimagePage toimagePage = this.PreviewPage;
			if (toimagePage != null)
			{
				bool flag = !toimagePage.HasChanged();
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000E6BF File Offset: 0x0000C8BF
		private void UpdatePreviewPage(int index)
		{
			if (index >= this.PageList.Count)
			{
				index = this.PageList.Count - 1;
			}
			if (index >= 0)
			{
				this.PreviewPage = this.PageList[index];
			}
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0000E6F4 File Offset: 0x0000C8F4
		public void Dispose()
		{
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
		private void OnSelectedItemsChanged()
		{
			ObservableCollection<object> observableCollection = this.SelectedItems as ObservableCollection<object>;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= this.OnSelectedItemsCollectionChanged;
				observableCollection.CollectionChanged += this.OnSelectedItemsCollectionChanged;
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000E738 File Offset: 0x0000C938
		private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.UpdateCheckedAll();
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0000E740 File Offset: 0x0000C940
		private void UpdateCheckedAll()
		{
			if (this.inCheckedAll)
			{
				return;
			}
			this.inCheckedAll = true;
			try
			{
				if (this.PageList.Count == 0 || this.selectedItems == null || this.SelectedItems.Count == 0)
				{
					this.IsCheckedAll = new bool?(false);
				}
				else if (this.PageList.Count == this.SelectedItems.Count)
				{
					this.IsCheckedAll = new bool?(true);
				}
				else
				{
					this.IsCheckedAll = null;
				}
				this.RotateCommand.NotifyCanExecuteChanged();
				this.DeleteCommand.NotifyCanExecuteChanged();
			}
			finally
			{
				this.inCheckedAll = false;
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000E7F4 File Offset: 0x0000C9F4
		private void OnIsCheckedAllChanged()
		{
			if (this.inCheckedAll)
			{
				return;
			}
			this.inCheckedAll = true;
			try
			{
				if (this.PageList.Count != 0)
				{
					if (this.SelectedItems.Count == this.PageList.Where((ToimagePage x) => x.IsNeedEnabled).ToArray<ToimagePage>().Count<ToimagePage>())
					{
						this.SelectedItems.Clear();
						this.IsCheckedAll = new bool?(false);
					}
					else if (this.SelectedItems.Count != this.PageList.Count)
					{
						foreach (ToimagePage toimagePage in this.PageList)
						{
							if (!this.SelectedItems.Contains(toimagePage) && toimagePage.IsNeedEnabled)
							{
								this.SelectedItems.Add(toimagePage);
							}
						}
						if (this.SelectedItems.Count != this.PageList.Count)
						{
							this.isCheckedAll = null;
						}
						else
						{
							this.IsCheckedAll = new bool?(true);
						}
					}
					else
					{
						this.SelectedItems.Clear();
						this.IsCheckedAll = new bool?(false);
					}
					this.RotateCommand.NotifyCanExecuteChanged();
					this.DeleteCommand.NotifyCanExecuteChanged();
				}
			}
			finally
			{
				this.inCheckedAll = false;
			}
		}

		// Token: 0x040001C6 RID: 454
		private List<PageSizeItem> pageSizeItems = new List<PageSizeItem>();

		// Token: 0x040001C7 RID: 455
		private List<PageMarginItem> contentMargins = new List<PageMarginItem>();

		// Token: 0x040001C8 RID: 456
		private List<PageSaveItem> saveFileWays = new List<PageSaveItem>();

		// Token: 0x040001C9 RID: 457
		private ToPDFFileItem _selectedItem;

		// Token: 0x040001CA RID: 458
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x040001CB RID: 459
		private PageSizeItem pageSize;

		// Token: 0x040001CC RID: 460
		private PageMarginItem contentMargin;

		// Token: 0x040001CD RID: 461
		private bool? _OutputInOneFile = new bool?(true);

		// Token: 0x040001CE RID: 462
		private PageSaveItem saveFile;

		// Token: 0x040001CF RID: 463
		private AsyncRelayCommand importCommand;

		// Token: 0x040001D0 RID: 464
		private AsyncRelayCommand<string> rotateCommand;

		// Token: 0x040001D1 RID: 465
		private AsyncRelayCommand<string> rotatePreviewCommand;

		// Token: 0x040001D2 RID: 466
		private AsyncRelayCommand deleteCommand;

		// Token: 0x040001D3 RID: 467
		private AsyncRelayCommand deletePreviewCommand;

		// Token: 0x040001D4 RID: 468
		private AsyncRelayCommand<object> previewCommand;

		// Token: 0x040001D5 RID: 469
		private AsyncRelayCommand previousPageCommand;

		// Token: 0x040001D6 RID: 470
		private AsyncRelayCommand nextPageCommand;

		// Token: 0x040001D7 RID: 471
		private AsyncRelayCommand openCropCommand;

		// Token: 0x040001D8 RID: 472
		private AsyncRelayCommand<Rect> cropCommand;

		// Token: 0x040001D9 RID: 473
		private AsyncRelayCommand revertCommand;

		// Token: 0x040001DA RID: 474
		private Visibility visibility = Visibility.Collapsed;

		// Token: 0x040001DB RID: 475
		public CancellationTokenSource cts = new CancellationTokenSource();

		// Token: 0x040001DC RID: 476
		private string[] dragFiles;

		// Token: 0x040001DD RID: 477
		private bool? isCheckedAll = new bool?(false);

		// Token: 0x040001DE RID: 478
		private IList selectedItems;

		// Token: 0x040001DF RID: 479
		private string _OutputFileFullName;

		// Token: 0x040001E0 RID: 480
		private bool inCheckedAll;

		// Token: 0x040001E1 RID: 481
		private ImagePdfGenerateSettings _imagePdfGenerateSettings;

		// Token: 0x040001E4 RID: 484
		private int pageNumber;

		// Token: 0x040001E5 RID: 485
		private string _OutputPath;

		// Token: 0x040001E6 RID: 486
		private DebounceHelper debounceAdjust = new DebounceHelper();

		// Token: 0x040001E7 RID: 487
		private bool isRemovePages;

		// Token: 0x040001E8 RID: 488
		private ImageViewerSource previewSource;

		// Token: 0x040001E9 RID: 489
		private double previewPageRotate;

		// Token: 0x040001EA RID: 490
		private readonly ImageService imageService = new ImageService();

		// Token: 0x040001ED RID: 493
		private ToimagePage previewPage;

		// Token: 0x040001EE RID: 494
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x040001EF RID: 495
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x040001F0 RID: 496
		private RelayCommand openOneFileInExplorer;

		// Token: 0x040001F1 RID: 497
		private RelayCommand selectPath;

		// Token: 0x040001F2 RID: 498
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };

		// Token: 0x040001F3 RID: 499
		private bool isExportAll;

		// Token: 0x020000F3 RID: 243
		public class DropHandler : DefaultDropHandler
		{
			// Token: 0x17000261 RID: 609
			// (get) Token: 0x0600082A RID: 2090 RVA: 0x00020FF2 File Offset: 0x0001F1F2
			private bool hasDragFiles
			{
				get
				{
					return this.viewModel.DragFiles != null;
				}
			}

			// Token: 0x0600082B RID: 2091 RVA: 0x00021002 File Offset: 0x0001F202
			public DropHandler(ImageToPDFViewModel viewModel)
			{
				this.viewModel = viewModel;
			}

			// Token: 0x0600082C RID: 2092 RVA: 0x00021011 File Offset: 0x0001F211
			public override void DragOver(IDropInfo dropInfo)
			{
				base.DragOver(dropInfo);
				if (this.hasDragFiles)
				{
					dropInfo.Effects = DragDropEffects.Copy;
				}
			}

			// Token: 0x0600082D RID: 2093 RVA: 0x00021029 File Offset: 0x0001F229
			public override void Drop(IDropInfo dropInfo)
			{
				if (this.hasDragFiles)
				{
					this.viewModel.ImportFormDrag();
					return;
				}
				base.Drop(dropInfo);
			}

			// Token: 0x040004F1 RID: 1265
			private readonly ImageToPDFViewModel viewModel;
		}
	}
}
