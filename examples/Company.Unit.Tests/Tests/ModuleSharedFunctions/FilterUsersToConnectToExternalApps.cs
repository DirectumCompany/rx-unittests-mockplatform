using Sungero.Company;
using Sungero.Company.PublicFunctions;
using Sungero.Company.UserBase;

namespace UnitTests.Company.ModuleSharedFunctions;

public class FilterUsersToConnectToExternalApps
{
  [Test]
  [Description("Отключенный от всех приложений пользователь должен быть в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_ConnectedAndDisconnectedUsers_PortalAndOmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var portal = ExternalApps.Create();
    portal.Sid = Sungero.Company.Constants.ExternalApp.PortalSid;

    var disconnectedUser = UserBases.Create();
    var connectedUser = UserBases.Create();
    connectedUser.OmniConnectionState = OmniConnectionState.Connected;
    connectedUser.PortalConnectionState = PortalConnectionState.Connected;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([disconnectedUser, connectedUser], [omni, portal]);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(filteredUsers, Has.Member(disconnectedUser));
      Assert.That(filteredUsers, Has.No.Member(connectedUser));
    });
  }

  [Test]
  [Description("Пользователь с ошибкой подключения должен быть в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_UserWithConnectionError_OmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;
    user.OmniConnectionState = OmniConnectionState.ConnectError;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [omni]);

    // Assert
    Assert.That(filteredUsers, Has.Member(user));
  }

  [Test]
  [Description("Пользователь с ошибкой отключения должен отсутствовать в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_UserWithDisconnectionError_OmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;
    user.OmniConnectionState = OmniConnectionState.DisconnectError;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [omni]);

    // Assert
    Assert.That(filteredUsers, Has.No.Member(user));
  }

  [Test]
  [Description("Пользователь в процессе подключения должен отсутствовать в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_ConnectingUser_OmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;
    user.OmniConnectionState = OmniConnectionState.Connecting;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [omni]);

    // Assert
    Assert.That(filteredUsers, Has.No.Member(user));
  }

  [Test]
  [Description("Пользователь без подключения должен быть в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_NotConnectedUser_OmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [omni]);

    // Assert
    Assert.That(filteredUsers, Has.Member(user));
  }

  [Test]
  [Description("Пользователь с ошибкой отключения должен отсутствовать в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_UserWithDisconnectionError_PortalChosen()
  {
    // Arrange
    var portal = ExternalApps.Create();
    portal.Sid = Sungero.Company.Constants.ExternalApp.PortalSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;
    user.OmniConnectionState = OmniConnectionState.DisconnectError;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [portal]);

    // Assert
    Assert.That(filteredUsers, Has.No.Member(user));
  }

  [Test]
  [Description("Пользователь в процессе отключения должен отсутствовать в списке для подключения.")]
  public void FilterUsersToConnectToExternalApps_DisconnectingUser_OmniChosen()
  {
    // Arrange
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;

    var user = UserBases.Create();
    user.PortalConnectionState = PortalConnectionState.Connected;
    user.OmniConnectionState = OmniConnectionState.Disconnecting;

    // Act
    var filteredUsers = Module.FilterUsersToConnectToExternalApps([user], [omni]);

    // Assert
    Assert.That(filteredUsers, Has.No.Member(user));
  }
}
