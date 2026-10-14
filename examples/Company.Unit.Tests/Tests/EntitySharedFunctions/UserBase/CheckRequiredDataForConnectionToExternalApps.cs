using Sungero.Company.Shared;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.Parties;
using MockPlatform.Extensions;

namespace UnitTests.Company.UserBaseSharedFunctions;

public class CheckRequiredDataForConnectionToExternalApps
{
  [Test]
  [Description("Пользователь не имеет почты, пользователь имеет нужные данные для подключения")]
  public void CheckRequiredDataForConnectionToExternalApps_UserHasNoEmail()
  {
    // Arrange
    var user = UserBases.Create();
    user.Person = People.Create();
    user.Person.PhoneHash = "test";
    user.IsWorkingInWebClient = true;
    user.Login = Logins.Create();

    // Act
    var result = user.GetSharedFunctions<UserBaseFunctions>()
      .CheckRequiredDataForConnectionToExternalApps(null);

    // Assert
    Assert.That(result);
  }

  [Test]
  [Description("Пользователь не имеет личного телефона, пользователь имеет нужные данные для подключения")]
  public void CheckRequiredDataForConnectionToExternalApps_UserHasNoPhone()
  {
    // Arrange
    var user = UserBases.Create();
    user.Person = People.Create();
    user.Email = "test@test.ru";
    user.IsWorkingInWebClient = true;
    user.Login = Logins.Create();

    // Act
    var result = user.GetSharedFunctions<UserBaseFunctions>()
      .CheckRequiredDataForConnectionToExternalApps(null);

    // Assert
    Assert.That(result);
  }

  [Test]
  [Description("Пользователь не имеет учетной записи, пользователь не имеет нужных данных для подключения")]
  public void CheckRequiredDataForConnectionToExternalApps_UserHasNoLogin()
  {
    // Arrange
    var user = UserBases.Create();
    user.Person = People.Create();
    user.Email = "test@test.ru";
    user.Person.PhoneHash = "test";
    user.IsWorkingInWebClient = true;

    // Act
    var result = user.GetSharedFunctions<UserBaseFunctions>()
      .CheckRequiredDataForConnectionToExternalApps(null);

    // Assert
    Assert.That(result, Is.False);
  }

  [Test]
  [Description("Пользователь не имеет личного телефона и почты, пользователь не имеет нужных данных для подключения")]
  public void CheckRequiredDataForConnectionToExternalApps_UserHasNoPhoneAndEmail()
  {
    // Arrange
    var user = UserBases.Create();
    user.Person = People.Create();
    user.IsWorkingInWebClient = true;
    user.Login = Logins.Create();

    // Act
    var result = user.GetSharedFunctions<UserBaseFunctions>()
      .CheckRequiredDataForConnectionToExternalApps(null);

    // Assert
    Assert.That(result, Is.False);
  }
}
