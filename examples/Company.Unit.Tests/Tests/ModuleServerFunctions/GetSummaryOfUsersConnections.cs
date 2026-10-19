using MockPlatform.Extensions;
using Sungero.Company;

namespace UnitTests.Company.ModuleServerFunctions
{
  /// <summary>
  /// Юнит тесты для функции GetSummaryOfUsersConnections.
  /// Функция формирует сводку по подключениям пользователей к внешним приложениям.
  /// </summary>
  public class GetSummaryOfUsersConnections
  {
    [Test]
    [Description("Нет пользователей для подключения — в отчете только заголовок и список приложений.")]
    public void GetSummaryOfUsersConnections_NoUsersToConnect_ReturnsAppsOnly()
    {
      // Arrange
      // Создание внешнего приложения
      var externalApp = Sungero.Company.ExternalApps.Create();
      externalApp.SetId(1);
      externalApp.Name = "Portal";

      var usersToConnect = new List<IUserBase>();
      var usersWithConnectionError = new List<IUserBase>();
      var usersWithoutRequiredData = new List<IUserBase>();
      var externalApps = new List<IExternalApp> { externalApp };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal

        """";

      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }

    [Test]
    [Description("Все пользователи подключены успешно — в отчете указано число подключенных.")]
    public void GetSummaryOfUsersConnections_UsersToConnectOnly_ReturnsConnectedCount()
    {
      // Arrange
      // Создание внешнего приложения
      var externalApp = Sungero.Company.ExternalApps.Create();
      externalApp.SetId(1);
      externalApp.Name = "Portal";

      // Создание пользователей для подключения
      var user1 = Sungero.Company.UserBases.Create();
      user1.SetId(1);
      user1.Name = "Иванов Иван Иванович";

      var user2 = Sungero.Company.UserBases.Create();
      user2.SetId(2);
      user2.Name = "Петров Петр Петрович";

      var usersToConnect = new List<IUserBase> { user1, user2 };
      var usersWithConnectionError = new List<IUserBase>();
      var usersWithoutRequiredData = new List<IUserBase>();
      var externalApps = new List<IExternalApp> { externalApp };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal
        Успешно подключено: 2

        """";

      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }

    [Test]
    [Description("Есть только пользователи с ошибкой подключения — в отчете указано число ошибок и ссылки на этих пользователей.")]
    public void GetSummaryOfUsersConnections_UsersWithConnectionErrorOnly_ReturnsErrorUsersLinks()
    {
      // Arrange
      // Создание внешнего приложения
      var externalApp = Sungero.Company.ExternalApps.Create();
      externalApp.SetId(1);
      externalApp.Name = "Portal";

      // Создание пользователей с ошибками подключения
      var user1 = Sungero.Company.UserBases.Create();
      user1.SetId(1);
      user1.Name = "Иванов Иван Иванович";

      var usersToConnect = new List<IUserBase>();
      var usersWithConnectionError = new List<IUserBase> { user1 };
      var usersWithoutRequiredData = new List<IUserBase>();
      var externalApps = new List<IExternalApp> { externalApp };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal
        Не подключено: 1
        Ошибка подключения: 1

        Пользователи, при подключении которых произошла ошибка:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=1

        """";

      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }

    [Test]
    [Description("Есть только пользователи без требуемых данных — в отчете указаны ссылки на этих пользователей.")]
    public void GetSummaryOfUsersConnections_UsersWithoutRequiredDataOnly_ReturnsUsersWithoutDataLinks()
    {
      // Arrange
      // Создание внешнего приложения
      var externalApp = Sungero.Company.ExternalApps.Create();
      externalApp.SetId(1);
      externalApp.Name = "Portal";

      // Создание пользователей без требуемых данных
      var user1 = Sungero.Company.UserBases.Create();
      user1.SetId(1);
      user1.Name = "Иванов Иван Иванович";

      var usersToConnect = new List<IUserBase>();
      var usersWithConnectionError = new List<IUserBase>();
      var usersWithoutRequiredData = new List<IUserBase> { user1 };
      var externalApps = new List<IExternalApp> { externalApp };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal
        Не подключено: 1

        Пользователи, для подключения которых не хватило данных:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=1

        """";

      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }

