using Client.Services;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public partial class App : Application
    {
        public static IServiceProvider Provider;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<FlightsViewModel>();
            services.AddScoped<AirportViewModel>();
            services.AddScoped<IHubService, HubService>();
            Provider = services.BuildServiceProvider();
        }
    }
}
