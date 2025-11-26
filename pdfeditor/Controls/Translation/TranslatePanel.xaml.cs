using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models.Translate;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Translation
{
	// Token: 0x020001E4 RID: 484
	public partial class TranslatePanel : UserControl
	{
		// Token: 0x06001B5F RID: 7007 RVA: 0x0006F7B9 File Offset: 0x0006D9B9
		public TranslatePanel()
		{
			this.InitializeComponent();
			this.showToastAnimation = this.LayoutRoot.Resources["ShowToastAnimation"] as Storyboard;
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x0006F7E7 File Offset: 0x0006D9E7
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("Translate", "Close", "Count", 1L);
			EventHandler closeButtonClick = this.CloseButtonClick;
			if (closeButtonClick == null)
			{
				return;
			}
			closeButtonClick(this, e);
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x06001B61 RID: 7009 RVA: 0x0006F814 File Offset: 0x0006DA14
		// (remove) Token: 0x06001B62 RID: 7010 RVA: 0x0006F84C File Offset: 0x0006DA4C
		public event EventHandler CloseButtonClick;

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x06001B63 RID: 7011 RVA: 0x0006F881 File Offset: 0x0006DA81
		// (set) Token: 0x06001B64 RID: 7012 RVA: 0x0006F893 File Offset: 0x0006DA93
		public string Translations
		{
			get
			{
				return (string)base.GetValue(TranslatePanel.TranslationsProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslationsProperty, value);
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x06001B65 RID: 7013 RVA: 0x0006F8A1 File Offset: 0x0006DAA1
		// (set) Token: 0x06001B66 RID: 7014 RVA: 0x0006F8B3 File Offset: 0x0006DAB3
		public bool Translating
		{
			get
			{
				return (bool)base.GetValue(TranslatePanel.TranslatingProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslatingProperty, value);
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06001B67 RID: 7015 RVA: 0x0006F8C6 File Offset: 0x0006DAC6
		// (set) Token: 0x06001B68 RID: 7016 RVA: 0x0006F8D8 File Offset: 0x0006DAD8
		public bool TranslateWords
		{
			get
			{
				return (bool)base.GetValue(TranslatePanel.TranslateWordsProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslateWordsProperty, value);
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06001B69 RID: 7017 RVA: 0x0006F8EB File Offset: 0x0006DAEB
		// (set) Token: 0x06001B6A RID: 7018 RVA: 0x0006F8FD File Offset: 0x0006DAFD
		public bool CanExchange
		{
			get
			{
				return (bool)base.GetValue(TranslatePanel.CanExchangeProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.CanExchangeProperty, value);
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06001B6B RID: 7019 RVA: 0x0006F910 File Offset: 0x0006DB10
		// (set) Token: 0x06001B6C RID: 7020 RVA: 0x0006F922 File Offset: 0x0006DB22
		public string Textual
		{
			get
			{
				return (string)base.GetValue(TranslatePanel.TextualProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TextualProperty, value);
			}
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06001B6D RID: 7021 RVA: 0x0006F930 File Offset: 0x0006DB30
		// (set) Token: 0x06001B6E RID: 7022 RVA: 0x0006F942 File Offset: 0x0006DB42
		public List<TranslateLanguage> TranslaeSourceLanguage
		{
			get
			{
				return (List<TranslateLanguage>)base.GetValue(TranslatePanel.TranslaeSourceLanguageProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslaeSourceLanguageProperty, value);
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06001B6F RID: 7023 RVA: 0x0006F950 File Offset: 0x0006DB50
		// (set) Token: 0x06001B70 RID: 7024 RVA: 0x0006F962 File Offset: 0x0006DB62
		public List<TranslateLanguage> TranslaeLanguage
		{
			get
			{
				return (List<TranslateLanguage>)base.GetValue(TranslatePanel.TranslaeLanguageProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslaeLanguageProperty, value);
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06001B71 RID: 7025 RVA: 0x0006F970 File Offset: 0x0006DB70
		// (set) Token: 0x06001B72 RID: 7026 RVA: 0x0006F982 File Offset: 0x0006DB82
		public List<TranslateLanguage> TranslateTargetLanguageList
		{
			get
			{
				return (List<TranslateLanguage>)base.GetValue(TranslatePanel.TranslateTargetLanguageListProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.TranslateTargetLanguageListProperty, value);
			}
		}

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06001B73 RID: 7027 RVA: 0x0006F990 File Offset: 0x0006DB90
		// (set) Token: 0x06001B74 RID: 7028 RVA: 0x0006F9A2 File Offset: 0x0006DBA2
		public TranslateLanguage SelectedSourceLanguage
		{
			get
			{
				return (TranslateLanguage)base.GetValue(TranslatePanel.SelectedSourceLanguageProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.SelectedSourceLanguageProperty, value);
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06001B75 RID: 7029 RVA: 0x0006F9B0 File Offset: 0x0006DBB0
		// (set) Token: 0x06001B76 RID: 7030 RVA: 0x0006F9C2 File Offset: 0x0006DBC2
		public TranslateLanguage SelectedTargetLanguage
		{
			get
			{
				return (TranslateLanguage)base.GetValue(TranslatePanel.SelectedTargetLanguageProperty);
			}
			set
			{
				base.SetValue(TranslatePanel.SelectedTargetLanguageProperty, value);
			}
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x0006F9D0 File Offset: 0x0006DBD0
		private void UpdateRecentStates()
		{
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x0006F9D4 File Offset: 0x0006DBD4
		private void SourceLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			requiredService.TranslateViewModel.Exchanged();
			if (this.SelectedSourceLanguage.Name != this.SelectedTargetLanguage.Name)
			{
				requiredService.TranslateViewModel.Translate();
			}
			this.TranslateButton.Focus();
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x0006FA2B File Offset: 0x0006DC2B
		private void TargetLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			requiredService.TranslateViewModel.Exchanged();
			requiredService.TranslateViewModel.Translate();
			this.TranslateButton.Focus();
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x0006FA58 File Offset: 0x0006DC58
		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			this.SourceTextBox.Text = "";
			Ioc.Default.GetRequiredService<MainViewModel>().TranslateViewModel.Textual = "";
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x0006FA84 File Offset: 0x0006DC84
		private void TranslateButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.SourceTextBox.Text))
			{
				return;
			}
			string text = this.SourceTextBox.Text;
			GAManager.SendEvent("Translate", "TranslateCmd", "Count", 1L);
			Ioc.Default.GetRequiredService<MainViewModel>().TranslateViewModel.translateAsync(text, false);
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x0006FAE0 File Offset: 0x0006DCE0
		private async void CopyBtn_ClickAsync(object sender, RoutedEventArgs e)
		{
			string text = this.Translation.Text;
			if (text != null)
			{
				((Button)sender).IsEnabled = false;
				try
				{
					Clipboard.SetDataObject(text);
					this.showToastAnimation.SkipToFill();
					this.showToastAnimation.Begin();
					await Task.Delay(200);
				}
				catch
				{
					this.showToastAnimation.SkipToFill();
				}
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x0006FB20 File Offset: 0x0006DD20
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (requiredService != null)
			{
				TranslateLanguage selectedSourceLanguage = requiredService.TranslateViewModel.SelectedSourceLanguage;
				if (requiredService.TranslateViewModel.SelectedTargetLanguage.Name != "AutoLanguage")
				{
					requiredService.TranslateViewModel.SelectedSourceLanguage = requiredService.TranslateViewModel.SelectedTargetLanguage;
				}
				else
				{
					string autoLanguage = this.GetAutoLanguage();
					foreach (TranslateLanguage translateLanguage in requiredService.TranslateViewModel.TranslateLanguageList)
					{
						if (translateLanguage.Name == autoLanguage)
						{
							requiredService.TranslateViewModel.SelectedSourceLanguage = translateLanguage;
						}
					}
				}
				requiredService.TranslateViewModel.SelectedTargetLanguage = selectedSourceLanguage;
			}
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x0006FBF4 File Offset: 0x0006DDF4
		private string GetAutoLanguage()
		{
			string actualAppLanguage = CultureInfoUtils.ActualAppLanguage;
			IReadOnlyList<string> allSupportLanguage = CultureInfoUtils.AllSupportLanguage;
			if (actualAppLanguage != null)
			{
				int length = actualAppLanguage.Length;
				if (length != 2)
				{
					if (length == 5)
					{
						if (actualAppLanguage == "zh-CN")
						{
							return "Chinese";
						}
					}
				}
				else
				{
					switch (actualAppLanguage[0])
					{
					case 'd':
						if (actualAppLanguage == "de")
						{
							return "Deutsch";
						}
						break;
					case 'e':
						if (actualAppLanguage == "en")
						{
							return "English";
						}
						if (actualAppLanguage == "es")
						{
							return "Spanish";
						}
						break;
					case 'f':
						if (actualAppLanguage == "fr")
						{
							return "French";
						}
						break;
					case 'i':
						if (actualAppLanguage == "it")
						{
							return "Italian";
						}
						break;
					case 'j':
						if (actualAppLanguage == "jp")
						{
							return "Japanese";
						}
						break;
					case 'k':
						if (actualAppLanguage == "ko")
						{
							return "Korean";
						}
						break;
					case 'n':
						if (actualAppLanguage == "nl")
						{
							return "Dutch";
						}
						break;
					case 'p':
						if (actualAppLanguage == "pt")
						{
							return "Portuguese";
						}
						break;
					case 'r':
						if (actualAppLanguage == "ru")
						{
							return "Russian";
						}
						break;
					}
				}
			}
			return "English";
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x0006FD7E File Offset: 0x0006DF7E
		private void UpgradeButton_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x0006FD80 File Offset: 0x0006DF80
		private void SourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				Color color = ((App.Current.GetCurrentActualAppTheme() == "Dark") ? Colors.White : Colors.Black);
				if (!string.IsNullOrEmpty(textBox.Text))
				{
					int length = textBox.Text.Length;
					if (length > 5000)
					{
						this.SourceCount.Foreground = new SolidColorBrush(Colors.Red);
					}
					else
					{
						this.SourceCount.Foreground = new SolidColorBrush(color);
					}
					this.SourceCount.Text = length.ToString();
					return;
				}
				this.SourceCount.Foreground = new SolidColorBrush(color);
				this.SourceCount.Text = "0";
			}
		}

		// Token: 0x040009BE RID: 2494
		private Storyboard showToastAnimation;

		// Token: 0x040009C0 RID: 2496
		public static readonly DependencyProperty TranslationsProperty = DependencyProperty.Register("Translations", typeof(string), typeof(TranslatePanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C1 RID: 2497
		public static readonly DependencyProperty TranslatingProperty = DependencyProperty.Register("Translating", typeof(bool), typeof(TranslatePanel), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C2 RID: 2498
		public static readonly DependencyProperty TranslateWordsProperty = DependencyProperty.Register("TranslateWords", typeof(bool), typeof(TranslatePanel), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C3 RID: 2499
		public static readonly DependencyProperty CanExchangeProperty = DependencyProperty.Register("CanExchange", typeof(bool), typeof(TranslatePanel), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C4 RID: 2500
		public static readonly DependencyProperty TextualProperty = DependencyProperty.Register("Textual", typeof(string), typeof(TranslatePanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C5 RID: 2501
		public static readonly DependencyProperty TranslaeSourceLanguageProperty = DependencyProperty.Register("TranslaeSourceLanguage", typeof(List<TranslateLanguage>), typeof(TranslatePanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C6 RID: 2502
		public static readonly DependencyProperty TranslaeLanguageProperty = DependencyProperty.Register("TranslaeLanguage", typeof(List<TranslateLanguage>), typeof(TranslatePanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C7 RID: 2503
		public static readonly DependencyProperty TranslateTargetLanguageListProperty = DependencyProperty.Register("TranslateTargetLanguageList", typeof(List<TranslateLanguage>), typeof(TranslatePanel), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C8 RID: 2504
		public static readonly DependencyProperty SelectedSourceLanguageProperty = DependencyProperty.Register("SelectedSourceLanguage", typeof(TranslateLanguage), typeof(TranslatePanel), new PropertyMetadata(TranslateLanguage.CreateAutoLanguage(false), delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));

		// Token: 0x040009C9 RID: 2505
		public static readonly DependencyProperty SelectedTargetLanguageProperty = DependencyProperty.Register("SelectedTargetLanguage", typeof(TranslateLanguage), typeof(TranslatePanel), new PropertyMetadata(TranslateLanguage.CreateAutoLanguage(true), delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			TranslatePanel sender = s as TranslatePanel;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					sender.UpdateRecentStates();
				}));
			}
		}));
	}
}
