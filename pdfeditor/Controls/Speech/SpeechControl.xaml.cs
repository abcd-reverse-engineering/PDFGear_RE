using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Speech
{
	// Token: 0x020001EF RID: 495
	public partial class SpeechControl : Window
	{
		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x000741F2 File Offset: 0x000723F2
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x00074200 File Offset: 0x00072400
		public SpeechControl()
		{
			this.InitializeComponent();
			this.imgaeFilePath = new ImgaeFilePath();
			if (this.VM.speechUtils != null)
			{
				this.VolumeBlock.Text = string.Format("{0}", this.VM.speechUtils.SpeechVolume);
				this.VolumeSlider.Value = (double)Convert.ToInt32(this.VM.speechUtils.SpeechVolume);
				this.ToneSli.Value = (double)Convert.ToInt32(this.VM.speechUtils.Pitch + 5.0);
				this.SpeedSli.Value = (double)Convert.ToInt32((this.VM.speechUtils.Rate + 10.0) / 2.0);
				if (this.VM.ViewToolbar.ReadButtonModel.IsChecked)
				{
					this.StopBtn.IsEnabled = true;
				}
				this.GlobeBtn.IsEnabled = true;
				if (this.VM.speechUtils.isSpeak())
				{
					this.imgaeFilePath.ImagePath = "pack://application:,,,/Style/Resources/Speech/Pause.png";
				}
			}
			else
			{
				this.VolumeBlock.Text = "60";
				this.VolumeSlider.Value = 60.0;
				this.ToneSli.Value = 5.0;
				this.SpeedSli.Value = 5.0;
				this.StopBtn.IsEnabled = false;
			}
			this.VolumeSlider.ValueChanged += this.VolumeSlider_ValueChanged;
			this.SpeedSli.ValueChanged += this.SpeedSli_ValueChanged;
			this.ToneSli.ValueChanged += this.ToneSli_ValueChanged;
			Binding binding = new Binding();
			binding.Source = this.imgaeFilePath;
			binding.Path = new PropertyPath("ImagePath", Array.Empty<object>());
			BindingOperations.SetBinding(this.Start, Image.SourceProperty, binding);
			base.Closed += this.SpeechControl_Closed;
			(((Ioc.Default.GetRequiredService<MainViewModel>().ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel)[3] as SpeechToolContextMenuItemModel).Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Speech/Checked.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Speech/Checked.png"));
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x0007446C File Offset: 0x0007266C
		private void SpeechControl_Closed(object sender, EventArgs e)
		{
			(((Ioc.Default.GetRequiredService<MainViewModel>().ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel)[3] as SpeechToolContextMenuItemModel).Icon = null;
			this.VolumeSlider.ValueChanged -= this.VolumeSlider_ValueChanged;
			this.SpeedSli.ValueChanged -= this.SpeedSli_ValueChanged;
			this.ToneSli.ValueChanged -= this.ToneSli_ValueChanged;
			this.VM.speechControl = null;
			base.Owner.Activate();
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x0007450F File Offset: 0x0007270F
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Speed.IsOpen = true;
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x0007451D File Offset: 0x0007271D
		private void TonedBtn_Click(object sender, RoutedEventArgs e)
		{
			this.Tone.IsOpen = true;
		}

		// Token: 0x06001C09 RID: 7177 RVA: 0x0007452C File Offset: 0x0007272C
		private void GlobeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.CultureListBox.Items.Count == 0)
			{
				this.CultureListBox.ItemsSource = this.Getculture();
				if (this.VM.ReadCulIndex < 0)
				{
					this.CultureListBox.SelectedIndex = this.GetcultureIndex();
				}
				else
				{
					this.CultureListBox.SelectedIndex = this.VM.ReadCulIndex;
				}
				this.CultureListBox.SelectionChanged += this.CultureListBox_SelectionChanged;
			}
			this.Globe.IsOpen = true;
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x000745B8 File Offset: 0x000727B8
		private string[] Getculture()
		{
			SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
			ReadOnlyCollection<InstalledVoice> installedVoices = speechSynthesizer.GetInstalledVoices();
			string[] array = new string[installedVoices.Count];
			int num = 0;
			foreach (InstalledVoice installedVoice in installedVoices)
			{
				string text = pdfeditor.Properties.Resources.ReadWinFemaleVoice;
				if (installedVoice.VoiceInfo.Gender.ToString() == "Male")
				{
					text = pdfeditor.Properties.Resources.ReadWinMaleVoice;
				}
				array[num] = installedVoice.VoiceInfo.Culture.DisplayName.ToString().Replace(")", ", " + text + " )");
				num++;
			}
			speechSynthesizer.Dispose();
			return array;
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x00074688 File Offset: 0x00072888
		public int GetcultureIndex()
		{
			try
			{
				SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
				CultureInfo culture = CommonLib.Properties.Resources.Culture;
				ReadOnlyCollection<InstalledVoice> installedVoices = speechSynthesizer.GetInstalledVoices();
				for (int i = 0; i < installedVoices.Count; i++)
				{
					if (culture.ToString() == installedVoices[i].VoiceInfo.Culture.ToString())
					{
						return i;
					}
				}
			}
			catch
			{
			}
			return 0;
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x000746F8 File Offset: 0x000728F8
		private void CultureListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = (ListBox)sender;
			if (listBox != null)
			{
				if (this.VM.IsReading && listBox.SelectedIndex != this.VM.speechUtils.CultureIndex)
				{
					if (!this.VM.speechUtils.Setculture(listBox.SelectedIndex, this))
					{
						listBox.SelectedIndex = this.VM.speechUtils.CultureIndex;
						this.Globe.IsOpen = false;
						return;
					}
					this.VolumeBlock.Text = string.Format("{0}", this.VM.speechUtils.SpeechVolume);
					this.VolumeSlider.Value = (double)Convert.ToInt32(this.VM.speechUtils.SpeechVolume);
					this.ToneSli.Value = (double)Convert.ToInt32(this.VM.speechUtils.Pitch + 5.0);
					this.SpeedSli.Value = (double)Convert.ToInt32((this.VM.speechUtils.Rate + 10.0) / 2.0);
				}
				else
				{
					this.VM.ReadCulIndex = listBox.SelectedIndex;
				}
			}
			this.Globe.IsOpen = false;
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x00074844 File Offset: 0x00072A44
		private void SpeedSli_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				double newValue = e.NewValue;
				if (this.VM.speechUtils != null)
				{
					GAManager.SendEvent("PDFReader", "SpeedChange", "Count", 1L);
					this.VM.speechUtils.Rate = newValue * 2.0 - 10.0;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x000748B8 File Offset: 0x00072AB8
		private void ToneSli_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				double newValue = e.NewValue;
				if (this.VM.speechUtils != null)
				{
					GAManager.SendEvent("PDFReader", "ToneChange", "Count", 1L);
					this.VM.speechUtils.Pitch = newValue - 5.0;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x00074920 File Offset: 0x00072B20
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				double newValue = e.NewValue;
				if (this.VM.speechUtils != null)
				{
					this.VM.speechUtils.SpeechVolume = (float)Convert.ToInt32(newValue);
					if (this.VolumeBlock != null)
					{
						this.VolumeBlock.Text = string.Format("{0}", this.VM.speechUtils.SpeechVolume);
					}
				}
				else
				{
					this.VolumeBlock.Text = newValue.ToString();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x000749B4 File Offset: 0x00072BB4
		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			this.VM.speechUtils.Stop();
			SpeechUtils speechUtils = this.VM.speechUtils;
			if (speechUtils != null)
			{
				speechUtils.Dispose();
			}
			this.VM.speechUtils = null;
			ContextMenuModel contextMenuModel = (this.VM.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
			(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = true;
			(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = true;
			(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = true;
			(contextMenuModel[0] as SpeedContextMenuItemModel).Icon = null;
			(contextMenuModel[1] as SpeedContextMenuItemModel).Icon = null;
			(contextMenuModel[2] as SpeedContextMenuItemModel).Icon = null;
			this.VM.ViewToolbar.ReadButtonModel.IsChecked = false;
			this.VM.IsReading = false;
			this.StopBtn.IsEnabled = false;
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x00074AB4 File Offset: 0x00072CB4
		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			ContextMenuModel contextMenuModel = (this.VM.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
			(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = false;
			(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = false;
			(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = false;
			this.VM.ViewToolbar.ReadButtonModel.IsChecked = true;
			this.StopBtn.IsEnabled = true;
			this.GlobeBtn.IsEnabled = true;
			if (this.VM.speechUtils != null)
			{
				if (this.VM.speechUtils.ProcessorStream != null)
				{
					this.VM.speechUtils.PauseSpeak();
					return;
				}
				if ((contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked)
				{
					(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = true;
					this.VM.speechUtils.SpeakCurrentPage(this.VM.CurrnetPageIndex - 1);
					return;
				}
				if ((contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked)
				{
					(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = true;
					this.VM.speechUtils.SpeakPages(0, this.VM.Document.Pages.Count - 1);
					return;
				}
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = true;
				this.VM.speechUtils.SpeakPages(this.VM.CurrnetPageIndex - 1, this.VM.Document.Pages.Count - 1);
				return;
			}
			else
			{
				this.VM.ViewToolbar.ReadButtonModel.IsChecked = true;
				PdfDocument pdfDocument;
				if (this.VM.Document != null)
				{
					pdfDocument = this.VM.Document;
				}
				else
				{
					pdfDocument = null;
				}
				this.VM.IsReading = true;
				SpeechUtils speechUtils = this.VM.speechUtils;
				if (speechUtils != null)
				{
					speechUtils.Dispose();
				}
				this.VM.speechUtils = new SpeechUtils(pdfDocument);
				this.VM.speechUtils.Rate = this.SpeedSli.Value * 2.0 - 10.0;
				this.VM.speechUtils.SpeechVolume = (float)Convert.ToInt32(this.VolumeSlider.Value);
				this.VM.speechUtils.Pitch = (double)Convert.ToInt32(this.ToneSli.Value - 5.0);
				if (this.VM.speechControl.CultureListBox.SelectedIndex < 0)
				{
					this.VM.speechUtils.CultureIndex = this.VM.speechUtils.GetcultureIndex();
				}
				else
				{
					this.VM.speechUtils.CultureIndex = this.VM.speechControl.CultureListBox.SelectedIndex;
				}
				if ((contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked)
				{
					(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = true;
					this.VM.speechUtils.SpeakCurrentPage(this.VM.CurrnetPageIndex - 1);
					return;
				}
				if ((contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked)
				{
					(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = true;
					this.VM.speechUtils.SpeakPages(0, this.VM.Document.Pages.Count - 1);
					return;
				}
				(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = true;
				this.VM.speechUtils.SpeakPages(this.VM.CurrnetPageIndex - 1, this.VM.Document.Pages.Count - 1);
				return;
			}
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00074E6C File Offset: 0x0007306C
		private void CluButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("explorer.exe", "ms-settings:speech");
		}

		// Token: 0x04000A25 RID: 2597
		public ImgaeFilePath imgaeFilePath;
	}
}
