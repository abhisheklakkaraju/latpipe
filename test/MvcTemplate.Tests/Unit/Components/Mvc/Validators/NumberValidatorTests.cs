using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class NumberValidatorTests
    {
        [Fact]
        public void AddValidation_Number()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new Dictionary<String, String>();
            ClientModelValidationContext context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);

            new NumberValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Numeric", "Int64"), attributes["data-val-number"]);
        }

        [Fact]
        public void AddValidation_ExistingNumber()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new Dictionary<String, String> { ["data-val-number"] = "Test" };
            ClientModelValidationContext context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);

            new NumberValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal("Test", attributes["data-val-number"]);
        }
    }
}
