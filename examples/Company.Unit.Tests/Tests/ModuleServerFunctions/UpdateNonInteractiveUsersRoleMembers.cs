using Moq;
using Sungero.Company;
using Sungero.Company.Server.AsyncHandlerInvokeArgs;
using UnitTests.Company.Managers;
using ModuleFunctions = Sungero.Company.Server.ModuleFunctions;

namespace UnitTests.Company.ModuleServerFunctions;

public class UpdateNonInteractiveUsersRoleMembers
{
  [Test]
  [Description("Пользователи с разным признаком работы в веб-клиенте — неинтерактивный включается в роль, интерактивный исключается.")]
  public void UpdateNonInteractiveUsersRoleMembers_InteractiveAndNonInteractiveUsers_UpdatesRoleMembers()
  {
    // Arrange
    var interactiveUser = UserBases.Create();
    interactiveUser.IsWorkingInWebClient = true;

    var nonInteractiveUser = UserBases.Create();
    nonInteractiveUser.IsWorkingInWebClient = false;

    var role = RoleManager.CreateNoninteractiveUsersRole();
    Mock.Get(role).Setup(r => r.State.IsChanged).Returns(true);
    role.RecipientLinks.AddNew().Member = interactiveUser;

    var args = new UpdateNonInteractiveUsersRoleMembersInvokeArgs
    {
      UserIds = string.Join(',', new object[] { interactiveUser.Id, nonInteractiveUser.Id })
    };

    // Act
    new ModuleFunctions().UpdateNonInteractiveUsersRoleMembers(args);

    // Assert
    var roleRecipients = role.RecipientLinks.Select(r => r.Member);
    Assert.Multiple(() =>
    {
      Assert.That(roleRecipients, Has.Member(nonInteractiveUser));
      Assert.That(roleRecipients, Has.No.Member(interactiveUser));
    });
  }
}