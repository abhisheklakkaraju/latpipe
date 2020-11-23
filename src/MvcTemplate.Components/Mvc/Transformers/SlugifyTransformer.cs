
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace MvcTemplate.Components.Mvc
{
    public class SlugifyTransformer : IOutboundParameterTransformer
    {
        private static ConcurrentDictionary<Object, String> Values { get; }

        static SlugifyTransformer()
        {
            Values = new ConcurrentDictionary<Object, String>();
        }

        public String? TransformOutbound(Object? value)
        {
            String? key = value?.ToString();

            if (key == null)
                return null;

            if (Values.TryGetValue(key, out String? slug))
                return slug;

            return Values[key] = Regex.Replace(key, "(?<!^)((?=[A-Z][a-z]+)|(?<![A-Z])(?=[A-Z][A-Z]+))", "-");
        }
    }
}
