using log4net.Config;
using Microsoft.Extensions.Logging;

namespace DomainTestCore
{
    internal static class Log4NetExtensions
    {
            public static ILoggingBuilder AddLog4Net(this ILoggingBuilder factory, bool skipDiagnosticLogs)
            {
                 return factory.AddProvider(new Log4NetProvider(skipDiagnosticLogs));
            }
    }

}
