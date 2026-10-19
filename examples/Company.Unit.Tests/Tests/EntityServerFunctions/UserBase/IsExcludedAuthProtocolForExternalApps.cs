using System.Reflection;
using MockPlatform.Core;
using Sungero.Company.Server;

namespace UnitTests.Company.UserBaseServerFunctions;

/// <summary>
/// Юнит тесты для функции IsExcludedAuthProtocolForExternalApps.
/// Функция проверяет, указан ли протокол внешней аутентификации в параметре системы ExcludedAuthProtocolsForExternalApps.
/// </summary>
public class IsExcludedAuthProtocolForExternalApps
{
  private const string Ntlm = "Ntlm";
  private const string MethodName = "IsExcludedAuthProtocolForExternalApps";

  [Test]
  [TestCase(null)]
  [TestCase("")]
  [Description("Если тип внешней аутентификации не заполнен, возвращается false.")]
  public void IsExcludedAuthProtocolForExternalApps_AuthTypeNotFilled_ReturnsFalse(string? externalAuthenticationType)
  {
    // Arrange
    using var parameterStub = MockExcludedProtocolsParameter(Ntlm);

    // Act
    var result = CallMethod(externalAuthenticationType);

    // Assert
    Assert.That(result, Is.False, "Незаданный тип аутентификации ошибочно считается исключенным.");
  }

  [Test]
  [TestCase(null)]
  [TestCase("")]
  [TestCase(" ")]
  [Description("Если параметр отсутствует или не заполнен, возвращается false.")]
  public void IsExcludedAuthProtocolForExternalApps_ParameterNotFilled_ReturnsFalse(string? parameterValue)
  {
    // Arrange
    using var parameterStub = MockExcludedProtocolsParameter(parameterValue);

    // Act
    var result = CallMethod(Ntlm);

    // Assert
    Assert.That(result, Is.False, "При незаполненном параметре протокол ошибочно считается исключенным.");
  }

  [Test]
  [TestCase(Ntlm, Ntlm)]
  [TestCase("OpenId", "Ntlm,OpenId,SAML")]
  [TestCase("SAML", "Ntlm,OpenId,SAML")]
  [TestCase("OpenId", " Ntlm , OpenId ,SAML")]
  [TestCase("Adfs", "Ntlm,  Adfs  ")]
  [Description("Если протокол указан в параметре системы, возвращается true.")]
  public void IsExcludedAuthProtocolForExternalApps_ProtocolInParameter_ReturnsTrue(string externalAuthenticationType, string parameterValue)
  {
    // Arrange
    using var parameterStub = MockExcludedProtocolsParameter(parameterValue);

    // Act
    var result = CallMethod(externalAuthenticationType);

    // Assert
    Assert.That(result, Is.True, "Протокол, указанный в параметре, не распознан как исключенный.");
  }

  [Test]
  [TestCase(Ntlm, "ntlm")]
  [TestCase("OpenId", "NTLM,openid")]
  [Description("Регистр протоколов в параметре не учитывается.")]
  public void IsExcludedAuthProtocolForExternalApps_DifferentCase_ReturnsTrue(string externalAuthenticationType, string parameterValue)
  {
    // Arrange
    using var parameterStub = MockExcludedProtocolsParameter(parameterValue);

    // Act
    var result = CallMethod(externalAuthenticationType);

    // Assert
    Assert.That(result, Is.True, "Различие в регистре помешало распознать протокол как исключенный.");
  }

  [Test]
  [TestCase("SAML", "Ntlm,OpenId")]
  [TestCase("OpenId", "OpenIdConnect")]
  [TestCase("SAML", "Saml2")]
  [TestCase(Ntlm, ",,")]
  [Description("Если протокол не указан в параметре системы или совпадает частично, возвращается false.")]
  public void IsExcludedAuthProtocolForExternalApps_ProtocolNotInParameter_ReturnsFalse(string externalAuthenticationType, string parameterValue)
  {
    // Arrange
    using var parameterStub = MockExcludedProtocolsParameter(parameterValue);

    // Act
    var result = CallMethod(externalAuthenticationType);

    // Assert
    Assert.That(result, Is.False, "Протокол, отсутствующий в параметре, ошибочно считается исключенным.");
  }

  #region Helper методы

  private static bool CallMethod(string? externalAuthenticationType)
  {
    var method = typeof(UserBaseFunctions).GetMethod(MethodName, BindingFlags.NonPublic | BindingFlags.Static);
    return (bool)method!.Invoke(null, new object?[] { externalAuthenticationType })!;
  }

  private static IDisposable MockExcludedProtocolsParameter(string? value)
    => MockFunctionStubs.Setup<Sungero.Docflow.Server.ModuleFunctions>(
      nameof(Sungero.Docflow.Server.ModuleFunctions.GetDocflowParamsStringValue), (string _) => value);

  #endregion
}
