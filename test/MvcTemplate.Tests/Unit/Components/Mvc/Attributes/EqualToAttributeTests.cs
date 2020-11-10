using MvcTemplate.Resources;
using MvcTemplate.Tests;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class EqualToAttributeTests
    {
        private EqualToAttribute attribute;

        public EqualToAttributeTests()
        {
            attribute = new EqualToAttribute(nameof(AllTypesView.StringField));
        }

        [Fact]
        public void EqualToAttribute_SetsOtherPropertyName()
        {
            String actual = new EqualToAttribute("Other").OtherPropertyName;
            String expected = "Other";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForProperty()
        {
            attribute.OtherPropertyDisplayName = "Other";

            String actual = attribute.FormatErrorMessage("EqualTo");
            String expected = Validation.For("EqualTo", "EqualTo", attribute.OtherPropertyDisplayName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidationResult_EqualValue()
        {
            ValidationContext context = new ValidationContext(new AllTypesView { StringField = "Test" });

            Assert.Null(attribute.GetValidationResult("Test", context));
        }

        [Fact]
        public void GetValidationResult_Property_Error()
        {
            ValidationContext context = new ValidationContext(new AllTypesView());

            String? actual = attribute.GetValidationResult("Test", context)?.ErrorMessage;
            String? expected = Validation.For("EqualTo", context.DisplayName, attribute.OtherPropertyName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidationResult_NoProperty_Error()
        {
            attribute = new EqualToAttribute("Temp");
            ValidationContext context = new ValidationContext(new AllTypesView());

            String? expected = Validation.For("EqualTo", context.DisplayName, "Temp");
            String? actual = attribute.GetValidationResult("Test", context)?.ErrorMessage;

            Assert.Equal(expected, actual);
        }
    }
}
