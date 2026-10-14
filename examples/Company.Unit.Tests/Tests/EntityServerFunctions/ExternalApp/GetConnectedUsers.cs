using MockPlatform.Extensions;
using Sungero.Company;
using Sungero.Company.Server;

namespace UnitTests.Company.ExternalAppServerFunctions;

/// <summary>
/// Юнит тесты для функции GetConnectedUsers.
/// Функция получает список подключенных к внешнему приложению пользователей.
/// </summary>
public class GetConnectedUsers
{
  [Test]
  [Description("Получение подключенных пользователей для неизвестного внешнего приложения")]
  public void GetConnectedUsers_UnknownExternalApp()
  {
    // Arrange
    // Создание внешнего приложения
    var externalApp = ExternalApps.Create();
    var externalAppFuncions = externalApp.GetServerFunctions<ExternalAppFunctions>();

    // Act
    var connectedUsers = externalAppFuncions.GetConnectedUsers();

    // Assert
    Assert.That(connectedUsers, Is.EquivalentTo(Enumerable.Empty<IUserBase>().AsQueryable()));
  }

  [Test]
  [Description("Получение подключенных пользователей для Omni")]
  public void GetConnectedUsers_OmniWithUsers()
  {
    // Arrange
    // Создание внешнего приложения Omni
    var externalApp = ExternalApps.Create();
    externalApp.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    // Создание пользователей с разными состояниями подключения
    var notConnectedUser = UserBases.Create();

    var connectingUser = UserBases.Create();
    connectingUser.OmniConnectionState = Sungero.Company.UserBase.OmniConnectionState.Connecting;

    var connectErrorUser = UserBases.Create();
    connectErrorUser.OmniConnectionState = Sungero.Company.UserBase.OmniConnectionState.ConnectError;

    var connectedUser = UserBases.Create();
    connectedUser.OmniConnectionState = Sungero.Company.UserBase.OmniConnectionState.Connected;

    var disconnectingUser = UserBases.Create();
    disconnectingUser.OmniConnectionState = Sungero.Company.UserBase.OmniConnectionState.Disconnecting;

    var disconnectErrorUser = UserBases.Create();
    disconnectErrorUser.OmniConnectionState = Sungero.Company.UserBase.OmniConnectionState.DisconnectError;

    var externalAppFuncions = externalApp.GetServerFunctions<ExternalAppFunctions>();

    // Act
    var connectedUsers = externalAppFuncions.GetConnectedUsers();

    // Assert
    IUserBase[] expected = [connectingUser, connectedUser, disconnectingUser, disconnectErrorUser];
    Assert.That(connectedUsers, Is.EquivalentTo(expected));
  }

  [Test]
  [Description("Получение подключенных пользователей для Portal")]
  public void GetConnectedUsers_PortalWithUsers()
  {
    // Arrange
    // Создание внешнего приложения Portal
    var externalApp = ExternalApps.Create();
    externalApp.Sid = Sungero.Company.Constants.ExternalApp.PortalSid;

    // Создание пользователей с разными состояниями подключения
    var notConnectedUser = UserBases.Create();

    var connectingUser = UserBases.Create();
    connectingUser.PortalConnectionState = Sungero.Company.UserBase.PortalConnectionState.Connecting;

    var connectErrorUser = UserBases.Create();
    connectErrorUser.PortalConnectionState = Sungero.Company.UserBase.PortalConnectionState.ConnectError;

    var connectedUser = UserBases.Create();
    connectedUser.PortalConnectionState = Sungero.Company.UserBase.PortalConnectionState.Connected;

    var disconnectingUser = UserBases.Create();
    disconnectingUser.PortalConnectionState = Sungero.Company.UserBase.PortalConnectionState.Disconnecting;

    var disconnectErrorUser = UserBases.Create();
    disconnectErrorUser.PortalConnectionState = Sungero.Company.UserBase.PortalConnectionState.DisconnectError;

    var externalAppFuncions = externalApp.GetServerFunctions<ExternalAppFunctions>();

    // Act
    var connectedUsers = externalAppFuncions.GetConnectedUsers();

    // Assert
    IUserBase[] expected = [connectingUser, connectedUser, disconnectingUser, disconnectErrorUser];
    Assert.That(connectedUsers, Is.EquivalentTo(expected));
  }
}
