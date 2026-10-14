using Moq;
using Sungero.Company;
using Sungero.Company.FolderFilterState;
using UnitTests.Company.Managers;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Тесты метода UserBasesApplyStrongFilter.
  /// </summary>
  public class UserBasesApplyStrongFilter
  {
    [Test]
    [Description("Задано подразделение — возвращаются только сотрудники этого подразделения.")]
    public void UserBasesApplyStrongFilter_ByDepartment_ReturnsDepartmentEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var firstDepartment = Departments.Create();
      var firstEmployee = EmployeeManager.CreateActiveEmployee();
      SetEmployeeDepartment(firstEmployee, firstDepartment);

      var secondDepartment = Departments.Create();
      var secondEmployee = EmployeeManager.CreateActiveEmployee();
      SetEmployeeDepartment(secondEmployee, secondDepartment);

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee }.AsQueryable();

      // Подготовка фильтра.
      var filter = Mock.Of<IUsersConnectionFilterState>(f => f.Department == firstDepartment);

      // Act
      var employeesWithFirstDepartments = new Sungero.Company.Server.ModuleFunctions().UserBasesApplyStrongFilter(employees, filter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWithFirstDepartments, Has.Member(firstEmployee));
        Assert.That(employeesWithFirstDepartments, Has.No.Member(secondEmployee));
      });
    }

    [Test]
    [Description("Задана НОР — возвращаются только сотрудники этой НОР.")]
    public void UserBasesApplyStrongFilter_ByBusinessUnit_ReturnsBusinessUnitEmployees()
    {
      // Arrange
      // Создание сотрудников.
      var firstBusinessUnit = BusinessUnits.Create();
      var firstEmployee = EmployeeManager.CreateActiveEmployee();
      firstEmployee.BusinessUnit = firstBusinessUnit;

      var secondBusinessUnit = BusinessUnits.Create();
      var secondEmployee = EmployeeManager.CreateActiveEmployee();
      secondEmployee.BusinessUnit = secondBusinessUnit;

      // Подготовка списка сотрудников для фильтрации.
      var employees = new List<IUserBase> { firstEmployee, secondEmployee }.AsQueryable();

      // Подготовка фильтра.
      var filter = Mock.Of<IUsersConnectionFilterState>(f => f.BusinessUnit == firstBusinessUnit);

      // Act
      var employeesWithFirstBusinessUnit = new Sungero.Company.Server.ModuleFunctions().UserBasesApplyStrongFilter(employees, filter);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(employeesWithFirstBusinessUnit, Has.Member(firstEmployee));
        Assert.That(employeesWithFirstBusinessUnit, Has.No.Member(secondEmployee));
      });
    }

    #region Helper методы

    /// <summary>
    /// Указать подразделение сотрудника и добавить сотрудника в состав подразделения.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="department">Подразделение.</param>
    private void SetEmployeeDepartment(IEmployee employee, IDepartment department)
    {
      employee.Department = department;
      department.RecipientLinks.AddNew().Member = employee;
    }

    #endregion
  }
}
