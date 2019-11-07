using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;

namespace MvcTemplate.Components.Mvc
{
    public class NumberValidator : IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {

            if (!context.Attributes.ContainsKey("data-val-number"))
                context.Attributes["data-val-number"] = Validation.For("Numeric", context.ModelMetadata.GetDisplayName());
        }
    }
}
