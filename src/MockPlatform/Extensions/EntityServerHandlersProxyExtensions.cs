using Sungero.Domain.Shared;

namespace MockPlatform.Extensions;

public static class EntityServerHandlersProxyExtensions
{
  public static T WithEntity<T>(this T handler, IEntity entity)
    where T : EntityBaseRuntimeProxy
  {
    typeof(EntityBaseRuntimeProxy)
      .GetProperty(nameof(EntityBaseRuntimeProxy.Entity))!
      .SetValue(handler, entity);

    return handler;
  }
}