using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class FormInputTagHelperTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("on", "on")]
        [InlineData(null, null)]
        [InlineData("off", "off")]
        public void Process_Autocomplete(String? value, String? expectedValue)
        {
            TagHelperContent content = new DefaultTagHelperContent();
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new("input", new TagHelperAttributeList(), (_, __) => Task.FromResult(content));
            FormInputTagHelper helper = new() { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            output.Attributes.Add("autocomplete", value);

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("form-control", output.Attributes["class"].Value);
            Assert.Equal(expectedValue, output.Attributes["autocomplete"].Value);
        }

        [Theory]
        [InlineData("", "form-control ")]
        [InlineData(null, "form-control ")]
        [InlineData("test", "form-control test")]
        public void Process_Class(String value, String expectedValue)
        {
            TagHelperContent content = new DefaultTagHelperContent();
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new("input", new TagHelperAttributeList(), (_, __) => Task.FromResult(content));
            FormInputTagHelper helper = new() { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            output.Attributes.Add("class", value);

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("off", output.Attributes["autocomplete"].Value);
            Assert.Equal(expectedValue, output.Attributes["class"].Value);
        }

        [Fact]
        public void Process_Input()
        {
            TagHelperContent content = new DefaultTagHelperContent();
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new("input", new TagHelperAttributeList(), (_, __) => Task.FromResult(content));
            FormInputTagHelper helper = new() { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("off", output.Attributes["autocomplete"].Value);
            Assert.Equal("form-control", output.Attributes["class"].Value);
        }
    }
}
