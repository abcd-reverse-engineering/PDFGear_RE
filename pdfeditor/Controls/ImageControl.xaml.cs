using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonLib.Common;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls.Screenshots;
using pdfeditor.Models;
using pdfeditor.Models.PageContents;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Controls
{
	// Token: 0x020001BB RID: 443
	public partial class ImageControl : UserControl
	{
		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06001925 RID: 6437 RVA: 0x00061832 File Offset: 0x0005FA32
		private MainViewModel VM
		{
			get
			{
				return this.annotationCanvas.DataContext as MainViewModel;
			}
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x00061844 File Offset: 0x0005FA44
		public ImageControl()
		{
			this.InitializeComponent();
			base.PreviewKeyDown += this.ImageControl_PreviewKeyDown;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x0006187C File Offset: 0x0005FA7C
		private async void ImageControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && this.ImageControlState)
			{
				this.ImageControlState = false;
				this.SiderEditorActionBtn.Visibility = Visibility.Visible;
				this.SiderEditorbar.Visibility = Visibility.Collapsed;
				this.SiderAcitonbar.Visibility = Visibility.Visible;
				this.selectedBoder.Visibility = Visibility.Visible;
				this.editorBoder.Visibility = Visibility.Collapsed;
				this.UpdateImageborder();
			}
			if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None && this.VM.SelectedAnnotation != null)
			{
				if (ModernMessageBox.Show(pdfeditor.Properties.Resources.ImageControl_DeleteConfirm, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
				{
					PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
					await this.DeleteImageCmd(pdfImageObject, this.Pageindex);
					base.Visibility = Visibility.Collapsed;
				}
			}
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x000618BC File Offset: 0x0005FABC
		public void CreateImageborder(AnnotationCanvas annotationCanvas, PdfDocument document, int PageIndex, int Imageindex, PdfViewer pdfViewer, bool createNew = false)
		{
			this.clickStartPosition = new global::System.Windows.Point(0.0, 0.0);
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.Document = document;
			this.Pageindex = PageIndex;
			this.imageindex = Imageindex;
			this.PdfViewer = pdfViewer;
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
			this.ImageRect = new FS_RECTF
			{
				right = pdfImageObject.BoundingBox.right,
				left = pdfImageObject.BoundingBox.left,
				top = pdfImageObject.BoundingBox.top,
				bottom = pdfImageObject.BoundingBox.bottom
			};
			Rect rect;
			pdfControl.TryGetClientRect(this.Pageindex, this.ImageRect, out rect);
			this.Imageborder.Width = rect.Width;
			this.Imageborder.Height = rect.Height;
			this.Left = rect.Left;
			this.Top = rect.Top;
			if (pdfControl != null)
			{
				this.Siderbar.VerticalAlignment = VerticalAlignment.Top;
				if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0 && this.Top < 0.0 && Math.Abs(this.Top) + 200.0 < this.Imageborder.Height)
				{
					this.Siderbar.Margin = new Thickness(-170.0, Math.Abs(this.Top), 0.0, 0.0);
				}
				else if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0 && this.Top < 0.0 && Math.Abs(this.Top) + 200.0 > this.Imageborder.Height)
				{
					this.Siderbar.Margin = new Thickness(-170.0, 0.0, 0.0, 0.0);
					this.Siderbar.VerticalAlignment = VerticalAlignment.Bottom;
				}
				else if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0)
				{
					this.Siderbar.Margin = new Thickness(-170.0, 0.0, 0.0, 0.0);
				}
				else if (this.Top < 0.0 && Math.Abs(this.Top) + 200.0 < this.Imageborder.Height)
				{
					this.Siderbar.Margin = new Thickness(0.0, Math.Abs(this.Top), 0.0, 0.0);
				}
				else if (this.Top < 0.0 && Math.Abs(this.Top) + 200.0 > this.Imageborder.Height)
				{
					this.Siderbar.Margin = new Thickness(0.0);
					this.Siderbar.VerticalAlignment = VerticalAlignment.Bottom;
				}
				else
				{
					this.Siderbar.Margin = new Thickness(0.0);
				}
			}
			this.layoutImageBorder();
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			base.IsVisibleChanged += this.ImageControl_IsVisibleChanged;
			this.UpdateHierarchyBtnIsenabled();
			if (createNew)
			{
				this.DeleteBtn.Visibility = Visibility.Visible;
				return;
			}
			this.DeleteBtn.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x00061CCC File Offset: 0x0005FECC
		private void Topleft_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				global::System.Windows.Point position = e.GetPosition(this);
				this.IsMoved = true;
				this.Imageborder.StrokeDashArray = new DoubleCollection { 2.0, 2.0 };
				float num = (float)(position.X - this.clickStartPosition.X);
				float num2 = (float)(position.Y - this.clickStartPosition.Y);
				this.Imageborder.Width = this.Imageborder.Width + (double)num;
				this.Imageborder.Height = this.Imageborder.Height + (double)num2;
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x00061D80 File Offset: 0x0005FF80
		private void layoutImageBorder()
		{
			if (this.Imageborder.Height < 210.0)
			{
				this.Leftcenter.VerticalAlignment = VerticalAlignment.Top;
				this.Rightcenter.VerticalAlignment = VerticalAlignment.Top;
				this.Leftcenter.Margin = new Thickness(-4.0, this.Imageborder.Height / 2.0, 4.0, 0.0);
				this.Rightcenter.Margin = new Thickness(4.0, this.Imageborder.Height / 2.0, -4.0, 0.0);
				this.Bottomleft.VerticalAlignment = VerticalAlignment.Top;
				this.Bottomright.VerticalAlignment = VerticalAlignment.Top;
				this.Bottomleft.Margin = new Thickness(-4.0, this.Imageborder.Height - 2.0, 0.0, -4.0);
				this.Bottomright.Margin = new Thickness(0.0, this.Imageborder.Height - 2.0, -4.0, -4.0);
				this.Bottomcenter.VerticalAlignment = VerticalAlignment.Top;
				this.Bottomcenter.Margin = new Thickness(0.0, this.Imageborder.Height - 2.0, 0.0, -4.0);
			}
			else
			{
				this.Bottomcenter.VerticalAlignment = VerticalAlignment.Bottom;
				this.Bottomleft.VerticalAlignment = VerticalAlignment.Bottom;
				this.Bottomright.VerticalAlignment = VerticalAlignment.Bottom;
				this.Leftcenter.VerticalAlignment = VerticalAlignment.Center;
				this.Rightcenter.VerticalAlignment = VerticalAlignment.Center;
				this.Leftcenter.Margin = new Thickness(-4.0, 0.0, 4.0, 0.0);
				this.Rightcenter.Margin = new Thickness(4.0, 0.0, -4.0, 0.0);
			}
			this.Imageborder2.Width = this.Imageborder.Width;
			this.Imageborder2.Height = this.Imageborder.Height;
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x00061FF8 File Offset: 0x000601F8
		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Siderbar.Visibility = Visibility.Collapsed;
			if (sender == this.Topleft)
			{
				this.mousePosition = ImageControl.MousePosition.topLeft;
				return;
			}
			if (sender == this.Topright)
			{
				this.mousePosition = ImageControl.MousePosition.topRight;
				return;
			}
			if (sender == this.Bottomright)
			{
				this.mousePosition = ImageControl.MousePosition.bottomRight;
				return;
			}
			if (sender == this.Bottomleft)
			{
				this.mousePosition = ImageControl.MousePosition.bottomLeft;
				return;
			}
			if (sender == this.Topcenter)
			{
				this.mousePosition = ImageControl.MousePosition.topCenter;
				return;
			}
			if (sender == this.Leftcenter)
			{
				this.mousePosition = ImageControl.MousePosition.leftCenter;
				return;
			}
			if (sender == this.Rightcenter)
			{
				this.mousePosition = ImageControl.MousePosition.rightCenter;
				return;
			}
			if (sender == this.Bottomcenter)
			{
				this.mousePosition = ImageControl.MousePosition.bottomCenter;
			}
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x0600192C RID: 6444 RVA: 0x00062098 File Offset: 0x00060298
		private bool IsShiftPressedInternal
		{
			get
			{
				return (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > KeyStates.None || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > KeyStates.None;
			}
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x000620B4 File Offset: 0x000602B4
		public void ImageControlReSizeImage(global::System.Windows.Point point)
		{
			this.IsMoved = true;
			global::System.Windows.Point point2 = new global::System.Windows.Point(0.0, 0.0);
			if (this.clickStartPosition == point2)
			{
				this.clickStartPosition = point;
			}
			this.Imageborder.StrokeDashArray = new DoubleCollection { 2.0, 2.0 };
			float num = (float)(point.X - this.clickStartPosition.X);
			float num2 = (float)(point.Y - this.clickStartPosition.Y);
			double width = this.Imageborder.Width;
			double height = this.Imageborder.Height;
			this.clickStartPosition = point;
			this.layoutImageBorder();
			if (width != 0.0 && height != 0.0 && this.IsShiftPressedInternal)
			{
				double num3 = width / height;
				num2 = num / (float)num3;
				if (this.mousePosition == ImageControl.MousePosition.topRight || this.mousePosition == ImageControl.MousePosition.bottomLeft)
				{
					num2 = -num / (float)num3;
				}
			}
			if (this.mousePosition == ImageControl.MousePosition.topLeft)
			{
				if (width - (double)num <= 0.0 || height - (double)num2 <= 0.0)
				{
					return;
				}
				this.Imageborder.Width -= (double)num;
				this.Imageborder.Height -= (double)num2;
				this.Top += (double)num2;
				this.Left += (double)num;
				Canvas.SetTop(this, this.Top);
				Canvas.SetLeft(this, this.Left);
				return;
			}
			else if (this.mousePosition == ImageControl.MousePosition.topRight)
			{
				if (width + (double)num <= 0.0 || height - (double)num2 <= 0.0)
				{
					return;
				}
				this.Imageborder.Width += (double)num;
				this.Imageborder.Height -= (double)num2;
				this.Top += (double)num2;
				Canvas.SetTop(this, this.Top);
				return;
			}
			else if (this.mousePosition == ImageControl.MousePosition.bottomLeft)
			{
				if (width - (double)num <= 0.0 || height + (double)num2 <= 0.0)
				{
					return;
				}
				this.Imageborder.Width -= (double)num;
				this.Imageborder.Height += (double)num2;
				this.Left += (double)num;
				Canvas.SetLeft(this, this.Left);
				return;
			}
			else if (this.mousePosition == ImageControl.MousePosition.bottomRight)
			{
				if (width + (double)num <= 0.0 || height + (double)num2 <= 0.0)
				{
					return;
				}
				this.Imageborder.Width += (double)num;
				this.Imageborder.Height += (double)num2;
				return;
			}
			else if (this.mousePosition == ImageControl.MousePosition.topCenter)
			{
				if (height - (double)num2 <= 0.5)
				{
					return;
				}
				this.Imageborder.Height -= (double)num2;
				this.Top += (double)num2;
				Canvas.SetTop(this, this.Top);
				return;
			}
			else if (this.mousePosition == ImageControl.MousePosition.bottomCenter)
			{
				if (height + (double)num2 <= 0.0)
				{
					return;
				}
				this.Imageborder.Height += (double)num2;
				return;
			}
			else
			{
				if (this.mousePosition != ImageControl.MousePosition.leftCenter)
				{
					if (this.mousePosition == ImageControl.MousePosition.rightCenter)
					{
						if (width + (double)num <= 0.0)
						{
							return;
						}
						this.Imageborder.Width += (double)num;
					}
					return;
				}
				if (width - (double)num <= 0.0)
				{
					return;
				}
				this.Imageborder.Width -= (double)num;
				this.Left += (double)num;
				Canvas.SetLeft(this, this.Left);
				return;
			}
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x00062488 File Offset: 0x00060688
		public async void ImageControlMoveImage(global::System.Windows.Point point)
		{
			this.IsMoved = false;
			PdfImageObject pdfImageObject = null;
			if (this.mousePosition != ImageControl.MousePosition.None)
			{
				this.clickStartPosition = new global::System.Windows.Point(0.0, 0.0);
				Rect rect = new Rect
				{
					Height = this.Imageborder.Height,
					Width = this.Imageborder.Width,
					X = this.Left,
					Y = this.Top
				};
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				FS_RECTF fs_RECTF;
				pdfControl.TryGetPageRect(this.Pageindex, rect, out fs_RECTF);
				if (this.Isseleted)
				{
					pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.Document.Pages[this.Pageindex].PageObjects.Count - 1] as PdfImageObject;
				}
				else if (this.Pageindex >= 0 && this.imageindex >= 0)
				{
					pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				}
				float num = this.ImageRect.Width - fs_RECTF.Width;
				float num2 = this.ImageRect.Height - fs_RECTF.Height;
				if (pdfImageObject != null)
				{
					FS_POINTF fs_MATRIX = this.GetFS_MATRIX(pdfImageObject, num, num2);
					float num3 = fs_RECTF.Width / this.ImageRect.Width;
					float num4 = fs_RECTF.Height / this.ImageRect.Height;
					FS_MATRIX matrix = pdfImageObject.Matrix;
					float num5 = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
					float num6 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
					matrix.Translate(-num5, -num6, false);
					matrix.Scale(num3, num4, false);
					matrix.Translate(fs_MATRIX.X, fs_MATRIX.Y, false);
					await this.ImageOperationCmd(pdfImageObject, this.Pageindex, this.imageindex, matrix);
					PdfImageObject pdfImageObject2 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
					this.ImageRect = new FS_RECTF
					{
						right = pdfImageObject2.BoundingBox.right,
						left = pdfImageObject2.BoundingBox.left,
						top = pdfImageObject2.BoundingBox.top,
						bottom = pdfImageObject2.BoundingBox.bottom
					};
					Rect rect2;
					pdfControl.TryGetClientRect(this.Pageindex, this.ImageRect, out rect2);
					this.Top = rect2.Top;
					this.Left = rect2.Left;
					this.UpdateImageborder();
				}
				pdfControl = null;
			}
			else
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				global::System.Windows.Point point2;
				pdfControl.TryGetPagePoint(this.Pageindex, point, out point2);
				global::System.Windows.Point point3;
				pdfControl.TryGetPagePoint(this.Pageindex, this.clickStartPosition, out point3);
				float num7 = (float)(point3.X - point2.X);
				float num8 = (float)(point3.Y - point2.Y);
				if (this.Isseleted)
				{
					pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.Document.Pages[this.Pageindex].PageObjects.Count - 1] as PdfImageObject;
				}
				else if (this.Pageindex >= 0 && this.imageindex >= 0)
				{
					pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				}
				if (pdfImageObject != null)
				{
					FS_MATRIX matrix2 = pdfImageObject.Matrix;
					matrix2.Translate(-num7, -num8, false);
					await this.ImageOperationCmd(pdfImageObject, this.Pageindex, this.imageindex, matrix2);
					PdfImageObject pdfImageObject3 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
					this.ImageRect = new FS_RECTF
					{
						right = pdfImageObject3.BoundingBox.right,
						left = pdfImageObject3.BoundingBox.left,
						top = pdfImageObject3.BoundingBox.top,
						bottom = pdfImageObject3.BoundingBox.bottom
					};
					Rect rect3;
					pdfControl.TryGetClientRect(this.Pageindex, this.ImageRect, out rect3);
					this.Top = rect3.Top;
					this.Left = rect3.Left;
					this.UpdateImageborder();
				}
				pdfControl = null;
			}
			if (this.Isseleted)
			{
				this.Isseleted = false;
				this.imageindex = this.Document.Pages[this.Pageindex].PageObjects.Count - 1;
				Rect rect4;
				global::PDFKit.PdfControl.GetPdfControl(this.Document).TryGetClientRect(this.Pageindex, this.ImageRect, out rect4);
				this.Imageborder.Width = rect4.Width;
				this.Imageborder.Height = rect4.Height;
				this.layoutImageBorder();
				this.Left = rect4.Left;
				this.Top = rect4.Top;
				Canvas.SetLeft(this, rect4.Left);
				Canvas.SetTop(this, rect4.Top);
			}
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x000624C8 File Offset: 0x000606C8
		private FS_POINTF GetFS_MATRIX(PdfImageObject imageObject, float distanceX, float distanceY)
		{
			ImageControl.MousePosition mousePosition = this.mousePosition;
			FS_POINTF fs_POINTF = new FS_POINTF(0f, 0f);
			if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate90)
			{
				if (this.mousePosition == ImageControl.MousePosition.topLeft)
				{
					mousePosition = ImageControl.MousePosition.bottomLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topRight)
				{
					mousePosition = ImageControl.MousePosition.topLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomRight)
				{
					mousePosition = ImageControl.MousePosition.topRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomLeft)
				{
					mousePosition = ImageControl.MousePosition.bottomRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topCenter)
				{
					mousePosition = ImageControl.MousePosition.leftCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.leftCenter)
				{
					mousePosition = ImageControl.MousePosition.bottomCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.rightCenter)
				{
					mousePosition = ImageControl.MousePosition.topCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomCenter)
				{
					mousePosition = ImageControl.MousePosition.rightCenter;
				}
			}
			else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate180)
			{
				if (this.mousePosition == ImageControl.MousePosition.topLeft)
				{
					mousePosition = ImageControl.MousePosition.bottomRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topRight)
				{
					mousePosition = ImageControl.MousePosition.bottomLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomRight)
				{
					mousePosition = ImageControl.MousePosition.topLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomLeft)
				{
					mousePosition = ImageControl.MousePosition.topRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topCenter)
				{
					mousePosition = ImageControl.MousePosition.bottomCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.leftCenter)
				{
					mousePosition = ImageControl.MousePosition.rightCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.rightCenter)
				{
					mousePosition = ImageControl.MousePosition.leftCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomCenter)
				{
					mousePosition = ImageControl.MousePosition.topCenter;
				}
			}
			else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate270)
			{
				if (this.mousePosition == ImageControl.MousePosition.topLeft)
				{
					mousePosition = ImageControl.MousePosition.topRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topRight)
				{
					mousePosition = ImageControl.MousePosition.bottomRight;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomRight)
				{
					mousePosition = ImageControl.MousePosition.bottomLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomLeft)
				{
					mousePosition = ImageControl.MousePosition.topLeft;
				}
				else if (this.mousePosition == ImageControl.MousePosition.topCenter)
				{
					mousePosition = ImageControl.MousePosition.rightCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.leftCenter)
				{
					mousePosition = ImageControl.MousePosition.topCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.rightCenter)
				{
					mousePosition = ImageControl.MousePosition.bottomCenter;
				}
				else if (this.mousePosition == ImageControl.MousePosition.bottomCenter)
				{
					mousePosition = ImageControl.MousePosition.leftCenter;
				}
			}
			if (mousePosition == ImageControl.MousePosition.topLeft)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - distanceY - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.topRight)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - distanceX - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - distanceY - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.bottomRight)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - distanceX - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.bottomLeft)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.topCenter)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - imageObject.BoundingBox.Width / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - distanceY - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.bottomCenter)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - imageObject.BoundingBox.Width / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - (imageObject.BoundingBox.Height - distanceY) / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.leftCenter)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - imageObject.BoundingBox.Height / 2f;
				return fs_POINTF;
			}
			if (mousePosition == ImageControl.MousePosition.rightCenter)
			{
				fs_POINTF.X = imageObject.BoundingBox.right - distanceX - (imageObject.BoundingBox.Width - distanceX) / 2f;
				fs_POINTF.Y = imageObject.BoundingBox.top - imageObject.BoundingBox.Height / 2f;
				return fs_POINTF;
			}
			return fs_POINTF;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x0006297A File Offset: 0x00060B7A
		private void ImageControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.Isseleted = false;
			this.IsMoved = false;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x0006298C File Offset: 0x00060B8C
		public void MoveImageBorder(global::System.Windows.Point point)
		{
			this.IsMoved = true;
			this.Imageborder.StrokeDashArray = new DoubleCollection { 2.0, 2.0 };
			global::System.Windows.Point point2 = new global::System.Windows.Point(0.0, 0.0);
			if (this.clickStartPosition == point2)
			{
				this.clickStartPosition = point;
			}
			float num = (float)(point.X - this.clickStartPosition.X);
			float num2 = (float)(point.Y - this.clickStartPosition.Y);
			Rect rect;
			global::PDFKit.PdfControl.GetPdfControl(this.Document).TryGetClientRect(this.Pageindex, this.ImageRect, out rect);
			double num3 = rect.Left + (double)num;
			double num4 = rect.Top + (double)num2;
			Canvas.SetLeft(this, num3);
			Canvas.SetTop(this, num4);
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x00062A70 File Offset: 0x00060C70
		public void UpdateImageborder()
		{
			if (base.Visibility == Visibility.Visible && this.VM.AnnotationMode != AnnotationMode.None && this.VM.AnnotationMode != AnnotationMode.Image)
			{
				base.Visibility = Visibility.Collapsed;
				return;
			}
			if (base.Visibility == Visibility.Visible)
			{
				FS_RECTF imageRect = this.ImageRect;
				this.clickStartPosition = new global::System.Windows.Point(0.0, 0.0);
				this.mousePosition = ImageControl.MousePosition.None;
				this.IsMoved = false;
				this.Siderbar.Visibility = Visibility.Visible;
				this.Imageborder.StrokeDashArray = new DoubleCollection();
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				Rect rect;
				pdfControl.TryGetClientRect(this.Pageindex, this.ImageRect, out rect);
				this.Imageborder.Width = rect.Width;
				this.Imageborder.Height = rect.Height;
				this.Top = rect.Top;
				this.Left = rect.Left;
				if (pdfControl != null)
				{
					this.Siderbar.VerticalAlignment = VerticalAlignment.Top;
					if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0 && this.Top < 0.0 && Math.Abs(this.Top) + 200.0 < this.Imageborder.Height)
					{
						this.Siderbar.Margin = new Thickness(-170.0, Math.Abs(this.Top), 0.0, 0.0);
					}
					else if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0 && this.Top < 0.0 && Math.Abs(this.Top) + 200.0 > this.Imageborder.Height)
					{
						this.Siderbar.Margin = new Thickness(-170.0, 0.0, 0.0, 0.0);
						this.Siderbar.VerticalAlignment = VerticalAlignment.Bottom;
					}
					else if (pdfControl.ActualWidth - this.Left - this.Imageborder.Width <= 38.0)
					{
						this.Siderbar.Margin = new Thickness(-170.0, 0.0, 0.0, 0.0);
					}
					else if (this.Top < 0.0 && Math.Abs(this.Top) + 200.0 < this.Imageborder.Height)
					{
						this.Siderbar.Margin = new Thickness(0.0, Math.Abs(this.Top), 0.0, 0.0);
					}
					else if (this.Top < 0.0 && Math.Abs(this.Top) + 200.0 > this.Imageborder.Height)
					{
						this.Siderbar.Margin = new Thickness(0.0);
						this.Siderbar.VerticalAlignment = VerticalAlignment.Bottom;
					}
					else
					{
						this.Siderbar.Margin = new Thickness(0.0);
					}
				}
				this.layoutImageBorder();
				Canvas.SetLeft(this, rect.Left);
				Canvas.SetTop(this, rect.Top);
			}
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x00062E00 File Offset: 0x00061000
		private void exprotbtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "ExportImage", "Count", 1L);
			PdfImageObject pdfImageObject = null;
			if (this.Isseleted)
			{
				pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.Document.Pages[this.Pageindex].PageObjects.Count - 1] as PdfImageObject;
			}
			else if (this.Pageindex >= 0 && this.imageindex >= 0)
			{
				pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			}
			if (pdfImageObject != null)
			{
				this.SaveImageAsync(pdfImageObject);
			}
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x00062EC0 File Offset: 0x000610C0
		private async void rotate_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "RotateImage", "Count", 1L);
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				FS_MATRIX matrix = pdfImageObject.Matrix;
				float num = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
				float num2 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
				matrix.Translate(-num, -num2, false);
				matrix.Rotate(-1.57079637f, false);
				matrix.Translate(num, num2, false);
				await this.ImageOperationCmd(pdfImageObject, this.Pageindex, this.imageindex, matrix);
				PdfImageObject pdfImageObject2 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				this.ImageRect = new FS_RECTF
				{
					right = pdfImageObject2.BoundingBox.right,
					left = pdfImageObject2.BoundingBox.left,
					top = pdfImageObject2.BoundingBox.top,
					bottom = pdfImageObject2.BoundingBox.bottom
				};
				this.UpdateImageborder();
			}
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x00062EF8 File Offset: 0x000610F8
		private async void ocrBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				GAManager.SendEvent("ImageAction", "OCRImage", "Count", 1L);
				PdfViewer viewer = this.annotationCanvas.PdfViewer;
				PdfImageObject imageObject = null;
				if (this.Isseleted)
				{
					imageObject = this.Document.Pages[this.Pageindex].PageObjects[this.Document.Pages[this.Pageindex].PageObjects.Count - 1] as PdfImageObject;
				}
				else if (this.Pageindex >= 0 && this.imageindex >= 0)
				{
					imageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				}
				using (MemoryStream ms = new MemoryStream())
				{
					imageObject.Bitmap.Image.Save(ms, ImageFormat.Png);
					ms.Position = 0L;
					global::System.Drawing.Image Image = global::System.Drawing.Image.FromStream(ms);
					if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate90)
					{
						Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
					else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate180)
					{
						Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
					}
					else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate270)
					{
						Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
					}
					PdfBitmap pdfBitmap = PdfBitmap.FromBitmap((Bitmap)Image);
					WriteableBitmap writeableBitmap = await this.GetImageAsync(pdfBitmap);
					FS_RECTF fs_RECTF = new FS_RECTF
					{
						bottom = imageObject.BoundingBox.bottom,
						top = imageObject.BoundingBox.top,
						left = imageObject.BoundingBox.left,
						right = imageObject.BoundingBox.right
					};
					new ScreenshotDialog();
					global::System.Drawing.Image image = imageObject.Bitmap.Image;
					Rect rect;
					viewer.TryGetClientRect(this.Pageindex, fs_RECTF, out rect);
					ScreenshotDialogResult screenshotDialogResult = ScreenshotDialogResult.CreateExtractImageText(this.Pageindex, "", writeableBitmap, Image, fs_RECTF, rect, true);
					if (screenshotDialogResult != null && screenshotDialogResult.Completed)
					{
						ExtractTextResultDialog extractTextResultDialog = ExtractTextResultDialog.FromImage(this.Document, screenshotDialogResult);
						extractTextResultDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
						extractTextResultDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
						extractTextResultDialog.ShowDialog();
					}
					Image = null;
				}
				MemoryStream ms = null;
				viewer = null;
				imageObject = null;
			}
			catch
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.ImageControl_OcrFailed, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x00062F30 File Offset: 0x00061130
		private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "DeleteImage", "Count", 1L);
			if (!ConfigManager.GetDoNotShowFlag("NotShowDeleteImageFlag", false))
			{
				MessageBoxHelper.RichMessageBoxResult richMessageBoxResult = MessageBoxHelper.Show(new MessageBoxHelper.RichMessageBoxContent
				{
					Content = pdfeditor.Properties.Resources.ImageControl_DeleteConfirm,
					ShowLeftBottomCheckbox = true,
					LeftBottomCheckboxContent = pdfeditor.Properties.Resources.WinPwdPasswordSaveTipNotshowagainContent
				}, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false);
				bool? checkboxResult = richMessageBoxResult.CheckboxResult;
				if (checkboxResult != null && checkboxResult.GetValueOrDefault())
				{
					ConfigManager.SetDoNotShowFlag("NotShowDeleteImageFlag", true);
				}
				if (richMessageBoxResult.Result != MessageBoxResult.Yes)
				{
					return;
				}
			}
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			await this.DeleteImageCmd(pdfImageObject, this.Pageindex);
			base.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x00062F68 File Offset: 0x00061168
		private void ReplaceBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "ReplaceImage", "Count", 1L);
			try
			{
				if (this.Pageindex >= 0 && this.imageindex >= 0)
				{
					OpenFileDialog openFileDialog = new OpenFileDialog
					{
						Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|Windows Bitmap(*.bmp)|*.bmp|Windows Icon(*.ico)|*.ico|Graphics Interchange Format (*.gif)|(*.gif)|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Tag Image File Format (*.tif)|*.tif;*.tiff",
						ShowReadOnly = false,
						ReadOnlyChecked = true
					};
					if (openFileDialog.ShowDialog(App.Current.MainWindow).GetValueOrDefault())
					{
						PdfImageObject pdfImageObject;
						if (this.Isseleted)
						{
							pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.Document.Pages[this.Pageindex].PageObjects.Count - 1] as PdfImageObject;
						}
						else
						{
							pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
						}
						if (pdfImageObject != null)
						{
							if (new FileInfo(openFileDialog.FileName).Length == 0L)
							{
								ModernMessageBox.Show(pdfeditor.Properties.Resources.UnsupportedImageMsg, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							else
							{
								global::System.Drawing.Image image = global::System.Drawing.Image.FromFile(openFileDialog.FileName);
								if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate90)
								{
									image.RotateFlip(RotateFlipType.Rotate270FlipNone);
								}
								else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate180)
								{
									image.RotateFlip(RotateFlipType.Rotate180FlipNone);
								}
								else if (this.Document.Pages[this.Pageindex].Rotation == PageRotate.Rotate270)
								{
									image.RotateFlip(RotateFlipType.Rotate90FlipNone);
								}
								PdfBitmap pdfBitmap = PdfBitmap.FromBitmap((Bitmap)image);
								this.ReplaceImageCmd(pdfImageObject, this.Pageindex, pdfBitmap, this);
								PdfImageObject pdfImageObject2 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
								this.ImageRect = new FS_RECTF
								{
									right = pdfImageObject2.BoundingBox.right,
									left = pdfImageObject2.BoundingBox.left,
									top = pdfImageObject2.BoundingBox.top,
									bottom = pdfImageObject2.BoundingBox.bottom
								};
								this.UpdateImageborder();
							}
						}
					}
				}
			}
			catch
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.UnsupportedImageMsg, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x000631F4 File Offset: 0x000613F4
		public async Task DeleteImageCmd(PdfImageObject imageObject, int pageIndex)
		{
			ImageControl.<>c__DisplayClass36_0 CS$<>8__locals1 = new ImageControl.<>c__DisplayClass36_0();
			CS$<>8__locals1.pageIndex = pageIndex;
			CS$<>8__locals1.imageModel = new PageImageModel(imageObject, CS$<>8__locals1.pageIndex);
			this.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.RemoveAt(CS$<>8__locals1.imageModel.ImageIndex);
			this.Document.Pages[CS$<>8__locals1.pageIndex].GenerateContentAdvance(false);
			await this.Document.Pages[CS$<>8__locals1.pageIndex].TryRedrawPageAsync(default(CancellationToken));
			await this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass36_0.<<DeleteImageCmd>b__0>d <<DeleteImageCmd>b__0>d;
				<<DeleteImageCmd>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<DeleteImageCmd>b__0>d.<>4__this = CS$<>8__locals1;
				<<DeleteImageCmd>b__0>d.doc = doc;
				<<DeleteImageCmd>b__0>d.<>1__state = -1;
				<<DeleteImageCmd>b__0>d.<>t__builder.Start<ImageControl.<>c__DisplayClass36_0.<<DeleteImageCmd>b__0>d>(ref <<DeleteImageCmd>b__0>d);
				return <<DeleteImageCmd>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass36_0.<<DeleteImageCmd>b__1>d <<DeleteImageCmd>b__1>d;
				<<DeleteImageCmd>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<DeleteImageCmd>b__1>d.<>4__this = CS$<>8__locals1;
				<<DeleteImageCmd>b__1>d.doc = doc;
				<<DeleteImageCmd>b__1>d.<>1__state = -1;
				<<DeleteImageCmd>b__1>d.<>t__builder.Start<ImageControl.<>c__DisplayClass36_0.<<DeleteImageCmd>b__1>d>(ref <<DeleteImageCmd>b__1>d);
				return <<DeleteImageCmd>b__1>d.<>t__builder.Task;
			}, "");
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00063248 File Offset: 0x00061448
		private async Task ImageOperationCmd(PdfImageObject imageObject, int pageIndex, int imageIndex, FS_MATRIX matrix)
		{
			ImageControl.<>c__DisplayClass37_0 CS$<>8__locals1 = new ImageControl.<>c__DisplayClass37_0();
			CS$<>8__locals1.pageIndex = pageIndex;
			CS$<>8__locals1.imageIndex = imageIndex;
			CS$<>8__locals1.matrix = matrix;
			CS$<>8__locals1.weakThis = new WeakReference(this);
			CS$<>8__locals1.oldMatrix = imageObject.Matrix;
			imageObject.Matrix = CS$<>8__locals1.matrix;
			this.Document.Pages[CS$<>8__locals1.pageIndex].GenerateContentAdvance(false);
			await this.Document.Pages[CS$<>8__locals1.pageIndex].TryRedrawPageAsync(default(CancellationToken));
			await this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass37_0.<<ImageOperationCmd>b__0>d <<ImageOperationCmd>b__0>d;
				<<ImageOperationCmd>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ImageOperationCmd>b__0>d.<>4__this = CS$<>8__locals1;
				<<ImageOperationCmd>b__0>d.doc = doc;
				<<ImageOperationCmd>b__0>d.<>1__state = -1;
				<<ImageOperationCmd>b__0>d.<>t__builder.Start<ImageControl.<>c__DisplayClass37_0.<<ImageOperationCmd>b__0>d>(ref <<ImageOperationCmd>b__0>d);
				return <<ImageOperationCmd>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass37_0.<<ImageOperationCmd>b__1>d <<ImageOperationCmd>b__1>d;
				<<ImageOperationCmd>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ImageOperationCmd>b__1>d.<>4__this = CS$<>8__locals1;
				<<ImageOperationCmd>b__1>d.doc = doc;
				<<ImageOperationCmd>b__1>d.<>1__state = -1;
				<<ImageOperationCmd>b__1>d.<>t__builder.Start<ImageControl.<>c__DisplayClass37_0.<<ImageOperationCmd>b__1>d>(ref <<ImageOperationCmd>b__1>d);
				return <<ImageOperationCmd>b__1>d.<>t__builder.Task;
			}, "");
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x000632AC File Offset: 0x000614AC
		public async void ReplaceImageCmd(PdfImageObject imageObject, int pageIndex, PdfBitmap pdfBitmap, ImageControl ImageControl)
		{
			ImageControl.<>c__DisplayClass38_0 CS$<>8__locals1 = new ImageControl.<>c__DisplayClass38_0();
			CS$<>8__locals1.weakThis = new WeakReference(this);
			CS$<>8__locals1.imageModel = new PageImageModel(imageObject, pageIndex);
			ImageMatrix replaceImageMatrix = this.GetReplaceImageMatrix(CS$<>8__locals1.imageModel.Matrix, imageObject.Bitmap, pdfBitmap, imageObject, imageObject.BoundingBox.Height, imageObject.BoundingBox.Width);
			PdfImageObject pdfImageObject = PdfImageObject.Create(this.Document, pdfBitmap, replaceImageMatrix.e, replaceImageMatrix.f);
			if ((float)pdfBitmap.Width > imageObject.BoundingBox.Width || (float)pdfBitmap.Height > imageObject.BoundingBox.Height)
			{
				float num;
				if (imageObject.BoundingBox.Height / (float)pdfBitmap.Height >= imageObject.BoundingBox.Width / (float)pdfBitmap.Width)
				{
					num = imageObject.BoundingBox.Width / (float)pdfBitmap.Width;
				}
				else
				{
					num = imageObject.BoundingBox.Height / (float)pdfBitmap.Height;
				}
				FS_MATRIX matrix = pdfImageObject.Matrix;
				float num2 = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
				float num3 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
				float left = imageObject.BoundingBox.left;
				float bottom = imageObject.BoundingBox.bottom;
				matrix.Translate(-num2, -num3, false);
				matrix.Scale(num, num, false);
				float num4 = (float)pdfBitmap.Width * num;
				float num5 = (float)pdfBitmap.Height * num;
				FS_RECTF fs_RECTF = new FS_RECTF(left, bottom + num5, left + num4, bottom);
				num2 = fs_RECTF.right - fs_RECTF.Width / 2f + (imageObject.BoundingBox.Width - num4) / 2f;
				num3 = fs_RECTF.top - fs_RECTF.Height / 2f + (imageObject.BoundingBox.Height - num5) / 2f;
				matrix.Translate(num2, num3, false);
				pdfImageObject.Matrix = matrix;
			}
			this.Document.Pages[pageIndex].PageObjects.RemoveAt(CS$<>8__locals1.imageModel.ImageIndex);
			this.Document.Pages[CS$<>8__locals1.imageModel.ImagePageIndex].PageObjects.Insert(CS$<>8__locals1.imageModel.ImageIndex, pdfImageObject);
			CS$<>8__locals1.replaceitem = new PageImageModel(pdfImageObject, CS$<>8__locals1.imageModel.ImagePageIndex);
			this.Document.Pages[pageIndex].GenerateContentAdvance(false);
			await this.Document.Pages[pageIndex].TryRedrawPageAsync(default(CancellationToken));
			await this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass38_0.<<ReplaceImageCmd>b__0>d <<ReplaceImageCmd>b__0>d;
				<<ReplaceImageCmd>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ReplaceImageCmd>b__0>d.<>4__this = CS$<>8__locals1;
				<<ReplaceImageCmd>b__0>d.doc = doc;
				<<ReplaceImageCmd>b__0>d.<>1__state = -1;
				<<ReplaceImageCmd>b__0>d.<>t__builder.Start<ImageControl.<>c__DisplayClass38_0.<<ReplaceImageCmd>b__0>d>(ref <<ReplaceImageCmd>b__0>d);
				return <<ReplaceImageCmd>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass38_0.<<ReplaceImageCmd>b__1>d <<ReplaceImageCmd>b__1>d;
				<<ReplaceImageCmd>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ReplaceImageCmd>b__1>d.<>4__this = CS$<>8__locals1;
				<<ReplaceImageCmd>b__1>d.doc = doc;
				<<ReplaceImageCmd>b__1>d.<>1__state = -1;
				<<ReplaceImageCmd>b__1>d.<>t__builder.Start<ImageControl.<>c__DisplayClass38_0.<<ReplaceImageCmd>b__1>d>(ref <<ReplaceImageCmd>b__1>d);
				return <<ReplaceImageCmd>b__1>d.<>t__builder.Task;
			}, "");
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x000632FC File Offset: 0x000614FC
		private ImageMatrix GetReplaceImageMatrix(ImageMatrix imageMatrix, PdfBitmap pdfbitmap1, PdfBitmap pdfbitmap2, PdfImageObject imageObject, float heigth, float width)
		{
			float num2;
			float num3;
			if (pdfbitmap2.Height < 200 && pdfbitmap2.Width < 200 && imageObject.BoundingBox.Height > 200f && imageObject.BoundingBox.Width > 200f)
			{
				if (pdfbitmap2.Height < pdfbitmap2.Width)
				{
					float num = 200f / (float)pdfbitmap2.Height;
					num2 = (float)pdfbitmap2.Width / imageObject.BoundingBox.Width * num;
					num3 = (float)pdfbitmap2.Height / imageObject.BoundingBox.Height * num;
				}
				else
				{
					float num4 = 200f / (float)pdfbitmap2.Width;
					num2 = (float)pdfbitmap2.Width / imageObject.BoundingBox.Width * num4;
					num3 = (float)pdfbitmap2.Height / imageObject.BoundingBox.Height * num4;
				}
			}
			else
			{
				num2 = (float)pdfbitmap2.Width / imageObject.BoundingBox.Width;
				num3 = (float)pdfbitmap2.Height / imageObject.BoundingBox.Height;
			}
			float num5 = 0f;
			float num6 = 0f;
			if (num2 > 1f)
			{
				num3 /= num2;
				num2 /= num2;
			}
			if (num3 > 1f)
			{
				num2 /= num3;
				num3 /= num3;
			}
			if (num2 < 1f && num3 < 1f)
			{
				num5 = (imageObject.BoundingBox.Width - (float)pdfbitmap2.Width) * (width / imageObject.BoundingBox.Width) / 2f;
				num6 = (imageObject.BoundingBox.Height - (float)pdfbitmap2.Height) * (heigth / imageObject.BoundingBox.Height) / 2f;
			}
			else if (num2 < 1f)
			{
				num5 = (imageObject.BoundingBox.Width - (float)pdfbitmap2.Width * (imageObject.BoundingBox.Height / (float)pdfbitmap2.Height)) * (width / imageObject.BoundingBox.Width) / 2f;
			}
			else if (num3 < 1f)
			{
				num6 = (imageObject.BoundingBox.Height - (float)pdfbitmap2.Height * (imageObject.BoundingBox.Width / (float)pdfbitmap2.Width)) * (heigth / imageObject.BoundingBox.Height) / 2f;
			}
			float num7 = imageObject.BoundingBox.right - imageObject.BoundingBox.Width / 2f;
			float num8 = imageObject.BoundingBox.top - imageObject.BoundingBox.Height / 2f;
			if (imageMatrix.e - num7 >= 0f)
			{
				num5 = -num5;
			}
			if (imageMatrix.f - num8 >= 0f)
			{
				num6 = -num6;
			}
			return new ImageMatrix(imageMatrix.a * num2, imageMatrix.b, imageMatrix.c, imageMatrix.d * num3, imageMatrix.e + num5, imageMatrix.f + num6);
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00063630 File Offset: 0x00061830
		private async Task SaveImageAsync(PdfImageObject imageObject)
		{
			DocumentWrapper documentWrapper = this.VM.DocumentWrapper;
			FileInfo fileInfo = new FileInfo((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			string fullPathWithoutPrefix = fileInfo.DirectoryName.FullPathWithoutPrefix;
			string text = fileInfo.Name;
			if (!string.IsNullOrEmpty(fileInfo.Extension))
			{
				text = text.Substring(0, text.Length - fileInfo.Extension.Length);
			}
			if (text.Length > 48)
			{
				text += string.Format(" [{0}].jpg", this.Pageindex + 1);
			}
			else
			{
				text += string.Format(" Image[{0}].jpg", this.Pageindex + 1);
			}
			if (text.Length > 128)
			{
				string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
				string text3 = text2 + " Image.jpg";
				int num = 0;
				try
				{
					while (File.Exists(global::System.IO.Path.Combine(fullPathWithoutPrefix, text3)))
					{
						num++;
						text3 = text2 + string.Format(" Image ({0}).jpg", num);
					}
					text = text3;
				}
				catch
				{
					text = text2 + " Image.pdf";
				}
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp|Png Image|*.png",
				CreatePrompt = false,
				OverwritePrompt = true,
				InitialDirectory = fullPathWithoutPrefix,
				FileName = text
			};
			if (saveFileDialog.ShowDialog().Value)
			{
				global::System.Drawing.Image image = imageObject.Bitmap.Image;
				if (global::System.IO.Path.GetExtension(saveFileDialog.FileName) == ".png")
				{
					image.Save(saveFileDialog.FileName, ImageFormat.Png);
				}
				else if (global::System.IO.Path.GetExtension(saveFileDialog.FileName) == ".jpg")
				{
					image.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
				}
				else
				{
					image.Save(saveFileDialog.FileName, ImageFormat.Bmp);
				}
				await new FileInfo(saveFileDialog.FileName).ShowInExplorerAsync(default(CancellationToken));
			}
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0006367C File Offset: 0x0006187C
		private async Task ImageChangeHierarchyCmd(PdfImageObject imageObject, int pageIndex, int newImageIndex, int oldImageIndex)
		{
			ImageControl.<>c__DisplayClass41_0 CS$<>8__locals1 = new ImageControl.<>c__DisplayClass41_0();
			CS$<>8__locals1.pageIndex = pageIndex;
			CS$<>8__locals1.newImageIndex = newImageIndex;
			CS$<>8__locals1.oldImageIndex = oldImageIndex;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.weakThis = new WeakReference(this);
			PdfImageObject pdfImageObject = PdfImageObject.Create(this.Document, imageObject.Bitmap, 0f, 0f);
			pdfImageObject.Matrix = imageObject.Matrix;
			pdfImageObject.SoftMask = imageObject.SoftMask;
			if (imageObject.Stream != null && imageObject.Stream.Dictionary.ContainsKey("SMask") && imageObject.Stream.Dictionary["SMask"].Is<PdfTypeStream>())
			{
				PdfTypeStream pdfTypeStream = imageObject.Stream.Dictionary["SMask"].As<PdfTypeStream>(true);
				pdfImageObject.Stream.Dictionary.Add("SMask", pdfTypeStream);
			}
			this.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Remove(imageObject);
			this.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Insert(CS$<>8__locals1.newImageIndex, pdfImageObject);
			this.imageindex = CS$<>8__locals1.newImageIndex;
			if (this.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Count - 1 == 0)
			{
				this.TopHierarchyBtn.IsEnabled = false;
				this.BottomHierarchyBtn.IsEnabled = false;
			}
			else if (CS$<>8__locals1.newImageIndex == 0)
			{
				this.TopHierarchyBtn.IsEnabled = true;
				this.BottomHierarchyBtn.IsEnabled = false;
			}
			else if (CS$<>8__locals1.newImageIndex == this.Document.Pages[CS$<>8__locals1.pageIndex].PageObjects.Count - 1)
			{
				this.TopHierarchyBtn.IsEnabled = false;
				this.BottomHierarchyBtn.IsEnabled = true;
			}
			else
			{
				this.TopHierarchyBtn.IsEnabled = true;
				this.BottomHierarchyBtn.IsEnabled = true;
			}
			this.Document.Pages[CS$<>8__locals1.pageIndex].GenerateContentAdvance(false);
			await this.Document.Pages[CS$<>8__locals1.pageIndex].TryRedrawPageAsync(default(CancellationToken));
			await this.VM.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass41_0.<<ImageChangeHierarchyCmd>b__0>d <<ImageChangeHierarchyCmd>b__0>d;
				<<ImageChangeHierarchyCmd>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ImageChangeHierarchyCmd>b__0>d.<>4__this = CS$<>8__locals1;
				<<ImageChangeHierarchyCmd>b__0>d.doc = doc;
				<<ImageChangeHierarchyCmd>b__0>d.<>1__state = -1;
				<<ImageChangeHierarchyCmd>b__0>d.<>t__builder.Start<ImageControl.<>c__DisplayClass41_0.<<ImageChangeHierarchyCmd>b__0>d>(ref <<ImageChangeHierarchyCmd>b__0>d);
				return <<ImageChangeHierarchyCmd>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				ImageControl.<>c__DisplayClass41_0.<<ImageChangeHierarchyCmd>b__1>d <<ImageChangeHierarchyCmd>b__1>d;
				<<ImageChangeHierarchyCmd>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ImageChangeHierarchyCmd>b__1>d.<>4__this = CS$<>8__locals1;
				<<ImageChangeHierarchyCmd>b__1>d.doc = doc;
				<<ImageChangeHierarchyCmd>b__1>d.<>1__state = -1;
				<<ImageChangeHierarchyCmd>b__1>d.<>t__builder.Start<ImageControl.<>c__DisplayClass41_0.<<ImageChangeHierarchyCmd>b__1>d>(ref <<ImageChangeHierarchyCmd>b__1>d);
				return <<ImageChangeHierarchyCmd>b__1>d.<>t__builder.Task;
			}, "");
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x000636E0 File Offset: 0x000618E0
		private async Task<WriteableBitmap> GetImageAsync(PdfBitmap pdfBitmap)
		{
			return await pdfBitmap.ToWriteableBitmapAsync(default(CancellationToken));
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x00063723 File Offset: 0x00061923
		private void Topleft_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ImageControlMoveImage(this.clickStartPosition);
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x00063731 File Offset: 0x00061931
		private void editorImagebtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "EditBtn", "Count", 1L);
			this.editorImageControl();
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x0006374F File Offset: 0x0006194F
		private void quitBtn_Click(object sender, RoutedEventArgs e)
		{
			this.quitImageControl();
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x00063758 File Offset: 0x00061958
		public void editorImageControl()
		{
			this.ImageControlState = true;
			this.SiderEditorbar.Visibility = Visibility.Visible;
			this.SiderAcitonbar.Visibility = Visibility.Collapsed;
			this.SiderEditorActionBtn.Visibility = Visibility.Collapsed;
			this.selectedBoder.Visibility = Visibility.Collapsed;
			this.editorBoder.Visibility = Visibility.Visible;
			this.VM.AnnotationToolbar.ImageButtonModel.IsChecked = true;
			this.UpdateImageborder();
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x000637C4 File Offset: 0x000619C4
		public void quitImageControl()
		{
			GAManager.SendEvent("ImageAction", "QuitBtn", "Count", 1L);
			this.ImageControlState = false;
			this.SiderEditorActionBtn.Visibility = Visibility.Visible;
			this.SiderEditorbar.Visibility = Visibility.Collapsed;
			this.SiderAcitonbar.Visibility = Visibility.Visible;
			this.selectedBoder.Visibility = Visibility.Visible;
			this.editorBoder.Visibility = Visibility.Collapsed;
			this.DeleteBtn.Visibility = Visibility.Collapsed;
			this.VM.AnnotationToolbar.ImageButtonModel.IsChecked = false;
			if (this.VM.AnnotationMode == AnnotationMode.Image)
			{
				this.VM.AnnotationMode = AnnotationMode.None;
			}
			this.UpdateImageborder();
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x00063870 File Offset: 0x00061A70
		private async void Flip_HorizontalBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "FlipHorizontalImage", "Count", 1L);
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				FS_MATRIX matrix = pdfImageObject.Matrix;
				float num = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
				float num2 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
				matrix.Translate(-num, -num2, false);
				FS_MATRIX fs_MATRIX = new FS_MATRIX(-1f, 0f, 0f, 1f, 0f, 0f);
				matrix.Concat(fs_MATRIX, false);
				matrix.Translate(num, num2, false);
				await this.ImageOperationCmd(pdfImageObject, this.Pageindex, this.imageindex, matrix);
				PdfImageObject pdfImageObject2 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				this.ImageRect = new FS_RECTF
				{
					right = pdfImageObject2.BoundingBox.right,
					left = pdfImageObject2.BoundingBox.left,
					top = pdfImageObject2.BoundingBox.top,
					bottom = pdfImageObject2.BoundingBox.bottom
				};
				this.UpdateImageborder();
			}
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x000638A8 File Offset: 0x00061AA8
		private async void Flip_VerticalBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ImageAction", "FlipVerticalImage", "Count", 1L);
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				FS_MATRIX matrix = pdfImageObject.Matrix;
				float num = pdfImageObject.BoundingBox.right - pdfImageObject.BoundingBox.Width / 2f;
				float num2 = pdfImageObject.BoundingBox.top - pdfImageObject.BoundingBox.Height / 2f;
				matrix.Translate(-num, -num2, false);
				FS_MATRIX fs_MATRIX = new FS_MATRIX(1f, 0f, 0f, -1f, 0f, 0f);
				matrix.Concat(fs_MATRIX, false);
				matrix.Translate(num, num2, false);
				await this.ImageOperationCmd(pdfImageObject, this.Pageindex, this.imageindex, matrix);
				PdfImageObject pdfImageObject2 = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
				this.ImageRect = new FS_RECTF
				{
					right = pdfImageObject2.BoundingBox.right,
					left = pdfImageObject2.BoundingBox.left,
					top = pdfImageObject2.BoundingBox.top,
					bottom = pdfImageObject2.BoundingBox.bottom
				};
				this.UpdateImageborder();
			}
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x000638DF File Offset: 0x00061ADF
		private void HierarchyBtn_Click(object sender, RoutedEventArgs e)
		{
			this.UpdateHierarchyBtnIsenabled();
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x000638E8 File Offset: 0x00061AE8
		private void UpdateHierarchyBtnIsenabled()
		{
			if (this.Document.Pages[this.Pageindex].PageObjects.Count - 1 == 0)
			{
				this.TopHierarchyBtn.IsEnabled = false;
				this.BottomHierarchyBtn.IsEnabled = false;
				return;
			}
			if (this.imageindex == 0)
			{
				this.TopHierarchyBtn.IsEnabled = true;
				this.BottomHierarchyBtn.IsEnabled = false;
				return;
			}
			if (this.imageindex == this.Document.Pages[this.Pageindex].PageObjects.Count - 1)
			{
				this.TopHierarchyBtn.IsEnabled = false;
				this.BottomHierarchyBtn.IsEnabled = true;
				return;
			}
			this.TopHierarchyBtn.IsEnabled = true;
			this.BottomHierarchyBtn.IsEnabled = true;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x000639AE File Offset: 0x00061BAE
		private void HierarchyMenu_MouseLeave(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x000639B0 File Offset: 0x00061BB0
		private async void TopHierarchyBtn_Click(object sender, RoutedEventArgs e)
		{
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				GAManager.SendEvent("ImageAction", "SetToTop", "Count", 1L);
				await this.ImageChangeHierarchyCmd(pdfImageObject, this.Pageindex, this.Document.Pages[this.Pageindex].PageObjects.Count - 1, this.imageindex);
			}
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x000639E8 File Offset: 0x00061BE8
		private async void FrontHierarchyBtn_Click(object sender, RoutedEventArgs e)
		{
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				await this.ImageChangeHierarchyCmd(pdfImageObject, this.Pageindex, this.imageindex + 1, this.imageindex);
			}
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x00063A20 File Offset: 0x00061C20
		private async void BackHierarchyBtn_Click(object sender, RoutedEventArgs e)
		{
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				await this.ImageChangeHierarchyCmd(pdfImageObject, this.Pageindex, this.imageindex - 1, this.imageindex);
			}
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x00063A58 File Offset: 0x00061C58
		private async void BottomHierarchyBtn_Click(object sender, RoutedEventArgs e)
		{
			PdfImageObject pdfImageObject = this.Document.Pages[this.Pageindex].PageObjects[this.imageindex] as PdfImageObject;
			if (pdfImageObject != null)
			{
				GAManager.SendEvent("ImageAction", "SetToBottom", "Count", 1L);
				await this.ImageChangeHierarchyCmd(pdfImageObject, this.Pageindex, 0, this.imageindex);
			}
		}

		// Token: 0x04000877 RID: 2167
		private AnnotationCanvas annotationCanvas;

		// Token: 0x04000878 RID: 2168
		private PdfViewerContextMenu contextMenu;

		// Token: 0x04000879 RID: 2169
		private PdfDocument Document;

		// Token: 0x0400087A RID: 2170
		public int Pageindex = -1;

		// Token: 0x0400087B RID: 2171
		public int imageindex = -1;

		// Token: 0x0400087C RID: 2172
		private PdfViewer PdfViewer;

		// Token: 0x0400087D RID: 2173
		public FS_RECTF ImageRect;

		// Token: 0x0400087E RID: 2174
		public bool Isseleted;

		// Token: 0x0400087F RID: 2175
		private PageImageModel PageImageModel;

		// Token: 0x04000880 RID: 2176
		public bool IsMoved;

		// Token: 0x04000881 RID: 2177
		private double Left;

		// Token: 0x04000882 RID: 2178
		private double Top;

		// Token: 0x04000883 RID: 2179
		public ImageControl.MousePosition mousePosition = ImageControl.MousePosition.None;

		// Token: 0x04000884 RID: 2180
		public global::System.Windows.Point clickStartPosition;

		// Token: 0x04000885 RID: 2181
		public bool ImageControlState;

		// Token: 0x020005D8 RID: 1496
		public enum MousePosition
		{
			// Token: 0x04001F7E RID: 8062
			topLeft,
			// Token: 0x04001F7F RID: 8063
			topRight,
			// Token: 0x04001F80 RID: 8064
			bottomLeft,
			// Token: 0x04001F81 RID: 8065
			bottomRight,
			// Token: 0x04001F82 RID: 8066
			topCenter,
			// Token: 0x04001F83 RID: 8067
			leftCenter,
			// Token: 0x04001F84 RID: 8068
			rightCenter,
			// Token: 0x04001F85 RID: 8069
			bottomCenter,
			// Token: 0x04001F86 RID: 8070
			None
		}
	}
}
