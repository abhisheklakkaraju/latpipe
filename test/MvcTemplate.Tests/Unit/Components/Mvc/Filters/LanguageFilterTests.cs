using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class LanguageFilterTests
    {
        [Fact]
        public void OnResourceExecuting_SetsCurrentLanguage()
        {
            ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ResourceExecutingContext context = new(action, Array.Empty<IFilterMetadata>(), Array.Empty<IValueProviderFactory>());
            Languages languages = new("en", new[] { new Language { Abbreviation = "en", Culture = new CultureInfo("en-gb") }, new Language { Abbreviation = "us", Culture = new CultureInfo("en-us") } });

            context.RouteData.Values["language"] = "us";
            new LanguageFilter(languages).OnResourceExecuting(context);

            Language expected = languages["us"];
            Language actual = languages.Current;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnResourceExecuting_SetsDefaultLanguage()
        {
            ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            Languages languages = new("en", new[] { new Language { Abbreviation = "en", Culture = new CultureInfo("en-gb") } });
            ResourceExecutingContext context = new(action, Array.Empty<IFilterMetadata>(), Array.Empty<IValueProviderFactory>());

            new LanguageFilter(languages).OnResourceExecuting(context);

            Language expected = languages.Default;
            Language actual = languages.Current;

            Assert.Equal(expected, actual);
        }
    }
}
