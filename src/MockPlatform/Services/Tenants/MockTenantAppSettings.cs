using System.Xml.Linq;
using Sungero.Tenants;

namespace MockPlatform.Services.Tenants;

/// <summary>
/// Настройки приложения с тестовым тенантом для mock-платформы.
/// </summary>
public class MockTenantAppSettings : TenantAppSettings
{
  private readonly XElement tenantsBlock = XElement.Parse($@"
<TENANTS>
<tenant
    name=""{TenantConstants.TestTenantId}""
    db=""{MockTenantConstants.DatabaseName}""
    hyperlinkServer = ""{MockTenantConstants.HyperlinkServer}""
    utcOffset = ""{MockTenantConstants.UtcOffset.Hours}""
    administratorEmail=""{MockTenantConstants.DefaultAdministratorEmail}""
    externalAuthenticationType = ""Windows""
    closeInactiveLoginAfterDays=""-1""
    logOffInactiveUsersAfterMinutes=""-1""
    licenseExpireNotificationDays=""14""
    />
</TENANTS>");

  public override XElement TenantsBlock => this.tenantsBlock;
}
