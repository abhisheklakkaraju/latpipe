using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MvcTemplate.Components.Logging
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider
    {
        private ILogger Logger { get; }

        public FileLoggerProvider(IConfiguration config)
        {
            String path = Path.Combine(config["Application:Path"], config["Logging:File:Path"]);
            Int64 rollSize = Int64.Parse(config["Logging:File:RollSize"]);

            Logger = new FileLogger(path, rollSize);
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
