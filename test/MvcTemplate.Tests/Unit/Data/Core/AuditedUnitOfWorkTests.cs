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
        private Role model;
        private DbContext context;
        private AuditedUnitOfWork unitOfWork;

        public AuditedUnitOfWorkTests()
        {
            context = TestingContext.Create();
            model = ObjectsFactory.CreateRole(0);
            unitOfWork = new AuditedUnitOfWork(context, 1);

            context.Drop().Add(model);
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
            context = TestingContext.Create();
            unitOfWork = new AuditedUnitOfWork(context, 1);
            unitOfWork.Insert(ObjectsFactory.CreateRole(1));

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<AModel>().Single());

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

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<AModel>().Single());

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

            LoggableEntity expected = new LoggableEntity(context.ChangeTracker.Entries<AModel>().Single());

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

            unitOfWork.Insert(ObjectsFactory.CreateRole(1));
            unitOfWork.Commit();

            Boolean actual = context.ChangeTracker.AutoDetectChangesEnabled;
            Boolean expected = detectChanges;

            Assert.Equal(expected, actual);
        }
    }
}
