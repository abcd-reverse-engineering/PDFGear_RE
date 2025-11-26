using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommonLib.Common.HotKeys;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf.Net;
using pdfeditor.Controls;
using pdfeditor.Controls.Screenshots;
using pdfeditor.Models.Operations;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000073 RID: 115
	public class AppHotKeyManager
	{
		// Token: 0x0600089D RID: 2205 RVA: 0x00029BF5 File Offset: 0x00027DF5
		public AppHotKeyManager()
		{
			AppHotKeyManager.InitializeHotKeys();
			this.UpdateHotKeyEnabledStates();
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00029C08 File Offset: 0x00027E08
		public void UpdateHotKeyEnabledStates()
		{
			MainViewModel mainViewModel;
			global::PDFKit.PdfControl pdfControl;
			if (AppHotKeyManager.TryGetViewModelAndPdfControl(out mainViewModel, out pdfControl))
			{
				bool isEditing = pdfControl.IsEditing;
				bool isVisible = pdfControl.Viewer.IsVisible;
				HotKeyManager.GetOrCreate("Editor_CreateNewPDF").IsEnabled = !isEditing;
				HotKeyManager.GetOrCreate("Editor_Open").IsEnabled = !isEditing;
				HotKeyManager.GetOrCreate("Editor_Print").IsEnabled = !isEditing;
				HotKeyManager.GetOrCreate("Editor_PreviousView").IsEnabled = isVisible || isEditing;
				HotKeyManager.GetOrCreate("Editor_NextView").IsEnabled = isVisible || isEditing;
				HotKeyManager.GetOrCreate("Editor_FitPage").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_FitPage2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_ActualSize").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_ActualSize2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_FitWidth").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_FitWidth2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_FitHeight").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_FitHeight2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_SinglePage").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_SinglePage2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_DoublePage").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_DoublePage2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Continuous").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Continuous2").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_AutoScroll").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Screenshot").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Highlight").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Underline").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Strikethrough").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_AreaHighlight").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Line").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Rectangle").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Oval").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Ink").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_TextBox").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Note").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_ManageAnnotation").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_HighlightFormField").IsEnabled = isVisible;
				HotKeyManager.GetOrCreate("Editor_Save").IsEnabled = !isEditing;
				HotKeyManager.GetOrCreate("Editor_SaveAs").IsEnabled = !isEditing;
			}
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x00029E7C File Offset: 0x0002807C
		private static void InitializeHotKeys()
		{
			if (AppHotKeyManager.initialized)
			{
				return;
			}
			AppHotKeyManager.initialized = true;
			HotKeyManager.GetOrCreate("Editor_CreateNewPDF", new HotKeyItem(Key.N, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextCreateNewPDF;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Open", new HotKeyItem(Key.O, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextOpenDocument;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Save", new HotKeyItem(Key.S, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextSaveDocument;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_SaveAs", new HotKeyItem(Key.S, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextSaveAsDocument;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Print", new HotKeyItem(Key.P, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextPrint;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Undo", new HotKeyItem(Key.Z, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextUndo;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl);
						if (annotationCanvas.ScreenshotDialog.Visibility != Visibility.Visible)
						{
							OperationManager operationManager = vm.OperationManager;
							if (operationManager != null && operationManager.CanGoBack)
							{
								vm.QuickToolUndoModel.Command.Execute(null);
							}
							return;
						}
						ScreenshotDialog screenshotDialog = annotationCanvas.ScreenshotDialog;
						if (screenshotDialog == null)
						{
							return;
						}
						screenshotDialog.UndoDrawControl();
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_Redo", new HotKeyItem(Key.Y, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextRedo;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Redo2", new HotKeyItem(Key.Z, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextRedo;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_AddBookmark", new HotKeyItem(Key.B, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAddBookmark;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							await vm.BookmarkAddCommand.ExecuteAsync(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_Find", new HotKeyItem(Key.F, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFind;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
							if (mainView == null)
							{
								return;
							}
							mainView.ShowSearchBox();
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_DocumentProperties", new HotKeyItem(Key.D, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextDocumentProperties;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_CloseDocument", new HotKeyItem(Key.W, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextCloseDocument;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						MainView mainView2 = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
						if (mainView2 == null)
						{
							return;
						}
						mainView2.Close();
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_PreviousView", new HotKeyItem(Key.Left, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextPreviousView;
				m.IsReadOnly = false;
				m.AllowRepeat = true;
			});
			HotKeyManager.GetOrCreate("Editor_NextView", new HotKeyItem(Key.Right, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextNextView;
				m.IsReadOnly = false;
				m.AllowRepeat = true;
			});
			HotKeyManager.GetOrCreate("Editor_ZoomIn", new HotKeyItem(Key.Add, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextZoomIn;
				m.IsReadOnly = false;
				m.AllowRepeat = true;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.IsVisible)
						{
							if (vm != null)
							{
								vm.ViewToolbar.DocZoomInCmd.Execute(null);
								return;
							}
						}
						else if (vm != null)
						{
							vm.PageEditors.PageEditorZoomInCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_ZoomIn2", new HotKeyItem(Key.OemPlus, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextZoomIn;
				m.IsReadOnly = false;
				m.IsVisible = false;
				m.AllowRepeat = true;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.IsVisible)
						{
							if (vm != null)
							{
								vm.ViewToolbar.DocZoomInCmd.Execute(null);
								return;
							}
						}
						else if (vm != null)
						{
							vm.PageEditors.PageEditorZoomInCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_ZoomOut", new HotKeyItem(Key.Subtract, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextZoomOut;
				m.IsReadOnly = false;
				m.AllowRepeat = true;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.IsVisible)
						{
							if (vm != null)
							{
								vm.ViewToolbar.DocZoomOutCmd.Execute(null);
								return;
							}
						}
						else if (vm != null)
						{
							vm.PageEditors.PageEditorZoomOutCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_ZoomOut2", new HotKeyItem(Key.OemMinus, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextZoomOut;
				m.IsReadOnly = false;
				m.IsVisible = false;
				m.AllowRepeat = true;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.IsVisible)
						{
							if (vm != null)
							{
								vm.ViewToolbar.DocZoomOutCmd.Execute(null);
								return;
							}
						}
						else if (vm != null)
						{
							vm.PageEditors.PageEditorZoomOutCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_FitPage", new HotKeyItem(Key.D0, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitPage;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_FitPage2", new HotKeyItem(Key.NumPad0, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitPage;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_ActualSize", new HotKeyItem(Key.D1, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFullSize;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_ActualSize2", new HotKeyItem(Key.NumPad1, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFullSize;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_FitWidth", new HotKeyItem(Key.D2, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitWidth;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_FitWidth2", new HotKeyItem(Key.NumPad2, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitWidth;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_FitHeight", new HotKeyItem(Key.D3, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitHeight;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_FitHeight2", new HotKeyItem(Key.NumPad3, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFitHeight;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_SinglePage", new HotKeyItem(Key.D4, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextSinglePage;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_SinglePage2", new HotKeyItem(Key.NumPad4, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextSinglePage;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_DoublePage", new HotKeyItem(Key.D5, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextDoublePage;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_DoublePage2", new HotKeyItem(Key.NumPad5, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextDoublePage;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_Continuous", new HotKeyItem(Key.D6, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextContinusRead;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Continuous2", new HotKeyItem(Key.NumPad6, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextContinusRead;
				m.IsReadOnly = false;
				m.IsVisible = false;
			});
			HotKeyManager.GetOrCreate("Editor_RotateLeft", new HotKeyItem(Key.Left, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextRotateLeft;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								if (vm != null)
								{
									vm.ViewToolbar.PageRotateLeftCmd.Execute(null);
									return;
								}
							}
							else if (vm != null)
							{
								vm.PageEditors.PageEditorRotateLeftCmd.Execute(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_RotateRight", new HotKeyItem(Key.Right, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextRotateRight;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								if (vm != null)
								{
									vm.ViewToolbar.PageRotateRightCmd.Execute(null);
									return;
								}
							}
							else if (vm != null)
							{
								vm.PageEditors.PageEditorRotateRightCmd.Execute(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_AutoScroll", new HotKeyItem(Key.H, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAutoScroll;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Ocr", new HotKeyItem(Key.O, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextOcrPages;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							await vm.ViewToolbar.DoOcr(Source.Viewer);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_Screenshot", new HotKeyItem(Key.X, ModifierKeys.Alt | ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextScreenshot;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Highlight", new HotKeyItem(Key.H, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextHighlight;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Underline", new HotKeyItem(Key.U, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextUnderline;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Strikethrough", new HotKeyItem(Key.S, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextStrike;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_AreaHighlight", new HotKeyItem(Key.A, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAreaHighlight;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Line", new HotKeyItem(Key.L, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateLine;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Rectangle", new HotKeyItem(Key.R, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateShape;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Oval", new HotKeyItem(Key.O, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateEllipse;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_Ink", new HotKeyItem(Key.I, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateInk;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_TextBox", new HotKeyItem(Key.B, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateTextBox;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_AddText", new HotKeyItem(Key.T, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateTypeWriter;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							if (vm.AnnotationToolbar.TextButtonModel.IsChecked)
							{
								vm.AnnotationToolbar.TextButtonModel.IsChecked = false;
							}
							else
							{
								vm.AnnotationToolbar.TextButtonModel.IsChecked = true;
							}
							vm.AnnotationToolbar.TextButtonModel.Command.Execute(vm.AnnotationToolbar.TextButtonModel);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_Note", new HotKeyItem(Key.N, ModifierKeys.Alt), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateNote;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_ShowHideComments", new HotKeyItem(Key.S, ModifierKeys.Alt | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextShowHideComments;
				m.IsReadOnly = true;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							vm.ShowHideAnnotationCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_ManageAnnotation", new HotKeyItem(Key.M, ModifierKeys.Alt | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextAnnotateManage;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_AddImage", new HotKeyItem(Key.I, ModifierKeys.Alt | ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextInsertImage;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (pdfControl.Viewer.IsVisible)
						{
							vm.AnnotationToolbar.ImageButtonModel.Command.Execute(vm.AnnotationToolbar.ImageButtonModel);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_HighlightFormField", new HotKeyItem(Key.H, ModifierKeys.Control), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextHighlightForm;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_ExtractPage", new HotKeyItem(Key.E, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextPageExtract;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorExtractCmd.ExecuteAsync(pdfPageEditListModel);
							}
							else
							{
								await vm.PageEditors.PageEditorExtractCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_DeletePage", new HotKeyItem(Key.D, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextPageDelete;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel2 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorDeleteCmd.ExecuteAsync(pdfPageEditListModel2);
							}
							else
							{
								await vm.PageEditors.PageEditorDeleteCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_InsertBlankPage", new HotKeyItem(Key.B, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextInsertPageSubBlankPage;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel3 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorInsertBlankCmd.ExecuteAsync(pdfPageEditListModel3);
							}
							else
							{
								await vm.PageEditors.PageEditorInsertBlankCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_InsertPDF", new HotKeyItem(Key.P, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextInsertPageSubFromPDF;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel4 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorInsertFromPDFCmd.ExecuteAsync(pdfPageEditListModel4);
							}
							else
							{
								await vm.PageEditors.PageEditorInsertFromPDFCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_InsertWord", new HotKeyItem(Key.W, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextInsertPageSubFromWord;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel5 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorInsertFromWordCmd.ExecuteAsync(pdfPageEditListModel5);
							}
							else
							{
								await vm.PageEditors.PageEditorInsertFromWordCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_InsertImage", new HotKeyItem(Key.I, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextInsertPageSubFromImage;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(async delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel6 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								await vm.PageEditors.PageEditorInsertFromImageCmd.ExecuteAsync(pdfPageEditListModel6);
							}
							else
							{
								await vm.PageEditors.PageEditorInsertFromImageCmd.ExecuteAsync(null);
							}
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_CropPage", new HotKeyItem(Key.C, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextCropPage;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel7 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								vm.PageEditors.CropPageCmd2.Execute(pdfPageEditListModel7);
								return;
							}
							vm.PageEditors.CropPageCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_ResizePage", new HotKeyItem(Key.R, ModifierKeys.Control | ModifierKeys.Shift), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.PageResizeWinTitle;
				m.IsReadOnly = false;
				m.Command = new RelayCommand(delegate
				{
					AppHotKeyManager.InvokeHotKeyCommand(delegate(MainViewModel vm, global::PDFKit.PdfControl pdfControl)
					{
						if (!pdfControl.IsEditing)
						{
							if (pdfControl.IsVisible)
							{
								PdfPageEditListModel pdfPageEditListModel8 = new PdfPageEditListModel(vm.Document, vm.SelectedPageIndex);
								vm.PageEditors.viewerResizePageCmd.Execute(pdfPageEditListModel8);
								return;
							}
							vm.PageEditors.ResizePageCmd.Execute(null);
						}
					});
				});
			});
			HotKeyManager.GetOrCreate("Editor_Present", new HotKeyItem(Key.F5), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextPresent;
				m.IsReadOnly = false;
			});
			HotKeyManager.GetOrCreate("Editor_FullScreen", new HotKeyItem(Key.F11), delegate(HotKeyModel m)
			{
				m.DisplayName = Resources.ShortcutTextFullScreen;
				m.IsReadOnly = false;
			});
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0002AAB8 File Offset: 0x00028CB8
		private static bool TryGetViewModelAndPdfControl(out MainViewModel mainViewModel, out global::PDFKit.PdfControl pdfControl)
		{
			pdfControl = null;
			mainViewModel = Ioc.Default.GetService<MainViewModel>();
			if (mainViewModel != null)
			{
				PdfDocument document = mainViewModel.Document;
				if (document != null)
				{
					pdfControl = global::PDFKit.PdfControl.GetPdfControl(document);
				}
			}
			return pdfControl != null;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0002AAF0 File Offset: 0x00028CF0
		private static void InvokeHotKeyCommand(Action<MainViewModel, global::PDFKit.PdfControl> pdfControlVisible)
		{
			MainViewModel mainViewModel;
			global::PDFKit.PdfControl pdfControl;
			if (AppHotKeyManager.TryGetViewModelAndPdfControl(out mainViewModel, out pdfControl) && HotKeyExtensions.IsWindowEnabled(pdfControl) && pdfControlVisible != null)
			{
				pdfControlVisible(mainViewModel, pdfControl);
			}
		}

		// Token: 0x04000445 RID: 1093
		private static bool initialized;
	}
}
