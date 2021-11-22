using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet_API.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNet_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            await CreateHostBuilder(args).Build().RunAsync();
            
            /*
             * Note : DO NOT use this method in the production code
             * Method: Run migration on startup
             */
            
            // using (var servicesScope = host.Services.CreateScope())
            // {
            //     var dbContext = servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //
            //     await dbContext.Database.MigrateAsync();
            // }
            //
            // await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}