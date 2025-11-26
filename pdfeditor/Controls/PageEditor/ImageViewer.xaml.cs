using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.Common;
using HandyControl.Interactivity;
using HandyControl.Properties.Langs;
using HandyControl.Tools;
using Microsoft.Win32;
using pdfeditor.Models.Scan;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000248 RID: 584
	public partial class ImageViewer : Control
	{
		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x00098951 File Offset: 0x00096B51
		// (set) Token: 0x06002141 RID: 8513 RVA: 0x00098958 File Offset: 0x00096B58
		public static BitmapFrame AdjustingImage { get; set; }

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06002142 RID: 8514 RVA: 0x00098960 File Offset: 0x00096B60
		// (set) Token: 0x06002143 RID: 8515 RVA: 0x00098967 File Offset: 0x00096B67
		public static ImageViewerSource AdjustingSource { get; set; }

		// Token: 0x06002144 RID: 8516 RVA: 0x00098970 File Offset: 0x00096B70
		public ImageViewer()
		{
			base.CommandBindings.Add(new CommandBinding(ControlCommands.Save, new ExecutedRoutedEventHandler(this.ButtonSave_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.Open, new ExecutedRoutedEventHandler(this.ButtonWindowsOpen_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.Restore, new ExecutedRoutedEventHandler(this.ButtonActual_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.Reduce, new ExecutedRoutedEventHandler(this.ButtonReduce_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.Enlarge, new ExecutedRoutedEventHandler(this.ButtonEnlarge_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.RotateLeft, new ExecutedRoutedEventHandler(this.ButtonRotateLeft_OnClick)));
			base.CommandBindings.Add(new CommandBinding(ControlCommands.RotateRight, new ExecutedRoutedEventHandler(this.ButtonRotateRight_OnClick)));
			base.Loaded += delegate(object s, RoutedEventArgs e)
			{
				this._isLoaded = true;
				this.Init();
			};
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x00098AC8 File Offset: 0x00096CC8
		public ImageViewer(Uri uri)
			: this()
		{
			try
			{
				this.ImageSource = BitmapFrame.Create(uri);
				this.ImgPath = uri.AbsolutePath;
				LongPathFile longPathFile = this.ImgPath;
				if (longPathFile.IsExists)
				{
					this.ImgSize = longPathFile.FileInfo.Length;
				}
			}
			catch
			{
				MessageBox.Show(Lang.ErrorImgPath);
			}
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x00098B3C File Offset: 0x00096D3C
		public ImageViewer(string path)
			: this(new Uri(path))
		{
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x00098B4C File Offset: 0x00096D4C
		private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (ImageViewer.isOnSourceChanged)
			{
				return;
			}
			ImageViewer imageViewer = (ImageViewer)d;
			if (ImageViewer.AdjustingImage == e.NewValue)
			{
				BitmapFrame bitmapFrame = e.NewValue as BitmapFrame;
				if (bitmapFrame != null)
				{
					BitmapFrame bitmapFrame2 = e.OldValue as BitmapFrame;
					if (bitmapFrame2 != null && bitmapFrame.PixelWidth == bitmapFrame2.PixelWidth && bitmapFrame.PixelHeight == bitmapFrame2.PixelHeight)
					{
						return;
					}
				}
			}
			imageViewer.ImageScale = (double)ImageViewer.ImageScaleProperty.DefaultMetadata.DefaultValue;
			imageViewer.Init();
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x00098BD4 File Offset: 0x00096DD4
		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
			{
				return;
			}
			try
			{
				ImageViewer.isOnSourceChanged = true;
				ImageViewer imageViewer = (ImageViewer)d;
				ImageViewerSource imageViewerSource = (ImageViewerSource)e.NewValue;
				imageViewer._imgActualRotate = imageViewerSource.Rotate;
				imageViewer._isOblique = ((int)imageViewerSource.Rotate - 90) % 180 == 0;
				imageViewer.ImageRotate = imageViewerSource.Rotate;
				imageViewer.ImageSource = imageViewerSource.Image;
				if (ImageViewer.AdjustingSource != imageViewerSource)
				{
					imageViewer.ImageScale = (double)ImageViewer.ImageScaleProperty.DefaultMetadata.DefaultValue;
					imageViewer.Init();
				}
			}
			finally
			{
				ImageViewer.isOnSourceChanged = false;
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06002149 RID: 8521 RVA: 0x00098C88 File Offset: 0x00096E88
		// (set) Token: 0x0600214A RID: 8522 RVA: 0x00098C9A File Offset: 0x00096E9A
		public bool IsFullScreen
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.IsFullScreenProperty);
			}
			set
			{
				base.SetValue(ImageViewer.IsFullScreenProperty, value);
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x0600214B RID: 8523 RVA: 0x00098CAD File Offset: 0x00096EAD
		// (set) Token: 0x0600214C RID: 8524 RVA: 0x00098CBF File Offset: 0x00096EBF
		public bool ShowImgMap
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.ShowImgMapProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ShowImgMapProperty, value);
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x0600214D RID: 8525 RVA: 0x00098CD2 File Offset: 0x00096ED2
		// (set) Token: 0x0600214E RID: 8526 RVA: 0x00098CE4 File Offset: 0x00096EE4
		public BitmapFrame ImageSource
		{
			get
			{
				return (BitmapFrame)base.GetValue(ImageViewer.ImageSourceProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageSourceProperty, value);
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x0600214F RID: 8527 RVA: 0x00098CF2 File Offset: 0x00096EF2
		// (set) Token: 0x06002150 RID: 8528 RVA: 0x00098D04 File Offset: 0x00096F04
		public bool ShowToolBar
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.ShowToolBarProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ShowToolBarProperty, value);
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06002151 RID: 8529 RVA: 0x00098D17 File Offset: 0x00096F17
		// (set) Token: 0x06002152 RID: 8530 RVA: 0x00098D24 File Offset: 0x00096F24
		internal object ImageContent
		{
			get
			{
				return base.GetValue(ImageViewer.ImageContentProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageContentProperty, value);
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06002153 RID: 8531 RVA: 0x00098D32 File Offset: 0x00096F32
		// (set) Token: 0x06002154 RID: 8532 RVA: 0x00098D44 File Offset: 0x00096F44
		internal string ImgPath
		{
			get
			{
				return (string)base.GetValue(ImageViewer.ImgPathProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImgPathProperty, value);
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06002155 RID: 8533 RVA: 0x00098D52 File Offset: 0x00096F52
		// (set) Token: 0x06002156 RID: 8534 RVA: 0x00098D64 File Offset: 0x00096F64
		internal long ImgSize
		{
			get
			{
				return (long)base.GetValue(ImageViewer.ImgSizeProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImgSizeProperty, value);
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06002157 RID: 8535 RVA: 0x00098D77 File Offset: 0x00096F77
		// (set) Token: 0x06002158 RID: 8536 RVA: 0x00098D89 File Offset: 0x00096F89
		internal bool ShowFullScreenButton
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.ShowFullScreenButtonProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ShowFullScreenButtonProperty, value);
			}
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06002159 RID: 8537 RVA: 0x00098D9C File Offset: 0x00096F9C
		// (set) Token: 0x0600215A RID: 8538 RVA: 0x00098DAE File Offset: 0x00096FAE
		internal Thickness ImageMargin
		{
			get
			{
				return (Thickness)base.GetValue(ImageViewer.ImageMarginProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageMarginProperty, value);
			}
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x0600215B RID: 8539 RVA: 0x00098DC1 File Offset: 0x00096FC1
		// (set) Token: 0x0600215C RID: 8540 RVA: 0x00098DD3 File Offset: 0x00096FD3
		internal double ImageWidth
		{
			get
			{
				return (double)base.GetValue(ImageViewer.ImageWidthProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageWidthProperty, value);
			}
		}

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x0600215D RID: 8541 RVA: 0x00098DE6 File Offset: 0x00096FE6
		// (set) Token: 0x0600215E RID: 8542 RVA: 0x00098DF8 File Offset: 0x00096FF8
		internal double ImageHeight
		{
			get
			{
				return (double)base.GetValue(ImageViewer.ImageHeightProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageHeightProperty, value);
			}
		}

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x0600215F RID: 8543 RVA: 0x00098E0B File Offset: 0x0009700B
		// (set) Token: 0x06002160 RID: 8544 RVA: 0x00098E1D File Offset: 0x0009701D
		internal double ImageScale
		{
			get
			{
				return (double)base.GetValue(ImageViewer.ImageScaleProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageScaleProperty, value);
			}
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06002161 RID: 8545 RVA: 0x00098E30 File Offset: 0x00097030
		// (remove) Token: 0x06002162 RID: 8546 RVA: 0x00098E68 File Offset: 0x00097068
		internal event Action<double> ImageScaleChanged;

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06002163 RID: 8547 RVA: 0x00098E9D File Offset: 0x0009709D
		// (set) Token: 0x06002164 RID: 8548 RVA: 0x00098EAF File Offset: 0x000970AF
		internal string ScaleStr
		{
			get
			{
				return (string)base.GetValue(ImageViewer.ScaleStrProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ScaleStrProperty, value);
			}
		}

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06002165 RID: 8549 RVA: 0x00098EBD File Offset: 0x000970BD
		// (set) Token: 0x06002166 RID: 8550 RVA: 0x00098ECF File Offset: 0x000970CF
		internal double ImageRotate
		{
			get
			{
				return (double)base.GetValue(ImageViewer.ImageRotateProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ImageRotateProperty, value);
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06002167 RID: 8551 RVA: 0x00098EE2 File Offset: 0x000970E2
		// (set) Token: 0x06002168 RID: 8552 RVA: 0x00098EF4 File Offset: 0x000970F4
		internal bool ShowSmallImgInternal
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.ShowSmallImgInternalProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ShowSmallImgInternalProperty, value);
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06002169 RID: 8553 RVA: 0x00098F07 File Offset: 0x00097107
		// (set) Token: 0x0600216A RID: 8554 RVA: 0x00098F19 File Offset: 0x00097119
		internal ImageViewerSource Source
		{
			get
			{
				return (ImageViewerSource)base.GetValue(ImageViewer.SourceProperty);
			}
			set
			{
				base.SetValue(ImageViewer.SourceProperty, value);
			}
		}

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x0600216B RID: 8555 RVA: 0x00098F27 File Offset: 0x00097127
		// (set) Token: 0x0600216C RID: 8556 RVA: 0x00098F2F File Offset: 0x0009712F
		private double ImageOriWidth { get; set; }

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x0600216D RID: 8557 RVA: 0x00098F38 File Offset: 0x00097138
		// (set) Token: 0x0600216E RID: 8558 RVA: 0x00098F40 File Offset: 0x00097140
		private double ImageOriHeight { get; set; }

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x0600216F RID: 8559 RVA: 0x00098F49 File Offset: 0x00097149
		// (set) Token: 0x06002170 RID: 8560 RVA: 0x00098F5B File Offset: 0x0009715B
		internal bool ShowCloseButton
		{
			get
			{
				return (bool)base.GetValue(ImageViewer.ShowCloseButtonProperty);
			}
			set
			{
				base.SetValue(ImageViewer.ShowCloseButtonProperty, value);
			}
		}

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06002171 RID: 8561 RVA: 0x00098F6E File Offset: 0x0009716E
		// (set) Token: 0x06002172 RID: 8562 RVA: 0x00098F78 File Offset: 0x00097178
		internal bool ShowBorderBottom
		{
			get
			{
				return this._showBorderBottom;
			}
			set
			{
				if (this._showBorderBottom == value)
				{
					return;
				}
				Border borderBottom = this._borderBottom;
				if (borderBottom != null)
				{
					borderBottom.BeginAnimation(UIElement.OpacityProperty, value ? AnimationHelper.CreateAnimation(1.0, 100.0) : AnimationHelper.CreateAnimation(0.0, 400.0));
				}
				this._showBorderBottom = value;
			}
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06002173 RID: 8563 RVA: 0x00098FDF File Offset: 0x000971DF
		// (set) Token: 0x06002174 RID: 8564 RVA: 0x00098FE7 File Offset: 0x000971E7
		internal bool MouseWheelEnabled { get; set; } = true;

		// Token: 0x06002175 RID: 8565 RVA: 0x00098FF0 File Offset: 0x000971F0
		public override void OnApplyTemplate()
		{
			if (this._imageMain != null)
			{
				this._imageMain.MouseLeftButtonDown -= this.ImageMain_OnMouseLeftButtonDown;
			}
			if (this._canvasSmallImg != null)
			{
				this._canvasSmallImg.MouseLeftButtonDown -= this.CanvasSmallImg_OnMouseLeftButtonDown;
				this._canvasSmallImg.MouseLeftButtonUp -= this.CanvasSmallImg_OnMouseLeftButtonUp;
				this._canvasSmallImg.MouseMove -= this.CanvasSmallImg_OnMouseMove;
			}
			base.OnApplyTemplate();
			this._panelMain = base.GetTemplateChild("PART_PanelMain") as Panel;
			this._canvasSmallImg = base.GetTemplateChild("PART_CanvasSmallImg") as Canvas;
			this._borderMove = base.GetTemplateChild("PART_BorderMove") as Border;
			this._imageMain = base.GetTemplateChild("PART_ImageMain") as Image;
			this._borderBottom = base.GetTemplateChild("PART_BorderBottom") as Border;
			if (this._imageMain != null)
			{
				RotateTransform rotateTransform = new RotateTransform();
				BindingOperations.SetBinding(rotateTransform, RotateTransform.AngleProperty, new Binding(ImageViewer.ImageRotateProperty.Name)
				{
					Source = this
				});
				this._imageMain.LayoutTransform = rotateTransform;
				this._imageMain.MouseLeftButtonDown += this.ImageMain_OnMouseLeftButtonDown;
			}
			if (this._canvasSmallImg != null)
			{
				this._canvasSmallImg.MouseLeftButtonDown += this.CanvasSmallImg_OnMouseLeftButtonDown;
				this._canvasSmallImg.MouseLeftButtonUp += this.CanvasSmallImg_OnMouseLeftButtonUp;
				this._canvasSmallImg.MouseMove += this.CanvasSmallImg_OnMouseMove;
			}
			this._borderSmallIsLoaded = false;
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x00099184 File Offset: 0x00097384
		private static void OnImageScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ImageViewer imageViewer = d as ImageViewer;
			if (imageViewer != null)
			{
				object newValue = e.NewValue;
				if (newValue is double)
				{
					double num = (double)newValue;
					imageViewer.ImageWidth = imageViewer.ImageOriWidth * num;
					imageViewer.ImageHeight = imageViewer.ImageOriHeight * num;
					imageViewer.ScaleStr = string.Format("{0:#0}%", num * 100.0);
					Action<double> imageScaleChanged = imageViewer.ImageScaleChanged;
					if (imageScaleChanged == null)
					{
						return;
					}
					imageScaleChanged(num);
				}
			}
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x000991FF File Offset: 0x000973FF
		public void Reinit()
		{
			this.ImageScale = (double)ImageViewer.ImageScaleProperty.DefaultMetadata.DefaultValue;
			this.Init();
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x00099224 File Offset: 0x00097424
		private void Init()
		{
			if (this.ImageSource == null || !this._isLoaded)
			{
				return;
			}
			if (this.ImageSource.IsDownloading)
			{
				this._dispatcher = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
				{
					Interval = TimeSpan.FromSeconds(1.0)
				};
				this._dispatcher.Tick += this.Dispatcher_Tick;
				this._dispatcher.Start();
				return;
			}
			double num;
			double num2;
			if (!this._isOblique)
			{
				num = (double)this.ImageSource.PixelWidth;
				num2 = (double)this.ImageSource.PixelHeight;
			}
			else
			{
				num = (double)this.ImageSource.PixelHeight;
				num2 = (double)this.ImageSource.PixelWidth;
			}
			this.ImageWidth = num;
			this.ImageHeight = num2;
			this.ImageOriWidth = num;
			this.ImageOriHeight = num2;
			this._scaleInternalWidth = this.ImageOriWidth * this.ScaleInternal;
			this._scaleInternalHeight = this.ImageOriHeight * this.ScaleInternal;
			if (Math.Abs(num2 - 0.0) < 0.001 || Math.Abs(num - 0.0) < 0.001)
			{
				MessageBox.Show(Lang.ErrorImgSize);
				return;
			}
			this._imgWidHeiScale = num / num2;
			double num3 = base.ActualWidth / base.ActualHeight;
			if (this._imgWidHeiScale > num3)
			{
				if (num > base.ActualWidth)
				{
					this.ImageScale = base.ActualWidth / num;
				}
				else
				{
					this.ImageScale = 1.0;
				}
			}
			else if (num2 > base.ActualHeight)
			{
				this.ImageScale = base.ActualHeight / num2;
			}
			else
			{
				this.ImageScale = 1.0;
			}
			this.ImageMargin = new Thickness((base.ActualWidth - this.ImageWidth) / 2.0, (base.ActualHeight - this.ImageHeight) / 2.0, 0.0, 0.0);
			this._imgActualScale = this.ImageScale;
			this._imgActualMargin = this.ImageMargin;
			this.InitBorderSmall();
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x00099430 File Offset: 0x00097630
		private void Dispatcher_Tick(object sender, EventArgs e)
		{
			if (this._dispatcher == null)
			{
				return;
			}
			if (this.ImageSource == null || !this._isLoaded)
			{
				this._dispatcher.Stop();
				this._dispatcher.Tick -= this.Dispatcher_Tick;
				this._dispatcher = null;
				return;
			}
			if (!this.ImageSource.IsDownloading)
			{
				this._dispatcher.Stop();
				this._dispatcher.Tick -= this.Dispatcher_Tick;
				this._dispatcher = null;
				this.Init();
			}
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x000994BC File Offset: 0x000976BC
		private void ButtonActual_OnClick(object sender, RoutedEventArgs e)
		{
			DoubleAnimation doubleAnimation = AnimationHelper.CreateAnimation(1.0, 200.0);
			doubleAnimation.FillBehavior = FillBehavior.Stop;
			this._imgActualScale = 1.0;
			doubleAnimation.Completed += delegate(object s, EventArgs e1)
			{
				this.ImageScale = 1.0;
				this._canMoveX = this.ImageWidth > this.ActualWidth;
				this._canMoveY = this.ImageHeight > this.ActualHeight;
				this.BorderSmallShowSwitch();
			};
			Thickness thickness = new Thickness((base.ActualWidth - this.ImageOriWidth) / 2.0, (base.ActualHeight - this.ImageOriHeight) / 2.0, 0.0, 0.0);
			ThicknessAnimation thicknessAnimation = AnimationHelper.CreateAnimation(thickness, 200.0);
			thicknessAnimation.FillBehavior = FillBehavior.Stop;
			this._imgActualMargin = thickness;
			thicknessAnimation.Completed += delegate(object s, EventArgs e1)
			{
				this.ImageMargin = thickness;
			};
			base.BeginAnimation(ImageViewer.ImageScaleProperty, doubleAnimation);
			base.BeginAnimation(ImageViewer.ImageMarginProperty, thicknessAnimation);
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x000995B3 File Offset: 0x000977B3
		private void ButtonReduce_OnClick(object sender, RoutedEventArgs e)
		{
			this.ScaleImg(false);
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x000995BC File Offset: 0x000977BC
		private void ButtonEnlarge_OnClick(object sender, RoutedEventArgs e)
		{
			this.ScaleImg(true);
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x000995C5 File Offset: 0x000977C5
		private void ButtonRotateLeft_OnClick(object sender, RoutedEventArgs e)
		{
			this.RotateImg(this._imgActualRotate - 90.0);
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x000995DD File Offset: 0x000977DD
		private void ButtonRotateRight_OnClick(object sender, RoutedEventArgs e)
		{
			this.RotateImg(this._imgActualRotate + 90.0);
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x000995F8 File Offset: 0x000977F8
		private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.ImageSource == null)
			{
				return;
			}
			ImageViewer.SaveFileDialog.FileName = string.Format("{0:yyyy-M-d-h-m-s.fff}", DateTime.Now);
			if (ImageViewer.SaveFileDialog.ShowDialog().GetValueOrDefault())
			{
				using (FileStream fileStream = new FileStream(ImageViewer.SaveFileDialog.FileName, FileMode.Create, FileAccess.Write))
				{
					new PngBitmapEncoder
					{
						Frames = { BitmapFrame.Create(this.ImageSource) }
					}.Save(fileStream);
				}
			}
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x00099690 File Offset: 0x00097890
		private void ButtonWindowsOpen_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(this.ImgPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x000996C8 File Offset: 0x000978C8
		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.MoveImg();
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x000996D0 File Offset: 0x000978D0
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			this.ShowBorderBottom = false;
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x000996D9 File Offset: 0x000978D9
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (this.MouseWheelEnabled)
			{
				this.ScaleImg(e.Delta > 0);
			}
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x000996F2 File Offset: 0x000978F2
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			this.OnRenderSizeChanged();
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x00099704 File Offset: 0x00097904
		private void OnRenderSizeChanged()
		{
			if (this.ImageWidth < 0.001 || this.ImageHeight < 0.001)
			{
				return;
			}
			this._canMoveX = true;
			this._canMoveY = true;
			double num = this.ImageMargin.Left;
			double num2 = this.ImageMargin.Top;
			if (this.ImageWidth <= base.ActualWidth)
			{
				this._canMoveX = false;
				num = (base.ActualWidth - this.ImageWidth) / 2.0;
			}
			if (this.ImageHeight <= base.ActualHeight)
			{
				this._canMoveY = false;
				num2 = (base.ActualHeight - this.ImageHeight) / 2.0;
			}
			this.ImageMargin = new Thickness(num, num2, 0.0, 0.0);
			this._imgActualMargin = this.ImageMargin;
			this.BorderSmallShowSwitch();
			this._imgSmallMouseDownMargin = this._borderMove.Margin;
			this.MoveSmallImg(this._imgSmallMouseDownMargin.Left, this._imgSmallMouseDownMargin.Top);
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x00099818 File Offset: 0x00097A18
		private void ImageMain_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._imgMouseDownPoint = Mouse.GetPosition(this._panelMain);
			this._imgMouseDownMargin = this.ImageMargin;
			this._imgIsMouseLeftButtonDown = true;
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x0009983E File Offset: 0x00097A3E
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			this._imgIsMouseLeftButtonDown = false;
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x00099848 File Offset: 0x00097A48
		private void BorderSmallShowSwitch()
		{
			if (this._canMoveX || this._canMoveY)
			{
				if (!this._borderSmallIsLoaded)
				{
					this._canvasSmallImg.Background = new VisualBrush(this._imageMain);
					this.InitBorderSmall();
					this._borderSmallIsLoaded = true;
				}
				this.ShowSmallImgInternal = true;
				this.UpdateBorderSmall();
				return;
			}
			this.ShowSmallImgInternal = false;
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x000998A8 File Offset: 0x00097AA8
		private void InitBorderSmall()
		{
			if (this._canvasSmallImg == null)
			{
				return;
			}
			double num = this._canvasSmallImg.MaxWidth / this._canvasSmallImg.MaxHeight;
			if (this._imgWidHeiScale > num)
			{
				this._canvasSmallImg.Width = this._canvasSmallImg.MaxWidth;
				this._canvasSmallImg.Height = this._canvasSmallImg.Width / this._imgWidHeiScale;
				return;
			}
			this._canvasSmallImg.Width = this._canvasSmallImg.MaxHeight * this._imgWidHeiScale;
			this._canvasSmallImg.Height = this._canvasSmallImg.MaxHeight;
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x00099948 File Offset: 0x00097B48
		private void UpdateBorderSmall()
		{
			if (!this.ShowSmallImgInternal)
			{
				return;
			}
			double num = Math.Min(this.ImageWidth, base.ActualWidth);
			double num2 = Math.Min(this.ImageHeight, base.ActualHeight);
			this._borderMove.Width = num / this.ImageWidth * this._canvasSmallImg.Width;
			this._borderMove.Height = num2 / this.ImageHeight * this._canvasSmallImg.Height;
			double num3 = -this.ImageMargin.Left / this.ImageWidth * this._canvasSmallImg.Width;
			double num4 = -this.ImageMargin.Top / this.ImageHeight * this._canvasSmallImg.Height;
			double num5 = this._canvasSmallImg.Width - this._borderMove.Width;
			double num6 = this._canvasSmallImg.Height - this._borderMove.Height;
			num3 = Math.Max(0.0, num3);
			num3 = Math.Min(num5, num3);
			num4 = Math.Max(0.0, num4);
			num4 = Math.Min(num6, num4);
			this._borderMove.Margin = new Thickness(num3, num4, 0.0, 0.0);
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x00099A8C File Offset: 0x00097C8C
		public void ScaleImg(bool isEnlarge)
		{
			if (isEnlarge && this._imgActualScale >= this.ScaleMax)
			{
				return;
			}
			if (!isEnlarge && this._imgActualScale <= this.ScaleMin)
			{
				return;
			}
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				return;
			}
			double imageWidth = this.ImageWidth;
			double imageHeight = this.ImageHeight;
			double num = (isEnlarge ? (this._imgActualScale + this.ScaleInternal) : (this._imgActualScale - this.ScaleInternal));
			if (num < this.ScaleMin)
			{
				num = this.ScaleMin;
			}
			else if (num > this.ScaleMax)
			{
				num = this.ScaleMax;
			}
			this.ImageScale = num;
			Point position = Mouse.GetPosition(this._panelMain);
			Point point = new Point(position.X - this._imgActualMargin.Left, position.Y - this._imgActualMargin.Top);
			double num2 = 0.5 * this._scaleInternalWidth;
			double num3 = 0.5 * this._scaleInternalHeight;
			if (this.ImageWidth > base.ActualWidth)
			{
				this._canMoveX = true;
				if (this.ImageHeight > base.ActualHeight)
				{
					this._canMoveY = true;
					num2 = point.X / imageWidth * this._scaleInternalWidth;
					num3 = point.Y / imageHeight * this._scaleInternalHeight;
				}
				else
				{
					this._canMoveY = false;
				}
			}
			else
			{
				this._canMoveY = this.ImageHeight > base.ActualHeight;
				this._canMoveX = false;
			}
			Thickness thickness;
			if (isEnlarge)
			{
				thickness = new Thickness(this._imgActualMargin.Left - num2, this._imgActualMargin.Top - num3, 0.0, 0.0);
			}
			else
			{
				double num4 = this._imgActualMargin.Left + num2;
				double num5 = this._imgActualMargin.Top + num3;
				double num6 = this.ImageWidth - base.ActualWidth;
				double num7 = this.ImageHeight - base.ActualHeight;
				double num8 = Math.Abs(this._borderMove.Width - this._canvasSmallImg.ActualWidth + this._borderMove.Margin.Left);
				double num9 = Math.Abs(this._borderMove.Height - this._canvasSmallImg.ActualHeight + this._borderMove.Margin.Top);
				if (Math.Abs(this.ImageMargin.Left) < 0.001 || num8 < 0.001)
				{
					num4 = this._imgActualMargin.Left + this._borderMove.Margin.Left / (this._canvasSmallImg.ActualWidth - this._borderMove.Width) * this._scaleInternalWidth;
				}
				if (Math.Abs(this.ImageMargin.Top) < 0.001 || num9 < 0.001)
				{
					num5 = this._imgActualMargin.Top + this._borderMove.Margin.Top / (this._canvasSmallImg.ActualHeight - this._borderMove.Height) * this._scaleInternalHeight;
				}
				if (num6 < 0.001)
				{
					num4 = (base.ActualWidth - this.ImageWidth) / 2.0;
				}
				if (num7 < 0.001)
				{
					num5 = (base.ActualHeight - this.ImageHeight) / 2.0;
				}
				thickness = new Thickness(num4, num5, 0.0, 0.0);
			}
			this.ImageMargin = thickness;
			this._imgActualScale = num;
			this._imgActualMargin = thickness;
			this.BorderSmallShowSwitch();
			this._imgSmallMouseDownMargin = this._borderMove.Margin;
			this.MoveSmallImg(this._imgSmallMouseDownMargin.Left, this._imgSmallMouseDownMargin.Top);
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x00099E58 File Offset: 0x00098058
		public void ScaleImgByCenter(double scale)
		{
			if (this.ImageSource == null || !this._isLoaded)
			{
				return;
			}
			double num = scale;
			if (num < this.ScaleMin)
			{
				num = this.ScaleMin;
			}
			else if (num > this.ScaleMax)
			{
				num = this.ScaleMax;
			}
			double actualWidth = this._imageMain.ActualWidth;
			double actualHeight = this._imageMain.ActualHeight;
			Point point = new Point(this._panelMain.ActualWidth / 2.0, this._panelMain.ActualHeight / 2.0);
			Point point2 = this._panelMain.TranslatePoint(point, this._imageMain);
			this.ImageScale = num;
			Thickness thickness = new Thickness(base.ActualWidth / 2.0 - this.ImageWidth * (point2.X / actualWidth), base.ActualHeight / 2.0 - this.ImageHeight * (point2.Y / actualHeight), 0.0, 0.0);
			if (this.ImageWidth < base.ActualWidth || this.ImageHeight < base.ActualHeight)
			{
				thickness.Left = (base.ActualWidth - this.ImageWidth) / 2.0;
				thickness.Top = (base.ActualHeight - this.ImageHeight) / 2.0;
			}
			this.ImageMargin = thickness;
			this._imgActualScale = num;
			this._imgActualMargin = thickness;
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x00099FC8 File Offset: 0x000981C8
		public void RotateImg(double rotate)
		{
			this._imgActualRotate = rotate;
			this._isOblique = ((int)this._imgActualRotate - 90) % 180 == 0;
			this.ShowSmallImgInternal = false;
			this.Init();
			this.InitBorderSmall();
			this.ImageRotate = rotate;
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x0009A004 File Offset: 0x00098204
		private void MoveImg()
		{
			this._imgCurrentPoint = Mouse.GetPosition(this._panelMain);
			this.ShowCloseButton = this._imgCurrentPoint.Y < 200.0;
			this.ShowBorderBottom = this._imgCurrentPoint.Y > base.ActualHeight - 200.0;
			if (Mouse.LeftButton == MouseButtonState.Released)
			{
				return;
			}
			if (this._imgIsMouseLeftButtonDown)
			{
				double num = this._imgCurrentPoint.X - this._imgMouseDownPoint.X;
				double num2 = this._imgCurrentPoint.Y - this._imgMouseDownPoint.Y;
				double num3 = this._imgMouseDownMargin.Left;
				if (this.ImageWidth > base.ActualWidth)
				{
					num3 = this._imgMouseDownMargin.Left + num;
					if (num3 >= 0.0)
					{
						num3 = 0.0;
					}
					else if (-num3 + base.ActualWidth >= this.ImageWidth)
					{
						num3 = base.ActualWidth - this.ImageWidth;
					}
					this._canMoveX = true;
				}
				double num4 = this._imgMouseDownMargin.Top;
				if (this.ImageHeight > base.ActualHeight)
				{
					num4 = this._imgMouseDownMargin.Top + num2;
					if (num4 >= 0.0)
					{
						num4 = 0.0;
					}
					else if (-num4 + base.ActualHeight >= this.ImageHeight)
					{
						num4 = base.ActualHeight - this.ImageHeight;
					}
					this._canMoveY = true;
				}
				this.ImageMargin = new Thickness(num3, num4, 0.0, 0.0);
				this._imgActualMargin = this.ImageMargin;
				this.UpdateBorderSmall();
			}
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x0009A1A4 File Offset: 0x000983A4
		private void MoveSmallImg()
		{
			if (!this._imgSmallIsMouseLeftButtonDown)
			{
				return;
			}
			if (Mouse.LeftButton == MouseButtonState.Released)
			{
				return;
			}
			this._imgSmallCurrentPoint = Mouse.GetPosition(this._canvasSmallImg);
			double num = this._imgSmallCurrentPoint.X - this._imgSmallMouseDownPoint.X;
			double num2 = this._imgSmallCurrentPoint.Y - this._imgSmallMouseDownPoint.Y;
			double num3 = this._imgSmallMouseDownMargin.Left + num;
			double num4 = this._imgSmallMouseDownMargin.Top + num2;
			this.MoveSmallImg(num3, num4);
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x0009A228 File Offset: 0x00098428
		private void MoveSmallImg(double marginX, double marginY)
		{
			if (marginX < 0.0)
			{
				marginX = 0.0;
			}
			else if (marginX + this._borderMove.Width >= this._canvasSmallImg.Width)
			{
				marginX = this._canvasSmallImg.Width - this._borderMove.Width;
			}
			if (marginY < 0.0)
			{
				marginY = 0.0;
			}
			else if (marginY + this._borderMove.Height >= this._canvasSmallImg.Height)
			{
				marginY = this._canvasSmallImg.Height - this._borderMove.Height;
			}
			this._borderMove.Margin = new Thickness(marginX, marginY, 0.0, 0.0);
			double num = (base.ActualWidth - this.ImageWidth) / 2.0;
			double num2 = (base.ActualHeight - this.ImageHeight) / 2.0;
			if (this._canMoveX)
			{
				num = -marginX / this._canvasSmallImg.Width * this.ImageWidth;
			}
			if (this._canMoveY)
			{
				num2 = -marginY / this._canvasSmallImg.Height * this.ImageHeight;
			}
			this.ImageMargin = new Thickness(num, num2, 0.0, 0.0);
			this._imgActualMargin = this.ImageMargin;
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x0009A388 File Offset: 0x00098588
		private void CanvasSmallImg_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._imgSmallMouseDownPoint = Mouse.GetPosition(this._canvasSmallImg);
			this._imgSmallMouseDownMargin = this._borderMove.Margin;
			this._imgSmallIsMouseLeftButtonDown = true;
			e.Handled = true;
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x0009A3BA File Offset: 0x000985BA
		private void CanvasSmallImg_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this._imgSmallIsMouseLeftButtonDown = false;
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x0009A3C3 File Offset: 0x000985C3
		private void CanvasSmallImg_OnMouseMove(object sender, MouseEventArgs e)
		{
			this.MoveSmallImg();
		}

		// Token: 0x04000D95 RID: 3477
		private const string ElementPanelMain = "PART_PanelMain";

		// Token: 0x04000D96 RID: 3478
		private const string ElementCanvasSmallImg = "PART_CanvasSmallImg";

		// Token: 0x04000D97 RID: 3479
		private const string ElementBorderMove = "PART_BorderMove";

		// Token: 0x04000D98 RID: 3480
		private const string ElementBorderBottom = "PART_BorderBottom";

		// Token: 0x04000D99 RID: 3481
		private const string ElementImageMain = "PART_ImageMain";

		// Token: 0x04000D9A RID: 3482
		public double ScaleInternal = 0.1;

		// Token: 0x04000D9B RID: 3483
		public double ScaleMin = 0.1;

		// Token: 0x04000D9C RID: 3484
		public double ScaleMax = 8.0;

		// Token: 0x04000D9D RID: 3485
		private static readonly SaveFileDialog SaveFileDialog = new SaveFileDialog
		{
			Filter = Lang.PngImg + "|*.png"
		};

		// Token: 0x04000D9E RID: 3486
		private Panel _panelMain;

		// Token: 0x04000D9F RID: 3487
		private Canvas _canvasSmallImg;

		// Token: 0x04000DA0 RID: 3488
		private Border _borderMove;

		// Token: 0x04000DA1 RID: 3489
		private Border _borderBottom;

		// Token: 0x04000DA2 RID: 3490
		private Image _imageMain;

		// Token: 0x04000DA3 RID: 3491
		private bool _borderSmallIsLoaded;

		// Token: 0x04000DA4 RID: 3492
		private bool _canMoveX;

		// Token: 0x04000DA5 RID: 3493
		private bool _canMoveY;

		// Token: 0x04000DA6 RID: 3494
		private Thickness _imgActualMargin;

		// Token: 0x04000DA7 RID: 3495
		private double _imgActualRotate;

		// Token: 0x04000DA8 RID: 3496
		private double _imgActualScale = 1.0;

		// Token: 0x04000DA9 RID: 3497
		private Point _imgCurrentPoint;

		// Token: 0x04000DAA RID: 3498
		private bool _imgIsMouseLeftButtonDown;

		// Token: 0x04000DAB RID: 3499
		private Thickness _imgMouseDownMargin;

		// Token: 0x04000DAC RID: 3500
		private Point _imgMouseDownPoint;

		// Token: 0x04000DAD RID: 3501
		private Point _imgSmallCurrentPoint;

		// Token: 0x04000DAE RID: 3502
		private bool _imgSmallIsMouseLeftButtonDown;

		// Token: 0x04000DAF RID: 3503
		private Thickness _imgSmallMouseDownMargin;

		// Token: 0x04000DB0 RID: 3504
		private Point _imgSmallMouseDownPoint;

		// Token: 0x04000DB1 RID: 3505
		private double _imgWidHeiScale;

		// Token: 0x04000DB2 RID: 3506
		private bool _isOblique;

		// Token: 0x04000DB3 RID: 3507
		private double _scaleInternalHeight;

		// Token: 0x04000DB4 RID: 3508
		private double _scaleInternalWidth;

		// Token: 0x04000DB5 RID: 3509
		private bool _showBorderBottom;

		// Token: 0x04000DB6 RID: 3510
		private DispatcherTimer _dispatcher;

		// Token: 0x04000DB7 RID: 3511
		private bool _isLoaded;

		// Token: 0x04000DB8 RID: 3512
		private static bool isOnSourceChanged;

		// Token: 0x04000DB9 RID: 3513
		public static readonly DependencyProperty ShowImgMapProperty = DependencyProperty.Register("ShowImgMap", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

		// Token: 0x04000DBA RID: 3514
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(BitmapFrame), typeof(ImageViewer), new PropertyMetadata(null, new PropertyChangedCallback(ImageViewer.OnImageSourceChanged)));

		// Token: 0x04000DBB RID: 3515
		public static readonly DependencyProperty ShowToolBarProperty = DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(ImageViewer), new PropertyMetadata(true));

		// Token: 0x04000DBC RID: 3516
		public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

		// Token: 0x04000DBD RID: 3517
		internal static readonly DependencyProperty ImgPathProperty = DependencyProperty.Register("ImgPath", typeof(string), typeof(ImageViewer), new PropertyMetadata(null));

		// Token: 0x04000DBE RID: 3518
		internal static readonly DependencyProperty ImgSizeProperty = DependencyProperty.Register("ImgSize", typeof(long), typeof(ImageViewer), new PropertyMetadata(-1L));

		// Token: 0x04000DBF RID: 3519
		internal static readonly DependencyProperty ShowFullScreenButtonProperty = DependencyProperty.Register("ShowFullScreenButton", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

		// Token: 0x04000DC0 RID: 3520
		internal static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

		// Token: 0x04000DC1 RID: 3521
		internal static readonly DependencyProperty ImageContentProperty = DependencyProperty.Register("ImageContent", typeof(object), typeof(ImageViewer), new PropertyMetadata(null));

		// Token: 0x04000DC2 RID: 3522
		internal static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageViewer), new PropertyMetadata(default(Thickness)));

		// Token: 0x04000DC3 RID: 3523
		internal static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageViewer), new PropertyMetadata(0.0));

		// Token: 0x04000DC4 RID: 3524
		internal static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageViewer), new PropertyMetadata(0.0));

		// Token: 0x04000DC5 RID: 3525
		internal static readonly DependencyProperty ImageScaleProperty = DependencyProperty.Register("ImageScale", typeof(double), typeof(ImageViewer), new PropertyMetadata(0.01, new PropertyChangedCallback(ImageViewer.OnImageScaleChanged)));

		// Token: 0x04000DC6 RID: 3526
		internal static readonly DependencyProperty ScaleStrProperty = DependencyProperty.Register("ScaleStr", typeof(string), typeof(ImageViewer), new PropertyMetadata("100%"));

		// Token: 0x04000DC7 RID: 3527
		internal static readonly DependencyProperty ImageRotateProperty = DependencyProperty.Register("ImageRotate", typeof(double), typeof(ImageViewer), new PropertyMetadata(0.0));

		// Token: 0x04000DC8 RID: 3528
		internal static readonly DependencyProperty ShowSmallImgInternalProperty = DependencyProperty.Register("ShowSmallImgInternal", typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

		// Token: 0x04000DC9 RID: 3529
		internal static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageViewerSource), typeof(ImageViewer), new PropertyMetadata(null, new PropertyChangedCallback(ImageViewer.OnSourceChanged)));
	}
}
