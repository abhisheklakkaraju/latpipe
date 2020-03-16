using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MvcTemplate.Components.Extensions
{
    public static class MvcGridExtensions
    {
        public static IGridColumn<T, IHtmlContent> AddAction<T>(this IGridColumnsOf<T> columns, String action, String iconClass) where T : class
        {
            if (!IsAuthorizedFor(columns.Grid.ViewContext!, action))
                return new GridColumn<T, IHtmlContent>(columns.Grid, model => HtmlString.Empty);

            IUrlHelperFactory? factory = columns.Grid.ViewContext?.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            IUrlHelper? url = factory?.GetUrlHelper(columns.Grid.ViewContext);

            return columns
                .Add(model => GenerateLink(model, url, action, iconClass))
                .Css($"action-cell {action.ToLower()}");
        }

        public static IGridColumn<T, DateTime> AddDate<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T, DateTime?> AddDate<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:d}");
        }
        public static IGridColumn<T, Boolean> AddBoolean<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean>> expression)
        {
            Func<T, Boolean> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .RenderedAs(model => valueFor(model) ? Resource.ForString("Yes") : Resource.ForString("No"));
        }
        public static IGridColumn<T, Boolean?> AddBoolean<T>(this IGridColumnsOf<T> columns, Expression<Func<T, Boolean?>> expression)
        {
            Func<T, Boolean?> valueFor = expression.Compile();

            return columns
                .AddProperty(expression)
                .RenderedAs(model =>
                    valueFor(model) != null
                        ? valueFor(model) == true
                            ? Resource.ForString("Yes")
                            : Resource.ForString("No")
                        : "");
        }
        public static IGridColumn<T, DateTime> AddDateTime<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T, DateTime?> AddDateTime<T>(this IGridColumnsOf<T> columns, Expression<Func<T, DateTime?>> expression)
        {
            return columns.AddProperty(expression).Formatted("{0:g}");
        }
        public static IGridColumn<T, TProperty> AddProperty<T, TProperty>(this IGridColumnsOf<T> columns, Expression<Func<T, TProperty>> expression)
        {
            return columns
                .Add(expression)
                .Named(NameFor(expression))
                .Css(CssClassFor<TProperty>())
                .Titled(Resource.ForProperty(expression));
        }

        public static IHtmlGrid<T> ApplyDefaults<T>(this IHtmlGrid<T> grid)
        {
            return grid
                .Pageable(pager => { pager.RowsPerPage = 16; })
                .Empty(Resource.ForString("NoDataFound"))
                .AppendCss("table-hover")
                .Filterable()
                .Sortable();
        }

        private static IHtmlContent GenerateLink<T>(T model, IUrlHelper? url, String action, String iconClass)
        {
            TagBuilder link = new TagBuilder("a");
            link.Attributes["href"] = url?.Action(action, RouteFor(model));
            link.Attributes["title"] = Resource.ForAction(action);
            link.AddCssClass(iconClass);

            return link;
        }
        private static Boolean IsAuthorizedFor(ActionContext context, String action)
        {
            IAuthorization authorization = context.HttpContext.RequestServices.GetService<IAuthorization>();
            if (authorization == null)
                return true;

            Int32? account = context.HttpContext.User.Id();
            String? area = context.RouteData.Values["area"] as String;
            String? controller = context.RouteData.Values["controller"] as String;

            return authorization.IsGrantedFor(account, area, controller, action);
        }
        private static IDictionary<String, Object?> RouteFor<T>(T model)
        {
            PropertyInfo id = typeof(T).GetProperty("Id") ?? throw new Exception($"{typeof(T).Name} type does not have an id.");

            return new Dictionary<String, Object?> { ["id"] = id.GetValue(model) };
        }
        private static String NameFor(LambdaExpression expression)
        {
            String text = expression.Body is MemberExpression member ? member.ToString() : "";
            text = text.IndexOf('.') > 0 ? text.Substring(text.IndexOf('.') + 1) : text;
            text = text.Replace("_", "-");

            return String.Join("-", Regex.Split(text, "(?<=[a-z])(?=[A-Z])|(?<!^)(?=[A-Z][a-z])")).ToLower();
        }
        private static String CssClassFor<TProperty>()
        {
            Type type = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
            if (type.IsEnum)
                return "text-left";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "text-right";
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return "text-center";
                default:
                    return "text-left";
            }
        }
    }
}
