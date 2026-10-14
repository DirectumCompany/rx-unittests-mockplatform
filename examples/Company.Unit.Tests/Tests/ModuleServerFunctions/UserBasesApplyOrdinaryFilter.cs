using Moq;
using Sungero.Company;
using Sungero.Company.FolderFilterState;
using Sungero.CoreEntities;
using UnitTests.Company.Managers;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Тесты метода UserBasesApplyOrdinaryFilter.
  /// </summary>
  public class UserBasesApplyOrdinaryFilter
  {
    [Test]
    [Description("Задан фильтр по работе в веб-клиенте — возвращаются только сотрудники с соответствующим значением свойства \"Работает в веб-клиенте\".")]
    public void UserBasesApplyOrdinaryFilter_ByWorkInWebClient_ReturnsMatchingEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var firstEmployee = EmployeeManager.CreateActiveEmployee();
      firstEmployee.IsWorkingInWebClient = true;

      var secondEmployee = EmployeeManager.CreateActiveEmployee();
      secondEmployee.IsWorkingInWebClient = false;

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee }.AsQueryable();

      // Подготовка фильтров.
      var enableWorkInWebClientFilter = Mock.Of<IUsersConnectionFilterState>(f => f.EnableWorkInWebClient);
      var disableWorkInWebClientFilter = Mock.Of<IUsersConnectionFilterState>(f => f.DisableWorkInWebClient);

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      var employeesWhoEnableWorkInWebClient = functions.UserBasesApplyOrdinaryFilter(employees, enableWorkInWebClientFilter);
      var employeesWhoDisableWorkInWebClient = functions.UserBasesApplyOrdinaryFilter(employees, disableWorkInWebClientFilter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWhoEnableWorkInWebClient, Has.Member(firstEmployee));
        Assert.That(employeesWhoEnableWorkInWebClient, Has.No.Member(secondEmployee));
        Assert.That(employeesWhoDisableWorkInWebClient, Has.Member(secondEmployee));
        Assert.That(employeesWhoDisableWorkInWebClient, Has.No.Member(firstEmployee));
      });
    }

    [Test]
    [Description("Задан фильтр по наличию учетной записи — возвращаются только сотрудники с учетной записью или только без нее.")]
    public void UserBasesApplyOrdinaryFilter_ByLoginIsSet_ReturnsMatchingEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var login = Logins.Create();
      var firstEmployee = EmployeeManager.CreateActiveEmployee();
      firstEmployee.Login = login;

      var secondEmployee = EmployeeManager.CreateActiveEmployee();

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee }.AsQueryable();

      // Подготовка фильтров.
      var loginIsSetFilter = Mock.Of<IUsersConnectionFilterState>(f => f.LoginIsSet);
      var loginIsNotSetFilter = Mock.Of<IUsersConnectionFilterState>(f => f.LoginIsNotSet);

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      var employeesWithLogins = functions.UserBasesApplyOrdinaryFilter(employees, loginIsSetFilter);
      var employeesWithoutLogins = functions.UserBasesApplyOrdinaryFilter(employees, loginIsNotSetFilter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWithLogins, Has.Member(firstEmployee));
        Assert.That(employeesWithLogins, Has.No.Member(secondEmployee));
        Assert.That(employeesWithoutLogins, Has.Member(secondEmployee));
        Assert.That(employeesWithoutLogins, Has.No.Member(firstEmployee));
      });
    }
  }
}
