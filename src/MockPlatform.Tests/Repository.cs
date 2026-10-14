using MockPlatform.Repositories;
using Sungero.Company;
using Sungero.Domain.Shared;
using Sungero.Parties;

namespace MockPlatform.Tests;

public class Repository
{
  [Test]
  public void CreatePerson()
  {
    var person = Sungero.Parties.People.Create();
    person.ShortName = "Иванов И.А.";
    Assert.That(person.ShortName, Is.EqualTo("Иванов И.А."));
  }


  [Test]
  public void CreateEmployee()
  {
    var employee = Employees.Create();
    ((IExtendedEntity)employee).Id = 555;
    Assert.That(employee.Id, Is.EqualTo(555));
  }

  /// <summary>
  /// Проверить, что все репозитории пустые после глобальной очистки.
  /// </summary>
  [Test]
  public void AllRepositoriesShouldBeEmptyAfterClearAll()
  {
    // Создание тестовых данных в репозитории People
    var person1 = Sungero.Parties.People.Create();
    person1.ShortName = "Иванов И.И.";

    var person2 = Sungero.Parties.People.Create();
    person2.ShortName = "Петров П.П.";

    // Создание тестовых данных в репозитории Employees
    var employee1 = Employees.Create();
    employee1.Person = person1;

    var employee2 = Employees.Create();
    employee2.Person = person2;

    // Проверка, что данные добавлены в оба репозитория
    var allPeopleBeforeClear = Sungero.Parties.People.GetAll();
    var allEmployeesBeforeClear = Employees.GetAll();
    Assert.Multiple(() =>
    {
      Assert.That(allPeopleBeforeClear.Count(), Is.EqualTo(2), "В репозитории People должно быть 2 записи перед очисткой");
      Assert.That(allEmployeesBeforeClear.Count(), Is.EqualTo(2), "В репозитории Employees должно быть 2 записи перед очисткой");
    });

    // Глобальная очистка всех репозиториев
    MockRepositories.ClearAll();

    // Проверка, что оба репозитория пусты после глобальной очистки
    var allPeopleAfterClear = Sungero.Parties.People.GetAll();
    var allEmployeesAfterClear = Employees.GetAll();
    Assert.Multiple(() =>
    {
      Assert.That(allPeopleAfterClear.Count(), Is.EqualTo(0), "Репозиторий People должен быть пустым после глобальной очистки");
      Assert.That(allEmployeesAfterClear.Count(), Is.EqualTo(0), "Репозиторий Employees должен быть пустым после глобальной очистки");
      Assert.That(Sungero.Parties.People.Get(1), Is.Null, "Person с Id=1 не должен существовать после глобальной очистки");
      Assert.That(Sungero.Parties.People.Get(2), Is.Null, "Person с Id=2 не должен существовать после глобальной очистки");
      Assert.That(Employees.Get(1), Is.Null, "Employee с Id=1 не должен существовать после глобальной очистки");
      Assert.That(Employees.Get(2), Is.Null, "Employee с Id=2 не должен существовать после глобальной очистки");
    });
  }

  /// <summary>
  /// Проверить, что сущность, созданная в производном репозитории, видна
  /// через GetAll() и Get(id) базовых репозиториев.
  /// </summary>
  [Test]
  public void CreateInDerivedRepositoryShouldBeVisibleInBaseRepositories()
  {
    var company = Companies.Create();

    var companyFromBaseRepo = CompanyBases.GetAll().FirstOrDefault();
    var companyFromCounterpartiesRepo = Counterparties.GetAll().FirstOrDefault();

    Assert.Multiple(() =>
    {
      Assert.That(companyFromBaseRepo, Is.SameAs(company), "Company должна быть видна в CompanyBases.GetAll()");
      Assert.That(companyFromCounterpartiesRepo, Is.SameAs(company), "Company должна быть видна в Counterparties.GetAll()");
      Assert.That(CompanyBases.Get(company.Id), Is.SameAs(company), "Company должна находиться по Id в CompanyBases");
      Assert.That(Counterparties.Get(company.Id), Is.SameAs(company), "Company должна находиться по Id в Counterparties");
    });
  }

  /// <summary>
  /// Проверить, что сущность базового типа НЕ видна через производные репозитории.
  /// </summary>
  [Test]
  public void CreateInBaseRepositoryShouldNotBeVisibleInDerivedRepositories()
  {
    var person = People.Create();
    person.ShortName = "Иванов И.И.";

    var allCounterparties = Counterparties.GetAll().ToList();
    var allCompanyBases = CompanyBases.GetAll().ToList();

    Assert.Multiple(() =>
    {
      Assert.That(allCounterparties, Has.Count.EqualTo(1), "Person должна быть видна как Counterparty");
      Assert.That(allCompanyBases, Is.Empty, "Person не должна быть видна в CompanyBases");
    });
  }
}
