using MvcTemplate.Resources;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class LessThanAttributeTests
    {
        private LessThanAttribute attribute;

        public LessThanAttributeTests()
        {
            attribute = new LessThanAttribute(12.56);
        }

        [Fact]
        public void LessThanAttribute_SetsMaximum()
        {
            Decimal actual = new LessThanAttribute(12.56).Maximum;
            Decimal expected = 12.56M;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new LessThanAttribute(12.56);

            String actual = attribute.FormatErrorMessage("Sum");
            String expected = Validation.For("LessThan", "Sum", attribute.Maximum);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Theory]
        [InlineData("100")]
        [InlineData(12.56)]
        [InlineData(12.561)]
        public void IsValid_GreaterOrEqualValue_ReturnsFalse(Object value)
        {
            Assert.False(attribute.IsValid(value));
        }

        [Theory]
        [InlineData(12)]
        [InlineData(-50)]
        public void IsValid_LesserValue(Object value)
        {
            Assert.True(attribute.IsValid(value));
        }

        [Fact]
        public void IsValid_NotDecimal_ReturnsFalse()
        {
            Assert.False(attribute.IsValid("12.55M"));
        }
    }
}
