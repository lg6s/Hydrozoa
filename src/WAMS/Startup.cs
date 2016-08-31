using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WAMS.Services.GPIOAccess;
using WAMS.Services.PlanManagement;
using WAMS.Services;
using System.Web.Http;

namespace WAMS
{
    public class Startup
    {
        private static bool dev_env { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            dev_env = env.IsDevelopment();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // framework services
            services.AddMvc();

            // own services
            services.AddTransient<IValve, Valve>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IValve Valve)
        {
            // Logging Setup
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // NLog Setup
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            LogManagement.Setup(loggerFactory, env);

            // Plan* Setup
            PlanContainer.Setup(loggerFactory, Valve);
            PlanWorker.Setup(loggerFactory, Valve);

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            } else { app.UseExceptionHandler("/Primary/Error"); }

            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Primary}/{action=ClientInterface}");
                routes.MapWebApiRoute(
                    name: "defaultAPI",
                    template: "api/{controller}/{action}/{NewPlan}",
                    defaults: new { NewPlan = RouteParameter.Optional });
            });
        }
    }
}
