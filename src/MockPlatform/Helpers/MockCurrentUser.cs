using Sungero.Common;
using Sungero.CoreEntities;

namespace MockPlatform;

/// <summary>
/// Хелпер для установки текущего пользователя в unit-тестах.
/// </summary>
public static class MockCurrentUser
{
  /// <summary>
  /// Текущий пользователь, установленный в тесте.
  /// </summary>
  internal static IUser Current { get; private set; }

  /// <summary>
  /// Установить текущего пользователя.
  /// </summary>
  /// <param name="user">Пользователь.</param>
  /// <returns>Объект для сброса текущего пользователя после выхода из using-блока.</returns>
  public static IDisposable Set(IUser user)
  {
    Current = user;
    return new DisposableObject(() => Current = null);
  }
}
