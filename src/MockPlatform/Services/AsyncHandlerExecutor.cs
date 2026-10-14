using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;

namespace MockPlatform.Services;

public class AsyncHandlerExecutor : IAsyncHandlerExecutor
{
  public void ExecuteAsync(
    IEnumerable<IAsyncHandler> handlers, string startedNotification, string completedNotification, string errorNotification, IUser user, TimeSpan? timeout)
  {
  }
}
