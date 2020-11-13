using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data;
using MvcTemplate.Objects;

namespace MvcTemplate.Tests
{
    public static class TestingContext
    {
        private static DbContextOptions Options { get; }

        static TestingContext()
        {
            Options = new DbContextOptionsBuilder()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcTemplateTest;Trusted_Connection=True;MultipleActiveResultSets=True")
                .Options;

            using Context context = new(Options);

            context.Database.Migrate();
        }

        public static DbContext Create()
        {
            return new Context(Options);
        }

        public static DbContext Drop(this DbContext context)
        {
            context.RemoveRange(context.Set<RolePermission>());
            context.RemoveRange(context.Set<Permission>());
            context.RemoveRange(context.Set<AuditLog>());
            context.RemoveRange(context.Set<Account>());
            context.RemoveRange(context.Set<Role>());

            context.SaveChanges();

            return context;
        }
    }
}
