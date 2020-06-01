using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace MvcTemplate.Data.Tests
{
    public class QueryTests : IDisposable
    {
        private DbContext context;
        private Query<Role> select;

        public QueryTests()
        {
            context = TestingContext.Create();
            select = new Query<Role>(context.Set<Role>());

            context.Drop().Add(ObjectsFactory.CreateRole(0));
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void ElementType_IsModelType()
        {
            Object expected = typeof(Role);
            Object actual = select.ElementType;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Expression_IsSetsExpression()
        {
            DbSet<Role> set = Substitute.For<DbSet<Role>, IQueryable>();
            DbContext testingContext = Substitute.For<DbContext>();
            ((IQueryable)set).Expression.Returns(Expression.Empty());
            testingContext.Set<Role>().Returns(set);

            select = new Query<Role>(testingContext.Set<Role>());

            Object expected = ((IQueryable)set).Expression;
            Object actual = select.Expression;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Provider_IsSetsProvider()
        {
            Object expected = ((IQueryable)context.Set<Role>()).Provider;
            Object actual = select.Provider;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Select_Selects()
        {
            IEnumerable<Int64> expected = context.Set<Role>().Select(model => model.Id);
            IEnumerable<Int64> actual = select.Select(model => model.Id);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Where_Filters(Boolean predicate)
        {
            IEnumerable<Role> expected = context.Set<Role>().Where(_ => predicate);
            IEnumerable<Role> actual = select.Where(_ => predicate);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void To_ProjectsSet()
        {
            IEnumerable<Int64> expected = context.Set<Role>().ProjectTo<RoleView>().Select(view => view.Id).ToArray();
            IEnumerable<Int64> actual = select.To<RoleView>().Select(view => view.Id).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_ReturnsSetEnumerator()
        {
            IEnumerable<Role> expected = context.Set<Role>();
            IEnumerable<Role> actual = select.ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_ReturnsSameEnumerator()
        {
            IEnumerable<Role> expected = context.Set<Role>();
            IEnumerable<Role> actual = select;

            Assert.Equal(expected, actual);
        }
    }
}
