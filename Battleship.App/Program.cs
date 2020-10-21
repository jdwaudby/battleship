using Battleship.Library.Services.Implementations;
using Battleship.Library.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Battleship.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);

            IServiceProvider provider = services.BuildServiceProvider();

            provider.GetService<App>().Start();            
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IGridService, GridService>();
            services.AddTransient<IShipService, ShipService>();

            services.AddSingleton<App>();
        }
    }
}