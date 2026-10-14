using MockPlatform.Extensions;

namespace UnitTests.Company.ModuleServerFunctions
{
  public class GetEmployeeInfo
  {
    [Test]
    [Description("Сотрудников нет — возвращается пустая строка.")]
    public void GetEmployeeInfo_NoEmployees_ReturnsEmpty()
    {
      // Arrange

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetEmployeeInfo(105);

      // Assert
      Assert.That(result, Is.Empty);
    }

    [Test]
    [Description("Сотрудник с указанным ИД есть — возвращается краткое имя сотрудника с ИД.")]
    public void GetEmployeeInfo_EmployeeExists_ReturnsShortNameWithId()
    {
      // Arrange
      // Создание персоны
      var person = Sungero.Parties.People.Create();
      person.SetId(6);
      person.ShortName = "Иванов И.И.";

      // Создание сотрудника
      var employee = Sungero.Company.Employees.Create();
      employee.SetId(6);
      employee.Person = person;

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetEmployeeInfo(6);

      // Assert
      Assert.That(result, Is.EqualTo("Иванов И.И. (Id 6)"));
    }

    [Test]
    [Description("Сотрудника с указанным ИД нет — возвращается пустая строка.")]
    public void GetEmployeeInfo_EmployeeNotExists_ReturnsEmpty()
    {
      // Arrange
      // Создание персоны
      var person = Sungero.Parties.People.Create();
      person.SetId(6);
      person.ShortName = "Иванов И.И.";

      // Создание сотрудника
      var employee = Sungero.Company.Employees.Create();
      employee.SetId(6);
      employee.Person = person;

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetEmployeeInfo(111);

      // Assert
      Assert.That(result, Is.Empty);
    }
  }
}