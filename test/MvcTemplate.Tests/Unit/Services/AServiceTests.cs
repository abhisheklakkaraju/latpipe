using MvcTemplate.Data;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Services.Tests
{
    public class AServiceTests : IDisposable
    {
        private IUnitOfWork unitOfWork;
        private AService service;

        public AServiceTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            service = Substitute.ForPartsOf<AService>(unitOfWork);
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            service.Dispose();
        }

        [Fact]
        public void Dispose_UnitOfWork()
        {
            service.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            service.Dispose();
            service.Dispose();
        }
    }
}
