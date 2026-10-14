using Sungero.Domain.Shared;

namespace MockPlatform.Repositories;

public static class MockRepositories
{
  /// <summary>
  /// Общий список сущностей.
  /// </summary>
  internal static List<IEntity> Entities { get; } = [];

  /// <summary>
  /// Очистить общий список сущностей.
  /// </summary>
  public static void ClearAll() => Entities.Clear();
}
