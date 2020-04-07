using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Globalization;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NumberAttribute : ValidationAttribute
    {
        public UInt32 Scale { get; }
        public UInt32 Precision { get; set; }

        public NumberAttribute(UInt32 precision, UInt32 scale)
            : base(() => Validation.For("Number"))
        {
            Precision = precision;
            Scale = scale;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Precision - Scale, Scale);
        }
        public override Boolean IsValid(Object? value)
        {
            if (value == null)
                return true;

            try
            {
                SqlDecimal number = new SqlDecimal(Trim(Convert.ToDecimal(value)));

                return number.Precision - number.Scale <= Precision - Scale && number.Scale <= Scale;
            }
            catch
            {
                return false;
            }
        }

        private Decimal Trim(Decimal value)
        {
            String trimmed = Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
            trimmed = trimmed.Contains('.') ? trimmed.TrimEnd('0') : trimmed;

            return Convert.ToDecimal(trimmed, CultureInfo.InvariantCulture);
        }
    }
}
