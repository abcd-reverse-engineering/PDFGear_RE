using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using pdfeditor.Controls.DigitalSignatures;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DF RID: 223
	internal static class SignatureValidateHelper
	{
		// Token: 0x06000C00 RID: 3072 RVA: 0x0003F478 File Offset: 0x0003D678
		public static async Task<PdfSignatureValidationResult> ValidateSignatureAsync(this PdfDigitalSignatureField field, bool useCachedResult, bool useDefaultCertificates = true)
		{
			PdfSignatureValidationResult pdfSignatureValidationResult;
			if (field == null)
			{
				pdfSignatureValidationResult = null;
			}
			else if (!field.IsSigned)
			{
				pdfSignatureValidationResult = null;
			}
			else if (useCachedResult && field.LastValidateResult != null)
			{
				pdfSignatureValidationResult = field.LastValidateResult;
			}
			else
			{
				if (useDefaultCertificates)
				{
					CertificateManagerWindow.X509CertificateModel[] certs = CertificateManagerWindow.GetVerificationCertificate().ToArray<CertificateManagerWindow.X509CertificateModel>();
					try
					{
						return await field.ValidateSignatureAsync(new X509CertificateCollection(certs.Select((CertificateManagerWindow.X509CertificateModel c) => c.Certificate).ToArray<X509Certificate2>()), new PdfSignatureValidationOptions
						{
							ValidateRevocationStatus = false
						});
					}
					finally
					{
						foreach (CertificateManagerWindow.X509CertificateModel x509CertificateModel in certs)
						{
							try
							{
								x509CertificateModel.Certificate.Dispose();
							}
							catch
							{
							}
						}
					}
				}
				pdfSignatureValidationResult = await field.ValidateSignatureAsync(new PdfSignatureValidationOptions
				{
					ValidateRevocationStatus = false
				});
			}
			return pdfSignatureValidationResult;
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0003F4CC File Offset: 0x0003D6CC
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "dialogResult", "validationResult" })]
		public static async Task<global::System.ValueTuple<bool, PdfSignatureValidationResult>> ShowSignatureStatusDialogAsync(PdfDigitalSignatureField field, bool useCachedResult, bool useDefaultCertificates = true)
		{
			global::System.ValueTuple<bool, PdfSignatureValidationResult> valueTuple;
			if (field == null || !field.IsSigned)
			{
				valueTuple = new global::System.ValueTuple<bool, PdfSignatureValidationResult>(false, null);
			}
			else
			{
				PdfSignatureValidationResult pdfSignatureValidationResult = await field.ValidateSignatureAsync(useCachedResult, useDefaultCertificates);
				if (pdfSignatureValidationResult != null)
				{
					valueTuple = new global::System.ValueTuple<bool, PdfSignatureValidationResult>(SignatureValidateHelper.ShowSignatureStatusDialog(field, pdfSignatureValidationResult), pdfSignatureValidationResult);
				}
				else
				{
					valueTuple = new global::System.ValueTuple<bool, PdfSignatureValidationResult>(false, null);
				}
			}
			return valueTuple;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0003F520 File Offset: 0x0003D720
		public static async Task ShowSignaturePropertiesDialogAsync(PdfDigitalSignatureField field, bool useCachedResult, bool useDefaultCertificates = true)
		{
			if (field != null && field.IsSigned)
			{
				PdfSignatureValidationResult pdfSignatureValidationResult = await field.ValidateSignatureAsync(useCachedResult, useDefaultCertificates);
				if (pdfSignatureValidationResult != null)
				{
					SignatureValidateHelper.ShowSignaturePropertiesDialog(field, pdfSignatureValidationResult);
				}
			}
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0003F573 File Offset: 0x0003D773
		public static void ShowSignaturePropertiesDialog(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			new SignaturePropertiesDialog(field, result)
			{
				Owner = App.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog();
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0003F59C File Offset: 0x0003D79C
		public static bool ShowSignatureStatusDialog(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			return field != null && result != null && new SignatureValidationStatusDialog(field, result)
			{
				Owner = App.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog().GetValueOrDefault();
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0003F5DC File Offset: 0x0003D7DC
		public static IReadOnlyList<PdfSignatureValidationResult> ValidateAllSignaturesWithProgress(this DigitalSignatureHelper helper)
		{
			return helper.ValidateAllSignaturesWithProgress(new PdfSignatureValidationOptions());
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0003F5EC File Offset: 0x0003D7EC
		public static IReadOnlyList<PdfSignatureValidationResult> ValidateAllSignaturesWithProgress(this DigitalSignatureHelper helper, PdfSignatureValidationOptions options)
		{
			SignatureValidateHelper.<>c__DisplayClass7_0 CS$<>8__locals1 = new SignatureValidateHelper.<>c__DisplayClass7_0();
			CS$<>8__locals1.helper = helper;
			CS$<>8__locals1.options = options;
			if (CS$<>8__locals1.helper == null)
			{
				return null;
			}
			CS$<>8__locals1.allResult = null;
			ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction progressAction)
			{
				SignatureValidateHelper.<>c__DisplayClass7_0.<<ValidateAllSignaturesWithProgress>b__0>d <<ValidateAllSignaturesWithProgress>b__0>d;
				<<ValidateAllSignaturesWithProgress>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ValidateAllSignaturesWithProgress>b__0>d.<>4__this = CS$<>8__locals1;
				<<ValidateAllSignaturesWithProgress>b__0>d.progressAction = progressAction;
				<<ValidateAllSignaturesWithProgress>b__0>d.<>1__state = -1;
				<<ValidateAllSignaturesWithProgress>b__0>d.<>t__builder.Start<SignatureValidateHelper.<>c__DisplayClass7_0.<<ValidateAllSignaturesWithProgress>b__0>d>(ref <<ValidateAllSignaturesWithProgress>b__0>d);
				return <<ValidateAllSignaturesWithProgress>b__0>d.<>t__builder.Task;
			}, "", "", false, null, 800);
			if (CS$<>8__locals1.allResult != null)
			{
				if (CS$<>8__locals1.allResult.All((PdfSignatureValidationResult c) => c == null))
				{
					return null;
				}
			}
			return CS$<>8__locals1.allResult;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0003F67D File Offset: 0x0003D87D
		public static bool IsValid(this PdfSignatureValidationResult result, bool useDefaultCertificates = true)
		{
			if (result == null)
			{
				return false;
			}
			if (useDefaultCertificates)
			{
				return result.SignatureStatus == SignatureStatus.Valid;
			}
			return result.IsSignatureValid || result.SignatureStatus == SignatureStatus.Valid;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0003F6A4 File Offset: 0x0003D8A4
		public static bool IsSupportedValidCertificate(X509Certificate2 cert)
		{
			if (cert == null)
			{
				return false;
			}
			if (cert.Extensions != null)
			{
				X509KeyUsageExtension x509KeyUsageExtension = cert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault<X509KeyUsageExtension>();
				if (x509KeyUsageExtension == null)
				{
					return cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault<X509EnhancedKeyUsageExtension>() == null;
				}
				if ((x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.None)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0003F6F9 File Offset: 0x0003D8F9
		public static bool IsSupportedSignCertificate(X509Certificate2 cert)
		{
			return cert != null && cert.HasPrivateKey && SignatureValidateHelper.IsSupportedValidCertificate(cert);
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0003F710 File Offset: 0x0003D910
		internal static async Task<MainViewModel.SaveResult> AddDigitalSignatureAndSaveAsAsync(this DigitalSignatureHelper helper, X509Certificate2 cert, DigitalSignatureDrawingHelper drawingHelper, FS_RECTF? signRect, bool certificated, bool documentModified, string signatureFieldName = null)
		{
			MainViewModel.SaveResult saveResult;
			if (signRect == null && string.IsNullOrEmpty(signatureFieldName))
			{
				saveResult = new MainViewModel.SaveResult(MainViewModel.SaveFailedResult.Unknown);
			}
			else
			{
				RectangleF? bounds = null;
				int curIndex = -1;
				Func<PdfLoadedSignatureField, bool> <>9__2;
				saveResult = await SignatureValidateHelper.SaveDigitalSignatureToFileAsync(helper, "Signed", documentModified, delegate
				{
					curIndex = helper.PdfiumDocument.Pages.CurrentIndex;
					double num;
					double num2;
					Pdfium.FPDF_GetPageSizeByIndex(helper.PdfiumDocument.Handle, curIndex, out num, out num2);
					PageRotate pageRotate;
					Pdfium.FPDF_GetPageRotationByIndex(helper.PdfiumDocument.Handle, curIndex, out pageRotate);
					if (pageRotate == PageRotate.Rotate90 || pageRotate == PageRotate.Rotate270)
					{
						double num3 = num2;
						num2 = num;
						num = num3;
					}
					if (signRect != null)
					{
						bounds = new RectangleF?(new RectangleF(signRect.Value.left, (float)num2 - signRect.Value.top, signRect.Value.Width, signRect.Value.Height));
					}
					return Task.CompletedTask;
				}, delegate(PdfLoadedDocument doc)
				{
					if (curIndex != -1)
					{
						PdfLoadedPage pdfLoadedPage = (PdfLoadedPage)doc.Pages[curIndex];
						DigitalSignatureDrawingLeftElement leftElement = drawingHelper.LeftElement;
						DigitalSignatureDrawingRightElement rightElement = drawingHelper.RightElement;
						PdfCertificate pdfCertificate = new PdfCertificate(cert);
						PdfLoadedSignatureField pdfLoadedSignatureField = null;
						if (!string.IsNullOrEmpty(signatureFieldName))
						{
							PdfLoadedForm form = doc.Form;
							PdfLoadedSignatureField pdfLoadedSignatureField2;
							if (form == null)
							{
								pdfLoadedSignatureField2 = null;
							}
							else
							{
								IEnumerable<PdfLoadedSignatureField> enumerable = form.Fields.OfType<PdfLoadedSignatureField>();
								Func<PdfLoadedSignatureField, bool> func;
								if ((func = <>9__2) == null)
								{
									func = (<>9__2 = (PdfLoadedSignatureField c) => c.Name == signatureFieldName);
								}
								pdfLoadedSignatureField2 = enumerable.FirstOrDefault(func);
							}
							pdfLoadedSignatureField = pdfLoadedSignatureField2;
							if (pdfLoadedSignatureField == null)
							{
								return Task.FromResult<bool>(false);
							}
							bounds = new RectangleF?(pdfLoadedSignatureField.Bounds);
						}
						else
						{
							signatureFieldName = Guid.NewGuid().ToString();
						}
						PdfSignature pdfSignature;
						if (pdfLoadedSignatureField == null)
						{
							pdfSignature = new PdfSignature(doc, pdfLoadedPage, pdfCertificate, signatureFieldName);
						}
						else
						{
							pdfSignature = new PdfSignature(doc, pdfLoadedPage, pdfCertificate, signatureFieldName, pdfLoadedSignatureField);
						}
						pdfSignature.LocationInfo = ((rightElement != null) ? rightElement.Location : null);
						pdfSignature.Reason = ((rightElement != null) ? rightElement.Reason : null);
						if (bounds != null)
						{
							pdfSignature.Bounds = bounds.Value;
						}
						pdfSignature.Certificated = certificated;
						pdfSignature.Certificate = pdfCertificate;
						pdfSignature.Settings.CryptographicStandard = CryptographicStandard.CMS;
						pdfSignature.Settings.DigestAlgorithm = DigestAlgorithm.SHA256;
						if (bounds != null)
						{
							float num4 = bounds.Value.Width;
							float num5 = bounds.Value.Height;
							if (pdfLoadedPage.Rotation != PdfPageRotateAngle.RotateAngle0)
							{
								pdfSignature.Appearance.Normal.Graphics.TranslateTransform(num4 / 2f, num5 / 2f);
								float num6;
								switch (pdfLoadedPage.Rotation)
								{
								case PdfPageRotateAngle.RotateAngle90:
									num6 = 270f;
									num4 = bounds.Value.Height;
									num5 = bounds.Value.Width;
									break;
								case PdfPageRotateAngle.RotateAngle180:
									num6 = 180f;
									num4 = bounds.Value.Width;
									num5 = bounds.Value.Height;
									break;
								case PdfPageRotateAngle.RotateAngle270:
									num6 = 90f;
									num4 = bounds.Value.Height;
									num5 = bounds.Value.Width;
									break;
								default:
									throw new InvalidOperationException("Rotation");
								}
								pdfSignature.Appearance.Normal.Graphics.RotateTransform(num6);
								pdfSignature.Appearance.Normal.Graphics.TranslateTransform(-num4 / 2f, -num5 / 2f);
							}
							drawingHelper.Draw(pdfSignature.Appearance.Normal.Graphics, new global::System.Windows.Size((double)num4, (double)num5), default(PointF));
						}
						return Task.FromResult<bool>(true);
					}
					return Task.FromResult<bool>(false);
				}, null);
			}
			return saveResult;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0003F788 File Offset: 0x0003D988
		internal static async Task<MainViewModel.SaveResult> RemoveDigitalSignatureAndSaveAsAsync(this DigitalSignatureHelper helper, PdfDigitalSignatureField field, bool documentModified)
		{
			string name = null;
			Func<PdfLoadedSignatureField, bool> <>9__2;
			return await SignatureValidateHelper.SaveDigitalSignatureToFileAsync(helper, "Copy", documentModified, delegate
			{
				name = field.LoadedSignatureField.Name;
				return Task.CompletedTask;
			}, delegate(PdfLoadedDocument doc)
			{
				IEnumerable<PdfLoadedSignatureField> enumerable = doc.Form.Fields.OfType<PdfLoadedSignatureField>();
				Func<PdfLoadedSignatureField, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (PdfLoadedSignatureField c) => c.Name == name);
				}
				PdfLoadedSignatureField pdfLoadedSignatureField = enumerable.FirstOrDefault(func);
				if (pdfLoadedSignatureField != null)
				{
					doc.Form.Fields.Remove(pdfLoadedSignatureField);
				}
				return Task.FromResult<bool>(true);
			}, null);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0003F7DC File Offset: 0x0003D9DC
		private static async Task<MainViewModel.SaveResult> SaveDigitalSignatureToFileAsync(DigitalSignatureHelper helper, string initialFileNamePostfixOverride, bool documentModified, Func<Task> beforeSaveAction, Func<PdfLoadedDocument, Task<bool>> afterSavingAction, Func<string, FileInfo, Task> afterSavedAction)
		{
			SignatureValidateHelper.<>c__DisplayClass13_0 CS$<>8__locals1 = new SignatureValidateHelper.<>c__DisplayClass13_0();
			CS$<>8__locals1.beforeSaveAction = beforeSaveAction;
			CS$<>8__locals1.afterSavingAction = afterSavingAction;
			CS$<>8__locals1.afterSavedAction = afterSavedAction;
			PdfControl pdfControl = PdfControl.GetPdfControl(helper.PdfiumDocument);
			MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			MainViewModel.SaveResult saveResult2;
			if (mainViewModel != null)
			{
				SignatureValidateHelper.<>c__DisplayClass13_1 CS$<>8__locals2 = new SignatureValidateHelper.<>c__DisplayClass13_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.password = mainViewModel.DocumentWrapper.EncryptManage.UserPassword;
				CS$<>8__locals2.signSuccessed = false;
				MainViewModel.SaveResult saveResult = await mainViewModel.SaveAsync(new MainViewModel.SaveOptions
				{
					ForceSaveAs = true,
					AllowSaveToCurrentFile = false,
					InitialFileNamePostfixOverride = initialFileNamePostfixOverride,
					ShowProgress = true,
					DocumentModified = documentModified,
					RemoveExistsDigitalSignaturesWhenSaveAs = false,
					BeforeSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.BeforeSaveActionArgs args)
					{
						SignatureValidateHelper.<>c__DisplayClass13_0.<<SaveDigitalSignatureToFileAsync>b__0>d <<SaveDigitalSignatureToFileAsync>b__0>d;
						<<SaveDigitalSignatureToFileAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
						<<SaveDigitalSignatureToFileAsync>b__0>d.<>4__this = CS$<>8__locals2.CS$<>8__locals1;
						<<SaveDigitalSignatureToFileAsync>b__0>d.<>1__state = -1;
						<<SaveDigitalSignatureToFileAsync>b__0>d.<>t__builder.Start<SignatureValidateHelper.<>c__DisplayClass13_0.<<SaveDigitalSignatureToFileAsync>b__0>d>(ref <<SaveDigitalSignatureToFileAsync>b__0>d);
						return <<SaveDigitalSignatureToFileAsync>b__0>d.<>t__builder.Task;
					},
					AfterSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.SaveResult result)
					{
						SignatureValidateHelper.<>c__DisplayClass13_1.<<SaveDigitalSignatureToFileAsync>b__1>d <<SaveDigitalSignatureToFileAsync>b__1>d;
						<<SaveDigitalSignatureToFileAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<SaveDigitalSignatureToFileAsync>b__1>d.<>4__this = CS$<>8__locals2;
						<<SaveDigitalSignatureToFileAsync>b__1>d.result = result;
						<<SaveDigitalSignatureToFileAsync>b__1>d.<>1__state = -1;
						<<SaveDigitalSignatureToFileAsync>b__1>d.<>t__builder.Start<SignatureValidateHelper.<>c__DisplayClass13_1.<<SaveDigitalSignatureToFileAsync>b__1>d>(ref <<SaveDigitalSignatureToFileAsync>b__1>d);
						return <<SaveDigitalSignatureToFileAsync>b__1>d.<>t__builder.Task;
					}
				});
				if (CS$<>8__locals2.signSuccessed)
				{
					saveResult2 = saveResult;
				}
				else
				{
					saveResult2 = new MainViewModel.SaveResult(MainViewModel.SaveFailedResult.Unknown);
				}
			}
			else
			{
				saveResult2 = new MainViewModel.SaveResult(MainViewModel.SaveFailedResult.DocumentNotExist);
			}
			return saveResult2;
		}

		// Token: 0x04000583 RID: 1411
		private const bool GlobalUseDefaultCertificates = true;
	}
}
