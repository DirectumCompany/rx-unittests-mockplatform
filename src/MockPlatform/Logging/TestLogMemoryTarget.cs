using System.Collections.Concurrent;
using NLog.Targets;
using NLog;

namespace MockPlatform.Logging;

[Target("TestLogMemory")]
public sealed class TestLogMemoryTarget : TargetWithLayout
{
  private readonly ConcurrentQueue<LogEventInfo> events = new();

  private readonly ConcurrentQueue<string> messages = new();

  public IReadOnlyList<LogEventInfo> GetLogs()
      => [.. events];

  public IReadOnlyList<string> GetMessages()
      => [.. messages];

  protected override void Write(LogEventInfo logEvent)
  {
    events.Enqueue(logEvent);
    messages.Enqueue(logEvent.Message);
    Console.WriteLine(Layout.Render(logEvent));
  }

  public void Clear()
  {
    while (events.TryDequeue(out _)) { }
    while (messages.TryDequeue(out _)) { }
  }
}
