using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class TransactionFilterTests
    {
        [Fact]
        public void OnResourceExecuted_CommitsTransaction()
        {
            ActionContext action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ResourceExecutedContext context = new ResourceExecutedContext(action, Array.Empty<IFilterMetadata>());
            DbContext currentContext = TestingContext.Create();
            DbContext testingContext = TestingContext.Create();
            Role model = ObjectsFactory.CreateRole(0);

            testingContext.Drop();

            TransactionFilter filter = new TransactionFilter(testingContext);

            testingContext.Add(model);
            testingContext.SaveChanges();

            Assert.Empty(currentContext.Set<Role>());
            Assert.Single(testingContext.Set<Role>());

            filter.OnResourceExecuted(context);

            Assert.Single(currentContext.Set<Role>());
            Assert.Single(testingContext.Set<Role>());
        }
    }
}
