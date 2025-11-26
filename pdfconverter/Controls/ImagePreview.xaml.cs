using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using HandyControl.Data;
using HandyControl.Tools;
using pdfconverter.ViewModels;

namespace pdfconverter.Controls
{
	// Token: 0x020000A0 RID: 160
	public partial class ImagePreview : Window
	{
		// Token: 0x060006F6 RID: 1782 RVA: 0x00018C00 File Offset: 0x00016E00
		public ImagePreview(ImageToPDFViewModel pagesView)
		{
			base.DataContext = pagesView;
			this.ViewModel = pagesView;
			this.InitializeComponent();
			base.Loaded += delegate(object s, RoutedEventArgs e)
			{
				TextBox textBox = this.zoomComboBox.Template.FindName("PART_EditableTextBox", this.zoomComboBox) as TextBox;
				if (textBox != null)
				{
					this.zoomComboBox.Text = string.Format("{0:#0}%", 100);
					this.lastZoomText = string.Format("{0:#0}%", 100);
					textBox.PreviewKeyDown += this.ZoomTextBox_PreviewKeyDown;
					textBox.LostFocus += this.ZoomTextBox_LostFocus;
					this.zoomTextBox = textBox;
				}
			};
			this.LoadedPreBorder();
			KeyboardHook.KeyDown += this.KeyboardHook_KeyDown;
			KeyboardHook.Start();
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00018C6C File Offset: 0x00016E6C
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			DependencyObject dependencyObject = FocusManager.GetFocusedElement(this) as DependencyObject;
			if (dependencyObject != null)
			{
				FocusManager.SetFocusedElement(FocusManager.GetFocusScope(dependencyObject), null);
			}
			Keyboard.ClearFocus();
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00018CA0 File Offset: 0x00016EA0
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			BindingOperations.ClearAllBindings(this);
			KeyboardHook.KeyDown -= this.KeyboardHook_KeyDown;
			KeyboardHook.Stop();
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00018CC8 File Offset: 0x00016EC8
		public void LoadedPreBorder()
		{
			double num = this.ViewModel.PreviewPage.Thumbnail2.Width;
			double num2 = this.ViewModel.PreviewPage.Thumbnail2.Height;
			double num3 = num2 / num;
			num2 = (double)this.canvasHeight;
			num = num2 / num3;
			double num4 = ((double)this.canvasWidth - num) / 2.0;
			double num5 = ((double)this.canvasHeight - num2) / 2.0;
			this.PreBorder.SetValue(Canvas.LeftProperty, num4);
			this.PreBorder.SetValue(Canvas.TopProperty, num5);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00018D68 File Offset: 0x00016F68
		private void ZoomButton_Click(object sender, RoutedEventArgs e)
		{
			int num3;
			if (((Button)sender).Tag.ToString().Equals(bool.TrueString, StringComparison.InvariantCulture))
			{
				int num;
				if (int.TryParse(this.zoomTextBox.Text.Replace("%", "").Trim(), out num))
				{
					this.zoomComboBox.SelectedIndex = -1;
					double num2 = (double)num / 100.0;
					if (num2 <= 7.9)
					{
						num2 += 0.1;
						this.zoomComboBox.Text = string.Format("{0:#0}%", num2 * 100.0);
						this.ScaleForComboBox();
						return;
					}
				}
			}
			else if (int.TryParse(this.zoomTextBox.Text.Replace("%", "").Trim(), out num3))
			{
				this.zoomComboBox.SelectedIndex = -1;
				double num4 = (double)num3 / 100.0;
				if (num4 >= 0.3)
				{
					num4 -= 0.1;
					this.zoomComboBox.Text = string.Format("{0:#0}%", num4 * 100.0);
					this.ScaleForComboBox();
				}
			}
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00018EA4 File Offset: 0x000170A4
		private void ZoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			base.Dispatcher.InvokeAsync(new Action(this.ScaleForComboBox), DispatcherPriority.Background);
			this.ScaleForComboBox();
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00018EC5 File Offset: 0x000170C5
		private void ZoomTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.ScaleForComboBox();
			}
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00018ED6 File Offset: 0x000170D6
		private void ZoomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			this.ScaleForComboBox();
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00018EE0 File Offset: 0x000170E0
		private void ScaleForComboBox()
		{
			if (!base.IsLoaded || !base.IsVisible)
			{
				return;
			}
			int num;
			if (int.TryParse(this.zoomTextBox.Text.Replace("%", "").Trim(), out num))
			{
				double num2 = (double)num / 100.0;
				if (num2 >= 0.2 && num2 <= 8.0)
				{
					this.image.Height = (double)this.canvasHeight * num2;
					this.image.Width = (double)this.canvasWidth * num2;
					double num3 = this.ViewModel.PreviewPage.Thumbnail2.Width;
					double num4 = this.ViewModel.PreviewPage.Thumbnail2.Height;
					double num5 = num4 / num3;
					num4 = (double)this.canvasHeight;
					num3 = num4 / num5;
					double num6 = ((double)this.canvasWidth - num3 * num2) / 2.0;
					double num7 = ((double)this.canvasHeight - num4 * num2) / 2.0;
					this.PreBorder.SetValue(Canvas.LeftProperty, num6);
					this.PreBorder.SetValue(Canvas.TopProperty, num7);
					this.zoomComboBox.Text = string.Format("{0:#0}%", num2 * 100.0);
					this.lastZoomText = string.Format("{0:#0}%", num2 * 100.0);
					if (num2 == 0.2)
					{
						this.ZoomOutBtn.IsEnabled = false;
						this.ZoomInBtn.IsEnabled = true;
						return;
					}
					if (num2 == 8.0)
					{
						this.ZoomOutBtn.IsEnabled = true;
						this.ZoomInBtn.IsEnabled = false;
						return;
					}
					this.ZoomOutBtn.IsEnabled = true;
					this.ZoomInBtn.IsEnabled = true;
					return;
				}
			}
			this.zoomComboBox.Text = this.lastZoomText;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x000190D0 File Offset: 0x000172D0
		private void KeyboardHook_KeyDown(object sender, KeyboardHookEventArgs e)
		{
			if (!base.IsActive)
			{
				return;
			}
			Key key = e.Key;
			if (key != Key.Escape)
			{
				switch (key)
				{
				case Key.Prior:
				case Key.Left:
				case Key.Up:
					if (!(Keyboard.FocusedElement is TextBoxBase))
					{
						base.Dispatcher.InvokeAsync(delegate
						{
							this.ViewModel.PreviousPageCommand.Execute(null);
						});
						return;
					}
					break;
				case Key.Next:
				case Key.Right:
				case Key.Down:
					if (!(Keyboard.FocusedElement is TextBoxBase))
					{
						base.Dispatcher.InvokeAsync(delegate
						{
							this.ViewModel.NextPageCommand.Execute(null);
						});
						return;
					}
					break;
				case Key.End:
				case Key.Home:
					break;
				default:
					return;
				}
			}
			else
			{
				if (Keyboard.FocusedElement is TextBoxBase)
				{
					base.Dispatcher.InvokeAsync(delegate
					{
						DependencyObject dependencyObject = FocusManager.GetFocusedElement(this) as DependencyObject;
						if (dependencyObject != null)
						{
							FocusManager.SetFocusedElement(FocusManager.GetFocusScope(dependencyObject), null);
						}
						Keyboard.ClearFocus();
					});
					return;
				}
				base.Close();
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00019194 File Offset: 0x00017394
		private static BitmapImage GetBitmapImage(Bitmap bitmap)
		{
			BitmapImage bitmapImage2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, ImageFormat.Png);
				memoryStream.Position = 0L;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.EndInit();
				bitmapImage2 = bitmapImage;
			}
			return bitmapImage2;
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x000191F8 File Offset: 0x000173F8
		private void image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			UIElement uielement = sender as global::System.Windows.Controls.Image;
			Border border = (sender as global::System.Windows.Controls.Image).Parent as Border;
			this._isMouseDown = true;
			this._mouseDownPosition = e.GetPosition(this);
			this._mouseDownControlPosition = new global::System.Windows.Point(double.IsNaN(Canvas.GetLeft(border)) ? 0.0 : Canvas.GetLeft(border), double.IsNaN(Canvas.GetTop(border)) ? 0.0 : Canvas.GetTop(border));
			uielement.CaptureMouse();
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00019280 File Offset: 0x00017480
		private void image_MouseMove(object sender, MouseEventArgs e)
		{
			if (this._isMouseDown)
			{
				Border border = (sender as global::System.Windows.Controls.Image).Parent as Border;
				Vector vector = e.GetPosition(this) - this._mouseDownPosition;
				Canvas.SetLeft(border, this._mouseDownControlPosition.X + vector.X);
				Canvas.SetTop(border, this._mouseDownControlPosition.Y + vector.Y);
			}
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x000192E9 File Offset: 0x000174E9
		private void image_MouseUp(object sender, MouseButtonEventArgs e)
		{
			UIElement uielement = sender as global::System.Windows.Controls.Image;
			this._isMouseDown = false;
			uielement.ReleaseMouseCapture();
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00019300 File Offset: 0x00017500
		private void image_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e == null)
			{
				return;
			}
			global::System.Windows.Point position = e.GetPosition(this.CanvasForPreView);
			int num;
			if (int.TryParse(this.zoomTextBox.Text.Replace("%", "").Trim(), out num))
			{
				double num2 = (double)num / 100.0;
				this.zoomComboBox.SelectedIndex = -1;
				if (e.Delta > 0)
				{
					num2 += 0.05;
					if (num2 <= 8.0)
					{
						this.zoomComboBox.Text = string.Format("{0:#0}%", num2 * 100.0);
						this.ScaleByMouseWheel(position, num2 / (num2 - 0.05));
					}
				}
				else if (e.Delta < 0)
				{
					num2 -= 0.05;
					if (num2 >= 0.2)
					{
						this.zoomComboBox.Text = string.Format("{0:#0}%", num2 * 100.0);
						this.ScaleByMouseWheel(position, num2 / (num2 + 0.05));
					}
				}
				e.GetPosition(this.image);
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00019428 File Offset: 0x00017628
		private void ScaleByMouseWheel(global::System.Windows.Point mousePoint, double scale)
		{
			Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(this.PreBorder);
			Rect rect = this.PreBorder.TransformToAncestor(this.CanvasForPreView).TransformBounds(descendantBounds);
			double num;
			double num2;
			if (this.ViewModel.PreviewPage.Rotate == 90 || this.ViewModel.PreviewPage.Rotate == 270)
			{
				num = rect.Height * scale;
				num2 = rect.Width * scale;
				double num3 = mousePoint.X - (mousePoint.X - rect.Left) * scale - (num - num2) / 2.0;
				double num4 = mousePoint.Y - (mousePoint.Y - rect.Top) * scale + (num - num2) / 2.0;
				this.PreBorder.SetValue(Canvas.LeftProperty, num3);
				this.PreBorder.SetValue(Canvas.TopProperty, num4);
			}
			else
			{
				num = rect.Width * scale;
				num2 = rect.Height * scale;
				double num3 = mousePoint.X - (mousePoint.X - rect.Left) * scale;
				double num4 = mousePoint.Y - (mousePoint.Y - rect.Top) * scale;
				this.PreBorder.SetValue(Canvas.LeftProperty, num3);
				this.PreBorder.SetValue(Canvas.TopProperty, num4);
			}
			this.image.Width = num;
			this.image.Height = num2;
		}

		// Token: 0x0400035A RID: 858
		public ImageToPDFViewModel ViewModel;

		// Token: 0x0400035B RID: 859
		private TextBox zoomTextBox;

		// Token: 0x0400035C RID: 860
		private string lastZoomText;

		// Token: 0x0400035D RID: 861
		private int canvasHeight = 728;

		// Token: 0x0400035E RID: 862
		private int canvasWidth = 1200;

		// Token: 0x0400035F RID: 863
		private bool _isMouseDown;

		// Token: 0x04000360 RID: 864
		private global::System.Windows.Point _mouseDownPosition;

		// Token: 0x04000361 RID: 865
		private global::System.Windows.Point _mouseDownControlPosition;
	}
}
