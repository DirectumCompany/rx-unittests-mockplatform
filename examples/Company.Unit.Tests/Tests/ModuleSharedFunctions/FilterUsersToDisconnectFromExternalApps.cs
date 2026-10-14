using Sungero.Company;
using Sungero.Company.Constants;
using Sungero.Company.UserBase;
using ModuleFunctions = Sungero.Company.Shared.ModuleFunctions;

namespace UnitTests.Company.ModuleSharedFunctions;

public class FilterUsersToDisconnectFromExternalApps
{
  [Test]
  [Description("Подключенный ко всем приложениям пользователь должен быть в списке для отключения.")]
  public void FilterUsersToDisconnectFromExternalApps_ConnectedAndDisconnectedUsers()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = ExternalApp.PortalSid;

    var connectedUser = UserBases.Create();
    connectedUser.PortalConnectionState = PortalConnectionState.Connected;
    connectedUser.OmniConnectionState = OmniConnectionState.Connected;

    var disconnectedUser = UserBases.Create();

    // Act
    var filteredUsers = new ModuleFunctions()
      .FilterUsersToDisconnectFromExternalApps([connectedUser, disconnectedUser], [omni, portal]);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(filteredUsers, Has.Member(connectedUser));
      Assert.That(filteredUsers, Has.No.Member(disconnectedUser));
    });
  }

  [Test]
  [Description("Пользователь с ошибкой отключения должен быть в списке для отключения.")]
  public void FilterUsersToDisconnectFromExternalApps_UserWithDisconnectionError()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = ExternalApp.PortalSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.DisconnectError;
    user.OmniConnectionState = OmniConnectionState.Connected;

    // Act
    var filteredUsers = new ModuleFunctions()
      .FilterUsersToDisconnectFromExternalApps([user], [omni, portal]);

    // Assert
    Assert.That(filteredUsers, Has.Member(user));
  }
}
