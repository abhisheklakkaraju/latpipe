using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MvcTemplate.Components.Lookups.Tests
{
    public class MvcLookupTests : IDisposable
    {
        private IUnitOfWork unitOfWork;
        private MvcLookup<Role, RoleView> lookup;

        public MvcLookupTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            lookup = new MvcLookup<Role, RoleView>(unitOfWork);
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        [Fact]
        public void GetColumnHeader_ReturnsPropertyTitle()
        {
            String actual = lookup.GetColumnHeader(typeof(RoleView).GetProperty(nameof(RoleView.Title))!);
            String expected = Resource.ForProperty(typeof(RoleView), nameof(RoleView.Title));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetColumnHeader_ReturnsRelationPropertyTitle()
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty(nameof(AllTypesView.Child))!;

            Assert.Empty(lookup.GetColumnHeader(property));
        }

        [Theory]
        [InlineData(nameof(AllTypesView.EnumField), "text-left")]
        [InlineData(nameof(AllTypesView.SByteField), "text-right")]
        [InlineData(nameof(AllTypesView.ByteField), "text-right")]
        [InlineData(nameof(AllTypesView.Int16Field), "text-right")]
        [InlineData(nameof(AllTypesView.UInt16Field), "text-right")]
        [InlineData(nameof(AllTypesView.Int32Field), "text-right")]
        [InlineData(nameof(AllTypesView.UInt32Field), "text-right")]
        [InlineData(nameof(AllTypesView.Int64Field), "text-right")]
        [InlineData(nameof(AllTypesView.UInt64Field), "text-right")]
        [InlineData(nameof(AllTypesView.SingleField), "text-right")]
        [InlineData(nameof(AllTypesView.DoubleField), "text-right")]
        [InlineData(nameof(AllTypesView.DecimalField), "text-right")]
        [InlineData(nameof(AllTypesView.BooleanField), "text-center")]
        [InlineData(nameof(AllTypesView.DateTimeField), "text-center")]

        [InlineData(nameof(AllTypesView.NullableEnumField), "text-left")]
        [InlineData(nameof(AllTypesView.NullableSByteField), "text-right")]
        [InlineData(nameof(AllTypesView.NullableByteField), "text-right")]
        [InlineData(nameof(AllTypesView.NullableInt16Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableUInt16Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableInt32Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableUInt32Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableInt64Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableUInt64Field), "text-right")]
        [InlineData(nameof(AllTypesView.NullableSingleField), "text-right")]
        [InlineData(nameof(AllTypesView.NullableDoubleField), "text-right")]
        [InlineData(nameof(AllTypesView.NullableDecimalField), "text-right")]
        [InlineData(nameof(AllTypesView.NullableBooleanField), "text-center")]
        [InlineData(nameof(AllTypesView.NullableDateTimeField), "text-center")]

        [InlineData(nameof(AllTypesView.StringField), "text-left")]
        [InlineData(nameof(AllTypesView.Child), "text-left")]
        public void GetColumnCssClass_ReturnsCssClassForPropertyType(String propertyName, String cssClass)
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty(propertyName)!;

            String actual = lookup.GetColumnCssClass(property);
            String expected = cssClass;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetModels_FromUnitOfWork()
        {
            unitOfWork.Select<Role>().To<RoleView>().Returns(Array.Empty<RoleView>().AsQueryable());

            Object actual = new MvcLookup<Role, RoleView>(unitOfWork).GetModels();
            Object expected = unitOfWork.Select<Role>().To<RoleView>();

            Assert.Same(expected, actual);
        }
    }
}
