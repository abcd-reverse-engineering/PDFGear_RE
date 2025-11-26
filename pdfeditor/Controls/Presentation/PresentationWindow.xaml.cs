using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CommonLib.Common;
using Patagames.Pdf.Net;
using pdfeditor.Utils;
using pdfeditor.Views;

namespace pdfeditor.Controls.Presentation
{
	// Token: 0x02000233 RID: 563
	public partial class PresentationWindow : Window
	{
		// Token: 0x06001FD9 RID: 8153 RVA: 0x0008FB44 File Offset: 0x0008DD44
		public PresentationWindow(PdfDocument doc, string fileName)
		{
			this.InitializeComponent();
			this.doc = doc;
			this.FileNameText.Text = ((fileName != null) ? fileName.Trim() : null) ?? "";
			this.ImageView.Document = doc;
			this.ImageView.PageIndex = 1;
			this.ImageView.PreviewMouseLeftButtonDown += this.ImageView_PreviewMouseLeftButtonDown;
			base.Loaded += this.PresentationWindow_Loaded;
			base.SizeChanged += this.PresentationWindow_SizeChanged;
			base.StateChanged += this.PresentationWindow_StateChanged;
			this.ExitButton.Click += delegate(object s, RoutedEventArgs a)
			{
				base.Close();
			};
			this.ExitButton2.Click += delegate(object s, RoutedEventArgs a)
			{
				base.Close();
			};
			this.FloatPrevButton.Click += delegate(object s, RoutedEventArgs a)
			{
				this.ShowToolbar();
				this.PrevPage();
			};
			this.FloatNextButton.Click += delegate(object s, RoutedEventArgs a)
			{
				this.ShowToolbar();
				this.NextPage();
			};
			this.UpdatePageIndexText();
		}

		// Token: 0x06001FDA RID: 8154 RVA: 0x0008FC54 File Offset: 0x0008DE54
		protected override async void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
			MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			if (Screen.AllScreens.Length > 1)
			{
				this.shouldCloseWhenDeactivated = false;
				HandleRef handleRef;
				Int32Rect int32Rect;
				Int32Rect int32Rect2;
				bool flag;
				if (mainView != null && ScreenUtils.TryGetMonitorForWindow(mainView, out handleRef) && ScreenUtils.GetMonitorInfo(handleRef.Handle, out int32Rect, out int32Rect2, out flag))
				{
					global::System.Drawing.Point centerWorkArea = new global::System.Drawing.Point(int32Rect2.X + int32Rect2.Width / 2, int32Rect2.Y + int32Rect2.Height / 2);
					FullScreenHelper.SetWindowPositionAndSize(hwndSource.Handle, new global::System.Drawing.Point?(centerWorkArea), null);
					await base.Dispatcher.InvokeAsync(delegate
					{
						FullScreenHelper.SetWindowPositionAndSize(hwndSource.Handle, new global::System.Drawing.Point?(centerWorkArea), null);
					});
				}
			}
			MainView mainView2 = mainView;
			if (mainView2 != null)
			{
				mainView2.Hide();
			}
			FullScreenHelper.SetIsFullScreenEnabled(this, true);
			hwndSource.AddHook(new HwndSourceHook(this.WndProc));
		}

