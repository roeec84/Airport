using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using Common.API;
using Common.Models;
using DAL.Context;
using DAL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Hubs;

namespace Server
{
    public class Startup
    {
        private readonly string policy = "AirportPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR((opt) => {
                opt.EnableDetailedErrors = false;
            });
            services.AddCors(options =>
            {
                options.AddPolicy(policy, policyBuilder =>
                    policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
            });
            services.AddDbContext<AirportDbContext>((opt) =>
            {
                opt.UseLazyLoadingProxies()
                   .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Singleton);
            services.AddSingleton<IRepository<Flight>, Repository<Flight>>();
            services.AddSingleton<IRepository<Airplane>, Repository<Airplane>>();
            services.AddSingleton<IRepository<Station>, Repository<Station>>();
            services.AddSingleton<IRepository<History>, Repository<History>>();
            services.AddSingleton<IFlightBL, FlightBL>();
            services.AddSingleton<IAirplaneBL, AirplaneBL>();
            services.AddSingleton<IStationBL, StationBL>();
            services.AddSingleton<IHistoryBL, HistoryBL>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AirportHub>("/Airport");
            });
        }
    }
}
