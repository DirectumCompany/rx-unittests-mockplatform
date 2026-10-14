using MockPlatform.Extensions;
using MockPlatform.Repositories;
using Sungero.Company;
using Sungero.Parties;

namespace MockPlatform.Tests;

public class Structures
{
  [Test]
  public void CreateStructure()
  {
    // Arrange
    var user = UserBases.Create();
    var person = People.Create();
    user.Person = person;
    person.LastName = "Иванов";
    person.FirstName = "Иван";
    person.MiddleName = "Иванович";
    person.Sex = Sungero.Parties.Person.Sex.Male;

    // Act
    var result = user.GetSharedFunctions<Sungero.Company.Shared.UserBaseFunctions>()
      .GetUserFullNameInDeclension(Sungero.Core.DeclensionCase.Nominative);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(result.LastName, Is.EqualTo("Иванов"));
      Assert.That(result.FirstName, Is.EqualTo("Иван"));
      Assert.That(result.MiddleName, Is.EqualTo("Иванович"));
    });
  }

  [Test]
  public void CreatePublicStructure()
  {
    // Arrange
    var absenceDto = Sungero.Company.Structures.Module.AbsenceDto.Create();
    absenceDto.Employee = Employees.Create();
    absenceDto.StartDate = DateTime.Now;
    absenceDto.EndDate = DateTime.Now.AddDays(1);
    absenceDto.IsWorkingDuringAbsence = true;
    absenceDto.Reason = AbsenceReasons.Create();
    absenceDto.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
    absenceDto.Note = "test note";
    absenceDto.NotifyList = [];

    // Act
    var result = Sungero.Company.PublicFunctions.Module.CreateAbsence(absenceDto);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(result.User, Is.EqualTo(absenceDto.Employee));
      Assert.That(result.StartDate, Is.EqualTo(absenceDto.StartDate));
      Assert.That(result.EndDate, Is.EqualTo(absenceDto.EndDate));
      Assert.That(result.UserIsWorking, Is.EqualTo(absenceDto.IsWorkingDuringAbsence));
      Assert.That(result.Reason, Is.EqualTo(absenceDto.Reason));
      Assert.That(result.Status, Is.EqualTo(absenceDto.Status));
      Assert.That(result.Note, Is.EqualTo(absenceDto.Note));
    });
  }
}
