using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using pdfeditor.Properties;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DD RID: 221
	internal static class SignatureFormatHelper
	{
		// Token: 0x06000BF7 RID: 3063 RVA: 0x0003F17D File Offset: 0x0003D37D
		public static string BuildValidateResultTitle(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			if (result.IsValid(true))
			{
				return Resources.DigSignResultValid.Replace("XXX", field.SignedName);
			}
			if (result.SignatureStatus == SignatureStatus.Unknown)
			{
				return Resources.DigSignResultUnknown;
			}
			return Resources.DigSignResultInvalid;
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x0003F1B4 File Offset: 0x0003D3B4
		public static IReadOnlyList<DigitalSignatureFormattedItem> BuildValidateResultStatusContent(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			List<DigitalSignatureFormattedItem> list = new List<DigitalSignatureFormattedItem>();
			SignatureFormatHelper.AppendValidateResultStatusContent(field, result, list);
			return list;
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0003F1D0 File Offset: 0x0003D3D0
		public static IReadOnlyList<DigitalSignatureFormattedItem> BuildDigitalSignatureInfosContent(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			List<DigitalSignatureFormattedItem> list = new List<DigitalSignatureFormattedItem>();
			SignatureFormatHelper.AppendDigitalSignatureInfos(field, result, list);
			return list;
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0003F1EC File Offset: 0x0003D3EC
		public static IReadOnlyList<DigitalSignatureFormattedItem> BuildSignaturePropertiesValidationResult(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			List<DigitalSignatureFormattedItem> list = new List<DigitalSignatureFormattedItem>();
			SignatureFormatHelper.AppendValidateResultStatusContent(field, result, list);
			SignatureFormatHelper.AppendDigitalSignatureInfos(field, result, list);
			return list;
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x0003F210 File Offset: 0x0003D410
		private static void AppendValidateResultStatusContent(PdfDigitalSignatureField field, PdfSignatureValidationResult result, List<DigitalSignatureFormattedItem> list)
		{
			if (result.IsDocumentModified)
			{
				list.Add(new DigitalSignatureFormattedItem("Status_IsDocumentModified", Resources.DigSignStatusHasBeenModified));
			}
			else
			{
				list.Add(new DigitalSignatureFormattedItem("Status_IsDocumentModified", Resources.DigSignStatusHasNotBeenModified));
			}
			if (field.IsBroken)
			{
				list.Add(new DigitalSignatureFormattedItem("Status_IsValid", Resources.DigSignStatusIdentityIsBroken));
				return;
			}
			if (result.IsValid(true))
			{
				list.Add(new DigitalSignatureFormattedItem("Status_IsValid", Resources.DigSignStatusIdentityIsValid));
				return;
			}
			if (result.SignatureStatus == SignatureStatus.Unknown)
			{
				list.Add(new DigitalSignatureFormattedItem("Status_IsValid", Resources.DigSignStatusNotIncludedInTrustedCerts));
				return;
			}
			if (!result.IsValidAtSignedTime() && !result.IsValidAtTimeStampTime() && result.Certificates != null && result.Certificates.Count > 0)
			{
				DateTime now = DateTime.Now;
				foreach (X509Certificate2 x509Certificate in result.Certificates)
				{
					if (now > x509Certificate.NotAfter)
					{
						list.Add(new DigitalSignatureFormattedItem("Status_IsValid", Resources.DigSignStatusIdentityIsExpired));
						break;
					}
				}
			}
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x0003F320 File Offset: 0x0003D520
		private static void AppendDigitalSignatureInfos(PdfDigitalSignatureField field, PdfSignatureValidationResult result, List<DigitalSignatureFormattedItem> list)
		{
			PdfSignature signature = field.Signature;
			bool? flag = ((signature != null) ? new bool?(signature.Certificated) : null);
			if (flag != null && flag.GetValueOrDefault())
			{
				list.Add(new DigitalSignatureFormattedItem("Info_Certificated", Resources.DigSignInfoNotAllowModify));
			}
			if (result.TimeStampInformation != null)
			{
				string text = result.TimeStampInformation.Time.ToString("G");
				string digSignInfoIncludesEmbeddedTimestamp = Resources.DigSignInfoIncludesEmbeddedTimestamp;
				list.Add(new DigitalSignatureFormattedItem("Info_Timestamp", digSignInfoIncludesEmbeddedTimestamp.Replace("XXX", text)));
				if (result.TimeStampInformation.IsValid)
				{
					string digSignInfoValidAtTime = Resources.DigSignInfoValidAtTime;
					list.Add(new DigitalSignatureFormattedItem("Info_ValidAtTime", digSignInfoValidAtTime.Replace("XXX", text)));
					return;
				}
			}
			else
			{
				list.Add(new DigitalSignatureFormattedItem("Info_Timestamp", Resources.DigSignInfoTimeFromSignersDevice));
				if (field.Signature != null && result.IsValidAtSignedTime())
				{
					string text2 = field.Signature.SignedDate.ToString("G");
					string digSignInfoValidatedAsSigningTime = Resources.DigSignInfoValidatedAsSigningTime;
					list.Add(new DigitalSignatureFormattedItem("Info_ValidAtTime", digSignInfoValidatedAsSigningTime.Replace("XXX", text2)));
				}
			}
		}
	}
}
