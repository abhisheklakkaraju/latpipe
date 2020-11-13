using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class DateValidatorTests
    {
        [Fact]
        public void AddValidation_Date()
        {
            Dictionary<String, String> attributes = new();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(DateTime));
            ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

            new DateValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Date", "DateTime"), attributes["data-val-date"]);
        }
    }
}
