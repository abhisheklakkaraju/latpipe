using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MvcTemplate.Components.Mvc
{
    public class MaxLengthAdapter : AttributeAdapterBase<MaxLengthAttribute>
    {
        public MaxLengthAdapter(MaxLengthAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For("MaxLength");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-length"] = GetErrorMessage(context);
            context.Attributes["data-val-length-max"] = Attribute.Length.ToString(CultureInfo.InvariantCulture);
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
