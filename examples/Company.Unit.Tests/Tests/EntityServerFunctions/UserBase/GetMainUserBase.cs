using MockPlatform.Extensions;
using Sungero.Company.Server;
using Sungero.CoreEntities;
using Sungero.CoreEntities.DatabookEntry;
using UnitTests.Company.Managers;

namespace UnitTests.Company.UserBaseServerFunctions;

/// <summary>
/// Юнит тесты для функции GetMainUserBase.
/// Функция возвращает основного пользователя.
/// </summary>
public class GetMainUserBase
{
  [Test]
  [Description("Если УЗ у текущего пользователя не заполнена, выбирается активный пользователь с той же персоной и заполненной УЗ.")]
  public void GetMainUserBase_PrefersUserWithLogin()
  {
    // Arrange
    var person = Sungero.Parties.People.Create();
    var login = Logins.Create();
    var current = UserBaseManager.CreateUserBase(1, person, Status.Active);
    var userWithoutLogin = UserBaseManager.CreateUserBase(2, person, Status.Active);
    var userWithLogin = UserBaseManager.CreateUserBase(3, person, Status.Active, login);

    var functions = current.GetServerFunctions<UserBaseFunctions>();

    // Act
    var result = functions.GetMainUserBase();

    // Assert
    Assert.That(result, Is.EqualTo(userWithLogin));
  }

  [Test]
  [Description("При одинковых персонах и разных учетных записей сортировка выдаст IsWorkingInWebClient=false раньше true.")]
  public void GetMainUserBase_PrefersIsWorkingInWebClientFalse()
  {
    // Arrange
    var person = Sungero.Parties.People.Create();
    var login = Logins.Create();
    var current = UserBaseManager.CreateUserBase(1, person, Status.Active);

    var userWorkingInWebClient = UserBaseManager.CreateUserBase(2, person, Status.Active);
    userWorkingInWebClient.IsWorkingInWebClient = true;

    var userNotWorkingInWebClient = UserBaseManager.CreateUserBase(3, person, Status.Active);
    userNotWorkingInWebClient.IsWorkingInWebClient = false;

    var functions = current.GetServerFunctions<UserBaseFunctions>();

    // Act
    var result = functions.GetMainUserBase();

    // Assert
    Assert.That(result, Is.EqualTo(userNotWorkingInWebClient));
  }

  [Test]
  [Description("Если все критерии одинаковые, выбирается пользователь с минимальным Id.")]
  public void GetMainUserBase_WhenTie_PrefersMinimalId()
  {
    // Arrange
    var person = Sungero.Parties.People.Create();
    var login = Logins.Create();
    var current = UserBaseManager.CreateUserBase(10, person, Status.Active);
    current.IsWorkingInWebClient = true;

    var userWithMidId = UserBaseManager.CreateUserBase(7, person, Status.Active);
    userWithMidId.IsWorkingInWebClient = true;

    var userWithMinimalId = UserBaseManager.CreateUserBase(1, person, Status.Active);
    userWithMinimalId.IsWorkingInWebClient = true;

    var functions = current.GetServerFunctions<UserBaseFunctions>();

    // Act
    var result = functions.GetMainUserBase();

    // Assert
    Assert.That(result, Is.EqualTo(userWithMinimalId));
  }

  [Test]
  [Description("В выборку попадают только активные пользователи и только с той же персоной.")]
  public void GetMainUserBase_FiltersByPersonAndActiveStatus()
  {
    // Arrange
    var firstPerson = Sungero.Parties.People.Create();
    var secondPerson = Sungero.Parties.People.Create();

    var login = Logins.Create();
    var current = UserBaseManager.CreateUserBase(1, firstPerson, Status.Active);

    var closedUserWithSamePerson = UserBaseManager.CreateUserBase(2, firstPerson, Status.Closed, login);

    var activeUserWithOtherPerson = UserBaseManager.CreateUserBase(3, secondPerson, Status.Active, login);

    var functions = current.GetServerFunctions<UserBaseFunctions>();

    // Act
    var result = functions.GetMainUserBase();

    // Assert
    Assert.That(result, Is.EqualTo(current));
  }

  [Test]
  [Description("Если нет основного пользователя, то возвращается сам пользователь.")]
  public void GetMainUserBase_NoCandidates_ReturnsNull()
  {
    // Arrange
    var person = Sungero.Parties.People.Create();

    var login = Logins.Create();
    var user = UserBaseManager.CreateUserBase(1, person, Status.Active);
    user.IsWorkingInWebClient = true;

    var functions = user.GetServerFunctions<UserBaseFunctions>();

    // Act
    var result = functions.GetMainUserBase();

    // Assert
    Assert.That(result, Is.EqualTo(user));
  }
}
