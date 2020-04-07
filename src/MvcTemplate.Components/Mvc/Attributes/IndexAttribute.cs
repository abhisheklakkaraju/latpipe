using System;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public Boolean IsUnique { get; set; }
    }
}
