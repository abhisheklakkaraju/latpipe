using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
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
            TestingContext currentContext = new TestingContext();
            TestingContext testingContext = new TestingContext();
            TestModel model = ObjectsFactory.CreateTestModel(0);

            testingContext.Drop();

            TransactionFilter filter = new TransactionFilter(testingContext);

            testingContext.Add(model);
            testingContext.SaveChanges();

            Assert.Empty(currentContext.Set<TestModel>());
            Assert.Single(testingContext.Set<TestModel>());

            filter.OnResourceExecuted(context);

            Assert.Single(currentContext.Set<TestModel>());
            Assert.Single(testingContext.Set<TestModel>());
        }
    }
}
