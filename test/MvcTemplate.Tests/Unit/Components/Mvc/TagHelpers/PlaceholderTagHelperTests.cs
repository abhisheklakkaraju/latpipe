using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class PlaceholderTagHelperTests
    {
        [Fact]
        public void Process_Placeholder()
        {
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (_, __) => null);
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            PlaceholderTagHelper helper = new PlaceholderTagHelper { For = new ModelExpression("Total", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            metadata.DisplayName.Returns("Test");

            helper.Process(null, output);

            Assert.Single(output.Attributes);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("Test", output.Attributes["placeholder"].Value);
        }
    }
}
