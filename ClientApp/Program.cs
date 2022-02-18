using LoggingApp;
using Serilog;
using Serilog.Formatting.Json;
using System;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Verbose()
                 .WriteTo.File(new JsonFormatter(), @"D:\Code\choreography-demo\output.log")
                 .CreateLogger();

            Log.Information("order created {order}", new { Id = "1", Text = "test" });
            Log.Information("order created {order}", new { Id = "2", Text = "test" });
            Log.Information("order created {order}", new { Id = "3", Text = "test" });
        }
    }
}
