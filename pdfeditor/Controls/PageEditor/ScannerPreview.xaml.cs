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
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000254 RID: 596
	public partial class ScannerPreview : Window
	{
		// Token: 0x06002280 RID: 8832 RVA: 0x000A27D4 File Offset: 0x000A09D4
		public ScannerPreview(InsertFromScannerViewModel pagesView)
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
			KeyboardHook.KeyDown += this.KeyboardHook_KeyDown;
			KeyboardHook.Start();
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x000A283C File Offset: 0x000A0A3C
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

		// Token: 0x06002282 RID: 8834 RVA: 0x000A2870 File Offset: 0x000A0A70
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			BindingOperations.ClearAllBindings(this);
			KeyboardHook.KeyDown -= this.KeyboardHook_KeyDown;
			KeyboardHook.Stop();
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x000A2898 File Offset: 0x000A0A98
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

		// Token: 0x06002284 RID: 8836 RVA: 0x000A29D4 File Offset: 0x000A0BD4
		private void ZoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			base.Dispatcher.InvokeAsync(new Action(this.ScaleForComboBox), DispatcherPriority.Background);
			this.ScaleForComboBox();
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x000A29F5 File Offset: 0x000A0BF5
		private void ZoomTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.ScaleForComboBox();
			}
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x000A2A06 File Offset: 0x000A0C06
		private void ZoomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			this.ScaleForComboBox();
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x000A2A10 File Offset: 0x000A0C10
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
					double num3 = ((double)this.canvasWidth - (double)this.canvasWidth * num2) / 2.0;
					double num4 = ((double)this.canvasHeight - (double)this.canvasHeight * num2) / 2.0;
					this.image.SetValue(Canvas.LeftProperty, num3);
					this.image.SetValue(Canvas.TopProperty, num4);
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

		// Token: 0x06002288 RID: 8840 RVA: 0x000A2BCC File Offset: 0x000A0DCC
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

		// Token: 0x06002289 RID: 8841 RVA: 0x000A2C90 File Offset: 0x000A0E90
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

		// Token: 0x0600228A RID: 8842 RVA: 0x000A2CF4 File Offset: 0x000A0EF4
		private void image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			global::System.Windows.Controls.Image image = sender as global::System.Windows.Controls.Image;
			this._isMouseDown = true;
			this._mouseDownPosition = e.GetPosition(this);
			this._mouseDownControlPosition = new global::System.Windows.Point(double.IsNaN(Canvas.GetLeft(image)) ? 0.0 : Canvas.GetLeft(image), double.IsNaN(Canvas.GetTop(image)) ? 0.0 : Canvas.GetTop(image));
			image.CaptureMouse();
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x000A2D6C File Offset: 0x000A0F6C
		private void image_MouseMove(object sender, MouseEventArgs e)
		{
			if (this._isMouseDown)
			{
				global::System.Windows.Controls.Image image = sender as global::System.Windows.Controls.Image;
				Vector vector = e.GetPosition(this) - this._mouseDownPosition;
				Canvas.SetLeft(image, this._mouseDownControlPosition.X + vector.X);
				Canvas.SetTop(image, this._mouseDownControlPosition.Y + vector.Y);
			}
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x000A2DCB File Offset: 0x000A0FCB
		private void image_MouseUp(object sender, MouseButtonEventArgs e)
		{
			UIElement uielement = sender as global::System.Windows.Controls.Image;
			this._isMouseDown = false;
			uielement.ReleaseMouseCapture();
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x000A2DE0 File Offset: 0x000A0FE0
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

		// Token: 0x0600228E RID: 8846 RVA: 0x000A2F08 File Offset: 0x000A1108
		private void ScaleByMouseWheel(global::System.Windows.Point mousePoint, double scale)
		{
			Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(this.image);
			Rect rect = this.image.TransformToAncestor(this.CanvasForPreView).TransformBounds(descendantBounds);
			double num;
			double num2;
			if (this.ViewModel.PreviewPage.Rotate == 90 || this.ViewModel.PreviewPage.Rotate == 270)
			{
				num = rect.Height * scale;
				num2 = rect.Width * scale;
				double num3 = mousePoint.X - (mousePoint.X - rect.Left) * scale - (num - num2) / 2.0;
				double num4 = mousePoint.Y - (mousePoint.Y - rect.Top) * scale + (num - num2) / 2.0;
				this.image.SetValue(Canvas.LeftProperty, num3);
				this.image.SetValue(Canvas.TopProperty, num4);
			}
			else
			{
				num = rect.Width * scale;
				num2 = rect.Height * scale;
				double num3 = mousePoint.X - (mousePoint.X - rect.Left) * scale;
				double num4 = mousePoint.Y - (mousePoint.Y - rect.Top) * scale;
				this.image.SetValue(Canvas.LeftProperty, num3);
				this.image.SetValue(Canvas.TopProperty, num4);
			}
			this.image.Width = num;
			this.image.Height = num2;
		}

		// Token: 0x04000EA5 RID: 3749
		public InsertFromScannerViewModel ViewModel;

		// Token: 0x04000EA6 RID: 3750
		private TextBox zoomTextBox;

		// Token: 0x04000EA7 RID: 3751
		private string lastZoomText;

		// Token: 0x04000EA8 RID: 3752
		private int canvasHeight = 728;

		// Token: 0x04000EA9 RID: 3753
		private int canvasWidth = 1200;

		// Token: 0x04000EAA RID: 3754
		private bool _isMouseDown;

		// Token: 0x04000EAB RID: 3755
		private global::System.Windows.Point _mouseDownPosition;

		// Token: 0x04000EAC RID: 3756
		private global::System.Windows.Point _mouseDownControlPosition;
	}
}
