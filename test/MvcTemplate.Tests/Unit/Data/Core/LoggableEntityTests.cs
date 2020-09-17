using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Data.Tests
{
    public class LoggableEntityTests : IDisposable
    {
        private Role model;
        private DbContext context;
        private EntityEntry<AModel> entry;

        public LoggableEntityTests()
        {
            using (context = TestingContext.Create())
            {
                context.Drop().Add(model = ObjectsFactory.CreateRole(0));
                context.SaveChanges();
            }

            context = TestingContext.Create();
            entry = context.Entry<AModel>(model);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void LoggableEntity_SetsAction()
        {
            entry.State = EntityState.Deleted;

            String expected = nameof(EntityState.Deleted);
            String actual = new LoggableEntity(entry).Action;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsName()
        {
            String actual = new LoggableEntity(entry).Name;
            String expected = nameof(Role);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_Proxy_SetsName()
        {
            model = context.Set<Role>().Single();
            entry = context.ChangeTracker.Entries<AModel>().Single();

            String actual = new LoggableEntity(entry).Name;
            String expected = nameof(Role);

            Assert.IsAssignableFrom<IProxyTargetAccessor>(model);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsId()
        {
            Int64 expected = model.Id;
            Int64 actual = new LoggableEntity(entry).Id();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_Added_SetsIsModified()
        {
            entry.State = EntityState.Added;

            Assert.True(new LoggableEntity(entry).IsModified);
        }

        [Fact]
        public void LoggableEntity_Modified_SetsIsModified()
        {
            model.Title += "Test";
            entry.State = EntityState.Modified;

            Assert.True(new LoggableEntity(entry).IsModified);
        }

        [Fact]
        public void LoggableEntity_NotModified_SetsIsModified()
        {
            entry.State = EntityState.Modified;

            Assert.False(new LoggableEntity(entry).IsModified);
        }

        [Fact]
        public void LoggableEntity_Deleted_SetsIsModified()
        {
            entry.State = EntityState.Deleted;

            Assert.True(new LoggableEntity(entry).IsModified);
        }

        [Fact]
        public void ToString_Added_Changes()
        {
            entry.State = EntityState.Added;

            String actual = new LoggableEntity(entry).ToString();
            String expected = $"CreationDate: \"{model.CreationDate}\"\nTitle: \"{model.Title}\"\n";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_Modified_Changes()
        {
            model.Title += "Test";
            entry.State = EntityState.Modified;

            String actual = new LoggableEntity(entry).ToString();
            String expected = $"Title: \"{model.Title[..^4]}\" => \"{model.Title}\"\n";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_Deleted_Changes()
        {
            entry.State = EntityState.Deleted;

            String actual = new LoggableEntity(entry).ToString();
            String expected = $"CreationDate: \"{model.CreationDate}\"\nTitle: \"{model.Title}\"\n";

            Assert.Equal(expected, actual);
        }
    }
}
