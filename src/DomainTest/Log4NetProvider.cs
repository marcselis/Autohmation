using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace DomainTestCore
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly bool _skipDiagnosticLogs;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        public Log4NetProvider(bool skipDiagnosticLogs)
        {
            _skipDiagnosticLogs = skipDiagnosticLogs;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        private ILogger CreateLoggerImplementation(string name)
        {
            return new Log4NetLogger(name, _skipDiagnosticLogs);
        }
    }

}
