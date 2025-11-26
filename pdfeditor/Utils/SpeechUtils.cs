using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Windows;
using CommonLib.Common;
using CommonLib.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using NAudio.Wave;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Speech;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using SoundTouch.Net.NAudioSupport;

namespace pdfeditor.Utils
{
	// Token: 0x020000A4 RID: 164
	public class SpeechUtils : IDisposable
	{
		// Token: 0x06000A25 RID: 2597 RVA: 0x0003389C File Offset: 0x00031A9C
		public SpeechUtils(PdfDocument PdfDocument)
		{
			this.pdfDocument = PdfDocument;
			this.Rate = 0.0;
			this.Volume = 60f;
			if (this.viewModel.ReadCulIndex < 0)
			{
				this.CultureIndex = this.GetcultureIndex();
				this.viewModel.ReadCulIndex = this.CultureIndex;
				return;
			}
			this.CultureIndex = this.viewModel.ReadCulIndex;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00033957 File Offset: 0x00031B57
		public void SpeakCurrentPage(int PageIndex)
		{
			this.Pagesindex = PageIndex;
			this.Pageend = PageIndex;
			this.Reading(PageIndex);
			this.Readcurent = true;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00033978 File Offset: 0x00031B78
		public bool Setculture(int index, Window window)
		{
			bool flag = false;
			if (this._waveOut.PlaybackState == PlaybackState.Playing)
			{
				flag = true;
			}
			if (new SpeechMessage(this.speechSynthesizer.GetInstalledVoices()[index].VoiceInfo.Culture.DisplayName, this.Pagesindex)
			{
				Owner = window,
				WindowStartupLocation = WindowStartupLocation.Manual,
				Top = window.Top + 130.0,
				Left = window.Left - 50.0
			}.ShowDialog().Value)
			{
				this.CultureIndex = index;
				this.viewModel.ReadCulIndex = index;
				if (this._waveOut != null)
				{
					this._waveOut.PlaybackStopped -= this.OnPlaybackStopped;
				}
				this.Close();
				this.Reading(this.Pagesindex);
				return true;
			}
			if (flag)
			{
				this.Play();
			}
			return false;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00033A5C File Offset: 0x00031C5C
		public void Activated()
		{
			if (this._waveOut != null)
			{
				this._waveOut.PlaybackStopped -= this.OnPlaybackStopped;
			}
			SpeechSynthesizer speechSynthesizer = this.speechSynthesizer;
			if (speechSynthesizer != null)
			{
				speechSynthesizer.Pause();
			}
			SpeechSynthesizer speechSynthesizer2 = this.speechSynthesizer;
			if (speechSynthesizer2 != null)
			{
				speechSynthesizer2.Dispose();
			}
			this.Pageend = -1;
			MemoryStream memoryStream = this.memoryStream;
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
			this.Close();
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00033AC8 File Offset: 0x00031CC8
		public void PauseSpeak()
		{
			try
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				if (this._waveOut.PlaybackState == PlaybackState.Playing)
				{
					this.Pause();
					this.Pausing = true;
					requiredService.speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Play.png";
				}
				else
				{
					this.Play();
					this.Pausing = false;
					requiredService.speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Pause.png";
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00033B4C File Offset: 0x00031D4C
		public bool isSpeak()
		{
			return this._waveOut != null && this._waveOut.PlaybackState == PlaybackState.Playing;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00033B67 File Offset: 0x00031D67
		public void SpeakPages(int PageStart, int PageEnd)
		{
			this.Pageend = PageEnd;
			this.Pagesindex = PageStart;
			this.Readcurent = false;
			if (PageEnd > 1)
			{
				this.Reading(this.Pagesindex);
				return;
			}
			this.Reading(this.Pagesindex);
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00033B9C File Offset: 0x00031D9C
		private void Reading(int PageIndex)
		{
			try
			{
				SpeechSynthesizer speechSynthesizer = this.speechSynthesizer;
				if (speechSynthesizer != null)
				{
					speechSynthesizer.Dispose();
				}
				this.speechSynthesizer = new SpeechSynthesizer();
				this.viewModel.IsReading = true;
				this.speechSynthesizer.SpeakCompleted += this.SpeechSynthesizer_SpeakCompleted;
				this.speechSynthesizer.Volume = 100;
				ReadOnlyCollection<InstalledVoice> installedVoices = this.speechSynthesizer.GetInstalledVoices();
				this.speechSynthesizer.SelectVoice(installedVoices[this.CultureIndex].VoiceInfo.Name);
				this.read = SpeechUtils.ExtractTextFromPage(this.pdfDocument, PageIndex).Replace("™", " ").Replace("\r\n\r\n", "   ")
					.Replace("\r\n", "");
				this.memoryStream = new MemoryStream();
				this.speechSynthesizer.SetOutputToWaveStream(this.memoryStream);
				if ((string.IsNullOrEmpty(this.read) || string.IsNullOrEmpty(this.read.Replace(" ", ""))) && this.Readcurent)
				{
					MessageBox.Show(App.Current.MainWindow, pdfeditor.Properties.Resources.ReadPageEmpty.Replace("XXX", (PageIndex + 1).ToString()), UtilManager.GetProductName());
					ContextMenuModel contextMenuModel = (this.viewModel.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
					(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = true;
					(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = true;
					(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = true;
					(contextMenuModel[0] as SpeedContextMenuItemModel).Icon = null;
					(contextMenuModel[1] as SpeedContextMenuItemModel).Icon = null;
					(contextMenuModel[2] as SpeedContextMenuItemModel).Icon = null;
					this.viewModel.ViewToolbar.ReadButtonModel.IsChecked = false;
					this.viewModel.IsReading = false;
					if (this.viewModel.speechControl != null)
					{
						this.viewModel.speechControl.StopBtn.IsEnabled = false;
					}
				}
				else
				{
					this.Readcurent = false;
					this.Readed = true;
					this.speechSynthesizer.SpeakAsync(this.read);
					MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
					if (requiredService.speechControl != null)
					{
						requiredService.speechControl.StopBtn.IsEnabled = true;
						requiredService.speechControl.GlobeBtn.IsEnabled = true;
					}
				}
			}
			catch
			{
				try
				{
					this.read = SpeechUtils.ExtractTextFromPage(this.pdfDocument, PageIndex).Replace("™", " ").Replace("\r\n\r\n", "   ")
						.Replace("\r\n", "");
					this.memoryStream = new MemoryStream();
					this.speechSynthesizer.SetOutputToWaveStream(this.memoryStream);
					if ((string.IsNullOrEmpty(this.read) || string.IsNullOrEmpty(this.read.Replace(" ", ""))) && this.Readcurent)
					{
						MessageBox.Show(App.Current.MainWindow, pdfeditor.Properties.Resources.ReadPageEmpty.Replace("XXX", (PageIndex + 1).ToString()), UtilManager.GetProductName());
						ContextMenuModel contextMenuModel2 = (this.viewModel.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
						(contextMenuModel2[2] as SpeedContextMenuItemModel).IsEnabled = true;
						(contextMenuModel2[1] as SpeedContextMenuItemModel).IsEnabled = true;
						(contextMenuModel2[0] as SpeedContextMenuItemModel).IsEnabled = true;
						(contextMenuModel2[0] as SpeedContextMenuItemModel).Icon = null;
						(contextMenuModel2[1] as SpeedContextMenuItemModel).Icon = null;
						(contextMenuModel2[2] as SpeedContextMenuItemModel).Icon = null;
						this.viewModel.ViewToolbar.ReadButtonModel.IsChecked = false;
						this.viewModel.IsReading = false;
						if (this.viewModel.speechControl != null)
						{
							this.viewModel.speechControl.StopBtn.IsEnabled = false;
						}
					}
					else
					{
						this.Readcurent = false;
						this.Readed = true;
						this.speechSynthesizer.SpeakAsync(this.read);
						MainViewModel requiredService2 = Ioc.Default.GetRequiredService<MainViewModel>();
						if (requiredService2.speechControl != null)
						{
							requiredService2.speechControl.StopBtn.IsEnabled = true;
							requiredService2.speechControl.GlobeBtn.IsEnabled = true;
						}
					}
				}
				catch
				{
					MessageBox.Show(App.Current.MainWindow, pdfeditor.Properties.Resources.ReadPageEmpty.Replace("XXX", (PageIndex + 1).ToString()), UtilManager.GetProductName());
				}
			}
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00034070 File Offset: 0x00032270
		public int GetcultureIndex()
		{
			try
			{
				SpeechSynthesizer speechSynthesizer = this.speechSynthesizer;
				if (speechSynthesizer != null)
				{
					speechSynthesizer.Dispose();
				}
				this.speechSynthesizer = new SpeechSynthesizer();
				CultureInfo culture = CommonLib.Properties.Resources.Culture;
				ReadOnlyCollection<InstalledVoice> installedVoices = this.speechSynthesizer.GetInstalledVoices();
				for (int i = 0; i < installedVoices.Count; i++)
				{
					if (culture.ToString() == installedVoices[i].VoiceInfo.Culture.ToString())
					{
						this.CultureIndex = i;
						this.viewModel.ReadCulIndex = i;
						return i;
					}
				}
			}
			catch
			{
			}
			return 0;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00034110 File Offset: 0x00032310
		private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
		{
			try
			{
				if (this.Readed)
				{
					this.memoryStream.Seek(0L, SeekOrigin.Begin);
					WaveStream waveStream = new WaveFileReader(this.memoryStream);
					if (this.OpenFile(waveStream))
					{
						this.Play();
					}
				}
				this.Readed = false;
				if (this.Pageend <= this.Pagesindex && (string.IsNullOrEmpty(this.read) || string.IsNullOrEmpty(this.read.Replace(" ", ""))))
				{
					MessageBox.Show(App.Current.MainWindow, pdfeditor.Properties.Resources.ReadPageEmpty.Replace("XXX", (this.Pagesindex + 1).ToString()), UtilManager.GetProductName());
				}
			}
			catch
			{
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000A2F RID: 2607 RVA: 0x000341D8 File Offset: 0x000323D8
		// (set) Token: 0x06000A30 RID: 2608 RVA: 0x000341E0 File Offset: 0x000323E0
		public SoundTouchWaveStream ProcessorStream { get; private set; }

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000A31 RID: 2609 RVA: 0x000341EC File Offset: 0x000323EC
		// (remove) Token: 0x06000A32 RID: 2610 RVA: 0x00034224 File Offset: 0x00032424
		public event EventHandler<bool> PlaybackStopped = delegate(object _, bool __)
		{
		};

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000A33 RID: 2611 RVA: 0x00034259 File Offset: 0x00032459
		// (set) Token: 0x06000A34 RID: 2612 RVA: 0x00034261 File Offset: 0x00032461
		public double Pitch
		{
			get
			{
				return this._pitch;
			}
			set
			{
				this.Set<double>(ref this._pitch, value, "Pitch");
				if (this.ProcessorStream != null)
				{
					this.ProcessorStream.PitchSemiTones = value;
				}
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00034289 File Offset: 0x00032489
		// (set) Token: 0x06000A36 RID: 2614 RVA: 0x00034291 File Offset: 0x00032491
		public float SpeechVolume
		{
			get
			{
				return this.Volume;
			}
			set
			{
				this.Set<float>(ref this.Volume, value, "SpeechVolume");
				if (this._waveOut != null)
				{
					this._waveOut.Volume = value / 100f;
				}
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x000342BF File Offset: 0x000324BF
		// (set) Token: 0x06000A38 RID: 2616 RVA: 0x000342C7 File Offset: 0x000324C7
		public double Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				this.Set<double>(ref this._rate, value, "Rate");
				if (this.ProcessorStream != null)
				{
					this.ProcessorStream.RateChange = value;
				}
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x000342EF File Offset: 0x000324EF
		// (set) Token: 0x06000A3A RID: 2618 RVA: 0x000342F7 File Offset: 0x000324F7
		public int Tempo
		{
			get
			{
				return this._tempo;
			}
			set
			{
				this.Set<int>(ref this._tempo, value, "Tempo");
				if (this.ProcessorStream != null)
				{
					this.ProcessorStream.TempoChange = (double)value;
				}
			}
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00034320 File Offset: 0x00032520
		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
			{
				return;
			}
			storage = value;
			this.OnPropertyChanged(propertyName);
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000A3C RID: 2620 RVA: 0x0003434C File Offset: 0x0003254C
		// (remove) Token: 0x06000A3D RID: 2621 RVA: 0x00034384 File Offset: 0x00032584
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000A3E RID: 2622 RVA: 0x000343B9 File Offset: 0x000325B9
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x000343D4 File Offset: 0x000325D4
		private bool OpenFile(WaveStream wave)
		{
			this.Close();
			bool flag;
			try
			{
				WaveChannel32 waveChannel = new WaveChannel32(wave)
				{
					PadWithZeroes = false
				};
				this.ProcessorStream = new SoundTouchWaveStream(waveChannel);
				this._waveOut = new WaveOutEvent
				{
					DesiredLatency = 100
				};
				this._waveOut.Init(this.ProcessorStream);
				this._waveOut.PlaybackStopped += this.OnPlaybackStopped;
				flag = true;
			}
			catch (Exception)
			{
				this._waveOut = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00034460 File Offset: 0x00032660
		private void Close()
		{
			SoundTouchWaveStream processorStream = this.ProcessorStream;
			if (processorStream != null)
			{
				processorStream.Dispose();
			}
			this.ProcessorStream = null;
			IWavePlayer waveOut = this._waveOut;
			if (waveOut != null)
			{
				waveOut.Dispose();
			}
			this._waveOut = null;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00034494 File Offset: 0x00032694
		private void speechQuit()
		{
			bool flag = false;
			using (IEnumerator enumerator = Application.Current.Windows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((Window)enumerator.Current).GetType() == typeof(SpeechControl))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService.speechControl.StopBtn.IsEnabled = false;
				requiredService.speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Play.png";
			}
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00034538 File Offset: 0x00032738
		private void OnPlaybackStopped(object sender, StoppedEventArgs args)
		{
			bool flag = this.ProcessorStream == null || this.ProcessorStream.Position >= this.ProcessorStream.Length;
			if (this.Pageend == -1)
			{
				return;
			}
			if (flag)
			{
				if (this.Pageend > this.Pagesindex)
				{
					try
					{
						this.Pagesindex++;
						this.read = SpeechUtils.ExtractTextFromPage(this.pdfDocument, this.Pagesindex).Replace("™", " ").Replace("\r\n\r\n", "   ")
							.Replace("\r\n", "");
						this.memoryStream.Close();
						this.memoryStream = new MemoryStream();
						this.memoryStream.Seek(0L, SeekOrigin.Begin);
						this.speechSynthesizer.SetOutputToWaveStream(this.memoryStream);
						this.Readed = true;
						if (!string.IsNullOrEmpty(this.read))
						{
							this.speechSynthesizer.SpeakAsync(this.read);
						}
						goto IL_0207;
					}
					catch
					{
						goto IL_0207;
					}
				}
				ContextMenuModel contextMenuModel = (this.viewModel.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
				(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = true;
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = true;
				(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = true;
				(contextMenuModel[0] as SpeedContextMenuItemModel).Icon = null;
				(contextMenuModel[1] as SpeedContextMenuItemModel).Icon = null;
				(contextMenuModel[2] as SpeedContextMenuItemModel).Icon = null;
				this.viewModel.ViewToolbar.ReadButtonModel.IsChecked = false;
				this.viewModel.IsReading = false;
				SoundTouchWaveStream processorStream = this.ProcessorStream;
				if (processorStream != null)
				{
					processorStream.Dispose();
				}
				this.ProcessorStream = null;
				if (this.viewModel.speechControl != null)
				{
					this.viewModel.speechControl.StopBtn.IsEnabled = false;
					this.viewModel.speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Play.png";
				}
			}
			IL_0207:
			this.PlaybackStopped(sender, flag);
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0003476C File Offset: 0x0003296C
		public bool Play()
		{
			if (this._waveOut == null)
			{
				return false;
			}
			if (this._waveOut.PlaybackState != PlaybackState.Playing)
			{
				this._waveOut.Volume = this.SpeechVolume / 100f;
				this.ProcessorStream.RateChange = this.Rate;
				this.ProcessorStream.TempoChange = this.Rate;
				this.ProcessorStream.PitchSemiTones = this.Pitch;
				this._waveOut.Play();
				this.Pausing = false;
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				if (requiredService.speechControl != null)
				{
					requiredService.speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Pause.png";
				}
			}
			return true;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0003481C File Offset: 0x00032A1C
		public bool Pause()
		{
			if (this._waveOut == null)
			{
				return false;
			}
			if (this._waveOut.PlaybackState == PlaybackState.Playing)
			{
				this._waveOut.Pause();
				this.Pausing = true;
				Ioc.Default.GetRequiredService<MainViewModel>().speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Play.png";
				return true;
			}
			return false;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00034874 File Offset: 0x00032A74
		public bool Stop()
		{
			if (this._waveOut == null || this.ProcessorStream == null || this.ProcessorStream.Length == 0L)
			{
				return false;
			}
			this._waveOut.Stop();
			this.ProcessorStream.Position = 0L;
			this.ProcessorStream.Flush();
			this.Pausing = true;
			Ioc.Default.GetRequiredService<MainViewModel>().speechControl.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Play.png";
			return true;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x000348EC File Offset: 0x00032AEC
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					SpeechSynthesizer speechSynthesizer = this.speechSynthesizer;
					if (speechSynthesizer != null)
					{
						speechSynthesizer.Pause();
					}
					SpeechSynthesizer speechSynthesizer2 = this.speechSynthesizer;
					if (speechSynthesizer2 != null)
					{
						speechSynthesizer2.Dispose();
					}
					this.Pageend = -1;
					MemoryStream memoryStream = this.memoryStream;
					if (memoryStream != null)
					{
						memoryStream.Dispose();
					}
					this.speechQuit();
					this.Close();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00034951 File Offset: 0x00032B51
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00034960 File Offset: 0x00032B60
		public static string ExtractTextFromPage(PdfDocument document, int pageIndex)
		{
			if (document == null || pageIndex < 0 || pageIndex >= document.Pages.Count)
			{
				return string.Empty;
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			string text;
			try
			{
				intPtr = Pdfium.FPDF_LoadPage(document.Handle, pageIndex);
				intPtr2 = Pdfium.FPDFText_LoadPage(intPtr);
				int num = Pdfium.FPDFText_CountChars(intPtr2);
				if (num > 0)
				{
					text = Pdfium.FPDFText_GetText(intPtr2, 0, num) ?? string.Empty;
				}
				else
				{
					text = string.Empty;
				}
			}
			finally
			{
				if (intPtr2 != IntPtr.Zero)
				{
					try
					{
						Pdfium.FPDFText_ClosePage(intPtr2);
					}
					catch
					{
					}
				}
				if (intPtr != IntPtr.Zero)
				{
					try
					{
						Pdfium.FPDF_ClosePage(intPtr);
					}
					catch
					{
					}
				}
			}
			return text;
		}

		// Token: 0x04000489 RID: 1161
		private PdfDocument pdfDocument;

		// Token: 0x0400048A RID: 1162
		public SpeechSynthesizer speechSynthesizer;

		// Token: 0x0400048B RID: 1163
		private MemoryStream memoryStream;

		// Token: 0x0400048C RID: 1164
		public bool IsReading;

		// Token: 0x0400048D RID: 1165
		private bool Readed;

		// Token: 0x0400048E RID: 1166
		private bool Readcurent = true;

		// Token: 0x0400048F RID: 1167
		private string read;

		// Token: 0x04000490 RID: 1168
		private double _pitch;

		// Token: 0x04000491 RID: 1169
		private double _rate;

		// Token: 0x04000492 RID: 1170
		private int _tempo;

		// Token: 0x04000493 RID: 1171
		private float Volume;

		// Token: 0x04000494 RID: 1172
		private int Pageend = 1;

		// Token: 0x04000495 RID: 1173
		private int Pagesindex;

		// Token: 0x04000496 RID: 1174
		public bool Pausing = true;

		// Token: 0x04000497 RID: 1175
		public int CultureIndex;

		// Token: 0x04000498 RID: 1176
		public bool toneChange;

		// Token: 0x04000499 RID: 1177
		private MainViewModel viewModel = Ioc.Default.GetRequiredService<MainViewModel>();

		// Token: 0x0400049C RID: 1180
		private IWavePlayer _waveOut;

		// Token: 0x0400049E RID: 1182
		private bool disposedValue;
	}
}
