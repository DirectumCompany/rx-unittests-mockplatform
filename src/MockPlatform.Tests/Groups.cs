using MockPlatform.Core;
using MockPlatform.Repositories;
using Moq;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.CoreEntities.Shared;

namespace MockPlatform.Tests;

public class Groups
{
  [Test]
  public void GetAllUsersInGroup()
  {
    // Arrange
    var bUnit = BusinessUnits.Create();
    var user = UserBases.Create();

    var implementerMock = new Mock<IGroupRepositoryImplementer<IBusinessUnit>>();
    implementerMock.Setup(r => r.GetAllUsersInGroup(It.IsAny<IGroup>())).Returns([user]);
    using var mock = IMockable<IGroupRepositoryImplementer<IBusinessUnit>>.WithMock(implementerMock.Object);

    // Act
    var users = BusinessUnits.GetAllUsersInGroup(bUnit);

    // Assert
    Assert.That(users, Does.Contain(user));
  }

  [Test]
  public void GetAllUsersInGroupWithoutMock()
  {
    // Arrange
    var bUnit = BusinessUnits.Create();

    // Act
    var users = BusinessUnits.GetAllUsersInGroup(bUnit);

    // Assert
    Assert.That(users, Is.Empty);
  }

  [Test]
  public void GetAllUsersInGroupOutOfScope()
  {
    // Arrange
    var bUnit = BusinessUnits.Create();
    var user = UserBases.Create();

    var implementerMock = new Mock<IGroupRepositoryImplementer<IBusinessUnit>>();
    implementerMock.Setup(r => r.GetAllUsersInGroup(It.IsAny<IGroup>())).Returns([user]);
    using (var mock = IMockable<IGroupRepositoryImplementer<IBusinessUnit>>.WithMock(implementerMock.Object))
    {

    }

    // Act
    var users = BusinessUnits.GetAllUsersInGroup(bUnit);

    // Assert
    Assert.That(users, Is.Empty);
  }

  [Test]
  public void GetAllUsersInGroupInsideMethod()
  {
    // Arrange
    var employee = Employees.Create();
    var absence = Absences.Create();
    absence.User = employee;

    var department = Departments.Create();

    var implementerMock = new Mock<IGroupRepositoryImplementer<IDepartment>>();
    implementerMock.Setup(r => r.GetAllUsersInGroup(It.IsAny<IGroup>())).Returns([employee]);
    using var mock = IMockable<IGroupRepositoryImplementer<IDepartment>>.WithMock(implementerMock.Object);

    // Act
    var absences = new Sungero.Company.Server.ModuleFunctions()
      .FilterAbsencesByDepartmentWithSubdepartments(new IAbsence[] { absence }.AsQueryable(), department);

    // Assert
    Assert.That(absences, Does.Contain(absence));
  }
}
