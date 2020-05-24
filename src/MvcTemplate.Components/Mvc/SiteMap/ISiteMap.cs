using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcTemplate.Components.Mvc
{
    public interface ISiteMap
    {
        SiteMapNode[] Tree { get; }

        SiteMapNode[] BreadcrumbFor(ViewContext context);
    }
}
