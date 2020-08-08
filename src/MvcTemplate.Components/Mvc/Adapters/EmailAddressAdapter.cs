using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Components.Mvc
{
    public class EmailAddressAdapter : AttributeAdapterBase<EmailAddressAttribute>
    {
        public EmailAddressAdapter(EmailAddressAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For("Email");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-email"] = GetErrorMessage(context);
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
