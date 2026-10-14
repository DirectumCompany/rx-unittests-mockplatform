namespace MockPlatform.Services.Tenants;

/// <summary>
/// Константы тенанта для mock-платформы.
/// </summary>
public static class MockTenantConstants
{
  /// <summary>
  /// Email администратора по умолчанию.
  /// </summary>
  public const string DefaultAdministratorEmail = "admin@test.local";

  /// <summary>
  /// БД тестового тенанта.
  /// </summary>
  public static string DatabaseName => "*_Tenant";

  /// <summary>
  /// Смещение по UTC тестового тенанта.
  /// </summary>
  public static readonly TimeSpan UtcOffset = TimeSpan.FromHours(0);

  /// <summary>
  /// Сервер гиперссылок для тестового тенанта.
  /// </summary>
  public static readonly Uri HyperlinkServer = new Uri("http://test.tenant.ru/Sungero");
}
