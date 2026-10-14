using MockPlatform.Extensions;
using Sungero.Company;
using Sungero.Company.Constants;
using Sungero.Company.Shared;
using Sungero.Company.UserBase;

namespace UnitTests.Company.UserBaseSharedFunctions;

/// <summary>
/// Юнит тесты для функции FilterUnconnectedApps.
/// Функция возвращает список приложений из выбранных, к которым пользователь не подключен.
/// </summary>
public class FilterUnconnectedApps
{
  [Test]
  [Description("Пользователь не подключен ни к одному приложению — возвращаются все выбранные приложения.")]
  public void FilterUnconnectedApps_NoConnections_ReturnsAllSelected()
  {
    // Arrange
    var user = UserBases.Create();

    var omni = ExternalApps.Create();
    omni.Sid = ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = ExternalApp.PortalSid;

    var selectedApps = new List<IExternalApp> { omni, portal };

    var functions = user.GetSharedFunctions<UserBaseFunctions>();

    // Act
    var result = functions.FilterUnconnectedApps(selectedApps);

    // Assert
    Assert.That(result, Is.EquivalentTo(selectedApps));
  }

  [Test]
  [Description("Пользователь подключен к Omni — из выбранных возвращается только Portal.")]
  public void FilterUnconnectedApps_ConnectedToOmni_ReturnsOnlyPortal()
  {
    // Arrange
    var user = UserBases.Create();
    user.OmniConnectionState = OmniConnectionState.Connected;

    var omni = ExternalApps.Create();
    omni.Sid = ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = ExternalApp.PortalSid;

    var functions = user.GetSharedFunctions<UserBaseFunctions>();

    // Act
    var result = functions.FilterUnconnectedApps([omni, portal]);

    // Assert
    Assert.That(result.Count, Is.EqualTo(1));
    Assert.That(result[0], Is.EqualTo(portal));
  }

  [Test]
  [Description("Пользователь подключен к Omni и Portal — из выбранных приложений не остается ни одного неподключенного.")]
  public void FilterUnconnectedApps_ConnectedToOmniAndPortal_ReturnsEmpty()
  {
    // Arrange
    var user = UserBases.Create();
    user.OmniConnectionState = OmniConnectionState.Connected;
    user.PortalConnectionState = PortalConnectionState.Connected;

    var omni = ExternalApps.Create();
    omni.Sid = ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = ExternalApp.PortalSid;

    var functions = user.GetSharedFunctions<UserBaseFunctions>();

    // Act
    var result = functions.FilterUnconnectedApps([omni, portal]);

    // Assert
    Assert.That(result.Count, Is.EqualTo(0));
  }
}