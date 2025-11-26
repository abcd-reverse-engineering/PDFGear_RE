using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonLib.Account;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using CommonLib.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.AutoSaveRestore;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Controls.Printer;
using pdfeditor.Controls.Speech;
using pdfeditor.Models;
using pdfeditor.Models.Attachments;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Models.Commets;
using pdfeditor.Models.LeftNavigations;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Models.Operations;
using pdfeditor.Models.Protection;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Copilot;
using pdfeditor.Utils.Enums;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Contents.Controls;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000064 RID: 100
	public class MainViewModel : ObservableObject
	{
		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x0001D833 File Offset: 0x0001BA33
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x0001D83B File Offset: 0x0001BA3B
		public bool MaybeHaveUnembeddedSignature { get; set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x0001D844 File Offset: 0x0001BA44
		public QuickToolModel QuickToolOpenModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolOpenModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.OpenDocCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolOpenModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x0001D87C File Offset: 0x0001BA7C
		public QuickToolModel QuickToolSaveModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolSaveModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.SaveCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolSaveModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001D8B4 File Offset: 0x0001BAB4
		public QuickToolModel QuickToolSaveAsModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolSaveAsModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.SaveAsCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolSaveAsModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x0001D8EC File Offset: 0x0001BAEC
		public QuickToolModel QuickToolPrintModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolPrintModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.PrintDocCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolPrintModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001D924 File Offset: 0x0001BB24
		public QuickToolModel QuickToolUndoModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolUndoModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.UndoCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolUndoModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x0001D95C File Offset: 0x0001BB5C
		public QuickToolModel QuickToolRedoModel
		{
			get
			{
				QuickToolModel quickToolModel;
				if ((quickToolModel = this.quickToolRedoModel) == null)
				{
					QuickToolModel quickToolModel2 = new QuickToolModel();
					quickToolModel2.IsVisible = true;
					quickToolModel2.Command = this.RedoCmd;
					QuickToolModel quickToolModel3 = quickToolModel2;
					this.quickToolRedoModel = quickToolModel2;
					quickToolModel = quickToolModel3;
				}
				return quickToolModel;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0001D994 File Offset: 0x0001BB94
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x0001D99C File Offset: 0x0001BB9C
		internal DataOperationModel ViewerOperationModel
		{
			get
			{
				return this.viewerOperationModel;
			}
			set
			{
				DataOperationModel dataOperationModel = this.viewerOperationModel;
				if (base.SetProperty<DataOperationModel>(ref this.viewerOperationModel, value, "ViewerOperationModel") && dataOperationModel != null)
				{
					dataOperationModel.Dispose();
				}
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0001D9CD File Offset: 0x0001BBCD
		public PdfViewJumpManager ViewJumpManager
		{
			get
			{
				return this.viewJumpManager;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000619 RID: 1561 RVA: 0x0001D9D8 File Offset: 0x0001BBD8
		public ConverterCommands ConverterCommands
		{
			get
			{
				ConverterCommands converterCommands;
				if ((converterCommands = this.converterCmds) == null)
				{
					converterCommands = (this.converterCmds = new ConverterCommands(this));
				}
				return converterCommands;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x0001D9FE File Offset: 0x0001BBFE
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x0001DA06 File Offset: 0x0001BC06
		public ViewToolbarViewModel ViewToolbar
		{
			get
			{
				return this.viewToolbarViewModel;
			}
			private set
			{
				base.SetProperty<ViewToolbarViewModel>(ref this.viewToolbarViewModel, value, "ViewToolbar");
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x0001DA1B File Offset: 0x0001BC1B
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x0001DA23 File Offset: 0x0001BC23
		public TranslateViewModel TranslateViewModel
		{
			get
			{
				return this.translateViewModel;
			}
			set
			{
				base.SetProperty<TranslateViewModel>(ref this.translateViewModel, value, "TranslateViewModel");
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x0001DA38 File Offset: 0x0001BC38
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x0001DA40 File Offset: 0x0001BC40
		public AnnotationToolbarViewModel AnnotationToolbar
		{
			get
			{
				return this.annotationToolbarViewModel;
			}
			private set
			{
				base.SetProperty<AnnotationToolbarViewModel>(ref this.annotationToolbarViewModel, value, "AnnotationToolbar");
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001DA58 File Offset: 0x0001BC58
		public MenuViewModel Menus
		{
			get
			{
				MenuViewModel menuViewModel;
				if ((menuViewModel = this.menus) == null)
				{
					menuViewModel = (this.menus = new MenuViewModel(this));
				}
				return menuViewModel;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x0001DA7E File Offset: 0x0001BC7E
		// (set) Token: 0x06000622 RID: 1570 RVA: 0x0001DA86 File Offset: 0x0001BC86
		public PageEditorViewModel PageEditors
		{
			get
			{
				return this.pageEditorViewModel;
			}
			private set
			{
				base.SetProperty<PageEditorViewModel>(ref this.pageEditorViewModel, value, "PageEditors");
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x0001DA9B File Offset: 0x0001BC9B
		// (set) Token: 0x06000624 RID: 1572 RVA: 0x0001DAA3 File Offset: 0x0001BCA3
		public ShareTabViewModel ShareTab
		{
			get
			{
				return this.shareTabViewModel;
			}
			private set
			{
				base.SetProperty<ShareTabViewModel>(ref this.shareTabViewModel, value, "ShareTab");
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x0001DAB8 File Offset: 0x0001BCB8
		// (set) Token: 0x06000626 RID: 1574 RVA: 0x0001DAC0 File Offset: 0x0001BCC0
		public bool TranslateWords
		{
			get
			{
				return this.translateWords;
			}
			set
			{
				GAManager.SendEvent("Translate", "AutoTranslateSelText", value ? "Enable" : "Disable", 1L);
				base.SetProperty<bool>(ref this.translateWords, value, "TranslateWords");
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x0001DAF5 File Offset: 0x0001BCF5
		// (set) Token: 0x06000628 RID: 1576 RVA: 0x0001DAFD File Offset: 0x0001BCFD
		public bool FileBtnIsChecked
		{
			get
			{
				return this.fileBtnIsChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.fileBtnIsChecked, value, "FileBtnIsChecked");
				if (value)
				{
					GAManager.SendEvent("FileMenu", "Show", "Count", 1L);
				}
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x0001DB2C File Offset: 0x0001BD2C
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x0001DBA8 File Offset: 0x0001BDA8
		public AnnotationMode AnnotationMode
		{
			get
			{
				AnnotationToolbarViewModel annotationToolbar = this.AnnotationToolbar;
				AnnotationMode? annotationMode;
				if (annotationToolbar == null)
				{
					annotationMode = null;
				}
				else
				{
					global::System.Collections.Generic.IReadOnlyList<ToolbarAnnotationButtonModel> allAnnotationButton = annotationToolbar.AllAnnotationButton;
					if (allAnnotationButton == null)
					{
						annotationMode = null;
					}
					else
					{
						ToolbarAnnotationButtonModel toolbarAnnotationButtonModel = allAnnotationButton.FirstOrDefault((ToolbarAnnotationButtonModel c) => c.IsChecked);
						annotationMode = ((toolbarAnnotationButtonModel != null) ? new AnnotationMode?(toolbarAnnotationButtonModel.Mode) : null);
					}
				}
				AnnotationMode? annotationMode2 = annotationMode;
				return annotationMode2.GetValueOrDefault(AnnotationMode.None);
			}
			set
			{
				if (this.AnnotationToolbar == null)
				{
					return;
				}
				AnnotationMode annotationMode = this.AnnotationMode;
				if (value == AnnotationMode.None || value == AnnotationMode.Stamp || value == AnnotationMode.Signature)
				{
					foreach (ToolbarAnnotationButtonModel toolbarAnnotationButtonModel in this.AnnotationToolbar.AllAnnotationButton)
					{
						ToolbarChildCheckableButtonModel toolbarChildCheckableButtonModel = toolbarAnnotationButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel;
						if (toolbarChildCheckableButtonModel != null && toolbarChildCheckableButtonModel.IsChecked)
						{
							toolbarChildCheckableButtonModel.IsChecked = false;
						}
						if (toolbarAnnotationButtonModel.IsChecked)
						{
							toolbarAnnotationButtonModel.IsChecked = false;
						}
					}
				}
				if (value != AnnotationMode.None)
				{
					this.ExitTransientMode(false, false, false, false, false);
					ToolbarAnnotationButtonModel toolbarAnnotationButtonModel2 = this.AnnotationToolbar.AllAnnotationButton.FirstOrDefault((ToolbarAnnotationButtonModel c) => c.Mode == value);
					if (toolbarAnnotationButtonModel2 != null && toolbarAnnotationButtonModel2.IsCheckable)
					{
						if (!toolbarAnnotationButtonModel2.IsChecked)
						{
							toolbarAnnotationButtonModel2.IsChecked = true;
						}
					}
					else
					{
						this.AnnotationMode = AnnotationMode.None;
					}
					if (MainViewModel.<>o__84.<>p__2 == null)
					{
						MainViewModel.<>o__84.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(MainViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, bool> target = MainViewModel.<>o__84.<>p__2.Target;
					CallSite <>p__ = MainViewModel.<>o__84.<>p__2;
					if (MainViewModel.<>o__84.<>p__1 == null)
					{
						MainViewModel.<>o__84.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(MainViewModel), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					Func<CallSite, object, MouseModes, object> target2 = MainViewModel.<>o__84.<>p__1.Target;
					CallSite <>p__2 = MainViewModel.<>o__84.<>p__1;
					if (MainViewModel.<>o__84.<>p__0 == null)
					{
						MainViewModel.<>o__84.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					if (target(<>p__, target2(<>p__2, MainViewModel.<>o__84.<>p__0.Target(MainViewModel.<>o__84.<>p__0, this.ViewerMouseMode), MouseModes.Default)))
					{
						if (MainViewModel.<>o__84.<>p__3 == null)
						{
							MainViewModel.<>o__84.<>p__3 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						MainViewModel.<>o__84.<>p__3.Target(MainViewModel.<>o__84.<>p__3, this.ViewerMouseMode, MouseModes.Default);
					}
				}
				base.OnPropertyChanged("AnnotationMode");
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x0001DE08 File Offset: 0x0001C008
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x0001DE10 File Offset: 0x0001C010
		public PageObjectType EditingPageObjectType
		{
			get
			{
				return this.editingPageObjectType;
			}
			set
			{
				if (base.SetProperty<PageObjectType>(ref this.editingPageObjectType, value, "EditingPageObjectType"))
				{
					this.viewToolbarViewModel.EditPageTextObjectButtonModel.IsChecked = value == PageObjectType.Text;
				}
			}
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0001DE3A File Offset: 0x0001C03A
		public void RaiseAnnotationModePropertyChanged()
		{
			base.OnPropertyChanged("AnnotationMode");
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001DE47 File Offset: 0x0001C047
		[Dynamic]
		public dynamic ViewerMouseMode
		{
			[return: Dynamic]
			get
			{
				return this.viewerMouseMode;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x0001DE4F File Offset: 0x0001C04F
		[Dynamic]
		public dynamic EditorMouseMode
		{
			[return: Dynamic]
			get
			{
				return this.editorMouseMode;
			}
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0001DE58 File Offset: 0x0001C058
		public MainViewModel()
		{
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0001E040 File Offset: 0x0001C240
		public MainViewModel(string startUpFilePath)
		{
			this.StartUpFilePath = startUpFilePath;
			this.IsDocumentOpened = !string.IsNullOrEmpty(this.StartUpFilePath);
			this.documentWrapper = new DocumentWrapper();
			this.documentWrapper.PasswordRequested += this.DocumentWrapper_PasswordRequested;
			this.documentWrapper.FileError += this.DocumentWrapper_FileError;
			this.TranslateViewModel = new TranslateViewModel(this);
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
			{
				this.ViewToolbar = new ViewToolbarViewModel(this);
				this.AnnotationToolbar = new AnnotationToolbarViewModel(this);
				this.PageEditors = new PageEditorViewModel(this);
				this.ShareTab = new ShareTabViewModel(this);
				this.InitDefaultSettings();
				this.InitAutoSave();
				this.AnnotationToolbar.LoadToolbarSettingsConfigAsync();
				this.Menus.ToolbarInited = true;
				if (MainViewModel.<>o__96.<>p__0 == null)
				{
					MainViewModel.<>o__96.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				MainViewModel.<>o__96.<>p__0.Target(MainViewModel.<>o__96.<>p__0, this.ViewerMouseMode);
			}));
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0001E2A0 File Offset: 0x0001C4A0
		private void InitAutoSave()
		{
			ConfigManager.GetAutoSaveAsync(default(CancellationToken)).GetAwaiter().GetResult();
			this.AutoSaveModel = new pdfeditor.AutoSaveRestore.AutoSaveModel
			{
				IsAuto = true,
				SpanMinutes = 1
			};
			pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().SaveStarted += this.AutoSaveStarted;
			pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().SaveCompleted += this.AutoSaveCompleted;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0001E30E File Offset: 0x0001C50E
		private void AutoSaveStarted(object sender, EventArgs e)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				MainView mainView = (MainView)App.Current.MainWindow;
				mainView.progressBar.Visibility = Visibility.Visible;
				mainView.progressBar.Value = 0.0;
			}));
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0001E33C File Offset: 0x0001C53C
		private void AutoSaveCompleted(object sender, EventArgs e)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(async delegate
			{
				MainView mainView = (MainView)App.Current.MainWindow;
				mainView.progressBar.Value = 1.0;
				await Task.Delay(500);
				mainView.progressBar.Value = 0.0;
				mainView.progressBar.Visibility = Visibility.Collapsed;
				mainView.lblsaveTime.Content = DateTime.Now.ToString("HH:mm:ss") + " " + Resources.WinStatuBarPDFAutoSaveText;
			}));
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0001E36C File Offset: 0x0001C56C
		private async void InitDefaultSettings()
		{
			PageSizeModel result = ConfigManager.GetPageSizeModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
			int num;
			if (result != null)
			{
				object obj = Enum.Parse(typeof(SizeModesWrap), result.SizeMode);
				this.ViewToolbar.DocSizeModeWrap = (SizeModesWrap)obj;
			}
			else
			{
				string pageDefaultSize = ConfigManager.GetPageDefaultSize();
				if (pageDefaultSize != "FitToWidth")
				{
					if (!(pageDefaultSize == "ZoomActualSize") && !(pageDefaultSize == "FitToSize") && !(pageDefaultSize == "FitToHeight"))
					{
						this.ViewToolbar.DocSizeModeWrap = SizeModesWrap.Zoom;
						if (pageDefaultSize != null)
						{
							num = pageDefaultSize.Length;
							if (num != 3)
							{
								if (num == 4)
								{
									switch (pageDefaultSize[0])
									{
									case '1':
										if (pageDefaultSize == "100%")
										{
											this.ViewToolbar.DocZoom = 1f;
											return;
										}
										if (pageDefaultSize == "125%")
										{
											this.ViewToolbar.DocZoom = 1.25f;
											return;
										}
										if (pageDefaultSize == "150%")
										{
											this.ViewToolbar.DocZoom = 1.5f;
											return;
										}
										break;
									case '2':
										if (pageDefaultSize == "200%")
										{
											this.ViewToolbar.DocZoom = 2f;
											return;
										}
										break;
									case '4':
										if (pageDefaultSize == "400%")
										{
											this.ViewToolbar.DocZoom = 4f;
											return;
										}
										break;
									case '6':
										if (pageDefaultSize == "600%")
										{
											this.ViewToolbar.DocZoom = 6f;
											return;
										}
										break;
									}
								}
							}
							else
							{
								switch (pageDefaultSize[0])
								{
								case '1':
									if (pageDefaultSize == "10%")
									{
										this.ViewToolbar.DocZoom = 0.1f;
										return;
									}
									break;
								case '2':
									if (pageDefaultSize == "25%")
									{
										this.ViewToolbar.DocZoom = 0.25f;
										return;
									}
									break;
								case '5':
									if (pageDefaultSize == "50%")
									{
										this.ViewToolbar.DocZoom = 0.5f;
										return;
									}
									break;
								case '7':
									if (pageDefaultSize == "75%")
									{
										this.ViewToolbar.DocZoom = 0.75f;
										return;
									}
									break;
								}
							}
						}
						this.ViewToolbar.DocZoom = 1f;
						return;
					}
					object obj2 = Enum.Parse(typeof(SizeModesWrap), pageDefaultSize);
					this.ViewToolbar.DocSizeModeWrap = (SizeModesWrap)obj2;
				}
				else
				{
					this.ViewToolbar.DocSizeModeWrap = SizeModesWrap.FitToWidth;
				}
			}
			PageDisplayModel result2 = ConfigManager.GetPageDisplayModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (result2 != null)
			{
				this.ViewToolbar.SubViewModePage = (SubViewModePage)Enum.Parse(typeof(SubViewModePage), result2.DisplayMode);
				this.ViewToolbar.SubViewModeContinuous = (SubViewModeContinuous)Enum.Parse(typeof(SubViewModeContinuous), result2.ContinuousDisplayMode);
			}
			else
			{
				this.ViewToolbar.SubViewModePage = SubViewModePage.SinglePage;
				this.ViewToolbar.SubViewModeContinuous = SubViewModeContinuous.Verticalcontinuous;
			}
			string defaultSelectionMode = ConfigManager.GetDefaultSelectionMode();
			if (string.IsNullOrEmpty(defaultSelectionMode))
			{
				if (MainViewModel.<>o__100.<>p__0 == null)
				{
					MainViewModel.<>o__100.<>p__0 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				MainViewModel.<>o__100.<>p__0.Target(MainViewModel.<>o__100.<>p__0, this.ViewerMouseMode, MouseModes.Default);
			}
			else if (defaultSelectionMode == "Hand")
			{
				if (MainViewModel.<>o__100.<>p__1 == null)
				{
					MainViewModel.<>o__100.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				MainViewModel.<>o__100.<>p__1.Target(MainViewModel.<>o__100.<>p__1, this.ViewerMouseMode, MouseModes.PanTool);
			}
			else
			{
				if (MainViewModel.<>o__100.<>p__2 == null)
				{
					MainViewModel.<>o__100.<>p__2 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				MainViewModel.<>o__100.<>p__2.Target(MainViewModel.<>o__100.<>p__2, this.ViewerMouseMode, MouseModes.Default);
			}
			BackgroundModel backGround = ConfigManager.GetBackgroundModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
			if (backGround != null)
			{
				IContextMenuModel contextMenuModel = this.ViewToolbar.BackgroundMenuItems.ToList<IContextMenuModel>().Find((IContextMenuModel x) => x.Name.Equals(backGround.BackgroundName));
				if (contextMenuModel != null)
				{
					BackgroundContextMenuItemModel backgroundContextMenuItemModel = contextMenuModel as BackgroundContextMenuItemModel;
					if (backgroundContextMenuItemModel != null)
					{
						backgroundContextMenuItemModel.IsChecked = true;
					}
				}
			}
			num = await ConfigManager.GetAutoScrollSpeedAsync(1);
			int autoScrollSpeed = num;
			ContextMenuItemModel contextMenuItemModel = this.ViewToolbar.AutoScrollMenuItems.OfType<ContextMenuItemModel>().FirstOrDefault(delegate(ContextMenuItemModel c)
			{
				object menuItemValue = c.TagData.MenuItemValue;
				if (menuItemValue is int)
				{
					int num2 = (int)menuItemValue;
					return num2 == autoScrollSpeed;
				}
				return false;
			});
			if (contextMenuItemModel == null)
			{
				contextMenuItemModel = this.ViewToolbar.AutoScrollMenuItems.OfType<ContextMenuItemModel>().FirstOrDefault(delegate(ContextMenuItemModel c)
				{
					object menuItemValue2 = c.TagData.MenuItemValue;
					if (menuItemValue2 is int)
					{
						int num3 = (int)menuItemValue2;
						return num3 == 1;
					}
					return false;
				});
				autoScrollSpeed = (int)contextMenuItemModel.TagData.MenuItemValue;
			}
			contextMenuItemModel.IsChecked = true;
			this.ViewToolbar.AutoScrollSpeed = autoScrollSpeed;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0001E3A4 File Offset: 0x0001C5A4
		private void DocumentWrapper_PasswordRequested(object sender, DocumentPasswordRequestedEventArgs e)
		{
			EnterPasswordDialog enterPasswordDialog = new EnterPasswordDialog(Path.GetFileName(e.FileName));
			enterPasswordDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			enterPasswordDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			e.Cancel = !enterPasswordDialog.ShowDialog().GetValueOrDefault();
			e.Password = enterPasswordDialog.Password;
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0001E408 File Offset: 0x0001C608
		private async void DocumentWrapper_FileError(object sender, EventArgs e)
		{
			await this.CloseDocumentAsync();
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x0001E43F File Offset: 0x0001C63F
		public DocumentWrapper DocumentWrapper
		{
			get
			{
				return this.documentWrapper;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x0001E447 File Offset: 0x0001C647
		public string StartUpFilePath { get; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x0001E450 File Offset: 0x0001C650
		public AsyncRelayCommand OpenStartUpFileCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.openStartUpFileCmd) == null)
				{
					asyncRelayCommand = (this.openStartUpFileCmd = new AsyncRelayCommand(async delegate
					{
						if (string.IsNullOrEmpty(this.StartUpFilePath) || !this.StartUpFilePath.ToLowerInvariant().EndsWith(".pdf"))
						{
							this.IsDocumentOpened = false;
							LaunchUtils.DoLaunchAction();
						}
						await this.OpenDocumentCoreAsync(this.StartUpFilePath, null, false);
						this.OpenDocCmd.NotifyCanExecuteChanged();
					}, () => !this.OpenStartUpFileCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x0001E48D File Offset: 0x0001C68D
		public bool HasExtraSaveOperation
		{
			get
			{
				return this.extraSaveOperationNames != null && this.extraSaveOperationNames.Count > 0;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0001E4A7 File Offset: 0x0001C6A7
		public global::System.Collections.Generic.IReadOnlyList<string> ExtraSaveOperationNames
		{
			get
			{
				List<string> list = this.extraSaveOperationNames;
				return ((list != null) ? list.ToArray() : null) ?? Array.Empty<string>();
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x0001E4C4 File Offset: 0x0001C6C4
		// (set) Token: 0x0600063E RID: 1598 RVA: 0x0001E4CC File Offset: 0x0001C6CC
		public OperationManager OperationManager
		{
			get
			{
				return this.operationManager;
			}
			private set
			{
				base.SetProperty<OperationManager>(ref this.operationManager, value, "OperationManager");
			}
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0001E4E1 File Offset: 0x0001C6E1
		public void SetCanSaveFlag()
		{
			this.SetCanSaveFlag("Unknown", false);
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0001E4F0 File Offset: 0x0001C6F0
		public void SetCanSaveFlag(string operation, bool clearOperationStack)
		{
			if (clearOperationStack)
			{
				OperationManager operationManager = this.OperationManager;
				if (operationManager != null)
				{
					Task task = operationManager.ClearAsync();
					if (task != null)
					{
						task.GetAwaiter().GetResult();
					}
				}
			}
			if (string.IsNullOrEmpty(operation))
			{
				operation = "Unknown";
			}
			this.<SetCanSaveFlag>g__AddOperation|124_0(operation);
			this.CanSave = true;
			pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().CanSaveByOperationManager = true;
			pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().LastOperationVersion = pdfeditor.AutoSaveRestore.AutoSaveManager.MutexOperationID;
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0001E55B File Offset: 0x0001C75B
		public void RemoveCanSaveFlag(bool clearOperationStack)
		{
			this.SetCanSaveFlag("Unknown", clearOperationStack);
			this.extraSaveOperationNames = null;
			this.UpdateCanSaveFlagState();
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x0001E578 File Offset: 0x0001C778
		public void UpdateCanSaveFlagState()
		{
			this.RedoCmd.NotifyCanExecuteChanged();
			this.UndoCmd.NotifyCanExecuteChanged();
			bool flag;
			if (!this.HasExtraSaveOperation)
			{
				string text = this.version;
				OperationManager operationManager = this.OperationManager;
				flag = text != ((operationManager != null) ? operationManager.Version : null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2 && this.Document != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				flag2 = flag2 || pdfControl.CanEditorUndo;
			}
			this.CanSave = flag2;
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x0001E5EF File Offset: 0x0001C7EF
		// (set) Token: 0x06000644 RID: 1604 RVA: 0x0001E5F8 File Offset: 0x0001C7F8
		public bool CanSave
		{
			get
			{
				return this.canSave;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.canSave, value, "CanSave"))
				{
					string text = this.CurrentFileName + " - PDFgear";
					if (value)
					{
						text += " *";
					}
					App.Current.MainWindow.Title = text;
					this.SaveCmd.NotifyCanExecuteChanged();
				}
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000645 RID: 1605 RVA: 0x0001E654 File Offset: 0x0001C854
		public string Password
		{
			get
			{
				DocumentWrapper documentWrapper = this.DocumentWrapper;
				if (((documentWrapper != null) ? documentWrapper.Document : null) != null)
				{
					if (this.DocumentWrapper.EncryptManage.IsHaveOwerPassword)
					{
						return this.DocumentWrapper.EncryptManage.OwerPassword;
					}
					if (this.DocumentWrapper.EncryptManage.IsHaveUserPassword)
					{
						return this.DocumentWrapper.EncryptManage.UserPassword;
					}
				}
				return null;
			}
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x0001E6BC File Offset: 0x0001C8BC
		private void UpdateDocument(bool closing)
		{
			OperationManager operationManager = this.OperationManager;
			if (operationManager != null)
			{
				operationManager.Dispose();
			}
			this.OperationManager = null;
			this.version = string.Empty;
			this.extraSaveOperationNames = null;
			this.pageEditorViewModel.FlushViewerAndThumbnail(false);
			DocumentWrapper documentWrapper = this.DocumentWrapper;
			if (((documentWrapper != null) ? documentWrapper.Document : null) != null)
			{
				this.OperationManager = new OperationManager(this.Document);
				this.OperationManager.StateChanged += delegate(object s, EventArgs a)
				{
					this.UpdateCanSaveFlagState();
				};
				if (this.DocumentWrapper.Document.FormFill != null)
				{
					this.DocumentWrapper.Document.FormFill.FieldChanged += this.DocumentFormFill_FieldChanged;
				}
				if (this.DocumentWrapper.Document.Pages.Count > 0)
				{
					PageDisposeHelper.TryFixPageAnnotations(this.DocumentWrapper.Document, 0);
				}
			}
			this.RedoCmd.NotifyCanExecuteChanged();
			this.UndoCmd.NotifyCanExecuteChanged();
			string text = this.version;
			OperationManager operationManager2 = this.OperationManager;
			this.CanSave = text != (((operationManager2 != null) ? operationManager2.Version : null) ?? string.Empty);
			this.TranslatePanelVisible = false;
			if (closing)
			{
				this.IsDocumentOpened = true;
				this.ChatButtonVisible = false;
				this.ChatPanelVisible = false;
			}
			else
			{
				DocumentWrapper documentWrapper2 = this.DocumentWrapper;
				this.IsDocumentOpened = ((documentWrapper2 != null) ? documentWrapper2.Document : null) != null;
				this.MaybeHaveUnembeddedSignature = false;
				DocumentWrapper documentWrapper3 = this.DocumentWrapper;
				this.ChatButtonVisible = ((documentWrapper3 != null) ? documentWrapper3.Document : null) != null && ConfigManager.GetShowcaseChatButtonFlag();
				this.ChatPanelVisible = !ConfigManager.GetChatPanelClosed() && this.ChatButtonVisible;
			}
			base.OnPropertyChanged("Document");
			this.UpdateDocumentCore();
			this.ViewToolbar.TryUpdateViewerBackground();
			if (!closing)
			{
				this.GetPageStyle();
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0001E880 File Offset: 0x0001CA80
		public void SetPageStyle()
		{
			MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			double contentWidth = mainView.LeftNavigationView.ContentWidth;
			bool flag = mainView.LeftNavigationView.SelectedIndex >= 0;
			double contentWidth2 = mainView.RightNavigationView.ContentWidth;
			NavigationModel selectedLeftNavItem = this.Menus.SelectedLeftNavItem;
			string text = ((selectedLeftNavItem != null) ? selectedLeftNavItem.Name : null);
			ConfigManager.SetPageStyleAsync(contentWidth, flag, text, contentWidth2);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0001E8EC File Offset: 0x0001CAEC
		public void GetPageStyle()
		{
			PageStyleModel pageStyle = ConfigManager.GetPageStyleAsync(default(CancellationToken)).ConfigureAwait(false).GetAwaiter()
				.GetResult();
			MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			if (mainView == null)
			{
				return;
			}
			if (pageStyle != null)
			{
				mainView.LeftNavigationView.ContentWidth = pageStyle.LeftNavigationViewWidth;
				if (pageStyle.RightNavigationViewWidth != 0.0)
				{
					mainView.RightNavigationView.ContentWidth = pageStyle.RightNavigationViewWidth;
				}
				if (!string.IsNullOrEmpty(pageStyle.LeftNavigationViewSelectItem))
				{
					ObservableCollection<NavigationModel> observableCollection = this.Menus.LeftNavList;
					NavigationModel navigationModel = ((observableCollection != null) ? observableCollection.FirstOrDefault((NavigationModel x) => x.Name.Equals(pageStyle.LeftNavigationViewSelectItem)) : null);
					if (navigationModel != null)
					{
						this.Menus.SelectedLeftNavItem = navigationModel;
						return;
					}
				}
			}
			else
			{
				mainView.LeftNavigationView.ContentWidth = 256.0;
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001E9F0 File Offset: 0x0001CBF0
		internal void UpdateDocumentCore()
		{
			AllPageCommetCollectionView allPageCommetCollectionView = this.PageCommetSource;
			if (allPageCommetCollectionView != null)
			{
				allPageCommetCollectionView.Dispose();
			}
			this.PageCommetSource = null;
			this.AttachmentSource = null;
			this.UpdateBookmarks();
			CopilotHelper copilotHelper = this.CopilotHelper;
			if (copilotHelper != null)
			{
				copilotHelper.Dispose();
			}
			this.CopilotHelper = null;
			DocumentWrapper documentWrapper = this.DocumentWrapper;
			if (((documentWrapper != null) ? documentWrapper.Document : null) != null)
			{
				this.Menus.SearchModel = new SearchModel(this.DocumentWrapper);
				this.ThumbnailItemSource = this.DocumentWrapper.Document.Pages.Select((PdfPage _, int i) => new PdfThumbnailModel(this.DocumentWrapper.Document, i)).ToList<PdfThumbnailModel>();
				PdfPageEditList pdfPageEditList = new PdfPageEditList(this.ThumbnailItemSource.Select((PdfThumbnailModel c) => new PdfPageEditListModel(c.Document, c.PageIndex)));
				if (this.PageEditors != null)
				{
					pdfPageEditList.Scale = this.PageEditors.PageEditerThumbnailScale;
				}
				this.PageEditors.PageEditListItemSource = pdfPageEditList;
				this.PageCommetSource = new AllPageCommetCollectionView(this.DocumentWrapper.Document);
				this.AttachmentSource = new AttachmentWrappersCollection(this.DocumentWrapper.Document);
				this.CopilotHelper = new CopilotHelper(this.DocumentWrapper.Document, this.DocumentWrapper.DocumentPath);
			}
			else
			{
				this.ThumbnailItemSource = null;
				this.PageEditors.PageEditListItemSource = null;
				this.PageCommetSource = null;
				this.AttachmentSource = null;
				this.ViewToolbar.AutoScrollButtonModel.IsChecked = false;
				this.ExitTransientMode(false, false, false, false, false);
			}
			this.pageEditorViewModel.FlushViewerAndThumbnail(false);
			NavigationModel selectedLeftNavItem = this.Menus.SelectedLeftNavItem;
			DocumentWrapper documentWrapper2 = this.DocumentWrapper;
			if (((documentWrapper2 != null) ? documentWrapper2.Document : null) == null)
			{
				this.Menus.SelectedLeftNavItem = null;
			}
			else if (((selectedLeftNavItem != null) ? selectedLeftNavItem.Name : null) == "Bookmark")
			{
				if (this.Bookmarks == null || this.Bookmarks.Children.Count == 0)
				{
					this.Menus.SelectedLeftNavItem = null;
				}
			}
			else if (((selectedLeftNavItem != null) ? selectedLeftNavItem.Name : null) == "Annotation")
			{
				AllPageCommetCollectionView allPageCommetCollectionView2 = this.PageCommetSource;
				if (allPageCommetCollectionView2 != null)
				{
					allPageCommetCollectionView2.StartLoad();
				}
				this.AllCount = 0;
				foreach (PageCommetCollection pageCommetCollection in this.PageCommetSource)
				{
					this.AllCount += pageCommetCollection.Count;
				}
			}
			base.OnPropertyChanged("TotalPagesCount");
			base.OnPropertyChanged("IsLeftNavigationMenuEnabled");
			base.OnPropertyChanged("IsFirstPage");
			base.OnPropertyChanged("IsLastPage");
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x0001EC9C File Offset: 0x0001CE9C
		internal void UpdateBookmarks()
		{
			if (this.DocumentWrapper.Document != null)
			{
				this.Bookmarks = BookmarkModel.Create(this.DocumentWrapper.Document);
				return;
			}
			this.Bookmarks = null;
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x0600064B RID: 1611 RVA: 0x0001ECC9 File Offset: 0x0001CEC9
		public PdfDocument Document
		{
			get
			{
				DocumentWrapper documentWrapper = this.DocumentWrapper;
				if (documentWrapper == null)
				{
					return null;
				}
				return documentWrapper.Document;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001ECDC File Offset: 0x0001CEDC
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x0001ECE4 File Offset: 0x0001CEE4
		public List<PdfThumbnailModel> ThumbnailItemSource
		{
			get
			{
				return this.thumbnailItemSource;
			}
			set
			{
				base.SetProperty<List<PdfThumbnailModel>>(ref this.thumbnailItemSource, value, "ThumbnailItemSource");
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0001ECF9 File Offset: 0x0001CEF9
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x0001ED04 File Offset: 0x0001CF04
		public double ThumbnailScale
		{
			get
			{
				return this.thumbnailScale;
			}
			set
			{
				if (value < 0.5)
				{
					value = 0.5;
				}
				if (value > 3.0)
				{
					value = 3.0;
				}
				if (base.SetProperty<double>(ref this.thumbnailScale, value, "ThumbnailScale"))
				{
					this.ThumbnailZoonOut.NotifyCanExecuteChanged();
					this.ThumbnailZoonIn.NotifyCanExecuteChanged();
				}
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x0001ED69 File Offset: 0x0001CF69
		// (set) Token: 0x06000651 RID: 1617 RVA: 0x0001ED71 File Offset: 0x0001CF71
		public List<PdfThumbnailModel> SelectedThumbnailList { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x0001ED7C File Offset: 0x0001CF7C
		public AsyncRelayCommand ThumbnailZoonOut
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.thumbnailZoonOut) == null)
				{
					asyncRelayCommand = (this.thumbnailZoonOut = new AsyncRelayCommand(async delegate
					{
						double num = this.thumbnailScale - (double)MainViewModel.ScaleStep;
						this.ThumbnailScale = num;
					}, () => this.CanThumbnailZoonOut()));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0001EDB9 File Offset: 0x0001CFB9
		private bool CanThumbnailZoonOut()
		{
			return this.ThumbnailScale > 0.0 && MainViewModel.MaxScale > 0f && (double)(MainViewModel.MinScale + MainViewModel.eps) < this.ThumbnailScale;
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x0001EDF0 File Offset: 0x0001CFF0
		public AsyncRelayCommand ThumbnailZoonIn
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.thumbnailZoonIn) == null)
				{
					asyncRelayCommand = (this.thumbnailZoonIn = new AsyncRelayCommand(async delegate
					{
						double num = this.thumbnailScale + (double)MainViewModel.ScaleStep;
						this.ThumbnailScale = num;
					}, () => this.CanThumbnailZoonIn()));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0001EE2D File Offset: 0x0001D02D
		private bool CanThumbnailZoonIn()
		{
			return this.ThumbnailScale > 0.0 && MainViewModel.MaxScale > 0f && this.ThumbnailScale + (double)MainViewModel.eps < (double)MainViewModel.MaxScale;
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x0001EE63 File Offset: 0x0001D063
		// (set) Token: 0x06000657 RID: 1623 RVA: 0x0001EE6B File Offset: 0x0001D06B
		public BookmarkModel Bookmarks
		{
			get
			{
				return this.bookmarks;
			}
			set
			{
				base.SetProperty<BookmarkModel>(ref this.bookmarks, value, "Bookmarks");
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x0001EE80 File Offset: 0x0001D080
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x0001EE88 File Offset: 0x0001D088
		public AllPageCommetCollectionView PageCommetSource
		{
			get
			{
				return this.pageCommetSource;
			}
			set
			{
				base.SetProperty<AllPageCommetCollectionView>(ref this.pageCommetSource, value, "PageCommetSource");
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x0001EE9D File Offset: 0x0001D09D
		// (set) Token: 0x0600065B RID: 1627 RVA: 0x0001EEA5 File Offset: 0x0001D0A5
		public AttachmentWrappersCollection AttachmentSource
		{
			get
			{
				return this.attachmentSource;
			}
			set
			{
				base.SetProperty<AttachmentWrappersCollection>(ref this.attachmentSource, value, "AttachmentSource");
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x0001EEBA File Offset: 0x0001D0BA
		// (set) Token: 0x0600065D RID: 1629 RVA: 0x0001EEC2 File Offset: 0x0001D0C2
		public CopilotHelper CopilotHelper
		{
			get
			{
				return this.copilotHelper;
			}
			set
			{
				base.SetProperty<CopilotHelper>(ref this.copilotHelper, value, "CopilotHelper");
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x0001EED7 File Offset: 0x0001D0D7
		// (set) Token: 0x0600065F RID: 1631 RVA: 0x0001EEDF File Offset: 0x0001D0DF
		public Thickness PdfToWordMargin
		{
			get
			{
				return this.pdfToWordMargin;
			}
			set
			{
				base.SetProperty<Thickness>(ref this.pdfToWordMargin, value, "PdfToWordMargin");
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0001EEF4 File Offset: 0x0001D0F4
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x0001EEFC File Offset: 0x0001D0FC
		public int SelectedPageIndex
		{
			get
			{
				return this.selectedPageIndex;
			}
			set
			{
				int num = 1;
				if (this.DocumentWrapper.Document != null)
				{
					num = this.DocumentWrapper.Document.Pages.Count;
				}
				int num2 = value;
				if (num2 > num - 1)
				{
					num2 = num - 1;
				}
				else if (num2 < 0)
				{
					try
					{
						int num3 = ((this.LastViewPage == null) ? 0 : this.LastViewPage.PageIndex);
						num2 = Math.Max(0, num3);
					}
					catch (Exception)
					{
						num2 = 0;
					}
				}
				if ((DateTime.Now - this.time).TotalMilliseconds >= 200.0 && !this.Jumping && this.CurrnetPageIndex >= 1)
				{
					this.viewJumpManager.StackChange();
					this.viewJumpManager.NewRecord(this.CurrnetPageIndex - 1);
					this.Jumping = false;
				}
				if (num2 != this.selectedPageIndex && PageDisposeHelper.TryFixPageAnnotations(this.DocumentWrapper.Document, num2) && this.DocumentWrapper.Document.Pages[num2].IsLoaded)
				{
					this.DocumentWrapper.Document.Pages[num2].Dispose();
				}
				if (base.SetProperty<int>(ref this.selectedPageIndex, num2, "SelectedPageIndex"))
				{
					base.OnPropertyChanged("IsFirstPage");
					base.OnPropertyChanged("IsLastPage");
					if (this.DocumentWrapper.Document != null)
					{
						DocumentWrapper documentWrapper = this.DocumentWrapper;
						string text = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
						if (!string.IsNullOrEmpty(text))
						{
							ConfigManager.SetDocumentCurrentPageNumberAsync(text, num2);
						}
					}
				}
				base.OnPropertyChanged("CurrnetPageIndex");
				this.Jumping = false;
				this.time = DateTime.Now;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0001F0A0 File Offset: 0x0001D2A0
		public bool IsLeftNavigationMenuEnabled
		{
			get
			{
				return this.Document != null;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x0001F0AB File Offset: 0x0001D2AB
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x0001F0B3 File Offset: 0x0001D2B3
		public bool IsDocumentOpened
		{
			get
			{
				return this.isDocumentOpened;
			}
			set
			{
				base.SetProperty<bool>(ref this.isDocumentOpened, value, "IsDocumentOpened");
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x0001F0C8 File Offset: 0x0001D2C8
		public int TotalPagesCount
		{
			get
			{
				if (this.Document == null || this.Document.Pages == null)
				{
					return 0;
				}
				return this.Document.Pages.Count;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x0001F0F1 File Offset: 0x0001D2F1
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x0001F112 File Offset: 0x0001D312
		public int CurrnetPageIndex
		{
			get
			{
				if (this.Document == null || this.Document.Pages == null)
				{
					return 0;
				}
				return this.SelectedPageIndex + 1;
			}
			set
			{
				this.SelectedPageIndex = value - 1;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0001F11D File Offset: 0x0001D31D
		public bool IsFirstPage
		{
			get
			{
				return this.Document == null || this.Document.Pages == null || this.Document.Pages.CurrentIndex <= 0;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x0001F14C File Offset: 0x0001D34C
		public bool IsLastPage
		{
			get
			{
				return this.Document == null || this.Document.Pages == null || this.Document.Pages.CurrentIndex >= this.Document.Pages.Count - 1;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x0001F18C File Offset: 0x0001D38C
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x0001F194 File Offset: 0x0001D394
		public bool FillForm
		{
			get
			{
				return this.fillForm;
			}
			set
			{
				base.SetProperty<bool>(ref this.fillForm, value, "FillForm");
				((MainView)App.Current.MainWindow).PdfControl.Viewer.IsFillFormHighlighted = value;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0001F1C8 File Offset: 0x0001D3C8
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x0001F1D0 File Offset: 0x0001D3D0
		public AllPageCommetCollectionView AllPageCommetSource
		{
			get
			{
				return this.allpageCommetSource;
			}
			set
			{
				base.SetProperty<AllPageCommetCollectionView>(ref this.allpageCommetSource, value, "AllPageCommetSource");
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0001F1E5 File Offset: 0x0001D3E5
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x0001F1F0 File Offset: 0x0001D3F0
		public bool? IsUserFilterAllChecked
		{
			get
			{
				return this.isUserFilterAllChecked;
			}
			set
			{
				if (value.GetValueOrDefault() && this.IsKindFilterAllChecked.GetValueOrDefault())
				{
					this.IsFilterAllChecked = new bool?(true);
				}
				else
				{
					this.IsFilterAllChecked = new bool?(false);
				}
				base.SetProperty<bool?>(ref this.isUserFilterAllChecked, value, "IsUserFilterAllChecked");
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0001F243 File Offset: 0x0001D443
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x0001F24B File Offset: 0x0001D44B
		public UserInfoModel UserInfoModel
		{
			get
			{
				return this.userInfoModel;
			}
			set
			{
				base.SetProperty<UserInfoModel>(ref this.userInfoModel, value, "UserInfoModel");
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000672 RID: 1650 RVA: 0x0001F260 File Offset: 0x0001D460
		// (set) Token: 0x06000673 RID: 1651 RVA: 0x0001F268 File Offset: 0x0001D468
		public int AllCount
		{
			get
			{
				return this.allCount;
			}
			set
			{
				base.SetProperty<int>(ref this.allCount, value, "AllCount");
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x0001F27D File Offset: 0x0001D47D
		// (set) Token: 0x06000675 RID: 1653 RVA: 0x0001F285 File Offset: 0x0001D485
		public int AttachmentAllCount
		{
			get
			{
				return this.attachmentAllCount;
			}
			set
			{
				base.SetProperty<int>(ref this.attachmentAllCount, value, "AttachmentAllCount");
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0001F29A File Offset: 0x0001D49A
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x0001F2A2 File Offset: 0x0001D4A2
		public bool? IsFilterAllChecked
		{
			get
			{
				return this.isFilterAllChecked;
			}
			set
			{
				base.SetProperty<bool?>(ref this.isFilterAllChecked, value, "IsFilterAllChecked");
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001F2B7 File Offset: 0x0001D4B7
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x0001F2C0 File Offset: 0x0001D4C0
		public bool? IsKindFilterAllChecked
		{
			get
			{
				return this.isKindFilterAllChecked;
			}
			set
			{
				if (value.GetValueOrDefault() && this.IsUserFilterAllChecked.GetValueOrDefault())
				{
					this.IsFilterAllChecked = new bool?(true);
				}
				else
				{
					this.IsFilterAllChecked = new bool?(false);
				}
				base.SetProperty<bool?>(ref this.isKindFilterAllChecked, value, "IsKindFilterAllChecked");
			}
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0001F313 File Offset: 0x0001D513
		public void FilterAnnotations()
		{
			this.IsSelectedAll = new bool?(false);
			this.pageCommetSource.FilterShowItems();
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0001F32C File Offset: 0x0001D52C
		public RelayCommand UserMannulCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.userMannulCmd) == null)
				{
					relayCommand = (this.userMannulCmd = new RelayCommand(async delegate
					{
						UserGuideUtils.OpenUserGuide();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x0001F370 File Offset: 0x0001D570
		public RelayCommand UserGuideCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.userGuideCmd) == null)
				{
					relayCommand = (this.userGuideCmd = new RelayCommand(delegate
					{
						this.OpenUserGuide();
					}, () => this.CanDoFeedBack()));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0001F3B0 File Offset: 0x0001D5B0
		public RelayCommand GetPhoneStoreCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.getPhoneStoreCmd) == null)
				{
					relayCommand = (this.getPhoneStoreCmd = new RelayCommand(delegate
					{
						this.OpenPhoneWin();
					}, () => this.CanDoFeedBack()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0001F3ED File Offset: 0x0001D5ED
		private void OpenPhoneWin()
		{
			GAManager.SendEvent("Ads", "GearForMobile", "ToolbarBtn", 1L);
			new GearForMobilephone
			{
				Owner = (MainView)Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog();
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0001F42C File Offset: 0x0001D62C
		private void OpenUserGuide()
		{
			GAManager.SendEvent("MainWindow", "UserGuide", "Count", 1L);
			string actualAppLanguage = CultureInfoUtils.ActualAppLanguage;
			string text = "https://www.pdfgear.com/";
			string text2 = "windows-user-guide/introduction-pdfgear.htm";
			string gearRate = Path.Combine(text, text2).Replace("\\", "/");
			if (actualAppLanguage.ToLower() == "es")
			{
				gearRate = Path.Combine(text, "es/windows-user-guide/introduccion-pdfgear.htm").Replace("\\", "/");
			}
			if (actualAppLanguage.ToLower() == "fr")
			{
				gearRate = Path.Combine(text, "fr/windows-user-guide/introduction-de-pdfgear.htm").Replace("\\", "/");
			}
			if (actualAppLanguage.ToLower() == "it")
			{
				gearRate = Path.Combine(text, "it/windows-user-guide/introduzione-pdfgear.htm").Replace("\\", "/");
			}
			if (actualAppLanguage.ToLower() == "pt")
			{
				gearRate = Path.Combine(text, "pt/windows-user-guide/introducao-pdfgear.htm").Replace("\\", "/");
			}
			if (actualAppLanguage.ToLower() == "de")
			{
				gearRate = Path.Combine(text, "de/anleitung/einfuehrung-in-pdfgear.htm").Replace("\\", "/");
			}
			object locker = new object();
			bool result = false;
			new Thread(delegate
			{
				try
				{
					Process.Start(gearRate);
					result = true;
				}
				catch
				{
					result = false;
				}
				finally
				{
					object locker2 = locker;
					lock (locker2)
					{
						Monitor.PulseAll(locker);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
			object locker3 = locker;
			lock (locker3)
			{
				Monitor.Wait(locker, 5000);
				bool result2 = result;
				GAManager.SendEvent("MainWindow", "GuideBlockExit", "Count", 1L);
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x0001F608 File Offset: 0x0001D808
		public RelayCommand FeedBackCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.feedBackCmd) == null)
				{
					relayCommand = (this.feedBackCmd = new RelayCommand(delegate
					{
						this.DoFeedBack();
					}, () => this.CanDoFeedBack()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0001F645 File Offset: 0x0001D845
		private void DoFeedBack()
		{
			DocumentWrapper documentWrapper = this.DocumentWrapper;
			SupportUtils.ShowFeedbackWindow((documentWrapper != null) ? documentWrapper.DocumentPath : null);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0001F65E File Offset: 0x0001D85E
		private bool CanDoFeedBack()
		{
			return true;
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000683 RID: 1667 RVA: 0x0001F664 File Offset: 0x0001D864
		public RelayCommand AutoSaveSettingCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.autoSaveSettingCmd) == null)
				{
					relayCommand = (this.autoSaveSettingCmd = new RelayCommand(delegate
					{
						this.DoAutoSaveSetting();
					}, () => this.CanDoAutoSaveSetting()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0001F6A1 File Offset: 0x0001D8A1
		public void DoAutoSaveSetting()
		{
			new SettingWindow().Show();
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0001F6AD File Offset: 0x0001D8AD
		private bool CanDoAutoSaveSetting()
		{
			return true;
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0001F6B0 File Offset: 0x0001D8B0
		public RelayCommand UpgradeAppCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.upgradeAppCmd) == null)
				{
					relayCommand = (this.upgradeAppCmd = new RelayCommand(delegate
					{
						this.ShowAppUpgrade();
					}, () => this.CanShowAppUpgrade()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0001F6ED File Offset: 0x0001D8ED
		private void ShowAppUpgrade()
		{
			GAManager.SendEvent("MainWindow", "UpdateBtnClick", "Count", 1L);
			new IAPWin(IAPType.APP).ShowDialog();
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0001F711 File Offset: 0x0001D911
		private bool CanShowAppUpgrade()
		{
			return true;
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000689 RID: 1673 RVA: 0x0001F714 File Offset: 0x0001D914
		public RelayCommand UpgradeAICmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.upgradeAICmd) == null)
				{
					relayCommand = (this.upgradeAICmd = new RelayCommand(delegate
					{
						this.ShowAIUpgrade();
					}, () => this.CanShowAIUpgrade()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0001F751 File Offset: 0x0001D951
		private void ShowAIUpgrade()
		{
			GAManager.SendEvent("MainWindow", "AIUpdateBtnClick", "Count", 1L);
			new IAPWin(IAPType.AI).ShowDialog();
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0001F775 File Offset: 0x0001D975
		private bool CanShowAIUpgrade()
		{
			return true;
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001F778 File Offset: 0x0001D978
		public RelayCommand AboutCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.aboutCmd) == null)
				{
					relayCommand = (this.aboutCmd = new RelayCommand(delegate
					{
						this.ShowAbout();
					}, () => this.CanShowAbout()));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x0001F7B8 File Offset: 0x0001D9B8
		public RelayCommand PropertiesCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.propertiesCmd) == null)
				{
					relayCommand = (this.propertiesCmd = new RelayCommand(delegate
					{
						this.ShowPropertiesFToolbar();
					}, () => this.CanShowProperties()));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001F7F8 File Offset: 0x0001D9F8
		public RelayCommand UpdateCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.updateCmd) == null)
				{
					relayCommand = (this.updateCmd = new RelayCommand(delegate
					{
						this.Update();
					}, () => this.CanShowAbout()));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600068F RID: 1679 RVA: 0x0001F838 File Offset: 0x0001DA38
		public AsyncRelayCommand SettingsCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.settingsCmd) == null)
				{
					asyncRelayCommand = (this.settingsCmd = new AsyncRelayCommand(async delegate
					{
						await Ioc.Default.GetRequiredService<AppSettingsViewModel>().RefreshSettingsAsync();
						AppSettingsWindow appSettingsWindow = new AppSettingsWindow();
						appSettingsWindow.Owner = App.Current.MainWindow;
						appSettingsWindow.WindowStartupLocation = ((appSettingsWindow.Owner != null) ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen);
						appSettingsWindow.ShowDialog();
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0001F87C File Offset: 0x0001DA7C
		private void ShowAbout()
		{
			AboutWindow aboutWindow = new AboutWindow();
			if (aboutWindow == null)
			{
				return;
			}
			aboutWindow.ShowDialog();
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0001F88E File Offset: 0x0001DA8E
		public void ShowPropertiesFToolbar()
		{
			GAManager.SendEvent("DocumentPropertiesWindow", "ShowSource", "Toolbar", 1L);
			this.ShowProperties();
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0001F8AC File Offset: 0x0001DAAC
		public void ShowProperties()
		{
			if (this.Document == null)
			{
				return;
			}
			PdfDocument document = this.Document;
			int currnetPageIndex = this.CurrnetPageIndex;
			DocumentWrapper documentWrapper = this.documentWrapper;
			DocumentPropertiesWindow documentPropertiesWindow = new DocumentPropertiesWindow(document, currnetPageIndex, (documentWrapper != null) ? documentWrapper.DocumentPath : null);
			if (documentPropertiesWindow != null)
			{
				documentPropertiesWindow.Owner = (MainView)Application.Current.MainWindow;
				documentPropertiesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				documentPropertiesWindow.ShowDialog();
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0001F90C File Offset: 0x0001DB0C
		private async void Update()
		{
			await UpdateHelper.UpdateAndExit(true);
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0001F93B File Offset: 0x0001DB3B
		private bool CanShowAbout()
		{
			return true;
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0001F93E File Offset: 0x0001DB3E
		private bool CanShowProperties()
		{
			return true;
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x0001F944 File Offset: 0x0001DB44
		public AsyncRelayCommand ShareFileMailCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.shareFileMailCmd) == null)
				{
					asyncRelayCommand = (this.shareFileMailCmd = new AsyncRelayCommand(async delegate
					{
						await this.SharebyEmailCmd(null);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x0001F978 File Offset: 0x0001DB78
		public AsyncRelayCommand ShareFileCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.shareFileCmd) == null)
				{
					asyncRelayCommand = (this.shareFileCmd = new AsyncRelayCommand(async delegate
					{
						this.SharebyFileCmd(null);
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0001F9AC File Offset: 0x0001DBAC
		private async Task SharebyEmailCmd(ContextMenuItemModel model = null)
		{
			if (this.Document != null)
			{
				if (this.CanSave)
				{
					ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					await ShareUtils.SendMailAsync(this.DocumentWrapper.DocumentPath);
				}
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001F9F0 File Offset: 0x0001DBF0
		private void SharebyFileCmd(ContextMenuItemModel model = null)
		{
			if (this.Document == null)
			{
				return;
			}
			if (this.CanSave)
			{
				ModernMessageBox.Show(Resources.SaveDocBeforeSendMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			new ShareSendFileDialog(this.DocumentWrapper.DocumentPath)
			{
				Owner = mainView,
				WindowStartupLocation = ((mainView == null) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner)
			}.ShowDialog();
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0001FA64 File Offset: 0x0001DC64
		public AsyncRelayCommand PrintDocCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.printDocCmd) == null)
				{
					asyncRelayCommand = (this.printDocCmd = new AsyncRelayCommand(async delegate
					{
						await this.PrintDoc(Source.Default);
					}, () => this.CanPrintDoc()));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x0001FAA4 File Offset: 0x0001DCA4
		public AsyncRelayCommand PrintDocCmdFromThumbnail
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.printDocCmdFromThumbnail) == null)
				{
					asyncRelayCommand = (this.printDocCmdFromThumbnail = new AsyncRelayCommand(async delegate
					{
						await this.PrintDoc(Source.Thumbnail);
					}, () => this.CanPrintDoc()));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001FAE4 File Offset: 0x0001DCE4
		public AsyncRelayCommand BatchPrintCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.batchPrintCmd) == null)
				{
					asyncRelayCommand = (this.batchPrintCmd = new AsyncRelayCommand(async delegate
					{
						await this.BatchPrintAsync("MainWindow");
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0001FB18 File Offset: 0x0001DD18
		public async Task ReleaseViewerFocusAsync(bool beforeSave)
		{
			try
			{
				PdfDocument document = this.Document;
				if (document != null)
				{
					PdfForms formFill = document.FormFill;
					if (formFill != null)
					{
						formFill.ForceToKillFocus();
					}
				}
			}
			catch
			{
			}
			if (beforeSave)
			{
				global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				if (viewer != null)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
					if (annotationHolderManager != null)
					{
						annotationHolderManager.CancelAll();
						await annotationHolderManager.WaitForCancelCompletedAsync();
					}
					AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(viewer);
					if (annotationCanvas != null)
					{
						annotationCanvas.PopupHolder.KillFocus();
					}
				}
				viewer = null;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001FB64 File Offset: 0x0001DD64
		public AsyncRelayCommand OpenDocCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.openDocCmd) == null)
				{
					asyncRelayCommand = (this.openDocCmd = new AsyncRelayCommand(async delegate
					{
						OpenFileDialog openFileDialog = new OpenFileDialog
						{
							Filter = "pdf|*.pdf",
							ShowReadOnly = false,
							ReadOnlyChecked = true
						};
						if (openFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
						{
							string fileName = openFileDialog.FileName;
							await this.OpenDocByPathAsync(fileName);
						}
					}, () => true));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0001FBB4 File Offset: 0x0001DDB4
		public async Task OpenDocByPathAsync(string filePath)
		{
			if (this.Document == null)
			{
				await this.OpenDocumentCoreAsync(filePath, null, false);
			}
			else
			{
				MainViewModel.OpenFile(filePath);
			}
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0001FC00 File Offset: 0x0001DE00
		public static bool OpenFile(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				try
				{
					if (new FileInfo(filePath).Extension.ToLowerInvariant() == ".pdf")
					{
						Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdfeditor.exe"), "\"" + filePath + "\"");
					}
					else
					{
						Process.Start(filePath);
					}
					return true;
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0001FC80 File Offset: 0x0001DE80
		public async Task<bool> OpenDocumentCoreAsync(string filePath, string password = null, bool isReOpenCurrentDoc = true)
		{
			bool flag;
			if (string.IsNullOrEmpty(filePath))
			{
				flag = false;
			}
			else if (filePath == this.DocumentWrapper.DocumentPath)
			{
				flag = true;
			}
			else
			{
				this.IsDocumentOpened = true;
				bool res = false;
				Exception exception = null;
				try
				{
					PdfDocument oldDocument = this.Document;
					if (!isReOpenCurrentDoc)
					{
						GAManager.SendEvent("MainWindow", "OpenDocumentCore", "Begin", 1L);
					}
					bool flag2 = await this.DocumentWrapper.OpenAsync(filePath, password);
					res = flag2;
					if (res)
					{
						Task.Run(delegate
						{
							bool flag3;
							HistoryManager.UpdateHistoryItem(filePath.FullPathWithoutPrefix, true, out flag3);
						});
						MainView mainView = (MainView)App.Current.MainWindow;
						mainView.lblsaveTime.Content = "";
						this.extraSaveOperationNames = null;
						PdfDocument pdfDocument = oldDocument;
						if (((pdfDocument != null) ? pdfDocument.FormFill : null) != null)
						{
							oldDocument.FormFill.FieldChanged -= this.DocumentFormFill_FieldChanged;
						}
						int num = await ConfigManager.GetDocumentCurrentPageNumberAsync(this.DocumentWrapper.DocumentPath, default(CancellationToken));
						if (this.documentWrapper.IsUntitledFile)
						{
							this.GetPageSizeZoomModel(null);
						}
						else
						{
							this.GetPageSizeZoomModel(this.DocumentWrapper.DocumentPath);
						}
						this.UpdateDocument(false);
						this.AutoSaveModel.SourceFileName = filePath;
						pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().Start(this.AutoSaveModel.SpanMinutes);
						this.time = DateTime.Now;
						if (num != -1 && num < this.DocumentWrapper.Document.Pages.Count)
						{
							this.SelectedPageIndex = num;
						}
						else
						{
							this.SelectedPageIndex = 0;
						}
						this.DocumentWrapper.TrimMemory();
						this.viewJumpManager.ClearStack();
						this.ReadAcitved();
						string fileName = Path.GetFileName(filePath);
						this.CurrentFileName = fileName;
						App.Current.MainWindow.Title = "PDFgear - " + fileName;
						mainView.PdfControl.Viewer.IsFillFormHighlighted = ConfigManager.GetIsFillFormHighlightedFlag();
						ConfigManager.SetCouldRateFlag(true);
						global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
						if (pdfControl != null)
						{
							pdfControl.Focus();
						}
						mainView = null;
					}
					else
					{
						ConfigManager.SetCouldRateFlag(false);
						if (!isReOpenCurrentDoc)
						{
							GAManager.SendEvent("MainWindow", "OpenDocumentCore", "Failed", 1L);
						}
					}
					oldDocument = null;
				}
				catch (Exception ex)
				{
					ConfigManager.SetCouldRateFlag(false);
					if (!isReOpenCurrentDoc)
					{
						GAManager.SendEvent("MainWindow", "OpenDocumentCore", "Exception", 1L);
					}
					exception = ex;
				}
				this.IsDocumentOpened = this.DocumentWrapper.Document != null;
				LaunchUtils.OnDocumentLoaded(this.Document);
				if (!res)
				{
					string msg = Resources.OpenDocFailedExceptionMsg;
					if (!string.IsNullOrEmpty(filePath.FullPathWithoutPrefix))
					{
						msg = msg + ": \"" + filePath.FullPathWithoutPrefix + "\"";
					}
					if (exception != null)
					{
						msg = msg + "\n\n" + exception.Message;
					}
					DispatcherHelper.UIDispatcher.Invoke(delegate
					{
						MessageBox.Show(msg, "PDFgear", MessageBoxButton.OK);
					});
				}
				flag = res;
			}
			return flag;
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0001FCDC File Offset: 0x0001DEDC
		private void ReadAcitved()
		{
			SpeechUtils speechUtils = this.speechUtils;
			if (speechUtils != null)
			{
				speechUtils.Dispose();
			}
			this.speechUtils = null;
			this.IsReading = false;
			this.ViewToolbar.ReadButtonModel.IsChecked = false;
			ContextMenuModel contextMenuModel = (this.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
			(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = true;
			(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = true;
			(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = true;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0001FD70 File Offset: 0x0001DF70
		private void GetPageSizeZoomModel(string filePath)
		{
			PageSizeZoomModel result = ConfigManager.GetPageSizeZoomModelAsync(filePath, default(CancellationToken)).GetAwaiter().GetResult();
			if (result != null)
			{
				object obj = Enum.Parse(typeof(SizeModesWrap), result.SizeMode);
				this.ViewToolbar.DocZoom = result.PageZoom;
				this.ViewToolbar.DocSizeModeWrap = (SizeModesWrap)obj;
				return;
			}
			string pageDefaultSize = ConfigManager.GetPageDefaultSize();
			if (!(pageDefaultSize != "FitToWidth"))
			{
				this.ViewToolbar.DocSizeModeWrap = SizeModesWrap.FitToWidth;
				return;
			}
			if (pageDefaultSize == "ZoomActualSize" || pageDefaultSize == "FitToSize" || pageDefaultSize == "FitToHeight")
			{
				object obj2 = Enum.Parse(typeof(SizeModesWrap), pageDefaultSize);
				this.ViewToolbar.DocSizeModeWrap = (SizeModesWrap)obj2;
				return;
			}
			if (pageDefaultSize != null)
			{
				int length = pageDefaultSize.Length;
				if (length != 3)
				{
					if (length == 4)
					{
						switch (pageDefaultSize[0])
						{
						case '1':
							if (pageDefaultSize == "100%")
							{
								this.ViewToolbar.DocZoom = 1f;
								goto IL_02E5;
							}
							if (pageDefaultSize == "125%")
							{
								this.ViewToolbar.DocZoom = 1.25f;
								goto IL_02E5;
							}
							if (pageDefaultSize == "150%")
							{
								this.ViewToolbar.DocZoom = 1.5f;
								goto IL_02E5;
							}
							break;
						case '2':
							if (pageDefaultSize == "200%")
							{
								this.ViewToolbar.DocZoom = 2f;
								goto IL_02E5;
							}
							break;
						case '4':
							if (pageDefaultSize == "400%")
							{
								this.ViewToolbar.DocZoom = 4f;
								goto IL_02E5;
							}
							break;
						case '6':
							if (pageDefaultSize == "600%")
							{
								this.ViewToolbar.DocZoom = 6f;
								goto IL_02E5;
							}
							break;
						}
					}
				}
				else
				{
					switch (pageDefaultSize[0])
					{
					case '1':
						if (pageDefaultSize == "10%")
						{
							this.ViewToolbar.DocZoom = 0.1f;
							goto IL_02E5;
						}
						break;
					case '2':
						if (pageDefaultSize == "25%")
						{
							this.ViewToolbar.DocZoom = 0.25f;
							goto IL_02E5;
						}
						break;
					case '5':
						if (pageDefaultSize == "50%")
						{
							this.ViewToolbar.DocZoom = 0.5f;
							goto IL_02E5;
						}
						break;
					case '7':
						if (pageDefaultSize == "75%")
						{
							this.ViewToolbar.DocZoom = 0.75f;
							goto IL_02E5;
						}
						break;
					}
				}
			}
			this.ViewToolbar.DocZoom = 1f;
			IL_02E5:
			this.ViewToolbar.DocSizeModeWrap = SizeModesWrap.Zoom;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0002007B File Offset: 0x0001E27B
		private void DocumentFormFill_FieldChanged(object sender, EventArgs e)
		{
			this.SetCanSaveFlag();
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00020084 File Offset: 0x0001E284
		public async Task CloseDocumentAsync()
		{
			await this.ReleaseViewerFocusAsync(true);
			this.DocumentWrapper.CloseDocument();
			this.UpdateDocument(true);
			await DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
			{
				this.DocumentWrapper.TrimMemory();
				TempFileUtils.ClearDocuments();
			}));
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x000200C8 File Offset: 0x0001E2C8
		public AsyncRelayCommand SaveCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.saveCmd) == null)
				{
					asyncRelayCommand = (this.saveCmd = new AsyncRelayCommand(async delegate
					{
						MainViewModel.<>c__DisplayClass311_0 CS$<>8__locals1 = new MainViewModel.<>c__DisplayClass311_0();
						CS$<>8__locals1.<>4__this = this;
						if (!this.SaveCmd.IsRunning)
						{
							if (this.Document != null)
							{
								if (this.CanSave)
								{
									MainViewModel.SavingExtraObjects savingExtraObjects = await this.GetSavingExtraObjectsAsync();
									CS$<>8__locals1.savingExtraObjects = savingExtraObjects;
									if (CS$<>8__locals1.savingExtraObjects.HasRedact || CS$<>8__locals1.savingExtraObjects.HasUnembeddedSignature)
									{
										MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
										{
											Title = Resources.IrreversibleSaveAsCopyMessageTitle,
											Content = Resources.IrreversibleSaveAsCopyMessageContent
										}, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
									}
									else
									{
										bool hasDigitalSignature = CS$<>8__locals1.savingExtraObjects.HasDigitalSignature;
									}
									if (this.viewToolbarViewModel.IsDocumentEdited)
									{
										GAManager.SendEvent("TextEditor2", "TextEditing", "Save", 1L);
										await this.viewToolbarViewModel.DocumentEditedSaveAsync(CS$<>8__locals1.savingExtraObjects.HasSavingExtraObjects);
									}
									else
									{
										global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
										CS$<>8__locals1.holders = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
										CS$<>8__locals1.selectedAnnot = CS$<>8__locals1.holders.SelectedAnnotation;
										await this.ReleaseViewerFocusAsync(true);
										MainViewModel.<>c__DisplayClass311_0 CS$<>8__locals2 = CS$<>8__locals1;
										DocumentWrapper documentWrapper = this.DocumentWrapper;
										string text;
										if (documentWrapper == null)
										{
											text = null;
										}
										else
										{
											EncryptManage encryptManage = documentWrapper.EncryptManage;
											text = ((encryptManage != null) ? encryptManage.UserPassword : null);
										}
										CS$<>8__locals2.password = text;
										if (string.IsNullOrEmpty(CS$<>8__locals1.password))
										{
											CS$<>8__locals1.password = null;
										}
										bool flag = !CS$<>8__locals1.savingExtraObjects.HasDigitalSignature || this.CanSave;
										if (this.DocumentWrapper.IsUntitledFile)
										{
											MainViewModel.SaveOptions saveOptions = new MainViewModel.SaveOptions();
											saveOptions.ForceSaveAs = true;
											saveOptions.AllowSaveToCurrentFile = !CS$<>8__locals1.savingExtraObjects.HasSavingExtraObjects;
											saveOptions.InitialFileNamePostfixOverride = (CS$<>8__locals1.savingExtraObjects.HasUnembeddedSignature ? "Signed" : "");
											saveOptions.ShowProgress = true;
											saveOptions.DocumentModified = flag;
											bool flag2;
											if (!this.OperationManager.CanGoBack)
											{
												flag2 = this.ExtraSaveOperationNames.Any((string c) => c != "AddDeferredDigitalSignature");
											}
											else
											{
												flag2 = true;
											}
											saveOptions.RemoveExistsDigitalSignaturesWhenSaveAs = flag2;
											saveOptions.BeforeSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.BeforeSaveActionArgs args)
											{
												MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__2>d <<get_SaveCmd>b__2>d;
												<<get_SaveCmd>b__2>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
												<<get_SaveCmd>b__2>d.<>4__this = CS$<>8__locals1;
												<<get_SaveCmd>b__2>d.<>1__state = -1;
												<<get_SaveCmd>b__2>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__2>d>(ref <<get_SaveCmd>b__2>d);
												return <<get_SaveCmd>b__2>d.<>t__builder.Task;
											};
											saveOptions.AfterSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.SaveResult result)
											{
												MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__3>d <<get_SaveCmd>b__3>d;
												<<get_SaveCmd>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
												<<get_SaveCmd>b__3>d.<>4__this = CS$<>8__locals1;
												<<get_SaveCmd>b__3>d.result = result;
												<<get_SaveCmd>b__3>d.<>1__state = -1;
												<<get_SaveCmd>b__3>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__3>d>(ref <<get_SaveCmd>b__3>d);
												return <<get_SaveCmd>b__3>d.<>t__builder.Task;
											};
											await this.SaveAsync(saveOptions);
										}
										else
										{
											MainViewModel.SaveOptions saveOptions2 = new MainViewModel.SaveOptions();
											saveOptions2.ForceSaveAs = CS$<>8__locals1.savingExtraObjects.HasSavingExtraObjects;
											saveOptions2.SaveAsWhenSaveFailed = true;
											saveOptions2.AllowSaveToCurrentFile = !CS$<>8__locals1.savingExtraObjects.HasSavingExtraObjects;
											saveOptions2.ShowProgress = false;
											bool flag3;
											if (!this.OperationManager.CanGoBack)
											{
												flag3 = this.ExtraSaveOperationNames.Any((string c) => c != "AddDeferredDigitalSignature");
											}
											else
											{
												flag3 = true;
											}
											saveOptions2.RemoveExistsDigitalSignaturesWhenSaveAs = flag3;
											saveOptions2.BeforeSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.BeforeSaveActionArgs args)
											{
												MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__5>d <<get_SaveCmd>b__5>d;
												<<get_SaveCmd>b__5>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
												<<get_SaveCmd>b__5>d.<>4__this = CS$<>8__locals1;
												<<get_SaveCmd>b__5>d.<>1__state = -1;
												<<get_SaveCmd>b__5>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__5>d>(ref <<get_SaveCmd>b__5>d);
												return <<get_SaveCmd>b__5>d.<>t__builder.Task;
											};
											saveOptions2.AfterSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.SaveResult result)
											{
												MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__6>d <<get_SaveCmd>b__6>d;
												<<get_SaveCmd>b__6>d.<>t__builder = AsyncTaskMethodBuilder.Create();
												<<get_SaveCmd>b__6>d.<>4__this = CS$<>8__locals1;
												<<get_SaveCmd>b__6>d.result = result;
												<<get_SaveCmd>b__6>d.<>1__state = -1;
												<<get_SaveCmd>b__6>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass311_0.<<get_SaveCmd>b__6>d>(ref <<get_SaveCmd>b__6>d);
												return <<get_SaveCmd>b__6>d.<>t__builder.Task;
											};
											await this.SaveAsync(saveOptions2);
										}
									}
								}
							}
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x000200FC File Offset: 0x0001E2FC
		public RelayCommand EncryptCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.encryptCMD) == null)
				{
					relayCommand = (this.encryptCMD = new RelayCommand(delegate
					{
						this.DocumentWrapper.ShowEncryptWindow();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x00020130 File Offset: 0x0001E330
		public RelayCommand RemovePasswordCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.removePasswordCMD) == null)
				{
					relayCommand = (this.removePasswordCMD = new RelayCommand(delegate
					{
						this.DocumentWrapper.ShowDecryptWindow();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x00020164 File Offset: 0x0001E364
		public AsyncRelayCommand SaveAsCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.saveAsCmd) == null)
				{
					asyncRelayCommand = (this.saveAsCmd = new AsyncRelayCommand(async delegate
					{
						MainViewModel.<>c__DisplayClass320_0 CS$<>8__locals1 = new MainViewModel.<>c__DisplayClass320_0();
						CS$<>8__locals1.<>4__this = this;
						if (!this.SaveCmd.IsRunning)
						{
							if (this.Document != null)
							{
								if (this.viewToolbarViewModel.IsDocumentEdited)
								{
									GAManager.SendEvent("TextEditor2", "TextEditing", "SaveAs", 1L);
									await this.viewToolbarViewModel.DocumentEditedSaveAsync(true);
								}
								else
								{
									CS$<>8__locals1.savingExtraObjects = await this.GetSavingExtraObjectsAsync();
									MainViewModel.<>c__DisplayClass320_0 CS$<>8__locals2 = CS$<>8__locals1;
									DocumentWrapper documentWrapper = this.DocumentWrapper;
									string text;
									if (documentWrapper == null)
									{
										text = null;
									}
									else
									{
										EncryptManage encryptManage = documentWrapper.EncryptManage;
										text = ((encryptManage != null) ? encryptManage.UserPassword : null);
									}
									CS$<>8__locals2.password = text;
									if (string.IsNullOrEmpty(CS$<>8__locals1.password))
									{
										CS$<>8__locals1.password = null;
									}
									bool flag = !CS$<>8__locals1.savingExtraObjects.HasDigitalSignature || this.CanSave;
									MainViewModel.SaveOptions saveOptions = new MainViewModel.SaveOptions();
									saveOptions.ForceSaveAs = true;
									saveOptions.AllowSaveToCurrentFile = !CS$<>8__locals1.savingExtraObjects.HasSavingExtraObjects;
									saveOptions.InitialFileNamePostfixOverride = (CS$<>8__locals1.savingExtraObjects.HasUnembeddedSignature ? "Signed" : "");
									saveOptions.ShowProgress = true;
									saveOptions.DocumentModified = flag;
									bool flag2;
									if (!this.OperationManager.CanGoBack)
									{
										flag2 = this.ExtraSaveOperationNames.Any((string c) => c != "AddDeferredDigitalSignature");
									}
									else
									{
										flag2 = true;
									}
									saveOptions.RemoveExistsDigitalSignaturesWhenSaveAs = flag2;
									saveOptions.BeforeSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.BeforeSaveActionArgs args)
									{
										MainViewModel.<>c__DisplayClass320_0.<<get_SaveAsCmd>b__3>d <<get_SaveAsCmd>b__3>d;
										<<get_SaveAsCmd>b__3>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
										<<get_SaveAsCmd>b__3>d.<>4__this = CS$<>8__locals1;
										<<get_SaveAsCmd>b__3>d.<>1__state = -1;
										<<get_SaveAsCmd>b__3>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass320_0.<<get_SaveAsCmd>b__3>d>(ref <<get_SaveAsCmd>b__3>d);
										return <<get_SaveAsCmd>b__3>d.<>t__builder.Task;
									};
									saveOptions.AfterSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.SaveResult result)
									{
										MainViewModel.<>c__DisplayClass320_0.<<get_SaveAsCmd>b__4>d <<get_SaveAsCmd>b__4>d;
										<<get_SaveAsCmd>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
										<<get_SaveAsCmd>b__4>d.<>4__this = CS$<>8__locals1;
										<<get_SaveAsCmd>b__4>d.result = result;
										<<get_SaveAsCmd>b__4>d.<>1__state = -1;
										<<get_SaveAsCmd>b__4>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass320_0.<<get_SaveAsCmd>b__4>d>(ref <<get_SaveAsCmd>b__4>d);
										return <<get_SaveAsCmd>b__4>d.<>t__builder.Task;
									};
									await this.SaveAsync(saveOptions);
								}
							}
						}
					}, () => !this.SaveAsCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000201A4 File Offset: 0x0001E3A4
		public async Task SaveFlattenSignature()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
			ProgressUtils.ShowProgressBar(async delegate(ProgressUtils.ProgressAction c)
			{
				Progress<double> progress = new Progress<double>();
				progress.ProgressChanged += delegate(object s, double a)
				{
					c.Report(a);
				};
				await this.annotationToolbarViewModel.ConvertSignatureObj(this.Document, progress);
				c.Complete();
			}, null, Resources.WinSignatureFlattenProcess, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
			await pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken));
			this.MaybeHaveUnembeddedSignature = false;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x000201E8 File Offset: 0x0001E3E8
		private async Task RemoveInvisibleInkAnnotations()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
			if (pdfControl != null)
			{
				AnnotationHolderManager holders = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
				foreach (PdfPage pdfPage in this.Document.Pages)
				{
					if (pdfPage.Annots != null)
					{
						PageDisposeHelper.TryFixPageAnnotations(this.Document, pdfPage.PageIndex);
						foreach (PdfAnnotation pdfAnnotation in pdfPage.Annots)
						{
							PdfInkAnnotation pdfInkAnnotation = pdfAnnotation as PdfInkAnnotation;
							if (pdfInkAnnotation != null)
							{
								PdfInkPointCollection inkList = pdfInkAnnotation.InkList;
								bool flag;
								if (inkList == null)
								{
									flag = true;
								}
								else
								{
									flag = inkList.All((PdfLinePointCollection<PdfInkAnnotation> c) => c.Count < 2);
								}
								if (flag)
								{
									await holders.DeleteAnnotationAsync(pdfInkAnnotation, false);
								}
							}
						}
						IEnumerator<PdfAnnotation> enumerator2 = null;
					}
				}
				IEnumerator<PdfPage> enumerator = null;
				holders = null;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0002022C File Offset: 0x0001E42C
		public AsyncRelayCommand UndoCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.undoCmd) == null)
				{
					asyncRelayCommand = (this.undoCmd = new AsyncRelayCommand(async delegate
					{
						if (!this.UndoCmd.IsRunning)
						{
							await this.ReleaseViewerFocusAsync(true);
							global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
							if (pdfControl != null)
							{
								if (pdfControl != null && pdfControl.CanEditorUndo)
								{
									GAManager.SendEvent("TextEditor2", "TextEditing", "Undo", 1L);
									pdfControl.UndoEditor();
									return;
								}
								if (pdfControl.IsEditing)
								{
									return;
								}
							}
							await this.OperationManager.GoBackAsync();
						}
					}, delegate
					{
						global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.Document);
						if (pdfControl2 == null || !pdfControl2.CanEditorUndo)
						{
							OperationManager operationManager = this.OperationManager;
							return operationManager != null && operationManager.CanGoBack;
						}
						return true;
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0002026C File Offset: 0x0001E46C
		public AsyncRelayCommand RedoCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.redoCmd) == null)
				{
					asyncRelayCommand = (this.redoCmd = new AsyncRelayCommand(async delegate
					{
						if (!this.RedoCmd.IsRunning)
						{
							await this.ReleaseViewerFocusAsync(true);
							global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
							if (pdfControl != null && pdfControl.CanEditorRedo)
							{
								GAManager.SendEvent("TextEditor2", "TextEditing", "Redo", 1L);
								pdfControl.RedoEditor();
							}
							else
							{
								await this.OperationManager.GoForwardAsync();
							}
						}
					}, delegate
					{
						global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.Document);
						if (pdfControl2 == null || !pdfControl2.CanEditorRedo)
						{
							OperationManager operationManager = this.OperationManager;
							return operationManager != null && operationManager.CanGoForward;
						}
						return true;
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x000202AC File Offset: 0x0001E4AC
		private async Task PrintDoc(Source source = Source.Default)
		{
			if (this.Document != null && this.Document.Pages != null)
			{
				GAManager.SendEvent("MainWindow", "PrintBtn", "Count", 1L);
				await this.ReleaseViewerFocusAsync(true);
				this.ExitTransientMode(false, false, false, false, false);
				MainView mainView = Application.Current.Windows.Cast<Window>().FirstOrDefault((Window window) => window is MainView) as MainView;
				if (PrinterSettings.InstalledPrinters.Count != 0 || new NoPrintDetectedWindow(Resources.NoPrintNoteWindowAddBtn)
				{
					Owner = mainView
				}.ShowDialog().GetValueOrDefault())
				{
					new WinPrinterDialog(this, source)
					{
						Owner = mainView
					}.ShowDialog();
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
					await ((pdfControl != null) ? pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken)) : null);
				}
			}
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x000202F8 File Offset: 0x0001E4F8
		public async Task BatchPrintAsync(string source)
		{
			if (this.Document != null && this.Document.Pages != null)
			{
				GAManager.SendEvent("PdfBatchPrintDocument2", "Show", source, 1L);
				await this.ReleaseViewerFocusAsync(true);
				this.ExitTransientMode(false, false, false, false, false);
				MainView mainView = Application.Current.Windows.Cast<Window>().FirstOrDefault((Window window) => window is MainView) as MainView;
				if (PrinterSettings.InstalledPrinters.Count != 0 || new NoPrintDetectedWindow(Resources.NoPrintNoteWindowAddBtn)
				{
					Owner = mainView
				}.ShowDialog().GetValueOrDefault())
				{
					new BatchPrinterListWindow(this.DocumentWrapper.DocumentPath, this.Document)
					{
						Owner = mainView,
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					}.ShowDialog();
				}
			}
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00020343 File Offset: 0x0001E543
		private bool CanPrintDoc()
		{
			return true;
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x00020346 File Offset: 0x0001E546
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x00020350 File Offset: 0x0001E550
		public PdfAnnotation SelectedAnnotation
		{
			get
			{
				return this.selectedAnnotation;
			}
			set
			{
				if (base.SetProperty<PdfAnnotation>(ref this.selectedAnnotation, value, "SelectedAnnotation"))
				{
					if (this.selectedAnnotation != null && this.AnnotationToolbar.InkButtonModel.IsChecked)
					{
						this.AnnotationToolbar.InkButtonModel.IsChecked = false;
					}
					if (this.selectedAnnotation is PdfInkAnnotation)
					{
						(this.AnnotationToolbar.InkButtonModel.ToolbarSettingModel[3] as ToolbarSettingInkEraserModel).IsChecked = false;
					}
					this.AnnotationToolbar.SetMenuItemValue();
					this.DeleteSelectedAnnotCmd.NotifyCanExecuteChanged();
					if (value != null)
					{
						if (MainViewModel.<>o__334.<>p__2 == null)
						{
							MainViewModel.<>o__334.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(MainViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						Func<CallSite, object, bool> target = MainViewModel.<>o__334.<>p__2.Target;
						CallSite <>p__ = MainViewModel.<>o__334.<>p__2;
						if (MainViewModel.<>o__334.<>p__1 == null)
						{
							MainViewModel.<>o__334.<>p__1 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof(MainViewModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						Func<CallSite, object, MouseModes, object> target2 = MainViewModel.<>o__334.<>p__1.Target;
						CallSite <>p__2 = MainViewModel.<>o__334.<>p__1;
						if (MainViewModel.<>o__334.<>p__0 == null)
						{
							MainViewModel.<>o__334.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						if (target(<>p__, target2(<>p__2, MainViewModel.<>o__334.<>p__0.Target(MainViewModel.<>o__334.<>p__0, this.ViewerMouseMode), MouseModes.Default)))
						{
							if (MainViewModel.<>o__334.<>p__3 == null)
							{
								MainViewModel.<>o__334.<>p__3 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(MainViewModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							MainViewModel.<>o__334.<>p__3.Target(MainViewModel.<>o__334.<>p__3, this.ViewerMouseMode, MouseModes.Default);
						}
						this.ExitTransientMode(false, false, false, false, false);
					}
				}
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x00020547 File Offset: 0x0001E747
		// (set) Token: 0x060006B4 RID: 1716 RVA: 0x0002054F File Offset: 0x0001E74F
		public bool IsAnnotationVisible
		{
			get
			{
				return this.isAnnotationVisible;
			}
			set
			{
				base.SetProperty<bool>(ref this.isAnnotationVisible, value, "IsAnnotationVisible");
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x00020564 File Offset: 0x0001E764
		public RelayCommand ExitAnnotationCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.exitAnnotationCmd) == null)
				{
					relayCommand = (this.exitAnnotationCmd = new RelayCommand(delegate
					{
						this.SelectedAnnotation = null;
						this.AnnotationMode = AnnotationMode.None;
						this.ExitTransientMode(false, false, false, false, false);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x00020598 File Offset: 0x0001E798
		public AsyncRelayCommand ShowHideAnnotationCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.showHideAnnotationCmd) == null)
				{
					asyncRelayCommand = (this.showHideAnnotationCmd = new AsyncRelayCommand(async delegate
					{
						if (this.Document != null)
						{
							this.IsAnnotationVisible = !this.IsAnnotationVisible;
							GAManager.SendEvent("MainWindow", "ShowHideAnnotation", "Count", 1L);
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x000205CC File Offset: 0x0001E7CC
		public AsyncRelayCommand MannageAnnotationCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.mannageAnnotationCmd) == null)
				{
					asyncRelayCommand = (this.mannageAnnotationCmd = new AsyncRelayCommand(async delegate
					{
						if (this.Document != null)
						{
							GAManager.SendEvent("MainWindow", "MannageAnnotation", "Count", 1L);
							this.Menus.SelectedLeftNavItem = this.Menus.LeftNavList.First((NavigationModel x) => x.Name == "Annotation");
						}
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x00020600 File Offset: 0x0001E800
		public AsyncRelayCommand DeleteSelectedAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteSelectedAnnotCmd) == null)
				{
					asyncRelayCommand = (this.deleteSelectedAnnotCmd = new AsyncRelayCommand(async delegate
					{
						if (this.Document != null)
						{
							if (!(this.SelectedAnnotation == null))
							{
								global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
								if (pdfControl != null)
								{
									AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
									PdfAnnotation pdfAnnotation = this.SelectedAnnotation;
									if (pdfAnnotation != null)
									{
										PdfFreeTextAnnotation pdfFreeTextAnnotation = pdfAnnotation as PdfFreeTextAnnotation;
										if (pdfFreeTextAnnotation != null && string.IsNullOrEmpty(pdfFreeTextAnnotation.Contents) && pdfFreeTextAnnotation.Intent == AnnotationIntent.FreeTextTypeWriter)
										{
											PdfObjectExtensions.GetAnnotationCanvas(pdfControl).HolderManager.CancelAll();
											return;
										}
									}
									if (annotationHolderManager != null)
									{
										await annotationHolderManager.DeleteAnnotationAsync(this.SelectedAnnotation, false);
									}
								}
							}
						}
					}, () => !this.DeleteSelectedAnnotCmd.IsRunning && this.SelectedAnnotation != null));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0002063D File Offset: 0x0001E83D
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x00020645 File Offset: 0x0001E845
		public bool IsDeleteAreaVisible
		{
			get
			{
				return this.isDeleteAreaVisible;
			}
			set
			{
				base.SetProperty<bool>(ref this.isDeleteAreaVisible, value, "IsDeleteAreaVisible");
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0002065A File Offset: 0x0001E85A
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x00020662 File Offset: 0x0001E862
		public bool? IsSelectedAll
		{
			get
			{
				return this.isSelectedAll;
			}
			set
			{
				base.SetProperty<bool?>(ref this.isSelectedAll, value, "IsSelectedAll");
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x00020678 File Offset: 0x0001E878
		public AsyncRelayCommand CanceldeleteAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.canceldeleteAnnotCmd) == null)
				{
					asyncRelayCommand = (this.canceldeleteAnnotCmd = new AsyncRelayCommand(async delegate
					{
						this.IsDeleteAreaVisible = false;
						PageEditorViewModel pageEditors = this.PageEditors;
						if (pageEditors != null)
						{
							pageEditors.NotifyPageAnnotationChanged(0);
						}
					}, () => !this.CanceldeleteAnnotCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x000206B8 File Offset: 0x0001E8B8
		public AsyncRelayCommand SelectAllAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.selectAllAnnotCmd) == null)
				{
					asyncRelayCommand = (this.selectAllAnnotCmd = new AsyncRelayCommand(async delegate
					{
						bool? flag = this.IsSelectedAll;
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							this.IsSelectedAll = new bool?(false);
							if (this.Document == null)
							{
								return;
							}
							global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
							if (pdfControl == null || PdfObjectExtensions.GetAnnotationHolderManager(pdfControl) == null)
							{
								return;
							}
							using (IEnumerator<PageCommetCollection> enumerator = this.pageCommetSource.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									PageCommetCollection pageCommetCollection = enumerator.Current;
									for (int i = pageCommetCollection.Count - 1; i >= 0; i--)
									{
										if (pageCommetCollection[i].IsChecked)
										{
											pageCommetCollection[i].IsChecked = false;
										}
									}
								}
								return;
							}
						}
						this.IsSelectedAll = new bool?(true);
						if (this.Document != null)
						{
							global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.Document);
							if (pdfControl2 != null && PdfObjectExtensions.GetAnnotationHolderManager(pdfControl2) != null)
							{
								foreach (PageCommetCollection pageCommetCollection2 in this.pageCommetSource)
								{
									for (int j = pageCommetCollection2.Count - 1; j >= 0; j--)
									{
										if (!pageCommetCollection2[j].IsChecked)
										{
											pageCommetCollection2[j].IsChecked = true;
										}
									}
								}
							}
						}
					}, () => !this.SelectAllAnnotCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x000206F8 File Offset: 0x0001E8F8
		public AsyncRelayCommand BatchdeleteAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.batchdeleteAnnotCmd) == null)
				{
					asyncRelayCommand = (this.batchdeleteAnnotCmd = new AsyncRelayCommand(async delegate
					{
						this.IsSelectedAll = new bool?(false);
						PageEditorViewModel pageEditors = this.PageEditors;
						if (pageEditors != null)
						{
							pageEditors.NotifyPageAnnotationChanged(0);
						}
					}, () => !this.BatchdeleteAnnotCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00020738 File Offset: 0x0001E938
		public AsyncRelayCommand DeleteAnnotCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteAnnotCmd) == null)
				{
					asyncRelayCommand = (this.deleteAnnotCmd = new AsyncRelayCommand(async delegate
					{
						if (this.Document != null)
						{
							string text = "DelPart";
							bool? flag = this.IsSelectedAll;
							bool flag2 = false;
							if ((flag.GetValueOrDefault() == flag2) & (flag != null))
							{
								ModernMessageBox.Show(Resources.BatchDeleteSelectNoneWarning, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false);
							}
							else
							{
								if (this.isSelectedAll.GetValueOrDefault())
								{
									text = "DelAll";
								}
								GAManager.SendEvent("AnnotationMgmt", "BatchDeleteBtn", text, 1L);
								global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
								if (pdfControl != null)
								{
									MainViewModel.<>c__DisplayClass370_0 CS$<>8__locals1 = new MainViewModel.<>c__DisplayClass370_0();
									CS$<>8__locals1.<>4__this = this;
									CS$<>8__locals1.holders = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
									if (CS$<>8__locals1.holders != null)
									{
										if (ModernMessageBox.Show(Resources.BatchDeleteWarning, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
										{
											return;
										}
										CS$<>8__locals1.annotations = new List<PdfAnnotation>();
										foreach (PageCommetCollection pageCommetCollection in this.pageCommetSource)
										{
											for (int i = pageCommetCollection.Count - 1; i >= 0; i--)
											{
												if (pageCommetCollection[i].IsChecked)
												{
													CS$<>8__locals1.annotations.Add(this.Document.Pages[pageCommetCollection[i].Annotation.PageIndex].Annots[pageCommetCollection[i].Annotation.AnnotIndex]);
												}
											}
										}
										ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
										{
											MainViewModel.<>c__DisplayClass370_0.<<get_DeleteAnnotCmd>b__2>d <<get_DeleteAnnotCmd>b__2>d;
											<<get_DeleteAnnotCmd>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
											<<get_DeleteAnnotCmd>b__2>d.<>4__this = CS$<>8__locals1;
											<<get_DeleteAnnotCmd>b__2>d.c = c;
											<<get_DeleteAnnotCmd>b__2>d.<>1__state = -1;
											<<get_DeleteAnnotCmd>b__2>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass370_0.<<get_DeleteAnnotCmd>b__2>d>(ref <<get_DeleteAnnotCmd>b__2>d);
											return <<get_DeleteAnnotCmd>b__2>d.<>t__builder.Task;
										}, null, Resources.BatchDeletingTitle, true, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 100);
										this.pageCommetSource.NotifyDeletePageAnnotationChanged();
										this.attachmentSource.NotifyAttachmentChanged();
									}
								}
								if (this.IsSelectedAll.GetValueOrDefault())
								{
									this.IsDeleteAreaVisible = false;
								}
								this.IsSelectedAll = new bool?(false);
							}
						}
					}, () => !this.DeleteAnnotCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00020778 File Offset: 0x0001E978
		public AsyncRelayCommand<PDFAttachmentWrapper> OpenAttachmentCmd
		{
			get
			{
				AsyncRelayCommand<PDFAttachmentWrapper> asyncRelayCommand;
				if ((asyncRelayCommand = this.openAttachmentCmd) == null)
				{
					asyncRelayCommand = (this.openAttachmentCmd = new AsyncRelayCommand<PDFAttachmentWrapper>(async delegate([Nullable(2)] PDFAttachmentWrapper c)
					{
						try
						{
							if (c.Attachment != null)
							{
								GAManager.SendEvent("PDFAttachment", "Open", "LeftPanel", 1L);
								if (ModernMessageBox.Show(Resources.AnnotationFileAttachmentOpenWarning, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
								{
									bool flag = false;
									PdfAttachment pdfAttachment = c.Attachment as PdfAttachment;
									if (pdfAttachment != null)
									{
										flag = await AttachmentFileUtils.OpenFileSpecAsync(pdfAttachment.FileSpecification);
									}
									else
									{
										PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = c.Attachment as PdfFileAttachmentAnnotation;
										if (pdfFileAttachmentAnnotation != null)
										{
											flag = await AttachmentFileUtils.OpenAttachmentFromAnnotation(pdfFileAttachmentAnnotation);
										}
									}
									if (!flag)
									{
										ModernMessageBox.Show(Resources.MsgAttachmentOpenFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
									}
								}
							}
						}
						catch (Exception)
						{
							ModernMessageBox.Show(Resources.MsgAttachmentOpenFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
					}, (PDFAttachmentWrapper c) => !this.OpenAttachmentCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x000207C8 File Offset: 0x0001E9C8
		public AsyncRelayCommand AddAttachmentCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.addAttachmentCmd) == null)
				{
					asyncRelayCommand = (this.addAttachmentCmd = new AsyncRelayCommand(async delegate
					{
						MainViewModel.<>c__DisplayClass376_0 CS$<>8__locals1 = new MainViewModel.<>c__DisplayClass376_0();
						CS$<>8__locals1.<>4__this = this;
						CS$<>8__locals1.openFileDialog = new OpenFileDialog
						{
							Multiselect = true,
							ShowReadOnly = false,
							ReadOnlyChecked = true
						};
						GAManager.SendEvent("PDFAttachment", "Add", "LeftPanel", 1L);
						if (CS$<>8__locals1.openFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
						{
							ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction c)
							{
								MainViewModel.<>c__DisplayClass376_0.<<get_AddAttachmentCmd>b__2>d <<get_AddAttachmentCmd>b__2>d;
								<<get_AddAttachmentCmd>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<get_AddAttachmentCmd>b__2>d.<>4__this = CS$<>8__locals1;
								<<get_AddAttachmentCmd>b__2>d.c = c;
								<<get_AddAttachmentCmd>b__2>d.<>1__state = -1;
								<<get_AddAttachmentCmd>b__2>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass376_0.<<get_AddAttachmentCmd>b__2>d>(ref <<get_AddAttachmentCmd>b__2>d);
								return <<get_AddAttachmentCmd>b__2>d.<>t__builder.Task;
							}, null, Resources.AttachmentPanelBtnAddAttachmentsText, false, App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>(), 0);
						}
					}, () => !this.AddAttachmentCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00020808 File Offset: 0x0001EA08
		public AsyncRelayCommand<PDFAttachmentWrapper> EditAttachmentDescriptionCmd
		{
			get
			{
				AsyncRelayCommand<PDFAttachmentWrapper> asyncRelayCommand;
				if ((asyncRelayCommand = this.editAttachmentDescriptionCmd) == null)
				{
					asyncRelayCommand = (this.editAttachmentDescriptionCmd = new AsyncRelayCommand<PDFAttachmentWrapper>(async delegate([Nullable(2)] PDFAttachmentWrapper c)
					{
						GAManager.SendEvent("PDFAttachment", "EditDescription", "Count", 1L);
						new AttachmentDescriptionWindow(this, c).ShowDialog();
					}, (PDFAttachmentWrapper c) => !this.EditAttachmentDescriptionCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00020848 File Offset: 0x0001EA48
		public AsyncRelayCommand<IList<PDFAttachmentWrapper>> SaveAttachmentCmd
		{
			get
			{
				AsyncRelayCommand<IList<PDFAttachmentWrapper>> asyncRelayCommand;
				if ((asyncRelayCommand = this.saveAttachmentCmd) == null)
				{
					asyncRelayCommand = (this.saveAttachmentCmd = new AsyncRelayCommand<IList<PDFAttachmentWrapper>>(async delegate([Nullable(new byte[] { 2, 0 })] IList<PDFAttachmentWrapper> wrapperList)
					{
						GAManager.SendEvent("PDFAttachment", "Save", "LeftPanel", 1L);
						if (wrapperList == null || wrapperList.Count == 0)
						{
							throw new ArgumentException("wrapperList");
						}
						if (wrapperList.Count == 1)
						{
							await this.SaveAttachment(wrapperList[0]);
						}
						else
						{
							await this.SaveAttachments(wrapperList);
						}
					}, ([Nullable(new byte[] { 2, 0 })] IList<PDFAttachmentWrapper> wrapperList) => !this.SaveAttachmentCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00020888 File Offset: 0x0001EA88
		private async Task SaveAttachment(PDFAttachmentWrapper wrapper)
		{
			try
			{
				if (wrapper.Attachment != null)
				{
					PdfAttachment pdfAttachment = wrapper.Attachment as PdfAttachment;
					if (pdfAttachment != null)
					{
						await AttachmentFileUtils.AttachmentSaveAsFileFromPDFAttachment(pdfAttachment, null);
					}
					else
					{
						PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = wrapper.Attachment as PdfFileAttachmentAnnotation;
						if (pdfFileAttachmentAnnotation != null)
						{
							await AttachmentFileUtils.AttachmentSaveAsFileFromAnnotation(pdfFileAttachmentAnnotation, null);
						}
					}
				}
			}
			catch (Exception)
			{
				ModernMessageBox.Show(Resources.MsgAttachmentSaveFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x000208CC File Offset: 0x0001EACC
		private async Task SaveAttachments(IList<PDFAttachmentWrapper> wrapperList)
		{
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true
			};
			if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				string path = commonOpenFileDialog.FileName;
				List<string> sucItems = new List<string>();
				foreach (PDFAttachmentWrapper wrapper in wrapperList)
				{
					try
					{
						if (wrapper.Attachment != null)
						{
							bool flag = false;
							PdfAttachment pdfAttachment = wrapper.Attachment as PdfAttachment;
							if (pdfAttachment != null)
							{
								flag = await AttachmentFileUtils.AttachmentSaveAsFileFromPDFAttachmentWithTargetFolder(pdfAttachment, path, false);
							}
							else
							{
								PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = wrapper.Attachment as PdfFileAttachmentAnnotation;
								if (pdfFileAttachmentAnnotation != null)
								{
									flag = await AttachmentFileUtils.AttachmentSaveAsFileFromAnnotationWithTargetFolder(pdfFileAttachmentAnnotation, path, false);
								}
							}
							if (flag)
							{
								sucItems.Add(wrapper.FileName);
							}
						}
					}
					catch (Exception)
					{
						ModernMessageBox.Show(Resources.MsgAttachmentSaveFailed, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					wrapper = null;
				}
				IEnumerator<PDFAttachmentWrapper> enumerator = null;
				if (sucItems.Count > 0)
				{
					await AttachmentFileUtils.ShowIfOpenFolderAfterSave(path, sucItems.ToArray());
				}
				path = null;
				sucItems = null;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00020910 File Offset: 0x0001EB10
		public AsyncRelayCommand<IList<PDFAttachmentWrapper>> DeleteAttachmentCmd
		{
			get
			{
				AsyncRelayCommand<IList<PDFAttachmentWrapper>> asyncRelayCommand;
				if ((asyncRelayCommand = this.deleteAttachmentCmd) == null)
				{
					asyncRelayCommand = (this.deleteAttachmentCmd = new AsyncRelayCommand<IList<PDFAttachmentWrapper>>(async delegate([Nullable(new byte[] { 2, 0 })] IList<PDFAttachmentWrapper> wrapperList)
					{
						GAManager.SendEvent("PDFAttachment", "Delete", "LeftPanel", 1L);
						if (ModernMessageBox.Show(Resources.Msg_DeleteAttachments, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
							if (vm != null)
							{
								List<PdfAttachment> deletedAttachments = new List<PdfAttachment>();
								List<PdfFileAttachmentAnnotation> deletedPdfAnnotations = new List<PdfFileAttachmentAnnotation>();
								foreach (PDFAttachmentWrapper pdfattachmentWrapper in wrapperList)
								{
									PdfAttachment pdfAttachment = pdfattachmentWrapper.Attachment as PdfAttachment;
									if (pdfAttachment != null)
									{
										deletedAttachments.Add(pdfAttachment);
									}
									else
									{
										PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = pdfattachmentWrapper.Attachment as PdfFileAttachmentAnnotation;
										if (pdfFileAttachmentAnnotation != null)
										{
											deletedPdfAnnotations.Add(pdfFileAttachmentAnnotation);
										}
									}
								}
								if (deletedPdfAnnotations.Count != 0)
								{
									await PdfAnnotationExtensions.WaitForAnnotationGenerateAsync();
								}
								await this.OperationManager.TraceAttachmentRemoveAsync(deletedAttachments, deletedPdfAnnotations, "");
								foreach (PdfAttachment pdfAttachment2 in deletedAttachments)
								{
									this.Document.Attachments.Remove(pdfAttachment2);
								}
								for (int i = 0; i < deletedPdfAnnotations.Count; i++)
								{
									deletedPdfAnnotations[i].DeleteAnnotation();
									PageEditorViewModel pageEditors = vm.PageEditors;
									if (pageEditors != null)
									{
										pageEditors.NotifyPageAnnotationChanged(deletedPdfAnnotations[i].Page.PageIndex);
									}
									await this.Document.Pages[deletedPdfAnnotations[i].Page.PageIndex].TryRedrawPageAsync(default(CancellationToken));
								}
								if (deletedAttachments != null)
								{
									PageEditorViewModel pageEditors2 = vm.PageEditors;
									if (pageEditors2 != null)
									{
										pageEditors2.NotifyAttachmentChanged();
									}
								}
								deletedAttachments = null;
								deletedPdfAnnotations = null;
							}
							vm = null;
						}
					}, ([Nullable(new byte[] { 2, 0 })] IList<PDFAttachmentWrapper> wrapperList) => !this.DeleteAttachmentCmd.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00020950 File Offset: 0x0001EB50
		public async Task<bool> TrySaveBeforeCloseDocumentAsync()
		{
			bool flag;
			if (this.documentClosing)
			{
				flag = false;
			}
			else if (this.Document == null)
			{
				flag = true;
			}
			else
			{
				try
				{
					this.documentClosing = true;
					if (this.CanSave)
					{
						FileInfo fileInfo = new FileInfo(this.DocumentWrapper.DocumentPath);
						MessageBoxResult messageBoxResult = MessageBox.Show((Resources.SaveDocBeforeCloseMsg ?? "").Replace("XXX", fileInfo.Name), "PDFgear", MessageBoxButton.YesNoCancel);
						if (messageBoxResult == MessageBoxResult.Yes)
						{
							if (!this.SaveCmd.IsRunning || this.SaveCmd.ExecutionTask == null)
							{
								await this.SaveCmd.ExecuteAsync(null);
							}
							else
							{
								await this.SaveCmd.ExecutionTask;
							}
							this.DelAutoSaveFile(this.DocumentWrapper.DocumentPath);
							flag = !this.CanSave;
						}
						else if (messageBoxResult == MessageBoxResult.No)
						{
							this.DelAutoSaveFile(this.DocumentWrapper.DocumentPath);
							flag = true;
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = true;
					}
				}
				finally
				{
					this.documentClosing = false;
				}
			}
			return flag;
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00020994 File Offset: 0x0001EB94
		public RelayCommand OpenImgCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openImgCmd) == null)
				{
					relayCommand = (this.openImgCmd = new RelayCommand(delegate
					{
					}, () => true));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x000209F7 File Offset: 0x0001EBF7
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x000209FF File Offset: 0x0001EBFF
		public BookmarkModel SelectedBookmark
		{
			get
			{
				return this.selectedBookmark;
			}
			set
			{
				if (base.SetProperty<BookmarkModel>(ref this.selectedBookmark, value, "SelectedBookmark"))
				{
					this.BookmarkRemoveCommand.NotifyCanExecuteChanged();
				}
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x00020A20 File Offset: 0x0001EC20
		public AsyncRelayCommand<BookmarkModel> BookmarkAddCommand
		{
			get
			{
				AsyncRelayCommand<BookmarkModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.bookmarkAddCommand) == null)
				{
					asyncRelayCommand = (this.bookmarkAddCommand = new AsyncRelayCommand<BookmarkModel>(async delegate([Nullable(2)] BookmarkModel c)
					{
						BookmarkModel bookmarkModel = c;
						if (c == null && (bookmarkModel = this.SelectedBookmark) == null)
						{
							BookmarkModel bookmarkModel2 = this.Bookmarks;
							bookmarkModel = ((bookmarkModel2 != null) ? bookmarkModel2.Children.LastOrDefault<BookmarkModel>() : null);
						}
						BookmarkModel bookmarkModel3 = bookmarkModel;
						BookmarkModel bookmarkModel4 = ((bookmarkModel3 != null) ? bookmarkModel3.Parent : null);
						GAManager.SendEvent("Bookmark", "AddBookmark", "All", 1L);
						BookmarkRenameDialog bookmarkRenameDialog = BookmarkRenameDialog.Create(Resources.NewBookmarkName);
						if (bookmarkRenameDialog.ShowDialog().GetValueOrDefault())
						{
							await this.AddBookmarkAsync(bookmarkModel3, bookmarkModel4, bookmarkRenameDialog.BookmarkTitle, this.SelectedPageIndex, null);
						}
					}, (BookmarkModel c) => !this.BookmarkAddCommand.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x00020A60 File Offset: 0x0001EC60
		public AsyncRelayCommand<BookmarkModel> BookmarkAddCommand2
		{
			get
			{
				AsyncRelayCommand<BookmarkModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.bookmarkAddCommand2) == null)
				{
					asyncRelayCommand = (this.bookmarkAddCommand2 = new AsyncRelayCommand<BookmarkModel>(async delegate([Nullable(2)] BookmarkModel c)
					{
						BookmarkModel bookmarkModel = c;
						if (c == null && (bookmarkModel = this.SelectedBookmark) == null)
						{
							BookmarkModel bookmarkModel2 = this.Bookmarks;
							bookmarkModel = ((bookmarkModel2 != null) ? bookmarkModel2.Children.LastOrDefault<BookmarkModel>() : null);
						}
						BookmarkModel bookmarkModel3 = bookmarkModel;
						BookmarkModel bookmarkModel4 = ((bookmarkModel3 != null) ? bookmarkModel3.Parent : null);
						GAManager.SendEvent("Bookmark", "AddBookmark", "All", 1L);
						BookmarkRenameDialog bookmarkRenameDialog = BookmarkRenameDialog.Create(Resources.NewBookmarkName);
						if (bookmarkRenameDialog.ShowDialog().GetValueOrDefault())
						{
							FS_RECTF selectedDestination = this.GetSelectedDestination();
							await this.AddBookmarkAsync(bookmarkModel3, bookmarkModel4, bookmarkRenameDialog.BookmarkTitle, this.SelectedPageIndex, new FS_RECTF?(selectedDestination));
						}
					}, (BookmarkModel c) => !this.BookmarkAddCommand.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00020AA0 File Offset: 0x0001ECA0
		private FS_RECTF GetSelectedDestination()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
			PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			foreach (TextInfo textInfo in PdfTextMarkupAnnotationUtils.GetTextInfos(this.Document, pdfViewer.SelectInfo, false).ToArray<TextInfo>())
			{
				using (IEnumerator<FS_RECTF> enumerator = PdfTextMarkupAnnotationUtils.GetNormalizedRects(pdfViewer, textInfo, true, true).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			return default(FS_RECTF);
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x00020B40 File Offset: 0x0001ED40
		public AsyncRelayCommand<BookmarkModel> BookmarkAddChildCommand
		{
			get
			{
				AsyncRelayCommand<BookmarkModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.bookmarkAddChildCommand) == null)
				{
					asyncRelayCommand = (this.bookmarkAddChildCommand = new AsyncRelayCommand<BookmarkModel>(async delegate([Nullable(2)] BookmarkModel c)
					{
						if (c != null)
						{
							BookmarkModel bookmarkModel = c.Children.LastOrDefault<BookmarkModel>();
							GAManager.SendEvent("Bookmark", "AddBookmarkChild", "All", 1L);
							BookmarkRenameDialog bookmarkRenameDialog = BookmarkRenameDialog.Create(Resources.NewBookmarkName);
							if (bookmarkRenameDialog.ShowDialog().GetValueOrDefault())
							{
								await this.AddBookmarkAsync(bookmarkModel, c, bookmarkRenameDialog.BookmarkTitle, this.SelectedPageIndex, null);
							}
						}
					}, (BookmarkModel c) => !this.BookmarkAddChildCommand.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x00020B80 File Offset: 0x0001ED80
		public AsyncRelayCommand<BookmarkModel> BookmarkRemoveCommand
		{
			get
			{
				AsyncRelayCommand<BookmarkModel> asyncRelayCommand;
				if ((asyncRelayCommand = this.bookmarkRemoveCommand) == null)
				{
					asyncRelayCommand = (this.bookmarkRemoveCommand = new AsyncRelayCommand<BookmarkModel>(async delegate([Nullable(2)] BookmarkModel c)
					{
						GAManager.SendEvent("Bookmark", "RemoveBookmark", "All", 1L);
						await this.RemoveBookmarkAsync(c ?? this.SelectedBookmark);
					}, (BookmarkModel c) => (c ?? this.SelectedBookmark) != null && !this.BookmarkRemoveCommand.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00020BC0 File Offset: 0x0001EDC0
		private async Task<bool> AddBookmarkAsync(BookmarkModel prev, BookmarkModel parent, string title, int pageIndex, FS_RECTF? destination)
		{
			bool flag;
			if (this.Document == null)
			{
				flag = false;
			}
			else
			{
				if (parent == null)
				{
					BookmarkModel bookmarkModel = prev;
					parent = ((bookmarkModel != null) ? bookmarkModel.Parent : null);
				}
				if (parent == null)
				{
					parent = this.Bookmarks;
				}
				BookmarkModel bookmarkModel2 = prev;
				if (((bookmarkModel2 != null) ? bookmarkModel2.Parent : null) == null)
				{
					prev = null;
				}
				BookmarkModel bookmarkModel3 = prev;
				if (((bookmarkModel3 != null) ? bookmarkModel3.Parent : null) != null && parent != null && prev.Parent != parent)
				{
					flag = false;
				}
				else
				{
					await this.Menus.ShowLeftNavMenuAsync("Bookmark");
					int num = -1;
					if (prev != null)
					{
						for (int i = 0; i < parent.Children.Count; i++)
						{
							if (parent.Children[i] == prev)
							{
								num = i;
								break;
							}
						}
						if (num == -1)
						{
							return false;
						}
					}
					num++;
					BookmarkRecord bookmarkRecord = new BookmarkRecord
					{
						Title = (title ?? ""),
						Index = num,
						Destination = new BookmarkRecord.BookmarkDestination
						{
							DestinationType = DestinationTypes.XYZ,
							PageIndex = pageIndex
						}
					};
					if (destination != null)
					{
						bookmarkRecord.Destination.Left = new float?(destination.Value.left);
						bookmarkRecord.Destination.Top = new float?(destination.Value.top);
						bookmarkRecord.Destination.Right = new float?(destination.Value.right);
						bookmarkRecord.Destination.Bottom = new float?(destination.Value.bottom);
					}
					BookmarkModel result = await this.OperationManager.InsertBookmarkAsync(this.Document, parent, bookmarkRecord, "");
					if (result != null)
					{
						this.SelectedBookmark = null;
						DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
						{
							this.SelectedBookmark = result;
						}));
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00020C30 File Offset: 0x0001EE30
		private async Task<bool> RemoveBookmarkAsync(BookmarkModel model)
		{
			bool flag;
			if (this.Document == null)
			{
				flag = false;
			}
			else if (model == null)
			{
				flag = false;
			}
			else
			{
				BookmarkModel bookmarkModel = this.SelectedBookmark;
				if (model == bookmarkModel)
				{
					this.SelectedBookmark = null;
				}
				if (model.Children.Count > 0 && !ConfigManager.GetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", false))
				{
					MessageBoxHelper.RichMessageBoxResult richMessageBoxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
					{
						Content = Resources.DeleteParentBookmarkWarning,
						ShowLeftBottomCheckbox = true,
						LeftBottomCheckboxContent = Resources.WinPwdPasswordSaveTipNotshowagainContent
					}, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false);
					if (richMessageBoxResult.Result == MessageBoxResult.Yes)
					{
						bool? checkboxResult = richMessageBoxResult.CheckboxResult;
						if (checkboxResult != null && checkboxResult.GetValueOrDefault())
						{
							ConfigManager.SetDoNotShowFlag("NotShowDigitalSignatureCreateTipFlag", true);
						}
					}
					if (richMessageBoxResult.Result != MessageBoxResult.Yes)
					{
						return false;
					}
				}
				TaskAwaiter<bool> taskAwaiter = this.OperationManager.RemoveBookmarkAsync(this.Document, model, "").GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00020C7C File Offset: 0x0001EE7C
		private BookmarkModel GetNextBookmark(BookmarkModel model)
		{
			List<BookmarkModel> list = model.Parent.Children.ToList<BookmarkModel>();
			int num = list.IndexOf(model);
			if (num == -1)
			{
				return null;
			}
			if (num > 0)
			{
				return list[num - 1];
			}
			if (!model.IsTopLevelModel && model.Parent != null)
			{
				return model.Parent;
			}
			return null;
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00020CD0 File Offset: 0x0001EED0
		public void DelAutoSaveFile(string filePath)
		{
			try
			{
				CommonLib.Common.AutoSaveManager.DelTempFileByCloseExe(Process.GetCurrentProcess().Id, filePath);
			}
			catch (Exception)
			{
				throw;
			}
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00020D04 File Offset: 0x0001EF04
		public static void RotatePageCore(PdfDocument doc, IEnumerable<int> pageIdxs, bool rotateRight)
		{
			MainViewModel.RotatePageCoreAsync(doc, pageIdxs, rotateRight, null);
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00020D10 File Offset: 0x0001EF10
		public static async Task RotatePageCoreAsync(PdfDocument doc, IEnumerable<int> pageIdxs, bool rotateRight, IProgress<double> progress = null)
		{
			int count = pageIdxs.Count<int>();
			int progressIdx = 0;
			if (progress != null)
			{
				progress.Report(0.0);
			}
			foreach (int num in pageIdxs)
			{
				if (num >= 0 && num <= doc.Pages.Count)
				{
					PdfPage pdfPage = doc.Pages[num];
					PageRotate pageRotate = pdfPage.Rotation;
					if (rotateRight)
					{
						if (pageRotate < PageRotate.Rotate270)
						{
							pageRotate++;
						}
						else
						{
							pageRotate = PageRotate.Normal;
						}
					}
					else if (pageRotate > PageRotate.Normal)
					{
						pageRotate--;
					}
					else
					{
						pageRotate = PageRotate.Rotate270;
					}
					pdfPage.Rotation = pageRotate;
					StrongReferenceMessenger.Default.Send<ValueChangedMessage<int>, string>(new ValueChangedMessage<int>(num), "MESSAGE_PAGE_ROTATE_CHANGED");
					int num2 = progressIdx;
					progressIdx = num2 + 1;
					if (count > 40 && progress != null && progressIdx % 10 == 0)
					{
						await Task.Yield();
					}
					if (progress != null)
					{
						progress.Report((double)progressIdx * 1.0 / (double)count);
					}
				}
			}
			IEnumerator<int> enumerator = null;
			if (progress != null)
			{
				progress.Report(1.0);
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00020D6C File Offset: 0x0001EF6C
		public void ExitTransientMode(bool fromShotScreen = false, bool fromEditText = false, bool fromAutoScroll = false, bool fromRedact = false, bool fromLink = false)
		{
			DataOperationModel dataOperationModel = this.ViewerOperationModel;
			if (dataOperationModel != null)
			{
				if (!dataOperationModel.IsDisposed)
				{
					dataOperationModel.Dispose();
				}
				this.ViewerOperationModel = null;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl);
			if (!fromShotScreen && annotationCanvas != null)
			{
				annotationCanvas.CloseScreenShot();
			}
			if (!fromEditText)
			{
				this.EditingPageObjectType = PageObjectType.None;
				ViewToolbarViewModel viewToolbarViewModel = this.viewToolbarViewModel;
				if (((viewToolbarViewModel != null) ? viewToolbarViewModel.EditPageTextObjectButtonModel : null) != null)
				{
					this.viewToolbarViewModel.EditPageTextObjectButtonModel.IsChecked = false;
				}
			}
			if (!fromAutoScroll)
			{
				ViewToolbarViewModel viewToolbarViewModel2 = this.viewToolbarViewModel;
				if (viewToolbarViewModel2 != null)
				{
					viewToolbarViewModel2.StopAutoScroll();
				}
			}
			if (!fromRedact && this.AnnotationToolbar != null && this.AnnotationToolbar.RedactionButtonModel.IsChecked)
			{
				this.AnnotationToolbar.RedactionButtonModel.IsChecked = false;
				this.AnnotationToolbar.NotifyPropertyChanged("CheckedButtonToolbarSetting");
				DataOperationModel dataOperationModel2 = this.ViewerOperationModel;
				if (dataOperationModel2 != null)
				{
					dataOperationModel2.Dispose();
				}
				this.ViewerOperationModel = null;
			}
			if (annotationCanvas != null && annotationCanvas.ImageControl.Visibility != Visibility.Collapsed)
			{
				annotationCanvas.ImageControl.Visibility = Visibility.Collapsed;
				annotationCanvas.ImageControl.quitImageControl();
			}
			if (!fromLink && pdfControl != null)
			{
				pdfControl.Viewer.IsLinkAnnotationHighlighted = false;
				if (!this.annotationToolbarViewModel.LinkButtonModel.IsChecked && this.selectedAnnotation is PdfLinkAnnotation)
				{
					this.ReleaseViewerFocusAsync(true);
				}
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00020EB3 File Offset: 0x0001F0B3
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00020EBB File Offset: 0x0001F0BB
		public bool ChatPanelState
		{
			get
			{
				return this.chatPanelState;
			}
			set
			{
				base.SetProperty<bool>(ref this.chatPanelState, value, "ChatPanelState");
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x00020ED0 File Offset: 0x0001F0D0
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x00020ED8 File Offset: 0x0001F0D8
		public bool ChatButtonVisible
		{
			get
			{
				return this.chatButtonVisible;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.chatButtonVisible, value, "ChatButtonVisible"))
				{
					base.OnPropertyChanged("ChatButtonActualVisible");
				}
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00020EF9 File Offset: 0x0001F0F9
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x00020F04 File Offset: 0x0001F104
		public bool ChatPanelVisible
		{
			get
			{
				return this.chatPanelVisible;
			}
			set
			{
				if (value)
				{
					if (this.TranslatePanelVisible)
					{
						this.TranslatePanelVisible = false;
						this.TranslatePanelState = true;
					}
					else
					{
						this.TranslatePanelState = false;
					}
				}
				if (base.SetProperty<bool>(ref this.chatPanelVisible, value, "ChatPanelVisible"))
				{
					base.OnPropertyChanged("ChatButtonActualVisible");
				}
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00020F52 File Offset: 0x0001F152
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x00020F5A File Offset: 0x0001F15A
		public bool TranslatePanelState
		{
			get
			{
				return this.translatePanelState;
			}
			set
			{
				base.SetProperty<bool>(ref this.translatePanelState, value, "TranslatePanelState");
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x00020F6F File Offset: 0x0001F16F
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x00020F77 File Offset: 0x0001F177
		public bool TranslatePanelVisible
		{
			get
			{
				return this.translatePanelVisible;
			}
			set
			{
				if (value)
				{
					if (this.ChatPanelVisible)
					{
						this.ChatPanelState = true;
						this.ChatPanelVisible = false;
					}
					else
					{
						this.ChatPanelState = false;
					}
				}
				base.SetProperty<bool>(ref this.translatePanelVisible, value, "TranslatePanelVisible");
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x00020FAE File Offset: 0x0001F1AE
		public bool ChatButtonActualVisible
		{
			get
			{
				return this.ChatButtonVisible && !this.ChatPanelVisible;
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00020FC3 File Offset: 0x0001F1C3
		public ObservableCollection<string> ZoomOptions { get; } = new ObservableCollection<string>
		{
			"1%",
			"25%",
			"50%",
			"75%",
			"100%",
			"125%",
			"150%",
			"200%",
			"250%",
			"300%",
			"450%",
			"600%",
			"800%",
			"1200%",
			"1600%",
			"2400%",
			"3200%",
			"6400%",
			"-------------",
			Resources.ShortcutTextFullSize,
			Resources.ShortcutTextFitPage,
			Resources.ShortcutTextFitWidth,
			Resources.ShortcutTextFitHeight
		};

		// Token: 0x060006E4 RID: 1764 RVA: 0x00020FCC File Offset: 0x0001F1CC
		public async Task CreatePdfFileAsync(CreatePdfFileType type)
		{
			string text = "";
			if (type != CreatePdfFileType.Blank)
			{
				if (type != CreatePdfFileType.FromScanner)
				{
					throw new ArgumentException("Not This Arguemnt");
				}
				new InsertPageFromScanner(null, this.Document, false, true, this)
				{
					Owner = Application.Current.MainWindow
				}.ShowDialog();
			}
			else
			{
				text = CreateFileHelper.CreateBlankPageAsync();
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (this.Document != null)
				{
					CreateFileHelper.OpenPDFFile(text, "open:CreatedFile");
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter = this.OpenDocumentCoreAsync(text, null, false).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						this.DocumentWrapper.SetUntitledFile();
						this.SetCanSaveFlag("CreateNew", false);
						pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().LastOperationVersion = "CreateNew";
					}
				}
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00021018 File Offset: 0x0001F218
		public async Task<MainViewModel.SavingExtraObjects> GetSavingExtraObjectsAsync()
		{
			bool hasSignature = this.MaybeHaveUnembeddedSignature && this.HasUnembeddedSignature();
			DocumentWrapper documentWrapper = this.DocumentWrapper;
			bool? flag;
			if (documentWrapper == null)
			{
				flag = null;
			}
			else
			{
				DigitalSignatureHelper digitalSignatureHelper = documentWrapper.DigitalSignatureHelper;
				if (digitalSignatureHelper == null)
				{
					flag = null;
				}
				else
				{
					global::System.Collections.Generic.IReadOnlyList<PdfDigitalSignatureLocation> locations = digitalSignatureHelper.Locations;
					if (locations == null)
					{
						flag = null;
					}
					else
					{
						flag = new bool?(locations.Any((PdfDigitalSignatureLocation c) => c.HasSigned));
					}
				}
			}
			bool hasDigSign = flag ?? false;
			bool flag2 = await this.OperationManager.ContainsTagAsync("Redact", OperationManagerEntryType.GoBack);
			DocumentWrapper documentWrapper2 = this.DocumentWrapper;
			bool? flag3;
			if (documentWrapper2 == null)
			{
				flag3 = null;
			}
			else
			{
				DigitalSignatureHelper digitalSignatureHelper2 = documentWrapper2.DigitalSignatureHelper;
				if (digitalSignatureHelper2 == null)
				{
					flag3 = null;
				}
				else
				{
					global::System.Collections.Generic.IReadOnlyList<PdfDigitalSignatureLocation> locations2 = digitalSignatureHelper2.Locations;
					if (locations2 == null)
					{
						flag3 = null;
					}
					else
					{
						flag3 = new bool?(locations2.Any((PdfDigitalSignatureLocation c) => !c.HasSigned));
					}
				}
			}
			bool flag4 = flag3 ?? false;
			return new MainViewModel.SavingExtraObjects((hasSignature ? MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.UnembeddedSignature : MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None) | (hasDigSign ? MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.DigitalSignature : MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None) | (flag2 ? MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.Redact : MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None) | (flag4 ? MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.DeferredDigitalSignature : MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None));
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0002105C File Offset: 0x0001F25C
		public bool HasUnembeddedSignature()
		{
			PdfDocument document = this.Document;
			if (document == null)
			{
				return false;
			}
			for (int i = 0; i < document.Pages.Count; i++)
			{
				IntPtr zero = IntPtr.Zero;
				PdfTypeDictionary pdfTypeDictionary = null;
				try
				{
					pdfTypeDictionary = PdfTypeDictionary.Create(Pdfium.FPDF_GetPageDictionary(document.Handle, i));
					if (pdfTypeDictionary.ContainsKey("Annots") && pdfTypeDictionary["Annots"].Is<PdfTypeArray>())
					{
						PdfTypeArray pdfTypeArray = pdfTypeDictionary["Annots"].As<PdfTypeArray>(true);
						for (int j = 0; j < pdfTypeArray.Count; j++)
						{
							if (pdfTypeArray[j].Is<PdfTypeDictionary>())
							{
								PdfTypeDictionary pdfTypeDictionary2 = pdfTypeArray[j].As<PdfTypeDictionary>(true);
								if (pdfTypeDictionary2.ContainsKey("Type") && pdfTypeDictionary2["Type"].Is<PdfTypeName>() && pdfTypeDictionary2["Type"].As<PdfTypeName>(true).Value == "Annot" && pdfTypeDictionary2.ContainsKey("Subtype") && pdfTypeDictionary2["Subtype"].Is<PdfTypeName>() && pdfTypeDictionary2["Subtype"].As<PdfTypeName>(true).Value == "Stamp" && pdfTypeDictionary2.ContainsKey("Subj") && pdfTypeDictionary2["Subj"].Is<PdfTypeString>() && pdfTypeDictionary2["Subj"].As<PdfTypeString>(true).UnicodeString == "Signature")
								{
									return true;
								}
							}
						}
					}
				}
				catch
				{
				}
				finally
				{
					if (pdfTypeDictionary != null)
					{
						pdfTypeDictionary.Dispose();
					}
				}
			}
			return false;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00021240 File Offset: 0x0001F440
		public async Task<MainViewModel.SaveResult> SaveAsync(MainViewModel.SaveOptions options)
		{
			return await this.SaveCore(options);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0002128C File Offset: 0x0001F48C
		private async Task<MainViewModel.SaveResult> SaveCore(MainViewModel.SaveOptions options)
		{
			MainViewModel.<>c__DisplayClass446_0 CS$<>8__locals1 = new MainViewModel.<>c__DisplayClass446_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.options = options;
			MainViewModel.SaveResult saveResult;
			if (CS$<>8__locals1.options.ShowProgress)
			{
				MainViewModel.<>c__DisplayClass446_1 CS$<>8__locals2 = new MainViewModel.<>c__DisplayClass446_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.result = null;
				ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction action)
				{
					MainViewModel.<>c__DisplayClass446_1.<<SaveCore>b__4>d <<SaveCore>b__4>d;
					<<SaveCore>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<SaveCore>b__4>d.<>4__this = CS$<>8__locals2;
					<<SaveCore>b__4>d.action = action;
					<<SaveCore>b__4>d.<>1__state = -1;
					<<SaveCore>b__4>d.<>t__builder.Start<MainViewModel.<>c__DisplayClass446_1.<<SaveCore>b__4>d>(ref <<SaveCore>b__4>d);
					return <<SaveCore>b__4>d.<>t__builder.Task;
				}, "", null, false, App.Current.MainWindow, (int)CS$<>8__locals2.CS$<>8__locals1.options.ProgressDelayTime.TotalMilliseconds);
				if (CS$<>8__locals2.result == null)
				{
					CS$<>8__locals2.result = new MainViewModel.SaveResult(MainViewModel.SaveFailedResult.Unknown);
				}
				saveResult = CS$<>8__locals2.result;
			}
			else
			{
				saveResult = await CS$<>8__locals1.<SaveCore>g__Run|0().ConfigureAwait(false);
			}
			return saveResult;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002140B File Offset: 0x0001F60B
		[CompilerGenerated]
		private void <SetCanSaveFlag>g__AddOperation|124_0(string _operation)
		{
			if (this.extraSaveOperationNames == null)
			{
				this.extraSaveOperationNames = new List<string> { _operation };
				return;
			}
			if (this.extraSaveOperationNames.IndexOf(_operation) == -1)
			{
				this.extraSaveOperationNames.Add(_operation);
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00021E74 File Offset: 0x00020074
		[CompilerGenerated]
		internal static async Task<bool> <SaveCore>g__BeforeSave|446_2(MainViewModel.SaveOptions _options, MainViewModel.BeforeSaveActionArgs args)
		{
			bool flag;
			if (((_options != null) ? _options.BeforeSaveAction : null) != null)
			{
				flag = await _options.BeforeSaveAction(_options, args);
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00021EC0 File Offset: 0x000200C0
		[CompilerGenerated]
		internal static async Task<MainViewModel.SaveResult> <SaveCore>g__AfterSave|446_3(MainViewModel.SaveOptions _options, MainViewModel.SaveResult _result)
		{
			if (_result == null)
			{
				throw new ArgumentException(null, "_result");
			}
			if (((_options != null) ? _options.AfterSaveAction : null) != null)
			{
				await _options.AfterSaveAction(_options, _result);
			}
			return _result;
		}

		// Token: 0x04000329 RID: 809
		private DocumentWrapper documentWrapper;

		// Token: 0x0400032A RID: 810
		private List<PdfThumbnailModel> thumbnailItemSource;

		// Token: 0x0400032B RID: 811
		private BookmarkModel bookmarks;

		// Token: 0x0400032C RID: 812
		private ObservableCollection<NavigationModel> leftNavList;

		// Token: 0x0400032D RID: 813
		private int selectedPageIndex;

		// Token: 0x0400032E RID: 814
		private DateTime time = DateTime.Now;

		// Token: 0x0400032F RID: 815
		public SpeechUtils speechUtils;

		// Token: 0x04000330 RID: 816
		public bool IsReading;

		// Token: 0x04000331 RID: 817
		public SpeechControl speechControl;

		// Token: 0x04000332 RID: 818
		private bool fileBtnIsChecked;

		// Token: 0x04000333 RID: 819
		public int ReadCulIndex = -1;

		// Token: 0x04000334 RID: 820
		public PdfPage LastViewPage;

		// Token: 0x04000335 RID: 821
		private bool translateWords;

		// Token: 0x04000336 RID: 822
		public SoundPlayer m_SoundPlayer;

		// Token: 0x04000337 RID: 823
		public pdfeditor.AutoSaveRestore.AutoSaveModel AutoSaveModel;

		// Token: 0x04000339 RID: 825
		public const string AppTitle = "PDFgear";

		// Token: 0x0400033A RID: 826
		public string CurrentFileName = string.Empty;

		// Token: 0x0400033B RID: 827
		public bool Jumping;

		// Token: 0x0400033C RID: 828
		private QuickToolModel quickToolOpenModel;

		// Token: 0x0400033D RID: 829
		private QuickToolModel quickToolSaveModel;

		// Token: 0x0400033E RID: 830
		private QuickToolModel quickToolSaveAsModel;

		// Token: 0x0400033F RID: 831
		private QuickToolModel quickToolPrintModel;

		// Token: 0x04000340 RID: 832
		private QuickToolModel quickToolUndoModel;

		// Token: 0x04000341 RID: 833
		private QuickToolModel quickToolRedoModel;

		// Token: 0x04000342 RID: 834
		private DataOperationModel viewerOperationModel;

		// Token: 0x04000343 RID: 835
		private PdfViewJumpManager viewJumpManager = new PdfViewJumpManager();

		// Token: 0x04000344 RID: 836
		private ConverterCommands converterCmds;

		// Token: 0x04000345 RID: 837
		private ViewToolbarViewModel viewToolbarViewModel;

		// Token: 0x04000346 RID: 838
		private AnnotationToolbarViewModel annotationToolbarViewModel;

		// Token: 0x04000347 RID: 839
		private PageEditorViewModel pageEditorViewModel;

		// Token: 0x04000348 RID: 840
		private MenuViewModel menus;

		// Token: 0x04000349 RID: 841
		private ShareTabViewModel shareTabViewModel;

		// Token: 0x0400034A RID: 842
		private PageObjectType editingPageObjectType;

		// Token: 0x0400034B RID: 843
		private TranslateViewModel translateViewModel;

		// Token: 0x0400034C RID: 844
		private PdfAnnotation selectedAnnotation;

		// Token: 0x0400034D RID: 845
		private EnumBindingObject<MouseModes> viewerMouseMode = new EnumBindingObject<MouseModes>(MouseModes.Default);

		// Token: 0x0400034E RID: 846
		private EnumBindingObject<EditorMouseModes> editorMouseMode = new EnumBindingObject<EditorMouseModes>(EditorMouseModes.SelectParagraph);

		// Token: 0x04000350 RID: 848
		private AsyncRelayCommand openStartUpFileCmd;

		// Token: 0x04000351 RID: 849
		private List<string> extraSaveOperationNames;

		// Token: 0x04000352 RID: 850
		private OperationManager operationManager;

		// Token: 0x04000353 RID: 851
		private string version;

		// Token: 0x04000354 RID: 852
		private bool canSave;

		// Token: 0x04000355 RID: 853
		private string _password = "";

		// Token: 0x04000356 RID: 854
		private double thumbnailScale = 1.0;

		// Token: 0x04000358 RID: 856
		private AsyncRelayCommand thumbnailZoonOut;

		// Token: 0x04000359 RID: 857
		private AsyncRelayCommand thumbnailZoonIn;

		// Token: 0x0400035A RID: 858
		public static float ScaleStep = 0.1f;

		// Token: 0x0400035B RID: 859
		public static float MinScale = 0.5f;

		// Token: 0x0400035C RID: 860
		public static float MaxScale = 3f;

		// Token: 0x0400035D RID: 861
		public static float eps = 1E-06f;

		// Token: 0x0400035E RID: 862
		private AllPageCommetCollectionView pageCommetSource;

		// Token: 0x0400035F RID: 863
		private AttachmentWrappersCollection attachmentSource;

		// Token: 0x04000360 RID: 864
		private CopilotHelper copilotHelper;

		// Token: 0x04000361 RID: 865
		private Thickness pdfToWordMargin = new Thickness(0.0, 0.0, 12.0, 12.0);

		// Token: 0x04000362 RID: 866
		private bool isDocumentOpened;

		// Token: 0x04000363 RID: 867
		private bool fillForm;

		// Token: 0x04000364 RID: 868
		private AllPageCommetCollectionView allpageCommetSource;

		// Token: 0x04000365 RID: 869
		private bool? isUserFilterAllChecked = new bool?(true);

		// Token: 0x04000366 RID: 870
		private UserInfoModel userInfoModel = UserStore.GetUserInfoModel();

		// Token: 0x04000367 RID: 871
		private int allCount;

		// Token: 0x04000368 RID: 872
		private int attachmentAllCount;

		// Token: 0x04000369 RID: 873
		private bool? isFilterAllChecked = new bool?(true);

		// Token: 0x0400036A RID: 874
		private bool? isKindFilterAllChecked = new bool?(true);

		// Token: 0x0400036B RID: 875
		private RelayCommand userMannulCmd;

		// Token: 0x0400036C RID: 876
		private RelayCommand userGuideCmd;

		// Token: 0x0400036D RID: 877
		private RelayCommand getPhoneStoreCmd;

		// Token: 0x0400036E RID: 878
		private RelayCommand feedBackCmd;

		// Token: 0x0400036F RID: 879
		private RelayCommand autoSaveSettingCmd;

		// Token: 0x04000370 RID: 880
		private RelayCommand upgradeAppCmd;

		// Token: 0x04000371 RID: 881
		private RelayCommand upgradeAICmd;

		// Token: 0x04000372 RID: 882
		private RelayCommand aboutCmd;

		// Token: 0x04000373 RID: 883
		private RelayCommand propertiesCmd;

		// Token: 0x04000374 RID: 884
		private RelayCommand updateCmd;

		// Token: 0x04000375 RID: 885
		private AsyncRelayCommand settingsCmd;

		// Token: 0x04000376 RID: 886
		private AsyncRelayCommand shareFileMailCmd;

		// Token: 0x04000377 RID: 887
		private AsyncRelayCommand shareFileCmd;

		// Token: 0x04000378 RID: 888
		private AsyncRelayCommand printDocCmd;

		// Token: 0x04000379 RID: 889
		private AsyncRelayCommand printDocCmdFromThumbnail;

		// Token: 0x0400037A RID: 890
		private AsyncRelayCommand batchPrintCmd;

		// Token: 0x0400037B RID: 891
		private AsyncRelayCommand openDocCmd;

		// Token: 0x0400037C RID: 892
		private AsyncRelayCommand saveCmd;

		// Token: 0x0400037D RID: 893
		private RelayCommand encryptCMD;

		// Token: 0x0400037E RID: 894
		private RelayCommand removePasswordCMD;

		// Token: 0x0400037F RID: 895
		private AsyncRelayCommand saveAsCmd;

		// Token: 0x04000380 RID: 896
		private AsyncRelayCommand undoCmd;

		// Token: 0x04000381 RID: 897
		private AsyncRelayCommand redoCmd;

		// Token: 0x04000382 RID: 898
		private bool isAnnotationVisible = true;

		// Token: 0x04000383 RID: 899
		private RelayCommand exitAnnotationCmd;

		// Token: 0x04000384 RID: 900
		private AsyncRelayCommand showHideAnnotationCmd;

		// Token: 0x04000385 RID: 901
		private AsyncRelayCommand mannageAnnotationCmd;

		// Token: 0x04000386 RID: 902
		private AsyncRelayCommand deleteSelectedAnnotCmd;

		// Token: 0x04000387 RID: 903
		private bool isDeleteAreaVisible;

		// Token: 0x04000388 RID: 904
		private bool? isSelectedAll = new bool?(false);

		// Token: 0x04000389 RID: 905
		private AsyncRelayCommand canceldeleteAnnotCmd;

		// Token: 0x0400038A RID: 906
		private AsyncRelayCommand selectAllAnnotCmd;

		// Token: 0x0400038B RID: 907
		private AsyncRelayCommand batchdeleteAnnotCmd;

		// Token: 0x0400038C RID: 908
		private AsyncRelayCommand deleteAnnotCmd;

		// Token: 0x0400038D RID: 909
		private AsyncRelayCommand<PDFAttachmentWrapper> openAttachmentCmd;

		// Token: 0x0400038E RID: 910
		private AsyncRelayCommand addAttachmentCmd;

		// Token: 0x0400038F RID: 911
		private AsyncRelayCommand<PDFAttachmentWrapper> editAttachmentDescriptionCmd;

		// Token: 0x04000390 RID: 912
		private AsyncRelayCommand<IList<PDFAttachmentWrapper>> saveAttachmentCmd;

		// Token: 0x04000391 RID: 913
		private AsyncRelayCommand<IList<PDFAttachmentWrapper>> deleteAttachmentCmd;

		// Token: 0x04000392 RID: 914
		private bool documentClosing;

		// Token: 0x04000393 RID: 915
		private RelayCommand openImgCmd;

		// Token: 0x04000394 RID: 916
		private AsyncRelayCommand<BookmarkModel> bookmarkAddCommand;

		// Token: 0x04000395 RID: 917
		private AsyncRelayCommand<BookmarkModel> bookmarkAddCommand2;

		// Token: 0x04000396 RID: 918
		private AsyncRelayCommand<BookmarkModel> bookmarkAddChildCommand;

		// Token: 0x04000397 RID: 919
		private AsyncRelayCommand<BookmarkModel> bookmarkRemoveCommand;

		// Token: 0x04000398 RID: 920
		private BookmarkModel selectedBookmark;

		// Token: 0x04000399 RID: 921
		private bool chatButtonVisible;

		// Token: 0x0400039A RID: 922
		private bool chatPanelVisible;

		// Token: 0x0400039B RID: 923
		private bool translatePanelVisible;

		// Token: 0x0400039C RID: 924
		private bool translatePanelState;

		// Token: 0x0400039D RID: 925
		private bool chatPanelState;

		// Token: 0x02000366 RID: 870
		public enum SelectReadPages
		{
			// Token: 0x04001436 RID: 5174
			CurrentPage,
			// Token: 0x04001437 RID: 5175
			FormCurrentPage,
			// Token: 0x04001438 RID: 5176
			AllPage
		}

		// Token: 0x02000367 RID: 871
		public class BeforeSaveActionArgs
		{
			// Token: 0x06002A53 RID: 10835 RVA: 0x000CB906 File Offset: 0x000C9B06
			public BeforeSaveActionArgs(string filePath, bool saveToCurrentFile)
			{
				this.FilePath = filePath;
				this.SaveToCurrentFile = saveToCurrentFile;
			}

			// Token: 0x17000C6C RID: 3180
			// (get) Token: 0x06002A54 RID: 10836 RVA: 0x000CB91C File Offset: 0x000C9B1C
			public string FilePath { get; }

			// Token: 0x17000C6D RID: 3181
			// (get) Token: 0x06002A55 RID: 10837 RVA: 0x000CB924 File Offset: 0x000C9B24
			public bool SaveToCurrentFile { get; }
		}

		// Token: 0x02000368 RID: 872
		public class SaveOptions
		{
			// Token: 0x17000C6E RID: 3182
			// (get) Token: 0x06002A56 RID: 10838 RVA: 0x000CB92C File Offset: 0x000C9B2C
			// (set) Token: 0x06002A57 RID: 10839 RVA: 0x000CB934 File Offset: 0x000C9B34
			public bool ForceSaveAs { get; set; }

			// Token: 0x17000C6F RID: 3183
			// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000CB93D File Offset: 0x000C9B3D
			// (set) Token: 0x06002A59 RID: 10841 RVA: 0x000CB945 File Offset: 0x000C9B45
			public bool SaveAsWhenSaveFailed { get; set; } = true;

			// Token: 0x17000C70 RID: 3184
			// (get) Token: 0x06002A5A RID: 10842 RVA: 0x000CB94E File Offset: 0x000C9B4E
			// (set) Token: 0x06002A5B RID: 10843 RVA: 0x000CB956 File Offset: 0x000C9B56
			public bool AllowSaveToCurrentFile { get; set; } = true;

			// Token: 0x17000C71 RID: 3185
			// (get) Token: 0x06002A5C RID: 10844 RVA: 0x000CB95F File Offset: 0x000C9B5F
			// (set) Token: 0x06002A5D RID: 10845 RVA: 0x000CB967 File Offset: 0x000C9B67
			public bool ShowProgress { get; set; }

			// Token: 0x17000C72 RID: 3186
			// (get) Token: 0x06002A5E RID: 10846 RVA: 0x000CB970 File Offset: 0x000C9B70
			// (set) Token: 0x06002A5F RID: 10847 RVA: 0x000CB978 File Offset: 0x000C9B78
			public TimeSpan ProgressDelayTime { get; set; } = TimeSpan.FromSeconds(1.0);

			// Token: 0x17000C73 RID: 3187
			// (get) Token: 0x06002A60 RID: 10848 RVA: 0x000CB981 File Offset: 0x000C9B81
			// (set) Token: 0x06002A61 RID: 10849 RVA: 0x000CB989 File Offset: 0x000C9B89
			public string InitialFileNamePostfixOverride { get; set; }

			// Token: 0x17000C74 RID: 3188
			// (get) Token: 0x06002A62 RID: 10850 RVA: 0x000CB992 File Offset: 0x000C9B92
			// (set) Token: 0x06002A63 RID: 10851 RVA: 0x000CB99A File Offset: 0x000C9B9A
			public bool ValidCanSaveBeforeActionInvoke { get; set; }

			// Token: 0x17000C75 RID: 3189
			// (get) Token: 0x06002A64 RID: 10852 RVA: 0x000CB9A3 File Offset: 0x000C9BA3
			// (set) Token: 0x06002A65 RID: 10853 RVA: 0x000CB9AB File Offset: 0x000C9BAB
			public bool DocumentModified { get; set; } = true;

			// Token: 0x17000C76 RID: 3190
			// (get) Token: 0x06002A66 RID: 10854 RVA: 0x000CB9B4 File Offset: 0x000C9BB4
			// (set) Token: 0x06002A67 RID: 10855 RVA: 0x000CB9BC File Offset: 0x000C9BBC
			public FileAttributes CreateNewFileAttributes { get; set; }

			// Token: 0x17000C77 RID: 3191
			// (get) Token: 0x06002A68 RID: 10856 RVA: 0x000CB9C5 File Offset: 0x000C9BC5
			// (set) Token: 0x06002A69 RID: 10857 RVA: 0x000CB9CD File Offset: 0x000C9BCD
			public bool RemoveExistsDigitalSignaturesWhenSaveAs { get; set; }

			// Token: 0x17000C78 RID: 3192
			// (get) Token: 0x06002A6A RID: 10858 RVA: 0x000CB9D6 File Offset: 0x000C9BD6
			// (set) Token: 0x06002A6B RID: 10859 RVA: 0x000CB9DE File Offset: 0x000C9BDE
			public Func<MainViewModel.SaveOptions, MainViewModel.BeforeSaveActionArgs, Task<bool>> BeforeSaveAction { get; set; }

			// Token: 0x17000C79 RID: 3193
			// (get) Token: 0x06002A6C RID: 10860 RVA: 0x000CB9E7 File Offset: 0x000C9BE7
			// (set) Token: 0x06002A6D RID: 10861 RVA: 0x000CB9EF File Offset: 0x000C9BEF
			public Func<MainViewModel.SaveOptions, MainViewModel.SaveResult, Task> AfterSaveAction { get; set; }
		}

		// Token: 0x02000369 RID: 873
		public class SaveResult
		{
			// Token: 0x06002A6F RID: 10863 RVA: 0x000CBA29 File Offset: 0x000C9C29
			public SaveResult(string filePath, bool saveToCurrentFile)
			{
				this.File = new FileInfo(filePath);
				this.FailedResult = MainViewModel.SaveFailedResult.Successed;
				this.SaveToCurrentFile = saveToCurrentFile;
			}

			// Token: 0x06002A70 RID: 10864 RVA: 0x000CBA4B File Offset: 0x000C9C4B
			public SaveResult(MainViewModel.SaveFailedResult result)
			{
				if (result == MainViewModel.SaveFailedResult.Successed)
				{
					throw new ArgumentException(null, "result");
				}
				this.FailedResult = result;
			}

			// Token: 0x17000C7A RID: 3194
			// (get) Token: 0x06002A71 RID: 10865 RVA: 0x000CBA69 File Offset: 0x000C9C69
			public FileInfo File { get; }

			// Token: 0x17000C7B RID: 3195
			// (get) Token: 0x06002A72 RID: 10866 RVA: 0x000CBA71 File Offset: 0x000C9C71
			public bool SaveToCurrentFile { get; }

			// Token: 0x17000C7C RID: 3196
			// (get) Token: 0x06002A73 RID: 10867 RVA: 0x000CBA79 File Offset: 0x000C9C79
			public MainViewModel.SaveFailedResult FailedResult { get; }
		}

		// Token: 0x0200036A RID: 874
		public enum SaveFailedResult
		{
			// Token: 0x0400144B RID: 5195
			Successed,
			// Token: 0x0400144C RID: 5196
			Unknown,
			// Token: 0x0400144D RID: 5197
			DocumentNotExist,
			// Token: 0x0400144E RID: 5198
			BeforeSaveFailed,
			// Token: 0x0400144F RID: 5199
			UserCanceled,
			// Token: 0x04001450 RID: 5200
			VerificationSaveFailed
		}

		// Token: 0x0200036B RID: 875
		public class SavingExtraObjects
		{
			// Token: 0x06002A74 RID: 10868 RVA: 0x000CBA81 File Offset: 0x000C9C81
			public SavingExtraObjects(MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave flags)
			{
				this.flags = flags;
			}

			// Token: 0x17000C7D RID: 3197
			// (get) Token: 0x06002A75 RID: 10869 RVA: 0x000CBA90 File Offset: 0x000C9C90
			public bool HasSavingExtraObjects
			{
				get
				{
					return this.flags > MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None;
				}
			}

			// Token: 0x17000C7E RID: 3198
			// (get) Token: 0x06002A76 RID: 10870 RVA: 0x000CBA9C File Offset: 0x000C9C9C
			public bool HasUnembeddedSignature
			{
				get
				{
					MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave extraObjectsNeedSave = MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.UnembeddedSignature;
					return this.HasFlag(in extraObjectsNeedSave);
				}
			}

			// Token: 0x17000C7F RID: 3199
			// (get) Token: 0x06002A77 RID: 10871 RVA: 0x000CBAB4 File Offset: 0x000C9CB4
			public bool HasDigitalSignature
			{
				get
				{
					MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave extraObjectsNeedSave = MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.DigitalSignature;
					return this.HasFlag(in extraObjectsNeedSave);
				}
			}

			// Token: 0x17000C80 RID: 3200
			// (get) Token: 0x06002A78 RID: 10872 RVA: 0x000CBACC File Offset: 0x000C9CCC
			public bool HasRedact
			{
				get
				{
					MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave extraObjectsNeedSave = MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.Redact;
					return this.HasFlag(in extraObjectsNeedSave);
				}
			}

			// Token: 0x17000C81 RID: 3201
			// (get) Token: 0x06002A79 RID: 10873 RVA: 0x000CBAE4 File Offset: 0x000C9CE4
			public bool HasDeferredDigitalSignature
			{
				get
				{
					MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave extraObjectsNeedSave = MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.DeferredDigitalSignature;
					return this.HasFlag(in extraObjectsNeedSave);
				}
			}

			// Token: 0x06002A7A RID: 10874 RVA: 0x000CBAFB File Offset: 0x000C9CFB
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool HasFlag(in MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave flags2)
			{
				return (this.flags & flags2) > MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave.None;
			}

			// Token: 0x04001451 RID: 5201
			private MainViewModel.SavingExtraObjects.ExtraObjectsNeedSave flags;

			// Token: 0x020007BD RID: 1981
			[Flags]
			public enum ExtraObjectsNeedSave
			{
				// Token: 0x04002718 RID: 10008
				None = 0,
				// Token: 0x04002719 RID: 10009
				UnembeddedSignature = 1,
				// Token: 0x0400271A RID: 10010
				DigitalSignature = 2,
				// Token: 0x0400271B RID: 10011
				Redact = 4,
				// Token: 0x0400271C RID: 10012
				DeferredDigitalSignature = 8
			}
		}
	}
}
