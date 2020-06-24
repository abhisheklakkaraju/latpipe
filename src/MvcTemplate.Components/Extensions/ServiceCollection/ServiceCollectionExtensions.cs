using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace MvcTemplate.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScopedImplementations<T>(this IServiceCollection services)
        {
            foreach (Type type in typeof(T).Assembly.GetTypes().Where(Implements<T>))
                if (type.GetInterface($"I{type.Name}") is Type typeInterface)
                    services.TryAddScoped(typeInterface, type);
        }
        private static Boolean Implements<T>(Type type)
        {
            return !type.IsAbstract && typeof(T).IsAssignableFrom(type);
        }
    }
}
