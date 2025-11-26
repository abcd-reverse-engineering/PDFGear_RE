using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Translate;
using pdfeditor.Properties;
using pdfeditor.Utils.Copilot;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200006D RID: 109
	public class TranslateViewModel : ObservableObject
	{
		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060007F0 RID: 2032 RVA: 0x00026A92 File Offset: 0x00024C92
		// (set) Token: 0x060007F1 RID: 2033 RVA: 0x00026A9A File Offset: 0x00024C9A
		public List<TranslateLanguage> TranslateLanguageList { get; set; } = new List<TranslateLanguage>();

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060007F2 RID: 2034 RVA: 0x00026AA3 File Offset: 0x00024CA3
		// (set) Token: 0x060007F3 RID: 2035 RVA: 0x00026AAB File Offset: 0x00024CAB
		public List<TranslateLanguage> TranslateTargetLanguageList { get; set; } = new List<TranslateLanguage>();

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060007F4 RID: 2036 RVA: 0x00026AB4 File Offset: 0x00024CB4
		// (set) Token: 0x060007F5 RID: 2037 RVA: 0x00026ABC File Offset: 0x00024CBC
		public TranslateLanguage SelectedSourceLanguage
		{
			get
			{
				return this.selectedSourceLanguage;
			}
			set
			{
				base.SetProperty<TranslateLanguage>(ref this.selectedSourceLanguage, value, "SelectedSourceLanguage");
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060007F6 RID: 2038 RVA: 0x00026AD1 File Offset: 0x00024CD1
		// (set) Token: 0x060007F7 RID: 2039 RVA: 0x00026AD9 File Offset: 0x00024CD9
		public TranslateLanguage SelectedTargetLanguage
		{
			get
			{
				return this.selectedTargetLanguage;
			}
			set
			{
				base.SetProperty<TranslateLanguage>(ref this.selectedTargetLanguage, value, "SelectedTargetLanguage");
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x00026AF0 File Offset: 0x00024CF0
		public RelayCommand CopyCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.copyCmd) == null)
				{
					relayCommand = (this.copyCmd = new RelayCommand(async delegate
					{
						if (this.Translations != null)
						{
							try
							{
								Clipboard.SetDataObject(this.Translations);
							}
							catch
							{
							}
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x00026B24 File Offset: 0x00024D24
		public AsyncRelayCommand ExchangeCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.exchangeCmd) == null)
				{
					asyncRelayCommand = (this.exchangeCmd = new AsyncRelayCommand(new Func<Task>(this.Exchange)));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00026B58 File Offset: 0x00024D58
		private async Task Exchange()
		{
			TranslateLanguage translateLanguage = this.SelectedSourceLanguage;
			this.SelectedSourceLanguage = this.SelectedTargetLanguage;
			this.SelectedTargetLanguage = translateLanguage;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00026B9C File Offset: 0x00024D9C
		public void Exchanged()
		{
			if (this.selectedSourceLanguage.Name == "Auto" || this.selectedSourceLanguage.Name == this.selectedTargetLanguage.Name)
			{
				this.CanExchange = false;
				return;
			}
			this.CanExchange = true;
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00026BEC File Offset: 0x00024DEC
		public void Translate()
		{
			this.translateAsync(this.Textual, false);
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x00026BFC File Offset: 0x00024DFC
		public RelayCommand OpenTranslatePanelCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openTranslatePanelCommand) == null)
				{
					relayCommand = (this.openTranslatePanelCommand = new RelayCommand(async delegate
					{
						this.mainViewModel.TranslatePanelVisible = true;
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060007FE RID: 2046 RVA: 0x00026C2D File Offset: 0x00024E2D
		// (set) Token: 0x060007FF RID: 2047 RVA: 0x00026C35 File Offset: 0x00024E35
		public string Textual
		{
			get
			{
				return this._textual;
			}
			set
			{
				base.SetProperty<string>(ref this._textual, value, "Textual");
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x00026C4A File Offset: 0x00024E4A
		// (set) Token: 0x06000801 RID: 2049 RVA: 0x00026C52 File Offset: 0x00024E52
		public string Translations
		{
			get
			{
				return this._translations;
			}
			set
			{
				base.SetProperty<string>(ref this._translations, value, "Translations");
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000802 RID: 2050 RVA: 0x00026C67 File Offset: 0x00024E67
		// (set) Token: 0x06000803 RID: 2051 RVA: 0x00026C6F File Offset: 0x00024E6F
		public bool Translating
		{
			get
			{
				return this.translating;
			}
			set
			{
				base.SetProperty<bool>(ref this.translating, value, "Translating");
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000804 RID: 2052 RVA: 0x00026C84 File Offset: 0x00024E84
		// (set) Token: 0x06000805 RID: 2053 RVA: 0x00026C8C File Offset: 0x00024E8C
		public bool CanExchange
		{
			get
			{
				return this.canExchange;
			}
			set
			{
				base.SetProperty<bool>(ref this.canExchange, value, "CanExchange");
			}
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00026CA4 File Offset: 0x00024EA4
		public TranslateViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
			this.Translations = "";
			this.Textual = "";
			this._sourceText = "";
			this.CanExchange = false;
			this.TranslateLanguageList.Add(TranslateLanguage.CreateAutoLanguage(false));
			this.TranslateTargetLanguageList.Add(TranslateLanguage.CreateAutoLanguage(true));
			this.TranslateLanguageList.AddRange(TranslateLanguage.AllLanguageModel.ToArray());
			this.TranslateTargetLanguageList.AddRange(TranslateLanguage.AllLanguageModel.ToArray());
			this.SelectedSourceLanguage = this.TranslateLanguageList[0];
			this.SelectedTargetLanguage = this.TranslateTargetLanguageList[0];
			this.lastSourceLanguage = this.SelectedSourceLanguage.Name;
			this.lastTargetLanguage = this.SelectedTargetLanguage.Name;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00026D90 File Offset: 0x00024F90
		public async Task translateAsync(string text, ContextMenuTranslateModel model)
		{
			if (model != null)
			{
				if (this._sourceText == text && this.lastSourceLanguage == this.SelectedSourceLanguage.Name && this.lastTargetLanguage == this.SelectedTargetLanguage.Name)
				{
					model.TranslatedText = this.Translations;
				}
				else
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
					this.Translating = true;
					model.Translating = true;
					text = text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
					this._sourceText = text;
					string text2 = text;
					string text3 = this.SelectedTargetLanguage.Name;
					string name = this.SelectedSourceLanguage.Name;
					this.lastSourceLanguage = this.SelectedSourceLanguage.Name;
					this.lastTargetLanguage = this.SelectedTargetLanguage.Name;
					if (this.SelectedTargetLanguage.Name == "AutoLanguage")
					{
						text3 = CultureInfoUtils.ActualAppLanguage;
					}
					this.Textual = text;
					CopilotHelper.CopilotResult copilotResult = await this.mainViewModel.CopilotHelper.TranslateAsync(text2, text3, new Func<string, CancellationToken, Task>(this.DisplayTranslationResultAsync), this.cts.Token, name);
					if (copilotResult != null)
					{
						model.TranslatedText = copilotResult.Text;
						this.Translations = copilotResult.Text;
					}
					this.Translating = false;
					model.Translating = false;
				}
			}
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00026DE4 File Offset: 0x00024FE4
		private async Task DisplayTranslationResultAsync(string text, CancellationToken cancellationToken)
		{
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x00026E1F File Offset: 0x0002501F
		private string TruncateString(string s, int maxLength)
		{
			if (s.Length > maxLength)
			{
				return s.Substring(0, maxLength);
			}
			return s;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x00026E34 File Offset: 0x00025034
		public async Task translateAsync(string text, bool FormSeleted = false)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (this._sourceText != text && !ConfigManager.GetTryoutFlag("TranslateChangeFlag"))
				{
					ConfigManager.SetTryoutFlag("TranslateChangeFlag", true);
				}
				if (!(this._sourceText == text) || !(this.lastSourceLanguage == this.SelectedSourceLanguage.Name) || !(this.lastTargetLanguage == this.SelectedTargetLanguage.Name))
				{
					if (text.Length > 5000)
					{
						text = this.TruncateString(text, 5000);
					}
					GAManager.SendEvent("Translate", "Translate", FormSeleted ? "SelectedText" : "TranslateCmd", 1L);
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
					this.Translating = true;
					if (FormSeleted)
					{
						text = text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
					}
					this._sourceText = text;
					string text2 = text;
					string text3 = this.SelectedTargetLanguage.Name;
					string name = this.SelectedSourceLanguage.Name;
					this.Textual = text;
					this.lastSourceLanguage = this.SelectedSourceLanguage.Name;
					this.lastTargetLanguage = this.SelectedTargetLanguage.Name;
					if (this.SelectedTargetLanguage.Name == "AutoLanguage")
					{
						text3 = CultureInfoUtils.ActualAppLanguage;
					}
					CopilotHelper.CopilotResult copilotResult = await this.mainViewModel.CopilotHelper.TranslateAsync(text2, text3, new Func<string, CancellationToken, Task>(this.DisplayTranslationResultAsync), this.cts.Token, name);
					if (copilotResult != null)
					{
						this.Translations = copilotResult.Text;
					}
					this.Translating = false;
					if (string.IsNullOrEmpty(this.Translations))
					{
						this.Translations = Resources.ImprotPagesUnexpectedError;
					}
				}
			}
		}

		// Token: 0x0400040E RID: 1038
		private string _translations;

		// Token: 0x0400040F RID: 1039
		private readonly MainViewModel mainViewModel;

		// Token: 0x04000410 RID: 1040
		private TranslateLanguage selectedSourceLanguage;

		// Token: 0x04000411 RID: 1041
		private TranslateLanguage selectedTargetLanguage;

		// Token: 0x04000412 RID: 1042
		private string _textual;

		// Token: 0x04000413 RID: 1043
		private string _sourceText;

		// Token: 0x04000414 RID: 1044
		private string lastSourceLanguage;

		// Token: 0x04000415 RID: 1045
		private string lastTargetLanguage;

		// Token: 0x04000416 RID: 1046
		private bool translating;

		// Token: 0x04000417 RID: 1047
		private bool canExchange;

		// Token: 0x04000418 RID: 1048
		private CancellationTokenSource cts;

		// Token: 0x0400041B RID: 1051
		private RelayCommand copyCmd;

		// Token: 0x0400041C RID: 1052
		private AsyncRelayCommand exchangeCmd;

		// Token: 0x0400041D RID: 1053
		private RelayCommand openTranslatePanelCommand;
	}
}
