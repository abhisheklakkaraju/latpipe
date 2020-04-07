using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Objects.Tests
{
    public class BaseModelTests
    {
        private BaseModel model;

        public BaseModelTests()
        {
            model = Substitute.For<BaseModel>();
        }

        [Fact]
        public void CreationDate_ReturnsSameValue()
        {
            DateTime expected = model.CreationDate;
            DateTime actual = model.CreationDate;

            Assert.Equal(expected, actual);
        }
    }
}
