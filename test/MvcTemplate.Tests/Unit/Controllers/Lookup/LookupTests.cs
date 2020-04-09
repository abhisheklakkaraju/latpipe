using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Lookups;
using MvcTemplate.Data;
using MvcTemplate.Objects;
using NonFactors.Mvc.Lookup;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class LookupTests : ControllerTests
    {
        private Lookup controller;
        private IUnitOfWork unitOfWork;
        private LookupFilter filter;
        private ALookup lookup;

        public LookupTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            controller = Substitute.ForPartsOf<Lookup>(unitOfWork);

            lookup = Substitute.For<ALookup>();
            filter = new LookupFilter();
        }
        public override void Dispose()
        {
            controller.Dispose();
            unitOfWork.Dispose();
        }

        [Fact]
        public void GetData_SetsFilter()
        {
            controller.GetData(lookup, filter);

            LookupFilter actual = lookup.Filter;
            LookupFilter expected = filter;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetData_ReturnsJsonResult()
        {
            lookup.GetData().Returns(new LookupData());

            Object actual = controller.GetData(lookup, filter).Value;
            Object expected = lookup.GetData();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Role_ReturnsRolesData()
        {
            Object expected = GetData<MvcLookup<Role, RoleView>>(controller);
            Object actual = controller.Role(filter);

            Assert.Same(expected, actual);
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

        private JsonResult GetData<TLookup>(Lookup lookupController) where TLookup : ALookup
        {
            JsonResult result = new JsonResult("Test");
            lookupController.GetData(Arg.Any<TLookup>(), filter).Returns(result);

            return result;
        }
    }
}
