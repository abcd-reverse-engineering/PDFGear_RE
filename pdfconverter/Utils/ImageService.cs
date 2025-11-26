using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommonLib.Common;
using ImageProcessor;
using ImageProcessor.Imaging;
using pdfconverter.Models;
using pdfconverter.Models.Image;

namespace pdfconverter.Utils
{
	// Token: 0x0200003F RID: 63
	internal class ImageService
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060004DC RID: 1244 RVA: 0x00013B90 File Offset: 0x00011D90
		// (remove) Token: 0x060004DD RID: 1245 RVA: 0x00013BC8 File Offset: 0x00011DC8
		public event Action<ToimagePage, Bitmap, bool> PreviewImageChanged;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060004DE RID: 1246 RVA: 0x00013C00 File Offset: 0x00011E00
		// (remove) Token: 0x060004DF RID: 1247 RVA: 0x00013C38 File Offset: 0x00011E38
		public event Action<ToimagePage, ImageSource, bool> PreviewImageChanged2;

		// Token: 0x060004E0 RID: 1248 RVA: 0x00013C6D File Offset: 0x00011E6D
		private void OnPreviewImageChanged(ToimagePage page, ImageSource imageSource, bool isAdjust = false)
		{
			Action<ToimagePage, ImageSource, bool> previewImageChanged = this.PreviewImageChanged2;
			if (previewImageChanged == null)
			{
				return;
			}
			previewImageChanged(page, imageSource, isAdjust);
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x00013C82 File Offset: 0x00011E82
		private void OnPreviewImageChanged(ToimagePage page, Bitmap bitmap, bool isAdjust = false)
		{
			Action<ToimagePage, Bitmap, bool> previewImageChanged = this.PreviewImageChanged;
			if (previewImageChanged == null)
			{
				return;
			}
			previewImageChanged(page, bitmap, isAdjust);
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00013C98 File Offset: 0x00011E98
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
			catch (Exception ex)
			{
				Log.WriteLog(ex.ToString());
			}
			return null;
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00013D24 File Offset: 0x00011F24
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

		// Token: 0x060004E4 RID: 1252 RVA: 0x00013DB8 File Offset: 0x00011FB8
		public static Bitmap GetSaveImage(ToimagePage page)
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

		// Token: 0x060004E5 RID: 1253 RVA: 0x00013E28 File Offset: 0x00012028
		public void Preview(ToimagePage page)
		{
			this.lastAdjustSettings = null;
			this.lastImageBeforeAdjust = null;
			if (page != null)
			{
				try
				{
					ImageSource thumbnail = page.Thumbnail2;
					if (thumbnail != null)
					{
						this.OnPreviewImageChanged(page, thumbnail, false);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00013E70 File Offset: 0x00012070
		public void Crop(ToimagePage page, Bitmap originalImage, Rect croppedRect)
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

		// Token: 0x060004E7 RID: 1255 RVA: 0x00013F00 File Offset: 0x00012100
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

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x00013FDC File Offset: 0x000121DC
		public bool IsAdjusting
		{
			get
			{
				return this.adjusting != 0;
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00013FE8 File Offset: 0x000121E8
		public async Task WaitForAdjusted()
		{
			while (this.IsAdjusting)
			{
				await Task.Delay(100);
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001402C File Offset: 0x0001222C
		public void Revert(ToimagePage page)
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

		// Token: 0x060004EB RID: 1259 RVA: 0x00014084 File Offset: 0x00012284
		private Bitmap GetCropImage(ToimagePage page, Image image, Rect croppedRect)
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

		// Token: 0x060004EC RID: 1260 RVA: 0x00014144 File Offset: 0x00012344
		private void SavePreviewImageAndUpdateThumbnail(ToimagePage page, Bitmap image)
		{
			Task.Run(delegate
			{
				string text = Path.Combine("D://PDF", string.Format("{0}{1}", Guid.NewGuid(), page.Extension));
				image.Save(text);
				page.PreviewPath = text;
				page.UpdateThumbnail(image);
			});
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001416C File Offset: 0x0001236C
		private Bitmap Save(ImageFactory factory)
		{
			MemoryStream memoryStream = new MemoryStream();
			factory.Save(memoryStream);
			return new Bitmap(memoryStream);
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00014190 File Offset: 0x00012390
		private void GetAdjustImage(AdjustSettings adjustSettings, out Bitmap imageBeforeAdjust, out Bitmap imageAfterAdjust)
		{
			ToimagePage page = adjustSettings.Page;
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

		// Token: 0x060004EF RID: 1263 RVA: 0x000142C8 File Offset: 0x000124C8
		private Bitmap GetImageAfterAdjust(AdjustSettings adjustSettings, Bitmap imageBeforeAdjust)
		{
			ToimagePage page = adjustSettings.Page;
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

		// Token: 0x060004F0 RID: 1264 RVA: 0x00014358 File Offset: 0x00012558
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

		// Token: 0x060004F1 RID: 1265 RVA: 0x00014443 File Offset: 0x00012643
		private Rectangle ToRectangle(Rect rect)
		{
			return new Rectangle((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
		}

		// Token: 0x0400026B RID: 619
		private AdjustSettings lastAdjustSettings;

		// Token: 0x0400026C RID: 620
		private Bitmap lastImageBeforeAdjust;

		// Token: 0x0400026D RID: 621
		private int adjusting;

		// Token: 0x0400026E RID: 622
		private DateTime adjustTime;

		// Token: 0x0400026F RID: 623
		private DateTime revertTime;
	}
}
