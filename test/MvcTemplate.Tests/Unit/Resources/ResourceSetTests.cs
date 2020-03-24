using System;
using Xunit;

namespace MvcTemplate.Resources.Tests
{
    public class ResourceSetTests
    {
        private ResourceSet resource;

        public ResourceSetTests()
        {
            resource = new ResourceSet();
        }

        [Theory]
        [InlineData("Test", "Group", "Key")]
        [InlineData("Language", "Test", "Key")]
        [InlineData("Language", "Group", "Test")]
        public void Indexer_NotFound_ReturnsNull(String language, String group, String key)
        {
            resource["Language", "Group", "Key"] = "test";

            Assert.Null(resource[language, group, key]);
        }

        [Fact]
        public void Indexer_Key_IsCaseInsensitive()
        {
            resource["A", "B", "C"] = "test resource";

            String? actual = resource["A", "B", "c"];
            String? expected = "test resource";

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Test", "Group", "Key")]
        [InlineData("Language", "Test", "Key")]
        [InlineData("Language", "Group", "Test")]
        public void Indexer_SetsResource(String language, String group, String key)
        {
            resource["Language", "Group", "Key"] = "existing resource";
            resource[language, group, key] = "test resource";

            String? actual = resource[language, group, key];
            String? expected = "test resource";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Override_Resources()
        {
            resource["Language", "Group", "Key"] = "existing resource";

            resource.Override("Language", @"{ ""Group"": { ""Test"": ""test"", ""Key"": ""new resource"" } }");

            Assert.Equal("new resource", resource["Language", "Group", "Key"]);
            Assert.Equal("test", resource["Language", "Group", "Test"]);
        }

        [Fact]
        public void Inherit_Resources()
        {
            ResourceSet fallbacks = new ResourceSet();
            resource["Language", "Group", "Key"] = "existing resource";
            fallbacks.Override("Language", @"{ ""Group"": { ""Test"": ""test"", ""Key"": ""fallback resource"" } }");

            resource.Inherit(fallbacks);

            Assert.Equal("existing resource", resource["Language", "Group", "Key"]);
            Assert.Equal("test", resource["Language", "Group", "Test"]);
        }
    }
}
