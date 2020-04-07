using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DigitsAttribute : ValidationAttribute
    {
        public DigitsAttribute()
            : base(() => Validation.For("Digits"))
        {
        }

        public override Boolean IsValid(Object? value)
        {
            return value == null || Regex.IsMatch(value.ToString(), "^[0-9]+$");
        }
    }
}
