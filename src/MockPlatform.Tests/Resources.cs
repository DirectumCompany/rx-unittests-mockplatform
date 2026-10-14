using Sungero.ExchangeCore;

namespace MockPlatform.Tests;

public class ResourcesRu
{
  [Test]
  public void GetModuleResources()
  {
    var resource = Sungero.Company.Resources.ReportAboutConnectedUsers.ToString();
    Assert.AreEqual("Отчет о подключении пользователей к Directum Omni\r\n\r\n", resource);
  }

  [Test]
  public void GetModuleResourcesWithFormat()
  {
    var resource = Sungero.Company.Resources.ExternalAppsFormat(543).ToString();
    Assert.AreEqual("Приложения: 543\r\n", resource);
  }

  [Test]
  public void GetModuleResourcesWithManyArgsFormat()
  {
    var resource = Sungero.Exchange.Resources.AssignDocumentVersionFormat("первый документ", 1, "диадок").ToString();
    Assert.AreEqual("Документ первый документ (версия 1) подписан контрагентом в сервисе обмена диадок.", resource);
  }

  [Test]
  public void GetEntityResource()
  {
    var resource = BusinessUnitBoxes.Resources.WrongPassword.ToString();
    Assert.AreEqual("Ошибка аутентификации. Проверьте правильность логина и пароля.", resource);
  }
}