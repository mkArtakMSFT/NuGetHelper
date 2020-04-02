using NuGet.Common;
using System;
using System.Threading.Tasks;

namespace NuGetPackageManager
{
    internal class CompositeLogger : ILogger
    {
        public void Log(LogLevel level, string data)
        {
            Console.WriteLine($"{level}:: {data}");
        }

        public void Log(ILogMessage message)
        {
            Console.WriteLine($"{message.Level}:: {message.Message}");
        }

        public Task LogAsync(LogLevel level, string data)
        {
            return Task.Run(() => Log(level, data));
        }

        public Task LogAsync(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string data)
        {
            Log(LogLevel.Debug, data);
        }

        public void LogError(string data)
        {
            Log(LogLevel.Error, data);
        }

        public void LogInformation(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogInformationSummary(string data)
        {
            Log(LogLevel.Information, data);
        }

        public void LogMinimal(string data)
        {
            Log(LogLevel.Minimal, data);
        }

        public void LogVerbose(string data)
        {
            Log(LogLevel.Verbose, data);
        }

        public void LogWarning(string data)
        {
            Log(LogLevel.Warning, data);
        }
    }
}
