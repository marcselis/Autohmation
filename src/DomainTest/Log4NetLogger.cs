﻿using System;
using System.Globalization;
using System.Reflection;
using log4net;
using log4net.Repository;
using Microsoft.Extensions.Logging;

namespace DomainTest
{
  public class Log4NetLogger : ILogger
  {
    private readonly string _name;
    private readonly ILog _log;
    private readonly bool _skipDiagnosticLogs;
    private readonly static ILoggerRepository _loggerRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

    public Log4NetLogger(string name, bool skipDiagnosticLogs)
    {
      _name = name;
      _log = LogManager.GetLogger(_loggerRepository.Name, name);
      _skipDiagnosticLogs = skipDiagnosticLogs;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return new NoDisposable();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      switch (logLevel)
      {
        case LogLevel.Critical:
          return _log.IsFatalEnabled;
        case LogLevel.Debug:
        case LogLevel.Trace:
          return _log.IsDebugEnabled && AllowDiagnostics();
        case LogLevel.Error:
          return _log.IsErrorEnabled;
        case LogLevel.Information:
          return _log.IsInfoEnabled && AllowDiagnostics();
        case LogLevel.Warning:
          return _log.IsWarnEnabled;
        default:
          throw new ArgumentOutOfRangeException(nameof(logLevel));
      }
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      if (!IsEnabled(logLevel))
      {
        return;
      }

      if (formatter == null)
      {
        throw new ArgumentNullException(nameof(formatter));
      }

      var message = $"{formatter(state, exception)} {exception}";

      if (string.IsNullOrEmpty(message))
      {
        return;
      }

      switch (logLevel)
      {
        case LogLevel.Critical:
          _log.Fatal(message);
          break;
        case LogLevel.Debug:
        case LogLevel.Trace:
          _log.Debug(message);
          break;
        case LogLevel.Error:
          _log.Error(message);
          break;
        case LogLevel.Information:
          _log.Info(message);
          break;
        case LogLevel.Warning:
          _log.Warn(message);
          break;
        default:
          _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
          _log.Info(message, exception);
          break;
      }
    }

    private bool AllowDiagnostics()
    {
      if (!_skipDiagnosticLogs)
      {
        return true;
      }

      return !(_name.ToLower(CultureInfo.CurrentCulture).StartsWith("microsoft", StringComparison.OrdinalIgnoreCase)
          || _name == "IdentityServer4.AccessTokenValidation.Infrastructure.NopAuthenticationMiddleware");
    }

    private class NoDisposable : IDisposable
    {
      public void Dispose()
      {
        GC.SuppressFinalize(this);
      }
    }
  }

}
