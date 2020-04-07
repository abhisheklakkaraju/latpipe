using Microsoft.EntityFrameworkCore;
using MvcTemplate.Data.Core;
using System;

namespace MvcTemplate.Tests
{
    public class TestingContext : Context
    {
        private String DatabaseName { get; }

        public TestingContext()
        {
            DatabaseName = Guid.NewGuid().ToString();
        }
        public TestingContext(TestingContext context)
        {
            DatabaseName = context.DatabaseName;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Model.AddEntityType(typeof(TestModel));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase(DatabaseName);
            builder.UseLazyLoadingProxies();
        }
    }
}
