using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModel;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                  .AddEnvironmentVariables();

            _config = builder.Build();
        }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           services.AddSingleton(_config);

          if(_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
                services.AddScoped<IMailServices, DeburgMailService>();
            }
            else
            {
                services.AddScoped<IMailServices, DeburgMailService>();
            }

          // identities 
            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
              //  config.LoginPath = "/Auth/Login";
               // config.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                //config.Cookies.ApplicationCookie.LoginPath = new PathString("/Auth/Login");
            })
            .AddEntityFrameworkStores<WorldContext>();
            // ends here 

            services.ConfigureApplicationCookie(config =>
            {
                // Cookie settings
                config.Cookie.HttpOnly = true;
                config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
               
                config.LoginPath = "/Auth/Login";
                config.AccessDeniedPath = "/Auth/Login/AccessDenied";
                config.SlidingExpiration = true;

                // for api athentication 
                config.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = async ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && 
                        ctx.Response.StatusCode ==200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        await Task.Yield();
                    }
                };
            });


            services.AddDbContext<WorldContext>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<GeoCoordsService>();
            services.AddTransient<WorldContextSeedData>();
            services.AddLogging();

            services.AddMvc(config =>
            {
                // for identies http 
                // to protect 
                if (_env.IsProduction())
                {
                    config.Filters.Add(new RequireHttpsAttribute());
                }
                
            })

                .AddJsonOptions(config =>
            {
                // json serializer
                config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env,WorldContextSeedData seeder,ILoggerFactory factory)

        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
            {
                factory.AddDebug(LogLevel.Error);   
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentity();

            Mapper.Initialize(Config =>
            {
                Config.CreateMap<TripViewModel, Trip>().ReverseMap();
                Config.CreateMap<StopViewModel, Stop>().ReverseMap();

            });

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" });

            }
                );

            seeder.EnsureSeedData().Wait();
        }
    }
}
