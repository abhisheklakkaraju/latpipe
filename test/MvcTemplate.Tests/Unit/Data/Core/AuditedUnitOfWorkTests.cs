using Microsoft.EntityFrameworkCore;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MvcTemplate.Data.Tests
{
    public class AuditedUnitOfWorkTests : IDisposable
    {
        private TestModel model;
        private TestingContext context;
        private AuditedUnitOfWork unitOfWork;

        public AuditedUnitOfWorkTests()
        {
            context = new TestingContext();
            model = ObjectsFactory.CreateTestModel();
            unitOfWork = new AuditedUnitOfWork(context, 1);

            context.Add(model);
            context.SaveChanges();
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            context.Dispose();
        }

        [Fact]
        public void Commit_AddedAudit()
        {
            context.Dispose();
            unitOfWork.Dispose();
            context = new TestingContext();
            unitOfWork = new AuditedUnitOfWork(context, 1);
            unitOfWork.Insert(ObjectsFactory.CreateTestModel());

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<BaseModel>().Single());

            unitOfWork.Commit();

            AuditLog actual = Assert.Single(unitOfWork.Select<AuditLog>());

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Commit_ModifiedAudit()
        {
            model.Title += "Test";

            unitOfWork.Update(model);

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<BaseModel>().Single());

            unitOfWork.Commit();

            AuditLog actual = Assert.Single(unitOfWork.Select<AuditLog>());

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Commit_NoChanges_DoesNotAudit()
        {
            unitOfWork.Update(model);
            unitOfWork.Commit();

            Assert.Empty(unitOfWork.Select<AuditLog>());
        }

        [Fact]
        public void Commit_DeletedAudit()
        {
            unitOfWork.Delete(model);

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<BaseModel>().Single());

            unitOfWork.Commit();

            AuditLog actual = Assert.Single(unitOfWork.Select<AuditLog>());

            Assert.Equal(expected.ToString(), actual.Changes);
            Assert.Equal(expected.Name, actual.EntityName);
            Assert.Equal(expected.Action, actual.Action);
            Assert.Equal(expected.Id(), actual.EntityId);
            Assert.Equal(1, actual.AccountId);
        }

        [Fact]
        public void Commit_UnsupportedState_DoesNotAudit()
        {
            IEnumerable<EntityState> unsupportedStates = Enum
                .GetValues(typeof(EntityState))
                .Cast<EntityState>()
                .Where(state =>
                    state != EntityState.Added &&
                    state != EntityState.Modified &&
                    state != EntityState.Deleted);

            foreach (EntityState state in unsupportedStates)
            {
                context.Add(model).State = state;

                unitOfWork.Commit();

                Assert.Empty(unitOfWork.Select<AuditLog>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Commit_DoesNotChangeTrackingBehaviour(Boolean detectChanges)
        {
            context.ChangeTracker.AutoDetectChangesEnabled = detectChanges;

            unitOfWork.Insert(ObjectsFactory.CreateTestModel());
            unitOfWork.Commit();

            Boolean actual = context.ChangeTracker.AutoDetectChangesEnabled;
            Boolean expected = detectChanges;

            Assert.Equal(expected, actual);
        }
    }
}
