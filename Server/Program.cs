﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Threading.Tasks;
using WelcomeTo.Server.Jobs;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WelcomeTo.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var webHost = CreateHostBuilder(args).Build();

                // Schedule clean up - this needs to take place after the Build() method which initializes the Database.
                var jobScheduler = await JobScheduler.GetSchedulerAsync();
                await jobScheduler.ScheduleDailyJobAsync<CleanUpJob>("CleanUp", "DailyCleanUpTrigger");

                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "A fatal exception occurred causing the application to terminate.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}