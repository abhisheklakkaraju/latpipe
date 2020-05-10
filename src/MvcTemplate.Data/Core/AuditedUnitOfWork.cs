using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Objects;

namespace MvcTemplate.Data
{
    public class AuditedUnitOfWork : UnitOfWork
    {
        private Int64? AccountId { get; }

        public AuditedUnitOfWork(DbContext context, Int64? accountId)
            : base(context)
        {
            AccountId = accountId;
        }

        public override void Commit()
        {
            LoggableEntity[] entities = Context
                .ChangeTracker
                .Entries<AModel>()
                .Where(entry =>
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Deleted ||
                    entry.State == EntityState.Modified)
                .Select(entry => new LoggableEntity(entry))
                .Where(entity => entity.IsModified)
                .ToArray();

            Context.SaveChanges();

            AddTrail(entities);
        }

        private void AddTrail(LoggableEntity[] entities)
        {
            Boolean detectChanges = Context.ChangeTracker.AutoDetectChangesEnabled;
            Context.ChangeTracker.AutoDetectChangesEnabled = false;

            foreach (LoggableEntity entity in entities)
                Context.Add(new AuditLog
                {
                    Changes = entity.ToString(),
                    EntityName = entity.Name,
                    Action = entity.Action,
                    EntityId = entity.Id(),
                    AccountId = AccountId
                });

            Context.ChangeTracker.AutoDetectChangesEnabled = detectChanges;
            Context.SaveChanges();
        }
    }
}
