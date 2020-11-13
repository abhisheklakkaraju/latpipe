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
    public class FormLabelTagHelperTests
    {
        [Theory]
        [InlineData(typeof(String), true, null, "*")]
        [InlineData(typeof(String), true, true, "*")]
        [InlineData(typeof(String), true, false, "")]
        [InlineData(typeof(String), false, null, "")]
        [InlineData(typeof(String), false, true, "*")]
        [InlineData(typeof(String), false, false, "")]
        [InlineData(typeof(Boolean), true, null, "")]
        [InlineData(typeof(Boolean), true, true, "*")]
        [InlineData(typeof(Boolean), true, false, "")]
        public void Process_Label(Type type, Boolean metadataRequired, Boolean? required, String require)
        {
            FormLabelTagHelper helper = new();
            TagHelperContent content = new DefaultTagHelperContent();
            TagHelperAttribute[] attributes = { new TagHelperAttribute("for", "Test") };
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(type));
            TagHelperOutput output = new("label", new TagHelperAttributeList(attributes), (_, __) => Task.FromResult(content));

            helper.For = new ModelExpression("Total.Sum", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null));
            metadata.IsRequired.Returns(metadataRequired);
            metadata.DisplayName.Returns("Progix");
            helper.Required = required;

            helper.Process(null, output);

            Assert.Equal("Test", output.Attributes["for"].Value);
            Assert.Equal($"<span class=\"require\">{require}</span>", output.Content.GetContent());
        }
    }
}
