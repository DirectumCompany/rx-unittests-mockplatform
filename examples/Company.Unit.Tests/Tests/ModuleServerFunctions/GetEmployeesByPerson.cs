using UnitTests.Company.Managers;

namespace UnitTests.Company.ModuleServerFunctions
{
  public class GetEmployeesByPerson
  {
    /// <summary>
    /// Идентификатор персоны по умолчанию.
    /// </summary>
    private const int DefaultPersonId = 1;

    /// <summary>
    /// Идентификатор дополнительной персоны.
    /// </summary>
    private const int OtherPersonId = 2;

    /// <summary>
    /// Идентификатор сотрудника по умолчанию.
    /// </summary>
    private const int DefaultEmployeeId = 1;

    /// <summary>
    /// Идентификатор дополнительного сотрудника.
    /// </summary>
    private const int OtherEmployeeId = 2;

    [Test]
    [Description("Персона не задана — возвращается пустой список.")]
    public void GetEmployeesByPerson_NullPerson_ReturnsEmpty()
    {
      // Arrange

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(null);

      // Assert
      Assert.That(result, Is.Empty, "Для отсутствующей персоны список сотрудников должен быть пустым");
    }

    [Test]
    [Description("У персоны есть действующий сотрудник — возвращается этот сотрудник.")]
    public void GetEmployeesByPerson_PersonWithActiveEmployee_ReturnsEmployee()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var employee = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(person);

      // Assert
      Assert.That(result, Has.Count.EqualTo(1), "Для персоны с действующим сотрудником должен вернуться один сотрудник");
      Assert.That(result.Single().Id, Is.EqualTo(employee.Id), "Должен вернуться сотрудник этой персоны");
    }

    [Test]
    [Description("У персоны только закрытый сотрудник — возвращается пустой список.")]
    public void GetEmployeesByPerson_OnlyClosedEmployee_ReturnsEmpty()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Closed);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(person);

      // Assert
      Assert.That(result, Is.Empty, "Закрытый сотрудник не должен попадать в результат");
    }

    [Test]
    [Description("С персоной не связан ни один сотрудник — возвращается пустой список.")]
    public void GetEmployeesByPerson_PersonWithoutEmployee_ReturnsEmpty()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(person);

      // Assert
      Assert.That(result, Is.Empty, "Для персоны без сотрудников список должен быть пустым");
    }

    [Test]
    [Description("У персоны несколько действующих сотрудников — возвращаются все.")]
    public void GetEmployeesByPerson_MultipleActiveEmployees_ReturnsAll()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var employeeFirst = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);
      var employeeSecond = EmployeeManager.CreateEmployee(OtherEmployeeId, person, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(person);

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Has.Count.EqualTo(2), "Должны вернуться все действующие сотрудники персоны");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeFirst.Id), "В результате должен быть первый сотрудник персоны");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeSecond.Id), "В результате должен быть второй сотрудник персоны");
      });
    }

    [Test]
    [Description("Есть несколько персон — возвращаются сотрудники только запрошенной персоны.")]
    public void GetEmployeesByPerson_SeveralPersons_ReturnsEmployeesOfRequestedPersonOnly()
    {
      // Arrange
      var targetPerson = EmployeeManager.CreatePerson(DefaultPersonId);
      var targetEmployee = EmployeeManager.CreateEmployee(DefaultEmployeeId, targetPerson, Sungero.Company.Employee.Status.Active);

      var otherPerson = EmployeeManager.CreatePerson(OtherPersonId);
      EmployeeManager.CreateEmployee(OtherEmployeeId, otherPerson, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPerson(targetPerson);

      // Assert
      Assert.That(result, Has.Count.EqualTo(1), "Должны вернуться сотрудники только запрошенной персоны");
      Assert.That(result.Single().Id, Is.EqualTo(targetEmployee.Id), "Не должен возвращаться сотрудник другой персоны");
    }
  }
}
