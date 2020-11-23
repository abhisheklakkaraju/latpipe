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
using MvcTemplate.Data;
using MvcTemplate.Data.Migrations;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using NonFactors.Mvc.Grid;
using System;
using System.IO;

namespace MvcTemplate.Web
{
    public class Startup
    {
        private IConfiguration Config { get; }
        private IHostEnvironment Environment { get; }

        public Startup(IHostEnvironment host)
        {
            Environment = host;
            Config = new ConfigurationBuilder()
                .SetBasePath(host.ContentRootPath)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddJsonFile("configuration.json")
                .AddJsonFile($"configuration.{host.EnvironmentName.ToLower()}.json", optional: true)
                .Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            RegisterMiddleware(app);
            RegisterResources();

            SeedDatabase(app);
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
                .AddMvcOptions(options =>
                {
                    options.Filters.Add<LanguageFilter>();
                    options.Filters.Add<TransactionFilter>();
                    options.Filters.Add<AuthorizationFilter>();
                    ModelMessagesProvider.Set(options.ModelBindingMessageProvider);
                    options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider());
                    options.ModelBinderProviders.Insert(4, new TrimmingModelBinderProvider());
                })
                .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
                .AddViewOptions(options => options.ClientModelValidatorProviders.Add(new ClientValidatorProvider()));

            services.AddAuthentication("Cookies").AddCookie(authentication =>
            {
                authentication.Cookie.Name = Config["Cookies:Auth"];
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

                if (Environment.IsDevelopment())
                    builder.AddConsole();
                else
                    builder.AddProvider(new FileLoggerProvider(Config["Logging:File:Path"], Config.GetValue<Int64>("Logging:File:RollSize")));
            });
        }
        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<CookieTempDataProviderOptions>(provider => provider.Cookie.Name = Config["Cookies:TempData"]);
            services.Configure<RouteOptions>(options => options.ConstraintMap["slug"] = typeof(SlugifyTransformer));
            services.Configure<SessionOptions>(session => session.Cookie.Name = Config["Cookies:Session"]);
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.Configure<MailConfiguration>(Config.GetSection("Mail"));
            services.Configure<AntiforgeryOptions>(antiforgery =>
            {
                antiforgery.Cookie.Name = Config["Cookies:Antiforgery"];
                antiforgery.FormFieldName = "_Token_";
            });
        }
        private void ConfigureDependencies(IServiceCollection services)
        {
            services.AddSession();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<Configuration>();
            services.AddDbContext<DbContext, Context>(options => options.UseSqlServer(Config["Data:Connection"]));
            services.AddScoped<IUnitOfWork>(provider => new AuditedUnitOfWork(
                provider.GetRequiredService<DbContext>(),
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.Id()));

            services.AddSingleton<IHasher, BCrypter>();
            services.AddSingleton<IMailClient, SmtpMailClient>();
            services.AddSingleton<IValidationAttributeAdapterProvider, ValidationAdapterProvider>();
            services.AddSingleton<IAuthorization>(provider => new Authorization(typeof(AController).Assembly, provider));

            Language[] supported = Config.GetSection("Languages:Supported").Get<Language[]>();
            services.AddSingleton<ILanguages>(new Languages(Config["Languages:Default"], supported));

            services.AddSingleton<ISiteMap>(provider => new SiteMap(
                File.ReadAllText(Config["SiteMap:Path"]), provider.GetRequiredService<IAuthorization>()));

            services.AddScopedImplementations<IService>();
            services.AddScopedImplementations<IValidator>();
        }

        private void RegisterResources()
        {
            if (Config["Resources:Path"] is String path && Directory.Exists(path))
                foreach (String resource in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
                {
                    String type = Path.GetFileNameWithoutExtension(resource);
                    String language = Path.GetExtension(type).TrimStart('.');
                    type = Path.GetFileNameWithoutExtension(type);

                    Resource.Set(type).Override(language, File.ReadAllText(resource));
                }

            foreach (Type view in typeof(AView).Assembly.GetTypes())
            {
                Type type = view;

                while (typeof(AView).IsAssignableFrom(type.BaseType))
                {
                    Resource.Set(view.Name).Inherit(Resource.Set(type.BaseType!.Name));

                    type = type.BaseType;
                }
            }
        }
        private void RegisterMiddleware(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
                app.UseMiddleware<DeveloperExceptionPageMiddleware>();
            else
                app.UseMiddleware<ErrorResponseMiddleware>();

            app.UseMiddleware<SecureHeadersMiddleware>();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (response) => response.Context.Response.Headers["Cache-Control"] = "max-age=8640000"
            });

            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("MultiArea", "{language}/{area:slug:exists}/{controller:slug}/{action:slug=Index}/{id:int?}");
                endpoints.MapControllerRoute("DefaultArea", "{area:slug:exists}/{controller:slug}/{action:slug=Index}/{id:int?}");
                endpoints.MapControllerRoute("Multi", "{language}/{controller:slug}/{action:slug=Index}/{id:int?}");
                endpoints.MapControllerRoute("Default", "{controller:slug}/{action:slug=Index}/{id:int?}");
                endpoints.MapControllerRoute("Home", "{controller:slug=Home}/{action:slug=Index}");
            });
        }

        private void SeedDatabase(IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using Configuration configuration = scope.ServiceProvider.GetRequiredService<Configuration>();

            configuration.Migrate();
        }
    }
}
