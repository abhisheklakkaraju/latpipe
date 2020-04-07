using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class EqualToAdapterTests
    {
        private EqualToAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public EqualToAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            adapter = new EqualToAdapter(new EqualToAttribute(nameof(AllTypesView.StringField)));
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        [Fact]
        public void AddValidation_EqualTo()
        {
            adapter.AddValidation(context);

            Assert.Equal(2, attributes.Count);
            Assert.Equal(adapter.Attribute.OtherPropertyName, attributes["data-val-equalto-other"]);
            Assert.Equal(Validation.For("EqualTo", context.ModelMetadata.PropertyName, adapter.Attribute.OtherPropertyName), attributes["data-val-equalto"]);
        }

        [Fact]
        public void GetErrorMessage_EqualTo()
        {
            String expected = Validation.For("EqualTo", context.ModelMetadata.PropertyName, "");
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(expected, actual);
        }
    }
}
