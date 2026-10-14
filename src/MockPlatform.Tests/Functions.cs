using MockPlatform.Extensions;
using MockPlatform.Repositories;

namespace MockPlatform.Tests;

public class Functions
{
  [Test]
  public void ExecuteModuleRemoteFunction()
  {
    Assert.That(Sungero.Docflow.PublicFunctions.Module.Remote.IsModuleAvailableByLicense(new Guid()), Is.True);
  }

  [Test]
  public void ExecuteModulePublicFunction()
  {
    var certificate = Sungero.CoreEntities.Certificates.Create();
    var parameterKey = "TestKey";
    var parameterValue = "TestValue";

    Sungero.Docflow.PublicFunctions.Module.AddOrUpdateCertificateParameter(certificate, parameterKey, parameterValue);

    Assert.That(certificate.Parameters, Is.Not.Null);
    Assert.That(certificate.Parameters, Has.Count.EqualTo(1));
    Assert.Multiple(() =>
    {
      Assert.That(certificate.Parameters.First().Key, Is.EqualTo(parameterKey));
      Assert.That(certificate.Parameters.First().Value, Is.EqualTo(parameterValue));
    });
  }

  [Test]
  public void ExecuteEntitySharedFunction()
  {
    // Arrange
    var businessUnit = Sungero.Company.BusinessUnits.Create();
    businessUnit.Name = "Test Business Unit";

    var exchangeService = Sungero.ExchangeCore.ExchangeServices.Create();
    exchangeService.Name = "Test Exchange Service";

    var box = Sungero.ExchangeCore.BusinessUnitBoxes.Create();
    box.BusinessUnit = businessUnit;
    box.ExchangeService = exchangeService;

    // Act
    box.GetSharedFunctions<Sungero.ExchangeCore.Shared.BusinessUnitBoxFunctions>().SetBusinessUnitBoxName();

    // Assert
    var expectedName = string.Format("{0} ({1})", businessUnit, exchangeService);
    Assert.That(box.Name, Is.EqualTo(expectedName));
  }

  [Test]
  public void ExecuteModuleSharedFunction()
  {
    // Arrange
    var validGuid = "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d";
    var functions = new Sungero.ExchangeCore.Shared.ModuleFunctions();

    // Act
    var result = functions.CheckGuid(validGuid);

    // Assert
    Assert.That(result, Is.True);
  }


  [Test]
  public void ExecuteEntityPublicStaticFunction()
  {
    // Arrange
    var tin = "12";
    var trrc = "23";

    // Act
    var result = Sungero.Company.PublicFunctions.BusinessUnit.GetBusinessUnits(tin, trrc);

    // Assert
    Assert.That(result, Is.Not.Null);
  }

  [Test]
  public void ExecuteEntityPublicStaticFunctionWithEntityAsFirstArg()
  {
    // Arrange
    var employee = Sungero.Company.Employees.Create();

    // Act
    var result = Sungero.Company.PublicFunctions.Employee.GetJobTitle(employee, Sungero.Core.DeclensionCase.Nominative);

    // Assert
    Assert.That(result, Is.Not.Null);
  }


  [Test]
  public void ExecuteEntityPublicInstanceFunction()
  {
    // Arrange
    var businessUnit = Sungero.Company.BusinessUnits.Create();
    businessUnit.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;

    // Act
    var result = Sungero.Company.PublicFunctions.BusinessUnit.GetIndexingJson(businessUnit);

    // Assert
    Assert.That(result, Is.Not.Null);
  }

  [Test]
  public void ExecuteModuleServerInstanceFunction()
  {
    // Arrange
    var userBase = Sungero.Company.UserBases.Create();

    // Act
    var result = new Sungero.Company.Server.ModuleFunctions().GetUserBasesByIds("1");

    // Assert
    Assert.That(result, Has.Count.EqualTo(1));
  }

  [Test]
  public void ExecuteModuleClientStaticFunction()
  {
    // Act
    Sungero.Company.Client.ModuleFunctions.GetAllActiveNoSystemGroups();
  }

  [Test]
  public void ExecuteModuleClientStaticPublicFunction()
  {
    // Act
    Sungero.Company.PublicFunctions.Module.GetAllActiveNoSystemGroups();
  }

  [Test]
  public void ExecuteEntityClientInstanceFunction()
  {
    // Arrange
    var doc = Sungero.Docflow.OfficialDocuments.Create();

    // Act
    var result = doc.GetClientFunctions<Sungero.Docflow.Client.OfficialDocumentFunctions>()
      .GetTextToMarkDocumentAsObsolete();

    // Assert
    Assert.That(result, Is.EqualTo(Sungero.Docflow.OfficialDocuments.Resources.MarkDocumentAsObsolete.ToString()));
  }

  [Test]
  public void ExecuteEntityClientStaticFunction()
  {
    // Act
    var result = Sungero.Docflow.Client.OfficialDocumentFunctions.TryParseColorCode(string.Empty);

    // Assert
    Assert.That(result, Is.EqualTo(Sungero.Core.Colors.Empty));
  }

  [Test]
  public void ExecuteEntityClientInstancePublicFunction()
  {
    // Arrange
    var doc = Sungero.Docflow.OfficialDocuments.Create();

    // Act
    var result = Sungero.Docflow.PublicFunctions.OfficialDocument.GetTextToMarkDocumentAsObsolete(doc);

    // Assert
    Assert.That(result, Is.EqualTo(Sungero.Docflow.OfficialDocuments.Resources.MarkDocumentAsObsolete.ToString()));
  }

  [Test]
  public void ExecuteEntityClientStaticPublicFunction()
  {
    // Arrange
    var doc = Sungero.Docflow.OfficialDocuments.Create();
    var attachment = Sungero.Docflow.ExchangeDocuments.Create();

    // Act
    var result = Sungero.Docflow.PublicFunctions.OfficialDocument.NeedToAttachDocument(attachment, doc);

    // Assert
    Assert.That(result, Is.False);
  }
}
