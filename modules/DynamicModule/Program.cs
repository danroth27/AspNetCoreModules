using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicModule
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(new ModuleInstanceIdProvider("DynamicModule"));
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
