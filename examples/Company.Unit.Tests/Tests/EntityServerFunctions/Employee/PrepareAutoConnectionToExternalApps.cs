using Moq;
using Sungero.Company;
using Sungero.Company.UserBase;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Parties;

namespace UnitTests.Company.EmployeeServerFunctions;

/// <summary>
/// Юнит тесты для функции PrepareAutoConnectionToExternalApps.
/// </summary>
public class PrepareAutoConnectionToExternalApps
{
  private const string AppsToConnectParamName = Sungero.Company.Constants.Employee.ExternalAppIdsToAutoConnect;

  [Test]
  [Description("Если статус подключения к приложению \"Отключен\", то автоподключение к нему не сработает")]
  public void PrepareAutoConnectionToExternalApps_OmniStatusIsDisconnected_NoNeedToAutoConnect()
  {
    // Arrange
    var omni = CreateOmni(true);
    var employee = Employees.Create();
    employee.OmniConnectionState = OmniConnectionState.Disconnected;

    // Act
    Sungero.Company.PublicFunctions.Employee.PrepareAutoConnectionToExternalApps(employee);

    // Assert
    Assert.That(employee.OmniConnectionState, Is.EqualTo(OmniConnectionState.Disconnected));

    var employeeParams = ((IExtendedEntity)employee).Params;
    Assert.That(employeeParams, Does.Not.ContainKey(AppsToConnectParamName));
  }

  [Test]
  [Description("Если НОР сотрудника включена в список НОР для автоподключения, то автоподключение сработает")]
  public void PrepareAutoConnectionToExternalApps_BusinessUnitIncluded_NeedToAutoConnect()
  {
    // Arrange
    var businessUnit = BusinessUnits.Create();
    var employee = CreateChangedEmployee(businessUnit);
    var omni = CreateOmni(false);
    omni.IncludedForBusinessUnits.AddNew().BusinessUnit = businessUnit;

    // Act
    Sungero.Company.PublicFunctions.Employee.PrepareAutoConnectionToExternalApps(employee);

    // Assert
    Assert.That(employee.OmniConnectionState, Is.EqualTo(OmniConnectionState.Connecting));

    var employeeParams = ((IExtendedEntity)employee).Params;
    Assert.Multiple(() =>
    {
      Assert.That(employeeParams, Does.ContainKey(AppsToConnectParamName));
      Assert.That((IEnumerable<long>)employeeParams[AppsToConnectParamName], Does.Contain(omni.Id));
    });
  }

  [Test]
  [Description("Если НОР, к которой сотрудник не принадлежит включена в список НОР для автоподключения, то автоподключение не сработает")]
  public void PrepareAutoConnectionToExternalApps_OtherBusinessUnitIncluded_NoNeedToAutoConnect()
  {
    // Arrange
    var businessUnit = BusinessUnits.Create();
    var employee = CreateChangedEmployee();
    var omni = CreateOmni(false);
    omni.IncludedForBusinessUnits.AddNew().BusinessUnit = businessUnit;

    // Act
    Sungero.Company.PublicFunctions.Employee.PrepareAutoConnectionToExternalApps(employee);

    // Assert
    Assert.That(employee.OmniConnectionState, Is.Null);

    var employeeParams = ((IExtendedEntity)employee).Params;
    Assert.That(employeeParams, Does.Not.ContainKey(AppsToConnectParamName));
  }

  [Test]
  [Description("Если НОР, к которой сотрудник не принадлежит исключена из списка НОР для автоподключения, то автоподключение сработает")]
  public void PrepareAutoConnectionToExternalApps_OtherBusinessUnitExcluded_NeedToAutoConnect()
  {
    // Arrange
    var businessUnit = BusinessUnits.Create();
    var employee = CreateChangedEmployee();
    var omni = CreateOmni(true);
    omni.ExcludedForBusinessUnits.AddNew().BusinessUnit = businessUnit;

    // Act
    Sungero.Company.PublicFunctions.Employee.PrepareAutoConnectionToExternalApps(employee);

    // Assert
    Assert.That(employee.OmniConnectionState, Is.EqualTo(OmniConnectionState.Connecting));

    var employeeParams = ((IExtendedEntity)employee).Params;
    Assert.Multiple(() =>
    {
      Assert.That(employeeParams, Does.ContainKey(AppsToConnectParamName));
      Assert.That((IEnumerable<long>)employeeParams[AppsToConnectParamName], Does.Contain(omni.Id));
    });
  }

  [Test]
  [Description("Если НОР сотрудника исключена из списка НОР для автоподключения, то автоподключение не сработает")]
  public void PrepareAutoConnectionToExternalApps_BusinessUnitExcluded_NoNeedToAutoConnect()
  {
    // Arrange
    var businessUnit = BusinessUnits.Create();
    var employee = CreateChangedEmployee(businessUnit);
    var omni = CreateOmni(true);
    omni.ExcludedForBusinessUnits.AddNew().BusinessUnit = businessUnit;

    // Act
    Sungero.Company.PublicFunctions.Employee.PrepareAutoConnectionToExternalApps(employee);

    // Assert
    Assert.That(employee.OmniConnectionState, Is.Null);
  }

  private static IExternalApp CreateOmni(bool enabledForAllUsers)
  {
    var omni = ExternalApps.Create();
    omni.Sid = Sungero.Company.Constants.ExternalApp.OmniSid;
    omni.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
    omni.EnabledForAllUsers = enabledForAllUsers;
    return omni;
  }

  private static IEmployee CreateChangedEmployee(IBusinessUnit? businessUnit = null)
  {
    var person = People.Create();
    var mockPersonState = new Mock<IPersonState>();
    mockPersonState.Setup(x => x.Properties.PhoneHash.OriginalValue).Returns(person.PhoneHash);
    Mock.Get(person).Setup(x => x.State).Returns(mockPersonState.Object);
    var login = Logins.Create();

    var employee = Employees.Create();
    employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
    employee.Person = person;
    employee.Login = login;
    employee.Email = "new@mail.ru";
    employee.BusinessUnit = businessUnit;

    var mockEmployeeState = new Mock<IEmployeeState>();
    mockEmployeeState.Setup(x => x.Properties.Status.OriginalValue).Returns(Sungero.CoreEntities.DatabookEntry.Status.Active);
    mockEmployeeState.Setup(x => x.Properties.Person.OriginalValue).Returns(person);
    mockEmployeeState.Setup(x => x.Properties.Login.OriginalValue).Returns(login);
    mockEmployeeState.Setup(x => x.Properties.BusinessUnit.OriginalValue).Returns(businessUnit);
    mockEmployeeState.Setup(x => x.Properties.Email.OriginalValue).Returns("origianl@mail.ru");

    Mock.Get(employee).Setup(x => x.State).Returns(() => mockEmployeeState.Object);
    return employee;
  }
}
