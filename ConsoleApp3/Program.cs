using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using System.Net;
using System.Threading.Tasks;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleApp3
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureLogging(builder =>
                {
                    _ = builder.ClearProviders();
                    var @namespace = typeof(Program).Namespace;
                    var logger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                        .CreateLogger();

                    _ = builder.AddSerilog(logger);
                })
                .UseOrleans((siloBuilder) =>
                {
                    _ = siloBuilder
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "dev";
                        })
                        .Configure<EndpointOptions>(options =>
                        {
                            options.AdvertisedIPAddress = IPAddress.Parse("172.17.110.63");
                            options.GatewayPort = 10200;
                            options.SiloPort = 11200;
                        })
                        .UseAdoNetClustering(options =>
                        {
                            // Not requested due to bug before request
                            options.ConnectionString = "";
                            options.Invariant = "Microsoft.Data.SqlClient";
                        });
                })
                .Build();

            await host.RunAsync();
        }
    }
}
