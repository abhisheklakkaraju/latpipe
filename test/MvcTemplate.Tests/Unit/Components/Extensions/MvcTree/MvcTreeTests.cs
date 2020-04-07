using Xunit;

namespace MvcTemplate.Components.Extensions.Tests
{
    public class MvcTreeTests
    {
        [Fact]
        public void MvcTree_CreatesEmpty()
        {
            MvcTree actual = new MvcTree();

            Assert.Empty(actual.Nodes);
            Assert.Empty(actual.SelectedIds);
        }
    }
}
