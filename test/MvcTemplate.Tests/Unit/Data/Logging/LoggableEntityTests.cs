using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace MvcTemplate.Data.Logging.Tests
{
    public class LoggableEntityTests : IDisposable
    {
        private EntityEntry<BaseModel> entry;
        private TestingContext context;
        private TestModel model;

        public LoggableEntityTests()
        {
            using (context = new TestingContext())
            {
                context.Add(model = ObjectsFactory.CreateTestModel());
                context.SaveChanges();
            }

            context = new TestingContext(context);
            entry = context.Entry<BaseModel>(model);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAddedEntity()
        {
            entry.State = EntityState.Added;

            AsssertProperties(entry.CurrentValues);
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForModifiedEntity()
        {
            String? title = model.Title;
            entry.State = EntityState.Modified;
            entry.CurrentValues[nameof(TestModel.Title)] = "Pneura";
            entry.OriginalValues[nameof(TestModel.Title)] = "Pneura";

            LoggableProperty expected = new LoggableProperty(entry.Property(nameof(TestModel.Title)), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForAttachedEntity()
        {
            context.Dispose();
            String? title = model.Title;

            context = new TestingContext(context);
            context.Set<TestModel>().Attach(model);

            entry = context.Entry<BaseModel>(model);
            entry.OriginalValues[nameof(TestModel.Title)] = "Aneura";
            entry.CurrentValues[nameof(TestModel.Title)] = "Aneura";
            entry.State = EntityState.Modified;

            LoggableProperty expected = new LoggableProperty(entry.Property(nameof(TestModel.Title)), title);
            LoggableProperty actual = new LoggableEntity(entry).Properties.Single();

            Assert.Equal(expected.IsModified, actual.IsModified);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void LoggableEntity_CreatesPropertiesForDeletedEntity()
        {
            entry.State = EntityState.Deleted;

            AsssertProperties(entry.GetDatabaseValues());
        }

        [Fact]
        public void LoggableEntity_SetsAction()
        {
            entry.State = EntityState.Deleted;

            String actual = new LoggableEntity(entry).Action;
            String expected = entry.State.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityTypeName()
        {
            String actual = new LoggableEntity(entry).Name;
            String expected = typeof(TestModel).Name;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoggableEntity_SetsEntityId()
        {
            Int64 actual = new LoggableEntity(entry).Id();
            Int64 expected = model.Id;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_FormsEntityChanges()
        {
            StringBuilder changes = new StringBuilder();
            LoggableEntity loggableEntity = new LoggableEntity(entry);
            foreach (LoggableProperty property in loggableEntity.Properties)
                changes.AppendFormat("{0}{1}", property, "\n");

            String actual = loggableEntity.ToString();
            String expected = changes.ToString();

            Assert.Equal(expected, actual);
        }

        private void AsssertProperties(PropertyValues newValues)
        {
            LoggableProperty[] actual = new LoggableEntity(entry).Properties.ToArray();
            LoggableProperty[] expected = newValues.Properties.Where(property => property.Name != "Id")
                .Select(property => new LoggableProperty(entry.Property(property.Name), newValues[property])).ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].IsModified, actual[i].IsModified);
                Assert.Equal(expected[i].ToString(), actual[i].ToString());
            }
        }
    }
}
