using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MvcTemplate.Components.Extensions;
using System;
using System.IO;
using System.Text;

namespace MvcTemplate.Components.Logging
{
    public class FileLogger : ILogger
    {
        private Int64 RollSize { get; }
        private String LogPath { get; }
        private String LogDirectory { get; }
        private String RollingFileFormat { get; }
        private IHttpContextAccessor Accessor { get; }
        private static Object LogWriting { get; } = new Object();

        public FileLogger(String path, Int64 rollSize)
        {
            String file = Path.GetFileNameWithoutExtension(path);
            LogDirectory = Path.GetDirectoryName(path) ?? "";
            String extension = Path.GetExtension(path);
            Accessor = new HttpContextAccessor();

            RollingFileFormat = $"{file}-{{0:yyyyMMdd-HHmmss}}{extension}";
            RollSize = rollSize;
            LogPath = path;
        }

        public Boolean IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }
        public IDisposable? BeginScope<TState>(TState state)
        {
            return null;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, String> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            StringBuilder log = new();
            log.Append("Id         : ").Append(Accessor.HttpContext?.TraceIdentifier).Append(" [").Append(Accessor.HttpContext?.User.Id()).AppendLine("]");
            log.Append("Time       : ").AppendFormat("{0:yyyy-MM-dd HH:mm:ss.ffffff}", DateTime.Now).AppendLine();
            log.AppendFormat("{0,11}", logLevel).Append(": ").AppendLine(formatter(state, exception));

            if (exception != null)
                log.AppendLine("Stack trace:");

            while (exception != null)
            {
                log.Append("    ").Append(exception.GetType()).Append(": ").AppendLine(exception.Message);

                if (exception.StackTrace is String stackTrace)
                    foreach (String line in stackTrace.Split('\n'))
                        log.Append("     ").AppendLine(line.TrimEnd('\r'));

                exception = exception.InnerException;
            }

            log.AppendLine();

            lock (LogWriting)
            {
                Directory.CreateDirectory(LogDirectory);
                File.AppendAllText(LogPath, log.ToString());

                if (RollSize <= new FileInfo(LogPath).Length)
                    File.Move(LogPath, Path.Combine(LogDirectory, String.Format(RollingFileFormat, DateTime.Now)));
            }
        }
    }
}
