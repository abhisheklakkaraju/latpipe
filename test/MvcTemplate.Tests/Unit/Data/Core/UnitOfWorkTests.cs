using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MvcTemplate.Data.Tests
{
    public class UnitOfWorkTests : IDisposable
    {
        private Role model;
        private UnitOfWork unitOfWork;
        private DbContext context;

        public UnitOfWorkTests()
        {
            context = TestingContext.Create();
            model = ObjectsFactory.CreateRole(0);
            unitOfWork = new UnitOfWork(context);

            context.Drop();
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            context.Dispose();
        }

        [Fact]
        public void GetAs_Null_ReturnsDestinationDefault()
        {
            Assert.Null(unitOfWork.GetAs<Role, RoleView>(null));
        }

        [Fact]
        public void GetAs_ReturnsModelAsDestinationModelById()
        {
            context.Add(model);
            context.SaveChanges();

            RoleView expected = Mapper.Map<RoleView>(model);
            RoleView actual = unitOfWork.GetAs<Role, RoleView>(model.Id)!;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Get_Null_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<Role>(null));
        }

        [Fact]
        public void Get_ModelById()
        {
            context.Add(model);
            context.SaveChanges();

            Role expected = context.Set<Role>().AsNoTracking().Single();
            Role actual = unitOfWork.Get<Role>(model.Id)!;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Get_NotFound_ReturnsNull()
        {
            Assert.Null(unitOfWork.Get<Role>(0));
        }

        [Fact]
        public void To_ConvertsSourceToDestination()
        {
            RoleView actual = unitOfWork.To<RoleView>(model);
            RoleView expected = Mapper.Map<RoleView>(model);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Select_FromSet()
        {
            context.Add(model);
            context.SaveChanges();

            IEnumerable<Role> actual = unitOfWork.Select<Role>();
            IEnumerable<Role> expected = context.Set<Role>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InsertRange_AddsModelsToDbSet()
        {
            IEnumerable<Role> models = new[] { ObjectsFactory.CreateRole(2), ObjectsFactory.CreateRole(3) };
            DbContext testingContext = Substitute.For<DbContext>();

            unitOfWork.Dispose();

            unitOfWork = new UnitOfWork(testingContext);
            unitOfWork.InsertRange(models);

            foreach (Role model in models)
                testingContext.Received().Add(model);
        }

        [Fact]
        public void Insert_AddsModelToDbSet()
        {
            unitOfWork.Insert(model);

            AModel actual = context.ChangeTracker.Entries<Role>().Single().Entity;
            AModel expected = model;

            Assert.Equal(EntityState.Added, context.Entry(model).State);
            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData(EntityState.Added, EntityState.Added)]
        [InlineData(EntityState.Deleted, EntityState.Deleted)]
        [InlineData(EntityState.Detached, EntityState.Modified)]
        [InlineData(EntityState.Modified, EntityState.Modified)]
        [InlineData(EntityState.Unchanged, EntityState.Unchanged)]
        public void Update_Entry(EntityState initialState, EntityState state)
        {
            EntityEntry<Role> entry = context.Entry(model);
            entry.State = initialState;

            unitOfWork.Update(model);

            EntityEntry<Role> actual = entry;

            Assert.Equal(state, actual.State);
            Assert.False(actual.Property(prop => prop.CreationDate).IsModified);
        }

        [Fact]
        public void DeleteRange_Models()
        {
            IEnumerable<Role> models = new[] { ObjectsFactory.CreateRole(2), ObjectsFactory.CreateRole(3) };

            context.AddRange(models);
            context.SaveChanges();

            unitOfWork.DeleteRange(models);
            unitOfWork.Commit();

            Assert.Empty(context.Set<Role>());
        }

        [Fact]
        public void Delete_Model()
        {
            context.Add(model);
            context.SaveChanges();

            unitOfWork.Delete(model);
            unitOfWork.Commit();

            Assert.Empty(context.Set<Role>());
        }

        [Fact]
        public void Delete_ModelById()
        {
            context.Add(model);
            context.SaveChanges();

            unitOfWork.Delete<Role>(model.Id);
            unitOfWork.Commit();

            Assert.Empty(context.Set<Role>());
        }

        [Fact]
        public void Commit_SavesChanges()
        {
            using DbContext testingContext = Substitute.For<DbContext>();
            using UnitOfWork testingUnitOfWork = new UnitOfWork(testingContext);

            testingUnitOfWork.Commit();

            testingContext.Received().SaveChanges();
        }

        [Fact]
        public void Dispose_Context()
        {
            DbContext testingContext = Substitute.For<DbContext>();

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
