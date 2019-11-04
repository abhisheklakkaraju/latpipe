using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MvcTemplate.Components.Mvc
{
    public class StringLengthAdapter : AttributeAdapterBase<StringLengthAttribute>
    {
        public StringLengthAdapter(StringLengthAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For(Attribute.MinimumLength == 0 ? "StringLength" : "StringLengthRange");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-length"] = GetErrorMessage(context);
            context.Attributes["data-val-length-max"] = Attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);

            if (Attribute.MinimumLength > 0)
                context.Attributes["data-val-length-min"] = Attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
        }
        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
