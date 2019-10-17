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
    public class StringLengthAdapterTests
    {
        private StringLengthAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public StringLengthAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            adapter = new StringLengthAdapter(new StringLengthAttribute(128));
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), "StringField");
            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        [Fact]
        public void AddValidation_StringLength()
        {
            adapter.Attribute.MinimumLength = 0;

            adapter.AddValidation(context);

            Assert.Equal(3, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal("128", attributes["data-val-length-max"]);
            Assert.Equal(Validation.For("StringLength", context.ModelMetadata.PropertyName, 128), attributes["data-val-length"]);
        }

        [Fact]
        public void AddValidation_StringLengthRange()
        {
            adapter.Attribute.MinimumLength = 4;

            adapter.AddValidation(context);

            Assert.Equal(4, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal("4", attributes["data-val-length-min"]);
            Assert.Equal("128", attributes["data-val-length-max"]);
            Assert.Equal(Validation.For("StringLengthRange", context.ModelMetadata.PropertyName, 128, 4), attributes["data-val-length"]);
        }

        [Fact]
        public void GetErrorMessage_StringLength()
        {
            adapter.Attribute.MinimumLength = 0;

            String expected = Validation.For("StringLength", context.ModelMetadata.PropertyName, 128);
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(Validation.For("StringLength"), adapter.Attribute.ErrorMessage);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetErrorMessage_StringLengthRange()
        {
            adapter.Attribute.MinimumLength = 4;

            String expected = Validation.For("StringLengthRange", context.ModelMetadata.PropertyName, 128, 4);
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(Validation.For("StringLengthRange"), adapter.Attribute.ErrorMessage);
            Assert.Equal(expected, actual);
        }
    }
}
