using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class SiteMapNodeTests
    {
        private IUrlHelper url;

        public SiteMapNodeTests()
        {
            url = Substitute.For<IUrlHelper>();
        }

        [Fact]
        public void SiteMapNode_Empty()
        {
            SiteMapNode actual = new SiteMapNode();

            Assert.Empty(actual.Children);
            Assert.Empty(actual.Route);
        }
        [Fact]
        public void Form_EmptyActionUrl()
        {
            Assert.Equal("#", new SiteMapNode().Form(url));
        }
    }
}
