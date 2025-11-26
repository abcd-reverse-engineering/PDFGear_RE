using System;
using System.Globalization;
using System.Windows.Controls;

namespace pdfconverter.Utils
{
	// Token: 0x02000045 RID: 69
	internal class TextNumberValidationRules : ValidationRule
	{
		// Token: 0x0600051B RID: 1307 RVA: 0x0001512C File Offset: 0x0001332C
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int num = 0;
			string text = (value ?? "").ToString();
			if (!string.IsNullOrWhiteSpace(text) && int.TryParse(text, out num))
			{
				return ValidationResult.ValidResult;
			}
			return new ValidationResult(false, "Please enter valid data");
		}
	}
}
