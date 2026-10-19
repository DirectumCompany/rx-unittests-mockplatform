using Sungero.Company;
using ModuleFunctions = Sungero.Company.Server.ModuleFunctions;

namespace UnitTests.Company.ModuleServerFunctions;

public class GetSummaryOfUsersDisconnections
{
  [Test]
  [Description("Нет пользователей для отключения — в отчете только заголовок и список приложений.")]
  public void GetSummaryOfUsersDisconnections_NoUsersToDisconnect_ReturnsAppsOnly()
  {
    // Arrange
    var externalApp = ExternalApps.Create();
    externalApp.Name = "Portal";

    // Act
    var functions = new ModuleFunctions();
    var result = functions.GetSummaryOfUsersDisconnections([], [], [externalApp]);

    // Assert
    var expected = """"
        Отчет об отключении пользователей от Directum Omni

        Приложения: Portal

        """";

    Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
  }

  [Test]
  [Description("Все пользователи отключены успешно — в отчете указано число отключенных.")]
  public void GetSummaryOfUsersDisconnections_UsersToDisconnectOnly_ReturnsDisconnectedCount()
  {
    // Arrange
    var externalApp = ExternalApps.Create();
    externalApp.Name = "Portal";

    var user1 = UserBases.Create();
    var user2 = UserBases.Create();

    // Act
    var functions = new ModuleFunctions();
    var result = functions.GetSummaryOfUsersDisconnections([user1, user2], [], [externalApp]);

    // Assert
    var expected = """"
        Отчет об отключении пользователей от Directum Omni

        Приложения: Portal
        Успешно отключено: 2

        """";

    Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
  }

  [Test]
  [Description("Есть только пользователи с ошибкой отключения — в отчете указано число ошибок и ссылки на этих пользователей.")]
  public void GetSummaryOfUsersDisconnections_UsersWithDisconnectionErrorOnly_ReturnsErrorUsersLinks()
  {
    // Arrange
    var externalApp = ExternalApps.Create();
    externalApp.Name = "Portal";

    var user = UserBases.Create();

    // Act
    var functions = new ModuleFunctions();
    var result = functions.GetSummaryOfUsersDisconnections([], [user], [externalApp]);

    // Assert
    var expected = """"
        Отчет об отключении пользователей от Directum Omni

        Приложения: Portal
        Ошибка отключения: 1

        Пользователи, при отключении которых произошла ошибка:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=1

        """";

    Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
  }

  [Test]
  [Description("Несколько внешних приложений — в отчете перечислены все приложения.")]
  public void GetSummaryOfUsersDisconnections_MultipleExternalApps_ReturnsAllAppNames()
  {
    // Arrange
    var externalApp1 = ExternalApps.Create();
    externalApp1.Name = "Portal";

    var externalApp2 = ExternalApps.Create();
    externalApp2.Name = "Directum";

    var user1 = UserBases.Create();
    var user2 = UserBases.Create();

    // Act
    var functions = new ModuleFunctions();
    var result = functions.GetSummaryOfUsersDisconnections([user1, user2], [], [externalApp1, externalApp2]);

    // Assert
    var expected = """"
        Отчет об отключении пользователей от Directum Omni

        Приложения: Portal, Directum
        Успешно отключено: 2

        """";
    Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
  }

  [Test]
  [Description("У пользователей разные результаты отключения — в отчете указано число по каждому результату и ссылки на пользователей с ошибкой.")]
  public void GetSummaryOfUsersDisconnections_MixedScenario_ReturnsCountsAndErrorUsersLinks()
  {
    // Arrange
    var externalApp1 = ExternalApps.Create();
    externalApp1.Name = "Portal";

    var externalApp2 = ExternalApps.Create();
    externalApp2.Name = "Directum";

    var user1 = UserBases.Create();
    var user2 = UserBases.Create();
    var user3 = UserBases.Create();

    // Act
    var functions = new ModuleFunctions();
    var result = functions.GetSummaryOfUsersDisconnections([user1, user2, user3], [user3], [externalApp1, externalApp2]);

    // Assert
    var expected = """"
        Отчет об отключении пользователей от Directum Omni

        Приложения: Portal, Directum
        Успешно отключено: 2
        Ошибка отключения: 1

        Пользователи, при отключении которых произошла ошибка:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=3

        """";

    Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
  }
}
