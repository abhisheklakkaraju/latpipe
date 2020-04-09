using MvcTemplate.Data;
using NSubstitute;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class LookupTests : ControllerTests
    {
        private Lookup controller;
        private IUnitOfWork unitOfWork;

        public LookupTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            controller = Substitute.ForPartsOf<Lookup>(unitOfWork);
        }
        public override void Dispose()
        {
            controller.Dispose();
            unitOfWork.Dispose();
        }

        [Fact]
        public void Dispose_UnitOfWork()
        {
            controller.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }
    }
}
