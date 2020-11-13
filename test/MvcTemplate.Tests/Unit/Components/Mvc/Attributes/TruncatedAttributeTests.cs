using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class TruncatedAttributeTests
    {
        private TruncatedAttribute attribute;
        private ModelBindingContext context;

        public TruncatedAttributeTests()
        {
            attribute = new TruncatedAttribute();
            context = new DefaultModelBindingContext();
            context.ActionContext = new ActionContext();
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider = Substitute.For<IValueProvider>();
            context.ActionContext.HttpContext = new DefaultHttpContext();
            context.HttpContext.RequestServices = Substitute.For<IServiceProvider>();
            context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)).Returns(Substitute.For<ILoggerFactory>());
        }

        [Fact]
        public void TruncatedAttribute_SetsBinderType()
        {
            Type expected = typeof(TruncatedAttribute);
            Type actual = attribute.BinderType;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_NoValue()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(DateTime?));
            context.ValueProvider.GetValue(context.ModelName).Returns(ValueProviderResult.None);
            context.ModelMetadata = metadata;

            await attribute.BindModelAsync(context);

            ModelBindingResult actual = context.Result;
            ModelBindingResult expected = new();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_TruncatesValue()
        {
            context.ValueProvider.GetValue(nameof(AllTypesView.TruncatedDateTimeField)).Returns(new ValueProviderResult(new DateTime(2017, 2, 3, 4, 5, 6).ToString(), CultureInfo.CurrentCulture));
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.TruncatedDateTimeField));
            context.ModelName = nameof(AllTypesView.TruncatedDateTimeField);
            context.ModelMetadata = metadata;

            await attribute.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(new DateTime(2017, 2, 3));
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }
    }
}
