using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Data.Logging;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MvcTemplate.Data.Core.Tests
{
    public class UnitOfWorkTests : IDisposable
    {
        private TestingContext context;
        private UnitOfWork unitOfWork;
        private IAuditLogger logger;
        private TestModel model;

        public UnitOfWorkTests()
        {
            context = new TestingContext();
            logger = Substitute.For<IAuditLogger>();
            model = ObjectsFactory.CreateTestModel();
            unitOfWork = new UnitOfWork(context, logger);
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            context.Dispose();
            logger.Dispose();
        }

        [Fact]
        public void GetAs_Null_ReturnsDestinationDefault()
        {
            Assert.Null(unitOfWork.GetAs<TestModel, TestView>(null));
        }

        [Fact]
        public void GetAs_ReturnsModelAsDestinationModelById()
        {
            context.Add(model);
            context.SaveChanges();

            TestView expected = Mapper.Map<TestView>(model);
            TestView actual = unitOfWork.GetAs<TestModel, TestView>(model.Id)!;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Get_Null_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<TestModel>(null));
        }

        [Fact]
        public void Get_ModelById()
        {
            context.Add(model);
            context.SaveChanges();

            TestModel expected = context.Set<TestModel>().AsNoTracking().Single();
            TestModel actual = unitOfWork.Get<TestModel>(model.Id)!;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Get_NotFound_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<TestModel>(0));
        }

        [Fact]
        public void To_ConvertsSourceToDestination()
        {
            TestView actual = unitOfWork.To<TestView>(model);
            TestView expected = Mapper.Map<TestView>(model);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Select_FromSet()
        {
            context.Add(model);
            context.SaveChanges();

            IEnumerable<TestModel> actual = unitOfWork.Select<TestModel>();
            IEnumerable<TestModel> expected = context.Set<TestModel>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InsertRange_AddsModelsToDbSet()
        {
            IEnumerable<TestModel> models = new[] { ObjectsFactory.CreateTestModel(2), ObjectsFactory.CreateTestModel(3) };
            TestingContext testingContext = Substitute.For<TestingContext>();

            unitOfWork.Dispose();

            unitOfWork = new UnitOfWork(testingContext);
            unitOfWork.InsertRange(models);

            foreach (TestModel model in models)
                testingContext.Received().Add(model);
        }

        [Fact]
        public void Insert_AddsModelToDbSet()
        {
            unitOfWork.Insert(model);

            BaseModel actual = context.ChangeTracker.Entries<TestModel>().Single().Entity;
            BaseModel expected = model;

            Assert.Equal(EntityState.Added, context.Entry(model).State);
            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData(EntityState.Added, EntityState.Modified)]
        [InlineData(EntityState.Deleted, EntityState.Modified)]
        [InlineData(EntityState.Detached, EntityState.Modified)]
        [InlineData(EntityState.Modified, EntityState.Modified)]
        [InlineData(EntityState.Unchanged, EntityState.Unchanged)]
        public void Update_Entry(EntityState initialState, EntityState state)
        {
            EntityEntry<TestModel> entry = context.Entry(model);
            entry.State = initialState;

            unitOfWork.Update(model);

            EntityEntry<TestModel> actual = entry;

            Assert.Equal(state, actual.State);
            Assert.False(actual.Property(prop => prop.CreationDate).IsModified);
        }

        [Fact]
        public void DeleteRange_Models()
        {
            IEnumerable<TestModel> models = new[] { ObjectsFactory.CreateTestModel(2), ObjectsFactory.CreateTestModel(3) };

            context.AddRange(models);
            context.SaveChanges();

            unitOfWork.DeleteRange(models);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        [Fact]
        public void Delete_Model()
        {
            context.Add(model);
            context.SaveChanges();

            unitOfWork.Delete(model);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        [Fact]
        public void Delete_ModelById()
        {
            context.Add(model);
            context.SaveChanges();

            unitOfWork.Delete<TestModel>(model.Id);
            unitOfWork.Commit();

            Assert.Empty(context.Set<TestModel>());
        }

        [Fact]
        public void Commit_SavesChanges()
        {
            using TestingContext testingContext = Substitute.For<TestingContext>();
            using UnitOfWork testingUnitOfWork = new UnitOfWork(testingContext);

            testingUnitOfWork.Commit();

            testingContext.Received().SaveChanges();
        }

        [Fact]
        public void Commit_Logs()
        {
            unitOfWork.Commit();

            logger.Received().Log(Arg.Any<IEnumerable<EntityEntry<BaseModel>>>());
            logger.Received().Save();
        }

        [Fact]
        public void Commit_Failed_DoesNotSaveLogs()
        {
            logger.When(sub => sub.Log(Arg.Any<IEnumerable<EntityEntry<BaseModel>>>())).Do(call => throw new Exception());
            Exception? exception = Record.Exception(() => unitOfWork.Commit());

            logger.Received().Log(Arg.Any<IEnumerable<EntityEntry<BaseModel>>>());
            logger.DidNotReceive().Save();
            Assert.NotNull(exception);
        }

        [Fact]
        public void Dispose_Logger()
        {
            unitOfWork.Dispose();

            logger.Received().Dispose();
        }

        [Fact]
        public void Dispose_Context()
        {
            TestingContext testingContext = Substitute.For<TestingContext>();

            new UnitOfWork(testingContext).Dispose();

            testingContext.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            unitOfWork.Dispose();
            unitOfWork.Dispose();
        }
    }
}
