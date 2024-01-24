using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TeskeHomeAssistant;

public static class CustomLoggingProvider
{
    /// <summary>
    ///     Adds standard Serilog logging configuration, from appsettings, as per:
    ///     https://github.com/datalust/dotnet6-serilog-example
    /// </summary>
    /// <param name="builder"></param>
    public static IHostBuilder UseCustomLogging(this IHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();       

        return builder.UseSerilog(logger);
    }
}
