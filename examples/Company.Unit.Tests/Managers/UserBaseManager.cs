using MockPlatform.Extensions;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.Parties;

namespace UnitTests.Company.Managers;

public class UserBaseManager
{
  public static IUserBase CreateUserBase(int id, IPerson person, Sungero.Core.Enumeration status, ILogin login = null, string email = null)
  {
    var user = UserBases.Create();
    user.SetId(id);
    user.Person = person;
    user.Status = status;
    user.Login = login;
    user.Email = email;
    return user;
  }
}
