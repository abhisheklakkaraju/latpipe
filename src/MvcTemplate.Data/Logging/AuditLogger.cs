using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTemplate.Data.Logging
{
    public class AuditLogger : IAuditLogger
    {
        private Int64? AccountId { get; }
        private DbContext Context { get; }
        private List<LoggableEntity> Entities { get; }

        public AuditLogger(DbContext context, Int64? accountId)
        {
            Context = context;
            AccountId = accountId;
            Entities = new List<LoggableEntity>();
        }

        public void Log(IEnumerable<EntityEntry<BaseModel>> entries)
        {
            foreach (EntityEntry<BaseModel> entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        LoggableEntity entity = new LoggableEntity(entry);
                        if (entity.Properties.Any())
                            Log(entity);
                        break;
                }
            }
        }
        public void Log(LoggableEntity entity)
        {
            Entities.Add(entity);
        }
        public void Save()
        {
            if (Entities.Count > 0)
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = false;

                foreach (LoggableEntity entity in Entities)
                {
                    AuditLog log = new AuditLog();
                    log.Changes = entity.ToString();
                    log.EntityName = entity.Name;
                    log.Action = entity.Action;
                    log.EntityId = entity.Id();
                    log.AccountId = AccountId;

                    Context.Add(log);
                }

                Context.SaveChanges();
                Entities.Clear();
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