    [Test]
    [Description("Несколько внешних приложений — в отчете перечислены все приложения.")]
    public void GetSummaryOfUsersConnections_MultipleExternalApps_ReturnsAllAppNames()
    {
      // Arrange
      // Создание нескольких внешних приложений
      var externalApp1 = Sungero.Company.ExternalApps.Create();
      externalApp1.SetId(1);
      externalApp1.Name = "Portal";

      var externalApp2 = Sungero.Company.ExternalApps.Create();
      externalApp2.SetId(2);
      externalApp2.Name = "Directum";

      var externalApp3 = Sungero.Company.ExternalApps.Create();
      externalApp3.SetId(3);
      externalApp3.Name = "Omni";

      // Создание пользователей для подключения
      var user1 = Sungero.Company.UserBases.Create();
      user1.SetId(1);
      user1.Name = "Иванов Иван Иванович";

      var user2 = Sungero.Company.UserBases.Create();
      user2.SetId(2);
      user2.Name = "Петров Петр Петрович";

      var usersToConnect = new List<IUserBase> { user1, user2 };
      var usersWithConnectionError = new List<IUserBase>();
      var usersWithoutRequiredData = new List<IUserBase>();
      var externalApps = new List<IExternalApp> { externalApp1, externalApp2, externalApp3 };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal, Directum, Omni
        Успешно подключено: 2

        """";
      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }

    [Test]
    [Description("У пользователей разные результаты подключения — в отчете указано число по каждому результату и ссылки на неподключенных пользователей.")]
    public void GetSummaryOfUsersConnections_MixedScenario_ReturnsCountsAndNotConnectedUsersLinks()
    {
      // Arrange
      // Создание внешних приложений
      var externalApp1 = Sungero.Company.ExternalApps.Create();
      externalApp1.SetId(1);
      externalApp1.Name = "Portal";

      var externalApp2 = Sungero.Company.ExternalApps.Create();
      externalApp2.SetId(2);
      externalApp2.Name = "Directum";

      // Создание пользователей
      var user1 = Sungero.Company.UserBases.Create();
      user1.SetId(1);
      user1.Name = "Иванов Иван Иванович";

      var user2 = Sungero.Company.UserBases.Create();
      user2.SetId(2);
      user2.Name = "Петров Петр Петрович";

      var user3 = Sungero.Company.UserBases.Create();
      user3.SetId(3);
      user3.Name = "Сидоров Сидор Сидорович";

      var user4 = Sungero.Company.UserBases.Create();
      user4.SetId(4);
      user4.Name = "Кузнецов Кузьма Кузьмич";

      // usersToConnect: 4 пользователя
      var usersToConnect = new List<IUserBase> { user1, user2, user3, user4 };
      // usersWithConnectionError: 1 пользователь (user2) - ошибка подключения
      var usersWithConnectionError = new List<IUserBase> { user2 };
      // usersWithoutRequiredData: 1 пользователь (user4) - нет данных
      var usersWithoutRequiredData = new List<IUserBase> { user4 };
      var externalApps = new List<IExternalApp> { externalApp1, externalApp2 };

      // Act
      var functions = new Sungero.Company.Server.ModuleFunctions();
      var result = functions.GetSummaryOfUsersConnections(usersToConnect, usersWithConnectionError, usersWithoutRequiredData, externalApps);

      // Assert
      string expected = """"
        Отчет о подключении пользователей к Directum Omni

        Приложения: Portal, Directum
        Успешно подключено: 2
        Не подключено: 2
        Ошибка подключения: 1

        Пользователи, для подключения которых не хватило данных:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=4

        Пользователи, при подключении которых произошла ошибка:
        http://mock.hyperlinks/Sungero?type=00000000-0000-0000-0000-000000000000&id=2

        """";

      Assert.That(result.ReplaceLineEndings(), Is.EqualTo(expected.ReplaceLineEndings()));
    }
  }
}
