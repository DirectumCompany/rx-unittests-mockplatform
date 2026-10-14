using Sungero.Parties;
using UnitTests.Company.Managers;

namespace UnitTests.Company.ModuleServerFunctions
{
  public class GetEmployeesByPeople
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
    [Description("Список персон не задан — возвращается пустой список.")]
    public void GetEmployeesByPeople_NullList_ReturnsEmpty()
    {
      // Arrange

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(null);

      // Assert
      Assert.That(result, Is.Empty, "Для отсутствующего списка персон результат должен быть пустым");
    }

    [Test]
    [Description("Передан пустой список персон — возвращается пустой список.")]
    public void GetEmployeesByPeople_EmptyList_ReturnsEmpty()
    {
      // Arrange

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>());

      // Assert
      Assert.That(result, Is.Empty, "Для пустого списка персон результат должен быть пустым");
    }

    [Test]
    [Description("У персон есть действующие сотрудники — возвращаются все сотрудники.")]
    public void GetEmployeesByPeople_PeopleWithActiveEmployees_ReturnsAll()
    {
      // Arrange
      var personFirst = EmployeeManager.CreatePerson(DefaultPersonId);
      var employeeFirst = EmployeeManager.CreateEmployee(DefaultEmployeeId, personFirst, Sungero.Company.Employee.Status.Active);

      var personSecond = EmployeeManager.CreatePerson(OtherPersonId);
      var employeeSecond = EmployeeManager.CreateEmployee(OtherEmployeeId, personSecond, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>() { personFirst, personSecond });

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Has.Count.EqualTo(2), "Должны вернуться сотрудники обеих персон");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeFirst.Id), "В результате должен быть сотрудник первой персоны");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeSecond.Id), "В результате должен быть сотрудник второй персоны");
      });
    }

    [Test]
    [Description("Закрытый сотрудник персоны исключается.")]
    public void GetEmployeesByPeople_ClosedEmployee_Excluded()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var activeEmployee = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);
      EmployeeManager.CreateEmployee(OtherEmployeeId, person, Sungero.Company.Employee.Status.Closed);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>() { person });

      // Assert
      Assert.That(result, Has.Count.EqualTo(1), "Закрытый сотрудник не должен попадать в результат");
      Assert.That(result.Single().Id, Is.EqualTo(activeEmployee.Id), "Должен вернуться только действующий сотрудник");
    }

    [Test]
    [Description("У одной персоны несколько действующих сотрудников — возвращаются все.")]
    public void GetEmployeesByPeople_MultipleEmployeesForSinglePerson_ReturnsAll()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var employeeFirst = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);
      var employeeSecond = EmployeeManager.CreateEmployee(OtherEmployeeId, person, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>() { person });

      // Assert
      Assert.Multiple(() =>
      {
        Assert.That(result, Has.Count.EqualTo(2), "Должны вернуться все действующие сотрудники персоны");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeFirst.Id), "В результате должен быть первый сотрудник персоны");
        Assert.That(result.Select(e => e.Id), Does.Contain(employeeSecond.Id), "В результате должен быть второй сотрудник персоны");
      });
    }

    [Test]
    [Description("Персона повторяется в списке — сотрудник не дублируется.")]
    public void GetEmployeesByPeople_DuplicatePersonInList_NoDuplicateEmployees()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var employee = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>() { person, person });

      // Assert
      Assert.That(result, Has.Count.EqualTo(1), "Повтор персоны не должен приводить к дублю сотрудника");
      Assert.That(result.Single().Id, Is.EqualTo(employee.Id), "Должен вернуться сотрудник этой персоны");
    }

    [Test]
    [Description("Null-элементы в списке персон игнорируются.")]
    public void GetEmployeesByPeople_NullElementInList_Ignored()
    {
      // Arrange
      var person = EmployeeManager.CreatePerson(DefaultPersonId);
      var employee = EmployeeManager.CreateEmployee(DefaultEmployeeId, person, Sungero.Company.Employee.Status.Active);

      // Act
      var result = new Sungero.Company.Server.ModuleFunctions().GetEmployeesByPeople(new List<IPerson>() { null, person });

      // Assert
      Assert.That(result, Has.Count.EqualTo(1), "Null-элемент не должен влиять на результат");
      Assert.That(result.Single().Id, Is.EqualTo(employee.Id), "Должен вернуться сотрудник заданной персоны");
    }
  }
}
