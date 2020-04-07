using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Objects.Tests
{
    public class BaseViewTests
    {
        private BaseView view;

        public BaseViewTests()
        {
            view = Substitute.For<BaseView>();
        }

        [Fact]
        public void CreationDate_ReturnsSameValue()
        {
            DateTime expected = view.CreationDate;
            DateTime actual = view.CreationDate;

            Assert.Equal(expected, actual);
        }
    }
}
