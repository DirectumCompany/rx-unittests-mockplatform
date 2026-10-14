using Sungero.Core;
using Sungero.Company.PublicFunctions;
using static Sungero.Company.UserBase.OmniConnectionState;

namespace UnitTests.Company.UserBaseSharedFunctions;

public class CanChangeOmniConnectionState
{
  private readonly List<Enumeration?> allStates = [
    null, Connected, ConnectError,
    Connecting, Disconnected, DisconnectError, Disconnecting
    ];

  [Test]
  [Description("Новое состояние является конечным.")]
  public void CanChangeOmniConnectionState_NewStateIsNotInProcess()
  {
    // Arrange
    List<Enumeration?> oldStates = allStates;
    List<Enumeration?> newStates = allStates.Except([Connecting, Disconnecting]).ToList();

    // Act
    List<bool> results = [];
    foreach (var oldState in oldStates)
      foreach (var newState in newStates)
        results.Add(UserBase.CanChangeOmniConnectionState(oldState, newState));

    // Assert
    Assert.That(results, Is.All.True);
  }

  [Test]
  [Description("Новое состояние - в процессе подключения.")]
  public void CanChangeOmniConnectionState_NewStateIsConnecting()
  {
    // Arrange
    List<Enumeration?> validOldStates = [null, ConnectError, Disconnected];
    List<Enumeration?> invalidOldStates = allStates.Except(validOldStates).ToList();
    var newState = Connecting;

    // Act
    List<bool> validResults = [];
    foreach (var oldState in validOldStates)
      validResults.Add(UserBase.CanChangeOmniConnectionState(oldState, newState));

    List<bool> invalidResults = [];
    foreach (var oldState in invalidOldStates)
      invalidResults.Add(UserBase.CanChangeOmniConnectionState(oldState, newState));

    // Assert
    Assert.That(validResults, Is.All.True);
    Assert.That(invalidResults, Is.All.False);
  }

  [Test]
  [Description("Новое состояние - в процессе отключения.")]
  public void CanChangeOmniConnectionState_NewStateIsDisconnecting()
  {
    // Arrange
    List<Enumeration?> validOldStates = [Connected, DisconnectError];
    List<Enumeration?> invalidOldStates = allStates.Except(validOldStates).ToList();
    var newState = Disconnecting;

    // Act
    List<bool> validResults = [];
    foreach (var oldState in validOldStates)
      validResults.Add(UserBase.CanChangeOmniConnectionState(oldState, newState));

    List<bool> invalidResults = [];
    foreach (var oldState in invalidOldStates)
      invalidResults.Add(UserBase.CanChangeOmniConnectionState(oldState, newState));

    // Assert
    Assert.That(validResults, Is.All.True);
    Assert.That(invalidResults, Is.All.False);
  }
}
