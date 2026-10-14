using MockPlatform.Core;
using Sungero.Common;
using Sungero.Core;

namespace MockPlatform.Stubs;

public static class CacheStubs
{
  public static IDisposable SetupCache()
  {
    return SetupCache(new Dictionary<string, object>());
  }

  public static IDisposable SetupCache(Dictionary<string, object> cache)
  {
    var storage = new Services.Storage(cache);
    var cacheStub = MockFunctionStubs.Setup(typeof(Cache), "get_InternalStorage", () => storage);

    return new DisposableObject(() => cacheStub.Dispose());
  }
}
