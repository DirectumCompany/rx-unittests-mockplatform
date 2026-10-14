using CommonLibrary;
using Moq;
using Sungero.Domain.Shared;

namespace MockPlatform.Core;

public class MockInstanceInitializer
{
  private static readonly Dictionary<Type, Delegate> typeInitializers = [];

  public static IDisposable AddTypeInitializer<T>(Action<Mock<T>> initializer)
    where T : class, IEntity
  {
    Type type = typeof(T);
    typeInitializers.Add(type, initializer);
    return new DisposableObject(() => typeInitializers.Remove(type));
  }

  public static void Initialize<T>(Mock<T> instance)
    where T : class, IEntity
  {
    if (!typeInitializers.TryGetValue(typeof(T), out var initializer))
      return;

    ((Action<Mock<T>>)initializer)(instance);
  }

  public static void Clear()
  {
    typeInitializers.Clear();
  }
}