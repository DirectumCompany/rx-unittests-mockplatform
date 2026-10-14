using MockPlatform.Core;
using MockPlatform.Extensions;
using Sungero.Company;
using Sungero.Company.Server;
using Sungero.CoreEntities;
using Sungero.CoreEntities.DatabookEntry;
using UnitTests.Company.Managers;

namespace UnitTests.Company.UserBaseServerFunctions;

/// <summary>
/// Юнит тесты для функции IsPasswordAuthenticationTypeForExternalApps.
/// Функция проверяет, используется ли для пользователя аутентификация по паролю при работе с корпоративными приложениями.
/// </summary>
public class IsPasswordAuthenticationTypeForExternalApps
{
  private const string ExternalAuthenticationType = "Ntlm";
  private const string IsExcludedAuthProtocolMethodName = "IsExcludedAuthProtocolForExternalApps";

  [Test]
  [Description("Если у учетной записи пользователя аутентификация по паролю, возвращается true.")]
  public void IsPasswordAuthenticationTypeForExternalApps_LoginWithPassword_ReturnsTrue()
  {
    // Arrange
    using var isExcludedStub = MockIsExcludedAuthProtocol(false);
    var user = this.CreateUserWithLogin(Sungero.CoreEntities.Login.TypeAuthentication.Password);

    // Act
    var result = user.GetServerFunctions<UserBaseFunctions>().IsPasswordAuthenticationTypeForExternalApps(ExternalAuthenticationType);

    // Assert
    Assert.That(result, Is.True, "Аутентификация по паролю у учетной записи не распознана.");
  }

  [Test]
  [TestCase(null)]
  [TestCase("")]
  [Description("Если тип внешней аутентификации не задан, возвращается true.")]
  public void IsPasswordAuthenticationTypeForExternalApps_ExternalAuthTypeNotFilled_ReturnsTrue(string? externalAuthenticationType)
  {
    // Arrange
    using var isExcludedStub = MockIsExcludedAuthProtocol(false);
    var user = this.CreateUserWithLogin(Sungero.CoreEntities.Login.TypeAuthentication.Windows);

    // Act
    var result = user.GetServerFunctions<UserBaseFunctions>().IsPasswordAuthenticationTypeForExternalApps(externalAuthenticationType);

    // Assert
    Assert.That(result, Is.True, "Незаданный тип внешней аутентификации не считается аутентификацией по паролю.");
  }

  [Test]
  [Description("Если протокол внешней аутентификации исключен, возвращается true.")]
  public void IsPasswordAuthenticationTypeForExternalApps_ExcludedProtocol_ReturnsTrue()
  {
    // Arrange
    using var isExcludedStub = MockIsExcludedAuthProtocol(true);
    var user = this.CreateUserWithLogin(Sungero.CoreEntities.Login.TypeAuthentication.Windows);

    // Act
    var result = user.GetServerFunctions<UserBaseFunctions>().IsPasswordAuthenticationTypeForExternalApps(ExternalAuthenticationType);

    // Assert
    Assert.That(result, Is.True, "Исключенный протокол внешней аутентификации не считается аутентификацией по паролю.");
  }

  [Test]
  [Description("Если у учетной записи Windows-аутентификация, тип внешней аутентификации задан и не исключен, возвращается false.")]
  public void IsPasswordAuthenticationTypeForExternalApps_WindowsLoginAndNotExcludedProtocol_ReturnsFalse()
  {
    // Arrange
    using var isExcludedStub = MockIsExcludedAuthProtocol(false);
    var user = this.CreateUserWithLogin(Sungero.CoreEntities.Login.TypeAuthentication.Windows);

    // Act
    var result = user.GetServerFunctions<UserBaseFunctions>().IsPasswordAuthenticationTypeForExternalApps(ExternalAuthenticationType);

    // Assert
    Assert.That(result, Is.False, "Windows-аутентификация с неисключенным протоколом ошибочно считается аутентификацией по паролю.");
  }

  [Test]
  [Description("Если у пользователя нет учетной записи, тип внешней аутентификации задан и не исключен, возвращается false.")]
  public void IsPasswordAuthenticationTypeForExternalApps_WithoutLogin_ReturnsFalse()
  {
    // Arrange
    using var isExcludedStub = MockIsExcludedAuthProtocol(false);
    var user = UserBaseManager.CreateUserBase(1, Sungero.Parties.People.Create(), Status.Active);

    // Act
    var result = user.GetServerFunctions<UserBaseFunctions>().IsPasswordAuthenticationTypeForExternalApps(ExternalAuthenticationType);

    // Assert
    Assert.That(result, Is.False, "Пользователь без учетной записи с неисключенным протоколом ошибочно считается аутентифицированным по паролю.");
  }

  #region Helper методы

  private IUserBase CreateUserWithLogin(Sungero.Core.Enumeration typeAuthentication)
  {
    var login = Logins.Create();
    login.TypeAuthentication = typeAuthentication;
    return UserBaseManager.CreateUserBase(1, Sungero.Parties.People.Create(), Status.Active, login);
  }

  private static IDisposable MockIsExcludedAuthProtocol(bool isExcluded)
    => MockFunctionStubs.Setup<UserBaseFunctions>(
      IsExcludedAuthProtocolMethodName, (string _) => isExcluded);

  #endregion
}
