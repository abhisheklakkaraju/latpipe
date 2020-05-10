using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Objects.Tests
{
    public class AViewTests
    {
        private AView view;

        public AViewTests()
        {
            view = Substitute.For<AView>();
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
