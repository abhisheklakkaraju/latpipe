using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Data;
using MvcTemplate.Objects;
using System;
using System.Linq;

namespace MvcTemplate.Tests
{
    public class TestingContext : Context
    {
        static TestingContext()
        {
            using (TestingContext context = new TestingContext())
                context.Database.Migrate();
        }
        public TestingContext(TestingContext context)
        {
        }
        public TestingContext()
        {
        }

        public TestingContext Drop()
        {
            RemoveRange(Set<RolePermission>());
            RemoveRange(Set<Permission>());
            RemoveRange(Set<TestModel>());
            RemoveRange(Set<AuditLog>());
            RemoveRange(Set<Account>());
            RemoveRange(Set<Role>());

            SaveChanges();

            return this;
        }

        public override Int32 SaveChanges()
        {
            Int32 affected = base.SaveChanges();

            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
                entry.State = EntityState.Detached;

            return affected;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Model.AddEntityType(typeof(TestModel));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcTemplateTest;Trusted_Connection=True;MultipleActiveResultSets=True");
        }
    }
}
