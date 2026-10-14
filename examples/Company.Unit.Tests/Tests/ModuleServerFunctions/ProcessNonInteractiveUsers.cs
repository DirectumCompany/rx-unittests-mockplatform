using MockPlatform.Core;
using Moq;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.CoreEntities.Login;
using Sungero.Domain.Shared;
using Sungero.Parties;
using System.Reflection;
using IdsOperation = Sungero.Company.Structures.UserBase.IdsOperation;
using IdsOperationStatus = Sungero.Company.Constants.UserBase.IdsOperationStatus;
using ModuleFunctions = Sungero.Company.Server.ModuleFunctions;

namespace UnitTests.Company.ModuleServerFunctions;

public class ProcessNonInteractiveUsers
{
  [Test]
  [Description("Пользователь и роль «Неинтерактивные пользователи» не заблокированы — пользователь получает учётную запись и включается в роль.")]
  public void ProcessNonInteractiveUsers_NotLocked_UserProcessed()
  {
    // Arrange
    var user = UserBases.Create();
    user.IsWorkingInWebClient = false;
    var person = People.Create();
    user.Person = person;

    var idsOperation = new IdsOperation
    {
      Status = IdsOperationStatus.InProcess,
      User = user
    };

    var role = Roles.Create();
    role.Sid = Sungero.Company.Constants.Module.NonInteractiveUsersRole;

    // Act
    this.CallMethod([idsOperation]);
    var login = user.Login;
    var roleRecipients = role.RecipientLinks.Select(r => r.Member);

    // Assert
    Assert.That(login, Is.Not.Null);
    Assert.Multiple(() =>
    {
      Assert.That(login.LoginName, Is.EqualTo($"RX{person.Id}"));
      Assert.That(login.TypeAuthentication, Is.EqualTo(TypeAuthentication.Windows));
      Assert.That(role.RecipientLinks.Select(r => r.Member), Does.Contain(user));
      Assert.That(idsOperation.Status, Is.EqualTo(IdsOperationStatus.InProcess));
    });
  }

  [Test]
  [Description("Пользователь заблокирован — операция получает статус блокировки.")]
  public void ProcessNonInteractiveUsers_UserLocked_StatusLock()
  {
    // Arrange
    var user = UserBases.Create();
    user.IsWorkingInWebClient = false;
    var person = People.Create();
    user.Person = person;

    var idsOperation = new IdsOperation
    {
      Status = IdsOperationStatus.InProcess,
      User = user
    };

    var role = Roles.Create();
    role.Sid = Sungero.Company.Constants.Module.NonInteractiveUsersRole;

    var locksMock = new Mock<ILocksImplementer>();
    locksMock.Setup(l => l.TryLock(role, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
    locksMock.Setup(l => l.TryLock(user, It.IsAny<bool>(), It.IsAny<bool>())).Returns(false);
    using var locksImplementer = IMockable<ILocksImplementer>.WithMock(locksMock.Object);

    // Act
    this.CallMethod([idsOperation]);

    // Assert
    Assert.That(user.Login, Is.Null);
    Assert.Multiple(() =>
    {
      Assert.That(role.RecipientLinks.Select(r => r.Member), Does.Contain(user));
      Assert.That(idsOperation.Status, Is.EqualTo(IdsOperationStatus.Lock));
    });
  }

  [Test]
  [Description("Роль «Неинтерактивные пользователи» заблокирована — операция получает статус блокировки.")]
  public void ProcessNonInteractiveUsers_RoleLocked_StatusLock()
  {
    // Arrange
    var user = UserBases.Create();
    user.IsWorkingInWebClient = false;
    var person = People.Create();
    user.Person = person;

    var idsOperation = new IdsOperation
    {
      Status = IdsOperationStatus.InProcess,
      User = user
    };

    var role = Roles.Create();
    role.Sid = Sungero.Company.Constants.Module.NonInteractiveUsersRole;

    var locksMock = new Mock<ILocksImplementer>();
    locksMock.Setup(l => l.TryLock(role, It.IsAny<bool>(), It.IsAny<bool>())).Returns(false);
    using var locksImplementer = IMockable<ILocksImplementer>.WithMock(locksMock.Object);

    // Act
    this.CallMethod([idsOperation]);

    // Assert
    Assert.That(user.Login, Is.Null);
    Assert.Multiple(() =>
    {
      Assert.That(role.RecipientLinks, Is.Empty);
      Assert.That(idsOperation.Status, Is.EqualTo(IdsOperationStatus.Lock));
    });
  }

  #region Helper методы

  /// <summary>
  /// Вызвать закрытый метод ProcessNonInteractiveUsers модуля через рефлексию.
  /// </summary>
  /// <param name="idsOperations">Операции обработки пользователей.</param>
  private void CallMethod(IEnumerable<Sungero.Company.Structures.UserBase.IIdsOperation> idsOperations)
  {
    var functions = new ModuleFunctions();
    var type = functions.GetType();
    var method = type.GetMethod("ProcessNonInteractiveUsers", BindingFlags.NonPublic | BindingFlags.Instance);
    method.Invoke(functions, new object[] { idsOperations });
  }

  #endregion
}