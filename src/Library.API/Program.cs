using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace Library.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Initializing main function");
                CreateDefaultWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error initing main function");
                throw;
            }
            finally
            {
                logger.Debug("Finally main function");
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();                
        }

        public static IWebHostBuilder CreateDefaultWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                                 .ConfigureAppConfiguration((host, config) =>
                                {
                                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                                    var env = host.HostingEnvironment;

                                    config.AddJsonFile("appsettings.json", false, true)
                                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                                          .AddEnvironmentVariables();

                                })   
                                .UseStartup<Startup>()
                                .ConfigureLogging((host, logBuilder) =>
                                {
                                    logBuilder.AddConsole();
                                    logBuilder.AddDebug();
                                    logBuilder.AddConfiguration(host.Configuration.GetSection("Logging"));
                                    logBuilder.SetMinimumLevel(LogLevel.Information);
                                })
                                .UseNLog();

            return builder;
        }

    }
}
