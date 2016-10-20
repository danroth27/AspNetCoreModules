using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Modules.Abstractions;
using Microsoft.AspNetCore.ViewTemplates;
using Microsoft.AspNetCore.Html;

namespace Module1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public static void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, ISharedServiceProvider sharedServices)
        {
            var templateManager = sharedServices.GetService<IViewTemplateManager>();
            templateManager?.AddTemplate(
                "Module1.Test",
                model => Task.FromResult<IHtmlContent>(new HtmlString("<b>View template from Module1</b>")));

            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }

        public static void ConfigureApplicationServices(IServiceCollection services)
        {
            // Share services with the app and other modules here
        }
    }
}
