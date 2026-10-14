using MockPlatform.Extensions;
using Moq;
using Sungero.Company;
using Sungero.Core;
using Sungero.Parties;

namespace UnitTests.Company.Managers;

/// <summary>
/// Вспомогательные методы для тестов получения сотрудников из персон
/// </summary>
public static class EmployeeManager
{
  /// <summary>
  /// Создать персону.
  /// </summary>
  /// <param name="id">Идентификатор персоны.</param>
  /// <returns>Персона.</returns>
  public static IPerson CreatePerson(int id)
  {
    var person = People.Create();
    person.SetId(id);
    return person;
  }

  /// <summary>
  /// Создать сотрудника, связанного с персоной, с заданным статусом.
  /// </summary>
  /// <param name="id">Идентификатор сотрудника.</param>
  /// <param name="person">Персона сотрудника.</param>
  /// <param name="status">Статус записи сотрудника.</param>
  /// <returns>Сотрудник.</returns>
  public static IEmployee CreateEmployee(int id, IPerson person, Enumeration status)
  {
    var employee = Employees.Create();
    employee.SetId(id);
    employee.Person = person;
    employee.Status = status;
    return employee;
  }

  /// <summary>
  /// Создать сотрудника с персоной с заданными ФИО.
  /// </summary>
  /// <param name="id">Идентификатор персоны и сотрудника.</param>
  /// <param name="lastName">Фамилия.</param>
  /// <param name="firstName">Имя.</param>
  /// <param name="middleName">Отчество.</param>
  /// <param name="status">Статус записи сотрудника. По умолчанию — действующая.</param>
  /// <returns>Сотрудник с полем Name в формате «Фамилия Имя Отчество».</returns>
  public static IEmployee CreateEmployee(int id, string lastName, string firstName, string? middleName = null, Enumeration? status = null)
  {
    var person = CreatePerson(id);
    person.LastName = lastName;
    person.FirstName = firstName;
    person.MiddleName = middleName;

    var employee = CreateEmployee(id, person, status ?? Sungero.CoreEntities.DatabookEntry.Status.Active);
    employee.Name = string.Join(" ", new[] { lastName, firstName, middleName }.Where(part => !string.IsNullOrEmpty(part)));
    return employee;
  }

  /// <summary>
  /// Создать действующего сотрудника.
  /// </summary>
  /// <returns>Сотрудник.</returns>
  public static IEmployee CreateActiveEmployee()
  {
    var employee = Employees.Create();
    employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
    return employee;
  }

  /// <summary>
  /// Создать действующего сотрудника с заданным признаком наличия личного телефона.
  /// </summary>
  /// <param name="isPersonalPhoneSet">Признак наличия личного телефона.</param>
  /// <returns>Сотрудник.</returns>
  public static IEmployee CreateActiveEmployeeWithPhone(bool isPersonalPhoneSet)
  {
    var employee = CreateActiveEmployee();
    // IsPersonalPhoneSet - readonly-свойство, поэтому мокаем его.
    Mock.Get(employee).Setup(x => x.IsPersonalPhoneSet).Returns(isPersonalPhoneSet);
    return employee;
  }

  /// <summary>
  /// Создать действующего сотрудника с заданным состоянием подключения к Omni.
  /// </summary>
  /// <param name="omniConnectionState">Состояние подключения к Omni.</param>
  /// <returns>Сотрудник.</returns>
  public static IEmployee CreateActiveEmployeeWithOmniConnectionState(Enumeration omniConnectionState)
  {
    var employee = CreateActiveEmployee();
    employee.OmniConnectionState = omniConnectionState;
    return employee;
  }
}
