using System;
using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class DateInPastAttribute : ValidationAttribute
{
    public DateInPastAttribute()
        : base("The {0} field must be a date in the past.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is DateTime date)
        {
            return date.Date < DateTime.Today
                ? ValidationResult.Success
                : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return new ValidationResult($"The {validationContext.DisplayName} field is not a valid date.");
    }
}
