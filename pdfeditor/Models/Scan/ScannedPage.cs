using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using pdfeditor.Utils.Scan;

namespace pdfeditor.Models.Scan
{
	// Token: 0x0200013F RID: 319
	public class ScannedPage : ObservableObject
	{
		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06001342 RID: 4930 RVA: 0x0004E78C File Offset: 0x0004C98C
		// (remove) Token: 0x06001343 RID: 4931 RVA: 0x0004E7C4 File Offset: 0x0004C9C4
		public event Action<ScannedPage, AdjustType> Adjusted;

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06001344 RID: 4932 RVA: 0x0004E7F9 File Offset: 0x0004C9F9
		// (set) Token: 0x06001345 RID: 4933 RVA: 0x0004E801 File Offset: 0x0004CA01
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

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06001346 RID: 4934 RVA: 0x0004E816 File Offset: 0x0004CA16
		// (set) Token: 0x06001347 RID: 4935 RVA: 0x0004E81E File Offset: 0x0004CA1E
		public string Extension { get; private set; } = ".jpg";

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06001348 RID: 4936 RVA: 0x0004E827 File Offset: 0x0004CA27
		// (set) Token: 0x06001349 RID: 4937 RVA: 0x0004E82F File Offset: 0x0004CA2F
		public string ImagePath { get; private set; }

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x0004E838 File Offset: 0x0004CA38
		// (set) Token: 0x0600134B RID: 4939 RVA: 0x0004E840 File Offset: 0x0004CA40
		public string PreviewPath { get; set; }

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x0600134C RID: 4940 RVA: 0x0004E849 File Offset: 0x0004CA49
		// (set) Token: 0x0600134D RID: 4941 RVA: 0x0004E851 File Offset: 0x0004CA51
		public BitmapFrame Thumbnail
		{
			get
			{
				return this.thumbnail;
			}
			private set
			{
				base.SetProperty<BitmapFrame>(ref this.thumbnail, value, "Thumbnail");
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x0600134E RID: 4942 RVA: 0x0004E866 File Offset: 0x0004CA66
		// (set) Token: 0x0600134F RID: 4943 RVA: 0x0004E86E File Offset: 0x0004CA6E
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

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06001350 RID: 4944 RVA: 0x0004E883 File Offset: 0x0004CA83
		// (set) Token: 0x06001351 RID: 4945 RVA: 0x0004E88B File Offset: 0x0004CA8B
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

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06001352 RID: 4946 RVA: 0x0004E8A0 File Offset: 0x0004CAA0
		// (set) Token: 0x06001353 RID: 4947 RVA: 0x0004E8A8 File Offset: 0x0004CAA8
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

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06001354 RID: 4948 RVA: 0x0004E8C5 File Offset: 0x0004CAC5
		// (set) Token: 0x06001355 RID: 4949 RVA: 0x0004E8CD File Offset: 0x0004CACD
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

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06001356 RID: 4950 RVA: 0x0004E8EA File Offset: 0x0004CAEA
		// (set) Token: 0x06001357 RID: 4951 RVA: 0x0004E8F2 File Offset: 0x0004CAF2
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

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06001358 RID: 4952 RVA: 0x0004E90F File Offset: 0x0004CB0F
		// (set) Token: 0x06001359 RID: 4953 RVA: 0x0004E917 File Offset: 0x0004CB17
		public Rect CroppedRect { get; set; } = Rect.Empty;

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x0600135A RID: 4954 RVA: 0x0004E920 File Offset: 0x0004CB20
		// (set) Token: 0x0600135B RID: 4955 RVA: 0x0004E928 File Offset: 0x0004CB28
		public bool IsScanned { get; private set; }

		// Token: 0x0600135C RID: 4956 RVA: 0x0004E934 File Offset: 0x0004CB34
		public ScannedPage(Bitmap bitmap, string extension = null)
		{
			bitmap = ScannerImageHelper.CheckPixelFormat(bitmap);
			if (!string.IsNullOrEmpty(extension))
			{
				this.Extension = extension.ToLower();
			}
			this.ImageBitmap = bitmap;
			this.UpdateThumbnail(bitmap);
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x0004E988 File Offset: 0x0004CB88
		public ScannedPage(Bitmap bitmap, int brightness, int contrast, int saturation)
		{
			this.IsScanned = true;
			this.ImageBitmap = new Bitmap(bitmap);
			bitmap = ScannerImageHelper.CheckPixelFormat(bitmap);
			this.originalBrightness = brightness;
			this.originalContrast = contrast;
			this.originalSaturation = saturation;
			this.brightness = brightness;
			this.contrast = contrast;
			this.saturation = saturation;
			this.UpdateThumbnail(bitmap);
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x0004EA00 File Offset: 0x0004CC00
		private ScannedPage(ScannedPage page)
		{
			this.ImagePath = page.ImagePath;
			this.rotate = page.rotate;
			this.brightness = page.brightness;
			this.contrast = page.contrast;
			this.saturation = page.saturation;
			this.CroppedRect = page.CroppedRect;
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x0004EA74 File Offset: 0x0004CC74
		public void UpdateThumbnail(Bitmap image)
		{
			Bitmap bitmap = ScannerImageService.Resize(image);
			BitmapFrame bitmapFrame = ScannerImageHelper.ToBitmapFrame(bitmap);
			ScannerDispatcherHelper.Invoke(delegate
			{
				this.Thumbnail = bitmapFrame;
			});
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x0004EAB0 File Offset: 0x0004CCB0
		private void OnAdjusted(AdjustType type)
		{
			if (this.isRevert)
			{
				return;
			}
			Action<ScannedPage, AdjustType> adjusted = this.Adjusted;
			if (adjusted == null)
			{
				return;
			}
			adjusted(this, type);
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x0004EAD0 File Offset: 0x0004CCD0
		public bool HasChanged()
		{
			return this.Rotate != 0 || this.Brightness != this.originalBrightness || this.Contrast != this.originalContrast || this.Saturation != this.originalSaturation || !this.CroppedRect.IsEmpty;
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0004EB24 File Offset: 0x0004CD24
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

		// Token: 0x06001363 RID: 4963 RVA: 0x0004EB88 File Offset: 0x0004CD88
		public ScannedPage Clone()
		{
			return new ScannedPage(this);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0004EB90 File Offset: 0x0004CD90
		public Bitmap GetOriginalImage()
		{
			Bitmap bitmap = ScannerImageHelper.GetBitmap(this.ImagePath);
			if (this.originalBrightness != 0 || this.originalContrast != 0 || this.originalSaturation != 0)
			{
				bitmap = ScannerImageService.Process(bitmap, this.originalBrightness, this.originalContrast, this.originalSaturation) ?? bitmap;
			}
			return bitmap;
		}

		// Token: 0x04000618 RID: 1560
		private readonly int originalBrightness;

		// Token: 0x04000619 RID: 1561
		private readonly int originalContrast;

		// Token: 0x0400061A RID: 1562
		private readonly int originalSaturation;

		// Token: 0x0400061B RID: 1563
		private string indexString;

		// Token: 0x0400061C RID: 1564
		private BitmapFrame thumbnail;

		// Token: 0x0400061D RID: 1565
		private int rotate;

		// Token: 0x0400061E RID: 1566
		private int brightness;

		// Token: 0x0400061F RID: 1567
		private int contrast;

		// Token: 0x04000620 RID: 1568
		private int saturation;

		// Token: 0x04000621 RID: 1569
		private bool isRevert;

		// Token: 0x04000622 RID: 1570
		private Bitmap imageBitmap;
	}
}
