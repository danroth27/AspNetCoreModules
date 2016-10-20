using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.Module.Data;
using Microsoft.AspNetCore.Identity.Module.Models;
using Microsoft.AspNetCore.Identity.Module.Services;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Microsoft.AspNetCore.Identity.Module
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var mvcBuilder = services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISharedServiceProvider sharedServices)
        {

            var templateManager = sharedServices.GetService<ITemplateManager>();
            templateManager?.AddTemplate(
                "Microsoft.AspNetCore.Identity.Module.Title",
                model => Task.FromResult<IHtmlContent>(new HtmlString("<b>Template from Microsoft.AspNetCore.Identity.Module</b>")));

            var htmlHelper = app.ApplicationServices.GetService<IHtmlHelper>();
            (htmlHelper as IViewContextAware)?.Contextualize(new ViewContext());
            templateManager?.AddTemplate("Microsoft.AspNetCore.Identity.Module.Login", model =>
            {
                var partialViewName = Path.Combine("Views", "Shared", "_LoginPartial.cshtml");
                return htmlHelper.PartialAsync(partialViewName);
            });
                
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("Application name: {applicationName}", env.ApplicationName);

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
