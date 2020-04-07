using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MvcTemplate.Components.Mvc
{
    public class RangeAdapter : AttributeAdapterBase<RangeAttribute>
    {
        public RangeAdapter(RangeAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For("Range");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-range"] = GetErrorMessage(context);
            context.Attributes["data-val-range-min"] = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture);
            context.Attributes["data-val-range-max"] = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture);
        }
        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
