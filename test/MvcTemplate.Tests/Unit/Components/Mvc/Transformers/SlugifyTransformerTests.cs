
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class SlugifyTransformerTests
    {
        [Fact]
        public void TransformOutbound_Null()
        {
            Assert.Null(new SlugifyTransformer().TransformOutbound(null));
        }

        [Theory]
        [InlineData("test", "test")]
        [InlineData("Test", "Test")]
        [InlineData("VIPWeek", "VIP-Week")]
        [InlineData("TestWeek", "Test-Week")]
        [InlineData("TestIWeek", "Test-I-Week")]
        [InlineData("TestInWeek", "Test-In-Week")]
        [InlineData("TestWeekVIP", "Test-Week-VIP")]
        [InlineData("VIPWeekMulti", "VIP-Week-Multi")]
        [InlineData("TestWeekMulti", "Test-Week-Multi")]
        [InlineData("TestWEEKMulti", "Test-WEEK-Multi")]
        public void TransformOutbound_Value(Object value, String slug)
        {
            String? expected = slug;
            String? actual = new SlugifyTransformer().TransformOutbound(value);

            Assert.Equal(expected, actual);
        }
    }
}
