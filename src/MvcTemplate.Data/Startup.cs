using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace MvcTemplate.Data
{
    public class Startup
    {
        private IConfiguration Config { get; }

        public Startup(IHostEnvironment env)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .SetBasePath(Directory.GetParent(env.ContentRootPath).FullName)
                .AddJsonFile("MvcTemplate.Web/configuration.json")
                .AddJsonFile($"MvcTemplate.Web/configuration.{env.EnvironmentName.ToLower()}.json", optional: true)
                .Build();
        }

        public void Configure()
        {
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Context>(options => options.UseSqlServer(Config["Data:Connection"]));
        }
    }
}
