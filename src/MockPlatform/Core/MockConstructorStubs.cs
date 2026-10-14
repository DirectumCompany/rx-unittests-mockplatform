using HarmonyLib;
using System.Collections.Concurrent;
using System.Reflection;

namespace MockPlatform.Core;

public class MockConstructorStubs : IDisposable
{
  private static readonly ConcurrentDictionary<string, MockConstructorStubs> stubs = [];

  private readonly ConstructorInfo constructor;
  private readonly MethodInfo prefix;

  private MockConstructorStubs(ConstructorInfo constructor)
  {
    this.constructor = constructor;
    this.prefix = AccessTools.Method(typeof(MockConstructorStubs), nameof(Skip));
    stubs.AddOrUpdate(constructor.Name, (_) => this, (_, oldVal) => oldVal);
    Patcher.Instance.Patch(constructor, prefix: new HarmonyMethod(this.prefix));
  }

  public static MockConstructorStubs Skip<TClass>(Type[] parameters)
  {
    var type = typeof(TClass);
    var constructor = type.GetConstructor(parameters) ??
      throw new MissingMethodException($"Constructor not found for {type} with parameters {parameters}");

    return new MockConstructorStubs(constructor);
  }

  private static bool Skip() => false;

  public void Dispose()
  {
    stubs.TryRemove(constructor.Name, out var _);
    Patcher.Instance.Unpatch(constructor, prefix);
  }
}
