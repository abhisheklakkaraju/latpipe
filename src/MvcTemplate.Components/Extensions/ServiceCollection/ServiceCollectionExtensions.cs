using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MvcTemplate.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTransientImplementations<T>(this IServiceCollection services)
        {
            foreach (Type type in typeof(T).Assembly.GetTypes().Where(Implements<T>))
                services.AddTransient(type.GetInterface($"I{type.Name}"), type);
        }
        private static Boolean Implements<T>(Type type)
        {
            return !type.IsAbstract && typeof(T).IsAssignableFrom(type);
        }
    }
}
