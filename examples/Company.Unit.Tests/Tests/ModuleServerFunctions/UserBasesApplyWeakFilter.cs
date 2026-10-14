using Moq;
using Sungero.Company;
using Sungero.Company.FolderFilterState;
using UnitTests.Company.Managers;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Тесты метода UserBasesApplyWeakFilter.
  /// </summary>
  public class UserBasesApplyWeakFilter
  {
    [Test]
    [Description("Задан фильтр по наличию телефона — возвращаются только сотрудники с указанным телефоном или только без него.")]
    public void UserBasesApplyWeakFilter_ByPhoneIsSet_ReturnsMatchingEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var firstEmployee = EmployeeManager.CreateActiveEmployeeWithPhone(true);
      var secondEmployee = EmployeeManager.CreateActiveEmployeeWithPhone(false);

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee }.AsQueryable();

      // Подготовка фильтров.
      var phoneIsSetFilter = Mock.Of<IUsersConnectionFilterState>(f => f.PhoneIsSet);
      var phoneIsNotSetFilter = Mock.Of<IUsersConnectionFilterState>(f => f.PhoneIsNotSet);

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      var employeesWithPhones = functions.UserBasesApplyWeakFilter(employees, phoneIsSetFilter);
      var employeesWithoutPhones = functions.UserBasesApplyWeakFilter(employees, phoneIsNotSetFilter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWithPhones, Has.Member(firstEmployee));
        Assert.That(employeesWithPhones, Has.No.Member(secondEmployee));
        Assert.That(employeesWithoutPhones, Has.Member(secondEmployee));
        Assert.That(employeesWithoutPhones, Has.No.Member(firstEmployee));
      });
    }

    [Test]
    [Description("Задан фильтр по состоянию подключения к Omni — возвращаются только сотрудники с соответствующим состоянием.")]
    public void UserBasesApplyWeakFilter_ByOmniConnectionState_ReturnsMatchingEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var firstEmployee = EmployeeManager.CreateActiveEmployeeWithOmniConnectionState(Sungero.Company.UserBase.OmniConnectionState.Connected);
      var secondEmployee = EmployeeManager.CreateActiveEmployee();
      var thirdEmployee = EmployeeManager.CreateActiveEmployeeWithOmniConnectionState(Sungero.Company.UserBase.OmniConnectionState.Disconnected);

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee, thirdEmployee }.AsQueryable();

      // Подготовка фильтров.
      var connectedToOmniFilter = Mock.Of<IUsersConnectionFilterState>(f => f.ConnectedToOmni);
      var notConnectedToOmniFilter = Mock.Of<IUsersConnectionFilterState>(f => f.NotConnectedToOmni);

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      var employeesWithOmniConnection = functions.UserBasesApplyWeakFilter(employees, connectedToOmniFilter);
      var employeesWithoutOmniConnection = functions.UserBasesApplyWeakFilter(employees, notConnectedToOmniFilter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWithOmniConnection, Has.Member(firstEmployee));
        Assert.That(employeesWithOmniConnection, Has.No.Member(secondEmployee));
        Assert.That(employeesWithOmniConnection, Has.No.Member(thirdEmployee));
        Assert.That(employeesWithoutOmniConnection, Has.Member(secondEmployee));
        Assert.That(employeesWithoutOmniConnection, Has.Member(thirdEmployee));
        Assert.That(employeesWithoutOmniConnection, Has.No.Member(firstEmployee));
      });
    }
  }
}
