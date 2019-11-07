using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;

namespace MvcTemplate.Components.Mvc
{
    public class DateValidator : IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-date"] = Validation.For("Date", context.ModelMetadata.GetDisplayName());
        }
    }
}
