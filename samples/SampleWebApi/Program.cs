using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.Logging;

namespace SampleWebApi
{
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        public static void Main(string[] args)
        {
            AppLog.InitLogger();
            try
            {
                AppLog.Logger.LogInformation("Application configure...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                AppLog.Logger.LogCritical(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog2(logger => logger.LogInformation("Application initializing..."))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
