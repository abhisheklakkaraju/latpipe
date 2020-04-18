using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MvcTemplate.Web
{
    public static class Program
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseDefaultServiceProvider(options => options.ValidateOnBuild = true)
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseIIS()
                .Build()
                .Run();
        }
    }
}
