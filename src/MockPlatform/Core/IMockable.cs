using Moq;
using Sungero.Common;

namespace MockPlatform.Core;

public interface IMockable<T> where T : class
{
  public static T? Mock { get; private set; }
  public static IDisposable WithMock(T mock)
  {
    Mock = mock;
    return new DisposableObject(() => Mock = null);
  }

  public static IDisposable WithMock(Action<Mock<T>> initializer)
  {
    var mock = new Mock<T>();
    initializer(mock);
    Mock = mock.Object;
    return new DisposableObject(() => Mock = null);
  }
}
