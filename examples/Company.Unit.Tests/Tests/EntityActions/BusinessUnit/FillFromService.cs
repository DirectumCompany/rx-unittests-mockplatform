using MockPlatform.Core;
using Sungero.Company;
using Sungero.Company.Client;
using Sungero.Domain.Client;
using Sungero.Domain.Shared;
using Sungero.Parties.Structures.Module;
using PartiesClientFunctions = Sungero.Parties.Client.ModuleClientPublicFunctions;

namespace UnitTests.Company.EntityActions.BusinessUnit;

public class FillFromService
{
  [Test]
  [Description("Организация не найдена в сервисе — реквизиты организации не изменяются.")]
  public void FillFromService_CompanyNotFoundInService()
  {
    // Arrange.
    using var searchStub = MockSearchCompanyFromService(null);

    var businessUnit = CreateBusinessUnit("Исходное имя", "0000000000", "0000000000000");

    // Act.
    ExecuteAction(businessUnit);

    // Assert.
    Assert.Multiple(() =>
    {
      Assert.That(businessUnit.Name, Is.EqualTo("Исходное имя"), "Реквизиты организации не должны измениться.");
      Assert.That(businessUnit.TIN, Is.EqualTo("0000000000"));
      Assert.That(businessUnit.PSRN, Is.EqualTo("0000000000000"));
    });
  }

  [Test]
  [Description("Организация найдена в сервисе — реквизиты организации заполняются данными из сервиса.")]
  public void FillFromService_CompanyFoundInService()
  {
    // Arrange.
    var foundCompany = CreateFoundCompany();
    using var searchStub = MockSearchCompanyFromService(foundCompany);

    var businessUnit = CreateBusinessUnit("Исходное имя", "0000000000", "0000000000000");

    // Act.
    ExecuteAction(businessUnit);

    // Assert.
    Assert.Multiple(() =>
    {
      Assert.That(businessUnit.Name, Is.EqualTo(foundCompany.Name));
      Assert.That(businessUnit.LegalName, Is.EqualTo(foundCompany.LegalName));
      Assert.That(businessUnit.TIN, Is.EqualTo(foundCompany.TIN));
      Assert.That(businessUnit.PSRN, Is.EqualTo(foundCompany.PSRN));
      Assert.That(businessUnit.TRRC, Is.EqualTo(foundCompany.TRRC));
    });
  }

  #region Helper методы

  /// <summary>
  /// Выполнить действие "Заполнить по данным сервиса".
  /// </summary>
  private static void ExecuteAction(IBusinessUnit businessUnit)
  {
    var args = new ExecuteActionArgs(FormType.Card, businessUnit, new ActionInfo(null));
    var actions = new BusinessUnitActions();
    actions.SetEntity(businessUnit);
    actions.FillFromService(args);
  }

  /// <summary>
  /// Создать организацию с исходными реквизитами.
  /// </summary>
  private static IBusinessUnit CreateBusinessUnit(string name, string tin, string psrn)
  {
    var businessUnit = BusinessUnits.Create();
    businessUnit.Name = name;
    businessUnit.TIN = tin;
    businessUnit.PSRN = psrn;
    return businessUnit;
  }

  /// <summary>
  /// Создать реквизиты организации, найденной в сервисе.
  /// </summary>
  private static IFoundCompany CreateFoundCompany()
  {
    var company = FoundCompany.Create();
    company.Name = "Организация из сервиса";
    company.LegalName = "ООО \"Организация из сервиса\"";
    company.TIN = "1234567890";
    company.PSRN = "1234567890123";
    company.TRRC = "123456789";
    return company;
  }

  /// <summary>
  /// Замокать Parties.PublicFunctions.Module.SearchCompanyFromService.
  /// </summary>
  private static IDisposable MockSearchCompanyFromService(IFoundCompany? foundCompany)
    => MockFunctionStubs.Setup<PartiesClientFunctions>(
      nameof(PartiesClientFunctions.SearchCompanyFromService),
      (string _, string _, string _, ExecuteActionArgs _) => foundCompany);

  #endregion
}
