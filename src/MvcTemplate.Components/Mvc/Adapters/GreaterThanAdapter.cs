using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Globalization;

namespace MvcTemplate.Components.Mvc
{
    public class GreaterThanAdapter : AttributeAdapterBase<GreaterThanAttribute>
    {
        public GreaterThanAdapter(GreaterThanAttribute attribute)
            : base(attribute, null)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-greater"] = GetErrorMessage(context);
            context.Attributes["data-val-greater-than"] = Attribute.Minimum.ToString(CultureInfo.InvariantCulture);
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
