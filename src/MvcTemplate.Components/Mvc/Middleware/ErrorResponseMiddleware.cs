using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MvcTemplate.Components.Mvc
{
    public class ErrorResponseMiddleware
    {
        private ILanguages Languages { get; }
        private RequestDelegate Next { get; }
        private ILogger<ErrorResponseMiddleware> Logger { get; }

        public ErrorResponseMiddleware(RequestDelegate next, ILanguages languages, ILogger<ErrorResponseMiddleware> logger)
        {
            Next = next;
            Logger = logger;
            Languages = languages;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);

                if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status404NotFound)
                    await View(context, "/home/not-found");
            }
            catch (Exception exception)
            {
                try
                {
                    Logger.LogError(exception, "An unhandled exception has occurred while executing the request.");
                }
                catch
                {
                }

                await View(context, "/home/error");
            }
        }

        private async Task View(HttpContext context, String path)
        {
            String originalPath = context.Request.Path;
            Match abbreviation = Regex.Match(originalPath, "^/(\\w{2})(/|$)");

            try
            {
                if (abbreviation.Success)
                {
                    Language language = Languages[abbreviation.Groups[1].Value];

                    if (language != Languages.Default)
                        context.Request.Path = $"/{language.Abbreviation}{path}";
                }
                else
                {
                    context.Request.Path = path;
                }

                context.Features.Get<IRouteValuesFeature>()?.RouteValues?.Clear();
                context.Request.Method = "GET";
                context.SetEndpoint(null);

                await Next(context);
            }
            finally
            {
                context.Request.Path = originalPath;
            }
        }
    }
}
