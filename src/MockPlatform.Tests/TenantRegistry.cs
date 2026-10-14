using MockPlatform.Services.Tenants;
using Sungero.Domain;

namespace MockPlatform.Tests;

/// <summary>
/// Тесты конфигурации TenantRegistry в mock-платформе.
/// </summary>
public class TenantRegistryTests
{
  [Test]
  [Description("AdministrationSettings.AdministratorEmail читается из настроек тенанта.")]
  public void AdministratorEmail_IsConfigured()
  {
    // Arrange
    // Act
    // Assert
    Assert.That(AdministrationSettings.AdministratorEmail, Is.EqualTo(MockTenantConstants.DefaultAdministratorEmail));
  }
}
