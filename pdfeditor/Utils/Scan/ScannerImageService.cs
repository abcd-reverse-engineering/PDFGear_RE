using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ImageProcessor;
using ImageProcessor.Imaging;
using pdfeditor.Models.Scan;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000BA RID: 186
	internal class ScannerImageService
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000B17 RID: 2839 RVA: 0x000392F0 File Offset: 0x000374F0
		// (remove) Token: 0x06000B18 RID: 2840 RVA: 0x00039328 File Offset: 0x00037528
		public event Action<ScannedPage, Bitmap, bool> PreviewImageChanged;

		// Token: 0x06000B19 RID: 2841 RVA: 0x0003935D File Offset: 0x0003755D
		private void OnPreviewImageChanged(ScannedPage page, Bitmap bitmap, bool isAdjust = false)
		{
			Action<ScannedPage, Bitmap, bool> previewImageChanged = this.PreviewImageChanged;
			if (previewImageChanged == null)
			{
				return;
			}
			previewImageChanged(page, bitmap, isAdjust);
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x00039374 File Offset: 0x00037574
		public static Bitmap Process(Bitmap image, int brightness, int contrast, int saturation)
		{
			try
			{
				using (ImageFactory imageFactory = new ImageFactory(false))
				{
					imageFactory.Load(image);
					if (brightness != 0)
					{
						imageFactory.Brightness(brightness);
					}
					if (contrast != 0)
					{
						imageFactory.Contrast(contrast);
					}
					if (saturation != 0)
					{
						imageFactory.Saturation(saturation);
					}
					MemoryStream memoryStream = new MemoryStream();
					imageFactory.Save(memoryStream);
					return new Bitmap(memoryStream);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x000393F4 File Offset: 0x000375F4
		public static Bitmap Resize(Bitmap image)
		{
			try
			{
				using (ImageFactory imageFactory = new ImageFactory(false))
				{
					imageFactory.Load(image);
					imageFactory.Resize(new ResizeLayer(new global::System.Drawing.Size(300, 300), global::ImageProcessor.Imaging.ResizeMode.Min, AnchorPosition.Center, true, null, null, null, null));
					MemoryStream memoryStream = new MemoryStream();
					imageFactory.Save(memoryStream);
					return new Bitmap(memoryStream);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x00039488 File Offset: 0x00037688
		public static Bitmap GetSaveImage(ScannedPage page)
		{
			Bitmap imageBitmap = page.ImageBitmap;
			if (page.Rotate == 0)
			{
				return imageBitmap;
			}
			Bitmap bitmap;
			using (ImageFactory imageFactory = new ImageFactory(false))
			{
				imageFactory.Load(imageBitmap);
				imageFactory.Rotate((float)page.Rotate);
				MemoryStream memoryStream = new MemoryStream();
				imageFactory.Save(memoryStream);
				bitmap = new Bitmap(memoryStream);
			}
			return bitmap;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x000394F8 File Offset: 0x000376F8
		public void Preview(ScannedPage page)
		{
			this.lastAdjustSettings = null;
			this.lastImageBeforeAdjust = null;
			if (page != null)
			{
				try
				{
					Bitmap imageBitmap = page.ImageBitmap;
					if (imageBitmap != null)
					{
						this.OnPreviewImageChanged(page, imageBitmap, false);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x00039540 File Offset: 0x00037740
		public void Crop(ScannedPage page, Bitmap originalImage, Rect croppedRect)
		{
			if (croppedRect.IsEmpty)
			{
				return;
			}
			Bitmap cropImage = this.GetCropImage(page, originalImage.Clone() as Image, croppedRect);
			if (cropImage == null)
			{
				return;
			}
			if (!page.CroppedRect.IsEmpty)
			{
				croppedRect.X += page.CroppedRect.X;
				croppedRect.Y += page.CroppedRect.Y;
			}
			page.CroppedRect = croppedRect;
			this.OnPreviewImageChanged(page, cropImage, false);
			this.SavePreviewImageAndUpdateThumbnail(page, cropImage);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x000395D0 File Offset: 0x000377D0
		public void Adjust(AdjustSettings adjustSettings)
		{
			this.adjusting++;
			try
			{
				this.adjustTime = DateTime.Now;
				if (this.lastImageBeforeAdjust != null && !adjustSettings.IsSame(this.lastAdjustSettings))
				{
					this.lastImageBeforeAdjust.Dispose();
					this.lastImageBeforeAdjust = null;
				}
				Bitmap bitmap = this.lastImageBeforeAdjust;
				Bitmap bitmap2 = null;
				if (bitmap != null)
				{
					bitmap2 = this.GetImageAfterAdjust(adjustSettings, bitmap.Clone() as Bitmap);
				}
				else
				{
					this.GetAdjustImage(adjustSettings, out bitmap, out bitmap2);
				}
				if (!(this.adjustTime < this.revertTime))
				{
					this.lastAdjustSettings = adjustSettings;
					this.lastImageBeforeAdjust = bitmap;
					this.OnPreviewImageChanged(adjustSettings.OriginalPage, bitmap2, true);
					this.SavePreviewImageAndUpdateThumbnail(adjustSettings.OriginalPage, bitmap2);
				}
			}
			finally
			{
				this.adjusting--;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000B20 RID: 2848 RVA: 0x000396AC File Offset: 0x000378AC
		public bool IsAdjusting
		{
			get
			{
				return this.adjusting != 0;
			}
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x000396B8 File Offset: 0x000378B8
		public async Task WaitForAdjusted()
		{
			while (this.IsAdjusting)
			{
				await Task.Delay(100);
			}
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x000396FC File Offset: 0x000378FC
		public void Revert(ScannedPage page)
		{
			this.revertTime = DateTime.Now;
			while (this.IsAdjusting)
			{
				Thread.Sleep(100);
			}
			this.lastAdjustSettings = null;
			this.lastImageBeforeAdjust = null;
			page.Revert();
			Bitmap originalImage = page.GetOriginalImage();
			this.OnPreviewImageChanged(page, originalImage, false);
			this.SavePreviewImageAndUpdateThumbnail(page, originalImage);
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x00039754 File Offset: 0x00037954
		private Bitmap GetCropImage(ScannedPage page, Image image, Rect croppedRect)
		{
			try
			{
				using (ImageFactory imageFactory = new ImageFactory(false))
				{
					imageFactory.Load(image);
					if (page.Rotate != 0)
					{
						imageFactory.Rotate((float)page.Rotate);
					}
					Rectangle rectangle = this.ToCroppedRectangle(croppedRect, imageFactory.Image.Width, imageFactory.Image.Height);
					if (rectangle.IsEmpty)
					{
						return null;
					}
					imageFactory.Crop(rectangle);
					if (page.Rotate != 0)
					{
						imageFactory.Rotate((float)(-(float)page.Rotate));
					}
					MemoryStream memoryStream = new MemoryStream();
					imageFactory.Save(memoryStream);
					return new Bitmap(memoryStream);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x00039814 File Offset: 0x00037A14
		private void SavePreviewImageAndUpdateThumbnail(ScannedPage page, Bitmap image)
		{
			Task.Run(delegate
			{
				string text = Path.Combine("D://PDF", string.Format("{0}{1}", Guid.NewGuid(), page.Extension));
				image.Save(text);
				page.PreviewPath = text;
				page.UpdateThumbnail(image);
			});
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0003983C File Offset: 0x00037A3C
		private Bitmap Save(ImageFactory factory)
		{
			MemoryStream memoryStream = new MemoryStream();
			factory.Save(memoryStream);
			return new Bitmap(memoryStream);
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x00039860 File Offset: 0x00037A60
		private void GetAdjustImage(AdjustSettings adjustSettings, out Bitmap imageBeforeAdjust, out Bitmap imageAfterAdjust)
		{
			ScannedPage page = adjustSettings.Page;
			AdjustType adjustType = adjustSettings.AdjustType;
			using (ImageFactory imageFactory = new ImageFactory(false))
			{
				imageFactory.Load(page.ImagePath);
				if (!page.CroppedRect.IsEmpty)
				{
					Rectangle rectangle = this.ToCroppedRectangle(page.CroppedRect, imageFactory.Image.Width, imageFactory.Image.Height);
					if (!rectangle.IsEmpty)
					{
						if (page.Rotate == 0)
						{
							imageFactory.Crop(rectangle);
						}
						else
						{
							imageFactory.Rotate((float)page.Rotate);
							imageFactory.Crop(rectangle);
							imageFactory.Rotate((float)(-(float)page.Rotate));
						}
					}
				}
				if (adjustType != AdjustType.Brightness)
				{
					imageFactory.Brightness(page.Brightness);
				}
				if (adjustType != AdjustType.Contrast)
				{
					imageFactory.Contrast(page.Contrast);
				}
				if (adjustType != AdjustType.Saturation)
				{
					imageFactory.Saturation(page.Saturation);
				}
				imageBeforeAdjust = this.Save(imageFactory);
				if (adjustType == AdjustType.Brightness)
				{
					imageFactory.Brightness(page.Brightness);
				}
				if (adjustType == AdjustType.Contrast)
				{
					imageFactory.Contrast(page.Contrast);
				}
				if (adjustType == AdjustType.Saturation)
				{
					imageFactory.Saturation(page.Saturation);
				}
				imageAfterAdjust = this.Save(imageFactory);
			}
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x00039998 File Offset: 0x00037B98
		private Bitmap GetImageAfterAdjust(AdjustSettings adjustSettings, Bitmap imageBeforeAdjust)
		{
			ScannedPage page = adjustSettings.Page;
			AdjustType adjustType = adjustSettings.AdjustType;
			Bitmap bitmap;
			using (ImageFactory imageFactory = new ImageFactory(false))
			{
				imageFactory.Load(imageBeforeAdjust);
				switch (adjustType)
				{
				case AdjustType.Brightness:
					imageFactory.Brightness(page.Brightness);
					break;
				case AdjustType.Contrast:
					imageFactory.Contrast(page.Contrast);
					break;
				case AdjustType.Saturation:
					imageFactory.Saturation(page.Saturation);
					break;
				}
				bitmap = this.Save(imageFactory);
			}
			return bitmap;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00039A28 File Offset: 0x00037C28
		private Rectangle ToCroppedRectangle(Rect croppedRect, int imageWidth, int imageHeight)
		{
			if (croppedRect.IsEmpty)
			{
				return Rectangle.Empty;
			}
			Rectangle rectangle = this.ToRectangle(croppedRect);
			if (rectangle.Width == imageWidth && rectangle.Height == imageHeight)
			{
				return Rectangle.Empty;
			}
			if (rectangle.X < 0)
			{
				rectangle.Width -= rectangle.X;
				rectangle.X = 0;
			}
			if (rectangle.Y < 0)
			{
				rectangle.Height -= rectangle.Y;
				rectangle.Y = 0;
			}
			if (rectangle.Width + rectangle.X > imageWidth)
			{
				rectangle.Width = imageWidth - rectangle.X;
			}
			if (rectangle.Height + rectangle.Y > imageHeight)
			{
				rectangle.Height = imageHeight - rectangle.Y;
			}
			if (rectangle.Width <= 0 || rectangle.Height <= 0)
			{
				return Rectangle.Empty;
			}
			return rectangle;
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x00039B13 File Offset: 0x00037D13
		private Rectangle ToRectangle(Rect rect)
		{
			return new Rectangle((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
		}

		// Token: 0x040004CB RID: 1227
		private AdjustSettings lastAdjustSettings;

		// Token: 0x040004CC RID: 1228
		private Bitmap lastImageBeforeAdjust;

		// Token: 0x040004CD RID: 1229
		private int adjusting;

		// Token: 0x040004CE RID: 1230
		private DateTime adjustTime;

		// Token: 0x040004CF RID: 1231
		private DateTime revertTime;
	}
}
