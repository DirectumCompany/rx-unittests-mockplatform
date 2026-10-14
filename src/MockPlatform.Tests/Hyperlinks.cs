using MockPlatform.Repositories;
using Moq;
using Sungero.Company;
using Sungero.Domain.Shared;

namespace MockPlatform.Tests;

public class Hyperlinks
{
  /// <summary>
  /// Проверить работу метода Hyperlinks.Get для сотрудника.
  /// </summary>
  [Test]
  public void ShouldGetHyperlinkForEmployee()
  {
    // Создание тестового сотрудника
    var employee = Employees.Create();

    // Получение гиперссылки для сотрудника
    var hyperlink = Sungero.Core.Hyperlinks.Get(employee);

    // Проверка, что гиперссылка содержит информацию о сотруднике
    Assert.That(hyperlink, Is.EqualTo("http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=1"));
  }

  /// <summary>
  /// Проверить работу метода Hyperlinks.Get для информации о сотруднике.
  /// </summary>
  [Test]
  public void ShouldGetHyperlinkForEmployeeInfo()
  {
    // Создание тестового сотрудника
    var employee = Employees.Create();
    var infoMock = new Mock<IEntityInfo>();
    var internalInfoMock = infoMock.As<IInternalEntityInfo>();
    internalInfoMock.Setup(i => i.EntityMetadata.NameGuid)
      .Returns(((IExtendedEntity)employee).TypeGuid);

    // Получение гиперссылки для сотрудника
    var hyperlinkWithId = Sungero.Core.Hyperlinks.Get(infoMock.Object, employee.Id);
    var hyperlink = Sungero.Core.Hyperlinks.Get(infoMock.Object);

    // Проверка, что гиперссылка содержит информацию о сотруднике
    Assert.Multiple(() =>
    {
      Assert.That(hyperlinkWithId, Is.EqualTo("http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=1"));
      Assert.That(hyperlink, Is.EqualTo("http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000"));
    });
  }
}
