using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sentry;
using System;
using System.Diagnostics;
using System.Threading;

namespace CoreAPI
{
    public static class Program
    {
        private static PerformanceCounter cpuCounter;

        public static void Main(string[] args)
        {
            String sentryDsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
            if (sentryDsn == null)
            {
                Console.WriteLine("Sentry DSN not configured, will not capture errors to sentry.io");
                startApplication(args);
            } else
            {
                using (SentrySdk.Init(o =>
                {
                    o.Dsn = sentryDsn;
                    o.TracesSampleRate = 1.0;
                }))
                {
                    startApplication(args);
                }
            }
        }

        public static void startApplication(string[] args)
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            Thread cpuThread = new Thread(monitorCPU);
            cpuThread.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static void monitorCPU()
        {
            while (true)
            {
                float value = cpuCounter.NextValue();
                if (value >= 100)
                {
                    Environment.Exit(0x00000041); // Exit now CPU usage is too high
                }
                Thread.Sleep(5000); // Check every 5 seconds
            };
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://0.0.0.0:5555");
                });
    }
}
