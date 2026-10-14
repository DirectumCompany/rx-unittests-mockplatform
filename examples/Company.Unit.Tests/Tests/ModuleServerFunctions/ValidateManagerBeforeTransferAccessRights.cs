using MockPlatform.Extensions;
using Sungero.Core;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Юнит тесты для функции ValidateManagerBeforeTransferAccessRights.
  /// Функция проверяет руководителя перед передачей прав по замещению.
  /// </summary>
  public class ValidateManagerBeforeTransferAccessRights
  {
    [Test]
    [Description("Запись руководителя действующая — исключение не выбрасывается.")]
    public void ValidateManagerBeforeTransferAccessRights_ManagerActive_NoException()
    {
      // Arrange
      // Создание персоны
      var person = Sungero.Parties.People.Create();
      person.SetId(6);
      person.ShortName = "Иванов И.И.";

      // Создание сотрудника с активным статусом
      var employee = Sungero.Company.Employees.Create();
      employee.SetId(6);
      employee.Person = person;
      employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      TestDelegate validate = () => functions.ValidateManagerBeforeTransferAccessRights(6);

      // Assert
      Assert.That(validate, Throws.Nothing);
    }

    [Test]
    [Description("Руководителя с указанным ИД нет — выбрасывается исключение о том, что руководитель не найден.")]
    public void ValidateManagerBeforeTransferAccessRights_ManagerNotFound_ThrowsException()
    {
      // Arrange
      // Репозиторий пустой - сотрудник не найден
      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      TestDelegate validate = () => functions.ValidateManagerBeforeTransferAccessRights(99);

      // Assert
      var exception = Assert.Throws<AppliedCodeException>(validate);
      Assert.That(exception.Message, Is.EqualTo("Manager with ID 99 not found."));
    }

    [Test]
    [Description("Запись руководителя закрыта — выбрасывается исключение о закрытой записи.")]
    public void ValidateManagerBeforeTransferAccessRights_ManagerClosed_ThrowsException()
    {
      // Arrange
      // Создание персоны
      var person = Sungero.Parties.People.Create();
      person.SetId(10);
      person.ShortName = "Петров П.П.";

      // Создание сотрудника с закрытым статусом
      var employee = Sungero.Company.Employees.Create();
      employee.SetId(10);
      employee.Person = person;
      employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;

      var functions = new Sungero.Company.Server.ModuleFunctions();

      // Act
      TestDelegate validate = () => functions.ValidateManagerBeforeTransferAccessRights(10);

      // Assert
      var exception = Assert.Throws<AppliedCodeException>(validate);
      Assert.That(exception.Message, Is.EqualTo("The manager's databook record with ID 10 is closed. Select a manager whose record is active."));
    }

  }
}
