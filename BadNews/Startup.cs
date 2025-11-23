using BadNews.Elevation;
using BadNews.Hubs;
using BadNews.ModelBuilders.News;
using BadNews.Repositories.Comments;
using BadNews.Repositories.News;
using BadNews.Repositories.Weather;
using BadNews.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace BadNews
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        // В конструкторе уже доступна информация об окружении и конфигурация
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        // В этом методе добавляются сервисы в DI-контейнер
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddServerSideBlazor();
            services.AddSingleton<INewsRepository, NewsIndexedRepository>();
            services.AddSingleton<INewsModelBuilder, NewsModelBuilder>();
            services.AddSingleton<IValidationAttributeAdapterProvider, StopWordsAttributeAdapterProvider>();
            services.AddSingleton<IWeatherForecastRepository, WeatherForecastRepository>();
            services.AddSingleton<CommentsRepository>();
            services.Configure<OpenWeatherOptions>(configuration.GetSection("OpenWeather"));
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            services.AddMemoryCache();
            var mvcBuilder = services.AddControllersWithViews();
            if (env.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();
        }

        // В этом методе конфигурируется последовательность обработки HTTP-запроса
        public void Configure(IApplicationBuilder app)
        {
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Errors/Exception");

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

            app.UseMiddleware<ElevationMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CommentsHub>("/commentsHub");
                endpoints.MapBlazorHub();
                endpoints.MapControllerRoute("status-code", "StatusCode/{code?}", new
                {
                    controller = "Errors",
                    action = "StatusCode"
                });
                endpoints.MapControllerRoute("default", "{controller=News}/{action=Index}/{id?}");
            });
            app.MapWhen(context => context.Request.IsElevated(), branchApp =>
            {
                branchApp.UseDirectoryBrowser("/files");
            });

            // Остальные запросы — 404 Not Found
        }
    }
}
