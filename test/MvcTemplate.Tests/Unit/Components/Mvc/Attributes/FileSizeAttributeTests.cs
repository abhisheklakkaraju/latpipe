using Microsoft.AspNetCore.Http;
using MvcTemplate.Resources;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class FileSizeAttributeTests
    {
        private FileSizeAttribute attribute;

        public FileSizeAttributeTests()
        {
            attribute = new FileSizeAttribute(12.25);
        }

        [Fact]
        public void FileSizeAttribute_SetsMaximumMB()
        {
            Decimal actual = new FileSizeAttribute(12.25).MaximumMB;
            Decimal expected = 12.25M;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new FileSizeAttribute(12.25);

            String expected = Validation.For("FileSize", "File", attribute.MaximumMB);
            String actual = attribute.FormatErrorMessage("File");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Fact]
        public void IsValid_NotIFormFileValueIsValid()
        {
            Assert.True(attribute.IsValid("100"));
        }

        [Theory]
        [InlineData(240546)]
        [InlineData(12845056)]
        public void IsValid_LowerOrEqualFileSize(Int64 size)
        {
            IFormFile file = Substitute.For<IFormFile>();
            file.Length.Returns(size);

            Assert.True(attribute.IsValid(file));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumIsNotValid()
        {
            IFormFile file = Substitute.For<IFormFile>();
            file.Length.Returns(12845057);

            Assert.False(attribute.IsValid(file));
        }

        [Theory]
        [InlineData(240546, 4574)]
        [InlineData(12840000, 5056)]
        public void IsValid_LowerOrEqualFileSizes(Int64 firstFileSize, Int64 secondFileSize)
        {
            IFormFile?[] files = { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() };
            files[1]?.Length.Returns(secondFileSize);
            files[0]?.Length.Returns(firstFileSize);

            Assert.True(attribute.IsValid(files));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumSizesAreNotValid()
        {
            IFormFile?[] files = { Substitute.For<IFormFile>(), Substitute.For<IFormFile>() };
            files[1]?.Length.Returns(12840000);
            files[0]?.Length.Returns(5057);

            Assert.False(attribute.IsValid(files));
        }
    }
}
