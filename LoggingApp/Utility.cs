using Serilog;
using Serilog.Formatting.Json;
using System;

namespace LoggingApp
{
    public static class Utility
    {
       
        public static void WriteMessage(string message,object obj)
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Verbose()
                 .WriteTo.File(new JsonFormatter(), @"D:\Code\choreography-demo\output.log")
                 .CreateLogger();

            Log.Debug(message, obj);
        }
    }
}
