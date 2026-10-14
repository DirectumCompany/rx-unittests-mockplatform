using Sungero.Tenants;

namespace MockPlatform.Services.Tenants;

/// <summary>
/// Источник, который предоставляет тестовый тенант.
/// </summary>
internal class MockTenantTestSource : ITenantSource
{
  public string TenantId => TenantConstants.TestTenantId;

  public string SourceInfo =>
    string.Format("source = {0}, tenant = {1}", this.GetType().Name, this.TenantId ?? "null");
}
