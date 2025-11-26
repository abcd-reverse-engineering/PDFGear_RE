using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using PDFKit;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x0200023A RID: 570
	internal class TextObjectContextMenuHolder
	{
		// Token: 0x0600206C RID: 8300 RVA: 0x00093EBF File Offset: 0x000920BF
		public TextObjectContextMenuHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.InitContextMenu();
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x0600206D RID: 8301 RVA: 0x00093EE3 File Offset: 0x000920E3
		public bool IsOpen
		{
			get
			{
				return this.textObjectContextMenu.IsOpen;
			}
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x00093EF0 File Offset: 0x000920F0
		private void InitContextMenu()
		{
			if (TextObjectContextMenuHolder.IsDesignMode)
			{
				return;
			}
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel
			{
				Name = "Edit",
				Caption = Resources.WinOCRSelectedLineContextMenuEdit,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_TextObjEdit.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_TextObjEdit.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextEditObject))
			};
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel
			{
				Name = "Delete",
				Caption = Resources.MenuRightAnnotateDelete,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_AnnotDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_AnnotDelete.png")),
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextDeleteObject))
			};
			ContextMenuItemModel contextMenuItemModel3 = new ContextMenuItemModel
			{
				Name = "ExitEdit",
				Caption = Resources.MenuRightAnnotateExitEdit,
				Command = new AsyncRelayCommand<ContextMenuItemModel>(new Func<ContextMenuItemModel, Task>(this.OnContextExitEditObject))
			};
			ContextMenuModel contextMenuModel = new ContextMenuModel { contextMenuItemModel, contextMenuItemModel2, contextMenuItemModel3 };
			this.textObjectContextMenu = new PdfViewerContextMenu
			{
				ItemsSource = contextMenuModel,
				PlacementTarget = this.annotationCanvas,
				AutoCloseOnMouseLeave = false
			};
			this.textObjectContextMenu.Closed += this.TextObjectContextMenu_Closed;
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x00094034 File Offset: 0x00092234
		private async Task OnContextEditObject(ContextMenuItemModel model)
		{
			if (this.annotationCanvas.TextObjectHolder.SelectedObject != null)
			{
				GAManager.SendEvent("ContextRightMenu", "EditTextObject", "Count", 1L);
				await this.annotationCanvas.TextObjectHolder.EditSelectedTextObjectAsync();
			}
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x00094078 File Offset: 0x00092278
		private async Task OnContextDeleteObject(ContextMenuItemModel model)
		{
			if (this.annotationCanvas.TextObjectHolder.SelectedObject != null)
			{
				GAManager.SendEvent("ContextRightMenu", "DeleteTextObject", "Count", 1L);
				GAManager.SendEvent("TextEditor", "DeleteSelectedObject", "ContextRightMenu", 1L);
				await this.annotationCanvas.TextObjectHolder.DeleteSelectedObjectAsync();
			}
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x000940BB File Offset: 0x000922BB
		private Task OnContextExitEditObject(ContextMenuItemModel arg)
		{
			this.annotationCanvas.TextObjectHolder.CancelTextObject();
			this.Hide();
			return Task.CompletedTask;
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x000940DC File Offset: 0x000922DC
		public async Task<bool> ShowAsync()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			bool flag;
			if (pdfViewer != null && pdfViewer.MouseMode == MouseModes.PanTool)
			{
				flag = false;
			}
			else
			{
				await Task.Delay(50);
				if (this.annotationCanvas.TextObjectHolder.SelectedObject == null)
				{
					flag = false;
				}
				else if (this.textObjectContextMenu.IsOpen)
				{
					flag = true;
				}
				else
				{
					this.textObjectContextMenu.Placement = PlacementMode.MousePoint;
					this.textObjectContextMenu.PlacementTarget = null;
					this.textObjectContextMenu.IsOpen = true;
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x00094120 File Offset: 0x00092320
		public async Task<bool> ShowAtSelectedObejctRightAsync()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			bool flag;
			if (pdfViewer != null && pdfViewer.MouseMode == MouseModes.PanTool)
			{
				flag = false;
			}
			else
			{
				await Task.Delay(50);
				if (this.annotationCanvas.TextObjectHolder.SelectedObject == null)
				{
					flag = false;
				}
				else if (this.textObjectContextMenu.IsOpen)
				{
					flag = true;
				}
				else
				{
					this.annotationCanvas.UpdateHoverPageObjectRect(Rect.Empty);
					this.textObjectContextMenu.Placement = PlacementMode.Right;
					this.textObjectContextMenu.PlacementTarget = this.annotationCanvas.TextObjectHolder.TextObjectEditControl;
					Rect rect = Rect.Empty;
					if (this.annotationCanvas.TextObjectHolder.TextObjectEditControl != null)
					{
						rect = new Rect(0.0, 0.0, this.annotationCanvas.TextObjectHolder.TextObjectEditControl.ActualWidth, this.annotationCanvas.TextObjectHolder.TextObjectEditControl.ActualHeight);
						rect.Intersect(this.annotationCanvas.TransformToVisual(this.annotationCanvas.TextObjectHolder.TextObjectEditControl).TransformBounds(new Rect(0.0, 0.0, this.annotationCanvas.ActualWidth, this.annotationCanvas.ActualWidth)));
					}
					if (rect.Width < 0.01 && rect.Height < 0.01)
					{
						rect = Rect.Empty;
					}
					this.textObjectContextMenu.PlacementRectangle = rect;
					this.textObjectContextMenu.IsOpen = true;
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x00094163 File Offset: 0x00092363
		public void Hide()
		{
			this.textObjectContextMenu.IsOpen = false;
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00094171 File Offset: 0x00092371
		private void TextObjectContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			this.textObjectContextMenu.Placement = PlacementMode.MousePoint;
			this.textObjectContextMenu.PlacementTarget = null;
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06002076 RID: 8310 RVA: 0x0009418B File Offset: 0x0009238B
		private static bool IsDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		// Token: 0x04000CFB RID: 3323
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CFC RID: 3324
		private PdfViewerContextMenu textObjectContextMenu;
	}
}
