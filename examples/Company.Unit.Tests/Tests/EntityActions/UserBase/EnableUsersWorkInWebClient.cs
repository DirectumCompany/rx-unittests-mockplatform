using MockPlatform;
using MockPlatform.Core;
using MockPlatform.Stubs;
using NUnit.Framework.Internal;
using Sungero.Company;
using Sungero.Company.Client;
using Sungero.Domain.Client;
using Sungero.Domain.Shared;
using DocflowClientFunctions = Sungero.Docflow.Client.ModuleClientPublicFunctions;

namespace UnitTests.Company.UserBaseActions
{
  public class EnableUsersWorkInWebClient
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
    [Description("Признак «Работает в веб-клиенте» выключен — признак включается.")]
    public void EnableUsersWorkInWebClient_WorkInWebClientDisabled_Enabled()
    {
      // Arrange
      var userBase = UserBases.Create();
      userBase.IsWorkingInWebClient = false;

      // Act
      var actions = new UserBaseCollectionActions();
      actions.SetEntities([userBase]);
      var args = new ExecuteActionArgs(FormType.Collection, null);
      actions.EnableUsersWorkInWebClient(args);

      // Assert
      Assert.That(userBase.IsWorkingInWebClient, Is.True);
    }

    [Test]
    [Description("Признак «Работает в веб-клиенте» включен — признак остаётся включенным.")]
    public void EnableUsersWorkInWebClient_WorkInWebClientEnabled_RemainsEnabled()
    {
      // Arrange
      var userBase = UserBases.Create();
      userBase.IsWorkingInWebClient = true;

      // Act
      var actions = new UserBaseCollectionActions();
      actions.SetEntities([userBase]);
      var args = new ExecuteActionArgs(FormType.Collection, null);
      actions.EnableUsersWorkInWebClient(args);

      // Assert
      Assert.That(userBase.IsWorkingInWebClient, Is.True);
    }
  }
}
