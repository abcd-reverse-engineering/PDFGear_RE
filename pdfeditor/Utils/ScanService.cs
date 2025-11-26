using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;
using NAPS2.Wia;
using Newtonsoft.Json;
using pdfeditor.Models.Scan;
using pdfeditor.Utils.Scan;

namespace pdfeditor.Utils
{
	// Token: 0x0200009E RID: 158
	internal class ScanService
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000A00 RID: 2560 RVA: 0x00032BA0 File Offset: 0x00030DA0
		// (remove) Token: 0x06000A01 RID: 2561 RVA: 0x00032BD8 File Offset: 0x00030DD8
		public event Action<List<ScannerDeviceInfo>> RunGetDeviceInfosTaskCompleted;

		// Token: 0x06000A02 RID: 2562 RVA: 0x00032C10 File Offset: 0x00030E10
		public ScanSettings LoadSettings(string deviceId)
		{
			string text = "device-" + deviceId;
			try
			{
				string setting = ConfigManager.GetSetting(text);
				if (!string.IsNullOrEmpty(setting))
				{
					return JsonConvert.DeserializeObject<ScanSettings>(setting);
				}
			}
			catch (Exception)
			{
			}
			return new ScanSettings();
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00032C60 File Offset: 0x00030E60
		public void SaveSettings(ScanSettings settings)
		{
			string text = "device-" + settings.Device.Id;
			try
			{
				string text2 = JsonConvert.SerializeObject(settings);
				if (!string.IsNullOrEmpty(text2))
				{
					ConfigManager.SetSetting(text2, text);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00032CB0 File Offset: 0x00030EB0
		public List<ScannerDeviceInfo> GetDeviceInfos()
		{
			List<ScannerDeviceInfo> list2;
			using (WiaDeviceManager wiaDeviceManager = new WiaDeviceManager())
			{
				List<ScannerDeviceInfo> list = (from x in wiaDeviceManager.GetDeviceInfos()
					select new ScannerDeviceInfo(x)).ToList<ScannerDeviceInfo>();
				using (List<ScannerDeviceInfo>.Enumerator enumerator = ScanService.GetLocalScanners().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ScannerDeviceInfo device = enumerator.Current;
						if (list.FindIndex((ScannerDeviceInfo x) => x.Name.ToLower().Replace(" ", "") == device.Name.ToLower().Replace(" ", "")) == -1)
						{
							list.Add(device);
						}
					}
				}
				list2 = list;
			}
			return list2;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00032D7C File Offset: 0x00030F7C
		public static List<ScannerDeviceInfo> GetLocalScanners()
		{
			ManagementScope managementScope = new ManagementScope(ManagementPath.DefaultPath);
			ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{6bdd1fc6-810f-11d0-bec7-08002be2092f}'");
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
			List<ScannerDeviceInfo> list = new List<ScannerDeviceInfo>();
			foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
			{
				List<ScannerDeviceInfo> list2 = list;
				ScannerDeviceInfo scannerDeviceInfo = new ScannerDeviceInfo();
				object obj = managementBaseObject["DeviceID"];
				scannerDeviceInfo.Id = ((obj != null) ? obj.ToString() : null);
				object obj2 = managementBaseObject["Name"];
				scannerDeviceInfo.Name = ((obj2 != null) ? obj2.ToString() : null);
				list2.Add(scannerDeviceInfo);
			}
			return list;
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00032E28 File Offset: 0x00031028
		public void RunGetDeviceInfosTask()
		{
			this.deviceInfosTask = new Task(async delegate
			{
				List<ScannerDeviceInfo> deviceInfos;
				for (;;)
				{
					try
					{
						deviceInfos = this.GetDeviceInfos();
						if (deviceInfos.Count > 0)
						{
							break;
						}
					}
					catch (Exception)
					{
					}
					await Task.Delay(3000);
				}
				Action<List<ScannerDeviceInfo>> runGetDeviceInfosTaskCompleted = this.RunGetDeviceInfosTaskCompleted;
				if (runGetDeviceInfosTaskCompleted != null)
				{
					runGetDeviceInfosTaskCompleted(deviceInfos);
				}
				this.deviceInfosTask = null;
			}, TaskCreationOptions.LongRunning);
			this.deviceInfosTask.Start();
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00032E4D File Offset: 0x0003104D
		public Task<WiaDevice> FindDevice(string id)
		{
			return Task.Run<WiaDevice>(delegate
			{
				try
				{
					using (WiaDeviceManager wiaDeviceManager = new WiaDeviceManager())
					{
						return wiaDeviceManager.FindDevice(id);
					}
				}
				catch (Exception)
				{
				}
				return null;
			});
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00032E6C File Offset: 0x0003106C
		public async Task<bool> Scan(ScanSettings settings, WiaDevice wiaDevice, CancellationToken cancellationToken)
		{
			WiaItem wiaItem = null;
			try
			{
				if (settings.Source != ScanSource.Glass && !wiaDevice.SupportsFeeder())
				{
					settings.Source = ScanSource.Glass;
				}
				if (settings.Source == ScanSource.Duplex && !wiaDevice.SupportsDuplex())
				{
					settings.Source = ScanSource.Glass;
				}
				List<WiaItem> subItems = wiaDevice.GetSubItems();
				if (subItems.Count == 0)
				{
					return false;
				}
				string name = ((settings.Source == ScanSource.Glass) ? "Flatbed" : "Feeder");
				wiaItem = subItems.FirstOrDefault((WiaItem x) => x.Name() == name) ?? subItems.First<WiaItem>();
				this.ApplySettings(ref wiaItem, settings, wiaDevice);
				int count = 0;
				using (WiaTransfer wiaTransfer = wiaItem.StartTransfer())
				{
					wiaTransfer.PageScanned += delegate(object sender, [Nullable(1)] WiaTransfer.PageScannedEventArgs args)
					{
						cancellationToken.ThrowIfCancellationRequested();
						using (args.Stream)
						{
							using (Bitmap bitmap = new Bitmap(args.Stream))
							{
								PageService.OnPageAdded(new ScannedPage(bitmap, settings.Brightness, settings.Contrast, settings.Saturation));
								int count2 = count;
								count = count2 + 1;
							}
						}
					};
					cancellationToken.ThrowIfCancellationRequested();
					if (wiaTransfer.Download())
					{
						return count > 0;
					}
				}
			}
			finally
			{
				if (wiaItem != null)
				{
					wiaItem.Dispose();
				}
				if (wiaDevice != null)
				{
					wiaDevice.Dispose();
				}
			}
			return false;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00032EC8 File Offset: 0x000310C8
		public void ApplySettings(ref WiaItem item, ScanSettings settings, WiaDevice device)
		{
			if (settings.Source != ScanSource.Glass)
			{
				item.SetProperty(3096, 0);
			}
			if (settings.Source == ScanSource.Feeder)
			{
				item.SetProperty(3088, 32);
			}
			else if (settings.Source == ScanSource.Duplex)
			{
				item.SetProperty(3088, 4);
			}
			item.SetProperty(4103, (int)settings.Color);
			int resolution = (int)settings.Resolution;
			int num = resolution;
			item.SetPropertyClosest(6147, ref resolution);
			item.SetPropertyClosest(6148, ref num);
			if (settings.Area > ScanArea.Auto)
			{
				try
				{
					PaperSizeAttribute paperSizeAttribute = EnumHelper.Get<PaperSizeAttribute>(settings.Area);
					int num2 = (int)Math.Round(PaperSizeConverter.Convert(paperSizeAttribute.Width, paperSizeAttribute.Unit, PaperSizeUnit.Inch, 96.0) * (double)resolution);
					int num3 = (int)Math.Round(PaperSizeConverter.Convert(paperSizeAttribute.Height, paperSizeAttribute.Unit, PaperSizeUnit.Inch, 96.0) * (double)num);
					int num4 = (int)item.Properties[6165].Value;
					int num5 = (int)item.Properties[6166].Value;
					int num6 = num4 * resolution / 1000;
					int num7 = num5 * num / 1000;
					if (num2 > num6)
					{
						num2 = num6;
					}
					if (num3 > num7)
					{
						num3 = num7;
					}
					item.SetProperty(6151, num2);
					item.SetProperty(6152, num3);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x04000485 RID: 1157
		private Task deviceInfosTask;
	}
}
