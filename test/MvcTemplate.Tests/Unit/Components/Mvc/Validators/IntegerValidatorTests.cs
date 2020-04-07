using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class IntegerValidatorTests
    {
        [Fact]
        public void AddValidation_Integer()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new Dictionary<String, String>();
            ClientModelValidationContext context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);

            new IntegerValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Integer", "Int64"), attributes["data-val-integer"]);
        }

        [Fact]
        public void AddValidation_ExistingInteger()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new Dictionary<String, String> { ["data-val-integer"] = "Test" };
            ClientModelValidationContext context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);

            new IntegerValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal("Test", attributes["data-val-integer"]);
        }
    }
}
