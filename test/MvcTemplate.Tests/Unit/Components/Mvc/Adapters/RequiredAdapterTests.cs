using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class RequiredAdapterTests
    {
        private RequiredAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public RequiredAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            adapter = new RequiredAdapter(new RequiredAttribute());
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        [Fact]
        public void AddValidation_Required()
        {
            adapter.AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Required", context.ModelMetadata.PropertyName), attributes["data-val-required"]);
        }

        [Fact]
        public void GetErrorMessage_Required()
        {
            String expected = Validation.For("Required", context.ModelMetadata.PropertyName);
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(Validation.For("Required"), adapter.Attribute.ErrorMessage);
            Assert.Equal(expected, actual);
        }
    }
}
