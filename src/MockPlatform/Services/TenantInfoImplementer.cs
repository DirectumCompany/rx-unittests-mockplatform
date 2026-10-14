using Sungero.Domain;
using Sungero.Domain.Shared;
using Sungero.Tenants;

namespace MockPlatform.Services;

public class TenantInfoImplementer : ITenantInfo
{
  public string TenantId => TenantRegistry.Instance.CurrentTenant.Id;

  public TimeSpan UtcOffset => TenantRegistry.Instance.CurrentTenant.GetKernelSettings().UtcOffset;

  public string TenantCultureName => TenantRegistry.Instance.CurrentTenant.GetKernelSettings().Culture.Name;
}
