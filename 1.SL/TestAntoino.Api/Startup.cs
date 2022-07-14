using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Commom.Impl;
using TestAntonio.Commom.Interfaces;
using TestAntonio.Infrastructure.Impl;
using TestAntonio.Infrastructure.Interfaces;
using TestAntonio.Services.Impl;
using TestAntonio.Services.Interfaces;

namespace TestAntoino.Api
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
            
            services.AddMemoryCache();
            services.AddControllers();

            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GClaims Test Swagger UI",
                    Description = "Antonio Santos Test",
                });                
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marvel API V1");
                //Remove Schema on Swagger UI
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
