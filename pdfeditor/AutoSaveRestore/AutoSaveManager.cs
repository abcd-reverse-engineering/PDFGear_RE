using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Models;
using pdfeditor.Models.Operations;
using pdfeditor.ViewModels;

namespace pdfeditor.AutoSaveRestore
{
	// Token: 0x020002C1 RID: 705
	public class AutoSaveManager
	{
		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x06002884 RID: 10372 RVA: 0x000BEB93 File Offset: 0x000BCD93
		private MainViewModel vm
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06002885 RID: 10373 RVA: 0x000BEBA0 File Offset: 0x000BCDA0
		// (remove) Token: 0x06002886 RID: 10374 RVA: 0x000BEBD8 File Offset: 0x000BCDD8
		public event EventHandler SaveStarted;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06002887 RID: 10375 RVA: 0x000BEC10 File Offset: 0x000BCE10
		// (remove) Token: 0x06002888 RID: 10376 RVA: 0x000BEC48 File Offset: 0x000BCE48
		public event EventHandler SaveCompleted;

		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x06002889 RID: 10377 RVA: 0x000BEC7D File Offset: 0x000BCE7D
		// (set) Token: 0x0600288A RID: 10378 RVA: 0x000BEC85 File Offset: 0x000BCE85
		public string LastOperationVersion { get; set; }

		// Token: 0x17000C54 RID: 3156
		// (get) Token: 0x0600288B RID: 10379 RVA: 0x000BEC8E File Offset: 0x000BCE8E
		// (set) Token: 0x0600288C RID: 10380 RVA: 0x000BEC96 File Offset: 0x000BCE96
		public bool CanSaveByOperationManager { get; set; }

		// Token: 0x0600288D RID: 10381 RVA: 0x000BEC9F File Offset: 0x000BCE9F
		public AutoSaveManager()
		{
			this.savetimer = new Timer(new TimerCallback(this.ToSave), null, -1, 1000);
			this.LastOperationVersion = "";
			this.CanSaveByOperationManager = true;
		}

		// Token: 0x0600288E RID: 10382 RVA: 0x000BECD7 File Offset: 0x000BCED7
		public static AutoSaveManager GetInstance()
		{
			if (AutoSaveManager.m_stance == null)
			{
				AutoSaveManager.m_stance = new AutoSaveManager();
			}
			return AutoSaveManager.m_stance;
		}

		// Token: 0x0600288F RID: 10383 RVA: 0x000BECF0 File Offset: 0x000BCEF0
		private void ToSave(object filename)
		{
			if (!this.vm.CanSave)
			{
				return;
			}
			string lastOperationVersion = this.LastOperationVersion;
			OperationManager operationManager = this.vm.OperationManager;
			if (lastOperationVersion == ((operationManager != null) ? operationManager.Version : null))
			{
				return;
			}
			if (!this.CanSaveByOperationManager)
			{
				return;
			}
			EventHandler saveStarted = this.SaveStarted;
			if (saveStarted != null)
			{
				saveStarted(this, EventArgs.Empty);
			}
			string saveDir = AutoSaveManager.SaveDir;
			DocumentWrapper documentWrapper = this.vm.DocumentWrapper;
			PdfDocument document = documentWrapper.Document;
			if (!string.IsNullOrEmpty((documentWrapper != null) ? documentWrapper.DocumentPath : null))
			{
				FileInfo fileInfo = new FileInfo((documentWrapper != null) ? documentWrapper.DocumentPath : null);
				string text = fileInfo.Name;
				if (!string.IsNullOrEmpty(fileInfo.Extension))
				{
					text = text.Substring(0, text.Length - fileInfo.Extension.Length);
				}
				if (!Directory.Exists(saveDir))
				{
					Directory.CreateDirectory(saveDir);
				}
				string text2;
				string text4;
				do
				{
					text2 = Guid.NewGuid().ToString("N").ToLower();
					string text3 = text + "_" + text2;
					text3 += ".data";
					text4 = Path.Combine(saveDir, text3);
				}
				while (File.Exists(text4));
				string text5 = text4;
				int id = Process.GetCurrentProcess().Id;
				this.DelSameFileByProcess(id);
				AutoSaveFileModel autoSaveFileModel = new AutoSaveFileModel
				{
					Guid = text2,
					SoruceFileFullName = fileInfo.FullName,
					LastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
					FileName = fileInfo.Name,
					CreatePid = id,
					TempFileName = text5
				};
				try
				{
					using (FileStream fileStream = File.OpenWrite(text5))
					{
						fileStream.Seek(0L, SeekOrigin.Begin);
						document.Save(fileStream, SaveFlags.NoIncremental, 0);
						fileStream.SetLength(fileStream.Position);
					}
				}
				catch (PathTooLongException)
				{
					string text6 = Guid.NewGuid().ToString("N").ToLower();
					text5 = Path.Combine(saveDir, text6) + ".data";
					autoSaveFileModel.TempFileName = text5;
					autoSaveFileModel.Guid = text6;
					using (FileStream fileStream2 = File.OpenWrite(text5))
					{
						fileStream2.Seek(0L, SeekOrigin.Begin);
						document.Save(fileStream2, SaveFlags.NoIncremental, 0);
						fileStream2.SetLength(fileStream2.Position);
					}
				}
				catch (DirectoryNotFoundException)
				{
					string text7 = Guid.NewGuid().ToString("N").ToLower();
					text5 = Path.Combine(saveDir, text7) + ".data";
					autoSaveFileModel.TempFileName = text5;
					autoSaveFileModel.Guid = text7;
					using (FileStream fileStream3 = File.OpenWrite(text5))
					{
						fileStream3.Seek(0L, SeekOrigin.Begin);
						document.Save(fileStream3, SaveFlags.NoIncremental, 0);
						fileStream3.SetLength(fileStream3.Position);
					}
				}
				catch (Exception ex)
				{
					GAManager.SendEvent("Exception", "AutoSave", ex.GetType().Name + ", " + ex.Message, 1L);
				}
				finally
				{
					ConfigManager.SetAutoSaveFileAsync(autoSaveFileModel);
					EventHandler saveCompleted = this.SaveCompleted;
					if (saveCompleted != null)
					{
						saveCompleted(this, EventArgs.Empty);
					}
					OperationManager operationManager2 = this.vm.OperationManager;
					this.LastOperationVersion = ((operationManager2 != null) ? operationManager2.Version : null);
				}
			}
		}

