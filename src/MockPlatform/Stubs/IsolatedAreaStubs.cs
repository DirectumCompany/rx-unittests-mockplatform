extern alias IsolatedArea;
using IsolatedArea::Sungero.IsolatedArea.Client;
using MockPlatform.Core;
using Moq;
using IsolatedAreaProxy = MockPlatform.Services.IsolatedAreaProxy;

namespace MockPlatform.Stubs;

public static class IsolatedAreaStubs
{
  public static IDisposable Setup() => MockFunctionStubs.Setup<IsolatedAreaSupervisor>(
    nameof(IsolatedAreaSupervisor.GetIsolatedAreaHost),
    () =>
    {
      var hostMock = new Mock<IIsolatedAreaHost>();
      hostMock.Setup(m => m.Proxy).Returns(new IsolatedAreaProxy());
      return hostMock.Object;
    });
}
