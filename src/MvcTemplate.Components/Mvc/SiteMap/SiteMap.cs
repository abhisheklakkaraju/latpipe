using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MvcTemplate.Components.Mvc
{
    public class SiteMap : ISiteMap
    {
        public SiteMapNode[] Tree { get; }
        private Dictionary<String, SiteMapNode[]> Lookup { get; }

        public SiteMap(String map)
        {
            XElement sitemap = XElement.Parse(map);
            Tree = ToMenuTree(Parse(sitemap, null));
            Lookup = ToLookup(Flatten(Parse(sitemap, null)));
        }

        public SiteMapNode[] BreadcrumbFor(ViewContext context)
        {
            String? area = context.RouteData.Values["area"] as String;
            String? action = context.RouteData.Values["action"] as String;
            String? controller = context.RouteData.Values["controller"] as String;

            return Lookup.TryGetValue($"{area}/{controller}/{action}", out SiteMapNode[]? breadcrumb) ? breadcrumb : Array.Empty<SiteMapNode>();
        }

        private Dictionary<String, SiteMapNode[]> ToLookup(List<SiteMapNode> nodes)
        {
            return new Dictionary<String, SiteMapNode[]>(nodes.Select(ToBreadcrumb), StringComparer.OrdinalIgnoreCase);
        }
        private KeyValuePair<String, SiteMapNode[]> ToBreadcrumb(SiteMapNode node)
        {
            List<SiteMapNode> breadcrumb = new List<SiteMapNode>();
            SiteMapNode? current = node;

            while (current != null)
            {
                breadcrumb.Insert(0, current);

                current = current.Parent;
            }

            return new KeyValuePair<String, SiteMapNode[]>($"{node.Area}/{node.Controller}/{node.Action}", breadcrumb.ToArray());
        }
        private List<SiteMapNode> Flatten(IEnumerable<SiteMapNode> branches)
        {
            List<SiteMapNode> list = new List<SiteMapNode>();

            foreach (SiteMapNode branch in branches)
            {
                list.Add(branch);
                list.AddRange(Flatten(branch.Children));
            }

            return list;
        }
        private SiteMapNode[] Parse(XContainer root, SiteMapNode? parent)
        {
            List<SiteMapNode> nodes = new List<SiteMapNode>();

            foreach (XElement element in root.Elements("siteMapNode"))
            {
                SiteMapNode node = new SiteMapNode();
                node.Action = (String)element.Attribute("action");
                node.Area = (String)element.Attribute("area") ?? parent?.Area;
                node.Controller = (String)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null);

                node.Path = $"{node.Area}/{node.Controller}/{node.Action}";
                node.IsMenu = (Boolean?)element.Attribute("menu") == true;
                node.IconClass = (String)element.Attribute("icon");
                node.Children = Parse(element, node);
                node.Route = ParseRoute(element);
                node.Parent = parent;

                nodes.Add(node);
            }

            return nodes.ToArray();
        }
        private Dictionary<String, String> ParseRoute(XElement element)
        {
            return element
                .Attributes()
                .Where(attribute => attribute.Name.LocalName.StartsWith("route-"))
                .ToDictionary(attribute => attribute.Name.LocalName.Substring(6), attribute => attribute.Value);
        }
        private SiteMapNode[] ToMenuTree(SiteMapNode[] nodes)
        {
            List<SiteMapNode> menu = new List<SiteMapNode>();

            foreach (SiteMapNode node in nodes)
            {
                node.Children = ToMenuTree(node.Children);

                if (node.IsMenu)
                    menu.Add(node);
                else
                    menu.AddRange(node.Children);
            }

            return menu.ToArray();
        }
    }
}