		// Token: 0x06001FDB RID: 8155 RVA: 0x0008FC93 File Offset: 0x0008DE93
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 126 && !this.closing)
			{
				base.Close();
			}
			return IntPtr.Zero;
		}

		// Token: 0x06001FDC RID: 8156 RVA: 0x0008FCAD File Offset: 0x0008DEAD
		private void PresentationWindow_StateChanged(object sender, EventArgs e)
		{
			if (base.WindowState == WindowState.Maximized)
			{
				this.ImageView.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06001FDD RID: 8157 RVA: 0x0008FCC4 File Offset: 0x0008DEC4
		private void PresentationWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			global::System.Windows.Point pixelOffset = PresentationWindow.GetPixelOffset(this.MainGrid);
			this.ImageViewTrans.X = pixelOffset.X;
			this.ImageViewTrans.Y = pixelOffset.Y;
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x0008FD01 File Offset: 0x0008DF01
		private void PresentationWindow_Loaded(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("Present", "Show", "Count", 1L);
			this.ImageView.Opacity = 1.0;
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06001FDF RID: 8159 RVA: 0x0008FD2D File Offset: 0x0008DF2D
		// (set) Token: 0x06001FE0 RID: 8160 RVA: 0x0008FD3A File Offset: 0x0008DF3A
		public int PageIndex
		{
			get
			{
				return this.ImageView.PageIndex;
			}
			set
			{
				if (this.ImageView.PageIndex != value)
				{
					this.ImageView.PageIndex = value;
					this.UpdatePageIndexText();
				}
			}
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x0008FD5C File Offset: 0x0008DF5C
		protected override void OnKeyDown(global::System.Windows.Input.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Escape)
			{
				if (!this.closing)
				{
					base.Close();
					return;
				}
			}
			else
			{
				if (e.Key == Key.Left || e.Key == Key.Up || e.Key == Key.Prior)
				{
					this.PrevPage();
					return;
				}
				if (e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Return || e.Key == Key.Next)
				{
					this.NextPage();
					return;
				}
				if (e.Key == Key.Home)
				{
					this.PageIndex = 0;
					return;
				}
				if (e.Key == Key.End)
				{
					this.PageIndex = this.doc.Pages.Count - 1;
				}
			}
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x0008FE13 File Offset: 0x0008E013
		private void ImageView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.NextPage();
			}
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x0008FE23 File Offset: 0x0008E023
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			base.OnPreviewMouseWheel(e);
			e.Handled = true;
			if (e.Delta > 0)
			{
				this.PrevPage();
				return;
			}
			this.NextPage();
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x0008FE4C File Offset: 0x0008E04C
		protected override void OnMouseMove(global::System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			global::System.Windows.Point position = e.GetPosition(this);
			if (Math.Abs(this.lastShowToolbarMousePosition.X - position.X) > 10.0 || Math.Abs(this.lastShowToolbarMousePosition.Y - position.Y) > 10.0)
			{
				this.lastShowToolbarMousePosition = position;
				this.ShowToolbar();
			}
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x0008FEBC File Offset: 0x0008E0BC
		private void PrevPage()
		{
			if (this.PageIndex > 0)
			{
				int pageIndex = this.PageIndex;
				this.PageIndex = pageIndex - 1;
			}
		}

		// Token: 0x06001FE6 RID: 8166 RVA: 0x0008FEE4 File Offset: 0x0008E0E4
		private void NextPage()
		{
			if (this.PageIndex < this.doc.Pages.Count - 1)
			{
				int pageIndex = this.PageIndex;
				this.PageIndex = pageIndex + 1;
			}
		}

		// Token: 0x06001FE7 RID: 8167 RVA: 0x0008FF1C File Offset: 0x0008E11C
		private void UpdatePageIndexText()
		{
			int count = this.doc.Pages.Count;
			int pageIndex = this.PageIndex;
			bool flag = pageIndex > 0;
			bool flag2 = pageIndex < count - 1;
			string text = string.Format("{0} / {1}", pageIndex + 1, count);
			this.PageText.Text = text;
			this.FloatPrevButton.IsEnabled = flag;
			this.FloatNextButton.IsEnabled = flag2;
		}

		// Token: 0x06001FE8 RID: 8168 RVA: 0x0008FF8C File Offset: 0x0008E18C
		private async Task CloseCoreAsync()
		{
			try
			{
				MainView mainView = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				if (mainView != null)
				{
					await Task.Delay(100);
					mainView.Show();
					mainView.Activate();
				}
				mainView = null;
			}
			catch
			{
			}
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x0008FFC7 File Offset: 0x0008E1C7
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			this.ImageView.Focus();
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x0008FFDC File Offset: 0x0008E1DC
		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			if (this.shouldCloseWhenDeactivated && !this.closing)
			{
				base.Close();
			}
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x0008FFFC File Offset: 0x0008E1FC
		protected override async void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			await this.CloseCoreAsync();
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x0009003B File Offset: 0x0008E23B
		protected override void OnClosing(CancelEventArgs e)
		{
			this.closing = true;
			base.OnClosing(e);
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x0009004C File Offset: 0x0008E24C
		private void ShowToolbar()
		{
			long timestamp = Stopwatch.GetTimestamp();
			if (timestamp - this.lastShowToolbarTicks < 10000000L)
			{
				return;
			}
			this.lastShowToolbarTicks = timestamp;
			Storyboard storyboard = this.toolbarSb;
			if (storyboard != null)
			{
				storyboard.Stop();
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(3.0);
			if (this.toolbarSb == null)
			{
				Storyboard storyboard2 = new Storyboard();
				DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames
				{
					KeyFrames = 
					{
						new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))),
						new DiscreteDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2) + timeSpan)),
						new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4) + timeSpan))
					}
				};
				Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(UIElement.OpacityProperty));
				DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = doubleAnimationUsingKeyFrames.Clone();
				Storyboard.SetTarget(doubleAnimationUsingKeyFrames, this.TopToolbarContainer);
				Storyboard.SetTarget(doubleAnimationUsingKeyFrames2, this.BottomFloatToolbarContainer);
				storyboard2.Children.Add(doubleAnimationUsingKeyFrames);
				storyboard2.Children.Add(doubleAnimationUsingKeyFrames2);
				this.toolbarSb = storyboard2;
			}
			this.toolbarSb.Begin();
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x000901A0 File Offset: 0x0008E3A0
		private static global::System.Windows.Point GetPixelOffset(UIElement UI)
		{
			global::System.Windows.Point point = default(global::System.Windows.Point);
			PresentationSource presentationSource = PresentationSource.FromVisual(UI);
			if (presentationSource != null)
			{
				Visual rootVisual = presentationSource.RootVisual;
				point = UI.TransformToAncestor(rootVisual).Transform(point);
				point = PresentationWindow.ApplyVisualTransform(point, rootVisual, false);
				point = presentationSource.CompositionTarget.TransformToDevice.Transform(point);
				point.X = Math.Round(point.X);
				point.Y = Math.Round(point.Y);
				point = presentationSource.CompositionTarget.TransformFromDevice.Transform(point);
				point = PresentationWindow.ApplyVisualTransform(point, rootVisual, true);
				GeneralTransform generalTransform = rootVisual.TransformToDescendant(UI);
				if (generalTransform != null)
				{
					point = generalTransform.Transform(point);
				}
			}
			return point;
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x00090250 File Offset: 0x0008E450
		private static global::System.Windows.Point ApplyVisualTransform(global::System.Windows.Point point, Visual v, bool inverse)
		{
			bool flag = true;
			return PresentationWindow.TryApplyVisualTransform(point, v, inverse, true, out flag);
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0009026C File Offset: 0x0008E46C
		private static global::System.Windows.Point TryApplyVisualTransform(global::System.Windows.Point point, Visual v, bool inverse, bool throwOnError, out bool success)
		{
			success = true;
			if (v != null)
			{
				Matrix visualTransform = PresentationWindow.GetVisualTransform(v);
				if (inverse)
				{
					if (!throwOnError && !visualTransform.HasInverse)
					{
						success = false;
						return new global::System.Windows.Point(0.0, 0.0);
					}
					visualTransform.Invert();
				}
				point = visualTransform.Transform(point);
			}
			return point;
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x000902C4 File Offset: 0x0008E4C4
		private static Matrix GetVisualTransform(Visual v)
		{
			if (v != null)
			{
				Matrix matrix = Matrix.Identity;
				Transform transform = VisualTreeHelper.GetTransform(v);
				if (transform != null)
				{
					Matrix value = transform.Value;
					matrix = Matrix.Multiply(matrix, value);
				}
				Vector offset = VisualTreeHelper.GetOffset(v);
				matrix.Translate(offset.X, offset.Y);
				return matrix;
			}
			return Matrix.Identity;
		}

		// Token: 0x04000CC1 RID: 3265
		private readonly PdfDocument doc;

		// Token: 0x04000CC2 RID: 3266
		private bool closing;

		// Token: 0x04000CC3 RID: 3267
		private Storyboard toolbarSb;

		// Token: 0x04000CC4 RID: 3268
		private long lastShowToolbarTicks;

		// Token: 0x04000CC5 RID: 3269
		private global::System.Windows.Point lastShowToolbarMousePosition;

		// Token: 0x04000CC6 RID: 3270
		private bool shouldCloseWhenDeactivated = true;
	}
}
