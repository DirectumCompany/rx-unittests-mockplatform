using NLog;
using NLog.Config;

namespace MockPlatform.Logging;

public static class LogManager
{
  private static readonly TestLogMemoryTarget target = new() { Name = "TestLogMemory" };

  public static void Configure()
  {
    var loggingConfiguration = new LoggingConfiguration();
    loggingConfiguration.AddTarget(target);
    loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
    NLog.LogManager.Configuration = loggingConfiguration;
  }

  public static IReadOnlyList<LogEventInfo> GetLogs() => target.GetLogs();

  public static IReadOnlyList<string> GetMessages() => target.GetMessages();

  public static IReadOnlyList<LogEventInfo> GetLogsByLevel(LogLevel level) => GetLogs().Where(l => l.Level == level).ToList();

  public static void Clear() => target.Clear();
}