		// Token: 0x06002890 RID: 10384 RVA: 0x000BF0F4 File Offset: 0x000BD2F4
		private void DelSameFileByProcess(int pid)
		{
			if (!Directory.Exists(AutoSaveManager.SaveDir))
			{
				return;
			}
			FileInfo[] files = new DirectoryInfo(AutoSaveManager.SaveDir).GetFiles();
			if (files.Length == 0)
			{
				return;
			}
			List<AutoSaveFileModel> result = ConfigManager.GetAutoSaveFilesAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (result == null)
			{
				return;
			}
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo f = files[i];
				AutoSaveFileModel autoSaveFileModel = result.Find((AutoSaveFileModel c) => c.TempFileName == f.FullName);
				if (autoSaveFileModel != null && f.Name.Contains(autoSaveFileModel.Guid) && pid == autoSaveFileModel.CreatePid)
				{
					f.Delete();
					ConfigManager.DelAutoSaveFileAsync(autoSaveFileModel.Guid, new int?(pid));
				}
			}
		}

		// Token: 0x06002891 RID: 10385 RVA: 0x000BF1C3 File Offset: 0x000BD3C3
		public void Start(int mins)
		{
			if (this.vm.AutoSaveModel.IsAuto)
			{
				this.autoSaveIntervalMinus = mins;
				this.isEnabled = true;
				this.savetimer.Change(0, 60000 * mins);
			}
		}

		// Token: 0x06002892 RID: 10386 RVA: 0x000BF1F9 File Offset: 0x000BD3F9
		public void Stop()
		{
			this.isEnabled = false;
			this.savetimer.Change(-1, 1000);
		}

		// Token: 0x06002893 RID: 10387 RVA: 0x000BF214 File Offset: 0x000BD414
		public void TrySaveImmediately()
		{
			bool flag = this.isEnabled;
			this.Stop();
			this.ToSave(null);
			if (flag)
			{
				this.Start(this.autoSaveIntervalMinus);
			}
		}

		// Token: 0x04001162 RID: 4450
		private static AutoSaveManager m_stance;

		// Token: 0x04001163 RID: 4451
		private Timer savetimer;

		// Token: 0x04001164 RID: 4452
		private bool isEnabled;

		// Token: 0x04001165 RID: 4453
		private int autoSaveIntervalMinus;

		// Token: 0x04001166 RID: 4454
		public static string SaveDir = Path.Combine(AppDataHelper.LocalFolder, "BackUp");

		// Token: 0x04001167 RID: 4455
		public static string MutexOperationID = "d672d331-57aa-4230-b3ce-d0488770978f";
	}
}
