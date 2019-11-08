using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Logging;
using MvcTemplate.Components.Mail;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Controllers;
using MvcTemplate.Data.Core;
using MvcTemplate.Data.Logging;
using MvcTemplate.Data.Migrations;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.IO;

namespace MvcTemplate.Web
{
    public class Startup
    {
        private IConfiguration Config { get; }

        public Startup(IHostEnvironment env)
        {
            Dictionary<String, String> config = new Dictionary<String, String>
            {
                { "Application:Path", env.ContentRootPath },
                { "Application:Env", env.EnvironmentName }
            };

            Config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddInMemoryCollection(config)
                .AddJsonFile("configuration.json")
                .AddJsonFile($"configuration.{env.EnvironmentName.ToLower()}.json", optional: true)
                .Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            RegisterMiddleware(app);
            RegisterResources();

            UpdateDatabase(app);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureMvc(services);
            ConfigureOptions(services);
            ConfigureDependencies(services);
        }

        private void ConfigureMvc(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddMvcOptions(options => options.Filters.Add<LanguageFilter>())
                .AddMvcOptions(options => options.Filters.Add<AuthorizationFilter>())
                .AddMvcOptions(options => ModelMessagesProvider.Set(options.ModelBindingMessageProvider))
                .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
                .AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider()))
                .AddViewOptions(options => options.ClientModelValidatorProviders.Add(new ClientValidatorProvider()))
                .AddMvcOptions(options => options.ModelBinderProviders.Insert(4, new TrimmingModelBinderProvider()));

            services.AddAuthentication("Cookies").AddCookie(authentication =>
            {
                authentication.Cookie.Name = Config["Cookies:Auth:Name"];
                authentication.Events = new AuthenticationEvents();
            });

            services.AddMvcGrid(filters =>
            {
                filters.BooleanFalseOptionText = () => Resource.ForString("No");
                filters.BooleanTrueOptionText = () => Resource.ForString("Yes");
            });

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Config.GetSection("Logging"));

                if (Config["Application:Env"] == Environments.Development)
                    builder.AddConsole();
                else
                    builder.AddProvider(new FileLoggerProvider(Config));
            });

        }
        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<CookieTempDataProviderOptions>(provider => provider.Cookie.Name = Config["Cookies:TempData:Name"]);
            services.Configure<SessionOptions>(session => session.Cookie.Name = Config["Cookies:Session:Name"]);
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.Configure<AntiforgeryOptions>(antiforgery =>
            {
                antiforgery.Cookie.Name = Config["Cookies:Antiforgery:Name"];
                antiforgery.FormFieldName = "_Token_";
            });
        }
        private void ConfigureDependencies(IServiceCollection services)
        {
            services.AddSession();
            services.AddSingleton(Config);

            services.AddTransient<Configuration>();
            services.AddTransient<DbContext, Context>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<Context>(options => options.UseSqlServer(Config["Data:Connection"]));

            services.AddTransient<IAuditLogger>(provider =>
                new AuditLogger(provider.GetService<DbContext>(),
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.Id()));

            services.AddSingleton<IHasher, BCrypter>();
            services.AddSingleton<IMailClient, SmtpMailClient>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IValidationAttributeAdapterProvider, ValidationAdapterProvider>();

            services.AddSingleton<IAuthorization>(provider =>
                new Authorization(typeof(BaseController).Assembly, provider));

            Language[] supported = Config.GetSection("Languages:Supported").Get<Language[]>();
            services.AddSingleton<ILanguages>(new Languages(Config["Languages:Default"], supported));

            String map = File.ReadAllText(Path.Combine(Config["Application:Path"], Config["SiteMap:Path"]));
            services.AddSingleton<ISiteMap>(provider => new SiteMap(map, provider.GetService<IAuthorization>()));

            services.AddTransientImplementations<IService>();
            services.AddTransientImplementations<IValidator>();
        }

        private void RegisterResources()
        {
            if (Config["Resources:Path"] is String path)
            {
                String directory = Path.Combine(Config["Application:Path"], path);
                if (Directory.Exists(directory))
                {
                    foreach (String resource in Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories))
                    {
                        String type = Path.GetFileNameWithoutExtension(resource);
                        String language = Path.GetExtension(type).TrimStart('.');
                        type = Path.GetFileNameWithoutExtension(type);

                        Resource.Set(type).Override(language, File.ReadAllText(resource));
                    }
                }
            }

            foreach (Type view in typeof(BaseView).Assembly.GetTypes())
            {
                Type type = view;

                while (typeof(BaseView).IsAssignableFrom(type.BaseType))
                {
                    Resource.Set(view.Name).Inherit(Resource.Set(type.BaseType!.Name));

                    type = type.BaseType;
                }
            }
        }
        private void RegisterMiddleware(IApplicationBuilder app)
        {
            if (Config["Application:Env"] == Environments.Development)
                app.UseMiddleware<DeveloperExceptionPageMiddleware>();
            else
                app.UseMiddleware<ErrorPagesMiddleware>();

            app.UseMiddleware<SecureHeadersMiddleware>();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (response) =>
                {
                    response.Context.Response.Headers["Cache-Control"] = "max-age=8640000";
                }
            });

            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("MultiArea", "{language}/{area}/{controller}/{action=Index}/{id:int?}");
                endpoints.MapControllerRoute("DefaultArea", "{area:exists}/{controller}/{action=Index}/{id:int?}");
                endpoints.MapControllerRoute("Multi", "{language}/{controller}/{action=Index}/{id:int?}");
                endpoints.MapControllerRoute("Default", "{controller}/{action=Index}/{id:int?}");
                endpoints.MapControllerRoute("Home", "{controller=Home}/{action=Index}");
            });
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using (Configuration configuration = app.ApplicationServices.GetService<Configuration>())
                configuration.UpdateDatabase();
        }
    }
}
