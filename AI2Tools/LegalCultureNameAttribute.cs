using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AI2Tools;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class LegalCultureNameAttribute : ValidationAttribute
{
	public LegalCultureNameAttribute() : base("'{0}' is an invalid culture name.")
	{
	}

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is string name)
		{
			try
			{
				CultureInfo.GetCultureInfo(name);
				return ValidationResult.Success;
			}
			catch
			{
			}
		}

		return new ValidationResult(FormatErrorMessage(value?.ToString() ?? string.Empty));
	}
}
