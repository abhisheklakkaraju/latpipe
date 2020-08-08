using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EqualToAttribute : ValidationAttribute
    {
        public String OtherPropertyName { get; }
        public String? OtherPropertyDisplayName { get; set; }

        public EqualToAttribute(String otherPropertyName)
            : base(() => Validation.For("EqualTo"))
        {
            OtherPropertyName = otherPropertyName;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }
        protected override ValidationResult IsValid(Object? value, ValidationContext validationContext)
        {
            PropertyInfo? other = validationContext.ObjectType.GetProperty(OtherPropertyName);

            if (other != null && Equals(value, other.GetValue(validationContext.ObjectInstance)))
                return ValidationResult.Success;

            OtherPropertyDisplayName = Resource.ForProperty(validationContext.ObjectType, OtherPropertyName);

            if (String.IsNullOrEmpty(OtherPropertyDisplayName))
                OtherPropertyDisplayName = OtherPropertyName;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
