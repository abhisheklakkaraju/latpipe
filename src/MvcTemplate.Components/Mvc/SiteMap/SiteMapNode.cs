using System;
using System.Collections.Generic;

namespace MvcTemplate.Components.Mvc
{
    public class SiteMapNode
    {
        public String? Url { get; set; }
        public String? Title { get; set; }
        public Boolean IsMenu { get; set; }
        public Boolean IsActive { get; set; }
        public String? IconClass { get; set; }

        public String? Area { get; set; }
        public String? Action { get; set; }
        public String? Controller { get; set; }
        public Dictionary<String, String> Route { get; set; }

        public SiteMapNode? Parent { get; set; }
        public IEnumerable<SiteMapNode> Children { get; set; }

        public SiteMapNode()
        {
            Children = Array.Empty<SiteMapNode>();
            Route = new Dictionary<String, String>();
        }
    }
}
