using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Commom.Impl;
using TestAntonio.Commom.Interfaces;
using TestAntonio.Infrastructure.Impl;
using TestAntonio.Infrastructure.Interfaces;
using TestAntonio.Services.Impl;
using TestAntonio.Services.Interfaces;

namespace TestAntonio.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMarvelSettings>(Configuration.GetSection("MarvelSettings").Get<MarvelSettings>());
            services.AddTransient<IComicsRepository, ComicsRepository>();
            services.AddTransient<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IMarvelServices, MarvelServices>();
            services.AddSingleton<IHttpRepository, HttpReposository>();
            services.AddHttpContextAccessor();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }        


            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
