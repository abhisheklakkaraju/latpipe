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
            Dictionary<String, String> attributes = new();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

            new NumberValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Number", "Int64"), attributes["data-val-number"]);
        }

        [Fact]
        public void AddValidation_ExistingNumber()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new() { ["data-val-number"] = "Test" };
            ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

            new NumberValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal("Test", attributes["data-val-number"]);
        }
    }
}
