using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
        private TestingContext context;
        private Query<TestModel> select;

        public QueryTests()
        {
            context = new TestingContext();
            select = new Query<TestModel>(context.Set<TestModel>());

            context.RemoveRange(context.Set<TestModel>());
            context.Add(ObjectsFactory.CreateTestModel(0));
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void ElementType_IsModelType()
        {
            Object expected = typeof(TestModel);
            Object actual = select.ElementType;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Expression_IsSetsExpression()
        {
            DbSet<TestModel> set = Substitute.For<DbSet<TestModel>, IQueryable>();
            TestingContext testingContext = Substitute.For<TestingContext>();
            ((IQueryable)set).Expression.Returns(Expression.Empty());
            testingContext.Set<TestModel>().Returns(set);

            select = new Query<TestModel>(testingContext.Set<TestModel>());

            Object expected = ((IQueryable)set).Expression;
            Object actual = select.Expression;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Provider_IsSetsProvider()
        {
            Object expected = ((IQueryable)context.Set<TestModel>()).Provider;
            Object actual = select.Provider;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Select_Selects()
        {
            IEnumerable<Int64> expected = context.Set<TestModel>().Select(model => model.Id);
            IEnumerable<Int64> actual = select.Select(model => model.Id);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Where_Filters(Boolean predicate)
        {
            IEnumerable<TestModel> expected = context.Set<TestModel>().Where(_ => predicate);
            IEnumerable<TestModel> actual = select.Where(_ => predicate);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void To_ProjectsSet()
        {
            IEnumerable<Int64> expected = context.Set<TestModel>().ProjectTo<TestView>().Select(view => view.Id).ToArray();
            IEnumerable<Int64> actual = select.To<TestView>().Select(view => view.Id).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_ReturnsSetEnumerator()
        {
            IEnumerable<TestModel> expected = context.Set<TestModel>();
            IEnumerable<TestModel> actual = select.ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_ReturnsSameEnumerator()
        {
            IEnumerable<TestModel> expected = context.Set<TestModel>();
            IEnumerable<TestModel> actual = select;

            Assert.Equal(expected, actual);
        }
    }
}
