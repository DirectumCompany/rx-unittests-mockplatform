using MockPlatform.Core;
using MockPlatform.Logging;
using MockPlatform.Repositories;
using MockPlatform.Services;
using MockPlatform.Services.Dialogs;

namespace MockPlatform;

/// <summary>
/// Хелпер для инициализации mock-окружения перед unit-тестом.
/// </summary>
public static class MockTestEnvironment
{
  /// <summary>
  /// Сбросить mock-окружение.
  /// </summary>
  public static void Reset()
  {
    MockRepositories.ClearAll();
    MockFunctionStubs.ClearAll();
    LogManager.Clear();
    HyperlinksImplementer.ClearOpenedUri();
    MockCurrentUser.Set(null);
    TaskDialog.Clear();
    MockInstanceInitializer.Clear();
  }
}
