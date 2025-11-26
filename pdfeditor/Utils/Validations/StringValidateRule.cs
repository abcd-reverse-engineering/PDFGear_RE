using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace pdfeditor.Utils.Validations
{
	// Token: 0x020000B0 RID: 176
	public class StringValidateRule : ValidationRule
	{
		// Token: 0x06000ADE RID: 2782 RVA: 0x00038834 File Offset: 0x00036A34
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value == null)
			{
				return new ValidationResult(false, "The content cannot be spatial！");
			}
			string text = value.ToString();
			if (string.IsNullOrWhiteSpace(text))
			{
				return new ValidationResult(false, "The content cannot be spatial！");
			}
			if (text.Count<char>() > 50)
			{
				return new ValidationResult(false, "The input content  cannot exceed 50 characters!");
			}
			return ValidationResult.ValidResult;
		}
	}
}
