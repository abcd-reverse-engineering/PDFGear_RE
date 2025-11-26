using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfconverter.Models.Image;
using pdfconverter.Utils;
using pdfconverter.Utils.Image;
using pdfconverter.ViewModels;
using PDFKit.GenerateImagePdf;

namespace pdfconverter.Models
{
	// Token: 0x02000082 RID: 130
	public class ToimagePage : ObservableObject
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000627 RID: 1575 RVA: 0x00016FE0 File Offset: 0x000151E0
		// (remove) Token: 0x06000628 RID: 1576 RVA: 0x00017018 File Offset: 0x00015218
		public event Action<ToimagePage, AdjustType> Adjusted;

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x0001704D File Offset: 0x0001524D
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x00017055 File Offset: 0x00015255
		public string IndexString
		{
			get
			{
				return this.indexString;
			}
			set
			{
				base.SetProperty<string>(ref this.indexString, value, "IndexString");
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x0001706A File Offset: 0x0001526A
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x00017072 File Offset: 0x00015272
		public string Extension { get; private set; } = ".jpg";

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x0001707B File Offset: 0x0001527B
		// (set) Token: 0x0600062E RID: 1582 RVA: 0x00017083 File Offset: 0x00015283
		public string ImagePath { get; private set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x0001708C File Offset: 0x0001528C
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x00017094 File Offset: 0x00015294
		public string PreviewPath { get; set; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x0001709D File Offset: 0x0001529D
		// (set) Token: 0x06000632 RID: 1586 RVA: 0x000170A5 File Offset: 0x000152A5
		public Bitmap PreviewThumbnail
		{
			get
			{
				return this._previewThumbnail;
			}
			private set
			{
				base.SetProperty<Bitmap>(ref this._previewThumbnail, value, "PreviewThumbnail");
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x000170BA File Offset: 0x000152BA
		// (set) Token: 0x06000634 RID: 1588 RVA: 0x000170C2 File Offset: 0x000152C2
		public bool IsNeedEnabled
		{
			get
			{
				return this.isNeedEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.isNeedEnabled, value, "IsNeedEnabled");
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x000170D7 File Offset: 0x000152D7
		// (set) Token: 0x06000636 RID: 1590 RVA: 0x000170DF File Offset: 0x000152DF
		public bool? IsSucceed
		{
			get
			{
				return this.isSucceed;
			}
			set
			{
				base.SetProperty<bool?>(ref this.isSucceed, value, "IsSucceed");
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x000170F4 File Offset: 0x000152F4
		// (set) Token: 0x06000638 RID: 1592 RVA: 0x000170FC File Offset: 0x000152FC
		public string FilePath
		{
			get
			{
				return this.filePath;
			}
			set
			{
				base.SetProperty<string>(ref this.filePath, value, "FilePath");
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x00017111 File Offset: 0x00015311
		public string FileName
		{
			get
			{
				return Path.GetFileName(this.FilePath);
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x0001711E File Offset: 0x0001531E
		// (set) Token: 0x0600063B RID: 1595 RVA: 0x00017126 File Offset: 0x00015326
		public ImageSource Thumbnail2
		{
			get
			{
				return this._imagesource;
			}
			private set
			{
				base.SetProperty<ImageSource>(ref this._imagesource, value, "Thumbnail2");
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0001713B File Offset: 0x0001533B
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x00017143 File Offset: 0x00015343
		public double pixelsPerDip
		{
			get
			{
				return this._pixelsPerDip;
			}
			set
			{
				base.SetProperty<double>(ref this._pixelsPerDip, value, "pixelsPerDip");
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x00017158 File Offset: 0x00015358
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x00017160 File Offset: 0x00015360
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				base.SetProperty<int>(ref this.width, value, "Width");
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x00017175 File Offset: 0x00015375
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x0001717D File Offset: 0x0001537D
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				base.SetProperty<int>(ref this.height, value, "Height");
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x00017192 File Offset: 0x00015392
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x0001719A File Offset: 0x0001539A
		public Bitmap ImageBitmap
		{
			get
			{
				return this.imageBitmap;
			}
			set
			{
				base.SetProperty<Bitmap>(ref this.imageBitmap, value, "ImageBitmap");
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x000171AF File Offset: 0x000153AF
		// (set) Token: 0x06000645 RID: 1605 RVA: 0x000171B7 File Offset: 0x000153B7
		public int Rotate
		{
			get
			{
				return this.rotate;
			}
			set
			{
				base.SetProperty<int>(ref this.rotate, value, "Rotate");
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x000171CC File Offset: 0x000153CC
		// (set) Token: 0x06000647 RID: 1607 RVA: 0x000171D4 File Offset: 0x000153D4
		public int Brightness
		{
			get
			{
				return this.brightness;
			}
			set
			{
				if (base.SetProperty<int>(ref this.brightness, value, "Brightness"))
				{
					this.OnAdjusted(AdjustType.Brightness);
				}
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x000171F1 File Offset: 0x000153F1
		// (set) Token: 0x06000649 RID: 1609 RVA: 0x000171F9 File Offset: 0x000153F9
		public int Contrast
		{
			get
			{
				return this.contrast;
			}
			set
			{
				if (base.SetProperty<int>(ref this.contrast, value, "Contrast"))
				{
					this.OnAdjusted(AdjustType.Contrast);
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x00017216 File Offset: 0x00015416
		// (set) Token: 0x0600064B RID: 1611 RVA: 0x0001721E File Offset: 0x0001541E
		public int Saturation
		{
			get
			{
				return this.saturation;
			}
			set
			{
				if (base.SetProperty<int>(ref this.saturation, value, "Saturation"))
				{
					this.OnAdjusted(AdjustType.Saturation);
				}
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001723B File Offset: 0x0001543B
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x00017243 File Offset: 0x00015443
		public Rect CroppedRect { get; set; } = Rect.Empty;

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0001724C File Offset: 0x0001544C
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x00017254 File Offset: 0x00015454
		public bool IsScanned { get; private set; }

		// Token: 0x06000650 RID: 1616 RVA: 0x00017260 File Offset: 0x00015460
		public ToimagePage(Bitmap bitmap, string fileName, string extension = null)
		{
			if (bitmap == null)
			{
				this.ImageBitmap = bitmap;
				this.FilePath = fileName;
				this.UpdateThumbnail(bitmap);
				this.isNeedEnabled = false;
				return;
			}
			bitmap = ImageHelper.CheckPixelFormat(bitmap);
			if (!string.IsNullOrEmpty(extension))
			{
				this.Extension = extension.ToLower();
			}
			this.ImageBitmap = bitmap;
			this.FilePath = fileName;
			this.UpdateThumbnail(bitmap);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x000172E4 File Offset: 0x000154E4
		public ToimagePage(Bitmap bitmap, int brightness, int contrast, int saturation)
		{
			this.IsScanned = true;
			this.ImageBitmap = new Bitmap(bitmap);
			bitmap = ImageHelper.CheckPixelFormat(bitmap);
			this.originalBrightness = brightness;
			this.originalContrast = contrast;
			this.originalSaturation = saturation;
			this.brightness = brightness;
			this.contrast = contrast;
			this.saturation = saturation;
			this.UpdateThumbnail(bitmap);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00017364 File Offset: 0x00015564
		private ToimagePage(ToimagePage page)
		{
			this.ImagePath = page.ImagePath;
			this.rotate = page.rotate;
			this.brightness = page.brightness;
			this.contrast = page.contrast;
			this.saturation = page.saturation;
			this.CroppedRect = page.CroppedRect;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x000173DC File Offset: 0x000155DC
		public async Task UpdateThumbnail(Bitmap image, ImagePdfGenerateSettings imagePdfGenerateSettings)
		{
			ToimagePage.<>c__DisplayClass89_0 CS$<>8__locals1 = new ToimagePage.<>c__DisplayClass89_0();
			CS$<>8__locals1.<>4__this = this;
			if (image == null)
			{
				ImageDispatcherHelper.Invoke(delegate
				{
					ToimagePage.<>c__DisplayClass89_0.<<UpdateThumbnail>b__0>d <<UpdateThumbnail>b__0>d;
					<<UpdateThumbnail>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<UpdateThumbnail>b__0>d.<>4__this = CS$<>8__locals1;
					<<UpdateThumbnail>b__0>d.<>1__state = -1;
					<<UpdateThumbnail>b__0>d.<>t__builder.Start<ToimagePage.<>c__DisplayClass89_0.<<UpdateThumbnail>b__0>d>(ref <<UpdateThumbnail>b__0>d);
				});
			}
			else
			{
				CS$<>8__locals1.generator = new ImagePdfGenerator();
				CS$<>8__locals1.item = new ImagePdfGenerateItem(BitmapImageSource.CreateFromSystemDrawingBitmap(image, false));
				CS$<>8__locals1.item.Settings = imagePdfGenerateSettings;
				ImageDispatcherHelper.Invoke(delegate
				{
					ToimagePage.<>c__DisplayClass89_0.<<UpdateThumbnail>b__1>d <<UpdateThumbnail>b__1>d;
					<<UpdateThumbnail>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<UpdateThumbnail>b__1>d.<>4__this = CS$<>8__locals1;
					<<UpdateThumbnail>b__1>d.<>1__state = -1;
					<<UpdateThumbnail>b__1>d.<>t__builder.Start<ToimagePage.<>c__DisplayClass89_0.<<UpdateThumbnail>b__1>d>(ref <<UpdateThumbnail>b__1>d);
				});
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00017430 File Offset: 0x00015630
		public async void UpdateThumbnail(Bitmap image)
		{
			ToimagePage.<>c__DisplayClass90_0 CS$<>8__locals1 = new ToimagePage.<>c__DisplayClass90_0();
			CS$<>8__locals1.<>4__this = this;
			if (image == null)
			{
				ImageDispatcherHelper.Invoke(delegate
				{
					ToimagePage.<>c__DisplayClass90_0.<<UpdateThumbnail>b__0>d <<UpdateThumbnail>b__0>d;
					<<UpdateThumbnail>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<UpdateThumbnail>b__0>d.<>4__this = CS$<>8__locals1;
					<<UpdateThumbnail>b__0>d.<>1__state = -1;
					<<UpdateThumbnail>b__0>d.<>t__builder.Start<ToimagePage.<>c__DisplayClass90_0.<<UpdateThumbnail>b__0>d>(ref <<UpdateThumbnail>b__0>d);
				});
			}
			else
			{
				CS$<>8__locals1.generator = new ImagePdfGenerator();
				CS$<>8__locals1.item = new ImagePdfGenerateItem(BitmapImageSource.CreateFromSystemDrawingBitmap(image, false));
				ImageToPDFViewModel requiredService = Ioc.Default.GetRequiredService<ImageToPDFViewModel>();
				CS$<>8__locals1.item.Settings = requiredService._ImagePdfGenerateSettings;
				ImageDispatcherHelper.Invoke(delegate
				{
					ToimagePage.<>c__DisplayClass90_0.<<UpdateThumbnail>b__1>d <<UpdateThumbnail>b__1>d;
					<<UpdateThumbnail>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<UpdateThumbnail>b__1>d.<>4__this = CS$<>8__locals1;
					<<UpdateThumbnail>b__1>d.<>1__state = -1;
					<<UpdateThumbnail>b__1>d.<>t__builder.Start<ToimagePage.<>c__DisplayClass90_0.<<UpdateThumbnail>b__1>d>(ref <<UpdateThumbnail>b__1>d);
				});
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00017470 File Offset: 0x00015670
		public static Bitmap ConvertImageSourceToBitmap(ImageSource imageSource)
		{
			Bitmap bitmap;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BmpBitmapEncoder
				{
					Frames = { BitmapFrame.Create((BitmapSource)imageSource) }
				}.Save(memoryStream);
				bitmap = new Bitmap(memoryStream);
			}
			return bitmap;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x000174C8 File Offset: 0x000156C8
		public static void SaveImage(ImageSource image)
		{
			string text = Path.Combine("C:\\", "temp");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			int num = 0;
			string text3;
			do
			{
				num++;
				string text2 = string.Format("{0}", num);
				text2 += ".png";
				text3 = Path.Combine(text, text2);
			}
			while (File.Exists(text3));
			BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
			bitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)image));
			using (FileStream fileStream = new FileStream(text3, FileMode.Create))
			{
				bitmapEncoder.Save(fileStream);
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0001757C File Offset: 0x0001577C
		private void OnAdjusted(AdjustType type)
		{
			if (this.isRevert)
			{
				return;
			}
			Action<ToimagePage, AdjustType> adjusted = this.Adjusted;
			if (adjusted == null)
			{
				return;
			}
			adjusted(this, type);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0001759C File Offset: 0x0001579C
		public bool HasChanged()
		{
			return this.Rotate != 0 || this.Brightness != this.originalBrightness || this.Contrast != this.originalContrast || this.Saturation != this.originalSaturation || !this.CroppedRect.IsEmpty;
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x000175F0 File Offset: 0x000157F0
		public void Revert()
		{
			this.isRevert = true;
			try
			{
				this.Rotate = 0;
				this.Brightness = this.originalBrightness;
				this.Contrast = this.originalContrast;
				this.Saturation = this.originalSaturation;
				this.CroppedRect = Rect.Empty;
			}
			finally
			{
				this.isRevert = false;
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00017654 File Offset: 0x00015854
		public ToimagePage Clone()
		{
			return new ToimagePage(this);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0001765C File Offset: 0x0001585C
		public Bitmap GetOriginalImage()
		{
			Bitmap bitmap = ImageHelper.GetBitmap(this.ImagePath);
			if (this.originalBrightness != 0 || this.originalContrast != 0 || this.originalSaturation != 0)
			{
				bitmap = ImageService.Process(bitmap, this.originalBrightness, this.originalContrast, this.originalSaturation) ?? bitmap;
			}
			return bitmap;
		}

		// Token: 0x040002F4 RID: 756
		private readonly int originalBrightness;

		// Token: 0x040002F5 RID: 757
		private readonly int originalContrast;

		// Token: 0x040002F6 RID: 758
		private readonly int originalSaturation;

		// Token: 0x040002F7 RID: 759
		private string indexString;

		// Token: 0x040002F8 RID: 760
		private Bitmap _previewThumbnail;

		// Token: 0x040002F9 RID: 761
		private ImageSource _imagesource;

		// Token: 0x040002FA RID: 762
		private string filePath;

		// Token: 0x040002FB RID: 763
		private string fileName;

		// Token: 0x040002FC RID: 764
		private int width;

		// Token: 0x040002FD RID: 765
		private int height;

		// Token: 0x040002FE RID: 766
		private int rotate;

		// Token: 0x040002FF RID: 767
		private int brightness;

		// Token: 0x04000300 RID: 768
		private int contrast;

		// Token: 0x04000301 RID: 769
		private int saturation;

		// Token: 0x04000302 RID: 770
		private bool isRevert;

		// Token: 0x04000303 RID: 771
		private Bitmap imageBitmap;

		// Token: 0x04000304 RID: 772
		private bool? isSucceed;

		// Token: 0x04000305 RID: 773
		private bool isNeedEnabled = true;

		// Token: 0x04000306 RID: 774
		private double _pixelsPerDip;
	}
}
