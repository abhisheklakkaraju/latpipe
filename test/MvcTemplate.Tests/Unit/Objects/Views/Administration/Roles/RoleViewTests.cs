using MvcTemplate.Components;
using Xunit;

namespace MvcTemplate.Objects.Tests
{
    public class RoleViewTests
    {
        [Fact]
        public void RoleView_CreatesEmpty()
        {
            MvcTree actual = new RoleView().Permissions;

            Assert.Empty(actual.SelectedIds);
            Assert.Empty(actual.Nodes);
        }
    }
}
