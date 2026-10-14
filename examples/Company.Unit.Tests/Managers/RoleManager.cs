using Sungero.CoreEntities;

namespace UnitTests.Company.Managers;

public class RoleManager
{
  public static IRole CreateNoninteractiveUsersRole()
  {
    var role = Roles.Create();
    role.Sid = Guid.Parse("E473E9EC-DF3F-4EE7-BDC4-2B8A7782FAAB");
    return role;
  }
}
