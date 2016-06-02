//TODO: verify that services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); is the right way to add HttpContext to the dependency container
// an alternate approach to grabbing the request object is to us ViewContextAttribute but this looks like it would only work in a TagHelper
// see also http://stackoverflow.com/questions/37371264/asp-net-core-rc2-invalidoperationexception-unable-to-resolve-service-for-type and http://www.jerriepelser.com/blog/accessing-request-object-inside-tag-helper-aspnet-core and https://github.com/aspnet/Mvc/issues/4744

using AspNetCoreFileManager.FileManager;
using AspNetCoreFileManager.ThumbnailProcessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreFileManager
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSession();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IFileManagerService, FileManagerService>();
            //services.AddTransient<UiNotificationMessageBus>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseThumbnailProcessor();

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}