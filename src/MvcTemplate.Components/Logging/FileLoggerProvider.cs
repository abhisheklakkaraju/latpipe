using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MvcTemplate.Components.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private ILogger Logger { get; }

        public FileLoggerProvider(IConfiguration config)
        {
            String path = Path.Combine(config["Application:Path"], config["Logging:File:Path"]);
            LogLevel logLevel = Enum.Parse<LogLevel>(config["Logging:File:LogLevel:Default"]);
            Int64 rollSize = Int64.Parse(config["Logging:File:RollSize"]);

            Logger = new FileLogger(path, logLevel, rollSize);
        }

        public ILogger CreateLogger(String categoryName)
        {
            return Logger;
        }

        public void Dispose()
        {
        }
    }
}
