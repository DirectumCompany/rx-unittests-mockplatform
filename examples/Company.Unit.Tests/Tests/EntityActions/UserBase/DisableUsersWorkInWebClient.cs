using MockPlatform;
using MockPlatform.Core;
using MockPlatform.Stubs;
using Sungero.Company;
using Sungero.Company.Client;
using Sungero.Domain.Client;
using Sungero.Domain.Shared;
using DocflowClientFunctions = Sungero.Docflow.Client.ModuleClientPublicFunctions;

namespace UnitTests.Company.UserBaseActions
{
  public class DisableUsersWorkInWebClient
  {
    [SetUp]
    public void SetUp()
    {
      MockCurrentUser.Set(UserBases.Create());

      RepositoryAccessRightsStubs.Setup<IUserBaseAccessRights>(typeof(UserBases),
        (mock) => { mock.Setup(ar => ar.CanUpdate()).Returns(true); });

      MockFunctionStubs.Setup<DocflowClientFunctions>(
        nameof(DocflowClientFunctions.ShowConfirmationDialog),
        (string _, string _, string _, string _) => true);
    }

    [Test]
    [Description("Признак «Работает в веб-клиенте» выключен — признак остаётся выключенным.")]
    public void DisableUsersWorkInWebClient_WorkInWebClientDisabled_RemainsDisabled()
    {
      // Arrange
      var userBase = UserBases.Create();
      userBase.IsWorkingInWebClient = false;

      // Act
      var actions = new UserBaseCollectionBulkActions();
      actions.SetEntityIds([new EntityIdentifier(userBase)]);
      var args = new ExecuteActionArgs(FormType.Collection, null);
      actions.DisableUsersWorkInWebClient(args);

      // Assert
      Assert.That(userBase.IsWorkingInWebClient, Is.False);
    }

    [Test]
    [Description("Признак «Работает в веб-клиенте» включен — признак выключается.")]
    public void DisableUsersWorkInWebClient_WorkInWebClientEnabled_Disabled()
    {
      // Arrange
      var userBase = UserBases.Create();
      userBase.IsWorkingInWebClient = true;

      // Act
      var actions = new UserBaseCollectionBulkActions();
      actions.SetEntityIds([new EntityIdentifier(userBase)]);
      var args = new ExecuteActionArgs(FormType.Collection, null);
      actions.DisableUsersWorkInWebClient(args);

      // Assert
      Assert.That(userBase.IsWorkingInWebClient, Is.False);
    }
  }
}
