using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using HandyControl.Controls;
using Microsoft.Win32;
using NAPS2.Wia;
using Nito.AsyncEx;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Models.Scan;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Scan;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000062 RID: 98
	public class InsertFromScannerViewModel : ObservableObject, IDisposable
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x0001BEB8 File Offset: 0x0001A0B8
		// (set) Token: 0x06000584 RID: 1412 RVA: 0x0001BEC0 File Offset: 0x0001A0C0
		public ObservableCollection<ScannerDeviceInfo> ScannerList
		{
			get
			{
				return this.scannerList;
			}
			set
			{
				if (base.SetProperty<ObservableCollection<ScannerDeviceInfo>>(ref this.scannerList, value, "ScannerList"))
				{
					this.OnScannerListChanged();
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0001BEDC File Offset: 0x0001A0DC
		// (set) Token: 0x06000586 RID: 1414 RVA: 0x0001BEE4 File Offset: 0x0001A0E4
		public bool ScannerBtnIsenable
		{
			get
			{
				return this.scannerBtnIsenable;
			}
			set
			{
				base.SetProperty<bool>(ref this.scannerBtnIsenable, value, "ScannerBtnIsenable");
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x0001BEF9 File Offset: 0x0001A0F9
		// (set) Token: 0x06000588 RID: 1416 RVA: 0x0001BF01 File Offset: 0x0001A101
		public ScannerDeviceInfo SelectedScanner
		{
			get
			{
				return this.selectedScanner;
			}
			set
			{
				if (base.SetProperty<ScannerDeviceInfo>(ref this.selectedScanner, value, "SelectedScanner"))
				{
					this.OnSelectedScannerChanged();
				}
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x0001BF1D File Offset: 0x0001A11D
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x0001BF25 File Offset: 0x0001A125
		public bool IsEnabled
		{
			get
			{
				return this.Isenabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.Isenabled, value, "IsEnabled");
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x0001BF3A File Offset: 0x0001A13A
		// (set) Token: 0x0600058C RID: 1420 RVA: 0x0001BF42 File Offset: 0x0001A142
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

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0001BF57 File Offset: 0x0001A157
		public bool HasScanner
		{
			get
			{
				return this.SelectedScanner != null;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0001BF62 File Offset: 0x0001A162
		public bool AddScannerImage
		{
			get
			{
				if (this.SelectedScanner == null)
				{
					ObservableCollection<ScannedPage> pageList = this.PageList;
					return pageList != null && pageList.Count < 1;
				}
				return false;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x0001BF82 File Offset: 0x0001A182
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x0001BF8A File Offset: 0x0001A18A
		public bool ScannerConnecting
		{
			get
			{
				return this.scannerConnecting;
			}
			set
			{
				base.SetProperty<bool>(ref this.scannerConnecting, value, "ScannerConnecting");
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x0001BF9F File Offset: 0x0001A19F
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x0001BFA7 File Offset: 0x0001A1A7
		public ObservableCollection<KeyValuePair<ScanSource, string>> SourceList
		{
			get
			{
				return this.sourceList;
			}
			set
			{
				base.SetProperty<ObservableCollection<KeyValuePair<ScanSource, string>>>(ref this.sourceList, value, "SourceList");
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x0001BFBC File Offset: 0x0001A1BC
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x0001BFC4 File Offset: 0x0001A1C4
		public ScanSource SelectedSource
		{
			get
			{
				return this.selectedSource;
			}
			set
			{
				base.SetProperty<ScanSource>(ref this.selectedSource, value, "SelectedSource");
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x0001BFD9 File Offset: 0x0001A1D9
		public List<KeyValuePair<ScanColor, string>> ColorList { get; } = new List<KeyValuePair<ScanColor, string>>(EnumHelper.GetDescriptionDictionary<ScanColor>());

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x0001BFE1 File Offset: 0x0001A1E1
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x0001BFE9 File Offset: 0x0001A1E9
		public ScanColor SelectedColor
		{
			get
			{
				return this.selectedColor;
			}
			set
			{
				base.SetProperty<ScanColor>(ref this.selectedColor, value, "SelectedColor");
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0001BFFE File Offset: 0x0001A1FE
		public List<KeyValuePair<ScanResolution, string>> ResolutionList { get; } = new List<KeyValuePair<ScanResolution, string>>(EnumHelper.GetDescriptionDictionary<ScanResolution>());

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x0001C006 File Offset: 0x0001A206
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x0001C00E File Offset: 0x0001A20E
		public ScanResolution SelectedResolution
		{
			get
			{
				return this.selectedResolution;
			}
			set
			{
				base.SetProperty<ScanResolution>(ref this.selectedResolution, value, "SelectedResolution");
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x0001C023 File Offset: 0x0001A223
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x0001C02B File Offset: 0x0001A22B
		public List<KeyValuePair<ScanArea, string>> AreaList { get; set; } = new List<KeyValuePair<ScanArea, string>>(EnumHelper.GetDescriptionDictionary<ScanArea>());

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x0001C034 File Offset: 0x0001A234
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x0001C03C File Offset: 0x0001A23C
		public ScanArea SelectedArea
		{
			get
			{
				return this.selectedArea;
			}
			set
			{
				base.SetProperty<ScanArea>(ref this.selectedArea, value, "SelectedArea");
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x0001C051 File Offset: 0x0001A251
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0001C059 File Offset: 0x0001A259
		public int Brightness
		{
			get
			{
				return this.brightness;
			}
			set
			{
				base.SetProperty<int>(ref this.brightness, value, "Brightness");
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x0001C06E File Offset: 0x0001A26E
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001C076 File Offset: 0x0001A276
		public int Contrast
		{
			get
			{
				return this.contrast;
			}
			set
			{
				base.SetProperty<int>(ref this.contrast, value, "Contrast");
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x0001C08B File Offset: 0x0001A28B
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x0001C093 File Offset: 0x0001A293
		public int Saturation
		{
			get
			{
				return this.saturation;
			}
			set
			{
				base.SetProperty<int>(ref this.saturation, value, "Saturation");
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001C0A8 File Offset: 0x0001A2A8
		public AsyncRelayCommand LoadedCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.loadedCommand) == null)
				{
					asyncRelayCommand = (this.loadedCommand = new AsyncRelayCommand(new Func<Task>(this.Loaded)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x0001C0DC File Offset: 0x0001A2DC
		public AsyncRelayCommand ReloadScannerListCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.reloadScannerListCommand) == null)
				{
					asyncRelayCommand = (this.reloadScannerListCommand = new AsyncRelayCommand(new Func<Task>(this.ReloadScannerList)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0001C110 File Offset: 0x0001A310
		public AsyncRelayCommand ScanCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.scanCommand) == null)
				{
					asyncRelayCommand = (this.scanCommand = new AsyncRelayCommand(new Func<Task>(this.Scan), new Func<bool>(this.CanScan)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001C14D File Offset: 0x0001A34D
		private void OnRunGetDeviceInfosTaskCompleted(List<ScannerDeviceInfo> deviceInfos)
		{
			ScannerDispatcherHelper.Invoke(delegate
			{
				this.ScannerList = new ObservableCollection<ScannerDeviceInfo>(deviceInfos);
			});
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0001C174 File Offset: 0x0001A374
		private void OnScannerListChanged()
		{
			if (this.SelectedScanner != null)
			{
				this.SelectedScanner = this.ScannerList.FirstOrDefault((ScannerDeviceInfo x) => x.Id == this.SelectedScanner.Id);
			}
			if (this.SelectedScanner == null)
			{
				this.SelectedScanner = this.ScannerList.FirstOrDefault<ScannerDeviceInfo>();
			}
			if (this.SelectedScanner == null)
			{
				InsertFromScannerViewModel.scanService.RunGetDeviceInfosTask();
			}
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0001C1D1 File Offset: 0x0001A3D1
		private void OnSelectedScannerChanged()
		{
			base.OnPropertyChanged("HasScanner");
			base.OnPropertyChanged("AddScannerImage");
			this.ScanCommand.NotifyCanExecuteChanged();
			this.ImportCommand.NotifyCanExecuteChanged();
			this.LoadSettings();
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0001C208 File Offset: 0x0001A408
		public async Task Loaded()
		{
			IReadOnlyList<ScannerDeviceInfo> readOnlyList = await InsertFromScannerViewModel.GetScanDeviceInfosAsync(default(CancellationToken));
			this.ScannerList = new ObservableCollection<ScannerDeviceInfo>(readOnlyList);
			this.ScannerConnecting = false;
			GAManager.SendEvent("ScannerWindow", "LoadedWindow", this.HasScanner ? "DoScanWindow" : "AddScannerWindow", 1L);
			if (this.HasScanner)
			{
				GAManager.SendEvent("ScannerWindow", "LoadedScannerCount", this.ScannerList.Count.ToString(), 1L);
			}
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0001C24C File Offset: 0x0001A44C
		public async Task Refreshed()
		{
			IReadOnlyList<ScannerDeviceInfo> readOnlyList = await InsertFromScannerViewModel.RefreshScanDeviceInfosAsync(default(CancellationToken));
			this.ScannerList = new ObservableCollection<ScannerDeviceInfo>(readOnlyList);
			this.ScannerConnecting = false;
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0001C290 File Offset: 0x0001A490
		private async Task ReloadScannerList()
		{
			if (this.ScannerList != null)
			{
				List<ScannerDeviceInfo> oldScanners = this.ScannerList.ToList<ScannerDeviceInfo>();
				IReadOnlyList<ScannerDeviceInfo> readOnlyList = await InsertFromScannerViewModel.GetScanDeviceInfosAsync(default(CancellationToken));
				bool flag = readOnlyList.Count != oldScanners.Count;
				if (!flag)
				{
					for (int i = 0; i < readOnlyList.Count; i++)
					{
						if (readOnlyList[i].Id != oldScanners[i].Id || readOnlyList[i].Name != oldScanners[i].Name)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.ScannerList = new ObservableCollection<ScannerDeviceInfo>(readOnlyList);
				}
			}
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0001C2D3 File Offset: 0x0001A4D3
		private Task CancelScan()
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			return Task.CompletedTask;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0001C2EC File Offset: 0x0001A4EC
		private async Task Scan()
		{
			if (this.CanScan())
			{
				GAManager.SendEvent("ScannerWindow", "ScanBtn", "Count", 1L);
				WiaDevice wiaDevice2 = await InsertFromScannerViewModel.scanService.FindDevice(this.SelectedScanner.Id);
				WiaDevice wiaDevice = wiaDevice2;
				if (wiaDevice == null)
				{
					ModernMessageBox.Show(Resources.ScannerWinCheckScannerContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					await this.ReloadScannerList();
				}
				else
				{
					ScanSettings settings = this.ToSettings();
					GAManager.SendEvent("ScannerSettings", "Source", settings.Source.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Color", settings.Color.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Resolution", settings.Resolution.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Area", settings.Area.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Brightness", settings.Brightness.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Contrast", settings.Contrast.ToString(), 1L);
					GAManager.SendEvent("ScannerSettings", "Saturation", settings.Saturation.ToString(), 1L);
					try
					{
						this.ScannerBtnIsenable = false;
						this.Visibility = Visibility.Visible;
						this.IsEnabled = true;
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
						if (settings.Source != ScanSource.Glass && !wiaDevice.SupportsFeeder())
						{
							settings.Source = ScanSource.Glass;
							this.SelectedSource = ScanSource.Glass;
						}
						if (settings.Source == ScanSource.Duplex && !wiaDevice.SupportsDuplex())
						{
							settings.Source = ScanSource.Glass;
							this.SelectedSource = ScanSource.Glass;
						}
						await Task.Run<bool>(() => InsertFromScannerViewModel.scanService.Scan(settings, wiaDevice, this.cts.Token));
					}
					catch (Exception ex)
					{
						GAManager.SendEvent("Exception", "ScanException", ex.GetType().Name + ", " + ex.Message, 1L);
						Log.WriteLog(ex.ToString());
						string text = null;
						WiaException ex2 = ex as WiaException;
						if (ex2 != null)
						{
							text = pdfeditor.Models.Scan.WiaErrorCodes.GetMessage(ex2.ErrorCode);
						}
						if (string.IsNullOrEmpty(text))
						{
							text = Resources.ScanErrorMessage;
						}
						Log.WriteLog(text);
						ModernMessageBox.Show(text, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					finally
					{
						this.Visibility = Visibility.Collapsed;
						this.ScannerBtnIsenable = true;
					}
				}
			}
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0001C32F File Offset: 0x0001A52F
		private bool CanScan()
		{
			ScannerDeviceInfo scannerDeviceInfo = this.SelectedScanner;
			return !string.IsNullOrEmpty((scannerDeviceInfo != null) ? scannerDeviceInfo.Id : null) && this.visibility == Visibility.Collapsed;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0001C358 File Offset: 0x0001A558
		private void LoadSettings()
		{
			ScannerDeviceInfo scannerDeviceInfo = this.SelectedScanner;
			if (string.IsNullOrEmpty((scannerDeviceInfo != null) ? scannerDeviceInfo.Id : null))
			{
				return;
			}
			base.PropertyChanged -= this.ScanViewModel_PropertyChanged;
			Task.Run(delegate
			{
				ScanSettings settings = InsertFromScannerViewModel.scanService.LoadSettings(this.SelectedScanner.Id);
				WiaDevice result = InsertFromScannerViewModel.scanService.FindDevice(this.SelectedScanner.Id).Result;
				Dictionary<ScanSource, string> sources = this.GetSources(result);
				ScannerDispatcherHelper.Invoke(delegate
				{
					this.SourceList = new ObservableCollection<KeyValuePair<ScanSource, string>>(sources);
					this.SelectedSource = (sources.ContainsKey(settings.Source) ? settings.Source : ScanSource.Glass);
					this.SelectedColor = settings.Color;
					this.SelectedResolution = settings.Resolution;
					this.SelectedArea = settings.Area;
					this.Brightness = settings.Brightness;
					this.Contrast = settings.Contrast;
					this.Saturation = settings.Saturation;
				});
			}).ContinueWith(delegate(Task t)
			{
				base.PropertyChanged += this.ScanViewModel_PropertyChanged;
			});
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0001C3B4 File Offset: 0x0001A5B4
		private Dictionary<ScanSource, string> GetSources(WiaDevice device)
		{
			Dictionary<ScanSource, string> descriptionDictionary = EnumHelper.GetDescriptionDictionary<ScanSource>();
			if (device != null)
			{
				if (!device.SupportsFeeder())
				{
					descriptionDictionary.Remove(ScanSource.Feeder);
					descriptionDictionary.Remove(ScanSource.Duplex);
				}
				else if (!device.SupportsDuplex())
				{
					descriptionDictionary.Remove(ScanSource.Duplex);
				}
			}
			return descriptionDictionary;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0001C3F5 File Offset: 0x0001A5F5
		private void ScanViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.savedProperties.Contains(e.PropertyName))
			{
				this.SaveSettings();
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0001C410 File Offset: 0x0001A610
		private void SaveSettings()
		{
			this.debounceSaveSettings.Invoke(100, delegate(object _)
			{
				InsertFromScannerViewModel.scanService.SaveSettings(this.ToSettings());
			}, null);
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0001C42C File Offset: 0x0001A62C
		private ScanSettings ToSettings()
		{
			if (this.SelectedScanner == null)
			{
				return null;
			}
			return new ScanSettings
			{
				Device = this.SelectedScanner,
				Source = this.SelectedSource,
				Color = this.SelectedColor,
				Resolution = this.SelectedResolution,
				Area = this.SelectedArea,
				Brightness = this.Brightness,
				Contrast = this.Contrast,
				Saturation = this.Saturation
			};
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x0001C4A8 File Offset: 0x0001A6A8
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x0001C4B0 File Offset: 0x0001A6B0
		public InsertFromScannerViewModel.DropHandler DropTarget { get; private set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001C4B9 File Offset: 0x0001A6B9
		// (set) Token: 0x060005B9 RID: 1465 RVA: 0x0001C4C1 File Offset: 0x0001A6C1
		public bool IsNeedConfirmOnClose { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001C4CA File Offset: 0x0001A6CA
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x0001C4D2 File Offset: 0x0001A6D2
		public ObservableCollection<ScannedPage> PageList { get; set; } = new ObservableCollection<ScannedPage>();

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x0001C4DB File Offset: 0x0001A6DB
		public int PageCount
		{
			get
			{
				return this.PageList.Count;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0001C4E8 File Offset: 0x0001A6E8
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0001C4F0 File Offset: 0x0001A6F0
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

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001C561 File Offset: 0x0001A761
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0001C56C File Offset: 0x0001A76C
		public ScannedPage PreviewPage
		{
			get
			{
				return this.previewPage;
			}
			set
			{
				ScannedPage scannedPage = this.previewPage;
				if (base.SetProperty<ScannedPage>(ref this.previewPage, value, "PreviewPage"))
				{
					if (scannedPage != null)
					{
						scannedPage.Adjusted -= this.OnPageAdjusted;
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

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0001C618 File Offset: 0x0001A818
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x0001C620 File Offset: 0x0001A820
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

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0001C656 File Offset: 0x0001A856
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x0001C65E File Offset: 0x0001A85E
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

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001C690 File Offset: 0x0001A890
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

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001C6DB File Offset: 0x0001A8DB
		// (set) Token: 0x060005C7 RID: 1479 RVA: 0x0001C6E3 File Offset: 0x0001A8E3
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

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x0001C70A File Offset: 0x0001A90A
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

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0001C71D File Offset: 0x0001A91D
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x0001C725 File Offset: 0x0001A925
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

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001C741 File Offset: 0x0001A941
		public List<KeyValuePair<ExportMode, string>> ExportModeList { get; } = new List<KeyValuePair<ExportMode, string>>(EnumHelper.GetDescriptionDictionary<ExportMode>());

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060005CC RID: 1484 RVA: 0x0001C749 File Offset: 0x0001A949
		// (set) Token: 0x060005CD RID: 1485 RVA: 0x0001C751 File Offset: 0x0001A951
		public ExportMode SelectedExportMode
		{
			get
			{
				return this.selectedExportMode;
			}
			set
			{
				base.SetProperty<ExportMode>(ref this.selectedExportMode, value, "SelectedExportMode");
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x0001C766 File Offset: 0x0001A966
		// (set) Token: 0x060005CF RID: 1487 RVA: 0x0001C76E File Offset: 0x0001A96E
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

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060005D0 RID: 1488 RVA: 0x0001C783 File Offset: 0x0001A983
		public List<KeyValuePair<PageZoomLevel, string>> ZoomList { get; } = new List<KeyValuePair<PageZoomLevel, string>>(EnumHelper.GetDescriptionDictionary<PageZoomLevel>());

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001C78B File Offset: 0x0001A98B
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x0001C793 File Offset: 0x0001A993
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

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001C7A8 File Offset: 0x0001A9A8
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

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001C7E8 File Offset: 0x0001A9E8
		public AsyncRelayCommand ShowExportCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.showExportCommand) == null)
				{
					asyncRelayCommand = (this.showExportCommand = new AsyncRelayCommand(new Func<Task>(this.ShowExport), new Func<bool>(this.CanShowExport)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0001C828 File Offset: 0x0001AA28
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

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x0001C868 File Offset: 0x0001AA68
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

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0001C8A8 File Offset: 0x0001AAA8
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

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001C8E8 File Offset: 0x0001AAE8
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

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0001C928 File Offset: 0x0001AB28
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

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x0001C95C File Offset: 0x0001AB5C
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

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x0001C99C File Offset: 0x0001AB9C
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

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x0001C9DC File Offset: 0x0001ABDC
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

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x0001CA10 File Offset: 0x0001AC10
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

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001CA44 File Offset: 0x0001AC44
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

		// Token: 0x060005DF RID: 1503 RVA: 0x0001CA78 File Offset: 0x0001AC78
		public InsertFromScannerViewModel()
		{
			this.PageList.CollectionChanged += this.OnPageListCollectionChanged;
			PageService.PageAdded += delegate(ScannedPage x)
			{
				ScannerDispatcherHelper.Invoke(delegate
				{
					this.PageList.Add(x);
				});
				this.IsNeedConfirmOnClose = true;
			};
			this.DropTarget = new InsertFromScannerViewModel.DropHandler(this);
			this.imageService.PreviewImageChanged += this.OnPreviewImageChanged;
			this.savedProperties = new List<string> { "SelectedSource", "SelectedColor", "SelectedResolution", "SelectedArea", "Brightness", "Contrast", "Saturation" };
			for (int i = 0; i < this.ResolutionList.Count; i++)
			{
				if (this.ResolutionList[i].Key == ScanResolution.Dpi300)
				{
					string text = "  300 DPI (" + Resources.ScannerWinRecommended + ")";
					this.ResolutionList[i] = new KeyValuePair<ScanResolution, string>(ScanResolution.Dpi300, text);
					break;
				}
			}
			InsertFromScannerViewModel.scanService.RunGetDeviceInfosTaskCompleted += this.OnRunGetDeviceInfosTaskCompleted;
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0001CC54 File Offset: 0x0001AE54
		private void OnPreviewImageChanged(ScannedPage page, Bitmap bitmap, bool isAdjust)
		{
			try
			{
				ImageViewerSource source = new ImageViewerSource(bitmap, (double)page.Rotate);
				if (isAdjust)
				{
					pdfeditor.Controls.PageEditor.ImageViewer.AdjustingSource = source;
				}
				ScannerDispatcherHelper.Invoke(delegate
				{
					this.PreviewSource = source;
				});
			}
			finally
			{
				pdfeditor.Controls.PageEditor.ImageViewer.AdjustingSource = null;
			}
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x0001CCBC File Offset: 0x0001AEBC
		private void OnPageListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.OnPropertyChanged("PageCount");
			this.UpdateCheckedAll();
			if (!this.isRemovePages)
			{
				this.RefreshList();
			}
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0001CCE0 File Offset: 0x0001AEE0
		private void RefreshList()
		{
			for (int i = 0; i < this.PageList.Count; i++)
			{
				ScannedPage scannedPage = this.PageList[i];
				string text = string.Format("{0}", i + 1);
				if (text != scannedPage.IndexString)
				{
					scannedPage.IndexString = text;
				}
			}
			this.ShowExportCommand.NotifyCanExecuteChanged();
			base.OnPropertyChanged("PageCount");
			base.OnPropertyChanged("AddScannerImage");
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0001CD5C File Offset: 0x0001AF5C
		private void OnSelectedItemsChanged()
		{
			ObservableCollection<object> observableCollection = this.SelectedItems as ObservableCollection<object>;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged -= this.OnSelectedItemsCollectionChanged;
				observableCollection.CollectionChanged += this.OnSelectedItemsCollectionChanged;
			}
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x0001CD9C File Offset: 0x0001AF9C
		private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.UpdateCheckedAll();
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x0001CDA4 File Offset: 0x0001AFA4
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

		// Token: 0x060005E6 RID: 1510 RVA: 0x0001CE58 File Offset: 0x0001B058
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
					if (this.SelectedItems.Count != this.PageList.Count)
					{
						foreach (ScannedPage scannedPage in this.PageList)
						{
							if (!this.SelectedItems.Contains(scannedPage))
							{
								this.SelectedItems.Add(scannedPage);
							}
						}
						this.IsCheckedAll = new bool?(true);
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

		// Token: 0x060005E7 RID: 1511 RVA: 0x0001CF44 File Offset: 0x0001B144
		private void OnPageAdjusted(ScannedPage page, AdjustType type)
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

		// Token: 0x060005E8 RID: 1512 RVA: 0x0001CFA4 File Offset: 0x0001B1A4
		private async Task Import()
		{
			if (this.CanImport())
			{
				GAManager.SendEvent("ScannerWindow", "ImportBtn", "Count", 1L);
				string[] array = InsertFromScannerViewModel.ShowImportDialog();
				if (array != null)
				{
					this.Visibility = Visibility.Visible;
					await this.RunImportWorker(array);
					this.Visibility = Visibility.Collapsed;
				}
			}
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0001CFE8 File Offset: 0x0001B1E8
		public static string[] ShowImportDialog()
		{
			string text = "*" + string.Join(";*", InsertFromScannerViewModel.ImageExtensions);
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

		// Token: 0x060005EA RID: 1514 RVA: 0x0001D092 File Offset: 0x0001B292
		private void ImportFormDrag()
		{
			string[] array = this.dragFiles;
			if (array != null && array.Length != 0)
			{
				Application.Current.Dispatcher.InvokeAsync<Task>(async delegate
				{
					await this.RunImportWorker(this.dragFiles);
				});
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0001D0C4 File Offset: 0x0001B2C4
		private async Task RunImportWorker(IEnumerable<string> filePaths)
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
			PageService.Import(filePaths, this.cts.Token);
			Task completedTask = Task.CompletedTask;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0001D110 File Offset: 0x0001B310
		private async Task ShowExport()
		{
			if (this.CanShowExport())
			{
				await Task.Yield();
				this.IsExportAll = this.SelectedCount == 0 || this.SelectedCount == this.PageCount;
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0001D153 File Offset: 0x0001B353
		private bool CanShowExport()
		{
			return this.PageList.Count > 0;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0001D163 File Offset: 0x0001B363
		private bool CanImport()
		{
			return this.visibility == Visibility.Collapsed;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0001D170 File Offset: 0x0001B370
		private List<ScannedPage> GetOrderedPagesForExport()
		{
			List<ScannedPage> list = this.PageList.ToList<ScannedPage>();
			if (this.IsExportAll || this.PageCount == this.SelectedCount)
			{
				return list;
			}
			List<ScannedPage> list2 = new List<ScannedPage>();
			foreach (ScannedPage scannedPage in list)
			{
				if (this.SelectedItems.Contains(scannedPage))
				{
					list2.Add(scannedPage);
				}
			}
			return list2;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0001D1F8 File Offset: 0x0001B3F8
		private async Task Rotate(string direction)
		{
			await Task.Yield();
			if (this.CanRotate(direction))
			{
				GAManager.SendEvent("ScannerWindow", "RotateImageBtn", "Count", 1L);
				if (direction == "left")
				{
					this.RotateLeft(this.SelectedItems.Cast<ScannedPage>().ToArray<ScannedPage>());
				}
				else if (direction == "right")
				{
					this.RotateRight(this.SelectedItems.Cast<ScannedPage>().ToArray<ScannedPage>());
				}
			}
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0001D244 File Offset: 0x0001B444
		private void RotateLeft(params ScannedPage[] pages)
		{
			foreach (ScannedPage scannedPage in pages)
			{
				int num = scannedPage.Rotate - 90;
				if (num < 0)
				{
					num += 360;
				}
				scannedPage.Rotate = num;
			}
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0001D280 File Offset: 0x0001B480
		private void RotateRight(params ScannedPage[] pages)
		{
			foreach (ScannedPage scannedPage in pages)
			{
				int num = scannedPage.Rotate + 90;
				if (num >= 360)
				{
					num -= 360;
				}
				scannedPage.Rotate = num;
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0001D2C0 File Offset: 0x0001B4C0
		private bool CanRotate(string direction)
		{
			IList list = this.SelectedItems;
			return list != null && list.Count > 0;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0001D2D8 File Offset: 0x0001B4D8
		private async Task RotatePreview(string direction)
		{
			await Task.Yield();
			if (this.CanRotatePreview(direction))
			{
				GAManager.SendEvent("ScannerWindowImageEdit", "RotateImageBtn", "Count", 1L);
				if (direction == "left")
				{
					this.RotateLeft(new ScannedPage[] { this.PreviewPage });
				}
				else if (direction == "right")
				{
					this.RotateRight(new ScannedPage[] { this.PreviewPage });
				}
				this.PreviewPageRotate = (double)this.PreviewPage.Rotate;
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001D323 File Offset: 0x0001B523
		private bool CanRotatePreview(string direction)
		{
			return this.PreviewPage != null;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0001D330 File Offset: 0x0001B530
		private async Task Delete()
		{
			await Task.Yield();
			if (this.CanDelete())
			{
				GAManager.SendEvent("ScannerWindow", "DeleteImageBtn", "Count", 1L);
				if (ModernMessageBox.Show(Resources.ScannerWinDeleteSelected.Replace("XXX", this.SelectedItems.Count.ToString()), UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
				{
					try
					{
						this.isRemovePages = true;
						foreach (ScannedPage scannedPage in this.SelectedItems.Cast<ScannedPage>().ToList<ScannedPage>())
						{
							this.PageList.Remove(scannedPage);
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

		// Token: 0x060005F7 RID: 1527 RVA: 0x0001D373 File Offset: 0x0001B573
		private bool CanDelete()
		{
			IList list = this.SelectedItems;
			return list != null && list.Count > 0;
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0001D38C File Offset: 0x0001B58C
		private async Task DeletePreview()
		{
			await Task.Yield();
			if (this.CanDeletePreview())
			{
				GAManager.SendEvent("ScannerWindowImageEdit", "DeleteImageBtn", "Count", 1L);
				if (ModernMessageBox.Show(Resources.ScannerWinDeletePreview, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
				{
					int num = this.PageList.IndexOf(this.PreviewPage);
					this.SelectedItems.Remove(this.PreviewPage);
					this.PageList.Remove(this.PreviewPage);
					if (this.PageList.Count == 0)
					{
						ScannerPreview scannerPreview = Application.Current.Windows.OfType<ScannerPreview>().FirstOrDefault<ScannerPreview>();
						if (scannerPreview != null)
						{
							scannerPreview.Close();
						}
					}
					else
					{
						this.UpdatePreviewPage(num);
					}
				}
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001D3CF File Offset: 0x0001B5CF
		private bool CanDeletePreview()
		{
			return this.PreviewPage != null;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x0001D3DC File Offset: 0x0001B5DC
		private async Task Preview(object obj)
		{
			await Task.Yield();
			GAManager.SendEvent("ScannerWindow", "PreviewImage", "Count", 1L);
			ScannedPage scannedPage = obj as ScannedPage;
			if (scannedPage != null)
			{
				this.PreviewPage = scannedPage;
				new ScannerPreview(this)
				{
					Owner = Application.Current.Windows.OfType<InsertPageFromScanner>().FirstOrDefault<InsertPageFromScanner>()
				}.ShowDialog();
				this.PreviewSource = null;
				this.PreviewPage = null;
				GC.Collect();
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0001D428 File Offset: 0x0001B628
		private async Task PreviousPage()
		{
			await Task.Yield();
			if (this.CanPreviousPage())
			{
				GAManager.SendEvent("ScannerWindowImageEdit", "PreviousImage", "Count", 1L);
				this.PreviewPage = this.PageList[this.PageList.IndexOf(this.PreviewPage) - 1];
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0001D46B File Offset: 0x0001B66B
		private bool CanPreviousPage()
		{
			return this.PageList.Count > 1 && this.PageList.IndexOf(this.PreviewPage) > 0;
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0001D494 File Offset: 0x0001B694
		private async Task NextPage()
		{
			await Task.Yield();
			if (this.CanNextPage())
			{
				GAManager.SendEvent("ScannerWindow", "NextImage", "Count", 1L);
				this.PreviewPage = this.PageList[this.PageList.IndexOf(this.PreviewPage) + 1];
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0001D4D7 File Offset: 0x0001B6D7
		private bool CanNextPage()
		{
			return this.PageList.Count > 1 && this.PageList.IndexOf(this.PreviewPage) < this.PageList.Count - 1;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0001D50C File Offset: 0x0001B70C
		private async Task OpenCrop()
		{
			await Task.Yield();
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0001D548 File Offset: 0x0001B748
		private async Task Crop(Rect rect)
		{
			await Task.Run(delegate
			{
				this.imageService.Crop(this.PreviewPage, this.PreviewSource.OriginalImage, rect);
			});
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x0001D594 File Offset: 0x0001B794
		private async Task Revert()
		{
			GAManager.SendEvent("ScannerWindowImageEdit", "RevertBtn", "Count", 1L);
			ScannedPage scannedPage = this.PreviewPage;
			if (scannedPage != null)
			{
				bool flag = !scannedPage.HasChanged();
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0001D5D7 File Offset: 0x0001B7D7
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

		// Token: 0x06000603 RID: 1539 RVA: 0x0001D60C File Offset: 0x0001B80C
		public void Dispose()
		{
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0001D610 File Offset: 0x0001B810
		private static async Task<IReadOnlyList<ScannerDeviceInfo>> RefreshScanDeviceInfosAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run<List<ScannerDeviceInfo>>(delegate
			{
				Thread.Sleep(500);
				return InsertFromScannerViewModel.scanService.GetDeviceInfos();
			}, cancellationToken).WaitAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0001D654 File Offset: 0x0001B854
		private static async Task<IReadOnlyList<ScannerDeviceInfo>> GetScanDeviceInfosAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run<List<ScannerDeviceInfo>>(() => InsertFromScannerViewModel.scanService.GetDeviceInfos(), cancellationToken).WaitAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x040002E1 RID: 737
		private static ScanService scanService = new ScanService();

		// Token: 0x040002E2 RID: 738
		private readonly List<string> savedProperties;

		// Token: 0x040002E3 RID: 739
		private DebounceHelper debounceSaveSettings = new DebounceHelper();

		// Token: 0x040002E4 RID: 740
		private ObservableCollection<ScannerDeviceInfo> scannerList;

		// Token: 0x040002E5 RID: 741
		private ObservableCollection<KeyValuePair<ScanSource, string>> sourceList;

		// Token: 0x040002E6 RID: 742
		private ScannerDeviceInfo selectedScanner;

		// Token: 0x040002E7 RID: 743
		private ScanSource selectedSource;

		// Token: 0x040002E8 RID: 744
		private ScanColor selectedColor;

		// Token: 0x040002E9 RID: 745
		private ScanResolution selectedResolution;

		// Token: 0x040002EA RID: 746
		private ScanArea selectedArea;

		// Token: 0x040002EB RID: 747
		private int brightness;

		// Token: 0x040002EC RID: 748
		private int contrast;

		// Token: 0x040002ED RID: 749
		private int saturation;

		// Token: 0x040002EE RID: 750
		private AsyncRelayCommand loadedCommand;

		// Token: 0x040002EF RID: 751
		private AsyncRelayCommand reloadScannerListCommand;

		// Token: 0x040002F0 RID: 752
		private AsyncRelayCommand scanCommand;

		// Token: 0x040002F1 RID: 753
		private bool Isenabled = true;

		// Token: 0x040002F2 RID: 754
		private static Dialog loading;

		// Token: 0x040002F3 RID: 755
		private Visibility visibility = Visibility.Collapsed;

		// Token: 0x040002F4 RID: 756
		private bool scannerBtnIsenable = true;

		// Token: 0x040002F5 RID: 757
		public CancellationTokenSource cts = new CancellationTokenSource();

		// Token: 0x040002F6 RID: 758
		private readonly ScannerImageService imageService = new ScannerImageService();

		// Token: 0x040002F7 RID: 759
		private bool? isCheckedAll = new bool?(false);

		// Token: 0x040002F8 RID: 760
		private IList selectedItems;

		// Token: 0x040002F9 RID: 761
		private ScannedPage previewPage;

		// Token: 0x040002FA RID: 762
		private ImageViewerSource previewSource;

		// Token: 0x040002FB RID: 763
		private double previewPageRotate;

		// Token: 0x040002FC RID: 764
		private int pageNumber;

		// Token: 0x040002FD RID: 765
		private bool inCheckedAll;

		// Token: 0x040002FE RID: 766
		private string[] dragFiles;

		// Token: 0x040002FF RID: 767
		private bool isRemovePages;

		// Token: 0x04000300 RID: 768
		private DebounceHelper debounceAdjust = new DebounceHelper();

		// Token: 0x04000301 RID: 769
		private AsyncRelayCommand importCommand;

		// Token: 0x04000302 RID: 770
		private AsyncRelayCommand showExportCommand;

		// Token: 0x04000303 RID: 771
		private AsyncRelayCommand exportCommand;

		// Token: 0x04000304 RID: 772
		private AsyncRelayCommand<string> rotateCommand;

		// Token: 0x04000305 RID: 773
		private AsyncRelayCommand<string> rotatePreviewCommand;

		// Token: 0x04000306 RID: 774
		private AsyncRelayCommand deleteCommand;

		// Token: 0x04000307 RID: 775
		private AsyncRelayCommand deletePreviewCommand;

		// Token: 0x04000308 RID: 776
		private AsyncRelayCommand<object> previewCommand;

		// Token: 0x04000309 RID: 777
		private AsyncRelayCommand previousPageCommand;

		// Token: 0x0400030A RID: 778
		private AsyncRelayCommand nextPageCommand;

		// Token: 0x0400030B RID: 779
		private AsyncRelayCommand openCropCommand;

		// Token: 0x0400030C RID: 780
		private AsyncRelayCommand<Rect> cropCommand;

		// Token: 0x0400030D RID: 781
		private AsyncRelayCommand revertCommand;

		// Token: 0x0400030E RID: 782
		private bool scannerConnecting = true;

		// Token: 0x04000316 RID: 790
		private ExportMode selectedExportMode;

		// Token: 0x04000317 RID: 791
		private bool isExportAll;

		// Token: 0x04000319 RID: 793
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };

		// Token: 0x02000348 RID: 840
		public class DropHandler : DefaultDropHandler
		{
			// Token: 0x17000C6B RID: 3179
			// (get) Token: 0x06002A13 RID: 10771 RVA: 0x000C9CFD File Offset: 0x000C7EFD
			private bool hasDragFiles
			{
				get
				{
					return this.viewModel.DragFiles != null;
				}
			}

			// Token: 0x06002A14 RID: 10772 RVA: 0x000C9D0D File Offset: 0x000C7F0D
			public DropHandler(InsertFromScannerViewModel viewModel)
			{
				this.viewModel = viewModel;
			}

			// Token: 0x06002A15 RID: 10773 RVA: 0x000C9D1C File Offset: 0x000C7F1C
			public override void DragOver(IDropInfo dropInfo)
			{
				base.DragOver(dropInfo);
				if (this.hasDragFiles)
				{
					dropInfo.Effects = DragDropEffects.Copy;
				}
			}

			// Token: 0x06002A16 RID: 10774 RVA: 0x000C9D34 File Offset: 0x000C7F34
			public override void Drop(IDropInfo dropInfo)
			{
				if (this.hasDragFiles)
				{
					this.viewModel.ImportFormDrag();
					return;
				}
				base.Drop(dropInfo);
			}

			// Token: 0x040013C9 RID: 5065
			private readonly InsertFromScannerViewModel viewModel;
		}
	}
}
