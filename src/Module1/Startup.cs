using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.ViewTemplates;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Modules.Mvc;

namespace Module1
{
    public class Startup
    {
        public void ConfigureSharedServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddViewTemplates();
            // Add framework services.
            var builder = services.AddMvcWithSharedRoutes();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ShareTemplate("Module1.Test", model => Task.FromResult<IHtmlContent>(new HtmlString("<b>View template from Module1</b>")));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithSharedRoutes(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
